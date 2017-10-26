// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Weapon
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  public class Weapon : Item, IItemWithPower
  {
    private List<WeaponModData> weaponMods = new List<WeaponModData>();
    private WeaponStats _stats;
    private float maxPower;
    private float _currentPower;
    private int _currentModIndex;
    private double lastShotTime;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) this._stats;
      }
    }

    public float Damage
    {
      get
      {
        return this.CurrentMod.Damage;
      }
      set
      {
      }
    }

    public bool HasPower
    {
      get
      {
        return (double) this.CurrentPower > 1.40129846432482E-45;
      }
      set
      {
      }
    }

    public bool HasAmmo
    {
      get
      {
        return this.Magazine != null && this.Magazine.HasAmmo;
      }
    }

    public Magazine Magazine { get; private set; }

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

    public int CurrentModIndex
    {
      get
      {
        return this._currentModIndex;
      }
      set
      {
        this._currentModIndex = value;
        this.CurrentMod = this.weaponMods[this._currentModIndex];
        this._stats.CurrentMod = new int?(this._currentModIndex);
      }
    }

    public WeaponModData CurrentMod { get; private set; }

    public float ChargeAmount
    {
      get
      {
        return 1f;
      }
    }

    public Weapon(IDynamicObjectAuxData data)
    {
      this._stats = new WeaponStats();
      if (data == null)
        return;
      WeaponData wd = data as WeaponData;
      if (wd.MagazineGUID != -1L)
        this.Magazine = (Server.Instance.GetObject(wd.MagazineGUID) as DynamicObject).Item as Magazine;
      this.SetData(wd);
    }

    private void SetData(WeaponData wd)
    {
      this.maxPower = wd.MaxPower;
      this.weaponMods = wd.weaponMods;
      this.Health = wd.Health;
      this.MaxHealth = wd.MaxHealth;
      foreach (WeaponModData weaponMod in this.weaponMods)
        weaponMod.RateOfFire *= 0.95f;
      this.CurrentPower = System.Math.Min(wd.CurrentPower, this.maxPower);
      this.CurrentModIndex = wd.CurrentMod;
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      WeaponStats weaponStats = stats as WeaponStats;
      if (!weaponStats.CurrentMod.HasValue)
        return false;
      this.CurrentModIndex = MathHelper.Clamp(weaponStats.CurrentMod.Value, 0, this.weaponMods.Count - 1);
      return true;
    }

    public void ConsumePower(double amount)
    {
    }

    public bool CanShoot()
    {
      if ((double) this.CurrentPower - (double) this.CurrentMod.PowerCons > 1.40129846432482E-45 && this.Magazine.BulletCount > 0 && Server.Instance.SolarSystem.CurrentTime - this.lastShotTime > (double) this.CurrentMod.RateOfFire)
      {
        this.CurrentPower = this.CurrentPower - this.CurrentMod.PowerCons;
        this.Magazine.ChangeQuantity(-1);
        this.lastShotTime = Server.Instance.SolarSystem.CurrentTime;
        this.DynamicObj.StatsChanged = true;
        return true;
      }
      if ((double) this.CurrentPower < 1.40129846432482E-45)
      {
        this.CurrentPower = 0.0f;
        this.DynamicObj.SendStatsToClient();
      }
      return false;
    }

    public override bool AttachChildItem(Item item)
    {
      if (!(item is Magazine))
        return false;
      this.Magazine = item as Magazine;
      return true;
    }

    public override bool RemoveChildItem(Item item)
    {
      if (item is Magazine && this.Magazine == item)
        this.Magazine = (Magazine) null;
      return true;
    }

    public override bool CanAttachChildItem(Item item)
    {
      return item is Magazine;
    }

    public void ChangeQuantity(float amount, bool overtime = true)
    {
      this.CurrentPower = !overtime ? System.Math.Min(this.CurrentPower + amount, this.maxPower) : System.Math.Min(this.CurrentPower + amount * (float) Server.Instance.DeltaTime, this.maxPower);
      if ((double) this.CurrentPower < 1.40129846432482E-45)
      {
        this.CurrentPower = 0.0f;
        this.DynamicObj.SendStatsToClient();
      }
      else if (!overtime)
        this.DynamicObj.SendStatsToClient();
      else
        this.DynamicObj.StatsChanged = true;
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataWeapon objectDataWeapon = new PersistenceObjectDataWeapon();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataWeapon);
      objectDataWeapon.WeaponData = new WeaponData();
      objectDataWeapon.WeaponData.ItemType = this.Type;
      objectDataWeapon.WeaponData.CurrentMod = this.CurrentModIndex;
      objectDataWeapon.WeaponData.CurrentPower = this.CurrentPower;
      objectDataWeapon.WeaponData.MagazineGUID = this.Magazine != null ? this.Magazine.GUID : -1L;
      objectDataWeapon.WeaponData.MaxPower = this.maxPower;
      objectDataWeapon.WeaponData.weaponMods = this.weaponMods;
      objectDataWeapon.WeaponData.Health = this.Health;
      objectDataWeapon.WeaponData.MaxHealth = this.MaxHealth;
      return (PersistenceObjectData) objectDataWeapon;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataWeapon objectDataWeapon = persistenceData as PersistenceObjectDataWeapon;
        if (objectDataWeapon == null)
          Dbg.Warning((object) "PersistenceObjectDataWeapon data is null", (object) this.GUID);
        else
          this.SetData(objectDataWeapon.WeaponData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
