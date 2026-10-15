using LostKingdom.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostKingdom.DataStructure
{
    /// <summary> 
    /// 对象池基类
    /// <para>可通过直接实例化或继承实现对象池 </para>
    /// <para>对象数组在新建时为空，需要自己手动初始化或添加初始化委托 </para>
    /// </summary>
    public class ObjectPool<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>
    {
        /// <summary> 对象池名称 </summary>
        public string Name { get; set; }
        /// <summary> 活跃对象数 </summary>
        public int Count { get; private set; }
        /// <summary> 对象池容量 </summary>
        public int Capacity { get; private set; }

        /// <summary> 对象数组 </summary>
        private T[] Obj;
        /// <summary> 对象状态记录器 </summary>
        private Dictionary<T, int> state;
        /// <summary> 对象存活计时器 </summary>
        private Dictionary<T, int> timer;
        /// <summary> 活跃对象枚举器 </summary>
        private IEnumerator<T> enumerator;
        /// <summary> 对象数组是否已变动 </summary>
        private bool changed;
        /// <summary> 有效对象数 </summary>
        private int length;
        /// <summary> 等待释放的对象 </summary>
        private Queue<T> releaseQueue;

        /// <summary> 激活对象时执行 </summary>
        public event Action<int, T> OnRequest;
        /// <summary> 停用对象时执行 </summary>
        public event Action<int, T> OnRelease;

        /// <summary> 创建对象池实例 </summary>
        /// <param Name="create"> 初始化委托，传入null则会调用Create方法，用于填充对象池（返回的必须是不同的实例） </param>
        /// <param Name="capacity"> 最大对象容量 </param>
        /// <param Name="name"> 对象池名称 </param>
        public ObjectPool(Func<int, T> create, int capacity = 32, string name = "Vanilla: Object Pool")
        {
            this.Count = 0;
            this.Name = name;
            this.Capacity = capacity;
            this.Obj = new T[capacity];
            this.state = new Dictionary<T, int>(capacity);
            this.timer = new Dictionary<T, int>(capacity);
            this.releaseQueue = new Queue<T>(capacity);
            this.changed = true;
            if (create == null) create = this.Create;
            for (int i = 0; i < capacity; i++) this.state.Add(this.Obj[i] = create.Invoke(i), -1);
            LK.OnUpdateLogic += this.UpdateObjectLife;
            this.OnRequest += this.OnActivate;
            this.OnRelease += this.OnDeactivate;
        }

        /// <summary> 获取空闲对象 </summary>
        private T Activate(int time)
        {
            /*
            激活对象时，将数组中第一个停用对象（即最后一个激活对象的后一个对象）激活即可
            更新该激活对象的编号（WhoAmI）和生命周期（TimeLeft）
            */
            if (this.Count >= this.Capacity) throw new InvalidOperationException("没有可激活的对象");
            this.changed = true;
            this.state[this.Obj[this.Count]] = this.Count;
            if (time > 0) this.timer.Add(this.Obj[this.Count], time);
            return this.Obj[this.Count];
        }

        /// <summary> 释放活跃对象 </summary>
        private T Deactivate(T obj)
        {
            /* 
            停用对象时，该对象会与最后一个有效对象交换
            需要配合哈希表（或字典）记录所有对象的编号（WhoAmI）和生命周期（TimeLeft）并保持更新
            */
            if (this.Count <= 0) throw new InvalidOperationException("没有可停用的对象");
            if (obj == null) throw new ArgumentNullException("obj", "对象不能为空");
            if (!this.Contains(obj)) throw new ArgumentException("该对象不属于该对象池", "obj");
            this.changed = true;
            T swap = this.Obj[--this.Count];
            if (!swap.Equals(obj)) // 若待交换的是同一个对象，则跳过交换操作
            {
                int l = this.state[obj];
                int r = this.state[swap];
                this.Obj[l] = swap;
                this.Obj[r] = obj;
                this.state[swap] = l;
            }
            this.state[obj] = -1;
            return obj;
        }

        /// <summary> 获取空闲对象后预处理 </summary>
        protected virtual void OnActivate(int id, T obj) { }

        /// <summary> 释放活跃对象后预处理 </summary>
        protected virtual void OnDeactivate(int id, T obj) { }

        /// <summary> 获取一个空闲对象并设置其生命周期 </summary>
        /// <param Name="timer"> 生命周期（逻辑帧），非正整数则无限制 </param>
        public T Request(int time = 0)
        {
            T obj = this.Activate(time);
            this.OnRequest?.Invoke(this.Count++, obj);
            return obj;
        }

        /// <summary> 释放对象并停用 </summary>
        /// <param Name="obj"> 目标对象 </param>
        /// <param Name="immediately"> 立即释放 </param>
        public T Release(T obj, bool immediately = true)
        {
            if (immediately)
            {
                this.Deactivate(obj);
                this.OnRelease(this.Count, obj);
            }
            else this.releaseQueue.Enqueue(obj);
            return obj;
        }

        /// <summary> 释放并停用所有活跃对象 </summary>
        public int Clear()
        {
            int count = this.Count;
            if (count > 0)
            {
                this.changed = true;
                while (this.Count > 0)
                {
                    T obj = this.Obj[--this.Count];
                    this.state[obj] = -1;
                    this.OnRelease(this.Count, obj);
                    this.timer.Remove(obj);
                }
            }
            return count;
        }

        public T Find(Predicate<T> match) => Array.Find(this.Obj, match);

        /// <summary> 输出对象池的状态 </summary>
        /// <param Name="full"> 输出所有对象 </param>
        public string ToString(bool full)
        {
            StringBuilder sb = new StringBuilder(this.Name);
            sb.Append(" {");
            if (full)
                foreach (var t in this.Obj)
                {
                    var state = this.state[t];
                    sb.Append(state < 0 ? " (-)" : $" [{state}]");
                }
            else
                foreach (var t in this)
                {
                    var state = this.state[t];
                    sb.Append(state < 0 ? " (-)" : $" [{state}]");
                }
            sb.Append(" }");
            return sb.ToString();
        }

        /// <summary>
        /// 默认创建对象函数
        /// <para> 重写该函数以在创建对象时对其初始化 </para>
        /// </summary>
        /// <param Name="flag"> 当前要操作对象的下标，从数组末尾开始执行 </param>
        protected virtual T Create(int index) => default(T);

        /// <summary> 复制活跃对象到数组中  </summary>
        /// <param Name="array"> 目标数组 </param>
        /// <param Name="arrayIndex"> 目标数组起始下标 </param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            int cnt = this.Count;
            int i = arrayIndex, j = 0;
            while (cnt-- > 0) array[i++] = this.Obj[j++];
        }

        /// <summary> 刷新活跃对象的生命周期  </summary>
        private void UpdateObjectLife()
        {
            while (this.releaseQueue.Count > 0)
                this.Release(this.releaseQueue.Dequeue(), true);
            for (int i = 0; i < this.timer.Count; i++)
            {
                T key = this.timer.Keys.ElementAt(i);
                if (this.timer[key] > 0) this.timer[key]--;
                else
                {
                    LKLog.LogMessage($"A lifetime of object has been ended (From: {key.ToString()})", "System");
                    this.Release(key, true);
                    this.timer.Remove(key);
                }
            }
        }

        /// <summary> 释放并返回该对象 </summary>
        /// <param Name="obj">目标对象</param>
        public T this[T obj] => this.Release(obj);

        /// <summary> 请求并返回一个对象 </summary>
        /// <param Name="value"> 对象池 </param>
        public static implicit operator T(ObjectPool<T> value) => value.Request();

        public bool IsReadOnly => ((ICollection<T>)Obj).IsReadOnly;

        int ICollection<T>.Count => this.Count;

        bool ICollection<T>.IsReadOnly => throw new NotImplementedException();

        void ICollection<T>.Add(T item) => throw new NotImplementedException();

        void ICollection<T>.Clear() => throw new NotImplementedException();

        bool ICollection<T>.Contains(T item) => this.state.Keys.Contains(item);

        bool ICollection<T>.Remove(T item) => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => this.enumerator;

        public IEnumerator<T> GetEnumerator()
        {
            if (this.changed)
            {
                var t = new T[this.Count];
                this.CopyTo(t, 0);
                this.enumerator = ((ICollection<T>)t).GetEnumerator();
                this.changed = false;
            }
            else this.enumerator.Reset();
            return this.enumerator;
        }
    }
}