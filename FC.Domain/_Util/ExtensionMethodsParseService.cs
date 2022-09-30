using FC.Domain.Model;
using FC.Domain.Model.Device;
using FC.Domain.Model.FCC;
using FC.Domain.Model.FlexCloudClone;
using FC.Domain.Model.IfThen;
using FC.Domain.Model.License;
using FC.Domain.Model.Project;
using FC.Domain.Model.User;
using FC.Domain.Repository.Util;
using Newtonsoft.Json;
using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FC.Domain._Util
{
    public static class ExtensionMethodsParseService
    {
        #region Project

        public static ProjectModel ParseObjectToProject(this ParseObject parseObject)
        {
            if (parseObject is null)
            {
                throw new ArgumentNullException(nameof(parseObject));
            }

            using ProjectModel project = new()
            {
                ParseObject = parseObject
            };

            if (parseObject.ContainsKey(AppConstants.PROJECTFILEDFOR))
            {
                if (project.FiledFor.Any())
                {
                    project.FiledFor.Clear();
                }

                foreach (object objectId in parseObject.Get<List<object>>(AppConstants.PROJECTFILEDFOR))
                {
                    project.FiledFor.Add(objectId.ToString());
                }
            }

            if (parseObject.ContainsKey(AppConstants.PROJECTUSERSROLE))
            {
                project.RoleUsers = parseObject.Get<ParseRole>(AppConstants.PROJECTUSERSROLE);
            }

            if (parseObject.ContainsKey(AppConstants.PROJECTVERSION))
            {
                project.Version = parseObject.Get<string>(AppConstants.PROJECTVERSION);
            }

            if (parseObject.ContainsKey(AppConstants.PROJECTLICENSE))
            {
                project.LicenseParseObject = parseObject.Get<ParseObject>(AppConstants.PROJECTLICENSE);
            }

            if (parseObject.ContainsKey(AppConstants.PROJECTNAME))
            {
                project.Name = parseObject.Get<string>(AppConstants.PROJECTNAME);
            }

            if (parseObject.ContainsKey(AppConstants.PROJECTDELETEDBY))
            {
                project.DeletedBy = parseObject.Get<ParseUser>(AppConstants.PROJECTDELETEDBY);
            }

            if (parseObject.ContainsKey(AppConstants.PROJECTARCHIVEDBY))
            {
                project.ArchivedBy = parseObject.Get<ParseUser>(AppConstants.PROJECTARCHIVEDBY);
            }

            if (parseObject.ContainsKey(AppConstants.PROJECTPRIMARYCOLOR))
            {
                project.PrimaryColor = parseObject.Get<string>(AppConstants.PROJECTPRIMARYCOLOR);
                project.ConvertColors(project.PrimaryColor);
            }

            if (parseObject.ContainsKey(AppConstants.PROJECTISDELETED))
            {
                project.IsDeleted = parseObject.Get<bool>(AppConstants.PROJECTISDELETED);
            }

            if (parseObject.ContainsKey(AppConstants.PROJECTPROGRAMMERS))
            {
                project.ProgrammersList = parseObject.Get<IList<string>>(AppConstants.PROJECTPROGRAMMERS).ToList();
            }

            if (parseObject.ContainsKey(AppConstants.PROJECTMASTERUSERS))
            {
                project.MasterUsersList = parseObject.Get<IList<string>>(AppConstants.PROJECTMASTERUSERS).ToList();
            }

            if (parseObject.ContainsKey(AppConstants.PROJECTUSERS))
            {
                project.UsersList = parseObject.Get<IList<string>>(AppConstants.PROJECTUSERS).ToList();
            }

            return project;
        }

        public static void ProjectToParseObject(this ProjectModel project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            project.UsersList = project.Users.Select(x => x.ParseUser.ObjectId).ToList();
            project.MasterUsersList = project.MasterUsers.Select(x => x.ParseUser.ObjectId).ToList();
            project.ProgrammersList = project.Programmers.Select(x => x.ParseUser.ObjectId).ToList();

            project.ParseObject ??= new ParseObject(AppConstants.PROJECTCLASS);
            project.ParseObject[AppConstants.PROJECTNAME] = project.Name;
            project.ParseObject[AppConstants.PROJECTPRIMARYCOLOR] = project.PrimaryColor;
            project.ParseObject[AppConstants.PROJECTVERSION] = project.Version;
            project.ParseObject[AppConstants.PROJECTFILEDFOR] = project.FiledFor;
            project.ParseObject[AppConstants.PROJECTPROGRAMMERS] = project.Programmers.Select(x => x.ParseUser.ObjectId).ToArray();
            project.ParseObject[AppConstants.PROJECTMASTERUSERS] = project.MasterUsers.Select(x => x.ParseUser.ObjectId).ToArray();
            project.ParseObject[AppConstants.PROJECTUSERS] = project.Users.Select(x => x.ParseUser.ObjectId).ToArray();
        }

        #endregion Project

        #region Gateway

        public static ParseUserCustom ParseObjectToRemoteAccessStandaloneUsers(this ParseObject parseObject)
        {
            if (parseObject is null)
            {
                throw new ArgumentNullException(nameof(parseObject));
            }

            using ParseUserCustom userCustom = new()
            {
                ParseObject = parseObject
            };

            if (parseObject.ContainsKey("assistants"))
            {
                IDictionary<string, object> dic = parseObject.Get<IDictionary<string, object>>("assistants");

                userCustom.IsGoogleAssistant = (bool)dic.ElementAt(0).Value;
                userCustom.IsAmazonAssistant = (bool)dic.ElementAt(1).Value;
            }

            if (parseObject.ContainsKey("user"))
            {
                userCustom.ParseUser = parseObject.Get<ParseUser>("user");
            }

            return userCustom;
        }

        public static void GatewayToParseObject(this GatewayModel device)
        {
            if (device is null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            device.DefaultName ??= device.Name;

            device.ParseObject ??= new ParseObject(AppConstants.GATEWAYCLASSNAME);

            device.ParseObject[AppConstants.DEVICENAME] = device.Name;
            device.ParseObject[AppConstants.ISPRIMARY] = device.IsPrimary;
            device.ParseObject[AppConstants.DEVICEDEFAULTNAME] = device.DefaultName;
            device.ParseObject[AppConstants.DEVICEIPLOCAL] = device.LocalIP;
            device.ParseObject[AppConstants.DEVICEPORTLOCAL] = device.LocalPortUDP;
            device.ParseObject[AppConstants.DEVICEFIRMWARE] = device.Firmware;
            device.ParseObject[AppConstants.DEVICEBUILD] = device.Build;
            device.ParseObject[AppConstants.DEVICEPRODUCTID] = device.ProductId;
            device.ParseObject[AppConstants.DEVICEMACADDRESS] = device.LocalMacAddress;
            device.ParseObject[AppConstants.DEVICEHOMEID] = device.HomeId;
            device.ParseObject[AppConstants.DEVICEPIN] = device.Pin;
            device.ParseObject[AppConstants.DEVICEAPIP] = device.APIP;
            device.ParseObject[AppConstants.DEVICEAPPASSWORD] = device.APPassword;
            device.ParseObject[AppConstants.DEVICEAPSSID] = device.APSSID;
            device.ParseObject[AppConstants.DEVICEAPSTATUS] = device.SelectedIndexAPStatus;
            device.ParseObject[AppConstants.DEVICEAPDHCP] = device.SelectedIndexAPDHCP;
            device.ParseObject[AppConstants.DEVICECONNECTIONTYPE] = device.ConnectionType.ToString();
            device.ParseObject[AppConstants.DEVICEZWAVEFREQUENCY] = device.ZwaveFrequency.ToString();
            device.ParseObject[AppConstants.DEVICEGATEWAYTYPE] = (int)device.GatewayModelEnum;
            device.ParseObject[AppConstants.DEVICEUID] = device.UID;
            device.ParseObject[AppConstants.DEVICEISIRBLASTERAVAILABLE] = device.IsIRBlasterAvailable;
            device.ParseObject[AppConstants.DEVICEIRCOMMANDSLIMIT] = device.IRCommandsLimit;
            device.ParseObject[AppConstants.DEVICEIRSIZELIMIT] = device.IRSizeLimit;

            #region Ethernet

            device.ParseObject[AppConstants.DEVICECURRENTIPETHERNET] = device.CurrentIpEthernet;
            device.ParseObject[AppConstants.DEVICEMACADDRESSETHERNET] = device.MacAddressEthernet;
            device.ParseObject[AppConstants.DEVICESTATICIPETHERNET] = device.StaticIPEthernet;
            device.ParseObject[AppConstants.DEVICEROUTERGATEWAYETHERNET] = device.RouterGatewayEthernet;
            device.ParseObject[AppConstants.DEVICEMASKETHERNET] = device.MaskEthernet;
            device.ParseObject[AppConstants.DEVICEPORTETHERNETTCP] = device.TcpPortEthernet;
            device.ParseObject[AppConstants.DEVICEPORTETHERNETUDP] = device.UdpPortEthernet;
            device.ParseObject[AppConstants.DEVICEISETHERNETSTATICIP] = device.SelectedIndexIPTypeEthernet != 0;

            #endregion Ethernet

            #region WiFi

            device.ParseObject[AppConstants.DEVICECURRENTIPWIFI] = device.CurrentIpWiFi;
            device.ParseObject[AppConstants.DEVICEMACADDRESSWIFI] = device.MacAddressWiFi;
            device.ParseObject[AppConstants.DEVICEWIFIPASSWORD] = StringCipher.Encrypt(device.Password);
            device.ParseObject[AppConstants.DEVICEWIFISSID] = device.SSID;
            device.ParseObject[AppConstants.DEVICESTATICIPWIFI] = device.StaticIPWiFi;
            device.ParseObject[AppConstants.DEVICEROUTERGATEWAYWIFI] = device.RouterGatewayWiFi;
            device.ParseObject[AppConstants.DEVICEMASKWIFI] = device.MaskWiFi;
            device.ParseObject[AppConstants.DEVICEISWIFISTATICIP] = device.SelectedIndexIPTypeWiFi != 0;
            device.ParseObject[AppConstants.DEVICETCPPORTWIFI] = device.TcpPortWiFi;
            device.ParseObject[AppConstants.DEVICEUDPPORTWIFI] = device.UdpPortWifi;
            device.ParseObject[AppConstants.DEVICEWIFIENABLE] = device.SelectedIndexWiFiStatus == 1;

            #endregion WiFi
        }

        public static void ParseObjectToRemoteAccessStandalone(this GatewayModel gateway, ParseObject parseObject)
        {
            if (gateway is null)
            {
                throw new ArgumentNullException(nameof(gateway));
            }

            if (parseObject is null)
            {
                throw new ArgumentNullException(nameof(parseObject));
            }

            gateway.RemoteAccessStandaloneModel.ParseObject = parseObject;

            if (parseObject.ContainsKey(AppConstants.REMOTEACCESSSTANDALONEUID))
            {
                gateway.RemoteAccessStandaloneModel.UID = parseObject.Get<string>(AppConstants.REMOTEACCESSSTANDALONEUID);
            }
        }

        public static GatewayModel ParseObjectToGateway(this ParseObject parseObject)
        {
            if (parseObject is null)
            {
                throw new ArgumentNullException(nameof(parseObject));
            }

            using GatewayModel gateway = new()
            {
                ParseObject = parseObject
            };

            if (parseObject.ContainsKey(AppConstants.ISPRIMARY))
            {
                gateway.IsPrimary = parseObject.Get<bool>(AppConstants.ISPRIMARY);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEAPDHCP))
            {
                gateway.SelectedIndexAPDHCP = parseObject.Get<int>(AppConstants.DEVICEAPDHCP);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEAPSTATUS))
            {
                gateway.SelectedIndexAPStatus = parseObject.Get<int>(AppConstants.DEVICEAPSTATUS);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEISIRBLASTERAVAILABLE))
            {
                gateway.IsIRBlasterAvailable = parseObject.Get<bool>(AppConstants.DEVICEISIRBLASTERAVAILABLE);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEIRCOMMANDSLIMIT))
            {
                gateway.IRCommandsLimit = parseObject.Get<int>(AppConstants.DEVICEIRCOMMANDSLIMIT);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEIRSIZELIMIT))
            {
                gateway.IRSizeLimit = parseObject.Get<int>(AppConstants.DEVICEIRSIZELIMIT);
            }

            if (parseObject.ContainsKey(AppConstants.GATEWAYREMOTEACCESSSTANDALONE))
            {
                gateway.ParseObjectToRemoteAccessStandalone(parseObject.Get<ParseObject>(AppConstants.GATEWAYREMOTEACCESSSTANDALONE));
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEGATEWAYTYPE))
            {
                gateway.GatewayModelEnum = (GatewayModelEnum)parseObject.Get<int>(AppConstants.DEVICEGATEWAYTYPE);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEZWAVEFREQUENCY))
            {
                gateway.ZwaveFrequency = (ZwaveFrequencyEnum)Enum.Parse(typeof(ZwaveFrequencyEnum), parseObject.Get<string>(AppConstants.DEVICEZWAVEFREQUENCY), true);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEBLACKLISTUSERS))
            {
                foreach (object objectId in parseObject.Get<ICollection<object>>(AppConstants.DEVICEBLACKLISTUSERS))
                {
                    gateway.Blacklist.Add((string)objectId);
                }

                if (gateway.Blacklist.Any())
                {
                    gateway.IsCurrentUserOnBlacklist = gateway.Blacklist.Contains(ParseUser.CurrentUser.ObjectId);
                }
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEUID))
            {
                gateway.UID = parseObject.Get<string>(AppConstants.DEVICEUID);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEAPIP))
            {
                gateway.APIP = parseObject.Get<string>(AppConstants.DEVICEAPIP);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEAPPASSWORD))
            {
                gateway.APPassword = parseObject.Get<string>(AppConstants.DEVICEAPPASSWORD);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEAPSSID))
            {
                gateway.APSSID = parseObject.Get<string>(AppConstants.DEVICEAPSSID);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEPIN))
            {
                gateway.Pin = parseObject.Get<string>(AppConstants.DEVICEPIN);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICECURRENTIPWIFI))
            {
                gateway.CurrentIpWiFi = parseObject.Get<string>(AppConstants.DEVICECURRENTIPWIFI);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICECURRENTIPETHERNET))
            {
                gateway.CurrentIpEthernet = parseObject.Get<string>(AppConstants.DEVICECURRENTIPETHERNET);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEPRODUCTID))
            {
                gateway.ProductId = parseObject.Get<int>(AppConstants.DEVICEPRODUCTID);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEBUILD))
            {
                gateway.Build = parseObject.Get<int>(AppConstants.DEVICEBUILD);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEMACADDRESSWIFI))
            {
                gateway.MacAddressWiFi = parseObject.Get<string>(AppConstants.DEVICEMACADDRESSWIFI);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEMACADDRESSETHERNET))
            {
                gateway.MacAddressEthernet = parseObject.Get<string>(AppConstants.DEVICEMACADDRESSETHERNET);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEWIFISSID))
            {
                gateway.SSID = parseObject.Get<string>(AppConstants.DEVICEWIFISSID);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEWIFIPASSWORD))
            {
                try
                {
                    gateway.Password = StringCipher.Decrypt(parseObject.Get<string>(AppConstants.DEVICEWIFIPASSWORD));
                }
                catch (Exception)
                {
                }
            }

            if (parseObject.ContainsKey(AppConstants.DEVICENAME))
            {
                gateway.Name = parseObject.Get<string>(AppConstants.DEVICENAME);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEFIRMWARE))
            {
                gateway.Firmware = parseObject.Get<string>(AppConstants.DEVICEFIRMWARE);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEIPLOCAL))
            {
                gateway.LocalIP = parseObject.Get<string>(AppConstants.DEVICEIPLOCAL);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEHOMEID))
            {
                gateway.HomeId = parseObject.Get<string>(AppConstants.DEVICEHOMEID);
                gateway.ModuleId = GetModuleIdGateway(gateway.HomeId);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEPORTLOCAL))
            {
                gateway.LocalPortUDP = parseObject.Get<int>(AppConstants.DEVICEPORTLOCAL);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEMACADDRESS))
            {
                gateway.LocalMacAddress = parseObject.Get<string>(AppConstants.DEVICEMACADDRESS);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICESTATICIPETHERNET))
            {
                gateway.StaticIPEthernet = parseObject.Get<string>(AppConstants.DEVICESTATICIPETHERNET);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEROUTERGATEWAYETHERNET))
            {
                gateway.RouterGatewayEthernet = parseObject.Get<string>(AppConstants.DEVICEROUTERGATEWAYETHERNET);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEMASKETHERNET))
            {
                gateway.MaskEthernet = parseObject.Get<string>(AppConstants.DEVICEMASKETHERNET);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEPORTETHERNETTCP))
            {
                gateway.TcpPortEthernet = parseObject.Get<int>(AppConstants.DEVICEPORTETHERNETTCP);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEPORTETHERNETUDP))
            {
                gateway.UdpPortEthernet = parseObject.Get<int>(AppConstants.DEVICEPORTETHERNETUDP);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEWIFIENABLE))
            {
                gateway.SelectedIndexWiFiStatus = parseObject.Get<bool>(AppConstants.DEVICEWIFIENABLE) ? 1 : 0;
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEISENABLEBLACKLIST))
            {
                gateway.IsEnableBlackList = parseObject.Get<bool>(AppConstants.DEVICEISENABLEBLACKLIST);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEISETHERNETSTATICIP))
            {
                gateway.SelectedIndexIPTypeEthernet = parseObject.Get<bool>(AppConstants.DEVICEISETHERNETSTATICIP) ? 1 : 0;
            }

            if (parseObject.ContainsKey(AppConstants.DEVICESTATICIPWIFI))
            {
                gateway.StaticIPWiFi = parseObject.Get<string>(AppConstants.DEVICESTATICIPWIFI);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEROUTERGATEWAYWIFI))
            {
                gateway.RouterGatewayWiFi = parseObject.Get<string>(AppConstants.DEVICEROUTERGATEWAYWIFI);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEMASKWIFI))
            {
                gateway.MaskWiFi = parseObject.Get<string>(AppConstants.DEVICEMASKWIFI);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEISWIFISTATICIP))
            {
                gateway.SelectedIndexIPTypeWiFi = parseObject.Get<bool>(AppConstants.DEVICEISWIFISTATICIP) ? 1 : 0;
            }

            if (parseObject.ContainsKey(AppConstants.DEVICETCPPORTWIFI))
            {
                gateway.TcpPortWiFi = parseObject.Get<int>(AppConstants.DEVICETCPPORTWIFI);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEUDPPORTWIFI))
            {
                gateway.UdpPortWifi = parseObject.Get<int>(AppConstants.DEVICEUDPPORTWIFI);
            }
            if (parseObject.ContainsKey(AppConstants.DEVICEDEFAULTNAME))
            {
                gateway.DefaultName = parseObject.Get<string>(AppConstants.DEVICEDEFAULTNAME);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICECONNECTIONTYPE))
            {
                if (Enum.TryParse(parseObject.Get<string>(AppConstants.DEVICECONNECTIONTYPE), out ConnectionType connectionType))
                {
                    gateway.ConnectionType = connectionType;
                }
            }

            gateway.GetConnectionType();

            return gateway;
        }

        private static int GetModuleIdGateway(string homeId)
        {
            return homeId is null
                ? -1
                : !Regex.IsMatch(homeId, "^[1234567890ABCDEF]{10}$")
                ? -1
                : int.Parse(homeId.Substring(8, 2), System.Globalization.NumberStyles.HexNumber);
        }

        #endregion Gateway

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

        #region ZwaveDevice

        public static void ZwaveDeviceToParseObject(this ZwaveDevice zwaveDevice, ProjectModel project)
        {
            if (zwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(zwaveDevice));
            }

            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            if (zwaveDevice.ParseObject is null)
            {
                return;
            }

            zwaveDevice.ParseObject[AppConstants.GATEWAYDEVICEDATABASE] = project.SelectedGateway.ParseObject;
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
            zwaveDevice.ParseObject[AppConstants.SCENE] = zwaveDevice.Scene;
            zwaveDevice.ParseObject[AppConstants.SELECTEDINDEXSCENE] = zwaveDevice.SelectedIndexScene;
        }

        public static ZwaveDevice ParseObjectToZwaveDevice(this ParseObject parseObject)
        {
            if (parseObject is null)
            {
                throw new ArgumentNullException(nameof(parseObject));
            }

            using ZwaveDevice zwaveDevice = new()
            {
                ParseObject = parseObject
            };

            if (parseObject.ContainsKey(AppConstants.NAMEDEVICEDATABASE))
            {
                zwaveDevice.Name = parseObject.Get<string>(AppConstants.NAMEDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.TYPE))
            {
                zwaveDevice.ZWaveDeviceType = Enum.TryParse(parseObject.Get<string>(AppConstants.TYPE), ignoreCase: true, out ZWaveDeviceType zwaveDeviceType)
                                            ? zwaveDeviceType
                                            : ZWaveDeviceType.NA;
            }

            if (parseObject.ContainsKey(AppConstants.SELECTEDINDEXTABSCHEDULE))
            {
                zwaveDevice.SelectedIndexTabControl = parseObject.Get<int>(AppConstants.SELECTEDINDEXTABSCHEDULE);
            }

            if (parseObject.ContainsKey(AppConstants.SELECTEDINDEXSCENE))
            {
                zwaveDevice.SelectedIndexScene = parseObject.Get<int?>(AppConstants.SELECTEDINDEXSCENE);
            }

            if (parseObject.ContainsKey(AppConstants.SCENE))
            {
                zwaveDevice.Scene = parseObject.Get<int?>(AppConstants.SCENE);

                if (zwaveDevice.Scene is int lenght)
                {
                    for (int i = 0; i < lenght; i++)
                    {
                        zwaveDevice.Scenes.Add(new Scene { Number = i + 1 });
                    }

                    if (!zwaveDevice.Scenes.Any())
                    {
                        goto End;
                    }

                    if (zwaveDevice.SelectedIndexScene is not int index)
                    {
                        goto End;
                    }

                    if (index < 0)
                    {
                        goto End;
                    }

                    if (!(zwaveDevice.SelectedIndexScene > zwaveDevice.Scenes.Count() - 1))
                    {
                        zwaveDevice.Scenes.ElementAt(index).IsOn = true;
                    }
                    else
                    {
                        zwaveDevice.SelectedIndexScene = -1;
                    }
                }
            End:;
            }

            if (parseObject.ContainsKey(AppConstants.GATEWAYDEVICEDATABASE))
            {
                zwaveDevice.GatewayParseObject = parseObject.Get<ParseObject>(AppConstants.GATEWAYDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.ASSOCIATIONDATADEVICEDATABASE))
            {
                zwaveDevice.AssociationsSerialize = parseObject.Get<string>(AppConstants.ASSOCIATIONDATADEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.NUMBEROFASSOCIATIONGROUPSDEVICEDATABASE))
            {
                zwaveDevice.NumberOfAssociationGroups = parseObject.Get<int>(AppConstants.NUMBEROFASSOCIATIONGROUPSDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.BASICDEVICECLASSDEVICEDATABASE))
            {
                zwaveDevice.BasicDeviceClass = parseObject.Get<int>(AppConstants.BASICDEVICECLASSDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.MODULEIDDEVICEDATABASE))
            {
                zwaveDevice.ModuleId = parseObject.Get<int>(AppConstants.MODULEIDDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.DEFAULTNAMEDEVICEDATABASE))
            {
                zwaveDevice.DefaultName = parseObject.Get<string>(AppConstants.DEFAULTNAMEDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEIMAGEDEVICEDATABASE))
            {
                zwaveDevice.ImageParseFile = parseObject.Get<ParseFile>(AppConstants.DEVICEIMAGEDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEENDPOINTSDEVICEDATABASE))
            {
                if (parseObject.Get<object>(AppConstants.DEVICEENDPOINTSDEVICEDATABASE) is List<object> list)
                {
                    if (list.Count != 0)
                    {
                        zwaveDevice.Endpoints = JsonConvert.DeserializeObject<IList<Endpoint>>(JsonConvert.SerializeObject(list));

                        for (int i = 0; i < zwaveDevice.Endpoints.Count; i++)
                        {
                            zwaveDevice.Endpoints.ElementAt(i).Channel = i + 1;
                        }
                    }
                }
                else if (parseObject.Get<object>(AppConstants.DEVICEENDPOINTSDEVICEDATABASE) is string response)
                {
                    zwaveDevice.Endpoints = JsonConvert.DeserializeObject<List<Endpoint>>(response);

                    if (zwaveDevice.Endpoints != null)
                    {
                        for (int i = 0; i < zwaveDevice.Endpoints.Count; i++)
                        {
                            zwaveDevice.Endpoints.ElementAt(i).Channel = i + 1;
                        }
                    }
                }
            }

            if (parseObject.ContainsKey(AppConstants.GENERICDEVICECLASSDEVICEDATABASE))
            {
                zwaveDevice.GenericDeviceClass = parseObject.Get<int>(AppConstants.GENERICDEVICECLASSDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.FIRMWAREVERSIONDEVICEDATABASE))
            {
                zwaveDevice.FirmwareVersion = parseObject.Get<int>(AppConstants.FIRMWAREVERSIONDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.CRYPTOGRAPHYDEVICEDATABASE))
            {
                if (parseObject.Get<object>(AppConstants.CRYPTOGRAPHYDEVICEDATABASE) is List<object> list)
                {
                    if (list.Count != 0)
                    {
                        zwaveDevice.Cryptographys = JsonConvert.DeserializeObject<IList<string>>(JsonConvert.SerializeObject(list));
                    }
                }
            }

            if (parseObject.ContainsKey(AppConstants.ASSOCIATIONGROUPSDEVICEDATABASE))
            {
                //todo verificar em todos os dispositivos
                if (parseObject.Get<object>(AppConstants.ASSOCIATIONGROUPSDEVICEDATABASE) is string)
                {
                    zwaveDevice.AssociationGroups = JsonConvert.DeserializeObject<IList<AssociationGroup>>(parseObject.Get<object>(AppConstants.ASSOCIATIONGROUPSDEVICEDATABASE).ToString());
                }
            }

            if (parseObject.ContainsKey(AppConstants.COMMANDCLASSESDEVICEDATABASE))
            {
                //todo verificar em todos os dispositivos
                if (parseObject.Get<object>(AppConstants.COMMANDCLASSESDEVICEDATABASE) is string)
                {
                    zwaveDevice.CommandClasses = JsonConvert.DeserializeObject<IList<CommandClass>>(parseObject.Get<object>(AppConstants.COMMANDCLASSESDEVICEDATABASE).ToString());
                }
                if (parseObject.Get<object>(AppConstants.COMMANDCLASSESDEVICEDATABASE) is List<object> list)
                {
                    if (list.Count != 0)
                    {
                        zwaveDevice.CommandClasses = JsonConvert.DeserializeObject<IList<CommandClass>>(JsonConvert.SerializeObject(list));
                    }
                }
            }

            if (parseObject.ContainsKey(AppConstants.MANUFACTURERKEYDEVICEDATABASE))
            {
                zwaveDevice.ManufacturerKey = parseObject.Get<int>(AppConstants.MANUFACTURERKEYDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.SPECIFICDEVICECLASSDEVICEDATABASE))
            {
                zwaveDevice.SpecificDeviceClass = parseObject.Get<int>(AppConstants.SPECIFICDEVICECLASSDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.PRODUCTKEYDEVICEDATABASE))
            {
                zwaveDevice.ProductKey = parseObject.Get<int>(AppConstants.PRODUCTKEYDEVICEDATABASE);
            }

            return zwaveDevice;
        }

        #endregion ZwaveDevice

        #region Ambience

        public static AmbienceModel ParseObjectToAmbience(this ParseObject parseObject)
        {
            if (parseObject is null)
            {
                throw new ArgumentNullException(nameof(parseObject));
            }

            using AmbienceModel ambienceModel = new()
            {
                ParseObject = parseObject
            };

            if (parseObject.ContainsKey(AppConstants.PROJECTFILEDFOR))
            {
                if (ambienceModel.FiledFor.Any())
                {
                    ambienceModel.FiledFor.Clear();
                }

                foreach (object objectId in parseObject.Get<List<object>>(AppConstants.PROJECTFILEDFOR))
                {
                    ambienceModel.FiledFor.Add(objectId.ToString());
                }
            }

            if (parseObject.ContainsKey(AppConstants.AMBIENCEBLACKLISTUSERS))
            {
                foreach (object objectId in parseObject.Get<ICollection<object>>(AppConstants.AMBIENCEBLACKLISTUSERS))
                {
                    ambienceModel.Blacklist.Add((string)objectId);
                }

                if (ambienceModel.Blacklist.Any())
                {
                    ambienceModel.IsCurrentUserOnBlacklist = ambienceModel.Blacklist.Contains(ParseUser.CurrentUser.ObjectId);
                }
            }

            if (parseObject.ContainsKey(AppConstants.AMBIENCEDELETEDBY))
            {
                ambienceModel.DeletedBy = parseObject.Get<ParseUser>(AppConstants.AMBIENCEDELETEDBY);
            }

            if (parseObject.ContainsKey(AppConstants.AMBIENCEARCHIVEDBY))
            {
                ambienceModel.ArchivedBy = parseObject.Get<ParseUser>(AppConstants.AMBIENCEARCHIVEDBY);
            }

            if (parseObject.ContainsKey(AppConstants.AMBIENCENAME))
            {
                ambienceModel.Name = parseObject.Get<string>(AppConstants.AMBIENCENAME);
            }

            if (parseObject.ContainsKey(AppConstants.AMBIENCEISDELETED))
            {
                ambienceModel.IsDeleted = parseObject.Get<bool>(AppConstants.AMBIENCEISDELETED);
            }

            if (parseObject.ContainsKey(AppConstants.AMBIENCEISENABLEBLACKLIST))
            {
                ambienceModel.IsEnableBlackList = parseObject.Get<bool>(AppConstants.AMBIENCEISENABLEBLACKLIST);
            }

            if (parseObject.ContainsKey(AppConstants.AMBIENCEFILEIMAGE))
            {
                ambienceModel.ImageParseFile = parseObject.Get<ParseFile>(AppConstants.AMBIENCEFILEIMAGE);
            }

            if (parseObject.ContainsKey(AppConstants.AMBIENCEFILEURL))
            {
                ambienceModel.ImageUrl = parseObject.Get<string>(AppConstants.AMBIENCEFILEURL);

                if (ambienceModel.ImageUrl != null)
                {
                    BitmapImage bitmapImage = new();

                    bitmapImage.BeginInit();

                    bitmapImage.UriSource = new Uri(ambienceModel.ImageUrl);

                    bitmapImage.EndInit();

                    ambienceModel.ImageSource = bitmapImage;
                }
            }

            if (parseObject.ContainsKey(AppConstants.AMBIENCEPRIMARYCOLOR))
            {
                ambienceModel.PrimaryColor = parseObject.Get<string>(AppConstants.AMBIENCEPRIMARYCOLOR);
                ambienceModel.ConvertColors(ambienceModel.PrimaryColor);
            }

            return ambienceModel;
        }

        public static void AmbienceToParseObject(this ProjectModel project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            project.SelectedAmbienceModel.ParseObject[AppConstants.AMBIENCENAME] = project.SelectedAmbienceModel.Name;

            project.SelectedAmbienceModel.ParseObject[AppConstants.AMBIENCEPRIMARYCOLOR] = project.SelectedAmbienceModel.PrimaryColor;

            project.SelectedAmbienceModel.ParseObject[AppConstants.AMBIENCEFILEURL] = project.SelectedAmbienceModel.ImageUrl;

            ParseRelation<ParseObject> relationGateway = project.SelectedAmbienceModel.ParseObject.GetRelation<ParseObject>(AppConstants.GATEWAYSAMBIENCE);

            if (project.SelectedAmbienceModel.ParseObjectGatewayToRemove is ParseObject toRemoveGateway)
            {
                relationGateway.Remove(toRemoveGateway);
                project.SelectedAmbienceModel.ParseObjectGatewayToRemove = null;
            }

            foreach (ParseObject parseObject in project.SelectedAmbienceModel.Devices.Select(x => x.ParseObject))
            {
                relationGateway.Add(parseObject);
            }

            ParseRelation<ParseObject> relationZwaveDevices = project.SelectedAmbienceModel.ParseObject.GetRelation<ParseObject>("zwaveDevices");

            if (project.SelectedAmbienceModel.ParseObjectZwaveDeviceToRemove is ParseObject toRemoveZwaveDevice)
            {
                relationZwaveDevices.Remove(toRemoveZwaveDevice);
            }

            foreach (ParseObject parseObject in project.SelectedAmbienceModel.ZwaveDevices.Select(x => x.ParseObject))
            {
                relationZwaveDevices.Add(parseObject);
            }
        }

        #endregion Ambience

        public static VoiceAssistantCommandModel ParseObjectToRemoteAccessStandaloneCommand(this ParseObject parseObject)
        {
            if (parseObject is null)
            {
                throw new ArgumentNullException(nameof(parseObject));
            }
            using VoiceAssistantCommandModel voiceAssistant = new()
            {
                ParseObject = parseObject
            };

            if (parseObject.ContainsKey("memoryId"))
            {
                voiceAssistant.MemoryId = parseObject.Get<int>("memoryId");
            }
            if (parseObject.ContainsKey("name"))
            {
                voiceAssistant.Name = parseObject.Get<string>("name");
            }
            if (parseObject.ContainsKey("commandType"))
            {
                if (Enum.TryParse(parseObject.Get<string>("commandType"), out CommandTypeVoiceAssistant commandType))
                {
                    voiceAssistant.CommandTypeVoiceAssistant = commandType;
                    voiceAssistant.Type = commandType.ToString();
                }
            }
            return voiceAssistant;
        }

        public static async Task RemoteAccessStandaloneToParseObject(this ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (selectedProject.SelectedGateway.RemoteAccessStandaloneModel.ParseObject is null)
            {
                selectedProject.SelectedGateway.RemoteAccessStandaloneModel.ParseObject = new ParseObject(AppConstants.REMOTEACCESSSTANDALONECLASSNAME);
            }

            await selectedProject.VoiceAssistantCommands();

            selectedProject.SelectedGateway.RemoteAccessStandaloneModel.ParseObject[AppConstants.REMOTEACCESSSTANDALONEUID] = selectedProject.SelectedGateway.UID;

            ParseRelation<ParseObject> relations = selectedProject.SelectedGateway.RemoteAccessStandaloneModel.ParseObject.GetRelation<ParseObject>("commands");

            foreach (ParseObject command in selectedProject.SelectedGateway.RemoteAccessStandaloneModel.CommandsCloud.Select(x => x.ParseObject))
            {
                relations.Add(command);
            }
        }

        public static async Task VoiceAssistantCommands(this ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            foreach (VoiceAssistantCommandModel command in selectedProject.SelectedGateway.RemoteAccessStandaloneModel.CommandsCloud)
            {
                command.ParseObject ??= new ParseObject(AppConstants.REMOTEACCESSSTANDALONECOMMAND);
                command.ParseObject["memoryId"] = command.MemoryId;
                command.ParseObject["name"] = command.Name;
                command.ParseObject["commandType"] = command.CommandTypeVoiceAssistant.ToString();

                await command.ParseObject.SaveAsync();
            }
        }

        public static LicenseModel ParseObjectToLicense(this ParseObject parseObject)
        {
            if (parseObject is null)
            {
                throw new ArgumentNullException(nameof(parseObject));
            }

            using LicenseModel licenseModel = new()
            {
                ParseObject = parseObject
            };

            if (parseObject.ContainsKey(AppConstants.LICENSEAPPUSERS))
            {
                ICollection<object> arrayList = (ICollection<object>)parseObject.Get<object>(AppConstants.LICENSEAPPUSERS);

                licenseModel.AppUsers ??= new List<string>();

                foreach (object attribute in arrayList)
                {
                    licenseModel.AppUsers.Add(attribute as string);
                }
            }

            if (parseObject.ContainsKey(AppConstants.LICENSEUSERPROJECTS))
            {
                ICollection<object> arrayList = (ICollection<object>)parseObject.Get<object>(AppConstants.LICENSEUSERPROJECTS);

                licenseModel.UserProjects ??= new List<string>();

                foreach (object attribute in arrayList)
                {
                    licenseModel.UserProjects.Add(attribute as string);
                }
            }

            if (parseObject.ContainsKey(AppConstants.LICENSEKEY))
            {
                licenseModel.LicenseKey = parseObject.Get<string>(AppConstants.LICENSEKEY);
            }

            if (parseObject.ContainsKey(AppConstants.LICENSETYPE))
            {
                licenseModel.LicenseType = GetLicenseType(parseObject.Get<string>(AppConstants.LICENSETYPE));
            }

            if (parseObject.ContainsKey(AppConstants.LICENSEEXPIRATIONDATE))
            {
                licenseModel.ExpirationDate = parseObject.Get<DateTime?>(AppConstants.LICENSEEXPIRATIONDATE);
            }

            if (parseObject.ContainsKey(AppConstants.LICENSESESSIONSPERUSER))
            {
                licenseModel.SessionsPerUser = parseObject.Get<int>(AppConstants.LICENSESESSIONSPERUSER);
            }

            if (parseObject.ContainsKey(AppConstants.LICENSEUSERLIMIT))
            {
                licenseModel.UserLimit = parseObject.Get<int>(AppConstants.LICENSEUSERLIMIT);
            }

            if (parseObject.ContainsKey(AppConstants.LICENSEOWNERUSER))
            {
                using ParseUserCustom parseUser = new()
                {
                    ParseUser = parseObject.Get<ParseUser>(AppConstants.LICENSEOWNERUSER)
                };
                licenseModel.Owner = parseUser;
                licenseModel.IsWriteAccess = ParseUser.CurrentUser.ObjectId == parseUser.ParseUser.ObjectId;
            }

            if (parseObject.ContainsKey(AppConstants.LICENSECREATEDBY))
            {
                using ParseUserCustom parseUser = new()
                {
                    ParseUser = parseObject.Get<ParseUser>(AppConstants.LICENSECREATEDBY)
                };
                licenseModel.CreateBy = parseUser;
            }

            if (parseObject.ContainsKey(AppConstants.LICENSEPROJECT))
            {
                licenseModel.ProjectParseObject = parseObject.Get<ParseObject>(AppConstants.LICENSEPROJECT);
            }

            return licenseModel;
        }

        private static LicenseTypeEnum GetLicenseType(string licenseType)
        {
            return licenseType.Equals(AppConstants.LICENSETYPDEVELOPMENT, StringComparison.OrdinalIgnoreCase)
                ? LicenseTypeEnum.Development
                : LicenseTypeEnum.Commercial;
        }

        public static IfThenModel ParseObjectToIfThenModel(this ParseObject parseObject)
        {
            if (parseObject is null)
            {
                throw new ArgumentNullException(nameof(parseObject));
            }

            using IfThenModel selectedIfThen = new()
            {
                ParseObject = parseObject
            };

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.NAME))
            {
                selectedIfThen.Name = selectedIfThen.ParseObject.Get<string>(UtilIfThen.NAME) ?? Domain.Properties.Resources.Unnamed;
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.ISENABLED))
            {
                selectedIfThen.IsEnabled = selectedIfThen.ParseObject.Get<bool>(UtilIfThen.ISENABLED);
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.IFTHENTYPE))
            {
                selectedIfThen.IfthenType = (IfthenType)selectedIfThen.ParseObject.Get<int>(UtilIfThen.IFTHENTYPE);
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.RULEIDS))
            {
                IList<object> ruleIds = selectedIfThen.ParseObject.Get<IList<object>>(UtilIfThen.RULEIDS);

                foreach (object ruleId in ruleIds)
                {
                    if (int.TryParse(ruleId.ToString(), out int id))
                    {
                        selectedIfThen.RuleIds.Add(id);
                    }
                }
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.MACROIDTHEN))
            {
                selectedIfThen.MacroIdThen = selectedIfThen.ParseObject.Get<int>(UtilIfThen.MACROIDTHEN);
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.MACROIDELSE))
            {
                selectedIfThen.MacroIdElse = selectedIfThen.ParseObject.Get<int>(UtilIfThen.MACROIDELSE);
            }
            return selectedIfThen;
        }

        public static ZwaveDevice ParseObjectToIfThenObject(this ParseObject parseObject)
        {
            if (parseObject is null)
            {
                throw new ArgumentNullException(nameof(parseObject));
            }

            using ZwaveDevice zwaveDevice = new()
            {
                ParseObject = parseObject
            };

            if (zwaveDevice.ParseObject.ContainsKey(AppConstants.SELECTEDINDEXTHERMOSTATMODE))
            {
                zwaveDevice.SelectedIndexThermostatMode = parseObject.Get<int>(AppConstants.SELECTEDINDEXTHERMOSTATMODE);
            }

            if (zwaveDevice.ParseObject.ContainsKey(AppConstants.SELECTEDINDEXTHERMOSTATFAN))
            {
                zwaveDevice.SelectedIndexThermostatFan = parseObject.Get<int>(AppConstants.SELECTEDINDEXTHERMOSTATFAN);
            }

            if (zwaveDevice.ParseObject.ContainsKey(AppConstants.SELECTEDINDEXTHERMOSTATFUNCTION))
            {
                zwaveDevice.SelectedIndexThermostatFunction = parseObject.Get<int>(AppConstants.SELECTEDINDEXTHERMOSTATFUNCTION);
            }

            if (zwaveDevice.ParseObject.ContainsKey(AppConstants.ROOMTEMPERATURE))
            {
                zwaveDevice.RoomTemperature = parseObject.Get<int>(AppConstants.ROOMTEMPERATURE);
            }

            if (zwaveDevice.ParseObject.ContainsKey(AppConstants.STATEINDEX))
            {
                zwaveDevice.StateIndex = parseObject.Get<int>(AppConstants.STATEINDEX);
            }

            if (zwaveDevice.ParseObject.ContainsKey(AppConstants.SELECTEDINDEXTABSCHEDULE))
            {
                zwaveDevice.SelectedIndexTabSchedule = parseObject.Get<int>(AppConstants.SELECTEDINDEXTABSCHEDULE);
            }

            if (zwaveDevice.ParseObject.ContainsKey(AppConstants.DEVICEIMAGEDEVICEDATABASE))
            {
                zwaveDevice.ImageParseFile = parseObject.Get<ParseFile>(AppConstants.DEVICEIMAGEDEVICEDATABASE);
            }

            if (zwaveDevice.ParseObject.ContainsKey(UtilIfThen.GATEWAYNAME))
            {
                zwaveDevice.GatewayName = parseObject.Get<string>(UtilIfThen.GATEWAYNAME);
            }

            if (zwaveDevice.ParseObject.ContainsKey(UtilIfThen.GATEWAYFUNCTIONMEMORYID))
            {
                zwaveDevice.GatewayFunctionMemoryId = parseObject.Get<int>(UtilIfThen.GATEWAYFUNCTIONMEMORYID);
            }

            if (zwaveDevice.ParseObject.ContainsKey(UtilIfThen.GATEWAYFUNCTIONUID))
            {
                zwaveDevice.GatewayFunctionUID = parseObject.Get<string>(UtilIfThen.GATEWAYFUNCTIONUID);
            }

            if (zwaveDevice.ParseObject.ContainsKey(UtilIfThen.VALUESCHEDULE))
            {
                zwaveDevice.ValueSchedule = parseObject.Get<long>(UtilIfThen.VALUESCHEDULE);
            }

            if (zwaveDevice.ParseObject.ContainsKey(UtilIfThen.TIMEPICKERVALUE))
            {
                zwaveDevice.TimePickerValue = parseObject.Get<DateTime>(UtilIfThen.TIMEPICKERVALUE);
            }

            if (zwaveDevice.ParseObject.ContainsKey(UtilIfThen.SELECTEDINDEXOPERATORSTYPE))
            {
                zwaveDevice.SelectedIndexOperatorsType = parseObject.Get<int>(UtilIfThen.SELECTEDINDEXOPERATORSTYPE);
            }

            if (zwaveDevice.ParseObject.ContainsKey(UtilIfThen.SELECTEDINDEXDAYSOFWEEK))
            {
                zwaveDevice.SelectedIndexDaysOfWeek = parseObject.Get<int>(UtilIfThen.SELECTEDINDEXDAYSOFWEEK);
            }

            if (zwaveDevice.ParseObject.ContainsKey(UtilIfThen.SELECTEDINDEXDATETYPE))
            {
                zwaveDevice.SelectedIndexDateType = parseObject.Get<int>(UtilIfThen.SELECTEDINDEXDATETYPE);
            }

            if (parseObject.ContainsKey(UtilIfThen.SELECTEDDATETYPE))
            {
                zwaveDevice.SelectedDateType = (DateTypeEnum)parseObject.Get<int>(UtilIfThen.SELECTEDDATETYPE);
            }

            if (parseObject.ContainsKey(UtilIfThen.SELECTEDDAYSOFWEEK))
            {
                zwaveDevice.SelectedDaysOfWeek = (DaysOfWeek)parseObject.Get<int>(UtilIfThen.SELECTEDDAYSOFWEEK);
            }

            if (parseObject.ContainsKey(UtilIfThen.SELECTEDOPERATORSTYPESCHEDULE))
            {
                zwaveDevice.SelectedOperatorsTypeSchedule = (OperatorsTypeSchedule)parseObject.Get<int>(UtilIfThen.SELECTEDOPERATORSTYPESCHEDULE);
            }

            if (parseObject.ContainsKey(UtilIfThen.ACTIONSRTSSOMFY))
            {
                zwaveDevice.ActionsRTSSomfy = (ActionsRTSSomfy)parseObject.Get<int>(UtilIfThen.ACTIONSRTSSOMFY);
            }

            if (parseObject.ContainsKey(UtilIfThen.SELECTEDINDEXOPERATORTYPE))
            {
                zwaveDevice.SelectedIndexOperatorType = parseObject.Get<int>(UtilIfThen.SELECTEDINDEXOPERATORTYPE);
            }

            if (parseObject.ContainsKey(UtilIfThen.SELECTEDINDEXENDPOINT))
            {
                zwaveDevice.SelectedIndexEndpoint = parseObject.Get<int>(UtilIfThen.SELECTEDINDEXENDPOINT);
            }

            if (parseObject.ContainsKey(UtilIfThen.SELECTEDINDEXLOGICGATEIFTHEN))
            {
                zwaveDevice.SelectedIndexLogicGateIfThen = parseObject.Get<int>(UtilIfThen.SELECTEDINDEXLOGICGATEIFTHEN);
            }

            if (parseObject.ContainsKey(UtilIfThen.INDEX))
            {
                zwaveDevice.Index = parseObject.Get<int>(UtilIfThen.INDEX);
            }

            if (parseObject.ContainsKey(UtilIfThen.NAME))
            {
                zwaveDevice.Name = parseObject.Get<string>(UtilIfThen.NAME);
            }

            if (parseObject.ContainsKey(UtilIfThen.GATEWAYFUNCTIONNAME))
            {
                zwaveDevice.GatewayFunctionName = parseObject.Get<string>(UtilIfThen.GATEWAYFUNCTIONNAME);
            }

            if (parseObject.ContainsKey(UtilIfThen.IFTHENTYPE))
            {
                zwaveDevice.IfthenType = (IfthenType)parseObject.Get<int>(UtilIfThen.IFTHENTYPE);
            }

            if (parseObject.ContainsKey(UtilIfThen.SELECTEDTABINDEXSENSORTYPE))
            {
                zwaveDevice.SelectedTabIndexSensorType = parseObject.Get<int>(UtilIfThen.SELECTEDTABINDEXSENSORTYPE);
            }

            if (parseObject.ContainsKey(UtilIfThen.MULTILEVEL))
            {
                zwaveDevice.MultiLevel = parseObject.Get<long>(UtilIfThen.MULTILEVEL);
            }

            if (parseObject.ContainsKey(UtilIfThen.ISON))
            {
                zwaveDevice.IsOn = parseObject.Get<bool>(UtilIfThen.ISON);
            }

            if (parseObject.ContainsKey(UtilIfThen.DELAY))
            {
                zwaveDevice.DelayIfThen = parseObject.Get<int?>(UtilIfThen.DELAY);
            }

            if (parseObject.ContainsKey(AppConstants.GATEWAYDEVICEDATABASE))
            {
                zwaveDevice.GatewayParseObject = parseObject.Get<ParseObject>(AppConstants.GATEWAYDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.ASSOCIATIONDATADEVICEDATABASE))
            {
                zwaveDevice.AssociationsSerialize = parseObject.Get<string>(AppConstants.ASSOCIATIONDATADEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.NUMBEROFASSOCIATIONGROUPSDEVICEDATABASE))
            {
                zwaveDevice.NumberOfAssociationGroups = parseObject.Get<int>(AppConstants.NUMBEROFASSOCIATIONGROUPSDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.BASICDEVICECLASSDEVICEDATABASE))
            {
                zwaveDevice.BasicDeviceClass = parseObject.Get<int>(AppConstants.BASICDEVICECLASSDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.MODULEIDDEVICEDATABASE))
            {
                zwaveDevice.ModuleId = parseObject.Get<int>(AppConstants.MODULEIDDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.DEFAULTNAMEDEVICEDATABASE))
            {
                zwaveDevice.DefaultName = parseObject.Get<string>(AppConstants.DEFAULTNAMEDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.NAMEDEVICEDATABASE))
            {
                zwaveDevice.Name = parseObject.Get<string>(AppConstants.NAMEDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEIMAGEDEVICEDATABASE))
            {
                zwaveDevice.ImageParseFile = parseObject.Get<ParseFile>(AppConstants.DEVICEIMAGEDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEENDPOINTSDEVICEDATABASE))
            {
                zwaveDevice.Endpoints = JsonConvert.DeserializeObject<List<Endpoint>>(parseObject.Get<object>(AppConstants.DEVICEENDPOINTSDEVICEDATABASE).ToString());

                if (zwaveDevice.Endpoints != null)
                {
                    for (int i = 0; i < zwaveDevice.Endpoints.Count; i++)
                    {
                        zwaveDevice.Endpoints.ElementAt(i).Channel = i + 1;
                    }
                }
            }

            if (parseObject.ContainsKey(AppConstants.DAYSOFWEEKLIST))
            {
                try
                {
                    zwaveDevice.DaysOfWeekList = JsonConvert.DeserializeObject<ObservableCollection<DaysOfWeekModel>>(parseObject.Get<object>(AppConstants.DAYSOFWEEKLIST).ToString());
                }
                catch (Exception)
                {
                }
            }

            if (parseObject.ContainsKey(AppConstants.GENERICDEVICECLASSDEVICEDATABASE))
            {
                zwaveDevice.GenericDeviceClass = parseObject.Get<int>(AppConstants.GENERICDEVICECLASSDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.FIRMWAREVERSIONDEVICEDATABASE))
            {
                zwaveDevice.FirmwareVersion = parseObject.Get<int>(AppConstants.FIRMWAREVERSIONDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.CRYPTOGRAPHYDEVICEDATABASE))
            {
                zwaveDevice.Cryptographys = JsonConvert.DeserializeObject<List<string>>(parseObject.Get<object>(AppConstants.CRYPTOGRAPHYDEVICEDATABASE).ToString());
            }

            if (parseObject.ContainsKey(AppConstants.ASSOCIATIONGROUPSDEVICEDATABASE))
            {
                zwaveDevice.AssociationGroups = JsonConvert.DeserializeObject<IList<AssociationGroup>>(parseObject.Get<object>(AppConstants.ASSOCIATIONGROUPSDEVICEDATABASE).ToString());
            }

            if (parseObject.ContainsKey(AppConstants.COMMANDCLASSESDEVICEDATABASE))
            {
                zwaveDevice.CommandClasses = JsonConvert.DeserializeObject<IList<CommandClass>>(parseObject.Get<object>(AppConstants.COMMANDCLASSESDEVICEDATABASE).ToString());
            }

            if (parseObject.ContainsKey(AppConstants.MANUFACTURERKEYDEVICEDATABASE))
            {
                zwaveDevice.ManufacturerKey = parseObject.Get<int>(AppConstants.MANUFACTURERKEYDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.SPECIFICDEVICECLASSDEVICEDATABASE))
            {
                zwaveDevice.SpecificDeviceClass = parseObject.Get<int>(AppConstants.SPECIFICDEVICECLASSDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.PRODUCTKEYDEVICEDATABASE))
            {
                zwaveDevice.ProductKey = parseObject.Get<int>(AppConstants.PRODUCTKEYDEVICEDATABASE);
            }

            return zwaveDevice;
        }
    }
}