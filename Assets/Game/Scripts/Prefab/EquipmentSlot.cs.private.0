using LostKingdom.Content;
using LostKingdom.ID;
using LostKingdom.Manager;
using LostKingdom.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace LostKingdom.Prefab
{
    public class EquipmentSlot : LKBehaviour
    {
        [SerializeField]
        private Image _obj_background;
        [SerializeField]
        private Image _obj_icon;

        internal int slot;
        internal Equipment equipment;

        protected override object HoverItem => this.equipment;

        public void SetEquipment(Equipment equipment)
        {
            this.equipment = equipment;
            if (equipment == null)
            {
                this._obj_background.SetActive(false);
                this._obj_icon.SetActive(false);
            }
            else if (this.CheckSlot(equipment.Slot))
            {
                this._obj_background.color = ColorID.GetColor(this.equipment.Quality.ToString());
                this._obj_icon.sprite = AssetManager.GetItem(equipment.Type);
                this._obj_background.SetActive(true);
                this._obj_icon.SetActive(true);
            }
        }

        private bool CheckSlot(Equipment.SlotType slot)
        {
            switch (slot) {
                case Equipment.SlotType.Head:
                    return this.slot == (int)Equipment.SlotType.Head;
                case Equipment.SlotType.Body:
                    return this.slot == (int)Equipment.SlotType.Body;
                case Equipment.SlotType.Legs:
                    return this.slot == (int)Equipment.SlotType.Legs;
                case Equipment.SlotType.LeftHand:
                    return this.slot == (int)Equipment.SlotType.LeftHand;
                case Equipment.SlotType.RightHand:
                    return this.slot == (int)Equipment.SlotType.RightHand;
                case Equipment.SlotType.LeftAccessory:
                    return this.slot == (int)Equipment.SlotType.LeftAccessory;
                case Equipment.SlotType.RightAccessory:
                    return this.slot == (int)Equipment.SlotType.RightAccessory;
                case Equipment.SlotType.Hands:
                    return this.slot == (int)Equipment.SlotType.LeftHand || this.slot == (int)Equipment.SlotType.RightHand;
                case Equipment.SlotType.Accessory:
                    return this.slot == (int)Equipment.SlotType.LeftAccessory || this.slot == (int)Equipment.SlotType.RightAccessory;
            }
            return false;
        }

        protected override bool OnShowHoverText()
        {
            UIManager.Game_HoverText.Equipment = this.equipment;
            return base.OnShowHoverText();
        }
    }
}
