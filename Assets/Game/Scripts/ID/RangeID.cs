using LostKingdom.DataStructure;


namespace LostKingdom.ID
{
    public static class RangeID
    {
        /// <summary> 原版范围类型总数 </summary>
        public const int Count = 4;

        /// <summary> 单体目标 </summary>
        public const int Single = 0;

        /// <summary> 多重目标 </summary>
        public const int Multiple = 1;

        /// <summary> 范围 </summary>
        public const int AoE = 2;

        /// <summary> 自身 </summary>
        public const int Self = 3;

        public enum Number
        {
            /// <summary> 单体目标 </summary>
            Single = RangeID.Single,
            /// <summary> 多重目标 </summary>
            Multiple = RangeID.Multiple,
            /// <summary> 范围 </summary>
            AoE = RangeID.AoE,
            /// <summary> 自身 </summary>
            Self = RangeID.Self,
        }
    }
}