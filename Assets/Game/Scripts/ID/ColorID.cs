using LostKingdom.Utility;
using System.Collections.Generic;


namespace LostKingdom.ID
{
    public static class ColorID
    {
        /// <summary> 技能稀有度-古铜 </summary>
        public const string Copper = "#F9C389";

        /// <summary> 技能稀有度-圣银 </summary>
        public const string Silver = "#C0C0C0";

        /// <summary> 技能稀有度-皇金 </summary>
        public const string Gold = "#F7EC5F";

        /// <summary> 技能稀有度-宝钻 </summary>
        public const string Diamond = "#C2FCFC";

        /// <summary> 装备稀有度-普通 </summary>
        public const string Accepted = "#606E72";

        /// <summary> 装备稀有度-精制 </summary>
        public const string Refined = "#9A9273";

        /// <summary> 装备稀有度-附魔 </summary>
        public const string Enchanted = "#829F78";

        /// <summary> 装备稀有度-失传 </summary>
        public const string Isolated = "#4C9CC6";

        /// <summary> 实体阵营-我军 </summary>
        public const string Party = "#CCCCCC";   

        /// <summary> 实体阵营-盟军 </summary>
        public const string Ally = "#4889DD";   

        /// <summary> 实体阵营-敌军 </summary>
        public const string Enemy = "#DC5C5C";

        /// <summary> 技能稀有度ID-古铜 </summary>
        public const int CopperID = 0;

        /// <summary> 技能稀有度ID-圣银 </summary>
        public const int SilverID = 1;

        /// <summary> 技能稀有度ID-皇金 </summary>
        public const int GoldID = 2;

        /// <summary> 技能稀有度ID-精钻 </summary>
        public const int DiamondID = 3;

        /// <summary> 装备稀有度ID-普通 </summary>
        public const int AcceptedID = 4;

        /// <summary> 装备稀有度ID-精制 </summary>
        public const int RefinedID = 5;

        /// <summary> 装备稀有度ID-附魔 </summary>
        public const int EnchantedID = 6;

        /// <summary> 装备稀有度ID-失传 </summary>
        public const int IsolatedID = 7;

        /// <summary> 实体阵营ID-我军 </summary>
        public const int PartyID = 8;

        /// <summary> 实体阵营ID-盟军 </summary>
        public const int AllyID = 9;

        /// <summary> 实体阵营ID-敌军 </summary>
        public const int EnemyID = 10;

        public enum Number
        {
            /// <summary> 技能稀有度ID-古铜 </summary>
            Copper = ColorID.CopperID,
            /// <summary> 技能稀有度ID-圣银 </summary>
            Silver = ColorID.SilverID,
            /// <summary> 技能稀有度ID-皇金 </summary>
            Gold = ColorID.GoldID,
            /// <summary> 技能稀有度ID-精钻 </summary>
            Diamond = ColorID.DiamondID,
            /// <summary> 装备稀有度ID-普通 </summary>
            Accepted = ColorID.AcceptedID,
            /// <summary> 装备稀有度ID-精制 </summary>
            Refined = ColorID.RefinedID,
            /// <summary> 装备稀有度ID-附魔 </summary>
            Enchanted = ColorID.EnchantedID,
            /// <summary> 装备稀有度ID-失传 </summary>
            Isolated = ColorID.IsolatedID,
            /// <summary> 实体阵营ID-我军 </summary>
            PartyID = ColorID.PartyID,
            /// <summary> 实体阵营ID-盟军 </summary>
            AllyID = ColorID.AllyID,
            /// <summary> 实体阵营ID-敌军 </summary>
            EnemyID = ColorID.EnemyID,
        }

        public static int GetNumber(string name) => ColorID.dict.GetValueOrDefault(name, -1);
        public static string GetCode(string name) => ColorID.dict.ContainsKey(name) ? ColorID.array[ColorID.dict[name]] : null;
        public static LKColor GetColor(string name) => ColorID.dict.ContainsKey(name) ? ColorID.color[ColorID.dict[name]] : null;
        public static LKColor GetColor(int id) => (id < 0 || id > ColorID.DiamondID) ? null : ColorID.color[id];

        private readonly static Dictionary<string, int> dict = new Dictionary<string, int>()
        {
            { nameof(ColorID.Copper), ColorID.CopperID },
            { nameof(ColorID.Silver), ColorID.SilverID },
            { nameof(ColorID.Gold), ColorID.GoldID },
            { nameof(ColorID.Diamond), ColorID.DiamondID },
            { nameof(ColorID.Accepted), ColorID.AcceptedID },
            { nameof(ColorID.Refined), ColorID.RefinedID },
            { nameof(ColorID.Enchanted), ColorID.EnchantedID },
            { nameof(ColorID.Isolated), ColorID.IsolatedID },
            { nameof(ColorID.Party), ColorID.PartyID },
            { nameof(ColorID.Ally), ColorID.AllyID },
            { nameof(ColorID.Enemy), ColorID.EnemyID },
        };

        private readonly static string[] array = new string[11]
        {
            ColorID.Copper,
            ColorID.Silver,
            ColorID.Gold,
            ColorID.Diamond,
            ColorID.Accepted,
            ColorID.Refined,
            ColorID.Enchanted,
            ColorID.Isolated,
            ColorID.Party,
            ColorID.Ally,
            ColorID.Enemy,
        };

        private readonly static LKColor[] color = new LKColor[11]
        {
            ColorID.Copper,
            ColorID.Silver,
            ColorID.Gold,
            ColorID.Diamond,
            ColorID.Accepted,
            ColorID.Refined,
            ColorID.Enchanted,
            ColorID.Isolated,
            ColorID.Party,
            ColorID.Ally,
            ColorID.Enemy,
        };
    }
}