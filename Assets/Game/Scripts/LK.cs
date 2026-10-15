using LostKingdom.IO;
using LostKingdom.Manager;
using LostKingdom.Utility;
using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace LostKingdom
{
    /// <summary> 程序入口 </summary>
    [DisallowMultipleComponent]
    public sealed class LK : MonoBehaviour
    {
        #region Basic Infomation
        /// <summary> 版本信息 </summary>
        public const string versionString = "Alpha v" + LK.versionNumber;
        /// <summary> 版本号 </summary>
        public const string versionNumber = "0.0.1.2";
        #endregion

        /// <summary> 光标的屏幕坐标 </summary>
        public static Vector2 MouseScreen => new Vector2(LK.MouseX, LK.MouseY);
        /// <summary> 光标的屏幕横坐标 </summary>
        public static float MouseX => Input.mousePosition.x;
        /// <summary> 光标的屏幕纵坐标 </summary>
        public static float MouseY => Input.mousePosition.y;
        /// <summary> 预设分辨率 </summary>
        public static Vector2 PreferredScreenSize => new Vector2(LK.PreferredScreenWidth, LK.PreferredScreenHeight);
        /// <summary> 预设分辨率宽度 </summary>
        public static int PreferredScreenWidth => 1920;
        /// <summary> 预设分辨率高度 </summary>
        public static int PreferredScreenHeight => 1080;
        /// <summary> 当前分辨率 </summary>
        public static Vector2 ScreenSize => new Vector2(LK.ScreenWidth, LK.ScreenHeight);
        /// <summary> 当前分辨率宽度 </summary>
        public static int ScreenWidth => Display.main.renderingWidth; //Screen.currentResolution.width;
        /// <summary> 当前分辨率高度 </summary>
        public static int ScreenHeight => Display.main.renderingHeight; //Screen.currentResolution.height;
        /// <summary> 程序内不抛出错误 </summary>
        public static bool IgnoreError = true;
        /// <summary> 应用暂停 </summary>
        public static bool AppPause = false;
        /// <summary> 游戏暂停 </summary>
        public static bool GamePause = false;
        /// <summary> 游戏逻辑暂停 </summary>
        public static bool LogicPause = false;
        /// <summary> 游戏计时器（每秒固定+60，暂停游戏时停止计时） </summary>
        public static int GameTime = 0;
        /// <summary> 逻辑计时器（每秒固定+60）</summary>
        public static int LogicTime = 0;
        /// <summary> 逻辑计时器（每秒固定+60，并自动取余）</summary>
        public static int LogicTimeLoop = 0;
        
        /// <summary> 单例模式 </summary>
        public static LK Instance;

        /// <summary> 游戏加载 </summary>
        public static event Action OnLoad;
        /// <summary> 更新渲染帧 </summary>
        public static event Action OnUpdateGame;
        /// <summary> 更新逻辑帧 </summary>
        public static event Action OnUpdateLogic;
        /// <summary> 游戏释放 </summary>
        public static event Action OnUnload;
        /// <summary> 修改分辨率时 </summary>
        public static event Action OnScreenSizeChanged;

        private static Vector2 lastScreenSize;

        private void Awake() => this.Initialize();
        private void Start() => this.Load();
        private void Update() => this.UpdateGame();
        private void FixedUpdate() => this.UpdateLogic();
        private void OnApplicationQuit() => this.Unload();
        private void OnDestroy() => this.Dispose();

        private void Initialize()
        {
            LK.Instance = this;
            LKLocalization.Initialize();
            LKLog.LogMessage("正在加载游戏内容", null);
            //LKAsset.Initialize();
            LKLog.LogMessage("正在读取游戏配置", null);
            Preference.LoadConfiguration(@"Assets/Game/JSONs/Preference/Configuration.json");
            LKLog.LogMessage("正在读取键位映射", null);
            Preference.LoadInputProfile(@"Assets/Game/JSONs/Preference/InputProfile.json");
            Options.FramePerSecond = 60;
            this.transform.localScale = Vector3.one;
            this.gameObject.SetActive(true);
        }

        public static void Quit()
        {
            Application.Quit(1);
        }

        private void Load()
        {
            var file = new FileStream(@"Assets/Game/JSONs/Preference/Keycode.json", FileMode.Create);
            var writer = new StreamWriter(file, Encoding.UTF8);
            KeyCode[] keys = (KeyCode[])Enum.GetValues(typeof(KeyCode));
            foreach (var i in keys)
                writer.WriteLine(string.Format("\"{0}\": \"{1}\",", i.ToString(), (int)i));
            writer.Close();
            file.Close();
            LK.OnLoad?.Invoke();
        }

        private void Unload()
        {
            LK.OnUnload?.Invoke();
        }

        private void Dispose()
        {
            Preference.SaveConfiguration(@"Assets/Game/JSONs/Preference/Configuration.json");
            Preference.SaveInputProfile(@"Assets/Game/JSONs/Preference/InputProfile.json");
        }

        private void UpdateGame()
        {
            if (LK.AppPause || LK.GamePause) return;
            LK.OnUpdateGame?.Invoke();
        }

        private void UpdateLogic()
        {
            if (LK.AppPause || LK.LogicPause) return;
            if (LK.lastScreenSize != LK.ScreenSize) LK.OnScreenSizeChanged?.Invoke();
            LK.lastScreenSize = LK.ScreenSize;
            LK.OnUpdateLogic?.Invoke();
            LK.GameTime++;
            LK.LogicTime++;
            LK.LogicTimeLoop = LK.LogicTime % 60;
        }

        /// <summary> 修改帧率 </summary>
        /// <param Name="fps"> 要修改的帧率（取值范围[24, 240]） </param>
        internal static IEnumerator SetFramePerSecond(int fps)
        {
            if (!LK.AppPause)
            {
                Application.targetFrameRate = LKMath.Clamp(fps, 24, 240);
                LK.AppPause = true;
                yield return null;
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(0.1f);
                LK.AppPause = false;
            }
        }

        /// <summary> 浮动数（0-1，采用Sin函数） </summary>
        /// <param name="speed"> 浮动速率 </param>
        /// <returns> 最终浮动值 </returns>
        public static float GetFloatingSin(float speed = 1f) => .5f + LKMath.Sin(LK.LogicTime * speed) * .5f;

        /// <summary> 浮动数（0-1，采用Cos函数） </summary>
        /// <param name="speed"> 浮动速率 </param>
        /// <returns> 最终浮动值 </returns>
        public static float GetFloatingCos(float speed = 1f) => .5f + LKMath.Cos(LK.LogicTime * speed) * .5f;

        public void ContinueGame() { }
        public void NewGame() { }
        public void Settings() { }
        public void ExitGame() { Application.Quit(); }
    }
}