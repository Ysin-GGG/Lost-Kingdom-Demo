using LostKingdom.Content;
using LostKingdom.Manager;
using LostKingdom.Prefab;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LostKingdom.UI
{
    [DisallowMultipleComponent]
    public sealed class SymbolCDPool : LKBehaviour
    {
        [SerializeField]
        private SymbolIcon _prefab_symbolIcon;
        [SerializeField]
        private GameObject _obj_panel;
        [SerializeField]
        private List<TMP_Text> labels;
        [SerializeField]
        private List<ScrollRect> scrolls;
        [SerializeField]
        private List<SymbolIcon> symbols;

        protected override object HoverItem => GameManager.LocalCharacter;

        public void UpdateSelf(Character character, IEnumerable<Symbol> symbols)
        {
            int i = 0;
            foreach (Symbol symbol in symbols)
            {
                SymbolIcon symbolIcon = null;
                if (i < this.symbols.Count) symbolIcon = this.symbols[i++];
                else
                {
                    symbolIcon = SymbolIcon.Instantiate(this._prefab_symbolIcon);
                    symbolIcon.name = "Symbol Icon " + ++i;
                    symbolIcon.CanClick = false;
                    this.symbols.Add(symbolIcon);
                }
                if (symbol.Cooldown < this.labels.Count)
                    symbolIcon.Parent = this.scrolls[symbol.Cooldown].content.gameObject;
                else
                    symbolIcon.Parent = this.scrolls[this.labels.Count].content.gameObject;
                symbolIcon.SetSymbol(symbol);
                symbolIcon.Active = symbol.Cooldown != 0 || character.Hand.Length > 11 || GameManager.InitiativeEntity == character;
            }
            while (i < this.symbols.Count)
                this.symbols[i++].Active = false;
        }

        protected override bool OnShowHoverText() => false;
    }
}