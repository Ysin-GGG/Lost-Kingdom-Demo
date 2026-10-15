using LostKingdom.Components;
using UnityEngine;
using UnityEngine.UI;

namespace LostKingdom.UI
{
    [DisallowMultipleComponent]
    public sealed class AudioSettings : LKBehaviour
    {
        [SerializeField]
        private Slider[] _obj_volumes;
        [SerializeField]
        private UIText[] _obj_texts;

        protected override void Start()
        {
            for (int i = 0; i < _obj_volumes.Length; i++)
            {
                this._obj_volumes[i].minValue = 0f;
                this._obj_volumes[i].maxValue = 1f;
            }
            this._obj_volumes[0].value = Options.MasterVolume;
            this._obj_volumes[1].value = Options.MusicVolume;
            this._obj_volumes[2].value = Options.AmbientVolume;
            this._obj_volumes[3].value = Options.SFXVolume;
            
        }

        public void UpdateVolume(int id)
        {
            float value = this._obj_volumes[id].value;
            this._obj_texts[id].HoverText = $"{(int)(value * 100)}%";
            switch (id)
            {
                case 0:
                    Options.MasterVolume = value;
                    return;
                case 1:
                    Options.MusicVolume = value;
                    return;
                case 2:
                    Options.AmbientVolume = value;
                    return;
                case 3:
                    Options.SFXVolume = value;
                    return;
            }
        }
    }
}