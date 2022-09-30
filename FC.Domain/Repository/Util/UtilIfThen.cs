using System.Collections.Generic;
using System.Linq;

namespace FC.Domain.Repository.Util
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
        public const string IFTHENATTACHCOND = "if_then_attach_cond";
        public const string IFTHENCONDUSEDCOM = "{\"command\":\"if_then_cond_used\",\"format\":1,\"invert\":true,\"type\":0}";
        public const string IFTHENCONDUSED = "if_then_cond_used";
        public const string ITRUCOM = "{\"command\":\"ITRU\",\"format\":1,\"invert\":true,\"type\":0}";
        public const string ITRU = "ITRU";
        public const string IFTHENRULESSET = "if_then_rules_set";

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
        public static IList<int> DELAYS = new List<int>(Enumerable.Range(1000, 10000).Where(x => x % 1000 == 0));
        public const string DAYSOFWEEKLIST = "daysOfWeekList";
        public const string CLASSNAME = "IfThenFlex";

        #region Parse

        public const string CONDITIONALIDS = "conditionalIds";

        public const string INSTRUCTIONIDS = "instructionIds";

        public const string RULEID = "ruleId";

        public const string RULEIDS = "ruleIds";

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

        public const string SELECTEDTABINDEXSENSORTYPE = "selectedTabIndexSensorType";

        public const string MULTILEVEL = "multiLevel";

        public const string ISENABLED = "isEnabled";

        public const string GETCLOCKTIME = "get_clock_time";

        public const string FLEXNETFORMAT = "flexnetFormat";

        public const string DATE = "date";

        public const string SELECTEDINDEXTABSCHEDULE = "selectedIndexTabSchedule";

        public const string IFTHENCONDDELETEALLCOMMAND = "{\"command\":\"if_then_cond_delete_all\",\"type\":0}";

        public const string IFTHENCONDDELETEALL = "if_then_cond_delete_all";

        public const string IFTHENRULEDELETEALL = "if_then_rule_delete_all";

        public const string IFTHENRULEDELETEALLCOMMAND = "{\"command\":\"if_then_rule_delete_all\",\"type\":0}";

        public const string STATEINDEX = "stateIndex";

        public const string ROOMTEMPERATURE = "roomTemperature";

        public const string SELECTEDINDEXTHERMOSTATFUNCTION = "selectedIndexThermostatFunction";

        public const string SELECTEDINDEXTHERMOSTATMODE = "selectedIndexThermostatMode";

        public static string SELECTEDINDEXTHERMOSTATFAN = "selectedIndexThermostatFan";

        #endregion Parse
    }
}