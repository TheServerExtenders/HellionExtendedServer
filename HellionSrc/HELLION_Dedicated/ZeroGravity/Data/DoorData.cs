// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.DoorData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

namespace ZeroGravity.Data
{
  public class DoorData : ISceneData
  {
    public int InSceneID;
    public int Room1ID;
    public int Room2ID;
    public float PassageArea;
    public bool IsSealable;
    public bool HasPower;
    public bool IsLocked;
    public bool IsOpen;
    public bool LockedAutoToggle;
    public float[] PositionRelativeToDockingPort;
  }
}
