// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.SpaceObjectVessel
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using ZeroGravity.BulletPhysics;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.ShipComponents;

namespace ZeroGravity.Objects
{
  public abstract class SpaceObjectVessel : ArtificialBody
  {
    public static double ArenaRescueTime = TimeSpan.FromMinutes(30.0).TotalSeconds;
    public List<VesselPrimitiveColliderData> PrimitiveCollidersData = new List<VesselPrimitiveColliderData>();
    public List<VesselMeshColliderData> MeshCollidersData = new List<VesselMeshColliderData>();
    public SpaceObjectVessel DockedToMainVessel = (SpaceObjectVessel) null;
    public SpaceObjectVessel DockedToVessel = (SpaceObjectVessel) null;
    public List<SpaceObjectVessel> AllDockedVessels = new List<SpaceObjectVessel>();
    public List<SpaceObjectVessel> DockedVessels = new List<SpaceObjectVessel>();
    public List<SpaceObjectVessel.SpawnObjectsWithChance> SpawnChance = new List<SpaceObjectVessel.SpawnObjectsWithChance>();
    public List<Door> Doors = new List<Door>();
    public List<VesselDockingPort> DockingPorts = new List<VesselDockingPort>();
    public List<CargoBay> CargoBays = new List<CargoBay>();
    public List<NameTagData> NameTags = new List<NameTagData>();
    public List<Room> Rooms = new List<Room>();
    public List<VesselComponent> Systems = new List<VesselComponent>();
    public List<VesselRepairPoint> RepairPoints = new List<VesselRepairPoint>();
    public List<ShipSpawnPoint> SpawnPoints = new List<ShipSpawnPoint>();
    public List<Player> VesselCrew = new List<Player>();
    public List<AuthorizedPerson> AuthorizedPersonel = new List<AuthorizedPerson>();
    public DistributionManager DistributionManager = (DistributionManager) null;
    public DistributionManager CompoundDistributionManager = (DistributionManager) null;
    public Dictionary<VesselObjectID, AttachPointType> AttachPointsTypes = new Dictionary<VesselObjectID, AttachPointType>();
    public Dictionary<int, VesselAttachPoint> AttachPoints = new Dictionary<int, VesselAttachPoint>();
    public long StartingSetId = -1;
    private bool updateHealth = false;
    public RigidBody RigidBody;
    public Vector3D RelativePositionFromMainParent;
    public QuaternionD RelativeRotationFromMainParent;
    public Vector3D RelativePositionFromParent;
    public QuaternionD RelativeRotationFromParent;
    public VesselData VesselData;
    public VesselCaps VesselCaps;
    public float MaxHealth;
    public float Health;
    public float DecayRate;
    public float Temperature;
    public double Mass;
    public float HeatCollectionFactor;
    public float HeatDissipationFactor;
    public bool IsArenaVessel;

    public GameScenes.SceneID SceneID
    {
      get
      {
        return this.VesselData != null ? this.VesselData.SceneID : GameScenes.SceneID.None;
      }
    }

    public bool IsDocked
    {
      get
      {
        return this.DockedToMainVessel != null;
      }
    }

    public string FullName
    {
      get
      {
        return this.VesselData != null ? this.VesselData.VesselRegistration + " " + this.VesselData.VesselName : "";
      }
    }

    public bool HasSpawnPoints
    {
      get
      {
        return this.SpawnPoints != null && this.SpawnPoints.Count > 0;
      }
    }

    public bool HasSecuritySystem { get; protected set; }

    public DistributionManager MainDistributionManager
    {
      get
      {
        if (this.DockedToMainVessel != null)
          return this.DockedToMainVessel.CompoundDistributionManager;
        if (this.CompoundDistributionManager != null)
          return this.CompoundDistributionManager;
        return this.DistributionManager;
      }
    }

    public abstract void AddPlayerToCrew(Player pl);

    public abstract void RemovePlayerFromCrew(Player pl, bool checkDetails = false);

    public abstract bool HasPlayerInCrew(Player pl);

    public bool HasSpawnPointsInHierarchy()
    {
      if (this.SpawnPoints != null && this.SpawnPoints.Count > 0)
        return true;
      if (this.DockedToMainVessel != null)
      {
        foreach (SpaceObjectVessel allDockedVessel in this.DockedToMainVessel.AllDockedVessels)
        {
          if (allDockedVessel.SpawnPoints != null && allDockedVessel.SpawnPoints.Count > 0)
            return true;
        }
      }
      else if (this.AllDockedVessels.Count > 0)
      {
        foreach (SpaceObjectVessel allDockedVessel in this.AllDockedVessels)
        {
          if (allDockedVessel.SpawnPoints != null && allDockedVessel.SpawnPoints.Count > 0)
            return true;
        }
      }
      return false;
    }

