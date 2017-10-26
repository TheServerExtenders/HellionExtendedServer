// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.CelestialBody
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.ShipComponents;

namespace ZeroGravity.Objects
{
  public class CelestialBody
  {
    public OrbitParameters Orbit = new OrbitParameters();
    public List<CelestialBody> ChildBodies = new List<CelestialBody>();
    private Dictionary<short[], CelestialBody.ResourceSection> resources = new Dictionary<short[], CelestialBody.ResourceSection>((IEqualityComparer<short[]>) new DistributionManager.ShortArrayComparer());
    public long GUID;
    public CelestialBody Parent;
    public double Radius;
    public static List<ResourceType> MiningResourceTypes;

    public Vector3D Position
    {
      get
      {
        return this.Orbit.Position;
      }
    }

    public Vector3D Velocity
    {
      get
      {
        return this.Orbit.Velocity;
      }
    }

    public double AtmosphereLevel1 { get; private set; }

    public double AtmosphereLevel2 { get; private set; }

    public double AtmosphereLevel3 { get; private set; }

    public int ResourceResolution { get; private set; }

    public CelestialBody(long guid)
    {
      this.GUID = guid;
      this.Orbit.SetCelestialBody(this);
      CelestialBody.MiningResourceTypes = new List<ResourceType>()
      {
        ResourceType.Ice,
        ResourceType.HeavyIce,
        ResourceType.DryIce,
        ResourceType.NitrateMinerals
      };
    }

    public bool Set(CelestialBody parent, double mass, double radius, double rotationPeriod, double eccentricity, double semiMajorAxis, double inclination, double argumentOfPeriapsis, double longitudeOfAscendingNode, double solarSystemTime, double atmosphereLevel1, double atmosphereLevel2, double atmosphereLevel3, int resourceResolution)
    {
      this.Parent = parent;
      if (this.Parent != null)
        this.Parent.ChildBodies.Add(this);
      this.Radius = radius;
      this.AtmosphereLevel1 = atmosphereLevel1;
      this.AtmosphereLevel2 = atmosphereLevel2;
      this.AtmosphereLevel3 = atmosphereLevel3;
      this.ResourceResolution = resourceResolution;
      this.SetResourcesInSections(new float[4]
      {
        1000f,
        1000f,
        1000f,
        1000f
      });
      this.Orbit.InitFromElements(this.Parent != null ? this.Parent.Orbit : (OrbitParameters) null, mass, radius, rotationPeriod, eccentricity, semiMajorAxis, inclination, argumentOfPeriapsis, longitudeOfAscendingNode, 0.0, 0.0);
      return this.Orbit.IsOrbitValid;
    }

    public void Update(double timeDelta)
    {
      if (!this.Orbit.IsOrbitValid)
        return;
      this.Orbit.UpdateOrbit(timeDelta);
    }

    private void FillLevels(short[] index, ref float[] zbirRes)
    {
      Random random = new Random();
      if (!this.resources.ContainsKey(index))
        this.resources.Add(index, new CelestialBody.ResourceSection());
      CelestialBody.ResourceSection resource = this.resources[index];
      bool flag = false;
      for (int index1 = 0; index1 < 3; ++index1)
      {
        int index2 = random.Next(0, CelestialBody.MiningResourceTypes.Count);
        float num = (float) random.NextDouble();
        if ((double) num > 0.0)
        {
          flag = true;
          if (resource.lvls[index1].ContainsKey(CelestialBody.MiningResourceTypes[index2]))
          {
            Dictionary<ResourceType, float> lvl = resource.lvls[index1];
            ResourceType miningResourceType = CelestialBody.MiningResourceTypes[index2];
            lvl[miningResourceType] = lvl[miningResourceType] + num;
          }
          else
            resource.lvls[index1].Add(CelestialBody.MiningResourceTypes[index2], num);
          zbirRes[index2] += resource.lvls[index1][CelestialBody.MiningResourceTypes[index2]];
        }
      }
      if (flag)
        return;
      this.resources.Remove(index);
    }

