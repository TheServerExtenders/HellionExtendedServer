// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Magazine
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  public class Magazine : Item
  {
    private MagazineStats _stats;
    private int _bulletCount;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) this._stats;
      }
    }

    public int BulletCount
    {
      get
      {
        return this._bulletCount;
      }
      private set
      {
        this._bulletCount = value;
        this._stats.BulletCount = new int?(value);
      }
    }

    public int MaxBulletCount { get; private set; }

    public bool HasAmmo
    {
      get
      {
        return this.BulletCount > 0;
      }
    }

    public Magazine(IDynamicObjectAuxData data)
    {
      this._stats = new MagazineStats();
      if (data == null)
        return;
      this.SetData(data as MagazineData);
    }

    private void SetData(MagazineData md)
    {
      this.BulletCount = md.BulletCount;
      this.MaxBulletCount = md.MaxBulletCount;
      this.Health = md.Health;
      this.MaxHealth = md.MaxHealth;
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      MagazineStats magazineStats = stats as MagazineStats;
      if (magazineStats.BulletsFrom.HasValue && magazineStats.BulletsTo.HasValue && (magazineStats.BulletsFrom.Value == this.GUID || magazineStats.BulletsTo.Value == this.GUID))
      {
        Magazine fromMag = magazineStats.BulletsFrom.Value == this.GUID ? this : Server.Instance.GetItem(magazineStats.BulletsFrom.Value) as Magazine;
        Magazine toMag = magazineStats.BulletsTo.Value == this.GUID ? this : Server.Instance.GetItem(magazineStats.BulletsTo.Value) as Magazine;
        if (fromMag != null && toMag != null)
          Magazine.SplitMagazines(fromMag, toMag);
      }
      return false;
    }

    private static void SplitMagazines(Magazine fromMag, Magazine toMag)
    {
      int num = toMag.MaxBulletCount - toMag.BulletCount;
      if (toMag.BulletCount == 0)
        num = fromMag.BulletCount / 2;
      else if (fromMag.BulletCount < num)
        num = fromMag.BulletCount;
      toMag.BulletCount += num;
      fromMag.BulletCount -= num;
      fromMag.DynamicObj.SendStatsToClient();
      toMag.DynamicObj.SendStatsToClient();
    }

    public void ChangeQuantity(int amount)
    {
      this.BulletCount = this.BulletCount + amount;
      if (this.BulletCount == 0)
        this.DynamicObj.SendStatsToClient();
      else
        this.DynamicObj.StatsChanged = true;
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataMagazine objectDataMagazine = new PersistenceObjectDataMagazine();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataMagazine);
      objectDataMagazine.MagazineData = new MagazineData();
      objectDataMagazine.MagazineData.ItemType = this.Type;
      objectDataMagazine.MagazineData.BulletCount = this.BulletCount;
      objectDataMagazine.MagazineData.MaxBulletCount = this.MaxBulletCount;
      objectDataMagazine.MagazineData.Health = this.Health;
      objectDataMagazine.MagazineData.MaxHealth = this.MaxHealth;
      return (PersistenceObjectData) objectDataMagazine;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataMagazine objectDataMagazine = persistenceData as PersistenceObjectDataMagazine;
        if (objectDataMagazine == null)
          Dbg.Warning((object) "PersistenceObjectDataMagazine data is null", (object) this.GUID);
        else
          this.SetData(objectDataMagazine.MagazineData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
