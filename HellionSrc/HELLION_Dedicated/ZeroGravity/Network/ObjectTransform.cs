// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.ObjectTransform
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using System.Collections.Generic;
using ZeroGravity.Objects;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class ObjectTransform
  {
    public long GUID;
    public SpaceObjectType Type;
    public DistressData Distress;
    public float[] Forward;
    public float[] Up;
    public float[] AngularVelocity;
    public float[] RotationVec;
    public OrbitData Orbit;
    public RealtimeData Realtime;
    public ManeuverData Maneuver;
    public List<CharacterMovementMessage> CharactersMovement;
    public List<MoveDynamicObectMessage> DynamicObjectsMovement;
    public List<MoveCorpseObectMessage> CorpsesMovement;
    public long? StabilizeToTargetGUID;
    public double[] StabilizeToTargetRelPosition;
  }
}
