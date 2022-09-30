using System.Collections.Generic;
using System.Linq;

namespace ConfigurationFlexCloudHubBlaster.Repository.Util
{
    public static class UtilIfThen
    {
        public const string GETNEXTRULEIDCOM = "@ITRU#";
        public const string GETNEXTCONDITIONALIDCOM = "@ITCU#";
        public const string GETNEXTMACROIDCOM = "{\"command\":\"next_macro_id\", \"type\":0}";
        public const string GETNEXTINSTRUCTIONIDCOM = "{\"command\":\"next_instruction_id\",\"type\":0}";
        public const string COMMAND = "command";
        public const string NEXTMACROID = "next_macro_id";
        public const string MACROID = "macro_id";
        public const string SETMACRO = "set_macro";
        public const string ID = "id";
        public const string NEXTINSTRUCTIONID = "next_instruction_id";
        public const string SETINSTRUCTION = "set_instruction";
        public const string ASSIGNINSTRUCTION = "assign_instruction";

        public const string ANTITHEFTCOMMANDCLASS = "5D";
        public const string BASICCOMMANDCLASS = "20";
        public const string BINARYSENSORCOMMANDCLASS = "30";
        public const string BINARYSWITCHCOMMANDCLASS = "25";
        public const string CENTRALSCENECOMMANDCLASS = "5B";
        public const string COLORSWITCHCOMMANDCLASS = "33";
        public const string DOORLOCKCOMMANDCLASS = "62";
        public const string ENTRYCONTROLCOMMANDCLASS = "6F";
        public const string HRVCONTROLCOMMANDCLASS = "39";
        public const string HRVSTATUSCOMMANDCLASS = "37";
        public const string HUMIDITYCONTROLMODECOMMANDCLASS = "6D";
        public const string HUMIDITYCONTROLOPERATINGSTATECOMMANDCLASS = "6E";
        public const string HUMIDITYCONTROLSETPOINTCOMMANDCLASS = "64";
        public const string IRRIGATIONCOMMANDCLASS = "6B";
        public const string METERCOMMANDCLASS = "32";
        public const string METERTABLEMONITORCOMMANDCLASS = "3D";
        public const string MULTICHANNELCOMMANDCLASS = "60";
        public const string MULTILEVELSENSORCOMMANDCLASS = "31";
        public const string MULTILEVELSWITCHCOMMANDCLASS = "26";
        public const string MULTILEVELTOGGLESWITCHCOMMANDCLASS = "29";
        public const string NOTIFICATIONCOMMANDCLASS = "71";
        public const string POWERLEVELCOMMANDCLASS = "73";
        public const string PULSEMETERCOMMANDCLASS = "35";
        public const string SCENEACTIVATIONCOMMANDCLASS = "2B";
        public const string SCHEDULECOMMANDCLASS = "53";
        public const string SCHEDULEENTRYLOCKCOMMANDCLASS = "4E";
        public const string SIMPLEAVCONTROLCOMMANDCLASS = "94";
        public const string SOUNDSWITCHCOMMANDCLASS = "79";
        public const string THERMOSTATFANMODECOMMANDCLASS = "44";
        public const string THERMOSTATFANSTATECOMMANDCLASS = "45";
        public const string THERMOSTATMODECOMMANDCLASS = "40";
        public const string THERMOSTATOPERATINGSTATECOMMANDCLASS = "42";
        public const string THERMOSTATSETBACKCOMMANDCLASS = "47";
        public const string THERMOSTATSETPOINTCOMMANDCLASS = "43";
        public const string USERCODECOMMANDCLASS = "63";
        public const string IGNORE = "07";
        public const string EQUALS = "03";
        public const string NOTEQUALS = "04";
        public const string INSTRUCTIONID = "instruction_id";
        public const string PLAYMACRO = "play_macro";

        public static long?[] FXS69ACOMMANDCLASS = new long?[] { 94, 134, 114, 90, 115, 50, 133, 89, 37, 32, 39, 112, 43, 44, 122 };
        public static long?[] FXA0600COMMANDCLASS = new long?[] { 94, 96, 134, 114, 90, 133, 89, 115, 32, 122, 37, 39, 43, 45, 136, 112, 119, 129, 34 };
        public static long?[] FXR5011COMMANDCLASS = new long?[] { 94, 133, 89, 142, 96, 85, 134, 114, 90, 115, 37, 39, 112, 44, 43, 91, 32, 122, 239, 38 };

        public static IList<int> DELAYS = new List<int>(Enumerable.Range(1000, 10000).Where(x => x % 1000 == 0));

        public const string CLASSNAME = "IfThenFlex";

        #region Parse

        public const string CONDITIONALIDS = "conditionalIds";

        public const string INSTRUCTIONIDS = "instructionIds";

        public const string RULEID = "ruleId";

        public const string MACROIDTHEN = "macroIdThen";

        public const string MACROIDELSE = "macroIdElse";

        public const string NAME = "name";

        public const string IFTHENS = "ifThens";

        public const string IF = "if";

        public const string THEN = "then";

        public const string ELSE = "else";

        public const string IFTHENOBJECTCLASS = "IfThenObject";

        public const string INDEX = "index";

        public const string IFTHENTYPE = "IfthenType";

        public const string INSTRUCTIONTYPE = "instructionType";

        public const string ISON = "isOn";

        public const string DELAY = "delay";

        public const string NEW = "New";

        public const string SELECTEDINDEXENDPOINT = "selectedIndexEndpoint";

        public const string SELECTEDINDEXOPERATORTYPE = "selectedIndexOperatorType";

        public const string SELECTEDINDEXLOGICGATEIFTHEN = "selectedIndexLogicGateIfThen";

        public const string IPCOMMANDIDS = "ipCommandIds";

        public const string GATEWAYFUNCTIONNAME = "gatewayFunctionName";

        public const string ACTIONSRTSSOMFY = "actionsRTSSomfy";

        public const string GATEWAYFUNCTIONUID = "gatewayFunctionUID";

        public const string GATEWAYFUNCTIONMEMORYID = "gatewayFunctionMemoryId";

        public const string SELECTEDINDEXDATETYPE = "selectedIndexDateType";

        public const string SELECTEDDATETYPE = "selectedDateType";

        public const string SELECTEDINDEXDAYSOFWEEK = "selectedIndexDaysOfWeek";

        public const string SELECTEDDAYSOFWEEK = "selectedDaysOfWeek";

        public const string SELECTEDINDEXOPERATORSTYPE = "selectedIndexOperatorsType";

        public const string SELECTEDOPERATORSTYPESCHEDULE = "selectedOperatorsTypeSchedule";

        public const string TIMEPICKERVALUE = "timePickerValue";

        public const string VALUESCHEDULE = "valueSchedule";

        public const string GATEWAYID = "gatewayId";

        public const string GATEWAYNAME = "gatewayName";

        #endregion Parse
    }
}