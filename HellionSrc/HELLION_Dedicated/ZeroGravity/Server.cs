// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Server
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using ZeroGravity.BulletPhysics;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;
using ZeroGravity.ShipComponents;
using ZeroGravity.Spawn;

namespace ZeroGravity
{
  public class Server
  {
    public static double RCS_THRUST_MULTIPLIER = 1.0;
    public static double RCS_ROTATION_MULTIPLIER = 1.0;
    public static double CELESTIAL_BODY_RADIUS_MULTIPLIER = 1.0;
    public static Properties Properties = (Properties) null;
    public static string ConfigDir = "";
    public static double PersistenceSaveInterval = 900.0;
    public static bool CleanStart = false;
    public static bool Restart = false;
    public static bool CleanRestart = false;
    public static double CelestialBodyDeathDistance = 10000.0;
    private static Server serverInstance = (Server) null;
    public static bool IsRunning = false;
    public static bool SavePersistenceDataOnShutdown = false;
    public static bool CheckInPassed = false;
    public static int MaxNumberOfSaveFiles = 10;
    public static double VesselDecayRateMultiplier = 1.0;
    public static double VesselExplosionRadiusMultiplier = 1.0;
    public static double VesselExplosionDamageMultiplier = 1.0;
    public static double VesselCollisionDamageMultiplier = 1.0;
    public static double ActivateRepairPointChanceMultiplier = 1.0;
    public static double ServerRestartTimeSec = -1.0;
    public static Server.ServerSetupType SetupType = Server.ServerSetupType.Default;
    public static double MovementMessageSendInterval = 0.1;
    public static bool ForceMovementMessageSend = false;
    private static double _movementMessageTimer = 0.0;
    public static uint NetworkDataHash = ClassHasher.GetClassHashCode(typeof (NetworkData), (string) null);
    public static uint SceneDataHash = ClassHasher.GetClassHashCode(typeof (ISceneData), (string) null);
    public static uint CombinedHash = Server.NetworkDataHash * Server.SceneDataHash;
    public static List<GameScenes.SceneID> RandomShipSpawnSceneIDs = new List<GameScenes.SceneID>()
    {
      GameScenes.SceneID.AltCorp_CorridorModule,
      GameScenes.SceneID.AltCorp_CorridorIntersectionModule,
      GameScenes.SceneID.AltCorp_Corridor45TurnModule,
      GameScenes.SceneID.AltCorp_Shuttle_SARA,
      GameScenes.SceneID.ALtCorp_PowerSupply_Module,
      GameScenes.SceneID.AltCorp_LifeSupportModule,
      GameScenes.SceneID.AltCorp_Cargo_Module,
      GameScenes.SceneID.AltCorp_CorridorVertical,
      GameScenes.SceneID.AltCorp_Command_Module,
      GameScenes.SceneID.AltCorp_Corridor45TurnRightModule,
      GameScenes.SceneID.AltCorp_StartingModule,
      GameScenes.SceneID.AltCorp_AirLock,
      GameScenes.SceneID.Generic_Debris_JuncRoom001,
      GameScenes.SceneID.Generic_Debris_JuncRoom002,
      GameScenes.SceneID.Generic_Debris_Corridor001,
      GameScenes.SceneID.Generic_Debris_Corridor002,
      GameScenes.SceneID.AltCorp_DockableContainer
    };
    public static double SpawnPointInviteTimer = 300.0;
    public static AutoResetEvent MainLoopEnded = new AutoResetEvent(false);
    private Dictionary<long, SpaceObject> _objects = new Dictionary<long, SpaceObject>();
    private Dictionary<long, Player> _players = new Dictionary<long, Player>();
    private Dictionary<long, SpaceObjectVessel> _vessels = new Dictionary<long, SpaceObjectVessel>();
    private List<UpdateTimer> timersToRemove = new List<UpdateTimer>();
    private List<UpdateTimer> timers = new List<UpdateTimer>();
    public SolarSystem SolarSystem = (SolarSystem) null;
    private long numberOfTicks = 64;
    public bool WorldInitialized = false;
    public DateTime ServerStartTime = DateTime.UtcNow;
    private float solarSystemStartTime = -1f;
    public string ServerPassword = "";
    public string ServerName = "Hellion Game Server";
    public string Description = "";
    public string NotificationEMail = "";
    public int MaxPlayers = 100;
    private IPAddressRange[] AdminIPAddressRanges = new IPAddressRange[0];
    private double timeToRestart = 1800.0;
    public List<DeathMatchArenaController> DeathMatchArenaControllers = new List<DeathMatchArenaController>();
    public DoomedShipController DoomedShipController = new DoomedShipController();
    public DateTime LastChatMessageTime = DateTime.UtcNow;
    private double persistenceSaveTimer = 0.0;
    private bool printDebugObjects = false;
    private Thread updateShipSystemsThread = (Thread) null;
    public List<Server.DynamicObjectsRespawn> DynamicObjectsRespawnList = new List<Server.DynamicObjectsRespawn>();
    public List<Player> PlayersMarkedToDestroy = new List<Player>();
    private double tickMilliseconds = 0.0;
    private Dictionary<string, Server.SpawnPointInviteData> SpawnPointInvites = new Dictionary<string, Server.SpawnPointInviteData>();
    public NetworkController NetworkController;
    public static DateTime restartTime;
    public static volatile int MainThreadID;
    public BulletPhysicsController PhysicsController;
    private bool manualSave;
    public double DeltaTime;
    private DateTime lastTime;

    public bool DoesObjectExist(long guid)
    {
      return this._objects.ContainsKey(guid);
    }

    public SpaceObject GetObject(long guid)
    {
      if (this._objects.ContainsKey(guid))
        return this._objects[guid];
      return (SpaceObject) null;
    }

    public SpaceObjectTransferable GetTransferable(long guid)
    {
      if (this._objects.ContainsKey(guid) && this._objects[guid] is SpaceObjectTransferable)
        return this._objects[guid] as SpaceObjectTransferable;
      return (SpaceObjectTransferable) null;
    }

    public Player GetPlayer(long guid)
    {
      Player player = (Player) null;
      this._players.TryGetValue(guid, out player);
      return player;
    }

    public Player GetPlayerFromSteamID(string steamID)
    {
      return this.GetPlayer(GUIDFactory.SteamIdToGuid(steamID));
    }

    public List<SpaceObjectVessel> GetVesselsInRange(SpaceObjectVessel myShip, double distance, GameScenes.SceneID type)
    {
      List<SpaceObjectVessel> spaceObjectVesselList = new List<SpaceObjectVessel>();
      foreach (KeyValuePair<long, SpaceObjectVessel> vessel in this._vessels)
      {
        if (vessel.Value.DockedToMainVessel == null && vessel.Value.SceneID == type && Vector3D.Distance(vessel.Value.Orbit.Position, myShip.Orbit.Position) < distance)
          spaceObjectVesselList.Add(vessel.Value);
        else if (vessel.Value.DockedToMainVessel != null && vessel.Value.SceneID == type && Vector3D.Distance(vessel.Value.DockedToMainVessel.Orbit.Position, myShip.Orbit.Position) < distance)
          spaceObjectVesselList.Add(vessel.Value);
      }
      return spaceObjectVesselList;
    }

    public SpaceObjectVessel GetVessel(long guid)
    {
      if (this._vessels.ContainsKey(guid))
        return this._vessels[guid];
      return (SpaceObjectVessel) null;
    }

    public Item GetItem(long guid)
    {
      if (this._objects.ContainsKey(guid) && this._objects[guid] is DynamicObject)
        return (this._objects[guid] as DynamicObject).Item;
      return (Item) null;
    }

    public void Add(Player player)
    {
      if (!this._players.ContainsKey(player.GUID))
        this._players.Add(player.GUID, player);
      if (this._objects.ContainsKey(player.FakeGuid))
        return;
      this._objects.Add(player.FakeGuid, (SpaceObject) player);
    }

    public void Add(SpaceObjectVessel vessel)
    {
      if (!this._vessels.ContainsKey(vessel.GUID))
        this._vessels.Add(vessel.GUID, vessel);
      if (this._objects.ContainsKey(vessel.GUID))
        return;
      this._objects.Add(vessel.GUID, (SpaceObject) vessel);
    }

    public void Add(DynamicObject dobj)
    {
      if (this._objects.ContainsKey(dobj.GUID))
        return;
      this._objects.Add(dobj.GUID, (SpaceObject) dobj);
    }

    public void Add(Corpse corpse)
    {
      if (this._objects.ContainsKey(corpse.GUID))
        return;
      this._objects.Add(corpse.GUID, (SpaceObject) corpse);
    }

    public void Remove(Player player)
    {
      if (this._players.ContainsKey(player.GUID))
        this._players.Remove(player.GUID);
      if (!this._objects.ContainsKey(player.FakeGuid))
        return;
      this._objects.Remove(player.FakeGuid);
    }

    public void Remove(SpaceObjectVessel vessel)
    {
      if (this._vessels.ContainsKey(vessel.GUID))
        this._vessels.Remove(vessel.GUID);
      if (!this._objects.ContainsKey(vessel.GUID))
        return;
      this._objects.Remove(vessel.GUID);
    }

    public void Remove(DynamicObject dobj)
    {
      if (!this._objects.ContainsKey(dobj.GUID))
        return;
      this._objects.Remove(dobj.GUID);
    }

    public void Remove(Corpse corpse)
    {
      if (!this._objects.ContainsKey(corpse.GUID))
        return;
      this._objects.Remove(corpse.GUID);
    }

    public Dictionary<long, SpaceObjectVessel>.ValueCollection AllVessels
    {
      get
      {
        return this._vessels.Values;
      }
    }

    public Dictionary<long, Player>.ValueCollection AllPlayers
    {
      get
      {
        return this._players.Values;
      }
    }

