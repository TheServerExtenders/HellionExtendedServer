// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.PlayerStatsMessage
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class PlayerStatsMessage : NetworkData
  {
    public long GUID;
    public bool? AnimationMaskChanged;
    public int HealthPoints;
    public int AnimationStatesMask;
    public int ReloadType;
    public float ShotDammage;
    public float HeatDammage;
    public float FrostDammage;
    public float SuffocateDammage;
    public float InpactDammage;
    public float PressureDammage;
    public float[] ShotDirection;
    public CharacterSoundData SoundData;
  }
}
