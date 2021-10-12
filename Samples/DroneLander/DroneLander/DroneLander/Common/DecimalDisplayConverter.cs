using System;
using System.Globalization;
using Xamarin.Forms;

namespace DroneLander.Common
{
    public class DecimalDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value).ToString("F0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    } 
}
