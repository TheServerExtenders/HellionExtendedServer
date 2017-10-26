// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.CharacterSoundData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using System.Collections.Generic;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class CharacterSoundData
  {
    public Dictionary<CharacterSoundData.SoundEvents, CharacterSoundData.SoundEventParams> SoundEventData = new Dictionary<CharacterSoundData.SoundEvents, CharacterSoundData.SoundEventParams>();

    public void AddCharacterSoundData(CharacterSoundData.SoundEvents eventID, CharacterSoundData.SoundEventParams param)
    {
      if ((uint) eventID <= 0U)
        return;
      this.SoundEventData.Add(eventID, param);
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class SoundEventParams
    {
      public bool? State;
      public CharacterSoundData.RTPCParameter[] RTPCParams;
      public CharacterSoundData.SwitchCombo[] SwitchCombos;
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class SwitchCombo
    {
      public byte? SwitchGroup;
      public byte? SwitchItem;

      public SwitchCombo()
      {
      }

      public SwitchCombo(byte swGroup, byte swItem)
      {
        this.SwitchGroup = new byte?(swGroup);
        this.SwitchItem = new byte?(swItem);
      }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class RTPCParameter
    {
      public byte? ParamName;
      public float? ParamValue;

      public RTPCParameter()
      {
      }

      public RTPCParameter(byte paramName, float paramValue)
      {
        this.ParamName = new byte?(paramName);
        this.ParamValue = new float?(paramValue);
      }
    }

    public enum SoundEvents : byte
    {
      Drop = 1,
      Pickup = 2,
      SwitchStance = 3,
      JetpackToggle = 4,
      OxygenSupply = 5,
    }
  }
}
