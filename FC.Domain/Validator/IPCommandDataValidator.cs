using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace FC.Domain.Validator
{
    public class IPCommandDataValidator : ValidationRule
    {
        public string ErrorText { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is not string data)
            {
                return new ValidationResult(false, Properties.Resources.Text_in_invalid_format);
            }
            if (!(data.Contains('{') || data.Contains('}') || data.Contains('"') || data.Contains(':')))
            {
                return new ValidationResult(true, null);
            }
            try
            {
                _ = JObject.Parse(data);
                return new ValidationResult(true, null);
            }
            catch (Exception)
            {
                return new ValidationResult(false, Properties.Resources.Json_in_the_wrong_format);
            }
        }
    }
}