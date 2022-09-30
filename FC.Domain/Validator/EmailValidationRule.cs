using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace FC.Domain.Validator
{
    public class EmailRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                email = (string)value;
            }
            catch (Exception e)
            {
                return new ValidationResult(false, e.Message);
            }

            Regex regex = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            return string.IsNullOrEmpty(email)
                ? new ValidationResult(false, Properties.Resources.Please_enter_an_email)
                : !regex.Match(email).Success
                    ? new ValidationResult(false, Properties.Resources.Please_enter_an_email_valid)
                    : ValidationResult.ValidResult;
        }

        private string email;
    }
}