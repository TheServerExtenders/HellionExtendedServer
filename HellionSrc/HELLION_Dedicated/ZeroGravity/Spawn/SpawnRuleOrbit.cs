// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Spawn.SpawnRuleOrbit
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ZeroGravity.Data;
using ZeroGravity.Math;

namespace ZeroGravity.Spawn
{
  public class SpawnRuleOrbit
  {
    public CelestialBodyGUID CelestialBody;
    public SpawnRange<double> PeriapsisDistance;
    public SpawnRange<double> ApoapsisDistance;
    public SpawnRange<double> Inclination;
    public SpawnRange<double> ArgumentOfPeriapsis;
    public SpawnRange<double> LongitudeOfAscendingNode;
    public SpawnRange<double> TrueAnomaly;

    public OrbitParameters GenerateRandomOrbit(ZeroGravity.Objects.CelestialBody parentBody = null)
    {
      if (parentBody == null)
        parentBody = Server.Instance.SolarSystem.GetCelestialBody((long) this.CelestialBody);
      double periapsisDistance = MathHelper.RandomRange(this.PeriapsisDistance.Min, this.PeriapsisDistance.Max);
      double apoapsisDistance = MathHelper.RandomRange(this.ApoapsisDistance.Min, this.ApoapsisDistance.Max);
      if (periapsisDistance > apoapsisDistance)
      {
        apoapsisDistance = periapsisDistance;
        periapsisDistance = apoapsisDistance;
      }
      OrbitParameters orbitParameters = new OrbitParameters();
      orbitParameters.InitFromPeriapisAndApoapsis(parentBody.Orbit, periapsisDistance, apoapsisDistance, MathHelper.RandomRange(this.Inclination.Min, this.Inclination.Max), MathHelper.RandomRange(this.ArgumentOfPeriapsis.Min, this.ArgumentOfPeriapsis.Max), MathHelper.RandomRange(this.LongitudeOfAscendingNode.Min, this.LongitudeOfAscendingNode.Max), MathHelper.RandomRange(this.TrueAnomaly.Min, this.TrueAnomaly.Max), 0.0);
      return orbitParameters;
    }
  }
}
