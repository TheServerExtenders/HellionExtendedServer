// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.GlowStick
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  internal class GlowStick : Item
  {
    public GlowStickStats stats = new GlowStickStats();
    public bool isOn;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) this.stats;
      }
    }

    public GlowStick(IDynamicObjectAuxData data)
    {
      if (data == null)
        return;
      this.SetData(data as GlowStickData);
    }

    private void SetData(GlowStickData gsd)
    {
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      GlowStickStats glowStickStats = stats as GlowStickStats;
      this.isOn = true;
      this.DynamicObj.SendStatsToClient();
      return false;
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataGlowStick objectDataGlowStick = new PersistenceObjectDataGlowStick();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataGlowStick);
      objectDataGlowStick.GlowStickData = new GlowStickData();
      objectDataGlowStick.GlowStickData.ItemType = this.Type;
      objectDataGlowStick.GlowStickData.IsOn = this.isOn;
      return (PersistenceObjectData) objectDataGlowStick;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataGlowStick objectDataGlowStick = persistenceData as PersistenceObjectDataGlowStick;
        if (objectDataGlowStick == null)
          Dbg.Warning((object) "PersistenceObjectDataGlowStick data is null", (object) this.GUID);
        else
          this.SetData(objectDataGlowStick.GlowStickData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
