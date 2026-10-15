using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LostKingdom.DataStructure
{
    public class BufferPool<TKey, TValue>
    {
        public int Count { get; private set; }
        public int Capacity { get; private set; }
        public string Suffix { get; set; }
        public string Clazz { get; set; }
        public string Name { get; set; }
        public string InnerName => $"{this.Clazz}: {this.Name} {this.Suffix}";

        private Func<TKey, TValue> OnMiss;
        private LinkedList<TValue> LRU_Link;
        private Dictionary<TKey, LinkedListNode<TValue>> LRU_Dict;

        public BufferPool(Func<TKey, TValue> onMiss, int capacity, string name, string clazz = "Vanilla", string suffix = "Buffer")
        {
            this.OnMiss = onMiss;
            this.Name = name;
            this.Clazz = clazz ?? "Vanilla";
            this.Suffix = suffix ?? "Buffer";
            this.Count = 0;
            this.Capacity = capacity;
            this.LRU_Link = new LinkedList<TValue>();
            this.LRU_Dict = new Dictionary<TKey, LinkedListNode<TValue>>(capacity);
        }

        public TValue Get(TKey key)
        {
            LinkedListNode<TValue> value;
            if (this.LRU_Dict.Keys.Contains(key))
            {
                value = this.LRU_Dict[key];
                this.LRU_Link.Remove(value);
                this.LRU_Link.AddLast(value);
            }
            else
            {
                value = new LinkedListNode<TValue>(this.OnMiss(key));
                if (value is null) return default(TValue);
                this.LRU_Link.AddLast(value);
                this.LRU_Dict.Add(key, value);
                if (this.Count < this.Capacity) this.Count++;
                else
                {
                    this.LRU_Link.RemoveFirst();
                    this.LRU_Dict.Remove(key);
                }
            }
            return value.Value;
        }
    }
}