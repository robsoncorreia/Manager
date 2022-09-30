using FC.Domain.Model.Device;
using FC.Domain.Model.Zwave;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FC.Domain.Service
{
    public interface IZwaveService
    {
        public IList<GenericClass> GenericClasses { get; set; }
        public IList<CommandClass> CommandClasses { get; set; }
        public IList<BasicDeviceClass> BasicDeviceClasses { get; set; }
        public IList<GenericDeviceClass> GenericDeviceClasses { get; set; }
        public IList<SpecificDeviceClass> SpecificDeviceClasses { get; set; }
    }

    public class ZwaveService : IZwaveService
    {
        public IList<SpecificDeviceClass> SpecificDeviceClasses { get; set; }
        public IList<GenericClass> GenericClasses { get; set; }
        public IList<CommandClass> CommandClasses { get; set; }
        public IList<GenericDeviceClass> GenericDeviceClasses { get; set; }
        public IList<BasicDeviceClass> BasicDeviceClasses { get; set; }

        public ZwaveService()
        {
            GenericClasses = JsonConvert.DeserializeObject<IList<GenericClass>>(Properties.Resources.genericType);
            CommandClasses = JsonConvert.DeserializeObject<IList<CommandClass>>(Properties.Resources.commandClasses);
            BasicDeviceClasses = JsonConvert.DeserializeObject<IList<BasicDeviceClass>>(Properties.Resources.basicDeviceClass);
            GenericDeviceClasses = JsonConvert.DeserializeObject<IList<GenericDeviceClass>>(Properties.Resources.genericDeviceClass);
            SpecificDeviceClasses = JsonConvert.DeserializeObject<IList<SpecificDeviceClass>>(Properties.Resources.specificDeviceClass);
        }
    }
}