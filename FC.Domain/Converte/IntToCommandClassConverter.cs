using CommunityToolkit.Mvvm.DependencyInjection;
using FC.Domain.Model.Device;
using FC.Domain.Service;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FC.Domain.Converte
{
    public class IntToCommandClassConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return string.Empty;
            }

            if (value is not CommandClass)
            {
                return string.Empty;
            }
            //TODO Verificar JSON
            CommandClass command = Ioc.Default.GetRequiredService<IZwaveService>().CommandClasses.FirstOrDefault(x => x.Key == $"0x{((CommandClass)value).CommandClassId:x2}");

            return $"{command?.Name}\n{Properties.Resources.Version} {((CommandClass)value)?.Version}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}