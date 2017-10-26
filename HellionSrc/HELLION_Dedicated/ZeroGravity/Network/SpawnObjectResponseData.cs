// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.SpawnObjectResponseData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using ZeroGravity.Objects;

namespace ZeroGravity.Network
{
  [ProtoInclude(4000, typeof (SpawnShipResponseData))]
  [ProtoInclude(4001, typeof (SpawnAsteroidResponseData))]
  [ProtoInclude(4002, typeof (SpawnDynamicObjectResponseData))]
  [ProtoInclude(4003, typeof (SpawnCorpseResponseData))]
  [ProtoInclude(4004, typeof (SpawnCharacterResponseData))]
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public abstract class SpawnObjectResponseData
  {
    public long GUID;

    public virtual SpaceObjectType Type
    {
      get
      {
        return SpaceObjectType.None;
      }
      set
      {
      }
    }
  }
}
