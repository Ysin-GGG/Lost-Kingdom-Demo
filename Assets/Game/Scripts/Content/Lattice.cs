using LostKingdom.DataStructure;
using LostKingdom.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

namespace LostKingdom.Content
{
    public struct Lattice : IEnumerable<Vector2Int>
    {
        private static int hash;
        private HashSet<Vector2Int> map;
        private int uuid;

        /// <summary> 空 </summary>
        public static Lattice Empty { get; } = new Lattice(new HashSet<Vector2Int>());

        /// <summary> 自身 </summary>
        public static Lattice Self { get; } = Lattice.Prismatic(0, true);

        /// <summary> 贴身 </summary>
        public static Lattice Melee { get; } = Lattice.Prismatic(1, false);

        public int Count => this.map.Count;

        public bool this[int x, int y]
        {
            get => this.map.Contains(new Vector2Int(x, y));
            set
            {
                if (value) this.map.Add(new Vector2Int(x, y));
                else this.map.Remove(new Vector2Int(x, y));
            }
        }

        public bool this[Vector2Int pos]
        {
            get => this.map.Contains(pos);
            set
            {
                if (value) this.map.Add(pos);
                else this.map.Remove(pos);
            }
        }

        public Lattice(BoolMap map, float pivotX = .5f, float pivotY = .5f)
        {
            int offsetX = LKMath.Floor(map.Width * pivotX.Clamp(0f, 1f));
            int offsetY = LKMath.Floor(map.Width * pivotX.Clamp(0f, 1f));
            this.uuid = Lattice.hash++;
            this.map = new HashSet<Vector2Int>();
            for (int i = 0; i < map.Width; i++)
                for (int j = 0; j < map.Height; j++)
                    if (map[i, j]) this.map.Add(new Vector2Int(offsetX - i, offsetY - j));
        }

        public Lattice(HashSet<Vector2Int> map)
        {
            this.uuid = Lattice.hash++;
            this.map = map;
        }

        /// <summary> 以自身为中心的十字形 </summary>
        /// <param name="length"> 中心向四个方向延伸的长度 </param>
        /// <param name="includeOrigin"> 是否包含中心点 </param>
        /// <param name="pivot"> 锚点 </param>
        public static Lattice Cross(byte length, bool includeOrigin = false)
        {
            HashSet<Vector2Int> map = new HashSet<Vector2Int>();
            if (includeOrigin) map.Add(new Vector2Int(0, 0));
            for (int i = 1; i <= length; i++)
            {
                map.Add(new Vector2Int(i, 0));
                map.Add(new Vector2Int(0, i));
                map.Add(new Vector2Int(-i, 0));
                map.Add(new Vector2Int(0, -i));
            }
            return new Lattice(map);
        }

        /// <summary> 以自身为中心的矩形 </summary>
        /// <param name="width"> 宽度 </param>
        /// <param name="height"> 高度 </param>
        public static Lattice Rectangle(byte width, byte height)
        {
            int offsetX = width / 2;
            int offsetY = height / 2;
            HashSet<Vector2Int> map = new HashSet<Vector2Int>();
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    map.Add(new Vector2Int(offsetX - i, offsetY - j));
            return new Lattice(map);
        }

        /// <summary> 以自身为中心的圆形 </summary>
        /// <param name="radius"> 半径 </param>
        /// <param name="includeOrigin"> 是否包含中心点 </param>
        /// <param name="pivot"> 锚点 </param>
        public static Lattice Circle(byte radius, bool includeOrigin = false)
        {
            int radius2 = radius * radius;
            HashSet<Vector2Int> map = new HashSet<Vector2Int>();
            if (includeOrigin) map.Add(new Vector2Int(0, 0));
            for (int i = 1; i <= radius; i++)
            {
                map.Add(new Vector2Int(i, 0));
                map.Add(new Vector2Int(0, i));
                map.Add(new Vector2Int(-i, 0));
                map.Add(new Vector2Int(0, -i));
                int radius3 = radius2 - i * i;
                for (int j = 1; j * j <= radius3; j++)
                {
                    map.Add(new Vector2Int(i, j));
                    map.Add(new Vector2Int(-i, j));
                    map.Add(new Vector2Int(-i, -j));
                    map.Add(new Vector2Int(i, -j));
                }
            }
            return new Lattice(map);
        }

        /// <summary> 以自身为中心的棱形 </summary>
        /// <param name="length"> 中心到四个顶点的长度 </param>
        /// <param name="includeOrigin"> 是否包含中心点 </param>
        /// <param name="pivot"> 锚点 </param>
        public static Lattice Prismatic(byte length, bool includeOrigin = false)
        {
            HashSet<Vector2Int> map = new HashSet<Vector2Int>();
            if (includeOrigin) map.Add(new Vector2Int(0, 0));
            for (int i = 1; i <= length; i++)
            {
                map.Add(new Vector2Int(i, 0));
                map.Add(new Vector2Int(0, i));
                map.Add(new Vector2Int(-i, 0));
                map.Add(new Vector2Int(0, -i));
                int length2 = length - i;
                for (int j = 1; j <= length2; j++)
                {
                    map.Add(new Vector2Int(i, j));
                    map.Add(new Vector2Int(-i, j));
                    map.Add(new Vector2Int(-i, -j));
                    map.Add(new Vector2Int(i, -j));
                }
            }
            return new Lattice(map);
        }

        /// <summary> 差 </summary>
        /// <param name="lattice1"> 区域1 </param>
        /// <param name="lattice2"> 区域2 </param>
        public static Lattice operator -(Lattice lattice1, Lattice lattice2)
        {
            lattice1.map.ExceptWith(lattice2.map);
            return lattice1;
        }

        /// <summary> 交 </summary>
        /// <param name="lattice1"> 区域1 </param>
        /// <param name="lattice2"> 区域2 </param>
        public static Lattice operator &(Lattice lattice1, Lattice lattice2)
        {
            lattice1.map.IntersectWith(lattice2.map);
            return lattice1;
        }

        /// <summary> 并 </summary>
        /// <param name="lattice1"> 区域1 </param>
        /// <param name="lattice2"> 区域2 </param>
        public static Lattice operator |(Lattice lattice1, Lattice lattice2)
        {
            lattice1.map.UnionWith(lattice2.map);
            return lattice1;
        }

        /// <summary> 异或 </summary>
        /// <param name="lattice1"> 区域1 </param>
        /// <param name="lattice2"> 区域2 </param>
        public static Lattice operator ^(Lattice lattice1, Lattice lattice2)
        {
            HashSet<Vector2Int>[] t =
            {
                new HashSet<Vector2Int>(lattice1.map),
                new HashSet<Vector2Int>(lattice1.map),
                new HashSet<Vector2Int>(lattice2.map),
                new HashSet<Vector2Int>(lattice2.map),
            };
            t[0].UnionWith(t[2]);
            t[1].IntersectWith(t[3]);
            t[0].ExceptWith(t[1]);
            lattice1.map = t[0];
            return lattice1;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Vector2Int pos in this.map)
                sb.AppendLine(pos.ToString());
            return sb.ToString();
        }

        public override int GetHashCode() => this.uuid;

        public override bool Equals(object obj) => obj is Lattice && this.Equals((Lattice)obj);

        public bool Equals(Lattice lattice) => this.uuid == lattice.uuid;

        public IEnumerator<Vector2Int> GetEnumerator() => this.map.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.map.GetEnumerator();


    }
}