using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;

namespace LazyAssemblyLoading.Serialization
{
    [Serializable]
    public class SerializableImportDefinition
    {
        public SerializableImportDefinition()
        {
        }

        public SerializableImportDefinition(ImportDefinition importDefinition)
        {
            IsExportFactory = ReflectionModelServices.IsExportFactoryImportDefinition(importDefinition);
            if (IsExportFactory)
            {
                // Handle export factories.
                importDefinition = ReflectionModelServices.GetExportFactoryProductImportDefinition(importDefinition);
            }

            ContractName = importDefinition.ContractName;
            Cardinality = importDefinition.Cardinality;
            IsRecomposable = importDefinition.IsRecomposable;
            IsPrerequisite = importDefinition.IsPrerequisite;
            Metadata = new SerializableDictionary<string,object>(importDefinition.Metadata);
            ImportingMember = ReflectionModelServices.IsImportingParameter(importDefinition) ?
                new SerializableLazyMemberInfo(ReflectionModelServices.GetImportingParameter(importDefinition).Value) :
                new SerializableLazyMemberInfo(ReflectionModelServices.GetImportingMember(importDefinition));

            var contractBasedImportDefinition = importDefinition as ContractBasedImportDefinition;
            if (contractBasedImportDefinition != null)
            {
                RequiredTypeIdentity = contractBasedImportDefinition.RequiredTypeIdentity;
                RequiredCreationPolicy = contractBasedImportDefinition.RequiredCreationPolicy;
                RequiredMetadata = new SerializableDictionary<string,string>(contractBasedImportDefinition.RequiredMetadata.Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value.AssemblyQualifiedName)));
            }
        }

        public string ContractName { get; set; }
        public ImportCardinality Cardinality { get; set; }
        public bool IsRecomposable { get; set; }
        public bool IsPrerequisite { get; set; }
        public bool IsExportFactory { get; set; }
        public SerializableDictionary<string, object> Metadata { get; set; }
        public SerializableLazyMemberInfo ImportingMember { get; set; }

        public string RequiredTypeIdentity { get; set; }
        public CreationPolicy RequiredCreationPolicy { get; set; }
        public SerializableDictionary<string, string> RequiredMetadata { get; set; }
    }
}
