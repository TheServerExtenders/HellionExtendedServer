// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.HandDrill
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  internal class HandDrill : Item
  {
    private HandDrillStats _stats;
    private double _inAsteroidDistanceSqr;
    private Asteroid _inAsteroidObj;
    private long _inAsteroidGUID;
    private double lastDrilledTime;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) this._stats;
      }
    }

    public Battery CurBattery { get; private set; }

    public Canister CurCanister { get; private set; }

    public long InAsteroidGUID
    {
      get
      {
        return this._inAsteroidGUID;
      }
      set
      {
        this._inAsteroidObj = value <= 0L ? (Asteroid) null : Server.Instance.GetObject(value) as Asteroid;
        this._inAsteroidDistanceSqr = this._inAsteroidObj != null ? Math.Pow(this._inAsteroidObj.Radius + 100.0, 2.0) : 0.0;
        this._inAsteroidGUID = value;
        this._stats.InAsteroidGUID = new long?(value);
      }
    }

    public float BatteryUsage { get; private set; }

    public float DrillingStrength { get; private set; }

    public bool HasPower
    {
      get
      {
        return this.CurBattery != null && this.CurBattery.HasPower;
      }
    }

    public bool HasSpace
    {
      get
      {
        return this.CurCanister != null && this.CurCanister.HasSpace;
      }
    }

    public HandDrill(IDynamicObjectAuxData data)
    {
      this._stats = new HandDrillStats();
      if (data == null)
        return;
      HandDrillData hd = data as HandDrillData;
      if (hd.BatteryGUID != -1L)
        this.CurBattery = (Server.Instance.GetObject(hd.BatteryGUID) as DynamicObject).Item as Battery;
      if (hd.CanisterGUID != -1L)
        this.CurCanister = (Server.Instance.GetObject(hd.CanisterGUID) as DynamicObject).Item as Canister;
      this.SetData(hd);
    }

    private void SetData(HandDrillData hd)
    {
      this.BatteryUsage = hd.BatteryConsumption;
      this.DrillingStrength = hd.DrillingStrength;
      this.Health = hd.Health;
      this.MaxHealth = hd.MaxHealth;
    }

    public override void Use()
    {
      float num = (float) (Server.Instance.SolarSystem.CurrentTime - this.lastDrilledTime);
      this.lastDrilledTime = Server.Instance.SolarSystem.CurrentTime;
      if (!this.HasPower || (double) num > 3.0)
        return;
      this.CurBattery.ChangeQuantity(-this.BatteryUsage * num);
      if (this._inAsteroidObj == null || !this.HasSpace || (!(this.DynamicObj.Parent is Player) || (this.DynamicObj.Parent.Position - this._inAsteroidObj.Position).SqrMagnitude >= this._inAsteroidDistanceSqr))
        return;
      short[] index;
      Dictionary<ResourceType, float> dbgres;
      Dictionary<ResourceType, float> drillingResources = this._inAsteroidObj.GetDrillingResources((this.DynamicObj.Parent as Player).LocalPosition, 0, this.DrillingStrength * num, out index, out dbgres);
      if (drillingResources != null)
        this.CurCanister.ChangeQuantity(drillingResources);
    }

    public override void SendAllStats()
    {
      if (this.CurBattery != null && this.CurBattery.DynamicObj.StatsChanged)
        this.CurBattery.DynamicObj.SendStatsToClient();
      if (this.CurCanister == null || !this.CurCanister.DynamicObj.StatsChanged)
        return;
      this.CurCanister.DynamicObj.SendStatsToClient();
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      HandDrillStats handDrillStats = stats as HandDrillStats;
      if (handDrillStats.InAsteroidGUID.HasValue)
        this.InAsteroidGUID = handDrillStats.InAsteroidGUID.Value;
      return true;
    }

    public override bool AttachChildItem(Item item)
    {
      if (item is Battery)
      {
        this.CurBattery = item as Battery;
        return true;
      }
      if (!(item is Canister))
        return false;
      this.CurCanister = item as Canister;
      return true;
    }

    public override bool RemoveChildItem(Item item)
    {
      if (item is Battery && this.CurBattery == item)
        this.CurBattery = (Battery) null;
      else if (item is Canister && this.CurCanister == item)
        this.CurCanister = (Canister) null;
      return true;
    }

    public override bool CanAttachChildItem(Item item)
    {
      return item is Battery || item is Canister;
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataHandDrill objectDataHandDrill = new PersistenceObjectDataHandDrill();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataHandDrill);
      objectDataHandDrill.HandDrillData = new HandDrillData();
      objectDataHandDrill.HandDrillData.ItemType = this.Type;
      objectDataHandDrill.HandDrillData.BatteryConsumption = this.BatteryUsage;
      objectDataHandDrill.HandDrillData.DrillingStrength = this.DrillingStrength;
      objectDataHandDrill.HandDrillData.Health = this.Health;
      objectDataHandDrill.HandDrillData.MaxHealth = this.MaxHealth;
      return (PersistenceObjectData) objectDataHandDrill;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataHandDrill objectDataHandDrill = persistenceData as PersistenceObjectDataHandDrill;
        if (objectDataHandDrill == null)
          Dbg.Warning((object) "PersistenceObjectDataHandDrill data is null", (object) this.GUID);
        else
          this.SetData(objectDataHandDrill.HandDrillData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
