using System;

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