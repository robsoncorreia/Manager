using ConfigurationFlexCloudHubBlaster._Base;

namespace ConfigurationFlexCloudHubBlaster.Model.IfThen
{
    public class IfThenZwaveSetModel : ModelBase
    {
        private int index = 0;

        public string NextIndex => index++.ToString("X2");

        public IfThenZwaveSetModel()
        {
        }

        public IfThenZwaveSetModel(string command)
        {
            ParseCommandToObject(command);
        }

        private const string COMMAND = "ITZS";

        private const string RESPONSEOK = "&ITZSOK#";

        private const string COMMANDRESPONSEEXAMPLE = "@ITZSAABBCCD1D2D3E1E2E3F1F2F3G1G2G3H1H2H3I1I2I3J1J2J3K1K2K3#";

        /// <summary>
        /// 0~255
        /// </summary>
        public string ConditionalID { get; set; } = "00";

        /// <summary>
        /// 1~253
        /// </summary>
        public string DeviceID { get; set; } = "00";

        public string CommandClass { get; set; } = "00";

        public string OperatorType_D1 { get; set; } = "00";
        public string ByteIndex_D1 { get; set; } = "00";
        public string Value_D1 { get; set; } = "00";
        public string Value_D2 { get; set; } = "00";
        public string Value_D3 { get; set; } = "00";
        public string Value_D4 { get; set; } = "00";

        public string OperatorType_E1 { get; set; } = "00";
        public string ByteIndex_E1 { get; set; } = "00";
        public string Value_E1 { get; set; } = "00";
        public string Value_E2 { get; set; } = "00";
        public string Value_E3 { get; set; } = "00";
        public string Value_E4 { get; set; } = "00";

        public string OperatorType_F1 { get; set; } = "00";
        public string ByteIndex_F1 { get; set; } = "00";
        public string Value_F1 { get; set; } = "00";
        public string Value_F2 { get; set; } = "00";
        public string Value_F3 { get; set; } = "00";
        public string Value_F4 { get; set; } = "00";

        public string OperatorType_G1 { get; set; } = "00";
        public string ByteIndex_G1 { get; set; } = "00";
        public string Value_G1 { get; set; } = "00";
        public string Value_G2 { get; set; } = "00";
        public string Value_G3 { get; set; } = "00";
        public string Value_G4 { get; set; } = "00";

        public string OperatorType_H1 { get; set; } = "00";
        public string ByteIndex_H1 { get; set; } = "00";
        public string Value_H1 { get; set; } = "00";
        public string Value_H2 { get; set; } = "00";
        public string Value_H3 { get; set; } = "00";
        public string Value_H4 { get; set; } = "00";

        public string OperatorType_I1 { get; set; } = "00";
        public string ByteIndex_I1 { get; set; } = "00";
        public string Value_I1 { get; set; } = "00";
        public string Value_I2 { get; set; } = "00";
        public string Value_I3 { get; set; } = "00";
        public string Value_I4 { get; set; } = "00";

        public string OperatorType_J1 { get; set; } = "00";
        public string ByteIndex_J1 { get; set; } = "00";
        public string Value_J1 { get; set; } = "00";
        public string Value_J2 { get; set; } = "00";
        public string Value_J3 { get; set; } = "00";
        public string Value_J4 { get; set; } = "00";

        public string OperatorType_K1 { get; set; } = "00";
        public string ByteIndex_K1 { get; set; } = "00";
        public string Value_K1 { get; set; } = "00";
        public string Value_K2 { get; set; } = "00";
        public string Value_K3 { get; set; } = "00";
        public string Value_K4 { get; set; } = "00";

        public bool IsResponseOK => Response.Equals(RESPONSEOK);

        public override string ToString()
        {
            return $"{INITIALIZER}{COMMAND}{ConditionalID}{DeviceID}{CommandClass}" +
                   $"{OperatorType_D1}{ByteIndex_D1}{Value_D4}{Value_D3}{Value_D2}{Value_D1}" +
                   $"{OperatorType_E1}{ByteIndex_E1}{Value_E4}{Value_E3}{Value_E2}{Value_E1}" +
                   $"{OperatorType_F1}{ByteIndex_F1}{Value_F4}{Value_F3}{Value_F2}{Value_F1}" +
                   $"{OperatorType_G1}{ByteIndex_G1}{Value_G4}{Value_G3}{Value_G2}{Value_G1}" +
                   $"{OperatorType_H1}{ByteIndex_H1}{Value_H4}{Value_H3}{Value_H2}{Value_H1}" +
                   $"{OperatorType_I1}{ByteIndex_I1}{Value_I4}{Value_I3}{Value_I2}{Value_I1}" +
                   $"{OperatorType_J1}{ByteIndex_J1}{Value_J4}{Value_J3}{Value_J2}{Value_J1}" +
                   $"{OperatorType_K1}{ByteIndex_K1}{Value_K4}{Value_K3}{Value_K2}{Value_K1}{FINISHER}";
        }

        public string Response { get; set; }
        public const string INITIALIZER = "@";
        public const string INITIALIZERRESPONSE = "&";
        public const string FINISHER = "#";

        private bool ParseCommandToObject(string command)
        {
            if (command.Length != COMMANDRESPONSEEXAMPLE.Length)
            {
                return false;
            }

            ConditionalID = command.Substring(5, 2);
            DeviceID = command.Substring(7, 2);
            CommandClass = command.Substring(9, 2);

            OperatorType_D1 = command.Substring(11, 2);
            ByteIndex_D1 = command.Substring(13, 2);
            Value_D1 = command.Substring(15, 2);

            OperatorType_E1 = command.Substring(17, 2);
            ByteIndex_E1 = command.Substring(19, 2);
            Value_E1 = command.Substring(21, 2);

            OperatorType_F1 = command.Substring(23, 2);
            ByteIndex_F1 = command.Substring(25, 2);
            Value_F1 = command.Substring(27, 2);

            OperatorType_G1 = command.Substring(29, 2);
            ByteIndex_G1 = command.Substring(31, 2);
            Value_G1 = command.Substring(33, 2);

            OperatorType_H1 = command.Substring(35, 2);
            ByteIndex_H1 = command.Substring(37, 2);
            Value_H1 = command.Substring(39, 2);

            OperatorType_I1 = command.Substring(41, 2);
            ByteIndex_I1 = command.Substring(43, 2);
            Value_I1 = command.Substring(45, 2);

            OperatorType_J1 = command.Substring(47, 2);
            ByteIndex_J1 = command.Substring(49, 2);
            Value_J1 = command.Substring(51, 2);

            OperatorType_K1 = command.Substring(53, 2);
            ByteIndex_K1 = command.Substring(55, 2);
            Value_K1 = command.Substring(57, 2);

            return true;
        }
    }
}