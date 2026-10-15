using LostKingdom.ID;
using LostKingdom.DataStructure;
using LostKingdom.Components;
using Unity.VisualScripting;
using LostKingdom.Manager;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

namespace LostKingdom.Content
{
    public class FieldTile
    {
        public int PositionX { get => this.position.x; set => this.position.x = value; }
        public int PositionY { get => this.position.y; set => this.position.y = value; }
        public Vector2Int Position { get => this.position; set => this.position = value; }
        private Vector2Int position;
        private ObjectMemory obj;
        private int type;

        public bool SetDefaults(int type)
        {
            this.type = type;
            return false;
        }

        public virtual void OnEnterTile(Entity entity) { }
        public virtual void OnLeaveTile(Entity entity) { }

        public override int GetHashCode() => this.position.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is FieldTile tile) return this.GetHashCode() == tile.GetHashCode();
            else return base.Equals(obj);
        }

        public bool Equals(FieldTile obj) => base.Equals(obj);
    }
}