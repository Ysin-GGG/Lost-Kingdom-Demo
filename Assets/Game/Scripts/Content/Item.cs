using LostKingdom.ID;
using LostKingdom.DataStructure;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using LostKingdom.Interface;
using LostKingdom.Manager;
using static LostKingdom.Content.Equipment;
using LostKingdom.Utility;
using UnityEngine.UIElements;

namespace LostKingdom.Content
{
    public class Item : IInventoryItem
    {
        /// <summary> 物品名 </summary>
        public string DisplayName { get; protected set; }
        /// <summary> 物品描述 </summary>
        public string Description { get; protected set; }
        /// <summary> 消耗类物品 </summary>
        public bool Consumable { get; protected set; }
        /// <summary> 物品堆叠上限 </summary>
        public int MaxStack { get => this.stack.ValueMax; }
        /// <summary> 物品堆叠数量 </summary>
        public int Stack { get => this.stack.Value; set => this.stack.Value = value; }
        /// <summary> 售出价 </summary>
        public int SalePrice { get; set; }
        /// <summary> 购入价 </summary>
        public int PurchasePrice { get; set; }
        /// <summary> 物品编号 </summary>
        public int Type { get; private set; }
        public int InventoryWidth { get; protected set; }
        public int InventoryHeight { get; protected set; }
        public int InventoryRotation { get; set; }
        public int InventoryLock { get; set; }

        private GameValue stack;

        public Item(int type)
        {
            this.stack = new GameValue(1, 1, 1, null, GameValue.UpdateType.KeepValue);
            this.SetDefaults(this.Type);
        }

        public bool SetDefaults(int type)
        {
            this.DisplayName = null;
            this.Description = null;
            this.Consumable = false;
            this.InventoryWidth = 1;
            this.InventoryHeight = 1;
            this.SalePrice = -1;
            this.PurchasePrice = -1;
            switch (this.Type = type)
            {
                case ItemID.None:
                    break;
                case ItemID.ArtonsSapphire:
                    this.SalePrice = 60;
                    break;
                case ItemID.ArchaicCloak:
                    this.SalePrice = 40;
                    this.PurchasePrice = 300;
                    break;
                case ItemID.WoodenStaff:
                    this.SalePrice = 20;
                    this.PurchasePrice = 120;
                    break;
            }
            if (this.SetDefaults())
            {
                bool flag1 = this.SalePrice < 0;
                bool flag2 = this.PurchasePrice < 0;
                if (flag1 && flag2) this.SalePrice = this.PurchasePrice = 0;
                else if (flag1 && !flag2) this.SalePrice = this.PurchasePrice / 5;
                else if (!flag1 && flag2) this.PurchasePrice = this.SalePrice * 5;
                this.DisplayName = LKLocalization.GetText("Item.Name." + ((ItemID.Number)this.Type).ToString());
                this.Description = LKLocalization.GetText("Item.Description." + ((ItemID.Number)this.Type).ToString());
            }
            return false;
        }

        public virtual bool SetDefaults() => this.Type >= 0;
    }
}