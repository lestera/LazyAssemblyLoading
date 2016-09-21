using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestInterfaces;

namespace TestAssembly1
{
    [PluginExport("My Plugin", "1.0")]
    public class TestPlugin1 : IPlugin
    {
        public void Initialize()
        {
            Console.WriteLine("TestPlugin1 initialized!");
        }
    }
}
