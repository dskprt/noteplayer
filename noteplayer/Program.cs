using Emgu.CV;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace noteplayer {

    class Program {

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public const Int32 WM_CHAR = 0x0102;
        public const Int32 WM_KEYDOWN = 0x0100;
        public const Int32 WM_KEYUP = 0x0101;
        public const Int32 VK_RETURN = 0x0D;

        /////[DllImport("user32.dll")]
        ///static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        static void Main(string[] args) {
            if(args.Length < 2) {
                // TODO find it automatically
                Console.WriteLine("provide a handle to \"Edit\" window in notepad (use spyxx) as the 1st argument and the video path as the 2nd argument");
                return;
            }

            //long textAddr = long.Parse(args[0], System.Globalization.NumberStyles.HexNumber);

            Process notepad = Process.GetProcessesByName("notepad")[0];
            MoveWindow(notepad.MainWindowHandle, 200, 100, 1280, 720 + 73, true);
            IntPtr editHandle = new IntPtr(int.Parse(args[0], System.Globalization.NumberStyles.HexNumber));
            //Console.WriteLine(editHandle);

            //for(int i = 0; i < (165 * 38); i++) {
            //    PostMessage(editHandle, WM_CHAR, new IntPtr((Int32)'A'), IntPtr.Zero);
            //}

            //Thread.Sleep(5000);

            Graphics g = Graphics.FromHwnd(editHandle);
            geroeks = g.MeasureString("A", f).Height;

            PlayVideo(args[1], g);

            //int a = 0;

            //for(int i = 0; i < (43 * 30); i++) {
            //    if (a == 0) {
            //        DrawChar(g, '█', new SolidBrush(Color.Red));
            //        a = 1;
            //    } else {
            //        DrawChar(g, '█', new SolidBrush(Color.Blue));
            //        a = 0;
            //    }
            //}

            //Bitmap breaogersg = new Bitmap(@"C:\Users\lenOwO\Pictures\Adnotacja 2020-06-07 154655.jpg");
            //g.DrawImage(breaogersg, new Point(0, 0));
            //Bitmap res = ResizeImage(breaogersg, 166, 37);
            //res.Save("suck.bmp");
            //DrawAscii(res, g);

            //g.DrawString("QeHquY7HCns8uLGEyEzB42JUPPUyGo3nXUcNBzAAuDu21dPPCWAatXLyfH0WfAdhtLcsQBdONCCmj2CLlsao9ZPUwMoIpflvA305kaYU6E23ICn4tXvYEX0G7qOFoZKUc7gmsqz5oZp0H50foNnZmA3C4aHkZqehKMpBvEFAV7YCMIYTUEHIEHR832ACHU4IAWHRIUACHTA7TVH82C", new Font("Consolas", 11, FontStyle.Regular), new SolidBrush(Color.Black), new RectangleF(0, 0, 1365, 768), StringFormat.GenericDefault);
            //g.DrawString("etbsve89tunsbm9vctaw9vn", new Font("Consolas", 11, FontStyle.Regular), new SolidBrush(Color.Black), new RectangleF(0, 0, 1365, 768), StringFormat.GenericDefault);
        }

        static PointF point = new PointF(0, 0);
        static Font f = new Font("Consolas", 11, FontStyle.Regular);
        static Rectangle rect = new Rectangle(0, 0, 1365, 768);
        static float geroeks = 0;

        static void DrawChar(Graphics g, char ch, Brush brush) {
            g.DrawString(ch.ToString(), f, brush, point);
            SizeF size = g.MeasureString(ch.ToString(), f, 10, StringFormat.GenericTypographic);
            if(point.X + size.Width >= 1365) {
                point.X = 0;
                point.Y = point.Y + geroeks;
            } else {
                point.X += size.Width;
            }
        }

        static void DrawPixel(Graphics g, Brush brush) {
            g.FillRectangle(brush, point.X, point.Y, 1, 1);
            //SizeF size = g.MeasureString(ch.ToString(), f, 10, StringFormat.GenericTypographic);
            if (point.X + 1 > 1280) {
                point.X = 0;
                point.Y = point.Y + geroeks;
            } else {
                point.X += 1;
            }
        }

        static void PlayVideo(string filename, Graphics g) {
            using(var video = new VideoCapture(filename)) {
                using(var img = new Mat()) {
                    while(video.Grab()) {
                        video.Retrieve(img);
                        g.DrawImage(img.ToBitmap(), new Point(0, 0));
                    }
                }
            }
        }

        //public static void DrawFrame(Bitmap map) {
        //    for(int y = 0; y < map.Height; y++) {
        //        for(int x = 0;)
        //    }
        //}

        public static Bitmap ResizeImage(Bitmap image, int width, int height) {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage)) {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        //private string[] _AsciiChars = { "#", "#", "@", "%", "=", "+", "*", ":", "-", ".", " " };

        private static void DrawAscii(Bitmap image, Graphics g) {

            Boolean toggle = false;

            //StringBuilder sb = new StringBuilder();



            for (int h = 0; h < image.Height; h++) {

                for (int w = 0; w < image.Width; w++) {

                    Color pixelColor = image.GetPixel(w, h);

                    

                    //Average out the RGB components to find the Gray Color

                    //int red = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    //int green = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    //int blue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    //Color grayColor = Color.FromArgb(red, green, blue);



                    //Use the toggle flag to minimize height-wise stretch

                    if (!toggle) {

                        //int index = (grayColor.R * 10) / 255;

                        //sb.Append(_AsciiChars[index]);
                        DrawChar(g, '█', new SolidBrush(Color.FromArgb(pixelColor.A, pixelColor.R, pixelColor.G, pixelColor.B)));
                    }

                }

                //if (!toggle) {

                //    sb.Append("\r\n");

                //    toggle = true;

                //} else {

                //    toggle = false;

                //}

            }

            //return sb.ToString();

        }
    }
}
