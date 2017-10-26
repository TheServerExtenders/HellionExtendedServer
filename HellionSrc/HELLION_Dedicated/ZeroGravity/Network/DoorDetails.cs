// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.DoorDetails
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class DoorDetails
  {
    public int InSceneID;
    public bool IsLocked;
    public bool IsOpen;
    public bool HasPower;
    public bool EquilizePressure;
    public float PressureEquilizationTime;
    public int AirFlowDirection;
    public float AirSpeed;
    public VesselObjectID Room1ID;
    public VesselObjectID Room2ID;
  }
}