    public ShipSpawnPoint GetPlayerSpawnPoint(Player pl)
    {
      if (this.SpawnPoints.Count > 0)
      {
        foreach (ShipSpawnPoint spawnPoint in this.SpawnPoints)
        {
          if (spawnPoint.Player == null && spawnPoint.Type == SpawnPointType.SimpleSpawn || spawnPoint.Player == pl || spawnPoint.InvitedPlayerSteamID == pl.SteamId)
            return spawnPoint;
        }
      }
      return (ShipSpawnPoint) null;
    }

    public bool HasEmptySimpleSpawnPoint()
    {
      return this.SpawnPoints != null && this.SpawnPoints.Find((Predicate<ShipSpawnPoint>) (m =>
      {
        if (m.Type == SpawnPointType.SimpleSpawn)
          return m.Player == null;
        return false;
      })) != null;
    }

    public SpaceObjectVessel(long guid, bool initializeOrbit, Vector3D position, Vector3D velocity, Vector3D forward, Vector3D up)
      : base(guid, initializeOrbit, position, velocity, forward, up)
    {
    }

    public ObjectTransform GetObjectTransform()
    {
      ObjectTransform objectTransform = new ObjectTransform()
      {
        GUID = this.GUID,
        Type = SpaceObjectType.Ship,
        Forward = this.Forward.ToFloatArray(),
        Up = this.Up.ToFloatArray()
      };
      if (this.Orbit.IsOrbitValid)
      {
        objectTransform.Orbit = new OrbitData()
        {
          ParentGUID = this.Orbit.Parent.CelestialBody.GUID
        };
        this.Orbit.FillOrbitData(ref objectTransform.Orbit, (SpaceObjectVessel) null);
      }
      else
        objectTransform.Realtime = new RealtimeData()
        {
          ParentGUID = this.Orbit.Parent.CelestialBody.GUID,
          Position = (this.Position - this.Orbit.Parent.Position).ToArray(),
          Velocity = this.Velocity.ToArray()
        };
      if (this.CurrentCourse != null)
        objectTransform.Maneuver = this.CurrentCourse.CurrentData();
      objectTransform.Distress = this.GetDistressData(true);
      return objectTransform;
    }

    public DistressData GetDistressData(bool returnNullIfOffline)
    {
      if (returnNullIfOffline && !this.IsDistresActive)
        return (DistressData) null;
      DistressData distressData = new DistressData();
      distressData.IsOn = this.IsDistresActive;
      distressData.Type = this.DistressType;
      if (this.IsDistresActive)
      {
        if (this.IsArenaVessel)
        {
          float num = (float) (SpaceObjectVessel.ArenaRescueTime - (Server.Instance.SolarSystem.CurrentTime - this.DistressActivatedTime));
          if ((double) num > 0.0)
            distressData.Time = new float?(num);
        }
        else if (this.DistressType == DistressType.Doomed && this is Ship && (this as Ship).IsDoomed)
          distressData.Time = new float?((this as Ship).TimeToLive);
      }
      return distressData;
    }

    public void ResetDockedToVessel()
    {
      foreach (SpaceObjectVessel dockedVessel in this.DockedVessels)
      {
        dockedVessel.DockedToVessel = this;
        dockedVessel.ResetDockedToVessel();
      }
    }

    public void SetMainVesselForChldren(SpaceObjectVessel newMainVessel)
    {
      foreach (SpaceObjectVessel dockedVessel in this.DockedVessels)
      {
        if (!newMainVessel.AllDockedVessels.Contains(dockedVessel))
        {
          dockedVessel.DockedToMainVessel = newMainVessel;
          newMainVessel.AllDockedVessels.Add(dockedVessel);
          dockedVessel.SetMainVesselForChldren(newMainVessel);
        }
      }
    }

    public void RecreateDockedVesselsTree()
    {
      this.DockedToMainVessel = (SpaceObjectVessel) null;
      this.DockedToVessel = (SpaceObjectVessel) null;
      this.RecreateDockedVesselsTree(this, (SpaceObjectVessel) null);
    }