    public static Server Instance
    {
      get
      {
        return Server.serverInstance;
      }
    }

    public TimeSpan RunTime
    {
      get
      {
        return DateTime.UtcNow - this.ServerStartTime;
      }
    }

    public Server()
    {
      Server.MainThreadID = Thread.CurrentThread.ManagedThreadId;
      Server.IsRunning = true;
      Server.serverInstance = this;
      this.PhysicsController = new BulletPhysicsController();
      this.NetworkController = new NetworkController();
      this.SolarSystem = new SolarSystem();
      this.LoadServerSettings();
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      Thread.Sleep(1);
      stopwatch.Stop();
      long num = (long) (1000.0 / stopwatch.Elapsed.TotalMilliseconds);
      Dbg.UnformattedMessage(string.Format("==============================================================================\r\n\tServer name: {5}\r\n\tServer ID: {1}\r\n\tGame port: {6}\r\n\tStatus port: {7}\r\n\tStart date: {0}\r\n\tServer ticks: {2}{4}\r\n\tMax server ticks (not precise): {3}\r\n==============================================================================", (object) DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.ffff"), (object) (this.NetworkController.ServerID <= 0L ? "Not yet assigned" : string.Concat((object) this.NetworkController.ServerID)), (object) this.numberOfTicks, (object) num, (object) (this.numberOfTicks > num ? " WARNING: Server ticks is larger than max tick" : ""), (object) this.ServerName, (object) this.NetworkController.GameClientPort, (object) this.NetworkController.StatusPort));
      StaticData.LoadData();
    }

    private void LoadServerSettings()
    {
      try
      {
        this.NetworkController.ServerID = long.Parse(System.IO.File.ReadAllText(Server.ConfigDir + "ServerID.txt").Trim());
      }
      catch
      {
      }
      Server.Properties.GetProperty<long>("server_tick_count", ref this.numberOfTicks);
      Server.Properties.GetProperty<int>("game_client_port", ref this.NetworkController.GameClientPort);
      Server.Properties.GetProperty<int>("status_port", ref this.NetworkController.StatusPort);
      Server.Properties.GetProperty<string>("main_server_ip", ref this.NetworkController.MainServerAddres);
      Server.Properties.GetProperty<int>("main_server_port", ref this.NetworkController.MainServerPort);
      Server.Properties.GetProperty<Server.ServerSetupType>("setup_type", ref Server.SetupType);
      Server.Properties.GetProperty<double>("movement_send_interval", ref Server.MovementMessageSendInterval);
      if (Server.MovementMessageSendInterval <= 1.40129846432482E-45)
        Server.MovementMessageSendInterval = 0.1;
      Server.Properties.GetProperty<float>("solar_system_time", ref this.solarSystemStartTime);
      Server.Properties.GetProperty<double>("save_interval", ref Server.PersistenceSaveInterval);
      Server.Properties.GetProperty<string>("server_password", ref this.ServerPassword);
      Server.Properties.GetProperty<string>("server_name", ref this.ServerName);
      Server.Properties.GetProperty<string>("description", ref this.Description);
      if (this.Description.Length > 500)
      {
        this.Description = this.Description.Substring(0, 497) + "...";
        Dbg.Error("Server description too long. Maximum length is 500 characters.");
      }
      Server.Properties.GetProperty<string>("notification_email_address", ref this.NotificationEMail);
      if (this.NotificationEMail.Length > 254)
      {
        this.NotificationEMail = "";
        Dbg.Error("Notification e-mail address too long. Maximum length is 254 characters.");
      }
      Server.Properties.GetProperty<int>("max_players", ref this.MaxPlayers);
      Server.Properties.GetProperty<int>("number_of_save_files", ref Server.MaxNumberOfSaveFiles);
      Server.MaxNumberOfSaveFiles = MathHelper.Clamp(Server.MaxNumberOfSaveFiles, 1, 100);
      Server.Properties.GetProperty<double>("vessel_decay_rate_multiplier", ref Server.VesselDecayRateMultiplier);
      Server.Properties.GetProperty<double>("vessel_explosion_radius_multiplier", ref Server.VesselExplosionRadiusMultiplier);
      Server.Properties.GetProperty<double>("vessel_explosion_damage_multiplier", ref Server.VesselExplosionDamageMultiplier);
      Server.Properties.GetProperty<double>("vessel_collision_damage_multiplier", ref Server.VesselCollisionDamageMultiplier);
      Server.Properties.GetProperty<double>("activate_repair_point_chance_multiplier", ref Server.ActivateRepairPointChanceMultiplier);
      Server.Properties.GetProperty<double>("server_restart_time", ref Server.ServerRestartTimeSec);
      double arenaRescueTime = SpaceObjectVessel.ArenaRescueTime;
      Server.Properties.GetProperty<double>("arena_ship_respawn_timer", ref arenaRescueTime);
      SpaceObjectVessel.ArenaRescueTime = arenaRescueTime;
      Server.Properties.GetProperty<bool>("print_debug_objects", ref this.printDebugObjects);
      Server.Properties.GetProperty<bool>("spawn_manager_print_categories", ref SpawnManager.Settings.PrintCategories);
      Server.Properties.GetProperty<bool>("spawn_manager_print_spawn_rules", ref SpawnManager.Settings.PrintSpawnRules);
      Server.Properties.GetProperty<bool>("spawn_manager_print_item_attach_points", ref SpawnManager.Settings.PrintItemAttachPoints);
      Server.Properties.GetProperty<bool>("spawn_manager_print_item_type_ids", ref SpawnManager.Settings.PrintItemTypeIDs);
    }

    public void LoginPlayer(long guid, string steamId, CharacterData characterData)
    {
      if (!this.WorldInitialized)
        this.InitializeWorld();
      if (this._players.ContainsKey(guid))
      {
        this.NetworkController.ConnectPlayer(this._players[guid], true);
      }
      else
      {
        Player player = new Player(guid, Vector3D.Zero, QuaternionD.Identity, characterData.Name, steamId, characterData.Gender, characterData.HeadType, characterData.HairType, true);
        this.Add(player);
        this.NetworkController.ConnectPlayer(player, false);
      }
    }

    private void ResetSpawnPointsForPlayer(Player pl, Ship skipShip)
    {
      if (!pl.SteamId.IsNullOrEmpty() && this.SpawnPointInvites.ContainsKey(pl.SteamId))
        this.ClearSpawnPointInvitation(pl.SteamId);
      foreach (SpaceObjectVessel allVessel in this.AllVessels)
      {
        if (allVessel is Ship && allVessel != skipShip)
          (allVessel as Ship).ResetSpawnPointsForPlayer(pl, true);
      }
    }

    private bool AddPlayerToShip(Player pl, long shipID, GameScenes.SceneID shipItemID)
    {
      if (shipID == 0L)
        return false;
      Ship ship = (Ship) null;
      ShipSpawnPoint spawnPoint = (ShipSpawnPoint) null;
      if (shipID < 0L)
      {
        if (shipID == -1L)
        {
          this.ResetSpawnPointsForPlayer(pl, (Ship) null);
          ship = SpawnManager.SpawnStartingSetup(this);
          if (ship != null)
            spawnPoint = SpawnManager.SetStartingSetupSpawnPoints((SpaceObjectVessel) ship, pl);
          if (ship == null || spawnPoint == null)
            Dbg.Error((object) "FAILED TO SPAWN STARTING MODULE", (object) pl.GUID, (object) (ship != null ? ship.GUID : -1L));
        }
        else if (shipID == -2L)
        {
          if (pl.AuthorizedSpawnPoint == null)
            return false;
          ship = pl.AuthorizedSpawnPoint.Ship;
          spawnPoint = pl.AuthorizedSpawnPoint;
        }
      }
      else if (shipID > 0L && this.SpawnPointInvites.ContainsKey(pl.SteamId) && this.SpawnPointInvites[pl.SteamId].SpawnPoint.Ship.GUID == shipID)
      {
        ship = this.GetVessel(shipID) as Ship;
        spawnPoint = ship != null ? this.SpawnPointInvites[pl.SteamId].SpawnPoint : (ShipSpawnPoint) null;
        if (spawnPoint != null)
          this.ResetSpawnPointsForPlayer(pl, (Ship) null);
      }
      if (ship == null || spawnPoint == null)
        return false;
      spawnPoint.Player = pl;
      if (spawnPoint.Executer != null)
        spawnPoint.IsPlayerInSpawnPoint = true;
      spawnPoint.AuthorizePlayerToSpawnPoint(pl, true);
      pl.Parent = (SpaceObject) ship;
      pl.SetSpawnPoint(spawnPoint);
      pl.SubscribeTo((SpaceObject) ship);
      return true;
    }

    public void CheckPosition(Player player)
    {
    }

    public void RemovePlayer(Player player)
    {
      if (player.Parent != null)
      {
        if (player.Parent.ObjectType == SpaceObjectType.PlayerPivot)
          (player.Parent as Pivot).Destroy();
        if (player.Parent.ObjectType == SpaceObjectType.Ship || player.Parent.ObjectType == SpaceObjectType.Asteroid)
          (player.Parent as SpaceObjectVessel).RemovePlayerFromCrew(player, true);
        else if (player.Parent.ObjectType != SpaceObjectType.Station)
          ;
      }
      this.Remove(player);
    }

    public List<SpawnPointDetails> GetAvailableSpawnPoints(Player pl)
    {
      List<SpawnPointDetails> spawnPointDetailsList = new List<SpawnPointDetails>();
      if (pl.SteamId != null && this.SpawnPointInvites.ContainsKey(pl.SteamId))
      {
        ShipSpawnPoint spawnPoint = this.SpawnPointInvites[pl.SteamId].SpawnPoint;
        SpawnPointDetails spawnPointDetails = new SpawnPointDetails();
        spawnPointDetails.Type = SpawnPointLocationType.Ship;
        spawnPointDetails.Name = spawnPoint.Ship.FullName;
        spawnPointDetails.IsPartOfCrew = false;
        spawnPointDetails.SpawnPointParentID = spawnPoint.Ship.GUID;
        spawnPointDetails.PlayersOnShip = new List<string>();
        foreach (Player player in spawnPoint.Ship.VesselCrew)
          spawnPointDetails.PlayersOnShip.Add(player.Name);
        spawnPointDetailsList.Add(spawnPointDetails);
      }
      return spawnPointDetailsList;
    }

    private void LatencyTestListener(NetworkData data)
    {
      this.NetworkController.SendToGameClient(data.Sender, data);
    }

    private void Start()
    {
      this.NetworkController.EventSystem.AddListener(typeof (PlayerSpawnRequest), new EventSystem.NetworkDataDelegate(this.PlayerSpawnRequestListener));
      this.NetworkController.EventSystem.AddListener(typeof (PlayerRespawnRequest), new EventSystem.NetworkDataDelegate(this.PlayerRespawnRequestListener));
      this.NetworkController.EventSystem.AddListener(typeof (SpawnObjectsRequest), new EventSystem.NetworkDataDelegate(this.SpawnObjectsRequestListener));
      this.NetworkController.EventSystem.AddListener(typeof (SubscribeToObjectsRequest), new EventSystem.NetworkDataDelegate(this.SubscribeToSpaceObjectListener));
      this.NetworkController.EventSystem.AddListener(typeof (UnsubscribeFromObjectsRequest), new EventSystem.NetworkDataDelegate(this.UnsubscribeFromSpaceObjectListener));
      this.NetworkController.EventSystem.AddListener(typeof (TextChatMessage), new EventSystem.NetworkDataDelegate(this.TextChatMessageListener));
      this.NetworkController.EventSystem.AddListener(typeof (TransferResourceMessage), new EventSystem.NetworkDataDelegate(this.TransferResourcesMessageListener));
      this.NetworkController.EventSystem.AddListener(typeof (RefineResourceMessage), new EventSystem.NetworkDataDelegate(this.RefineResourceMessageListener));
      this.NetworkController.EventSystem.AddListener(typeof (FabricateItemMessage), new EventSystem.NetworkDataDelegate(this.FabricateItemMessageListener));
      this.NetworkController.EventSystem.AddListener(typeof (CheckInResponse), new EventSystem.NetworkDataDelegate(this.CheckInResponseListener));
      this.NetworkController.EventSystem.AddListener(typeof (PlayersOnServerRequest), new EventSystem.NetworkDataDelegate(this.PlayersOnServerRequestListener));
      this.NetworkController.EventSystem.AddListener(typeof (AvailableSpawnPointsRequest), new EventSystem.NetworkDataDelegate(this.AvailableSpawnPointsRequestListener));
      this.NetworkController.EventSystem.AddListener(typeof (RepairItemMessage), new EventSystem.NetworkDataDelegate(this.RepairMessageListener));
      this.NetworkController.EventSystem.AddListener(typeof (RepairVesselMessage), new EventSystem.NetworkDataDelegate(this.RepairMessageListener));
      this.NetworkController.EventSystem.AddListener(typeof (HurtPlayerMessage), new EventSystem.NetworkDataDelegate(this.HurtPlayerMessageListener));
      this.NetworkController.EventSystem.AddListener(typeof (LatencyTestMessage), new EventSystem.NetworkDataDelegate(this.LatencyTestListener));
      this.NetworkController.EventSystem.AddListener(typeof (SaveGameMessage), new EventSystem.NetworkDataDelegate(this.SaveGameMessageListener));
      this.NetworkController.EventSystem.AddListener(typeof (NameTagMessage), new EventSystem.NetworkDataDelegate(this.NameTagMessageListener));
      NetworkController networkController = this.NetworkController;
      CheckInRequest checkInRequest = new CheckInRequest();
      checkInRequest.ServerID = this.NetworkController.ServerID;
      checkInRequest.ServerName = this.ServerName;
      checkInRequest.GamePort = this.NetworkController.GameClientPort;
      checkInRequest.StatusPort = this.NetworkController.StatusPort;
      int num1 = !this.ServerPassword.IsNullOrEmpty() ? 1 : 0;
      checkInRequest.Private = num1 != 0;
      int combinedHash = (int) Server.CombinedHash;
      checkInRequest.ServerHash = (uint) combinedHash;
      int num2 = Server.CleanStart ? 1 : 0;
      checkInRequest.CleanStart = num2 != 0;
      networkController.SendToMainServer((NetworkData) checkInRequest);
    }

    public void TransferResourcesMessageListener(NetworkData data)
    {
      TransferResourceMessage trm = data as TransferResourceMessage;
      SpaceObjectVessel vessel1 = Server.Instance.GetVessel(trm.FromVesselGuid);
      SpaceObjectVessel vessel2 = Server.Instance.GetVessel(trm.ToVesselGuid);
      if (trm.FromLocationType == ResourceLocationType.ResourcesTransferPoint)
      {
        ICargo toCargo = (ICargo) null;
        if (trm.ToLocationType == ResourceLocationType.CargoBay)
          toCargo = (ICargo) vessel2.CargoBays.Find((Predicate<CargoBay>) (m => m.InSceneID == trm.ToInSceneID));
        else if (trm.ToLocationType == ResourceLocationType.ResourceTank)
          toCargo = (ICargo) vessel2.MainDistributionManager.GetResourceContainer(new VesselObjectID(trm.ToVesselGuid, trm.ToInSceneID));
        DynamicObject dynamicObject = vessel1.DynamicObjects.InnerList.Find((Predicate<DynamicObject>) (m =>
        {
          if (m.Item != null && m.Item.AttachPointKey != null)
            return m.Item.AttachPointKey.InSceneID == trm.FromInSceneID;
          return false;
        }));
        if (dynamicObject == null || dynamicObject.Item == null || !(dynamicObject.Item is ICargo) || toCargo == null)
          return;
        if (trm.ToLocationType != ResourceLocationType.None)
          this.TransferResources(dynamicObject.Item as ICargo, trm.FromCompartmentID, toCargo, trm.ToCompartmentID, trm.ResourceType, trm.Quantity);
        else
          this.VentResources(dynamicObject.Item as ICargo, trm.FromCompartmentID, trm.ResourceType, trm.Quantity);
      }
      else if (trm.ToLocationType == ResourceLocationType.ResourcesTransferPoint)
      {
        ICargo fromCargo = (ICargo) null;
        if (trm.FromLocationType == ResourceLocationType.CargoBay)
          fromCargo = (ICargo) vessel1.CargoBays.Find((Predicate<CargoBay>) (m => m.InSceneID == trm.FromInSceneID));
        else if (trm.FromLocationType == ResourceLocationType.ResourceTank)
          fromCargo = (ICargo) vessel1.MainDistributionManager.GetResourceContainer(new VesselObjectID(trm.FromVesselGuid, trm.FromInSceneID));
        DynamicObject dynamicObject = vessel2.DynamicObjects.InnerList.Find((Predicate<DynamicObject>) (m =>
        {
          if (m.Item != null && m.Item.AttachPointKey != null)
            return m.Item.AttachPointKey.InSceneID == trm.ToInSceneID;
          return false;
        }));
        if (dynamicObject == null || dynamicObject.Item == null || !(dynamicObject.Item is ICargo) || fromCargo == null)
          return;
        if (trm.ToLocationType != ResourceLocationType.None)
          this.TransferResources(fromCargo, trm.FromCompartmentID, dynamicObject.Item as ICargo, trm.ToCompartmentID, trm.ResourceType, trm.Quantity);
        else
          this.VentResources(dynamicObject.Item as ICargo, trm.FromCompartmentID, trm.ResourceType, trm.Quantity);
      }
      else
      {
        ICargo fromCargo = (ICargo) null;
        if (trm.FromLocationType == ResourceLocationType.CargoBay)
          fromCargo = (ICargo) vessel1.CargoBays.Find((Predicate<CargoBay>) (m => m.InSceneID == trm.FromInSceneID));
        else if (trm.FromLocationType == ResourceLocationType.ResourceTank)
          fromCargo = (ICargo) vessel1.MainDistributionManager.GetResourceContainer(new VesselObjectID(trm.FromVesselGuid, trm.FromInSceneID));
        ICargo toCargo = (ICargo) null;
        if (trm.ToLocationType == ResourceLocationType.CargoBay)
          toCargo = (ICargo) vessel2.CargoBays.Find((Predicate<CargoBay>) (m => m.InSceneID == trm.ToInSceneID));
        else if (trm.ToLocationType == ResourceLocationType.ResourceTank)
          toCargo = (ICargo) vessel2.MainDistributionManager.GetResourceContainer(new VesselObjectID(trm.ToVesselGuid, trm.ToInSceneID));
        else if (trm.ToLocationType == ResourceLocationType.None)
        {
          this.VentResources(fromCargo, trm.FromCompartmentID, trm.ResourceType, trm.Quantity);
          return;
        }
        if (fromCargo == null || toCargo == null)
          return;
        this.TransferResources(fromCargo, trm.FromCompartmentID, toCargo, trm.ToCompartmentID, trm.ResourceType, trm.Quantity);
      }
    }

    public void TransferResources(ICargo fromCargo, short fromCompartmentID, ICargo toCargo, short toCompartmentID, ResourceType resourceType, float quantity)
    {
      CargoCompartmentData cargoCompartmentData1 = fromCargo.Compartments.Find((Predicate<CargoCompartmentData>) (m => (int) m.ID == (int) fromCompartmentID));
      CargoCompartmentData cargoCompartmentData2 = toCargo.Compartments.Find((Predicate<CargoCompartmentData>) (m => (int) m.ID == (int) toCompartmentID));
      if (cargoCompartmentData1 == null || cargoCompartmentData2 == null || cargoCompartmentData2.AllowedResources.Count > 0 && !cargoCompartmentData2.AllowedResources.Contains(resourceType) || cargoCompartmentData2.AllowOnlyOneType && cargoCompartmentData2.Resources.Count > 0 && cargoCompartmentData2.Resources[0].ResourceType != resourceType)
        return;
      float quantity1 = quantity;
      ZeroGravity.Data.CargoResourceData cargoResourceData = cargoCompartmentData1.Resources.Find((Predicate<ZeroGravity.Data.CargoResourceData>) (m =>
      {
        if (m != null)
          return m.ResourceType == resourceType;
        return false;
      }));
      if (cargoResourceData == null)
        return;
      float quantity2 = cargoResourceData.Quantity;
      if ((double) quantity2 <= 1.40129846432482E-45)
        return;
      if ((double) quantity1 > (double) quantity2)
        quantity1 = quantity2;
      float num1 = cargoCompartmentData2.Capacity - cargoCompartmentData2.Resources.Sum<ZeroGravity.Data.CargoResourceData>((Func<ZeroGravity.Data.CargoResourceData, float>) (m => m.Quantity));
      if ((double) quantity1 > (double) num1)
        quantity1 = num1;
      double num2 = (double) fromCargo.ChangeQuantityBy((int) fromCompartmentID, resourceType, -quantity1, false);
      double num3 = (double) toCargo.ChangeQuantityBy((int) toCompartmentID, resourceType, quantity1, false);
    }

    private void VentResources(ICargo fromCargo, short fromCompartmentID, ResourceType resourceType, float quantity)
    {
      CargoCompartmentData cargoCompartmentData = fromCargo.Compartments.Find((Predicate<CargoCompartmentData>) (m => (int) m.ID == (int) fromCompartmentID));
      if (cargoCompartmentData == null)
        return;
      float num1 = quantity;
      ZeroGravity.Data.CargoResourceData cargoResourceData = cargoCompartmentData.Resources.Find((Predicate<ZeroGravity.Data.CargoResourceData>) (m =>
      {
        if (m != null)
          return m.ResourceType == resourceType;
        return false;
      }));
      if (cargoResourceData == null)
        return;
      float quantity1 = cargoResourceData.Quantity;
      if ((double) num1 > (double) quantity1)
        num1 = quantity1;
      double num2 = (double) fromCargo.ChangeQuantityBy((int) fromCompartmentID, resourceType, -num1, false);
    }

    public void RefineResourceMessageListener(NetworkData data)
    {
      RefineResourceMessage rrm = data as RefineResourceMessage;
      SpaceObjectVessel vessel1 = Server.Instance.GetVessel(rrm.FromVesselGuid);
      SpaceObjectVessel vessel2 = Server.Instance.GetVessel(rrm.ToVesselGuid);
      ICargo fromCargo = (ICargo) null;
      if (rrm.FromLocationType == ResourceLocationType.ResourcesTransferPoint)
      {
        DynamicObject dynamicObject = vessel1.DynamicObjects.InnerList.Find((Predicate<DynamicObject>) (m =>
        {
          if (m.Item != null && m.Item.AttachPointKey != null)
            return m.Item.AttachPointKey.InSceneID == rrm.FromInSceneID;
          return false;
        }));
        if (dynamicObject == null || dynamicObject.Item == null || !(dynamicObject.Item is ICargo))
          return;
        fromCargo = dynamicObject.Item as ICargo;
      }
      else if (rrm.FromLocationType == ResourceLocationType.CargoBay)
        fromCargo = (ICargo) vessel1.CargoBays.Find((Predicate<CargoBay>) (m => m.InSceneID == rrm.FromInSceneID));
      vessel2.DistributionManager.Refine(rrm, fromCargo);
    }

    public void FabricateItemMessageListener(NetworkData data)
    {
      FabricateItemMessage fabricateItemMessage = data as FabricateItemMessage;
      if (!(this.GetPlayer(fabricateItemMessage.Sender).Parent is Ship))
        return;
      Ship parent = this.GetPlayer(fabricateItemMessage.Sender).Parent as Ship;
      if (parent.CargoBays.Count > 0)
        parent.DistributionManager.Fabricator.FabricateItem((ItemType) fabricateItemMessage.ItemType.Value, parent.CargoBays[0]);
    }

    public void RepairMessageListener(NetworkData data)
    {
      Player player = this.GetPlayer(data.Sender);
      if (player == null)
        return;
      Item slotItem = player.PlayerInventory.HandsSlot.SlotItem;
      if (slotItem is RepairTool)
      {
        if (data is RepairVesselMessage)
        {
          RepairVesselMessage repairVesselMessage = data as RepairVesselMessage;
          (slotItem as RepairTool).RepairVessel(repairVesselMessage.ID);
        }
        else if (data is RepairItemMessage)
        {
          RepairItemMessage repairItemMessage = data as RepairItemMessage;
          RepairTool repairTool = slotItem as RepairTool;
          if (repairItemMessage.GUID > 0L)
            repairTool.RepairItem(repairItemMessage.GUID);
          else
            repairTool.ConsumeFuel(repairTool.RepairAmount * repairTool.FuelConsumption);
        }
      }
    }

    public void HurtPlayerMessageListener(NetworkData data)
    {
      this.GetPlayer(data.Sender).Stats.TakeDammage(0.0f, new Vector3D?(), (data as HurtPlayerMessage).Damage, 0.0f, 0.0f, 0.0f, 0.0f);
    }

    public void TextChatMessageListener(NetworkData data)
    {
      TextChatMessage textChatMessage = data as TextChatMessage;
      Player player1 = this._players[textChatMessage.Sender];
      textChatMessage.GUID = player1.FakeGuid;
      textChatMessage.Name = player1.Name;
      if (textChatMessage.MessageText.Length > 250)
        textChatMessage.MessageText = textChatMessage.MessageText.Substring(0, 250);
      if (textChatMessage.Local)
      {
        Vector3D vector3D = player1.Parent.Position + player1.Position;
        foreach (Player player2 in this._players.Values)
        {
          if ((player2.Parent.Position + player2.Position - vector3D).SqrMagnitude < 1000000.0 && player2 != player1)
            this.NetworkController.SendToGameClient(player2.GUID, (NetworkData) textChatMessage);
        }
      }
      else
        this.NetworkController.SendToAllClients((NetworkData) textChatMessage, textChatMessage.Sender);
    }

    public TextChatMessage SendSystemMessage(SystemMessagesTypes type, Ship sh)
    {
      TextChatMessage textChatMessage = new TextChatMessage();
      textChatMessage.GUID = -1L;
      textChatMessage.Name = "System";
      textChatMessage.MessageType = new SystemMessagesTypes?(type);
      textChatMessage.MessageText = "";
      if (type == SystemMessagesTypes.DoomedOutpostSpawned)
      {
        if (sh.Orbit.Parent != null && sh.Orbit.Parent.CelestialBody != null)
        {
          TimeSpan timeSpan = new TimeSpan(0, 0, (int) sh.TimeToLive);
          textChatMessage.MessageParam = new string[3]
          {
            sh.VesselData.VesselRegistration,
            ((CelestialBodyGUID) sh.Orbit.Parent.CelestialBody.GUID).ToString(),
            timeSpan.ToString()
          };
        }
      }
      else if (type == SystemMessagesTypes.ShipWillArriveIn)
      {
        double num = sh.RespawnTimeForShip - sh.timePassedSinceRequest;
        if (num > 0.0)
        {
          TimeSpan timeSpan = new TimeSpan(0, 0, (int) num);
          textChatMessage.MessageParam = new string[1]
          {
            timeSpan.ToString()
          };
        }
        else
          Dbg.Error("Strange things happened with spawn ship on request timers");
      }
      else if (type == SystemMessagesTypes.ShipTimerAllreadyStarted || type == SystemMessagesTypes.ShipInRange)
        textChatMessage.MessageParam = (string[]) null;
      else if (type == SystemMessagesTypes.RestartServerTime)
      {
        string str = this.timeToRestart > 10.0 ? (this.timeToRestart / 60.0).ToString() : this.timeToRestart.ToString();
        textChatMessage.MessageParam = new string[2]
        {
          str,
          this.timeToRestart <= 10.0 ? "seconds" : "minutes"
        };
      }
      textChatMessage.Local = false;
      return textChatMessage;
    }

    private void PlayerSpawnRequestListener(NetworkData data)
    {
      PlayerSpawnRequest playerSpawnRequest = data as PlayerSpawnRequest;
      bool flag = false;
      Player pl = (Player) null;
      if (this.NetworkController.clientList.ContainsKey(playerSpawnRequest.Sender))
        pl = this.NetworkController.clientList[playerSpawnRequest.Sender].Player;
      if (pl == null)
      {
        Dbg.Error((object) "Player spawn request error, player is null", (object) playerSpawnRequest.Sender);
      }
      else
      {
        if (pl.MessagesReceivedWhileLoading.Count > 0)
          pl.MessagesReceivedWhileLoading.Clear();
        if (pl.IsAlive)
          flag = pl.Parent != null && pl.Parent is ArtificialBody;
        else if (playerSpawnRequest.SpawnType == SpawnPointLocationType.Ship)
          flag = this.AddPlayerToShip(pl, playerSpawnRequest.SpawPointParentID, playerSpawnRequest.ShipItemID);
        PlayerSpawnResponse playerSpawnResponse = new PlayerSpawnResponse();
        if (!flag)
        {
          playerSpawnResponse.Response = ResponseResult.Error;
        }
        else
        {
          if (pl.Parent is Pivot && (pl.Parent as Pivot).StabilizeToTargetObj != null)
            (pl.Parent as Pivot).DisableStabilization(false, true);
          if (pl.Parent != null && !pl.IsSubscribedTo(pl.Parent.GUID))
            pl.SubscribeTo(pl.Parent);
          ArtificialBody parent = pl.Parent as ArtificialBody;
          playerSpawnResponse.ParentID = parent.GUID;
          playerSpawnResponse.ParentType = parent.ObjectType;
          playerSpawnResponse.MainVesselID = parent.GUID;
          SpaceObjectVessel spaceObjectVessel = parent as SpaceObjectVessel;
          if (spaceObjectVessel != null && spaceObjectVessel.IsDocked)
          {
            spaceObjectVessel = spaceObjectVessel.DockedToMainVessel;
            playerSpawnResponse.MainVesselID = spaceObjectVessel.GUID;
          }
          ArtificialBody artificialBody = spaceObjectVessel != null ? (ArtificialBody) spaceObjectVessel : parent;
          playerSpawnResponse.ParentTransform = new ObjectTransform()
          {
            GUID = artificialBody.GUID,
            Type = artificialBody.ObjectType,
            Forward = artificialBody.Forward.ToFloatArray(),
            Up = artificialBody.Up.ToFloatArray()
          };
          if (spaceObjectVessel != null && spaceObjectVessel is Ship)
          {
            Ship ship = spaceObjectVessel as Ship;
            playerSpawnResponse.DockedVessels = ship.GetDockedVesselsData();
            playerSpawnResponse.VesselData = ship.VesselData;
            playerSpawnResponse.VesselCaps = ship.VesselCaps;
            playerSpawnResponse.VesselObjects = ship.GetShipObjects();
          }
          else if (spaceObjectVessel != null && spaceObjectVessel is Asteroid)
            playerSpawnResponse.VesselData = spaceObjectVessel.VesselData;
          if (artificialBody.Orbit.IsOrbitValid)
          {
            playerSpawnResponse.ParentTransform.Orbit = new OrbitData()
            {
              ParentGUID = artificialBody.Orbit.Parent.CelestialBody.GUID
            };
            artificialBody.Orbit.FillOrbitData(ref playerSpawnResponse.ParentTransform.Orbit, (SpaceObjectVessel) null);
          }
          else
            playerSpawnResponse.ParentTransform.Realtime = new RealtimeData()
            {
              ParentGUID = artificialBody.Orbit.Parent.CelestialBody.GUID,
              Position = artificialBody.Orbit.RelativePosition.ToArray(),
              Velocity = artificialBody.Orbit.Velocity.ToArray()
            };
          if (pl.CurrentSpawnPoint != null && (pl.CurrentSpawnPoint.IsPlayerInSpawnPoint && pl.CurrentSpawnPoint.Ship == pl.Parent || pl.CurrentSpawnPoint.Type == SpawnPointType.SimpleSpawn && pl.CurrentSpawnPoint.Executer == null && !pl.IsAlive))
          {
            playerSpawnResponse.SpawnPointID = pl.CurrentSpawnPoint.SpawnPointID;
          }
          else
          {
            playerSpawnResponse.CharacterTransform = new CharacterTransformData();
            playerSpawnResponse.CharacterTransform.LocalPosition = pl.LocalPosition.ToFloatArray();
            playerSpawnResponse.CharacterTransform.LocalRotation = pl.LocalRotation.ToFloatArray();
          }
          List<DynamicObjectDetails> dynamicObjectDetailsList = new List<DynamicObjectDetails>();
          foreach (DynamicObject dynamicObject in pl.DynamicObjects)
            dynamicObjectDetailsList.Add(dynamicObject.GetDetails());
          playerSpawnResponse.DynamicObjects = dynamicObjectDetailsList;
          playerSpawnResponse.Health = pl.Health;
          if (pl.AuthorizedSpawnPoint != null)
            playerSpawnResponse.HomeGUID = new long?(pl.AuthorizedSpawnPoint.Ship.GUID);
        }
        this.NetworkController.SendToGameClient(playerSpawnRequest.Sender, (NetworkData) playerSpawnResponse);
      }
    }

    private void PlayerRespawnRequestListener(NetworkData data)
    {
    }

    private void SpawnObjectsRequestListener(NetworkData data)
    {
      SpawnObjectsRequest spawnObjectsRequest = (SpawnObjectsRequest) data;
      SpawnObjectsResponse spawnObjectsResponse = new SpawnObjectsResponse();
      Player player = this.GetPlayer(data.Sender);
      if (player == null)
        return;
      foreach (long guiD in spawnObjectsRequest.GUIDs)
      {
        SpaceObject spaceObject = this.GetObject(guiD);
        if (spaceObject != null)
          spawnObjectsResponse.Data.Add(spaceObject.GetSpawnResponseData(player));
      }
      Server.Instance.NetworkController.SendToGameClient(spawnObjectsRequest.Sender, (NetworkData) spawnObjectsResponse);
    }

    private void UpdateDynamicObjectsTimers(double deltaTime)
    {
      List<Server.DynamicObjectsRespawn> dynamicObjectsRespawnList = new List<Server.DynamicObjectsRespawn>();
      foreach (Server.DynamicObjectsRespawn dynamicObjectsRespawn in this.DynamicObjectsRespawnList)
      {
        if (dynamicObjectsRespawn.Timer > 0.0)
          dynamicObjectsRespawn.Timer -= deltaTime;
        else
          dynamicObjectsRespawnList.Add(dynamicObjectsRespawn);
      }
      if (dynamicObjectsRespawnList.Count <= 0)
        return;
      foreach (Server.DynamicObjectsRespawn dynamicObjectsRespawn in dynamicObjectsRespawnList)
      {
        Server.DynamicObjectsRespawn dos = dynamicObjectsRespawn;
        if (dos.Parent is SpaceObjectVessel && dos.Parent.DynamicObjects.InnerList.Find((Predicate<DynamicObject>) (m =>
        {
          if (m.Item != null && m.Item.AttachPointKey != null && dos.APDetails != null)
            return m.Item.AttachPointKey.InSceneID == dos.APDetails.InSceneID;
          return false;
        })) != null)
        {
          dos.Timer = dos.RespawnTime;
        }
        else
        {
          this.DynamicObjectsRespawnList.Remove(dos);
          if (this._vessels.ContainsKey(dos.Parent.GUID))
          {
            DynamicObject dynamicObject = new DynamicObject(dos.Data, dos.Parent, -1L);
            if (dos.Data.AttachPointInSceneId > 0 && dynamicObject.Item != null)
              dynamicObject.Item.SetAttachPoint(dos.APDetails);
            dynamicObject.APDetails = dos.APDetails;
            dynamicObject.RespawnTime = dos.Data.SpawnSettings.Length != 0 ? dos.Data.SpawnSettings[0].RespawnTime : -1f;
            if (dynamicObject.Item != null && dynamicObject.Item != null && (double) dos.MaxHealth >= 0.0 && (double) dos.MinHealth >= 0.0)
            {
              IDamageable damageable = (IDamageable) dynamicObject.Item;
              damageable.Health = (float) (int) ((double) damageable.MaxHealth * (double) MathHelper.Clamp(MathHelper.RandomRange(dos.MinHealth, dos.MaxHealth), 0.0f, 1f));
            }
            Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) new SpawnObjectsResponse()
            {
              Data = {
                dynamicObject.GetSpawnResponseData((Player) null)
              }
            }, -1L, dos.Parent);
          }
        }
      }
      dynamicObjectsRespawnList.Clear();
    }

