// Decompiled with JetBrains decompiler
// Type: ZeroGravity.PersistenceObjectDataHandheldAsteroidScanner
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ZeroGravity.Data;

namespace ZeroGravity
{
  public class PersistenceObjectDataHandheldAsteroidScanner : PersistenceObjectDataItem
  {
    public HandheldAsteroidScannerData ScannerData;

    public override Persistence.ObjectType Type
    {
      get
      {
        return Persistence.ObjectType.HandheldAsteroidScanner;
      }
    }
  }
}
