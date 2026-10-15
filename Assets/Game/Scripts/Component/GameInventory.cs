using LostKingdom.Content;
using LostKingdom.Manager;
using LostKingdom.Utility;
using LostKingdom.ID;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Unity.VisualScripting.ReorderableList;
using System.Collections.Generic;
using LostKingdom.Prefab;

namespace LostKingdom.Components
{
    [RequireComponent(typeof(Image))]
    public class GameInventory : LKBehaviour
    {
        private List<InventoryItem> datas;
        private Inventory<Item> inventory;
        private InventoryItem selectItem;
        private Image image;

        protected override object HoverItem => this.image;

        protected override void Awake()
        {
            this.inventory = new Inventory<Item>(8, 6);
            if (this.TryGetComponent(out this.image))
            {
                this.rectTransform = this.image.rectTransform;
                this.rectTransform.SetAnchor(Vector4.zero, Vector2.zero, false);
                this.rectTransform.sizeDelta = this.inventory.Size * GridManager.GridSize;
            }
        }

        public void AddItem(Item item, Vector2Int location)
        {
            if (this.inventory.AddItem(item, location))
            {
                int idx = -1;
                if (this.datas.Count < this.inventory.Count)
                {
                    idx = this.datas.Count;
                    GameObject obj = new GameObject("Item - " + this.inventory.Count, typeof(Image));
                    obj.SetParent(this);
                    this.datas.Add(obj.AddComponent<InventoryItem>());
                }
                else
                    while (++idx < this.datas.Count)
                        if (!this.datas[idx].Active ?? false)
                            break;
                this.datas[idx].Active = true;
                this.datas[idx].SetItem(item);
                this.datas[idx].Location = location;
            }
        }

        public override void OnPointerEnter(PointerEventData eventData) => this.OnPointerMove(eventData);

        public override void OnPointerMove(PointerEventData eventData)
        {
            Vector2Int location = (eventData.position - this.WorldPosition).Floor() / GridManager.GridSize;
            if (this.inventory.FindItem(this.inventory.GetItem(location), out location))
                this.selectItem = datas.Find((data) => data.Location == location);
            else this.selectItem = null;
            if (this.selectItem == null)
                UIManager.Game_GridInventory.SetHighLight(this, eventData.position);
            else
                UIManager.Game_GridInventory.SetHighLight(this.selectItem);
        }

        public override void OnPointerExit(PointerEventData eventData) => UIManager.Game_GridInventory.SetHighLight(null);
    }
}
