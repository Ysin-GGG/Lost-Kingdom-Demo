namespace LostKingdom.Interface
{
    public interface IInventoryItem
    {
        /// <summary> 物品栏占位宽度 </summary>
        public int InventoryWidth { get; }
        /// <summary> 物品栏占位高度 </summary>
        public int InventoryHeight { get; }
        /// <summary> 物品栏旋转次数 </summary>
        public int InventoryRotation { get; set; }
        /// <summary> 物品栏锁定 </summary>
        public int InventoryLock { get; set; }
    }
}