    private void RecreateDockedVesselsTree(SpaceObjectVessel mainVessel, SpaceObjectVessel parentVessel)
    {
      this.AllDockedVessels.Clear();
      this.DockedVessels.Clear();
      foreach (VesselDockingPort dockingPort in this.DockingPorts)
      {
        if (dockingPort.DockedVessel != null && dockingPort.DockedVessel != parentVessel)
        {
          dockingPort.DockedVessel.DockedToMainVessel = mainVessel;
          dockingPort.DockedVessel.DockedToVessel = this;
          mainVessel.AllDockedVessels.Add(dockingPort.DockedVessel);
          this.DockedVessels.Add(dockingPort.DockedVessel);
          dockingPort.DockedVessel.RecreateDockedVesselsTree(mainVessel, this);
        }
      }
    }

    public void DbgLogDockedVesseslTree()
    {
      SpaceObjectVessel spaceObjectVessel = (SpaceObjectVessel) null;
      if (this.IsDocked)
        spaceObjectVessel = this.DockedToMainVessel;
      else if (this.AllDockedVessels.Count > 0)
        spaceObjectVessel = this;
      if (spaceObjectVessel == null)
        return;
      spaceObjectVessel.DbgLogDockedVesslesTreeWorker(1);
    }

    private void DbgLogDockedVesslesTreeWorker(int padding)
    {
      foreach (SpaceObjectVessel dockedVessel in this.DockedVessels)
        dockedVessel.DbgLogDockedVesslesTreeWorker(padding + 1);
    }

    public void FitMachineryPart(VesselObjectID slotID, MachineryPart part)
    {
      VesselComponent componentByPartSlot = this.DistributionManager.GetVesselComponentByPartSlot(slotID);
      if (componentByPartSlot == null)
        return;
      componentByPartSlot.FitPartToSlot(slotID, part);
    }

    public void SetPhysicsParameters()
    {
      if (this.RigidBody == null)
        return;
      this.RigidBody.MotionState = (MotionState) new DefaultMotionState(BulletHelper.AffineTransformation(1f, BulletHelper.LookRotation(this.Forward.ToVector3(), this.Up.ToVector3()), this.Position.ToVector3()));
      if (!this.RigidBody.IsActive)
        this.RigidBody.Activate(true);
      this.RigidBody.LinearVelocity = this.Velocity.ToVector3();
      this.RigidBody.AngularVelocity = this.AngularVelocity.ToVector3();
    }

    public void ReadPhysicsParameters()
    {
      if (this.RigidBody == null)
        return;
      this.PhysicsVelocityDifference = this.RigidBody.LinearVelocity.ToVector3D() - this.Velocity;
      this.PhysicsRotationDifference = QuaternionD.LookRotation(this.Forward, this.Up).Inverse() * (this.RigidBody.AngularVelocity.ToVector3D() - this.AngularVelocity) * (180.0 / System.Math.PI);
    }

    public void RemoveMachineryPart(VesselObjectID slotID)
    {
      VesselComponent componentByPartSlot = this.DistributionManager.GetVesselComponentByPartSlot(slotID);
      if (componentByPartSlot == null)
        return;
      componentByPartSlot.RemovePartFromSlot(slotID);
    }

    public virtual void SendSpawnMessage(long clientID, bool isDummy)
    {
    }

    public virtual void SetRadius(double radius)
    {
      this.Radius = radius;
    }

    protected void fillVesselCaps()
    {
      if (this.VesselCaps == null)
        this.VesselCaps = new VesselCaps();
      if (this.DistributionManager.RCS != null)
      {
        this.VesselCaps.RcsAcceleration = this.DistributionManager.RCS.Acceleration;
        this.VesselCaps.RotationAcceleration = this.DistributionManager.RCS.RotationAcceleration;
        this.VesselCaps.RotationStabilizationDeacceleration = this.DistributionManager.RCS.RotationStabilization;
      }
      if (this.DistributionManager.Engine != null)
      {
        this.VesselCaps.EngineAcceleration = this.DistributionManager.Engine.Acceleration;
        this.VesselCaps.EngineReverseAcceleration = this.DistributionManager.Engine.ReverseAcceleration;
        this.VesselCaps.EngineAccelerationBuildup = this.DistributionManager.Engine.AccelerationBuildup;
      }
      SubSystemFTL ftl = this.DistributionManager.FTL;
      if (ftl == null)
        return;
      this.VesselCaps.WarpAcceleration = ftl.WarpsData.Length != 0 ? (float) ftl.WarpsData[0].MaxAcceleration : 0.0f;
    }

