using System;
using System.IO;
using System.Linq;
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

    public class JSaver
    {
        private JObject J;
        public JSaver(JObject jo)
        {
            J = jo;
        }

        /// <summary>
        /// Allows for keys with '.'
        /// Example test.key.key2
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetValue(int key)
        {
            
        }
        public object GetValue(string key)
        {
            if (key == null) return null;
            if (J.KeyExists(key)) return J[key];
            string[] keys = key.Split(".".ToCharArray());
            if (keys.Length <= 0) return null;
            string newkey = String.Join(".",keys);
            if (!J.KeyExists(keys[0])) return null;
            JToken value = J[keys[0]];
            if (value.Type != JTokenType.Object && value.Type != JTokenType.Array) return null;
            JSaver njs = new JSaver(value);
            return J[key];
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

        public static bool KeyExists(this JObject JO, string key)
        {
            if (JO[key] != null) return true;
            return false;
        }


        public static object Get(JObject J, string key)
        {
            if (key == null) return null;
            if (J.KeyExists(key)) return J[key];
            string[] keys = key.Split(".".ToCharArray());
            if (keys.Length <= 0) return null;
            string newkey = String.Join(".", keys.Skip(1));
            if (!J.KeyExists(keys[0])) return null;
            JToken value = J[keys[0]];
            if (value.Type != JTokenType.Object) return null;
            JObject jo = value as JObject;
            //TODO check if Int?
            return jo.GetValue(newkey);
        }

        public static void Save(this JObject JO, string file)
        {
            File.WriteAllText(file, JO.ToString());
        }
    }
}
