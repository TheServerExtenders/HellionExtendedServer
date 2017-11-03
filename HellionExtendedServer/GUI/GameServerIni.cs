#pragma warning disable IDE1006

using System;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using HellionExtendedServer.Common;


namespace HellionExtendedServer.GUI
{
    public class GameServerIni
    {
        private GameServerIni m_instance;

        public static string m_fileName = "GameServer.ini";
        private static string m_originalFileName = "hes\\config\\GameServer.ini.original";
        private static string m_backupFileName = "hes\\config\\GameServer.ini.backup";
        private static string m_exampleFileName = "hes\\config\\GameServer_example.ini";

        public Dictionary<string, string> Settings = new Dictionary<string, string>();

       
        #region Fields

        // Server Options
        private static string serverName;
        private static int clientPort;
        private static int statusPort;
        private static int maxPlayers;
        private static string serverPass;
        private static string serverDescription;
        private static int saveInterval;
        private static int maxSaveFileCount;
        private static int serverRestartTime;
        private static bool printDebugObjects;

        // Game Options

        private static int _vessel_collision_damage_multiplier;        //(Default: 1)					Damage multiplier for vessel collision
        private static int _vessel_explosion_radius_multiplier;        //(Default: 1)                    Damage radius for exploding vessels
        private static int _vessel_explosion_damage_multiplier;        //(Default: 1)					Damage multiplier for exploding vessels
        private static float _vessel_decay_rate;                         //(Default: 0.05)                 Vessel decay rate in HPs per second
        private static int _activate_repair_point_chance_multiplier;   //(Default: 1)						If value is less then 1 (0-0,9) then chance for is lowered if number is higher than 1 chance is increased 
        private static int _doomed_ship_spawn_frequency;               //(Default: 10800)				Doomed outpust spawn interval, value in seconds // 10800 = 3 hours
        private static int _doomed_ship_timer_min;                     //(Default: 1800)					Doomed outpust minimum despawn time, value in seconds // 1800 = 30 minutes
        private static int _doomed_ship_timer_max;                     //(Default: 3600)					Doomed outpust maximum despawn time, value in seconds // 3600 = 1 hour
        private static float _doomed_ship_spawn_chance;                  //(Default: 0.5)					Doomed outpust spawn chance, value in % (0-1)
        private static bool _spawn_manager_print_categories;            //(Default: false)				Spawn manager debug information
        private static bool _spawn_manager_print_spawn_rules;           //(Default: false)				Spawn manager debug information
        private static bool _spawn_manager_print_item_attach_points;    //(Default: false)				Spawn manager debug information
        private static bool _spawn_manager_print_item_type_ids;         //(Default: false)				Spawn manager debug information


        private static int serverTickCount;
        private static int solarSystemTime;

        #endregion

        public GameServerIni()
        {
            m_instance = this;

            CopyFiles();

            LoadDefaults();
        }

        #region ServerConfig Properties
        [ReadOnly(false)]
        [Description("The max amount of save files that can exist at once. (Set lower to save drive space)")]
        [Category("2. Server Settings")]
        [DisplayName("Max Save Files")]
        [DefaultValue(10)]
        public int number_of_save_files
        {
            get { return maxSaveFileCount; }
            set { maxSaveFileCount = value; }
        }

        [ReadOnly(false)]
        [Description("How many times the server is allowed to update per second (Default: 64)")]
        [Category("2. Server Settings")]
        [DisplayName("Server Tick Count")]
        [DefaultValue(64)]
        public int server_tick_count
        {
            get { return serverTickCount; }
            set { serverTickCount = value; }
        }

        [ReadOnly(false)]
        [Description("How long the time in between saves in seconds. (Default: 900) (-1 = Off)")]
        [Category("2. Server Settings")]
        [DisplayName("Save Interval")]
        [DefaultValue(900)]
        public int save_interval
        {
            get { return saveInterval; }
            set { saveInterval = value; }
        }

        [ReadOnly(false)]
        [Description("The time of the solar systems birth (Default: -1) (-1 = Random)")]
        [Category("2. Server Settings")]
        [DisplayName("Solar System Time")]
        [DefaultValue(-1)]
        public int solar_system_time
        {
            get { return solarSystemTime; }
            set { solarSystemTime = value; }
        }

        [ReadOnly(false)]
        [Description("The name of the server shown to clients in the server browser. (Default: Hellion Dedicated Server)")]
        [Category("1. Required Settings")]
        [DisplayName("Server Name")]
        [DefaultValue("Hellion Dedicated Server")]
        public string server_name
        {
            get { return serverName; }
            set { serverName = value; }
        }

