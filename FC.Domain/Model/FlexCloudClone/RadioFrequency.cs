namespace FC.Domain.Model.FlexCloudClone
{
    public enum Frequency
    {
        F_433_92,
        F_433_42,
        F_433,
        F_433_96
    }

    public class RadioFrequency
    {
        public Frequency Frequency { get; set; }
        public string Name { get; set; }
    }
}