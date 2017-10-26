// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.CharacterMovementMessage
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using System.Collections.Generic;
using ZeroGravity.Objects;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class CharacterMovementMessage : NetworkData
  {
    public long ParentGUID = -1;
    public SpaceObjectType ParentType = SpaceObjectType.None;
    public CharacterTransformData[] TransformData;
    public CharacterAnimationData AnimationData;
    public long GUID;
    public float[] Gravity;
    public Dictionary<byte, RagdollItemData> RagdollData;
    public sbyte[] JetpackDirection;
    public bool PivotReset;
    public long NearestVesselGUID;
    public float NearestVesselDistance;
    public bool StickToVessel;
    public float[] PivotPositionCorrection;
    public float[] PivotVelocityCorrection;
    public float? ImpactVellocity;
    public bool? DockUndockMsg;
  }
}