    public void SetResourcesInSections(float[] resourceAmountToSet)
    {
      Random random = new Random();
      float[] zbirRes = new float[CelestialBody.MiningResourceTypes.Count];
      for (short index1 = 0; (int) index1 < this.ResourceResolution * 2; ++index1)
      {
        for (short index2 = 0; (int) index2 < this.ResourceResolution; ++index2)
          this.FillLevels(new short[2]{ index1, index2 }, ref zbirRes);
      }
      float[] numArray1 = new float[CelestialBody.MiningResourceTypes.Count];
      for (int index = 0; index < numArray1.Length; ++index)
        numArray1[index] = resourceAmountToSet[index] / zbirRes[index];
      short[] index3 = new short[2];
      float[] numArray2 = new float[4];
      for (short index1 = 0; (int) index1 < this.ResourceResolution * 2; ++index1)
      {
        for (short index2 = 0; (int) index2 < this.ResourceResolution; ++index2)
        {
          index3[0] = index1;
          index3[1] = index2;
          int index4 = 0;
          while (true)
          {
            if (index4 < this.resources[new short[2]
            {
              index1,
              index2
            }].lvls.Count)
            {
              for (int index5 = 0; index5 < CelestialBody.MiningResourceTypes.Count; ++index5)
              {
                if (this.resources[index3].lvls[index4].ContainsKey(CelestialBody.MiningResourceTypes[index5]))
                {
                  float num = this.resources[index3].lvls[index4][CelestialBody.MiningResourceTypes[index5]];
                  Dictionary<ResourceType, float> lvl = this.resources[index3].lvls[index4];
                  ResourceType miningResourceType = CelestialBody.MiningResourceTypes[index5];
                  lvl[miningResourceType] = lvl[miningResourceType] * numArray1[index5];
                  numArray2[index5] += this.resources[index3].lvls[index4][CelestialBody.MiningResourceTypes[index5]];
                }
              }
              ++index4;
            }
            else
              break;
          }
        }
      }
    }

    public Dictionary<ResourceType, float> GetDrillingResourcesOnLevel(Vector3D position, out short lvlRet, out short[] index, out List<float> dbgRes)
    {
      short[] sectionFromPosition = this.GetSectionFromPosition(position);
      int magnitude = (int) position.Magnitude;
      dbgRes = new List<float>();
      int index1 = (double) magnitude >= this.Orbit.Radius + this.AtmosphereLevel1 ? ((double) magnitude >= this.Orbit.Radius + this.AtmosphereLevel2 || (double) magnitude <= this.Orbit.Radius + this.AtmosphereLevel1 ? ((double) magnitude >= this.Orbit.Radius + this.AtmosphereLevel3 || (double) magnitude <= this.Orbit.Radius + this.AtmosphereLevel2 ? -1 : 2) : 1) : 0;
      lvlRet = (short) index1;
      index = sectionFromPosition;
      if (index1 == -1 || !this.resources.ContainsKey(sectionFromPosition))
        return (Dictionary<ResourceType, float>) null;
      Dictionary<ResourceType, float> dictionary = new Dictionary<ResourceType, float>();
      foreach (KeyValuePair<ResourceType, float> keyValuePair in this.resources[sectionFromPosition].lvls[index1])
      {
        float num = keyValuePair.Value * (float) Server.Instance.DeltaTime;
        if ((double) this.resources[sectionFromPosition].lvls[index1][keyValuePair.Key] - (double) num < 0.0)
          num = this.resources[sectionFromPosition].lvls[index1][keyValuePair.Key];
        dictionary.Add(keyValuePair.Key, num);
      }
      foreach (KeyValuePair<ResourceType, float> keyValuePair in dictionary)
      {
        Dictionary<ResourceType, float> lvl1 = this.resources[sectionFromPosition].lvls[index1];
        ResourceType key1 = keyValuePair.Key;
        lvl1[key1] = lvl1[key1] - keyValuePair.Value;
        if ((double) this.resources[sectionFromPosition].lvls[index1][keyValuePair.Key] <= 0.0)
        {
          this.resources[sectionFromPosition].lvls[index1].Remove(keyValuePair.Key);
        }
        else
        {
          Dictionary<ResourceType, float> lvl2 = this.resources[sectionFromPosition].lvls[index1];
          ResourceType key2 = keyValuePair.Key;
          lvl2[key2] = lvl2[key2] - keyValuePair.Value;
        }
        dbgRes.Add(this.resources[sectionFromPosition].lvls[index1][keyValuePair.Key]);
      }
      return dictionary;
    }

    public short[] GetSectionFromPosition(Vector3D position)
    {
      short num1 = (short) System.Math.Tan(-1.0 * position.Y / position.X);
      short num2 = (short) System.Math.Tan(-1.0 * System.Math.Sqrt(position.X * position.X + position.Y * position.Y) / position.Z);
      short num3 = (short) (360 / (this.ResourceResolution * 2));
      short num4 = (short) (360 / this.ResourceResolution);
      return new short[2]
      {
        (short) ((int) num1 / (int) num3),
        (short) (((int) num2 + 180) / (int) num4)
      };
    }

    public class ResourceSection
    {
      public List<Dictionary<ResourceType, float>> lvls = new List<Dictionary<ResourceType, float>>()
      {
        new Dictionary<ResourceType, float>(),
        new Dictionary<ResourceType, float>(),
        new Dictionary<ResourceType, float>()
      };
    }
  }
}
