using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestInterfaces
{
    public interface IPluginMetadata
    {
        string Name { get; }
        string Version { get; }
    }
}
