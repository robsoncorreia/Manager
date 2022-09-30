using System;
using System.Globalization;
using System.Windows.Controls;

namespace ConfigurationFlexCloudHubBlaster.Validator
{
    public class ComboboxValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (value is null)
                {
                    return new ValidationResult(false, Properties.Resources.Select_a_combobox_item);
                }
                if (!(int.TryParse(value.ToString(), out int index)))
                {
                    return new ValidationResult(false, Properties.Resources.Select_a_combobox_item);
                }
                if (index < 0)
                {
                    return new ValidationResult(false, Properties.Resources.Select_a_combobox_item);
                }

                return new ValidationResult(true, null);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, Properties.Resources.Select_a_combobox_item);
            }
        }
    }
}