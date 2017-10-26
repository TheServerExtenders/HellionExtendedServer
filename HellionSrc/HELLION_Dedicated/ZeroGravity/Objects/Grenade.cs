// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Grenade
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.Timers;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.ShipComponents;

namespace ZeroGravity.Objects
{
  public class Grenade : Item
  {
    private GrenadeStats gs;
    private bool isActive;
    private float damage;
    private float areaOfEffect;
    private float detonationTime;
    private TypeOfDamage DamageType;
    public long PlayersGUID;
    private double activationTime;
    private Timer destroyTimer;
    private bool isCanceled;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) this.gs;
      }
    }

    public Grenade(IDynamicObjectAuxData data)
    {
      this.gs = new GrenadeStats();
      if (data != null)
        this.SetData(data as GrenadeData);
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (GrenadeBlastMessage), new EventSystem.NetworkDataDelegate(this.GrenadeBlastMessageListener));
    }

    private void GrenadeBlastMessageListener(NetworkData data)
    {
      GrenadeBlastMessage grenadeBlastMessage = data as GrenadeBlastMessage;
      if (grenadeBlastMessage.GrenadeGUID != this.GUID || this.PlayersGUID != grenadeBlastMessage.Sender)
        return;
      if (grenadeBlastMessage.Guids != null && (uint) grenadeBlastMessage.Guids.Length > 0U)
      {
        Vector3D position = this.DynamicObj.Position;
        foreach (long guid in grenadeBlastMessage.Guids)
        {
          SpaceObject spaceObject = Server.Instance.GetObject(guid);
          if (spaceObject is Player)
            (spaceObject as Player).Stats.TakeDammage(0.0f, new Vector3D?(), 0.0f, 0.0f, 0.0f, this.damage, 0.0f);
          if (spaceObject is DynamicObject && (spaceObject as DynamicObject).Item != null)
            (spaceObject as DynamicObject).Item.TakeDamage(new Dictionary<TypeOfDamage, float>()
            {
              {
                this.DamageType,
                this.damage
              }
            });
        }
        Extensions.Invoke((Action) (() => this.DestroyItem()), 1.0);
      }
      if (this.DamageType == TypeOfDamage.Impact && this.DynamicObj.Parent is SpaceObjectVessel)
      {
        List<VesselRepairPoint> repairPoints = (List<VesselRepairPoint>) null;
        SpaceObjectVessel parent = this.DynamicObj.Parent as SpaceObjectVessel;
        if (grenadeBlastMessage.RepairPointIDs != null)
        {
          repairPoints = new List<VesselRepairPoint>();
          foreach (int repairPointId in grenadeBlastMessage.RepairPointIDs)
          {
            int rpid = repairPointId;
            repairPoints.Add(parent.RepairPoints.Find((Predicate<VesselRepairPoint>) (m => m.ID.InSceneID == rpid)));
          }
        }
        if ((double) parent.ChangeHealthBy(-this.damage, repairPoints) != 0.0)
        {
          ShipCollisionMessage collisionMessage = new ShipCollisionMessage();
          collisionMessage.CollisionVelocity = 0.0f;
          long guid = this.DynamicObj.Parent.GUID;
          collisionMessage.ShipOne = guid;
          long num = -1;
          collisionMessage.ShipTwo = num;
          Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) collisionMessage, -1L, this.DynamicObj.Parent);
        }
      }
    }

    public override void DestroyItem()
    {
      base.DestroyItem();
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (GrenadeBlastMessage), new EventSystem.NetworkDataDelegate(this.GrenadeBlastMessageListener));
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      GrenadeStats grenadeStats = stats as GrenadeStats;
      if (grenadeStats.IsActive.HasValue && grenadeStats.IsActive.Value != this.isActive)
      {
        this.PlayersGUID = this.DynamicObj.Parent.GUID;
        bool? isActive;
        int num;
        if (this.isActive)
        {
          isActive = grenadeStats.IsActive;
          bool flag = false;
          num = isActive.GetValueOrDefault() == flag ? (isActive.HasValue ? 1 : 0) : 0;
        }
        else
          num = 0;
        if (num != 0)
        {
          this.isCanceled = true;
          this.activationTime = -1.0;
          this.destroyTimer.Dispose();
        }
        this.gs.IsActive = new bool?(this.isActive = grenadeStats.IsActive.Value);
        isActive = this.gs.IsActive;
        bool flag1 = true;
        if (isActive.GetValueOrDefault() == flag1 && isActive.HasValue)
        {
          this.isCanceled = false;
          this.activationTime = Server.Instance.SolarSystem.CurrentTime;
          this.CallBlastAfterTime(new double?());
        }
      }
      return false;
    }

    private void SetData(GrenadeData l)
    {
      this.Health = l.Health;
      this.MaxHealth = l.MaxHealth;
      this.gs.IsActive = new bool?(l.IsActive);
      this.damage = l.Damage;
      this.areaOfEffect = l.AreaOfEffect;
      this.detonationTime = l.DetonationTime;
      this.DamageType = (TypeOfDamage) l.TypeOfDamage;
      if (!l.IsActive)
        return;
      this.CallBlastAfterTime(new double?());
    }

    public void CallBlastAfterTime(double? time = null)
    {
      this.destroyTimer = new Timer(TimeSpan.FromSeconds((double) this.detonationTime).TotalMilliseconds);
      this.destroyTimer.Elapsed += (ElapsedEventHandler) ((sender, args) => this.Blast());
      this.destroyTimer.Enabled = true;
    }

    private void Blast()
    {
      if (this.DynamicObj.Parent == null)
        return;
      if (!this.isActive || this.isCanceled || this.activationTime == -1.0 || Server.Instance.SolarSystem.CurrentTime - this.activationTime < (double) this.detonationTime * 0.899999976158142)
      {
        this.isCanceled = false;
        this.activationTime = -1.0;
      }
      else
      {
        this.gs.Blast = new bool?(true);
        this.DynamicObj.SendStatsToClient();
      }
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataGrenade objectDataGrenade = new PersistenceObjectDataGrenade();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataGrenade);
      objectDataGrenade.GrenadeData = new GrenadeData();
      objectDataGrenade.GrenadeData.ItemType = this.Type;
      objectDataGrenade.GrenadeData.AreaOfEffect = this.areaOfEffect;
      objectDataGrenade.GrenadeData.DetonationTime = this.detonationTime;
      objectDataGrenade.GrenadeData.Damage = this.damage;
      objectDataGrenade.GrenadeData.Health = this.Health;
      objectDataGrenade.GrenadeData.MaxHealth = this.MaxHealth;
      objectDataGrenade.GrenadeData.IsActive = false;
      return (PersistenceObjectData) objectDataGrenade;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataGrenade objectDataGrenade = persistenceData as PersistenceObjectDataGrenade;
        if (objectDataGrenade == null)
          Dbg.Warning((object) "PersistenceObjectDataHandheldGrenade data is null", (object) this.GUID);
        else
          this.SetData(objectDataGrenade.GrenadeData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
