using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace HellionExtendedServer.Modules
{
    [Serializable]
    public class Settings
    {
        public bool RestartNeeded = false;

        public Settings()
        {
            //Defaults
            AutoStartEnable = false;
            DebugMode = false;
            AutoRestartsEnable = false;
            AutoRestartTime = 0;
            AnnounceRestartTime = true;
            EnableAutomaticUpdates = true;
            EnableDevelopmentVersion = false;
            EnableHellionAutomaticUpdates = true;
            CheckUpdatesTime = 60;
            CurrentLanguage = Config.Language.English;

            usePreReleaseVersions = EnableDevelopmentVersion;
        }

        [Category("Development")]
        [DisplayName("Debug Mode - (Default: False )")]
        [Description("Enable Verbose printing to the console.")]
        public bool DebugMode { get; set; }

        [Category("Main")]
        [DisplayName("Start Server On Load - (Default: False )")]
        [Description("Starts the Hellion server on HES load.\r\n" +
            "If enabled, Hellion Dedicated will automatically start after HES initializes")]
        public bool AutoStartEnable { get; set; }

        [ReadOnly(true)]
        [Category("Main")]
        [DisplayName("Language - (Default: English)")]
        [Description("Language of HES.\r\n" +
            "You can find more Languages in our GitHub. Thats if someone has made a new language file for us!")]
        public Config.Language CurrentLanguage { get; set; }

        [Category("Updates")]
        [DisplayName("Enable Automatic Restarts - (Default: False )")]
        [Description("Allow HES to Restart itself after a set time elapses.\r\n" +
            "Used for automatic restarts and releasing Hes's resources after a set time.")]
        public bool AutoRestartsEnable { get; set; }

        [ReadOnly(true)]
        [Category("Updates")]
        [DisplayName("Automatic Restart Time - (Default: 0 )")]
        [Description("Auto-Restart HES After a set time in minutes.\r\n" +
            "Used for automatic updates and releasing Hes's resources after a set time.")]
        public float AutoRestartTime { get; set; }

        [ReadOnly(true)]
        [Category("Updates")]
        [DisplayName("Announce Restart Time - (Default: True )")]
        [Description("Announce restart time to players on the server.\r\n" +
            "Used for automatic updates and releasing Hes's resources after a set time.")]
        public bool AnnounceRestartTime { get; set; }

        [Category("Updates")]
        [DisplayName("Enable Automatic Updates - (Default: True )")]
        [Description("Allow HES to update itself.\r\n" +
            "Used for automatic updates and releasing Hes's resources after a set time.")]
        public bool EnableAutomaticUpdates { get; set; }

        private bool usePreReleaseVersions = false;

        [Category("Development")]
        [DisplayName("Enable Development Version (Restart Required)- (Default: false )")]
        [Description("Change to true if you would like to use Development versions (I.E. PreReleases).\r\n" +
            "Used for automatic updates and releasing Hes's resources after a set time.")]
        public bool EnableDevelopmentVersion
        {
            get => usePreReleaseVersions;
            set
            {
                usePreReleaseVersions = value;
                RestartNeeded = true;
            }
        }

        [Category("Updates")]
        [DisplayName("Enable Hellion Automatic Updates - (Default: True )")]
        [Description("Allows HES to update Hellion Dedicated.\r\n" +
            "Used for automatic updates and releasing Hes's resources after a set time.")]
        public bool EnableHellionAutomaticUpdates { get; set; }

        [ReadOnly(true)]
        [Category("Updates")]
        [DisplayName("Check Updates Time - (Default: 60 )")]
        [Description("How often should HES check for updates, in minutes.\r\n" +
            "Used for automatic updates and releasing Hes's resources after a set time.")]
        public int CheckUpdatesTime { get; set; }
    }

    /// <summary>
    /// Using XML configurations as its easy to add comments into the configuration file.
    /// </summary>
    public class Config
    {
        public static string FileName = "Hes/config/Config.xml";

        private Settings _settings;

        public static Config Instance;

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
            Instance = this;
            _settings = new Settings();
            LoadConfiguration();
        }

        public bool SaveConfiguration()
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

                if (_settings.RestartNeeded)
                {
                    if (HES.GUI != null)
                    {
                        var result = System.Windows.Forms.MessageBox.Show(
                            "You must Restart HES for this change to take effect\r\n\r\n" +
                            "Would you like to restart now?\r\n" +
                            "Note: If the server is running, it will be stopped before HES is restarted",
                            "HES Restart Needed!",
                            System.Windows.Forms.MessageBoxButtons.YesNoCancel,
                            System.Windows.Forms.MessageBoxIcon.Exclamation);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            HES.Restart(true);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("HellionExtendedServer:  Configuration Save Failed! (SaveConfiguration):" + ex.ToString());
            }

            return false;
        }

        public bool LoadConfiguration()
        {
            if (!File.Exists(FileName))
            {
                Console.WriteLine("HellionExtendedServer:  HES Config does not exist, creating one from defaults.");
                SaveConfiguration();
                return true;
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
                        return false;
                    }

                    _settings = (Settings)serializer.Deserialize(reader);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("HellionExtendedServer:  Configuration Load Failed! (LoadConfiguration):" + ex.ToString());
            }
            return false;
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