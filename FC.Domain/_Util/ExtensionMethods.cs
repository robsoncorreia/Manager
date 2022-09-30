using CommunityToolkit.Mvvm.ComponentModel;
using FC.Domain._Util;
using FC.Domain.Model;
using FC.Domain.Model.Device;
using FC.Domain.Model.FCC;
using FC.Domain.Model.IpCommand;
using FC.Domain.Model.IR;
using FC.Domain.Model.Project;
using FC.Domain.Repository.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Parse;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FC.Domain.Util
{
    public static class ExtensionMethods
    {
        public static string GetEnumDescription(this Enum enumObj)
        {
            if (enumObj is null)
            {
                return string.Empty;
            }

            if (enumObj.GetType().GetField(enumObj.ToString()) is not FieldInfo fieldInfo)
            {
                return string.Empty;
            }

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                return attrib.Description;
            }
        }

        private static string Hex2binary(string hexvalue)
        {
            string binaryval = Convert.ToString(Convert.ToInt32(hexvalue, 16), 2);
            string newBinary = string.Empty;
            for (int i = 0; i < 8 - binaryval.Length; i++)
            {
                newBinary += 0;
            }
            return Reverse($"{newBinary}{binaryval}");
        }

        public static string Reverse(string s)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private static bool[] RulesUsed(string response)
        {
            bool[] rulesUsed = new bool[256];

            string binary = string.Empty;

            for (int i = 0; i < response.Length; i += 2)
            {
                string teste = response.Substring(i, 2);
                binary += Hex2binary(teste);
            }

            for (int i = 0; i < binary.Length; i++)
            {
                rulesUsed[i] = binary[i].Equals('1');
            }
            return rulesUsed;
        }

        public static string StringToNextRuleOrConditionalFree(this string response)
        {
            bool[] rulesUsed = RulesUsed(response);

            int ruleFree = 0;

            for (int i = 0; i < rulesUsed.Length; i++)
            {
                if (!rulesUsed[i])
                {
                    ruleFree = i;
                    i = 257;
                }
            }

            return $"{ruleFree:X2}";
        }

        public static void ZwaveDeviceToIfThenParseObject(this ZwaveDevice zwaveDevice, ProjectModel project)
        {
            if (zwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(zwaveDevice));
            }

            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            zwaveDevice.ParseObject ??= new ParseObject(UtilIfThen.IFTHENOBJECTCLASS);
            zwaveDevice.ParseObject[UtilIfThen.INDEX] = zwaveDevice.Index;
            zwaveDevice.ParseObject[UtilIfThen.ACTIONSRTSSOMFY] = (int)zwaveDevice.ActionsRTSSomfy;
            zwaveDevice.ParseObject[AppConstants.SELECTEDINDEXTABSCHEDULE] = zwaveDevice.SelectedIndexTabSchedule;
            zwaveDevice.ParseObject[UtilIfThen.IFTHENTYPE] = (int)zwaveDevice.IfthenType;
            zwaveDevice.ParseObject[UtilIfThen.INSTRUCTIONTYPE] = (int)zwaveDevice.InstructionType;
            zwaveDevice.ParseObject[UtilIfThen.ISON] = zwaveDevice.IsOn;
            zwaveDevice.ParseObject[UtilIfThen.DELAY] = zwaveDevice.DelayIfThen;
            zwaveDevice.ParseObject[UtilIfThen.SELECTEDINDEXENDPOINT] = zwaveDevice.SelectedIndexEndpoint;
            zwaveDevice.ParseObject[UtilIfThen.SELECTEDINDEXLOGICGATEIFTHEN] = zwaveDevice.SelectedIndexLogicGateIfThen;
            zwaveDevice.ParseObject[UtilIfThen.SELECTEDINDEXOPERATORTYPE] = zwaveDevice.SelectedIndexOperatorType;
            zwaveDevice.ParseObject[UtilIfThen.NAME] = zwaveDevice.Name;
            zwaveDevice.ParseObject[UtilIfThen.GATEWAYFUNCTIONNAME] = zwaveDevice.GatewayFunctionName;
            zwaveDevice.ParseObject[UtilIfThen.GATEWAYFUNCTIONUID] = zwaveDevice.GatewayFunctionUID;
            zwaveDevice.ParseObject[UtilIfThen.GATEWAYFUNCTIONMEMORYID] = zwaveDevice.GatewayFunctionMemoryId;
            zwaveDevice.ParseObject[UtilIfThen.GATEWAYNAME] = zwaveDevice.GatewayName;
            zwaveDevice.ParseObject[UtilIfThen.SELECTEDINDEXDATETYPE] = zwaveDevice.SelectedIndexDateType;
            zwaveDevice.ParseObject[UtilIfThen.SELECTEDDATETYPE] = (int)zwaveDevice.SelectedDateType;
            zwaveDevice.ParseObject[UtilIfThen.SELECTEDINDEXDAYSOFWEEK] = zwaveDevice.SelectedIndexDaysOfWeek;
            zwaveDevice.ParseObject[UtilIfThen.SELECTEDDAYSOFWEEK] = (int)zwaveDevice.SelectedDaysOfWeek;
            zwaveDevice.ParseObject[UtilIfThen.SELECTEDINDEXOPERATORSTYPE] = zwaveDevice.SelectedIndexOperatorsType;
            zwaveDevice.ParseObject[UtilIfThen.SELECTEDOPERATORSTYPESCHEDULE] = (int)zwaveDevice.SelectedOperatorsTypeSchedule;
            zwaveDevice.ParseObject[UtilIfThen.TIMEPICKERVALUE] = zwaveDevice.TimePickerValue;
            zwaveDevice.ParseObject[UtilIfThen.VALUESCHEDULE] = zwaveDevice.ValueSchedule;
            zwaveDevice.ParseObject[UtilIfThen.SELECTEDTABINDEXSENSORTYPE] = zwaveDevice.SelectedTabIndexSensorType;
            zwaveDevice.ParseObject[UtilIfThen.DAYSOFWEEKLIST] = JsonConvert.SerializeObject(zwaveDevice.DaysOfWeekList);
            zwaveDevice.ParseObject[UtilIfThen.MULTILEVEL] = zwaveDevice.MultiLevel;
            zwaveDevice.ParseObject[UtilIfThen.STATEINDEX] = zwaveDevice.StateIndex;
            zwaveDevice.ParseObject[UtilIfThen.ROOMTEMPERATURE] = zwaveDevice.RoomTemperature;
            zwaveDevice.ParseObject[UtilIfThen.SELECTEDINDEXTHERMOSTATFUNCTION] = zwaveDevice.SelectedIndexThermostatFunction;
            zwaveDevice.ParseObject[UtilIfThen.SELECTEDINDEXTHERMOSTATMODE] = zwaveDevice.SelectedIndexThermostatMode;
            zwaveDevice.ParseObject[UtilIfThen.SELECTEDINDEXTHERMOSTATFAN] = zwaveDevice.SelectedIndexThermostatFan;

            zwaveDevice.Name ??= zwaveDevice.DefaultName;
            zwaveDevice.AssociationsSerialize = JsonConvert.SerializeObject(ConvertAssociations(zwaveDevice));
            zwaveDevice.ParseObject[AppConstants.ASSOCIATIONDATADEVICEDATABASE] = zwaveDevice.AssociationsSerialize;
            zwaveDevice.ParseObject[AppConstants.NUMBEROFASSOCIATIONGROUPSDEVICEDATABASE] = zwaveDevice.NumberOfAssociationGroups;
            zwaveDevice.ParseObject[AppConstants.DEFAULTNAMEDEVICEDATABASE] = zwaveDevice.DefaultName;
            zwaveDevice.ParseObject[AppConstants.DEVICEIMAGEDEVICEDATABASE] = zwaveDevice.ImageParseFile;
            zwaveDevice.ParseObject[AppConstants.BASICDEVICECLASSDEVICEDATABASE] = zwaveDevice.BasicDeviceClass;
            zwaveDevice.ParseObject[AppConstants.GENERICDEVICECLASSDEVICEDATABASE] = zwaveDevice.GenericDeviceClass;
            zwaveDevice.ParseObject[AppConstants.SPECIFICDEVICECLASSDEVICEDATABASE] = zwaveDevice.SpecificDeviceClass;
            zwaveDevice.ParseObject[AppConstants.MANUFACTURERKEYDEVICEDATABASE] = zwaveDevice.ManufacturerKey;
            zwaveDevice.ParseObject[AppConstants.PRODUCTKEYDEVICEDATABASE] = zwaveDevice.ProductKey;
            zwaveDevice.ParseObject[AppConstants.FIRMWAREVERSIONDEVICEDATABASE] = zwaveDevice.FirmwareVersion;
            zwaveDevice.ParseObject[AppConstants.CRYPTOGRAPHYDEVICEDATABASE] = JsonConvert.SerializeObject(zwaveDevice.Cryptographys, Formatting.Indented);
            zwaveDevice.ParseObject[AppConstants.COMMANDCLASSESDEVICEDATABASE] = JsonConvert.SerializeObject(zwaveDevice.CommandClasses, Formatting.Indented);
            zwaveDevice.ParseObject[AppConstants.DEVICEENDPOINTSDEVICEDATABASE] = JsonConvert.SerializeObject(zwaveDevice.Endpoints, Formatting.Indented);
            zwaveDevice.ParseObject[AppConstants.ASSOCIATIONGROUPSDEVICEDATABASE] = JsonConvert.SerializeObject(zwaveDevice.AssociationGroups, Formatting.Indented);
            zwaveDevice.ParseObject[AppConstants.NAMEDEVICEDATABASE] = zwaveDevice.Name;
            zwaveDevice.ParseObject[AppConstants.MODULEIDDEVICEDATABASE] = zwaveDevice.ModuleId;
        }

        private static AssociationGroup[] ConvertAssociations(ZwaveDevice device)
        {
            AssociationGroup[] associations = new AssociationGroup[device.Associations.Count];

            for (int i = 0; i < associations.Length; i++)
            {
                associations[i] = new AssociationGroup
                {
                    Name = $"{Properties.Resources.Group} {i + 1}",
                    MaxRegister = device.Associations.ElementAt(i).MaxRegister,
                    GroupId = device.Associations.ElementAt(i).GroupId,
                    Data = device.Associations.ElementAt(i).ZwaveDevices.Select(x => x.ModuleId).ToList()
                };
            }

            return associations;
        }

        public static bool TryGetIntArray(this JObject json, string key, out int[] ids)
        {
            try
            {
                if (json is null)
                {
                    ids = null;
                    return false;
                }

                JArray jArray = json.Value<JArray>(key);
                ids = jArray.Select(jv => (int)jv).ToArray();
                return true;
            }
            catch (Exception)
            {
                ids = null;
                return false;
            }
        }

        public static void ParseToVoiceCommand(this GatewayModel gateway)
        {
            if (gateway is null)
            {
                throw new ArgumentNullException(nameof(gateway));
            }
            if (gateway.RemoteAccessStandaloneModel.Commands.Any())
            {
                gateway.RemoteAccessStandaloneModel.Commands.Clear();
            }

            if (gateway.IRsGateway.Any())
            {
                foreach (IRModel ir in gateway.IRsGateway)
                {
                    gateway.RemoteAccessStandaloneModel.Commands.Add(new VoiceAssistantCommandModel
                    {
                        MemoryId = ir.MemoryId,
                        Name = ir.Description,
                        Type = "IR",
                        CommandTypeVoiceAssistant = CommandTypeVoiceAssistant.IR
                    });
                }
            }

            if (gateway.Radios433Gateway.Any())
            {
                foreach (Model.FlexCloudClone.RadioModel radio433 in gateway.Radios433Gateway)
                {
                    gateway.RemoteAccessStandaloneModel.Commands.Add(new VoiceAssistantCommandModel
                    {
                        MemoryId = radio433.MemoryId,
                        Name = radio433.Description,
                        Type = "Radio 433",
                        CommandTypeVoiceAssistant = CommandTypeVoiceAssistant.Radio433
                    });
                }
            }

            if (gateway.IpCommands.Any())
            {
                foreach (IpCommandModel ip in gateway.IpCommands)
                {
                    gateway.RemoteAccessStandaloneModel.Commands.Add(new VoiceAssistantCommandModel
                    {
                        MemoryId = ip.MemoryId,
                        Name = ip.Name,
                        Type = "IP Command",
                        CommandTypeVoiceAssistant = CommandTypeVoiceAssistant.IPCommand
                    });
                }
            }

            if (gateway.RadiosRTSGateway.Any())
            {
                foreach (Model.FlexCloudClone.RadioModel ip in gateway.RadiosRTSGateway)
                {
                    gateway.RemoteAccessStandaloneModel.Commands.Add(new VoiceAssistantCommandModel
                    {
                        MemoryId = ip.MemoryId,
                        Name = ip.Description,
                        Type = "RTS",
                        CommandTypeVoiceAssistant = CommandTypeVoiceAssistant.RTS
                    });
                }
            }
        }

        public static string StringToIPCommandFormat(this IpCommandModel ipCommand)
        {
            if (ipCommand is null)
            {
                return string.Empty;
            }

            switch (Enum.Parse(typeof(CommandTypeEnum), ipCommand.CommandTypeIndex + ""))
            {
                case CommandTypeEnum.Hexa:
                    return $"<{ipCommand.Command}>";

                case CommandTypeEnum.Base64:
                    return ipCommand.Command;

                case CommandTypeEnum.Text:
                    if (ipCommand.Command.TryParseJObject() is not null)
                    {
                        return ipCommand.Command.Replace("\"", "\\\"");
                    }
                    return ipCommand.Command;
            }
            return string.Empty;
        }

        public static GatewayModel StringEthernetNetworkStatusToGateway(this string response, string ip, int port)
        {
            try
            {
                if (string.IsNullOrEmpty(response))
                {
                    return null;
                }

                if (string.IsNullOrEmpty(ip))
                {
                    return null;
                }

                if (response.TryParseJObject() is not JObject json)
                {
                    return null;
                }

                if (!json.ContainsKey(UtilGateway.COMMAND))
                {
                    return null;
                }

                using GatewayModel selectedGateway = new()
                {
                    ConnectionType = ConnectionType.Default,
                    LocalIP = ip,
                    LocalPortUDP = port
                };

                if (json.ContainsKey(UtilGateway.MACADDRESS))
                {
                    selectedGateway.MacAddressEthernet = json.JArrayToMAC(UtilGateway.MACADDRESS);
                    selectedGateway.Pin = selectedGateway.MacAddressEthernet.Substring(12, selectedGateway.MacAddressEthernet.Length - 12).Replace(":", "");
                }
                if (json.ContainsKey(UtilGateway.PRODUCT))
                {
                    selectedGateway.ProductId = json.Value<int>(UtilGateway.PRODUCT);
                }

                return selectedGateway;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static GatewayModel GCRToGateway(this string response, string ip, int port)
        {
            try
            {
                if (string.IsNullOrEmpty(response))
                {
                    return null;
                }

                if (string.IsNullOrEmpty(ip))
                {
                    return null;
                }

                using GatewayModel selectedGateway = new()
                {
                    ConnectionType = ConnectionType.Default,
                    LocalIP = ip,
                    LocalPortUDP = port,
                    GatewayModelEnum = (GatewayModelEnum)int.Parse(response.Substring(75, 4), System.Globalization.NumberStyles.HexNumber),
                    MacAddressEthernet = ToMacAddress(response.Substring(40, 12)),
                    Pin = response.Substring(48, 4),
                    Firmware = response.Substring(71, 4)
                };

                return selectedGateway;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string ToMacAddress(string macaddress)
        {
            string regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
            string replace = "$1:$2:$3:$4:$5:$6";
            return Regex.Replace(macaddress, regex, replace);
        }

        public static int BuildToInt(this JObject json, string key)
        {
            try
            {
                return json is null ? -1 : string.IsNullOrEmpty(key) ? -1 : int.TryParse(json.Value<string>(key), out int build) ? build : -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static string StringToMac(this JObject json, string key)
        {
            try
            {
                return json is null
                    ? null
                    : string.IsNullOrEmpty(key)
                    ? null
                    : json.Value<string>(key).Insert(2, ":").Insert(5, ":").Insert(8, ":").Insert(11, ":").Insert(14, ":");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string IPToArray(this string ip)
        {
            try
            {
                return ip is null ? null : $"[{ip.Replace('.', ',')}]";
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string JArrayToIP(this JObject json, string key)
        {
            if (json is null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            try
            {
                JArray jArray = json.Value<JArray>(key);
                int[] items = jArray.Select(jv => (int)jv).ToArray();
                string join = string.Join(".", items);
                return join;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string JArrayToMAC(this JObject json, string key)
        {
            try
            {
                if (json is null)
                {
                    return null;
                }

                if (string.IsNullOrEmpty(key))
                {
                    return null;
                }

                JArray jArray = json.Value<JArray>(key);
                string[] items = jArray.Select(jv => $"{(int)jv:X2}").ToArray();
                string join = string.Join(":", items);
                return join;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static JObject TryParseJObject(this string resp)
        {
            if (string.IsNullOrEmpty(resp))
            {
                throw new ArgumentException($"'{nameof(resp)}' não pode ser nulo nem vazio.", nameof(resp));
            }

            try
            {
                return JObject.Parse(resp);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool TryParseJObject(this string resp, out JObject json)
        {
            if (string.IsNullOrEmpty(resp))
            {
                throw new ArgumentException($"'{nameof(resp)}' não pode ser nulo nem vazio.", nameof(resp));
            }

            try
            {
                json = JObject.Parse(resp);
                return true;
            }
            catch (Exception)
            {
                json = null;
                return false;
            }
        }

        public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
        {
            List<PropertyInfo> sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
            List<PropertyInfo> destProps = typeof(TU).GetProperties()
                    .Where(x => x.CanWrite)
                    .ToList();

            foreach (PropertyInfo sourceProp in sourceProps)
            {
                if (destProps.Any(x => x.Name == sourceProp.Name))
                {
                    PropertyInfo p = destProps.First(x => x.Name == sourceProp.Name);
                    if (p.CanWrite)
                    { // check if the property can be set or no.
                        p.SetValue(dest, sourceProp.GetValue(source, null), null);
                    }
                }
            }
        }

        public static ImageSource ByteToImage(this ObservableRecipient viewModel,
                                              byte[] imageData,
                                              int decodePixelWidth = 233)
        {
            BitmapImage biImg = new();
            MemoryStream ms = new(imageData);
            biImg.BeginInit();
            biImg.DecodePixelWidth = decodePixelWidth;
            biImg.StreamSource = ms;
            biImg.EndInit();

            ImageSource imgSrc = biImg;

            return imgSrc;
        }

        public static string[] ExportToExcel<T>(List<T> model)
        {
            List<string> lines = new();

            IEnumerable<PropertyDescriptor> props = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>();

            string header = string.Join(";", props.ToList().Select(x => x.Name));

            lines.Add(header);

            IEnumerable<string> valueLines = model.Select(row => string.Join(";", header.Split(';').Select(a => row.GetType().GetProperty(a).GetValue(row, null))));

            lines.AddRange(valueLines);

            return lines.ToArray();
        }
    }
}