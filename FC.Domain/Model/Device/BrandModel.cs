using System;

namespace FC.Domain.Model.Device
{
    public enum ManufacturerEnum
    {
        FlexAutomation
    }

    public class BrandModel
    {
        public int Id { get; set; }
        public Uri Image { get; set; }
        public ManufacturerEnum Manufacturer { get; set; }
        public string Name { get; set; }
    }
}