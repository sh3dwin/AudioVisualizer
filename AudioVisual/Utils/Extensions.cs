using AudioVisual.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AudioVisual.Utils
{
    public static class Extensions
    {
        public static void Clear(this Canvas c)
        {
            c.Background = new SolidColorBrush(Color.FromRgb(10, 10, 10));
            c.Children.Clear();
        }

        public static float[] ToFloats(this byte[] byteArray)
        {
            // Check if the length of the byte array is a multiple of 4
            if (byteArray.Length % 4 != 0)
            {
                throw new ArgumentException("Byte array length must be a multiple of 4.");
            }

            // Create an array to store the floats
            var floatArray = new float[byteArray.Length / 4];

            // Convert each set of 4 bytes to a float
            for (var i = 0; i < floatArray.Length; i++)
            {
                floatArray[i] = BitConverter.ToSingle(byteArray, i * 4);
            }

            return floatArray;
        }

        public static IReadOnlyList<T> SubSample<T>(this IReadOnlyList<T> list, int samples)
        {
            if (list.Count < samples)
                return list;

            var result = new List<T>(samples);

            var sampleStep = list.Count / (double)samples;

            for (var i = 0; i < samples; i++)
            {
                result.Add(list[(int)(i * sampleStep)]);
            }

            return result;
        }

        public static List<Line> Offset(this List<Line> lines, double xOffset = 0.0, double yOffset = 0.0)
        {
            foreach (var line in lines)
            {
                line.X1 += xOffset;
                line.X2 += xOffset;

                line.Y1 += yOffset;
                line.Y2 += yOffset;
            }

            return lines;
        }

        public static List<Line> Scale(this List<Line> lines, double scaleX = 0.0, double scaleY = 0.0)
        {
            foreach (var line in lines)
            {

                line.X1 *= scaleX;
                line.X2 *= scaleX;

                line.Y1 *= scaleY;
                line.Y2 *= scaleY;
            }

            return lines;
        }

        public static List<Line> ScaleFromCenter(this List<Line> lines, Point center, double scaleX = 0.0, double scaleY = 0.0)
        {
            return lines
                .Offset(-center.X, -center.Y)
                .Scale(scaleX, scaleY)
                .Offset(center.X, center.Y);
        }

        public static List<double> NormalizeValues(this IReadOnlyList<double> values)
        {
            var maxValue = values.Max(Math.Abs);
            const double epsilon = 1e-3;
            if (maxValue < epsilon)
                return values.Select(_ => 0.0).ToList();
            var normalizationFactor = 1.0 / maxValue;
            return values.Select(x => x * normalizationFactor).ToList();
        }

        public static List<List<double>> NormalizeValues(this List<List<double>> lists)
        {
            var maxValue = lists.Select(x => x.Select(Math.Abs).Max()).Max();
            const double epsilon = 1e-3;
            if (maxValue < epsilon)
                return lists;
            var normalizationFactor = 1.0 / maxValue;

            var normalizedLists = new List<List<double>>();

            foreach (var list in lists)
            {
                var normalizedList = list.Select(x => x * normalizationFactor).ToList();
                normalizedLists.Add(normalizedList);
            }

            return normalizedLists;
        }

        public static List<Line> Stylize(this List<Line> lines, SolidColorBrush color, int thickness)
        {
            foreach (var line in lines)
            {
                line.Stroke = line.Stroke is null ? color : line.Stroke;
                line.StrokeThickness = thickness;
            }

            return lines;
        }

        public static List<Shape> GetDrawable(this List<Line> lines, DrawingLayoutData drawingLayout)
        {
            var lineThickness =
                (int)(Constants.LineThickness * 
                      Math.Min(
                          Math.Max(lines.Select(line => line.Y2).Max(), 0.2), 1.0));

            return lines
                .Scale(drawingLayout.ScalingX, drawingLayout.ScalingY)
                .Offset(drawingLayout.Offset.X, drawingLayout.Offset.Y)
                .Stylize(drawingLayout.Brush, lineThickness)
                .Select(x => (x as Shape)).ToList();
        }
    }
}
