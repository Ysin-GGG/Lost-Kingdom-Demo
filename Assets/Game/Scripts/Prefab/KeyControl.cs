using LostKingdom.IO;
using LostKingdom.Manager;
using LostKingdom.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace LostKingdom.Prefab
{
    public class KeyControl : LKBehaviour
    {
        public string Description { get; set; } = null;

        public string Name { get => this._obj_name.text; set => this._obj_name.text = value; }

        public Keymap.GameKey GameKey { get; internal set; }

        public KeyCode KeyboardKey1 { get => this.keys[0]; set => this.SetKey(0, value); }

        public KeyCode KeyboardKey2 { get => this.keys[1]; set => this.SetKey(1, value); }

        public KeyCode GamepadKey1 { get => this.keys[2]; set => this.SetKey(2, value); }

        public KeyCode GamepadKey2 { get => this.keys[3]; set => this.SetKey(3, value); }

        public KeyCode this[int id] { get => this.keys[id]; set => this.SetKey(id, value); }

        [SerializeField]
        private Text _obj_name;

        [SerializeField]
        private Text[] _obj_keys;

        private KeyCode[] keys = new KeyCode[Keymap.MaxMultiKeymap] { KeyCode.None, KeyCode.None, KeyCode.None, KeyCode.None };

        protected override object HoverItem => this.Description;

        public void SetDefaults(int type) => this.SetDefaults((Keymap.GameKey)type);

        public void SetDefaults(Keymap.GameKey gameKey)
        {
            this.GameKey = gameKey;
            this.name = "Key Control - " + this.GameKey.ToString();
            this.Name = LKLocalization.GetText("Keymap.Name." + this.GameKey.ToString());
            KeyCode[] keys = Keymap.GetKeymapGroup(this.GameKey);
            this.SetKeys(keys[0], keys[1], keys[2], keys[3]);
        }

        /// <summary> 设置游戏键位的按键映射 </summary>
        /// <param name="id">键位编号（0-3）</param>
        /// <param name="key">新键位</param>
        /// <returns>键位应显示的文本</returns>
        public string SetKey(int id, KeyCode key)
        {
            string name = key == KeyCode.None ? "(None)" : key.ToString();
            this.keys[id] = key;
            Keymap.SetKeymapGroup(this.GameKey, this.KeyboardKey1, this.KeyboardKey2, this.GamepadKey1, this.GamepadKey2);
            return this._obj_keys[id].text = name;
        }

        /// <summary> 设置游戏键位的按键映射 </summary>
        /// <param Name="keyboard1"> 键鼠按键1（null则不改变） </param>
        /// <param Name="keyboard2"> 键鼠按键2（null则不改变） </param>
        /// <param Name="keyboard1"> 手柄按键1（null则不改变） </param>
        /// <param Name="keyboard2"> 手柄按键2（null则不改变） </param>
        public void SetKeys(KeyCode? keyboard1 = null, KeyCode? keyboard2 = null, KeyCode? gamepad1 = null, KeyCode? gamepad2 = null)
        {
            if (keyboard1.HasValue) this.SetKey(0, keyboard1.Value);
            if (keyboard2.HasValue) this.SetKey(1, keyboard2.Value);
            if (gamepad1.HasValue) this.SetKey(2, gamepad1.Value);
            if (gamepad2.HasValue) this.SetKey(3, gamepad2.Value);
        }

        public void SetKeyControl(int id) => UIManager.Settings_Control.SetKeyControl(this, id);
    }
}
