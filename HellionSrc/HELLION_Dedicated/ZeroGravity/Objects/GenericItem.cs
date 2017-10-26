// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.GenericItem
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  public class GenericItem : Item
  {
    private GenericItemSubType subType;
    private GenericItemStats stats;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) this.stats;
      }
    }

    public string Look
    {
      get
      {
        return this.stats != null ? this.stats.Look : "";
      }
      set
      {
        this.stats.Look = value;
      }
    }

    public GenericItem(IDynamicObjectAuxData data)
    {
      this.stats = new GenericItemStats();
      if (data == null)
        return;
      this.SetData(data as GenericItemData);
    }

    private void SetData(GenericItemData l)
    {
      this.stats.Health = new float?(this.Health = l.Health);
      this.subType = l.SubType;
      this.Health = l.Health;
      this.MaxHealth = l.MaxHealth;
      this.Look = l.Look;
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      return false;
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataGenericItem objectDataGenericItem = new PersistenceObjectDataGenericItem();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataGenericItem);
      objectDataGenericItem.GenericData = new GenericItemData();
      objectDataGenericItem.GenericData.ItemType = this.Type;
      objectDataGenericItem.GenericData.Health = this.Health;
      objectDataGenericItem.GenericData.MaxHealth = this.MaxHealth;
      objectDataGenericItem.GenericData.Look = this.Look;
      return (PersistenceObjectData) objectDataGenericItem;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataGenericItem objectDataGenericItem = persistenceData as PersistenceObjectDataGenericItem;
        if (objectDataGenericItem == null)
          Dbg.Warning((object) "PersistenceObjectDataGenericItem data is null", (object) this.GUID);
        else
          this.SetData(objectDataGenericItem.GenericData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
