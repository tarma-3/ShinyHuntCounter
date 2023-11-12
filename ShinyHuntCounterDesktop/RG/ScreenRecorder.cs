using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace ShinyHuntCounterDesktop.RG
{
    class ScreenRecorder
    {
        double screenLeft = SystemParameters.VirtualScreenLeft;
        double screenTop = SystemParameters.VirtualScreenTop;
        double screenWidth = SystemParameters.VirtualScreenWidth;
        double screenHeight = SystemParameters.VirtualScreenHeight;
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr onj);
        
        Action<Bitmap> callback;
        int x = 0, y = 0;
        int width = 100, height = 100;
        public ScreenRecorder(int x, int y, int width, int height, Action<Bitmap> onScreenshotTaken){
            this.callback = onScreenshotTaken;
        }

        public void run()
        {
            Rectangle rect = new Rectangle(x, y, width, height);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            callback(bmp);
            g.Dispose();
            bmp.Dispose();
        }

        internal void SetY(int y)
        {
            this.y = y;
        }
        internal void SetX(int x)
        {
            this.x = x;
        }
        internal void SetWidth(int width) { 
            this.width = width;
        }
        internal void SetHeight(int height) { 
            this.height = height;
        } 
        
    }
}