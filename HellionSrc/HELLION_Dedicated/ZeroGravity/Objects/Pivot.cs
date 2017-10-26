// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Pivot
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ZeroGravity.Math;

namespace ZeroGravity.Objects
{
  public class Pivot : ArtificialBody
  {
    public SpaceObjectTransferable Child;

    public override SpaceObjectType ObjectType
    {
      get
      {
        if (this.Child == null)
          return SpaceObjectType.None;
        if (this.Child.ObjectType == SpaceObjectType.Player)
          return SpaceObjectType.PlayerPivot;
        if (this.Child.ObjectType == SpaceObjectType.DynamicObject)
          return SpaceObjectType.DynamicObjectPivot;
        return this.Child.ObjectType == SpaceObjectType.Corpse ? SpaceObjectType.CorpsePivot : SpaceObjectType.None;
      }
    }

    public Pivot(Player child, SpaceObjectVessel vessel)
      : base(child.FakeGuid, true, vessel.Position, vessel.Velocity, Vector3D.Forward, Vector3D.Up)
    {
      this.Child = (SpaceObjectTransferable) child;
    }

    public Pivot(SpaceObjectTransferable child, ArtificialBody abody)
      : base(child.GUID, true, abody.Position, abody.Velocity, Vector3D.Forward, Vector3D.Up)
    {
      this.Child = child;
    }

    public Pivot(SpaceObjectTransferable child, Vector3D position, Vector3D velocity)
      : base(child.GUID, true, position, velocity, Vector3D.Forward, Vector3D.Up)
    {
      this.Child = child;
    }

    public void AdjustPositionAndVelocity(Vector3D positionAddition, Vector3D velocityAddition)
    {
      this.Orbit.RelativePosition += positionAddition;
      this.Orbit.RelativeVelocity += velocityAddition;
      this.Orbit.InitFromCurrentStateVectors(Server.Instance.SolarSystem.CurrentTime);
    }

    public override void Destroy()
    {
      base.Destroy();
    }
  }
}
