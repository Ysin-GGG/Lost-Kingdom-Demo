using LostKingdom.Manager;
using LostKingdom.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.UI;

namespace LostKingdom.Prefab
{
    public class ChatOption : LKBehaviour
    {
        /// <summary> 选项文本描述（仅能单行显示） </summary>
        public string Description { get => this._obj_text.text; set => this._obj_text.text = value; }

        /// <summary> 选项内部编号（即该选项为第index+1个选项） </summary>
        public int Index { get; internal set; }

        private TMP_Text _obj_text;

        protected override void Awake() => this._obj_text = this.GetComponentInChildren<TMP_Text>();

        public void SetDefaults(int index)
        {
            this.Index = index;
            this.Active = true;
        }

        /// <summary> 点击事件 </summary>
        public virtual void OnClick() => UIManager.Game_ChatWindow.SelectOption(this.Index);
    }
}
