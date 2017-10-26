// Decompiled with JetBrains decompiler
// Type: ZeroGravity.PersistenceObjectDataPlayer
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity
{
  public class PersistenceObjectDataPlayer : PersistenceObjectData
  {
    public long FakeGUID;
    public long ParentGUID;
    public SpaceObjectType ParentType;
    public double[] ParentPosition;
    public double[] ParentVelocity;
    public double[] LocalPosition;
    public double[] LocalRotation;
    public bool IsAlive;
    public string Name;
    public string SteamId;
    public Gender Gender;
    public byte HeadType;
    public byte HairType;
    public float HealthPoints;
    public float MaxHealthPoints;
    public CharacterAnimationData AnimationData;
    public int AnimationStatsMask;
    public float[] Gravity;
    public double[] Velocity;
    public double[] AngularVelocity;
    public int? CurrentRoomID;
    public float CoreTemperature;
    public List<PersistenceObjectData> ChildObjects;

    public override Persistence.ObjectType Type
    {
      get
      {
        return Persistence.ObjectType.Player;
      }
    }
  }
}
