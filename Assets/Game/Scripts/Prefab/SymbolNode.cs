using LostKingdom.Content;
using LostKingdom.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace LostKingdom.Prefab
{
    public class SymbolNode : LKBehaviour
    {
        public string DisplayName => this.symbol.DisplayName;
        public int SymbolCost => this.symbol == null ? -1 : this.symbol.Cost;

        [SerializeField]
        private Text _obj_name;
        [SerializeField]
        private Text _obj_cost;

        internal Symbol symbol;

        protected override void Start()
        {

        }

        protected override bool OnActivate()
        {
            return true;
        }
        protected override bool OnDeactivate()
        {
            return true;
        }

        public void SetSymbol(Symbol symbol)
        {
            this.symbol = symbol;
            if (this.symbol == null)
            {
                this._obj_name.text = null;
                this._obj_cost.text = null;
                this.Active = false;
            }
            else
            {
                this._obj_name.text = this.symbol.DisplayName;
                this._obj_cost.text = this.symbol.Cost.ToString();
                this.Active = true;
            }
        }
    }
}
