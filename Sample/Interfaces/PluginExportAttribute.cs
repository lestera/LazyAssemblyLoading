using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PluginExportAttribute : ExportAttribute, IPluginMetadata
    {
        public PluginExportAttribute(string pluginName, string pluginVersion)
            : base(typeof(IPlugin))
        {
            Name = pluginName;
            Version = pluginVersion;
        }

        public string Name { get; set; }
        public string Version { get; set; }
    }
}