    private void UpdateData(double deltaTime)
    {
      this.NetworkController.EventSystem.InvokeQueuedData();
      this.SolarSystem.UpdatePositions(deltaTime);
      this.PhysicsController.Update(deltaTime);
      this.UpdateDynamicObjectsTimers(deltaTime);
      this.UpdateObjectTimers(deltaTime);
      this.UpdatePlayerInvitationTimers(deltaTime);
      Server._movementMessageTimer += deltaTime;
      if (Server._movementMessageTimer < Server.MovementMessageSendInterval && !Server.ForceMovementMessageSend)
        return;
      Server._movementMessageTimer = 0.0;
      Server.ForceMovementMessageSend = false;
      this.SolarSystem.SendMovementMessage();
    }

    public void RemoveWorldObjects()
    {
      Dbg.Info("REMOVING ALL WORLD OBJECTS");
      try
      {
        foreach (long num in this._players.Keys.ToArray<long>())
        {
          Server.Instance.NetworkController.LogOutPlayer(num);
          Server.Instance.NetworkController.DisconnectClient(num);
        }
        Server.Instance.NetworkController.DisconnectAllClients();
      }
      catch (Exception ex)
      {
      }
      this._players.Clear();
      this._objects.Clear();
      foreach (ArtificialBody artificialBody in Server.Instance.SolarSystem.GetArtificialBodies())
      {
        if (artificialBody is Ship)
          (artificialBody as Ship).Destroy();
        else if (artificialBody is Asteroid)
          (artificialBody as Asteroid).Destroy();
        else
          Server.Instance.SolarSystem.RemoveArtificialBody(artificialBody);
      }
      this._vessels.Clear();
      this.WorldInitialized = false;
    }

