using LostKingdom.Content;
using LostKingdom.IO;
using System;
using UnityEngine;

namespace LostKingdom.Manager
{
    [DisallowMultipleComponent]
    public sealed class InputManager : LKBehaviour
    {
        private static InputManager instance;
        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<InputManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("InputManager");
                        instance = go.AddComponent<InputManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }

        public event Action<KeyCode> OnKeyDown;
        public event Action<KeyCode> OnKeyUp;
        public event Action<Keymap.GameKey, KeyCode> OnControlAction;

        private static KeyCode[] codes;

        protected override void Awake()
        {
            base.Awake();
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            InputManager.codes = Enum.GetValues(typeof(KeyCode)) as KeyCode[];
        }

        protected override void Update()
        {
            foreach (KeyCode key in InputManager.codes)
                if (Input.GetKeyDown(key)) OnKeyDown?.Invoke(key);
                else if (Input.GetKeyUp(key)) OnKeyUp?.Invoke(key);
            for (int i = 0; i < Keymap.MaxKeymap; i++)
                for (int j = 0; j < Keymap.MaxMultiKeymap; j++)
                    if (Keymap.CurrentKeymap[i][j] != KeyCode.None)
                        if (Input.GetKeyUp(Keymap.CurrentKeymap[i][j]))
                        {
                            this.Action((Keymap.GameKey)i, Keymap.CurrentKeymap[i][j]);
                            break;
                        }
        }

        public bool Action(Keymap.GameKey gameKey, KeyCode key)
        {
            switch (gameKey)
            {
                case Keymap.GameKey.CharacterUp:
                    return false;
                case Keymap.GameKey.CharacterDown:
                    return false;
                case Keymap.GameKey.CharacterLeft:
                    return false;
                case Keymap.GameKey.CharacterRight:
                    return false;
                case Keymap.GameKey.CharacterMoveMode:
                    GridManager.SwitchMode();
                    break;
                case Keymap.GameKey.OpenInventoryUI:
                    if (UIManager.Game_CharacterInfo.Active.Value && UIManager.Game_CharacterInfo.ToggleID == 0)
                    {
                        UIManager.Game_CharacterInfo.Active = false;
                    }
                    else
                    {
                        UIManager.Game_CharacterInfo.ToggleID = 0;
                        UIManager.Game_CharacterInfo.Active = true;
                    }
                    break;
                case Keymap.GameKey.OpenSymbolUI:
                    if (UIManager.Game_CharacterInfo.Active.Value && UIManager.Game_CharacterInfo.ToggleID == 1)
                    {
                        UIManager.Game_CharacterInfo.Active = false;
                    }
                    else
                    {
                        UIManager.Game_CharacterInfo.ToggleID = 1;
                        UIManager.Game_CharacterInfo.Active = true;
                    }
                    break;
                case Keymap.GameKey.OpenMapUI:
                    UIManager.Game_FieldMap.Active = null;
                    break;
                case Keymap.GameKey.EndAction:
                    if (GameManager.InitiativeEntity is Character character) character.EndAction();
                    break;
                case Keymap.GameKey.Interaction:
                    return false;
                case Keymap.GameKey.ShowOrHideHUD:
                    UIManager.HUD_APList.Active = null;
                    UIManager.HUD_HPBar.Active = null;
                    UIManager.HUD_SymbolList.Active = null;
                    UIManager.HUD_InitiativeTrack.Active = null;
                    break;
                case Keymap.GameKey.PauseOrBack:
                    UIManager.PauseGame();
                    break;
                default:
                    return false;
            }
            this.OnControlAction?.Invoke(gameKey, key);
            return true;
        }

        public void Action(int id) => this.Action((Keymap.GameKey)id, KeyCode.None);
    }
}