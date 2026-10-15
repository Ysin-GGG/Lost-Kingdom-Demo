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
    [DisallowMultipleComponent]
    public class SymbolIcon : LKBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Image _obj_icon;

        [SerializeField]
        private bool canClick = true;

        internal Symbol symbol;

        public bool CanClick { get => this.canClick; set => this.canClick = value; }

        protected override object HoverItem => this.symbol;

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
                this.Active = true;
            }
        }


        protected override bool OnShowHoverText()
        {
            UIManager.Game_HoverText.Symbol = this.symbol;
            return base.OnShowHoverText();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (this.CanClick)
                if (GameManager.SelectedSymbol == this.symbol)
                {
                    GameManager.SelectedSymbol = null;
                    GridManager.Range = Lattice.Empty;
                    GridManager.Area = Lattice.Empty;
                }
                else
                {

                    GameManager.SelectedSymbol = this.symbol;
                    GridManager.Range = this.symbol.EffectiveRange;
                    GridManager.Area = this.symbol.EffectiveArea;
                }
        }
    }
}
