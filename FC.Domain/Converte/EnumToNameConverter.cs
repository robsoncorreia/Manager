using FC.Domain.Model;
using FC.Domain.Model.Device;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FC.Domain.Converte
{
    public class EnumToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return null;
            }
            if (value is TypeIPEnum type)
            {
                return type switch
                {
                    TypeIPEnum.DHCP => Properties.Resources.DHCP,
                    TypeIPEnum.Static => Properties.Resources.Static,
                    _ => null,
                };
            }
            else if (value is AutoReportConditionTimeInterval autoReportConditionTime)
            {
                return autoReportConditionTime switch
                {
                    AutoReportConditionTimeInterval.DisableAutoReport => Properties.Resources.Disabled,
                    AutoReportConditionTimeInterval._1_H => Properties.Resources._1_H,
                    AutoReportConditionTimeInterval._2_H => Properties.Resources._2_H,
                    AutoReportConditionTimeInterval._3_H => Properties.Resources._3_H,
                    AutoReportConditionTimeInterval._4_H => Properties.Resources._4_H,
                    AutoReportConditionTimeInterval._5_H => Properties.Resources._5_H,
                    AutoReportConditionTimeInterval._6_H => Properties.Resources._6_H,
                    AutoReportConditionTimeInterval._7_H => Properties.Resources._7_H,
                    AutoReportConditionTimeInterval._8_H => Properties.Resources._8_H,
                    _ => null,
                };
            }
            else if (value is AutoReportCondition autoReportCondition)
            {
                return autoReportCondition switch
                {
                    AutoReportCondition.DisableAutoReport => Properties.Resources.Disabled,
                    AutoReportCondition._1_F_0_5_C => Properties.Resources._1_F_0_5_C,
                    AutoReportCondition._2_F_1_C => Properties.Resources._2_F_1_C,
                    AutoReportCondition._3_F_1_5_C => Properties.Resources._3_F_1_5_C,
                    AutoReportCondition._4_F_2_C => Properties.Resources._4_F_2_C,
                    AutoReportCondition._5_F_2_5_C => Properties.Resources._5_F_2_5_C,
                    AutoReportCondition._6_F_3_C => Properties.Resources._6_F_3_C,
                    AutoReportCondition._7_F_3_5_C => Properties.Resources._7_F_3_5_C,
                    AutoReportCondition._8_F_4_C => Properties.Resources._8_F_4_C,
                    _ => null,
                };
            }
            else if (value is TemperatureOffsetValueEnum temperatureOffsetValue)
            {
                return temperatureOffsetValue switch
                {
                    TemperatureOffsetValueEnum._0_C_Default => Properties.Resources._0_C__Default_,
                    TemperatureOffsetValueEnum._1_C => Properties.Resources._1_C,
                    TemperatureOffsetValueEnum._2_C => Properties.Resources._2_C,
                    TemperatureOffsetValueEnum._3_C => Properties.Resources._3_C,
                    TemperatureOffsetValueEnum._4_C => Properties.Resources._4_C,
                    TemperatureOffsetValueEnum._5_C => Properties.Resources._5_C,
                    TemperatureOffsetValueEnum._1_C_Negative => Properties.Resources._1_C_Negative,
                    TemperatureOffsetValueEnum._2_C_Negative => Properties.Resources._2_C_Negative,
                    TemperatureOffsetValueEnum._3_C_Negative => Properties.Resources._3_C_Negative,
                    TemperatureOffsetValueEnum._4_C_Negative => Properties.Resources._4_C_Negative,
                    TemperatureOffsetValueEnum._5_C_Negative => Properties.Resources._5_C_Negative,
                    _ => null
                };
            }
            else if (value is DayPeriodEnum dayPeriodEnum)
            {
                return dayPeriodEnum switch
                {
                    DayPeriodEnum.Sunrise => Properties.Resources.Sunrise,
                    DayPeriodEnum.Sunset => Properties.Resources.Sunset,
                    _ => null,
                };
            }
            else if (value is ZWaveDeviceType deviceType)
            {
                switch (deviceType)
                {
                    case ZWaveDeviceType.NA:
                        return Properties.Resources.Others;

                    case ZWaveDeviceType.Gateway:
                        return Properties.Resources.Gateway;

                    case ZWaveDeviceType.Sensor:
                        return Properties.Resources.Sensor;

                    case ZWaveDeviceType.Panel:
                        return Properties.Resources.Panel;

                    case ZWaveDeviceType.MicroModule:
                        return Properties.Resources.Micro_Module;

                    case ZWaveDeviceType.Generic:
                        return Properties.Resources.Generic;

                    default:
                        break;
                }
            }
            else if (value is IfthenType ifthen)
            {
                return ifthen switch
                {
                    IfthenType.Device => Properties.Resources.Device,
                    IfthenType.IR => Properties.Resources.IR,
                    IfthenType.Radio433 => Properties.Resources.Radio_433MHz,
                    IfthenType.RTS => Properties.Resources.RTS,
                    IfthenType.Schedule => Properties.Resources.Schedule,
                    IfthenType.IPCommand => Properties.Resources.Ip_Command,
                    IfthenType.Relay => Properties.Resources.Relay,
                    IfthenType.Serial => Properties.Resources.Serial,
                    _ => null,
                };
            }
            else if (value is DateTypeEnum dateType)
            {
                return dateType switch
                {
                    DateTypeEnum.EveryAmountSeconds => Properties.Resources.Seconds,
                    DateTypeEnum.EveryAmountMinutes => Properties.Resources.Minutes,
                    DateTypeEnum.EveryAmountHours => Properties.Resources.Hours,
                    DateTypeEnum.EveryAmountDays => Properties.Resources.Days,
                    DateTypeEnum.EveryAmountMonths => Properties.Resources.Months,
                    DateTypeEnum.EveryAmountYears => Properties.Resources.Years,
                    DateTypeEnum.CompareClock => Properties.Resources.Compare_clock,
                    DateTypeEnum.CompareDayWeek => Properties.Resources.Compare_day_of_the_week,
                    _ => null,
                };
            }
            else if (value is RelayStateMode relayState)
            {
                return relayState switch
                {
                    RelayStateMode.Permanent => Properties.Resources.Permanent,
                    RelayStateMode.Pulse => Properties.Resources.Pulse,
                    _ => null,
                };
            }
            else if (value is ProtocolTypeEnum protocol)
            {
                return protocol switch
                {
                    ProtocolTypeEnum.UDP => Properties.Resources.UDP,
                    ProtocolTypeEnum.TCP => Properties.Resources.TCP,
                    _ => null,
                };
            }
            else if (value is APDHCP aPDHCP)
            {
                return aPDHCP switch
                {
                    APDHCP.Disabled => Properties.Resources.Disabled,
                    APDHCP.Enable => Properties.Resources.Enabled,
                    _ => null,
                };
            }
            else if (value is APStatusEnum aP)
            {
                return aP switch
                {
                    APStatusEnum.Disabled => Properties.Resources.Disabled,
                    APStatusEnum.Enable => Properties.Resources.Enabled,
                    _ => null,
                };
            }
            else if (value is WiFiStatusEnum statusEnum)
            {
                return statusEnum switch
                {
                    WiFiStatusEnum.Disabled => Properties.Resources.Disabled,
                    WiFiStatusEnum.Enable => Properties.Resources.Enabled,
                    _ => null,
                };
            }
            else if (value is OperatorTypeIfThen @operator)
            {
                return @operator switch
                {
                    OperatorTypeIfThen.LessThan => Properties.Resources.Less_Than,
                    OperatorTypeIfThen.LessThanOrEqual => Properties.Resources.Less_Than_Or_Equal,
                    OperatorTypeIfThen.Equal => Properties.Resources.Equal,
                    OperatorTypeIfThen.NotEqual => Properties.Resources.Not_equal,
                    OperatorTypeIfThen.GreaterThanOrEqual => Properties.Resources.Greater_Than_Or_Equal,
                    OperatorTypeIfThen.GreaterThan => Properties.Resources.Greater_Than,
                    _ => null,
                };
            }
            else if (value is LogicGateIfThen logicGateIfThen)
            {
                return logicGateIfThen switch
                {
                    LogicGateIfThen.Disabled => Properties.Resources.Disabled,
                    LogicGateIfThen.And => Properties.Resources.And,
                    LogicGateIfThen.Or => Properties.Resources.Or,
                    LogicGateIfThen.Xor => LogicGateIfThen.Xor,
                    LogicGateIfThen.Nand => LogicGateIfThen.Nand,
                    LogicGateIfThen.Nor => LogicGateIfThen.Nor,
                    LogicGateIfThen.Xnor => LogicGateIfThen.Xnor,
                    _ => null,
                };
            }
            else if (value is EndpointState endpointState)
            {
                return endpointState switch
                {
                    EndpointState.On => Properties.Resources.On,
                    EndpointState.Off => Properties.Resources.Off,
                    EndpointState.Toggle => Properties.Resources.Toggle,
                    _ => null,
                };
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "cu";
        }
    }
}