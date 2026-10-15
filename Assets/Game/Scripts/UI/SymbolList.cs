using LostKingdom.Content;
using LostKingdom.DataStructure;
using LostKingdom.Manager;
using LostKingdom.Prefab;
using System.Collections.Generic;
using UnityEngine;

namespace LostKingdom.UI
{
    [DisallowMultipleComponent]
    public sealed class SymbolList : LKBehaviour
    {
        [SerializeField]
        private SymbolIcon _prefab_symbolIcon;

        private ObjectPool<SymbolIcon> symbolIcons;
        private List<SymbolIcon> symbols;

        protected override void Awake()
        {
            this.symbols = new List<SymbolIcon>();
            this.symbolIcons = new ObjectPool<SymbolIcon>((i) =>
            {
                SymbolIcon symbolIcon = SymbolIcon.Instantiate(this._prefab_symbolIcon);
                symbolIcon.name = "Symbol " + (i + 1);
                symbolIcon.SetSymbol(null);
                symbolIcon.Parent = this.gameObject;
                symbolIcon.Active = false;
                this.symbols.Add(symbolIcon);
                return symbolIcon;
            }, 11, "UI_HUD: Symbol Icons");
        }

        protected override void Update()
        {
            if (GameManager.LocalCharacter != null)
            {
                this.symbolIcons.Clear();
                foreach (SymbolIcon symbolIcon in this.symbols) symbolIcon.Active = false;
                foreach (Symbol symbol in GameManager.LocalCharacter.Hand)
                    if (symbol != null)
                        this.AddSymbol(symbol);
            }
        }

        public void AddSymbol(Symbol symbol)
        {
            if (symbol != null)
            {
                SymbolIcon symbolIcon = this.symbolIcons.Request();
                symbolIcon.Active = true;
                symbolIcon.SetSymbol(symbol);
            }
        }

        public bool RemoveSymbol(Symbol symbol)
        {
            SymbolIcon symbolIcon = this.symbolIcons.Find((t) => t.symbol == symbol);
            if (symbolIcon != null)
            {
                this.symbolIcons.Release(symbolIcon).SetSymbol(null);
                symbolIcon.Active = false;
                symbolIcon.SetSymbol(null);
            }
            return symbolIcon != null;
        }
    }
}