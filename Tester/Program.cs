using LazyAssemblyLoading;
using LazyAssemblyLoading.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestInterfaces;

namespace Tester
{
    class Program : MarshalByRefObject
    {
        private const string DataFile = @"C:\partInformation.xml";

        [ImportMany]
        public Lazy<IPlugin, IPluginMetadata>[] Plugins { get; set; }

        static void Main(string[] args)
        {
            // Serialize the plugin assembly's exports to a file.
            // This is done in a different app domain, which is then immediately unloaded so the assembly is not loaded after this line.
            // In a real-world application, the serialization process would probably take place in the plugin's post-build event.
            CreatePartInformationInAnotherAppDomain("TestAssembly1.dll", DataFile);

            // Compose
            var p = new Program();
            p.ComposeUsingLazyAssemblyCatalog();

            // Plugin metadata is accessible even though the plugin assembly has not been loaded
            Console.WriteLine("Plugin name: {0}", p.Plugins[0].Metadata.Name);
            Console.WriteLine("Is Loaded: {0}", IsAssemblyLoaded("TestAssembly1"));

            // Accessing the plugin itself will implicitly load its assembly
            Console.WriteLine("Creating plugin");
            var plugin = p.Plugins[0].Value;
            Console.WriteLine("Is Loaded: {0}", IsAssemblyLoaded("TestAssembly1"));
            plugin.Initialize();
        }

        private static void CreatePartInformationInAnotherAppDomain(string assemblyLocation, string outputDataFile)
        {
            var pluginDomain = AppDomain.CreateDomain("PluginDomain");
            try
            {
                var pluginDomainProgram = (Program)pluginDomain.CreateInstanceAndUnwrap(typeof(Program).Assembly.FullName, typeof(Program).FullName);
                pluginDomainProgram.CreatePartInformationFromAssembly(assemblyLocation, outputDataFile);
            }
            finally
            {
                AppDomain.Unload(pluginDomain);
            }
        }

        private void CreatePartInformationFromAssembly(string assemblyLocation, string outputDataFile)
        {
            var assembly = Assembly.LoadFrom(assemblyLocation);
            SerializationUtils.SerializeAssemblyPartInformation(assembly, outputDataFile);
        }

        private void ComposeUsingLazyAssemblyCatalog()
        {
            var catalog = new LazyAssemblyCatalog(DataFile);
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }

        private static bool IsAssemblyLoaded(string assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Select(a => new AssemblyName(a.FullName))
                                                          .Any(a => a.Name == assemblyName);
        }
    }
}
