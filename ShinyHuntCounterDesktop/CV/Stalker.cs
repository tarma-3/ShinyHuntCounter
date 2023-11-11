using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShinyHuntCounterDesktop.CV
{
    class Stalker
    {

        private Mat pokeMat;

        public Stalker()
        {
            string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
            Debug.WriteLine("Running path: " + RunningPath);

            var pokemonSprite = new Mat(
                "Resources\\match_left.jpg",
                ImreadModes.Unchanged);
            pokeMat = pokemonSprite.CvtColor(ColorConversionCodes.BGR2GRAY);
            pokemonSprite.Dispose();
        }


        ~Stalker()
        {
            pokeMat.Dispose();
        }
    }
}
