// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.LogItem
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  internal class LogItem : Item
  {
    private int logId;
    private LogItemStats lis;

    public int LogID
    {
      get
      {
        return this.logId;
      }
      set
      {
        this.logId = value;
        this.lis.LogID = value;
      }
    }

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) this.lis;
      }
    }

    public LogItem(IDynamicObjectAuxData data)
    {
      this.lis = new LogItemStats();
      if (data == null)
        return;
      this.SetData(data as LogItemData);
    }

    private void SetData(LogItemData l)
    {
      int num = l.logID;
      this.Health = l.Health;
      this.MaxHealth = l.MaxHealth;
      if (num == -1)
        num = MathHelper.RandomRange(0, Enum.GetValues(typeof (LogItemTypes)).Length);
      this.LogID = num;
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      return false;
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataLogItem objectDataLogItem = new PersistenceObjectDataLogItem();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataLogItem);
      objectDataLogItem.LogItemData = new LogItemData();
      objectDataLogItem.LogItemData.ItemType = this.Type;
      objectDataLogItem.LogItemData.logID = this.logId;
      objectDataLogItem.LogItemData.Health = this.Health;
      objectDataLogItem.LogItemData.MaxHealth = this.MaxHealth;
      return (PersistenceObjectData) objectDataLogItem;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataLogItem objectDataLogItem = persistenceData as PersistenceObjectDataLogItem;
        if (objectDataLogItem == null)
          Dbg.Warning((object) "PersistenceObjectDataLogItem data is null", (object) this.GUID);
        else
          this.SetData(objectDataLogItem.LogItemData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
