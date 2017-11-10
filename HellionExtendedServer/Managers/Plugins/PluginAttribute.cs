using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionExtendedServer.Managers.Plugins
{

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class PluginAttribute : Attribute
    {
        public string Name;
        public string Version;
        public string Description;
        public string Author;
        public string API;
        public string[] Alliais;
    }
}
