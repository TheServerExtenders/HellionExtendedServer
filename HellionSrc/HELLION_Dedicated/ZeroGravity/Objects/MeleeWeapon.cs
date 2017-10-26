// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.MeleeWeapon
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  internal class MeleeWeapon : Item
  {
    public float RateOfFire;
    public float Damage;
    public float Range;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) null;
      }
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      return false;
    }

    public MeleeWeapon(IDynamicObjectAuxData data)
    {
      if (data == null)
        return;
      this.SetData(data as MeleeWeaponData);
    }

    private void SetData(MeleeWeaponData mwd)
    {
      this.RateOfFire = mwd.RateOfFire;
      this.Damage = mwd.Damage;
      this.Range = mwd.Range;
      this.Health = mwd.Health;
      this.MaxHealth = mwd.MaxHealth;
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataMeleeWeapon objectDataMeleeWeapon = new PersistenceObjectDataMeleeWeapon();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataMeleeWeapon);
      objectDataMeleeWeapon.MeleeWeaponData = new MeleeWeaponData();
      objectDataMeleeWeapon.MeleeWeaponData.ItemType = this.Type;
      objectDataMeleeWeapon.MeleeWeaponData.Damage = this.Damage;
      objectDataMeleeWeapon.MeleeWeaponData.Range = this.Range;
      objectDataMeleeWeapon.MeleeWeaponData.RateOfFire = this.RateOfFire;
      objectDataMeleeWeapon.MeleeWeaponData.Health = this.Health;
      objectDataMeleeWeapon.MeleeWeaponData.MaxHealth = this.MaxHealth;
      return (PersistenceObjectData) objectDataMeleeWeapon;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataMeleeWeapon objectDataMeleeWeapon = persistenceData as PersistenceObjectDataMeleeWeapon;
        if (objectDataMeleeWeapon == null)
          Dbg.Warning((object) "PersistenceObjectDataMeleeWeapon data is null", (object) this.GUID);
        else
          this.SetData(objectDataMeleeWeapon.MeleeWeaponData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
