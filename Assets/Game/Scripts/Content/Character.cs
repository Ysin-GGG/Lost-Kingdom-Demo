using LostKingdom.DataStructure;
using LostKingdom.ID;
using LostKingdom.Manager;
using LostKingdom.Prefab;
using LostKingdom.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LostKingdom.Content
{
    [Serializable]
    public class Character : Entity
    {
        /// <summary> 角色编号 </summary>
        public sealed override int Type => this.type;
        /// <summary> 玩家名 </summary>
        public override string DisplayName { get; set; }
        /// <summary> 角色名 </summary>
        public override string Name { get; protected set; }
        /// <summary> 角色全名 </summary>
        public string FullName { get; protected set; }
        /// <summary> 行动值 </summary>
        public int AP { get => this.ap.Value; set => this.ap.Value = value; }
        /// <summary> 基础行动值上限 </summary>
        public int BasicMaxAP { get => this.maxAP.BasicValue; protected set => this.maxAP.BasicValue = value; }
        /// <summary> 最终行动值上限 </summary>
        public int FinalMaxAP => this.maxAP;

        /// <summary> 手牌 </summary>
        public Symbol[] Hand => this.hand.ToArray();

        /// <summary> 牌组 </summary>
        public Symbol[] Deck => this.deck.ToArray();

        private int type;
        private GameValue ap;
        private GameProperty maxAP;

        private LinkedList<Symbol> symbolCDList;

        public event Action<Symbol> OnAddSymbolToDeck;
        public event Action<Symbol> OnAddSymbolToHand;
        public event Func<Symbol, bool> OnRemoveSymbolFromDeck;
        public event Func<Symbol, bool> OnRemoveSymbolFromHand;

        private List<Symbol> deck;
        private List<Symbol> hand;

        public Character(int type, EntityObject eobj) : base(eobj)
        {
            this.deck = new List<Symbol>();
            this.hand = new List<Symbol>();
            this.ap = new GameValue(1, 0, 1, null, GameValue.UpdateType.KeepDifference);
            this.maxAP = new GameProperty("AP", 1, null, (o, n) => this.ap.ValueMax = n);
            this.symbolCDList = new LinkedList<Symbol>();
            this.SetDefaults(type);
        }

        public override bool SetDefaults(int type)
        {
            switch (this.type = type)
            {
                case 0:
                    this.Name = null;
                    this.FullName = null;
                    this.DisplayName = null;
                    this.HP = 0;
                    this.AP = 0;
                    this.BasicHP = 0;
                    this.BasicMaxAP = 0;
                    this.BasicAttack = 0;
                    this.BasicDefence = 0;
                    this.BasicAgility = 0;
                    return true;
                case 1:
                    this.Name = "Arton";
                    this.FullName = "Arton miesteer";
                    this.DisplayName = "Arton miesteer";
                    this.BasicHP = 70;
                    this.BasicMaxAP = 5;
                    this.BasicAttack = 3;
                    this.BasicDefence = 2;
                    this.BasicAgility = 20;
                    this.HP = this.FinalMaxHP;
                    this.AP = this.FinalMaxAP;
                    return true;
            }
            this.type = -1;
            return false;
        }

        public void AddSymbol(Symbol symbol)
        {
            this.deck.Add(symbol);
            this.OnAddSymbolToDeck?.Invoke(symbol);
        }

        public bool RemoveSymbol(Symbol symbol)
        {
            if (this.deck.Remove(symbol))
            {
                this.OnAddSymbolToDeck?.Invoke(symbol);
                return true;
            }
            return false;
        }

        public override void OnBattleBegin()
        {
            this.symbolCDList.Clear();
            foreach(Symbol symbol in this.deck) 
            {
                symbol.Cooldown = 0; 
                this.symbolCDList.AddLast(symbol);
            }
            this.RecoverSymbol(6);
        }

        public override void OnActionBegin()
        {
            this.AP += 5;
            this.RecoverSymbol(5);
            UIManager.Game_SymbolCDPool.UpdateSelf(this, this.symbolCDList);
        }

        public override void OnActionEnd()
        {
            foreach (Symbol symbol in this.symbolCDList) symbol.Cooldown--;
            UIManager.Game_SymbolCDPool.UpdateSelf(this, this.symbolCDList);
        }

        /// <summary> 使用技能 </summary>
        /// <param name="id"> 技能索引（从0计起） </param>
        public void EmploySymbol(int id) 
        {
            if (id < 0 || id >= this.hand.Count) return;
            Symbol symbol = this.hand[id];
            if (this.AP < symbol.Cost) return;
            if (this.OnRemoveSymbolFromHand?.Invoke(symbol) ?? true)
            {
                symbol.Employ(this, GridManager.GetAreaEntity());
                GameManager.SelectedSymbol = null;
                GridManager.ShowRangeMap = false;
                GridManager.ShowAreaMap = false;
                GridManager.Range = Lattice.Empty;
                GridManager.Area = Lattice.Empty;
                this.hand.Remove(symbol);
                this.AP -= symbol.Cost;
                if (this.symbolCDList.Count > 0)
                {
                    LinkedListNode<Symbol> curr = this.symbolCDList.Last;
                    do if (symbol.Cooldown > curr.Value.Cooldown) break;
                    while ((curr = curr.Previous) != null);
                    if (curr == null) this.symbolCDList.AddFirst(symbol);
                    else this.symbolCDList.AddAfter(curr, symbol);
                }
                else this.symbolCDList.AddLast(symbol);
                UIManager.Game_SymbolCDPool.UpdateSelf(this, this.symbolCDList);
            }
        }

        /// <summary> 使用技能 </summary>
        /// <param name="symbol"> 目标技能 </param>
        public void EmploySymbol(Symbol symbol) => this.EmploySymbol(this.hand.IndexOf(symbol));

        /// <summary> 恢复技能到可用区 </summary>
        /// <param name="count"> 最大恢复数量 </param>
        public void RecoverSymbol(int count)
        {
            LinkedListNode<Symbol> curr = this.symbolCDList.First;
            while(count-- > 0)
            {
                if (curr == null || curr.Value.Cooldown > 0) return;
                if (this.hand.Count < 10)
                {
                    Symbol symbol = curr.Value;
                    LinkedListNode<Symbol> next = curr.Next;
                    this.symbolCDList.Remove(curr);
                    this.hand.Add(symbol);
                    this.OnAddSymbolToHand?.Invoke(symbol);
                    curr = next;
                }
                else break;
            }
        }
    }
}