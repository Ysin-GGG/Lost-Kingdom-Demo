using LostKingdom.Localization;
using LostKingdom.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LostKingdom.IO
{
    /// <summary> 配置类 </summary>
    public static class Preference
    {
        /// <summary> 读取游戏配置文件 </summary>
        /// <param Name="file"> 文件名（可省略后缀，常命名为*.json） </param>
        public static void LoadConfiguration(string file = "Configuration.json")
        {
            string _file = file;
            bool initScreen = true;
            FileStream stream = null;
            StreamReader reader = null;
            JsonTextReader jreader = null;
            JToken jtoken = null;
            if (!Path.HasExtension(file)) _file += ".json";
            if (!File.Exists(_file)) Preference.SaveConfiguration(file);
            try
            {
                stream = new FileStream(_file, FileMode.Open);
                reader = new StreamReader(stream, Encoding.UTF8);
                jreader = new JsonTextReader(reader);
                jtoken = JToken.ReadFrom(jreader);
                reader.Close();
                jreader.Close();
                stream.Close();
            }
            catch
            {
                reader?.Close();
                jreader?.Close();
                stream?.Close();
                Preference.SaveConfiguration(file);
                stream = new FileStream(_file, FileMode.Open);
                reader = new StreamReader(stream, Encoding.UTF8);
                jreader = new JsonTextReader(reader);
                jtoken = JToken.ReadFrom(jreader);
                reader.Close();
                jreader.Close();
                stream.Close();
            }
            try
            {
                void AddPair(Dictionary<string, string> dict, string key, JToken jo)
                {
                    if (jo.HasValues) foreach (JToken child in jo.ToArray()) AddPair(dict, key, child);
                    else dict.Add($"{key}.{jo.Parent.Path}".Trim('.'), jo.ToString());
                }
                Dictionary<string, string> data = new Dictionary<string, string>();
                AddPair(data, file, jtoken);
                string obj1 = null;
                string obj2 = null;
                if (data.TryGetValue(string.Join(".", file, "FPS"), out obj1))
                {
                    int fps = int.Parse(obj1);
                    if (fps == 60 || fps == 120 || fps == 240) Options.FramePerSecond = fps;
                }
                if (data.TryGetValue(string.Join(".", file, "Language.ID"), out obj1)) Options.LanguageID = byte.Parse(obj1);
                if (data.TryGetValue(string.Join(".", file, "Volume.Master"), out obj1)) Options.MasterVolume = float.Parse(obj1);
                if (data.TryGetValue(string.Join(".", file, "Volume.Music"), out obj1)) Options.MusicVolume = float.Parse(obj1);
                if (data.TryGetValue(string.Join(".", file, "Volume.Ambient"), out obj1)) Options.AmbientVolume = float.Parse(obj1);
                if (data.TryGetValue(string.Join(".", file, "Volume.SFX"), out obj1)) Options.SFXVolume = float.Parse(obj1);
                if (data.TryGetValue(string.Join(".", file, "ScreenResolution.Width"), out obj1))
                    if (data.TryGetValue(string.Join(".", file, "ScreenResolution.Height"), out obj2))
                    {
                        initScreen = false;
                        Options.ScreenResolution = new Vector2(short.Parse(obj1), short.Parse(obj2));
                    }
            }
            catch (Exception e)
            {
                LKLog.LogError($"Invalid File \"{_file}\"");
                LKLog.LogError(e.Message);
                reader?.Close();
                jreader?.Close();
                stream?.Close();
                Preference.SaveConfiguration(file);
                if (initScreen) Options.ScreenResolution = Vector2.zero;
                if (File.Exists(_file)) File.Delete(_file);
                if (!LK.IgnoreError) throw e;
            }
        }

        /// <summary> 保存游戏配置文件 </summary>
        /// <param Name="file"> 文件名（可省略后缀，常命名为*.json） </param>
        public static void SaveConfiguration(string file = "Configuration.json")
        {
            if (!Path.HasExtension(file)) file += ".json";
            FileStream stream = null;
            try
            {
                stream = new FileStream(file, FileMode.Create);
                var data = new
                {
                    FPS = Options.FramePerSecond,
                    Language = new
                    {
                        ID = Options.LanguageID,
                        Name = GameCulture.GameCultureInfo[(GameCulture.CultureInfo)Options.LanguageID].key,
                        Lang = GameCulture.GameCultureInfo[(GameCulture.CultureInfo)Options.LanguageID].info,
                    },
                    ScreenResolution = new
                    {
                        Width = (int)Options.ScreenResolution.x,
                        Height = (int)Options.ScreenResolution.y,
                    },
                    Volume = new
                    {
                        Master = Options.MasterVolume,
                        Music = Options.MusicVolume,
                        Ambient = Options.AmbientVolume,
                        SFX = Options.SFXVolume,
                    }
                };
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
                writer.Write(JsonConvert.SerializeObject(data, Formatting.Indented));
                writer.Close();
                stream.Close();
            }
            catch (Exception e)
            {
                LKLog.LogError($"Invalid File \"{file}\"");
                LKLog.LogError(e.Message);
                stream?.Close();
                if (File.Exists(file)) File.Delete(file);
                if (!LK.IgnoreError) throw e;
            }
        }

        /// <summary> 读取游戏键位文件 </summary>
        /// <param Name="file"> 文件名（可省略后缀，常命名为*.json） </param>
        public static void LoadInputProfile(string file = "InputProfile.json")
        {
            string _file = file;
            if (!Path.HasExtension(file)) _file += ".json";
            if (!File.Exists(_file)) Preference.SaveInputProfile(file);
            FileStream stream = new FileStream(_file, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            JsonTextReader jreader = new JsonTextReader(reader);
            try
            {
                JToken jtoken = JToken.ReadFrom(jreader);
                object res = null;
                if (jtoken != null)
                    foreach (var token in jtoken.ToArray())
                        if (Enum.TryParse(typeof(Keymap.GameKey), token.Path, out res))
                        {
                            int id = (int)(Keymap.GameKey)res;
                            string[] keys = JsonConvert.DeserializeObject<string[]>(token.First.ToString(Formatting.None));
                            for (int i = 0; i < keys.Length && i < Keymap.MaxMultiKeymap; i++)
                                if (Enum.TryParse(typeof(KeyCode), keys[i], out res))
                                    Keymap.CurrentKeymap[id][i] = (KeyCode)res;
                        }
            }
            catch (Exception e)
            {
                LKLog.LogError($"Invalid File \"{file}\"");
                LKLog.LogError(e);
            }
            reader.Close();
            jreader.Close();
            stream.Close();
            Preference.SaveInputProfile(file);
        }

        /// <summary> 保存游戏键位文件 </summary>
        /// <param Name="file"> 文件名（可省略后缀，常命名为*.json） </param>
        public static void SaveInputProfile(string file = "InputProfile.json")
        {
            FileStream stream = null;
            StreamWriter writer = null;
            if (!Path.HasExtension(file)) file += ".json";
            if (File.Exists(file)) File.Delete(file);
            try
            {
                stream = new FileStream(file, FileMode.Create);
                writer = new StreamWriter(stream, Encoding.UTF8);
                JObject content = new JObject();
                for (int i = 0; i < Keymap.CurrentKeymap.Length; i++)
                {
                    JArray keys = new JArray();
                    for (int j = 0; j < Keymap.MaxMultiKeymap; j++)
                        keys.Add(Keymap.CurrentKeymap[i][j]);
                    content.Add(((Keymap.GameKey)i).ToString(), keys);
                }
                writer.Write(content.ToString(Formatting.Indented));
                writer.Close();
                stream.Close();
            }
            catch (Exception e)
            {
                LKLog.LogError($"Invalid File \"{file}\"");
                LKLog.LogError(e.Message);
                writer?.Close();
                stream?.Close();
                if (File.Exists(file)) File.Delete(file);
                if (!LK.IgnoreError) throw e;
            }
        }
    }
}