        [ReadOnly(false)]
        [Description("The password to the server. (Default: None)")]
        [Category("1. Required Settings")]
        [DisplayName("Server Password")]
        [DefaultValue("")]
        public string server_password
        {
            get { return serverPass; }
            set { serverPass = value; }
        }

        [ReadOnly(false)]
        [Description("The description to the server. (Default: None)")]
        [Category("2. Server Settings")]
        [DisplayName("Server Description")]
        [DefaultValue("")]
        public string description
        {
            get { return serverDescription; }
            set { serverDescription = value; }
        }

        [ReadOnly(false)]
        [Description("The port to use for the server browser status, ping, and player count. (Default: 5970) (Any)")]
        [Category("1. Required Settings")]
        [DisplayName("Status Port")]
        [DefaultValue(5970)]
        public int status_port
        {
            get { return statusPort; }
            set { statusPort = value; }
        }

        [ReadOnly(false)]
        [Description("The port your clients will use to connect to your server. (Default: 5969) (Any)")]
        [Category("1. Required Settings")]
        [DisplayName("Client Port")]
        [DefaultValue(5969)]
        public int game_client_port
        {
            get { return clientPort; }
            set { clientPort = value; }
        }

        [ReadOnly(false)]
        [Description("Maximum Players allowed on the server. (Default: 20) (Max: 100)")]
        [Category("1. Required Settings")]
        [DisplayName("Max Players")]
        [DefaultValue(20)]
        public int max_players
        {
            get { return maxPlayers; }
            set { maxPlayers = value; }
        }

        [ReadOnly(false)]
        [Description("Automatic server restart, value in seconds (Default: -1) (-1 = Disabled)")]
        [Category("2. Server Settings")]
        [DisplayName("Spawn Manager Print Item Attach Points")]
        [DefaultValue(-1)]
        public int server_restart_time
        {
            get
            {
                if (serverRestartTime == 0)
                    serverRestartTime = -1;

                return serverRestartTime;
            }
            set { serverRestartTime = value; }

        }//(Default: -1) (-1 = Disabled)	Automatic server restart, value in seconds

        [ReadOnly(false)]
        [Description("Set to true for objects on server print (Default: false)	")]
        [Category("2. Server Settings")]
        [DisplayName("Spawn Manager Print Item Attach Points")]
        [DefaultValue(false)]
        public bool print_debug_objects
        {
            get { return printDebugObjects; }
            set { printDebugObjects = value; }
        }//(Default: false)				Set to true for objects on server print

        [ReadOnly(false)]
        [Description("Damage multiplier for vessel collision (Default: 1)")]
        [Category("3. Game Options")]
        [DisplayName("Vessel Collision Damage Multiplier")]
        [DefaultValue(1)]
        public int vessel_collision_damage_multiplier
        {
            get { return _vessel_collision_damage_multiplier; }
            set { _vessel_collision_damage_multiplier = value; }
        }

        [ReadOnly(false)]
        [Description("Damage radius for exploding vessels (Default: 1)")]
        [Category("3. Game Options")]
        [DisplayName("Vessel Explosion Radius Multiplier")]
        [DefaultValue(1)]
        public int vessel_explosion_radius_multiplier        //(Default: 1)                    
        {
            get { return _vessel_explosion_radius_multiplier; }
            set { _vessel_explosion_radius_multiplier = value; }
        }


        [ReadOnly(false)]
        [Description("Damage multiplier for exploding vessels (Default: 1)")]
        [Category("3. Game Options")]
        [DisplayName("Vessel Explosion Damage Multiplier")]
        [DefaultValue(1)]
        public int vessel_explosion_damage_multiplier
        {
            get { return _vessel_explosion_damage_multiplier; }
            set { _vessel_explosion_damage_multiplier = value; }
        }
        //(Default: 1)					Damage multiplier for exploding vessels

        [ReadOnly(false)]
        [Description("Vessel decay rate in HPs per second (Default: 0.05) ")]
        [Category("3. Game Options")]
        [DisplayName("Vessel Decay Rate")]
        [DefaultValue(0.05)]
        public float vessel_decay_rate
        {
            get { return _vessel_decay_rate; }
            set { _vessel_decay_rate = value; }
        }
        //(Default: 0.05)                 Vessel decay rate in HPs per second

        [ReadOnly(false)]
        [Description("If value is less then 1 (0-0,9) then chance for is lowered if number is higher than 1 chance is increased  (Default: 1)")]
        [Category("3. Game Options")]
        [DisplayName("Activate Repair Point Chance Multiplier")]
        [DefaultValue(1)]
        public int activate_repair_point_chance_multiplier
        {
            get { return _activate_repair_point_chance_multiplier; }
            set { _activate_repair_point_chance_multiplier = value; }
        }
        //(Default: 1)						If value is less then 1 (0-0,9) then chance for is lowered if number is higher than 1 chance is increased 

