using System;
using System.Linq;
using UnityEngine;

namespace LostKingdom.Utility
{
    /// <summary> 数学工具库 </summary>
    public static class LKMath
    {
        /// <summary> 纯小数黄金分割数（双精度） </summary>
        public const double Phi0 = 0.6180339887498948d;
        /// <summary> 纯小数黄金分割数（双精度） </summary>
        public const double Phi1 = 1.6180339887498948d;
        /// <summary> 重力加速度（双精度） </summary>
        public const double G = 9.8061905036721905d;
        /// <summary> 自然对数（双精度） </summary>
        public const double E = 2.7182818284590451d;
        /// <summary> 双精度圆周率（180°） </summary>
        public const double Pi = 3.1415926535897931d;
        /// <summary> 双精度圆弧度（360°） </summary>
        public const double Pi2 = LKMath.Pi * 2d;
        /// <summary> 双精度角度制（1°） </summary>
        public const double Deg01 = LKMath.Pi / 180d;
        /// <summary> 双精度角度制（15°） </summary>
        public const double Deg15 = LKMath.Pi / 12d;
        /// <summary> 双精度角度制（30°） </summary>
        public const double Deg30 = LKMath.Pi / 6d;
        /// <summary> 双精度角度制（45°） </summary>
        public const double Deg45 = LKMath.Pi / 4d;
        /// <summary> 双精度角度制（60°） </summary>
        public const double Deg60 = LKMath.Pi / 3d;
        /// <summary> 双精度角度制（90°） </summary>
        public const double Deg90 = LKMath.Pi / 2d;
        /// <summary> 双精度角度制（105°） </summary>
        public const double Deg105 = LKMath.Deg90 + LKMath.Deg15;
        /// <summary> 双精度角度制（120°） </summary>
        public const double Deg120 = LKMath.Deg90 + LKMath.Deg30;
        /// <summary> 双精度角度制（135°） </summary>
        public const double Deg135 = LKMath.Deg90 + LKMath.Deg45;
        /// <summary> 双精度角度制（150°） </summary>
        public const double Deg150 = LKMath.Deg90 + LKMath.Deg60;
        /// <summary> 纯小数黄金分割数（单精度） </summary>
        public const float phi0 = 0.6180339f;
        /// <summary> 带小数黄金分割数（单精度） </summary>
        public const float phi1 = 1.6180339f;
        /// <summary> 重力加速度（单精度） </summary>
        public const float g = 9.8066500f;
        /// <summary> 自然对数（单精度） </summary>
        public const float e = 2.7182818f;
        /// <summary> 单精度圆周率（180°） </summary>
        public const float pi = 3.1415927f;
        /// <summary> 单精度圆弧度（360°） </summary>
        public const float pi2 = LKMath.pi * 2f;
        /// <summary> 单精度角度制（1°） </summary>
        public const float deg01 = LKMath.pi / 180f;
        /// <summary> 单精度角度制（15°） </summary>
        public const float deg15 = LKMath.pi / 12f;
        /// <summary> 单精度角度制（30°） </summary>
        public const float deg30 = LKMath.pi / 6f;
        /// <summary> 单精度角度制（45°） </summary>
        public const float deg45 = LKMath.pi / 4f;
        /// <summary> 单精度角度制（60°） </summary>
        public const float deg60 = LKMath.pi / 3f;
        /// <summary> 单精度角度制（90°） </summary>
        public const float deg90 = LKMath.pi / 2f;
        /// <summary> 单精度角度制（105°） </summary>
        public const float deg105 = LKMath.deg90 + LKMath.deg15;
        /// <summary> 单精度角度制（120°） </summary>
        public const float deg120 = LKMath.deg90 + LKMath.deg30;
        /// <summary> 单精度角度制（135°） </summary>
        public const float deg135 = LKMath.deg90 + LKMath.deg45;
        /// <summary> 单精度角度制（150°） </summary>
        public const float deg150 = LKMath.deg90 + LKMath.deg60;

