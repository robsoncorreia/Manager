using FC.Domain._Base;

namespace FC.Domain.Model.ZXT600
{
    public class ZXTIRLearningMappingModel : ModelBase
    {
        public int StorageLocation { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}