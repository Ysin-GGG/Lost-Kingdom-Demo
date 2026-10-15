using LostKingdom.DataStructure;
using LostKingdom.Utility;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LostKingdom.IO
{
    public class DataBuffer
    {
        private static readonly Type[] types =
        {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(string),
            typeof(char),
            typeof(bool),
            typeof(LKColor),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector2Int),
            typeof(Vector3Int),
        };

        private readonly Dictionary<Type, ICollection> register = new Dictionary<Type, ICollection>();

        /// <summary> 获取缓存区数据个数 </summary>
        /// <typeparam name="T"> 目标类型 </typeparam>
        /// <returns> 若该类型不存在于缓存区，返回-1，否则返回其缓存区数据个数 </returns>
        public int Count<T>() => this.GetBuffer<T>(false)?.Count ?? -1;

        /// <summary> 翻转缓存的序列 </summary>
        /// <typeparam name="T"> 目标类型 </typeparam>
        public DataBuffer Reverse<T>() { this.GetBuffer<T>(false)?.Reverse(); return this; }

        /// <summary> 清空缓存的序列 </summary>
        /// <typeparam name="T"> 目标类型 </typeparam>
        public DataBuffer Flush<T>() { this.GetBuffer<T>(false)?.Flush(); return this; }

        /// <summary> 存入单一数据到缓存区末端（若第一次存入则自动注册对应缓存区） </summary>
        /// <typeparam name="T"> 目标类型 </typeparam>
        /// <param name="data"> 数据 </param>
        public DataBuffer Input<T>(T data) { this.GetBuffer<T>(true).Input(data); return this; }

        /// <summary> 存入若干数据到缓存区末端（若第一次存入则自动注册对应缓存区） </summary>
        /// <typeparam name="T"> 目标类型 </typeparam>
        /// <param name="datas"> 数据数组 </param>
        public DataBuffer Input<T>(params T[] datas) { this.GetBuffer<T>(true).Input(datas); return this; }

        /// <summary> 修改缓存区指定位置的数据 </summary>
        /// <typeparam name="T"> 目标类型 </typeparam>
        /// <param name="index"> 索引 </param>
        /// <param name="newData"> 新数据 </param>
        public DataBuffer Modify<T>(int index, T newData) { this.GetBuffer<T>(false).Modify(index, newData); return this; }

        /// <summary> 从缓存区末端取出单一数据 </summary>
        /// <typeparam name="T"> 目标类型 </typeparam>
        /// <param name="datas"> 数据 </param>
        /// <returns> 是否成功取出数据 </returns>
        public bool Output<T>(out T data) => this.GetBuffer<T>(false).Output(out data);

        /// <summary> 从缓存区末端取出若干数据 </summary>
        /// <typeparam name="T"> 目标类型 </typeparam>
        /// <param name="datas"> 数据数组 </param>
        /// <param name="count"> 取出数量（负数则全取） </param>
        /// <returns> 是否成功取出数据 </returns>
        public bool Output<T>(out T[] datas, int count = -1) => this.GetBuffer<T>(false).Output(out datas, count);

        /// <summary> 从缓存区末端取出单一数据 </summary>
        /// <typeparam name="T"> 目标类型 </typeparam>
        /// <returns> 若缓存区为空，则返回类默认值，否则返回缓存区末端的数据 </returns>
        public T Output<T>() => this.GetBuffer<T>(false).Output(out T data) ? data : default(T);

        /// <summary> 从缓存区末端取出若干数据 </summary>
        /// <typeparam name="T"> 目标类型 </typeparam>
        /// <param name="count"> 取出数量（负数则全取） </param>
        /// <returns> 若缓存区为空，则返回null，否则返回缓存区末端的指定长度数据数组 </returns>
        public T[] Output<T>(int count = -1)
        {
            try
            {
                return this.GetBuffer<T>(false).Output(out T[] data) ? data : null;
            }
            catch (Exception ex)
            {
                Debug.Log("No Type: " + typeof(T).Name);
                return null;
            }
        }

        /*
        public void Register<T>()
        {
            Type type = typeof(T);
            if (this.register.ContainsKey(type))
            {
                object data = this.register[type][0];
                this.register.Remove(type);
                this.register.Add(type, new object[2] { data, write });
            }
            else this.register.Add(type, new object[] { new DataBuffer<T>(), write });
        }
        */

        private DataBuffer<T> GetBuffer<T>(bool register = false)
        {
            Type type = typeof(T);
            if (!this.register.ContainsKey(type))
                if (!register) return null;
                else this.register.Add(type, new DataBuffer<T>()); //this.Register<T>();
            return this.register[type] as DataBuffer<T>;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<Type, ICollection> kvp in this.register)
                sb.AppendLine(kvp.Key.Name + ": " + (kvp.Value as ICollection).Count);
            return sb.ToString();
        }

        public virtual void Write(BinaryWriter writer)
        {
            Bit16[] ba =
            {
                new Bit16(
                    this.register.ContainsKey(DataBuffer.types[0]) && this.Count<byte>() > 0,
                    this.register.ContainsKey(DataBuffer.types[1]) && this.Count<sbyte>() > 0,
                    this.register.ContainsKey(DataBuffer.types[2]) && this.Count<short>() > 0,
                    this.register.ContainsKey(DataBuffer.types[3]) && this.Count<ushort>() > 0,
                    this.register.ContainsKey(DataBuffer.types[4]) && this.Count<int>() > 0,
                    this.register.ContainsKey(DataBuffer.types[5]) && this.Count<uint>() > 0,
                    this.register.ContainsKey(DataBuffer.types[6]) && this.Count<long>() > 0,
                    this.register.ContainsKey(DataBuffer.types[7]) && this.Count<ulong>() > 0,
                    this.register.ContainsKey(DataBuffer.types[8]) && this.Count<float>() > 0,
                    this.register.ContainsKey(DataBuffer.types[9]) && this.Count<double>() > 0,
                    this.register.ContainsKey(DataBuffer.types[10]) && this.Count<decimal>() > 0,
                    this.register.ContainsKey(DataBuffer.types[11]) && this.Count<string>() > 0,
                    this.register.ContainsKey(DataBuffer.types[12]) && this.Count<char>() > 0,
                    this.register.ContainsKey(DataBuffer.types[13]) && this.Count<bool>() > 0
                ),
                new Bit16(
                    this.register.ContainsKey(DataBuffer.types[14]) && this.Count<LKColor>() > 0,
                    this.register.ContainsKey(DataBuffer.types[15]) && this.Count<Vector2>() > 0,
                    this.register.ContainsKey(DataBuffer.types[16]) && this.Count<Vector3>() > 0,
                    this.register.ContainsKey(DataBuffer.types[17]) && this.Count<Vector2Int>() > 0,
                    this.register.ContainsKey(DataBuffer.types[18]) && this.Count<Vector3Int>() > 0
                )
            };
            writer.Write((byte)ba.Length);
            foreach (Bit16 b in ba) writer.Write((ushort)b);
            if (ba[0][0])
            {
                writer.Write(this.Count<byte>());
                while (this.Output(out byte data)) writer.Write(data);
            }
            if (ba[0][1])
            {
                writer.Write(this.Count<sbyte>());
                while (this.Output(out sbyte data)) writer.Write(data);
            }
            if (ba[0][2])
            {
                writer.Write(this.Count<short>());
                while (this.Output(out short data)) writer.Write(data);
            }
            if (ba[0][3])
            {
                writer.Write(this.Count<ushort>());
                while (this.Output(out ushort data)) writer.Write(data);
            }
            if (ba[0][4])
            {
                writer.Write(this.Count<int>());
                while (this.Output(out int data)) writer.Write(data);
            }
            if (ba[0][5])
            {
                writer.Write(this.Count<uint>());
                while (this.Output(out uint data)) writer.Write(data);
            }
            if (ba[0][6])
            {
                writer.Write(this.Count<long>());
                while (this.Output(out long data)) writer.Write(data);
            }
            if (ba[0][7])
            {
                writer.Write(this.Count<ulong>());
                while (this.Output(out ulong data)) writer.Write(data);
            }
            if (ba[0][8])
            {
                writer.Write(this.Count<float>());
                while (this.Output(out float data)) writer.Write(data);
            }
            if (ba[0][9])
            {
                writer.Write(this.Count<double>());
                while (this.Output(out double data)) writer.Write(data);
            }
            if (ba[0][10])
            {
                writer.Write(this.Count<decimal>());
                while (this.Output(out decimal data)) writer.Write(data);
            }
            if (ba[0][11])
            {
                writer.Write(this.Count<string>());
                while (this.Output(out string data)) writer.Write(data);
            }
            if (ba[0][12])
            {
                writer.Write(this.Count<char>());
                while (this.Output(out char data)) writer.Write(data);
            }
            if (ba[0][13])
            {
                writer.Write(this.Count<bool>());
                while (this.Output(out bool data)) writer.Write(data);
            }
            if (ba[1][0])
            {
                writer.Write(this.Count<LKColor>());
                while (this.Output(out LKColor data)) writer.Write((uint)data);
            }
            if (ba[1][1])
            {
                writer.Write(this.Count<Vector2>());
                while (this.Output(out Vector2 data))
                {
                    writer.Write(data.x);
                    writer.Write(data.y);
                }
            }
            if (ba[1][2])
            {
                writer.Write(this.Count<Vector3>());
                while (this.Output(out Vector3 data))
                {
                    writer.Write(data.x);
                    writer.Write(data.y);
                    writer.Write(data.z);
                }
            }
            if (ba[1][3])
            {
                writer.Write(this.Count<Vector2Int>());
                while (this.Output(out Vector2Int data))
                {
                    writer.Write(data.x);
                    writer.Write(data.y);
                }
            }
            if (ba[1][4])
            {
                writer.Write(this.Count<Vector3Int>());
                while (this.Output(out Vector3Int data))
                {
                    writer.Write(data.x);
                    writer.Write(data.y);
                    writer.Write(data.z);
                }
            }
        }

        public virtual void Read(BinaryReader reader)
        {
            Bit16[] ba = new Bit16[reader.ReadByte()];
            for (int i = 0; i < ba.Length; i++)
                ba[i] = reader.ReadUInt16();
            if (ba[0][0])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(reader.ReadByte());
            if (ba[0][1])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(reader.ReadSByte());
            if (ba[0][2])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(reader.ReadInt16());
            if (ba[0][3])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(reader.ReadUInt16());
            if (ba[0][4])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(reader.ReadInt32());
            if (ba[0][5])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(reader.ReadUInt32());
            if (ba[0][6])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(reader.ReadInt64());
            if (ba[0][7])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(reader.ReadUInt64());
            if (ba[0][8])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(reader.ReadSingle());
            if (ba[0][9])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(reader.ReadDouble());
            if (ba[0][10])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(reader.ReadDecimal());
            if (ba[0][11])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(reader.ReadString());
            if (ba[0][12])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(reader.ReadChar());
            if (ba[0][13])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(reader.ReadBoolean());
            if (ba[1][0])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input((LKColor)reader.ReadUInt32());
            if (ba[1][1])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(new Vector2(reader.ReadSingle(), reader.ReadSingle()));
            if (ba[1][2])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
            if (ba[1][3])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(new Vector2Int(reader.ReadInt32(), reader.ReadInt32()));
            if (ba[1][4])
                for (int i = reader.ReadInt32(); i > 0; i--)
                    this.Input(new Vector3Int(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()));
        }
    }

    public class DataBuffer<T> : ICollection
    {
        private Type type = typeof(T);
        private List<T> data = new List<T>();
        public int Count => this.data.Count;

        public DataBuffer<T> Reverse() {this.data.Reverse(); return this; }
        public DataBuffer<T> Flush() { this.data.Clear(); return this; }
        public DataBuffer<T> Input(T data) { this.data.Add(data); return this; }
        public DataBuffer<T> Input(params T[] data) { this.data.AddRange(data); return this; }
        public DataBuffer<T> Modify(int index, T newData) {this.data[index] = newData; return this; }
        public T Output()
        {
            T data = default(T);
            if (this.Count > 0)
            {
                int index = this.Count - 1;
                data = this.data[index];
                this.data.RemoveAt(index);
            }
            return data;
        }

        public bool Output(out T data)
        {
            data = default(T);
            if (this.Count == 0) return false;
            int index = this.Count - 1;
            data = this.data[index];
            this.data.RemoveAt(index);
            return true;
        }

        public bool Output(out T[] data, int count = -1)
        {
            data = null;
            if (count == 0 || count > this.Count) return false;
            if (count < 0 || count == this.Count)
            {
                data = this.data.ToArray();
                this.data.Clear();
            }
            else if (count == 1)
            {
                data = new T[1] { this.Output() };
            }
            else
            {
                data = new T[count];
                int index = this.Count - count;
                this.data.CopyTo(index, data, 0, count);
                this.data.RemoveRange(index, count);
            }
            return true;
        }

        public override bool Equals(object obj) => obj is DataBuffer<T> buffer ? buffer.type == this.type : base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();

        public void CopyTo(Array array, int index) => this.data.CopyTo(array as T[], index);

        public IEnumerator GetEnumerator() => this.data.GetEnumerator();

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

    }
}