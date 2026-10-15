using LostKingdom.DataStructure;
using System.Collections.Generic;
using Unity.VisualScripting;


namespace LostKingdom.ID
{
    public static class EquipmentID
    {
        /// <summary> 装备总数 </summary>
        public const int Count = 3;

        /// <summary> 无 </summary>
        public const int None = ItemID.None;

        /// <summary> 蓝宝石吊坠 </summary>
        public const int ArtonsSapphire = ItemID.ArtonsSapphire;

        /// <summary> 古法斗篷 </summary>
        public const int ArchaicCloak = ItemID.ArchaicCloak;

        /// <summary> 木制法杖 </summary>
        public const int WoodenStaff = ItemID.WoodenStaff;

        public enum Number
        {
            /// <summary> 无 </summary>
            None = ItemID.None,
            /// <summary> 蓝宝石吊坠 </summary>
            ArtonsSapphire = ItemID.ArtonsSapphire,
            /// <summary> 古法斗篷 </summary>
            ArchaicCloak = ItemID.ArchaicCloak,
            /// <summary> 木制法杖 </summary>
            WoodenStaff = ItemID.WoodenStaff,
        }

        public static int GetNumber(string name) => EquipmentID.dict.GetValueOrDefault(name, EquipmentID.None);

        private static Dictionary<string, int> dict = new Dictionary<string, int>()
        {
            { nameof(EquipmentID.ArtonsSapphire), EquipmentID.ArtonsSapphire },
            { nameof(EquipmentID.ArchaicCloak), EquipmentID.ArchaicCloak },
            { nameof(EquipmentID.WoodenStaff), EquipmentID.WoodenStaff },
        };
    }
}