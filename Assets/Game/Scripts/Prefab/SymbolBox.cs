using LostKingdom.Content;
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
    public class SymbolBox : LKBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Image _obj_icon;
        [SerializeField]
        private Text _obj_cost;

        internal int slot;
        internal Symbol symbol;

        private static LKColor[] quality = { "#F9C389", "#C0C0C0", "#F7EC5F", "#C2FCFC" };

        public void SetSymbol(Symbol symbol)
        {
            this.symbol = symbol;
            if (symbol == null)
            {
                this.symbol = null;
                this.Active = false;
            }
            else
            {
                this.symbol = symbol;
                this._obj_icon.sprite = AssetManager.GetSymbol(symbol.Type);
                this._obj_cost.text = this.symbol.Cost.ToString();
                this.Active = true;
            }
        }

        protected override object HoverItem => this.symbol;

        protected override bool OnShowHoverText()
        {
            UIManager.Game_HoverText.Symbol = this.symbol;
            return base.OnShowHoverText();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UIManager.Game_CharacterInfo.SelectSymbol(this.symbol);
        }
    }
}
