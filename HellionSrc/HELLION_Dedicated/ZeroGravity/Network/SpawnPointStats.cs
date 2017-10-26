// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.SpawnPointStats
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using ZeroGravity.Data;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class SpawnPointStats
  {
    public int InSceneID;
    public SpawnPointType? NewType;
    public SpawnPointState? NewState;
    public bool? HackUnlock;
    public long? PlayerGUID;
    public string PlayerName;
    public string PlayerSteamID;
    public bool? PlayerInvite;
    public string InvitedPlayerSteamID;
    public string InvitedPlayerName;
  }
}
