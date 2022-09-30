using System;
using System.Globalization;
using System.Windows.Data;

namespace FC.Domain.Converte
{
    public class TemperatureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int temperature)
            {
                return null;
            }

            if (temperature <= 0)
            {
                return "0";
            }

            string temperatureToString = temperature.ToString();

            if (temperature < 10)
            {
                temperatureToString = temperatureToString.Insert(0, "0");
                return temperatureToString.Insert(temperatureToString.Length - 1, ".");
            }

            return temperatureToString.Insert(temperatureToString.Length - 1, ".");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is not string temperature
                ? null
                : (object)(!int.TryParse(temperature.Replace(",", "").Replace(".", ""), out int temp) ? null : temp);
        }
    }
}