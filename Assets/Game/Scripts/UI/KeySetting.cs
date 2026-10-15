using LostKingdom.IO;
using LostKingdom.Manager;
using LostKingdom.Prefab;
using LostKingdom.Utility;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LostKingdom.UI
{
    [DisallowMultipleComponent]
    public sealed class KeySetting : LKBehaviour
    {
        [SerializeField]
        private Text _obj_tip;

        [SerializeField]
        private Text _obj_oldKey;

        [SerializeField]
        private Text _obj_newKey;

        private KeyControl target;

        private int id;

        private bool reading;

        [SerializeReference]
        private KeyCode newKey;

        public KeyCode OldKey => this.target[this.id];

        public KeyCode NewKey { get => this.newKey; set => this._obj_newKey.text = (this.newKey = value).ToString(); }

        private static string[] keymapNames = new string[Keymap.MaxMultiKeymap] { "Keyboard 1", "Keyboard 2", "Gamepad 1", "Gamepad 2" };
        private static KeyCode[] keys = Enum.GetValues(typeof(KeyCode)) as KeyCode[];

        internal void SetKeyControl(KeyControl keyControl, int id)
        {
            this.id = id;
            this.target = keyControl;
            this.newKey = this.target[id];
            this._obj_oldKey.text = this.target[id].ToString();
            this._obj_newKey.text = this.target[id].ToString();
            this._obj_tip.text = LKLocalization.GetText("UIText.Description.KeySettingTip", this.target.Name, KeySetting.keymapNames[this.id]);
            this.Active = true;
        }

        protected override void Update()
        {
            if (this.reading && Input.anyKeyDown)
            {
                this.reading = false;
                foreach (KeyCode key in KeySetting.keys)
                    if (Input.GetKeyDown(key) && key != KeyCode.Mouse0)
                    {
                        this.NewKey = key;
                        break;
                    }
            }
        }

        public void Replace() => this.reading = true;

        public void Clear() => this.NewKey = KeyCode.None;

        public void Reset() => this.NewKey = Keymap.DefaultKeymap[(int)this.target.GameKey][id];

        public void Save()
        {
            this.target[id] = this.newKey;
            this.Close();
        }

        public void Close()
        {
            this.Active = false;
            UIManager.IsHoverTextShowing = false;
            UIManager.Game_HoverText.Active = false;
        }
    }
}