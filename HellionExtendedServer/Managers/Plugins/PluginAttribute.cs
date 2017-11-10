using System;

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