using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;

namespace LazyAssemblyLoading.Serialization
{
    [Serializable]
    public class SerializableComposablePartDefinition
    {
        public SerializableComposablePartDefinition()
        {
        }

        public static SerializableComposablePartDefinition FromComposablePartDefinition(ComposablePartDefinition composablePartDefinition)
        {
            return new SerializableComposablePartDefinition()
            {
                PartTypeAssemblyQualifiedName = ReflectionModelServices.GetPartType(composablePartDefinition).Value.AssemblyQualifiedName,
                IsDisposalRequired = ReflectionModelServices.IsDisposalRequired(composablePartDefinition),
                ImportDefinitions = composablePartDefinition.ImportDefinitions.Select(importDefinition => new SerializableImportDefinition(importDefinition)).ToList(),
                ExportDefinitions = composablePartDefinition.ExportDefinitions.Select(exportDefinition => new SerializableExportDefinition(exportDefinition)).ToList(),
                Metadata = new SerializableDictionary<string, object>(composablePartDefinition.Metadata),
            };
        }

        public string PartTypeAssemblyQualifiedName { get; set; }
        public bool IsDisposalRequired { get; set; }
        public List<SerializableImportDefinition> ImportDefinitions { get; set; }
        public List<SerializableExportDefinition> ExportDefinitions { get; set; }
        public SerializableDictionary<string, object> Metadata { get; set; }
    }
}
