using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using LazyAssemblyLoading.Utilities;

namespace LazyAssemblyLoading.Serialization
{
    public static class SerializationUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Serializes an assembly's part information to a file.
        /// </summary>
        /// <param name="assembly">The assembly to serialize the part information for.</param>
        /// <param name="outputDataFile">The path to the file that will hold the assembly's serialization information.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="outputDataFile"/> is null or empty.</exception>
        public static void SerializeAssemblyPartInformation(Assembly assembly, string outputDataFile)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            if (string.IsNullOrEmpty(outputDataFile)) throw new ArgumentNullException("outputDataFile");

            var assemblyCatalog = new AssemblyCatalog(assembly);
            var parts = assemblyCatalog.Select(SerializableComposablePartDefinition.FromComposablePartDefinition).ToList();
            var serializer = new XmlSerializer(typeof(List<SerializableComposablePartDefinition>));

            using (var output = File.Open(outputDataFile, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(output, parts);
            }
        }
        
        #endregion

        #region Internal Static Methods

        internal static ComposablePartDefinition CreateComposablePartDefinition(SerializableComposablePartDefinition serializableComposablePartDefinition)
        {
            return ReflectionModelServices.CreatePartDefinition(
                new Lazy<Type>(() => Type.GetType(serializableComposablePartDefinition.PartTypeAssemblyQualifiedName)),
                serializableComposablePartDefinition.IsDisposalRequired,
                new Lazy<IEnumerable<ImportDefinition>>(() => serializableComposablePartDefinition.ImportDefinitions.Select(CreateImportDefinition)),
                new Lazy<IEnumerable<ExportDefinition>>(() => serializableComposablePartDefinition.ExportDefinitions.Select(CreateExportDefinition)),
                new Lazy<IDictionary<string, object>>(() => serializableComposablePartDefinition.Metadata),
                null); // TODO: Is it OK to have null for origin?
        }
        
        internal static ImportDefinition CreateImportDefinition(SerializableImportDefinition serializableImportDefinition)
        {
            if (serializableImportDefinition.ImportingMember.MemberType == MemberTypes.Constructor)
            {
                var lazyParameter = new Lazy<ParameterInfo>(() =>
                {
                    Type declaringType = Type.GetType(serializableImportDefinition.ImportingMember.DeclaringTypeAssemblyQualifiedName);
                    return MefUtils.GetImportingConstructor(declaringType).GetParameters().Single(x => x.Name == serializableImportDefinition.ImportingMember.MemberName);
                });

                return ReflectionModelServices.CreateImportDefinition(
                    lazyParameter,
                    serializableImportDefinition.ContractName,
                    serializableImportDefinition.RequiredTypeIdentity,
                    serializableImportDefinition.RequiredMetadata.Select(kvp => new KeyValuePair<string, Type>(kvp.Key, Type.GetType(kvp.Value))),
                    serializableImportDefinition.Cardinality,
                    serializableImportDefinition.RequiredCreationPolicy,
                    serializableImportDefinition.Metadata,
                    serializableImportDefinition.IsExportFactory,
                    null); // TODO: Is it OK to have null for origin?
            }
            else
            {
                return ReflectionModelServices.CreateImportDefinition(
                    CreateLazyMemberInfo(serializableImportDefinition.ImportingMember),
                    serializableImportDefinition.ContractName,
                    serializableImportDefinition.RequiredTypeIdentity,
                    serializableImportDefinition.RequiredMetadata.Select(kvp => new KeyValuePair<string, Type>(kvp.Key, Type.GetType(kvp.Value))),
                    serializableImportDefinition.Cardinality,
                    serializableImportDefinition.IsRecomposable,
                    serializableImportDefinition.IsPrerequisite,
                    serializableImportDefinition.RequiredCreationPolicy,
                    serializableImportDefinition.Metadata,
                    serializableImportDefinition.IsExportFactory,
                    null); // TODO: Is it OK to have null for origin?
            }
        }

        internal static ExportDefinition CreateExportDefinition(SerializableExportDefinition serializableExportDefinition)
        {
            return ReflectionModelServices.CreateExportDefinition(
                CreateLazyMemberInfo(serializableExportDefinition.ExportingMember),
                serializableExportDefinition.ContractName,
                new Lazy<IDictionary<string, object>>(() => serializableExportDefinition.Metadata),
                null); // TODO: Is it OK to have null for origin?
        }
        
        #endregion

        #region Private Static Methods

        private static LazyMemberInfo CreateLazyMemberInfo(SerializableLazyMemberInfo serializableLazyMemberInfo)
        {
            return new LazyMemberInfo(serializableLazyMemberInfo.MemberType, () => GetAccessors(serializableLazyMemberInfo));
        }

        private static MemberInfo[] GetAccessors(SerializableLazyMemberInfo serializableLazyMemberInfo)
        {
            Type declaringType = Type.GetType(serializableLazyMemberInfo.DeclaringTypeAssemblyQualifiedName);
            if (declaringType == null)
            {
                throw new Exception("Could not find type " + serializableLazyMemberInfo.DeclaringTypeAssemblyQualifiedName);
            }

            switch (serializableLazyMemberInfo.MemberType)
            {
                //case MemberTypes.Constructor:
                //    return new[] { MefUtils.GetImportingConstructor(declaringType) };

                case MemberTypes.Field:
                    var field = declaringType.GetField(serializableLazyMemberInfo.MemberName, MefUtils.DefaultBindingFlags);
                    return new[] { field };

                case MemberTypes.Property:
                    var prop = declaringType.GetProperty(serializableLazyMemberInfo.MemberName, MefUtils.DefaultBindingFlags);
                    return new[] { prop.GetGetMethod(true), prop.GetSetMethod(true) };

                case MemberTypes.Method:
                    var method = declaringType.GetMethod(serializableLazyMemberInfo.MemberName, MefUtils.DefaultBindingFlags);
                    return new[] { method };

                case MemberTypes.TypeInfo:
                    return new[] { declaringType };

                default:
                    throw new NotSupportedException();
            }
        }

        #endregion
    }
}