    public void DestroyArtificialBody(ArtificialBody ab, bool destroyChildren = true, bool vesselExploded = false)
    {
      if (ab == null)
        return;
      if (ab is SpaceObjectVessel)
      {
        SpaceObjectVessel spaceObjectVessel = ab as SpaceObjectVessel;
        if (destroyChildren && spaceObjectVessel.AllDockedVessels.Count > 0)
        {
          foreach (ArtificialBody ab1 in new List<SpaceObjectVessel>((IEnumerable<SpaceObjectVessel>) spaceObjectVessel.AllDockedVessels))
            this.DestroyArtificialBody(ab1, false, vesselExploded);
        }
        foreach (Player player in new List<Player>((IEnumerable<Player>) spaceObjectVessel.VesselCrew))
        {
          try
          {
            player.KillYourself(CauseOfDeath.Shipwrecked, false);
          }
          catch (Exception ex)
          {
            Dbg.Exception(ex);
          }
        }
        if (vesselExploded)
        {
          spaceObjectVessel.DamageVesselsInExplosionRadius();
          NetworkController networkController = this.NetworkController;
          DestroyVesselMessage destroyVesselMessage = new DestroyVesselMessage();
          destroyVesselMessage.GUID = spaceObjectVessel.GUID;
          long skipPlalerGUID = -1;
          SpaceObject[] spaceObjectArray = new SpaceObject[1]
          {
            (SpaceObject) spaceObjectVessel
          };
          networkController.SendToClientsSubscribedTo((NetworkData) destroyVesselMessage, skipPlalerGUID, spaceObjectArray);
        }
        spaceObjectVessel.Destroy();
      }
      else
        ab.Destroy();
    }