    public List<PairedDoorsDetails> GetPairedDoors(VesselDockingPort port)
    {
      List<PairedDoorsDetails> pairedDoorsDetailsList = new List<PairedDoorsDetails>();
      foreach (short doorsId in port.DoorsIDs)
      {
        Door door = this.DistributionManager.GetDoor(new VesselObjectID(port.ID.VesselGUID, (int) doorsId));
        if (door != null)
          pairedDoorsDetailsList.Add(new PairedDoorsDetails()
          {
            DoorID = door.ID,
            PairedDoorID = door.PairedDoorID
          });
      }
      return pairedDoorsDetailsList;
    }

    private List<string> ReturnTags(string tag)
    {
      List<string> stringList = new List<string>();
      if (tag.IsNullOrEmpty())
      {
        stringList.Add("None");
      }
      else
      {
        string str = tag;
        char[] chArray = new char[1]{ ';' };
        foreach (string val in str.Split(chArray))
        {
          if (val.IsNullOrEmpty())
            stringList.Add("None");
          else
            stringList.Add(val);
        }
      }
      return stringList;
    }

    private bool CompareTags(List<string> shipTags, List<string> objectTags)
    {
      foreach (string shipTag in shipTags)
      {
        foreach (string objectTag in objectTags)
        {
          if (shipTag == objectTag || objectTag == "*")
            return true;
        }
      }
      return false;
    }

    public bool CheckTag(string tag, SpawnSettingsCase ssCase = SpawnSettingsCase.EnableIf)
    {
      bool flag = this.CompareTags(this.ReturnTags(this.VesselData.Tag), this.ReturnTags(tag));
      return flag && ssCase == SpawnSettingsCase.EnableIf || !flag && ssCase == SpawnSettingsCase.DisableIf;
    }

    private void CopyAuthorizedPersonelListFromShip(SpaceObjectVessel fromVessel)
    {
      if (!this.HasSecuritySystem || fromVessel == this || GameScenes.Ranges.IsShip(this.SceneID) || GameScenes.Ranges.IsShip(fromVessel.SceneID))
        return;
      this.AuthorizedPersonel.Clear();
      this.AuthorizedPersonel = new List<AuthorizedPerson>((IEnumerable<AuthorizedPerson>) fromVessel.AuthorizedPersonel);
    }

    public void CopyAuthorizedPersonelListToChildren()
    {
      if (this.DockedToMainVessel != null)
      {
        foreach (SpaceObjectVessel allDockedVessel in this.DockedToMainVessel.AllDockedVessels)
          allDockedVessel.CopyAuthorizedPersonelListFromShip(this);
      }
      else
      {
        if (this.AllDockedVessels.Count <= 0)
          return;
        foreach (SpaceObjectVessel allDockedVessel in this.AllDockedVessels)
          allDockedVessel.CopyAuthorizedPersonelListFromShip(this);
      }
    }

    private void UpdateAuthorizationData(Player pl)
    {
      AuthorizedPerson authorizedPerson = this.AuthorizedPersonel.Find((Predicate<AuthorizedPerson>) (m =>
      {
        if (m.PlayerGUID != pl.GUID)
          return m.SteamID == pl.SteamId;
        return true;
      }));
      if (authorizedPerson == null)
        return;
      authorizedPerson.PlayerGUID = pl.GUID;
      authorizedPerson.Name = pl.Name;
    }

    public bool AddAuthorizedPerson(Player executingPl, Player pl, AuthorizedPersonRank rank)
    {
      return this.AddAuthorizedPerson_Impl(executingPl, pl, (string) null, (string) null, rank);
    }

    public bool AddAuthorizedPerson(Player executingPl, string steamID, string name, AuthorizedPersonRank rank)
    {
      return this.AddAuthorizedPerson_Impl(executingPl, Server.Instance.GetPlayerFromSteamID(steamID), steamID, name, rank);
    }

    private void AddModifyPlayerPosition(long guid, string steamId, string name, AuthorizedPersonRank rank)
    {
      AuthorizedPerson authorizedPerson1 = this.AuthorizedPersonel.Find((Predicate<AuthorizedPerson>) (m => m.SteamID == steamId));
      if (authorizedPerson1 != null)
      {
        authorizedPerson1.Rank = rank;
        authorizedPerson1.SteamID = steamId;
        authorizedPerson1.Name = name;
        authorizedPerson1.PlayerGUID = guid;
      }
      else
      {
        List<AuthorizedPerson> authorizedPersonel = this.AuthorizedPersonel;
        AuthorizedPerson authorizedPerson2 = new AuthorizedPerson();
        authorizedPerson2.PlayerGUID = guid;
        string str1 = steamId;
        authorizedPerson2.SteamID = str1;
        string str2 = name;
        authorizedPerson2.Name = str2;
        int num = (int) rank;
        authorizedPerson2.Rank = (AuthorizedPersonRank) num;
        authorizedPersonel.Add(authorizedPerson2);
      }
    }

