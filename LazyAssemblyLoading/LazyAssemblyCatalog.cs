using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using LazyAssemblyLoading.Serialization;

namespace LazyAssemblyLoading
{
    /// <summary>
    /// Discovers parts from serialized part information.
    /// </summary>
    public class LazyAssemblyCatalog : ComposablePartCatalog
    {
        #region Private Members

        private IList<ComposablePartDefinition> parts;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyAssemblyCatalog"/>
        /// </summary>
        /// <param name="dataFilePath">Path to the file containing the plugin assembly's serialized part definitions.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dataFilePath"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">The path provided in <paramref name="dataFilePath"/> does not point to an existing file.</exception>
        public LazyAssemblyCatalog(string dataFilePath)
        {
            if (string.IsNullOrEmpty(dataFilePath)) throw new ArgumentNullException("dataFilePath");
            if (!File.Exists(dataFilePath)) throw new ArgumentException("File does not exist", "dataFilePath");

            // Deserialize part data from file
            IEnumerable<SerializableComposablePartDefinition> serializedPartDefinitions;
            using (var dataFile = File.Open(dataFilePath, FileMode.Open, FileAccess.Read))
            {
                var serializer = new XmlSerializer(typeof(List<SerializableComposablePartDefinition>));
                serializedPartDefinitions = (IEnumerable<SerializableComposablePartDefinition>)serializer.Deserialize(dataFile);
            }

            // TODO: Initialize the parts upon request and not in the constructor
            InitializeParts(serializedPartDefinitions);
        }

        #endregion

        #region Overrides

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return parts.AsQueryable(); }
        }

        #endregion

        #region Private Methods

        private void InitializeParts(IEnumerable<SerializableComposablePartDefinition> serializedPartDefinitions)
        {
            parts = new List<ComposablePartDefinition>();
            foreach (var spd in serializedPartDefinitions)
            {
                parts.Add(SerializationUtils.CreateComposablePartDefinition(spd));
            }
        }

        #endregion
    }
}
