using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Managers;
using System.Reflection;

namespace HellionExtendedServer.Components
{
    public class Config
    {
        #region Fields

        private bool m_debugMode = false;

        public enum Language
        {
            French,
            English
        }

        public static string FileName = "HES.cfg";

        #endregion

        #region Properties

        public Language CurrentLanguage = Language.English;
        public bool InDebugMode { get { return m_debugMode; } }

        #endregion

        #region Methods

        public Config()
        {

        }

        public void Load()
        {
            if (File.Exists(FileName))
            {
                foreach (string line in File.ReadLines(FileName))
                {
                    if (!line.StartsWith("#"))
                    {
                        var lineSetting = line.ToLower().Split("=".ToCharArray(), 2);
                        if (lineSetting[0] == "language") { CurrentLanguage = ParseLanguage(lineSetting[1]); }
                        if (lineSetting[0] == "debugmode") { m_debugMode = bool.Parse(lineSetting[1]); }
                    }
                }
            }
            else
            {
                Log.Instance.Warn("No config file found ! Default config file created.");
                Save();
            }
        }

        public void Save()
        {
            File.WriteAllLines(FileName, new string[] { "debugmode=" + m_debugMode.ToString(), "language=" + CurrentLanguage.ToString() });
            Load();
        }
        
        private Language ParseLanguage(string language)
        {
            if (language == "english")
                return Language.English;
            if (language == "french")
                return Language.French;
            return Language.English;
        }

        #endregion
    }
}
