// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Corpse
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using ZeroGravity.Math;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  public class Corpse : SpaceObjectTransferable
  {
    public static double ArenaTimer = TimeSpan.FromMinutes(30.0).TotalMilliseconds;
    public static double EmptyCorpseTimer = TimeSpan.FromMinutes(5.0).TotalMilliseconds;
    public static double OutsideTimer = TimeSpan.FromHours(3.0).TotalMilliseconds;
    public static double InsideModuleTimer = TimeSpan.FromHours(24.0).TotalMilliseconds;
    public double DestroyTime = 120000.0;
    public long ListenToSenderID = 0;
    private double WaitForSenderTime = 1.0;
    private Vector3D pivotPositionCorrection = Vector3D.Zero;
    private Vector3D pivotVelocityCorrection = Vector3D.Zero;
    private DateTime lastPivotResetTime = DateTime.UtcNow;
    private SpaceObject _Parent = (SpaceObject) null;
    private DateTime lastSenderTime;
    public bool IsInsideSpaceObject;
    public Dictionary<byte, RagdollItemData> RagdollDataList;
    private Timer destroyTimer;

    public override SpaceObjectType ObjectType
    {
      get
      {
        return SpaceObjectType.Corpse;
      }
    }

    public Inventory CorpseInventory { get; private set; }

    public override SpaceObject Parent
    {
      get
      {
        return this._Parent;
      }
      set
      {
        if (this._Parent != null)
          this.Parent.Corpses.Remove(this);
        this._Parent = value;
        if (this._Parent == null || this._Parent.Corpses.Contains(this))
          return;
        this.Parent.Corpses.Add(this);
      }
    }

    public Corpse(Player player)
      : base(GUIDFactory.NextObjectGUID(), player.LocalPosition, player.LocalRotation)
    {
      if (player.Parent is Pivot)
      {
        Pivot pivot = new Pivot((SpaceObjectTransferable) this, player.Parent as ArtificialBody);
        this.Parent = (SpaceObject) pivot;
        this.pivotPositionCorrection = Vector3D.Zero;
        this.pivotVelocityCorrection = Vector3D.Zero;
        foreach (Player allPlayer in Server.Instance.AllPlayers)
        {
          if (allPlayer.IsSubscribedTo(player.Parent.GUID))
            allPlayer.SubscribeTo((SpaceObject) pivot);
        }
      }
      else
        this.Parent = player.Parent;
      this.LocalPosition = player.LocalPosition;
      this.LocalRotation = player.LocalRotation;
      this.CorpseInventory = player.PlayerInventory;
      this.CorpseInventory.ChangeParent(this);
      Server.Instance.Add(this);
      this.ConnectToNetworkController();
      this.DestroyTime = !(this.Parent is SpaceObjectVessel) ? Corpse.OutsideTimer : ((this.Parent as SpaceObjectVessel).IsArenaVessel ? Corpse.ArenaTimer : Corpse.InsideModuleTimer);
      bool flag = this.CorpseInventory.HandsSlot.SlotItem == null;
      if (this.CorpseInventory.CurrOutfit != null)
      {
        foreach (KeyValuePair<short, InventorySlot> slot in this.CorpseInventory.CurrOutfit.Slots)
        {
          if (slot.Value.SlotItem != null)
            flag = false;
        }
      }
      if (flag)
        this.DestroyTime = Corpse.EmptyCorpseTimer;
      if (this.DestroyTime <= -1.0)
        return;
      this.destroyTimer = new Timer(this.DestroyTime);
      this.destroyTimer.Elapsed += (ElapsedEventHandler) ((sender, args) => Corpse.DestoyCorpseTimerElapsed((object) this));
      this.destroyTimer.Enabled = true;
    }

    private static void DestoyCorpseTimerElapsed(object sender)
    {
      Corpse corpse = sender as Corpse;
      if (corpse == null)
        return;
      corpse.Destroy();
    }

    public void ConnectToNetworkController()
    {
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (MoveCorpseObectMessage), new EventSystem.NetworkDataDelegate(this.MoveCorpseObectMessageListener));
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (CorpseStatsMessage), new EventSystem.NetworkDataDelegate(this.CorpseStatsMessageListener));
    }

    public void DisconnectFromNetworkController()
    {
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (MoveCorpseObectMessage), new EventSystem.NetworkDataDelegate(this.MoveCorpseObectMessageListener));
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (CorpseStatsMessage), new EventSystem.NetworkDataDelegate(this.CorpseStatsMessageListener));
    }

    private void MoveCorpseObectMessageListener(NetworkData data)
    {
      MoveCorpseObectMessage corpseObectMessage = data as MoveCorpseObectMessage;
      if (corpseObectMessage.GUID != this.GUID || this.ListenToSenderID != 0L && corpseObectMessage.Sender != this.ListenToSenderID && (DateTime.UtcNow - this.lastSenderTime).TotalSeconds < this.WaitForSenderTime)
        return;
      this.ListenToSenderID = corpseObectMessage.Sender;
      this.lastSenderTime = DateTime.UtcNow;
      Vector3D vector3D = corpseObectMessage.LocalPosition.ToVector3D();
      QuaternionD quaternionD = corpseObectMessage.LocalRotation.ToQuaternionD();
      this.RagdollDataList = corpseObectMessage.RagdollDataList;
      this.LocalPosition = vector3D;
      this.LocalRotation = quaternionD;
      this.IsInsideSpaceObject = corpseObectMessage.IsInsideSpaceObject;
    }

    private void CorpseStatsMessageListener(NetworkData data)
    {
      CorpseStatsMessage corpseStatsMessage = data as CorpseStatsMessage;
      if (corpseStatsMessage.GUID != this.GUID)
        return;
      SpaceObject parent1 = this.Parent;
      if (this.Parent is SpaceObjectVessel && corpseStatsMessage.ParentType == SpaceObjectType.CorpsePivot)
      {
        SpaceObjectVessel parent2 = this.Parent as SpaceObjectVessel;
        Pivot pivot = new Pivot((SpaceObjectTransferable) this, parent2.DockedToMainVessel != null ? (ArtificialBody) parent2.DockedToMainVessel : (ArtificialBody) parent2);
        this.Parent = (SpaceObject) pivot;
        this.pivotPositionCorrection = Vector3D.Zero;
        this.pivotVelocityCorrection = Vector3D.Zero;
        foreach (Player allPlayer in Server.Instance.AllPlayers)
        {
          if (allPlayer.IsSubscribedTo(parent2.GUID))
            allPlayer.SubscribeTo((SpaceObject) pivot);
        }
      }
      else if (this.Parent is Pivot && corpseStatsMessage.ParentType != SpaceObjectType.CorpsePivot)
      {
        Pivot parent2 = this.Parent as Pivot;
        this.Parent = Server.Instance.GetObject(corpseStatsMessage.ParentGUID);
        if (this.Parent != null)
        {
          foreach (Player allPlayer in Server.Instance.AllPlayers)
          {
            if (allPlayer.IsSubscribedTo(parent2.GUID))
              allPlayer.UnsubscribeFrom((SpaceObject) parent2);
          }
          Server.Instance.SolarSystem.RemoveArtificialBody((ArtificialBody) parent2);
        }
      }
      else if (corpseStatsMessage.ParentType == SpaceObjectType.Ship || corpseStatsMessage.ParentType == SpaceObjectType.Station || corpseStatsMessage.ParentType == SpaceObjectType.Asteroid)
        this.Parent = Server.Instance.GetObject(corpseStatsMessage.ParentGUID);
      else
        Dbg.Error((object) "Dont know what happened to corpse parent", (object) parent1.GUID, (object) parent1.ObjectType, (object) corpseStatsMessage.ParentGUID, (object) corpseStatsMessage.ParentType);
      if (parent1 == this.Parent)
        ;
      this.ListenToSenderID = corpseStatsMessage.Sender;
      this.lastSenderTime = DateTime.UtcNow;
      Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) corpseStatsMessage, -1L, parent1, this.Parent, parent1 != null ? parent1.Parent : (SpaceObject) null, this.Parent != null ? this.Parent.Parent : (SpaceObject) null);
    }

    internal void CheckInventoryDestroy()
    {
      if (this.CorpseInventory.HandsSlot.SlotItem != null || this.CorpseInventory.CurrOutfit != null && this.CorpseInventory.CurrOutfit.Slots.Where<KeyValuePair<short, InventorySlot>>((Func<KeyValuePair<short, InventorySlot>, bool>) (m => m.Value.SlotItem != null)) == null)
        return;
      if (this.destroyTimer != null)
        this.destroyTimer.Dispose();
      this.destroyTimer = new Timer(TimeSpan.FromMinutes(5.0).TotalMilliseconds);
      this.destroyTimer.Elapsed += (ElapsedEventHandler) ((sender, args) => Corpse.DestoyCorpseTimerElapsed((object) this));
      this.destroyTimer.Enabled = true;
    }

    public MoveCorpseObectMessage GetMovementMessage()
    {
      return new MoveCorpseObectMessage()
      {
        GUID = this.GUID,
        RagdollDataList = this.RagdollDataList,
        LocalPosition = this.LocalPosition.ToFloatArray(),
        LocalRotation = this.LocalRotation.ToFloatArray(),
        IsInsideSpaceObject = this.IsInsideSpaceObject
      };
    }

    public bool PlayerReceivesMovementMessage(long playerGuid)
    {
      return playerGuid != this.ListenToSenderID && (ulong) this.ListenToSenderID > 0UL;
    }

    public void DestroyCorpse()
    {
      this.DisconnectFromNetworkController();
      if (this.destroyTimer == null)
        return;
      this.destroyTimer.Dispose();
    }

    public CorpseDetails GetDetails()
    {
      List<DynamicObjectDetails> dynamicObjectDetailsList1 = new List<DynamicObjectDetails>();
      foreach (DynamicObject dynamicObject in this.DynamicObjects)
        dynamicObjectDetailsList1.Add(dynamicObject.GetDetails());
      CorpseDetails corpseDetails = new CorpseDetails();
      corpseDetails.GUID = this.GUID;
      corpseDetails.ParentGUID = this.Parent == null ? -1L : this.Parent.GUID;
      corpseDetails.ParentType = (SpaceObjectType) (this.Parent == null ? 0 : (int) this.Parent.ObjectType);
      corpseDetails.LocalPosition = this.LocalPosition.ToFloatArray();
      corpseDetails.LocalRotation = this.LocalRotation.ToFloatArray();
      int num = this.IsInsideSpaceObject ? 1 : 0;
      corpseDetails.IsInsideSpaceObject = num != 0;
      Dictionary<byte, RagdollItemData> ragdollDataList = this.RagdollDataList;
      corpseDetails.RagdollDataList = ragdollDataList;
      List<DynamicObjectDetails> dynamicObjectDetailsList2 = dynamicObjectDetailsList1;
      corpseDetails.DynamicObjectData = dynamicObjectDetailsList2;
      return corpseDetails;
    }

    public override SpawnObjectResponseData GetSpawnResponseData(Player pl)
    {
      SpawnCorpseResponseData corpseResponseData = new SpawnCorpseResponseData();
      long guid = this.GUID;
      corpseResponseData.GUID = guid;
      CorpseDetails details = this.GetDetails();
      corpseResponseData.Details = details;
      return (SpawnObjectResponseData) corpseResponseData;
    }

    public override void Destroy()
    {
      this.DestroyCorpse();
      base.Destroy();
    }
  }
}
