using LostKingdom.DataStructure;
using LostKingdom.ID;
using LostKingdom.Manager;
using LostKingdom.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostKingdom.Content
{
    public class Buff
    {
        /// <summary> 显示名称 </summary>
        public string DisplayName { get; protected set; }
        /// <summary> 描述文本 </summary>
        public string Description { get; protected set; }
        /// <summary> 最大效果级别 </summary>
        public int MaxLevel { get; set; }
        /// <summary> 当前效果级别 </summary>
        public int Level { get; set; }
        /// <summary> 效果级别 </summary>
        public int Decay { get; set; }
        /// <summary> 是否为强效Buff（不可免疫） </summary>
        public bool IsPotent { get => this.bits[0]; protected set => this.bits[0] = value; }
        /// <summary> 是否为减益Buff </summary>
        public bool IsDebuff { get => this.bits[1]; protected set => this.bits[1] = value; }
        /// <summary> 类型编号 </summary>
        public int Type { get; private set; }

        private Bit8 bits;

        public Buff(int type)
        {
            this.bits = new Bit8(0);
            this.SetDefaults(type);
            if (this.Type != 0)
            {
                this.Level = 0;
                this.Decay = 1;
                this.IsPotent = false;
                this.IsDebuff = false;
                this.DisplayName = LKLocalization.GetText("Buff.Name." + ((BuffID.Number)this.Type).ToString());
                this.Description = LKLocalization.GetText("Buff.Description." + ((BuffID.Number)this.Type).ToString());
            }
            else
            {
                this.DisplayName = null;
                this.Description = null;
            }
        }

        public void SetDefaults(int type)
        {
            switch (this.Type = type)
            {
                case BuffID.None:
                    return;
                case BuffID.Wet:
                    this.IsDebuff = true;
                    break;
                case BuffID.MagicShield:
                    break;
                case BuffID.Burn:
                    this.IsDebuff = true;
                    break;
                case BuffID.Frozen:
                    this.IsDebuff = true;
                    break;
            }
        }

        public virtual void UpdateSelf(Entity entity)
        {
            int lv = this.Level;
            int dmg = 0;
            switch (this.Type)
            {
                case BuffID.None:
                    return;
                case BuffID.Wet:
                    break;
                case BuffID.MagicShield:
                    break;
                case BuffID.Burn:
                    dmg = lv;
                    break;
                case BuffID.Frozen:
                    break;
            }
            if (dmg > 0)
            {
                entity.HP -= dmg;
                GameManager.NewCombatText(0, dmg.ToString()).Position = entity.Position + Vector2.up;
            }
        }

        public virtual void OnActivate(Entity entity)
        {
            int lv = 0;
            int dmg = 0;
            switch (this.Type)
            {
                case BuffID.None:
                    return;
                case BuffID.Wet:
                    lv = entity.HasBuff(BuffID.Burn);
                    if (lv > 0)
                    {
                        dmg = (lv + this.Level) << 1;
                        entity.RemoveBuff(BuffID.Burn);
                        entity.HP -= dmg;
                        GameManager.NewCombatText(0, dmg.ToString()).Position = entity.Position + Vector2.up;
                    }
                    break;
                case BuffID.MagicShield:
                    break;
                case BuffID.Burn:
                    lv = entity.HasBuff(BuffID.Wet);
                    if (lv > 0)
                    {
                        dmg = (lv + this.Level) << 1;
                        entity.RemoveBuff(BuffID.Wet);
                    }
                    break;
                case BuffID.Frozen:
                    this.IsDebuff = true;
                    break;

            }
            if (dmg > 0)
            {
                entity.HP -= dmg;
                GameManager.NewCombatText(0, dmg.ToString()).Position = entity.Position + Vector2.up;
            }
        }

        public virtual void OnDeactivate(Entity entity)
        {

        }

        public override string ToString() => string.Format(LKLocalization.GetText("Game.Legacy.BuffLine"), this.DisplayName, this.Level, this.Description);
    }
}