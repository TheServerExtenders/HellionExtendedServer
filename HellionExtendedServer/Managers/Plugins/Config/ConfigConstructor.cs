using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace HellionExtendedServer.Managers.Plugins.Config
{
    public class ConfigConstructor
    {

        private bool IsValid = false;
        private string FilePath;
        private JObject JO;

        public static JObject GetJObject(string file)
        {
            try
            {
                string json = File.ReadAllText(file, Encoding.UTF8);
                return JObject.Parse(json);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public static class JobjectStatic
    {
        public static int GetInt(this JObject j, string key)
        {
            if (j.KeyExists(key) && j[key].Type == JTokenType.Integer) return j[key].Value<int>();
            return 0;
        }
        public static string GetString(this JObject j, string key)
        {
            if (j.KeyExists(key) && j[key].Type == JTokenType.String) return j[key].Value<string>();
            return "";
        }
        public static string GetJobject(this JObject j, string key)
        {
            if (j.KeyExists(key) && j[key].Type == JTokenType.String) return j[key].Value<string>();
            return "";
        }

        public static bool KeyExists(this JObject JO, string key)
        {
            if (JO[key] != null) return true;
            return false;
        }
    }
}
