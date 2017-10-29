#pragma warning disable IDE1006

using System;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using HellionExtendedServer.Common;


namespace HellionExtendedServer.Common.Components
{
    public class GameServerIni
    {
        private GameServerIni m_instance;

        public string FileName = "GameServer.ini";
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

        private static int f_vessel_collision_damage_multiplier;        //(Default: 1)					Damage multiplier for vessel collision
        private static int f_vessel_explosion_radius_multiplier;        //(Default: 1)                    Damage radius for exploding vessels
        private static int f_vessel_explosion_damage_multiplier;        //(Default: 1)					Damage multiplier for exploding vessels
        private static float f_vessel_decay_rate;                         //(Default: 0.05)                 Vessel decay rate in HPs per second
        private static int f_activate_repair_point_chance_multiplier;   //(Default: 1)						If value is less then 1 (0-0,9) then chance for is lowered if number is higher than 1 chance is increased 
        //private static int f_doomed_ship_spawn_frequency;               //(Default: 10800)				Doomed outpust spawn interval, value in seconds // 10800 = 3 hours
        //private static int f_doomed_ship_timer_min;                     //(Default: 1800)					Doomed outpust minimum despawn time, value in seconds // 1800 = 30 minutes
        //private static int f_doomed_ship_timer_max;                     //(Default: 3600)					Doomed outpust maximum despawn time, value in seconds // 3600 = 1 hour
        //private static float f_doomed_ship_spawn_chance;                  //(Default: 0.5)					Doomed outpust spawn chance, value in % (0-1)
        private static bool f_spawn_manager_print_categories;            //(Default: false)				Spawn manager debug information
        private static bool f_spawn_manager_print_spawn_rules;           //(Default: false)				Spawn manager debug information
        private static bool f_spawn_manager_print_item_attach_points;    //(Default: false)				Spawn manager debug information
        private static bool f_spawn_manager_print_item_type_ids;         //(Default: false)				Spawn manager debug information


        private static int solarSystemTime;

        #endregion

        public GameServerIni()
        {
            m_instance = this;

            if (!File.Exists(FileName + ".original"))
                File.Copy(FileName, FileName + ".original");
        }

        #region ServerConfig Properties
        [ReadOnly(false)]
        [Description("The max amount of save files that can exist at once. (Set lower to save drive space)")]
        [Category("Server Settings")]
        [DisplayName("Max Save Files")]
        public int number_of_save_file
        {
            get { return maxSaveFileCount; }
            set { maxSaveFileCount = value; }
        }

        [ReadOnly(false)]
        [Description("How long the time in between saves in seconds. (Default: 900) (-1 = Off)")]
        [Category("Server Settings")]
        [DisplayName("Save Interval")]
        public int save_interval
        {
            get { return saveInterval; }
            set { saveInterval = value; }
        }

        [ReadOnly(false)]
        [Description("The time of the solar systems birth (Default: -1) (-1 = Random)")]
        [Category("Server Settings")]
        [DisplayName("Solar System Time")]
        public int solar_system_time
        {
            get { return solarSystemTime; }
            set { solarSystemTime = value; }
        }
 
        [ReadOnly(false)]
        [Description("The name of the server shown to clients in the server browser. (Default: Hellion Dedicated Server)")]
        [Category("Required Settings")]
        [DisplayName("Server Name")]
        public string server_name
        {
            get { return serverName; }
            set { serverName = value; }
        }

        [ReadOnly(false)]
        [Description("The password to the server. (Default: None)")]
        [Category("Required Settings")]
        [DisplayName("Server Password")]
        public string server_password
        {
            get { return serverPass; }
            set { serverPass = value; }
        }

        [ReadOnly(false)]
        [Description("The description to the server. (Default: None)")]
        [Category("Server Settings")]
        [DisplayName("Server Description")]
        public string description
        {
            get { return serverDescription; }
            set { serverDescription = value; }
        }

        [ReadOnly(false)]
        [Description("The port to use for the server browser status, ping, and player count. (Default: 5970) (Any)")]
        [Category("Required Settings")]
        [DisplayName("Status Port")]
        public int status_port
        {
            get { return statusPort; }
            set { statusPort = value; }
        }

        [ReadOnly(false)]
        [Description("The port your clients will use to connect to your server. (Default: 5969) (Any)")]
        [Category("Required Settings")]
        [DisplayName("Client Port")]
        public int game_client_port
        {
            get { return clientPort; }
            set { clientPort = value; }
        }

