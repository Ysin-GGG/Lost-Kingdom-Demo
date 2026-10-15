using LostKingdom.UI;
using LostKingdom.Utility;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using AudioSettings = LostKingdom.UI.AudioSettings;
using CharacterInfo = LostKingdom.UI.CharacterInfo;

namespace LostKingdom.Manager
{
    [DisallowMultipleComponent]
    public sealed class UIManager : LKBehaviour
    {
        [SerializeField]
        private Canvas _game_CharacterInfo;
        [SerializeField]
        private Canvas _game_ChatWindow;
        [SerializeField]
        private Canvas _game_SymbolCDPool;
        [SerializeField]
        private Canvas _game_HoverTooltip;
        [SerializeField]
        private Canvas _game_HoverText;
        [SerializeField]
        private Canvas _game_GridInventory;
        [SerializeField]
        private Canvas _game_FieldMap;
        [SerializeField]
        private Canvas _hud;
        [SerializeField]
        private Canvas _settings;
        [SerializeField]
        private Canvas _loadingScreen;

        /// <summary> 角色界面 </summary>
        internal static CharacterInfo Game_CharacterInfo { get; private set; }

        /// <summary> 对话窗 </summary>
        internal static ChatWindow Game_ChatWindow { get; private set; }

        /// <summary> 对话窗 </summary>
        internal static SymbolCDPool Game_SymbolCDPool { get; private set; }

        /// <summary> 悬浮文本 </summary>
        internal static HoverTooltip Game_HoverTooltip { get; private set; }

        /// <summary> 悬浮文本 </summary>
        internal static HoverText Game_HoverText { get; private set; }

        /// <summary> 网格背包 </summary>
        internal static GridInventory Game_GridInventory { get; private set; }

        /// <summary> 场景地图 </summary>
        internal static FieldMap Game_FieldMap { get; private set; }

        /// <summary> 技能栏 </summary>
        internal static SymbolList HUD_SymbolList { get; private set; }

        /// <summary> 行动条 </summary>
        internal static APList HUD_APList { get; private set; }

        /// <summary> 血条 </summary>
        internal static HPBar HUD_HPBar { get; private set; }

        /// <summary> 血条 </summary>
        internal static InitiativeTrack HUD_InitiativeTrack { get; private set; }

        /// <summary> 通用设置 </summary>
        internal static GeneralSettings Settings_General { get; private set; }
        /// <summary> 图像设置 </summary>
        internal static GraphicSettings Settings_Graphic { get; private set; }
        /// <summary> 音频设置 </summary>
        internal static AudioSettings Settings_Audio { get; private set; }
        /// <summary> 游戏设置 </summary>
        internal static GameplaySettings Settings_Gameplay { get; private set; }
        /// <summary> UI设置 </summary>
        internal static UISettings Settings_UI { get; private set; }
        /// <summary> 键位设置 </summary>
        internal static ControlSettings Settings_Control { get; private set; }

        /// <summary> 显示悬浮文本所需时长 </summary>
        public static int HoverDelay { get; set; } = 15;

        /// <summary> 显示悬浮文本所需时长的计时器 </summary>
        public static int HoverDelayTimer { get; set; } = 0;

        /// <summary> 悬浮文本是否显示 </summary>
        public static bool IsHoverTextShowing { get; set; } = false;

        /// <summary> 悬浮文本是否跟随鼠标移动 </summary>
        public static bool IsHoverTextMoving { get; set; } = false;

        /// <summary> 加载界面 </summary>
        internal static LoadingScreen LoadingScreen { get; private set; }

        private static List<Toggle> toggles;
        private static UIManager instance;
        public static UIManager Instance => instance;

        protected override void Awake()
        {
            UIManager.instance = this;
            UIManager.Game_CharacterInfo = this._game_CharacterInfo.GetComponent<CharacterInfo>();
            UIManager.Game_ChatWindow = this._game_ChatWindow.GetComponent<ChatWindow>();
            UIManager.Game_SymbolCDPool = this._game_SymbolCDPool.GetComponent<SymbolCDPool>();
            UIManager.Game_HoverTooltip = this._game_HoverTooltip.GetComponent<HoverTooltip>();
            UIManager.Game_HoverText = this._game_HoverText.GetComponent<HoverText>();
            UIManager.Game_GridInventory = this._game_GridInventory.GetComponent<GridInventory>();
            UIManager.Game_FieldMap = this._game_FieldMap.GetComponent<FieldMap>();
            UIManager.HUD_SymbolList = this._hud.GetComponentInChildren<SymbolList>();
            UIManager.HUD_APList = this._hud.GetComponentInChildren<APList>();
            UIManager.HUD_HPBar = this._hud.GetComponentInChildren<HPBar>();
            UIManager.HUD_InitiativeTrack = this._hud.GetComponentInChildren<InitiativeTrack>();
            UIManager.Settings_General = this._settings.GetComponentInChildren<GeneralSettings>();
            UIManager.Settings_Graphic = this._settings.GetComponentInChildren<GraphicSettings>();
            UIManager.Settings_Audio = this._settings.GetComponentInChildren<AudioSettings>();
            UIManager.Settings_Gameplay = this._settings.GetComponentInChildren<GameplaySettings>();
            UIManager.Settings_UI = this._settings.GetComponentInChildren<UISettings>();
            UIManager.Settings_Control = this._settings.GetComponentInChildren<ControlSettings>();
            UIManager.LoadingScreen = this._loadingScreen.GetComponent<LoadingScreen>();
            UIManager.toggles = new List<Toggle>(this._settings.GetComponentsInChildren<Toggle>());
        }

        protected override void Start()
        {
            UIManager.Game_HoverText.Active = false;
            UIManager.Game_ChatWindow.Active = false;
            if (UIManager.Game_ChatWindow.Active.Value) this._hud.gameObject.SetActive(false);
            else if (!this._hud.GetActive()) this._hud.gameObject.SetActive(true);
        }

        protected override void FixedUpdate()
        {
            if (UIManager.IsHoverTextShowing) UIManager.Game_HoverText.Active = true;
            else UIManager.HoverDelayTimer = 0;
        }

        public static void PauseGame()
        {
            if (UIManager.Game_SymbolCDPool.Active.Value)
                UIManager.Game_SymbolCDPool.Active = false;
            else if (UIManager.Settings_Control.KeySetting.Active.Value)
                UIManager.Settings_Control.KeySetting.Close();
            else if (UIManager.instance._settings.gameObject.GetActive())
                UIManager.instance._settings.gameObject.SetActive(false);
            else
            {
                UIManager.instance._settings.gameObject.SetActive(true);
                UIManager.toggles[0].isOn = true;
            }
        }

        public void EnableMainUI()
        {
            if (this._game_CharacterInfo != null) this._game_CharacterInfo.gameObject.SetActive(true);
            if (this._game_HoverTooltip != null) this._game_HoverTooltip.gameObject.SetActive(true);
            if (this._game_ChatWindow != null) this._game_ChatWindow.gameObject.SetActive(true);
            if (this._hud != null) this._hud.gameObject.SetActive(true);
        }
    }
}