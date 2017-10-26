// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.ShipSaveData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ZeroGravity.Objects;

namespace ZeroGravity.Data
{
  public class ShipSaveData
  {
    public long ParentGUID = -1;
    public long GUID;
    public double[] Position;
    public float[] Forward;
    public float[] Up;
    public SpaceObjectType ParentType;
  }
}
