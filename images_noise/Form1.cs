using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace images_noise
{
    public partial class Form1 : Form
    {
        public double Pa=0.05, Pb=0.05;
        public string path;
        int change = 1;
        public int k = 64;//定义高斯噪声的程度
        private static int kk = 273;//定义卷积核的程度
        public double[,] GaussianBlur = new double[5, 5] {{(double)1/kk,(double)4/kk,(double)7/kk,(double)4/kk,(double)1/kk},
                                                        {(double)4/kk,(double)16/kk,(double)26/kk,(double)16/kk,(double)4/kk},
                                                        {(double)7/kk,(double)26/kk,(double)41/kk,(double)26/kk,(double)7/kk},
                                                        {(double)4/kk,(double)16/kk,(double)26/kk,(double)16/kk,(double)4/kk},
                                                        {(double)1/kk,(double)4/kk,(double)7/kk,(double)4/kk,(double)1/kk}};//声明高斯模糊卷积核函数
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog imageDialog = new OpenFileDialog();
            imageDialog.Filter = "图片|*.jpg;*.png;*.gif;*.jpeg;*.bmp";
            imageDialog.Title = "选择照片";
            if (imageDialog.ShowDialog() == DialogResult.OK)
            {
                path = imageDialog.FileName;
                pictureBox1.ImageLocation = imageDialog.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(path == null)
            {
                return;
            }
            AddSalt(path);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (path == null)
            {
                return;
            }
            pictureBox1.ImageLocation = path;
        }
        //另存为
        private void button12_Click(object sender, EventArgs e)
        {
            if (path == null)
            {
                return;
            }
            string cur_path = Path.GetDirectoryName(path);
            string nfile = Path.ChangeExtension(path, ".jpg");
            string name = Path.GetFileName(nfile);
            pictureBox1.Image.Save(cur_path+"\\"+change.ToString() + name, System.Drawing.Imaging.ImageFormat.Jpeg);
            MessageBox.Show("另存为成功");
            change++;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (path == null)
            {
                return;
            }
            AddGaosi(path);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (path == null)
            {
                return;
            }
            Bitmap picc = (Bitmap)pictureBox1.Image;
            pictureBox1.Image = mean(picc);
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (path == null)
            {
                return;
            }
            Bitmap picc = (Bitmap)pictureBox1.Image;
            pictureBox1.Image = Smooth(picc);
        }
        private void button8_Click(object sender, EventArgs e)
        {
            if (path == null)
            {
                return;
            }
            Bitmap picc = (Bitmap)pictureBox1.Image;
            pictureBox1.Image = middle(picc);
        }
        private void button9_Click(object sender, EventArgs e)
        {
            if (path == null)
            {
                return;
            }
            Bitmap picc = (Bitmap)pictureBox1.Image;
            pictureBox1.Image = Sharp(picc);
        }
        private void button10_Click(object sender, EventArgs e)
        {
            if (path == null)
            {
                return;
            }
            Bitmap picc = (Bitmap)pictureBox1.Image;
            pictureBox1.Image = lowpassed(picc, Convert.ToInt16(this.textBox1.Text), 0);
        }
        private void button11_Click(object sender, EventArgs e)
        {
            if (path == null)
            {
                return;
            }
            Bitmap picc = (Bitmap)pictureBox1.Image;
            pictureBox1.Image = lowpassed(picc,Convert.ToInt16(this.textBox2.Text),1);
        }
        private void AddSalt(string img_path)
        {
            if (true)
            {
                Bitmap pic = (Bitmap)Bitmap.FromFile(img_path, false);
                //Bitmap pic = new Bitmap(pictureBox1.Image, WI, HE);
                Pa = Convert.ToDouble(Pa);//接受输入的Pa
                Pb = Convert.ToDouble(Pb);//接受输入的Pb
                double P = Pb / (1 - Pa);//程序要,为了得到一个概率Pb事件
                int width = pic.Width;
                int height = pic.Height;
                Random rand = new Random();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int gray;
                        int noise = 1;
                        double probility = rand.NextDouble();
                        if (probility < Pa)
                        {
                            noise = 255;//有Pa概率 噪声设为最大值
                        }
                        else
                        {
                            double temp = rand.NextDouble();
                            if (temp < Pb)//有1 - Pa的几率到达这里，再乘以 P ，刚好等于Pb
                                noise = 0;
                        }
                        if (noise != 1)
                        {
                            gray = noise;
                        }
                        else
                        {
                            //gray = pic.GetPixel(j, i).R;//这里决定要不要将图片变成黑白
                            continue;
                        }
                        Color color = Color.FromArgb(gray, gray, gray);
                        pic.SetPixel(j, i, color);
                    }
                }
                pictureBox1.Image = pic;
            }
            else
            {

            }
        }
        static int GetRandomSeed() //产生随机种子
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        public double GaussNiose1()//用box muller的方法产生均值为0，方差为1的正太分布随机数
        {
            // Random ro = new Random(10);
            // long tick = DateTime.Now.Ticks;
            Random ran = new Random(GetRandomSeed());
            // Random rand = new Random();
            double r1 = ran.NextDouble();
            double r2 = ran.NextDouble();
            double result = Math.Sqrt((-2) * Math.Log(r2)) * Math.Sin(2 * Math.PI * r1);
            return result;//返回随机数
        }

        public void AddGaosi(string path)
        {
            //Bitmap picc = (Bitmap)pictureBox1.Image;
            //double ran = GaussNiose1();
            Bitmap pic = (Bitmap)Bitmap.FromFile(path, false);
            int width = pic.Width;
            int height = pic.Height;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int red,green,blue;
                    red = pic.GetPixel(j, i).R+ (int)(GaussNiose1()*k);
                    green = pic.GetPixel(j, i).G+ (int)(GaussNiose1()*k);
                    blue = pic.GetPixel(j, i).B+ (int)(GaussNiose1()*k);
                    red = between_0_255(red);
                    blue = between_0_255(blue);
                    green = between_0_255(green);
                    Color color = Color.FromArgb(red, green, blue);
                    pic.SetPixel(j, i, color);
                }
            }
            pictureBox1.Image = pic;
        }
        public int between_0_255(int color)
        {
            if (color < 0)
                color = 0;
            if (color > 255)
                color = 255;
            return color;
        }

        
        //均值滤波
        public Bitmap mean(Bitmap bitmap)
        {
            int[,,] InputPicture = new int[3, bitmap.Width, bitmap.Height];//以GRB以及位图的长宽建立整数输入的位图的数组

            Color color = new Color();//储存某一像素的颜色
            //循环使得InputPicture数组得到位图的RGB
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    color = bitmap.GetPixel(i, j);
                    InputPicture[0, i, j] = color.R;
                    InputPicture[1, i, j] = color.G;
                    InputPicture[2, i, j] = color.B;
                }
            }

            //int[,,] OutputPicture = new int[3, bitmap.Width, bitmap.Height];//以GRB以及位图的长宽建立整数输出的位图的数组
            Bitmap smooth = new Bitmap(bitmap.Width, bitmap.Height);//创建新位图
            //循环计算使得OutputPicture数组得到计算后位图的RGB
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    int R = 0;
                    int G = 0;
                    int B = 0;

                    //每一个像素计算使用均值进行计算
                    for (int r = 0; r < 3; r++)//循环均值矩阵的每一行
                    {
                        for (int f = 0; f < 3; f++)//循环均值矩阵的每一列
                        {
                            int row = i - 1 + r;
                            int index = j - 1 + f;

                            //当超出位图的大小范围时，选择最边缘的像素值作为该点的像素值
                            row = row < 0 ? 0 : row;
                            index = index < 0 ? 0 : index;
                            row = row >= bitmap.Width ? bitmap.Width - 1 : row;
                            index = index >= bitmap.Height ? bitmap.Height - 1 : index;

                            //输出得到像素的RGB值
                            R += (int)(InputPicture[0, row, index]/9);
                            G += (int)(InputPicture[1, row, index]/9);
                            B += (int)(InputPicture[2, row, index]/9);
                        }
                    }
                    color = Color.FromArgb(R, G, B);//颜色结构储存该点RGB
                    smooth.SetPixel(i, j, color);//位图存储该点像素值
                }
            }
            return smooth;
        }
        /// <summary>
        /// 对图像进行平滑处理（利用高斯平滑Gaussian Blur）
        /// </summary>
        /// <param name="bitmap">要处理的位图</param>
        /// <returns>返回平滑处理后的位图</returns>
        /// 高斯滤波
        public Bitmap Smooth(Bitmap bitmap)
        {
            int[,,] InputPicture = new int[3, bitmap.Width, bitmap.Height];//以GRB以及位图的长宽建立整数输入的位图的数组

            Color color = new Color();//储存某一像素的颜色
            //循环使得InputPicture数组得到位图的RGB
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    color = bitmap.GetPixel(i, j);
                    InputPicture[0, i, j] = color.R;
                    InputPicture[1, i, j] = color.G;
                    InputPicture[2, i, j] = color.B;
                }
            }

            int[,,] OutputPicture = new int[3, bitmap.Width, bitmap.Height];//以GRB以及位图的长宽建立整数输出的位图的数组
            Bitmap smooth = new Bitmap(bitmap.Width, bitmap.Height);//创建新位图
            //循环计算使得OutputPicture数组得到计算后位图的RGB
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    int R = 0;
                    int G = 0;
                    int B = 0;

                    //每一个像素计算使用高斯模糊卷积核进行计算
                    for (int r = 0; r < 5; r++)//循环卷积核的每一行
                    {
                        for (int f = 0; f < 5; f++)//循环卷积核的每一列
                        {
                            //控制与卷积核相乘的元素
                            int row = i - 2 + r;
                            int index = j - 2 + f;

                            //当超出位图的大小范围时，选择最边缘的像素值作为该点的像素值
                            row = row < 0 ? 0 : row;
                            index = index < 0 ? 0 : index;
                            row = row >= bitmap.Width ? bitmap.Width - 1 : row;
                            index = index >= bitmap.Height ? bitmap.Height - 1 : index;

                            //输出得到像素的RGB值
                            R += (int)(GaussianBlur[r, f] * InputPicture[0, row, index]);
                            G += (int)(GaussianBlur[r, f] * InputPicture[1, row, index]);
                            B += (int)(GaussianBlur[r, f] * InputPicture[2, row, index]);
                        }
                    }
                    color = Color.FromArgb(R, G, B);//颜色结构储存该点RGB
                    smooth.SetPixel(i, j, color);//位图存储该点像素值
                }
            }
            return smooth;
        }
        
        //中值滤波
        public Bitmap middle(Bitmap bitmap)
        {
            int[,,] InputPicture = new int[3, bitmap.Width, bitmap.Height];//以GRB以及位图的长宽建立整数输入的位图的数组

            Color color = new Color();//储存某一像素的颜色
            //循环使得InputPicture数组得到位图的RGB
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    color = bitmap.GetPixel(i, j);
                    InputPicture[0, i, j] = color.R;
                    InputPicture[1, i, j] = color.G;
                    InputPicture[2, i, j] = color.B;
                }
            }

            //int[,,] OutputPicture = new int[3, bitmap.Width, bitmap.Height];//以GRB以及位图的长宽建立整数输出的位图的数组
            Bitmap smooth = new Bitmap(bitmap.Width, bitmap.Height);//创建新位图
            //循环计算使得OutputPicture数组得到计算后位图的RGB
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    int R = 0;
                    int G = 0;
                    int B = 0;

                    //记录int数组第几个像素点
                    int k = 0;

                    //用来存储9个像素值
                    int[] rm = new int[9];
                    int[] gm = new int[9];
                    int[] bm = new int[9];
                    //对9个像素点求中值
                    for (int r = 0; r < 3; r++)//循环均值矩阵的每一行
                    {
                        for (int f = 0; f < 3; f++)//循环均值矩阵的每一列
                        {
                            int row = i - 1 + r;
                            int index = j - 1 + f;
                            //当超出位图的大小范围时，选择最边缘的像素值作为该点的像素值
                            row = row < 0 ? 0 : row;
                            index = index < 0 ? 0 : index;
                            row = row >= bitmap.Width ? bitmap.Width - 1 : row;
                            index = index >= bitmap.Height ? bitmap.Height - 1 : index;

                            //存入int数组
                            rm[k] = InputPicture[0, row, index];
                            gm[k] = InputPicture[1, row, index];
                            bm[k] = InputPicture[2, row, index];
                            k++;
                        }
                    }
                    Array.Sort(rm);
                    Array.Sort(gm);
                    Array.Sort(bm);
                    R = rm[4];
                    G = gm[4];
                    B = bm[4];
                    color = Color.FromArgb(R, G, B);//颜色结构储存该点RGB
                    smooth.SetPixel(i, j, color);//位图存储该点像素值
                }
            }
            return smooth;
        }
        //锐化滤波
        public Bitmap Sharp(Bitmap bitmap)
        {
            int[,,] InputPicture = new int[3, bitmap.Width, bitmap.Height];//以GRB以及位图的长宽建立整数输入的位图的数组

            Color color = new Color();//储存某一像素的颜色
            //循环使得InputPicture数组得到位图的RGB
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    color = bitmap.GetPixel(i, j);
                    InputPicture[0, i, j] = color.R;
                    InputPicture[1, i, j] = color.G;
                    InputPicture[2, i, j] = color.B;
                }
            }
            int[] Laplace = { -1, -1, -1, -1, 9, -1, -1, -1, -1 };
            //int[] Laplace = { 1, 4, 1, 4, -20, 4, 1, 4, 1 };
            //int[,,] OutputPicture = new int[3, bitmap.Width, bitmap.Height];//以GRB以及位图的长宽建立整数输出的位图的数组
            Bitmap smooth = new Bitmap(bitmap.Width, bitmap.Height);//创建新位图
            //循环计算使得OutputPicture数组得到计算后位图的RGB
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    int R = 0;
                    int G = 0;
                    int B = 0;
                    int lap_loca = 0;
                    //每一个像素计算使用均值进行计算
                    for (int r = 0; r < 3; r++)//循环均值矩阵的每一行
                    {
                        for (int f = 0; f < 3; f++)//循环均值矩阵的每一列
                        {
                            int row = i - 1 + r;
                            int index = j - 1 + f;

                            //当超出位图的大小范围时，选择最边缘的像素值作为该点的像素值
                            row = row < 0 ? 0 : row;
                            index = index < 0 ? 0 : index;
                            row = row >= bitmap.Width ? bitmap.Width - 1 : row;
                            index = index >= bitmap.Height ? bitmap.Height - 1 : index;

                            //输出得到像素的RGB值
                            R += (int)(InputPicture[0, row, index] * Laplace[lap_loca]);
                            G += (int)(InputPicture[1, row, index] * Laplace[lap_loca]);
                            B += (int)(InputPicture[2, row, index] * Laplace[lap_loca]);
                            lap_loca++;
                        }
                    }
                    R = between_0_255(R);
                    G = between_0_255(G);
                    B = between_0_255(B);
                    color = Color.FromArgb(R, G, B);//颜色结构储存该点RGB
                    smooth.SetPixel(i, j, color);//位图存储该点像素值
                }
            }
            return smooth;
        }
        //均值滤波
        public Bitmap lowpassed(Bitmap bitmap,int n,int flag)
        {
            fftt fftt = new fftt();
            //bitmap = this.ToGray(bitmap);
            
            byte[] InputPicture0 = new byte[bitmap.Height * bitmap.Width];
            byte[] InputPicture1 = new byte[bitmap.Height * bitmap.Width];
            byte[] InputPicture2 = new byte[bitmap.Height * bitmap.Width];

            int ximg_width, ximg_height;
            ximg_width = fftt.find2n(bitmap.Width);
            ximg_height = fftt.find2n(bitmap.Height);

            byte[] OutputPicture0 = new byte[ximg_width * ximg_height];
            byte[] OutputPicture1 = new byte[ximg_width * ximg_height];
            byte[] OutputPicture2 = new byte[ximg_width * ximg_height];

            Color color = new Color();//储存某一像素的颜色
            //循环使得InputPicture数组得到位图的RGB
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    color = bitmap.GetPixel(j, i);
                    InputPicture0[i * bitmap.Width + j] = (byte)color.R;
                    InputPicture1[i * bitmap.Width + j] = (byte)color.G;
                    InputPicture2[i * bitmap.Width + j] = (byte)color.B;
                }
            }
            OutputPicture0 = fftt.lowpass(InputPicture0, bitmap.Width, bitmap.Height, n ,flag);
            OutputPicture1 = fftt.lowpass(InputPicture1, bitmap.Width, bitmap.Height, n ,flag);
            OutputPicture2 = fftt.lowpass(InputPicture2, bitmap.Width, bitmap.Height, n ,flag);
            
            Bitmap smooth = new Bitmap(ximg_width, ximg_height);//创建新位图
            //循环计算使得OutputPicture数组得到计算后位图的RGB
            for (int i = 0; i < ximg_height; i++)
            {
                for (int j = 0; j < ximg_width; j++)
                {
                    int R = (int)OutputPicture0[i * ximg_width + j];
                    int G = (int)OutputPicture1[i * ximg_width + j];
                    int B = (int)OutputPicture2[i * ximg_width + j];

                    R = between_0_255(R);
                    G = between_0_255(G);
                    B = between_0_255(B);
                    color = Color.FromArgb(R, G, B);//颜色结构储存该点RGB
                    smooth.SetPixel(j, i, color);//位图存储该点像素值
                }
            }
            return smooth;
        }



        /// <summary>
        /// 图像灰度化
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public Bitmap ToGray(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    //利用公式计算灰度值
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    Color newColor = Color.FromArgb(gray, gray, gray);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }
    }
}
