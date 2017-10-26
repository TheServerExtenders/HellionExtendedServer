// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.RoomDetails
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class RoomDetails
  {
    public int InSceneID;
    public bool UseGravity;
    public float AirQuality;
    public float AirPressure;
    public float Temperature;
    public short CompoundRoomID;
    public float AirInTank;
    public bool Fire;
    public bool Breach;
    public bool GravityMalfunction;
  }
}
