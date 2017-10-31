using HellionExtendedServer.Common;
using System.IO;

namespace HellionExtendedServer.Modules
{
    public class Config
    {
        public static string FileName = "Hes/config/Config.cfg";
        private bool m_debugMode = false;
        public Config.Language CurrentLanguage = Config.Language.English;

        public bool InDebugMode
        {
            get
            {
                return this.m_debugMode;
            }
        }

        public void Load()
        {
            if (File.Exists(Config.FileName))
            {
                foreach (string readLine in File.ReadLines(Config.FileName))
                {
                    if (!readLine.StartsWith("#"))
                    {
                        string[] strArray = readLine.ToLower().Split("=".ToCharArray(), 2);
                        if (strArray[0] == "language")
                            this.CurrentLanguage = this.ParseLanguage(strArray[1]);
                        if (strArray[0] == "debugmode")
                            this.m_debugMode = bool.Parse(strArray[1]);
                    }
                }
            }
            else
            {
                Log.Instance.Warn("No config file found ! Default config file created.");
                this.Save();
            }
        }

        public void Save()
        {
            File.WriteAllLines(Config.FileName, new string[2]
            {
                "debugmode=" + this.m_debugMode.ToString(),
                "language=" + this.CurrentLanguage.ToString()
            });
            this.Load();
        }

        private Config.Language ParseLanguage(string language)
        {
            return language == "english" || !(language == "french") ? Config.Language.English : Config.Language.French;
        }

        public enum Language
        {
            French,
            English,
        }
    }
}
