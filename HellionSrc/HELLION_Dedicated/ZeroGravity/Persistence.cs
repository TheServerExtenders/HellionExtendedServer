// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Persistence
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;
using ZeroGravity.Spawn;

namespace ZeroGravity
{
  public class Persistence
  {
    public static string PersistanceFileName = "ServerSave_{0}.save";
    public static int CurrentSaveType = 1;
    public int SaveType;
    public double SolarSystemTime;
    public List<PersistenceObjectData> Ships;
    public List<PersistenceObjectData> Asteroids;
    public List<PersistenceObjectData> Players;
    public List<PersistenceObjectData> RespawnObjects;
    public List<PersistenceObjectData> SpawnPoints;
    public List<PersistenceObjectData> ArenaControllers;
    public PersistenceObjectData DoomControllerData;
    public PersistenceObjectData SpawnManagerData;

    private static void SaveVesselPersistence(ref Persistence per, SpaceObjectVessel ves)
    {
      if (ves.ObjectType == SpaceObjectType.Ship)
        per.Ships.Add((ves as IPersistantObject).GetPersistenceData());
      else if (ves.ObjectType == SpaceObjectType.Asteroid)
        per.Asteroids.Add((ves as IPersistantObject).GetPersistenceData());
      if (ves.DockedVessels != null && ves.DockedVessels.Count > 0)
      {
        foreach (SpaceObjectVessel dockedVessel in ves.DockedVessels)
          Persistence.SaveVesselPersistence(ref per, dockedVessel);
      }
      if (ves.StabilizedToTargetChildren == null || ves.StabilizedToTargetChildren.Count <= 0)
        return;
      foreach (SpaceObjectVessel stabilizedToTargetChild in ves.StabilizedToTargetChildren)
      {
        if (!stabilizedToTargetChild.IsDocked)
          Persistence.SaveVesselPersistence(ref per, stabilizedToTargetChild);
      }
    }

    private static void SaveRespawnObjectPersistence(ref Persistence per, Server.DynamicObjectsRespawn obj)
    {
      PersistenceObjectDataRespawnObject dataRespawnObject = new PersistenceObjectDataRespawnObject()
      {
        ItemID = obj.Data.ItemID,
        ParentGUID = obj.Parent.GUID,
        ParentType = obj.Parent.ObjectType,
        Position = obj.Data.Position,
        Forward = obj.Data.Forward,
        Up = obj.Data.Up,
        AuxData = obj.Data.AuxData,
        RespawnTime = (float) (obj.Data.SpawnSettings.Length != 0 ? (double) obj.Data.SpawnSettings[0].RespawnTime : -1.0),
        Timer = obj.Timer
      };
      if (obj.APDetails != null)
        dataRespawnObject.AttachPointID = new int?(obj.APDetails.InSceneID);
      per.RespawnObjects.Add((PersistenceObjectData) dataRespawnObject);
    }

    private static void SaveSpawnPointPeristence(ref Persistence per, ShipSpawnPoint sp)
    {
      PersistenceObjectDataSpawnPoint objectDataSpawnPoint1 = new PersistenceObjectDataSpawnPoint();
      long guid = sp.Ship.GUID;
      objectDataSpawnPoint1.GUID = guid;
      int spawnPointId = sp.SpawnPointID;
      objectDataSpawnPoint1.SpawnID = spawnPointId;
      int type = (int) sp.Type;
      objectDataSpawnPoint1.SpawnType = (SpawnPointType) type;
      int state = (int) sp.State;
      objectDataSpawnPoint1.SpawnState = (SpawnPointState) state;
      PersistenceObjectDataSpawnPoint objectDataSpawnPoint2 = objectDataSpawnPoint1;
      if (sp.Player != null)
      {
        objectDataSpawnPoint2.PlayerGUID = new long?(sp.Player.GUID);
        objectDataSpawnPoint2.IsPlayerInSpawnPoint = new bool?(sp.IsPlayerInSpawnPoint);
      }
      per.SpawnPoints.Add((PersistenceObjectData) objectDataSpawnPoint2);
    }

