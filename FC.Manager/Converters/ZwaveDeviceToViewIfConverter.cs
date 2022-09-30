using FC.Domain._Util;
using FC.Domain.Model.Device;
using FC.Manager.View.Components;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FC.Manager.Converte
{
    public class ZwaveDeviceToViewIfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ZwaveDevice zwaveDevice)
            {
                return null;
            }
            else if (zwaveDevice.IfthenType == IfthenType.Schedule)
            {
                return new ScheduleUserControl();
            }
            else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.ZXT600))
            {
                return new ZXT600IfUserControl();
            }
            else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXC222))
            {
                return new FXC222IfUserControl();
            }
            else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXD211))
            {
                return new FXD211IfUserControl();
            }
            else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXD220))
            {
                return new FXD220IfUserControl();
            }
            else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXR211))
            {
                return new FXR211IfUserControl();
            }
            else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXD5011))
            {
                return new FXD5011IfUserControl();
            }
            else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXR5011))
            {
                return new FXAIfUserControl();
                //return new FXR5011IfUserControl();
            }
            else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXR5012))
            {
                return new FXAIfUserControl();
                //return new FXR5011IfUserControl();
            }
            else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXR5013))
            {
                return new FXAIfUserControl();
                // return new FXR5011IfUserControl();
            }
            else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA5018))
            {
                return new FXAIfUserControl();
            }
            else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA3012))
            {
                return new FXAIfUserControl();
            }
            else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA0400))
            {
                return new FXAIfUserControl();
            }
            else
            {
                return zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA5016)
                    ? new FXAIfUserControl()
                    : zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA0600)
                                    ? new FXA0600IfUserControl()
                                    : zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA0404)
                                                    ? new FXA0600IfUserControl()
                                                    : zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA5014)
                                                                    ? new FXAIfUserControl()
                                                                    : zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA5029)
                                                                                    ? new FXAIfUserControl()
                                                                                    : zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA3011)
                                                                                                    ? new FXAIfUserControl()
                                                                                                    : zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXS69A)
                                                                                                                    ? new FXS69AIfUserControl()
                                                                                                                    : zwaveDevice.CustomId.Equals(ZwaveModelUtil.DomeDoorWindowSensor)
                                                                                                                                    ? new DomeDoorWindowSensorIfUserControl()
                                                                                                                                    : zwaveDevice.CustomId.Equals(ZwaveModelUtil.DomeMotionSensor) ? new MotionSensorIfUserControl() : (object)null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}