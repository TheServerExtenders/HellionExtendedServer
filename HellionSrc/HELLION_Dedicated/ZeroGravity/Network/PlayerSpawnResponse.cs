// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.PlayerSpawnResponse
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using System.Collections.Generic;
using ZeroGravity.Objects;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class PlayerSpawnResponse : NetworkData
  {
    public ResponseResult Response = ResponseResult.Success;
    public long ParentID;
    public SpaceObjectType ParentType;
    public ObjectTransform ParentTransform;
    public long MainVesselID;
    public VesselData VesselData;
    public VesselCaps VesselCaps;
    public VesselObjects VesselObjects;
    public List<DockedVesselsData> DockedVessels;
    public int Health;
    public List<DynamicObjectDetails> DynamicObjects;
    public int SpawnPointID;
    public CharacterTransformData CharacterTransform;
    public long? HomeGUID;
  }
}
