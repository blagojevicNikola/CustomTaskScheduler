using MyTaskScheduler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFScheduler.Tasks
{
    class SingleInputCBTask : UserTask
    {

        private int kernelDimension = 15;

        public SingleInputCBTask(string name, int priority, int degreeOfParallelism) : base(name, priority, degreeOfParallelism)
        { }


        public override void algoritam()
        {
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = getDegreeOfParallelism();
            lockResourceByIndex(0);
            Bitmap image = new Bitmap(getResourceByIndex(0).getPath());
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
                double progresVal = 0.0;
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
                        //if(x*y/(heightInPixels*bd.Width)*100.0F - progresVal > 1000)
                        //{
                        //    progressOfTask.Report(x * y / (heightInPixels * bd.Width) * 100.0F);
                        //    progresVal = x * y / (heightInPixels * bd.Width) * 100.0F;
                        //}
                    }
                    if(y%400==0)
                    {
                        int temp = (int)Math.Round(y * 100.0 / heightInPixels);
                        progressOfTask.Report(temp);
                    }
                });

                System.Runtime.InteropServices.Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
                image.UnlockBits(bd);

                image.Save("output16.jpg");
            }
            else
            {
                Console.WriteLine("Nije pronadjena slika");
            }
            unlockResourceByIndex(0);
        }

        private int[] help(byte[] array, int x, int y, int startX, int startY, int bytesPerPixel, int stride)
        {
            double kernel = 1 / Math.Pow(kernelDimension, 2);
            double newRed = 0;
            double newGreen = 0;
            double newBlue = 0;
            int startingPoint = startY - (kernelDimension / 2) * bytesPerPixel;
            for (int j = 0; j < kernelDimension; j++)
            {
                int currentLine = startY + j * stride;
                for (int i = 0; i < kernelDimension; i++)
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

            int[] ret = new int[3] { (int)Math.Round(newBlue), (int)Math.Round(newGreen), (int)Math.Round(newRed) };
            return ret;
        }
    }
}
