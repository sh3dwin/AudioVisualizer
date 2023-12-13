using CSCore.DSP;
using CSCore.Utils;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AudioVisual
{
    public static class FourierTransform
    {
        public static int Size = 2048;
        public static Complex[] FFT(byte[] buffer, Canvas canvas)
        {
            var complexBuffer = new Complex[buffer.Length];
            for (int i = 0; i < buffer.Length; i++)
                complexBuffer[i] = new Complex((int)buffer[i]);
            FastFourierTransformation.Fft(complexBuffer, 2);
            foreach (var complex in complexBuffer) {
                Console.WriteLine(complex.Real + " " + complex.Imaginary);

            }
                
            return complexBuffer;
        }
        private static void DrawRectangle(Canvas canvas, float frequency, float amplitude)
        {
            Rectangle rec = new Rectangle();
            rec.Width = Math.Abs(frequency);
            rec.Height = Math.Abs(100 * amplitude);
            rec.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            canvas.Children.Add(rec);
        }
    }
}
