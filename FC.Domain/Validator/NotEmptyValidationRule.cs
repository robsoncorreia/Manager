using System.Globalization;
using System.Windows.Controls;

namespace FC.Domain.Validator
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public string TextError { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is null)
            {
                return new ValidationResult(false, TextError);
            }
            else if (string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(false, TextError);
            }
            return ValidationResult.ValidResult;
        }
    }
}