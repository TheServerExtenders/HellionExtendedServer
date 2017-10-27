using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace HellionExtendedServer.Common.GameServerIni
{
    public static class GameServerINI
    {
        private const string m_fileName = "GameServer.ini";

        public static List<Setting> ParseSettings(string fileName = m_fileName)
        {
            List<Setting> settings = new List<Setting>();

            try
            {
                //server_name=Hellion Game Server
                //description =
                if (File.Exists(m_fileName))
                {
                    foreach (string line in File.ReadLines(m_fileName))
                    {
                        Setting currentSetting = new Setting();

                        currentSetting.Line = line;

                        var regex = @"(#|)([a-z_]+|[a-zA-Z_-]+)(=|)(.+|)";
                        Match match = Regex.Match(line, regex);
                        if (match.Success)
                        {
                            bool settingEnabled = match.Groups[1].Value.Equals("#") ? false : true;
                            string settingName = match.Groups[2].Value;
                            bool settingIsValid = match.Groups[3].Value.Equals("=") ? true : false;
                            string settingValue = match.Groups[4].Value;

                            currentSetting.Valid = settingIsValid;
                            currentSetting.Enabled = settingEnabled;
                            currentSetting.Name = settingName;

                            int intParseRes;
                            float floatParseRes;
                            bool boolParseRes;

                            if (int.TryParse(settingValue, out intParseRes))
                            {
                                currentSetting.Type = typeof(int);
                                currentSetting.Value = intParseRes;
                            }
                            else if (float.TryParse(settingValue, out floatParseRes))
                            {
                                currentSetting.Type = typeof(float);
                                currentSetting.Value = floatParseRes;
                            }
                            else if (bool.TryParse(settingValue, out boolParseRes))
                            {
                                currentSetting.Type = typeof(bool);
                                currentSetting.Value = boolParseRes;
                            }
                            else
                            {
                                currentSetting.Type = typeof(string);
                                currentSetting.Value = settingValue;
                            }

                            settings.Add(currentSetting);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Instance.Error($"[ERROR] Hellion Extended Server[{ex.TargetSite}]: {ex.StackTrace}");
            }
            return settings;
        }
    }
}