using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace images_noise
{
    public class FFTHelp
    {
        /// <summary>
        /// 快速傅里叶变换（当信号源长度等于2^N时，结果与dft相同，当长度不等于2^N时，先在尾部补零，所以计算结果与dft不同）
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] fft(double[] array)
        {
            array = FillArray(array);
            int N = array.Length;

            //生成WN表，以免运行时进行重复计算
            Complex[] WN = new Complex[N];
            for (int i = 0; i < N / 2; i++)
            {
                WN[i] = new Complex(Math.Cos(2 * Math.PI / N * i), -1 * Math.Sin(2 * Math.PI / N * i));
            }

            int stageNum = ReLog2N(N);
            int[] stage = new int[stageNum];
            stage[0] = 0;
            for (int i = 1; i < stageNum; i++)
            {
                stage[i] = Convert.ToInt32(Math.Round(Math.Pow(2, stageNum - 1 - i)));
            }

            //重排数据
            Complex[] Register = new Complex[N];
            for (int i = 0; i < N; i++)
            {
                int index = ReArrange(i, stageNum);
                Register[i] = new Complex(array[index], 0);
            }

            //蝶形运算
            Complex[] p = new Complex[N];
            Complex[] q = new Complex[N];
            Complex[] w = new Complex[N];
            int group = N;
            for (int i = 0; i < stageNum; i++)
            {
                group = group >> 1;
                int subnum = N / group;

                for (int k = 0; k < group; k++)
                {
                    for (int n = 0; n < subnum / 2; n++)
                    {
                        p[k * subnum + n] = p[k * subnum + n + subnum / 2] = Register[k * subnum + n];
                        w[k * subnum + n] = WN[stage[i] * n];
                    }

                    for (int n = subnum / 2; n < subnum; n++)
                    {
                        q[k * subnum + n] = q[k * subnum + n - subnum / 2] = Register[k * subnum + n];
                        w[k * subnum + n] = -1 * w[n - subnum / 2];
                    }
                }

                for (int k = 0; k < N; k++)
                {
                    Register[k] = p[k] + w[k] * q[k];
                }

            }

            double[] dest = new double[N];
            for (int k = 0; k < N; k++)
            {
                dest[k] = Register[k].Modulus();
            }
            return dest;
        }
        /// <summary>
        /// 离散傅里叶变换
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] dft(double[] array)
        {
            //array = FillArray(array);
            int N = array.Length;

            //重排数据
            Complex[] Register = new Complex[N];
            Complex[] dest = new Complex[N];
            for (int i = 0; i < N; i++)
            {
                Register[i] = new Complex(array[i], 0);
            }
            for (int k = 0; k < N; k++)
            {
                Complex sum = new Complex();
                for (int n = 0; n < N; n++)
                {
                    sum += Register[n] * (new Complex(Math.Cos(2 * Math.PI / N * n * k), -1 * Math.Sin(2 * Math.PI / N * n * k)));
                }
                dest[k] = sum;
            }

            double[] dest2 = new double[N];
            for (int k = 0; k < N; k++)
            {
                dest2[k] = dest[k].Modulus();
            }
            return dest2;
        }
        /// <summary>
        /// 离散傅里叶逆变换
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] idft(double[] array)
        {
            //array = FillArray(array);
            int N = array.Length;

            //重排数据
            Complex[] Register = new Complex[N];
            Complex[] dest = new Complex[N];
            for (int i = 0; i < N; i++)
            {
                Register[i] = new Complex(array[i], 0);
            }
            for (int k = 0; k < N; k++)
            {
                Complex sum = new Complex();
                for (int n = 0; n < N; n++)
                {
                    sum += Register[n] * (new Complex(Math.Cos(2 * Math.PI / N * n * k), -1 * Math.Sin(2 * Math.PI / N * n * k)));
                }
                dest[k] = sum / N;
            }

            double[] dest2 = new double[N];
            for (int k = 0; k < N; k++)
            {
                dest2[k] = dest[k].Modulus();
            }
            return dest2;
        }
        /// <summary>
        /// 离散傅里叶变换
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static Complex[] dft(Complex[] array)
        {
            //array = FillArray(array);
            int N = array.Length;

            //重排数据
            Complex[] Register = array;
            Complex[] dest = new Complex[N];
            //for (int i = 0; i < N; i++)
            //{
            //    Register[i] = new Complex(array[i], 0);
            //}
            for (int k = 0; k < N; k++)
            {
                Complex sum = new Complex();
                for (int n = 0; n < N; n++)
                {
                    sum += Register[n] * (new Complex(Math.Cos(2 * Math.PI / N * n * k), -1 * Math.Sin(2 * Math.PI / N * n * k)));
                }
                dest[k] = sum;
            }

            //double[] dest2 = new double[N];
            //for (int k = 0; k < N; k++)
            //{
            //    dest2[k] = dest[k].Modulus();
            //}
            //return dest2;
            return dest;
        }

        /// <summary>
        /// 离散傅里叶逆变换
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static Complex[] idft(Complex[] array)
        {
            //array = FillArray(array);
            int N = array.Length;

            //重排数据
            Complex[] Register = array;
            Complex[] dest = new Complex[N];
            //for (int i = 0; i < N; i++)
            //{
            //    Register[i] = new Complex(array[i], 0);
            //}
            for (int k = 0; k < N; k++)
            {
                Complex sum = new Complex();
                for (int n = 0; n < N; n++)
                {
                    sum += Register[n] * (new Complex(Math.Cos(-2 * Math.PI / N * n * k), -1 * Math.Sin(-2 * Math.PI / N * n * k)));
                }
                dest[k] = sum / N;
            }

            //double[] dest2 = new double[N];
            //for (int k = 0; k < N; k++)
            //{
            //    dest2[k] = dest[k].Modulus();
            //}
            //return dest2;
            return dest;
        }

        /// <summary>
        /// 利用傅里叶变换实现的低通滤波（消除高频信号）
        /// </summary>
        /// <param name="array">信号源</param>
        /// <param name="n">截止频率</param>
        /// <returns></returns>
        public static double[] FFTFilter(double[] array, int n)
        {
            if (array == null || n <= 0 || array.Length <= n)
                return array;

            int N = array.Length;
            Complex[] Register = new Complex[N];
            for (int i = 0; i < N; i++)
            {
                Register[i] = new Complex(array[i], 0);
            }
            Complex[] dest = dft(Register);
            for (int i = 0; i < dest.Length; i++)
            {
                if (i > n && i < dest.Length - n)
                    dest[i].Im = dest[i].Re = 0;
            }
            Complex[] dest2 = idft(dest);

            double[] result = new double[dest2.Length];
            for (int i = 0; i < dest2.Length; i++)
            {
                result[i] = dest2[i].Modulus();
            }
            return result;
        }
        private static double[] FillArray(double[] array)
        {
            //补零后长度
            int relog2N = ReLog2N(array.Length);

            int bitlenghth = relog2N;
            int N = 0x01 << bitlenghth;
            double[] ret = new double[N];
            for (int i = 0; i < N; i++)
            {
                if (i < array.Length)
                    ret[i] = array[i];
                else ret[i] = 0;
            }
            return ret;
        }

        // 获取扩展长度后的幂次
        // 由于fft要求长度为2^n，所以用此函数来获取所需长度
        public static int ReLog2N(int count)
        {
            int log2N = 0;
            uint mask = 0x80000000;
            for (int i = 0; i < 32; i++)
            {
                if (0 != ((mask >> i) & count))
                {
                    if ((mask >> i) == count) log2N = 31 - i;
                    else log2N = 31 - i + 1;
                    break;
                }
            }
            return log2N;
        }

        // 获取按位逆序，bitlenght为数据长度
        // fft函数内使用
        private static int ReArrange(int dat, int bitlenght)
        {
            int ret = 0;
            for (int i = 0; i < bitlenght; i++)
            {
                if (0 != (dat & (0x01 << i))) ret |= ((0x01 << (bitlenght - 1)) >> i);
            }
            return ret;
        }
    }
    /// <summary>
    /// 表示一个复数
    /// </summary>
    public class Complex
    {
        public double Re;
        public double Im;
        public Complex()
        {
            Re = 0;
            Im = 0;
        }
        public Complex(double re)
        {
            Re = re;
            Im = 0;
        }
        public Complex(double re, double im)
        {
            Re = re;
            Im = im;
        }

        public double Modulus()
        {
            return Math.Sqrt(Re * Re + Im * Im);
        }

        public override string ToString()
        {
            string retStr;
            if (Math.Abs(Im) < 0.0001) retStr = Re.ToString("f4");
            else if (Math.Abs(Re) < 0.0001)
            {
                if (Im > 0) retStr = "j" + Im.ToString("f4");
                else retStr = "- j" + (0 - Im).ToString("f4");
            }
            else
            {
                if (Im > 0) retStr = Re.ToString("f4") + "+ j" + Im.ToString("f4");
                else retStr = Re.ToString("f4") + "- j" + (0 - Im).ToString("f4");
            }
            retStr += " ";
            return retStr;
        }

        //操作符重载
        public static Complex operator +(Complex c1, Complex c2)
        {
            return new Complex(c1.Re + c2.Re, c1.Im + c2.Im);
        }
        public static Complex operator +(double d, Complex c)
        {
            return new Complex(d + c.Re, c.Im);
        }
        public static Complex operator -(Complex c1, Complex c2)
        {
            return new Complex(c1.Re - c2.Re, c1.Im - c2.Im);
        }
        public static Complex operator -(double d, Complex c)
        {
            return new Complex(d - c.Re, -c.Im);
        }
        public static Complex operator *(Complex c1, Complex c2)
        {
            return new Complex(c1.Re * c2.Re - c1.Im * c2.Im, c1.Re * c2.Im + c2.Re * c1.Im);
        }
        public static Complex operator *(Complex c, double d)
        {
            return new Complex(c.Re * d, c.Im * d);
        }
        public static Complex operator *(double d, Complex c)
        {
            return new Complex(c.Re * d, c.Im * d);
        }
        public static Complex operator /(Complex c, double d)
        {
            return new Complex(c.Re / d, c.Im / d);
        }
        public static Complex operator /(double d, Complex c)
        {
            double temp = d / (c.Re * c.Re + c.Im * c.Im);
            return new Complex(c.Re * temp, -c.Im * temp);
        }
        public static Complex operator /(Complex c1, Complex c2)
        {
            double temp = 1 / (c2.Re * c2.Re + c2.Im * c2.Im);
            return new Complex((c1.Re * c2.Re + c1.Im * c2.Im) * temp, (-c1.Re * c2.Im + c2.Re * c1.Im) * temp);
        }
    }

}
