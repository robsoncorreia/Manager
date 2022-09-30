using System;
using System.Globalization;
using System.Windows.Data;

namespace FC.Domain.Converte
{
    public class ValueToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(string))
            {
                return !string.IsNullOrEmpty((string)value);
            }
            else if (value.GetType() == typeof(int) || value.GetType() == typeof(long) || value.GetType() == typeof(float) || value.GetType() == typeof(double))
            {
                return (int)value != -1;
            }
            //OBS: treat another type of object if you need.
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}