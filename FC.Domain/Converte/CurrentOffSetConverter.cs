using System;
using System.Globalization;
using System.Windows.Data;

namespace FC.Domain.Converte
{
    public class CurrentOffSetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return value;
            }
            if (value is not DateTime date)
            {
                return null;
            }

            TimeZone localZone = TimeZone.CurrentTimeZone;

            DateTime currentDate = DateTime.Now;

            TimeSpan currentOffset = localZone.GetUtcOffset(currentDate);

            return date + currentOffset;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}