    public void UpdateObjectTimers(double deltaTime)
    {
      HashSet<SpaceObjectVessel> spaceObjectVesselSet = (HashSet<SpaceObjectVessel>) null;
      foreach (SpaceObjectVessel allVessel in this.AllVessels)
      {
        allVessel.UpdateTimers(deltaTime);
        if (allVessel is Ship && (double) allVessel.Health < 1.40129846432482E-45)
        {
          if (allVessel.DockedToVessel == null && allVessel.DockedVessels.Count == 0)
          {
            if (spaceObjectVesselSet == null)
              spaceObjectVesselSet = new HashSet<SpaceObjectVessel>();
            spaceObjectVesselSet.Add(allVessel.DockedToMainVessel != null ? allVessel.DockedToMainVessel : allVessel);
          }
          else
            allVessel.UndockAll();
        }
      }
      if (spaceObjectVesselSet != null)
      {
        foreach (ArtificialBody ab in spaceObjectVesselSet)
          this.DestroyArtificialBody(ab, true, true);
      }
      foreach (SpaceObject allPlayer in this.AllPlayers)
        allPlayer.UpdateTimers(deltaTime);
      if (this.PlayersMarkedToDestroy.Count <= 0)
        return;
      foreach (SpaceObject spaceObject in this.PlayersMarkedToDestroy)
        spaceObject.Destroy();
      this.PlayersMarkedToDestroy.Clear();
    }

