// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.SceneDockingPortDetails
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using System.Collections.Generic;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class SceneDockingPortDetails
  {
    public VesselObjectID ID;
    public VesselObjectID DockedToID;
    public bool DockingStatus;
    public float[] RelativePosition;
    public float[] RelativeRotation;
    public float[] CollidersCenterOffset;
    public Dictionary<long, float[]> RelativePositionUpdate;
    public Dictionary<long, float[]> RelativeRotationUpdate;
    public List<ExecuterMergeDetails> ExecutersMerge;
    public List<PairedDoorsDetails> PairedDoors;
    public bool Locked;
    public float[] CollidersCenterOffsetOther;
    public OrbitData VesselOrbit;
    public OrbitData VesselOrbitOther;
  }
}
