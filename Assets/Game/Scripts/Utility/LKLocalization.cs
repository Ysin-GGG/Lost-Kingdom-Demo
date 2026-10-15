using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LostKingdom.Localization;

namespace LostKingdom.Utility
{
    /// <summary> 游戏本地化 </summary>
    public static class LKLocalization
    {
        private static Dictionary<string, string> RawText => LKLocalization.Culture.Keys.Contains(Options.LanguageID) ? LKLocalization.Culture[Options.LanguageID] : new Dictionary<string, string>();
        private static Dictionary<byte, Dictionary<string, string>> Culture;

        /// <summary> 原版初始本地化 </summary>
        public static void Initialize()
        {
            LKLocalization.Culture = new Dictionary<byte, Dictionary<string, string>>();
            foreach (var culture in GameCulture.GameCultureInfo.Values) LKLocalization.LoadCulture(culture);
        }
        
        /// <summary> 读取本地化文件 </summary>
        public static Dictionary<string, string> LoadCulture(GameCulture culture)
        {
            string path = Path.Combine(@"Assets\Game\JSONs\Localization", culture.info);
            if (!Directory.Exists(path)) return null;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Queue<DirectoryInfo> queue = new Queue<DirectoryInfo>(30);
            DirectoryInfo curDir = new DirectoryInfo(path);
            while (true)
            {
                FileInfo[] files = curDir.GetFiles("*.json");
                DirectoryInfo[] dirs = curDir.GetDirectories();
                foreach (DirectoryInfo dir in dirs) queue.Enqueue(dir);
                foreach (FileInfo file in files)
                {
                    StreamReader stream = file.OpenText();
                    JsonTextReader reader = new JsonTextReader(stream);
                    JToken token = JToken.ReadFrom(reader);
                    LKLocalization.AddPair(dict, Path.GetFileNameWithoutExtension(file.Name), token);
                }
                if (queue.Count > 0) curDir = queue.Dequeue();
                else break;
            }
            if (!LKLocalization.Culture.Values.Contains(dict)) LKLocalization.Culture.Add(culture.id, dict); 
            else LKLocalization.Culture[culture.id] = dict;
            return dict;
        }

        private static void AddPair(Dictionary<string, string> dict, string key, JToken jo)
        {
            if (jo.HasValues)
            {
                JToken[] tjo = jo.ToArray();
                foreach (JToken child in tjo)
                    AddPair(dict, key, child);
            }
            else dict.Add($"{key}.{jo.Parent.Path}".Trim('.'), jo.ToString());
        }

        /// <summary> 获取第一个包含关键字的对应文本 </summary>
        /// <param Name="key"> 关键字（完整格式：文件名.关键字） </param>
        public static string GetText(string key)
        {
            if (LKLocalization.RawText == null) return key;
            // 若该key不存在，则放宽限制，查看是否有包含这个key的key
            if (!LKLocalization.RawText.Keys.Contains(key))
            {
                foreach (string tkey in LKLocalization.RawText.Keys)
                    if (tkey.Contains(key)) return LKLocalization.GetText(tkey);
                return key;
            }

            int startIndex, endIndex;
            startIndex = LKLocalization.RawText[key].IndexOf("{$");
            if (startIndex >= 0)
            {
                endIndex = LKLocalization.RawText[key].IndexOf("}", startIndex);
                if (endIndex > startIndex)
                {
                    string nkey = LKLocalization.RawText[key].Substring(startIndex, endIndex - startIndex + 1);
                    string nvalue = LKLocalization.GetText(nkey.Substring(2, nkey.Length - 3)); //替换时产生的新args将会保留
                    // 修正调用：以避免反复执行替换
                    LKLocalization.RawText[key] = LKLocalization.RawText[key].Replace(nkey, nvalue);
                }
            }
            return LKLocalization.RawText[key];
        }

        /// <summary> 获取第一个包含关键字的对应文本 </summary>
        /// <param Name="key"> 关键字（完整格式：文件名.关键字） </param>
        /// <param Name="args"> 格式化输出的对象 </param>
        public static string GetText(string key, params object[] args)
        {
            if (LKLocalization.RawText == null) return string.Format(key, args);
            // 若该key不存在，则放宽限制，查看是否有包含这个key的key
            if (!LKLocalization.RawText.Keys.Contains(key))
            {
                foreach (string tkey in LKLocalization.RawText.Keys)
                    if (tkey.Contains(key)) return LKLocalization.GetText(tkey, args);
                return string.Format(key, args);
            }

            int startIndex, endIndex;
            startIndex = LKLocalization.RawText[key].IndexOf("{$");
            if (startIndex >= 0)
            {
                endIndex = LKLocalization.RawText[key].IndexOf("}", startIndex);
                if (endIndex > startIndex)
                {
                    string nkey = LKLocalization.RawText[key].Substring(startIndex, endIndex - startIndex + 1);
                    string nvalue = LKLocalization.GetText(nkey.Substring(2, nkey.Length - 3)); //替换时产生的新args将会保留
                    // 修正调用：以避免反复执行替换
                    LKLocalization.RawText[key] = LKLocalization.RawText[key].Replace(nkey, nvalue);
                }
            }
            return string.Format(LKLocalization.RawText[key], args);
        }
    }
}