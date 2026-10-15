using LostKingdom.DataStructure;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UFont = UnityEngine.Font;
using UTile = UnityEngine.Tilemaps.Tile;

namespace LostKingdom.Utility
{
    /// <summary> 游戏资源 </summary>
    public static class LKAsset
    {
        private static Sprite[] UIBox = null;
        private static Sprite[] SymbolIcon = null;
        private static Sprite[] APPoint = null;
        private static AudioClip[][] GameSound_Interaction = new AudioClip[2][];
        private static AudioClip[][] GameSound_Music = null;
        private static UFont[] TextFont = null;
        private static UTile AirTile = null;
        private static UTile[] Foundation = null;
        private static UTile[] Ground = null;
        private static UTile[][][] Barrier = null;

        private static string RootPath;
        private static Comparison<string> FileComparer => (a, b) => int.Parse(Regex.Match(a, @"\d+").Value).CompareTo(int.Parse(Regex.Match(b, @"\d+").Value));

        private static UFont[] LoadFonts(string path)
        {
            List<UFont> fonts = new List<UFont>();
            string[] fontList = new string[7] { "ExtraLight", "Light", "Normal", "Regular", "Medium", "Bold", "Heavy" };
            for (int i = 0; i < fontList.Length; i++)
                fonts.Add(AssetDatabase.LoadAssetAtPath<UFont>(Path.Combine(path, $"SourceHanSansSC-{fontList[i]}.otf")));
            return fonts.ToArray();
        }

        private static Sprite[] LoadBoxes(string path)
        {
            List<Sprite> sprites = new List<Sprite>();
            string[] files = Directory.GetFiles(path, "*.png");
            Array.Sort(files, LKAsset.FileComparer);
            foreach (string file in files)
                sprites.Add(AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(path, Path.GetFileName(file))));
            return sprites.ToArray();
        }

        private static Sprite[] LoadSymbols(string path)
        {
            List<Sprite> sprites = new List<Sprite>();
            string[] files = Directory.GetFiles(path, "*.png");
            foreach (string file in files)
                if (file.Substring(path.Length).StartsWith("SymbolBank"))
                    sprites.AddRange(LKUtility.GetSlices(Path.Combine(path, Path.GetFileName(file))));
            return sprites.ToArray();
        }

