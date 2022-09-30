using System;
using System.Globalization;
using System.Windows.Controls;

namespace FC.Domain.Validator
{
    public class NumberRangeValidationRule : ValidationRule
    {
        public int Max { get; set; }

        public int Min { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                return !int.TryParse(value.ToString(), out int number)
                    ? new ValidationResult(false, Properties.Resources.Only_numbers_are_allowed)
                    : (number < Min) || (number > Max)
                    ? new ValidationResult(false, string.Format(CultureInfo.CurrentCulture, Properties.Resources.Please_enter_a_number_in_the_range, Min, Max))
                    : ValidationResult.ValidResult;
            }
            catch (Exception)
            {
                return new ValidationResult(false, Properties.Resources.Only_numbers_are_allowed);
            }
        }
    }
}