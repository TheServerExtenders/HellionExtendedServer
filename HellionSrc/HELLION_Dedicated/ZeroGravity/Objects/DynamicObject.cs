// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.DynamicObject
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.Linq;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.ShipComponents;
using ZeroGravity.Spawn;

namespace ZeroGravity.Objects
{
  public class DynamicObject : SpaceObjectTransferable, IPersistantObject
  {
    private Player _listenToPlayer = (Player) null;
    private long _listenToSenderID = 0;
    private double WaitForSenderTime = 1.5;
    public double TimeToLive = -1.0;
    private Vector3D pivotPositionCorrection = Vector3D.Zero;
    private Vector3D pivotVelocityCorrection = Vector3D.Zero;
    private DateTime lastPivotResetTime = DateTime.UtcNow;
    private SpaceObject _Parent = (SpaceObject) null;
    private bool pickedUp = false;
    public Item Item = (Item) null;
    public float RespawnTime = -1f;
    public float SpawnMaxHealth = -1f;
    public float SpawnMinHealth = -1f;
    public float SpawnWearMultiplier = 1f;
    public short ItemID;
    public ItemType ItemType;
    private DateTime lastSenderTime;
    public DynamicObjectSceneData DynamicObjectSceneData;
    public AttachPointDetails APDetails;

    public override SpaceObjectType ObjectType
    {
      get
      {
        return SpaceObjectType.DynamicObject;
      }
    }

    public long ListenToSenderID
    {
      get
      {
        return this._listenToSenderID;
      }
      private set
      {
        this._listenToSenderID = value;
        this._listenToPlayer = Server.Instance.GetPlayer(value);
      }
    }

    public bool IsAttached
    {
      get
      {
        return this.Item != null && (this.Item.Slot != null || this.Item.AttachPointType != AttachPointType.None || this.Parent is DynamicObject);
      }
    }

    public short InvSlotID
    {
      get
      {
        return this.Item == null || this.Item.Slot == null ? (short) -1111 : this.Item.Slot.SlotID;
      }
    }

    public bool StatsChanged { get; set; }

    public double LastChangeTime { get; private set; }

    public DynamicObjectStats StatsNew
    {
      get
      {
        if (this.Item == null)
          return (DynamicObjectStats) null;
        DynamicObjectStats statsNew = this.Item.StatsNew;
        if (this.Item.StatsNew != null)
          statsNew.Health = new float?(this.Item.Health);
        return statsNew;
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
        if (this._Parent != null)
          this._Parent.DynamicObjects.Remove(this);
        this._Parent = value;
        if (this._Parent != null && !this._Parent.DynamicObjects.Contains(this))
          this._Parent.DynamicObjects.Add(this);
        if (this._Parent is Pivot)
          Server.Instance.SubscribeToTimer(UpdateTimer.TimerStep.Step_1_0_min, new UpdateTimer.TimeStepDelegate(this.SelfDestructCheck));
        else
          Server.Instance.UnsubscribeFromTimer(UpdateTimer.TimerStep.Step_1_0_min, new UpdateTimer.TimeStepDelegate(this.SelfDestructCheck));
      }
    }

    public void SendStatsToClient()
    {
      DynamicObjectStatsMessage objectStatsMessage = new DynamicObjectStatsMessage();
      objectStatsMessage.Info.GUID = this.GUID;
      objectStatsMessage.Info.Stats = this.StatsNew;
      if (this.Parent != null)
        Server.Instance.NetworkController.SendToClientsSubscribedToParents((NetworkData) objectStatsMessage, this.Parent, -1L, 4);
      this.StatsChanged = false;
    }

    public void PickedUp()
    {
      if (this.pickedUp)
        return;
      this.pickedUp = true;
      if ((double) this.RespawnTime > 0.0)
        Server.Instance.DynamicObjectsRespawnList.Add(new Server.DynamicObjectsRespawn()
        {
          Data = this.DynamicObjectSceneData,
          Parent = this.Parent,
          Timer = (double) this.RespawnTime,
          RespawnTime = (double) this.RespawnTime,
          MaxHealth = this.SpawnMaxHealth,
          MinHealth = this.SpawnMinHealth,
          WearMultiplier = this.SpawnWearMultiplier,
          APDetails = this.APDetails
        });
      if (this.IsPartOfSpawnSystem)
        SpawnManager.RemoveSpawnSystemObject((SpaceObject) this, false);
    }

