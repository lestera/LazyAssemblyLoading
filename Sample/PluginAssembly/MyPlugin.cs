using System;
using Interfaces;

namespace PluginAssembly
{
    [PluginExport("My Plugin", "1.0")]
    public class MyPlugin : IPlugin
    {
        public void Initialize()
        {
            Console.WriteLine("{0} initialized!", GetType().FullName);
        }
    }
}
