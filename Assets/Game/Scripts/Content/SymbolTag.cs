using LostKingdom.DataStructure;
using LostKingdom.ID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostKingdom.Content
{
    public class SymbolTag
    {
        public string DisplayName { get; protected set; }
        public string Description { get; protected set; }
        public int Cost { get; protected set; }
        public int Type { get; private set; }

        public QualityType Quality { get; protected set; }

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
            Platinum,
        }

        public SymbolTag(int type)
        {
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
                    break;
                case 1:
                    this.Quality = QualityType.Copper;
                    this.DisplayName = "Fire Storm";
                    this.Description = "Storm of Fire";
                    this.Cost = 3;
                    break;
                case 2:
                    this.Quality = QualityType.Silver;
                    this.DisplayName = "Rain";
                    this.Description = "Rain of Water";
                    this.Cost = 2;
                    break;
                case 3:
                    this.Quality = QualityType.Silver;
                    this.DisplayName = "Arcane Surge";
                    this.Description = "Surge of Arcane";
                    this.Cost = 2;
                    break;
            }
        }
    }
}