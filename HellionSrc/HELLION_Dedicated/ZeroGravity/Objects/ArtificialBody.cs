// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.ArtificialBody
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.ShipComponents;

namespace ZeroGravity.Objects
{
  public class ArtificialBody : SpaceObject
  {
    private static double stabilizeToTargetMaxVelocityDiff = 2.0;
    private static double stabilizeToTargetMaxPositionDiff = 100.0;
    public OrbitParameters Orbit = new OrbitParameters();
    public ManeuverCourse CurrentCourse = (ManeuverCourse) null;
    public double LastOrientationChangeTime = 0.0;
    public Vector3D _Forward = Vector3D.Forward;
    public Vector3D _Up = Vector3D.Up;
    private bool updateAngularVelocity = true;
    public List<SpaceObjectVessel> StabilizedToTargetChildren = new List<SpaceObjectVessel>();
    private double? stabilizationDisabledTime = new double?();
    private bool? stabilizationDisableAfterUpdate = new bool?();
    private Vector3D? stabilizationDisableRelativePositionExtra = new Vector3D?();
    private Vector3D? stabilizationDisableRelativeVelocityExtra = new Vector3D?();
    public double Radius;
    public Vector3D AngularVelocity;
    public Vector3D Rotation;
    public Vector3D PhysicsVelocityDifference;
    public Vector3D PhysicsRotationDifference;
    private bool _markForDestruction;

    public bool IsDistresActive { get; protected set; }

    public double DistressActivatedTime { get; protected set; }

    public DistressType DistressType { get; protected set; }

    public override Vector3D Position
    {
      get
      {
        return this.Orbit.Position;
      }
    }

    public override Vector3D Velocity
    {
      get
      {
        return this.Orbit.Velocity;
      }
    }

    public Vector3D Forward
    {
      get
      {
        return this._Forward;
      }
      set
      {
        if (!value.IsEpsilonEqual(Vector3D.Zero, 1.00000001335143E-10))
          ;
        if (!this._Forward.IsEpsilonEqual(value, 9.99999974737875E-06))
          this.LastOrientationChangeTime = Server.Instance.SolarSystem.CurrentTime;
        this._Forward = value.IsEpsilonEqual(Vector3D.Zero, 1.00000001335143E-10) ? this._Forward : value;
      }
    }

    public Vector3D Up
    {
      get
      {
        return this._Up;
      }
      set
      {
        if (!value.IsEpsilonEqual(Vector3D.Zero, 1.00000001335143E-10))
          ;
        if (!this._Up.IsEpsilonEqual(value, 9.99999974737875E-06))
          this.LastOrientationChangeTime = Server.Instance.SolarSystem.CurrentTime;
        this._Up = value.IsEpsilonEqual(Vector3D.Zero, 1.00000001335143E-10) ? this._Up : value;
      }
    }

    public bool MarkForDestruction
    {
      get
      {
        return this._markForDestruction;
      }
      set
      {
        this._markForDestruction = value;
        Server.Instance.SolarSystem.CheckDestroyMarkedBodies = true;
      }
    }

    public double RadarSignature { get; protected set; }

    public ArtificialBody(long guid, bool initializeOrbit, Vector3D position, Vector3D velocity, Vector3D forward, Vector3D up)
      : base(guid)
    {
      if (!initializeOrbit)
        return;
      this.Orbit.SetArtificialBody(this);
      this.InitializeFromStateVectors(position, velocity);
      this.Forward = forward;
      this.Up = up;
      Server.Instance.SolarSystem.AddArtificialBody(this);
    }

    public void InitializeOrbit(Vector3D position, Vector3D velocity, Vector3D forward, Vector3D up)
    {
      this.Orbit.SetArtificialBody(this);
      this.InitializeFromStateVectors(position, velocity);
      this.Forward = forward;
      this.Up = up;
      Server.Instance.SolarSystem.AddArtificialBody(this);
    }

