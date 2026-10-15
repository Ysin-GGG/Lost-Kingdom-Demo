using LostKingdom.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace LostKingdom.Content
{
    /// <summary> 资源值 </summary>
    [Serializable]
    public struct GameValue
    {
        /// <summary> 值（修改时不会超过值的上下限） </summary>
        public int Value
        {
            get => this.value;
            set
            {
                value = value.Clamp(this.valueMin, this.valueMax);
                if (this.value != value)
                {
                    this.OnValueChanged?.Invoke(this.value, value);
                    this.value = value;
                    this.UpdatePercent();
                }
            }
        }
        /// <summary> 值下限（修改时不会超过值上限） </summary>
        public int ValueMin { get => this.valueMin; set => this.UpdateValue(value, this.valueMax); }
        /// <summary> 值上限（修改时不会超过值下限） </summary>
        public int ValueMax { get => this.valueMax; set => this.UpdateValue(this.valueMin, value); }
        /// <summary> 上下限变动时对原值的处理 </summary>
        public UpdateType ValueUpdateType { get; set; }

        public event Action<int, int> OnValueChanged;

        [SerializeField]
        private int value;
        [SerializeField]
        private int valueMin;
        [SerializeField]
        private int valueMax;
        private float percent1;
        private double percent2;

        public enum UpdateType
        {
            /// <summary> 维持值不变（如果值范围不在上下限内，则会自动改为相应的最值） </summary>
            KeepValue = 0,
            /// <summary> 维持值的百分比不变 </summary>
            KeepPercent = 1,
            /// <summary> 根据上下限变动的差值对值进行固定值调整 </summary>
            KeepDifference = 2,
        }

        public GameValue(int value, int min, int max, GameValue.UpdateType type = GameValue.UpdateType.KeepValue)
        {
            this.ValueUpdateType = type;
            this.value = value.Clamp(min, max);
            this.valueMin = min;
            this.valueMax = max;
            this.percent1 = 0f;
            this.percent2 = 0d;
            this.OnValueChanged = null;
            this.UpdatePercent();
        }

        public GameValue(int value, int min, int max, Action<int, int> onValueChanged, GameValue.UpdateType type = GameValue.UpdateType.KeepValue) : this(value, min, max, type)
        {
            if (onValueChanged != null) this.OnValueChanged += onValueChanged;
        }

        private void UpdateValue(int min, int max)
        {
            bool updated = false;
            int oldValue = this.value;
            if (this.valueMin != min || this.valueMax != max) updated = false;
            if (!updated)
            {
                switch (this.ValueUpdateType)
                {
                    case UpdateType.KeepValue:
                        this.value = LKMath.Clamp(this.value, min, max);
                        break;
                    case UpdateType.KeepPercent:
                        this.value = LKMath.Clamp((int)(min + (max - min) * this.percent2), min, max);
                        break;
                    case UpdateType.KeepDifference:
                        this.value = LKMath.Clamp(this.value + (max - this.ValueMax) - (min - this.ValueMin), min, max);
                        break;
                }
                this.valueMin = min.Min(this.valueMax);
                this.valueMax = max.Max(this.valueMin);
                if (this.value != oldValue)
                {
                    this.OnValueChanged?.Invoke(oldValue, this.value);
                    this.UpdatePercent();
                }
            }
        }

        private void UpdatePercent()
        {
            this.percent1 = Convert.ToSingle(this.value - this.valueMin) / Convert.ToSingle(this.valueMax - this.valueMin);
            this.percent2 = Convert.ToDouble(this.value - this.valueMin) / Convert.ToDouble(this.valueMax - this.valueMin);
        }

        public static GameValue operator +(GameValue value, int num)
        {
            if (num != 0) {
                int old = value.value;
                value.value = (value.value + num).Clamp(value.valueMin, value.valueMax);
                if (old != value.value) value.UpdatePercent();
            }
            return value;
        }

        public static GameValue operator -(GameValue value, int num)
        {
            if (num != 0)
            {
                int old = value.value;
                value.value = (value.value - num).Clamp(value.valueMin, value.valueMax);
                if (old != value.value) value.UpdatePercent();
            }
            return value;
        }

        public static GameValue operator *(GameValue value, int num)
        {
            int old = value.value;
            value.value = ((value.value - value.valueMin) * num + value.valueMin).Clamp(value.valueMin, value.valueMax);
            if (old != value.value) value.UpdatePercent();
            return value;
        }

        public static GameValue operator /(GameValue value, int num)
        {
            int old = value.value;
            value.value = ((value.value - value.valueMin) / num + value.valueMin).Clamp(value.valueMin, value.valueMax);
            if (old != value.value) value.UpdatePercent();
            return value;
        }

        public static GameValue operator +(GameValue value, float num)
        {
            if (num != 0) {
                int old = value.value;
                value.value = Convert.ToInt32(value.value + (value.value - value.valueMin) * num).Clamp(value.valueMin, value.valueMax);
                if (old != value.value) value.UpdatePercent();
            }
            return value;
        }

        public static GameValue operator -(GameValue value, float num)
        {
            if (num != 0)
            {
                int old = value.value;
                value.value = Convert.ToInt32(value.value - (value.value - value.valueMin) * num).Clamp(value.valueMin, value.valueMax);
                if (old != value.value) value.UpdatePercent();
            }
            return value;
        }

        public static GameValue operator *(GameValue value, float num)
        {
            int old = value.value;
            value.value = Convert.ToInt32((value.value - value.valueMin) * num + value.valueMin).Clamp(value.valueMin, value.valueMax);
            if (old != value.value) value.UpdatePercent();
            return value;
        }

        public static GameValue operator /(GameValue value, float num)
        {
            int old = value.value;
            value.value = Convert.ToInt32((value.value - value.valueMin) / num + value.valueMin).Clamp(value.valueMin, value.valueMax);
            if (old != value.value) value.UpdatePercent();
            return value;
        }

        public static GameValue operator +(GameValue value, double num)
        {
            if (num != 0)
            {
                int old = value.value;
                value.value = Convert.ToInt32(value.value + (value.value - value.valueMin) * num).Clamp(value.valueMin, value.valueMax);
                if (old != value.value) value.UpdatePercent();
            }
            return value;
        }

        public static GameValue operator -(GameValue value, double num)
        {
            if (num != 0)
            {
                int old = value.value;
                value.value = Convert.ToInt32(value.value - (value.value - value.valueMin) * num).Clamp(value.valueMin, value.valueMax);
                if (old != value.value) value.UpdatePercent();
            }
            return value;
        }

        public static GameValue operator *(GameValue value, double num)
        {
            int old = value.value;
            value.value = Convert.ToInt32((value.value - value.valueMin) * num + value.valueMin).Clamp(value.valueMin, value.valueMax);
            if (old != value.value) value.UpdatePercent();
            return value;
        }

        public static GameValue operator /(GameValue value, double num)
        {
            int old = value.value;
            value.value = Convert.ToInt32((value.value - value.valueMin) / num + value.valueMin).Clamp(value.valueMin, value.valueMax);
            if (old != value.value) value.UpdatePercent();
            return value;
        }

        /// <summary> 值自增1 </summary>
        /// <param name="value">原值</param>
        /// <returns> 自增后的值 </returns>
        public static GameValue operator ++(GameValue value) => value += 1;

        /// <summary> 值自减1 </summary>
        /// <param name="value">原值</param>
        /// <returns> 自减后的值 </returns>
        public static GameValue operator --(GameValue value) => value -= 1;

        /// <summary> 转整数值 </summary>
        /// <param name="value">原值</param>
        /// <returns> 整数值 </returns>
        public static implicit operator int(GameValue value) => value.value;

        /// <summary> 转百分比值（单精度） </summary>
        /// <param name="value">原值</param>
        /// <returns> 百分比值（单精度） </returns>
        public static implicit operator float(GameValue value) => value.percent1;

        /// <summary> 转百分比值（双精度） </summary>
        /// <param name="value">原值</param>
        /// <returns> 百分比值（双精度） </returns>
        public static implicit operator double(GameValue value) => value.percent2;

        public override string ToString()
        {
            this.UpdatePercent();
            return $"[{this.valueMin}, {this.value}({(this.percent2 * 100).Round(2)}%), {this.valueMax}], Type = {this.ValueUpdateType.ToString()}";
        }
    }
}