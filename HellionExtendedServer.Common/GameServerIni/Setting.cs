using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HellionExtendedServer.Common.GameServerIni
{
    public class Setting
    {
        public string Name;
        public bool Enabled;
        public bool Required;
        public string Category;
        public Type Type;
        public object DefaultValue;
        public object Value;
        public string Description;
        public string Line;
        public bool Valid;

     
        public Setting()
        {
            Name = string.Empty;
            Enabled = false;
            Required = false;
            Category = string.Empty;
            Type = null;
            DefaultValue = null;
            Value = null;
            Description = string.Empty;
            Line = string.Empty;
            Valid = false;
        }

        public override string ToString()
        {
            try
            {
                var regex = @"(#|)([a-z_]+|[a-zA-Z_-]+)(=|)(.+|)";

                if (Type != null && Valid)
                    return Regex.Replace(Line, regex, Enabled ? "" : "#" + "$2=" + Convert.ChangeType(Value, Type) + "$'");
                else
                    return Line;
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"[ERROR] Hellion Extended Server[{ex.TargetSite}]: {ex.StackTrace}");
            }
            return base.ToString();
        }
    }
}
