using System;
using System.IO;
using LazyAssemblyLoading.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LazyAssemblyLoadingUnitTest
{
    [TestClass]
    public class SerializationUtilsUnitTest
    {
        private const string DataFile = "partInformation.xml";

        [TestInitialize]
        public void InitializeTest()
        {
            File.Delete(DataFile);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SerializeAssemblyPartInformation_NoAssemnbly_ThrowsArgumentException()
        {
            SerializationUtils.SerializeAssemblyPartInformation(null, DataFile);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SerializeAssemblyPartInformation_NoOutputDataFile_ThrowsArgumentException()
        {
            SerializationUtils.SerializeAssemblyPartInformation(typeof(SerializationUtilsUnitTest).Assembly, null);
        }

        [TestMethod]
        public void SerializeAssemblyPartInformation_ValidParameters_DoesNotThrowException()
        {
            AssertEx.DoesNotThrow<Exception>(() => SerializationUtils.SerializeAssemblyPartInformation(typeof(SerializationUtilsUnitTest).Assembly, DataFile));
        }

        [TestMethod]
        public void SerializeAssemblyPartInformation_ValidParameters_DataFileIsCreated()
        {
            Assert.IsFalse(File.Exists(DataFile));
            SerializationUtils.SerializeAssemblyPartInformation(typeof(SerializationUtilsUnitTest).Assembly, DataFile);
            Assert.IsTrue(File.Exists(DataFile));
        }
    }
}
