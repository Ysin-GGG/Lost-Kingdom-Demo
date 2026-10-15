using LostKingdom.ID;
using LostKingdom.Interface;
using LostKingdom.Manager;
using LostKingdom.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LostKingdom.Content
{
    public class Equipment : Item
    {
        /// <summary> 装备工具提示 </summary>
        public string DisplayTooltip { get; protected set; }
        /// <summary> 装备类型 </summary>
        public SlotType Slot { get; protected set; }
        /// <summary> 装备品质 </summary>
        public QualityType Quality { get; protected set; }
        /// <summary> 装备攻击力加成 </summary>
        public int Attack { get; protected set; }
        /// <summary> 装备防御力加成 </summary>
        public int Defence { get; protected set; }
        /// <summary> 装备生命力加成 </summary>
        public int Life { get; protected set; }
        /// <summary> 装备行动力加成 </summary>
        public int Cost { get; protected set; }
        /// <summary> 装备追加技能 </summary>
        public Symbol Symbol { get; protected set; }

        /// <summary> 适用部位 </summary>
        public enum SlotType
        {
            /// <summary> 头部装备 </summary>
            Head = 0,
            /// <summary> 躯干装备 </summary>
            Body = 1,
            /// <summary> 腿部装备 </summary>
            Legs = 2,
            /// <summary> 左手装备 </summary>
            LeftHand = 3,
            /// <summary> 右手装备 </summary>
            RightHand = 4,
            /// <summary> 左部配饰 </summary>
            LeftAccessory = 5,
            /// <summary> 右部配饰 </summary>
            RightAccessory = 6,
            /// <summary> 手持装备 </summary>
            Hands = 7,
            /// <summary> 通用配饰 </summary>
            Accessory = 8,
        }

        /// <summary> 装备品质 </summary>
        public enum QualityType
        {
            /// <summary> 普通 </summary>
            Accepted = 0,
            /// <summary> 精制 </summary>
            Refined = 1,
            /// <summary> 附魔 </summary>
            Enchanted = 2,
            /// <summary> 失传 </summary>
            Isolated = 3,
        }

        public Equipment(int type) : base(type)
        {
            this.SetDefaults(type);
        }

        public override bool SetDefaults()
        {
            this.DisplayName = null;
            this.DisplayTooltip = null;
            this.Description = null;
            this.Slot = SlotType.Accessory;
            this.Quality = QualityType.Accepted;
            this.Attack = 0;
            this.Defence = 0;
            this.Life = 0;
            this.Cost = 0;
            this.Symbol = null;
            switch (this.Type)
            {
                case EquipmentID.None:
                    this.DisplayName = string.Empty;
                    this.DisplayTooltip = string.Empty;
                    this.Description = string.Empty;
                    return true;
                case EquipmentID.ArtonsSapphire:
                    this.DisplayTooltip = "-Unlimited arcanum-";
                    this.Quality = QualityType.Refined;
                    this.Slot = SlotType.Accessory;
                    this.Attack = 2;
                    this.Defence = 2;
                    this.Life = 4;
                    this.Cost = 1;
                    this.InventoryHeight = 2;
                    return true;
                case EquipmentID.ArchaicCloak:
                    this.Quality = QualityType.Accepted;
                    this.Slot = SlotType.Accessory;
                    this.Defence = 5;
                    this.Life = 2;
                    this.Symbol = new Symbol(SymbolID.ArcaneSurge);
                    this.InventoryHeight = 2;
                    return true;
                case EquipmentID.WoodenStaff:
                    this.Quality = QualityType.Accepted;
                    this.Slot = SlotType.Hands;
                    this.Attack = 8;
                    this.InventoryWidth = 2;
                    this.InventoryHeight = 3;
                    return true;
            }
            return false;
        }

        public virtual void ModifyTooltip(List<(string, int)> tooltips)
        {
            if (this.Type == 3) tooltips.Clear();
            tooltips.Add(("test", 2));
        }
    }
}