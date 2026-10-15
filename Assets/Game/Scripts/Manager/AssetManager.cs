using LostKingdom.Content;
using LostKingdom.ID;
using LostKingdom.Utility;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
namespace LostKingdom.Manager
{
    [DisallowMultipleComponent]
    public class AssetManager : LKBehaviour
    {
        [SerializeField]
        private Sprite[] _img_symbols;
        [SerializeField]
        private Sprite[] _img_items;
        [SerializeField]
        private Sprite[] _img_tags_range;
        [SerializeField]
        private Sprite[] _img_tags_damage;
        [SerializeField]
        private Sprite[] _img_tags_element;
        [SerializeField]
        private Sprite[] _img_appoints;
        [SerializeField]
        private Sprite[] _img_enemies_stand;
        [SerializeField]
        private Sprite[] _img_characters_stand;

        private static Dictionary<int, Sprite> spriteDict;
        private static Dictionary<Sprite, int> spriteIdDict;
        private static Sprite[] sprites;

        private static Dictionary<int, Material> materialDict;
        private static Dictionary<Material, int> materialIdDict;
        private static Material[] materials;

        private static Dictionary<int, TileBase> tileDict;
        private static Dictionary<TileBase, int> tileIdDict;
        private static TileBase[] tiles;

        private static AssetManager instance;

        public static Sprite[] Symbols => AssetManager.instance._img_symbols;
        public static Sprite[] Items => AssetManager.instance._img_items;
        public static Sprite[] RangeTags => AssetManager.instance._img_tags_range;
        public static Sprite[] DamageTags => AssetManager.instance._img_tags_damage;
        public static Sprite[] ElementTags => AssetManager.instance._img_tags_element;
        public static Sprite[] APPoints => AssetManager.instance._img_appoints;
        public static Sprite[] EnemyStand => AssetManager.instance._img_enemies_stand;
        public static Sprite[] CharacterStand => AssetManager.instance._img_characters_stand;
        public static Sprite[] Sprites => AssetManager.sprites;
        public static Material[] Materials => AssetManager.materials;
        public static TileBase[] Tiles => AssetManager.tiles;

        protected override void Awake()
        {
            AssetManager.instance = this;
#if UNITY_EDITOR
            AssetManager.sprites = AssetManager.FindAssets<Sprite>(out AssetManager.spriteDict, out AssetManager.spriteIdDict, @"Assets\Game\Images\");
            AssetManager.materials = AssetManager.FindAssets<Material>(out AssetManager.materialDict, out AssetManager.materialIdDict, null);
            AssetManager.tiles = AssetManager.FindAssets<TileBase>(out AssetManager.tileDict, out AssetManager.tileIdDict, @"Assets\Game\");
#endif
        }

        public static Sprite GetSymbol(int type) => AssetManager.instance._img_symbols[type > SymbolID.Count ? 0 : type.Max(0)];
        public static Sprite GetSymbol(Symbol symbol) => AssetManager.instance._img_symbols[symbol.Type > SymbolID.Count ? 0 : symbol.Type.Max(0)];
        public static Sprite GetTag(RangeID.Number tagID) => AssetManager.instance._img_tags_range[(int)tagID < RangeID.Count ? ((int)tagID).Max(0) : 0];
        public static Sprite GetTag(DamageID.Number tagID) => AssetManager.instance._img_tags_damage[(int)tagID < DamageID.Count ? ((int)tagID).Max(0) : 0];
        public static Sprite GetTag(ElementID.Number tagID) => AssetManager.instance._img_tags_element[(int)tagID < ElementID.Count ? ((int)tagID).Max(0) : 0];
        public static Sprite GetItem(int type) => AssetManager.instance._img_items[type > ItemID.Count ? 0 : type.Max(0)];
        public static Sprite GetItem(Equipment equipment) => AssetManager.instance._img_items[equipment.Type > ItemID.Count ? 0 : equipment.Type.Max(0)];

        public static Sprite GetSprite(int instanceID) => AssetManager.spriteDict.GetValueOrDefault(instanceID, null);
        public static Material GetMaterial(int instanceID) => AssetManager.materialDict.GetValueOrDefault(instanceID, null);
        public static TileBase GetTileBase(int instanceID) => AssetManager.tileDict.GetValueOrDefault(instanceID, null);
        public static int GetSpriteID(Sprite sprite) => sprite == null ? -1 : AssetManager.spriteIdDict.GetValueOrDefault(sprite, -1);
        public static int GetMaterialID(Material material) => material == null ? -1 : AssetManager.materialIdDict.GetValueOrDefault(material, -1);
        public static int GetTileBaseID(TileBase tile) => tile == null ? -1 : AssetManager.tileIdDict.GetValueOrDefault(tile, -1);

        public static int GetObjectID<T>(T obj) where T : Object
        {
            if (obj is Sprite sprite) return AssetManager.GetSpriteID(sprite);
            if (obj is Material material) return AssetManager.GetMaterialID(material);
            if (obj is TileBase tile) return AssetManager.GetTileBaseID(tile);
            return -1;
        }

        private static T[] FindAssets<T>(out Dictionary<int, T> dict1, out Dictionary<T, int> dict2, string path = @"Assets\Game\") where T : Object
        {
            dict1 = new Dictionary<int, T>();
            dict2 = new Dictionary<T, int>();
            List<T> list = new List<T>();
            string filter = "t:" + typeof(T).Name;
            string[] guids = { path != null && path.EndsWith('\\') ? path : path + '\\' };
            try { guids = AssetDatabase.FindAssets(filter, guids); }
            catch { guids = AssetDatabase.FindAssets(filter); }
            foreach (string guid in guids)
                foreach (T obj in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(guid)).OfType<T>().ToArray())
                    if (obj != null)
                    {
                        int id = obj.GetInstanceID();
                        dict1.Add(id, obj);
                        dict2.Add(obj, id);
                        list.Add(obj);
                    }
            return list.ToArray();
        }
    }
}
