using ShinyHuntCounterDesktop.RG;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ShinyHuntCounterDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        int x = 0, y = 0;
        int width = 100, height = 100;
        ScreenRecorder srec;

        public MainWindow()
        {
            InitializeComponent();
            srec = new ScreenRecorder(x,y,width,height,((bmp) =>
            {

                Dispatcher.Invoke(() =>
                {
                        IntPtr hBitmap = bmp.GetHbitmap();
                        try
                        {
                            var i = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                                                BitmapSizeOptions.FromEmptyOptions());
                             CapturedImage.Source = i;
                        }
                        finally
                        {
                            DeleteObject(hBitmap);
                        }
                });
            }));
            new Thread(() =>
            {
                while (true) { 
                srec.run();
                }
            }).Start();
        }
        
        private void YCoTextChanged(object sender, TextChangedEventArgs args)
        {
            if (srec == null) return;
            int x = 0;
            if (!Int32.TryParse(YCoTextBox.Text, out x)) return;
            srec.SetY(x);
     
        }
        private void XCoTextChanged(object sender, TextChangedEventArgs args)
        {
            if (srec == null) return;
            int x = 0;
            if (!Int32.TryParse(XCoTextBox.Text, out x)) return;
            srec.SetX(x);
            srec.run();
        }
        private void WidthTextChanged(object sender, TextChangedEventArgs args)
        {
            if (srec == null) return;
            int x = 0;
            if (!Int32.TryParse(WidthTextBox.Text, out x)) return;
            srec.SetWidth(x);
 
        }
        private void HeightTextChanged(object sender, TextChangedEventArgs args)
        {
            if (srec == null) return;
            int x = 0;
            if (!Int32.TryParse(HeightTextBox.Text, out x)) return;
            srec.SetHeight(x);
  
        }
    }
}
