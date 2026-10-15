using LostKingdom.Components;
using LostKingdom.Manager;
using LostKingdom.Prefab;
using LostKingdom.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace LostKingdom.UI
{
    public class GridInventory : LKBehaviour
    {
        [SerializeField]
        private Image highlight;
        private Image image;
        public GameInventory other;
        public GameInventory player;

        protected override object HoverItem => this.image;

        protected override void Awake()
        {
            this.TryGetComponent(out this.image);
            this.highlight.transform.parent = this.transform;
            this.highlight.rectTransform.SetAnchor(Vector4.zero, Vector2.zero, false);
            this.highlight.SetNativeSize();
            this.highlight.SetActive(false);
        }

        public void SetHighLight(GameInventory inventory, Vector2 position)
        {
            if (inventory == null)
            {
                this.highlight.transform.parent = this.transform;
                this.highlight.SetActive(false);
            }
            else
            {
                Vector2 location = (position - inventory.WorldPosition) / GridManager.GridSize;
                Vector2 size = Vector2.one / 2;
                Vector2 edge = inventory.rectTransform.sizeDelta / GridManager.GridSize - size;
                location = (location.Clamp(Vector2.zero, edge).Floor() - size.Floor()) * GridManager.GridSize;
                this.highlight.rectTransform.localPosition = location;
                this.highlight.rectTransform.sizeDelta = 2 * size * GridManager.GridSize;
                this.highlight.transform.parent = inventory.transform;
                this.highlight.transform.SetAsFirstSibling();
                this.highlight.SetActive(true);
            }
        }

        public void SetHighLight(InventoryItem itemData)
        {
            if (itemData == null)
            {
                this.highlight.transform.parent = this.transform;
                this.highlight.SetActive(false);
            }
            else
            {
                this.highlight.rectTransform.position = (Vector2)itemData.Location * GridManager.GridSize;
                this.highlight.rectTransform.sizeDelta = new Vector2(itemData.item.InventoryWidth, itemData.item.InventoryHeight) * GridManager.GridSize;
                this.highlight.transform.parent = itemData.Parent.transform;
                this.highlight.transform.SetAsFirstSibling();
                this.highlight.SetActive(true);
            }
        }
    }
}