    private void AddModifyPlayerPosition(Player pl, AuthorizedPersonRank rank)
    {
      if (pl == null)
        return;
      this.AddModifyPlayerPosition(pl.GUID, pl.SteamId, pl.Name, rank);
    }

    private bool AddAuthorizedPerson_Impl(Player executingPl, Player pl, string steamID, string name, AuthorizedPersonRank rank)
    {
      if (executingPl == null || pl == null && steamID.IsNullOrEmpty())
        return false;
      this.UpdateAuthorizationData(executingPl);
      if (pl != null)
        steamID = pl.SteamId;
      AuthorizedPerson authorizedPerson1 = this.AuthorizedPersonel.Find((Predicate<AuthorizedPerson>) (m => m.Rank == AuthorizedPersonRank.CommandingOfficer));
      AuthorizedPerson authorizedPerson2 = this.AuthorizedPersonel.Find((Predicate<AuthorizedPerson>) (m => m.Rank == AuthorizedPersonRank.ExecutiveOfficer));
      bool flag1 = authorizedPerson1 == null || authorizedPerson1.PlayerGUID == executingPl.GUID;
      bool flag2 = authorizedPerson2 == null || authorizedPerson2.PlayerGUID == executingPl.GUID;
      bool flag3 = false;
      if (rank == AuthorizedPersonRank.CommandingOfficer & flag1)
      {
        if (authorizedPerson1 != null && authorizedPerson1.SteamID != steamID)
          this.AddModifyPlayerPosition(authorizedPerson1.PlayerGUID, authorizedPerson1.SteamID, authorizedPerson1.Name, AuthorizedPersonRank.Crewman);
        flag3 = true;
      }
      else if (rank == AuthorizedPersonRank.ExecutiveOfficer && flag2 | flag1)
      {
        if (authorizedPerson2 != null && authorizedPerson2.SteamID != steamID)
          this.AddModifyPlayerPosition(authorizedPerson2.PlayerGUID, authorizedPerson2.SteamID, authorizedPerson2.Name, AuthorizedPersonRank.Crewman);
        flag3 = true;
      }
      else if (rank == AuthorizedPersonRank.Crewman && flag2 | flag1)
        flag3 = true;
      if (!flag3)
        return false;
      if (pl != null)
        this.AddModifyPlayerPosition(pl, rank);
      else
        this.AddModifyPlayerPosition(0L, steamID, name, rank);
      this.CopyAuthorizedPersonelListToChildren();
      return true;
    }

    public bool RemoveAuthorizedPerson(Player executingPl, Player pl)
    {
      return this.RemoveAuthorizedPerson_Impl(executingPl, pl, (string) null);
    }

    public bool RemoveAuthorizedPerson(Player executingPl, string steamID)
    {
      return this.RemoveAuthorizedPerson_Impl(executingPl, Server.Instance.GetPlayerFromSteamID(steamID), steamID);
    }

    private bool RemoveAuthorizedPerson_Impl(Player executingPl, Player pl, string steamID)
    {
      if (executingPl == null || pl == null && steamID.IsNullOrEmpty())
        return false;
      if (pl != null)
      {
        this.UpdateAuthorizationData(pl);
        steamID = pl.SteamId;
      }
      AuthorizedPerson authorizedPerson = this.AuthorizedPersonel.Find((Predicate<AuthorizedPerson>) (m => m.SteamID == steamID));
      if (authorizedPerson == null)
        return false;
      bool flag1 = this.AuthorizedPersonel.Find((Predicate<AuthorizedPerson>) (m =>
      {
        if (m.PlayerGUID == executingPl.GUID)
          return m.Rank == AuthorizedPersonRank.CommandingOfficer;
        return false;
      })) != null;
      bool flag2 = this.AuthorizedPersonel.Find((Predicate<AuthorizedPerson>) (m =>
      {
        if (m.PlayerGUID == executingPl.GUID)
          return m.Rank == AuthorizedPersonRank.ExecutiveOfficer;
        return false;
      })) != null;
      bool flag3 = false;
      if (executingPl == pl)
        flag3 = true;
      else if (authorizedPerson.Rank == AuthorizedPersonRank.ExecutiveOfficer & flag1)
        flag3 = true;
      else if (authorizedPerson.Rank == AuthorizedPersonRank.Crewman && flag1 | flag2)
        flag3 = true;
      if (!flag3)
        return false;
      this.AuthorizedPersonel.Remove(authorizedPerson);
      this.CopyAuthorizedPersonelListToChildren();
      return true;
    }

