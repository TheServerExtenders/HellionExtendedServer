using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionExtendedServer.Managers.Plugins
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CommandAttribute : Attribute
    {
        public string CommandName;
        public string Description;
        public string Usage;
        public string Permission;
        public string Plugin;
    }
}
