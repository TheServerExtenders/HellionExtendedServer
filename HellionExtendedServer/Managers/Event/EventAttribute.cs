using System;

namespace HellionExtendedServer.Managers.Event
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class HESEventAttribute : Attribute
    {
        public EventID EventType;
    }
}