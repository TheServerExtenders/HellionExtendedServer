// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.SpaceObjectTransferable
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ZeroGravity.Math;

namespace ZeroGravity.Objects
{
  public abstract class SpaceObjectTransferable : SpaceObject
  {
    public Vector3D LocalPosition;
    public QuaternionD LocalRotation;

    public override Vector3D Position
    {
      get
      {
        return this.Parent != null ? this.Parent.Position + this.LocalPosition : this.LocalPosition;
      }
    }

    public SpaceObjectTransferable(long guid, Vector3D localPosition, QuaternionD localRotation)
      : base(guid)
    {
      this.LocalPosition = localPosition;
      this.LocalRotation = localRotation;
    }
  }
}
