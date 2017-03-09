using HellionExtendedServer.Common.Plugins;
using System;
using System.Reflection;

namespace HellionExtendedServer.Managers.Plugins
{
    public class PluginInfo
    {
        #region Fields

        internal Assembly Assembly;
        public String Directory;
        public Guid Guid;
        public PluginBase MainClass;
        public Dictionary<String, Command> CommandList;
        public List<Type> FoundCommands = new List<Type>();
        public Type MainClassType;

        #endregion Fields

         //TODO load Plugin's Commands Now!
        #region Methods

        static public Boolean operator ==(PluginInfo obj1, PluginInfo obj2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)obj1 == null) || ((object)obj2 == null))
            {
                return false;
            }
            return obj1.Guid == obj2.Guid;
        }

        static public Boolean operator !=(PluginInfo obj1, PluginInfo obj2)
        {
            return !(obj1 == obj2);
        }

        public override bool Equals(object obj)
        {
            if (obj is PluginInfo)
            {
                return this == (PluginInfo)obj;
            }
            return false;
        }

        public override int GetHashCode()
        {
            byte[] by = Guid.ToByteArray();
            int value = 0;
            for (int i = 0; i < by.GetLength(0); i++)
            {
                value += (int)(by[i] & 0xffL) << (8 * i);
            }
            return value;
        }

        #endregion Methods
    }
}