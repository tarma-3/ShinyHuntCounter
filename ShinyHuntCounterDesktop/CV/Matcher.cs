using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.XFeatures2D;

namespace ShinyHuntCounterDesktop.CV
{
    internal class Matcher
    {
        public static int FindMatches(Mat btmTemplate, Mat btmSource)
        {
            // detecting keypoints
            // FastFeatureDetector, StarDetector, SIFT, SURF, ORB, BRISK, MSER, GFTTDetector, DenseFeatureDetector, SimpleBlobDetector
            // SURF = Speeded Up Robust Features
            var detector = SURF.Create(hessianThreshold: 400); //A good default value could be from 300 to 500, depending from the image contrast.
            var keypoints1 = detector.Detect(btmTemplate);
            var keypoints2 = detector.Detect(btmSource);

            // computing descriptors, BRIEF, FREAK
            // BRIEF = Binary Robust Independent Elementary Features
            var extractor = BriefDescriptorExtractor.Create();
            var descriptors1 = new Mat();
            var descriptors2 = new Mat();
            extractor.Compute(btmTemplate, ref keypoints1, descriptors1);
            extractor.Compute(btmSource, ref keypoints2, descriptors2);

            // matching descriptors
            var matcher = new BFMatcher(NormTypes.L2, false);
            var flannMatcher = new FlannBasedMatcher();
            var matches = matcher.Match(descriptors1, descriptors2);
            Debug.WriteLine("matches: "+matches.Length );
            return matches.Length;
        }
    }
}
