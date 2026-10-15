using LostKingdom.ID;
using LostKingdom.IO;
using System;
using UnityEngine;

namespace LostKingdom.Utility
{
    /// <summary> 伪随机数生成器 </summary>
    public class LKRandom
    {
        /*
         * 由于double随机数生成不需要类型转换，所以返回值写的是“this.Next(min, max - min)”，而不是有精度丢失可能的“min + this.Next(0.0d, max - min)”
         * 此类下的数据转换均不使用Convert类（因为Convert类对浮点数转整数会进行四舍五入）
         */

        /// <summary> 全局默认伪随机数生成器 </summary>
        public static readonly LKRandom Instance = new LKRandom(null, false);
        /// <summary> 种子 </summary>
        public ulong Seed { get; internal set; }
        /// <summary> 是否启用正态分布 </summary>
        public bool Gaussian { get; set; }

        internal byte head;
        internal byte tail;
        internal ulong[] rand = new ulong[256];

        /// <summary> 创建伪随机数生成器 </summary>
        /// <param Name="seed"> 种子（设null则取当前时间为种子） </param>
        public LKRandom(ulong? seed = null, bool gaussian = false)
        {
            this.Seed = seed ?? (ulong)DateTime.Now.Ticks;
            ulong ii = 1, jj = this.Seed;
            this.head = 0;
            this.tail = 127;
            this.rand[255] = jj;
            for (byte i, j = 255; j > 0; j--)
            {
                i = (byte)(127 * j);
                this.rand[i] = ii;
                ii = jj - ii;
                jj = this.rand[i];
            }
            for (byte i = 255; i > 0; i--)
                this.rand[i] -= this.rand[(byte)(i + 127)];
            for (byte i = 255; i > 0; i--)
                this.next(0, 0);
        }

        public void Load(GameData data)
        {
            
        }

        /// <summary> 泛用随机数（[0,1) * mul + add） </summary>
        /// <param Name="add"> 追加数 </param>
        /// <param Name="mul"> 追乘数 </param>
        private double next(double add = 0.0d, double mul = 1.0d) => this.Gaussian ? this.NextGauss(add + mul / 2.0d, mul / 2.0d) : this.Next(add, mul);

        /// <summary> 服从均匀分布的泛用随机数（[0,1) * mul + add） </summary>
        /// <param Name="add"> 追加数 </param>
        /// <param Name="mul"> 追乘数 </param>
        public double Next(double add = 0.0d, double mul = 1.0d)
        {
            while (++this.head == 0) ;
            while (++this.tail == 0) ;
            return (this.rand[this.head] -= this.rand[this.tail]) / (double)ulong.MaxValue * mul + add;
        }

        /// <summary> 服从正态分布的泛用随机数 </summary>
        /// <param Name="add"> 追加数 </param>
        /// <param Name="mul"> 追乘数 </param>
        public double NextGauss(double add = 0.0d, double mul = 1.0d)
        {
            double r = 1.0d - this.Next(0.0d, 1.0d); // 由于Ln(0)为负无穷大，取值范围应为(0, 1]，
            double theta = this.Next(0.0d, 1.0d) * LKMath.Pi2;
            return LKMath.Sqrt(-2.0d * r.Ln()) * Math.Sin(LKMath.Pi2 * theta).Abs() * this.Next(add, mul) / 8.8480445953779840d; // 用Box-Muller将均匀分布转换为正态分布
        }

        /// <summary> 随机8位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public byte NextByte(byte min = byte.MinValue, byte max = byte.MaxValue) => (byte)(min + this.next(0.0d, max - min));

        /// <summary> 随机8位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public sbyte NextSByte(sbyte min = sbyte.MinValue, sbyte max = sbyte.MaxValue) => (sbyte)(min + this.next(0.0d, max - min));
        
        /// <summary> 随机16位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public short NextShort(short min = short.MinValue, short max = short.MaxValue) => (short)(min + this.next(0.0d, max - min));

        /// <summary> 随机16位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public ushort NextUShort(ushort min = ushort.MinValue, ushort max = ushort.MaxValue) => (ushort)(min + this.next(0.0d, max - min));

        /// <summary> 随机32位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public int NextInt(int min = int.MinValue, int max = int.MaxValue) => (int)(min + this.next(0.0d, max - min));

        /// <summary> 随机32位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public uint NextUInt(uint min = uint.MinValue, uint max = uint.MaxValue) => (uint)(min + this.next(0.0d, max - min));

