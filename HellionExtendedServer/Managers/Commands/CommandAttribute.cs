using System;

namespace HellionExtendedServer.Managers.Commands
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