    public static void Save()
    {
      Persistence per = new Persistence();
      per.SaveType = Persistence.CurrentSaveType;
      per.SolarSystemTime = Server.Instance.SolarSystem.CurrentTime;
      per.Ships = new List<PersistenceObjectData>();
      per.Asteroids = new List<PersistenceObjectData>();
      per.Players = new List<PersistenceObjectData>();
      per.RespawnObjects = new List<PersistenceObjectData>();
      per.SpawnPoints = new List<PersistenceObjectData>();
      per.ArenaControllers = new List<PersistenceObjectData>();
      foreach (SpaceObjectVessel allVessel in Server.Instance.AllVessels)
      {
        if (!allVessel.IsDocked && allVessel.StabilizeToTargetObj == null && (allVessel.ObjectType == SpaceObjectType.Ship || allVessel.ObjectType == SpaceObjectType.Asteroid))
          Persistence.SaveVesselPersistence(ref per, allVessel);
      }
      foreach (Player allPlayer in Server.Instance.AllPlayers)
      {
        if (allPlayer != null)
          per.Players.Add(allPlayer.GetPersistenceData());
      }
      foreach (Server.DynamicObjectsRespawn dynamicObjectsRespawn in Server.Instance.DynamicObjectsRespawnList)
        Persistence.SaveRespawnObjectPersistence(ref per, dynamicObjectsRespawn);
      foreach (SpaceObjectVessel allVessel in Server.Instance.AllVessels)
      {
        if (allVessel.SpawnPoints != null && allVessel.SpawnPoints.Count > 0)
        {
          foreach (ShipSpawnPoint spawnPoint in allVessel.SpawnPoints)
            Persistence.SaveSpawnPointPeristence(ref per, spawnPoint);
        }
      }
      foreach (DeathMatchArenaController matchArenaController in Server.Instance.DeathMatchArenaControllers)
        per.ArenaControllers.Add(matchArenaController.GetPersistenceData());
      per.DoomControllerData = Server.Instance.DoomedShipController.GetPersistenceData();
      per.SpawnManagerData = SpawnManager.GetPersistenceData();
      FileInfo[] files = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Server.ConfigDir)).GetFiles("*.save");
      if (files.Length >= Server.MaxNumberOfSaveFiles)
      {
        Array.Sort<FileInfo>(files, (Comparison<FileInfo>) ((file1, file2) => file2.CreationTimeUtc.CompareTo(file1.CreationTimeUtc)));
        for (int index = Server.MaxNumberOfSaveFiles - 1; index < files.Length; ++index)
          files[index].Delete();
      }
      Json.SerializeToFile((object) per, string.Format(Path.Combine(Server.ConfigDir, Persistence.PersistanceFileName), (object) DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss")), Json.Formatting.Indented);
    }

    public static void LoadRespawnObjectPersistence(PersistenceObjectDataRespawnObject data)
    {
      SpaceObject spaceObject = Server.Instance.GetObject(data.ParentGUID);
      if (spaceObject == null)
      {
        Dbg.Error((object) "Could not find parent object for respawn object persistence", (object) data.ItemID);
      }
      else
      {
        AttachPointDetails attachPointDetails1 = (AttachPointDetails) null;
        if (data.AttachPointID.HasValue && data.AttachPointID.Value > 0)
          attachPointDetails1 = new AttachPointDetails()
          {
            InSceneID = data.AttachPointID.Value
          };
        DynamicObjectSceneData dynamicObjectSceneData1 = new DynamicObjectSceneData();
        dynamicObjectSceneData1.ItemID = data.ItemID;
        dynamicObjectSceneData1.Position = data.Position;
        dynamicObjectSceneData1.Forward = data.Forward;
        dynamicObjectSceneData1.Up = data.Up;
        dynamicObjectSceneData1.AttachPointInSceneId = data.AttachPointID.HasValue ? data.AttachPointID.Value : -1;
        dynamicObjectSceneData1.AuxData = data.AuxData;
        dynamicObjectSceneData1.SpawnSettings = new DynaminObjectSpawnSettings[1]
        {
          new DynaminObjectSpawnSettings()
          {
            RespawnTime = data.RespawnTime,
            SpawnChance = -1f,
            Tag = ""
          }
        };
        DynamicObjectSceneData dynamicObjectSceneData2 = dynamicObjectSceneData1;
        List<Server.DynamicObjectsRespawn> objectsRespawnList = Server.Instance.DynamicObjectsRespawnList;
        Server.DynamicObjectsRespawn dynamicObjectsRespawn = new Server.DynamicObjectsRespawn();
        dynamicObjectsRespawn.Data = dynamicObjectSceneData2;
        dynamicObjectsRespawn.Parent = spaceObject;
        double timer = data.Timer;
        dynamicObjectsRespawn.Timer = timer;
        double respawnTime = (double) data.RespawnTime;
        dynamicObjectsRespawn.RespawnTime = respawnTime;
        AttachPointDetails attachPointDetails2 = attachPointDetails1;
        dynamicObjectsRespawn.APDetails = attachPointDetails2;
        objectsRespawnList.Add(dynamicObjectsRespawn);
      }
    }

    public static void LoadSpawnPointPeristence(PersistenceObjectDataSpawnPoint data)
    {
      try
      {
        ShipSpawnPoint spawnPoint = (Server.Instance.GetVessel(data.GUID) as Ship).SpawnPoints.Find((Predicate<ShipSpawnPoint>) (m => m.SpawnPointID == data.SpawnID));
        spawnPoint.State = data.SpawnState;
        spawnPoint.Type = data.SpawnType;
        if (!data.PlayerGUID.HasValue)
          return;
        Player player = Server.Instance.GetPlayer(data.PlayerGUID.Value);
        spawnPoint.Player = player;
        spawnPoint.IsPlayerInSpawnPoint = data.IsPlayerInSpawnPoint.Value;
        if (spawnPoint.IsPlayerInSpawnPoint || spawnPoint.State == SpawnPointState.Authorized)
          player.SetSpawnPoint(spawnPoint);
      }
      catch (Exception ex)
      {
        Dbg.Error((object) "Failed to load spawn point from persistence", (object) ex.Message, (object) ex.StackTrace);
      }
    }

    public static bool Load()
    {
      FileInfo[] files = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Server.ConfigDir)).GetFiles("*.save");
      FileInfo fileInfo1 = (FileInfo) null;
      foreach (FileInfo fileInfo2 in files)
      {
        if (fileInfo1 == null || fileInfo2.CreationTimeUtc > fileInfo1.CreationTimeUtc)
          fileInfo1 = fileInfo2;
      }
      if (fileInfo1 == null)
        return false;
      Persistence persistence = Json.Load<Persistence>(fileInfo1.FullName);
      Server.Instance.SolarSystem.CalculatePositionsAfterTime(persistence.SolarSystemTime);
      if (persistence.Asteroids != null)
      {
        foreach (PersistenceObjectData asteroid in persistence.Asteroids)
          new Asteroid(asteroid.GUID, false, Vector3D.Zero, Vector3D.One, Vector3D.Forward, Vector3D.Up).LoadPersistenceData(asteroid);
      }
      if (persistence.Ships != null)
      {
        foreach (PersistenceObjectData ship in persistence.Ships)
          new Ship(ship.GUID, false, Vector3D.Zero, Vector3D.One, Vector3D.Forward, Vector3D.Up).LoadPersistenceData(ship);
      }
      if (persistence.Players != null)
      {
        foreach (PersistenceObjectDataPlayer player in persistence.Players)
          new Player(player.GUID, Vector3D.Zero, QuaternionD.Identity, "PersistenceLoad", "", player.Gender, player.HeadType, player.HairType, false).LoadPersistenceData((PersistenceObjectData) player);
      }
      if (persistence.RespawnObjects != null)
      {
        foreach (PersistenceObjectDataRespawnObject respawnObject in persistence.RespawnObjects)
          Persistence.LoadRespawnObjectPersistence(respawnObject);
      }
      if (persistence.SpawnPoints != null)
      {
        foreach (PersistenceObjectDataSpawnPoint spawnPoint in persistence.SpawnPoints)
          Persistence.LoadSpawnPointPeristence(spawnPoint);
      }
      if (persistence.ArenaControllers != null)
      {
        foreach (PersistenceObjectData arenaController in persistence.ArenaControllers)
          new DeathMatchArenaController().LoadPersistenceData(arenaController);
      }
      if (persistence.DoomControllerData != null)
        Server.Instance.DoomedShipController.LoadPersistenceData(persistence.DoomControllerData);
      SpawnManager.LoadPersistenceData(persistence.SpawnManagerData);
      return true;
    }

    public static DynamicObject CreateDynamicObject(PersistenceObjectDataDynamicObject persistenceData, SpaceObject parent, StructureSceneData structureSceneData = null)
    {
      try
      {
        if (persistenceData == null)
          return (DynamicObject) null;
        int num1 = -1;
        IDynamicObjectAuxData idynamicObjectAuxData = (IDynamicObjectAuxData) null;
        if (persistenceData.Type != Persistence.ObjectType.DynamicObject)
        {
          PersistenceObjectDataItem data = persistenceData as PersistenceObjectDataItem;
          if (data.AttachPointID.HasValue)
            num1 = data.AttachPointID.Value;
          if (persistenceData.Type == Persistence.ObjectType.Item && structureSceneData != null && structureSceneData.DynamicObjects != null)
          {
            DynamicObjectSceneData dynamicObjectSceneData = structureSceneData.DynamicObjects.Find((Predicate<DynamicObjectSceneData>) (m => (int) m.ItemID == (int) data.ItemID));
            if (dynamicObjectSceneData != null)
              idynamicObjectAuxData = ObjectCopier.DeepCopy<IDynamicObjectAuxData>(dynamicObjectSceneData.AuxData, 10);
          }
        }
        DynamicObjectSceneData dosd = new DynamicObjectSceneData();
        dosd.ItemID = persistenceData.ItemID;
        dosd.Position = persistenceData.RespawnTime.HasValue ? persistenceData.RespawnPosition : persistenceData.LocalPosition;
        dosd.Forward = persistenceData.RespawnTime.HasValue ? persistenceData.RespawnForward : (persistenceData.LocalRotation.ToQuaternionD() * Vector3D.Forward).ToFloatArray();
        dosd.Up = persistenceData.RespawnTime.HasValue ? persistenceData.RespawnUp : (persistenceData.LocalRotation.ToQuaternionD() * Vector3D.Up).ToFloatArray();
        dosd.AttachPointInSceneId = num1;
        dosd.AuxData = persistenceData.RespawnAuxData;
        DynamicObjectSceneData dynamicObjectSceneData1 = dosd;
        DynaminObjectSpawnSettings[] objectSpawnSettingsArray;
        if (!persistenceData.RespawnTime.HasValue)
          objectSpawnSettingsArray = (DynaminObjectSpawnSettings[]) null;
        else
          objectSpawnSettingsArray = new DynaminObjectSpawnSettings[1]
          {
            new DynaminObjectSpawnSettings()
            {
              RespawnTime = persistenceData.RespawnTime.Value,
              SpawnChance = -1f,
              Tag = ""
            }
          };
        dynamicObjectSceneData1.SpawnSettings = objectSpawnSettingsArray;
        DynamicObject dynamicObject = new DynamicObject(dosd, parent, persistenceData.GUID);
        if (dynamicObject.Item != null)
        {
          PersistenceObjectDataItem data = persistenceData as PersistenceObjectDataItem;
          int num2;
          if (dynamicObject.Parent is SpaceObjectVessel)
          {
            PersistenceObjectDataItem persistenceObjectDataItem = data;
            if ((persistenceObjectDataItem != null ? (persistenceObjectDataItem.AttachPointID.HasValue ? 1 : 0) : 0) != 0)
            {
              num2 = dynamicObject.Parent.DynamicObjects.InnerList.Find((Predicate<DynamicObject>) (m =>
              {
                Item obj = m.Item;
                if ((obj != null ? obj.AttachPointKey : (VesselObjectID) null) != null)
                  return m.Item.AttachPointKey.InSceneID == data.AttachPointID.Value;
                return false;
              })) != null ? 1 : 0;
              goto label_19;
            }
          }
          num2 = 0;
label_19:
          if (num2 != 0)
          {
            dynamicObject.DestroyDynamicObject();
            return (DynamicObject) null;
          }
          dynamicObject.Item.LoadPersistenceData((PersistenceObjectData) persistenceData);
        }
        else
          dynamicObject.LoadPersistenceData((PersistenceObjectData) persistenceData);
        foreach (PersistenceObjectDataDynamicObject childObject in persistenceData.ChildObjects)
          Persistence.CreateDynamicObject(childObject, (SpaceObject) dynamicObject, structureSceneData);
        return dynamicObject;
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
        return (DynamicObject) null;
      }
    }

    public enum ObjectType
    {
      NONE = 0,
      Ship = 1,
      Asteroid = 2,
      DynamicObject = 3,
      Item = 4,
      MachineryPart = 5,
      Jetpack = 6,
      Canister = 7,
      Outfit = 8,
      Helmet = 9,
      Cargo = 10,
      VesselComponent = 11,
      Player = 12,
      Weapon = 13,
      Magazine = 14,
      MeleeWeapon = 15,
      GlowStick = 16,
      Medpack = 17,
      HandheldAsteroidScanner = 18,
      HandDrill = 19,
      Battery = 20,
      HackingTool = 21,
      RespawnObject = 22,
      SpawnPoint = 23,
      Room = 24,
      LogItem = 25,
      ArenaController = 26,
      GenericItem = 27,
      Door = 28,
      Grenade = 28,
      DoomController = 29,
      Executer = 30,
      NameTag = 31,
      PortableTurret = 32,
      SpawnManager = 33,
      PlayerStash = 34,
      DockingPort = 35,
      RepairPoint = 36,
      RepairTool = 37,
    }
  }
}