        #region General
        /// <summary> 向上取整 </summary>
        /// <param Name="value"> 原值 </param>
        public static int Ceiling(this float value) => (int)MathF.Ceiling(value);
        /// <summary> 向上取整 </summary>
        /// <param Name="value"> 原值 </param>
        public static int Ceiling(this double value) => (int)Math.Ceiling(value);
        /// <summary> 向上取整 </summary>
        /// <param Name="value"> 原值 </param>
        public static Vector2Int Ceiling(this Vector2 value) => new Vector2Int(value.x.Ceiling(), value.y.Ceiling());
        /// <summary> 向上取整 </summary>
        /// <param Name="value"> 原值 </param>
        public static Vector3Int Ceiling(this Vector3 value) => new Vector3Int(value.x.Ceiling(), value.y.Ceiling(), value.z.Ceiling());
        /// <summary> 向下取整 </summary>
        /// <param Name="value"> 原值 </param>
        public static int Floor(this float value) => (int)MathF.Floor(value);
        /// <summary> 向下取整 </summary>
        /// <param Name="value"> 原值 </param>
        public static int Floor(this double value) => (int)Math.Floor(value);
        /// <summary> 向下取整 </summary>
        /// <param Name="value"> 原值 </param>
        public static Vector2Int Floor(this Vector2 value) => new Vector2Int(value.x.Floor(), value.y.Floor());
        /// <summary> 向下取整 </summary>
        /// <param Name="value"> 原值 </param>
        public static Vector3Int Floor(this Vector3 value) => new Vector3Int(value.x.Floor(), value.y.Floor(), value.z.Floor());
        /// <summary> 四舍五入 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="digits"> 保留的小数位数 </param>
        public static int Round(this float value) => (int)MathF.Round(value, 0);
        /// <summary> 四舍五入 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="digits"> 保留的小数位数 </param>
        public static float Round(this float value, int digits) => (float)MathF.Round((float)value, digits);
        /// <summary> 四舍五入 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="digits"> 保留的小数位数 </param>
        public static int Round(this double value) => (int)Math.Round(value, 0);
        /// <summary> 四舍五入 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="digits"> 保留的小数位数 </param>
        public static float Round(this double value, int digits) => (float)MathF.Round((float)value, digits);
        /// <summary> 四舍五入 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="digits"> 保留的小数位数 </param>
        public static Vector2Int Round(this Vector2 value) => new Vector2Int(value.x.Round(), value.y.Round());
        /// <summary> 四舍五入 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="digits"> 保留的小数位数 </param>
        public static Vector2 Round(this Vector2 value, int digits) => new Vector2(value.x.Round(digits), value.y.Round(digits));
        /// <summary> 四舍五入 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="digits"> 保留的小数位数 </param>
        public static Vector3Int Round(this Vector3 value) => new Vector3Int(value.x.Round(), value.y.Round(), value.z.Round());
        /// <summary> 四舍五入 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="digits"> 保留的小数位数 </param>
        public static Vector3 Round(this Vector3 value, int digits) => new Vector3(value.x.Round(digits), value.y.Round(digits), value.z.Round(digits));
        /// <summary> 单向取余 </summary>
        /// <param Name="x"> 自变量 </param>
        /// <param Name="a"> 余数 </param>
        public static float Mod(this int x, int a) => (int)(((x % a) + a) % a);
        /// <summary> 单向取余 </summary>
        /// <param Name="x"> 自变量 </param>
        /// <param Name="a"> 余数 </param>
        public static float Mod(this float x, double a) => (float)(((x % a) + a) % a);
        /// <summary> 单向取余 </summary>
        /// <param Name="x"> 自变量 </param>
        /// <param Name="a"> 余数 </param>
        public static double Mod(this double x, double a) => (double)(((x % a) + a) % a);
        /// <summary> 幂函数 </summary>
        /// <param Name="x"> 自变量 </param>
        /// <param Name="a"> 幂 </param>
        public static float Pow(this float x, double a = LKMath.E) => (float)MathF.Pow(x, (float)a);
        /// <summary> 幂函数 </summary>
        /// <param Name="x"> 自变量 </param>
        /// <param Name="a"> 幂 </param>
        public static float Pow(this double x, double a = LKMath.E) => (float)MathF.Pow((float)x, (float)a);
        /// <summary> 自然对数函数 </summary>
        /// <param Name="x"> 自变量 </param>
        public static float Ln(this float x) => (float)MathF.Log(x);
        /// <summary> 自然对数函数 </summary>
        /// <param Name="x"> 自变量 </param>
        public static double Ln(this double x) => (double)Math.Log(x);
        /// <summary> 对数函数 </summary>
        /// <param Name="x"> 自变量 </param>
        /// <param Name="a"> 对数 </param>
        public static float Log(this float x, double a) => (float)(MathF.Log10(x) / Math.Log10(a));
        /// <summary> 对数函数 </summary>
        /// <param Name="x"> 自变量 </param>
        /// <param Name="a"> 对数 </param>
        public static double Log(this double x, double a) => (double)(Math.Log10(x) / Math.Log10(a));
        #endregion

