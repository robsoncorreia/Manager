using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;

namespace FC.Domain.Validator
{
    public class TextBoxValidationRule : ValidationRule
    {
        public int Max { get; set; }
        public int Min { get; set; }
        public string TextError { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string text = string.Empty;

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
            catch (Exception e)
            {
                Debug.Write(e.StackTrace);
                return new ValidationResult(false, $"Field is required.");
            }

            return (text.Length < Min) || (text.Length > Max) ? new ValidationResult(false, TextError) : new ValidationResult(true, null);
        }
    }
}