    public double TickMilliseconds
    {
      get
      {
        return this.tickMilliseconds;
      }
    }

    private void PrintObjectsDebug(double time)
    {
      Dbg.Info((object) "Server stats, objects", (object) this._objects.Count, (object) "players", (object) this._players.Count, (object) "vessels", (object) this._vessels.Count, (object) "artificial bodies", (object) this.SolarSystem.ArtificialBodiesCount);
    }

    public void MainLoop()
    {
      this.SolarSystem.InitializeData();
      if (Server.CleanStart || Server.PersistenceSaveInterval < 0.0 || (Server.CleanStart = !Persistence.Load()))
      {
        if ((double) this.solarSystemStartTime < 0.0)
          this.SolarSystem.CalculatePositionsAfterTime(MathHelper.RandomRange(86400.0, 5256000.0));
        else
          this.SolarSystem.CalculatePositionsAfterTime((double) this.solarSystemStartTime);
        if (Server.SetupType == Server.ServerSetupType.Default || CustomServerInitializers.AutoInitializeWorld(Server.SetupType))
          this.InitializeWorld();
      }
      else
        this.WorldInitialized = true;
      this.Start();
      this.NetworkController.Start();
      this.tickMilliseconds = System.Math.Floor(1000.0 / (double) this.numberOfTicks);
      this.lastTime = DateTime.UtcNow;
      if (Server.ServerRestartTimeSec > 0.0)
      {
        Server.restartTime = DateTime.UtcNow.AddSeconds(Server.ServerRestartTimeSec);
        this.SubscribeToTimer(UpdateTimer.TimerStep.Step_1_0_sec, new UpdateTimer.TimeStepDelegate(this.ServerAutoRestartTimer));
      }
      this.DoomedShipController.SubscribeToTimer();
      bool flag = true;
      DateTime dateTime = DateTime.MinValue;
      if (this.printDebugObjects)
        this.SubscribeToTimer(UpdateTimer.TimerStep.Step_1_0_hr, new UpdateTimer.TimeStepDelegate(this.PrintObjectsDebug));
      this.SubscribeToTimer(UpdateTimer.TimerStep.Step_1_0_sec, new UpdateTimer.TimeStepDelegate(this.UpdateShipSystemsTimer));
      while (Server.IsRunning)
      {
        try
        {
          DateTime utcNow = DateTime.UtcNow;
          TimeSpan timeSpan = utcNow - this.lastTime;
          if (timeSpan.TotalMilliseconds >= this.tickMilliseconds)
          {
            if (this.printDebugObjects && !flag && (utcNow - dateTime).TotalSeconds > 60.0)
            {
              Dbg.Info((object) "Server ticked without sleep, time span ms", (object) timeSpan.TotalMilliseconds, (object) "tick ms", (object) this.tickMilliseconds, (object) "objects", (object) this._objects.Count, (object) "players", (object) this._players.Count, (object) "vessels", (object) this._vessels.Count, (object) "artificial bodies", (object) this.SolarSystem.ArtificialBodiesCount);
              dateTime = utcNow;
            }
            flag = false;
            this.DeltaTime = timeSpan.TotalSeconds;
            this.UpdateData(timeSpan.TotalSeconds);
            this.lastTime = utcNow;
            foreach (UpdateTimer timer in this.timers)
              timer.AddTime(this.DeltaTime);
            if (this.timersToRemove.Count > 0)
            {
              foreach (UpdateTimer updateTimer in this.timersToRemove)
              {
                if (updateTimer.OnTick == null)
                  this.timers.Remove(updateTimer);
              }
              this.timersToRemove.Clear();
            }
            SpawnManager.UpdateTimers(this.DeltaTime);
            if (Server.PersistenceSaveInterval > 0.0 || this.manualSave)
            {
              this.persistenceSaveTimer = this.persistenceSaveTimer + timeSpan.TotalSeconds;
              if (this.persistenceSaveTimer >= Server.PersistenceSaveInterval || this.manualSave)
              {
                this.persistenceSaveTimer = 0.0;
                Persistence.Save();
              }
              this.manualSave = false;
            }
          }
          else
          {
            flag = true;
            Thread.Sleep((int) (this.tickMilliseconds - timeSpan.TotalMilliseconds));
          }
        }
        catch (Exception ex)
        {
          Dbg.Exception(ex);
        }
      }
      if (Server.SavePersistenceDataOnShutdown)
        Persistence.Save();
      Server.MainLoopEnded.Set();
      if (!Server.Restart)
        return;
      Server.RestartServer(Server.CleanRestart);
    }

    public static void RestartServer(bool clean)
    {
      string fileName = "";
      string arguments = "";
      foreach (string commandLineArg in Environment.GetCommandLineArgs())
      {
        if (fileName == "")
          fileName = commandLineArg;
        else if (commandLineArg.ToLower() != "-noload" && commandLineArg.ToLower() != "-clean")
          arguments = arguments + commandLineArg + " ";
      }
      if (clean)
        arguments += " -noload";
      Process.Start(fileName, arguments);
      Process.GetCurrentProcess().Kill();
    }

    public void InitializeWorld()
    {
      if (this.WorldInitialized)
        return;
      SpawnManager.Initialize(false);
      if (Server.SetupType != Server.ServerSetupType.Default)
        CustomServerInitializers.WorldInitialize(this, Server.SetupType);
      this.WorldInitialized = true;
    }

