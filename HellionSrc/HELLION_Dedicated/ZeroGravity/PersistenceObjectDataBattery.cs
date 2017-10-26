// Decompiled with JetBrains decompiler
// Type: ZeroGravity.PersistenceObjectDataBattery
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ZeroGravity.Data;

namespace ZeroGravity
{
  public class PersistenceObjectDataBattery : PersistenceObjectDataItem
  {
    public BatteryData BatteryData;

    public override Persistence.ObjectType Type
    {
      get
      {
        return Persistence.ObjectType.Battery;
      }
    }
  }
}
