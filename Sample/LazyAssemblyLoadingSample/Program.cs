using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Interfaces;
using LazyAssemblyLoading;
using LazyAssemblyLoading.Serialization;

namespace LazyAssemblyLoadingSample
{
    class Program : MarshalByRefObject
    {
        private const string DataFile = @"C:\partInformation.xml";
        private const string PluginAssemblyName = "PluginAssembly";
        private const string PluginAssemblyFileName = "PluginAssembly.dll";

        [ImportMany]
        public Lazy<IPlugin, IPluginMetadata>[] Plugins { get; set; }

        static void Main(string[] args)
        {
            // Serialize the plugin assembly's exports to a file.
            // This is done in a different app domain, which is then immediately unloaded so the assembly is not loaded after this line.
            // In a real-world application, the serialization process would probably take place in the plugin's post-build event.
            CreatePartInformationInAnotherAppDomain(PluginAssemblyFileName, DataFile);

            // Compose
            var p = new Program();
            p.ComposeUsingLazyAssemblyCatalog();

            // Plugin metadata is accessible even though the plugin assembly has not been loaded
            string pluginName = p.Plugins[0].Metadata.Name;
            Console.WriteLine("Plugin name (taken from metadata): {0}", pluginName);
            Console.WriteLine("Is assembly {0} loaded: {1}", PluginAssemblyName, IsAssemblyLoaded(PluginAssemblyName));

            // Accessing the plugin itself will implicitly load its assembly
            Console.WriteLine("Creating plugin {0} by accessing its Value property", pluginName);
            var plugin = p.Plugins[0].Value;
            Console.WriteLine("Is assembly {0} loaded: {1}", PluginAssemblyName, IsAssemblyLoaded(PluginAssemblyName));

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