    public override void Destroy()
    {
      base.Destroy();
      this.DisableStabilization(true, false);
      foreach (Player allPlayer in Server.Instance.AllPlayers)
      {
        if (allPlayer.IsSubscribedTo(this.GUID))
          allPlayer.UnsubscribeFrom((SpaceObject) this);
        if (allPlayer.CurrentSpawnPoint != null && allPlayer.CurrentSpawnPoint.Ship == this)
          allPlayer.SetSpawnPoint((ShipSpawnPoint) null);
        if (allPlayer.AuthorizedSpawnPoint != null && allPlayer.AuthorizedSpawnPoint.Ship == this)
          allPlayer.ClearAuthorizedSpawnPoint();
      }
      Server.Instance.SolarSystem.RemoveArtificialBody(this);
    }

    public void InitializeFromStateVectors(Vector3D position, Vector3D velocity)
    {
      CelestialBody celestialBodyParent = Server.Instance.SolarSystem.FindCelestialBodyParent(position);
      if (velocity.SqrMagnitude < double.Epsilon)
        velocity = this.Forward * celestialBodyParent.Orbit.CircularOrbitVelocityMagnitudeAtDistance(Vector3D.Distance(celestialBodyParent.Position, position));
      this.Orbit.InitFromStateVectors(celestialBodyParent.Orbit, position, velocity, Server.Instance.SolarSystem.CurrentTime, false);
      this.Orbit.SetArtificialBody(this);
    }

    private void ApplyRotation(double deltaTime)
    {
      if (this.ObjectType != SpaceObjectType.Player && this.ObjectType != SpaceObjectType.Ship && this.ObjectType != SpaceObjectType.Asteroid)
        return;
      if (this.Rotation.Y.IsNotEpsilonZeroD(1E-05))
        this.Forward = QuaternionD.AngleAxis(this.Rotation.Y * deltaTime, this.Up) * this.Forward;
      if (this.Rotation.X.IsNotEpsilonZeroD(1E-05))
      {
        Vector3D vector3D = Vector3D.Cross(-this.Forward, this.Up);
        this.Forward = QuaternionD.AngleAxis(this.Rotation.X * deltaTime, vector3D) * this.Forward;
        this.Up = Vector3D.Cross(this.Forward, vector3D);
      }
      if (this.Rotation.Z.IsNotEpsilonZeroD(1E-05))
        this.Up = QuaternionD.AngleAxis(this.Rotation.Z * deltaTime, this.Forward) * this.Up;
    }

    private void UpdatePosition(double timeDelta)
    {
      if (this is Ship && (this as Ship).IsDocked)
        return;
      if (this.stabilizationDisabledTime.HasValue)
      {
        if (this.stabilizationDisabledTime.Value.IsEpsilonEqualD(Server.Instance.SolarSystem.CurrentTime, double.Epsilon))
        {
          this.stabilizationDisabledTime = new double?();
          return;
        }
        this.stabilizationDisabledTime = new double?();
      }
      if (this.CheckCurrentCourse())
        this.Orbit.InitFromCurrentStateVectors(Server.Instance.SolarSystem.CurrentTime);
      else
        this.Orbit.UpdateOrbit(timeDelta);
      if (this.CheckThrustAndRotation(timeDelta))
      {
        if (this.CurrentCourse != null)
          this.CurrentCourse.Invalidate();
        this.Orbit.InitFromCurrentStateVectors(Server.Instance.SolarSystem.CurrentTime);
      }
      if (this is SpaceObjectVessel)
        (this as SpaceObjectVessel).SetPhysicsParameters();
      if (this.CheckGravityInfluenceRadius())
      {
        if (this.CurrentCourse != null)
          this.CurrentCourse.OrbitParentChanged();
        this.Orbit.InitFromCurrentStateVectors(Server.Instance.SolarSystem.CurrentTime);
      }
      if (!this.CheckPlanetDeath())
        return;
      this.MarkForDestruction = true;
    }

