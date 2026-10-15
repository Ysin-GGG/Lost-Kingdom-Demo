using LostKingdom.DataStructure;
using System;
using System.Collections.Generic;


namespace LostKingdom.ID
{
    public static class SymbolID
    {
        /// <summary> 原版技能总数 </summary>
        public const int Count = 6;

        /// <summary> 无 </summary>
        public const int None = 0;

        /// <summary> 火球 </summary>
        public const int Fireball = 1;

        /// <summary> 降雨 </summary>
        public const int Rain = 2;

        /// <summary> 魔能护盾 </summary>
        public const int MagicShield = 3;

        /// <summary> 冰箭术 </summary>
        public const int IceArrow = 4;

        /// <summary> 焰火轮 </summary>
        public const int FireWheel = 5;

        /// <summary> 奥术潮涌 </summary>
        public const int ArcaneSurge = 6;

        public enum Number
        {
            /// <summary> 无 </summary>
            None = SymbolID.None,
            /// <summary> 火球 </summary>
            Fireball = SymbolID.Fireball,
            /// <summary> 降雨 </summary>
            Rain = SymbolID.Rain,
            /// <summary> 魔能护盾 </summary>
            MagicShield = SymbolID.MagicShield,
            /// <summary> 冰箭术 </summary>
            IceArrow = SymbolID.IceArrow,
            /// <summary> 焰火轮 </summary>
            FireWheel = SymbolID.FireWheel,
            /// <summary> 奥术潮涌 </summary>
            ArcaneSurge = SymbolID.ArcaneSurge,
        }

        public static int GetNumber(string name) => SymbolID.dict.GetValueOrDefault(name, SymbolID.None);

        private static Dictionary<string, int> dict = new Dictionary<string, int>()
        {
            { nameof(SymbolID.Fireball), SymbolID.Fireball },
            { nameof(SymbolID.Rain), SymbolID.Rain },
            { nameof(SymbolID.MagicShield), SymbolID.MagicShield },
            { nameof(SymbolID.IceArrow), SymbolID.IceArrow },
            { nameof(SymbolID.FireWheel), SymbolID.FireWheel },
            { nameof(SymbolID.ArcaneSurge), SymbolID.ArcaneSurge },
        };
    }
}