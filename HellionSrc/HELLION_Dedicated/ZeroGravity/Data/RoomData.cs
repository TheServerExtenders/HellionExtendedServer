// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.RoomData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

namespace ZeroGravity.Data
{
  public class RoomData : ISceneData
  {
    public int InSceneID;
    public bool UseGravity;
    public bool GravityAutoToggle;
    public float Volume;
    public float AirQuality;
    public float AirPressure;
    public int ParentRoomID;
  }
}
