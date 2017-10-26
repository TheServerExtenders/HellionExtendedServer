// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.CharacterAnimationData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using ZeroGravity.Math;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class CharacterAnimationData
  {
    public byte VelocityForward;
    public byte VelocityRight;
    public byte ZeroGForward;
    public byte ZeroGRight;
    public byte InteractType;
    public byte PlayerStance;
    public byte TurningDirection;
    public byte EquipOrDeEquip;
    public byte EquipItemId;
    public byte EmoteType;
    public byte ReloadItemType;
    public byte MeleeAttackType;
    public sbyte LadderDirection;
    public byte PlayerStanceFloat;
    public byte GetUpType;
    public byte FireMode;
    public float AirTime;

    public CharacterAnimationData()
    {
    }

    public CharacterAnimationData(float velocityForward, float velocityRight, float zeroGForward, float zeroGRight, float interactType, int playerStance, int turningDirection, float equipOrDeEquip, float equipItemId, float emotetype, float reloadItemType, float meleeAttackType, float ladderDirection, float playerStanceFloat, float getUpType, float fireMode, float airTime)
    {
      this.VelocityForward = (byte) MathHelper.ProportionalValue(velocityForward, -1f, 1f, 0.0f, (float) byte.MaxValue);
      this.VelocityRight = (byte) MathHelper.ProportionalValue(velocityRight, -1f, 1f, 0.0f, (float) byte.MaxValue);
      this.ZeroGForward = (byte) MathHelper.ProportionalValue(zeroGForward, -1f, 1f, 0.0f, (float) byte.MaxValue);
      this.ZeroGRight = (byte) MathHelper.ProportionalValue(zeroGRight, -1f, 1f, 0.0f, (float) byte.MaxValue);
      this.InteractType = (byte) interactType;
      this.PlayerStance = (byte) playerStance;
      this.TurningDirection = (byte) turningDirection;
      this.EquipOrDeEquip = (byte) equipOrDeEquip;
      this.EquipItemId = (byte) equipItemId;
      this.EmoteType = (byte) emotetype;
      this.ReloadItemType = (byte) reloadItemType;
      this.MeleeAttackType = (byte) meleeAttackType;
      this.LadderDirection = (sbyte) ladderDirection;
      this.PlayerStanceFloat = (byte) playerStanceFloat;
      this.GetUpType = (byte) getUpType;
      this.FireMode = (byte) fireMode;
      this.AirTime = airTime;
    }
  }
}
