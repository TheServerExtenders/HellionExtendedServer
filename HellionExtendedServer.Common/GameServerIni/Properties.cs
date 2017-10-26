using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HellionExtendedServer.Common.GameServerIni
{
    public class Properties
    {
        private static string m_fileName = "GameServer.ini";
        private List<Setting> m_settings = new List<Setting>();



        public List<Setting> Settings => m_settings;



        public Properties()
        {
            if (!File.Exists(m_fileName + ".original"))
                File.Copy(m_fileName, m_fileName + ".original");

            LoadDefaults();
        }

        public void Save()
        {
            try
            {
                if (File.Exists(m_fileName))
                {
                    using (StreamWriter file = new StreamWriter(m_fileName))
                        foreach (var entry in Settings)
                            file.WriteLine("{0}={1}", entry.Name, entry.Value);
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

        public void Load()
        {
            try
            {

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
                m_settings = DefaultGameServerINI.ParseSettings();
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"[ERROR] Hellion Extended Server[{ex.TargetSite}]: {ex.StackTrace}");
            }
        }


    }
}
