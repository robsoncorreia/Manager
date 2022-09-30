using FC.Domain._Base;

namespace FC.Domain.Model.Device
{
    public class CommonIP : ModelBase
    {
        public string Send { get; set; }
        public string Gateway { get; set; }
        public int ID { get; set; }
    }
}