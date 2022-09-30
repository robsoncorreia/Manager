using ConfigurationFlexCloudHubBlaster.Model.Device;
using ConfigurationFlexCloudHubBlaster.Repository.Util;
using ConfigurationFlexCloudHubBlaster.View.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ConfigurationFlexCloudHubBlaster.Converte
{
    public class ZwaveDeviceToViewThenElseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!(value is ZwaveDevice zwaveDevice))
                {
                    return null;
                }

                if (zwaveDevice.IfthenType == IfthenType.Radio433
                    || zwaveDevice.IfthenType == IfthenType.IR
                    || zwaveDevice.IfthenType == IfthenType.RTS)
                {
                    return new GatewayFunctionsUserControl();
                }

                var classes = new HashSet<long?>(zwaveDevice.CommandClasses.Select(c => c.CommandClassId));

                var FXR5011 = new HashSet<long?>(UtilIfThen.FXR5011COMMANDCLASS);
                var FXA0600 = new HashSet<long?>(UtilIfThen.FXA0600COMMANDCLASS);
                var FXS69A = new HashSet<long?>(UtilIfThen.FXS69ACOMMANDCLASS);

                if (classes.SetEquals(FXA0600))
                {
                    return new FXA0600ThenElseUserControl();
                }

                if (classes.SetEquals(FXR5011))
                {
                    return new FXR5011ThenElseUserControl();
                }
                else if (classes.SetEquals(FXS69A))
                {
                    return new FXS69AThenElseUserControl(); ;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}