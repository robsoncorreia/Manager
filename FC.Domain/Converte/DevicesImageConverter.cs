using FC.Domain._Util;
using FC.Domain.Model;
using FC.Domain.Model.Device;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FC.Domain.Converte
{
    public class DevicesImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is ZwaveDevice temp
                ? temp.DefaultName == AppConstants.FCZIR100311
                    ? $"/FC.Domain;component/Assets/Gateway/Black/drawable-hdpi/Black.png"
                    : temp.DefaultName == AppConstants.FCZWS100
                    ? $"/FC.Domain;component/Assets/Gateway/White/drawable-hdpi/White.png"
                    : temp.ZWaveComponents == ZWaveComponents.Controller
                    ? $"/FC.Domain;component/Assets/Gateway/Black/drawable-hdpi/Black.png"
                    : string.IsNullOrEmpty(temp.DefaultName)
                    ? null
                    : temp.DefaultName == Domain.Properties.Resources.On_Off ||
                    temp.DefaultName == Domain.Properties.Resources.Dimmer ||
                    temp.DefaultName == Domain.Properties.Resources.Shutter ||
                    temp.DefaultName == Domain.Properties.Resources.Door_Sensor ||
                    temp.DefaultName == Domain.Properties.Resources.Door_Lock
                    ? $"/FC.Domain;component/Assets/default.png"
                    : (object)$"/FC.Domain;component/Assets/ZwaveDevice/{temp.DefaultName}/drawable-hdpi/{temp.DefaultName}.png"
                : value is GatewayModel gateway
                ? gateway.GatewayModelEnum is GatewayModelEnum.FCZWS100V1 or
                    GatewayModelEnum.FCZWS100V2
                    ? $"/FC.Domain;component/Assets/Gateway/White/drawable-hdpi/White.png"
                    : (object)$"/FC.Domain;component/Assets/Gateway/Black/drawable-hdpi/Black.png"
                : $"/FC.Domain;component/Assets/default.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}