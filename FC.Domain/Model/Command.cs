using FC.Domain._Base;
using System;

namespace FC.Domain.Model
{
    public enum ProtocolTypeEnum
    {
        UDP,
        TCP
    }

    public class CommandModel : ModelBase
    {
        public int Id { get; set; }

        public string IP { get; set; }

        public string ParseUserObjectId { get; set; }

        public int Port { get; set; }

        public ProtocolTypeEnum ProtocolType { get; set; }

        public ConnectionType ConnectionType { get; set; }

        public string Receive
        {
            get => _receive;
            set => _receive = value ?? Properties.Resources.No_reply;
        }

        public double ResponseTime { get; set; }

        public string Send
        {
            get => _send;
            set
            {
                _send = value;
                StartDateTime = DateTime.Now;
            }
        }

        public DateTime StartDateTime { get; set; }
        public string IPTarget { get; set; }
        public int Timeout { get; internal set; }

        public CommandModel(string send,
                            string ip,
                            int port,
                            ProtocolTypeEnum protocolType,
                            string parseUserObjectId)
        {
            Send = send;
            IP = ip;
            Port = port;
            ProtocolType = protocolType;
            ParseUserObjectId = parseUserObjectId;
        }

        public CommandModel()
        {
        }

        public CommandModel(string send,
                            string ip,
                            int port,
                            ConnectionType connectionType,
                            ProtocolTypeEnum protocolTypeEnum,
                            string objectId)
        {
            Send = send;
            IP = ip;
            Port = port;
            ConnectionType = connectionType;
            ProtocolType = protocolTypeEnum;
            ParseUserObjectId = objectId;
        }

        public void GetResponseTime()
        {
            TimeSpan diff = DateTime.Now - StartDateTime;
            ResponseTime = Math.Round(diff.TotalMilliseconds, 1, MidpointRounding.ToEven);
        }

        public override string ToString()
        {
            return $"IP: {IP}\nPort: {Port}\nSend: {Send}\nReceive: {Receive}";
        }

        public string _responseTime;
        private string _send;
        private string _receive = Properties.Resources.No_reply;
    }
}