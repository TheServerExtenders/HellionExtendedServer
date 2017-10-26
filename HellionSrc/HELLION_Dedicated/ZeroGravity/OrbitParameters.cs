// Decompiled with JetBrains decompiler
// Type: ZeroGravity.OrbitParameters
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity
{
  public class OrbitParameters
  {
    private double mass = 0.0;
    private double radius = 0.0;
    private double rotationPeriod = 86400.0;
    private double gravParameter = 0.0;
    private CelestialBody celestialBodyObj = (CelestialBody) null;
    private ArtificialBody artificialBodyObj = (ArtificialBody) null;
    private double eccentricity = 0.0;
    private double semiMajorAxis = 0.0;
    private double semiMinorAxis = 0.0;
    public Vector3D RelativePosition = Vector3D.Zero;
    public Vector3D RelativeVelocity = Vector3D.Zero;
    private double lastValidTrueAnomaly = 0.0;
    private double lastValidTimeSincePeriapsis = 0.0;
    public const double AstronomicalUnitLength = 149597870700.0;
    public const double GravitationalConstant = 6.67384E-11;
    public const double MaxObjectDistance = 897587224200.0;
    private double gravityInfluenceRadius;
    private double gravityInfluenceRadiusSquared;
    private OrbitParameters parent;
    private double inclination;
    private double argumentOfPeriapsis;
    private double longitudeOfAscendingNode;
    private double orbitalPeriod;
    private double timeSincePeriapsis;
    private double solarSystemTimeAtPeriapsis;
    private double lastChangeTime;

    public Vector3D Position
    {
      get
      {
        return this.parent != null ? this.parent.Position + this.RelativePosition : this.RelativePosition;
      }
    }

    public Vector3D Velocity
    {
      get
      {
        return this.parent != null ? this.parent.Velocity + this.RelativeVelocity : this.RelativeVelocity;
      }
    }

    public double OrbitalPeriod
    {
      get
      {
        return this.orbitalPeriod;
      }
    }

    public double Radius
    {
      get
      {
        return this.radius;
      }
    }

    public double GravParameter
    {
      get
      {
        return this.gravParameter;
      }
    }

    public double GravityInfluenceRadius
    {
      get
      {
        return this.gravityInfluenceRadius;
      }
    }

    public double GravityInfluenceRadiusSquared
    {
      get
      {
        return this.gravityInfluenceRadiusSquared;
      }
    }

    public double LastChangeTime
    {
      get
      {
        return this.lastChangeTime;
      }
    }

    public OrbitParameters Parent
    {
      get
      {
        return this.parent;
      }
    }

    public bool IsOrbitValid
    {
      get
      {
        return this.semiMajorAxis != 0.0 && this.semiMinorAxis != 0.0;
      }
    }

    public double TimeSincePeriapsis
    {
      get
      {
        return this.timeSincePeriapsis;
      }
    }

    public double SolarSystemTimeAtPeriapsis
    {
      get
      {
        return this.solarSystemTimeAtPeriapsis;
      }
    }

    public double Eccentricity
    {
      get
      {
        return this.eccentricity;
      }
    }

    public CelestialBody CelestialBody
    {
      get
      {
        return this.celestialBodyObj;
      }
    }

    public ArtificialBody ArtificialBody
    {
      get
      {
        return this.artificialBodyObj;
      }
    }

    public double LongitudeOfAscendingNode
    {
      get
      {
        return this.longitudeOfAscendingNode;
      }
    }

    public double ArgumentOfPeriapsis
    {
      get
      {
        return this.argumentOfPeriapsis;
      }
    }

    public double Inclination
    {
      get
      {
        return this.inclination;
      }
    }

    public double PeriapsisDistance
    {
      get
      {
        return OrbitParameters.CalculatePeriapsisDistance(this);
      }
    }

    public double ApoapsisDistance
    {
      get
      {
        return OrbitParameters.CalculateApoapsisDistance(this);
      }
    }

    public double SemiMajorAxis
    {
      get
      {
        return this.semiMajorAxis;
      }
    }

    public double SemiMinorAxis
    {
      get
      {
        return this.semiMinorAxis;
      }
    }

    public double Circumference
    {
      get
      {
        return OrbitParameters.CalculateCircumference(this);
      }
    }

    public long GUID
    {
      get
      {
        if (this.artificialBodyObj != null)
          return this.artificialBodyObj.GUID;
        if (this.celestialBodyObj != null)
          return this.celestialBodyObj.GUID;
        return 0;
      }
    }

    public void InitFromElements(OrbitParameters parent, double mass, double radius, double rotationPeriod, double eccentricity, double semiMajorAxis, double inclination, double argumentOfPeriapsis, double longitudeOfAscendingNode, double timeSincePeriapsis, double solarSystemTime)
    {
      OrbitParameters.FixEccentricity(ref eccentricity);
      this.parent = parent;
      if (mass > 0.0)
        this.mass = mass;
      if (radius > 0.0)
        this.radius = radius;
      if (rotationPeriod > 0.0)
        this.rotationPeriod = rotationPeriod;
      this.eccentricity = eccentricity;
      this.semiMajorAxis = semiMajorAxis;
      this.inclination = inclination;
      this.argumentOfPeriapsis = argumentOfPeriapsis;
      this.longitudeOfAscendingNode = longitudeOfAscendingNode;
      this.timeSincePeriapsis = timeSincePeriapsis;
      this.solarSystemTimeAtPeriapsis = solarSystemTime - timeSincePeriapsis;
      this.lastChangeTime = solarSystemTime;
      this.gravParameter = 6.67384E-11 * this.mass;
      if (parent != null)
      {
        this.orbitalPeriod = eccentricity >= 1.0 ? 2.0 * System.Math.PI * System.Math.Sqrt(System.Math.Pow(-semiMajorAxis, 3.0) / parent.gravParameter) : 2.0 * System.Math.PI * System.Math.Sqrt(System.Math.Pow(semiMajorAxis, 3.0) / parent.gravParameter);
        while (this.timeSincePeriapsis < -this.orbitalPeriod)
          this.timeSincePeriapsis = this.timeSincePeriapsis + this.orbitalPeriod;
        this.semiMinorAxis = eccentricity >= 1.0 ? semiMajorAxis * System.Math.Sqrt(eccentricity * eccentricity - 1.0) : semiMajorAxis * System.Math.Sqrt(1.0 - eccentricity * eccentricity);
        double trueAnomaly = OrbitParameters.CalculateTrueAnomaly(this, timeSincePeriapsis);
        this.RelativePosition = this.PositionAtTrueAnomaly(trueAnomaly, true);
        this.RelativeVelocity = this.VelocityAtTrueAnomaly(trueAnomaly, true);
        this.gravityInfluenceRadius = semiMajorAxis * (1.0 - eccentricity) * System.Math.Pow(mass / (3.0 * parent.mass), 1.0 / 3.0);
        this.gravityInfluenceRadiusSquared = this.gravityInfluenceRadius * this.gravityInfluenceRadius;
      }
      else
      {
        this.RelativePosition = Vector3D.Zero;
        this.RelativeVelocity = Vector3D.Zero;
        this.orbitalPeriod = 0.0;
        this.timeSincePeriapsis = 0.0;
        this.semiMajorAxis = 0.0;
        this.semiMinorAxis = 0.0;
        this.gravityInfluenceRadius = double.PositiveInfinity;
        this.gravityInfluenceRadiusSquared = double.PositiveInfinity;
      }
    }

    public void InitFromPeriapsis(OrbitParameters parent, double mass, double radius, double rotationPeriod, double eccentricity, double periapsisDistance, double inclination, double argumentOfPeriapsis, double longitudeOfAscendingNode, double timeSincePeriapsis, double solarSystemTime)
    {
      OrbitParameters.FixEccentricity(ref eccentricity);
      double semiMajorAxis = periapsisDistance / (1.0 - eccentricity);
      this.InitFromElements(parent, mass, radius, rotationPeriod, eccentricity, semiMajorAxis, inclination, argumentOfPeriapsis, longitudeOfAscendingNode, timeSincePeriapsis, solarSystemTime);
    }

    public void InitFromPeriapisAndApoapsis(OrbitParameters parent, double periapsisDistance, double apoapsisDistance, double inclination, double argumentOfPeriapsis, double longitudeOfAscendingNode, double trueAnomalyAngleDeg, double solarSystemTime)
    {
      this.parent = parent;
      this.inclination = inclination;
      this.argumentOfPeriapsis = argumentOfPeriapsis;
      this.longitudeOfAscendingNode = longitudeOfAscendingNode;
      this.semiMajorAxis = (periapsisDistance + apoapsisDistance) / 2.0;
      this.eccentricity = (apoapsisDistance - periapsisDistance) / (apoapsisDistance + periapsisDistance);
      this.orbitalPeriod = this.eccentricity >= 1.0 ? 2.0 * System.Math.PI * System.Math.Sqrt(System.Math.Pow(-this.semiMajorAxis, 3.0) / parent.gravParameter) : 2.0 * System.Math.PI * System.Math.Sqrt(System.Math.Pow(this.semiMajorAxis, 3.0) / parent.gravParameter);
      this.timeSincePeriapsis = OrbitParameters.CalculateTimeSincePeriapsis(this, OrbitParameters.CalculateMeanAnomalyFromTrueAnomaly(this, trueAnomalyAngleDeg * (System.Math.PI / 180.0)));
      this.InitFromElements(parent, 0.0, 0.0, 0.0, this.eccentricity, this.semiMajorAxis, inclination, argumentOfPeriapsis, longitudeOfAscendingNode, this.timeSincePeriapsis, solarSystemTime);
    }

    public void InitFromStateVectors(OrbitParameters parent, Vector3D position, Vector3D velocity, double solarSystemTime, bool areValuesRelative)
    {
      if (parent == null)
        throw new Exception("Parent object cannot be null only sun has no parent.");
      if (parent.gravParameter == 0.0)
        throw new Exception("Parent object grav parameter is not set.");
      this.parent = parent;
      this.RelativePosition = position;
      this.RelativeVelocity = velocity;
      if (!areValuesRelative)
      {
        this.RelativePosition = this.RelativePosition - parent.Position;
        this.RelativeVelocity = this.RelativeVelocity - parent.Velocity;
      }
      double magnitude1 = this.RelativePosition.Magnitude;
      Vector3D rhs1 = Vector3D.Cross(this.RelativePosition, this.RelativeVelocity);
      Vector3D right = Vector3D.Right;
      Vector3D lhs;
      if (rhs1.SqrMagnitude.IsEpsilonEqualD(0.0, double.Epsilon))
      {
        this.inclination = 180.0 - System.Math.Acos(this.RelativePosition.Y / magnitude1) * (180.0 / System.Math.PI);
        lhs = Vector3D.Cross(this.RelativePosition, Vector3D.Up);
        if (lhs.SqrMagnitude.IsEpsilonEqualD(0.0, double.Epsilon))
          lhs = Vector3D.Right;
      }
      else
      {
        this.inclination = 180.0 - System.Math.Acos(rhs1.Y / rhs1.Magnitude) * (180.0 / System.Math.PI);
        lhs = Vector3D.Cross(Vector3D.Up, rhs1);
      }
      double magnitude2 = lhs.Magnitude;
      Vector3D rhs2 = Vector3D.Cross(this.RelativeVelocity, rhs1) / parent.gravParameter - this.RelativePosition / magnitude1;
      this.eccentricity = rhs2.Magnitude;
      OrbitParameters.FixEccentricity(ref this.eccentricity);
      double num1 = this.RelativeVelocity.SqrMagnitude / 2.0 - parent.gravParameter / magnitude1;
      if (this.eccentricity < 1.0)
      {
        this.semiMajorAxis = -parent.gravParameter / (2.0 * num1);
        this.semiMinorAxis = this.semiMajorAxis * System.Math.Sqrt(1.0 - this.eccentricity * this.eccentricity);
      }
      else
      {
        this.semiMajorAxis = -(rhs1.SqrMagnitude / parent.gravParameter) / (this.eccentricity * this.eccentricity - 1.0);
        this.semiMinorAxis = this.semiMajorAxis * System.Math.Sqrt(this.eccentricity * this.eccentricity - 1.0);
      }
      if (magnitude2.IsEpsilonEqualD(0.0, double.Epsilon))
      {
        this.longitudeOfAscendingNode = 0.0;
        double num2 = OrbitParameters.CalculateTrueAnomaly(this, this.RelativePosition, this.RelativeVelocity) * (180.0 / System.Math.PI);
        double num3 = -MathHelper.AngleSigned(Vector3D.Right, this.RelativePosition, Vector3D.Up);
        if (num3 < 0.0)
          num3 += 360.0;
        this.argumentOfPeriapsis = num3 - num2;
      }
      else
      {
        this.longitudeOfAscendingNode = 180.0 - System.Math.Acos(lhs.X / magnitude2) * (180.0 / System.Math.PI);
        if (lhs.Z > 0.0)
          this.longitudeOfAscendingNode = 360.0 - this.longitudeOfAscendingNode;
        this.argumentOfPeriapsis = !this.eccentricity.IsEpsilonEqualD(0.0, 1E-10) ? 180.0 - System.Math.Acos(MathHelper.Clamp(Vector3D.Dot(lhs, rhs2) / (magnitude2 * this.eccentricity), -1.0, 1.0)) * (180.0 / System.Math.PI) : 0.0;
        if (rhs2.Y > 0.0 && !this.argumentOfPeriapsis.IsEpsilonEqualD(0.0, double.Epsilon))
          this.argumentOfPeriapsis = 360.0 - this.argumentOfPeriapsis;
      }
      this.orbitalPeriod = this.eccentricity >= 1.0 ? 2.0 * System.Math.PI * System.Math.Sqrt(System.Math.Pow(-this.semiMajorAxis, 3.0) / parent.gravParameter) : 2.0 * System.Math.PI * System.Math.Sqrt(System.Math.Pow(this.semiMajorAxis, 3.0) / parent.gravParameter);
      this.timeSincePeriapsis = OrbitParameters.CalculateTimeSincePeriapsis(this, this.RelativePosition, this.RelativeVelocity);
      this.solarSystemTimeAtPeriapsis = solarSystemTime - this.timeSincePeriapsis;
      this.lastChangeTime = solarSystemTime;
    }

    public void InitFromCurrentStateVectors(double solarSystemTime)
    {
      this.InitFromStateVectors(this.parent, this.RelativePosition, this.RelativeVelocity, solarSystemTime, true);
    }

    public void SetCelestialBody(CelestialBody body)
    {
      this.celestialBodyObj = body;
    }

    public void SetArtificialBody(ArtificialBody body)
    {
      this.artificialBodyObj = body;
    }

    private static void FixEccentricity(ref double eccentricity)
    {
      if (eccentricity != 1.0)
        return;
      eccentricity = eccentricity + 1E-11;
    }

    private static double CalculateTrueAnomaly(OrbitParameters o, double timeSincePeriapsis)
    {
      double num1;
      if (o.eccentricity < 1.0)
      {
        double meanAnomaly = timeSincePeriapsis / o.orbitalPeriod * 2.0 * System.Math.PI;
        double eccentricAnomaly = OrbitParameters.CalculateEccentricAnomaly(o, meanAnomaly, 1E-06, 50.0, 10.0);
        num1 = System.Math.Acos((System.Math.Cos(eccentricAnomaly) - o.eccentricity) / (1.0 - o.eccentricity * System.Math.Cos(eccentricAnomaly)));
        if (timeSincePeriapsis > o.orbitalPeriod / 2.0)
          num1 = 2.0 * System.Math.PI - num1;
      }
      else
      {
        double num2 = 2.0 * System.Math.PI * System.Math.Abs(timeSincePeriapsis) / o.orbitalPeriod;
        if (timeSincePeriapsis < 0.0)
          num2 *= -1.0;
        double eccentricAnomaly = OrbitParameters.CalculateEccentricAnomaly(o, System.Math.Abs(num2), 1E-06, 50.0, 10.0);
        num1 = System.Math.Atan2(System.Math.Sqrt(o.eccentricity * o.eccentricity - 1.0) * System.Math.Sinh(eccentricAnomaly), o.eccentricity - System.Math.Cosh(eccentricAnomaly));
        if (timeSincePeriapsis < 0.0)
          num1 = 2.0 * System.Math.PI - num1;
      }
      return num1;
    }

    private static double CalculateTrueAnomaly(OrbitParameters o, Vector3D position, Vector3D velocity)
    {
      if (o.eccentricity.IsEpsilonEqualD(0.0, 1E-10))
      {
        double num = MathHelper.AngleSigned(QuaternionD.AngleAxis(-o.longitudeOfAscendingNode, Vector3D.Up) * Vector3D.Right, position, Vector3D.Cross(position, velocity).Normalized);
        if (num < 0.0)
          num += 360.0;
        return num * (System.Math.PI / 180.0);
      }
      double d = System.Math.Acos(MathHelper.Clamp(Vector3D.Dot(Vector3D.Cross(velocity, Vector3D.Cross(position, velocity)) / o.parent.gravParameter - position / position.Magnitude, position) / (o.eccentricity * position.Magnitude), -1.0, 1.0));
      if (Vector3D.Dot(position, velocity) < 0.0)
        d = 2.0 * System.Math.PI - d;
      if (double.IsNaN(d))
        d = System.Math.PI;
      return d;
    }

    private static double CalculateTrueAnomalyFromEccentricAnomaly(OrbitParameters o, double eccentricAnomaly)
    {
      double num;
      if (o.eccentricity < 1.0)
      {
        num = System.Math.Acos((System.Math.Cos(eccentricAnomaly) - o.eccentricity) / (1.0 - o.eccentricity * System.Math.Cos(eccentricAnomaly)));
        if (eccentricAnomaly > System.Math.PI)
          num = 2.0 * System.Math.PI - num;
      }
      else
      {
        num = System.Math.Atan2(System.Math.Sqrt(o.eccentricity * o.eccentricity - 1.0) * System.Math.Sinh(eccentricAnomaly), o.eccentricity - System.Math.Cosh(eccentricAnomaly));
        if (eccentricAnomaly < 0.0)
          num = 2.0 * System.Math.PI - num;
      }
      return num;
    }

    private static double CalculateEccentricAnomaly(OrbitParameters o, double meanAnomaly, double maxDeltaDiff = 1E-06, double maxCalculations = 50.0, double maxCalculationsExtremeEcc = 10.0)
    {
      if (o.eccentricity < 1.0)
      {
        if (o.eccentricity < 0.9)
        {
          double num1 = 1.0;
          double num2 = meanAnomaly + o.eccentricity * System.Math.Sin(meanAnomaly) + 0.5 * o.eccentricity * o.eccentricity * System.Math.Sin(2.0 * meanAnomaly);
          for (int index = 0; System.Math.Abs(num1) > maxDeltaDiff && (double) index < maxCalculations; ++index)
          {
            num1 = (meanAnomaly - (num2 - o.eccentricity * System.Math.Sin(num2))) / (1.0 - o.eccentricity * System.Math.Cos(num2));
            num2 += num1;
          }
          return num2;
        }
        double num3 = meanAnomaly + 0.85 * o.eccentricity * (double) System.Math.Sign(System.Math.Sin(meanAnomaly));
        for (int index = 0; (double) index < maxCalculationsExtremeEcc; ++index)
        {
          double num1 = o.eccentricity * System.Math.Sin(num3);
          double num2 = num3 - num1 - meanAnomaly;
          double num4 = 1.0 - o.eccentricity * System.Math.Cos(num3);
          num3 += -5.0 * num2 / (num4 + (double) System.Math.Sign(num4) * System.Math.Sqrt(System.Math.Abs(16.0 * num4 * num4 - 20.0 * num2 * num1)));
        }
        return num3;
      }
      if (double.IsInfinity(meanAnomaly))
        return meanAnomaly;
      double num5 = 1.0;
      double num6 = System.Math.Log(2.0 * meanAnomaly / o.eccentricity + 1.8);
      for (int index = 0; System.Math.Abs(num5) > maxDeltaDiff && (double) index < maxCalculations; ++index)
      {
        num5 = (o.eccentricity * System.Math.Sinh(num6) - num6 - meanAnomaly) / (o.eccentricity * System.Math.Cosh(num6) - 1.0);
        num6 -= num5;
      }
      return num6;
    }

    private static double CalculateEccentricAnomalyFromTrueAnomaly(OrbitParameters o, double trueAnomaly)
    {
      double num1 = System.Math.Cos(trueAnomaly);
      double num2;
      if (o.eccentricity < 1.0)
      {
        num2 = System.Math.Acos((o.eccentricity + num1) / (1.0 + o.eccentricity * num1));
        if (trueAnomaly > System.Math.PI)
          num2 = 2.0 * System.Math.PI - num2;
      }
      else
        num2 = System.Math.Abs(o.eccentricity * num1 + 1.0) < 1E-05 ? (trueAnomaly < System.Math.PI ? double.PositiveInfinity : double.NegativeInfinity) : (o.eccentricity * num1 < -1.0 ? double.NaN : MathHelper.Acosh((o.eccentricity + num1) / (1.0 + o.eccentricity * num1)));
      return num2;
    }

    private static double CalculateMeanAnomalyFromTrueAnomaly(OrbitParameters o, double trueAnomaly)
    {
      double anomalyFromTrueAnomaly = OrbitParameters.CalculateEccentricAnomalyFromTrueAnomaly(o, trueAnomaly);
      return OrbitParameters.CalculateMeanAnomaly(o, trueAnomaly, anomalyFromTrueAnomaly);
    }

    private static double CalculateMeanAnomaly(OrbitParameters o, double trueAnomaly, double eccentricAnomaly)
    {
      double num = eccentricAnomaly;
      if (o.eccentricity < 1.0)
        num = eccentricAnomaly - o.eccentricity * System.Math.Sin(eccentricAnomaly);
      else if (!double.IsInfinity(eccentricAnomaly))
        num = (o.eccentricity * System.Math.Sinh(eccentricAnomaly) - eccentricAnomaly) * (trueAnomaly >= System.Math.PI ? -1.0 : 1.0);
      return num;
    }

    private static double CalculateDistanceAtTrueAnomaly(OrbitParameters o, double trueAnomaly)
    {
      if (o.eccentricity < 1.0)
        return o.semiMajorAxis * (1.0 - o.eccentricity * o.eccentricity) / (1.0 + o.eccentricity * System.Math.Cos(trueAnomaly));
      return -o.semiMajorAxis * (o.eccentricity * o.eccentricity - 1.0) / (1.0 + o.eccentricity * System.Math.Cos(trueAnomaly));
    }

    private static double CalculateTimeSincePeriapsis(OrbitParameters o, double meanAnomaly)
    {
      if (o.eccentricity < 1.0)
        return meanAnomaly / (2.0 * System.Math.PI) * o.orbitalPeriod;
      return System.Math.Sqrt(System.Math.Pow(-o.semiMajorAxis, 3.0) / o.parent.gravParameter) * meanAnomaly;
    }

    private static double CalculateTimeSincePeriapsis(OrbitParameters o, Vector3D relPosition, Vector3D relVelocity)
    {
      double trueAnomaly = OrbitParameters.CalculateTrueAnomaly(o, relPosition, relVelocity);
      double anomalyFromTrueAnomaly = OrbitParameters.CalculateEccentricAnomalyFromTrueAnomaly(o, trueAnomaly);
      double meanAnomaly = OrbitParameters.CalculateMeanAnomaly(o, trueAnomaly, anomalyFromTrueAnomaly);
      return OrbitParameters.CalculateTimeSincePeriapsis(o, meanAnomaly);
    }

    private static double CalculatePeriapsisDistance(OrbitParameters o)
    {
      return o.semiMajorAxis * (1.0 - o.eccentricity);
    }

    private static double CalculateApoapsisDistance(OrbitParameters o)
    {
      return o.semiMajorAxis * (1.0 + o.eccentricity);
    }

    private static double CalculateCircumference(OrbitParameters o)
    {
      if (o.eccentricity.IsEpsilonEqualD(0.0, double.Epsilon))
        return 2.0 * o.semiMajorAxis * System.Math.PI;
      return System.Math.PI * (3.0 * (o.semiMajorAxis + o.semiMinorAxis) - System.Math.Sqrt((3.0 * o.semiMajorAxis + o.semiMinorAxis) * (o.semiMajorAxis + 3.0 * o.semiMinorAxis)));
    }

    public Vector3D PositionAtTrueAnomaly(double angleRad, bool getRelativePosition)
    {
      double distanceAtTrueAnomaly = OrbitParameters.CalculateDistanceAtTrueAnomaly(this, angleRad);
      Vector3D axis1 = QuaternionD.AngleAxis(-this.longitudeOfAscendingNode, Vector3D.Up) * Vector3D.Right;
      Vector3D axis2 = QuaternionD.AngleAxis(this.inclination, axis1) * Vector3D.Up;
      Vector3D vector3D = QuaternionD.AngleAxis(-this.argumentOfPeriapsis - angleRad * (180.0 / System.Math.PI), axis2) * axis1 * distanceAtTrueAnomaly;
      if (getRelativePosition)
        return vector3D;
      return vector3D + this.parent.Position;
    }

    public Vector3D PositionAtTimeAfterPeriapsis(double timeAfterPeriapsis, bool getRelativePosition)
    {
      return this.PositionAtTrueAnomaly(OrbitParameters.CalculateTrueAnomaly(this, timeAfterPeriapsis), getRelativePosition);
    }

    public Vector3D PositionAfterTime(double time, bool getRelativePosition)
    {
      if (!this.IsOrbitValid)
        return Vector3D.Zero;
      double trueAnomaly = OrbitParameters.CalculateTrueAnomaly(this, this.timeSincePeriapsis + time);
      if (getRelativePosition)
        return this.PositionAtTrueAnomaly(trueAnomaly, true);
      return this.parent.PositionAfterTime(time, false) + this.PositionAtTrueAnomaly(trueAnomaly, true);
    }

    public Vector3D PositionAtEccentricAnomaly(double angleRad, bool getRelativePosition)
    {
      return this.PositionAtTrueAnomaly(OrbitParameters.CalculateTrueAnomalyFromEccentricAnomaly(this, angleRad), getRelativePosition);
    }

    public Vector3D VelocityAtTrueAnomaly(double trueAnomaly, bool getRelativeVelocity)
    {
      double num1 = System.Math.Cos(trueAnomaly);
      double num2 = System.Math.Sin(trueAnomaly);
      double num3 = System.Math.Sqrt(this.parent.gravParameter / (this.semiMajorAxis * (1.0 - this.eccentricity * this.eccentricity)));
      Vector3D axis1 = QuaternionD.AngleAxis(-this.longitudeOfAscendingNode, Vector3D.Up) * Vector3D.Right;
      Vector3D axis2 = QuaternionD.AngleAxis(this.inclination, axis1) * Vector3D.Up;
      Vector3D vector3D1 = QuaternionD.AngleAxis(-this.argumentOfPeriapsis, axis2) * axis1;
      Vector3D vector3D2 = QuaternionD.AngleAxis(-this.argumentOfPeriapsis - 90.0, axis2) * axis1;
      Vector3D vector3D3 = vector3D1 * (-num2 * num3) + vector3D2 * ((this.eccentricity + num1) * num3);
      if (getRelativeVelocity)
        return vector3D3;
      return vector3D3 + this.parent.Velocity;
    }

    public Vector3D VelocityAtTimeAfterPeriapsis(double timeAfterPeriapsis, bool getRelativeVelocity)
    {
      return this.VelocityAtTrueAnomaly(OrbitParameters.CalculateTrueAnomaly(this, timeAfterPeriapsis), getRelativeVelocity);
    }

    public Vector3D VelocityAfterTime(double time, bool getRelativeVelocity)
    {
      double trueAnomaly = OrbitParameters.CalculateTrueAnomaly(this, this.timeSincePeriapsis + time);
      if (!this.IsOrbitValid)
        return Vector3D.Zero;
      if (getRelativeVelocity)
        return this.VelocityAtTrueAnomaly(trueAnomaly, getRelativeVelocity);
      return this.parent.VelocityAfterTime(time, false) + this.VelocityAtTrueAnomaly(trueAnomaly, true);
    }

    public Vector3D VelocityAtEccentricAnomaly(double angleRad, bool getRelativePosition)
    {
      return this.VelocityAtTrueAnomaly(OrbitParameters.CalculateTrueAnomalyFromEccentricAnomaly(this, angleRad), getRelativePosition);
    }

    public void FillPositionAndVelocityAtTrueAnomaly(double angleRad, bool fillRelativeData, ref Vector3D position, ref Vector3D velocity)
    {
      position = this.PositionAtTrueAnomaly(angleRad, fillRelativeData);
      velocity = this.VelocityAtTrueAnomaly(angleRad, fillRelativeData);
    }

    public void FillPositionAndVelocityAfterTime(double time, bool fillRelativeData, ref Vector3D position, ref Vector3D velocity)
    {
      position = this.PositionAfterTime(time, fillRelativeData);
      velocity = this.VelocityAfterTime(time, fillRelativeData);
    }

    public double GetRotationAngle(double solarSystemTime)
    {
      if (this.rotationPeriod == 0.0)
        return 0.0;
      return 360.0 * (solarSystemTime % this.rotationPeriod / this.rotationPeriod);
    }

    public void UpdateOrbit(double timeDelta)
    {
      if (this.parent == null)
        return;
      if (this.IsOrbitValid)
      {
        this.timeSincePeriapsis = this.timeSincePeriapsis + timeDelta;
        if (this.eccentricity < 1.0 && this.timeSincePeriapsis > this.orbitalPeriod)
        {
          this.solarSystemTimeAtPeriapsis = this.solarSystemTimeAtPeriapsis + this.orbitalPeriod;
          this.timeSincePeriapsis = this.timeSincePeriapsis % this.orbitalPeriod;
        }
        if (this.eccentricity < 1.0)
        {
          double trueAnomaly = OrbitParameters.CalculateTrueAnomaly(this, this.timeSincePeriapsis);
          this.RelativePosition = this.PositionAtTrueAnomaly(trueAnomaly, true);
          this.RelativeVelocity = this.VelocityAtTrueAnomaly(trueAnomaly, true);
        }
        else
        {
          double trueAnomaly = OrbitParameters.CalculateTrueAnomaly(this, this.timeSincePeriapsis);
          this.RelativePosition = this.PositionAtTrueAnomaly(trueAnomaly, true);
          this.RelativeVelocity = this.VelocityAtTrueAnomaly(trueAnomaly, true);
          if (this.RelativePosition.IsInfinity() || this.RelativePosition.IsNaN())
          {
            Vector3D vector3D1 = this.PositionAtTrueAnomaly(this.lastValidTrueAnomaly, true);
            Vector3D vector3D2 = this.VelocityAtTrueAnomaly(this.lastValidTrueAnomaly, true);
            this.RelativePosition = vector3D1 + vector3D2 * (this.timeSincePeriapsis - this.lastValidTimeSincePeriapsis);
            this.RelativeVelocity = vector3D2;
          }
          else
          {
            this.lastValidTrueAnomaly = trueAnomaly;
            this.lastValidTimeSincePeriapsis = this.timeSincePeriapsis;
          }
        }
      }
      else
      {
        this.RelativeVelocity = this.RelativeVelocity + (-this.RelativePosition).Normalized * (this.parent.gravParameter / this.RelativePosition.SqrMagnitude) * timeDelta;
        this.RelativePosition = this.RelativePosition + this.Velocity * timeDelta;
      }
    }

    public void ResetOrbit(double solarSystemTime)
    {
      if (!this.IsOrbitValid)
        return;
      if (this.eccentricity < 1.0)
      {
        this.timeSincePeriapsis = solarSystemTime - this.solarSystemTimeAtPeriapsis;
        while (this.timeSincePeriapsis < -this.orbitalPeriod)
          this.timeSincePeriapsis = this.timeSincePeriapsis + this.orbitalPeriod;
        if (this.timeSincePeriapsis > this.orbitalPeriod)
        {
          this.timeSincePeriapsis = this.timeSincePeriapsis % this.orbitalPeriod;
          this.solarSystemTimeAtPeriapsis = solarSystemTime - this.timeSincePeriapsis;
        }
      }
      else
        this.timeSincePeriapsis = solarSystemTime - this.solarSystemTimeAtPeriapsis;
      double trueAnomaly = OrbitParameters.CalculateTrueAnomaly(this, this.timeSincePeriapsis);
      this.RelativePosition = this.PositionAtTrueAnomaly(trueAnomaly, true);
      this.RelativeVelocity = this.VelocityAtTrueAnomaly(trueAnomaly, true);
      this.lastChangeTime = solarSystemTime;
    }

    public List<Vector3D> GetOrbitPositions(int numberOfPositions, double timeStep)
    {
      if (!this.IsOrbitValid)
        return new List<Vector3D>();
      List<Vector3D> vector3DList = new List<Vector3D>();
      if (this.eccentricity < 1.0)
      {
        double num = 2.0 * System.Math.PI / (double) numberOfPositions;
        for (int index = 0; index < numberOfPositions; ++index)
          vector3DList.Add(this.PositionAtEccentricAnomaly((double) index * num, true));
      }
      else
      {
        for (int index = 0; index < numberOfPositions; ++index)
          vector3DList.Add(this.PositionAtTimeAfterPeriapsis(this.timeSincePeriapsis + (double) index * timeStep, true));
      }
      return vector3DList;
    }

    public List<Vector3D> GetOrbitVelocities(int numberOfPositions, bool getRelativeVelocities, double timeStep)
    {
      if (!this.IsOrbitValid)
        return new List<Vector3D>();
      List<Vector3D> vector3DList = new List<Vector3D>();
      if (this.eccentricity < 1.0)
      {
        double num = 2.0 * System.Math.PI / (double) numberOfPositions;
        for (int index = 0; index < numberOfPositions; ++index)
          vector3DList.Add(this.VelocityAtEccentricAnomaly((double) index * num, getRelativeVelocities));
      }
      else
      {
        for (int index = 0; index < numberOfPositions; ++index)
          vector3DList.Add(this.VelocityAtTimeAfterPeriapsis(this.timeSincePeriapsis + (double) index * timeStep, getRelativeVelocities));
      }
      return vector3DList;
    }

    public double GetTimeAfterPeriapsis(Vector3D position, Vector3D velocity, bool areValuesRelative)
    {
      if (!areValuesRelative)
      {
        position -= this.parent.Position;
        velocity -= this.parent.Velocity;
      }
      return OrbitParameters.CalculateTimeSincePeriapsis(this, position, velocity);
    }

    public void ChangeOrbitParent(OrbitParameters newParent)
    {
      this.RelativePosition = this.Position - newParent.Position;
      this.RelativeVelocity = this.Velocity - newParent.Velocity;
      this.parent = newParent;
    }

    public void GetOrbitPlaneData(out QuaternionD rotation, out Vector3D centerPosition)
    {
      Vector3D axis = QuaternionD.AngleAxis(-this.longitudeOfAscendingNode, Vector3D.Up) * Vector3D.Right;
      Vector3D normalized = (QuaternionD.AngleAxis(-this.argumentOfPeriapsis, QuaternionD.AngleAxis(this.inclination, axis) * Vector3D.Up) * axis).Normalized;
      rotation = QuaternionD.LookRotation(normalized, Vector3D.Cross(-this.RelativePosition, this.RelativeVelocity).Normalized);
      centerPosition = normalized * (OrbitParameters.CalculatePeriapsisDistance(this) - this.semiMajorAxis);
    }

    public double TrueAnomalyAtZeroTime()
    {
      return OrbitParameters.CalculateTrueAnomaly(this, this.orbitalPeriod - this.solarSystemTimeAtPeriapsis % this.orbitalPeriod);
    }

    public double TrueAnomalyAtZeroTimeFromCurrent(double extraTime)
    {
      return OrbitParameters.CalculateTrueAnomaly(this, this.timeSincePeriapsis + extraTime);
    }

    public void FillOrbitData(ref OrbitData data, SpaceObjectVessel targetVessel = null)
    {
      data.ParentGUID = this.parent.celestialBodyObj.GUID;
      if (targetVessel != null)
      {
        data.VesselID = new long?(targetVessel.GUID);
        if (targetVessel is Ship)
          data.VesselType = new SpaceObjectType?(SpaceObjectType.Ship);
        else if (targetVessel is Asteroid)
          data.VesselType = new SpaceObjectType?(SpaceObjectType.Asteroid);
      }
      data.Eccentricity = this.eccentricity;
      data.SemiMajorAxis = this.semiMajorAxis;
      data.Inclination = this.inclination;
      data.ArgumentOfPeriapsis = this.argumentOfPeriapsis;
      data.LongitudeOfAscendingNode = this.longitudeOfAscendingNode;
      data.TimeSincePeriapsis = this.timeSincePeriapsis;
      data.SolarSystemPeriapsisTime = this.solarSystemTimeAtPeriapsis;
    }

    private void CheckParent(long parentGUID)
    {
      if (this.parent != null && this.parent.celestialBodyObj != null && this.parent.celestialBodyObj.GUID == parentGUID)
        return;
      this.parent = Server.Instance.SolarSystem.GetCelestialBody(parentGUID).Orbit;
    }

    public void ParseNetworkData(OrbitData data, bool resetOrbit = false)
    {
      this.CheckParent(data.ParentGUID);
      this.solarSystemTimeAtPeriapsis = data.SolarSystemPeriapsisTime;
      double currentTime = Server.Instance.SolarSystem.CurrentTime;
      this.timeSincePeriapsis = currentTime - this.solarSystemTimeAtPeriapsis;
      this.InitFromElements(this.parent, -1.0, -1.0, -1.0, data.Eccentricity, data.SemiMajorAxis, data.Inclination, data.ArgumentOfPeriapsis, data.LongitudeOfAscendingNode, this.timeSincePeriapsis, currentTime);
      if (!resetOrbit)
        return;
      this.ResetOrbit(currentTime);
    }

    public void ParseNetworkData(RealtimeData data)
    {
      this.CheckParent(data.ParentGUID);
      this.RelativePosition = data.Position.ToVector3D();
      this.RelativeVelocity = data.Velocity.ToVector3D();
    }

    public void ParseNetworkData(ManeuverData data)
    {
      this.CheckParent(data.ParentGUID);
      this.RelativePosition = data.RelPosition.ToVector3D();
      this.RelativeVelocity = data.RelVelocity.ToVector3D();
    }

    public bool AreOrbitsEqual(OrbitParameters orbit)
    {
      return this.parent == orbit.parent && this.eccentricity.IsEpsilonEqualD(orbit.eccentricity, 1E-08) && (this.semiMajorAxis.IsEpsilonEqualD(orbit.semiMajorAxis, 1E-08) && this.inclination.IsEpsilonEqualD(orbit.inclination, 1E-08)) && (this.argumentOfPeriapsis.IsEpsilonEqualD(orbit.argumentOfPeriapsis, 1E-08) && this.longitudeOfAscendingNode.IsEpsilonEqualD(orbit.longitudeOfAscendingNode, 1E-08)) && this.solarSystemTimeAtPeriapsis.IsEpsilonEqualD(orbit.solarSystemTimeAtPeriapsis, 0.001);
    }

    private static bool AreAnglesEqualDeg(double angle1, double angle2, double anglePrecissionDeg)
    {
      angle1 %= 360.0;
      angle2 %= 360.0;
      if (angle1 < 0.0)
        angle1 += 360.0;
      if (angle2 < 0.0)
        angle2 += 360.0;
      return angle1.IsEpsilonEqualD(angle2, anglePrecissionDeg) || angle1 >= 360.0 - anglePrecissionDeg && angle2 <= anglePrecissionDeg - 360.0 + angle1 || angle2 >= 360.0 - anglePrecissionDeg && angle1 <= anglePrecissionDeg - 360.0 + angle2;
    }

    public bool AreOrbitsOverlapping(OrbitParameters orbit, double axisPrecision = 1.0, double eccentricityPrecision = 1E-08, double anglePrecissionDeg = 1.0, double eccentricityZero = 0.001)
    {
      if (this.parent != orbit.parent || !this.eccentricity.IsEpsilonEqualD(orbit.eccentricity, eccentricityPrecision) || !this.semiMajorAxis.IsEpsilonEqualD(orbit.semiMajorAxis, axisPrecision))
        return false;
      if (!this.eccentricity.IsEpsilonEqualD(0.0, eccentricityZero))
        return OrbitParameters.AreAnglesEqualDeg(this.longitudeOfAscendingNode, orbit.longitudeOfAscendingNode, anglePrecissionDeg) && OrbitParameters.AreAnglesEqualDeg(this.argumentOfPeriapsis, orbit.argumentOfPeriapsis, anglePrecissionDeg) && OrbitParameters.AreAnglesEqualDeg(this.inclination, orbit.inclination, anglePrecissionDeg);
      bool flag1 = OrbitParameters.AreAnglesEqualDeg(this.longitudeOfAscendingNode, orbit.longitudeOfAscendingNode, anglePrecissionDeg);
      bool flag2 = OrbitParameters.AreAnglesEqualDeg(this.inclination, orbit.inclination, anglePrecissionDeg);
      return flag2 && (flag1 || this.inclination.IsEpsilonEqualD(0.0, eccentricityZero)) || flag1 && (flag2 || OrbitParameters.AreAnglesEqualDeg(this.inclination, 180.0 - orbit.inclination, anglePrecissionDeg));
    }

    public void CopyDataFrom(OrbitParameters orbit, double solarSystemTime, bool exactCopy = false)
    {
      this.parent = orbit.parent;
      this.eccentricity = orbit.eccentricity;
      this.semiMajorAxis = orbit.semiMajorAxis;
      this.semiMinorAxis = orbit.semiMinorAxis;
      this.inclination = orbit.inclination;
      this.argumentOfPeriapsis = orbit.argumentOfPeriapsis;
      this.longitudeOfAscendingNode = orbit.longitudeOfAscendingNode;
      this.solarSystemTimeAtPeriapsis = orbit.solarSystemTimeAtPeriapsis;
      this.orbitalPeriod = orbit.orbitalPeriod;
      if (exactCopy)
      {
        this.timeSincePeriapsis = orbit.timeSincePeriapsis;
        if (this.lastChangeTime < orbit.lastChangeTime)
          this.lastChangeTime = orbit.lastChangeTime;
        this.RelativePosition = orbit.RelativePosition;
        this.RelativeVelocity = orbit.RelativeVelocity;
      }
      else
        this.ResetOrbit(solarSystemTime);
    }

    public void SetLastChangeTime(double time)
    {
      this.lastChangeTime = time;
    }

    public double CircularOrbitVelocityMagnitudeAtDistance(double distance)
    {
      return System.Math.Sqrt(this.gravParameter / distance);
    }

    public double RandomOrbitVelocityMagnitudeAtDistance(double distance)
    {
      double num1 = MathHelper.RandomRange(0.0, 0.8);
      double num2 = distance / (1.0 - num1);
      if (num2 + num2 - distance > this.gravityInfluenceRadius)
      {
        num2 = this.gravityInfluenceRadius * 0.8 / 2.0;
        num1 = 1.0 - distance / num2;
      }
      if (num2 + num2 - distance > 897587224200.0)
      {
        double num3 = 359034889680.0;
        num1 = 1.0 - distance / num3;
      }
      if (num1 < 0.0)
        num1 = 0.0;
      return System.Math.Sqrt((num1 + 1.0) / distance * this.gravParameter);
    }

    public string DebugString()
    {
      return string.Format("P {0}, ECC {1}, SMA {2}, INC {3}, AOP {4}, LOAN {5}, SSTAP {6}, TSP {7}", (object) (this.parent != null ? this.parent.GUID : -1L), (object) this.eccentricity, (object) this.semiMajorAxis, (object) this.inclination, (object) this.argumentOfPeriapsis, (object) this.longitudeOfAscendingNode, (object) this.solarSystemTimeAtPeriapsis, (object) this.timeSincePeriapsis);
    }
  }
}