        [ReadOnly(false)]
        [Description("Maximum Players allowed on the server. (Default: 20) (Max: 100)")]
        [Category("Required Settings")]
        [DisplayName("Max Players")]
        public int max_players
        {
            get { return maxPlayers; }
            set { maxPlayers = value; }
        }

        [ReadOnly(false)]
        [Description("Automatic server restart, value in seconds (Default: -1) (-1 = Disabled)")]
        [Category("Server Settings")]
        [DisplayName("Spawn Manager Print Item Attach Points")]
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
        [Category("Server Settings")]
        [DisplayName("Spawn Manager Print Item Attach Points")]
        public bool print_debug_objects
        {
            get { return printDebugObjects; }
            set { printDebugObjects = value; }
        }//(Default: false)				Set to true for objects on server print

        [ReadOnly(false)]
        [Description("Damage multiplier for vessel collision (Default: 1)")]
        [Category("Game Settings")]
        [DisplayName("Vessel Collision Damage Multiplier")]
        public int vessel_collision_damage_multiplier
        {
            get { return f_vessel_collision_damage_multiplier; }
            set { f_vessel_collision_damage_multiplier = value; }
        }

        [ReadOnly(false)]
        [Description("Damage radius for exploding vessels (Default: 1)")]
        [Category("Game Settings")]
        [DisplayName("Vessel Explosion Radius Multiplier")]
        public int vessel_explosion_radius_multiplier        //(Default: 1)                    
        {
            get { return f_vessel_explosion_radius_multiplier; }
            set { f_vessel_explosion_radius_multiplier = value; }
        }
        

        [ReadOnly(false)]
        [Description("Damage multiplier for exploding vessels (Default: 1)")]
        [Category("Game Settings")]
        [DisplayName("Vessel Explosion Damage Multiplier")]
        public int vessel_explosion_damage_multiplier
        {
            get { return f_vessel_explosion_damage_multiplier; }
            set { f_vessel_explosion_damage_multiplier = value; }
        }
        //(Default: 1)					Damage multiplier for exploding vessels

        [ReadOnly(false)]
        [Description("Vessel decay rate in HPs per second (Default: 0.05) ")]
        [Category("Game Settings")]
        [DisplayName("Vessel Decay Rate")]
        public float vessel_decay_rate
        {
            get { return f_vessel_decay_rate; }
            set { f_vessel_decay_rate = value; }
        }
        //(Default: 0.05)                 Vessel decay rate in HPs per second

        [ReadOnly(false)]
        [Description("If value is less then 1 (0-0,9) then chance for is lowered if number is higher than 1 chance is increased  (Default: 1)")]
        [Category("Game Settings")]
        [DisplayName("Activate Repair Point Chance Multiplier")]
        public int activate_repair_point_chance_multiplier
        {
            get { return f_activate_repair_point_chance_multiplier; }
            set { f_activate_repair_point_chance_multiplier = value; }
        }
        //(Default: 1)						If value is less then 1 (0-0,9) then chance for is lowered if number is higher than 1 chance is increased 
       
        /*
        [ReadOnly(false)]
        [Description("Doomed outpost spawn interval, value in seconds // 10800 = 3 hours (Default: 10800)")]
        [Category("Game Settings")]
        [DisplayName("Doomed Ship Spawn Frequency")]
        public int doomed_ship_spawn_frequency
        {
            get { return f_doomed_ship_spawn_frequency; }
            set { f_doomed_ship_spawn_frequency = value; }
        }
        //(Default: 10800)				Doomed outpust spawn interval, value in seconds // 10800 = 3 hours
        
        [ReadOnly(false)]
        [Description("Doomed outpost minimum despawn time, value in seconds // 1800 = 30 minutes (Default: 1800)")]
        [Category("Game Settings")]
        [DisplayName("Doomed Ship Timer Min")]
        public int doomed_ship_timer_min
        {
            get { return f_doomed_ship_timer_min; }
            set { doomed_ship_timer_min = value; }
        }
        //(Default: 1800)					Doomed outpust minimum despawn time, value in seconds // 1800 = 30 minutes

        [ReadOnly(false)]
        [Description("Doomed outpost maximum despawn time, value in seconds // 3600 = 1 hour (Default: 3600)")]
        [Category("Game Settings")]
        [DisplayName("Doomed Ship Timer Max")]
        public int doomed_ship_timer_max
        {
            get { return f_doomed_ship_timer_max; }
            set { f_doomed_ship_timer_max = value; }
        }
        //(Default: 3600)					Doomed outpust maximum despawn time, value in seconds // 3600 = 1 hour

        [ReadOnly(false)]
        [Description("Doomed outpost spawn chance, value in % (0-1) (Default: 0.5)")]
        [Category("Game Settings")]
        [DisplayName("Doomed Ship Spawn Chance")]
        public float doomed_ship_spawn_chance
        {
            get { return f_doomed_ship_spawn_chance; }
            set { doomed_ship_spawn_chance = value; }
        }
        //(Default: 0.5)					Doomed outpust spawn chance, value in % (0-1)
        */

