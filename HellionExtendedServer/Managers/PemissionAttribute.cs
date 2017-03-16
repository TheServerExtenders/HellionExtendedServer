using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionExtendedServer.Managers
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class PermissionAttribute : Attribute
    {
        public string PermissionName;
        public string Description;
        public string Default = "default";//default, OP, false
        //TODO change later
    }
}
