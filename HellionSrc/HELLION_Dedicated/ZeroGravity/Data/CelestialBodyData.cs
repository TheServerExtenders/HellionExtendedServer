// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.CelestialBodyData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

namespace ZeroGravity.Data
{
  public class CelestialBodyData : ISceneData
  {
    public short GUID;
    public string Name;
    public long ParentGUID;
    public double Mass;
    public double Radius;
    public double RotationPeriod;
    public double Eccentricity;
    public double SemiMajorAxis;
    public double Inclination;
    public double ArgumentOfPeriapsis;
    public double LongitudeOfAscendingNode;
    public string PlanetsPrefabPath;
    public string MainCameraPrefabPath;
    public string NavigationPrefabPath;
    public double AtmosphereLevel1;
    public double AtmosphereLevel2;
    public double AtmosphereLevel3;
    public int ResourceResolution;
  }
}
