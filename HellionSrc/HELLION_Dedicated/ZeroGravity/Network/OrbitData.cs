// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.OrbitData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using ZeroGravity.Objects;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class OrbitData
  {
    public long ParentGUID;
    public long? VesselID;
    public SpaceObjectType? VesselType;
    public double Eccentricity;
    public double SemiMajorAxis;
    public double LongitudeOfAscendingNode;
    public double ArgumentOfPeriapsis;
    public double Inclination;
    public double TimeSincePeriapsis;
    public double SolarSystemPeriapsisTime;
  }
}
