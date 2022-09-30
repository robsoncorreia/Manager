using System;
using System.Globalization;
using System.Windows.Data;

namespace FC.Domain.Converte
{
    public class DateToHours : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime src = (DateTime)value;
            return $"{src.Hour}:{(src.Minute < 10 ? "0" + src.Minute : src.Minute.ToString())}:{(src.Second < 10 ? "0" + src.Second.ToString() : src.Second.ToString())}:{src.Millisecond}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}