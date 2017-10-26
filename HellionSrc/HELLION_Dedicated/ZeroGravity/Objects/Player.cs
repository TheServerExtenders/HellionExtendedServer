// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Player
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Helpers;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.ShipComponents;

namespace ZeroGravity.Objects
{
  public class Player : SpaceObjectTransferable, IPersistantObject, IAirConsumer
  {
    public double LastMovementMessageSolarSystemTime = -1.0;
    public List<long> UpdateArtificialBodyMovement = new List<long>();
    public bool IsAlive = false;
    public bool EnviromentReady = false;
    public bool PlayerReady = false;
    private HashSet<long> subscribedToSpaceObjects = new HashSet<long>();
    private Vector3D velocity = Vector3D.Zero;
    private Vector3D angularVelocity = Vector3D.Zero;
    public ThreadSafeList<CharacterTransformData> TransformDataList = new ThreadSafeList<CharacterTransformData>();
    private Vector3D pivotPositionCorrection = Vector3D.Zero;
    private Vector3D pivotVelocityCorrection = Vector3D.Zero;
    private Vector3D? dockUndockPositionCorrection = new Vector3D?();
    private QuaternionD? dockUndockRotationCorrection = new QuaternionD?();
    private bool dockUndockWaitForMsg = false;
    private SpaceObject _Parent = (SpaceObject) null;
    private VesselObjectID _currentRoomID = (VesselObjectID) null;
    public float CoreTemperature = 37f;
    public List<ShipStatsMessage> MessagesReceivedWhileLoading = new List<ShipStatsMessage>();
    public string Name;
    public string SteamId;
    public Gender Gender;
    public byte HeadType;
    public byte HairType;
    public float MouseLook;
    public float FreeLookX;
    public float FreeLookY;
    public float CameraY;
    public Vector3D ZeroGOrientation;
    public long FakeGuid;
    public CharacterAnimationData AnimationData;
    public int AnimationStatsMask;
    private float[] gravity;
    private Helmet currentHelmet;
    private Jetpack currentJetpack;
    public Dictionary<byte, RagdollItemData> RagdollData;
    private sbyte[] jetpackDirection;
    public const float NoAirMaxDamage = 1f;
    public const float NoPressureMaxDamage = 2f;
    public const float TemperatureMaxDamage = 0.5f;
    public double updateItemTimer;
    public const float timeToUpdateItems = 0.3f;
    private Room currentRoom;
    private bool isOutsideRoom;
    public Inventory PlayerInventory;
    private bool outfitTempRegulationActive;
    private bool IsUsingActiveItem;
    private long PrevNearestVesselGUID;
    private double lastPivotResetTime;
    private double lateDisconnectWait;
    public bool IsInsideSpawnPoint;

    public override SpaceObjectType ObjectType
    {
      get
      {
        return SpaceObjectType.Player;
      }
    }

    public PlayerStats Stats { get; private set; }

    public ShipSpawnPoint CurrentSpawnPoint { get; private set; }

    public ShipSpawnPoint AuthorizedSpawnPoint { get; private set; }

    public Helmet CurrentHelmet
    {
      get
      {
        return this.currentHelmet;
      }
      set
      {
        this.currentHelmet = value;
        if (value != null || this.CurrentJetpack == null)
          return;
        this.CurrentJetpack.Helmet = (Helmet) null;
      }
    }

    public Jetpack CurrentJetpack
    {
      get
      {
        return this.currentJetpack;
      }
      set
      {
        this.currentJetpack = value;
        if (value != null || this.CurrentHelmet == null)
          return;
        this.CurrentHelmet.Jetpack = value;
      }
    }

    public Item CurrentActiveItem
    {
      get
      {
        return this.PlayerInventory.HandsSlot.SlotItem;
      }
    }

    public int Health
    {
      get
      {
        return (int) this.Stats.HealthPoints;
      }
      set
      {
        this.Stats.HealthPoints = (float) MathHelper.Clamp(value, 0, 100);
      }
    }

    public override SpaceObject Parent
    {
      get
      {
        return this._Parent;
      }
      set
      {
        if (this._Parent != null && this._Parent is SpaceObjectVessel)
          ((SpaceObjectVessel) this._Parent).RemovePlayerFromCrew(this, false);
        this._Parent = value;
        if (this._Parent == null || !(this._Parent is SpaceObjectVessel))
          return;
        ((SpaceObjectVessel) this._Parent).AddPlayerToCrew(this);
      }
    }

    public VesselObjectID CurrentRoomID
    {
      get
      {
        return this._currentRoomID;
      }
      set
      {
        this._currentRoomID = value;
        if (this._currentRoomID != null)
        {
          Ship ship = this.Parent.GUID == this._currentRoomID.VesselGUID ? this.Parent as Ship : Server.Instance.GetObject(this._currentRoomID.VesselGUID) as Ship;
          if (ship != null)
            this.currentRoom = ship.MainDistributionManager.GetRoom(value);
          else
            this.currentRoom = (Room) null;
        }
        else
          this.currentRoom = (Room) null;
      }
    }

    public bool GodMode
    {
      get
      {
        return this.Stats.GodMode;
      }
    }

    public float? AmbientTemperature
    {
      get
      {
        if (this.PlayerInventory.CurrOutfit != null)
          return new float?(this.PlayerInventory.CurrOutfit.InternalTemperature);
        if (this.Parent is SpaceObjectVessel)
          return new float?((this.Parent as SpaceObjectVessel).Temperature);
        return new float?();
      }
    }

    public Player(long guid, Vector3D localPosition, QuaternionD localRotation, string name, string steamId, Gender gender, byte headType, byte hairType, bool addToServerList = true)
      : base(guid, localPosition, localRotation)
    {
      this.FakeGuid = GUIDFactory.NextPlayerFakeGUID();
      this.Name = name;
      this.SteamId = steamId;
      this.Gender = gender;
      this.HeadType = headType;
      this.HairType = hairType;
      this.Stats = new PlayerStats();
      this.Stats.pl = this;
      this.PlayerInventory = new Inventory(this);
      if (!addToServerList)
        return;
      Server.Instance.Add(this);
    }

