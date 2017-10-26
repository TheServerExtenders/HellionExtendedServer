// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.PlayerRespawnResponse
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using System.Collections.Generic;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class PlayerRespawnResponse : NetworkData
  {
    public long GUID;
    public int SpawnPointID;
    public List<CorpseDetails> Corpses;
  }
}
