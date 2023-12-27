using System.Windows;
using System.Windows.Media;

namespace AudioVisual.DataStructures
{
    public class DrawingLayoutData
    {
        public Point Offset { get; set; }
        public double ScalingX { get; set; }
        public double ScalingY { get; set; }
        public SolidColorBrush Brush { get; set; }

        public DrawingLayoutData(Point offset, double scalingX, double scalingY, SolidColorBrush brush)
        {
            Offset = offset;
            ScalingX = scalingX;
            ScalingY = scalingY;
            Brush = brush;
        }

    }
}
