using Parse;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FC.Domain.Converte
{
    public class ParseObjectToObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter is null || value is not ParseObject parseObject
                ? null
                : !parseObject.ContainsKey(parameter.ToString()) ? null : parseObject.Get<object>(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}