    public void ConnectToNetworkController()
    {
      this.EnviromentReady = false;
      this.PlayerReady = false;
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (CharacterMovementMessage), new EventSystem.NetworkDataDelegate(this.UpdateMovementListener));
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (EnvironmentReadyMessage), new EventSystem.NetworkDataDelegate(this.EnvironmentReadyListener));
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (PlayerShootingMessage), new EventSystem.NetworkDataDelegate(this.PlayerShootingListener));
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (PlayerHitMessage), new EventSystem.NetworkDataDelegate(this.PlayerHitListener));
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (PlayerStatsMessage), new EventSystem.NetworkDataDelegate(this.PlayerStatsMessageListener));
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (PlayerDrillingMessage), new EventSystem.NetworkDataDelegate(this.PlayerDrillingListener));
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (ToggleGodModeMessage), new EventSystem.NetworkDataDelegate(this.ToggleGodModeListener));
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (PlayerRoomMessage), new EventSystem.NetworkDataDelegate(this.PlayerRoomMessageListener));
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (SuicideRequest), new EventSystem.NetworkDataDelegate(this.SuicideListener));
    }

    public void DiconnectFromNetworkContoller()
    {
      this.EnviromentReady = false;
      this.PlayerReady = false;
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (CharacterMovementMessage), new EventSystem.NetworkDataDelegate(this.UpdateMovementListener));
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (EnvironmentReadyMessage), new EventSystem.NetworkDataDelegate(this.EnvironmentReadyListener));
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (PlayerShootingMessage), new EventSystem.NetworkDataDelegate(this.PlayerShootingListener));
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (PlayerHitMessage), new EventSystem.NetworkDataDelegate(this.PlayerHitListener));
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (PlayerStatsMessage), new EventSystem.NetworkDataDelegate(this.PlayerStatsMessageListener));
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (PlayerDrillingMessage), new EventSystem.NetworkDataDelegate(this.PlayerDrillingListener));
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (ToggleGodModeMessage), new EventSystem.NetworkDataDelegate(this.ToggleGodModeListener));
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (PlayerRoomMessage), new EventSystem.NetworkDataDelegate(this.PlayerRoomMessageListener));
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (SuicideRequest), new EventSystem.NetworkDataDelegate(this.SuicideListener));
    }

    private void ToggleGodModeListener(NetworkData data)
    {
    }

    private void PlayerStatsMessageListener(NetworkData data)
    {
      PlayerStatsMessage playerStatsMessage = data as PlayerStatsMessage;
      if (this.FakeGuid != playerStatsMessage.GUID)
        return;
      if (playerStatsMessage.AnimationMaskChanged.HasValue && playerStatsMessage.AnimationMaskChanged.Value)
        this.AnimationStatsMask = playerStatsMessage.AnimationStatesMask;
      else
        playerStatsMessage.AnimationStatesMask = this.AnimationStatsMask;
      Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) playerStatsMessage, data.Sender, this.Parent);
    }

    protected void PlayerHitListener(NetworkData data)
    {
      PlayerHitMessage playerHitMessage = data as PlayerHitMessage;
      if (playerHitMessage.Sender != this.GUID)
        return;
      if (playerHitMessage.HitSuccessfull || MathHelper.RandomNextDouble() > 0.699999988079071)
      {
        if (this.Stats.TakeHitDamage(playerHitMessage.HitIndentifier))
          Server.Instance.NetworkController.SendToGameClient(this.GUID, (NetworkData) new PlayerStatsMessage()
          {
            HealthPoints = (int) this.Stats.HealthPoints,
            GUID = this.FakeGuid
          });
      }
      else
        this.Stats.UnqueueHit(playerHitMessage.HitIndentifier);
    }

    private static void PassTroughtShootMessage(PlayerShootingMessage psm)
    {
      Server.Instance.NetworkController.SendToAllClients((NetworkData) new PlayerShootingMessage()
      {
        HitIndentifier = -1,
        ShotData = psm.ShotData,
        HitGUID = psm.HitGUID,
        GUID = psm.GUID
      }, psm.Sender);
    }

    protected void PlayerShootingListener(NetworkData data)
    {
      PlayerShootingMessage psm = data as PlayerShootingMessage;
      Weapon handsItemIfType = this.PlayerInventory.GetHandsItemIfType<Weapon>() as Weapon;
      if (handsItemIfType == null && !psm.ShotData.IsMeleeAttack || data.Sender != this.GUID)
        return;
      bool flag = false;
      if (psm.ShotData.IsMeleeAttack)
      {
        if (Server.Instance.SolarSystem.CurrentTime - this.Stats.lastMeleeTime > 1.0)
        {
          flag = true;
          this.Stats.lastMeleeTime = Server.Instance.SolarSystem.CurrentTime;
        }
      }
      else if (handsItemIfType != null && handsItemIfType.CanShoot())
        flag = true;
      if (flag)
      {
        if (psm.HitGUID == -1L)
        {
          psm.HitGUID = -2L;
          Player.PassTroughtShootMessage(psm);
        }
        else
        {
          SpaceObject spaceObject = Server.Instance.GetObject(psm.HitGUID);
          float damage = handsItemIfType == null ? (psm.ShotData.IsMeleeAttack ? 30f : 0.0f) : (psm.ShotData.IsMeleeAttack ? handsItemIfType.MeleeDamage : handsItemIfType.Damage);
          if (spaceObject is DynamicObject)
            (spaceObject as DynamicObject).Item.TakeDamage(new Dictionary<TypeOfDamage, float>()
            {
              {
                TypeOfDamage.Hit,
                damage
              }
            });
          Player player = Server.Instance.GetObject(psm.HitGUID) as Player;
          if (player != null)
          {
            if (Server.Instance.NetworkController.clientList.ContainsKey(player.GUID))
            {
              Player.PassTroughtShootMessage(psm);
              if (this.CurrentActiveItem is Weapon && (player.Position - this.Position).Magnitude <= (double) (this.CurrentActiveItem as Weapon).CurrentMod.Range || psm.ShotData.IsMeleeAttack)
                player.Stats.TakeHitDamage(player.Stats.QueueHit((PlayerStats.HitBoxType) psm.ShotData.colliderType, damage, psm.ShotData.Orientation.ToVector3D(), psm.ShotData.IsMeleeAttack));
            }
            else
              player.Stats.TakeHitDamage(player.Stats.QueueHit((PlayerStats.HitBoxType) psm.ShotData.colliderType, damage, psm.ShotData.Orientation.ToVector3D(), psm.ShotData.IsMeleeAttack));
          }
        }
      }
    }

    protected void PlayerDrillingListener(NetworkData data)
    {
      PlayerDrillingMessage playerDrillingMessage = data as PlayerDrillingMessage;
      if (data.Sender != this.GUID || this.CurrentActiveItem == null || !(this.CurrentActiveItem is HandDrill))
        return;
      if (playerDrillingMessage.isDrilling.HasValue)
        this.IsUsingActiveItem = playerDrillingMessage.isDrilling.Value;
      Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) new PlayerDrillingMessage()
      {
        DrillersGUID = new long?(this.FakeGuid),
        dontPlayEffect = playerDrillingMessage.dontPlayEffect,
        isDrilling = playerDrillingMessage.isDrilling
      }, this.GUID, this.Parent);
    }

    protected void EnvironmentReadyListener(NetworkData data)
    {
      if ((data as EnvironmentReadyMessage).Sender != this.GUID)
        return;
      this.EnviromentReady = true;
      SpawnObjectsResponse res = new SpawnObjectsResponse();
      Server.Instance.NetworkController.AddCharacterSpawnsToResponse(this, ref res);
      if (this.Parent is SpaceObjectVessel)
      {
        SpaceObjectVessel spaceObjectVessel = this.Parent as SpaceObjectVessel;
        if (spaceObjectVessel.IsDocked)
          spaceObjectVessel = spaceObjectVessel.DockedToMainVessel;
        foreach (DynamicObject dynamicObject in spaceObjectVessel.DynamicObjects)
          res.Data.Add(dynamicObject.GetSpawnResponseData(this));
        foreach (Corpse corpse in spaceObjectVessel.Corpses)
          res.Data.Add(corpse.GetSpawnResponseData(this));
        if (spaceObjectVessel.AllDockedVessels.Count > 0)
        {
          foreach (SpaceObjectVessel allDockedVessel in spaceObjectVessel.AllDockedVessels)
          {
            foreach (DynamicObject dynamicObject in allDockedVessel.DynamicObjects)
              res.Data.Add(dynamicObject.GetSpawnResponseData(this));
            foreach (Corpse corpse in allDockedVessel.Corpses)
              res.Data.Add(corpse.GetSpawnResponseData(this));
          }
        }
      }
      Server.Instance.NetworkController.SendToGameClient(this.GUID, (NetworkData) res);
      Server.Instance.NetworkController.SendCharacterSpawnToOtherPlayers(this);
      if (this.MessagesReceivedWhileLoading != null && this.MessagesReceivedWhileLoading.Count > 0)
      {
        foreach (NetworkData data1 in this.MessagesReceivedWhileLoading)
          Server.Instance.NetworkController.SendToGameClient(this.GUID, data1);
        this.MessagesReceivedWhileLoading.Clear();
      }
      this.IsAlive = true;
    }

    private void SuicideListener(NetworkData data)
    {
      if (data.Sender != this.GUID)
        return;
      this.KillYourself(CauseOfDeath.Suicide, true);
    }

    public void SetDockUndockCorrection(Vector3D? posCorrection, QuaternionD? rotCorrection)
    {
      this.dockUndockPositionCorrection = posCorrection;
      this.dockUndockRotationCorrection = rotCorrection;
      this.dockUndockWaitForMsg = posCorrection.HasValue && rotCorrection.HasValue;
    }

    public void ModifyLocalPositionAndRotation(Vector3D locPos, QuaternionD locRot)
    {
      this.LocalPosition = this.LocalPosition + locPos;
      this.LocalRotation = this.LocalRotation * locRot;
      if (this.TransformDataList == null || this.TransformDataList.Count <= 0)
        return;
      this.TransformDataList[this.TransformDataList.Count - 1].LocalPosition = this.LocalPosition.ToFloatArray();
      this.TransformDataList[this.TransformDataList.Count - 1].LocalRotation = this.LocalRotation.ToFloatArray();
    }

    private void UpdateMovementListener(NetworkData data)
    {
      CharacterMovementMessage characterMovementMessage = data as CharacterMovementMessage;
      if (characterMovementMessage.Sender != this.GUID || !this.IsAlive || this.Parent is Pivot && characterMovementMessage.ParentType == SpaceObjectType.None)
        return;
      this.MouseLook = characterMovementMessage.TransformData[0].MouseLook;
      this.FreeLookX = characterMovementMessage.TransformData[0].FreeLookX;
      this.FreeLookY = characterMovementMessage.TransformData[0].FreeLookY;
      this.AnimationData = new CharacterAnimationData()
      {
        VelocityForward = characterMovementMessage.AnimationData.VelocityForward,
        VelocityRight = characterMovementMessage.AnimationData.VelocityRight,
        ZeroGForward = characterMovementMessage.AnimationData.ZeroGForward,
        ZeroGRight = characterMovementMessage.AnimationData.ZeroGRight,
        PlayerStance = characterMovementMessage.AnimationData.PlayerStance,
        InteractType = characterMovementMessage.AnimationData.InteractType,
        TurningDirection = characterMovementMessage.AnimationData.TurningDirection,
        EquipOrDeEquip = characterMovementMessage.AnimationData.EquipOrDeEquip,
        EquipItemId = characterMovementMessage.AnimationData.EquipItemId,
        EmoteType = characterMovementMessage.AnimationData.EmoteType,
        ReloadItemType = characterMovementMessage.AnimationData.ReloadItemType,
        MeleeAttackType = characterMovementMessage.AnimationData.MeleeAttackType,
        LadderDirection = characterMovementMessage.AnimationData.LadderDirection,
        PlayerStanceFloat = characterMovementMessage.AnimationData.PlayerStanceFloat,
        GetUpType = characterMovementMessage.AnimationData.GetUpType,
        FireMode = characterMovementMessage.AnimationData.FireMode,
        AirTime = characterMovementMessage.AnimationData.AirTime
      };
      if (this.pivotPositionCorrection.IsNotEpsilonZero(double.Epsilon) && this.Parent is Pivot && !characterMovementMessage.PivotReset)
        return;
      if (this.pivotPositionCorrection.IsNotEpsilonZero(double.Epsilon) && characterMovementMessage.ParentType == SpaceObjectType.PlayerPivot && characterMovementMessage.PivotReset)
      {
        this.pivotPositionCorrection = Vector3D.Zero;
      }
      else
      {
        this.TransformDataList.Add(characterMovementMessage.TransformData[0]);
        this.LocalPosition = characterMovementMessage.TransformData[0].LocalPosition.ToVector3D();
        if (this.pivotPositionCorrection.IsNotEpsilonZero(double.Epsilon) && this.Parent is Pivot && !characterMovementMessage.PivotReset)
        {
          this.LocalPosition = this.LocalPosition - this.pivotPositionCorrection;
          this.TransformDataList[this.TransformDataList.Count - 1].LocalPosition = this.LocalPosition.ToFloatArray();
        }
        this.LocalRotation = characterMovementMessage.TransformData[0].LocalRotation.ToQuaternionD();
        if (characterMovementMessage.DockUndockMsg.HasValue && this.dockUndockWaitForMsg)
          this.SetDockUndockCorrection(new Vector3D?(), new QuaternionD?());
        if (this.dockUndockPositionCorrection.HasValue && this.dockUndockRotationCorrection.HasValue)
        {
          this.LocalPosition = this.LocalPosition + this.dockUndockPositionCorrection.Value;
          this.LocalRotation = this.LocalRotation * this.dockUndockRotationCorrection.Value;
          this.TransformDataList[this.TransformDataList.Count - 1].LocalPosition = this.LocalPosition.ToFloatArray();
          this.TransformDataList[this.TransformDataList.Count - 1].LocalRotation = this.LocalRotation.ToFloatArray();
        }
        this.velocity = characterMovementMessage.TransformData[0].Velocity.ToVector3D();
        this.angularVelocity = characterMovementMessage.TransformData[0].AngularVelocity.ToVector3D();
        this.gravity = characterMovementMessage.Gravity;
        if (characterMovementMessage.ImpactVellocity.HasValue)
          this.Stats.DoCollisionDamage(characterMovementMessage.ImpactVellocity.Value);
        if (characterMovementMessage.RagdollData != null)
          this.RagdollData = new Dictionary<byte, RagdollItemData>((IDictionary<byte, RagdollItemData>) characterMovementMessage.RagdollData);
        else if (this.RagdollData != null)
        {
          this.RagdollData.Clear();
          this.RagdollData = (Dictionary<byte, RagdollItemData>) null;
        }
        if (characterMovementMessage.JetpackDirection != null)
          this.jetpackDirection = new sbyte[4]
          {
            characterMovementMessage.JetpackDirection[0],
            characterMovementMessage.JetpackDirection[1],
            characterMovementMessage.JetpackDirection[2],
            characterMovementMessage.JetpackDirection[3]
          };
        else if (this.jetpackDirection != null)
          this.jetpackDirection = (sbyte[]) null;
        if (this.Parent is SpaceObjectVessel && characterMovementMessage.ParentType == SpaceObjectType.PlayerPivot)
        {
          SpaceObjectVessel vessel = this.Parent as SpaceObjectVessel;
          if (vessel.DockedToMainVessel != null)
            vessel = vessel.DockedToMainVessel;
          Pivot pivot = new Pivot(this, vessel);
          pivot.Orbit.CopyDataFrom(vessel.Orbit, Server.Instance.SolarSystem.CurrentTime, true);
          pivot.Orbit.SetLastChangeTime(Server.Instance.SolarSystem.CurrentTime);
          this.pivotPositionCorrection = Vector3D.Zero;
          this.pivotVelocityCorrection = Vector3D.Zero;
          this.LocalPosition = Vector3D.Zero;
          this.SubscribeTo(this.Parent);
          foreach (Player allPlayer in Server.Instance.AllPlayers)
          {
            if (allPlayer.IsSubscribedTo(this.Parent.GUID))
              allPlayer.SubscribeTo((SpaceObject) pivot);
          }
          this.Parent = (SpaceObject) pivot;
        }
        else if (this.Parent is SpaceObjectVessel && this.Parent.GUID != characterMovementMessage.ParentGUID)
        {
          SpaceObjectVessel parent = this.Parent as SpaceObjectVessel;
          if ((characterMovementMessage.ParentType == SpaceObjectType.Ship || characterMovementMessage.ParentType == SpaceObjectType.Asteroid || characterMovementMessage.ParentType == SpaceObjectType.Station) && Server.Instance.DoesObjectExist(characterMovementMessage.ParentGUID))
            this.Parent = (SpaceObject) Server.Instance.GetVessel(characterMovementMessage.ParentGUID);
          else
            Dbg.Error((object) "Unable to find new parent", (object) this.GUID, (object) this.Name, (object) "new parent", (object) characterMovementMessage.ParentType, (object) characterMovementMessage.ParentGUID);
        }
        else if (this.Parent is Pivot && characterMovementMessage.ParentType != SpaceObjectType.PlayerPivot)
        {
          Pivot parent = this.Parent as Pivot;
          if ((characterMovementMessage.ParentType == SpaceObjectType.Ship || characterMovementMessage.ParentType == SpaceObjectType.Asteroid || characterMovementMessage.ParentType == SpaceObjectType.Station) && Server.Instance.DoesObjectExist(characterMovementMessage.ParentGUID))
          {
            this.Parent = (SpaceObject) Server.Instance.GetVessel(characterMovementMessage.ParentGUID);
            this.SubscribeTo(this.Parent);
            parent.Destroy();
          }
          else
            Dbg.Error((object) "Unable to find new parent", (object) this.GUID, (object) this.Name, (object) "new parent", (object) characterMovementMessage.ParentType, (object) characterMovementMessage.ParentGUID);
        }
        else
        {
          TimeSpan runTime;
          int num;
          if (this.Parent is Pivot && this.pivotPositionCorrection.IsEpsilonZero(double.Epsilon))
          {
            runTime = Server.Instance.RunTime;
            num = runTime.TotalSeconds - this.lastPivotResetTime > 1.0 ? 1 : 0;
          }
          else
            num = 0;
          if (num != 0)
          {
            Pivot parent = this.Parent as Pivot;
            SpaceObjectVessel spaceObjectVessel = characterMovementMessage.NearestVesselGUID > 0L ? Server.Instance.GetVessel(characterMovementMessage.NearestVesselGUID) : (SpaceObjectVessel) null;
            if (spaceObjectVessel != null && spaceObjectVessel.DockedToMainVessel != null)
              spaceObjectVessel = spaceObjectVessel.DockedToMainVessel;
            Vector3D vector3D = parent.Position + this.LocalPosition;
            if (spaceObjectVessel != null && this.PrevNearestVesselGUID != characterMovementMessage.NearestVesselGUID && (characterMovementMessage.StickToVessel || (double) characterMovementMessage.NearestVesselDistance <= 50.0))
            {
              if (spaceObjectVessel.DockedToMainVessel != null)
                spaceObjectVessel = spaceObjectVessel.DockedToMainVessel;
              Vector3D position = parent.Position;
              Vector3D velocity = parent.Velocity;
              parent.Orbit.CopyDataFrom(spaceObjectVessel.Orbit, Server.Instance.SolarSystem.CurrentTime, true);
              parent.Orbit.SetLastChangeTime(Server.Instance.SolarSystem.CurrentTime);
              this.pivotPositionCorrection = parent.Position - position;
              this.pivotVelocityCorrection = parent.Velocity - velocity;
              this.UpdateArtificialBodyMovement.Add(parent.GUID);
              this.UpdateArtificialBodyMovement.Add(spaceObjectVessel.GUID);
              this.PrevNearestVesselGUID = characterMovementMessage.NearestVesselGUID;
            }
            else if ((spaceObjectVessel == null || (double) characterMovementMessage.NearestVesselDistance > 50.0) && this.LocalPosition.SqrMagnitude > 2500.0 && !characterMovementMessage.StickToVessel)
            {
              this.pivotPositionCorrection = this.LocalPosition;
              this.pivotVelocityCorrection = this.velocity.SqrMagnitude <= 100.0 ? Vector3D.Zero : this.velocity - Vector3D.ClampMagnitude(this.velocity, 5.0);
              parent.AdjustPositionAndVelocity(this.pivotPositionCorrection, this.pivotVelocityCorrection);
              this.UpdateArtificialBodyMovement.Add(parent.GUID);
              this.UpdateArtificialBodyMovement.Add(this.PrevNearestVesselGUID);
              this.PrevNearestVesselGUID = -1L;
            }
            if (this.pivotPositionCorrection.IsNotEpsilonZero(double.Epsilon))
            {
              runTime = Server.Instance.RunTime;
              this.lastPivotResetTime = runTime.TotalSeconds;
              this.LocalPosition = this.LocalPosition - this.pivotPositionCorrection;
              foreach (CharacterTransformData transformData in this.TransformDataList)
                transformData.LocalPosition = (transformData.LocalPosition.ToVector3D() - this.pivotPositionCorrection).ToFloatArray();
            }
          }
        }
        this.PlayerReady = true;
      }
    }

    public override void UpdateTimers(double deltaTime)
    {
      if (!this.IsAlive)
        return;
      if (this.IsUsingActiveItem && this.CurrentActiveItem != null)
        this.CurrentActiveItem.Use();
      this.updateTemperature(deltaTime);
      if ((double) this.CoreTemperature < 20.0 || (double) this.CoreTemperature <= 45.0)
        ;
      float suffocate = 0.0f;
      float pressure = 0.0f;
      if (!this.IsInsideSpawnPoint)
      {
        if (this.CurrentJetpack != null && this.CurrentHelmet != null && (!this.CurrentHelmet.IsVisorToggleable || this.CurrentHelmet.IsVisorActive))
        {
          if (this.CurrentJetpack.HasOxygen)
            this.CurrentJetpack.ConsumeResources(new float?(), new float?(), new float?(this.CurrentJetpack.OxygenConsumption * (float) deltaTime));
          else
            suffocate = 1f * (float) deltaTime;
        }
        else if (this.currentRoom == null || !this.currentRoom.IsAirOk)
          suffocate = 1f * (float) deltaTime;
      }
      if ((this.currentRoom == null || (double) this.currentRoom.AirPressure < 0.300000011920929) && (this.PlayerInventory.CurrOutfit == null || this.CurrentHelmet == null || this.CurrentHelmet.IsVisorToggleable && !this.CurrentHelmet.IsVisorActive))
        pressure = 2f * (float) deltaTime;
      this.Stats.TakeDammage(0.0f, new Vector3D?(), 0.0f, 0.0f, suffocate, 0.0f, pressure);
      if (this.CurrentJetpack != null && this.jetpackDirection != null && ((int) this.jetpackDirection[0] != 0 || (int) this.jetpackDirection[1] != 0 || (int) this.jetpackDirection[2] != 0 || (uint) this.jetpackDirection[3] > 0U))
        this.CurrentJetpack.ConsumeResources(new float?(this.CurrentJetpack.PropellantConsumption * (float) deltaTime), new float?(), new float?());
      if (this.CurrentHelmet != null && this.CurrentJetpack != null && this.CurrentHelmet.IsLightActive)
        this.CurrentJetpack.ConsumeResources(new float?(), new float?(0.1f * (float) deltaTime), new float?());
      if (this.CurrentHelmet == null && this.CurrentJetpack == null && this.CurrentActiveItem == null)
        return;
      this.updateItemTimer = this.updateItemTimer + deltaTime;
      if (this.updateItemTimer >= 0.300000011920929)
      {
        DynamicObjectsInfoMessage objectsInfoMessage = new DynamicObjectsInfoMessage();
        objectsInfoMessage.Infos = new List<DynamicObjectInfo>();
        if (this.CurrentHelmet != null && this.CurrentHelmet.DynamicObj.StatsChanged)
        {
          objectsInfoMessage.Infos.Add(new DynamicObjectInfo()
          {
            GUID = this.CurrentHelmet.GUID,
            Stats = this.CurrentHelmet.StatsNew
          });
          this.CurrentHelmet.DynamicObj.StatsChanged = false;
        }
        if (this.CurrentJetpack != null && this.CurrentJetpack.DynamicObj.StatsChanged)
        {
          objectsInfoMessage.Infos.Add(new DynamicObjectInfo()
          {
            GUID = this.CurrentJetpack.GUID,
            Stats = this.CurrentJetpack.StatsNew
          });
          this.CurrentJetpack.DynamicObj.StatsChanged = false;
        }
        if (this.CurrentActiveItem != null)
          this.CurrentActiveItem.SendAllStats();
        if (!(this.CurrentActiveItem is HandDrill) && this.CurrentActiveItem is Weapon)
        {
          Weapon currentActiveItem = this.CurrentActiveItem as Weapon;
          if (currentActiveItem.DynamicObj.StatsChanged)
          {
            objectsInfoMessage.Infos.Add(new DynamicObjectInfo()
            {
              GUID = this.CurrentActiveItem.GUID,
              Stats = currentActiveItem.StatsNew
            });
            currentActiveItem.DynamicObj.StatsChanged = false;
          }
          if (currentActiveItem.Magazine != null && currentActiveItem.Magazine.DynamicObj.StatsChanged)
          {
            objectsInfoMessage.Infos.Add(new DynamicObjectInfo()
            {
              GUID = currentActiveItem.Magazine.GUID,
              Stats = currentActiveItem.Magazine.StatsNew
            });
            currentActiveItem.Magazine.DynamicObj.StatsChanged = false;
          }
        }
        if (objectsInfoMessage.Infos.Count > 0)
          Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) objectsInfoMessage, -1L, this.Parent);
        this.updateItemTimer = 0.0;
      }
    }

    public void SubscribeTo(SpaceObject spaceObject)
    {
      this.subscribedToSpaceObjects.Add(spaceObject.GUID);
      if (!(spaceObject is SpaceObjectVessel))
        return;
      SpaceObjectVessel spaceObjectVessel = spaceObject as SpaceObjectVessel;
      if (spaceObjectVessel.IsDocked)
      {
        this.subscribedToSpaceObjects.Add(spaceObjectVessel.DockedToMainVessel.GUID);
        foreach (SpaceObject allDockedVessel in spaceObjectVessel.DockedToMainVessel.AllDockedVessels)
          this.subscribedToSpaceObjects.Add(allDockedVessel.GUID);
      }
      else if (spaceObjectVessel.AllDockedVessels != null && spaceObjectVessel.AllDockedVessels.Count > 0)
      {
        foreach (SpaceObject allDockedVessel in spaceObjectVessel.AllDockedVessels)
          this.subscribedToSpaceObjects.Add(allDockedVessel.GUID);
      }
    }

    public void UnsubscribeFrom(SpaceObject spaceObject)
    {
      this.subscribedToSpaceObjects.Remove(spaceObject.GUID);
    }

    public void UnsubscribeFromAll()
    {
      this.subscribedToSpaceObjects.Clear();
    }

    public bool IsSubscribedTo(SpaceObject spaceObject, bool checkParent)
    {
      if (!checkParent)
        return this.subscribedToSpaceObjects.Contains(spaceObject.GUID);
      return this.subscribedToSpaceObjects.Contains(spaceObject.GUID) || spaceObject.Parent != null && this.subscribedToSpaceObjects.Contains(spaceObject.Parent.GUID);
    }

    public bool IsSubscribedTo(long guid)
    {
      return this.subscribedToSpaceObjects.Contains(guid);
    }

    public void PlayerRoomMessageListener(NetworkData data)
    {
      if (data.Sender != this.GUID)
        return;
      PlayerRoomMessage playerRoomMessage = (PlayerRoomMessage) data;
      this.isOutsideRoom = playerRoomMessage.IsOutsideRoom.HasValue && playerRoomMessage.IsOutsideRoom.Value;
      if (playerRoomMessage.InSceneID == -1)
      {
        if (this.Parent is Ship)
          (this.Parent as Ship).MainDistributionManager.SetPlayerRoom(this, (VesselObjectID) null);
        this.CurrentRoomID = (VesselObjectID) null;
      }
      else
      {
        Ship ship = this.Parent.GUID == playerRoomMessage.VesselGUID ? this.Parent as Ship : Server.Instance.GetObject(playerRoomMessage.VesselGUID) as Ship;
        if (ship != null)
        {
          if (this.Parent is SpaceObjectVessel)
          {
            SpaceObjectVessel spaceObjectVessel = this.Parent as SpaceObjectVessel;
            if (spaceObjectVessel.IsDocked)
              spaceObjectVessel = spaceObjectVessel.DockedToMainVessel;
            if (!spaceObjectVessel.AllDockedVessels.Contains((SpaceObjectVessel) ship) && this.Parent is Ship)
              (this.Parent as Ship).MainDistributionManager.SetPlayerRoom(this, (VesselObjectID) null);
          }
          this.CurrentRoomID = new VesselObjectID(ship.GUID, playerRoomMessage.InSceneID);
          ship.MainDistributionManager.SetPlayerRoom(this, this.CurrentRoomID);
        }
        else
        {
          this.CurrentRoomID = (VesselObjectID) null;
          if (this.Parent is Ship)
            (this.Parent as Ship).MainDistributionManager.SetPlayerRoom(this, (VesselObjectID) null);
        }
      }
    }

    public CharacterMovementMessage GetCharacterMovementMessage()
    {
      if (this.TransformDataList.Count < 1)
        return (CharacterMovementMessage) null;
      CharacterMovementMessage characterMovementMessage = new CharacterMovementMessage();
      characterMovementMessage.GUID = this.FakeGuid;
      if (this.Parent != null)
      {
        characterMovementMessage.ParentGUID = this.Parent.GUID;
        characterMovementMessage.ParentType = this.Parent.ObjectType;
      }
      else
      {
        characterMovementMessage.ParentGUID = -1L;
        characterMovementMessage.ParentType = SpaceObjectType.None;
      }
      characterMovementMessage.TransformData = this.TransformDataList.InnerList.ToArray();
      characterMovementMessage.Gravity = this.gravity;
      characterMovementMessage.AnimationData = new CharacterAnimationData();
      characterMovementMessage.AnimationData.VelocityForward = this.AnimationData.VelocityForward;
      characterMovementMessage.AnimationData.VelocityRight = this.AnimationData.VelocityRight;
      characterMovementMessage.AnimationData.ZeroGForward = this.AnimationData.ZeroGForward;
      characterMovementMessage.AnimationData.ZeroGRight = this.AnimationData.ZeroGRight;
      characterMovementMessage.AnimationData.PlayerStance = this.AnimationData.PlayerStance;
      characterMovementMessage.AnimationData.InteractType = this.AnimationData.InteractType;
      characterMovementMessage.AnimationData.TurningDirection = this.AnimationData.TurningDirection;
      characterMovementMessage.AnimationData.EquipOrDeEquip = this.AnimationData.EquipOrDeEquip;
      characterMovementMessage.AnimationData.EquipItemId = this.AnimationData.EquipItemId;
      characterMovementMessage.AnimationData.EmoteType = this.AnimationData.EmoteType;
      characterMovementMessage.AnimationData.ReloadItemType = this.AnimationData.ReloadItemType;
      characterMovementMessage.AnimationData.MeleeAttackType = this.AnimationData.MeleeAttackType;
      characterMovementMessage.AnimationData.LadderDirection = this.AnimationData.LadderDirection;
      characterMovementMessage.AnimationData.PlayerStanceFloat = this.AnimationData.PlayerStanceFloat;
      characterMovementMessage.AnimationData.GetUpType = this.AnimationData.GetUpType;
      characterMovementMessage.AnimationData.FireMode = this.AnimationData.FireMode;
      characterMovementMessage.AnimationData.AirTime = this.AnimationData.AirTime;
      if (this.RagdollData != null && this.RagdollData.Count > 0)
        characterMovementMessage.RagdollData = new Dictionary<byte, RagdollItemData>((IDictionary<byte, RagdollItemData>) this.RagdollData);
      if (this.jetpackDirection != null)
        characterMovementMessage.JetpackDirection = new sbyte[4]
        {
          this.jetpackDirection[0],
          this.jetpackDirection[1],
          this.jetpackDirection[2],
          this.jetpackDirection[3]
        };
      characterMovementMessage.PivotReset = this.pivotPositionCorrection.IsNotEpsilonZero(double.Epsilon);
      characterMovementMessage.PivotPositionCorrection = this.pivotPositionCorrection.ToFloatArray();
      characterMovementMessage.PivotVelocityCorrection = this.pivotVelocityCorrection.ToFloatArray();
      return characterMovementMessage;
    }

    public override SpawnObjectResponseData GetSpawnResponseData(Player pl)
    {
      SpawnCharacterResponseData characterResponseData = new SpawnCharacterResponseData();
      long guid = this.GUID;
      characterResponseData.GUID = guid;
      CharacterDetails details = this.GetDetails(true);
      characterResponseData.Details = details;
      return (SpawnObjectResponseData) characterResponseData;
    }

    public CharacterDetails GetDetails(bool checkAlive = false)
    {
      List<DynamicObjectDetails> dynamicObjectDetailsList = new List<DynamicObjectDetails>();
      foreach (DynamicObject dynamicObject in this.DynamicObjects)
        dynamicObjectDetailsList.Add(dynamicObject.GetDetails());
      CharacterDetails characterDetails = new CharacterDetails()
      {
        GUID = this.FakeGuid,
        Name = this.Name,
        Gender = this.Gender,
        HeadType = this.HeadType,
        HairType = this.HairType,
        SteamId = this.SteamId,
        ParentID = this.Parent != null ? this.Parent.GUID : -1L,
        ParentType = (SpaceObjectType) (this.Parent != null ? (int) this.Parent.ObjectType : 0),
        DynamicObjects = dynamicObjectDetailsList,
        AnimationStatsMask = this.AnimationStatsMask
      };
      if (this.IsAlive || !checkAlive || this.CurrentSpawnPoint == null)
      {
        characterDetails.TransformData = new CharacterTransformData();
        characterDetails.TransformData.LocalPosition = this.LocalPosition.ToFloatArray();
        characterDetails.TransformData.LocalRotation = this.LocalRotation.ToFloatArray();
        characterDetails.TransformData.MouseLook = this.MouseLook;
        characterDetails.TransformData.FreeLookX = this.FreeLookX;
        characterDetails.TransformData.FreeLookY = this.FreeLookY;
      }
      else
        characterDetails.SpawnPointID = this.CurrentSpawnPoint.SpawnPointID;
      return characterDetails;
    }

    public override void Destroy()
    {
      this.DiconnectFromNetworkContoller();
      while (this.DynamicObjects.Count > 0)
        this.DynamicObjects[0].DestroyDynamicObject();
      foreach (SpaceObjectVessel allVessel in Server.Instance.AllVessels)
      {
        if (allVessel is Ship)
          (allVessel as Ship).ResetSpawnPointsForPlayer(this, true);
      }
      Server.Instance.Remove(this);
      base.Destroy();
    }

    private void updateTemperature(double deltaTime)
    {
      this.updateOutfitTemperature(deltaTime);
      if (this.AmbientTemperature.HasValue)
      {
        double coreTemperature1 = (double) this.CoreTemperature;
        float? ambientTemperature = this.AmbientTemperature;
        float coreTemperature2 = this.CoreTemperature;
        double? nullable = ambientTemperature.HasValue ? new double?(((double) ambientTemperature.GetValueOrDefault() - (double) coreTemperature2) * 0.01) : new double?();
        double num1 = deltaTime;
        double num2 = (nullable.HasValue ? new double?(nullable.GetValueOrDefault() * num1) : new double?()).Value;
        this.CoreTemperature = (float) (coreTemperature1 + num2);
      }
      else
        this.CoreTemperature = this.SpaceExposureTemperature(this.CoreTemperature, 10000f, 20f, 80f, deltaTime);
    }

    private void updateOutfitTemperature(double deltaTime)
    {
      Outfit currOutfit = this.PlayerInventory.CurrOutfit;
      if (currOutfit == null)
        return;
      if (this.Parent is Pivot)
        currOutfit.ExternalTemperature = this.SpaceExposureTemperature(currOutfit.ExternalTemperature, 10000f, 20f, 80f, deltaTime);
      else if (this.Parent is SpaceObjectVessel)
        currOutfit.ExternalTemperature += (float) (((double) (this.Parent as SpaceObjectVessel).Temperature - (double) currOutfit.ExternalTemperature) * 0.001 * deltaTime);
      float num1 = 0.1f;
      currOutfit.InternalTemperature += (float) (((double) currOutfit.ExternalTemperature - (double) currOutfit.InternalTemperature) * 0.1 * deltaTime) * num1;
      if (this.CurrentHelmet == null || !this.CurrentHelmet.IsVisorActive || this.CurrentJetpack == null || !this.CurrentJetpack.HasPower)
        return;
      float num2 = 5f;
      float num3 = (float) MathHelper.Clamp(37.0 - (double) currOutfit.InternalTemperature, -(double) num2 * deltaTime, (double) num2 * deltaTime);
      currOutfit.InternalTemperature += num3;
      this.outfitTempRegulationActive = System.Math.Abs((double) num3 / deltaTime) > 0.5;
    }

    public void KillYourself(CauseOfDeath causeOfdeath, bool createCorpse = true)
    {
      this.IsAlive = false;
      Corpse corpse = (Corpse) null;
      if (createCorpse)
      {
        corpse = new Corpse(this);
      }
      else
      {
        while (this.DynamicObjects.Count > 0)
          this.DynamicObjects[0].DestroyDynamicObject();
      }
      this.PlayerInventory = new Inventory(this);
      this.CurrentJetpack = (Jetpack) null;
      this.CurrentHelmet = (Helmet) null;
      if (this.DynamicObjects.Count > 0)
      {
        string str = "Player had some dynamic objects that are not moved to corpse:";
        foreach (DynamicObject dynamicObject in this.DynamicObjects)
          str = str + " " + (object) dynamicObject.GUID + ",";
        this.DynamicObjects.Clear();
      }
      this.UnsubscribeFromAll();
      this.Health = 100;
      if (this.Parent is Ship)
      {
        Ship parent = this.Parent as Ship;
        parent.RemovePlayerFromRoom(this);
        parent.RemovePlayerFromCrew(this, true);
      }
      else if (this.Parent is Pivot)
        (this.Parent as Pivot).Destroy();
      if (this.CurrentSpawnPoint != null && this.CurrentSpawnPoint.Type == SpawnPointType.SimpleSpawn)
      {
        this.CurrentSpawnPoint.Player = (Player) null;
        this.CurrentSpawnPoint.IsPlayerInSpawnPoint = false;
        this.CurrentSpawnPoint = (ShipSpawnPoint) null;
      }
      if (this.RagdollData != null)
      {
        this.RagdollData.Clear();
        this.RagdollData = (Dictionary<byte, RagdollItemData>) null;
      }
      NetworkController networkController = Server.Instance.NetworkController;
      KillPlayerMessage killPlayerMessage = new KillPlayerMessage();
      killPlayerMessage.GUID = this.FakeGuid;
      killPlayerMessage.CauseOfDeath = causeOfdeath;
      killPlayerMessage.CorpseDetails = corpse != null ? corpse.GetDetails() : (CorpseDetails) null;
      long guid = this.GUID;
      int depth = 4;
      networkController.SendToClientsSubscribedToParents((NetworkData) killPlayerMessage, (SpaceObject) this, guid, depth);
      this.Parent = (SpaceObject) null;
      this.CurrentRoomID = (VesselObjectID) null;
      this.isOutsideRoom = false;
      if (!Server.Instance.NetworkController.clientList.ContainsKey(this.GUID))
        return;
      Server.Instance.NetworkController.LogOutPlayer(this.GUID);
      Server.Instance.NetworkController.SendToGameClient(this.GUID, (NetworkData) new KillPlayerMessage()
      {
        GUID = this.FakeGuid,
        CauseOfDeath = causeOfdeath
      });
      this.lateDisconnectWait = 0.0;
      Server.Instance.SubscribeToTimer(UpdateTimer.TimerStep.Step_0_1_sec, new UpdateTimer.TimeStepDelegate(this.LateDisconnect));
    }

    public void LateDisconnect(double dbl)
    {
      this.lateDisconnectWait = this.lateDisconnectWait + dbl;
      if (this.lateDisconnectWait <= 1.0)
        return;
      Server.Instance.NetworkController.DisconnectClient(this.GUID);
      Server.Instance.UnsubscribeFromTimer(UpdateTimer.TimerStep.Step_0_1_sec, new UpdateTimer.TimeStepDelegate(this.LateDisconnect));
      this.lateDisconnectWait = 0.0;
    }

    public void LogoutDisconnectReset()
    {
      if (this.Parent is Ship)
        (this.Parent as Ship).RemovePlayerFromExecuters(this);
      this.UnsubscribeFromAll();
      this.EnviromentReady = false;
      this.PlayerReady = false;
      this.LastMovementMessageSolarSystemTime = -1.0;
      this.MessagesReceivedWhileLoading.Clear();
      try
      {
        if (this.Parent == null || !(this.Parent is Ship) || !this.isOutsideRoom)
          return;
        Pivot pivot = new Pivot(this, this.Parent as SpaceObjectVessel);
        pivot.Orbit.CopyDataFrom((this.Parent as Ship).Orbit, Server.Instance.SolarSystem.CurrentTime, true);
        pivot.Orbit.RelativePosition += this.LocalPosition;
        pivot.Orbit.InitFromCurrentStateVectors(Server.Instance.SolarSystem.CurrentTime);
        pivot.StabilizeToTarget((SpaceObjectVessel) (this.Parent as Ship), true);
        this.LocalPosition = Vector3D.Zero;
        this.Parent = (SpaceObject) pivot;
      }
      catch (Exception ex)
      {
      }
    }

    public PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataPlayer objectDataPlayer = new PersistenceObjectDataPlayer();
      objectDataPlayer.GUID = this.GUID;
      objectDataPlayer.FakeGUID = this.FakeGuid;
      if (this.Parent != null)
      {
        objectDataPlayer.ParentGUID = this.Parent.GUID;
        objectDataPlayer.ParentType = this.Parent.ObjectType;
        if (this.Parent.ObjectType == SpaceObjectType.PlayerPivot)
        {
          objectDataPlayer.ParentPosition = this.Parent.Position.ToArray();
          objectDataPlayer.ParentVelocity = this.Parent.Velocity.ToArray();
        }
      }
      else
      {
        objectDataPlayer.ParentGUID = -1L;
        objectDataPlayer.ParentType = SpaceObjectType.None;
      }
      objectDataPlayer.LocalPosition = this.LocalPosition.ToArray();
      objectDataPlayer.LocalRotation = this.LocalRotation.ToArray();
      objectDataPlayer.IsAlive = this.IsAlive;
      objectDataPlayer.Name = this.Name;
      objectDataPlayer.SteamId = this.SteamId;
      objectDataPlayer.Gender = this.Gender;
      objectDataPlayer.HeadType = this.HeadType;
      objectDataPlayer.HairType = this.HairType;
      objectDataPlayer.HealthPoints = this.Stats.HealthPoints;
      objectDataPlayer.MaxHealthPoints = this.Stats.MaxHealthPoints;
      objectDataPlayer.AnimationData = ObjectCopier.DeepCopy<CharacterAnimationData>(this.AnimationData, 10);
      objectDataPlayer.AnimationStatsMask = this.AnimationStatsMask;
      objectDataPlayer.Gravity = this.gravity;
      objectDataPlayer.Velocity = this.velocity.ToArray();
      objectDataPlayer.AngularVelocity = this.angularVelocity.ToArray();
      if (this.CurrentRoomID != null)
        objectDataPlayer.CurrentRoomID = new int?(this.CurrentRoomID.InSceneID);
      objectDataPlayer.CoreTemperature = this.CoreTemperature;
      objectDataPlayer.ChildObjects = new List<PersistenceObjectData>();
      DynamicObject dynamicObject = this.DynamicObjects.InnerList.Find((Predicate<DynamicObject>) (m =>
      {
        if (m.Item != null && m.Item.Slot != null)
          return (int) m.Item.Slot.SlotID == -2;
        return false;
      }));
      if (dynamicObject != null)
        objectDataPlayer.ChildObjects.Add(dynamicObject.Item != null ? dynamicObject.Item.GetPersistenceData() : dynamicObject.GetPersistenceData());
      foreach (DynamicObject inner in this.DynamicObjects.InnerList)
      {
        if (inner != dynamicObject)
          objectDataPlayer.ChildObjects.Add(inner.Item != null ? inner.Item.GetPersistenceData() : inner.GetPersistenceData());
      }
      return (PersistenceObjectData) objectDataPlayer;
    }

    public void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        PersistenceObjectDataPlayer objectDataPlayer = persistenceData as PersistenceObjectDataPlayer;
        if (objectDataPlayer == null)
        {
          Dbg.Warning((object) "PersistenceObjectDataPlayer data is null", (object) this.GUID);
        }
        else
        {
          this.GUID = objectDataPlayer.GUID;
          this.FakeGuid = objectDataPlayer.FakeGUID;
          this.LocalPosition = objectDataPlayer.LocalPosition.ToVector3D();
          this.LocalRotation = objectDataPlayer.LocalRotation.ToQuaternionD();
          this.IsAlive = objectDataPlayer.IsAlive;
          this.Name = objectDataPlayer.Name;
          this.SteamId = objectDataPlayer.SteamId;
          this.Gender = objectDataPlayer.Gender;
          this.HeadType = objectDataPlayer.HeadType;
          this.HairType = objectDataPlayer.HairType;
          this.Stats.MaxHealthPoints = objectDataPlayer.MaxHealthPoints;
          this.Stats.HealthPoints = objectDataPlayer.HealthPoints;
          this.AnimationData = ObjectCopier.DeepCopy<CharacterAnimationData>(objectDataPlayer.AnimationData, 10);
          this.AnimationStatsMask = objectDataPlayer.AnimationStatsMask;
          this.gravity = objectDataPlayer.Gravity;
          this.velocity = objectDataPlayer.Velocity.ToVector3D();
          this.angularVelocity = objectDataPlayer.AngularVelocity.ToVector3D();
          this.CoreTemperature = objectDataPlayer.CoreTemperature;
          SpaceObject spaceObject = (SpaceObject) null;
          if (objectDataPlayer.ParentType == SpaceObjectType.PlayerPivot)
            spaceObject = (SpaceObject) new Pivot((SpaceObjectTransferable) this, objectDataPlayer.ParentPosition.ToVector3D(), objectDataPlayer.ParentVelocity.ToVector3D());
          else if (objectDataPlayer.ParentGUID != -1L)
            spaceObject = Server.Instance.GetObject(objectDataPlayer.ParentGUID);
          if (spaceObject != null)
          {
            this.Parent = spaceObject;
            if (objectDataPlayer.CurrentRoomID.HasValue)
              this.CurrentRoomID = new VesselObjectID()
              {
                InSceneID = objectDataPlayer.CurrentRoomID.Value,
                VesselGUID = this.Parent.GUID
              };
          }
          else
          {
            if (objectDataPlayer.ParentGUID != -1L && spaceObject == null)
            {
              Dbg.Error((object) "Player papa object not found, SAVE MIGHT BE CORRUPTED", (object) this.GUID, (object) objectDataPlayer.ParentGUID, (object) objectDataPlayer.ParentType);
              return;
            }
            this.Parent = (SpaceObject) null;
            this.KillYourself(CauseOfDeath.None, false);
          }
          if (this.Parent != null)
          {
            foreach (PersistenceObjectDataDynamicObject childObject in objectDataPlayer.ChildObjects)
              Persistence.CreateDynamicObject(childObject, (SpaceObject) this, (StructureSceneData) null);
          }
          Server.Instance.Add(this);
        }
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }

    public void SetSpawnPoint(ShipSpawnPoint spawnPoint)
    {
      if (spawnPoint != null && spawnPoint.Type == SpawnPointType.WithAuthorization)
        this.AuthorizedSpawnPoint = spawnPoint;
      this.CurrentSpawnPoint = spawnPoint;
    }

    public void ClearAuthorizedSpawnPoint()
    {
      this.AuthorizedSpawnPoint = (ShipSpawnPoint) null;
    }

    public float AirQualityDegradationRate
    {
      get
      {
        return this.IsInsideSpawnPoint || !this.IsAlive || this.CurrentHelmet != null && (!this.CurrentHelmet.IsVisorToggleable || this.CurrentHelmet.IsVisorActive) ? 0.0f : 0.05f;
      }
    }

    public float AirQuantityDecreaseRate
    {
      get
      {
        return 0.0f;
      }
    }

    public bool AffectsQuality
    {
      get
      {
        return (double) this.AirQualityDegradationRate > 0.0;
      }
    }

    public bool AffectsQuantity
    {
      get
      {
        return false;
      }
    }

    internal CharacterData GetCharacterData()
    {
      return new CharacterData()
      {
        Name = this.Name,
        Gender = this.Gender,
        HeadType = this.HeadType,
        HairType = this.HairType
      };
    }
  }
}