        #region Trigonometric
        /// <summary> 开平方 </summary>
        /// <param Name="value"> 原值 </param>
        public static float Sqrt(this double value) => (float)MathF.Sqrt((float)value);
        /// <summary> 正弦函数 </summary>
        /// <param Name="angle"> 角度 </param>
        public static float Sin(this float angle) => (float)MathF.Sin(angle * LKMath.deg01);
        /// <summary> 正弦函数 </summary>
        /// <param Name="radian"> 弧度 </param>
        public static float Sin(this double radian) => (float)MathF.Sin((float)radian);
        /// <summary> 余弦函数 </summary>
        /// <param Name="angle"> 角度 </param>
        public static float Cos(this float angle) => (float)MathF.Cos(angle * LKMath.deg01);
        /// <summary> 余弦函数 </summary>
        /// <param Name="radian"> 弧度 </param>
        public static float Cos(this double radian) => (float)MathF.Cos((float)radian);
        /// <summary> 正切函数 </summary>
        /// <param Name="angle"> 角度 </param>
        public static float Tan(this float angle) => (float)MathF.Tan(angle * LKMath.deg01);
        /// <summary> 正切函数 </summary>
        /// <param Name="radian"> 弧度 </param>
        public static float Tan(this double radian) => (float)MathF.Tan((float)radian);
        /// <summary> 向量旋转（角度制） </summary>
        /// <param Name="value"> 向量 </param>
        /// <param Name="angle"> 角度 </param>
        public static Vector2 RotateBy(this Vector2 value, float angle)
        {
            float sin = LKMath.Sin(angle);
            float cos = LKMath.Cos(angle);
            float x = value.x * cos + value.y * sin;
            float y = value.y * cos - value.x * sin;
            return new Vector2(x, y);
        }
        /// <summary> 向量旋转（弧度制） </summary>
        /// <param Name="value"> 向量 </param>
        /// <param Name="radian"> 弧度 </param>
        public static Vector2 RotateBy(this Vector2 value, double radian)
        {
            double sin = LKMath.Sin(radian);
            double cos = LKMath.Cos(radian);
            double x = value.x * cos + value.y * sin;
            double y = value.y * cos - value.x * sin;
            return new Vector2((float)x, (float)y);
        }
        #endregion

