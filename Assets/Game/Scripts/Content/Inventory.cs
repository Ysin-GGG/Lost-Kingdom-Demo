using LostKingdom.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostKingdom.Content
{
    public sealed class Inventory<T> where T : IInventoryItem // Item
    {
        /// <summary> 物品栏尺寸 </summary>
        public Vector2Int Size { get; private set; }
        /// <summary> 物品栏宽度 </summary>
        public int Width { get; private set; }
        /// <summary> 物品栏高度 </summary>
        public int Height { get; private set; }
        /// <summary> 物品栏容量（宽×高） </summary>
        public int Capacity { get; private set; }
        /// <summary> 物品种类数量（相同但多堆叠算多种） </summary>
        public int Count => this.Capacity - this.queue.Count;

        private T[] items;
        private int[,] prefix;
        private int[,] inventory;
        private Vector3Int[] itemdata;
        private Queue<int> queue;
        private bool updated;

        /// <summary> 新建物品栏 </summary>
        /// <param name="width"> 物品栏宽度（格） </param>
        /// <param name="height"> 物品栏高度（格） </param>
        public Inventory(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Size = new Vector2Int(this.Width, this.Height);
            this.Capacity = this.Width * this.Height;
            this.inventory = new int[this.Width, this.Height];
            this.prefix = new int[this.Width + 1, this.Height + 1];
            this.items = new T[this.Capacity + 1];
            this.itemdata = new Vector3Int[this.Capacity + 1];
            this.queue = new Queue<int>(this.Capacity);
            for (int i = 1; i <= this.Capacity; i++)
                this.queue.Enqueue(i);
        }

        private void UpdatePrefix()
        {
            if (this.updated) return;
            this.updated = true;
            for (int i1 = 0, i2 = 1; i1 < this.Width; i1++, i2++)
                for (int j1 = 0, j2 = 1; j1 < this.Height; j1++, j2++)
                    this.prefix[i2, j2] = this.prefix[i2, j1] + this.prefix[i1, j2] - this.prefix[i1, j1] + ((this.inventory[i1, j1] > 0) ? 1 : 0);
        }

        /// <summary> 放入物品到该物品栏（自动寻位） </summary>
        /// <param name="item"> 待放入物品 </param>
        /// <returns> 是否成功放入物品 </returns>
        public bool AddItem(T item)
        {
            int x, y, z;
            if (this.CheckItem(item, out x, out y) && this.queue.TryDequeue(out z))
            {
                this.updated = false;
                this.items[z] = item;
                this.itemdata[z] = new Vector3Int(x, y, z);
                for (int i = 0; i < item.InventoryWidth; i++)
                    for (int j = 0; j < item.InventoryHeight; j++)
                        this.inventory[x + i, y + j] = z;
                return true;
            }
            return false;
        }

        /// <summary> 放入物品到该物品栏（指定坐标） </summary>
        /// <param name="item"> 待放入物品 </param>
        /// <param name="location"> 物品栏坐标（左上角为锚点） </param>
        /// <returns> 是否成功放入物品 </returns>
        public bool AddItem(T item, Vector2Int location)
        {
            int x = location.x;
            int y = location.y;
            if (x < 0 || x + item.InventoryWidth > this.Width ||
                y < 0 || y + item.InventoryHeight > this.Height)
                return false;
            if (this.queue.Count > 0)
            {
                int i1 = x + 1, i2 = i1 + item.InventoryWidth;
                int j1 = y + 1, j2 = j1 + item.InventoryHeight;
                if (this.prefix[i2, j2] - this.prefix[i1, j2] - this.prefix[i2, j1] + this.prefix[i1, j1] == 0)
                {
                    int z = this.queue.Dequeue();
                    this.items[z] = item;
                    this.itemdata[z] = new Vector3Int(x, y, z);
                    for (int i = 0; i < item.InventoryWidth; i++)
                        for (int j = 0; j < item.InventoryHeight; j++)
                            this.inventory[x + i, y + j] = z;
                    this.updated = false;
                    this.UpdatePrefix();
                    return true;
                }
            }
            return false;
        }

        /// <summary> 移除该物品栏指定坐标的物品 </summary>
        /// <param name="location"> 物品栏坐标（左上角为锚点） </param>
        /// <returns> 是否成功移除物品 </returns>
        public bool RemoveItem(Vector2Int location)
        {
            T item = this.GetItem(location);
            if (item != null)
            {
                int x = location.x;
                int y = location.y;
                int z = this.inventory[x, y];
                this.items[z] = default(T);
                this.itemdata[z] = Vector3Int.zero;
                this.queue.Enqueue(z);
                for (int i = 0; i < item.InventoryWidth; i++)
                    for (int j = 0; j < item.InventoryHeight; j++)
                        this.inventory[x + i, y + j] = 0;
                this.updated = false;
                return true;
            }
            return false;
        }

        /// <summary> 查找该物品栏内有无指定物品 </summary>
        /// <param name="item"> 指定物品 </param>
        /// <returns> 是否成功查找到物品 </returns>
        public bool HasItem(T item)
        {
            if (item != null)
                for (int i = 0; i < this.Capacity; i++)
                    if (item.Equals(this.items[i]))
                        return true;
            return false;
        }

        /// <summary> 查找该物品栏内有无指定物品 </summary>
        /// <param name="item"> 指定物品 </param>
        /// <param name="comparer"> 比较器 </param>
        /// <returns> 是否成功查找到物品 </returns>
        public bool HasItem(T item, IComparer<T> comparer)
        {
            if (item != null)
                for (int i = 0; i < this.Capacity; i++)
                    if (comparer.Compare(item, this.items[i]) == 0)
                        return true;
            return false;
        }

        /// <summary> 获取指定坐标的物品数据 </summary>
        /// <param name="location"> 物品栏坐标 </param>
        /// <returns> 目标坐标的物品，若没有物品则返回null </returns>
        public T GetItem(Vector2Int location) => this.GetItem(location.x, location.y);

        /// <summary> 获取指定坐标的物品数据 </summary>
        /// <param name="locationX"> 水平物品栏坐标 </param>
        /// <param name="locationY"> 垂直物品栏坐标 </param>
        /// <returns> 目标坐标的物品，若没有物品则返回null </returns>
        public T GetItem(int locationX, int locationY)
        {
            if (locationX < 0 || locationX >= this.Width) return default(T);
            if (locationY < 0 || locationY >= this.Height) return default(T);
            int idx = this.inventory[locationX, locationY];
            if (idx > 0) return this.items[idx];
            return default(T);
        }

        /// <summary> 检索当前物品栏是否包含某物品 </summary>
        /// <param name="item"> 要放入的物品 </param>
        /// <returns> 可放入的物品栏坐标，无法放入则返回null </returns>
        public Vector2Int? FindItem(T item)
        {
            if (item != null)
                for (int i = 1; i <= this.Capacity; i++)
                    if (item.Equals(this.items[i]))
                        return (Vector2Int)this.itemdata[i];
            return null;
        }

        /// <summary> 检索当前物品栏是否包含某物品 </summary>
        /// <param name="item"> 要放入的物品 </param>
        /// <returns> 可放入的物品栏坐标，无法放入则返回null </returns>
        public Vector2Int? FindItem(T item, IComparer<T> comparer)
        {
            if (item != null)
                for (int i = 1; i <= this.Capacity; i++)
                    if (comparer.Compare(item, this.items[i]) == 0)
                        return (Vector2Int)this.itemdata[i];
            return null;
        }

        /// <summary> 检索当前物品栏是否包含某物品 </summary>
        /// <param name="item"> 要放入的物品 </param>
        /// <returns> 可放入的物品栏坐标，无法放入则返回null </returns>
        public bool FindItem(T item, out Vector2Int location)
        {
            location = -Vector2Int.one;
            if (item != null)
                for (int i = 1; i <= this.Capacity; i++)
                    if (item.Equals(this.items[i]))
                    {
                        location = (Vector2Int)this.itemdata[i];
                        return true;
                    }
            return false;
        }

        /// <summary> 检索当前物品栏是否包含某物品 </summary>
        /// <param name="item"> 要检索的物品 </param>
        /// <param name="location"> 返回的物品栏锚点坐标，未检索到则返回(-1, -1) </param>
        /// <returns> 是否成功检索到物品 </returns>
        public bool FindItem(T item, IComparer<T> comparer, out Vector2Int location)
        {
            location = -Vector2Int.one;
            if (item != null)
                for (int i = 1; i <= this.Capacity; i++)
                    if (comparer.Compare(item, this.items[i]) == 0)
                    {
                        location = (Vector2Int)this.itemdata[i];
                        return true;
                    }
            return false;
        }

        /// <summary> 检索某物品是否可放入当前物品栏 </summary>
        /// <param name="item"> 待放入的物品 </param>
        /// <returns> 可放入的物品栏坐标，无法放入则返回null </returns>
        public Vector2Int? CheckItem(T item)
        {
            this.UpdatePrefix();
            if (item != null)
                for (int i1 = 1, i2 = item.InventoryWidth + 1; i2 <= this.Width; i1++, i2++)
                    for (int j1 = 1, j2 = item.InventoryHeight + 1; j2 <= this.Height; j1++, j2++)
                        if (this.prefix[i2, j2] - this.prefix[i1, j2] - this.prefix[i2, j1] + this.prefix[i1, j1] == 0)
                            return new Vector2Int(i2 - 1, j2 - 1);
            return null;
        }

        /// <summary> 检索某物品是否可放入当前物品栏 </summary>
        /// <param name="item"> 要放入的物品 </param>
        /// <param name="location"> 返回的物品栏坐标，无法放入则返回(-1, -1) </param>
        /// <returns> 是否成功放入物品 </returns>
        public bool CheckItem(T item, out Vector2Int location)
        {
            location = this.CheckItem(item) ?? -Vector2Int.one;
            return location.x < 0 || location.y < 0;
        }

        /// <summary> 检索某物品是否可放入当前物品栏 </summary>
        /// <param name="item"> 要放入的物品 </param>
        /// <param name="locationX"> 返回的水平物品栏坐标，无法放入则返回-1 </param>
        /// <param name="locationY"> 返回的垂直物品栏坐标，无法放入则返回-1 </param>
        /// <returns> 是否成功放入物品 </returns>
        public bool CheckItem(T item, out int locationX, out int locationY)
        {
            Vector2Int location;
            locationX = locationY = -1;
            if (this.CheckItem(item, out location))
            {
                locationX = location.x;
                locationY = location.y;
                return true;
            }
            return false;
        }

        /// <summary> 在同一物品栏内交换物品 </summary>
        /// <param name="sourceLocation"> 起始坐标 </param>
        /// <param name="destLocation"> 目标坐标 </param>
        /// <returns> 是否成功交换物品 </returns>
        public bool MoveItem(Vector2Int sourceLocation, Vector2Int destLocation)
        {
            T item = this.GetItem(sourceLocation);
            if (this.AddItem(item, destLocation))
                return this.RemoveItem(sourceLocation);
            return false;
        }

        /// <summary> 与鼠标交换或堆叠物品 </summary>
        /// <param name="location"> 物品坐标 </param>
        /// <returns> 是否成功交换或堆叠物品 </returns>
        public bool SwapItemWithMouse(Vector2Int location)
        {
            // todo
            return false;
        }

        /// <summary> 在同一物品栏内交换物品 </summary>
        /// <param name="sourceLocation"> 物品1的坐标 </param>
        /// <param name="destLocation"> 物品2的坐标 </param>
        /// <returns> 是否成功交换物品 </returns>
        public bool SwapItemWithSelf(Vector2Int sourceLocation, Vector2Int destLocation)
        {
            T a = this.GetItem(sourceLocation);
            T b = this.GetItem(destLocation);
            if (a == null) return this.MoveItem(destLocation, sourceLocation);
            if (b == null) return this.MoveItem(sourceLocation, destLocation);
            this.RemoveItem(sourceLocation);
            this.RemoveItem(destLocation);
            if (this.AddItem(a, destLocation))
                if (this.AddItem(b, sourceLocation)) return true;
                else this.RemoveItem(destLocation);
            this.AddItem(a, sourceLocation);
            this.AddItem(b, destLocation);
            return false;
        }

        /// <summary> 与其他物品栏交换物品 </summary>
        /// <param name="inventory"> 目标物品栏 </param>
        /// <param name="location"> 当前物品栏内的物品坐标 </param>
        /// <returns> 是否成功放入物品 </returns>
        public bool SwapItemWithOther(Inventory<T> inventory, Vector2Int location)
        {
            T item = this.GetItem(location);
            if (item != null && inventory.AddItem(item))
                return this.RemoveItem(location);
            return false;
        }

        /// <summary> 将物品放入到其他物品栏的指定坐标 </summary>
        /// <param name="inventory"> 目标物品栏 </param>
        /// <param name="sourceLocation"> 当前物品栏内的物品坐标 </param>
        /// <param name="destLocation"> 目标物品栏内的物品坐标 </param>
        /// <returns> 是否成功放入物品 </returns>
        public bool SwapItemWithOther(Inventory<T> inventory, Vector2Int sourceLocation, Vector2Int destLocation)
        {
            // todo
            return false;
        }
    }
}