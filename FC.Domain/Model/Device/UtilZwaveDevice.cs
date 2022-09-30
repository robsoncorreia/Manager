using FC.Domain._Util;

namespace FC.Domain.Model.Device
{
    public static class UtilZwaveDevice
    {
        //todo #1 ifthen

        public static string[] DEVICESSUPPORTTEST = new string[] { ZwaveModelUtil.ZXT600, ZwaveModelUtil.DOORLOCK, ZwaveModelUtil.SHUTTER, ZwaveModelUtil.DIMMER, ZwaveModelUtil.ONOFF, ZwaveModelUtil.FXM5012, ZwaveModelUtil.FXC221, ZwaveModelUtil.FXA3000, ZwaveModelUtil.FXA0600, ZwaveModelUtil.FXR5011, ZwaveModelUtil.FXS69A, ZwaveModelUtil.FXR5012, ZwaveModelUtil.FXR5013, ZwaveModelUtil.FXD5011, ZwaveModelUtil.FXR211, ZwaveModelUtil.FXD220, ZwaveModelUtil.FXD211, ZwaveModelUtil.FXD211B, ZwaveModelUtil.FXC222, ZwaveModelUtil.FXA5018, ZwaveModelUtil.FXA3012, ZwaveModelUtil.FXA0400, ZwaveModelUtil.FXA3011, ZwaveModelUtil.FXA5016, ZwaveModelUtil.FXA0404, ZwaveModelUtil.FXA5029, ZwaveModelUtil.FXA5014 };

        public static string[] DEVICESSUPPORTIF = new string[] { ZwaveModelUtil.FXA0600, ZwaveModelUtil.FXR5011, ZwaveModelUtil.FXS69A, ZwaveModelUtil.DomeDoorWindowSensor, ZwaveModelUtil.DomeMotionSensor, ZwaveModelUtil.FXR5012, ZwaveModelUtil.FXR5013, ZwaveModelUtil.FXD5011, ZwaveModelUtil.FXR211, ZwaveModelUtil.FXD220, ZwaveModelUtil.FXD211, ZwaveModelUtil.FXD211B, ZwaveModelUtil.FXC222, ZwaveModelUtil.FXA5018, ZwaveModelUtil.FXA3012, ZwaveModelUtil.FXA0400, ZwaveModelUtil.FXA3011, ZwaveModelUtil.FXA5016, ZwaveModelUtil.FXA0404, ZwaveModelUtil.FXA5014, ZwaveModelUtil.FXA5029, ZwaveModelUtil.ZXT600 };

        public static string[] DEVICESSUPPORTTHENELSE = new string[] { ZwaveModelUtil.ONOFF, ZwaveModelUtil.FXA0600, ZwaveModelUtil.FXR5011, ZwaveModelUtil.FXS69A, ZwaveModelUtil.FXR5012, ZwaveModelUtil.FXR5013, ZwaveModelUtil.FXD5011, ZwaveModelUtil.FXR211, ZwaveModelUtil.FXD220, ZwaveModelUtil.FXD211, ZwaveModelUtil.FXC222, ZwaveModelUtil.FXA5018, ZwaveModelUtil.FXA3012, ZwaveModelUtil.FXA0400, ZwaveModelUtil.FXA5016, ZwaveModelUtil.FXA3011, ZwaveModelUtil.FXA0404, ZwaveModelUtil.FXA5014, ZwaveModelUtil.FXA5029, ZwaveModelUtil.ZXT600 };

        public static int ConvertRange(this int value, int originalStart = 0, int originalEnd = 100, int newStart = 27, int newEnd = 99)
        {
            double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
            int @return = (int)(newStart + ((value - originalStart) * scale));
            return @return < 0 ? 0 : @return;
        }

        public static long ConvertRange(this long value, int originalStart = 0, int originalEnd = 100, int newStart = 27, int newEnd = 99)
        {
            double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
            return (int)(newStart + ((value - originalStart) * scale));
        }
    }
}