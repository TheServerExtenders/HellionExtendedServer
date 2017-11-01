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

                        if (m_settings.Exists(x => x.Name.Equals(setting.Name)))
                            inSetting = m_settings.First(x => x.Name.Equals(setting.Name));

                        if (inSetting.Valid)
                            newSetting = inSetting;
                        else
                            newSetting = setting;

                        if(newSetting.Valid)
                            Console.WriteLine($"enabled:{newSetting.Enabled} name:{newSetting.Name} valid:{newSetting.Valid}");

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

        public List<Setting> Load()
        {
            var m_settings = GameServerINI.ParseSettings();

            try
            {
                List<Setting> tmp = new List<Setting>();

                // for every setting in the gameserver_example.ini
                foreach (Setting defaultSetting in GameServerINI.ParseDefaultSettings())
                {
                    // then set the out setting to the default setting incase something goes wrong
                    Setting setting = defaultSetting;

                    // if the setting exists, then get the setting from the gameserver.ini
                    if (m_settings.Exists(x => x.Name == defaultSetting.Name))
                        setting = m_settings.Find(x => x.Name == defaultSetting.Name);

                    // set the new setting to have the values of the default setting
                    var newSetting = defaultSetting;
                  
                    // if the values of the default setting and the current setting
                    // are not the same, and the setting contains an '=' sign
                    if (setting.Enabled)
                    {                
                        // set the new value to the current value
                        newSetting.Value = setting.Value;
                        // pull if the line was orginally disabled ( starts with a '#' )
                        newSetting.Enabled = setting.Enabled;
                        newSetting.Valid = setting.Valid;

                        // add the new setting to the temp list that goes to the property panel
                        tmp.Add(newSetting);
                    }
                    else
                    {
                        // if the values are the same, then just add the default value to the list
                        tmp.Add(defaultSetting);
                    }

                }

                return tmp;
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"[ERROR] Hellion Extended Server[{ex.TargetSite}]: {ex.StackTrace}");
            }

            return null;
        }

        public List<Setting> LoadDefaults()
        {
            return  GameServerINI.ParseDefaultSettings();           
        }
    }
}