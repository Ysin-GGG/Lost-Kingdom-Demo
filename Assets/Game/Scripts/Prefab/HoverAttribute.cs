using LostKingdom.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.UI;

namespace LostKingdom.Prefab
{
    public class HoverAttribute : LKBehaviour
    {
        [SerializeField]
        private Text _obj_name;
        [SerializeField]
        private Text _obj_value;

        /// <summary> 选项文本描述（仅能单行显示） </summary>
        public string Name { get => this._obj_name.text; set => this._obj_name.text = value; }

        /// <summary> 属性值（整数，若为非负数则会在前方补上加号） </summary>
        public int Value { get => this.value; set => this._obj_value.text = (this.value = value) < 0 ? value.ToString() : (value > 0 ? "+" : "±") + value; }

        private int value;

        protected override void Start()
        {
        }
    }
}
