﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace YPS.GridConverters
{
    public class TextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Color.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Color.Black;
        }
    }
}
