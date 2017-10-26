// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.ShipStatsMessage
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using System.Collections.Generic;
using ZeroGravity.Data;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class ShipStatsMessage : NetworkData
  {
    public long GUID;
    public float[] Thrust;
    public float[] Rotation;
    public bool? AutoStabilizeX;
    public bool? AutoStabilizeY;
    public bool? AutoStabilizeZ;
    public bool? EngineOnline;
    public float? EngineThrustPercentage;
    public VesselObjects VesselObjects;
    public RcsThrustStats ThrustStats;
    public float? Temperature;
    public float? Health;
    public long? TargetStabilizationGUID;
    public bool? GatherAtmosphere;
    public Dictionary<ResourceType, float> resourcesOnAsteroid;
    public long? AsteroidGuid;
    public short[] index;
    public short? lvl;
    public float[] debugRes;
  }
}