        #region Absolute
        /// <summary> 绝对值 </summary>
        /// <param Name="value"> 原值 </param>
        public static sbyte Abs(this sbyte value) => Math.Abs(value);
        /// <summary> 绝对值 </summary>
        /// <param Name="value"> 原值 </param>
        public static short Abs(this short value) => Math.Abs(value);
        /// <summary> 绝对值 </summary>
        /// <param Name="value"> 原值 </param>
        public static int Abs(this int value) => Math.Abs(value);
        /// <summary> 绝对值 </summary>
        /// <param Name="value"> 原值 </param>
        public static long Abs(this long value) => Math.Abs(value);
        /// <summary> 绝对值 </summary>
        /// <param Name="value"> 原值 </param>
        public static decimal Abs(this decimal value) => Math.Abs(value);
        /// <summary> 绝对值 </summary>
        /// <param Name="value"> 原值 </param>
        public static float Abs(this float value) => Math.Abs(value);
        /// <summary> 绝对值 </summary>
        /// <param Name="value"> 原值 </param>
        public static double Abs(this double value) => Math.Abs(value);
        /// <summary> 绝对值 </summary>
        /// <param Name="value"> 原值 </param>
        public static Vector2 Abs(this Vector2 value) => new Vector2(Math.Abs(value.x), Math.Abs(value.y));
        /// <summary> 绝对值 </summary>
        /// <param Name="value"> 原值 </param>
        public static Vector2Int Abs(this Vector2Int value) => new Vector2Int(Math.Abs(value.x), Math.Abs(value.y));
        #endregion

