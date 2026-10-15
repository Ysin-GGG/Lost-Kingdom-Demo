using LostKingdom.ID;
using LostKingdom.Utility;
using System.Collections.Generic;
using System.Text;

namespace LostKingdom.Content
{
    public class Symbol
    {
        /// <summary> 技能名 </summary>
        public string DisplayName { get; protected set; }
        /// <summary> 技能描述 </summary>
        public string Description { get; protected set; }
        /// <summary> 技能备注 </summary>
        public string Dedication { get; protected set; }
        /// <summary> 内部编号 </summary>
        public int Type { get; private set; }
        /// <summary> AP需求值 </summary>
        public int Cost { get; protected set; }
        /// <summary> 冷却回合数 </summary>
        public int Cooldown { get => this.cd.Value; set => this.cd.Value = value; }
        /// <summary> 基础冷却回合数 </summary>
        public int BasicCooldown { get => this.maxCD.BasicValue; protected set => this.maxCD.BasicValue = value; }
        /// <summary> 最终冷却回合数 </summary>
        public int FinalCooldown => this.maxCD.FinalValue;
        /// <summary> 技能品质 </summary>
        public QualityType Quality { get; protected set; }
        /// <summary> 有效射程（即技能可锁定的范围） </summary>
        public Lattice EffectiveRange { get; protected set; }
        /// <summary> 有效区域（即技能可影响的范围） </summary>
        public Lattice EffectiveArea { get; protected set; }

        private List<SoleEffect> effects;

        private GameValue cd;
        private GameProperty maxCD;

        public DamageID.Number DamageID { get; protected set; } = ID.DamageID.Number.Sever;

        public enum Tag4Effect
        {
            Attack,
            Skill,
            Magic,
        }
        public enum Tag4Range
        {
            Single,
            Multiple,
            AoE,
        }

        public enum QualityType
        {
            Copper,
            Silver,
            Gold,
            Diamond,
        }

        public Symbol(int type)
        {
            this.cd = new GameValue(0, 0, 1, null, GameValue.UpdateType.KeepValue);
            this.maxCD = new GameProperty("Cooldown", 1, null, (o, n) => this.cd.ValueMax = n);
            this.effects = new List<SoleEffect>();
            this.SetDefaults(type);
        }

        public void SetDefaults(int type)
        {
            switch (this.Type = type)
            {
                case 0:
                    this.DisplayName = null;
                    this.Description = null;
                    this.Cost = 0;
                    this.EffectiveRange = Lattice.Self;
                    this.EffectiveArea = Lattice.Self;
                    break;
                case SymbolID.Fireball:
                    this.Quality = QualityType.Copper;
                    this.DisplayName = LKLocalization.GetText("Symbol.Name.Fireball");
                    this.Description = LKLocalization.GetText("Symbol.Description.Fireball");
                    this.Cost = 2;
                    this.BasicCooldown = 2;
                    this.EffectiveRange = Lattice.Circle(8, false) - Lattice.Circle(3, true);
                    this.EffectiveArea = Lattice.Circle(1, true);
                    this.effects.Add(new SoleEffect(this, 1));
                    this.effects.Add(new SoleEffect(this, 2));
                    break;
                case SymbolID.Rain:
                    this.Quality = QualityType.Copper;
                    this.DisplayName = LKLocalization.GetText("Symbol.Name.Rain");
                    this.Description = LKLocalization.GetText("Symbol.Description.Rain");
                    this.Cost = 2;
                    this.BasicCooldown = 3;
                    this.EffectiveRange = Lattice.Circle(4, true);
                    this.EffectiveArea = Lattice.Rectangle(3, 3);
                    this.effects.Add(new SoleEffect(this, 1));
                    break;
                case SymbolID.MagicShield:
                    this.Quality = QualityType.Silver;
                    this.DisplayName = LKLocalization.GetText("Symbol.Name.MagicShield");
                    this.Description = LKLocalization.GetText("Symbol.Description.MagicShield");
                    this.Cost = 3;
                    this.BasicCooldown = 3;
                    this.EffectiveRange = Lattice.Self;
                    this.EffectiveArea = Lattice.Self;
                    this.effects.Add(new SoleEffect(this, 1));
                    break;
                case SymbolID.IceArrow:
                    this.Quality = QualityType.Copper;
                    this.DisplayName = LKLocalization.GetText("Symbol.Name.IceArrow");
                    this.Description = LKLocalization.GetText("Symbol.Description.IceArrow");
                    this.Cost = 1;
                    this.BasicCooldown = 1;
                    this.EffectiveRange = Lattice.Circle(7, false);
                    this.EffectiveArea = Lattice.Self;
                    this.effects.Add(new SoleEffect(this, 1));
                    this.effects.Add(new SoleEffect(this, 2));
                    break;
                case SymbolID.FireWheel:
                    this.Quality = QualityType.Silver;
                    this.DisplayName = LKLocalization.GetText("Symbol.Name.FireWheel");
                    this.Description = LKLocalization.GetText("Symbol.Description.FireWheel");
                    this.Cost = 2;
                    this.BasicCooldown = 4;
                    this.EffectiveRange = Lattice.Self;
                    this.EffectiveArea = Lattice.Rectangle(3, 3) - Lattice.Self;
                    this.effects.Add(new SoleEffect(this, 1));
                    this.effects.Add(new SoleEffect(this, 2));
                    break;
                case SymbolID.ArcaneSurge:
                    this.Quality = QualityType.Silver;
                    this.DisplayName = LKLocalization.GetText("Symbol.Name.ArcaneSurge");
                    this.Description = LKLocalization.GetText("Symbol.Description.ArcaneSurge");
                    this.Cost = 3;
                    this.BasicCooldown = 4;
                    this.EffectiveRange = Lattice.Self;
                    this.EffectiveArea = Lattice.Self;
                    this.effects.Add(new SoleEffect(this, 1));
                    break;
            }
        }

        public string GetBasicText()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Cooldown - " + this.BasicCooldown);
            stringBuilder.AppendLine("AP Cost - " + this.Cost);
            return stringBuilder.ToString();
        }

        public string GetEffectText()
        {
            StringBuilder stringBuilder = new StringBuilder();  
            foreach (SoleEffect effect in this.effects) 
                stringBuilder.AppendLine(effect.Description);
            return stringBuilder.ToString();
        }

        /// <summary> 使用技能 </summary>
        public void Employ(Character employer, Entity[] targets)
        {
            this.Cooldown = this.FinalCooldown;
            foreach (Entity target in targets)
                foreach (SoleEffect effect in this.effects)
                    effect.Apply(employer, target);
        }
    }
}