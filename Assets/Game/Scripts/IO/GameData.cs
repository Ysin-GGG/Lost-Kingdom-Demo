using LostKingdom.ID;
using LostKingdom.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace LostKingdom.IO
{
    /// <summary> 
    /// 游戏数据包
    /// <para> 可直接作为存档保存和读取，继承此类可重写独立的数据读写 </para>
    /// <para> 后期预设中，也会作为联机之间发送的数据包 </para>
    /// </summary>
    public class GameData
    {
        /// <summary> 存储路径 </summary>
        public string FilePath { get; set; }
        /// <summary> 存储名 </summary>
        public string FileName { get; set; }
        /// <summary> 文件扩展名 </summary>
        public string Extension { get; private set; }
        /// <summary> 文件版本 </summary>
        public string Version { get; private set; }
        /// <summary> 数据包生成时间 </summary>
        public DateTime CreateTime { get; private set; }

        private Dictionary<string, LKRandom> randoms = new Dictionary<string, LKRandom>();
        private Dictionary<int, List<object>> data = new Dictionary<int, List<object>>();

        /// <summary> 新建一个空的游戏数据包 </summary>
        /// <param Name="name"> 文件名 </param>
        /// <param Name="extension"> 扩展名 </param>
        public GameData(string name, string extension = ".dat", string path = @".\Assets\Game\Saves\")
        {
            this.FilePath = path;
            this.FileName = name;
            this.Extension = extension;
            this.Version = LK.versionString;
            this.CreateTime = DateTime.Now;
        }

        /// <summary> 传入指定数据 </summary>
        /// <typeparam Name="T"> 数据类型 </typeparam>
        /// <param Name="header"> 数据包头部ID </param>
        /// <param Name="datas"> 最多传入6个数据 </param>
        /// <returns> 数据是否传入成功 </returns>
        public bool SetData<T>(int header, params T[] datas)
        {
            if (datas != null)
            {
                switch (datas.Length)
                {
                    case 1: return this.SetData<T, object, object, object, object, object>(header, datas[0], null, null, null, null, null);
                    case 2: return this.SetData<T, T, object, object, object, object>(header, datas[0], datas[1], null, null, null, null);
                    case 3: return this.SetData<T, T, T, object, object, object>(header, datas[0], datas[1], datas[2], null, null, null);
                    case 4: return this.SetData<T, T, T, T, object, object>(header, datas[0], datas[1], datas[2], datas[3], null, null);
                    case 5: return this.SetData<T, T, T, T, T, object>(header, datas[0], datas[1], datas[2], datas[3], datas[4], null);
                    case 6: return this.SetData<T, T, T, T, T, T>(header, datas[0], datas[1], datas[2], datas[3], datas[4], datas[5]);
                }
            }
            return false;
        }

        /// <summary> 传入指定数据 </summary>
        /// <typeparam Name="T1"> 数据类型1 </typeparam>
        /// <typeparam Name="T2"> 数据类型2 </typeparam>
        /// <param Name="header"> 数据包头部ID </param>
        /// <param Name="data0_a"> 数据1a </param>
        /// <param Name="data0_b"> 数据1b </param>
        /// <param Name="data1_a"> 数据2a </param>
        /// <param Name="data1_b"> 数据2b </param>
        /// <returns> 数据是否传入成功 </returns>
        public bool SetData<T1, T2>(int header, T1 data0_a, T1 data0_b, T2 data1_a, T2 data1_b)
            => this.SetData<T1, T1, T2, T2, object, object>(header, data0_a, data0_b, data1_a, data1_b, null, null);

        /// <summary> 传入指定数据 </summary>
        /// <typeparam Name="T1"> 数据类型1 </typeparam>
        /// <typeparam Name="T2"> 数据类型2 </typeparam>
        /// <param Name="header"> 数据包头部ID </param>
        /// <param Name="data0_a"> 数据1a </param>
        /// <param Name="data0_b"> 数据1b </param>
        /// <param Name="data0_c"> 数据1c </param>
        /// <param Name="data1_a"> 数据2a </param>
        /// <param Name="data1_b"> 数据2b </param>
        /// <param Name="data1_c"> 数据2c </param>
        /// <returns> 数据是否传入成功 </returns>
        public bool SetData<T1, T2>(int header, T1 data0_a, T1 data0_b, T1 data0_c, T2 data1_a, T2 data1_b, T2 data1_c)
            => this.SetData<T1, T1, T1, T2, T2, T2>(header, data0_a, data0_b, data0_c, data1_a, data1_b, data1_c);

        /// <summary> 传入指定数据 </summary>
        /// <typeparam Name="T1"> 数据类型1 </typeparam>
        /// <typeparam Name="T2"> 数据类型2 </typeparam>
        /// <typeparam Name="T3"> 数据类型3 </typeparam>
        /// <param Name="header"> 数据包头部ID </param>
        /// <param Name="data0_a"> 数据1a </param>
        /// <param Name="data0_b"> 数据1b </param>
        /// <param Name="data1_a"> 数据2a </param>
        /// <param Name="data1_b"> 数据2b </param>
        /// <param Name="data2_a"> 数据3a </param>
        /// <param Name="data2_b"> 数据3b </param>
        /// <returns> 数据是否传入成功 </returns>
        public bool SetData<T1, T2, T3>(int header, T1 data0_a, T1 data0_b, T2 data1_a, T2 data1_b, T3 data2_a, T3 data2_b)
            => this.SetData<T1, T1, T2, T2, T3, T3>(header, data0_a, data0_b, data1_a, data1_b, data2_a, data2_b);

        /// <summary> 传入指定数据 </summary>
        /// <typeparam Name="T1"> 数据类型1 </typeparam>
        /// <typeparam Name="T2"> 数据类型2 </typeparam>
        /// <param Name="header"> 数据包头部ID </param>
        /// <param Name="data0"> 数据1 </param>
        /// <param Name="data1"> 数据2 </param>
        /// <returns> 数据是否传入成功 </returns>
        public bool SetData<T1, T2>(int header, T1 data0, T2 data1)
            => this.SetData<T1, T2, object, object, object, object>(header, data0, data1, null, null, null, null);

        /// <summary> 传入指定数据 </summary>
        /// <typeparam Name="T1"> 数据类型1 </typeparam>
        /// <typeparam Name="T2"> 数据类型2 </typeparam>
        /// <typeparam Name="T3"> 数据类型3 </typeparam>
        /// <param Name="header"> 数据包头部ID </param>
        /// <param Name="data0"> 数据1 </param>
        /// <param Name="data1"> 数据2 </param>
        /// <param Name="data2"> 数据3 </param>
        /// <returns> 数据是否传入成功 </returns>
        public bool SetData<T1, T2, T3>(int header, T1 data0, T2 data1, T3 data2)
            => this.SetData<T1, T2, T3, object, object, object>(header, data0, data1, data2, null, null, null);

        /// <summary> 传入指定数据 </summary>
        /// <typeparam Name="T1"> 数据类型1 </typeparam>
        /// <typeparam Name="T2"> 数据类型2 </typeparam>
        /// <typeparam Name="T3"> 数据类型3 </typeparam>
        /// <typeparam Name="T4"> 数据类型4 </typeparam>
        /// <param Name="header"> 数据包头部ID </param>
        /// <param Name="data0"> 数据1 </param>
        /// <param Name="data1"> 数据2 </param>
        /// <param Name="data2"> 数据3 </param>
        /// <param Name="data3"> 数据4 </param>
        /// <returns> 数据是否传入成功 </returns>
        public bool SetData<T1, T2, T3, T4>(int header, T1 data0, T2 data1, T3 data2, T4 data3)
            => this.SetData<T1, T2, T3, T4, object, object>(header, data0, data1, data2, data3, null, null);

        /// <summary> 传入指定数据 </summary>
        /// <typeparam Name="T1"> 数据类型1 </typeparam>
        /// <typeparam Name="T2"> 数据类型2 </typeparam>
        /// <typeparam Name="T3"> 数据类型3 </typeparam>
        /// <typeparam Name="T5"> 数据类型4 </typeparam>
        /// <typeparam Name="T5"> 数据类型5 </typeparam>
        /// <param Name="header"> 数据包头部ID </param>
        /// <param Name="data0"> 数据1 </param>
        /// <param Name="data1"> 数据2 </param>
        /// <param Name="data2"> 数据3 </param>
        /// <param Name="data3"> 数据4 </param>
        /// <param Name="data4"> 数据5 </param>
        /// <returns> 数据是否传入成功 </returns>
        public bool SetData<T1, T2, T3, T4, T5>(int header, T1 data0, T2 data1, T3 data2, T4 data3, T5 data4)
            => this.SetData<T1, T2, T3, T4, T5, object>(header, data0, data1, data2, data3, data4, null);

        /// <summary> 传入指定数据 </summary>
        /// <typeparam Name="T1"> 数据类型1 </typeparam>
        /// <typeparam Name="T2"> 数据类型2 </typeparam>
        /// <typeparam Name="T3"> 数据类型3 </typeparam>
        /// <typeparam Name="T4"> 数据类型4 </typeparam>
        /// <typeparam Name="T5"> 数据类型5 </typeparam>
        /// <typeparam Name="T6"> 数据类型6 </typeparam>
        /// <param Name="header"> 数据包头部ID </param>
        /// <param Name="data0"> 数据1 </param>
        /// <param Name="data1"> 数据2 </param>
        /// <param Name="data2"> 数据3 </param>
        /// <param Name="data3"> 数据4 </param>
        /// <param Name="data4"> 数据5 </param>
        /// <param Name="data5"> 数据6 </param>
        /// <returns> 数据是否传入成功 </returns>
        public virtual bool SetData<T1, T2, T3, T4, T5, T6>(int header, T1 data0, T2 data1, T3 data2, T4 data3, T5 data4, T6 data5)
        {
            try
            {
                switch (header)
                {
                    default:
                        return false;
                    case DataID.LKRandomData:
                        return true;
                }
            }
            catch (Exception e)
            {
                LKLog.LogError(e, null, null);
                return false;
            }
        }

        /// <summary> 保存游戏进度到本地 </summary>
        /// <returns> 是否保存成功 </returns>
        public bool Save()
        {
            FileStream stream = null;
            BinaryWriter writer = null;
            try
            {
                if (!Directory.Exists(this.FilePath)) Directory.CreateDirectory(this.FilePath);
                stream = new FileStream(Path.Combine(this.FilePath, this.FileName + this.Extension), FileMode.Create);
                writer = new BinaryWriter(stream, Encoding.UTF8);
                this.OnSave(writer);
                LKLog.LogMessage("游戏进度保存完毕：" + this.FileName, null, null);
                return true;
            }
            catch (Exception e)
            {
                LKLog.LogError("保存游戏进度时出错：" + this.FileName, null, null);
                LKLog.LogError(e, null, null);
                return false;
            }
            finally
            {
                writer?.Dispose();
                stream?.Dispose();
            }
        }

        /// <summary> 读取本地游戏进度 </summary>
        /// <returns> 是否读取成功 </returns>
        public bool Load()
        {
            FileStream stream = null;
            BinaryReader reader = null;
            try
            {
                stream = new FileStream(Path.Combine(this.FilePath, this.FileName + this.Extension), FileMode.Open);
                reader = new BinaryReader(stream, Encoding.UTF8);
                this.OnLoad(reader);
                LKLog.LogMessage("游戏进度读取完毕：" + this.FileName, null, null);
                return true;
            }
            catch (Exception e)
            {
                LKLog.LogError("读取游戏进度时出错：" + this.FileName, null, null);
                LKLog.LogError(e, null, null);
                return false;
            }
            finally
            {
                reader?.Dispose();
                stream?.Dispose();
            }
        }

        public virtual void OnSave(BinaryWriter writer) { }

        public virtual void OnLoad(BinaryReader reader) { }
    }
}
