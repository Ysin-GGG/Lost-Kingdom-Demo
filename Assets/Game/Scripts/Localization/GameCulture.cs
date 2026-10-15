using System.Collections.Generic;

namespace LostKingdom.Localization
{
    public class GameCulture
    {
        public byte id;
        public string info;
        public string key;

        public enum CultureInfo
        {
            Unknown = 0,
            English = 1,
            Chinese = 2,
        }

        public static readonly Dictionary<CultureInfo, GameCulture> GameCultureInfo = new Dictionary<CultureInfo, GameCulture>()
        {
            {
                CultureInfo.English,
                new GameCulture(1, "en-US", "English")
            },
            {
                CultureInfo.Chinese,
                new GameCulture(2, "zh-Hans", "简体中文")
            },
        };

        public GameCulture(byte id, string info, string nameKey = null)
        {
            this.id = id;
            this.info = info;
            this.key = nameKey ?? "UserInterface.Options.Language.Unknown";
        }

        public override string ToString()
        {
            return $"{this.key}[{this.id}: {this.info}]";
        }
    }
}