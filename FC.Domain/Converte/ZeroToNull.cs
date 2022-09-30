using System;
using System.Globalization;
using System.Windows.Data;

namespace FC.Domain.Converte
{
    public class ZeroToNull : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 0 ? null : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}