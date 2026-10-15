using LostKingdom.DataStructure;


namespace LostKingdom.ID
{
    public static class ElementID
    {
        /// <summary> 原版元素类型总数 </summary>
        public const int Count = 10;

        /// <summary> 无属性伤害 </summary>
        public const int None = 0;

        /// <summary> 水属性伤害 </summary>
        public const int Water = 1;

        /// <summary> 火属性伤害 </summary>
        public const int Fire = 2;

        /// <summary> 土属性伤害 </summary>
        public const int Earth = 3;

        /// <summary> 风属性伤害 </summary>
        public const int Air = 4;

        /// <summary> 冰属性伤害 </summary>
        public const int Ice = 5;

        /// <summary> 雷属性伤害 </summary>
        public const int Thunder = 6;

        /// <summary> 木属性伤害 </summary>
        public const int Plant = 7;

        /// <summary> 光属性伤害 </summary>
        public const int Miracle = 8;

        /// <summary> 暗属性伤害 </summary>
        public const int Shadow = 9;

        public enum Number
        {
            /// <summary> 无属性伤害 </summary>
            None = ElementID.None,
            /// <summary> 水属性伤害 </summary>
            Water = ElementID.Water,
            /// <summary> 火属性伤害 </summary>
            Fire = ElementID.Fire,
            /// <summary> 土属性伤害 </summary>
            Earth = ElementID.Earth,
            /// <summary> 风属性伤害 </summary>
            Air = ElementID.Air,
            /// <summary> 冰属性伤害 </summary>
            Ice = ElementID.Ice,
            /// <summary> 雷属性伤害 </summary>
            Thunder = ElementID.Thunder,
            /// <summary> 木属性伤害 </summary>
            Plant = ElementID.Plant,
            /// <summary> 光属性伤害 </summary>
            Miracle = ElementID.Miracle,
            /// <summary> 暗属性伤害 </summary>
            Shadow = ElementID.Shadow,
        }
    }
}