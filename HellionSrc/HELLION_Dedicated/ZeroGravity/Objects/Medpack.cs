// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Medpack
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Timers;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  public class Medpack : Item
  {
    public float RegenRate;
    public float MaxHp;
    private MedpackStats medStats;
    private Timer destroyTimer;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) this.medStats;
      }
    }

    public Medpack(IDynamicObjectAuxData data)
    {
      this.medStats = new MedpackStats();
      if (data == null)
        return;
      this.SetData(data as MedpackData);
    }

    private void SetData(MedpackData md)
    {
      this.RegenRate = md.RegenRate;
      this.MaxHp = md.MaxHP;
      this.Health = md.Health;
      this.MaxHealth = md.MaxHealth;
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      if (!(stats as MedpackStats).Use)
        return false;
      if (this.DynamicObj.Parent is Player)
        (this.DynamicObj.Parent as Player).Stats.HealOverTime(this.RegenRate, this.MaxHp / this.RegenRate);
      this.DynamicObj.SendStatsToClient();
      this.destroyTimer = new Timer(2500.0);
      this.destroyTimer.Elapsed += (ElapsedEventHandler) ((sender, args) => this.DestroyItem());
      this.destroyTimer.Enabled = true;
      return true;
    }

    public override void DestroyItem()
    {
      base.DestroyItem();
      if (this.destroyTimer == null)
        return;
      this.destroyTimer.Dispose();
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataMedpack objectDataMedpack = new PersistenceObjectDataMedpack();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataMedpack);
      objectDataMedpack.MedpackData = new MedpackData();
      objectDataMedpack.MedpackData.ItemType = this.Type;
      objectDataMedpack.MedpackData.MaxHP = this.MaxHp;
      objectDataMedpack.MedpackData.RegenRate = this.RegenRate;
      objectDataMedpack.MedpackData.Health = this.Health;
      objectDataMedpack.MedpackData.MaxHealth = this.MaxHealth;
      return (PersistenceObjectData) objectDataMedpack;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataMedpack objectDataMedpack = persistenceData as PersistenceObjectDataMedpack;
        if (objectDataMedpack == null)
          Dbg.Warning((object) "PersistenceObjectDataMedpack data is null", (object) this.GUID);
        else
          this.SetData(objectDataMedpack.MedpackData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
