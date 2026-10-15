using LostKingdom.DataStructure;
using LostKingdom.Manager;
using LostKingdom.Prefab;
using LostKingdom.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LostKingdom.Content
{
    [Serializable]
    public abstract class Entity
    {
        /// <summary> 实体编号 </summary>
        public abstract int Type { get; }
        /// <summary> 实体名 </summary>
        public virtual string Name { get; protected set; }
        /// <summary> 实体展示名 </summary>
        public virtual string DisplayName { get; set; }
        /// <summary> 内部编号 </summary>
        public int ID { get; internal set; }
        /// <summary> 生命值 </summary>
        public int HP { get => this.hp.Value; set => this.hp.Value = value; }
        /// <summary> 基础生命值上限 </summary>
        public int BasicHP { get => this.maxHP.BasicValue; protected set => this.maxHP.BasicValue = value; }
        /// <summary> 基础攻击力 </summary>
        public int BasicAttack { get => this.attack.BasicValue; protected set => this.attack.BasicValue = value; }
        /// <summary> 基础防御力 </summary>
        public int BasicDefence { get => this.defence.BasicValue; protected set => this.defence.BasicValue = value; }
        /// <summary> 基础敏捷值 </summary>
        public int BasicAgility { get => this.agility.BasicValue; protected set => this.agility.BasicValue = value; }
        /// <summary> 最终生命值上限 </summary>
        public int FinalMaxHP => this.maxHP.FinalValue;
        /// <summary> 最终攻击力 </summary>
        public int FinalAttack => this.attack;
        /// <summary> 最终防御力 </summary>
        public int FinalDefence => this.defence;
        /// <summary> 最终敏捷值 </summary>
        public int FinalAgility => this.agility;
        /// <summary> 所在图格横坐标 </summary>
        public int PositionX { get => this.position.x; set => this.position.x = value; }
        /// <summary> 所在图格纵坐标 </summary>
        public int PositionY { get => this.position.y; set => this.position.y = value; }
        /// <summary> 所在图格坐标 </summary>
        public Vector2Int Position { get => this.position; set => this.position = value; }
        /// <summary> 是否正在行动 </summary>
        public bool Initiating{ get; private set; }
        public int CurrPriority { get; private set; }
        public int NextPriority { get; private set; }

        public EntityObject EntityObject { get; private set; }

        private LinkedList<Buff> buffs;
        [SerializeField]
        private Vector2Int position;
        [SerializeField]
        private GameValue hp;
        [SerializeField]
        private GameProperty maxHP;
        [SerializeField]
        private GameProperty attack;
        [SerializeField]
        private GameProperty defence;
        [SerializeField]
        private GameProperty agility;

        public Entity(EntityObject eobj)
        {
            this.hp = new GameValue(1, 0, 100, null, GameValue.UpdateType.KeepPercent);
            this.maxHP = new GameProperty("HP", 1, null, (o, n) => this.hp.ValueMax = n);
            this.attack = new GameProperty("ATK", 0);
            this.defence = new GameProperty("DEF", 0);
            this.agility = new GameProperty("AGI", 0);
            this.buffs = new LinkedList<Buff>();
            this.SetDefaults(0);
        }

        /// <summary> 施加状态效果 </summary>
        /// <param name="type">Buff编号</param>
        /// <param name="level">施加层数</param>
        /// <param name="levelup">是否叠加</param>
        public void AddBuff(int type, int level = 1, bool levelup = false)
        {
            for (LinkedListNode<Buff> node = this.buffs.First; true; node = node.Next)
            {
                if (node == null)
                {
                    Buff buff = new Buff(type);
                    buff.Level = level;
                    buff.OnActivate(this);
                    this.buffs.AddLast(buff);
                    break;
                }
                else if (node.Value.Type == type)
                {
                    Buff buff = node.Value;
                    buff.Level = level + (levelup ? buff.Level : 0);
                    break;
                }
            }
        }

        /// <summary> 查看状态效果 </summary>
        /// <param name="type">Buff编号</param>
        /// <returns>目标状态效果的层数，若未查找到则返回0</returns>
        public int HasBuff(int type)
        {
            for (LinkedListNode<Buff> node = this.buffs.First; node != null; node = node.Next)
                if (node.Value.Type == type) return node.Value.Level;
            return 0;
        }

        /// <summary> 移除状态效果 </summary>
        /// <param name="type">Buff编号</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveBuff(int type)
        {
            for (LinkedListNode<Buff> node = this.buffs.First; node != null; node = node.Next)
            {
                if (node.Value.Type == type)
                {
                    node.Value.OnDeactivate(this);
                    node.Value.Level = 0;
                    this.buffs.Remove(node);
                    return true;
                }
            }
            return false;
        }

        public void UpdateSelf()
        {
            for (LinkedListNode<Buff> node = this.buffs.First; node != null; node = node.Next)
            {
                node.Value.UpdateSelf(this);
                if (node.Value.Decay > 0) node.Value.Level -= node.Value.Decay;
                if (node.Value.Level <= 0)
                {
                    node.Value.OnDeactivate(this);
                    this.buffs.Remove(node);
                }
            }
        }

        public void DealDamage(Entity opponent, Symbol symbol, int damage)
        {
            if (this.PreDealDamage(opponent, symbol, ref damage))
                opponent.TakeDamage(this, symbol, damage.Max(0));
        }

        public void TakeDamage(Entity attacker, Symbol symbol, int damage)
        {
            if (this.PreTakeDamage(attacker, symbol, ref damage))
            {
                damage = (damage - this.FinalDefence).Max(0);
                this.HP -= damage;
                GameManager.NewCombatText(0, damage.ToString()).Position = this.position + Vector2.up;
                attacker.PostDealDamage(this, symbol, damage);
                this.PostTakeDamage(attacker, symbol, damage);
            }
        }

        public virtual bool PreDealDamage(Entity opponent, Symbol symbol, ref int damage) => true; 
        public virtual bool PreTakeDamage(Entity attacker, Symbol symbol, ref int damage) => true;
        public virtual void PostDealDamage(Entity opponent, Symbol symbol, int damage) { }
        public virtual void PostTakeDamage(Entity attacker, Symbol symbol, int damage) { }
        public abstract bool SetDefaults(int type);

        public void BeginAction() 
        {
            if (this.PreActionBegin())
            {
                this.Initiating = true;
                this.OnActionBegin();
                this.PostActionBegin();
            }
        }
        

        public void EndAction() 
        {
            if (this.PreActionEnd())
            {
                this.OnActionEnd();
                this.PostActionEnd();
                this.Initiating = false;
                GameManager.NextInitiative();
            }
        }

        public void NextRound()
        {
            this.UpdateSelf();
        }

        /// <summary> 战斗开始时 </summary>
        public virtual void OnBattleBegin() { }

        /// <summary> 战斗结束时 </summary>
        public virtual void OnBattleEnd() { }

        /// <summary> 行动开始时 </summary>
        public virtual void OnActionBegin() { }

        /// <summary> 行动结束时 </summary>
        public virtual void OnActionEnd() { }

        /// <summary> 行动开始前 </summary>
        /// <returns> 是否开始行动 </returns>
        public virtual bool PreActionBegin() => true;

        /// <summary> 行动开始后 </summary>
        public virtual void PostActionBegin() { }

        /// <summary> 行动结束前 </summary>
        /// <returns> 是否结束行动 </returns>
        public virtual bool PreActionEnd() => true;

        /// <summary> 行动结束后 </summary>
        public virtual void PostActionEnd() { }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(this.DisplayName);
            stringBuilder.Append($"HP: {this.HP}/{this.FinalMaxHP} ({(100.0f * this.HP / this.FinalMaxHP).Round()}%)");
            foreach (Buff buff in this.buffs) stringBuilder.Append('\n' + buff.ToString());
            return stringBuilder.ToString();
        }
    }
}