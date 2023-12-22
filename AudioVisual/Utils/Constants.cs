using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioVisual.Utils
{
    public static class Constants
    {
        public const int PowerOfTwo = 15;
        public const int SegmentCount = 200;
        public const int SampleRate = 48000;
        public const int DefaultWavePartitions = 1;

        public const Enums.VisualizationMode DefaultVisualizationMode = Enums.VisualizationMode.Frequency;
    }
}
