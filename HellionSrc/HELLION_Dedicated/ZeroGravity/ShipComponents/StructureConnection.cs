// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.StructureConnection
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

namespace ZeroGravity.ShipComponents
{
  public class StructureConnection
  {
    public StructureConnection.ConnectionType Type;
    public bool IsFemale;

    public enum ConnectionType
    {
      AltCorp_Connection1 = 100,
      ScienceCorp_Connection1 = 200,
      ScienceCorp_Connection2 = 201,
    }
  }
}