        [ReadOnly(false)]
        [Description("Doomed outpost spawn interval, value in seconds // 10800 = 3 hours (Default: 10800)")]
        [Category("3. Game Options")]
        [DisplayName("Doomed Ship Spawn Frequency")]
        [DefaultValue(10800)]
        public int doomed_ship_spawn_frequency
        {
            get { return _doomed_ship_spawn_frequency; }
            set { _doomed_ship_spawn_frequency = value; }
        }
        //(Default: 10800)				Doomed outpust spawn interval, value in seconds // 10800 = 3 hours

        [ReadOnly(false)]
        [Description("Doomed outpost minimum despawn time, value in seconds // 1800 = 30 minutes (Default: 1800)")]
        [Category("3. Game Options")]
        [DisplayName("Doomed Ship Timer Min")]
        [DefaultValue(1800)]
        public int doomed_ship_timer_min
        {
            get { return _doomed_ship_timer_min; }
            set { _doomed_ship_timer_min = value; }
        }
        //(Default: 1800)					Doomed outpust minimum despawn time, value in seconds // 1800 = 30 minutes

        [ReadOnly(false)]
        [Description("Doomed outpost maximum despawn time, value in seconds // 3600 = 1 hour (Default: 3600)")]
        [Category("3. Game Options")]
        [DisplayName("Doomed Ship Timer Max")]
        [DefaultValue(3600)]
        public int doomed_ship_timer_max
        {
            get { return _doomed_ship_timer_max; }
            set { _doomed_ship_timer_max = value; }
        }
        //(Default: 3600)					Doomed outpust maximum despawn time, value in seconds // 3600 = 1 hour

        [ReadOnly(false)]
        [Description("Doomed outpost spawn chance, value in % (0-1) (Default: 0.5)")]
        [Category("3. Game Options")]
        [DisplayName("Doomed Ship Spawn Chance")]
        [DefaultValue(0.5)]
        public float doomed_ship_spawn_chance
        {
            get { return _doomed_ship_spawn_chance; }
            set { _doomed_ship_spawn_chance = value; }
        }
        //(Default: 0.5)					Doomed outpust spawn chance, value in % (0-1)

        [ReadOnly(false)]
        [Description("Spawn manager debug information (Default: false)")]
        [Category("3. Game Options")]
        [DisplayName("Spawn Manager Print Categories")]
        [DefaultValue(false)]
        public bool spawn_manager_print_categories
        {
            get { return _spawn_manager_print_categories; }
            set { _spawn_manager_print_categories = value; }
        }
        //(Default: false)				Spawn manager debug information

        [ReadOnly(false)]
        [Description("Spawn manager debug information (Default: false)")]
        [Category("3. Game Options")]
        [DisplayName("Spawn Manager Print Spawn Rules")]
        [DefaultValue(false)]
        public bool spawn_manager_print_spawn_rules
        {
            get { return _spawn_manager_print_spawn_rules; }
            set { _spawn_manager_print_spawn_rules = value; }
        }
        //(Default: false)				Spawn manager debug information

        [ReadOnly(false)]
        [Description("Spawn manager debug information (Default: false)")]
        [Category("3. Game Options")]
        [DisplayName("Spawn Manager Print Item Attach Points")]
        [DefaultValue(false)]
        public bool spawn_manager_print_item_attach_points
        {
            get { return _spawn_manager_print_item_attach_points; }
            set { _spawn_manager_print_item_attach_points = value; }
        }
        //(Default: false)				Spawn manager debug information

        [ReadOnly(false)]
        [Description("Spawn manager debug information (Default: false)")]
        [Category("3. Game Options")]
        [DisplayName("Spawn Manager Print Item Attach Points")]
        [DefaultValue(false)]
        public bool spawn_manager_print_item_type_ids
        {
            get { return _spawn_manager_print_item_type_ids; }
            set { _spawn_manager_print_item_type_ids = value; }
        }
        //(Default: false)				Spawn manager debug information


        private object this[string property]
        {
            get
            {
                PropertyInfo myPropInfo = typeof(GameServerIni).GetProperty(property);
                return myPropInfo.GetValue(this, null);
            }
            set
            {
                PropertyInfo myPropInfo = typeof(GameServerIni).GetProperty(property);

                myPropInfo.SetValue(this, Convert.ChangeType(value, myPropInfo.PropertyType), null);

            }

        }

        #endregion

