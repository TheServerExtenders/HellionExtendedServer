using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace HellionExtendedServer.Common.GameServerIni
{
    public class GameServerProperties
    {
        private static string m_fileName = "GameServer.ini";
        private static string m_originalFileName = "hes\\GameServer.ini.original";
        private static string m_backupFileName = "hes\\GameServer.ini.backup";

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
                else
                {
                    Log.Instance.Warn("GameServer.Ini wasn't found! Creating one from the original made on " +
                        File.GetLastWriteTime(m_originalFileName));

                    File.Copy(m_originalFileName, m_fileName);
                }
            }

            if (!File.Exists(m_originalFileName))
                File.Copy(m_fileName, m_originalFileName);
        }

        public bool Save()
        {
            try
            {
                if (File.Exists(m_fileName))
                {
                    // pull in the current settings in the file
                    List<Setting> prevSettings = GameServerINI.ParseSettings();
                    // the list that will be saved to the GameServer.Ini
                    List<Setting> outSettings = new List<Setting>();

                    foreach (Setting setting in prevSettings)
                    {
                        Setting newSetting = setting;
                        Setting inSetting = setting;

                        if (m_settings.Exists(x => x.Name.Equals(setting.Name)))
                            inSetting = m_settings.Find(x => x.Name.Equals(setting.Name));

                        if (inSetting.Valid)
                        {
                            outSettings.Add(inSetting);
                            Console.WriteLine(string.Format($"Changing value of { (inSetting.Enabled ? "" : "#") }{setting.Name}={setting.Value} to {inSetting.Enabled}{inSetting.Value}"));
                        }                      
                        else
                        {
                            outSettings.Add(setting);
                        }
                           

                    }

                    File.Copy(m_fileName, m_backupFileName, true);

                    using (StreamWriter file = new StreamWriter(m_fileName))
                        foreach (var entry in outSettings)
                            file.WriteLine(entry.ToString());

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
                        newSetting.Valid = setting.Valid;
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