        #region Extension
        /// <summary> 线性插值 </summary>
        /// <param Name="weight"> 权重（取值范围[0, 1]） </param>
        /// <param Name="damage"> 值1 </param>
        /// <param Name="combo"> 值2 </param>
        public static int Lerp(this float weight, int value1, int value2) => (int)(value1 * (1f - LKMath.Clamp(weight, 0, 1)) + value2 * LKMath.Clamp(weight, 0, 1));
        /// <summary> 多点线性插值 </summary>
        /// <param Name="weight"> 权重（取值范围[0, 数组长度-1]） </param>
        /// <param Name="values"> 值数组（至少要有两个值） </param>
        public static int Lerp(this float weight, params int[] values) => LKMath.Lerp(weight % 1f, values[LKMath.Floor(LKMath.Clamp(weight, 0, values.Length - 1))], values[LKMath.Ceiling(LKMath.Clamp(weight, 1, values.Length))]);
        /// <summary> 线性插值 </summary>
        /// <param Name="weight"> 权重（取值范围[0, 1]） </param>
        /// <param Name="damage"> 值1 </param>
        /// <param Name="combo"> 值2 </param>
        public static float Lerp(this float weight, float value1, float value2) => (float)(value1 * (1f - LKMath.Clamp(weight, 0, 1)) + value2 * LKMath.Clamp(weight, 0, 1));
        /// <summary> 多点线性插值 </summary>
        /// <param Name="weight"> 权重（取值范围[0, 数组长度-1]） </param>
        /// <param Name="values"> 值数组（至少要有两个值） </param>
        public static float Lerp(this float weight, params float[] values) => LKMath.Lerp(weight % 1f, values[LKMath.Floor(LKMath.Clamp(weight, 0, values.Length - 1))], values[LKMath.Ceiling(LKMath.Clamp(weight, 1, values.Length))]);
        /// <summary> 线性插值 </summary>
        /// <param Name="weight"> 权重（取值范围[0, 1]） </param>
        /// <param Name="damage"> 值1 </param>
        /// <param Name="combo"> 值2 </param>
        public static Vector2Int Lerp(this float weight, Vector2Int value1, Vector2Int value2) => new Vector2Int((int)LKMath.Lerp(weight, value1.x, value2.x), (int)LKMath.Lerp(weight, value1.y, value2.y));
        /// <summary> 多点线性插值 </summary>
        /// <param Name="weight"> 权重（取值范围[0, 数组长度-1]） </param>
        /// <param Name="values"> 值数组（至少要有两个值） </param>
        public static Vector2Int Lerp(this float weight, params Vector2Int[] values) => LKMath.Lerp(weight % 1f, values[LKMath.Floor(LKMath.Clamp(weight, 0, values.Length - 1))], values[LKMath.Ceiling(LKMath.Clamp(weight, 1, values.Length))]);
        /// <summary> 线性插值 </summary>
        /// <param Name="weight"> 权重（取值范围[0, 1]） </param>
        /// <param Name="damage"> 值1 </param>
        /// <param Name="combo"> 值2 </param>
        public static Vector2 Lerp(this float weight, Vector2 value1, Vector2 value2) => new Vector2((float)LKMath.Lerp(weight, value1.x, value2.x), (float)LKMath.Lerp(weight, value1.y, value2.y));
        /// <summary> 多点线性插值 </summary>
        /// <param Name="weight"> 权重（取值范围[0, 数组长度-1]） </param>
        /// <param Name="values"> 值数组（至少要有两个值） </param>
        public static Vector2 Lerp(this float weight, params Vector2[] values) => LKMath.Lerp(weight % 1f, values[LKMath.Floor(LKMath.Clamp(weight, 0, values.Length - 1))], values[LKMath.Ceiling(LKMath.Clamp(weight, 1, values.Length))]);
        /// <summary> 线性插值 </summary>
        /// <param Name="weight"> 权重（取值范围[0, 1]） </param>
        /// <param Name="damage"> 值1 </param>
        /// <param Name="combo"> 值2 </param>
        public static Vector2Int Lerp(this Vector2 weight, Vector2Int value1, Vector2Int value2) => new Vector2Int((int)LKMath.Lerp(weight.x, value1.x, value2.x), (int)LKMath.Lerp(weight.y, value1.y, value2.y));
        /// <summary> 多点线性插值 </summary>
        /// <param Name="weight"> 权重（取值范围[0, 数组长度-1]） </param>
        /// <param Name="values"> 值数组（至少要有两个值） </param>
        public static Vector2Int Lerp(this Vector2 weight, params Vector2Int[] values) => new Vector2Int(LKMath.Lerp(weight.x % 1f, values[LKMath.Floor(LKMath.Clamp(weight.x, 0, values.Length - 1))].x, values[LKMath.Ceiling(LKMath.Clamp(weight.x, 1, values.Length))].x), LKMath.Lerp(weight.y % 1f, values[LKMath.Floor(LKMath.Clamp(weight.y, 0, values.Length - 1))].y, values[LKMath.Ceiling(LKMath.Clamp(weight.y, 1, values.Length))].y));
        /// <summary> 线性插值 </summary>
        /// <param Name="weight"> 权重（取值范围[0, 1]） </param>
        /// <param Name="damage"> 值1 </param>
        /// <param Name="combo"> 值2 </param>
        public static Vector2 Lerp(this Vector2 weight, Vector2 value1, Vector2 value2) => new Vector2((float)LKMath.Lerp(weight.x, value1.x, value2.x), (float)LKMath.Lerp(weight.y, value1.y, value2.y));
        /// <summary> 多点线性插值 </summary>
        /// <param Name="weight"> 权重（取值范围[0, 数组长度-1]） </param>
        /// <param Name="values"> 值数组（至少要有两个值） </param>
        public static Vector2 Lerp(this Vector2 weight, params Vector2[] values) => new Vector2(LKMath.Lerp(weight.x % 1f, values[LKMath.Floor(LKMath.Clamp(weight.x, 0, values.Length - 1))].x, values[LKMath.Ceiling(LKMath.Clamp(weight.x, 1, values.Length))].x), LKMath.Lerp(weight.y % 1f, values[LKMath.Floor(LKMath.Clamp(weight.y, 0, values.Length - 1))].y, values[LKMath.Ceiling(LKMath.Clamp(weight.y, 1, values.Length))].y));
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        public static byte Clamp(this byte value, byte min, byte max) => ((value < min) ? (min) : ((value > max) ? (max) : (value)));
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        /// <param Name="extra"> 差值 </param>
        public static byte Clamp(this byte value, byte min, byte max, out int extra) => (byte)LKMath.Clamp((extra = value - (value = LKMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        public static sbyte Clamp(this sbyte value, sbyte min, sbyte max) => ((value < min) ? (min) : ((value > max) ? (max) : (value)));
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        /// <param Name="extra"> 差值 </param>
        public static sbyte Clamp(this sbyte value, sbyte min, sbyte max, out int extra) => (sbyte)LKMath.Clamp((extra = value - (value = LKMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        public static short Clamp(this short value, short min, short max) => ((value < min) ? (min) : ((value > max) ? (max) : (value)));
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        /// <param Name="extra"> 差值 </param>
        public static short Clamp(this short value, short min, short max, out int extra) => (short)LKMath.Clamp((extra = value - (value = LKMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        public static ushort Clamp(this ushort value, ushort min, ushort max) => ((value < min) ? (min) : ((value > max) ? (max) : (value)));
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        /// <param Name="extra"> 差值 </param>
        public static ushort Clamp(this ushort value, ushort min, ushort max, out int extra) => (ushort)LKMath.Clamp((extra = value - (value = LKMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        public static int Clamp(this int value, int min, int max) => ((value < min) ? (min) : ((value > max) ? (max) : (value)));
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        /// <param Name="extra"> 差值 </param>
        public static int Clamp(this int value, int min, int max, out int extra) => LKMath.Clamp((extra = value - (value = LKMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        public static uint Clamp(this uint value, uint min, uint max) => ((value < min) ? (min) : ((value > max) ? (max) : (value)));
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        /// <param Name="extra"> 差值 </param>
        public static uint Clamp(this uint value, uint min, uint max, out uint extra) => LKMath.Clamp((extra = value - (value = LKMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        public static long Clamp(this long value, long min, long max) => ((value < min) ? (min) : ((value > max) ? (max) : (value)));
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        /// <param Name="extra"> 差值 </param>
        public static long Clamp(this long value, long min, long max, out long extra) => LKMath.Clamp((extra = value - (value = LKMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        public static ulong Clamp(this ulong value, ulong min, ulong max) => ((value < min) ? (min) : ((value > max) ? (max) : (value)));
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        /// <param Name="extra"> 差值 </param>
        public static ulong Clamp(this ulong value, ulong min, ulong max, out ulong extra) => LKMath.Clamp((extra = value - (value = LKMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        public static float Clamp(this float value, float min, float max) => ((value < min) ? (min) : ((value > max) ? (max) : (value)));
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        /// <param Name="extra"> 差值 </param>
        public static float Clamp(this float value, float min, float max, out float extra) => LKMath.Clamp((extra = value - (value = LKMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        public static double Clamp(this double value, double min, double max) => ((value < min) ? (min) : ((value > max) ? (max) : (value)));
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        /// <param Name="extra"> 差值 </param>
        public static double Clamp(this double value, double min, double max, out double extra) => LKMath.Clamp((extra = value - (value = LKMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max) => new Vector2(LKMath.Clamp(value.x, min.x, max.x), LKMath.Clamp(value.y, min.y, max.y));
        /// <summary> 返回范围内的值（闭区间） </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="min"> 下限 </param>
        /// <param Name="max"> 上限 </param>
        /// <param Name="extra"> 差值 </param>
        public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max, out Vector2 extra) => LKMath.Clamp((extra = value - (value = LKMath.Clamp(value, min, max))) + value, min, max);
        /// <summary> 取向量长度 </summary>
        /// <param Name="value"> 原值 </param>
        public static float Length(this Vector2 value) => LKMath.Sqrt(value.x * value.x + value.y * value.y);
        /// <summary> 取向量长度 </summary>
        /// <param Name="value"> 原值 </param>
        public static float Length(this Vector2Int value) => LKMath.Sqrt(value.x * value.x + value.y * value.y);
        /// <summary> 取向量长度 </summary>
        /// <param Name="value"> 原值 </param>
        public static float Length(this Vector3 value) => LKMath.Sqrt(value.x * value.x + value.y * value.y + value.z * value.z);
        /// <summary> 取向量长度 </summary>
        /// <param Name="value"> 原值 </param>
        public static float Length(this Vector3Int value) => LKMath.Sqrt(value.x * value.x + value.y * value.y + value.z * value.z);
        #endregion

        #region Max/Min
        /// <summary> 返回最大值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static byte Max(this byte value, params byte[] values) => Math.Max(value, values.Max());
        /// <summary> 返回最大值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static sbyte Max(this sbyte value, params sbyte[] values) => Math.Max(value, values.Max());
        /// <summary> 返回最大值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static short Max(this short value, params short[] values) => Math.Max(value, values.Max());
        /// <summary> 返回最大值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static ushort Max(this ushort value, params ushort[] values) => Math.Max(value, values.Max());
        /// <summary> 返回最大值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static int Max(this int value, params int[] values) => Math.Max(value, values.Max());
        /// <summary> 返回最大值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static uint Max(this uint value, params uint[] values) => Math.Max(value, values.Max());
        /// <summary> 返回最大值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static long Max(this long value, params long[] values) => Math.Max(value, values.Max());
        /// <summary> 返回最大值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static ulong Max(this ulong value, params ulong[] values) => Math.Max(value, values.Max());
        /// <summary> 返回最大值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static float Max(this float value, params float[] values) => Math.Max(value, values.Max());
        /// <summary> 返回最大值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static double Max(this double value, params double[] values) => Math.Max(value, values.Max());
        /// <summary> 返回最大值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static decimal Max(this decimal value, params decimal[] values) => Math.Max(value, values.Max());

        /// <summary> 返回最小值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static byte Min(this byte value, params byte[] values) => Math.Min(value, values.Min());
        /// <summary> 返回最小值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static sbyte Min(this sbyte value, params sbyte[] values) => Math.Min(value, values.Min());
        /// <summary> 返回最小值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static short Min(this short value, params short[] values) => Math.Min(value, values.Min());
        /// <summary> 返回最小值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static ushort Min(this ushort value, params ushort[] values) => Math.Min(value, values.Min());
        /// <summary> 返回最小值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static int Min(this int value, params int[] values) => Math.Min(value, values.Min());
        /// <summary> 返回最小值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static uint Min(this uint value, params uint[] values) => Math.Min(value, values.Min());
        /// <summary> 返回最小值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static long Min(this long value, params long[] values) => Math.Min(value, values.Min());
        /// <summary> 返回最小值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static ulong Min(this ulong value, params ulong[] values) => Math.Min(value, values.Min());
        /// <summary> 返回最小值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static float Min(this float value, params float[] values) => Math.Min(value, values.Min());
        /// <summary> 返回最小值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static double Min(this double value, params double[] values) => Math.Min(value, values.Min());
        /// <summary> 返回最小值 </summary>
        /// <param Name="value"> 原值 </param>
        /// <param Name="values"> 数组 </param>
        public static decimal Min(this decimal value, params decimal[] values) => Math.Min(value, values.Min());
        #endregion

        #region Sum
        /// <summary> 求和 </summary>
        /// <param Name="values"> 数组 </param>
        /// <param Name="basic"> 初始值 </param>
        public static byte Sum(this byte[] values, byte basic = (byte)0)
        {
            foreach (byte value in values) basic = (byte)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="basic"> 初始值 </param>
        /// <param Name="values"> 数组 </param>
        public static byte Sum(this byte basic, params byte[] values)
        {
            foreach (byte value in values) basic = (byte)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="values"> 数组 </param>
        /// <param Name="basic"> 初始值 </param>
        public static sbyte Sum(this sbyte[] values, sbyte basic = (sbyte)0)
        {
            foreach (sbyte value in values) basic = (sbyte)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="basic"> 初始值 </param>
        /// <param Name="values"> 数组 </param>
        public static sbyte Sum(this sbyte basic, params sbyte[] values)
        {
            foreach (sbyte value in values) basic = (sbyte)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="values"> 数组 </param>
        /// <param Name="basic"> 初始值 </param>
        public static short Sum(this short[] values, short basic = (short)0)
        {
            foreach (short value in values) basic = (short)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="basic"> 初始值 </param>
        /// <param Name="values"> 数组 </param>
        public static short Sum(this short basic, params short[] values)
        {
            foreach (short value in values) basic = (short)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="values"> 数组 </param>
        /// <param Name="basic"> 初始值 </param>
        public static ushort Sum(this ushort[] values, ushort basic = (ushort)0)
        {
            foreach (ushort value in values) basic = (ushort)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="basic"> 初始值 </param>
        /// <param Name="values"> 数组 </param>
        public static ushort Sum(this ushort basic, params ushort[] values)
        {
            foreach (ushort value in values) basic = (ushort)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="values"> 数组 </param>
        /// <param Name="basic"> 初始值 </param>
        public static int Sum(this int[] values, int basic = (int)0)
        {
            foreach (int value in values) basic = (int)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="basic"> 初始值 </param>
        /// <param Name="values"> 数组 </param>
        public static int Sum(this int basic, params int[] values)
        {
            foreach (int value in values) basic = (int)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="values"> 数组 </param>
        /// <param Name="basic"> 初始值 </param>
        public static uint Sum(this uint[] values, uint basic = (uint)0)
        {
            foreach (uint value in values) basic = (uint)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="basic"> 初始值 </param>
        /// <param Name="values"> 数组 </param>
        public static uint Sum(this uint basic, params uint[] values)
        {
            foreach (uint value in values) basic = (uint)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="values"> 数组 </param>
        /// <param Name="basic"> 初始值 </param>
        public static long Sum(this long[] values, long basic = (long)0)
        {
            foreach (long value in values) basic = (long)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="basic"> 初始值 </param>
        /// <param Name="values"> 数组 </param>
        public static long Sum(this long basic, params long[] values)
        {
            foreach (long value in values) basic = (long)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="values"> 数组 </param>
        /// <param Name="basic"> 初始值 </param>
        public static ulong Sum(this ulong[] values, ulong basic = (ulong)0)
        {
            foreach (ulong value in values) basic = (ulong)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="basic"> 初始值 </param>
        /// <param Name="values"> 数组 </param>
        public static ulong Sum(this ulong basic, params ulong[] values)
        {
            foreach (ulong value in values) basic = (ulong)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="values"> 数组 </param>
        /// <param Name="basic"> 初始值 </param>
        public static float Sum(this float[] values, float basic = (float)0)
        {
            foreach (float value in values) basic = (float)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="basic"> 初始值 </param>
        /// <param Name="values"> 数组 </param>
        public static float Sum(this float basic, params float[] values)
        {
            foreach (float value in values) basic = (float)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="values"> 数组 </param>
        /// <param Name="basic"> 初始值 </param>
        public static double Sum(this double[] values, double basic = (double)0)
        {
            foreach (double value in values) basic = (double)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="basic"> 初始值 </param>
        /// <param Name="values"> 数组 </param>
        public static double Sum(this double basic, params double[] values)
        {
            foreach (double value in values) basic = (double)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="values"> 数组 </param>
        /// <param Name="basic"> 初始值 </param>
        public static decimal Sum(this decimal[] values, decimal basic = (decimal)0)
        {
            foreach (decimal value in values) basic = (decimal)(basic + value);
            return basic;
        }
        /// <summary> 求和 </summary>
        /// <param Name="basic"> 初始值 </param>
        /// <param Name="values"> 数组 </param>
        public static decimal Sum(this decimal basic, params decimal[] values)
        {
            foreach (decimal value in values) basic = (decimal)(basic + value);
            return basic;
        }
        #endregion
    }
}
