using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Practice
{
    class Klasa
    {

        private int kernelDimension = 19;
        private readonly object lockObj = new object();

        public Klasa()
        {
            
        }
    
        public void convolutionBlur()
        {

            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = 4;
            Bitmap image = new Bitmap(@"C:\Users\win7\Desktop\blurInput.jpg");
            if(image!=null)
            {
                BitmapData bd = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
                int bitsPerPixel = Bitmap.GetPixelFormatSize(bd.PixelFormat);

                int bytesPerPixel = Bitmap.GetPixelFormatSize(image.PixelFormat) / 8;
                Console.WriteLine(bytesPerPixel);
                int byteCount = bd.Stride * image.Height;
                byte[] pixels = new byte[byteCount];
                IntPtr ptrFirstPixel = bd.Scan0;
                System.Runtime.InteropServices.Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
                int heightInPixels = bd.Height;
                int widthInBytes = bd.Width * bytesPerPixel;
                Console.WriteLine("Widt:{0} , Height:{1}",bytesPerPixel, heightInPixels);
                //for (int y = 0; y < heightInPixels; y++)
                //{
                //    int currentLine = y * bd.Stride;
                //    //Console.WriteLine("Current line:{0}", currentLine);
                //    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                //    {
                //        //int alpha = pixels[currentLine + x];
                //        int oldBlue = pixels[currentLine + x];
                //        int oldGreen = pixels[currentLine + x + 1];
                //        int oldRed = pixels[currentLine + x + 2];

                //        int indexX1 = x - (kernelDimension / 2) * bytesPerPixel;
                //        int indexY1 = (bd.Stride*(y-kernelDimension/2))+x;


                //        if (indexX1 < 0)
                //        {
                //            continue;
                //        }
                //        if (indexY1 < 0)
                //        {
                //            continue;
                //        }
                //        if (indexX1 + bytesPerPixel * kernelDimension >= widthInBytes)
                //        {
                //            //Console.WriteLine("isois 1");

                //            continue;
                //        }
                //        if (indexY1 + kernelDimension * bd.Stride >= pixels.Length - 1)
                //        {
                //            //Console.WriteLine("isois 2");
                //            continue;
                //        }

                //        int[] newPixel = help(pixels, x, y, indexX1, indexY1, bytesPerPixel, bd.Stride);

                //        // calculate new pixel value
                //        pixels[currentLine + x] = (byte)newPixel[0];
                //        pixels[currentLine + x + 1] = (byte)newPixel[1];
                //        pixels[currentLine + x + 2] = (byte)newPixel[2];
                //    }
                //}

                //for (int x = 0; x < widthInBytes; x+=bytesPerPixel)
                //{
                //    //int currentLine = y * bd.Stride;
                //    //Console.WriteLine("Current line:{0}", currentLine);
                //    for (int y = 0; y < heightInPixels; y++)
                //    {
                //        int currentLine = y * bd.Stride + x;
                //        //int alpha = pixels[currentLine];
                //        int oldBlue = pixels[currentLine  ];
                //        int oldGreen = pixels[currentLine + 1];
                //        int oldRed = pixels[currentLine + 2];

                //        int indexY = (bd.Stride * (y - kernelDimension / 2)) + x;

                //        if(indexY < 0)
                //        {
                //            continue;
                //        }

                //        if(indexY+ kernelDimension * bd.Stride > pixels.Length-1)
                //        {
                //            continue;
                //        }

                //        int[] newPixel = helpY(pixels, x, y, indexY, bytesPerPixel, bd.Stride);

                //        // calculate new pixel value

                //        pixels[currentLine ] = (byte)newPixel[0];
                //        pixels[currentLine + 1] = (byte)newPixel[1];
                //        pixels[currentLine + 2] = (byte)newPixel[2];
                //    }
                //}

                Parallel.For(0, heightInPixels, options, (y) =>
                {
                    int currentLine = y * bd.Stride;
                    //Console.WriteLine("Current line:{0}", currentLine);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        //int alpha = pixels[currentLine + x];
                        int oldBlue = pixels[currentLine + x];
                        int oldGreen = pixels[currentLine + x + 1];
                        int oldRed = pixels[currentLine + x + 2];

                        int indexX1 = x - (kernelDimension / 2) * bytesPerPixel;
                        int indexY1 = (bd.Stride * (y - kernelDimension / 2)) + x;


                        if (indexX1 < 0)
                        {
                            continue;
                        }
                        if (indexY1 < 0)
                        {
                            continue;
                        }
                        if (indexX1 + bytesPerPixel * kernelDimension >= widthInBytes)
                        {
                            //Console.WriteLine("isois 1");

                            continue;
                        }
                        if (indexY1 + kernelDimension * bd.Stride >= pixels.Length - 1)
                        {
                            //Console.WriteLine("isois 2");
                            continue;
                        }

                        int[] newPixel = help(pixels, x, y, indexX1, indexY1, bytesPerPixel, bd.Stride);

                        // calculate new pixel value
                        pixels[currentLine + x] = (byte)newPixel[0];
                        pixels[currentLine + x + 1] = (byte)newPixel[1];
                        pixels[currentLine + x + 2] = (byte)newPixel[2];
                    }
                });


                System.Runtime.InteropServices.Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
                image.UnlockBits(bd);

                //for (int i = 0; i < image.Width; i++)
                //{
                //    for (int j = 0; j < image.Height; j++)
                //    {
                //        int indexX1 = i - kernelDimension / 2;
                //        int indexY1 = j - kernelDimension / 2;

                //        if (indexX1 < 0)
                //        {
                //            continue;
                //        }
                //        if (indexY1 < 0)
                //        {
                //            continue;
                //        }
                //        if (indexX1 + kernelDimension >= image.Width)
                //        {
                //            continue;
                //        }
                //        if (indexY1 + kernelDimension >= image.Height)
                //        {
                //            continue;
                //        }
                //        Color[,] subImage = new Color[kernelDimension, kernelDimension];
                //        for (int x = 0; x < kernelDimension; x++)
                //        {
                //            for (int y = 0; y < kernelDimension; y++)
                //            {
                //                subImage[x, y] = image.GetPixel(indexX1 + x, indexY1 + y);
                //            }
                //        }
                //        Color newPixel = convolve(subImage);
                //        image.SetPixel(i, j, newPixel);
                //    }
                //}
                //int width = image.Width;
                //int height = image.Height;
               

                image.Save("output11.jpg");
            }
            else
            {
                Console.WriteLine("Nije pronadjena slika");
            }
            
        }

        public int[] help(byte[] array, int x, int y, int startX, int startY, int bytesPerPixel, int stride)
        {
            double kernel = 1 / Math.Pow(kernelDimension, 2);
            double newRed = 0;
            double newGreen = 0;
            double newBlue = 0;
            int startingPoint = startY - (kernelDimension / 2) * bytesPerPixel;
            for(int j = 0; j < kernelDimension; j++)
            {
                int currentLine = startY + j * stride;
                for(int i = 0; i < kernelDimension; i++)
                {
                    int blue = array[currentLine + i * bytesPerPixel];
                    int green = array[currentLine + i * bytesPerPixel + 1];
                    int red = array[currentLine + i * bytesPerPixel + 2];

                    newBlue += blue * kernel;
                    newGreen += green * kernel;
                    newRed += red * kernel;
                }
            }

            if (newRed > 255.0)
            {
                newRed = 255;
            }
            else if (newRed < 0.0)
            {

                newRed = 0;
            }
            if (newBlue > 255.0)
            {
                newBlue = 255;
            }
            else if (newBlue < 0.0)
            {
                newBlue = 0;
            }
            if (newGreen > 255.0)
            {
                newGreen = 255;
            }
            else if (newGreen < 0.0)
            {
                newGreen = 0;
            }

            int[] ret = new int[3] { (int) Math.Round(newBlue), (int)Math.Round(newGreen), (int)Math.Round(newRed) };
            return ret;
        }


        public void parallelPhotoBlur()
        {
            List<string> lista = new List<string>();
            lista.Add(@"C:\Users\win7\Desktop\blurInput1.jpg");
            lista.Add(@"C:\Users\win7\Desktop\blurInput2.jpg");
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = 2;
            Parallel.ForEach(lista, options, (l) => {
                Bitmap image = new Bitmap(l);
                if (image != null)
                {
                    BitmapData bd = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
                    int bitsPerPixel = Bitmap.GetPixelFormatSize(bd.PixelFormat);

                    int bytesPerPixel = Bitmap.GetPixelFormatSize(image.PixelFormat) / 8;
                    Console.WriteLine(bytesPerPixel);
                    int byteCount = bd.Stride * image.Height;
                    byte[] pixels = new byte[byteCount];
                    IntPtr ptrFirstPixel = bd.Scan0;
                    System.Runtime.InteropServices.Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
                    int heightInPixels = bd.Height;
                    int widthInBytes = bd.Width * bytesPerPixel;

                    for (int y = 0; y < heightInPixels; y++)
                    {
                        int currentLine = y * bd.Stride;
                        //Console.WriteLine("Current line:{0}", currentLine);
                        for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                        {
                            //int alpha = pixels[currentLine + x];
                            int oldBlue = pixels[currentLine + x];
                            int oldGreen = pixels[currentLine + x + 1];
                            int oldRed = pixels[currentLine + x + 2];

                            int indexX1 = x - (kernelDimension / 2) * bytesPerPixel;
                            int indexY1 = (bd.Stride * (y - kernelDimension / 2)) + x;


                            if (indexX1 < 0)
                            {
                                continue;
                            }
                            if (indexY1 < 0)
                            {
                                continue;
                            }
                            if (indexX1 + bytesPerPixel * kernelDimension >= widthInBytes)
                            {
                                //Console.WriteLine("isois 1");

                                continue;
                            }
                            if (indexY1 + kernelDimension * bd.Stride >= pixels.Length - 1)
                            {
                                //Console.WriteLine("isois 2");
                                continue;
                            }

                            int[] newPixel = help(pixels, x, y, indexX1, indexY1, bytesPerPixel, bd.Stride);

                            // calculate new pixel value
                            pixels[currentLine + x] = (byte)newPixel[0];
                            pixels[currentLine + x + 1] = (byte)newPixel[1];
                            pixels[currentLine + x + 2] = (byte)newPixel[2];
                        }
                    }
                    System.Runtime.InteropServices.Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
                    image.UnlockBits(bd);
                    string naziv = l.GetHashCode().ToString();
                    image.Save(naziv + ".jpg");
                }
            });
        }

        //public Color convolve(Color[,] subImage)
        //{
        //    double kernel = 1 / Math.Pow(kernelDimension, 2);
        //    double newRed = 0;
        //    double newGreen = 0;
        //    double newBlue = 0;
        //    int length = (int)Math.Sqrt(subImage.Length);

        //    for (int i = 0; i<length;i++)
        //    {
        //        for(int j = 0; j<length;j++)
        //        {
        //            double red = subImage[i, j].R;
        //            double blue = subImage[i, j].B;
        //            double green = subImage[i, j].G;

        //            red *= kernel;
        //            blue *= kernel;
        //            green *= kernel;

        //            newRed += red;
        //            newBlue += blue;
        //            newGreen += green;
        //        }
        //    }

        //    if(newRed>255.0)
        //    {
        //        newRed = 255.0;
        //    } else if(newRed<0.0)
        //    {
        //        newRed = 0.0;
        //    }
        //    if(newBlue>255.0)
        //    {
        //        newBlue = 255.0;
        //    }else if(newBlue < 0.0)
        //    {
        //        newBlue = 0.0;
        //    }
        //    if(newGreen>255.0)
        //    {
        //        newGreen = 255.0;
        //    }else if(newGreen < 0.0)
        //    {
        //        newGreen = 0.0;
        //    }

        //    Color newPixel = Color.FromArgb(255, (int)newRed, (int)newGreen, (int)newBlue);
        //    return newPixel;
        //}
     
    }
}