    public bool ChangeVesselName(Player pl, string newName)
    {
      if (pl == null || this.AuthorizedPersonel.Find((Predicate<AuthorizedPerson>) (m =>
      {
        if (m.PlayerGUID == pl.GUID || m.SteamID == pl.SteamId)
          return m.Rank == AuthorizedPersonRank.CommandingOfficer;
        return false;
      })) == null)
        return false;
      this.VesselData.VesselName = newName;
      return true;
    }

    public bool ClearSecuritySystem(Player pl)
    {
      if (pl == null || pl.CurrentActiveItem == null || !ItemTypeRange.IsHackingTool(pl.CurrentActiveItem.Type) || pl.Parent != this)
        return false;
      this.AuthorizedPersonel.Clear();
      this.CopyAuthorizedPersonelListToChildren();
      pl.CurrentActiveItem.ChangeStats((DynamicObjectStats) new DisposableHackingToolStats()
      {
        Use = true
      });
      return true;
    }

    public VesselSecurityData GetVesselSecurityData(bool includeName = false)
    {
      List<VesselSecurityAuthorizedPerson> authorizedPersonList = new List<VesselSecurityAuthorizedPerson>();
      foreach (AuthorizedPerson authorizedPerson in this.AuthorizedPersonel)
      {
        Player player = Server.Instance.GetPlayer(authorizedPerson.PlayerGUID);
        authorizedPersonList.Add(new VesselSecurityAuthorizedPerson()
        {
          SteamID = authorizedPerson.SteamID,
          GUID = player != null ? player.FakeGuid : 0L,
          Name = player != null ? player.Name : authorizedPerson.Name,
          Rank = authorizedPerson.Rank
        });
      }
      if (includeName)
        return new VesselSecurityData()
        {
          VesselName = this.VesselData.VesselName,
          AuthorizedPersonel = authorizedPersonList,
          AuthorizedPersonelCount = new int?(authorizedPersonList.Count)
        };
      VesselSecurityData vesselSecurityData = new VesselSecurityData();
      vesselSecurityData.AuthorizedPersonel = authorizedPersonList;
      int? nullable = new int?(authorizedPersonList.Count);
      vesselSecurityData.AuthorizedPersonelCount = nullable;
      return vesselSecurityData;
    }

    public void SendSecurityResponse(bool includeVesselName, bool sendForAllChildren = true)
    {
      if (this.HasSecuritySystem)
      {
        NetworkController networkController = Server.Instance.NetworkController;
        VesselSecurityResponse securityResponse = new VesselSecurityResponse();
        securityResponse.VesselGUID = this.GUID;
        securityResponse.Data = this.GetVesselSecurityData(includeVesselName);
        long skipPlalerGUID = -1;
        SpaceObject[] spaceObjectArray = new SpaceObject[1]
        {
          (SpaceObject) this
        };
        networkController.SendToClientsSubscribedTo((NetworkData) securityResponse, skipPlalerGUID, spaceObjectArray);
      }
      if (!sendForAllChildren)
        return;
      if (this.DockedToMainVessel != null)
      {
        foreach (SpaceObjectVessel allDockedVessel in this.DockedToMainVessel.AllDockedVessels)
          allDockedVessel.SendSecurityResponse(includeVesselName, false);
      }
      else if (this.AllDockedVessels.Count > 0)
      {
        foreach (SpaceObjectVessel allDockedVessel in this.AllDockedVessels)
          allDockedVessel.SendSecurityResponse(includeVesselName, false);
      }
    }

    public virtual void UpdateVesselSystems()
    {
    }

    public override void UpdateTimers(double deltaTime)
    {
      base.UpdateTimers(deltaTime);
    }

    public virtual float ChangeHealthBy(float value, List<VesselRepairPoint> repairPoints = null)
    {
      return 0.0f;
    }

