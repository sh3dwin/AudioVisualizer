﻿using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace AudioVisual.Utils
{
    public static class FrequencyToColorMapper
    {
        public static Color ColorFromHsv(double hue, double saturation, double value)
        {
            var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            var f = hue / 60 - Math.Floor(hue / 60);

            value *= 255;
            var v = Convert.ToInt32(value);
            var p = Convert.ToInt32(value * (1 - saturation));
            var q = Convert.ToInt32(value * (1 - f * saturation));
            var t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            return hi switch
            {
                0 => Color.FromArgb(255, (byte)v, (byte)t, (byte)p),
                1 => Color.FromArgb(255, (byte)q, (byte)v, (byte)p),
                2 => Color.FromArgb(255, (byte)p, (byte)v, (byte)t),
                3 => Color.FromArgb(255, (byte)p, (byte)q, (byte)v),
                4 => Color.FromArgb(255, (byte)t, (byte)p, (byte)v),
                _ => Color.FromArgb(255, (byte)v, (byte)p, (byte)q)
            };
        }

        public static List<double> GetListOfHues(int count)
        {
            var hueStep = 360.0 / (count);

            var hues = new List<double>(count);
            for (var iSegment = 0; iSegment < count; iSegment++)
            {
                hues.Add(hueStep * iSegment);
            }

            return hues;
        }
    }
}