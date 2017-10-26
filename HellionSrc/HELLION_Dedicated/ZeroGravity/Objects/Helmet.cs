// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Helmet
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  public class Helmet : Item, IItemWithPower
  {
    public float DamageReduction = 0.0f;
    public float DamageResistance = 1f;
    private HelmetStats _stats;
    public bool IsVisorToggleable;
    private bool _isVisorActive;
    private bool _isLightActive;
    public Jetpack Jetpack;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) this._stats;
      }
    }

    public bool HasPower
    {
      get
      {
        return this.Jetpack != null && this.Jetpack.HasPower;
      }
    }

    public bool IsVisorActive
    {
      get
      {
        return this._isVisorActive;
      }
      set
      {
        this._isVisorActive = value;
        this._stats.isVisorActive = new bool?(value);
      }
    }

    public bool IsLightActive
    {
      get
      {
        return this._isLightActive;
      }
      set
      {
        this._isLightActive = value;
        this._stats.isLightActive = new bool?(value);
      }
    }

    public Helmet(IDynamicObjectAuxData data)
    {
      this._stats = new HelmetStats();
      if (data == null)
        return;
      this.SetData(data as HelmetData);
    }

    private void SetData(HelmetData hd)
    {
      this.IsLightActive = hd.IsLightActive;
      this.IsVisorActive = hd.IsVisorActive;
      this.IsVisorToggleable = hd.IsVisorToggleable;
      this.Health = hd.Health;
      this.MaxHealth = hd.MaxHealth;
      this.DamageReduction = hd.DamageReduction;
      this.DamageResistance = hd.DamageResistance;
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      HelmetStats helmetStats = stats as HelmetStats;
      bool? nullable;
      int num;
      if (helmetStats.isLightActive.HasValue)
      {
        nullable = helmetStats.isLightActive;
        bool isLightActive = this.IsLightActive;
        num = nullable.GetValueOrDefault() == isLightActive ? (!nullable.HasValue ? 1 : 0) : 1;
      }
      else
        num = 0;
      if (num != 0)
      {
        if (helmetStats.isLightActive.Value && !this.HasPower)
          return false;
        this.IsLightActive = helmetStats.isLightActive.Value;
      }
      if (helmetStats.isVisorActive.HasValue)
      {
        nullable = helmetStats.isVisorActive;
        bool isVisorActive = this.IsVisorActive;
        if (nullable.GetValueOrDefault() == isVisorActive && nullable.HasValue)
          return false;
        this.IsVisorActive = helmetStats.isVisorActive.Value;
      }
      return true;
    }

    public void UpdateStats(bool hasPower)
    {
      if (hasPower)
        return;
      this.IsLightActive = false;
      this.DynamicObj.StatsChanged = true;
    }

    protected override void ChangeEquip(Inventory.EquipType equipType)
    {
      if (!(this.DynamicObj.Parent is Player))
        return;
      Player parent = this.DynamicObj.Parent as Player;
      if (equipType == Inventory.EquipType.EquipInventory)
      {
        parent.CurrentHelmet = this;
        if (parent.CurrentJetpack == null)
          return;
        this.Jetpack = parent.CurrentJetpack;
        this.Jetpack.Helmet = this;
      }
      else if (parent.CurrentHelmet == this)
      {
        parent.CurrentHelmet = (Helmet) null;
        if (this.Jetpack != null)
        {
          this.Jetpack.Helmet = (Helmet) null;
          this.Jetpack = (Jetpack) null;
          this.UpdateStats(false);
        }
      }
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataHelmet objectDataHelmet = new PersistenceObjectDataHelmet();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataHelmet);
      objectDataHelmet.HelmetData = new HelmetData();
      objectDataHelmet.HelmetData.ItemType = this.Type;
      objectDataHelmet.HelmetData.IsVisorToggleable = this.IsVisorToggleable;
      objectDataHelmet.HelmetData.IsLightActive = this.IsLightActive;
      objectDataHelmet.HelmetData.IsVisorActive = this.IsVisorActive;
      objectDataHelmet.HelmetData.DamageReduction = this.DamageReduction;
      objectDataHelmet.HelmetData.DamageResistance = this.DamageResistance;
      objectDataHelmet.HelmetData.Health = this.Health;
      objectDataHelmet.HelmetData.MaxHealth = this.MaxHealth;
      return (PersistenceObjectData) objectDataHelmet;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataHelmet objectDataHelmet = persistenceData as PersistenceObjectDataHelmet;
        if (objectDataHelmet == null)
          Dbg.Warning((object) "PersistenceObjectDataHelmet data is null", (object) this.GUID);
        else
          this.SetData(objectDataHelmet.HelmetData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
