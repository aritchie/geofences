using System;
using System.Globalization;
using Xamarin.Forms;


namespace Samples
{
    public class DoubleConverter : IValueConverter
    {
        public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return String.Empty;

            if (value is double)
                return value.ToString();

            return value;
        }

        public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Double.TryParse(value as string, out var dbl))
                return dbl;

            return null;
        }
    }
}