    public void UpdateStabilization(double timeDelta)
    {
      if (this.StabilizeToTargetObj == null || !(this is Ship) || (this as Ship).IsDocked)
        return;
      if (this.CurrentCourse != null)
        this.CurrentCourse.Invalidate();
      this.Orbit.CopyDataFrom(this.StabilizeToTargetObj.Orbit, Server.Instance.SolarSystem.CurrentTime, true);
      this.Orbit.RelativePosition += this.StabilizeToTargetRelPosition;
      if (this.stabilizationDisableRelativePositionExtra.HasValue)
      {
        this.Orbit.RelativePosition += this.stabilizationDisableRelativePositionExtra.Value;
        this.stabilizationDisableRelativePositionExtra = new Vector3D?();
      }
      if (this.stabilizationDisableRelativeVelocityExtra.HasValue)
      {
        this.Orbit.RelativeVelocity += this.stabilizationDisableRelativeVelocityExtra.Value;
        this.stabilizationDisableRelativeVelocityExtra = new Vector3D?();
      }
      this.Orbit.InitFromCurrentStateVectors(Server.Instance.SolarSystem.CurrentTime);
      if (this is SpaceObjectVessel)
        (this as SpaceObjectVessel).SetPhysicsParameters();
      if (this.StabilizedToTargetChildren.Count <= 0)
        return;
      foreach (ArtificialBody stabilizedToTargetChild in this.StabilizedToTargetChildren)
        stabilizedToTargetChild.UpdateStabilization(timeDelta);
    }

    private bool CheckCurrentCourse()
    {
      if (this.CurrentCourse == null || !this.CurrentCourse.IsValid)
        return false;
      if (this.CurrentCourse.IsActivated && !this.CurrentCourse.IsStartingSoonSent && this.CurrentCourse.StartSolarSystemTime > Server.Instance.SolarSystem.CurrentTime && Server.Instance.SolarSystem.CurrentTime >= this.CurrentCourse.StartSolarSystemTime - ManeuverCourse.StartingSoonTime)
        this.CurrentCourse.SendCourseStartingSoonResponse();
      else if (this.CurrentCourse.StartSolarSystemTime <= Server.Instance.SolarSystem.CurrentTime)
      {
        if (!this.CurrentCourse.IsActivated)
        {
          this.CurrentCourse.Invalidate();
          return false;
        }
        if (!this.CurrentCourse.IsInProgress)
        {
          if (!this.CurrentCourse.StartManeuver())
            return false;
          if (!this.CurrentCourse.IsStartingSoonSent)
            this.CurrentCourse.SendCourseStartingSoonResponse();
        }
        this.CurrentCourse.FillPositionAndVelocityAtCurrentTime(ref this.Orbit.RelativePosition, ref this.Orbit.RelativeVelocity);
        if (this.CurrentCourse.Type == ManeuverType.Engine || this.CurrentCourse.Type == ManeuverType.Transfer)
        {
          Vector3D lhs = Vector3D.Cross(this.Forward, this.Up);
          this.Forward = Vector3D.Lerp(this.Forward, this.Orbit.RelativeVelocity.Normalized, Server.Instance.DeltaTime).Normalized;
          this.Up = Vector3D.Cross(lhs, this.Forward);
        }
        if (!this.CurrentCourse.CheckTargetCollision() || this.CurrentCourse.EndSolarSystemTime > Server.Instance.SolarSystem.CurrentTime)
          return true;
        this.CurrentCourse.ReadNextManeuverCourse();
        return true;
      }
      return false;
    }

    private void ApplyThrust(double timeDelta, ref Vector3D thrust)
    {
      if ((this.StabilizeToTargetObj != null || this.StabilizedToTargetChildren.Count > 0) && thrust.IsNotEpsilonZero(0.001))
      {
        if (this.StabilizeToTargetObj != null)
        {
          this.DisableStabilizationAfterUpdate(new Vector3D?(thrust * timeDelta), new Vector3D?(thrust));
        }
        else
        {
          foreach (ArtificialBody stabilizedToTargetChild in this.StabilizedToTargetChildren)
            stabilizedToTargetChild.DisableStabilizationAfterUpdate(new Vector3D?(-thrust * timeDelta), new Vector3D?(-thrust));
        }
      }
      this.Orbit.RelativePosition += thrust * timeDelta;
      this.Orbit.RelativeVelocity += thrust;
      thrust = Vector3D.Zero;
    }

