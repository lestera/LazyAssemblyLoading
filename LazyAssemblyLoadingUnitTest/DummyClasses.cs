using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyAssemblyLoadingUnitTest
{
    [Export]
    internal class DummyClass1
    {
        [Import]
        public DummyClass2 DummyClass2 { get; set; }

        [Import]
        public DummyClass3 DummyClass3 { get; set; }
    }

    [Export]
    internal class DummyClass2
    {
    }

    [Export]
    internal class DummyClass3
    {
        [Import]
        public DummyClass2 DummyClass2 { get; set; }
    }
}
