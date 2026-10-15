using LostKingdom.DataStructure;
using System;
using System.Collections.Generic;


namespace LostKingdom.ID
{
    public static class BuffID
    {
        /// <summary> 原版效果总数 </summary>
        public const int Count = 4;

        /// <summary> 无 </summary>
        public const int None = 0;

        /// <summary> 潮湿 </summary>
        public const int Wet = 1;

        /// <summary> 魔能护盾 </summary>
        public const int MagicShield = 2;

        /// <summary> 灼烧 </summary>
        public const int Burn = 3;

        /// <summary> 冻结 </summary>
        public const int Frozen = 4;

        public enum Number
        {
            /// <summary> 无 </summary>
            None = BuffID.None,
            /// <summary> 潮湿 </summary>
            Wet = BuffID.Wet,
            /// <summary> 魔能护盾 </summary>
            MagicShield = BuffID.MagicShield,
            /// <summary> 灼烧 </summary>
            Burn = BuffID.Burn,
            /// <summary> 冻结 </summary>
            Frozen = BuffID.Frozen,
        }

        public static int GetNumber(string name) => BuffID.dict.GetValueOrDefault(name, BuffID.None);

        private static Dictionary<string, int> dict = new Dictionary<string, int>()
        {
            { nameof(BuffID.Wet), BuffID.Wet },
            { nameof(BuffID.MagicShield), BuffID.MagicShield },
            { nameof(BuffID.Burn), BuffID.Burn },
            { nameof(BuffID.Frozen), BuffID.Frozen },
        };
    }
}