using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace HellionExtendedServer.Common.GameServerIni
{
    public static class GameServerINI
    {
        private const string m_fileName = "GameServer.ini";
        private const string m_originalFileName = "hes\\config\\GameServer.ini.original";
        private const string m_backupFileName = "hes\\config\\GameServer.ini.backup";
        private const string m_exampleFileName = "hes\\config\\GameServer_example.ini";

        public static bool SaveSettings(List<Setting> newSettings)
        {
            try
            {
                if (File.Exists(m_fileName))
                {
                    var tmp = new List<Setting>();

                    File.Copy(m_fileName, m_backupFileName, true);

                    // for every setting in the current gameserver.ini
                    foreach (Setting oldSetting in ParseSettings())
                    {
                        //Set the newSetting to be the old setting at first in case something goes wrong
                        var newSetting = oldSetting;

                        //Get the new setting if it exists from the propertygrid
                        // and set the setting to save to the ini as the one from the grid
                        if (newSettings.Exists(x => x.Name.Equals(oldSetting.Name)))
                            newSetting = newSettings.Find(setting => setting.Name.Equals(oldSetting.Name));

                        if (!tmp.Contains(newSetting))
                        {
                            if (newSetting.Value != oldSetting.Value)
                            {
                                newSetting.Valid = true;
                                newSetting.Enabled = true;
                            }
                            else
                            {
                                newSetting.Enabled = false;
                            }

                            tmp.Add(newSetting);
                        }
                        else
                            tmp.Add(oldSetting);
                    }

                    using (StreamWriter newFile = new StreamWriter(m_fileName))
                        foreach (Setting theSetting in tmp)
                            newFile.WriteLine(theSetting.ToLine());

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

        /// <summary>
        /// Parses the settings from the specified filename.
        /// </summary>
        /// <param name="fileName">The name of the file to parse the settings from</param>
        /// <returns>The list of Settings that were parsed</returns>
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

                            if (int.TryParse(settingValue, out int intParseRes))
                            {
                                currentSetting.Type = typeof(int);
                                currentSetting.Value = intParseRes;
                            }
                            else if (float.TryParse(settingValue, out float floatParseRes))
                            {
                                currentSetting.Type = typeof(float);
                                currentSetting.Value = floatParseRes;
                            }
                            else if (bool.TryParse(settingValue, out bool boolParseRes))
                            {
                                currentSetting.Type = typeof(bool);
                                currentSetting.Value = boolParseRes;
                            }
                            else
                            {
                                currentSetting.Type = typeof(string);
                                currentSetting.Value = settingValue;
                            }
                        }

                        //if (currentSetting.Valid)

                        //if(settings.Exists(x => x.Line != currentSetting.Line))
                        //Console.WriteLine(currentSetting.ToLine());

                        settings.Add(currentSetting);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Instance.Error($"[ERROR] Hellion Extended Server[{ex.TargetSite}]: {ex.StackTrace}");
            }
            return settings;
        }

        /// <summary>
        /// Parses the default settings from the specified filename
        /// </summary>
        /// <param name="fileName">The name of the defaults file to parse the settings from</param>
        /// <returns></returns>
        public static List<Setting> ParseDefaultSettings(string fileName = m_exampleFileName)
        {
            List<Setting> settings = new List<Setting>();

            if (!File.Exists(m_exampleFileName))
                return settings;

            string category = "2. Server Settings";

            try
            {
                foreach (string line in File.ReadLines(fileName))
                {
                    //#save_interval (Default: 900) (-1 = Disabled)	Automatic server save, value in seconds

                    if (line.Contains("#Server Options"))
                        category = "2. Server Settings";

                    if (line.Contains("#Game Optipons") || line.Contains("#Game Options"))
                        category = "3. Game Options";

                    // parse and split
                    var regex = @"(#[a-z_]+(?:[a-zA-Z]+))\s+(\(Default\:\ (?:[-a-z0-9._]+)\)|\(Required\))(.+)";
                    Match match = Regex.Match(line, regex);
                    if (match.Success)
                    {
                        string name = match.Groups[1].Value.Replace("#", "");
                        string defaultText = match.Groups[2].Value;
                        string description = match.Groups[3].Value ?? string.Empty;

                        Setting setting = new Setting();

                        // match the default setting
                        var defaultRegex = @"(?:\(Default\:\s+|\S+)([-0-9._]+|false|true|_blank_)\)";
                        Match match2 = Regex.Match(defaultText, defaultRegex);
                        if (match2.Success)
                        {
                            string defaultValue = match2.Groups[1].Value;

                            int intParseRes;
                            float floatParseRes;
                            bool boolParseRes;

                            if (int.TryParse(defaultValue, out intParseRes))
                            {
                                setting.Type = typeof(int);
                                setting.DefaultValue = intParseRes;
                            }
                            else if (float.TryParse(defaultValue, out floatParseRes))
                            {
                                setting.Type = typeof(float);
                                setting.DefaultValue = floatParseRes;
                            }
                            else if (bool.TryParse(defaultValue, out boolParseRes))
                            {
                                setting.Type = typeof(bool);
                                setting.DefaultValue = boolParseRes;
                            }
                            else if (defaultValue.Contains("_blank_"))
                            {
                                setting.Type = typeof(string);
                                setting.DefaultValue = string.Empty;
                            }
                            else
                            {
                                setting.Type = typeof(string);
                                setting.DefaultValue = string.Empty;
                            }
                            setting.Category = category;
                        }

                        // if the default or description of the setting contains the string (Required)
                        if (defaultText.Contains("(Required)") || description.Contains("(Required)"))
                            setting.Required = true;
                        else
                            setting.Required = false;

                        //TODO: Parse the description
                        setting.Description = Regex.Replace(description, @"\s+", " ");

                        if (name == "server_name")
                        {
                            setting.DefaultValue = "Hellion Game Server";
                            setting.Type = typeof(string);
                            setting.Category = "1. Required Settings";
                        }
                        else if (name == "game_client_port")
                        {
                            setting.DefaultValue = 5969;
                            setting.Type = typeof(int);
                            setting.Category = "1. Required Settings";
                        }
                        else if (name == "status_port")
                        {
                            setting.DefaultValue = 5970;
                            setting.Type = typeof(int);
                            setting.Category = "1. Required Settings";
                        }

                        setting.Line = line;
                        setting.Name = name;
                        setting.Enabled = false;
                        setting.Value = setting.DefaultValue;

                        settings.Add(setting);
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