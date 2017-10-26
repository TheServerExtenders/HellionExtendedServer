// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Battery
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  public class Battery : Item, IItemWithPower
  {
    private BatteryStats _stats;
    private float _currentPower;
    private float accumulatedPower;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        this.accumulatedPower = 0.0f;
        return (DynamicObjectStats) this._stats;
      }
    }

    public bool HasPower
    {
      get
      {
        return (double) this.CurrentPower > 1.40129846432482E-45;
      }
    }

    public float CurrentPower
    {
      get
      {
        return this._currentPower;
      }
      set
      {
        this._currentPower = value;
        this._stats.CurrentPower = new float?(value);
      }
    }

    public float MaxPower { get; private set; }

    public float ChargeAmount
    {
      get
      {
        return 1f;
      }
    }

    public Battery(IDynamicObjectAuxData data)
    {
      this._stats = new BatteryStats();
      if (data == null)
        return;
      this.SetData(data as BatteryData);
    }

    private void SetData(BatteryData bd)
    {
      this.MaxPower = bd.MaxPower;
      this.CurrentPower = bd.CurrentPower;
      this.Health = bd.Health;
      this.MaxHealth = bd.MaxHealth;
    }

    public void ChangeQuantity(float amount)
    {
      this.CurrentPower = Math.Min(this.CurrentPower + amount, this.MaxPower);
      this.accumulatedPower = this.accumulatedPower + amount;
      if ((double) this.CurrentPower < 1.40129846432482E-45)
      {
        this.CurrentPower = 0.0f;
        this.DynamicObj.SendStatsToClient();
      }
      else if ((double) this.accumulatedPower > (double) this.MaxPower * 0.00999999977648258)
        this.DynamicObj.SendStatsToClient();
      else
        this.DynamicObj.StatsChanged = true;
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      return false;
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataBattery objectDataBattery = new PersistenceObjectDataBattery();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataBattery);
      objectDataBattery.BatteryData = new BatteryData();
      objectDataBattery.BatteryData.ItemType = this.Type;
      objectDataBattery.BatteryData.Health = this.Health;
      objectDataBattery.BatteryData.MaxHealth = this.MaxHealth;
      objectDataBattery.BatteryData.CurrentPower = this.CurrentPower;
      objectDataBattery.BatteryData.MaxPower = this.MaxPower;
      return (PersistenceObjectData) objectDataBattery;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataBattery objectDataBattery = persistenceData as PersistenceObjectDataBattery;
        if (objectDataBattery == null)
          Dbg.Warning((object) "PersistenceObjectDataBattery data is null", (object) this.GUID);
        else
          this.SetData(objectDataBattery.BatteryData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
