using LostKingdom.DataStructure;
using LostKingdom.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LostKingdom.IO
{
    /// <summary>
    /// 游戏存档数据类
    /// </summary>
    public class SaveData : GameData
    {
        #region 存档基本信息
        /// <summary> 存档ID </summary>
        public int SaveID { get; private set; }
        /// <summary> 存档名称 </summary>
        public string SaveName { get; set; }
        /// <summary> 游戏时长（秒） </summary>
        public float PlayTime { get; set; }
        /// <summary> 最后保存时间 </summary>
        public DateTime LastSaveTime { get; private set; }
        #endregion

        #region 游戏数据
        /// <summary> 玩家数据 </summary>
        public PlayerData PlayerData { get; private set; }
        /// <summary> 游戏进度数据 </summary>
        public GameProgressData ProgressData { get; private set; }
        #endregion

        public SaveData(int saveId, string saveName) : base($"save_{saveId}", ".sav")
        {
            SaveID = saveId;
            SaveName = saveName;
            PlayTime = 0;
            LastSaveTime = DateTime.Now;
            PlayerData = new PlayerData();
            ProgressData = new GameProgressData();
        }

        public override bool SetData<T1, T2, T3, T4, T5, T6>(int header, T1 data0, T2 data1, T3 data2, T4 data3, T5 data4, T6 data5)
        {
            try
            {
                switch (header)
                {
                    case 0: // 基本信息
                        if (data0 is string name) SaveName = name;
                        if (data1 is float time) PlayTime = time;
                        return true;
                    case 1: // 玩家数据
                        return PlayerData.SetData(data0, data1, data2);
                    case 2: // 进度数据
                        return ProgressData.SetData(data0, data1, data2);
                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                LKLog.LogError(e, null, null);
                return false;
            }
        }

        public new bool Save()
        {
            FileStream stream = null;
            BinaryWriter writer = null;
            try
            {
                if (!Directory.Exists(FilePath)) 
                    Directory.CreateDirectory(FilePath);
                
                string fullPath = Path.Combine(FilePath, FileName + Extension);
                stream = new FileStream(fullPath, FileMode.Create);
                writer = new BinaryWriter(stream, Encoding.UTF8);

                // 写入存档基本信息
                writer.Write(Version);
                writer.Write(SaveID);
                writer.Write(SaveName);
                writer.Write(PlayTime);
                writer.Write(DateTime.Now.ToBinary());

                // 写入玩家数据
                PlayerData.Serialize(writer);

                // 写入进度数据
                ProgressData.Serialize(writer);

                LastSaveTime = DateTime.Now;
                LKLog.LogMessage($"游戏存档保存完毕：{SaveName}", null, null);
                return true;
            }
            catch (Exception e)
            {
                LKLog.LogError($"保存游戏存档时出错：{SaveName}", null, null);
                LKLog.LogError(e, null, null);
                return false;
            }
            finally
            {
                writer?.Dispose();
                stream?.Dispose();
            }
        }

        public new bool Load()
        {
            FileStream stream = null;
            BinaryReader reader = null;
            try
            {
                string fullPath = Path.Combine(FilePath, FileName + Extension);
                stream = new FileStream(fullPath, FileMode.Open);
                reader = new BinaryReader(stream, Encoding.UTF8);

                // 读取存档基本信息
                string version = reader.ReadString();
                if (version != Version)
                {
                    throw new Exception($"存档版本不匹配：期望 {Version}，实际 {version}");
                }

                SaveID = reader.ReadInt32();
                SaveName = reader.ReadString();
                PlayTime = reader.ReadSingle();
                LastSaveTime = DateTime.FromBinary(reader.ReadInt64());

                // 读取玩家数据
                PlayerData = new PlayerData();
                PlayerData.Deserialize(reader);

                // 读取进度数据
                ProgressData = new GameProgressData();
                ProgressData.Deserialize(reader);

                LKLog.LogMessage($"游戏存档读取完毕：{SaveName}", null, null);
                return true;
            }
            catch (Exception e)
            {
                LKLog.LogError($"读取游戏存档时出错：{SaveName}", null, null);
                LKLog.LogError(e, null, null);
                return false;
            }
            finally
            {
                reader?.Dispose();
                stream?.Dispose();
            }
        }
    }

    /// <summary>
    /// 玩家数据
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public string PlayerName { get; set; }
        public int Level { get; set; }
        public float Experience { get; set; }
        public Dictionary<string, GameProperty> Properties { get; private set; }

        public PlayerData()
        {
            PlayerName = "Player";
            Level = 1;
            Experience = 0;
            Properties = new Dictionary<string, GameProperty>();
        }

        public bool SetData<T1, T2, T3>(T1 data1, T2 data2, T3 data3)
        {
            if (data1 is string name) PlayerName = name;
            if (data2 is int level) Level = level;
            if (data3 is float exp) Experience = exp;
            return true;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(PlayerName);
            writer.Write(Level);
            writer.Write(Experience);
            
            // 写入属性数据
            writer.Write(Properties.Count);
            foreach (var prop in Properties)
            {
                writer.Write(prop.Key);
                writer.Write(prop.Value.Name);
                writer.Write(prop.Value.BasicValue);
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            PlayerName = reader.ReadString();
            Level = reader.ReadInt32();
            Experience = reader.ReadSingle();

            // 读取属性数据
            int propCount = reader.ReadInt32();
            Properties.Clear();
            for (int i = 0; i < propCount; i++)
            {
                string key = reader.ReadString();
                string name = reader.ReadString();
                int value = reader.ReadInt32();
                Properties[key] = new GameProperty(name, value);
            }
        }
    }

    /// <summary>
    /// 游戏进度数据
    /// </summary>
    [Serializable]
    public class GameProgressData
    {
        public int CurrentChapter { get; set; }
        public int BattleRound { get; set; }
        public HashSet<string> UnlockedAchievements { get; private set; }

        public GameProgressData()
        {
            CurrentChapter = 1;
            BattleRound = 1;
            UnlockedAchievements = new HashSet<string>();
        }

        public bool SetData<T1, T2, T3>(T1 data1, T2 data2, T3 data3)
        {
            if (data1 is int chapter) CurrentChapter = chapter;
            if (data2 is int round) BattleRound = round;
            return true;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(CurrentChapter);
            writer.Write(BattleRound);

            // 写入成就数据
            writer.Write(UnlockedAchievements.Count);
            foreach (string achievement in UnlockedAchievements)
            {
                writer.Write(achievement);
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            CurrentChapter = reader.ReadInt32();
            BattleRound = reader.ReadInt32();

            // 读取成就数据
            int achievementCount = reader.ReadInt32();
            UnlockedAchievements.Clear();
            for (int i = 0; i < achievementCount; i++)
            {
                UnlockedAchievements.Add(reader.ReadString());
            }
        }
    }
} 