using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace HellionExtendedServer.Modules
{
    [Serializable]
    public class Settings
    {

        [DisplayName("Debug Mode  - Verbose printing to the console (Default: false )")]
        public bool DebugMode { get; set; }

        [DisplayName("Enable Automatic Restarts  - Allow HES to Restart itself after a set time elapses. (Default: false )")]
        [Description("Used for automatic restarts and releasing Hes's resources after a set time.")]
        public bool AutoRestartsEnable { get; set; }

        [DisplayName("Automatic Restart Time  - Auto-Restart HES After a set time in minutes. (Default: 0 )")]
        [Description("Used for automatic updates and releasing Hes's resources after a set time.")]
        public float AutoRestartTime { get; set; }

        [DisplayName("Announce Restart Time  - Announce restart time to players on the server. (Default: true )")]
        [Description("Used for automatic updates and releasing Hes's resources after a set time.")]
        public bool AnnounceRestartTime { get; set; }

        [DisplayName("Enable Automatic Updates  - Allow HES to update itself.   (Default: true )")]
        [Description("Used for automatic updates and releasing Hes's resources after a set time.")]
        public bool EnableAutomaticUpdates { get; set; }

        [DisplayName("Enable Hellion Automatic Updates - Allows HES to update Hellion Dedicated (Default: true )")]
        [Description("Used for automatic updates and releasing Hes's resources after a set time.")]
        public bool EnableHellionAutomaticUpdates { get; set; }

        [DisplayName("Check Updates Time - How often should HES check for updates, in minutes.  (Default: 60 )")]
        [Description("Used for automatic updates and releasing Hes's resources after a set time.")]
        public int CheckUpdatesTime { get; set; }

        [DisplayName("Language  - Language of the console (Default: English)")]
        [Description("You can find more Languages in our GitHub. Thats if someone has made a new language file for us!")]
        public Config.Language CurrentLanguage { get; set; }

        public Settings()
        {
            DebugMode = false;
            AutoRestartsEnable = false;
            AutoRestartTime = 0;
            AnnounceRestartTime = true;
            EnableAutomaticUpdates = true;
            EnableHellionAutomaticUpdates = true;
            CheckUpdatesTime = 60;
            CurrentLanguage = Config.Language.English;
        }
    }

    /// <summary>
    /// Using XML configurations as its easy to add comments into the configuration file.
    /// </summary>
    public class Config
    {
        public static string FileName = "Hes/config/Config.xml";

        private Settings _settings;

        public Settings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                SaveConfiguration();
            }
        }

        public Config()
        {
            _settings = new Settings();
            LoadConfiguration();
        }

        public void SaveConfiguration()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    var writer = new StreamWriter(ms);
                    var serializer = new XmlSerializer(typeof(Settings));
                    serializer.Serialize(writer, _settings);
                    writer.Flush();

                    File.WriteAllBytes(FileName, ms.ToArray());                   
                }

                WriteComments();
            }
            catch (Exception ex)
            {
                Console.WriteLine("HellionExtendedServer:  Configuration Save Failed! (SaveConfiguration):" + ex.ToString());
            }
        }

        public void LoadConfiguration()
        {
            if (!File.Exists(FileName))
            {
                Console.WriteLine("HellionExtendedServer:  HES Config does not exist, creating one from defaults.");
                SaveConfiguration();              
                return;
            }

            try
            {
                var serializer = new XmlSerializer(typeof(Settings));

                using (Stream fs = new FileStream(FileName, FileMode.Open))
                using (XmlReader reader = new XmlTextReader(fs))
                {
                    if (!serializer.CanDeserialize(reader))
                    {
                        Console.WriteLine("HellionExtendedServer:  Could not deserialize HES's Configuration File! Load Failed!");
                        return;
                    }

                    _settings = (Settings)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HellionExtendedServer:  Configuration Load Failed! (LoadConfiguration):" + ex.ToString());
            }
        }

        private void WriteComments()
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(FileName);

                var parent = doc.SelectSingleNode(typeof(Settings).Name);
                if (parent == null) return;

                foreach (XmlNode child in parent.ChildNodes)
                {
                    PropertyInfo property = (typeof(Settings).GetProperty(child.Name));

                    if (property != null)
                    {
                        XmlNode nameNode = null;
                        if (Attribute.IsDefined(property, typeof(DisplayNameAttribute)))
                        {
                            DisplayNameAttribute name = (DisplayNameAttribute)property.GetCustomAttribute(typeof(DisplayNameAttribute));
                            nameNode = parent.InsertBefore(doc.CreateComment(name.DisplayName), child);
                        }

                        if (Attribute.IsDefined(property, typeof(DescriptionAttribute)))
                        {
                            DescriptionAttribute description = (DescriptionAttribute)property.GetCustomAttribute(typeof(DescriptionAttribute));

                            parent.InsertBefore(doc.CreateComment(description.Description), child);

                        }
                    }
                }
                doc.Save(FileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("HellionExtendedServer:  Configuration Save Failed! (WriteComments)" + ex.ToString());
            }
        }

        public enum Language
        {
            English,
        }
    }
}