    private bool CheckThrustAndRotation(double timeDelta)
    {
      Vector3D rotation = this.Rotation;
      bool flag = false;
      if (this is Ship)
      {
        Ship ship = this as Ship;
        if (ship.CalculateEngineThrust(timeDelta))
        {
          flag = true;
          this.ApplyThrust(timeDelta, ref ship.EngineThrustVelocityDifference);
        }
        if (ship.CalculateRcsThrust(timeDelta))
        {
          flag = true;
          this.ApplyThrust(timeDelta, ref ship.RcsThrustVelocityDifference);
        }
        if (this.PhysicsVelocityDifference.IsNotEpsilonZero(0.001))
        {
          flag = true;
          this.ApplyThrust(timeDelta, ref this.PhysicsVelocityDifference);
        }
        if (ship.CalculateRotationThrust(timeDelta))
        {
          this.updateAngularVelocity = true;
          this.Rotation = this.Rotation + ship.RotationThrustVelocityDifference;
          ship.RotationThrustVelocityDifference = Vector3D.Zero;
        }
        if (ship.CalculateRotationDampen(timeDelta))
          this.updateAngularVelocity = true;
        else if (ship.CalculateAutoStabilizeRotation(timeDelta))
          this.updateAngularVelocity = true;
        if (this.PhysicsRotationDifference.IsNotEpsilonZero(0.001))
        {
          this.updateAngularVelocity = true;
          this.Rotation = this.Rotation + this.PhysicsRotationDifference;
          this.PhysicsRotationDifference = Vector3D.Zero;
        }
        if (this.updateAngularVelocity)
        {
          QuaternionD quaternionD = QuaternionD.LookRotation(this.Forward, this.Up);
          if (this.Rotation.IsNotEpsilonZero(double.Epsilon))
            this.ApplyRotation(timeDelta);
          this.AngularVelocity = (QuaternionD.LookRotation(this.Forward, this.Up) * quaternionD.Inverse()).EulerAngles / timeDelta * (System.Math.PI / 180.0);
          this.updateAngularVelocity = false;
        }
        else if (this.Rotation.IsNotEpsilonZero(double.Epsilon))
          this.ApplyRotation(timeDelta);
        ship.CheckThrustStatsMessage();
      }
      else if (this is Asteroid)
      {
        if (this.PhysicsVelocityDifference.IsNotEpsilonZero(0.001))
        {
          flag = true;
          this.ApplyThrust(timeDelta, ref this.PhysicsVelocityDifference);
        }
        if (this.PhysicsRotationDifference.IsNotEpsilonZero(0.001))
        {
          this.updateAngularVelocity = true;
          this.Rotation = this.Rotation + this.PhysicsRotationDifference;
          this.PhysicsRotationDifference = Vector3D.Zero;
        }
        if (this.updateAngularVelocity)
        {
          QuaternionD quaternionD = QuaternionD.LookRotation(this.Forward, this.Up);
          if (this.Rotation.IsNotEpsilonZero(double.Epsilon))
            this.ApplyRotation(timeDelta);
          this.AngularVelocity = (QuaternionD.LookRotation(this.Forward, this.Up) * quaternionD.Inverse()).EulerAngles / timeDelta * (System.Math.PI / 180.0);
          this.updateAngularVelocity = false;
        }
        else if (this.Rotation.IsNotEpsilonZero(double.Epsilon))
          this.ApplyRotation(timeDelta);
      }
      return flag;
    }

    private bool CheckGravityInfluenceRadius()
    {
      if (!double.IsInfinity(this.Orbit.Parent.GravityInfluenceRadiusSquared) && this.Orbit.RelativePosition.SqrMagnitude > this.Orbit.Parent.GravityInfluenceRadiusSquared)
      {
        this.Orbit.ChangeOrbitParent(this.Orbit.Parent.Parent);
        return true;
      }
      if (this.Orbit.Parent.CelestialBody.ChildBodies.Count > 0)
      {
        foreach (CelestialBody childBody in this.Orbit.Parent.CelestialBody.ChildBodies)
        {
          if (this.Orbit.Position.DistanceSquared(childBody.Position) < childBody.Orbit.GravityInfluenceRadiusSquared)
          {
            this.Orbit.ChangeOrbitParent(childBody.Orbit);
            return true;
          }
        }
      }
      return false;
    }

