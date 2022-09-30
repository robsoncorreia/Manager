using System;
using System.Globalization;
using System.Windows.Controls;

namespace FC.Domain.Validator
{
    public class FutureDateValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return !DateTime.TryParse((value ?? "").ToString(),
                CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces,
                out DateTime time)
                ? new ValidationResult(false, "Invalid date")
                : time.Date < DateTime.Now.Date
                ? new ValidationResult(false, "Future date required")
                : ValidationResult.ValidResult;
        }
    }
}