using LostKingdom.IO;

namespace LostKingdom.Interface
{
    public interface IDataSL
    {
        /// <summary> 保存数据到缓存区 </summary>
        /// <param name="buffer">缓存区</param>
        public void SaveData(DataBuffer buffer);
        /// <summary> 从缓存区载入数据 </summary>
        /// <param name="buffer">缓存区</param>
        public void LoadData(DataBuffer buffer);
    }
}