using FC.Domain._Util;
using FC.Domain.Model.Device;
using FC.Manager.View.Components;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FC.Manager.Converte
{
    public class ZwaveDeviceToViewThenElseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is not ZwaveDevice zwaveDevice)
                {
                    return null;
                }
                else if (zwaveDevice.IfthenType is IfthenType.Radio433
                    or IfthenType.IR
                    or IfthenType.IPCommand
                    or IfthenType.Relay
                    or IfthenType.Serial
                    or IfthenType.RTS)
                {
                    return new GatewayFunctionsUserControl();
                }
                else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.ONOFF))
                {
                    return new OnOffThenElseUserControl();
                }
                else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.ZXT600))
                {
                    return new ZXT600ThenElseUserControl();
                }
                else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXC222))
                {
                    return new ZXT600ThenElseUserControl();
                }
                else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXD211))
                {
                    return new FXD211ThenElseUserControl();
                }
                else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXR211))
                {
                    return new FXR211ThenElseUserControl();
                }
                else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXD220))
                {
                    return new FXD220ThenElseUserControl();
                }
                else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXR5011))
                {
                    //return new FXR5011ThenElseUserControl();
                    return new FXAThenElseUserControl();
                }
                else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXR5012))
                {
                    //return new FXR5011ThenElseUserControl();
                    return new FXAThenElseUserControl();
                }
                else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXR5013))
                {
                    return new FXAThenElseUserControl();
                    //return new FXR5011ThenElseUserControl();
                }
                else if (zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA0400))
                {
                    return new FXAThenElseUserControl();
                }
                else
                {
                    return zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA5016)
                        ? new FXAThenElseUserControl()
                        : zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA0600)
                                            ? new FXA0600ThenElseUserControl()
                                            : zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA0404)
                                                                ? new FXA0600ThenElseUserControl()
                                                                : zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA5029)
                                                                                    ? new FXAThenElseUserControl()
                                                                                    : zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA3011)
                                                                                                        ? new FXAThenElseUserControl()
                                                                                                        : zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA5014)
                                                                                                                            ? new FXAThenElseUserControl()
                                                                                                                            : zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA3012)
                                                                                                                                                ? new FXAThenElseUserControl()
                                                                                                                                                : zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA5018)
                                                                                                                                                                    ? new FXAThenElseUserControl()
                                                                                                                                                                    : zwaveDevice.CustomId.Equals(ZwaveModelUtil.FXS69A) ? new FXS69AThenElseUserControl() : (object)null;
                }
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