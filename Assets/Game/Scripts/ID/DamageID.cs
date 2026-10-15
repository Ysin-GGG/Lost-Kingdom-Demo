using LostKingdom.DataStructure;


namespace LostKingdom.ID
{
    public static class DamageID
    {
        /// <summary> Ô­°æÉËº¦ÀàĞÍ×ÜÊı </summary>
        public const int Count = 4;

        /// <summary> Õ¶»÷ÉËº¦ </summary>
        public const int Sever = 0;

        /// <summary> ¶Û»÷ÉËº¦ </summary>
        public const int Blunt = 1;

        /// <summary> ´©´ÌÉËº¦ </summary>
        public const int Pierce = 2;

        /// <summary> ·¨ÊõÉËº¦ </summary>
        public const int Spell = 3;

        public enum Number
        {
            /// <summary> Õ¶»÷ÉËº¦ </summary>
            Sever = DamageID.Sever,
            /// <summary> ¶Û»÷ÉËº¦ </summary>
            Blunt = DamageID.Blunt,
            /// <summary> ¶Û»÷ÉËº¦ </summary>
            Pierce = DamageID.Pierce,
            /// <summary> ·¨ÊõÉËº¦ </summary>
            Spell = DamageID.Spell,
        }
    }
}