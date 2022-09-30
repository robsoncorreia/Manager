using System;
using System.Globalization;
using System.Windows.Controls;

namespace FC.Domain.Validator
{
    public class ComboboxValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                return value is null
                    ? new ValidationResult(false, Properties.Resources.Select_a_combobox_item)
                    : !int.TryParse(value.ToString(), out int index)
                    ? new ValidationResult(false, Properties.Resources.Select_a_combobox_item)
                    : index < 0 ? new ValidationResult(false, Properties.Resources.Select_a_combobox_item) : new ValidationResult(true, null);
            }
            catch (Exception)
            {
                return new ValidationResult(false, Properties.Resources.Select_a_combobox_item);
            }
        }
    }
}