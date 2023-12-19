﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace AudioVisual
{
    [ValueConversion(typeof(float), typeof(float))]
    public class PercentageToWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null)
            {
                return (double)values[0] * (float)values[1];
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}