using Parse;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FC.Domain.Converte
{
    public class ParseUserToObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is null || value is not ParseUser)
            {
                return DependencyProperty.UnsetValue;
            }

            ParseUser parserUser = value as ParseUser;

            return !parserUser.ContainsKey(parameter.ToString()) ? DependencyProperty.UnsetValue : parserUser.Get<object>(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // According to https://msdn.microsoft.com/en-us/library/system.windows.data.ivalueconverter.convertback(v=vs.110).aspx#Anchor_1
            // (kudos Scott Chamberlain), if you do not support a conversion
            // back you should return a Binding.DoNothing or a
            // DependencyProperty.UnsetValue
            return Binding.DoNothing;
            // Original code:
            // throw new NotImplementedException();
        }
    }
}