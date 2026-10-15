using LostKingdom.ID;
using LostKingdom.DataStructure;
using LostKingdom.Utility;
using LostKingdom.Manager;

namespace LostKingdom.Content
{
    public class SoleEffect
    {
        public string Description { get; protected set; }
        private int type;
        private Bit16 data;
        private Symbol symbol;
        private int combo;
        private int damage;
        private int buffID;
        private int buffLV;
        private float chance;
        private float bonus;

        public SoleEffect(Symbol symbol, int type)
        {
            this.symbol = symbol;
            this.type = 0;
            this.SetDefaults(type);
        }

        /// <summary> 应用此效果 </summary>
        /// <param name="dealer">应用方</param>
        /// <param name="taker">被应用方</param>
        public void Apply(Entity dealer, Entity taker)
        {
            switch (this.type)
            {
                case 1:
                    switch (this.symbol.Type)
                    {
                        case SymbolID.Fireball:
                            dealer.DealDamage(taker, this.symbol, this.damage);
                            break;
                        case SymbolID.Rain:
                            taker.AddBuff(this.buffID, this.buffLV, true);
                            break;
                        case SymbolID.MagicShield:
                            taker.AddBuff(this.buffID, this.buffLV, true);
                            break;
                        case SymbolID.IceArrow:
                            dealer.DealDamage(taker, this.symbol, this.damage);
                            break;
                        case SymbolID.FireWheel:
                            for (int i = 0; i < this.combo; i++)
                                dealer.DealDamage(taker, this.symbol, this.damage);
                            break;
                        case SymbolID.ArcaneSurge:
                            break;
                    }
                    break;
                case 2:
                    switch (this.symbol.Type)
                    {
                        case SymbolID.Fireball:
                            if (GameManager.RandGen_BattleEffect.NextBool(this.chance)) 
                                taker.AddBuff(this.buffID, this.buffLV, true);
                            break;
                        case SymbolID.IceArrow:
                            if (GameManager.RandGen_BattleEffect.NextBool(this.chance)) taker.AddBuff(this.buffID, this.buffLV, true);
                            break;
                        case SymbolID.FireWheel:
                            for (int i = 0; i < 3; i++) if (GameManager.RandGen_BattleEffect.NextBool(this.chance)) taker.AddBuff(this.buffID, this.buffLV, true);
                            break;
                    }
                    break;
            }
        }
        /// <summary> 初始化数据 </summary>
        /// <param name="type">类型编号</param>
        public void SetDefaults(int type)
        {
            this.combo = 0;
            this.damage = 0;
            this.buffID = 0;
            this.buffLV = 0;
            this.chance = 0f;
            this.bonus = 0f;
            this.Description = "";
            switch (this.type = type)
            {
                case 1:
                    switch (this.symbol.Type)
                    {
                        case SymbolID.Fireball:
                            this.damage = 11;
                            this.Description = LKLocalization.GetText($"Symbol.SoleEffect.{((SymbolID.Number)this.symbol.Type).ToString()}_{this.type}", this.damage);
                            break;
                        case SymbolID.Rain:
                            this.buffID = 1;
                            this.buffLV = 3;
                            this.Description = LKLocalization.GetText($"Symbol.SoleEffect.{((SymbolID.Number)this.symbol.Type).ToString()}_{this.type}", this.buffLV, LKLocalization.GetText($"Buff.Name.{((BuffID.Number)this.buffID).ToString()}"));
                            break;
                        case SymbolID.MagicShield:
                            this.buffID = 2;
                            this.buffLV = 2;
                            this.Description = LKLocalization.GetText($"Symbol.SoleEffect.{((SymbolID.Number)this.symbol.Type).ToString()}_{this.type}", this.buffLV, LKLocalization.GetText($"Buff.Name.{((BuffID.Number)this.buffID).ToString()}"));
                            break;
                        case SymbolID.IceArrow:
                            this.damage = 7;
                            this.Description = LKLocalization.GetText($"Symbol.SoleEffect.{((SymbolID.Number)this.symbol.Type).ToString()}_{this.type}", this.damage);
                            break;
                        case SymbolID.FireWheel:
                            this.combo = 3;
                            this.damage = 7;
                            this.Description = LKLocalization.GetText($"Symbol.SoleEffect.{((SymbolID.Number)this.symbol.Type).ToString()}_{this.type}", this.damage, this.combo);
                            break;
                        case SymbolID.ArcaneSurge:
                            this.bonus = .1f;
                            this.Description = LKLocalization.GetText($"Symbol.SoleEffect.{((SymbolID.Number)this.symbol.Type).ToString()}_{this.type}", (this.bonus * 100f).Round()); ;
                            break;
                    }
                    break;
                case 2:
                    switch (this.symbol.Type)
                    {
                        case SymbolID.Fireball:
                            this.buffID = 3;
                            this.buffLV = 2;
                            this.chance = .5f;
                            this.Description = LKLocalization.GetText($"Symbol.SoleEffect.{((SymbolID.Number)this.symbol.Type).ToString()}_{this.type}", (this.chance * 100f).Round(), this.buffLV, LKLocalization.GetText($"Buff.Name.{((BuffID.Number)this.buffID).ToString()}"));
                            break;
                        case SymbolID.IceArrow:
                            this.buffID = 4;
                            this.buffLV = 1;
                            this.chance = .1f;
                            this.Description = LKLocalization.GetText($"Symbol.SoleEffect.{((SymbolID.Number)this.symbol.Type).ToString()}_{this.type}", (this.chance * 100f).Round(), this.buffLV, LKLocalization.GetText($"Buff.Name.{((BuffID.Number)this.buffID).ToString()}"));
                            break;
                        case SymbolID.FireWheel:
                            this.buffID = 3;
                            this.buffLV = 1;
                            this.chance = .33f;
                            this.Description = LKLocalization.GetText($"Symbol.SoleEffect.{((SymbolID.Number)this.symbol.Type).ToString()}_{this.type}", (this.chance * 100f).Round(), this.buffLV, LKLocalization.GetText($"Buff.Name.{((BuffID.Number)this.buffID).ToString()}"));
                            break;
                    }
                    break;
            }
        }
    }
}