using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HellionExtendedServer.Common.GameServerIni
{
    public class GameServerProperties
    {
        private static string m_fileName = "GameServer.ini";
        private static string m_originalFileName = "hes\\config\\GameServer.ini.original";
        private static string m_backupFileName = "hes\\config\\GameServer.ini.backup";
        private static string m_exampleFileName = "hes\\config\\GameServer_example.ini";

        private List<Setting> m_settings = new List<Setting>();

        public List<Setting> Settings { get { return m_settings; } set { m_settings = value; } }

        public GameServerProperties()
        {
            CopyFiles();
            Load();
        }

        private void CopyFiles()
        {
            if (!File.Exists(m_fileName))
            {
                if (File.Exists(m_backupFileName))
                {
                    Log.Instance.Warn("GameServer.Ini wasn't found! Creating one from a backup made on " +
                        File.GetLastWriteTime(m_backupFileName));

                    File.Copy(m_backupFileName, m_fileName);
                }
                else if (File.Exists(m_originalFileName))
                {
                    Log.Instance.Warn("GameServer.Ini wasn't found! Creating one from the original made on " +
                        File.GetLastWriteTime(m_originalFileName));

                    File.Copy(m_originalFileName, m_fileName);
                }
                else
                {
                    Log.Instance.Warn("GameServer.Ini wasn't found! Creating one from GameServer_example.Ini made on " +
                        File.GetLastWriteTime(m_exampleFileName));

                    File.Copy(m_exampleFileName, m_fileName);
                }
                   
            }

            if (!File.Exists(m_originalFileName))
                File.Copy(m_fileName, m_originalFileName);
        }

        public bool Save(List<Setting> mySettings)
        {
            m_settings = mySettings;

            try
            {
                if (File.Exists(m_fileName))
                {
                    // pull in the default settings
                    List<Setting> defSettings = GameServerINI.ParseDefaultSettings();
                    // pull in the current settings in the file
                    List<Setting> prevSettings = GameServerINI.ParseSettings();
                    // the list that will be saved to the GameServer.Ini
                    List<Setting> outSettings = new List<Setting>();

                    foreach (Setting setting in prevSettings)
                    {
                        Setting newSetting = setting;
                        Setting inSetting = setting;
                        Setting defSetting = setting;

                        if (m_settings.Exists(x => x.Name.Equals(setting.Name)))
                            inSetting = m_settings.First(x => x.Name.Equals(setting.Name));

                        if (defSettings.Exists(x => x.Name.Equals(setting.Name)))
                            defSetting = defSettings.First(x => x.Name.Equals(setting.Name));

                        if (inSetting.Valid)
                            newSetting = inSetting;
                        else
                            newSetting = setting;


                        outSettings.Add(newSetting);
                    }

                    File.Copy(m_fileName, m_backupFileName, true);

                    using (StreamWriter file = new StreamWriter(m_fileName))
                        foreach (var entry in outSettings)
                            file.WriteLine(entry.ToLine());

                    return true;
                }
                else
                {
                    Log.Instance.Warn("GameServer.Ini wasn't found! Creating a new one based on Defaults. ");
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"[ERROR] Hellion Extended Server[Save]: {ex.Message} Trace:{ex.StackTrace}");
            }

            return false;
        }

        public bool Load()
        {
            m_settings = GameServerINI.ParseSettings();

            try
            {
                List<Setting> tmp = new List<Setting>();

                foreach (Setting defaultSetting in GameServerINI.ParseDefaultSettings())
                {
                    Setting setting = defaultSetting;

                    if (m_settings.Exists(x => x.Name == defaultSetting.Name))
                        setting = m_settings.Find(x => x.Name == defaultSetting.Name);

                    var newSetting = defaultSetting;
                  
                    if (setting.Value != defaultSetting.Value && setting.Valid)
                    {                       
                        newSetting.Value = setting.Value;
                        newSetting.Enabled = setting.Enabled;
                        tmp.Add(newSetting);
                    }
                    else
                        tmp.Add(defaultSetting);
                }

                m_settings = tmp;

                return true;
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"[ERROR] Hellion Extended Server[{ex.TargetSite}]: {ex.StackTrace}");
            }
            return false;
        }

        public void LoadDefaults()
        {
            try
            {
                m_settings = GameServerINI.ParseDefaultSettings();
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"[ERROR] Hellion Extended Server[{ex.TargetSite}]: {ex.StackTrace}");
            }
        }
    }
}