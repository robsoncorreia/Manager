using System;
using System.Globalization;
using System.Windows.Controls;

namespace FC.Domain.Validator
{
    public class PortValidationRule : ValidationRule
    {
        public int Min { get; set; }
        public int RangeEnd { get; set; }
        public int RangeInit { get; set; }
        public string TextMinError { get; set; }
        public string TextNumberInvalid { get; set; }
        public string TextRangeError { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string text;

            try
            {
                if (value is null)
                {
                    return new ValidationResult(false, $"Field is required.");
                }

                if (((string)value).Length > 3)
                {
                    text = (string)value;
                }
            }
            catch (Exception)
            {
                return new ValidationResult(false, $"Field is required.");
            }

            text = (string)value;

            return !int.TryParse(text, out int result)
                ? new ValidationResult(false, TextNumberInvalid)
                : text.Length < Min
                ? new ValidationResult(false, TextMinError)
                : result < RangeInit || result > RangeEnd ? new ValidationResult(false, TextRangeError) : new ValidationResult(true, null);
        }
    }
}