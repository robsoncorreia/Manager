using System;
using System.Globalization;
using System.Windows.Data;

namespace FC.Domain.Converte
{
    public class IsStringNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null ? true : string.IsNullOrWhiteSpace(value.ToString()) ? true : (object)false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}