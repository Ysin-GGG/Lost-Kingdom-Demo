using LostKingdom.Content;
using LostKingdom.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace LostKingdom.Components
{
    public class InventoryItem : LKBehaviour
    {
        [SerializeField]
        private Image _obj_img;

        public int Type { get => this.item.Type; set => this.item.SetDefaults(value); }

        public Vector2Int Location { get => this.location; set => this.SetLocation(value); }

        private Vector2Int location;
        public Item item;

        protected override void Awake()
        {
            this.TryGetComponent<Image>(out this._obj_img);
        }

        public void SetItem(int type)
        {
            this.item.SetDefaults(type);
            this._obj_img.sprite = AssetManager.GetItem(this.item.Type);
            this._obj_img.SetNativeSize();
        }

        public void SetItem(Item item)
        { 
            this.item = item;
            this._obj_img.sprite = AssetManager.GetItem(this.item.Type);
            this._obj_img.SetNativeSize();
        }

        private void SetLocation(Vector2Int location)
        {
            this.location = location;
            this.LocalPosition = location * GridManager.GridSize + GridManager.GridSize * new Vector2(this.item.InventoryWidth, this.item.InventoryHeight) / 2;
        }
    }
}
