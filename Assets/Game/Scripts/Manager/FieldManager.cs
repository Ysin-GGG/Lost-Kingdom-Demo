using AmplifyShaderEditor;
using LostKingdom.Components;
using LostKingdom.Content;
using LostKingdom.DataStructure;
using LostKingdom.Prefab;
using LostKingdom.IO;
using LostKingdom.Utility;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System;
using System.Runtime.InteropServices.ComTypes;

namespace LostKingdom.Manager
{
    [DisallowMultipleComponent]
    public sealed class FieldManager : LKBehaviour
    {
        [SerializeField]
        private GameObject outline;
        [SerializeField]
        private GameObject objects;
        [SerializeField]
        private List<GameObject> fieldPrefabs;
        [SerializeField]
        private Tilemap ground;
        [SerializeField]
        private Tilemap water;
        [SerializeField]
        private Tilemap surface;
        [SerializeField]
        private Tilemap barrier;
        private List<SpriteRenderer> shadows;
        private static Field field;

        private static FieldManager instance;
        private static ObjectPool<ObjectMemory> memories;

        public static Field Field
        {
            get => FieldManager.field;
            set
            {
                if ((FieldManager.field = value) != null)
                {
                    FieldManager.memories.Clear();
                    foreach (FieldObject obj in FieldManager.field.Initialization(FieldManager.instance.ground, FieldManager.instance.water, FieldManager.instance.surface))
                    {
                        ObjectMemory dest = FieldManager.memories.Request();
                        obj.LoadData(dest);
                        dest.Active = true;
                    }
                    FieldManager.instance.barrier.SetActive(false);
                }
            }
        }

        protected override void Awake()
        {
            FieldManager.instance = this;
            Transform transform =  FieldManager.instance.transform;
            this.ground = transform.Find("GroundLayer").GetComponent<Tilemap>();
            this.water = transform.Find("WaterLayer").GetComponent<Tilemap>();
            this.surface = transform.Find("SurfaceLayer").GetComponent<Tilemap>();
            this.barrier = transform.Find("BarrierLayer").GetComponent<Tilemap>();
            this.ground.SetActive(true);
            this.water.SetActive(true);
            this.surface.SetActive(true);
            this.barrier.SetActive(true);
            if (this.outline != null)
            {
                this.shadows = new List<SpriteRenderer>();
                foreach (SpriteRenderer shadow in this.outline.GetComponentsInChildren<SpriteRenderer>())
                    this.shadows.Add(shadow);
            }

            FieldManager.memories = new ObjectPool<ObjectMemory>((i) =>
            {
                ObjectMemory renderer = new GameObject("Renderer ", typeof(SpriteRenderer)).AddComponent<ObjectMemory>();
                renderer.Parent = this.objects.gameObject;
                renderer.name += i + 1;
                renderer.Active = false;
                return renderer;
            }, 1 << 10, "Field: Renderers");
            FieldManager.memories.OnRelease += (i, o) => o.Active = false;
        }

        protected override void Start()
        {
            if (this.objects != null)
            {
                foreach (GameObject field in this.fieldPrefabs)
                {
                    field.SetActive(true);
                    FieldManager.GenerateFieldData(field, field.name, @".\Assets\Game\Fields\");
                }
                FieldManager.Field = new Field(@".\Assets\Game\Fields\HuntersHouseField.bin");
            }
        }

        protected override void FixedUpdate()
        {
            foreach (ObjectMemory obj in FieldManager.memories)
            {
                // if "obj" not in "camera": obj.Active = false;
            }
        }

        public static void SetOutline(SpriteRenderer target)
        {
            if (target == null)
            {
                FieldManager.instance.outline.SetActive(false);
                FieldManager.instance.outline.SetParent(FieldManager.instance.transform);
            }
            else
            {
                FieldManager.instance.outline.SetActive(true);
                FieldManager.instance.outline.SetParent(target.transform);
                FieldManager.instance.outline.transform.localPosition = Vector2.zero;
                foreach (SpriteRenderer shadow in FieldManager.instance.shadows)
                {
                    shadow.flipX = target.flipX;
                    shadow.flipY = target.flipY;
                    shadow.sprite = target.sprite;
                    shadow.sortingOrder = target.sortingOrder - 1;
                    shadow.sortingLayerID = target.sortingLayerID;
                }
            }
        }
        public static bool IsBarrier(Vector2Int position) => FieldManager.field?.IsBarrier(position) ?? false;

#if UNITY_EDITOR
        internal static Field GenerateFieldData(GameObject clone, string name, string path = @".\Assets\Game\Fields\")
        {
            HashSet<FieldObject> objs = new HashSet<FieldObject>();
            if (clone != null)
            {
                ObjectMemory[] objects = clone.GetComponentsInChildren<ObjectMemory>();
                foreach (ObjectMemory obj in objects)
                {
                    FieldObject fo = new FieldObject();
                    fo.SaveData(obj);
                    objs.Add(fo);
                }
                clone.SetActive(false);
            }
            Tilemap[] tilemaps =
            {
                clone.transform.Find("GroundLayer").GetComponent<Tilemap>(),
                clone.transform.Find("WaterLayer").GetComponent<Tilemap>(),
                clone.transform.Find("SurfaceLayer").GetComponent<Tilemap>(),
                clone.transform.Find("BarrierLayer").GetComponent<Tilemap>(),
            };
            return new Field(name, path, objs, tilemaps[0], tilemaps[1], tilemaps[2], tilemaps[3]);
        }
#endif
    }
}