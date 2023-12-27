using System;
using System.Windows.Media;
using System.Windows.Shapes;
using NAudio.Dsp;
using NAudio.Wave;

namespace AudioVisual.Utils
{
    public static class GeneralUtils
    {
        public static Complex[] CreateAndInitializeComplexArray(WaveBuffer buffer)
        {
            var floatBuffer = buffer.ByteBuffer.ToFloats();
            
            var m = (int)Math.Log(floatBuffer.Length, 2);
            var len = (int)Math.Pow(2, m);
            var values = new Complex[len];

            if (values.Length < floatBuffer.Length)
            {
                for (var i = 0; i < len; i++)
                {
                    values[i].Y = 0;
                    values[i].X = floatBuffer[i];
                }
            }
            else
            {
                for (var i = 0; i < floatBuffer.Length; i++)
                {
                    values[i].Y = 0;
                    values[i].X = floatBuffer[i];
                }
                for (var i = floatBuffer.Length; i < len; i++)
                {
                    values[i].Y = 0;
                    values[i].X = 0;
                }
            }

            return values;
        }
    }
}