        [ReadOnly(false)]
        [Description("Spawn manager debug information (Default: false)")]
        [Category("Game Settings")]
        [DisplayName("Spawn Manager Print Categories")]
        public bool spawn_manager_print_categories
        {
            get { return f_spawn_manager_print_categories; }
            set { f_spawn_manager_print_categories = value; }
        }
        //(Default: false)				Spawn manager debug information

        [ReadOnly(false)]
        [Description("Spawn manager debug information (Default: false)")]
        [Category("Game Settings")]
        [DisplayName("Spawn Manager Print Spawn Rules")]
        public bool spawn_manager_print_spawn_rules
        {
            get { return f_spawn_manager_print_spawn_rules; }
            set { f_spawn_manager_print_spawn_rules = value; }
        }
        //(Default: false)				Spawn manager debug information

        [ReadOnly(false)]
        [Description("Spawn manager debug information (Default: false)")]
        [Category("Game Settings")]
        [DisplayName("Spawn Manager Print Item Attach Points")]
        public bool spawn_manager_print_item_attach_points
        {
            get { return f_spawn_manager_print_item_attach_points; }
            set { f_spawn_manager_print_item_attach_points = value; }
        }
        //(Default: false)				Spawn manager debug information

        [ReadOnly(false)]
        [Description("Spawn manager debug information (Default: false)")]
        [Category("Game Settings")]
        [DisplayName("Spawn Manager Print Item Attach Points")]
        public bool spawn_manager_print_item_type_ids
        {
            get { return f_spawn_manager_print_item_type_ids; }
            set { f_spawn_manager_print_item_type_ids = value; }
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
                maxPlayers = 100;
                maxSaveFileCount = 10;
                solarSystemTime = -1;
                saveInterval = 900;
                serverRestartTime = -1;
                f_vessel_collision_damage_multiplier = 1;
                f_vessel_explosion_radius_multiplier = 1;        //(Default: 1)                    Damage radius for exploding vessels
                f_vessel_explosion_damage_multiplier = 1;        //(Default: 1)					Damage multiplier for exploding vessels
                f_vessel_decay_rate = 0.05F;                         //(Default: 0.05)                 Vessel decay rate in HPs per second
                f_activate_repair_point_chance_multiplier = 1;   //(Default: 1)						If value is less then 1 (0-0,9) then chance for is lowered if number is higher than 1 chance is increased 
                //f_doomed_ship_spawn_frequency = 10800;               //(Default: 10800)				Doomed outpust spawn interval, value in seconds // 10800 = 3 hours
                //f_doomed_ship_timer_min = 1800;                     //(Default: 1800)					Doomed outpust minimum despawn time, value in seconds // 1800 = 30 minutes
                //f_doomed_ship_timer_max = 3600;                     //(Default: 3600)					Doomed outpust maximum despawn time, value in seconds // 3600 = 1 hour
                //f_doomed_ship_spawn_chance = 0.5F;                  //(Default: 0.5)					Doomed outpust spawn chance, value in % (0-1)
                f_spawn_manager_print_categories = false;            //(Default: false)				Spawn manager debug information
                f_spawn_manager_print_spawn_rules = false;           //(Default: false)				Spawn manager debug information
                f_spawn_manager_print_item_attach_points = false;    //(Default: false)				Spawn manager debug information
                f_spawn_manager_print_item_type_ids = false;         //(Default: false)				Spawn manager debug information



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

        public bool Save(bool ignoreFileExists = false, bool backupIni = true)
        {
            try
            {
                var fileExists = File.Exists(FileName);

                if (ignoreFileExists)
                    fileExists = true;

                if (fileExists)
                {
                    if (backupIni)
                        File.Copy(FileName, FileName + ".hesprevious", true);

                    SetSettings();

                    using (StreamWriter file = new StreamWriter(FileName))
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
                if (File.Exists(FileName))
                {
                   
                    foreach (string line in File.ReadLines(FileName))
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