    public void DamageVesselsInExplosionRadius()
    {
      float radius = (float) ((System.Math.Sqrt(this.Mass) + (this.DistributionManager.Engine == null || this.DistributionManager.Engine.Status != SystemStatus.OnLine ? 0.0 : 10.0) + (this.DistributionManager.FTL == null || this.DistributionManager.FTL.Status != SystemStatus.OnLine ? 0.0 : 20.0)) * Server.VesselExplosionRadiusMultiplier);
      float num1 = this.MaxHealth * (float) Server.VesselExplosionDamageMultiplier;
      Vector3D vector3D1;
      foreach (ArtificialBody artificialBody in Server.Instance.SolarSystem.GetArtificialBodieslsInRange((ArtificialBody) this, radius))
      {
        if (artificialBody is SpaceObjectVessel)
        {
          SpaceObjectVessel spaceObjectVessel1 = artificialBody as SpaceObjectVessel;
          vector3D1 = this.Position - spaceObjectVessel1.Position;
          float magnitude = (float) vector3D1.Magnitude;
          float num2 = MathHelper.Clamp((radius - magnitude) / radius, 0.0f, 1f);
          double num3 = (double) spaceObjectVessel1.ChangeHealthBy(-num2 * num1, (List<VesselRepairPoint>) null);
          vector3D1 = spaceObjectVessel1.Position - this.Position;
          Vector3D vector3D2 = vector3D1.Normalized * 5.0 * (double) num2;
          SpaceObjectVessel spaceObjectVessel2 = spaceObjectVessel1;
          Vector3D vector3D3 = spaceObjectVessel2.AngularVelocity + new Vector3D(MathHelper.RandomNextDouble(), MathHelper.RandomNextDouble(), MathHelper.RandomNextDouble()) * 5.0 * (double) num2;
          spaceObjectVessel2.AngularVelocity = vector3D3;
          spaceObjectVessel1.Orbit.InitFromStateVectors(spaceObjectVessel1.Orbit.Parent, spaceObjectVessel1.Orbit.Position, spaceObjectVessel1.Orbit.Velocity + vector3D2, Server.Instance.SolarSystem.CurrentTime, false);
        }
        else if (artificialBody is Pivot)
        {
          Pivot pivot = artificialBody as Pivot;
          Vector3D vector3D2 = pivot.Position + pivot.Child.LocalRotation * pivot.Child.LocalPosition;
          vector3D1 = this.Position - vector3D2;
          float magnitude = (float) vector3D1.Magnitude;
          float num2 = MathHelper.Clamp((radius - magnitude) / radius, 0.0f, 1f);
          if (pivot.Child is Player)
            (pivot.Child as Player).Stats.TakeDammage(0.0f, new Vector3D?(), 0.0f, 0.0f, 0.0f, num2 * num1, 0.0f);
          vector3D1 = vector3D2 - this.Position;
          Vector3D vector3D3 = vector3D1.Normalized * 10.0 * (double) num2;
          pivot.Orbit.InitFromStateVectors(pivot.Orbit.Parent, pivot.Orbit.Position, pivot.Orbit.Velocity + vector3D3, Server.Instance.SolarSystem.CurrentTime, false);
        }
      }
    }

    public double GetCompoundMass()
    {
      SpaceObjectVessel spaceObjectVessel1 = this.DockedToMainVessel != null ? this.DockedToMainVessel : this;
      double mass = spaceObjectVessel1.Mass;
      foreach (SpaceObjectVessel spaceObjectVessel2 in spaceObjectVessel1.AllDockedVessels.Where<SpaceObjectVessel>((Func<SpaceObjectVessel, bool>) (v => v.GUID != this.GUID)))
        mass += spaceObjectVessel2.Mass;
      return mass;
    }

    public List<VesselRepairPointDetails> GetVesselRepairPointsDetails(bool changedOnly)
    {
      List<VesselRepairPointDetails> repairPointDetailsList = new List<VesselRepairPointDetails>();
      foreach (VesselRepairPoint repairPoint in this.RepairPoints)
      {
        if (!changedOnly || repairPoint.StatusChanged)
        {
          repairPointDetailsList.Add(repairPoint.GetDetails());
          repairPoint.StatusChanged = false;
        }
      }
      return repairPointDetailsList;
    }

