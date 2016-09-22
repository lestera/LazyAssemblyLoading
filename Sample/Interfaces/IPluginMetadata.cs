using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IPluginMetadata
    {
        string Name { get; }
        string Version { get; }
    }
}
