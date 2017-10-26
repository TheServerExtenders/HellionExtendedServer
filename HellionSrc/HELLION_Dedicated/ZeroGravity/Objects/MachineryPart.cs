// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.MachineryPart
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  public class MachineryPart : Item, IPersistantObject
  {
    public int Tier = 0;
    public float BoostFactor = 1f;
    public float WearMultiplier = 1f;
    public MachineryPartType PartType;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        MachineryPartStats machineryPartStats = new MachineryPartStats();
        float? nullable = new float?(this.Health);
        machineryPartStats.Health = nullable;
        return (DynamicObjectStats) machineryPartStats;
      }
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      return false;
    }

    public MachineryPart(IDynamicObjectAuxData data)
    {
      if (data == null)
        return;
      MachineryPartData machineryPartData = data as MachineryPartData;
      this.PartType = machineryPartData.PartType;
      this.Tier = machineryPartData.Tier;
      this.MaxHealth = machineryPartData.MaxHealth;
      this.Health = machineryPartData.Health;
      this.MaxHealth = machineryPartData.MaxHealth;
      this.Armor = machineryPartData.Armor;
      this.BoostFactor = machineryPartData.BoostFactor;
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataMachineryPart dataMachineryPart = new PersistenceObjectDataMachineryPart();
      this.FillPersistenceData((PersistenceObjectDataItem) dataMachineryPart);
      dataMachineryPart.PartData = new MachineryPartData();
      dataMachineryPart.PartData.PartType = this.PartType;
      dataMachineryPart.PartData.Tier = this.Tier;
      dataMachineryPart.PartData.MaxHealth = this.MaxHealth;
      dataMachineryPart.PartData.Health = this.Health;
      dataMachineryPart.PartData.MaxHealth = this.MaxHealth;
      dataMachineryPart.PartData.Armor = this.Armor;
      dataMachineryPart.PartData.BoostFactor = this.BoostFactor;
      return (PersistenceObjectData) dataMachineryPart;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataMachineryPart dataMachineryPart = persistenceData as PersistenceObjectDataMachineryPart;
        if (dataMachineryPart == null)
        {
          Dbg.Warning((object) "PersistenceObjectDataMachineryPart data is null", (object) this.GUID);
        }
        else
        {
          this.PartType = dataMachineryPart.PartData.PartType;
          this.Tier = dataMachineryPart.PartData.Tier;
          this.MaxHealth = dataMachineryPart.PartData.MaxHealth;
          this.Health = dataMachineryPart.PartData.Health;
          this.Armor = dataMachineryPart.PartData.Armor;
          this.BoostFactor = dataMachineryPart.PartData.BoostFactor;
        }
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
