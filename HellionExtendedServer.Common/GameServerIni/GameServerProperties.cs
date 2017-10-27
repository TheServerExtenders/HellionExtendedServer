using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HellionExtendedServer.Common.GameServerIni
{
    public class GameServerProperties
    {
        private static string m_fileName = "GameServer.ini";
        private List<Setting> m_defaultSettings = new List<Setting>();
        private List<Setting> m_settings = new List<Setting>();
        

        public List<Setting> Settings => m_settings;
        public List<Setting> DefaultSettings => m_defaultSettings;

        public GameServerProperties()
        {
            if (!File.Exists(m_fileName + ".original"))
                File.Copy(m_fileName, m_fileName + ".original");

            LoadDefaults();
            Load();
        }

        public void Save()
        {
            List<Setting> newSettings = new List<Setting>();

            try
            {
                if (File.Exists(m_fileName))
                {
                    LoadDefaults();

                    using (StreamWriter file = new StreamWriter(m_fileName + ".txt"))
                        foreach (var entry in m_settings)
                        {

                            var defaultSetting = entry;

                             if (m_defaultSettings.Exists(x => x.Name == entry.Name))
                                    defaultSetting = m_defaultSettings.Find(x => x.Name == entry.Name);

                            if (entry.Value != defaultSetting.Value)
                            {
                                file.WriteLine("{0}{1}={2}", entry.Enabled ? "" : "#", entry.Name, entry.Value);
                            }
                        }
                         

                }
                else
                {
                    Log.Instance.Warn("GameServer.Ini wasn't found! Creating a new one based on Defaults. ");
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"[ERROR] Hellion Extended Server[{ex.TargetSite}]: {ex.StackTrace}");
            }
        }

        // build list from default settings, update default setting with settings from gameserver.ini

        public void Load()
        {
            m_settings = GameServerINI.ParseSettings();

            try
            {
                
                List<Setting> tmp = new List<Setting>();

                foreach(Setting defaultSetting in m_defaultSettings)
                {
                    Setting setting = defaultSetting;

                    if(m_settings.Exists(x => x.Name == defaultSetting.Name))
                        setting = m_settings.Find(x => x.Name == defaultSetting.Name);

                    var newSetting = defaultSetting;


                    if (setting.Value != defaultSetting.Value)
                    {

                        //if (setting.Valid)
                        //{                           
                        newSetting.Value = setting.Value;
                        newSetting.Enabled = setting.Enabled;
                        //newSetting.

                        tmp.Add(newSetting);
                        //}

                    }
                    else
                    {
                        tmp.Add(defaultSetting);
                    }
                 
                }

                m_settings = tmp;

            }
            catch (Exception ex)
            {
                Log.Instance.Error($"[ERROR] Hellion Extended Server[{ex.TargetSite}]: {ex.StackTrace}");
            }
        }

        public void LoadDefaults()
        {
            try
            {
                m_defaultSettings = DefaultGameServerINI.ParseSettings();
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"[ERROR] Hellion Extended Server[{ex.TargetSite}]: {ex.StackTrace}");
            }
        }
    }
}