using System.Globalization;
using System.Windows.Controls;

namespace FC.Domain.Validator
{
    public class IndexComboxValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return !int.TryParse(value.ToString(), out int index)
                ? new ValidationResult(false, "Number")
                : index < 0 ? new ValidationResult(false, "Select a item.") : new ValidationResult(true, null);
        }
    }
}