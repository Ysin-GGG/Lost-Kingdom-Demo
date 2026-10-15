using LostKingdom.DataStructure;
using System;
using System.Collections.Generic;


namespace LostKingdom.ID
{
    public static class DataID
    {
        /// <summary> 原版数据包头部总数 </summary>
        public const int Count = 2;

        /// <summary> 无 </summary>
        public const int None = 0;

        /// <summary> 伪随机数生成器 </summary>
        public const int LKRandomData = 1;

        /// <summary> 角色 </summary>
        public const int Character = 2;

        /// <summary> 敌怪 </summary>
        public const int Enemy = 3;

        public enum Number
        {
            /// <summary> 无 </summary>
            None = DataID.None,
            /// <summary> 伪随机数生成器 </summary>
            LKRandomData = DataID.LKRandomData,
            /// <summary> 角色 </summary>
            Character = DataID.Character,
            /// <summary> 敌怪 </summary>
            Enemy = DataID.Enemy,
        }

        public static int GetNumber(string name) => DataID.dict.GetValueOrDefault(name, DataID.None);

        private static Dictionary<string, int> dict = new Dictionary<string, int>()
        {
            { nameof(DataID.LKRandomData), DataID.LKRandomData },
            { nameof(DataID.Character), DataID.Character },
            { nameof(DataID.Enemy), DataID.Enemy },
        };
    }
}