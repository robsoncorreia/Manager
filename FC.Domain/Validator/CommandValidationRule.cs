using System;
using System.Globalization;
using System.Windows.Controls;

namespace FC.Domain.Validator
{
    public class CommandValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                command = (string)value;
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Illegal characters or {e.Message}");
            }

            return string.IsNullOrEmpty(command) ? new ValidationResult(false, "Please fill field.") : new ValidationResult(true, null);
        }

        private string command;
    }
}