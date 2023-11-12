using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace ShinyHuntCounterDesktop.CV
{
    class Stalker
    {

        public Mat pokeMat;

        public Stalker()
        {
            string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
            Debug.WriteLine("Running path: " + RunningPath);

            var pokemonSprite = new Mat(
                RunningPath+"Resources\\match.jpg",
                ImreadModes.Unchanged);
            pokeMat = pokemonSprite.CvtColor(ColorConversionCodes.BGR2GRAY);
            pokemonSprite.Dispose();
        }


        ~Stalker()
        {
            pokeMat.Dispose();
        }
        public List<Rect> ExtractImage(Mat src, Mat refMat, Mat mobTemplate, double minThreashold = 0.9)
        {
            using Mat res = new Mat(src.Rows - mobTemplate.Rows + 1, src.Cols - mobTemplate.Cols + 1, MatType.CV_32FC1);
            Cv2.MatchTemplate(refMat, mobTemplate, res, TemplateMatchModes.CCoeffNormed);
            // Cv2.Threshold(res, res, 0.1, 1.0, ThresholdTypes.Tozero);
            List<Rect> rects = new List<Rect>();

            double minval, maxval;
            Point minloc;
            Point maxloc;
            Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);
            Debug.WriteLine("maxVal: " + maxval);

            double threshold = maxval - 0.01;
            while (true)
            {
                Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);
                Debug.WriteLine("maxVal: " + maxval + " threshold: "+threshold);
                if (maxval > minThreashold)
                {
                    //Setup the rectangle to draw
                    Rect r = new Rect(new Point(maxloc.X, maxloc.Y),
                        new Size(mobTemplate.Width, mobTemplate.Height));
                    //Draw a rectangle of the matching area
                    Cv2.Rectangle(src, r, Scalar.LimeGreen);
                    rects.Add(r);
                    //Fill in the res Mat so you don't find the same area again in the MinMaxLoc
                    Rect outRect;
                    Cv2.FloodFill(res, maxloc, new Scalar(0), out outRect, new Scalar(0.1),
                        new Scalar(1.0));
                }
                else
                    break;
            }

            return rects;
        }
        internal bool IsOnBattle(int x, int y, int width, int height, Bitmap bmp)
        {
            using Mat src0 = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
            List<Point> l = new List<OpenCvSharp.Point>();
            l.Add(new Point(x, y));
            l.Add(new Point(width-1, height-1));
            var boundingRect = Cv2.BoundingRect(l);
            using Mat src = new Mat(src0, boundingRect);
            using Mat refMat = src.CvtColor(ColorConversionCodes.BGR2GRAY);

            var match = ExtractImage(src, refMat, pokeMat);
            Debug.WriteLine(match.Count);

            return true;
        }
    }
}
