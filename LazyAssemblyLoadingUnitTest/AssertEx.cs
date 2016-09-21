using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LazyAssemblyLoadingUnitTest
{
    public static class AssertEx
    {
        public static void DoesNotThrow<T>(Action action)
            where T : Exception
        {
            try
            {
                action();
            }
            catch (T ex)
            {
                Assert.Fail("Expected no {0} to be thrown. Details: {1}", typeof(T).Name, ex);
            }
        }
    }
}
