using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;

namespace LazyAssemblyLoading.Serialization
{
    [Serializable]
    public class SerializableExportDefinition
    {
        public SerializableExportDefinition()
        {
        }

        public SerializableExportDefinition(ExportDefinition exportDefinition)
        {
            ContractName = exportDefinition.ContractName;
            Metadata = new SerializableDictionary<string,object>(exportDefinition.Metadata);
            ExportingMember = new SerializableLazyMemberInfo(ReflectionModelServices.GetExportingMember(exportDefinition));
        }

        public string ContractName { get; set; }
        public SerializableDictionary<string, object> Metadata { get; set; }
        public SerializableLazyMemberInfo ExportingMember { get; set; }
    }
}
