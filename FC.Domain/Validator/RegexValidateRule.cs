using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace FC.Domain.Validator
{
    public class RegexValidateRule : ValidationRule
    {
        public string Expression { get; set; }
        public string TextError { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty((string)value))
            {
                return new ValidationResult(false, Properties.Resources.Field_Is_Required);
            }

            Regex rx = new(@"" + Expression + "");

            Match match = rx.Match((string)value);

            return !match.Success ? new ValidationResult(false, TextError) : new ValidationResult(true, null);
        }
    }
}