// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.CorpseStatsMessage
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using ZeroGravity.Objects;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class CorpseStatsMessage : NetworkData
  {
    public long ParentGUID = -1;
    public SpaceObjectType ParentType = SpaceObjectType.None;
    public long GUID;
    public bool DestroyCorpse;
    public float[] LocalPosition;
    public float[] LocalRotation;
  }
}
