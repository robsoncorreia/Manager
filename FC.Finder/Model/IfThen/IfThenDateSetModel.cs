using ConfigurationFlexCloudHubBlaster._Base;

namespace ConfigurationFlexCloudHubBlaster.Model.IfThen
{
    public class IfThenDateSetModel : ModelBase
    {
        private const string COMMANDEXAMPLE = "@ITDSAABBCCDDEEFF#";
        private const string COMMAND = "ITDS";
        private const string RESPONSEOK = "&ITDSOK#";

        /// <summary>
        /// 0~255
        /// </summary>
        public string ConditionalID { get; set; } = "00";

        /// <summary>
        /// Tipo do Date:
        ///0 - Every Amount of Seconds
        ///1 - Every Amount of Minutes
        ///2 - Every Amount of Hours
        ///3 - Every Amount of Days
        ///4 - Every Amount of Months
        ///5 - Every Amount of Years
        ///6 - Compare Clock
        ///7 - -
        ///8 - -
        ///9 - -
        ///10 - Compare Day of the week
        ///11 - -
        ///12 - -
        ///13 - -
        /// </summary>
        public string DataType { get; set; } = "00";

        public string ValueCC { get; set; } = "00";
        public string ValueDD { get; set; } = "00";
        public string ValueEE { get; set; } = "00";
        public string ValueFF { get; set; } = "00";

        public override string ToString()
        {
            return $"@{COMMAND}{ConditionalID}{DataType}{ValueCC}{ValueDD}{ValueEE}{ValueFF}#";
        }
    }
}