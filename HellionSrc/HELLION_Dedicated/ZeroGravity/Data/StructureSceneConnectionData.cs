// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.StructureSceneConnectionData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ZeroGravity.ShipComponents;

namespace ZeroGravity.Data
{
  public class StructureSceneConnectionData
  {
    public int InSceneID;
    public StructureConnection.ConnectionType Type;
    public bool IsOut;
    public float[] Position;
  }
}