        /// <summary> 随机64位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public long NextLong(long min = long.MinValue, long max = long.MaxValue) => (long)(min + this.next(0.0d, max - min));

        /// <summary> 随机64位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public ulong NextULong(ulong min = ulong.MinValue, ulong max = ulong.MaxValue) => (ulong)(min + this.next(0.0d, max - min));

        /// <summary> 随机单精度浮点数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public float NextFloat(float min = float.MinValue, float max = float.MaxValue) => (float)this.next(min, max - min);

        /// <summary> 随机双精度浮点数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        public double NextDouble(double min = double.MinValue, double max = double.MaxValue) => (double)this.next(min, max - min);

        /// <summary> 随机布尔值 </summary>
        /// <param Name="denominator"> 分母（返回true的概率为：1/denominator * 100%） </param>
        public bool NextBool(int denominator) => this.next(0.0d, 1.0d) < (1.0d / LKMath.Max(1, denominator));

        /// <summary> 随机布尔值 </summary>
        /// <param Name="probability"> 百分比概率（返回true的可能性为：probability * 100%） </param>
        public bool NextBool(double probability = 0.5d) => this.next(0.0d, 1.0d) < probability;

        /// <summary> 随机取单个字符 </summary>
        /// <param Name="str"> 源字符串 </param>
        public char NextChar(string str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ") => str[(int)(this.next(0.0d, str.Length))];

        /// <summary> 随机取角度（单精度） </summary>
        /// <param Name="min"> 最小角度 </param>
        /// <param Name="max"> 最大角度 </param>
        public float NextRadian(float min = 0.0f, float max = 360.0f) => (float)(this.next(min, max - min) * LKMath.Deg01);

        /// <summary> 随机取角度（双精度） </summary>
        /// <param Name="min"> 最小角度 </param>
        /// <param Name="max"> 最大角度 </param>
        public double NextRadian(double min = 0.0d, double max = 360.0d) => (double)(this.next(min, max - min) * LKMath.Deg01);

        /// <summary> 随机取对象 </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="objs"> 对象数组 </param>
        public T NextObject<T>(params T[] objs) => objs[(int)this.next(0.0d, objs.Length)];

        /// <summary> 随机取对象（指定概率） </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="pairs"> 对象-概率对数组 </param>
        public T NextObject<T>(params (T, double)[] pairs)
        {
            int length = pairs.Length;
            double min = 0.0d, max = 0.0d;
            double probability = this.next(0.0d, 1.0d);
            for (int i = 0; i < length; i++)
            {
                min = max;
                max += pairs[i].Item2;
                if (min <= probability && probability < max)
                    return pairs[i].Item1;
            }
            return default(T);
        }

        /// <summary> 随机取对象（指定概率） </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="objs"> 对象数组 </param>
        /// <param Name="probabilities"> 概率数组（若Length少于对象数组，则会循环取概率） </param>
        public T NextObject<T>(T[] objs, double[] probabilities)
        {
            int length = objs.Length;
            (T, double)[] pairs = new (T, double)[length];
            for (int i = 0; i < length; i++)
                pairs[i] = (objs[i], probabilities[i % length]);
            return this.NextObject<T>(pairs);
        }

        /// <summary> 随机取对象（指定占比） </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="pairs"> 对象-占比对数组 </param>
        public T NextObject<T>(params (T, int)[] pairs)
        {
            int length = pairs.Length;
            int sum = 0;
            for (int i = 0; i < length; i++)
                sum += pairs[i].Item2;
            int idx = (int)this.next(0.0d, sum);
            int min = 0, max = 0;
            for (int i = 0; i < length; i++)
            {
                min = max;
                max += pairs[i].Item2;
                if (min <= idx && idx < max)
                    return pairs[i].Item1;
            }
            return default(T);
        }

        /// <summary> 随机取对象（指定占比） </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="objs"> 对象数组 </param>
        /// <param Name="shares"> 占比数组（若Length少于对象数组，则会循环取概率） </param>
        public T NextObject<T>(T[] objs, int[] shares)
        {
            int length = objs.Length;
            (T, int)[] pairs = new (T, int)[length];
            for (int i = 0; i < length; i++)
                pairs[i] = (objs[i], shares[i % length]);
            return this.NextObject<T>(pairs);
        }

        /// <summary> 泛用随机数（[0,1) * mul + add） </summary>
        /// <param Name="add"> 追加数 </param>
        /// <param Name="mul"> 追乘数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static double Next(double add = 0.0d, double mul = 1.0d, LKRandom generator = null) => (generator ?? LKRandom.Instance).next(add, mul);

        /// <summary> 随机8位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static byte NextByte(byte min = byte.MinValue, byte max = byte.MaxValue, LKRandom generator = null) => (generator ?? LKRandom.Instance).NextByte(min, max);

        /// <summary> 随机8位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static sbyte NextUbyte(sbyte min = sbyte.MinValue, sbyte max = sbyte.MaxValue, LKRandom generator = null) => (generator ?? LKRandom.Instance).NextSByte(min, max);

        /// <summary> 随机16位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static short NextShort(short min = short.MinValue, short max = short.MaxValue, LKRandom generator = null) => (generator ?? LKRandom.Instance).NextShort(min, max);

        /// <summary> 随机16位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static ushort NextUShort(ushort min = ushort.MinValue, ushort max = ushort.MaxValue, LKRandom generator = null) => (generator ?? LKRandom.Instance).NextUShort(min, max);
        
        /// <summary> 随机32位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static int NextInt(int min = int.MinValue, int max = int.MaxValue, LKRandom generator = null) => (generator ?? LKRandom.Instance).NextInt(min, max);

        /// <summary> 随机32位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static uint NextUInt(uint min = uint.MinValue, uint max = uint.MaxValue, LKRandom generator = null) => (generator ?? LKRandom.Instance).NextUInt(min, max);
        
        /// <summary> 随机64位带符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static long NextLong(long min = long.MinValue, long max = long.MaxValue, LKRandom generator = null) => (generator ?? LKRandom.Instance).NextLong(min, max);

        /// <summary> 随机64位无符号整型数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static ulong NextULong(ulong min = ulong.MinValue, ulong max = ulong.MaxValue, LKRandom generator = null) => (generator ?? LKRandom.Instance).NextULong(min, max);

        /// <summary> 随机单精度浮点数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static float NextFloat(float min = float.MinValue, float max = float.MaxValue, LKRandom generator = null) => (generator ?? LKRandom.Instance).NextFloat(min, max);

        /// <summary> 随机双精度浮点数 </summary>
        /// <param Name="min"> 最小随机数 </param>
        /// <param Name="max"> 最大随机数 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static double NextDouble(double min = double.MinValue, double max = double.MaxValue, LKRandom generator = null) => (generator ?? LKRandom.Instance).NextDouble(min, max);

        /// <summary> 随机布尔值 </summary>
        /// <param Name="denominator"> 分母（返回true的概率为：1/denominator * 100%） </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static bool NextBool(int denominator, LKRandom generator = null) => (generator ?? LKRandom.Instance).NextBool(denominator);

        /// <summary> 随机布尔值 </summary>
        /// <param Name="probability"> 百分比概率（返回true的可能性为：probability * 100%） </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static bool NextBool(double probability = 0.5d, LKRandom generator = null) => (generator ?? LKRandom.Instance).NextBool(probability);

        /// <summary> 随机取单个字符 </summary>
        /// <param Name="str"> 源字符串 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static char NextChar(string str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", LKRandom generator = null) => (generator ?? LKRandom.Instance).NextChar(str);

        /// <summary> 随机取对象 </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="objs"> 对象数组 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static T NextObject<T>(LKRandom generator = null, params T[] objs) => (generator ?? LKRandom.Instance).NextObject<T>(objs);

        /// <summary> 随机取对象（指定概率） </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="pairs"> 对象-概率对数组 </param>
        /// <param Name="generator"> 生成器（null则使用默认生成器） </param>
        public static T NextObject<T>(LKRandom generator = null, params (T, double)[] pairs) => (generator ?? LKRandom.Instance).NextObject<T>(pairs);

        /// <summary> 随机取对象（指定概率） </summary>
        /// <typeparam Name="T"> 对象类型 </typeparam>
        /// <param Name="objs"> 对象数组 </param>
        /// <param Name="probabilities"> 概率数组（若Length少于对象数组，则会循环取概率） </param>
        public static T NextObject<T>(T[] objs, double[] probabilities, LKRandom generator = null) => (generator ?? LKRandom.Instance).NextObject<T>(objs, probabilities);
    }
}
