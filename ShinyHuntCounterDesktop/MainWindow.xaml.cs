using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using OpenCvSharp.Extensions;
using ShinyHuntCounterDesktop.CV;
using ShinyHuntCounterDesktop.RG;

namespace ShinyHuntCounterDesktop;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private static int MAX_FRAMERATE = 60;

    private readonly int height = 400;
    private readonly ScreenRecorder srec;
    private readonly Stalker stalker;
    private readonly int width = 600;

    private readonly int x = 0;
    private readonly int y = 0;
    private int persistentResultFrameCount = 0;
    private bool oldOnBattleVar = false;
    public MainWindow()
    {
        InitializeComponent();
        stalker = new Stalker();
  
        srec = new ScreenRecorder(x, y, width, height, bmp =>
        {
            
            var onBattle = stalker.IsOnBattle(x, y, width, height, bmp);
            if (onBattle == oldOnBattleVar)
            {
                if (persistentResultFrameCount++ == 5)
                {
                    DisplayOnBattle(onBattle);
                    if (onBattle) UpdateAttempts();
                }
            }
            else persistentResultFrameCount = 0;
            
            oldOnBattleVar = onBattle;
            


            //DisplayCapture(bmp);
            DisplayTemplate(stalker.pokeMat.ToBitmap());
            DisplayCapture(bmp);



        });

        new Thread(() =>
        {
            while (true)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                srec.run();
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                var timeToSleep = 1000 / MAX_FRAMERATE - elapsedMs;
                if (timeToSleep>=0)
                {
                    Thread.Sleep((int)timeToSleep);
                }
                else
                {
                    Debug.WriteLine(1000/ elapsedMs);
                }

            }
        }).Start();
    }

    private void UpdateAttempts()
    {
        Dispatcher.Invoke(() =>
        {
            var text = ResetsTextBlock.Text;
            var x = 0;
            if (!int.TryParse(text, out x)) return;
            ResetsTextBlock.Text = (++x).ToString();
        });
    }

    [DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);

    private void DisplayTemplate(Bitmap bmp)
    {
        Dispatcher.Invoke(() =>
        {
            var hBitmap = bmp.GetHbitmap();
            try
            {
                var i = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                TemplateImage.Source = i;
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        });
    }

    private void DisplayCapture(Bitmap bmp)
    {
        Dispatcher.Invoke(() =>
        {
            var hBitmap = bmp.GetHbitmap();
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
    }

    private void DisplayOnBattle(bool isOnBattle)
    {
        Dispatcher.Invoke(() => { IsOnBattleCheckbox.IsChecked = isOnBattle; });
    }

    private void YCoTextChanged(object sender, TextChangedEventArgs args)
    {
        if (srec == null) return;
        var x = 0;
        if (!int.TryParse(YCoTextBox.Text, out x)) return;
        srec.SetY(x);
    }

    private void XCoTextChanged(object sender, TextChangedEventArgs args)
    {
        if (srec == null) return;
        var x = 0;
        if (!int.TryParse(XCoTextBox.Text, out x)) return;
        srec.SetX(x);
    }

    private void WidthTextChanged(object sender, TextChangedEventArgs args)
    {
        if (srec == null) return;
        var x = 0;
        if (!int.TryParse(WidthTextBox.Text, out x)) return;
        srec.SetWidth(x);
    }

    private void HeightTextChanged(object sender, TextChangedEventArgs args)
    {
        if (srec == null) return;
        var x = 0;
        if (!int.TryParse(HeightTextBox.Text, out x)) return;
        srec.SetHeight(x);
    }
}