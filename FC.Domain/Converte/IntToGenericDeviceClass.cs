using CommunityToolkit.Mvvm.DependencyInjection;
using FC.Domain.Service;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FC.Domain.Converte
{
    public class IntToGenericDeviceClass : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null
                ? string.Empty
                : !int.TryParse(value.ToString(), out int id)
                ? string.Empty
                : id == 0
                ? string.Empty
                : (object)Ioc.Default.GetRequiredService<IZwaveService>().GenericDeviceClasses.FirstOrDefault(x => x.Key == $"0x{id:x2}").Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}