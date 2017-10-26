// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.ManeuverCourse
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class ManeuverCourse
  {
    private static int numberOfCheckPoints = 100;
    private static double activationTimeDifference = 5.0;
    private static double activationDirectionDifference = 5.0;
    public static double StartingSoonTime = 2.0;
    private bool isValid = false;
    private bool isActivated = false;
    private bool isStartingSoonSent = false;
    private bool isStarted = false;
    private int currentCourseDataIndex = -1;
    private List<CourseItemData> courseItems = new List<CourseItemData>();
    private Vector3D startDir = Vector3D.Forward;
    private SpaceObjectVessel targetVessel = (SpaceObjectVessel) null;
    private double targetVesselStopDistance = 1000000.0;
    private long GUID;
    private Ship parentShip;
    private ManeuverType type;
    private double startSolarSystemTime;
    private double endSolarSystemTime;
    private double travelTime;
    private BezierD bezCurve;
    private double bezCurveScale;
    private Vector3D startPos;
    private Vector3D startVel;
    private Vector3D targetPos;
    private Vector3D targetVel;
    private double warpAcceleration;
    private bool isSameParentWarp;
    private double warpDistance;
    private double tparam;

    public long CourseGUID
    {
      get
      {
        return this.GUID;
      }
    }

    public bool IsValid
    {
      get
      {
        return this.isValid;
      }
    }

    public bool IsActivated
    {
      get
      {
        return this.isActivated;
      }
    }

    public double StartSolarSystemTime
    {
      get
      {
        return this.startSolarSystemTime;
      }
    }

    public double EndSolarSystemTime
    {
      get
      {
        return this.endSolarSystemTime;
      }
    }

    public bool IsInProgress
    {
      get
      {
        return this.isValid && this.isStarted;
      }
    }

    public bool IsStartingSoonSent
    {
      get
      {
        return this.isStartingSoonSent;
      }
    }

    public ManeuverType Type
    {
      get
      {
        return this.type;
      }
    }

    public SpaceObjectVessel TargetVessel
    {
      get
      {
        return this.targetVessel;
      }
    }

    public double TargetVesselStopDistance
    {
      get
      {
        return this.targetVesselStopDistance;
      }
    }

    public bool ReadNextManeuverCourse()
    {
      this.currentCourseDataIndex = this.currentCourseDataIndex + 1;
      if (this.courseItems != null && this.courseItems.Count > this.currentCourseDataIndex)
      {
        CourseItemData courseItem = this.courseItems[this.currentCourseDataIndex];
        this.type = courseItem.Type;
        this.isActivated = false;
        this.isStartingSoonSent = false;
        this.isStarted = false;
        if (courseItem.Type == ManeuverType.Engine)
          this.isValid = this.CheckEngineManeuverData(courseItem);
        else if (courseItem.Type == ManeuverType.Transfer)
          this.isValid = this.CheckTransferManeuverData(courseItem);
        else if (courseItem.Type == ManeuverType.Warp)
          this.isValid = this.CheckWarpManeuverData(courseItem);
        if (!this.isValid)
        {
          NetworkController networkController = Server.Instance.NetworkController;
          ManeuverCourseResponse maneuverCourseResponse = new ManeuverCourseResponse();
          int num = this.isValid ? 1 : 0;
          maneuverCourseResponse.IsValid = num != 0;
          long guid1 = this.GUID;
          maneuverCourseResponse.CourseGUID = guid1;
          long guid2 = this.parentShip.GUID;
          maneuverCourseResponse.VesselGUID = guid2;
          long skipPlalerGUID = -1;
          SpaceObject[] spaceObjectArray = new SpaceObject[1]
          {
            (SpaceObject) this.parentShip
          };
          networkController.SendToClientsSubscribedTo((NetworkData) maneuverCourseResponse, skipPlalerGUID, spaceObjectArray);
        }
        else if (courseItem.EndOrbit.VesselID.HasValue)
        {
          if (courseItem.EndOrbit.VesselType.Value == SpaceObjectType.Ship || courseItem.EndOrbit.VesselType.Value == SpaceObjectType.Asteroid)
            this.targetVessel = Server.Instance.GetVessel(courseItem.EndOrbit.VesselID.Value);
          this.targetVesselStopDistance = this.targetVessel == null || this.targetVessel.Radius <= 2.0 || this.parentShip.Radius <= 2.0 ? 1000000.0 : System.Math.Pow(this.targetVessel.Radius + this.parentShip.Radius + 100.0, 2.0);
        }
      }
      else
      {
        this.isValid = false;
        this.isActivated = false;
        this.isStartingSoonSent = false;
        this.isStarted = false;
        NetworkController networkController = Server.Instance.NetworkController;
        ManeuverCourseResponse maneuverCourseResponse = new ManeuverCourseResponse();
        maneuverCourseResponse.IsValid = false;
        bool? nullable = new bool?(true);
        maneuverCourseResponse.IsFinished = nullable;
        long guid1 = this.GUID;
        maneuverCourseResponse.CourseGUID = guid1;
        long guid2 = this.parentShip.GUID;
        maneuverCourseResponse.VesselGUID = guid2;
        long skipPlalerGUID = -1;
        SpaceObject[] spaceObjectArray = new SpaceObject[1]
        {
          (SpaceObject) this.parentShip
        };
        networkController.SendToClientsSubscribedTo((NetworkData) maneuverCourseResponse, skipPlalerGUID, spaceObjectArray);
      }
      return this.isValid;
    }

    public void ToggleActivated(bool activate)
    {
      if (activate == this.isActivated)
        return;
      if (this.StartSolarSystemTime < Server.Instance.SolarSystem.CurrentTime)
      {
        this.Invalidate();
      }
      else
      {
        if (activate && (this.StartSolarSystemTime - Server.Instance.SolarSystem.CurrentTime > ManeuverCourse.activationTimeDifference || Vector3D.Angle(this.parentShip.Forward, this.startDir) > ManeuverCourse.activationDirectionDifference))
          return;
        this.isActivated = activate;
      }
    }

    public bool FillPositionAndVelocityAtCurrentTime(ref Vector3D relativePosition, ref Vector3D relativeVelocity)
    {
      return this.FillPositionAndVelocityAtTime(Server.Instance.SolarSystem.CurrentTime, ref relativePosition, ref relativeVelocity);
    }

    private bool FillPositionAndVelocityAtTime(double solarSystemTime, ref Vector3D relativePosition, ref Vector3D relativeVelocity)
    {
      if (this.type == ManeuverType.Engine || this.type == ManeuverType.Transfer)
      {
        this.tparam = MathHelper.Clamp((solarSystemTime - this.startSolarSystemTime) / (this.endSolarSystemTime - this.startSolarSystemTime), 0.0, 1.0);
        this.bezCurve.FillDataAtPart(this.tparam, ref relativePosition, ref relativeVelocity);
        relativeVelocity = relativeVelocity / this.bezCurveScale;
        return true;
      }
      if (this.type != ManeuverType.Warp)
        return false;
      double num1 = MathHelper.Clamp(solarSystemTime - this.startSolarSystemTime, 0.0, this.endSolarSystemTime - this.startSolarSystemTime);
      if (num1 < this.travelTime / 2.0)
      {
        this.tparam = 0.5 * this.warpAcceleration * num1 * num1 / this.warpDistance;
      }
      else
      {
        double num2 = this.travelTime - num1;
        this.tparam = 1.0 - 0.5 * this.warpAcceleration * num2 * num2 / this.warpDistance;
      }
      if (this.isSameParentWarp)
      {
        relativePosition = Vector3D.Lerp(this.startPos, this.targetPos, this.tparam);
        relativeVelocity = Vector3D.Lerp(this.startVel, this.targetVel, this.tparam);
      }
      else
      {
        relativePosition = Vector3D.Lerp(this.startPos, this.targetPos, this.tparam) - this.parentShip.Orbit.Parent.Position;
        relativeVelocity = Vector3D.Lerp(this.startVel, this.targetVel, this.tparam) - this.parentShip.Orbit.Parent.Velocity;
      }
      return true;
    }

    public bool CheckTargetCollision()
    {
      if (this.targetVessel == null || this.parentShip.Position.DistanceSquared(this.targetVessel.Position) >= this.targetVesselStopDistance)
        return true;
      this.parentShip.Orbit.RelativePosition = this.targetVessel.Orbit.RelativePosition + (this.parentShip.Orbit.RelativePosition - this.targetVessel.Orbit.RelativePosition).Normalized * System.Math.Sqrt(this.targetVesselStopDistance);
      this.parentShip.Orbit.RelativeVelocity = this.targetVessel.Orbit.RelativeVelocity;
      this.Invalidate();
      return false;
    }

    public void Invalidate()
    {
      if (!this.isValid)
        return;
      this.isStarted = false;
      this.isValid = false;
      NetworkController networkController = Server.Instance.NetworkController;
      ManeuverCourseResponse maneuverCourseResponse = new ManeuverCourseResponse();
      int num = this.isValid ? 1 : 0;
      maneuverCourseResponse.IsValid = num != 0;
      long guid1 = this.GUID;
      maneuverCourseResponse.CourseGUID = guid1;
      long guid2 = this.parentShip.GUID;
      maneuverCourseResponse.VesselGUID = guid2;
      long skipPlalerGUID = -1;
      SpaceObject[] spaceObjectArray = new SpaceObject[1]
      {
        (SpaceObject) this.parentShip
      };
      networkController.SendToClientsSubscribedTo((NetworkData) maneuverCourseResponse, skipPlalerGUID, spaceObjectArray);
    }

    public void OrbitParentChanged()
    {
      if (this.type == ManeuverType.Warp)
        return;
      this.Invalidate();
    }

    private double GetTimeToOrbitStart(CourseItemData data)
    {
      OrbitParameters orbitParameters = new OrbitParameters();
      orbitParameters.ParseNetworkData(data.StartOrbit, true);
      orbitParameters.FillPositionAndVelocityAtTrueAnomaly((double) data.StartOrbitAngle * (System.Math.PI / 180.0), true, ref this.startPos, ref this.startVel);
      double num = orbitParameters.GetTimeAfterPeriapsis(this.startPos, this.startVel, true) - orbitParameters.TimeSincePeriapsis;
      if (num < 0.0)
        num += orbitParameters.OrbitalPeriod;
      return num;
    }

    private bool CheckEngineManeuverData(CourseItemData data)
    {
      OrbitParameters orbit = this.parentShip.Orbit;
      OrbitParameters orbitParameters = new OrbitParameters();
      orbitParameters.ParseNetworkData(data.EndOrbit, true);
      double timeToOrbitStart = this.GetTimeToOrbitStart(data);
      orbit.FillPositionAndVelocityAfterTime(timeToOrbitStart, true, ref this.startPos, ref this.startVel);
      orbitParameters.FillPositionAndVelocityAtTrueAnomaly((double) data.EndOrbitAngle * (System.Math.PI / 180.0), true, ref this.targetPos, ref this.targetVel);
      double timeSincePeriapsis1 = orbit.TimeSincePeriapsis;
      double timeAfterPeriapsis = orbit.GetTimeAfterPeriapsis(this.startPos, this.startVel, true);
      double timeSincePeriapsis2 = orbitParameters.TimeSincePeriapsis;
      double num1 = orbitParameters.GetTimeAfterPeriapsis(this.targetPos, this.targetVel, true) - timeSincePeriapsis2;
      if (num1 < 0.0)
        num1 += orbitParameters.OrbitalPeriod;
      double num2 = timeAfterPeriapsis - timeSincePeriapsis1;
      if (num2 < 0.0)
        num2 += orbit.OrbitalPeriod;
      this.travelTime = num1 - num2;
      if (this.travelTime < 0.0)
        return false;
      double num3 = 1.0;
      if (this.bezCurve == null)
        this.bezCurve = new BezierD(this.startPos, this.startPos + this.startVel * num3, this.targetPos - this.targetVel * num3, this.targetPos);
      else
        this.bezCurve.SetPoints(this.startPos, this.startPos + this.startVel * num3, this.targetPos - this.targetVel * num3, this.targetPos);
      Vector3D zero1 = Vector3D.Zero;
      Vector3D zero2 = Vector3D.Zero;
      Vector3D zero3 = Vector3D.Zero;
      Vector3D zero4 = Vector3D.Zero;
      this.bezCurve.FillDataAtPart(0.0, ref zero1, ref zero2);
      this.startDir = zero2.Normalized;
      Vector3D vector3D1 = zero2;
      Vector3D vector3D2 = zero1;
      double num4 = 0.0;
      Vector3D vector3D3;
      for (int index = 0; index < ManeuverCourse.numberOfCheckPoints; ++index)
      {
        this.bezCurve.FillDataAtPart((double) index / (double) (ManeuverCourse.numberOfCheckPoints - 1), ref zero1, ref zero2);
        Vector3D vector3D4 = -((vector3D2 + zero1) / 2.0);
        Vector3D vector3D5 = vector3D4.Normalized * (orbit.Parent.GravParameter / vector3D4.SqrMagnitude);
        vector3D3 = zero1 - vector3D2;
        double magnitude = vector3D3.Magnitude;
        vector3D3 = vector3D1 + zero2;
        double num5 = vector3D3.Magnitude / 2.0;
        double num6 = magnitude / num5;
        num4 += num6;
        vector3D1 = zero2;
        vector3D2 = zero1;
      }
      if (this.travelTime < num4)
        return false;
      double num7 = this.travelTime / num4;
      this.bezCurve.SetPoints(this.startPos, this.startPos + this.startVel * num7, this.targetPos - this.targetVel * num7, this.targetPos);
      this.bezCurve.FillDataAtPart(0.0, ref zero1, ref zero2);
      Vector3D vector3D6 = zero2;
      Vector3D vector3D7 = zero1;
      double engineAcceleration = (double) this.parentShip.VesselCaps.EngineAcceleration;
      double num8 = 0.0;
      for (int index = 0; index < ManeuverCourse.numberOfCheckPoints; ++index)
      {
        this.bezCurve.FillDataAtPart((double) index / (double) (ManeuverCourse.numberOfCheckPoints - 1), ref zero1, ref zero2);
        Vector3D vector3D4 = -((vector3D7 + zero1) / 2.0);
        Vector3D vector3D5 = vector3D4.Normalized * (orbit.Parent.GravParameter / vector3D4.SqrMagnitude);
        Vector3D vector3D8 = vector3D6 / num7;
        Vector3D vector3D9 = zero2 / num7;
        vector3D3 = zero1 - vector3D7;
        double magnitude = vector3D3.Magnitude;
        vector3D3 = vector3D8 + vector3D9;
        double num5 = vector3D3.Magnitude / 2.0;
        double num6 = magnitude / num5;
        num8 += num6;
        if (((vector3D9 - vector3D8) / num6 - vector3D5).Magnitude > engineAcceleration)
          return false;
        vector3D6 = zero2;
        vector3D7 = zero1;
      }
      this.bezCurveScale = num7;
      this.startSolarSystemTime = orbit.SolarSystemTimeAtPeriapsis + timeAfterPeriapsis;
      this.endSolarSystemTime = this.startSolarSystemTime + this.travelTime;
      return true;
    }

    private bool CheckTransferManeuverData(CourseItemData data)
    {
      OrbitParameters orbit = this.parentShip.Orbit;
      OrbitParameters orbitParameters = new OrbitParameters();
      orbitParameters.ParseNetworkData(data.EndOrbit, true);
      double timeToOrbitStart = this.GetTimeToOrbitStart(data);
      orbit.FillPositionAndVelocityAfterTime(timeToOrbitStart, true, ref this.startPos, ref this.startVel);
      orbitParameters.FillPositionAndVelocityAtTrueAnomaly((double) data.EndOrbitAngle * (System.Math.PI / 180.0), true, ref this.targetPos, ref this.targetVel);
      double timeSincePeriapsis = orbit.TimeSincePeriapsis;
      double timeAfterPeriapsis = orbit.GetTimeAfterPeriapsis(this.startPos, this.startVel, true);
      double num1 = timeAfterPeriapsis - timeSincePeriapsis;
      if (num1 < 0.0)
      {
        double num2 = num1 + orbit.OrbitalPeriod;
      }
      double num3 = 1.0;
      if (this.bezCurve == null)
        this.bezCurve = new BezierD(this.startPos, this.startPos + this.startVel * num3, this.targetPos - this.targetVel * num3, this.targetPos);
      else
        this.bezCurve.SetPoints(this.startPos, this.startPos + this.startVel * num3, this.targetPos - this.targetVel * num3, this.targetPos);
      Vector3D zero1 = Vector3D.Zero;
      Vector3D zero2 = Vector3D.Zero;
      Vector3D zero3 = Vector3D.Zero;
      Vector3D zero4 = Vector3D.Zero;
      this.bezCurve.FillDataAtPart(0.0, ref zero1, ref zero2);
      this.startDir = zero2.Normalized;
      Vector3D vector3D1 = zero2;
      Vector3D vector3D2 = zero1;
      double num4 = 0.0;
      Vector3D vector3D3;
      for (int index = 0; index < ManeuverCourse.numberOfCheckPoints; ++index)
      {
        this.bezCurve.FillDataAtPart((double) index / (double) (ManeuverCourse.numberOfCheckPoints - 1), ref zero1, ref zero2);
        Vector3D vector3D4 = -((vector3D2 + zero1) / 2.0);
        Vector3D vector3D5 = vector3D4.Normalized * (orbit.Parent.GravParameter / vector3D4.SqrMagnitude);
        vector3D3 = zero1 - vector3D2;
        double magnitude = vector3D3.Magnitude;
        vector3D3 = vector3D1 + zero2;
        double num5 = vector3D3.Magnitude / 2.0;
        double num6 = magnitude / num5;
        num4 += num6;
        vector3D1 = zero2;
        vector3D2 = zero1;
      }
      if (data.TravelTime < num4)
        return false;
      double num7 = data.TravelTime / num4;
      this.bezCurve.SetPoints(this.startPos, this.startPos + this.startVel * num7, this.targetPos - this.targetVel * num7, this.targetPos);
      this.bezCurve.FillDataAtPart(0.0, ref zero1, ref zero2);
      Vector3D vector3D6 = zero2;
      Vector3D vector3D7 = zero1;
      double engineAcceleration = (double) this.parentShip.VesselCaps.EngineAcceleration;
      double num8 = 0.0;
      for (int index = 0; index < ManeuverCourse.numberOfCheckPoints; ++index)
      {
        this.bezCurve.FillDataAtPart((double) index / (double) (ManeuverCourse.numberOfCheckPoints - 1), ref zero1, ref zero2);
        Vector3D vector3D4 = -((vector3D7 + zero1) / 2.0);
        Vector3D vector3D5 = vector3D4.Normalized * (orbit.Parent.GravParameter / vector3D4.SqrMagnitude);
        Vector3D vector3D8 = vector3D6 / num7;
        Vector3D vector3D9 = zero2 / num7;
        vector3D3 = zero1 - vector3D7;
        double magnitude = vector3D3.Magnitude;
        vector3D3 = vector3D8 + vector3D9;
        double num5 = vector3D3.Magnitude / 2.0;
        double num6 = magnitude / num5;
        num8 += num6;
        if (((vector3D9 - vector3D8) / num6 - vector3D5).Magnitude > engineAcceleration)
          return false;
        vector3D6 = zero2;
        vector3D7 = zero1;
      }
      this.bezCurveScale = num7;
      this.startSolarSystemTime = orbit.SolarSystemTimeAtPeriapsis + timeAfterPeriapsis;
      this.endSolarSystemTime = this.startSolarSystemTime + data.TravelTime;
      return true;
    }

    private bool CheckWarpManeuverData(CourseItemData data)
    {
      OrbitParameters orbit = this.parentShip.Orbit;
      OrbitParameters orbitParameters = new OrbitParameters();
      orbitParameters.ParseNetworkData(data.EndOrbit, true);
      double timeToOrbitStart = this.GetTimeToOrbitStart(data);
      orbit.FillPositionAndVelocityAfterTime(timeToOrbitStart, true, ref this.startPos, ref this.startVel);
      orbitParameters.FillPositionAndVelocityAtTrueAnomaly((double) data.EndOrbitAngle * (System.Math.PI / 180.0), true, ref this.targetPos, ref this.targetVel);
      double timeSincePeriapsis1 = orbit.TimeSincePeriapsis;
      double timeAfterPeriapsis1 = orbit.GetTimeAfterPeriapsis(this.startPos, this.startVel, true);
      double timeSincePeriapsis2 = orbitParameters.TimeSincePeriapsis;
      double timeAfterPeriapsis2 = orbitParameters.GetTimeAfterPeriapsis(this.targetPos, this.targetVel, true);
      double time1 = timeAfterPeriapsis1 - timeSincePeriapsis1;
      if (time1 < 0.0)
        time1 += orbit.OrbitalPeriod;
      double time2 = timeAfterPeriapsis2 - timeSincePeriapsis2;
      if (time2 < 0.0)
        time2 += orbitParameters.OrbitalPeriod;
      this.travelTime = time2 - time1;
      if (this.travelTime <= 0.0)
        return false;
      this.isSameParentWarp = orbit.Parent == orbitParameters.Parent;
      orbit.FillPositionAndVelocityAfterTime(time1, this.isSameParentWarp, ref this.startPos, ref this.startVel);
      orbitParameters.FillPositionAndVelocityAfterTime(time2, this.isSameParentWarp, ref this.targetPos, ref this.targetVel);
      this.warpDistance = (this.startPos - this.targetPos).Magnitude;
      this.warpAcceleration = 4.0 * this.warpDistance / (this.travelTime * this.travelTime);
      if (!this.CheckManeuverStartData(data, false, false))
        return false;
      this.startSolarSystemTime = orbit.SolarSystemTimeAtPeriapsis + timeAfterPeriapsis1;
      this.endSolarSystemTime = this.startSolarSystemTime + this.travelTime;
      this.startDir = (this.targetPos - this.startPos).Normalized;
      return true;
    }

    public bool StartManeuver()
    {
      if (!this.isValid)
        return false;
      if (!this.isActivated || Vector3D.Angle(this.parentShip.Forward, this.startDir) > ManeuverCourse.activationDirectionDifference || !this.CheckManeuverStartData(this.courseItems[this.currentCourseDataIndex], true, true))
      {
        this.Invalidate();
        return false;
      }
      this.isStarted = true;
      this.parentShip.DisableStabilization(true, false);
      return true;
    }

    private bool CheckManeuverStartData(CourseItemData data, bool checkSystems, bool consumeResources)
    {
      try
      {
        if (data.Type == ManeuverType.Warp)
        {
          if (this.parentShip.AllDockedVessels.Count > 1)
            return false;
          if (this.parentShip.AllDockedVessels.Count == 1)
          {
            if (this.parentShip.SceneID == GameScenes.SceneID.AltCorp_Shuttle_SARA)
            {
              VesselDockingPort vesselDockingPort = this.parentShip.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.OrderID == 1));
              if ((vesselDockingPort != null ? vesselDockingPort.DockedVessel : (SpaceObjectVessel) null) != null)
                return false;
            }
            else if (GameScenes.Ranges.IsShip(this.parentShip.AllDockedVessels[0].SceneID))
              return false;
          }
          else if (this.parentShip.DockedToMainVessel != null && this.parentShip.DockedToMainVessel.AllDockedVessels.Count > 1 || this.parentShip.DockedToMainVessel != null && this.parentShip.DockedToMainVessel.AllDockedVessels.Count == 1 && GameScenes.Ranges.IsShip(this.parentShip.DockedToMainVessel.AllDockedVessels[0].SceneID) && GameScenes.Ranges.IsShip(this.parentShip.DockedToMainVessel.SceneID))
            return false;
          WarpData warpData = this.parentShip.DistributionManager.FTL.WarpsData[data.ManeuverIndex];
          if (this.warpAcceleration > (double) warpData.MaxAcceleration || this.warpAcceleration < (double) warpData.MinAcceleration)
            return false;
          Dictionary<int, float?> warpCellsFuel = this.parentShip.DistributionManager.FTL.GetWarpCellsFuel();
          int num1 = 0;
          foreach (KeyValuePair<int, float?> keyValuePair in warpCellsFuel)
          {
            if (keyValuePair.Value.HasValue)
              ++num1;
          }
          if (num1 < data.ManeuverIndex + 1)
            return false;
          float num2 = 0.0f;
          foreach (int warpCell in data.WarpCells)
          {
            double num3 = (double) num2;
            float? nullable = warpCellsFuel[warpCell];
            double num4;
            if (!nullable.HasValue)
            {
              num4 = 0.0;
            }
            else
            {
              nullable = warpCellsFuel[warpCell];
              num4 = (double) nullable.Value;
            }
            num2 = (float) (num3 + num4);
          }
          if ((double) num2 / this.travelTime < (double) warpData.CellConsumption || checkSystems && (this.parentShip.DistributionManager.FTL.Status != SystemStatus.OnLine || (double) this.parentShip.DistributionManager.FTL.ResourceRequirements[DistributionSystemType.Power].Nominal > (double) this.parentShip.DistributionManager.Capacitor.Capacity))
            return false;
          if (consumeResources)
            this.parentShip.DistributionManager.FTL.ConsumeWarpResources(data.WarpCells, (float) (int) (this.travelTime * (double) warpData.CellConsumption));
        }
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static ManeuverCourse ParseNetworkData(ManeuverCourseRequest req, Ship sh)
    {
      ManeuverCourse maneuverCourse = new ManeuverCourse();
      maneuverCourse.parentShip = sh;
      maneuverCourse.GUID = req.CourseGUID;
      if (req.CourseItems == null || req.CourseItems.Count == 0)
        return maneuverCourse;
      maneuverCourse.courseItems = new List<CourseItemData>((IEnumerable<CourseItemData>) req.CourseItems);
      return maneuverCourse;
    }

    public ManeuverData CurrentData()
    {
      if (!this.IsValid || this.type != ManeuverType.Warp || Server.Instance.SolarSystem.CurrentTime < this.startSolarSystemTime || Server.Instance.SolarSystem.CurrentTime > this.endSolarSystemTime)
        return (ManeuverData) null;
      return new ManeuverData()
      {
        GUID = this.GUID,
        Type = this.type,
        ParentGUID = this.parentShip.Orbit.Parent.CelestialBody.GUID,
        RelPosition = this.parentShip.Orbit.RelativePosition.ToArray(),
        RelVelocity = this.parentShip.Orbit.RelativeVelocity.ToArray()
      };
    }

    public void SendCourseStartResponse()
    {
      if (!this.isValid)
        return;
      NetworkController networkController = Server.Instance.NetworkController;
      ManeuverCourseResponse maneuverCourseResponse = new ManeuverCourseResponse();
      int num = this.isValid ? 1 : 0;
      maneuverCourseResponse.IsValid = num != 0;
      long guid1 = this.GUID;
      maneuverCourseResponse.CourseGUID = guid1;
      long guid2 = this.parentShip.GUID;
      maneuverCourseResponse.VesselGUID = guid2;
      bool? nullable1 = new bool?(this.isActivated);
      maneuverCourseResponse.IsActivated = nullable1;
      double? nullable2 = new double?(this.startSolarSystemTime);
      maneuverCourseResponse.StartTime = nullable2;
      double? nullable3 = new double?(this.endSolarSystemTime);
      maneuverCourseResponse.EndTime = nullable3;
      float[] floatArray = this.startDir.ToFloatArray();
      maneuverCourseResponse.StartDirection = floatArray;
      bool? nullable4 = new bool?(this.isActivated && this.startSolarSystemTime > Server.Instance.SolarSystem.CurrentTime && Server.Instance.SolarSystem.CurrentTime >= this.startSolarSystemTime - ManeuverCourse.StartingSoonTime);
      maneuverCourseResponse.StaringSoon = nullable4;
      long skipPlalerGUID = -1;
      SpaceObject[] spaceObjectArray = new SpaceObject[1]
      {
        (SpaceObject) this.parentShip
      };
      networkController.SendToClientsSubscribedTo((NetworkData) maneuverCourseResponse, skipPlalerGUID, spaceObjectArray);
    }

    public void SendCourseStartingSoonResponse()
    {
      if (!this.isValid || this.isStartingSoonSent)
        return;
      this.isStartingSoonSent = true;
      NetworkController networkController = Server.Instance.NetworkController;
      ManeuverCourseResponse maneuverCourseResponse = new ManeuverCourseResponse();
      int num = this.isValid ? 1 : 0;
      maneuverCourseResponse.IsValid = num != 0;
      long guid1 = this.GUID;
      maneuverCourseResponse.CourseGUID = guid1;
      long guid2 = this.parentShip.GUID;
      maneuverCourseResponse.VesselGUID = guid2;
      bool? nullable1 = new bool?(this.isActivated);
      maneuverCourseResponse.IsActivated = nullable1;
      double? nullable2 = new double?(this.startSolarSystemTime);
      maneuverCourseResponse.StartTime = nullable2;
      double? nullable3 = new double?(this.endSolarSystemTime);
      maneuverCourseResponse.EndTime = nullable3;
      float[] floatArray = this.startDir.ToFloatArray();
      maneuverCourseResponse.StartDirection = floatArray;
      bool? nullable4 = new bool?(true);
      maneuverCourseResponse.StaringSoon = nullable4;
      long skipPlalerGUID = -1;
      SpaceObject[] spaceObjectArray = new SpaceObject[1]
      {
        (SpaceObject) this.parentShip
      };
      networkController.SendToClientsSubscribedTo((NetworkData) maneuverCourseResponse, skipPlalerGUID, spaceObjectArray);
    }
  }
}
