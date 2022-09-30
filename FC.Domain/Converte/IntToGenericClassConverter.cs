using CommunityToolkit.Mvvm.DependencyInjection;
using FC.Domain.Model.Device;
using FC.Domain.Service;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FC.Domain.Converte
{
    public class IntToGenericClassConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return string.Empty;
            }

            if (value is not Endpoint)
            {
                return string.Empty;
            }

            Model.Zwave.GenericClass generic = Ioc.Default.GetRequiredService<IZwaveService>().GenericClasses.FirstOrDefault(x => x.Key == $"0x{((Endpoint)value).GenericDeviceClass:x2}");

            return $"{generic.Name}\n{generic.Specifics.FirstOrDefault(x => x.Key == $"0x{((Endpoint)value).SpecificDeviceClass:x2}").Name}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}