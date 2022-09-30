namespace FC.Domain.Service.Rede
{
    public static class UtilRede
    {
        public const string DISABLE = "00";
        public const string ENABLE = "01";

        //FEEDBACK
        public const string FEEDBACK_IP_PORTS_CALBLE = "@GCR#";

        public const string FEEDBACK_IP_PORTS_WIFI = "@WGCR#";

        /// <summary>
        /// É o caracterer finalizador do comando flexnet
        /// </summary>
        public const string FINISHER = "#";

        /// <summary>
        /// IP e DHCP da Ethernet
        /// </summary>
        public const string GET_DHCP_IPACTUAL_IPDEFINED_IPROUTER_MASK_ETHERNET = "@ENCG#";

        /// <summary>
        /// IP e DHCP da WiFi
        /// </summary>
        public const string GET_DHCP_IPACTUAL_IPDEFINED_IPROUTER_MASK_WIFI = "@WNCG#";

        /// <summary>
        /// Enable Wi-Fi Station
        /// </summary>
        public const string GET_ENABLE_STATION_WIFI = "@WFSG#";

        /// <summary>
        /// Mac Address da Ethernet
        /// </summary>
        public const string GET_MAC_ETHERNET = "@MADG#";

        /// <summary>
        /// Mac Address da Wi-Fi
        /// </summary>
        public const string GET_MAC_WIFI = "@WMAG#";

        /// <summary>
        /// Mac Address da Wi-Fi
        /// </summary>
        public const string GET_MANUFACTURERCODE_FWVERSION_PRODUCTCODE_WIFE = "@GBIG#";

        /// <summary>
        /// Password
        /// </summary>
        public const string GET_PASSWORD = "@WSHG#";

        //GET
        /// <summary>
        /// Portas da Ethernet
        /// </summary>
        public const string GET_POR_TCP_UDP_ETHERNET = "@EPNG#";

        /// <summary>
        /// Portas da WiFi
        /// </summary>
        public const string GET_PORT_TCP_UDP_WIFI = "@WPNG#";

        /// <summary>
        /// SSID
        /// </summary>
        public const string GET_SSID = "@WNMG#";

        public const string GET_UNIQUE_ID = "{\"command\":\"get_unique_id\", \"type\":0}";

        /// <summary>
        /// If Then Rule Set
        /// </summary>
        public const string IF_THEN_RULE_SET = "ITRS";

        //SET
        /// <summary>
        /// É o caracterer inicializador do comando flexnet
        /// </summary>
        public const string INITIALIZE = "@";

        public const string LOCALIP = "192.168.4.1";
        public const int PORT_DEFAULT_TCP = 6666;
        public const int PORT_DEFAULT_UDP = 9999;
        /*********
        COMMAND'S:
        **********/
        public const string REBOOT = "@RIP#";
        public const string SET_DHCP_IPACTUAL_IPDEFINED_IPROUTER_MASK_ETHERNET = "@ENCS";
        public const string SET_DHCP_IPACTUAL_IPDEFINED_IPROUTER_MASK_WIFI = "@WNCS";
        public const string SET_ENABLE_STATION_WIFI = "@WFSS";
        public const string SET_PASSWORD = "@WSHS";
        public const string SET_POR_TCP_UDP_ETHERNET = "@EPNS";
        public const string SET_PORT_TCP_UDP_WIFI = "@WPNS";
        public const string SET_SSID = "@WNMS";

        public const string INITIALIZER = "@";

        public const string ETHERNETPORTSNETWORKSET = "EPNS";

        public const string WIFIPORTSNETWORKSET = "WPNS";

        public const string NETWORKSTATUS = "@WGCR#";

        public const string GETZWAVEHOMEID = "{\"command\":\"getZwaveHomeId\",\"type\":0}";

        public const string PEDROGET = "{\"command\":\"pedroGet\",\"type\":0}";

        public const string GETWIFIAPSSID = "{\"command\":\"get_WIFI_ap_ssid\", \"type\":0}";

        public const string GETWIFIAPPASS = "{\"command\":\"get_WIFI_ap_pass\", \"type\":0}";

        public const string GETWIFIAPENABLE = "{\"command\":\"get_WIFI_ap_enable\", \"type\":0}";

        public const string GETWIFIAPDHCP = "{\"command\":\"get_WIFI_ap_dhcp\", \"type\":0}";

        public const string WIFINETWORKCONFIGURATIONGET = "{\"command\":\"WifiNetworkConfigurationGet\", \"type\":0}";

        public const string WIFIPORTNUMBERGET = "{\"command\":\"WifiPortNumberGet\", \"type\":0}";

        public const string GETWIFISTATIONSSID = "{\"command\":\"get_WIFI_station_ssid\", \"type\":0}";

        public const string GETWIFISTATIONPASS = "{\"command\":\"get_WIFI_station_pass\", \"type\":0}";

        public const string GETWIFISTATIONENABLE = "{\"command\":\"get_WIFI_station_enable\", \"type\":0}";

        public const string WIFIMACADDRESSGET = "{\"command\":\"WifiMacAddressGet\", \"type\":0}";

        public const string ETHNETWORKCONFIGGET = "{\"command\":\"ethNetworkConfigGet\", \"type\":0}";

        public const string ETHPORTNUMBERGET = "{\"command\":\"ethPortNumberGet\",\"type\":0}";

        public const string GETETHERNETMAC = "{\"command\":\"get_ethernet_mac\", \"type\":0 }";

        public const string GETZWAVEFREQUENCY = "{\"command\":\"getZwaveFrequency\",\"type\":0}";

        public const string GETUNIQUEID = "{\"command\":\"get_unique_id\",\"type\":0}";

        public const string CHECKPRIMARY = "{\"command\":\"checkPrimary\",\"type\":0}";

        public const int MILLISECONDSDELAYAWAITREBOOT = 15000;
    }
}