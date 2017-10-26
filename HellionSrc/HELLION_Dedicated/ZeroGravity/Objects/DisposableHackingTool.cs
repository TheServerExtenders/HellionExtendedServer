// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.DisposableHackingTool
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  public class DisposableHackingTool : Item
  {
    private DisposableHackingToolStats objStats = new DisposableHackingToolStats();

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) this.objStats;
      }
    }

    public DisposableHackingTool(IDynamicObjectAuxData data)
    {
      if (data == null)
        return;
      this.SetData(data as DisposableHackingToolData);
    }

    private void SetData(DisposableHackingToolData dhtd)
    {
      this.Health = dhtd.Health;
      this.MaxHealth = dhtd.MaxHealth;
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      DisposableHackingToolStats hackingToolStats = stats as DisposableHackingToolStats;
      if (!hackingToolStats.Use)
        return false;
      this.objStats.Use = hackingToolStats.Use;
      this.DynamicObj.SendStatsToClient();
      Extensions.Invoke(new Action(this.Destroy), 0.699999988079071);
      return true;
    }

    public void Destroy()
    {
      this.SetSlot((InventorySlot) null);
      this.SetAttachPoint((AttachPointDetails) null);
      this.DynamicObj.DestroyDynamicObject();
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataHackingTool objectDataHackingTool = new PersistenceObjectDataHackingTool();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataHackingTool);
      objectDataHackingTool.HackingToolData = new DisposableHackingToolData();
      objectDataHackingTool.HackingToolData.ItemType = this.Type;
      objectDataHackingTool.HackingToolData.Health = this.Health;
      objectDataHackingTool.HackingToolData.MaxHealth = this.MaxHealth;
      return (PersistenceObjectData) objectDataHackingTool;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataHackingTool objectDataHackingTool = persistenceData as PersistenceObjectDataHackingTool;
        if (objectDataHackingTool == null)
          Dbg.Warning((object) "PersistenceObjectDataHackingTool data is null", (object) this.GUID);
        else
          this.SetData(objectDataHackingTool.HackingToolData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
