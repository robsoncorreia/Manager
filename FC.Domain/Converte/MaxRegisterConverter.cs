using FC.Domain.Model.Device;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FC.Domain.Converte
{
    public class MaxRegisterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null
                ? false
                : value is not AssociationGroup association ? false : (object)(association.ZwaveDevices.Count < association.MaxRegister);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}