// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.CharacterDetails
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using System.Collections.Generic;
using ZeroGravity.Objects;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class CharacterDetails
  {
    public long ParentID;
    public SpaceObjectType ParentType;
    public Gender Gender;
    public byte HeadType;
    public byte HairType;
    public long GUID;
    public string Name;
    public string SteamId;
    public CharacterTransformData TransformData;
    public int SpawnPointID;
    public List<DynamicObjectDetails> DynamicObjects;
    public int AnimationStatsMask;
  }
}
