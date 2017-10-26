// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.CharacterTransformData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class CharacterTransformData
  {
    public float[] LocalPosition;
    public float[] LocalRotation;
    public float[] Velocity;
    public float[] AngularVelocity;
    public float FreeLookX;
    public float FreeLookY;
    public float MouseLook;
    public float DeltaTime;
    public float[] PlatformRelativePos;
  }
}