    private void ToggleLockDoor(Ship ship, short inSceneId, bool isLocked)
    {
      foreach (short doorsId in ship.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == (int) inSceneId)).DoorsIDs)
      {
        short id = doorsId;
        Door door = ship.Doors.Find((Predicate<Door>) (m => m.ID.InSceneID == (int) id));
        if (door != null)
          door.IsLocked = isLocked;
      }
    }

    private void OpenDoor(Ship ship, short inSceneId, int newState)
    {
      SceneTriggerExecuter sceneTriggerExecuter1 = ship.SceneTriggerExecuters.Find((Predicate<SceneTriggerExecuter>) (m => m.InSceneID == (int) inSceneId));
      if (sceneTriggerExecuter1 == null)
        return;
      SceneTriggerExecuter sceneTriggerExecuter2 = sceneTriggerExecuter1;
      long sender = 0;
      SceneTriggerExecuterDetails details = new SceneTriggerExecuterDetails();
      details.InSceneID = (int) inSceneId;
      details.NewStateID = newState;
      details.CurrentStateID = 1;
      details.IsImmediate = new bool?(true);
      int num1 = 0;
      details.IsFail = num1 != 0;
      long num2 = 0;
      details.PlayerThatActivated = num2;
      sceneTriggerExecuter2.ChangeState(sender, details);
    }

    private void SubscribeToSpaceObjectListener(NetworkData data)
    {
      SubscribeToObjectsRequest toObjectsRequest = data as SubscribeToObjectsRequest;
      foreach (long guiD in toObjectsRequest.GUIDs)
      {
        SpaceObject spaceObject = this.GetObject(guiD);
        if (spaceObject == null)
        {
          Dbg.Info((object) "Received subscribe to object that does not exist", (object) guiD);
          break;
        }
        if (this._players.ContainsKey(toObjectsRequest.Sender))
        {
          Player player = this._players[toObjectsRequest.Sender];
          player.SubscribeTo(spaceObject);
          this.NetworkController.SendToGameClient(toObjectsRequest.Sender, (NetworkData) spaceObject.GetInitializeMessage());
          if (spaceObject is ArtificialBody)
            player.UpdateArtificialBodyMovement.Add(spaceObject.GUID);
        }
      }
    }

    private void UnsubscribeFromSpaceObjectListener(NetworkData data)
    {
      UnsubscribeFromObjectsRequest fromObjectsRequest = data as UnsubscribeFromObjectsRequest;
      foreach (long guiD in fromObjectsRequest.GUIDs)
      {
        SpaceObject spaceObject = this.GetObject(guiD);
        if (spaceObject == null)
          break;
        if (this._players.ContainsKey(fromObjectsRequest.Sender))
          this._players[fromObjectsRequest.Sender].UnsubscribeFrom(spaceObject);
      }
    }

    public void SubscribeToTimer(UpdateTimer.TimerStep step, UpdateTimer.TimeStepDelegate del)
    {
      UpdateTimer updateTimer = this.timers.Find((Predicate<UpdateTimer>) (x => x.Step == step));
      if (updateTimer == null)
      {
        updateTimer = new UpdateTimer(step);
        this.timers.Add(updateTimer);
      }
      updateTimer.OnTick += del;
    }

    public void UnsubscribeFromTimer(UpdateTimer.TimerStep step, UpdateTimer.TimeStepDelegate del)
    {
      UpdateTimer updateTimer = this.timers.Find((Predicate<UpdateTimer>) (x => x.Step == step));
      if (updateTimer == null)
        return;
      updateTimer.OnTick -= del;
      if (updateTimer.OnTick == null)
        this.timersToRemove.Add(updateTimer);
    }

    public void CheckInResponseListener(NetworkData data)
    {
      CheckInResponse checkInResponse = data as CheckInResponse;
      if (checkInResponse.Response == ResponseResult.Success)
      {
        if (this.NetworkController.ServerID != checkInResponse.ServerID)
        {
          this.NetworkController.ServerID = checkInResponse.ServerID;
          Dbg.UnformattedMessage("==============================================================================\r\n\tServer ID: " + (object) this.NetworkController.ServerID + "\r\n==============================================================================\r\n");
          try
          {
            System.IO.File.WriteAllText(Server.ConfigDir + "ServerID.txt", string.Concat((object) this.NetworkController.ServerID));
          }
          catch
          {
          }
        }
        Server.CheckInPassed = true;
        this.AdminIPAddressRanges = checkInResponse.AdminIPAddressRanges;
        this.SubscribeToTimer(UpdateTimer.TimerStep.Step_1_0_hr, new UpdateTimer.TimeStepDelegate(this.SendCheckInMessage));
      }
      else
      {
        Server.IsRunning = false;
        Dbg.Exception(new Exception(checkInResponse.Message));
      }
    }

    public void PlayersOnServerRequestListener(NetworkData data)
    {
      PlayersOnServerRequest req = data as PlayersOnServerRequest;
      SpaceObjectVessel spaceObjectVessel = (SpaceObjectVessel) null;
      if (req.SpawnPointID != null)
        spaceObjectVessel = this.GetVessel(req.SpawnPointID.VesselGUID);
      else if (req.SecuritySystemID != null)
        spaceObjectVessel = this.GetVessel(req.SecuritySystemID.VesselGUID);
      if (spaceObjectVessel == null)
        return;
      if (req.SpawnPointID != null)
      {
        if (spaceObjectVessel.SpawnPoints.Find((Predicate<ShipSpawnPoint>) (m => m.SpawnPointID == req.SpawnPointID.InSceneID)) == null)
          return;
        PlayersOnServerResponse onServerResponse = new PlayersOnServerResponse();
        onServerResponse.SpawnPointID = new VesselObjectID()
        {
          InSceneID = req.SpawnPointID.InSceneID,
          VesselGUID = req.SpawnPointID.VesselGUID
        };
        onServerResponse.PlayersOnServer = new List<PlayerOnServerData>();
        this.NetworkController.clientList.Lock();
        foreach (NetworkController.Client client in (IEnumerable<NetworkController.Client>) this.NetworkController.clientList.Values)
        {
          if (client.Player != null && !client.Player.SteamId.IsNullOrEmpty())
          {
            List<PlayerOnServerData> playersOnServer = onServerResponse.PlayersOnServer;
            PlayerOnServerData playerOnServerData = new PlayerOnServerData();
            playerOnServerData.SteamID = client.Player.SteamId;
            playerOnServerData.Name = client.Player.Name;
            int num = this.SpawnPointInvites.ContainsKey(client.Player.SteamId) ? 1 : 0;
            playerOnServerData.AlreadyHasInvite = num != 0;
            playersOnServer.Add(playerOnServerData);
          }
          if (client.Player != null && client.Player.SteamId.IsNullOrEmpty())
            Dbg.Error((object) "Player steam ID is null or empty", (object) client.Player.GUID, (object) client.Player.Name);
        }
        this.NetworkController.clientList.Unlock();
        this.NetworkController.SendToGameClient(req.Sender, (NetworkData) onServerResponse);
      }
      else
      {
        if (req.SecuritySystemID == null)
          return;
        PlayersOnServerResponse onServerResponse1 = new PlayersOnServerResponse();
        PlayersOnServerResponse onServerResponse2 = onServerResponse1;
        VesselObjectID vesselObjectId = new VesselObjectID();
        vesselObjectId.InSceneID = 0;
        long vesselGuid = req.SecuritySystemID.VesselGUID;
        vesselObjectId.VesselGUID = vesselGuid;
        onServerResponse2.SecuritySystemID = vesselObjectId;
        onServerResponse1.PlayersOnServer = new List<PlayerOnServerData>();
        this.NetworkController.clientList.Lock();
        foreach (NetworkController.Client client in (IEnumerable<NetworkController.Client>) this.NetworkController.clientList.Values)
        {
          if (client.Player != null && !client.Player.SteamId.IsNullOrEmpty())
          {
            List<PlayerOnServerData> playersOnServer = onServerResponse1.PlayersOnServer;
            PlayerOnServerData playerOnServerData = new PlayerOnServerData();
            playerOnServerData.SteamID = client.Player.SteamId;
            playerOnServerData.Name = client.Player.Name;
            int num = 0;
            playerOnServerData.AlreadyHasInvite = num != 0;
            playersOnServer.Add(playerOnServerData);
          }
          if (client.Player != null && client.Player.SteamId.IsNullOrEmpty())
            Dbg.Error((object) "Player steam ID is null or empty", (object) client.Player.GUID, (object) client.Player.Name);
        }
        this.NetworkController.clientList.Unlock();
        this.NetworkController.SendToGameClient(req.Sender, (NetworkData) onServerResponse1);
      }
    }

    public void AvailableSpawnPointsRequestListener(NetworkData data)
    {
      AvailableSpawnPointsRequest spawnPointsRequest = data as AvailableSpawnPointsRequest;
      Player player = this.GetPlayer(spawnPointsRequest.Sender);
      if (player == null)
        return;
      this.NetworkController.SendToGameClient(spawnPointsRequest.Sender, (NetworkData) new AvailableSpawnPointsResponse()
      {
        SpawnPoints = this.GetAvailableSpawnPoints(player)
      });
    }

    public void SaveGameMessageListener(NetworkData data)
    {
    }

    private void NameTagMessageListener(NetworkData data)
    {
      NameTagMessage msg = data as NameTagMessage;
      SpaceObjectVessel vessel = this.GetVessel(msg.ID.VesselGUID);
      this.NetworkController.SendToClientsSubscribedTo(data, -1L, (SpaceObject) vessel);
      try
      {
        vessel.NameTags.Find((Predicate<NameTagData>) (m => m.InSceneID == msg.ID.InSceneID)).NameTagText = msg.NameTagText;
      }
      catch
      {
      }
    }

    public void SendCheckInMessage(double amount)
    {
      this.NetworkController.SendToMainServer((NetworkData) new CheckInMessage()
      {
        ServerID = this.NetworkController.ServerID
      });
    }

    public bool IsAddressAutorized(string address)
    {
      try
      {
        if (address == "127.0.0.1" || address == "localhost")
          return true;
        byte[] addressBytes1 = IPAddress.Parse(address).GetAddressBytes();
        Array.Reverse((Array) addressBytes1);
        uint uint32_1 = BitConverter.ToUInt32(addressBytes1, 0);
        foreach (IPAddressRange adminIpAddressRange in this.AdminIPAddressRanges)
        {
          byte[] addressBytes2 = IPAddress.Parse(adminIpAddressRange.StartAddress).GetAddressBytes();
          Array.Reverse((Array) addressBytes2);
          byte[] addressBytes3 = IPAddress.Parse(adminIpAddressRange.EndAddress).GetAddressBytes();
          Array.Reverse((Array) addressBytes3);
          uint uint32_2 = BitConverter.ToUInt32(addressBytes2, 0);
          uint uint32_3 = BitConverter.ToUInt32(addressBytes3, 0);
          if (uint32_1 >= uint32_2 && uint32_1 <= uint32_3)
            return true;
        }
      }
      catch
      {
      }
      return false;
    }

    public void UpdatePlayerInvitationTimers(double deltaTime)
    {
      if (this.SpawnPointInvites.Count == 0)
        return;
      List<string> stringList = (List<string>) null;
      foreach (KeyValuePair<string, Server.SpawnPointInviteData> spawnPointInvite in this.SpawnPointInvites)
      {
        spawnPointInvite.Value.InviteTimer -= deltaTime;
        if (spawnPointInvite.Value.InviteTimer <= 0.0)
        {
          if (stringList == null)
            stringList = new List<string>();
          stringList.Add(spawnPointInvite.Key);
        }
      }
      if (stringList == null || stringList.Count <= 0)
        return;
      foreach (string steamID in stringList)
        this.ClearSpawnPointInvitation(steamID);
    }

    public void ClearSpawnPointInvitation(string steamID)
    {
      if (!this.SpawnPointInvites.ContainsKey(steamID))
        return;
      this.SpawnPointInvites[steamID].SpawnPoint.SetInvitation("", "", true);
      this.SpawnPointInvites.Remove(steamID);
    }

    public void CreateSpawnPointInvitation(ShipSpawnPoint sp, string steamID, string playerName)
    {
      this.SpawnPointInvites.Add(steamID, new Server.SpawnPointInviteData()
      {
        SpawnPoint = sp,
        InviteTimer = Server.SpawnPointInviteTimer
      });
      sp.SetInvitation(steamID, playerName, true);
    }

    public bool PlayerInviteChanged(ShipSpawnPoint sp, string invitedPlayerSteamID, string invitedPlayerName, Player sender)
    {
      if (!invitedPlayerSteamID.IsNullOrEmpty())
      {
        if (this.SpawnPointInvites.ContainsKey(invitedPlayerSteamID) && this.SpawnPointInvites[invitedPlayerSteamID].SpawnPoint == sp && sp.InvitedPlayerSteamID == invitedPlayerSteamID)
          return false;
        if (this.SpawnPointInvites.ContainsKey(invitedPlayerSteamID))
          this.ClearSpawnPointInvitation(invitedPlayerSteamID);
        if (!sp.InvitedPlayerSteamID.IsNullOrEmpty() && this.SpawnPointInvites.ContainsKey(sp.InvitedPlayerSteamID))
          this.ClearSpawnPointInvitation(sp.InvitedPlayerSteamID);
        this.CreateSpawnPointInvitation(sp, invitedPlayerSteamID, invitedPlayerName);
        return true;
      }
      if (sp.InvitedPlayerSteamID.IsNullOrEmpty())
        return false;
      if (this.SpawnPointInvites.ContainsKey(sp.InvitedPlayerSteamID))
        this.ClearSpawnPointInvitation(sp.InvitedPlayerSteamID);
      else
        sp.SetInvitation("", "", true);
      return true;
    }

    private void ServerAutoRestartTimer(double time)
    {
      DateTime utcNow = DateTime.UtcNow;
      if (utcNow.AddSeconds(this.timeToRestart) > Server.restartTime)
      {
        if ((Server.restartTime - utcNow).TotalSeconds >= this.timeToRestart - 2.0)
          this.NetworkController.SendToAllClients((NetworkData) this.SendSystemMessage(SystemMessagesTypes.RestartServerTime, (Ship) null), -1L);
        this.timeToRestart = this.timeToRestart != 300.0 ? (this.timeToRestart != 60.0 ? (this.timeToRestart > 10.0 ? this.timeToRestart - 300.0 : this.timeToRestart - 1.0) : 10.0) : 60.0;
      }
      if (!(utcNow > Server.restartTime))
        return;
      Server.IsRunning = false;
      Server.Restart = true;
      Server.SavePersistenceDataOnShutdown = true;
      Server.restartTime = utcNow.AddSeconds(Server.ServerRestartTimeSec);
    }

    private void UpdateShipSystemsTimer(double time)
    {
      if (this.updateShipSystemsThread != null)
        return;
      this.updateShipSystemsThread = new Thread((ThreadStart) (() =>
      {
        try
        {
          foreach (SpaceObjectVessel spaceObjectVessel in new List<SpaceObjectVessel>((IEnumerable<SpaceObjectVessel>) this._vessels.Values))
          {
            spaceObjectVessel.UpdateVesselSystems();
            double num = (double) spaceObjectVessel.ChangeHealthBy((float) (-(double) spaceObjectVessel.DecayRate * Server.VesselDecayRateMultiplier * time), (List<VesselRepairPoint>) null);
          }
        }
        catch
        {
        }
        this.updateShipSystemsThread = (Thread) null;
      }));
      this.updateShipSystemsThread.Start();
    }

    public enum ServerSetupType
    {
      Default = 1,
      RadeTesting = 1000,
      SpawnForShomy = 1002,
      ZareTesting = 1003,
      xXx_DimasGreatServerSetupGG_xXx = 1004,
      PlanetResourceGathering = 1005,
      VujaTesting = 1008,
      MarioTesting = 1009,
      Dimbe = 1010,
      Test = 1011,
    }

    public class DynamicObjectsRespawn
    {
      public float MaxHealth = -1f;
      public float MinHealth = -1f;
      public float WearMultiplier = 1f;
      public DynamicObjectSceneData Data;
      public SpaceObject Parent;
      public AttachPointDetails APDetails;
      public double Timer;
      public double RespawnTime;
    }

    public class SpawnPointInviteData
    {
      public double InviteTimer = 300.0;
      public ShipSpawnPoint SpawnPoint;
    }

    public static class NameGenerator
    {
      private static DateTime lastClearDate = DateTime.UtcNow;
      private static Dictionary<GameScenes.SceneID, int> dailySpawnCount = new Dictionary<GameScenes.SceneID, int>();
      private static List<char> monthCodes = new List<char>()
      {
        'A',
        'B',
        'C',
        'D',
        'E',
        'F',
        'G',
        'H',
        'I',
        'J',
        'K',
        'L'
      };
      private static Dictionary<GameScenes.SceneID, string> shipNaming = new Dictionary<GameScenes.SceneID, string>()
      {
        {
          GameScenes.SceneID.AltCorp_LifeSupportModule,
          "LSM-AC:HE3/"
        },
        {
          GameScenes.SceneID.ALtCorp_PowerSupply_Module,
          "PSM-AC:HE1/"
        },
        {
          GameScenes.SceneID.AltCorp_AirLock,
          "AM-AC:HE1/"
        },
        {
          GameScenes.SceneID.AltCorp_Cargo_Module,
          "CBM-AC:HE1/"
        },
        {
          GameScenes.SceneID.AltCorp_Command_Module,
          "CM-AC:HE3/"
        },
        {
          GameScenes.SceneID.AltCorp_DockableContainer,
          "IC-AC:HE2/"
        },
        {
          GameScenes.SceneID.AltCorp_CorridorIntersectionModule,
          "CTM-AC:HE1/"
        },
        {
          GameScenes.SceneID.AltCorp_Corridor45TurnModule,
          "CLM-AC:HE1/"
        },
        {
          GameScenes.SceneID.AltCorp_Corridor45TurnRightModule,
          "CRM-AC:HE1/"
        },
        {
          GameScenes.SceneID.AltCorp_CorridorVertical,
          "CSM-AC:HE1/"
        },
        {
          GameScenes.SceneID.AltCorp_CorridorModule,
          "CIM-AC:HE1/"
        },
        {
          GameScenes.SceneID.AltCorp_StartingModule,
          "OUTPOST "
        },
        {
          GameScenes.SceneID.AltCorp_Shuttle_SARA,
          "AC-ARG HE1/"
        },
        {
          GameScenes.SceneID.AltCorp_CrewQuarters_Module,
          "CQM-AC:HE1/"
        }
      };
      private static List<GameScenes.SceneID> derelicts = new List<GameScenes.SceneID>()
      {
        GameScenes.SceneID.Generic_Debris_Corridor001,
        GameScenes.SceneID.Generic_Debris_Corridor002,
        GameScenes.SceneID.Generic_Debris_JuncRoom001,
        GameScenes.SceneID.Generic_Debris_JuncRoom002
      };

      public static string GenerateObjectRegistration(SpaceObjectType type, CelestialBody parentCB, GameScenes.SceneID sceneId)
      {
        string str1 = "";
        string format = "000";
        if (type == SpaceObjectType.Ship && !Server.NameGenerator.derelicts.Contains(sceneId))
        {
          if (Server.NameGenerator.shipNaming.ContainsKey(sceneId))
          {
            str1 += Server.NameGenerator.shipNaming[sceneId];
          }
          else
          {
            Dbg.Warning((object) "No name tag for ship", (object) sceneId);
            str1 = str1 + type.ToString() + MathHelper.RandomNextInt().ToString("X");
          }
        }
        else
        {
          string str2;
          if (parentCB != null)
          {
            string str3 = ((CelestialBodyGUID) parentCB.GUID).ToString();
            str2 = str3.Substring(0, System.Math.Min(3, str3.Length)).ToUpper();
          }
          else
            str2 = "HEL";
          if (type == SpaceObjectType.Ship)
          {
            str1 = str1 + "DERELICT " + str2 + "-";
            format = "00";
          }
          else if (type == SpaceObjectType.Asteroid)
            str1 = str1 + "ASTEROID " + str2 + "-";
        }
        if ((DateTime.UtcNow - Server.NameGenerator.lastClearDate).TotalDays > 1.0)
        {
          Server.NameGenerator.lastClearDate = DateTime.UtcNow;
          Server.NameGenerator.dailySpawnCount.Clear();
        }
        if (Server.NameGenerator.dailySpawnCount.ContainsKey(sceneId))
        {
          Dictionary<GameScenes.SceneID, int> dailySpawnCount = Server.NameGenerator.dailySpawnCount;
          GameScenes.SceneID sceneId1 = sceneId;
          int num1 = (int) sceneId1;
          int num2 = dailySpawnCount[(GameScenes.SceneID) num1];
          int num3 = (int) sceneId1;
          int num4 = num2 + 1;
          dailySpawnCount[(GameScenes.SceneID) num3] = num4;
        }
        else
          Server.NameGenerator.dailySpawnCount.Add(sceneId, 1);
        return str1 + DateTime.UtcNow.Day.ToString("00") + Server.NameGenerator.monthCodes[DateTime.UtcNow.Month - 1].ToString() + Server.NameGenerator.dailySpawnCount[sceneId].ToString(format);
      }

      public static string GenerateStationRegistration()
      {
        return "STATION";
      }
    }
  }
}
