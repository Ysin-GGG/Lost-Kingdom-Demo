using LostKingdom.DataStructure;
using LostKingdom.ID;
using LostKingdom.Manager;
using LostKingdom.Prefab;
using LostKingdom.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace LostKingdom.Content
{
    [Serializable]
    public class Enemy : Entity
    {
        /// <summary> 敌怪编号 </summary>
        public sealed override int Type => this.type;
        /// <summary> 敌怪名 </summary>
        public override string DisplayName { get; set; }

        private int type;

        public Enemy(int type, EntityObject eobj) : base(eobj)
        {
            this.SetDefaults(type);
        }

        public override bool SetDefaults(int type)
        {
            switch (this.type = type)
            {
                case 0:
                    this.DisplayName = null;
                    this.Name = null;
                    this.BasicHP = 0;
                    this.BasicAttack = 0;
                    this.BasicDefence = 0;
                    return true;
                case 1:
                    this.DisplayName = "Monster 1";
                    this.Name = "Monster 1";
                    this.BasicHP = 130;
                    this.BasicAttack = 2;
                    this.BasicDefence = 2;
                    this.HP = this.FinalMaxHP;
                    return true;
            }
            this.type = -1;
            return false;
        }

        /// <summary> 行动AI </summary>
        /// <returns> AI是否已完成行动 </returns>
        public virtual bool AI(int timer)
        {
            switch (this.type) { 
                case 0:
                    return true;
                case 1:
                    if(LK.LogicTimeLoop == 30) { this.DealDamage(GameManager.Characters[0], new Symbol(1), 10); }
                    return LK.LogicTimeLoop == 30;
            }
            return true;
        }

        public void Kill()
        {

        }
    }
}