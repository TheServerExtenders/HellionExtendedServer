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
        /*TODO Maybe....
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
                }*/

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

        public static bool KeyExists(this JArray JO, int key)
        {
            if (JO[key] != null) return true;
            return false;
        }
        public static bool KeyExists(this JObject JO, string key)
        {
            if (JO[key] != null) return true;
            return false;
        }


        public static object Get(this JArray J, string key)
        {
            int i;
            if (key == null) return null;
            string[] keys = key.Split(".".ToCharArray());
            if (keys.Length <= 0) return null;
            string newkey = String.Join(".", keys.Skip(1));
            Boolean itp = int.TryParse(keys[0], out i);
            JToken value = itp ? J[i] : J[keys[0]];
            if (value.Type == JTokenType.Object)
            {
                JObject jo = value as JObject;
                return jo.Get(newkey);
            }
            else if (value.Type == JTokenType.Array)
            {
                JArray ja = value as JArray;
                return ja.Get(newkey);
            }
            return value;
        }
        public static object Get(this JObject J, string key)
        {
            int i;
            if (key == null) return null;
            //if (J.KeyExists(key)) return J[key];
            string[] keys = key.Split(".".ToCharArray());
            if (keys.Length <= 0) return null;
            string newkey = String.Join(".", keys.Skip(1));
            //if (!J.KeyExists(keys[0])) return null;
            Boolean itp = int.TryParse(keys[0], out i);
            JToken value = itp ? J[i] : J[keys[0]];
            if (value.Type == JTokenType.Object)
            {
                JObject jo = value as JObject;
                return jo.Get(newkey);
            }
            else if (value.Type == JTokenType.Array)
            {
                JArray ja = value as JArray;
                return ja.Get(newkey);
            }
            return value;
        }

        public static void Save(this JObject JO, string file)
        {
            File.WriteAllText(file, JO.ToString());
        }
    }
}