    private bool CheckPlanetDeath()
    {
      return this is Ship && this.Orbit.RelativePosition.SqrMagnitude < System.Math.Pow(this.Orbit.Parent.Radius + Server.CelestialBodyDeathDistance, 2.0);
    }

    public void Update(double timeDelta)
    {
      this.UpdatePosition(timeDelta);
    }

    public void AfterUpdate(double timeDelta)
    {
      if (this.stabilizationDisableAfterUpdate.HasValue && this.stabilizationDisableAfterUpdate.Value)
      {
        this.DisableStabilization(true, true);
      }
      else
      {
        if (this.StabilizeToTargetObj == null)
          return;
        this.UpdateStabilization(timeDelta);
      }
    }

    public SpaceObjectVessel StabilizeToTargetObj { get; private set; }

    public Vector3D StabilizeToTargetRelPosition { get; protected set; }

    public double StabilizeToTargetTime { get; private set; }

    public bool StabilizeToTarget(SpaceObjectVessel target, bool forceStabilize = false)
    {
      if (!(this is SpaceObjectVessel) || target == null || target.StabilizeToTargetObj != null)
        return false;
      if (target != null && target.IsDocked)
        target = target.DockedToMainVessel;
      if (target == this || this is SpaceObjectVessel && (this as SpaceObjectVessel).IsDocked)
        return false;
      if (!forceStabilize)
      {
        Vector3D vector3D = target.Velocity - this.Velocity;
        int num;
        if (vector3D.Magnitude <= ArtificialBody.stabilizeToTargetMaxVelocityDiff)
        {
          vector3D = target.Position - this.Position;
          num = vector3D.Magnitude - target.Radius > ArtificialBody.stabilizeToTargetMaxPositionDiff ? 1 : 0;
        }
        else
          num = 1;
        if (num != 0 || target.StabilizedToTargetChildren.Contains(this as SpaceObjectVessel))
          return false;
      }
      this.StabilizeToTargetObj = target;
      this.StabilizeToTargetRelPosition = this.Position - target.Position;
      target.StabilizedToTargetChildren.Add(this as SpaceObjectVessel);
      this.StabilizeToTargetTime = Server.Instance.SolarSystem.CurrentTime;
      return true;
    }

    public void DisableStabilization(bool disableForChildren, bool updateBeforeDisable)
    {
      if (!(this is SpaceObjectVessel))
        return;
      this.stabilizationDisableAfterUpdate = new bool?();
      if (this.StabilizeToTargetObj != null)
      {
        if (updateBeforeDisable)
          this.UpdateStabilization(0.0);
        this.StabilizeToTargetObj.StabilizedToTargetChildren.Remove(this as SpaceObjectVessel);
        this.StabilizeToTargetObj = (SpaceObjectVessel) null;
        this.stabilizationDisabledTime = new double?(Server.Instance.SolarSystem.CurrentTime);
      }
      if (!disableForChildren || this.StabilizedToTargetChildren.Count <= 0)
        return;
      int num;
      for (num = 0; this.StabilizedToTargetChildren.Count > 0 && num < 1000; ++num)
        this.StabilizedToTargetChildren[0].DisableStabilization(false, updateBeforeDisable);
      if (num >= 1000)
        Dbg.Error((object) "When disabling stabilization for", (object) this.GUID, (object) "children, sanity check reached", (object) num);
    }

    public void DisableStabilizationAfterUpdate(Vector3D? relativePositionExtra, Vector3D? relativeVelocityExtra)
    {
      if (this.StabilizeToTargetObj == null)
        return;
      this.stabilizationDisableAfterUpdate = new bool?(true);
      this.stabilizationDisableRelativePositionExtra = relativePositionExtra;
      this.stabilizationDisableRelativePositionExtra = relativeVelocityExtra;
    }

    public void ToggleDistress(bool isOn, DistressType type = DistressType.Distress)
    {
      this.IsDistresActive = isOn;
      this.DistressType = type;
      this.DistressActivatedTime = Server.Instance.SolarSystem.CurrentTime;
    }
  }
}
