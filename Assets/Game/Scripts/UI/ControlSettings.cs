using LostKingdom.IO;
using LostKingdom.Prefab;
using UnityEngine;

namespace LostKingdom.UI
{
    [DisallowMultipleComponent]
    public sealed class ControlSettings : LKBehaviour
    {
        [SerializeField]
        private GameObject _obj_content;

        [SerializeField]
        private KeySetting _obj_keySetting;

        [SerializeField]
        private KeyControl _prefab_keyControl;

        private KeyControl[] keyControls;

        public KeySetting KeySetting => this._obj_keySetting;

        protected override void Awake()
        {
            this.keyControls = new KeyControl[Keymap.MaxKeymap];
            for (int i = 0; i < Keymap.MaxKeymap; i++)
            {
                this.keyControls[i] = KeyControl.Instantiate(this._prefab_keyControl);

                this.keyControls[i].Parent = this._obj_content;
                this.keyControls[i].Active = true;
            }
        }
        protected override void Start()
        {
            for (int i = 0; i < Keymap.MaxKeymap; i++)
                this.keyControls[i].SetDefaults(i);
        }

        internal void SetKeyControl(KeyControl keyControl, int id) => this._obj_keySetting.SetKeyControl(keyControl, id);
    }
}