    public DynamicObject(DynamicObjectSceneData dosd, SpaceObject parent, long guid = -1)
      : base(guid == -1L ? GUIDFactory.NextObjectGUID() : guid, dosd.Position.ToVector3D(), QuaternionD.LookRotation(dosd.Forward.ToVector3D(), dosd.Up.ToVector3D()))
    {
      this.DynamicObjectSceneData = ObjectCopier.DeepCopy<DynamicObjectSceneData>(dosd, 10);
      this.ItemID = this.DynamicObjectSceneData.ItemID;
      this.ItemType = StaticData.DynamicObjectsDataList[this.ItemID].ItemType;
      this.Parent = parent;
      this.Item = Item.Create(this, this.ItemType, this.DynamicObjectSceneData.AuxData);
      if (this.Item is ICargo)
      {
        ICargo cargo = this.Item as ICargo;
        if (cargo.Compartments != null)
        {
          foreach (CargoCompartmentData cargoCompartmentData in cargo.Compartments.Where<CargoCompartmentData>((Func<CargoCompartmentData, bool>) (m => m.Resources != null)))
          {
            foreach (ZeroGravity.Data.CargoResourceData cargoResourceData in cargoCompartmentData.Resources.Where<ZeroGravity.Data.CargoResourceData>((Func<ZeroGravity.Data.CargoResourceData, bool>) (m => m.SpawnSettings != null)))
            {
              foreach (ResourcesSpawnSettings spawnSetting in cargoResourceData.SpawnSettings)
              {
                if (this.Parent is SpaceObjectVessel && (this.Parent as SpaceObjectVessel).CheckTag(spawnSetting.Tag, spawnSetting.Case))
                {
                  float num = MathHelper.RandomRange(spawnSetting.MinQuantity, spawnSetting.MaxQuantity);
                  cargoResourceData.Quantity = 0.0f;
                  float max = cargoCompartmentData.Capacity - cargoCompartmentData.Resources.Sum<ZeroGravity.Data.CargoResourceData>((Func<ZeroGravity.Data.CargoResourceData, float>) (m => m.Quantity));
                  cargoResourceData.Quantity = MathHelper.Clamp(num, 0.0f, max);
                  break;
                }
              }
            }
          }
        }
        if (cargo is Canister && cargo.Compartments != null)
          cargo.GetCompartment(new int?()).Resources.RemoveAll((Predicate<ZeroGravity.Data.CargoResourceData>) (m => (double) m.Quantity <= 1.40129846432482E-45));
      }
      Server.Instance.Add(this);
      this.ConnectToNetworkController();
      this.LastChangeTime = Server.Instance.SolarSystem.CurrentTime;
    }

    private void SelfDestructCheck(double dbl)
    {
      if (!(this.Parent is Pivot) || (DateTime.UtcNow - this.lastSenderTime).TotalSeconds < 300.0)
        return;
      Server.Instance.UnsubscribeFromTimer(UpdateTimer.TimerStep.Step_1_0_min, new UpdateTimer.TimeStepDelegate(this.SelfDestructCheck));
      this.Destroy();
    }