        #region Methods
        public void LoadDefaults()
        {
            try
            {
                serverName = "Hellion Dedicated Server";
                serverPass = "";
                serverDescription = "";
                statusPort = 5970;
                clientPort = 5969;
                maxPlayers = 20;
                maxSaveFileCount = 10;
                serverTickCount = 64;
                solarSystemTime = -1;
                saveInterval = 900;
                serverRestartTime = -1;
                _vessel_collision_damage_multiplier = 1;
                _vessel_explosion_radius_multiplier = 1;        //(Default: 1)                    Damage radius for exploding vessels
                _vessel_explosion_damage_multiplier = 1;        //(Default: 1)					Damage multiplier for exploding vessels
                _vessel_decay_rate = 0.05F;                         //(Default: 0.05)                 Vessel decay rate in HPs per second
                _activate_repair_point_chance_multiplier = 1;   //(Default: 1)						If value is less then 1 (0-0,9) then chance for is lowered if number is higher than 1 chance is increased 
                _doomed_ship_spawn_frequency = 10800;               //(Default: 10800)				Doomed outpust spawn interval, value in seconds // 10800 = 3 hours
                _doomed_ship_timer_min = 1800;                     //(Default: 1800)					Doomed outpust minimum despawn time, value in seconds // 1800 = 30 minutes
                _doomed_ship_timer_max = 3600;                     //(Default: 3600)					Doomed outpust maximum despawn time, value in seconds // 3600 = 1 hour
                _doomed_ship_spawn_chance = 0.5F;                  //(Default: 0.5)					Doomed outpust spawn chance, value in % (0-1)
                _spawn_manager_print_categories = false;            //(Default: false)				Spawn manager debug information
                _spawn_manager_print_spawn_rules = false;           //(Default: false)				Spawn manager debug information
                _spawn_manager_print_item_attach_points = false;    //(Default: false)				Spawn manager debug information
                _spawn_manager_print_item_type_ids = false;         //(Default: false)				Spawn manager debug information



                SetSettings();
            }
            catch (Exception ex)
            {
                Log.Instance.Error("[ERROR] Hellion Extended Server[GameServerIni]: [LoadDefaults]" + ex.StackTrace);
            }

        }


        private void SetSettings()
        {
            try
            {
                object obj = m_instance;

                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    Settings[prop.Name.ToLower()] = prop.GetValue(obj, null).ToString();
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error("[ERROR] Hellion Extended Server[GameServerIni]: [SetSettings]" + ex.StackTrace);
            }

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
                    Log.Instance.Warn("GameServer.Ini wasn't found! Creating one from the GameServer_example.ini made on " +
                       File.GetLastWriteTime(m_exampleFileName));

                    File.Copy(m_exampleFileName, m_fileName);
                }

            }

            if (!File.Exists(m_originalFileName))
                File.Copy(m_fileName, m_originalFileName);
        }

        public bool Save(bool ignoreFileExists = false, bool backupIni = true)
        {
            try
            {
                var fileExists = File.Exists(m_fileName);

                if (ignoreFileExists)
                    fileExists = true;

                if (fileExists)
                {
                    //if (backupIni)
                        //if (!File.Exists(m_fileName + "hesbackup"))
                            //File.Copy(m_fileName, m_fileName + "hesbackup");

                    SetSettings();

                    using (StreamWriter file = new StreamWriter(m_fileName))
                        foreach (var entry in Settings)
                            file.WriteLine("{0}={1}", entry.Key, entry.Value);

                    return true;
                }
                else
                {
                    Log.Instance.Error("[ERROR] Hellion Extended Server[GameServerIni]: GAMESERVER.INI Does not exist!");
                    return false;
                }
            }
            catch (IOException)
            {
                Log.Instance.Warn("Could not save GameServer.Ini as the file is in use.");
            }
            catch (Exception ex)
            {
                Log.Instance.Error("[ERROR] Hellion Extended Server[GameServerIni]: Could not save config settings. EX: " + ex.ToString());
            }

            return true;
        }

        public void Load()
        {
            try
            {
                if (File.Exists(m_fileName))
                {
                    foreach (string line in File.ReadLines(m_fileName))
                    {
                        try
                        {
                            if (!line.StartsWith("#"))
                            {
                                var lineSetting = line.Split("=".ToCharArray(), 2);
                                this[lineSetting[0]] = lineSetting[1];
                                Settings.Add(lineSetting[0].ToLower(), lineSetting[1]);
                            }
                        }
                        catch { }
                    }
                }
                else
                {
                    Log.Instance.Warn("GameServer.Ini wasn't found! Creating a new one based on Defaults. ");
                    LoadDefaults();
                    Save(true, false);
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex.ToString());
            }


        }
        #endregion
    }
}