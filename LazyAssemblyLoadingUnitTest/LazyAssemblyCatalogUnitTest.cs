using System;
using LazyAssemblyLoading;
using LazyAssemblyLoading.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace LazyAssemblyLoadingUnitTest
{
    [TestClass]
    public class LazyAssemblyCatalogUnitTest
    {
        [TestInitialize]
        private void Initialize()
        {
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NoDataFile_ThrowsArgumentNullException()
        {
            new LazyAssemblyCatalog(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_DataFileDoesNotExist_ThrowsArgumentException()
        {
            new LazyAssemblyCatalog("non-existing-file");
        }

        [TestMethod]
        public void Constructor_InitializeFromValidDataFile_DoesNotThrow()
        {
            const string dataFile = "data.xml";
            SerializationUtils.SerializeAssemblyPartInformation(typeof(DummyClass1).Assembly, dataFile);
            AssertEx.DoesNotThrow<Exception>(() => new LazyAssemblyCatalog(dataFile));

            File.Delete(dataFile);
        }
    }
}
