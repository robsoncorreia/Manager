using CommunityToolkit.Mvvm.DependencyInjection;
using FC.Domain.Model.Device;
using FC.Domain.Service;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FC.Domain.Converte
{
    public class IntToSpecificDeviceClassConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null
                ? string.Empty
                : value is not ZwaveDevice
                ? string.Empty
                : (object)(Ioc.Default.GetRequiredService<IZwaveService>().GenericClasses
                                                                  .FirstOrDefault(x => x.Key == $"0x{((ZwaveDevice)value).GenericDeviceClass:x2}")?.Specifics
                                                                  .FirstOrDefault(x => x.Key == $"0x{((ZwaveDevice)value).SpecificDeviceClass:x2}")?.Name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}