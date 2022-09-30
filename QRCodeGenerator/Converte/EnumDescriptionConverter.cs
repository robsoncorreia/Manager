using QRCodeGenerator.Util;
using System;
using System.Globalization;
using System.Windows.Data;

namespace QRCodeGenerator.Converte
{
    public class EnumDescriptionConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is Enum myEnum) ? string.Empty : (object)myEnum.GetEnumDescription();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}