    public void ConnectToNetworkController()
    {
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (MoveDynamicObectMessage), new EventSystem.NetworkDataDelegate(this.MoveDynamicObectMessageListener));
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (DynamicObjectStatsMessage), new EventSystem.NetworkDataDelegate(this.DynamicObjectStatsMessageListener));
    }

    public void DisconnectFromNetworkController()
    {
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (MoveDynamicObectMessage), new EventSystem.NetworkDataDelegate(this.MoveDynamicObectMessageListener));
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (DynamicObjectStatsMessage), new EventSystem.NetworkDataDelegate(this.DynamicObjectStatsMessageListener));
    }

    private void MoveDynamicObectMessageListener(NetworkData data)
    {
      MoveDynamicObectMessage dynamicObectMessage = data as MoveDynamicObectMessage;
      if (dynamicObectMessage.GUID != this.GUID || this.ListenToSenderID != 0L && dynamicObectMessage.Sender != this.ListenToSenderID && this._listenToPlayer != null && (this._listenToPlayer.Parent == this.Parent || this._listenToSenderID == dynamicObectMessage.Sender || this.Parent.ObjectType == SpaceObjectType.DynamicObjectPivot) && (DateTime.UtcNow - this.lastSenderTime).TotalSeconds < this.WaitForSenderTime)
        return;
      this.ListenToSenderID = dynamicObectMessage.Sender;
      this.lastSenderTime = DateTime.UtcNow;
      dynamicObectMessage.LocalPosition.ToVector3D();
      dynamicObectMessage.LocalRotation.ToQuaternionD();
      bool flag = false;
      if (flag || !this.LocalPosition.IsEpsilonEqual(dynamicObectMessage.LocalPosition.ToVector3D(), 0.0001))
      {
        this.LocalPosition = dynamicObectMessage.LocalPosition.ToVector3D();
        flag = true;
      }
      if (flag || !this.LocalRotation.IsEpsilonEqual(dynamicObectMessage.LocalRotation.ToQuaternionD(), 1E-05))
      {
        this.LocalRotation = dynamicObectMessage.LocalRotation.ToQuaternionD();
        flag = true;
      }
      if (flag)
        this.LastChangeTime = Server.Instance.SolarSystem.CurrentTime;
    }

    public void SetStatsChanged()
    {
    }

    private string GetUnknownAttachMessage(DynamicObjectAttachData data)
    {
      DynamicObjectAttachData currAttachData = this.GetCurrAttachData();
      return string.Format("Current: {0}, {1}, {10}, {2}, {3}, {4}\r\nNew: {5}, {6}, {7}, {8}, {9}", (object) currAttachData.ParentGUID, (object) currAttachData.ParentType, (object) currAttachData.IsAttached, (object) currAttachData.InventorySlotID, (object) (currAttachData.APDetails != null ? currAttachData.APDetails.InSceneID : 0), (object) data.ParentGUID, (object) data.ParentType, (object) data.IsAttached, (object) data.InventorySlotID, (object) (data.APDetails != null ? data.APDetails.InSceneID : 0), (object) (GameScenes.SceneID) (this.Parent == null || !(this.Parent is Ship) ? -1 : (int) (this.Parent as Ship).SceneID));
    }

    private bool CanBePickedUp(Player player, DynamicObject parentDObj)
    {
      return parentDObj.Parent == player || parentDObj.Item != null && parentDObj.Item is Outfit && !(parentDObj.Parent is Player);
    }

    private void DynamicObjectStatsMessageListener(NetworkData data)
    {
      DynamicObjectStatsMessage objectStatsMessage = data as DynamicObjectStatsMessage;
      if (objectStatsMessage.Info.GUID != this.GUID)
        return;
      SpaceObject parent = this.Parent;
      try
      {
        if (objectStatsMessage.Info.Stats != null && this.Item != null && this.Parent.GUID == objectStatsMessage.Sender)
          this.StatsChanged = this.Item.ChangeStats(objectStatsMessage.Info.Stats) || this.StatsChanged;
        if (objectStatsMessage.AttachData != null)
        {
          bool flag = false;
          if (objectStatsMessage.AttachData.ParentType == SpaceObjectType.Player)
          {
            Player player = Server.Instance.GetObject(objectStatsMessage.AttachData.ParentGUID) as Player;
            if (player != null && this.Item != null && (!(parent is Player) || parent == player) && (!(parent is DynamicObject) || this.CanBePickedUp(player, parent as DynamicObject)) && player.PlayerInventory.AddItemToInventory(this.Item, objectStatsMessage.AttachData.InventorySlotID))
            {
              this.Parent = (SpaceObject) player;
              if (parent is Pivot)
              {
                Pivot pivot = parent as Pivot;
                foreach (Player allPlayer in Server.Instance.AllPlayers)
                {
                  if (allPlayer.IsSubscribedTo(pivot.GUID))
                    allPlayer.UnsubscribeFrom((SpaceObject) pivot);
                }
                Server.Instance.SolarSystem.RemoveArtificialBody((ArtificialBody) pivot);
              }
              else if (parent is SpaceObjectVessel)
              {
                if (this.Item.AttachPointType != AttachPointType.None || this.Item.AttachPointKey != null)
                {
                  if (this.Item is MachineryPart)
                    (parent as SpaceObjectVessel).RemoveMachineryPart(this.Item.AttachPointKey);
                  this.Item.SetAttachPoint((AttachPointDetails) null);
                }
              }
              else if (!(parent is Corpse) && (!(parent is Player) || parent != player) && (!(parent is DynamicObject) || parent.Parent != player))
                Dbg.Warning((object) "UNKNOWN ATTACH", (object) this.GetUnknownAttachMessage(objectStatsMessage.AttachData));
            }
            else
              goto label_112;
          }
          else if (objectStatsMessage.AttachData.ParentType == SpaceObjectType.Ship || objectStatsMessage.AttachData.ParentType == SpaceObjectType.Asteroid || objectStatsMessage.AttachData.ParentType == SpaceObjectType.Station)
          {
            SpaceObjectVessel spaceObjectVessel = Server.Instance.GetObject(objectStatsMessage.AttachData.ParentGUID) as SpaceObjectVessel;
            if (spaceObjectVessel != null && spaceObjectVessel != parent)
            {
              if (parent is Player && objectStatsMessage.Sender == parent.GUID)
              {
                if ((parent as Player).PlayerInventory.DropItem(this.InvSlotID))
                {
                  this.Parent = (SpaceObject) spaceObjectVessel;
                  if (this.Item != null && objectStatsMessage.AttachData.APDetails != null)
                    this.Item.SetAttachPoint(objectStatsMessage.AttachData.APDetails);
                  if (this.Item != null && (uint) this.Item.AttachPointType > 0U && (this.Item is MachineryPart && this.Item.AttachPointType == AttachPointType.MachineryPartSlot))
                    spaceObjectVessel.FitMachineryPart(this.Item.AttachPointKey, this.Item as MachineryPart);
                }
                else
                  goto label_112;
              }
              else if (parent is Pivot && this.ListenToSenderID == objectStatsMessage.Sender)
              {
                Pivot pivot = parent as Pivot;
                this.LocalPosition = objectStatsMessage.AttachData.LocalPosition.ToVector3D();
                this.LocalRotation = objectStatsMessage.AttachData.LocalRotation.ToQuaternionD();
                this.Parent = (SpaceObject) spaceObjectVessel;
                foreach (Player allPlayer in Server.Instance.AllPlayers)
                {
                  if (allPlayer.IsSubscribedTo(pivot.GUID))
                    allPlayer.UnsubscribeFrom((SpaceObject) pivot);
                }
                Server.Instance.SolarSystem.RemoveArtificialBody((ArtificialBody) pivot);
              }
              else if (parent is DynamicObject && parent.Parent.GUID == objectStatsMessage.Sender)
              {
                Item obj = (parent as DynamicObject).Item;
                if (obj != null && this.Item != null && obj.RemoveChildItem(this.Item))
                  this.Parent = (SpaceObject) spaceObjectVessel;
                else
                  goto label_112;
              }
              else if (parent is Ship)
              {
                Ship ship = parent as Ship;
                this.LocalPosition = objectStatsMessage.AttachData.LocalPosition.ToVector3D();
                this.LocalRotation = objectStatsMessage.AttachData.LocalRotation.ToQuaternionD();
                this.Parent = (SpaceObject) spaceObjectVessel;
              }
              else
              {
                Dbg.Warning((object) "UNKNOWN ATTACH", (object) this.GetUnknownAttachMessage(objectStatsMessage.AttachData));
                goto label_112;
              }
              if (!this.IsAttached)
              {
                if (objectStatsMessage.AttachData.LocalPosition != null)
                  this.LocalPosition = objectStatsMessage.AttachData.LocalPosition.ToVector3D();
                if (objectStatsMessage.AttachData.LocalRotation != null)
                  this.LocalRotation = objectStatsMessage.AttachData.LocalRotation.ToQuaternionD();
              }
            }
            else
              goto label_112;
          }
          else if (objectStatsMessage.AttachData.ParentType == SpaceObjectType.DynamicObjectPivot)
          {
            if (!(parent is Pivot))
            {
              if (parent is SpaceObjectVessel && !this.IsAttached)
              {
                SpaceObjectVessel spaceObjectVessel = parent as SpaceObjectVessel;
                Pivot pivot = new Pivot((SpaceObjectTransferable) this, spaceObjectVessel.DockedToMainVessel != null ? (ArtificialBody) spaceObjectVessel.DockedToMainVessel : (ArtificialBody) spaceObjectVessel);
                this.Parent = (SpaceObject) pivot;
                this.pivotPositionCorrection = Vector3D.Zero;
                this.pivotVelocityCorrection = Vector3D.Zero;
                foreach (Player allPlayer in Server.Instance.AllPlayers)
                {
                  if (allPlayer.IsSubscribedTo(spaceObjectVessel.GUID) || allPlayer.Parent == spaceObjectVessel)
                    allPlayer.SubscribeTo((SpaceObject) pivot);
                }
              }
              else if (parent is Player && parent.Parent is ArtificialBody)
              {
                if ((this.Parent as Player).PlayerInventory.DropItem(this.InvSlotID))
                {
                  Pivot pivot = new Pivot((SpaceObjectTransferable) this, this.Parent.Parent as ArtificialBody);
                  this.Parent = (SpaceObject) pivot;
                  this.pivotPositionCorrection = Vector3D.Zero;
                  this.pivotVelocityCorrection = Vector3D.Zero;
                  foreach (Player allPlayer in Server.Instance.AllPlayers)
                  {
                    if (allPlayer.IsSubscribedTo(parent.Parent.GUID))
                      allPlayer.SubscribeTo((SpaceObject) pivot);
                  }
                }
                else
                  goto label_112;
              }
              else if (parent is DynamicObject && parent.Parent.GUID == objectStatsMessage.Sender && parent.Parent.Parent is ArtificialBody)
              {
                Item obj = (parent as DynamicObject).Item;
                if (obj != null && this.Item != null && obj.RemoveChildItem(this.Item))
                {
                  Pivot pivot = new Pivot((SpaceObjectTransferable) this, parent.Parent.Parent as ArtificialBody);
                  this.Parent = (SpaceObject) pivot;
                  this.pivotPositionCorrection = Vector3D.Zero;
                  this.pivotVelocityCorrection = Vector3D.Zero;
                  foreach (Player allPlayer in Server.Instance.AllPlayers)
                  {
                    if (allPlayer.IsSubscribedTo(parent.Parent.Parent.GUID))
                      allPlayer.SubscribeTo((SpaceObject) pivot);
                  }
                }
                else
                  goto label_112;
              }
              else
              {
                Dbg.Warning((object) "UNKNOWN ATTACH", (object) this.GetUnknownAttachMessage(objectStatsMessage.AttachData));
                goto label_112;
              }
              if (!this.IsAttached)
              {
                if (objectStatsMessage.AttachData.LocalPosition != null)
                  this.LocalPosition = objectStatsMessage.AttachData.LocalPosition.ToVector3D();
                if (objectStatsMessage.AttachData.LocalRotation != null)
                  this.LocalRotation = objectStatsMessage.AttachData.LocalRotation.ToQuaternionD();
              }
            }
            else
              goto label_112;
          }
          else if (objectStatsMessage.AttachData.ParentType == SpaceObjectType.DynamicObject)
          {
            DynamicObject dynamicObject = Server.Instance.GetObject(objectStatsMessage.AttachData.ParentGUID) as DynamicObject;
            if (dynamicObject != null && parent != dynamicObject && (this.Item != null && dynamicObject.Item != null) && (!(parent is Player) || parent == dynamicObject.Parent) && (!(parent is DynamicObject) || parent.Parent == dynamicObject.Parent))
            {
              if (parent is Player)
              {
                if ((parent as Player).PlayerInventory.DropItem(this.InvSlotID) && dynamicObject.Item.AttachChildItem(this.Item))
                {
                  this.PickedUp();
                  this.Parent = (SpaceObject) dynamicObject;
                }
                else
                  goto label_112;
              }
              else if (parent is Pivot)
              {
                if (dynamicObject.Item.AttachChildItem(this.Item))
                {
                  this.PickedUp();
                  Pivot pivot = parent as Pivot;
                  foreach (Player allPlayer in Server.Instance.AllPlayers)
                  {
                    if (allPlayer.IsSubscribedTo(pivot.GUID))
                      allPlayer.UnsubscribeFrom((SpaceObject) pivot);
                  }
                  Server.Instance.SolarSystem.RemoveArtificialBody((ArtificialBody) pivot);
                  this.Parent = (SpaceObject) dynamicObject;
                }
                else
                  goto label_112;
              }
              else if (parent is SpaceObjectVessel)
              {
                if (dynamicObject.Item.AttachChildItem(this.Item))
                {
                  this.PickedUp();
                  if (this.Item.AttachPointType != AttachPointType.None || this.Item.AttachPointKey != null)
                  {
                    if (this.Item is MachineryPart)
                      (parent as SpaceObjectVessel).RemoveMachineryPart(this.Item.AttachPointKey);
                    this.Item.SetAttachPoint((AttachPointDetails) null);
                  }
                  this.Parent = (SpaceObject) dynamicObject;
                }
                else
                  goto label_112;
              }
              else
                Dbg.Warning((object) "UNKNOWN ATTACH", (object) this.GetUnknownAttachMessage(objectStatsMessage.AttachData));
            }
            else
              goto label_112;
          }
          flag = true;
label_112:
          if (flag)
          {
            this.LastChangeTime = Server.Instance.SolarSystem.CurrentTime;
            if (this.Parent is SpaceObjectVessel)
            {
              Player player = Server.Instance.GetPlayer(objectStatsMessage.Sender);
              if (player != null && this.Parent == player.Parent)
              {
                this.ListenToSenderID = objectStatsMessage.Sender;
                this.lastSenderTime = DateTime.UtcNow;
              }
            }
            else
            {
              this.ListenToSenderID = objectStatsMessage.Sender;
              this.lastSenderTime = DateTime.UtcNow;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
      if (!this.StatsChanged && objectStatsMessage.AttachData == null)
        return;
      int num = !this.StatsChanged ? 0 : (this.Item != null ? 1 : 0);
      objectStatsMessage.Info.Stats = num == 0 ? (DynamicObjectStats) null : this.Item.StatsNew;
      if (objectStatsMessage.AttachData != null)
      {
        float[] velocity = objectStatsMessage.AttachData.Velocity;
        float[] torque = objectStatsMessage.AttachData.Torque;
        float[] throwForce = objectStatsMessage.AttachData.ThrowForce;
        objectStatsMessage.AttachData = this.GetCurrAttachData();
        objectStatsMessage.AttachData.Velocity = velocity;
        objectStatsMessage.AttachData.Torque = torque;
        objectStatsMessage.AttachData.ThrowForce = throwForce;
      }
      List<SpaceObject> parents = this.Parent.GetParents(true, 10);
      if (parent != null)
        parents.AddRange((IEnumerable<SpaceObject>) parent.GetParents(true, 10));
      Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) objectStatsMessage, -1L, parents.ToArray());
      if (this.DynamicObjects.Count > 0)
      {
        DynamicObjectsInfoMessage objectsInfoMessage = new DynamicObjectsInfoMessage();
        objectsInfoMessage.Infos = new List<DynamicObjectInfo>();
        foreach (DynamicObject dynamicObject in this.DynamicObjects)
        {
          if (dynamicObject.StatsChanged)
          {
            objectsInfoMessage.Infos.Add(new DynamicObjectInfo()
            {
              GUID = dynamicObject.GUID,
              Stats = dynamicObject.StatsNew
            });
            dynamicObject.StatsChanged = false;
          }
        }
        if (objectsInfoMessage.Infos.Count > 0)
          Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) objectsInfoMessage, -1L, parents.ToArray());
      }
      this.StatsChanged = false;
    }

    public DynamicObjectAttachData GetCurrAttachData()
    {
      DynamicObjectAttachData objectAttachData1 = new DynamicObjectAttachData();
      objectAttachData1.ParentGUID = this.Parent is Player ? (this.Parent as Player).FakeGuid : this.Parent.GUID;
      objectAttachData1.ParentType = this.Parent.ObjectType;
      objectAttachData1.IsAttached = this.IsAttached;
      objectAttachData1.InventorySlotID = this.InvSlotID;
      DynamicObjectAttachData objectAttachData2 = objectAttachData1;
      AttachPointDetails attachPointDetails;
      if (this.Item != null && this.Item.AttachPointKey != null)
        attachPointDetails = new AttachPointDetails()
        {
          InSceneID = this.Item.AttachPointKey.InSceneID
        };
      else
        attachPointDetails = (AttachPointDetails) null;
      objectAttachData2.APDetails = attachPointDetails;
      objectAttachData1.LocalPosition = this.IsAttached ? (float[]) null : this.LocalPosition.ToFloatArray();
      objectAttachData1.LocalRotation = this.IsAttached ? (float[]) null : this.LocalRotation.ToFloatArray();
      return objectAttachData1;
    }

    public MoveDynamicObectMessage GetMoveDynamicObectMessage()
    {
      return new MoveDynamicObectMessage()
      {
        GUID = this.GUID,
        LocalPosition = this.LocalPosition.ToFloatArray(),
        LocalRotation = this.LocalRotation.ToFloatArray()
      };
    }

    public bool PlayerReceivesMovementMessage(long playerGuid)
    {
      return playerGuid != this.ListenToSenderID && (ulong) this.ListenToSenderID > 0UL;
    }

    public void DestroyDynamicObject()
    {
      if (this.Item != null)
      {
        this.Item.SetSlot((InventorySlot) null);
        this.Item.SetAttachPoint((AttachPointDetails) null);
      }
      this.DisconnectFromNetworkController();
      base.Destroy();
    }

    public override SpawnObjectResponseData GetSpawnResponseData(Player pl)
    {
      SpawnDynamicObjectResponseData objectResponseData = new SpawnDynamicObjectResponseData();
      long guid = this.GUID;
      objectResponseData.GUID = guid;
      DynamicObjectDetails details = this.GetDetails();
      objectResponseData.Details = details;
      return (SpawnObjectResponseData) objectResponseData;
    }

    private List<DynamicObjectDetails> GetChildDynamicObjects()
    {
      if (this.DynamicObjects == null || this.DynamicObjects.Count == 0)
        return (List<DynamicObjectDetails>) null;
      List<DynamicObjectDetails> dynamicObjectDetailsList = new List<DynamicObjectDetails>();
      foreach (DynamicObject dynamicObject in this.DynamicObjects)
        dynamicObjectDetailsList.Add(dynamicObject.GetDetails());
      return dynamicObjectDetailsList;
    }

    public DynamicObjectDetails GetDetails()
    {
      return new DynamicObjectDetails()
      {
        GUID = this.GUID,
        ItemID = this.ItemID,
        LocalPosition = this.LocalPosition.ToFloatArray(),
        LocalRotation = this.LocalRotation.ToFloatArray(),
        StatsData = this.StatsNew,
        AttachData = this.GetCurrAttachData(),
        ChildObjects = this.GetChildDynamicObjects()
      };
    }

    public override void Destroy()
    {
      this.DestroyDynamicObject();
      base.Destroy();
    }

    public void FillPersistenceData(PersistenceObjectDataDynamicObject data)
    {
      data.GUID = this.GUID;
      data.ItemID = this.ItemID;
      data.LocalPosition = this.LocalPosition.ToFloatArray();
      data.LocalRotation = this.LocalRotation.ToFloatArray();
      if (!this.pickedUp && (double) this.RespawnTime > 0.0)
      {
        data.RespawnTime = new float?(this.RespawnTime);
        data.MaxHealth = new float?(this.SpawnMaxHealth);
        data.MinHealth = new float?(this.SpawnMinHealth);
        data.WearMultiplier = new float?(this.SpawnWearMultiplier);
        data.RespawnPosition = this.DynamicObjectSceneData.Position;
        data.RespawnForward = this.DynamicObjectSceneData.Forward;
        data.RespawnUp = this.DynamicObjectSceneData.Up;
        data.RespawnAuxData = this.DynamicObjectSceneData.AuxData;
      }
      data.ChildObjects = new List<PersistenceObjectData>();
      foreach (DynamicObject dynamicObject in this.DynamicObjects)
        data.ChildObjects.Add(dynamicObject.Item != null ? dynamicObject.Item.GetPersistenceData() : dynamicObject.GetPersistenceData());
    }

    public PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataDynamicObject data = new PersistenceObjectDataDynamicObject();
      this.FillPersistenceData(data);
      return (PersistenceObjectData) data;
    }

    public void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        PersistenceObjectDataDynamicObject dataDynamicObject = persistenceData as PersistenceObjectDataDynamicObject;
        this.ItemID = dataDynamicObject.ItemID;
        this.LocalPosition = dataDynamicObject.LocalPosition.ToVector3D();
        this.LocalRotation = dataDynamicObject.LocalRotation.ToQuaternionD();
        this.pickedUp = false;
        this.RespawnTime = -1f;
        this.SpawnMaxHealth = -1f;
        this.SpawnMinHealth = -1f;
        this.SpawnWearMultiplier = 1f;
        if (dataDynamicObject.RespawnTime.HasValue)
          this.RespawnTime = dataDynamicObject.RespawnTime.Value;
        if (dataDynamicObject.MaxHealth.HasValue)
          this.SpawnMaxHealth = dataDynamicObject.MaxHealth.Value;
        if (dataDynamicObject.MinHealth.HasValue)
          this.SpawnMinHealth = dataDynamicObject.MinHealth.Value;
        if (!dataDynamicObject.WearMultiplier.HasValue)
          return;
        this.SpawnWearMultiplier = dataDynamicObject.WearMultiplier.Value;
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }

    public static bool SpawnDynamicObject(ItemType itemType, GenericItemSubType subType, MachineryPartType mpType, SpaceObjectVessel vessel, int apId = -1, Vector3D? position = null, Vector3D? forward = null, Vector3D? up = null)
    {
      DynamicObjectData dynamicObjectData = itemType != ItemType.GenericItem ? (itemType != ItemType.MachineryPart ? StaticData.DynamicObjectsDataList.Values.Where<DynamicObjectData>((Func<DynamicObjectData, bool>) (m => m.ItemType == itemType)).First<DynamicObjectData>() : StaticData.DynamicObjectsDataList.Values.Where<DynamicObjectData>((Func<DynamicObjectData, bool>) (m =>
      {
        if (m.ItemType == itemType && m.DefaultAuxData != null && m.DefaultAuxData is MachineryPartData)
          return (m.DefaultAuxData as MachineryPartData).PartType == mpType;
        return false;
      })).First<DynamicObjectData>()) : StaticData.DynamicObjectsDataList.Values.Where<DynamicObjectData>((Func<DynamicObjectData, bool>) (m =>
      {
        if (m.ItemType == itemType && m.DefaultAuxData != null && m.DefaultAuxData is GenericItemData)
          return (m.DefaultAuxData as GenericItemData).SubType == subType;
        return false;
      })).First<DynamicObjectData>();
      if (dynamicObjectData == null)
        return false;
      DynamicObject dynamicObject = new DynamicObject(new DynamicObjectSceneData()
      {
        ItemID = dynamicObjectData.ItemID,
        Position = position.HasValue ? position.Value.ToFloatArray() : Vector3D.Zero.ToFloatArray(),
        Forward = forward.HasValue ? forward.Value.ToFloatArray() : Vector3D.Forward.ToFloatArray(),
        Up = up.HasValue ? up.Value.ToFloatArray() : Vector3D.Up.ToFloatArray(),
        AttachPointInSceneId = apId,
        AuxData = dynamicObjectData.DefaultAuxData,
        SpawnSettings = (DynaminObjectSpawnSettings[]) null
      }, (SpaceObject) vessel, -1L);
      if (dynamicObject.Item == null)
        return true;
      if (apId > 0)
      {
        AttachPointDetails data = new AttachPointDetails()
        {
          InSceneID = apId
        };
        dynamicObject.Item.SetAttachPoint(data);
        dynamicObject.APDetails = data;
      }
      if (dynamicObject.Item is MachineryPart)
      {
        (dynamicObject.Item as MachineryPart).WearMultiplier = 1f;
        if (dynamicObject.Item.AttachPointType == AttachPointType.MachineryPartSlot)
          vessel.FitMachineryPart(dynamicObject.Item.AttachPointKey, dynamicObject.Item as MachineryPart);
      }
      Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) new SpawnObjectsResponse()
      {
        Data = {
          dynamicObject.GetSpawnResponseData((Player) null)
        }
      }, -1L, dynamicObject.Parent);
      return true;
    }
  }
}
