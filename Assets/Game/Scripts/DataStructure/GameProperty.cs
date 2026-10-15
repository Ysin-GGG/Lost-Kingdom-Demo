using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace LostKingdom.DataStructure
{
    /// <summary> 属性值 </summary>
    [Serializable]
    public struct GameProperty
    {
        public string Name { get; set; }
        public int BasicValue { get => this.basicValue; set => this.UpdateBasicValue(value); }
        public int FinalValue { get => this.finalValue; }

        public event Action<int, int> OnFinalValueChanged;
        public event Action<int, int> OnBasicValueChanged;

        [SerializeField]
        private int basicValue;
        [SerializeField]
        private int finalValue;
        [SerializeField]
        private List<float> valueAdd;
        [SerializeField]
        private List<float> valueMul;
        private static readonly int length = Enum.GetValues(typeof(GameProperty.ValueType)).Length;

        public enum ValueType
        {
            General = 0,
            Equipment = 1,
            Buff = 2,
        }

        public GameProperty(string name, int value)
        {
            this.Name = name;
            this.basicValue = value;
            this.finalValue = value;
            this.OnFinalValueChanged = null;
            this.OnBasicValueChanged = null;
            this.valueAdd = new List<float>(GameProperty.length);
            this.valueMul = new List<float>(GameProperty.length);
            for (int i = 0; i < GameProperty.length; i++)
            {
                this.valueAdd.Add(0f);
                this.valueMul.Add(1f);
            }
        }

        /// <summary> 实例化一个空属性值 </summary>
        /// <param name="name">属性名</param>
        /// <param name="value">基础值</param>
        /// <param name="onBasicValueChanged">基础值变动时的事件处理</param>
        /// <param name="onFinalValueChanged">最终值变动时的事件处理</param>
        public GameProperty(string name, int value, Action<int, int> onBasicValueChanged, Action<int, int> onFinalValueChanged) : this(name, value)
        {
            if (onBasicValueChanged != null) this.OnBasicValueChanged += onBasicValueChanged;
            if (onFinalValueChanged != null) this.OnFinalValueChanged += onFinalValueChanged;
        }

        /// <summary> 设置乘区数值 </summary>
        /// <param name="type"> 乘区类型 </param>
        /// <param name="add"> 加算值 </param>
        /// <param name="mul"> 乘算值 </param>
        public void SetExtraValue(GameProperty.ValueType type, float add, float mul)
        {
            int style = (int)type;
            bool updated = true;
            if (this.valueAdd[style] != add) { this.valueAdd[style] = add; updated = false; }
            if (this.valueMul[style] != mul) { this.valueMul[style] = mul; updated = false; }
            if (!updated) this.UpdateFinalValue();
        }

        private void UpdateBasicValue(int value)
        {
            if (this.basicValue != value)
            {
                this.OnBasicValueChanged?.Invoke(this.basicValue, value);
                this.basicValue = value;
                this.UpdateFinalValue();
            }
        }

        private void UpdateFinalValue()
        {
            float value = this.basicValue;
            for (int i = 0; i < GameProperty.length; i++)
            {
                value += this.basicValue * this.valueAdd[i];
                value *= this.valueMul[i];
            }
            int v = Convert.ToInt32(value);
            this.OnFinalValueChanged?.Invoke(this.finalValue, v);
            this.finalValue = v;
        }

        public static GameProperty operator +(GameProperty property, int value)
        {
            property.BasicValue += value;
            return property;
        }

        public static GameProperty operator -(GameProperty property, int value)
        {
            property.BasicValue -= value;
            return property;
        }

        public static GameProperty operator *(GameProperty property, float value)
        {
            property.BasicValue = Convert.ToInt32(property.basicValue * value);
            return property;
        }

        public static GameProperty operator /(GameProperty property, float value)
        {
            property.BasicValue = Convert.ToInt32(property.basicValue * value);
            return property;
        }

        public static implicit operator int(GameProperty property) => property.finalValue;
    }
}