    internal void UndockAll()
    {
      List<ShipStatsMessage> shipStatsMessageList = new List<ShipStatsMessage>();
      foreach (SpaceObjectVessel spaceObjectVessel in this.DockedVessels.Where<SpaceObjectVessel>((Func<SpaceObjectVessel, bool>) (m => m is Ship)))
      {
        VesselDockingPort vesselDockingPort = spaceObjectVessel.DockingPorts.Find((Predicate<VesselDockingPort>) (m =>
        {
          if (m.DockingStatus)
            return m.DockedVessel == this;
          return false;
        }));
        if (vesselDockingPort != null)
        {
          SceneDockingPortDetails details = vesselDockingPort.GetDetails();
          details.DockingStatus = false;
          shipStatsMessageList.Add(new ShipStatsMessage()
          {
            GUID = spaceObjectVessel.GUID,
            VesselObjects = new VesselObjects()
            {
              DockingPorts = new List<SceneDockingPortDetails>()
              {
                details
              }
            }
          });
        }
      }
      if (this.DockedToVessel != null && this.DockedToVessel is Ship)
      {
        VesselDockingPort vesselDockingPort = this.DockingPorts.Find((Predicate<VesselDockingPort>) (m =>
        {
          if (m.DockingStatus)
            return m.DockedVessel == this.DockedToVessel;
          return false;
        }));
        if (vesselDockingPort != null)
        {
          SceneDockingPortDetails details = vesselDockingPort.GetDetails();
          details.DockingStatus = false;
          shipStatsMessageList.Add(new ShipStatsMessage()
          {
            GUID = this.GUID,
            VesselObjects = new VesselObjects()
            {
              DockingPorts = new List<SceneDockingPortDetails>()
              {
                details
              }
            }
          });
        }
      }
      foreach (ShipStatsMessage shipStatsMessage in shipStatsMessageList)
        (Server.Instance.GetVessel(shipStatsMessage.GUID) as Ship).ShipStatsMessageListener((NetworkData) shipStatsMessage);
    }

    protected SubSystem createSubSystem(SubSystemData ssData)
    {
      VesselObjectID id = new VesselObjectID(this.GUID, ssData.InSceneID);
      SubSystem subSystem;
      if (ssData.Type == SubSystemType.Light)
        subSystem = (SubSystem) new SubSystemLights(this, id, ssData);
      else if (ssData.Type == SubSystemType.EmergencyLight)
        subSystem = (SubSystem) new SubSystemEmergencyLights(this, id, ssData);
      else if (ssData.Type == SubSystemType.AirDevice)
        subSystem = (SubSystem) new SubSystemAirDevice(this, id, ssData);
      else if (ssData.Type == SubSystemType.ScrubberDevice)
        subSystem = (SubSystem) new SubSystemScrubberDevice(this, id, ssData);
      else if (ssData.Type == SubSystemType.ACDevice)
        subSystem = (SubSystem) new SubSystemACDevice(this, id, ssData);
      else if (ssData.Type == SubSystemType.AirVent)
        subSystem = (SubSystem) new SubSystemAirVent(this, id, ssData);
      else if (ssData.Type == SubSystemType.PDU)
        subSystem = (SubSystem) new SubSystemPDU(this, id, ssData);
      else if (ssData.Type == SubSystemType.RCS)
        subSystem = (SubSystem) new SubSystemRCS(this, id, ssData);
      else if (ssData.Type == SubSystemType.Engine)
        subSystem = (SubSystem) new SubSystemEngine(this, id, ssData);
      else if (ssData.Type == SubSystemType.FTL)
        subSystem = (SubSystem) new SubSystemFTL(this, id, ssData);
      else if (ssData.Type == SubSystemType.Refinery)
        subSystem = (SubSystem) new SubSystemRefinery(this, id, ssData);
      else
        subSystem = (SubSystem) new SubSystemFabricator(this, id, ssData);
      return subSystem;
    }

    protected Generator createGenerator(GeneratorData genData)
    {
      VesselObjectID id = new VesselObjectID(this.GUID, genData.InSceneID);
      Generator generator;
      if (genData.Type == GeneratorType.Power)
        generator = (Generator) new GeneratorPower(this, id, genData);
      else if (genData.Type == GeneratorType.Air)
        generator = (Generator) new GeneratorAir(this, id, genData);
      else if (genData.Type == GeneratorType.AirScrubber)
        generator = (Generator) new GeneratorScrubber(this, id, genData);
      else if (genData.Type == GeneratorType.Capacitor)
      {
        generator = (Generator) new GeneratorCapacitor(this, id, genData);
      }
      else
      {
        if (genData.Type != GeneratorType.Solar)
          throw new Exception("Unsupported generator type " + (object) genData.Type);
        generator = (Generator) new GeneratorSolar(this, id, genData);
      }
      return generator;
    }

    public class SpawnObjectsWithChance
    {
      public int InSceneID;
      public float Chance;
    }
  }
}