        private static AudioClip[][] LoadSounds_Music(string path)
        {
            List<AudioClip> village = new List<AudioClip>();
            List<AudioClip> field = new List<AudioClip>();
            List<AudioClip> boss = new List<AudioClip>();
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
                if (file.EndsWith(".wav") || file.EndsWith(".mp3") || file.EndsWith(".ogg"))
                {
                    string name = file.Substring(path.Length);
                    if (name.StartsWith("Village")) village.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(Path.Combine(path, Path.GetFileName(file))));
                    else if (name.StartsWith("Field")) field.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(Path.Combine(path, Path.GetFileName(file))));
                    else if (name.StartsWith("Boss")) boss.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(Path.Combine(path, Path.GetFileName(file))));
                }
            return new AudioClip[][] { village.ToArray(), field.ToArray(), boss.ToArray() };
        }

        private static AudioClip[][] LoadSounds_Interaction(string path)
        {
            List<AudioClip> legacy = new List<AudioClip>();
            List<AudioClip> mouse = new List<AudioClip>();
            string[] files = Directory.GetFiles(path, "*.wav");
            foreach (string file in files)
                switch (Path.GetFileNameWithoutExtension(file).Split('_')[0])
                {
                    case "Legacy":
                        legacy.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(Path.Combine(path, Path.GetFileName(file))));
                        break;
                    case "Mouse":
                        mouse.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(Path.Combine(path, Path.GetFileName(file))));
                        break;
                }
            return new AudioClip[][] { legacy.ToArray(), mouse.ToArray() };
        }

        private static UTile[][][] LoadBarriers(string path)
        {
            const int maxSize_x = 2;
            const int maxSize_y = 1;
            UTile[][][] tiles = new UTile[maxSize_x + 1][][];
            for(int i = 1; i <= maxSize_x; i++)
            {
                tiles[i] = new UTile[maxSize_y + 1][];
                for (int j = 1; j <= maxSize_y; j++)
                {
                    string _path = Path.Combine(path, $@"{i}x{j}\Tile");
                    string[] files = Directory.GetFiles(_path, "*.asset");
                    Array.Sort(files, LKAsset.FileComparer);
                    List<UTile> list = new List<UTile>();
                    foreach (string file in files)
                        list.Add(AssetDatabase.LoadAssetAtPath<UTile>(Path.Combine(_path, Path.GetFileName(file))));
                    tiles[i][j] = list.ToArray();
                }
            }
            return tiles;
        }

        private static void LoadTiles_Foundation(string path)
        {
            string[] files = Directory.GetFiles(path, "*.asset");
            Array.Sort(files, LKAsset.FileComparer);
            List<UTile> list = new List<UTile>();
            foreach (string file in files)
                list.Add(AssetDatabase.LoadAssetAtPath<UTile>(Path.Combine(path, Path.GetFileName(file))));
            LKAsset.Foundation = list.ToArray();
        }

        private static void LoadTiles_Ground(string path)
        {
            string[] files = Directory.GetFiles(path, "*.asset");
            Array.Sort(files, LKAsset.FileComparer);
            List<UTile> list = new List<UTile>();
            foreach (string file in files)
                list.Add(AssetDatabase.LoadAssetAtPath<UTile>(Path.Combine(path, Path.GetFileName(file))));
            LKAsset.Ground = list.ToArray();
        }

        public static void Initialize()
        {
            LKAsset.RootPath = Environment.CurrentDirectory;
            LKAsset.APPoint = LKUtility.GetSlices(@"Assets\Game\Content\Images\UI\Point.png");
            LKAsset.AirTile = AssetDatabase.LoadAssetAtPath<UTile>($@"Assets\Game\Content\Images\Cell\None.asset");

            LKAsset.TextFont = LKAsset.LoadFonts(@"Assets\Game\Content\Fonts\");
            LKAsset.UIBox = LKAsset.LoadBoxes(@"Assets\Game\Content\Images\UI\Boxes\");
            LKAsset.SymbolIcon = LKAsset.LoadSymbols(@"Assets\Game\Content\Images\Symbol\");
            LKAsset.GameSound_Music = LKAsset.LoadSounds_Music(@"Assets\Game\Content\Sounds\Music\");
            LKAsset.GameSound_Interaction = LKAsset.LoadSounds_Interaction(@"Assets\Game\Content\Sounds\Interaction\");

            LKAsset.Barrier = LKAsset.LoadBarriers(@"Assets\Game\Content\Images\Cell\Barrier\");

            LKAsset.LoadTiles_Foundation(@"Assets\Game\Content\Images\Cell\Foundation\Tile\");
            LKAsset.LoadTiles_Ground(@"Assets\Game\Content\Images\Cell\Ground\Tile\");
        }
        
        #region Assets

        private class AssetBuffer<T> : BufferPool<string, T> where T : UnityEngine.Object
        {
            public AssetBuffer(int capacity, string name) : base(new Func<string, T>((path) => AssetDatabase.LoadAssetAtPath<T>(path)), capacity, name, "Asset", "Buffer") { }
            public T this[string path] => this.Get(Path.IsPathRooted(path) ? Path.GetRelativePath(LKAsset.RootPath, path) : path);
        }

        /// <summary> 贴图资源 </summary>
        public static class Image
        {
            public static Sprite[] UIBox => LKAsset.UIBox;
            public static Sprite[] SymbolIcon => LKAsset.SymbolIcon;
            public static Sprite[] APPoint => LKAsset.APPoint;

            private static AssetBuffer<Sprite> Sprites = new AssetBuffer<Sprite>(512, "Sprite");
            private static AssetBuffer<Texture> Textures = new AssetBuffer<Texture>(128, "Tex");
            private static AssetBuffer<Texture2D> Texture2Ds = new AssetBuffer<Texture2D>(128, "Tex2D");

            /// <summary> 获取Sprite </summary>
            /// <param Name="path"> 路径 </param>
            public static Sprite GetSprite(string path) => LKAsset.Image.Sprites[path];

            /// <summary> 获取Texture </summary>
            /// <param Name="path"> 路径 </param>
            public static Texture GetTexture(string path) => LKAsset.Image.Textures[path];

            /// <summary> 获取Texture2D </summary>
            /// <param Name="path"> 路径 </param>
            public static Texture2D GetTexture2D(string path) => LKAsset.Image.Texture2Ds[path];
        }

        /// <summary> 音效资源 </summary>
        public static class Sound
        {
            public static AudioClip[] Interaction_Legacy => LKAsset.GameSound_Interaction[0];
            public static AudioClip[] Interaction_Mouse => LKAsset.GameSound_Interaction[1];

            private static AssetBuffer<AudioClip> Sounds = new AssetBuffer<AudioClip>(256, "Sound");

            /// <summary> 获取AudioClip </summary>
            /// <param Name="path"> 路径 </param>
            public static AudioClip GetAudioClip(string path) => LKAsset.Sound.Sounds[path];
        }

        /// <summary> BGM资源 </summary>
        public static class Music
        {
            public static AudioClip[] Village => LKAsset.GameSound_Music[0];
            public static AudioClip[] Field => LKAsset.GameSound_Music[1];
            public static AudioClip[] Boss => LKAsset.GameSound_Music[2];

            private static AssetBuffer<AudioClip> Musics = new AssetBuffer<AudioClip>(64, "Music");
            
            /// <summary> 获取AudioClip </summary>
            /// <param Name="path"> 路径 </param>
            public static AudioClip GetAudioClip(string path) => LKAsset.Music.Musics[path];
        }

        /// <summary> 图格资源 </summary>
        public static class Cell
        {
            public static UTile None => LKAsset.AirTile;
            public static UTile[] Barrier1x1 => LKAsset.Barrier[1][1];
            public static UTile[] Barrier2x1 => LKAsset.Barrier[2][1];

            public static UTile[] Foundation => LKAsset.Foundation;
            public static UTile[] Ground => LKAsset.Ground;

            private static AssetBuffer<UTile> Tiles = new AssetBuffer<UTile>(256, "Tile");
            private static AssetBuffer<RuleTile> RuleTiles = new AssetBuffer<RuleTile>(256, "Rule Tile");

            /// <summary> 获取Tile </summary>
            /// <param Name="path"> 路径 </param>
            public static UTile GetTile(string path) => LKAsset.Cell.Tiles[path];

            /// <summary> 获取RuleTile </summary>
            /// <param Name="path"> 路径 </param>
            public static RuleTile GetRuleTile(string path) => LKAsset.Cell.RuleTiles[path];
        }

        /// <summary> 字体资源 </summary>
        public static class Font
        {
            public static UFont UIText => LKAsset.TextFont[2];
            public static UFont CombatText => LKAsset.TextFont[5];
            public static UFont BoldCombatText => LKAsset.TextFont[6];

            private static AssetBuffer<UFont> Fonts = new AssetBuffer<UFont>(64, "Font");

            public static UFont GetFont(string path) => LKAsset.Font.Fonts[path];
        }

        #endregion
    }
}