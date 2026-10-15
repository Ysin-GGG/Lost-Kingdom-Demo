using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace LostKingdom.Utility
{
    /// <summary>
    /// 魔改Color类
    /// <para> 适合在只需要修改单色亮度的情况下使用，不建议直接赋值使用 </para>
    /// <para> 如果有必要通过直接赋值来修改颜色，应使用该类的Set函数 </para>
    /// </summary>
    public class LKColor
    {
        private Color32 color;
        /// <summary> 红色亮度（0f~1f） </summary> 
        public float r { get => this.color.r / (float)byte.MaxValue; set { byte v = Convert.ToByte(value.Clamp(0f, 1f) * byte.MaxValue); if (this.color.r != v) { this.color.r = v; this.OnColorUpdated?.Invoke(this); } } }
        /// <summary> 绿色亮度（0f~1f） </summary> 
        public float g { get => this.color.g / (float)byte.MaxValue; set { byte v = Convert.ToByte(value.Clamp(0f, 1f) * byte.MaxValue); if (this.color.g != v) { this.color.g = v; this.OnColorUpdated?.Invoke(this); } } }
        /// <summary> 蓝色亮度（0f~1f） </summary> 
        public float b { get => this.color.b / (float)byte.MaxValue; set { byte v = Convert.ToByte(value.Clamp(0f, 1f) * byte.MaxValue); if (this.color.b != v) { this.color.b = v; this.OnColorUpdated?.Invoke(this); } } }
        /// <summary> 透明度（0f~1f） </summary> 
        public float a { get => this.color.a / (float)byte.MaxValue; set { byte v = Convert.ToByte(value.Clamp(0f, 1f) * byte.MaxValue); if (this.color.a != v) { this.color.a = v; this.OnColorUpdated?.Invoke(this); } } }
        /// <summary> 红色亮度（0~255） </summary> 
        public int R { get => this.color.r; set { if (this.color.a != value.Clamp(0, byte.MaxValue)) { this.color.r = (byte)value.Clamp(0, byte.MaxValue); this.OnColorUpdated?.Invoke(this); } } }
        /// <summary> 绿色亮度（0~255） </summary> 
        public int G { get => this.color.g; set { if (this.color.a != value.Clamp(0, byte.MaxValue)) { this.color.g = (byte)value.Clamp(0, byte.MaxValue); this.OnColorUpdated?.Invoke(this); } } }
        /// <summary> 蓝色亮度（0~255） </summary> 
        public int B { get => this.color.b; set { if (this.color.a != value.Clamp(0, byte.MaxValue)) { this.color.b = (byte)value.Clamp(0, byte.MaxValue); this.OnColorUpdated?.Invoke(this); } } }
        /// <summary> 透明度（0~255） </summary> 
        public int A { get => this.color.a; set { if (this.color.a != value.Clamp(0, byte.MaxValue)) { this.color.a = (byte)value.Clamp(0, byte.MaxValue); this.OnColorUpdated?.Invoke(this); } } }
        /// <summary> 颜色变更时 </summary>
        public event Action<LKColor> OnColorUpdated;

        /// <summary> 根据提供的字符，可同对RGBA赋予同一的分量 </summary>
        /// <param Name="key"> 可包含RGBA的字符串（不分大小写） </param>
        /// <param Name="value"> 单精度浮点数分量（0f~1f） </param>
        public Color this[string key, float value]
        {
            get
            {
                // 待修改成Set格式
                key = key.ToUpper();
                if (key.Contains('R')) this.r = value;
                if (key.Contains('G')) this.g = value;
                if (key.Contains('B')) this.b = value;
                if (key.Contains('A')) this.a = value;
                return this.color;
            }
        }

        /// <summary> 根据提供的字符，可同对RGBA赋予同一的分量 </summary>
        /// <param Name="key"> 可包含RGBA的字符串（不分大小写） </param>
        /// <param Name="value"> 8位无符号整数分量（0~255） </param>
        public Color32 this[string key, byte value]
        {
            get
            {
                // 待修改成Set格式
                key = key.ToUpper();
                if (key.Contains('R')) this.R = value;
                if (key.Contains('G')) this.G = value;
                if (key.Contains('B')) this.B = value;
                if (key.Contains('A')) this.A = value;
                return this.color;
            }
        }

        /// <summary> 根据已有Color创建LKColor对象 </summary>
        /// <param Name="color"> 目标Color结构体 </param>
        /// <param Name="onColorUpdate"> 常用于在颜色变动时，同步修改指定的Color对象 </param>
        public LKColor(Color color, Action<LKColor> onColorUpdate = null) { this.color = new Color(color.r, color.g, color.b, color.a); if (onColorUpdate != null) this.OnColorUpdated += onColorUpdate; }

        /// <summary> 根据已有Color创建LKColor对象 </summary>
        /// <param Name="color"> 目标Color结构体 </param>
        /// <param Name="onColorUpdate"> 常用于在颜色变动时，同步修改指定的Color对象 </param>
        public LKColor(Color32 color, Action<LKColor> onColorUpdate = null) { this.color = new Color(color.r, color.g, color.b, color.a); if (onColorUpdate != null) this.OnColorUpdated += onColorUpdate; }

        /// <summary> 根据十六进制颜色码创建LKColor对象 </summary>
        /// <param Name="code"> 十六进制颜色码（需以'#'开头，RGBA格式） </param>
        public LKColor(string code)
        {
            this.color = new Color(1f, 1f, 1f, 1f);
            if (code.StartsWith('#'))
            {
                if (code.Length > 2) this.color.r = Convert.ToByte(code.Substring(1, 2), 16);
                if (code.Length > 4) this.color.g = Convert.ToByte(code.Substring(3, 2), 16);
                if (code.Length > 6) this.color.b = Convert.ToByte(code.Substring(5, 2), 16);
                if (code.Length > 8) this.color.a = Convert.ToByte(code.Substring(7, 2), 16);
            }
        }

        /// <summary> 根据1个单精度浮点数创建LKColor对象 </summary>
        /// <param Name="value"> RGBA的分量（0f~1f） </param>
        public LKColor(float value) => this.color = new Color(value, value, value, value);

        /// <summary> 根据2个单精度浮点数创建LKColor对象 </summary>
        /// <param Name="value"> RGB的分量（0f~1f） </param>
        /// <param Name="a"> 透明度（0f~1f） </param>
        public LKColor(float value, float a) => this.color = new Color(value, value, value, a);

        /// <summary> 根据4个单精度浮点数创建LKColor对象 </summary>
        /// <param Name="r"> 红色亮度（0f~1f） </param>
        /// <param Name="g"> 绿色亮度（0f~1f） </param>
        /// <param Name="b"> 蓝色亮度（0f~1f） </param>
        /// <param Name="a"> 透明度（0f~1f） </param>
        public LKColor(float r, float g, float b, float a = 1f) => this.color = new Color(r, g, b, a);

        /// <summary> 根据1个8位无符号整数创建LKColor对象 </summary>
        /// <param Name="value"> RGBA的分量（0~255） </param>
        public LKColor(byte value) => this.color = new Color32(value, value, value, value);

        /// <summary> 根据2个8位无符号整数创建LKColor对象 </summary>
        /// <param Name="value"> RGB的分量（0~255） </param>
        /// <param Name="A"> 透明度（0~255） </param>
        public LKColor(byte value, byte A) => this.color = new Color32(value, value, value, A);

        /// <summary> 根据4个8位无符号整数创建LKColor对象 </summary>
        /// <param Name="R"> 红色亮度（0~255） </param>
        /// <param Name="G"> 绿色亮度（0~255） </param>
        /// <param Name="B"> 蓝色亮度（0~255） </param>
        /// <param Name="A"> 透明度（0~255） </param>
        public LKColor(byte R, byte G, byte B, byte A = 255) => this.color = new Color32(R, G, B, A);

        /// <summary> 根据1个四维向量创建LKColor对象 </summary>
        /// <param Name="value"> 对应RGBA的XYZW四维向量 </param>
        public LKColor(Vector4 value) => this.color = new Color(value.x, value.y, value.z, value.w);

        /// <summary> 根据1个32位无符号整数创建LKColor对象 </summary>
        /// <param Name="code"> 从高位到低位的每8位，分别对应A、R、G、B的亮度（建议使用十六进制格式表示） </param>
        public LKColor(uint code = 0xFF) => this.color = new Color32((byte)((code >> 16) % 256), (byte)((code >> 8) % 256), (byte)((code >> 0) % 256), (byte)((code >> 24) % 256));
        
        /// <summary> 复制色号 </summary>
        /// <param Name="color"> 目标色号 </param>
        /// <param Name="alpha"> 是否同时复制其透明度 </param>
        public LKColor Set(Color color, bool alpha = false)
        {
            if (alpha)
            {
                byte R = Convert.ToByte(color.r.Clamp(0, 1f));
                byte G = Convert.ToByte(color.g.Clamp(0, 1f));
                byte B = Convert.ToByte(color.b.Clamp(0, 1f));
                byte A = Convert.ToByte(color.a.Clamp(0, 1f));
                this.Set(R, G, B, A);
            }
            else
            {
                byte R = Convert.ToByte(color.r.Clamp(0, 1f));
                byte G = Convert.ToByte(color.g.Clamp(0, 1f));
                byte B = Convert.ToByte(color.b.Clamp(0, 1f));
                this.Set(R, G, B);
            }
            return this;
        }

        /// <summary> 复制色号 </summary>
        /// <param Name="color"> 目标色号 </param>
        /// <param Name="alpha"> 是否同时复制其透明度 </param>
        public LKColor Set(Color32 color, bool alpha = false)
        {
            if (alpha)
            {
                byte R = color.r;
                byte G = color.g;
                byte B = color.b;
                byte A = color.a;
                this.Set(R, G, B, A);
            }
            else
            {
                byte R = color.r;
                byte G = color.g;
                byte B = color.b;
                this.Set(R, G, B);
            }
            return this;
        }

        /// <summary> 复制色号 </summary>
        /// <param Name="color"> 目标色号 </param>
        /// <param Name="alpha"> 是否同时复制其透明度 </param>
        public LKColor Set(LKColor color, bool alpha = false)
        {
            if (alpha)
            {
                byte R = color.color.r;
                byte G = color.color.g;
                byte B = color.color.b;
                byte A = color.color.a;
                this.Set(R, G, B, A);
            }
            else
            {
                byte R = color.color.r;
                byte G = color.color.g;
                byte B = color.color.b;
                this.Set(R, G, B);
            }
            return this;
        }

        /// <summary> 设置多个色号 </summary>
        /// <param Name="r"> 红色亮度（0f~1f） </param>
        /// <param Name="g"> 绿色亮度（0f~1f） </param>
        /// <param Name="b"> 蓝色亮度（0f~1f） </param>
        public LKColor Set(float r, float g, float b)
        {
            byte R = Convert.ToByte(r.Clamp(0, 1f));
            byte G = Convert.ToByte(g.Clamp(0, 1f));
            byte B = Convert.ToByte(b.Clamp(0, 1f));
            this.Set(R, G, B);
            return this;
        }

        /// <summary> 设置多个色号 </summary>
        /// <param Name="r"> 红色亮度（0f~1f） </param>
        /// <param Name="g"> 绿色亮度（0f~1f） </param>
        /// <param Name="b"> 蓝色亮度（0f~1f） </param>
        /// <param Name="a"> 透明度（0f~1f） </param>
        public LKColor Set(float r, float g, float b, float a)
        {
            byte R = Convert.ToByte(r.Clamp(0, 1f));
            byte G = Convert.ToByte(g.Clamp(0, 1f));
            byte B = Convert.ToByte(b.Clamp(0, 1f));
            byte A = Convert.ToByte(a.Clamp(0, 1f));
            this.Set(R, G, B, A);
            return this;
        }

        /// <summary> 设置多个色号 </summary>
        /// <param Name="r"> 红色亮度（0~255） </param>
        /// <param Name="g"> 绿色亮度（0~255） </param>
        /// <param Name="b"> 蓝色亮度（0~255） </param>
        public LKColor Set(int r, int g, int b)
        {
            byte R = Convert.ToByte(r.Clamp(0, byte.MaxValue));
            byte G = Convert.ToByte(g.Clamp(0, byte.MaxValue));
            byte B = Convert.ToByte(b.Clamp(0, byte.MaxValue));
            this.Set(R, G, B);
            return this;
        }

        /// <summary> 设置多个色号 </summary>
        /// <param Name="r"> 红色亮度（0~255） </param>
        /// <param Name="g"> 绿色亮度（0~255） </param>
        /// <param Name="b"> 蓝色亮度（0~255） </param>
        /// <param Name="a"> 透明度（0~255） </param>
        public LKColor Set(int r, int g, int b, int a)
        {
            byte R = Convert.ToByte(r.Clamp(0, byte.MaxValue));
            byte G = Convert.ToByte(g.Clamp(0, byte.MaxValue));
            byte B = Convert.ToByte(b.Clamp(0, byte.MaxValue));
            byte A = Convert.ToByte(a.Clamp(0, byte.MaxValue));
            this.Set(R, G, B, A);
            return this;
        }

        /// <summary> 根据十六进制颜色码设置多个色号 </summary>
        /// <param Name="code"> 十六进制颜色码（需以'#'开头，RGBA格式） </param>
        public LKColor Set(string code)
        {
            byte R = 255;
            byte G = 255;
            byte B = 255;
            byte A = 255;
            if (code.StartsWith('#'))
            {
                if (code.Length > 2) R = Convert.ToByte(code.Substring(1, 2), 16);
                if (code.Length > 4) G = Convert.ToByte(code.Substring(3, 2), 16);
                if (code.Length > 6) B = Convert.ToByte(code.Substring(5, 2), 16);
                if (code.Length > 8) A = Convert.ToByte(code.Substring(7, 2), 16);
            }
            this.Set(R, G, B, A);
            return this;
        }

        /// <summary> 根据32位无符号整数设置多个色号 </summary>
        /// <param Name="code"> 从高位到低位的每8位，分别对应A、R、G、B的亮度（建议使用十六进制格式表示） </param>
        public LKColor Set(uint code)
        {
            byte R = (byte)((code >> 16) % 256);
            byte G = (byte)((code >> 8) % 256);
            byte B = (byte)((code >> 0) % 256);
            byte A = (byte)((code >> 24) % 256);
            this.Set(R, G, B, A);
            return this;
        }

        private void Set(byte R, byte G, byte B)
        {
            bool updated = false;
            if (this.color.r != R)
            {
                this.color.r = R;
                updated = true;
            }
            if (this.color.g != G)
            {
                this.color.g = G;
                updated = true;
            }
            if (this.color.b != B)
            {
                this.color.b = B;
                updated = true;
            }
            if (updated) this.OnColorUpdated?.Invoke(this);
        }

        private void Set(byte R, byte G, byte B, byte A)
        {
            bool updated = false;
            if (this.color.r != R)
            {
                this.color.r = R;
                updated = true;
            }
            if (this.color.g != G)
            {
                this.color.g = G;
                updated = true;
            }
            if (this.color.b != B)
            {
                this.color.b = B;
                updated = true;
            }
            if (this.color.a != A)
            {
                this.color.a = A;
                updated = true;
            }
            if (updated) this.OnColorUpdated?.Invoke(this);
        }

        /// <summary> 将RGBA以字符串返回 </summary>
        public override string ToString() => string.Format("[ R = {0}({4}), G = {1}({5}), B = {2}({6}}), A = {3}({7}) ]", this.R, this.G, this.B, this.A, this.r, this.g, this.b, this.a);

        /// <summary> 由UInt（ARGB）隐式转换为LKColor </summary>
        public static implicit operator LKColor(uint code) => new LKColor(code);
        /// <summary> 由十六进制颜色码（需以'#'开头，RGBA格式）隐式转换为LKColor </summary>
        public static implicit operator LKColor(string code) => new LKColor(code);
        /// <summary> 由Color隐式转换为LKColor </summary>
        public static implicit operator LKColor(Color color) => new LKColor(color);
        /// <summary> 由Color32隐式转换为LKColor </summary>
        public static implicit operator LKColor(Color32 color) => new LKColor(color);
        /// <summary> 隐式转换为Color </summary>
        public static implicit operator Color(LKColor color) => color.color;
        /// <summary> 隐式转换为Color32 </summary>
        public static implicit operator Color32(LKColor color) => color.color;
        /// <summary> 隐式转换为UInt（ARGB格式） </summary>
        public static implicit operator uint(LKColor color) => Convert.ToUInt32((((uint)color.R) << 16) + (((uint)color.G) << 8) + (((uint)color.B) << 0) + (((uint)color.A) << 24));
        /// <summary> 隐式转换为16进制的字符串（RGBA格式） </summary>
        public static implicit operator string(LKColor color) => Convert.ToString((((uint)color.R) << 24) + (((uint)color.G) << 16) + (((uint)color.B) << 8) + (((uint)color.A) << 0), 16);
    }
}
