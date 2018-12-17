using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace images_noise
{
    class fftt
    {
        //寻找最小2次幂值具体函数如下：
        public int find2n(int num)
        {
            double b = Math.Log(num, 2);
            int c = (int)(Math.Log(num, 2));
            if (b - c != 0)
            {
                num = (int)Math.Pow(2, (int)c + 1);
            }
            return num;
        }
        
        //imageWidth和imageHeight为原图的宽度和长度；Width和Height为需拓展的宽度和长度
        public byte[] expand(byte[] image, int imageWidth, int imageHeight, int Width, int Height)
        {
            byte[,] matrix = new byte[Height, Width];
            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    matrix[i, j] = image[i * imageWidth + j];
                }
            }

            for (int i = imageHeight; i < Height; i++)
            {
                for (int j = imageWidth; j < Width; j++)
                {
                    matrix[i, j] = 0;
                }
            }
            byte[] result = new byte[Width * Height];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    result[i * Width + j] = matrix[i, j];
                }
            }
            return result;
        }
        
        //图像傅里叶变换************************
        public Complex[] FFT(byte[] imageData, int imageWidth, int imageHeight)
        {
            int bytes = imageWidth * imageHeight;
            byte[] bmpValues = new byte[bytes];
            Complex[] tempCom1 = new Complex[bytes];

            bmpValues = (byte[])imageData.Clone();

            //赋值：把实数变为复数，即虚部为0
            for (int i = 0; i < bytes; i++)
            {
                tempCom1[i] = new Complex(bmpValues[i], 0);
            }
            //水平方向快速傅里叶变换
            Complex[] tempCom2 = new Complex[imageWidth];
            Complex[] tempCom3 = new Complex[imageWidth];
            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    tempCom2[j] = tempCom1[i * imageWidth + j];
                }
                //调用一维傅里叶变换
                tempCom3 = fft(tempCom2, imageWidth);



                Complex[] tempCom9 = new Complex[imageWidth];
                for (int j = 0; j < imageWidth; j++)
                {
                    tempCom9[j] = tempCom3[j];
                }

                //将结果赋值回去
                for (int j = 0; j < imageWidth; j++)
                {
                    tempCom1[i * imageWidth + j] = tempCom3[j];
                }
            }
            //垂直方向傅里叶变换
            Complex[] tempCom4 = new Complex[imageHeight];
            Complex[] tempCom5 = new Complex[imageHeight];
            for (int i = 0; i < imageWidth; i++)
            {
                for (int j = 0; j < imageHeight; j++)
                {
                    tempCom4[j] = tempCom1[j * imageWidth + i];
                }
                //调用一维傅里叶变换
                tempCom5 = fft(tempCom4, imageHeight);

                //把结果赋值回去
                for (int j = 0; j < imageHeight; j++)
                {
                    tempCom1[j * imageWidth + i] = tempCom5[j];
                }
            }
            return tempCom1;
        }

        /// <summary>
        /// 二维快速傅里叶变换
        /// </summary>
        /// <param name="imageData">图像序列</param>
        /// <param name="imageWidth">图像宽度</param>
        /// <param name="imageHeight">图像长度</param>
        /// <param name="inv">标识是否进行坐标位移变换</param>
        /// <returns></returns>
        private Complex[] fft2(byte[] imageData, int imageWidth, int imageHeight, bool inv)
        {
            int bytes = imageWidth * imageHeight;
            byte[] bmpValues = new byte[bytes];
            Complex[] tempCom1 = new Complex[bytes];
            bmpValues = (byte[])imageData.Clone();

            //赋值:把实数变为复数,即虚部为0
            for (int i = 0; i < bytes; i++)
            {
                if (inv == true)
                {
                    //进行频域坐标位移
                    if ((i / imageWidth + i % imageWidth) % 2 == 0)
                    {
                        tempCom1[i] = new Complex(bmpValues[i], 0);
                    }
                    else
                    {
                        tempCom1[i] = new Complex(-bmpValues[i], 0);
                    }

                }
                else
                {
                    // 不进行频域坐标位移
                    tempCom1[i] = new Complex(bmpValues[i], 0);
                }

            }

            //水平方向快速傅里叶变换
            Complex[] tempCom2 = new Complex[imageWidth];
            Complex[] tempCom3 = new Complex[imageWidth];
            for (int i = 0; i < imageHeight; i++)
            {
                //得到水平方向复数序列
                for (int j = 0; j < imageWidth; j++)
                {
                    tempCom2[j] = tempCom1[i * imageWidth + j];
                }
                //调用一维FFT
                tempCom3 = fft(tempCom2, imageWidth);
                //把结果赋值回去
                for (int j = 0; j < imageWidth; j++)
                {
                    tempCom1[i * imageWidth + j] = tempCom3[j];

                }

            }
            //垂直方向快速FFT

            Complex[] tempCom4 = new Complex[imageHeight];
            Complex[] tempCom5 = new Complex[imageHeight];
            for (int i = 0; i < imageWidth; i++)
            {
                //得到水平方向复数序列
                for (int j = 0; j < imageHeight; j++)
                {
                    tempCom4[j] = tempCom1[j * imageWidth + i];
                }
                //调用一维FFT
                tempCom5 = fft(tempCom4, imageHeight);
                //把结果赋值回去
                for (int j = 0; j < imageHeight; j++)
                {
                    tempCom1[j * imageHeight + i] = tempCom5[j];
                }
            }
            return tempCom1;
        }

        /// <summary>
        /// 二维快速傅里叶逆变换
        /// </summary>
        /// <param name="freData">频数据</param>
        /// <param name="imageWidth">图像宽度</param>
        /// <param name="imageHeight">图像长度</param>
        /// <param name="inv">标识是否进行坐标位移变换,要与二维FFT变换一致</param>
        /// <returns></returns>
        private byte[] ifft2(Complex[] freData, int imageWidth, int imageHeight, bool inv)
        {
            int bytes = imageWidth * imageHeight;
            byte[] bmpValues = new byte[bytes];
            Complex[] tempCom1 = new Complex[bytes];

            tempCom1 = (Complex[])freData.Clone();
            //水平方向FFT变换
            Complex[] tempCom2 = new Complex[imageWidth];
            Complex[] tempCom3 = new Complex[imageWidth];
            for (int i = 0; i < imageHeight; i++)//水平方向
            {
                //得到水平方向复数序列
                for (int j = 0; j < imageWidth; j++)
                {
                    tempCom2[j] = tempCom1[i * imageWidth + j];
                }
                //调用一维傅里变换
                tempCom3 = ifft(tempCom2, imageWidth);
                //把结果赋值回去
                for (int j = 0; j < imageWidth; j++)
                {
                    tempCom1[i * imageWidth + j] = tempCom3[j];
                }
            }
            //垂直方向FFT变换
            Complex[] tempCom4 = new Complex[imageHeight];
            Complex[] tempCom5 = new Complex[imageHeight];
            for (int i = 0; i < imageWidth; i++)//垂直方向
            {
                //得到垂直方向复数序列
                for (int j = 0; j < imageHeight; j++)
                {
                    tempCom4[j] = tempCom1[j * imageWidth + i];
                }
                //调用一维傅里叶变换
                tempCom5 = ifft(tempCom4, imageHeight);
                //把结果赋值回去
                for (int j = 0; j < imageHeight; j++)
                {
                    tempCom1[j * imageHeight + i] = tempCom5[j];
                }
            }
            //赋值:把复数转换为实数,只保留复数的实数部分
            double tempDouble;
            for (int i = 0; i < bytes; i++)
            {
                if (inv == true)
                {
                    //进行坐标位移
                    if ((i / imageWidth + i % imageHeight) % 2 == 0)
                    {
                        tempDouble = tempCom1[i].Real;
                    }
                    else
                    {
                        tempDouble = -tempCom1[i].Real;
                    }
                }
                else
                {
                    //不进行坐标位移
                    tempDouble = tempCom1[i].Real;
                }

                if (tempDouble > 255)
                {
                    bmpValues[i] = 255;
                }
                else
                {
                    if (tempDouble < 0)
                    {
                        bmpValues[i] = 0;
                    }
                    else
                    {
                        bmpValues[i] = Convert.ToByte(tempDouble);
                    }
                }
            }

            return bmpValues;
        }

        //一维傅里叶变换
        private Complex[] fft(Complex[] sourceData, int countN)
        {
            //fft的级数
            int r = Convert.ToInt32(Math.Log(countN, 2));

            Complex[] w = new Complex[countN / 2];
            Complex[] interVar1 = new Complex[countN];
            Complex[] interVar2 = new Complex[countN];

            interVar1 = (Complex[])sourceData.Clone(); 

            //求加权系数w
            for (int i = 0; i < countN / 2; i++)
            {
                double angle = -i * Math.PI * 2 / countN;
                w[i] = new Complex(Math.Cos(angle), Math.Sin(angle));
            }

            //蝶形运算
            for (int i = 0; i < r; i++)
            {
                int interval = 1 << i;
                int halfN = 1 << (r - i);
                //对每级的每一组点循环
                for (int j = 0; j < interval; j++)
                {
                    int gap = j * halfN;
                    //对每级的每一点循环
                    for (uint k = 0; k < halfN / 2; k++)
                    {
                        //运行蝶形算法
                        interVar2[k + gap] = interVar1[k + gap] + interVar1[k + gap + halfN / 2];
                        interVar2[k + gap + halfN / 2] = (interVar1[k + gap] - interVar1[k + gap + halfN / 2]) * w[k * interval];
                    }
                }
                interVar1 = (Complex[])interVar2.Clone();
            }

            //按位取反
            for (uint j = 0; j < countN; j++)
            {
                uint rev = 0;
                uint num = j;
                //重新排序
                for (uint i = 0; i < r; i++)
                {
                    rev <<= 1;
                    rev |= num & 1;
                    num >>= 1;
                }
                interVar2[rev] = interVar1[j];
            }
            return interVar2;
        }
        
        //傅里叶反变换************************
        private double[] IFFT(Complex[] freData, int imageWidth, int imageHeight)
        {
            int bytes = imageWidth * imageHeight;
            byte[] bmpValues = new byte[bytes];
            Complex[] tempCom1 = new Complex[bytes];

            tempCom1 = (Complex[])freData.Clone();

            //水平方向傅里叶逆变化
            Complex[] tempCom2 = new Complex[imageWidth];
            Complex[] tempCom3 = new Complex[imageWidth];
            for (int i = 0; i < imageHeight; i++)
            {
                //得到水平方向复数序列
                {
                    for (int j = 0; j < imageWidth; j++)
                    {
                        tempCom2[j] = tempCom1[i * imageWidth + j];
                    }
                    //调用一维傅里叶变换
                    tempCom3 = ifft(tempCom2, imageWidth);
                    //把结果赋值回去
                    for (int j = 0; j < imageWidth; j++)
                    {
                        tempCom1[i * imageWidth + j] = tempCom3[j];
                    }
                }
            }
            //垂直方向傅里叶逆变换
            Complex[] tempCom4 = new Complex[imageHeight];
            Complex[] tempCom5 = new Complex[imageHeight];
            for (int i = 0; i < imageWidth; i++)
            {
                //得到垂直方向复数序列
                for (int j = 0; j < imageHeight; j++)
                {
                    tempCom4[j] = tempCom1[j * imageWidth + i];
                }
                //调用一维傅里叶变换
                tempCom5 = ifft(tempCom4, imageHeight);
                //把结果赋值回去
                for (int j = 0; j < imageHeight; j++)
                {
                    tempCom1[j * imageWidth + i] = tempCom5[j];
                }
            }
            //赋值：把复数转换为实数，只保留复数的实数部分
            double[] tempDouble = new double[bytes];
            for (int i = 0; i < bytes; i++)
            {
                tempDouble[i] = tempCom1[i].Real;
            }
            return tempDouble;
        }
        
        //一维傅里叶反变换
        private Complex[] ifft(Complex[] sourceData, int countN)
        {
            //共轭变换
            for (uint i = 0; i < countN; i++)
            {
                sourceData[i] = sourceData[i].conjugate();
            }

            Complex[] interVar = new Complex[countN];
            //调用快速傅里叶变换
            interVar = fft(sourceData, countN);
            //共轭变换，并除以长度
            for (uint i = 0; i < countN; i++)
            {
                interVar[i] = new Complex(interVar[i].Real / countN, -interVar[i].Imaginary / countN);
            }
            return interVar;
        }

        //对图片进行低通滤波
        public byte[] lowpass(byte[] imageData, int imageWidth, int imageHeight,int n,int flag)
        {
            int new_width = this.find2n(imageWidth);
            int new_height = this.find2n(imageHeight);
            byte[] new_image = this.expand(imageData, imageWidth, imageHeight, new_width, new_height);
            Complex[] ximg = fft2(new_image, new_width, new_height,true);

            int minLen = Math.Min(new_width, new_height);
            double radius = n * minLen / 100;

            //滤波
            for (int i = 0; i < new_height; i++)
            {
                for (int j = 0; j < new_width; j++)
                {
                    //当前像素点到图像几何中心的距离
                    double distance = (double)(j - new_width / 2) *
                        (j - new_width / 2) + (i - new_height / 2) *
                        (i - new_height / 2);
                    distance = Math.Sqrt(distance);

                    switch (flag)
                    {
                        case 0:
                            //低通滤波
                            if (distance > radius)
                            {
                                ximg[i * new_width + j] = new Complex(0.0, 0.0);
                            }
                            break;
                        case 1:
                            //带阻滤波
                            if (distance < radius)
                            {
                                ximg[i * new_width + j] = new Complex(0.0, 0.0);
                            }
                            break;
                        default:
                            break;

                    }
                }
            }
            return ifft2(ximg, new_width, new_height,true);
        }
    }
    class Complex
    {
        //复数的实部
        private double real = 0.0;
        //复数的虚部
        private double imaginary = 0.0;
        //实部的属性
        public double Real
        {
            get
            {
                return real;
            }
            set
            {
                real = value;
            }
        }
        //虚部的属性
        public double Imaginary
        {
            get
            {
                return imaginary;
            }
            set
            {
                imaginary = value;
            }
        }
        //基本构造函数
        public Complex()
        {
        }
        //指定值得构造函数
        public Complex(double dbreal, double dbimag)
        {
            real = dbreal;
            imaginary = dbimag;
        }
        //复制构造函数
        public Complex(Complex other)
        {
            real = other.real;
            imaginary = other.imaginary;
        }
        //重载+运算符
        public static Complex operator +(Complex comp1, Complex comp2)
        {
            return comp1.Add(comp2);
        }
        //重载-运算符
        public static Complex operator -(Complex compl, Complex comp2)
        {
            return compl.Subtract(comp2);
        }
        //重载*运算符
        public static Complex operator *(Complex compl, Complex comp2)
        {
            return compl.Multiply(comp2);
        }
        //实现复数加法
        public Complex Add(Complex comp)
        {
            double x = real + comp.real;
            double y = imaginary + comp.imaginary;
            return new Complex(x, y);
        }
        //实现复数减法
        public Complex Subtract(Complex comp)
        {
            double x = real - comp.real;
            double y = imaginary - comp.imaginary;
            return new Complex(x, y);
        }
        //实现复数乘法
        public Complex Multiply(Complex comp)
        {
            double x = real * comp.real - imaginary * comp.imaginary;
            double y = real * comp.imaginary + imaginary * comp.real;
            return new Complex(x, y);
        }
        //求幅度
        public double Abs()
        {
            //取得实部的绝对值
            double x = Math.Abs(real);
            //取得虚部的绝对值
            double y = Math.Abs(imaginary);
            //实部为0
            if (real == 0)
            {
                return y;
            }
            //虚部为0
            if (imaginary == 0)
            {
                return x;
            }
            //计算模
            if (x > y)
            {
                return (x * Math.Sqrt(1 + (y / x) * (y / x)));
            }
            else
            {
                return (y * Math.Sqrt(1 + (y / x) * (y / x)));
            }
        }
        //求相位角
        public double Angle()
        {
            //实数和虚数都为0
            if (real == 0 && imaginary == 0)
                return 0;
            if (real == 0)
            {
                //实部位0
                if (imaginary > 0)
                    return Math.PI / 2;
                else
                    return -Math.PI / 2;
            }
            else
            {
                if (real > 0)
                {
                    //实部大于0
                    return Math.Atan2(imaginary, real);
                }
                else
                {
                    //实部小于0
                    if (imaginary >= 0)
                        return Math.Atan2(imaginary, real) + Math.PI;
                    else
                        return Math.Atan2(imaginary, real) - Math.PI;
                }
            }
        }
        //共轭复数
        public Complex conjugate()
        {
            return new Complex(this.real, -this.imaginary);
        }
    }
}
