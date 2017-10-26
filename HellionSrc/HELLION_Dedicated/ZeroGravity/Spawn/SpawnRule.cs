// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Spawn.SpawnRule
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.Linq;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Objects;
using ZeroGravity.ShipComponents;

namespace ZeroGravity.Spawn
{
  public class SpawnRule
  {
    public double CurrTimerSec = 0.0;
    public List<SpaceObjectVessel> SpawnedVessels = new List<SpaceObjectVessel>();
    private SpawnRange<int> totalScenes = new SpawnRange<int>();
    private Dictionary<int, int> clusterVesselsCount = new Dictionary<int, int>();
    public string Name;
    public string StationName;
    public SpawnRuleOrbit Orbit;
    public SpawnRuleLocationType LocationType;
    public string LocationTag;
    public double RespawnTimerSec;
    public SpawnRange<int> NumberOfClusters;
    public List<SpawnRuleScene> ScenePool;
    public List<SpawnRuleLoot> LootPool;
    public bool IsVisibleOnRadar;
    private int maxScenesPerCluster;

    public int NumberOfClustersCurr
    {
      get
      {
        return this.clusterVesselsCount.Count;
      }
    }

    private void removeVesselFromCluster(int cluster)
    {
      if (!this.clusterVesselsCount.ContainsKey(cluster))
        return;
      Dictionary<int, int> clusterVesselsCount = this.clusterVesselsCount;
      int num1 = cluster;
      int index1 = num1;
      int num2 = clusterVesselsCount[index1];
      int index2 = num1;
      int num3 = num2 - 1;
      clusterVesselsCount[index2] = num3;
      if (this.clusterVesselsCount[cluster] <= 0)
        this.clusterVesselsCount.Remove(cluster);
    }

    public bool AddVesselToRule(SpaceObjectVessel ves, SpawnRuleScene pool, int cluster)
    {
      ++this.totalScenes.Min;
      --pool.Count;
      if (pool.Count < 0)
        pool.Count = 0;
      if (!this.clusterVesselsCount.ContainsKey(cluster))
        this.clusterVesselsCount.Add(cluster, 0);
      Dictionary<int, int> clusterVesselsCount = this.clusterVesselsCount;
      int num1 = cluster;
      int index1 = num1;
      int num2 = clusterVesselsCount[index1];
      int index2 = num1;
      int num3 = num2 + 1;
      clusterVesselsCount[index2] = num3;
      this.SpawnedVessels.Add(ves);
      return true;
    }

    public bool AddDynamicObjectToRule(DynamicObject dobj, SpawnRuleLoot pool)
    {
      --pool.Count;
      if (pool.Count < 0)
        pool.Count = 0;
      return true;
    }

    public void Initialize(bool isPersistenceInitialize)
    {
      this.totalScenes.Max = 0;
      if (this.ScenePool != null && this.ScenePool.Count > 0)
      {
        foreach (SpawnRuleScene spawnRuleScene in this.ScenePool)
          this.totalScenes.Max += spawnRuleScene.CountMax;
      }
      if (this.totalScenes.Max > 0 && this.totalScenes.Max < this.NumberOfClusters.Max)
      {
        Dbg.Warning(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" max number of clusters \"{1}\" are lower than number of total scenes \"{2}\", clusters auto adjusted", (object) this.Name, (object) this.NumberOfClusters.Max, (object) this.totalScenes.Max));
        this.NumberOfClusters.Max = this.totalScenes.Max;
      }
      if (this.totalScenes.Max > 0 && this.totalScenes.Max < this.NumberOfClusters.Min)
      {
        Dbg.Warning(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" min number of clusters \"{1}\" are lower than number of total scenes \"{2}\", clusters auto adjusted", (object) this.Name, (object) this.NumberOfClusters.Min, (object) this.totalScenes.Max));
        this.NumberOfClusters.Min = this.totalScenes.Max;
      }
      if (this.NumberOfClusters.Min > this.NumberOfClusters.Max)
        this.NumberOfClusters.Min = this.NumberOfClusters.Max;
      this.maxScenesPerCluster = this.NumberOfClusters.Max != 0 ? System.Math.Max(this.totalScenes.Max / this.NumberOfClusters.Max, 1) : 0;
      if (this.LocationType != SpawnRuleLocationType.StartingScene && this.LocationType != SpawnRuleLocationType.Random)
        this.LocationTag = !this.LocationTag.IsNullOrEmpty() ? this.LocationTag + ";" + SpawnManager.Settings.StationTag : SpawnManager.Settings.StationTag;
      if (isPersistenceInitialize)
        return;
      if (this.LocationType == SpawnRuleLocationType.Random)
      {
        if (this.NumberOfClustersCurr >= this.NumberOfClusters.Min)
          return;
        for (int index = 0; index < this.NumberOfClusters.Min; ++index)
          this.ExecuteRule(true);
      }
      else
      {
        if (this.LocationType == SpawnRuleLocationType.StartingScene)
          return;
        this.ExecuteRule(true);
      }
    }

    private Vector3D FindEmptyRelativePosition(SpaceObjectVessel ves, ref List<SpaceObjectVessel> spawnedVessels)
    {
      SpaceObjectVessel spaceObjectVessel = spawnedVessels[0];
      int num1 = 0;
      int num2 = 0;
      int num3 = 1;
      while (num1 < 200)
      {
        Vector3D b = spaceObjectVessel.Orbit.RelativePosition + MathHelper.RandomRotation() * (Vector3D.Forward * (ves.Radius + spaceObjectVessel.Radius + SpawnManager.Settings.RandomLocationClusterItemCheckDistance * (double) num3));
        bool flag = true;
        for (int index = 1; index < spawnedVessels.Count; ++index)
        {
          if (spawnedVessels[index].Orbit.RelativePosition.DistanceSquared(b) < System.Math.Pow(ves.Radius + spaceObjectVessel.RadarSignature + SpawnManager.Settings.RandomLocationClusterItemCheckDistance, 2.0))
          {
            flag = false;
            break;
          }
        }
        if (flag)
          return b - spaceObjectVessel.Orbit.RelativePosition;
        ++num1;
        ++num2;
        if (num2 == 5)
        {
          num2 = 0;
          if (num1 < 100)
            ++num3;
          else
            num3 += 5;
        }
      }
      return Vector3D.Zero;
    }

    private SpaceObjectVessel ExecuterRandomRule()
    {
      if (this.LocationType != SpawnRuleLocationType.Random)
        return (SpaceObjectVessel) null;
      bool flag1 = false;
      if (this.NumberOfClustersCurr < this.NumberOfClusters.Max)
      {
        int num1 = 1;
        if (this.totalScenes.Max > this.NumberOfClusters.Max)
          num1 = MathHelper.Clamp((this.totalScenes.Max - this.totalScenes.Min) / (this.NumberOfClusters.Max - this.NumberOfClustersCurr), 1, this.maxScenesPerCluster);
        int key = 0;
        if (this.clusterVesselsCount.Count > 0)
        {
          for (int index = 0; index < this.NumberOfClusters.Max; ++index)
          {
            if (!this.clusterVesselsCount.Keys.Contains<int>(index))
            {
              key = index;
              break;
            }
          }
        }
        SpaceObjectVessel target = (SpaceObjectVessel) null;
        List<SpaceObjectVessel> spawnedVessels = new List<SpaceObjectVessel>();
        bool flag2 = false;
        for (int index1 = 0; index1 < this.ScenePool.Count && num1 > 0; ++index1)
        {
          if (this.ScenePool[index1].Count == 0)
          {
            if (index1 == this.ScenePool.Count - 1 && num1 > 0)
            {
              flag2 = true;
              index1 = -1;
            }
          }
          else
          {
            SpawnRuleScene spawnRuleScene = this.ScenePool[index1];
            int num2 = flag2 || num1 != 1 ? (!flag2 ? (spawnRuleScene.CountMax < this.NumberOfClusters.Max ? MathHelper.RandomRange(0, 2) : MathHelper.RandomRange(1, spawnRuleScene.CountMax / this.NumberOfClusters.Max + 1)) : 1) : MathHelper.RandomRange(0, 2);
            if (num2 > spawnRuleScene.Count)
              num2 = spawnRuleScene.Count;
            if (num2 > num1)
              num2 = num1;
            if (num2 > 0)
            {
              int num3 = 0;
              flag1 = true;
              if (target == null)
              {
                target = !GameScenes.Ranges.IsAsteroid(spawnRuleScene.SceneID) ? (SpaceObjectVessel) Ship.CreateNewShip(spawnRuleScene.SceneID, "", -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), this.LocationTag, true, 0.03, 0.3, this.Orbit, 1.5, SpawnManager.Settings.RandomLocationCheckDistance) : (SpaceObjectVessel) Asteroid.CreateNewAsteroid(spawnRuleScene.SceneID, "", -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), true, 0.03, 0.3, this.Orbit, 1.5, SpawnManager.Settings.RandomLocationCheckDistance);
                if (target is Ship && this.IsVisibleOnRadar)
                  (target as Ship).ToggleDistress(true, DistressType.AlwaysVisible);
                target.IsPartOfSpawnSystem = true;
                spawnedVessels.Add(target);
                SpawnManager.SpawnedVessels.Add(target.GUID, new Tuple<SpawnRule, SpawnRuleScene, int>(this, spawnRuleScene, key));
                target.Health = MathHelper.RandomRange(target.MaxHealth * spawnRuleScene.HealthMin, target.MaxHealth * spawnRuleScene.HealthMax);
                --spawnRuleScene.Count;
                ++this.totalScenes.Min;
                --num1;
                num3 = 1;
              }
              for (int index2 = num3; index2 < num2; ++index2)
              {
                QuaternionD? nullable;
                SpaceObjectVessel ves;
                if (GameScenes.Ranges.IsAsteroid(spawnRuleScene.SceneID))
                {
                  int sceneId = (int) spawnRuleScene.SceneID;
                  string registration = "";
                  long asteroidGUID = -1;
                  nullable = new QuaternionD?(MathHelper.RandomRotation());
                  List<long> nearArtificialBodyGUIDs = new List<long>();
                  nearArtificialBodyGUIDs.Add(target.GUID);
                  // ISSUE: variable of the null type
                  __Null local1 = null;
                  Vector3D? positionOffset = new Vector3D?();
                  Vector3D? velocityAtPosition = new Vector3D?();
                  QuaternionD? localRotation = nullable;
                  int num4 = 0;
                  double distanceFromSurfacePercMin = 0.03;
                  double distanceFromSurfacePercMax = 0.3;
                  // ISSUE: variable of the null type
                  __Null local2 = null;
                  double celestialBodyDeathDistanceMultiplier = 1.5;
                  double artificialBodyDistanceCheck = 100.0;
                  ves = (SpaceObjectVessel) Asteroid.CreateNewAsteroid((GameScenes.SceneID) sceneId, registration, asteroidGUID, nearArtificialBodyGUIDs, (List<long>) local1, positionOffset, velocityAtPosition, localRotation, num4 != 0, distanceFromSurfacePercMin, distanceFromSurfacePercMax, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier, artificialBodyDistanceCheck);
                }
                else
                {
                  int sceneId = (int) spawnRuleScene.SceneID;
                  string registration = "";
                  long shipID = -1;
                  nullable = new QuaternionD?(MathHelper.RandomRotation());
                  string locationTag = this.LocationTag;
                  List<long> nearArtificialBodyGUIDs = new List<long>();
                  nearArtificialBodyGUIDs.Add(target.GUID);
                  // ISSUE: variable of the null type
                  __Null local1 = null;
                  Vector3D? positionOffset = new Vector3D?();
                  Vector3D? velocityAtPosition = new Vector3D?();
                  QuaternionD? localRotation = nullable;
                  string vesselTag = locationTag;
                  int num4 = 0;
                  double distanceFromSurfacePercMin = 0.03;
                  double distanceFromSurfacePercMax = 0.3;
                  // ISSUE: variable of the null type
                  __Null local2 = null;
                  double celestialBodyDeathDistanceMultiplier = 1.5;
                  double artificialBodyDistanceCheck = 100.0;
                  ves = (SpaceObjectVessel) Ship.CreateNewShip((GameScenes.SceneID) sceneId, registration, shipID, nearArtificialBodyGUIDs, (List<long>) local1, positionOffset, velocityAtPosition, localRotation, vesselTag, num4 != 0, distanceFromSurfacePercMin, distanceFromSurfacePercMax, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier, artificialBodyDistanceCheck);
                }
                Vector3D relativePosition = this.FindEmptyRelativePosition(ves, ref spawnedVessels);
                if (relativePosition.IsEpsilonZero(double.Epsilon))
                {
                  Dbg.Error((object) "SPAWN MANAGER - Failed to find empty spawn position for rule", (object) this.Name);
                  Server.Instance.DestroyArtificialBody((ArtificialBody) ves, true, false);
                }
                else
                {
                  ves.Orbit.RelativePosition = target.Orbit.RelativePosition + relativePosition;
                  ves.StabilizeToTarget(target, true);
                  ves.IsPartOfSpawnSystem = true;
                  spawnedVessels.Add(ves);
                  SpawnManager.SpawnedVessels.Add(ves.GUID, new Tuple<SpawnRule, SpawnRuleScene, int>(this, spawnRuleScene, key));
                  ves.Health = MathHelper.RandomRange(ves.MaxHealth * spawnRuleScene.HealthMin, ves.MaxHealth * spawnRuleScene.HealthMax);
                  --spawnRuleScene.Count;
                  ++this.totalScenes.Min;
                  --num1;
                }
              }
            }
            if (num1 > 0)
            {
              if (index1 == this.ScenePool.Count - 1 && num1 > 0)
              {
                flag2 = true;
                index1 = -1;
              }
            }
            else
              break;
          }
        }
        if (flag1)
        {
          this.SpawnedVessels.AddRange((IEnumerable<SpaceObjectVessel>) spawnedVessels);
          this.clusterVesselsCount.Add(key, spawnedVessels.Count);
          this.DistributeLoot(spawnedVessels);
        }
        return target;
      }
      if (!flag1)
        this.DistributeLoot(this.SpawnedVessels);
      return (SpaceObjectVessel) null;
    }

    private SpaceObjectVessel ExecuterStationRule(bool isInitialize)
    {
      if (!isInitialize)
      {
        this.DistributeLoot(this.SpawnedVessels);
        return (SpaceObjectVessel) null;
      }
      if (this.LocationType == SpawnRuleLocationType.Automated_Refinery_B7)
      {
        Ship newShip1 = Ship.CreateNewShip(GameScenes.SceneID.ALtCorp_PowerSupply_Module, "", -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), this.LocationTag, true, 0.03, 0.3, this.Orbit, 1.5, SpawnManager.Settings.RandomLocationCheckDistance);
        if (this.IsVisibleOnRadar)
          newShip1.ToggleDistress(true, DistressType.AlwaysVisible);
        int num1 = 5;
        string registration1 = "";
        long shipID1 = -1;
        List<long> nearArtificialBodyGUIDs1 = new List<long>();
        nearArtificialBodyGUIDs1.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local1 = null;
        Vector3D? positionOffset1 = new Vector3D?();
        Vector3D? velocityAtPosition1 = new Vector3D?();
        QuaternionD? localRotation1 = new QuaternionD?();
        string locationTag1 = this.LocationTag;
        int num2 = 1;
        double distanceFromSurfacePercMin1 = 0.03;
        double distanceFromSurfacePercMax1 = 0.3;
        // ISSUE: variable of the null type
        __Null local2 = null;
        double celestialBodyDeathDistanceMultiplier1 = 1.5;
        double artificialBodyDistanceCheck1 = 100.0;
        Ship newShip2 = Ship.CreateNewShip((GameScenes.SceneID) num1, registration1, shipID1, nearArtificialBodyGUIDs1, (List<long>) local1, positionOffset1, velocityAtPosition1, localRotation1, locationTag1, num2 != 0, distanceFromSurfacePercMin1, distanceFromSurfacePercMax1, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier1, artificialBodyDistanceCheck1);
        newShip2.DockToShip(newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip1.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip1, true);
        int num3 = 9;
        string registration2 = "";
        long shipID2 = -1;
        List<long> nearArtificialBodyGUIDs2 = new List<long>();
        nearArtificialBodyGUIDs2.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local3 = null;
        Vector3D? positionOffset2 = new Vector3D?();
        Vector3D? velocityAtPosition2 = new Vector3D?();
        QuaternionD? localRotation2 = new QuaternionD?();
        string locationTag2 = this.LocationTag;
        int num4 = 1;
        double distanceFromSurfacePercMin2 = 0.03;
        double distanceFromSurfacePercMax2 = 0.3;
        // ISSUE: variable of the null type
        __Null local4 = null;
        double celestialBodyDeathDistanceMultiplier2 = 1.5;
        double artificialBodyDistanceCheck2 = 100.0;
        Ship newShip3 = Ship.CreateNewShip((GameScenes.SceneID) num3, registration2, shipID2, nearArtificialBodyGUIDs2, (List<long>) local3, positionOffset2, velocityAtPosition2, localRotation2, locationTag2, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local4, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
        newShip3.DockToShip(newShip3.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 13)), newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip2, true);
        int num5 = 9;
        string registration3 = "";
        long shipID3 = -1;
        List<long> nearArtificialBodyGUIDs3 = new List<long>();
        nearArtificialBodyGUIDs3.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local5 = null;
        Vector3D? positionOffset3 = new Vector3D?();
        Vector3D? velocityAtPosition3 = new Vector3D?();
        QuaternionD? localRotation3 = new QuaternionD?();
        string locationTag3 = this.LocationTag;
        int num6 = 1;
        double distanceFromSurfacePercMin3 = 0.03;
        double distanceFromSurfacePercMax3 = 0.3;
        // ISSUE: variable of the null type
        __Null local6 = null;
        double celestialBodyDeathDistanceMultiplier3 = 1.5;
        double artificialBodyDistanceCheck3 = 100.0;
        Ship newShip4 = Ship.CreateNewShip((GameScenes.SceneID) num5, registration3, shipID3, nearArtificialBodyGUIDs3, (List<long>) local5, positionOffset3, velocityAtPosition3, localRotation3, locationTag3, num6 != 0, distanceFromSurfacePercMin3, distanceFromSurfacePercMax3, (SpawnRuleOrbit) local6, celestialBodyDeathDistanceMultiplier3, artificialBodyDistanceCheck3);
        newShip4.DockToShip(newShip4.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 12)), newShip3.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 8)), newShip3, true);
        int num7 = 9;
        string registration4 = "";
        long shipID4 = -1;
        List<long> nearArtificialBodyGUIDs4 = new List<long>();
        nearArtificialBodyGUIDs4.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local7 = null;
        Vector3D? positionOffset4 = new Vector3D?();
        Vector3D? velocityAtPosition4 = new Vector3D?();
        QuaternionD? localRotation4 = new QuaternionD?();
        string locationTag4 = this.LocationTag;
        int num8 = 1;
        double distanceFromSurfacePercMin4 = 0.03;
        double distanceFromSurfacePercMax4 = 0.3;
        // ISSUE: variable of the null type
        __Null local8 = null;
        double celestialBodyDeathDistanceMultiplier4 = 1.5;
        double artificialBodyDistanceCheck4 = 100.0;
        Ship newShip5 = Ship.CreateNewShip((GameScenes.SceneID) num7, registration4, shipID4, nearArtificialBodyGUIDs4, (List<long>) local7, positionOffset4, velocityAtPosition4, localRotation4, locationTag4, num8 != 0, distanceFromSurfacePercMin4, distanceFromSurfacePercMax4, (SpawnRuleOrbit) local8, celestialBodyDeathDistanceMultiplier4, artificialBodyDistanceCheck4);
        newShip5.DockToShip(newShip5.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 8)), newShip4.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 8)), newShip4, true);
        int num9 = 12;
        string registration5 = "";
        long shipID5 = -1;
        List<long> nearArtificialBodyGUIDs5 = new List<long>();
        nearArtificialBodyGUIDs5.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local9 = null;
        Vector3D? positionOffset5 = new Vector3D?();
        Vector3D? velocityAtPosition5 = new Vector3D?();
        QuaternionD? localRotation5 = new QuaternionD?();
        string locationTag5 = this.LocationTag;
        int num10 = 1;
        double distanceFromSurfacePercMin5 = 0.03;
        double distanceFromSurfacePercMax5 = 0.3;
        // ISSUE: variable of the null type
        __Null local10 = null;
        double celestialBodyDeathDistanceMultiplier5 = 1.5;
        double artificialBodyDistanceCheck5 = 100.0;
        Ship newShip6 = Ship.CreateNewShip((GameScenes.SceneID) num9, registration5, shipID5, nearArtificialBodyGUIDs5, (List<long>) local9, positionOffset5, velocityAtPosition5, localRotation5, locationTag5, num10 != 0, distanceFromSurfacePercMin5, distanceFromSurfacePercMax5, (SpawnRuleOrbit) local10, celestialBodyDeathDistanceMultiplier5, artificialBodyDistanceCheck5);
        newShip6.DockToShip(newShip6.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip4.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 13)), newShip4, true);
        int num11 = 8;
        string registration6 = "";
        long shipID6 = -1;
        List<long> nearArtificialBodyGUIDs6 = new List<long>();
        nearArtificialBodyGUIDs6.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local11 = null;
        Vector3D? positionOffset6 = new Vector3D?();
        Vector3D? velocityAtPosition6 = new Vector3D?();
        QuaternionD? localRotation6 = new QuaternionD?();
        string locationTag6 = this.LocationTag;
        int num12 = 1;
        double distanceFromSurfacePercMin6 = 0.03;
        double distanceFromSurfacePercMax6 = 0.3;
        // ISSUE: variable of the null type
        __Null local12 = null;
        double celestialBodyDeathDistanceMultiplier6 = 1.5;
        double artificialBodyDistanceCheck6 = 100.0;
        Ship newShip7 = Ship.CreateNewShip((GameScenes.SceneID) num11, registration6, shipID6, nearArtificialBodyGUIDs6, (List<long>) local11, positionOffset6, velocityAtPosition6, localRotation6, locationTag6, num12 != 0, distanceFromSurfacePercMin6, distanceFromSurfacePercMax6, (SpawnRuleOrbit) local12, celestialBodyDeathDistanceMultiplier6, artificialBodyDistanceCheck6);
        newShip7.DockToShip(newShip7.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip5.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 13)), newShip5, true);
        int num13 = 5;
        string registration7 = "";
        long shipID7 = -1;
        List<long> nearArtificialBodyGUIDs7 = new List<long>();
        nearArtificialBodyGUIDs7.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local13 = null;
        Vector3D? positionOffset7 = new Vector3D?();
        Vector3D? velocityAtPosition7 = new Vector3D?();
        QuaternionD? localRotation7 = new QuaternionD?();
        string locationTag7 = this.LocationTag;
        int num14 = 1;
        double distanceFromSurfacePercMin7 = 0.03;
        double distanceFromSurfacePercMax7 = 0.3;
        // ISSUE: variable of the null type
        __Null local14 = null;
        double celestialBodyDeathDistanceMultiplier7 = 1.5;
        double artificialBodyDistanceCheck7 = 100.0;
        Ship newShip8 = Ship.CreateNewShip((GameScenes.SceneID) num13, registration7, shipID7, nearArtificialBodyGUIDs7, (List<long>) local13, positionOffset7, velocityAtPosition7, localRotation7, locationTag7, num14 != 0, distanceFromSurfacePercMin7, distanceFromSurfacePercMax7, (SpawnRuleOrbit) local14, celestialBodyDeathDistanceMultiplier7, artificialBodyDistanceCheck7);
        newShip8.DockToShip(newShip8.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip7.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip7, true);
        newShip1.VesselData.VesselRegistration = !this.StationName.IsNullOrEmpty() ? this.StationName : Server.NameGenerator.GenerateStationRegistration();
        this.SpawnedVessels.Add((SpaceObjectVessel) newShip1);
        newShip1.IsArenaVessel = true;
        foreach (VesselDockingPort dockingPort in newShip1.DockingPorts)
          dockingPort.Locked = true;
        foreach (SpaceObjectVessel allDockedVessel in newShip1.AllDockedVessels)
        {
          allDockedVessel.IsArenaVessel = true;
          this.SpawnedVessels.Add(allDockedVessel);
          foreach (VesselDockingPort dockingPort in allDockedVessel.DockingPorts)
            dockingPort.Locked = true;
        }
        this.DistributeLoot(this.SpawnedVessels);
        return (SpaceObjectVessel) newShip1;
      }
      if (this.LocationType == SpawnRuleLocationType.Waystation_H4)
      {
        Ship newShip1 = Ship.CreateNewShip(GameScenes.SceneID.ALtCorp_PowerSupply_Module, "", -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), this.LocationTag, true, 0.03, 0.3, this.Orbit, 1.5, SpawnManager.Settings.RandomLocationCheckDistance);
        if (this.IsVisibleOnRadar)
          newShip1.ToggleDistress(true, DistressType.AlwaysVisible);
        int num1 = 4;
        string registration1 = "";
        long shipID1 = -1;
        List<long> nearArtificialBodyGUIDs1 = new List<long>();
        nearArtificialBodyGUIDs1.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local1 = null;
        Vector3D? positionOffset1 = new Vector3D?();
        Vector3D? velocityAtPosition1 = new Vector3D?();
        QuaternionD? localRotation1 = new QuaternionD?();
        string locationTag1 = this.LocationTag;
        int num2 = 1;
        double distanceFromSurfacePercMin1 = 0.03;
        double distanceFromSurfacePercMax1 = 0.3;
        // ISSUE: variable of the null type
        __Null local2 = null;
        double celestialBodyDeathDistanceMultiplier1 = 1.5;
        double artificialBodyDistanceCheck1 = 100.0;
        Ship newShip2 = Ship.CreateNewShip((GameScenes.SceneID) num1, registration1, shipID1, nearArtificialBodyGUIDs1, (List<long>) local1, positionOffset1, velocityAtPosition1, localRotation1, locationTag1, num2 != 0, distanceFromSurfacePercMin1, distanceFromSurfacePercMax1, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier1, artificialBodyDistanceCheck1);
        newShip2.DockToShip(newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip1.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip1, true);
        int num3 = 8;
        string registration2 = "";
        long shipID2 = -1;
        List<long> nearArtificialBodyGUIDs2 = new List<long>();
        nearArtificialBodyGUIDs2.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local3 = null;
        Vector3D? positionOffset2 = new Vector3D?();
        Vector3D? velocityAtPosition2 = new Vector3D?();
        QuaternionD? localRotation2 = new QuaternionD?();
        string locationTag2 = this.LocationTag;
        int num4 = 1;
        double distanceFromSurfacePercMin2 = 0.03;
        double distanceFromSurfacePercMax2 = 0.3;
        // ISSUE: variable of the null type
        __Null local4 = null;
        double celestialBodyDeathDistanceMultiplier2 = 1.5;
        double artificialBodyDistanceCheck2 = 100.0;
        Ship newShip3 = Ship.CreateNewShip((GameScenes.SceneID) num3, registration2, shipID2, nearArtificialBodyGUIDs2, (List<long>) local3, positionOffset2, velocityAtPosition2, localRotation2, locationTag2, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local4, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
        newShip3.DockToShip(newShip3.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 5)), newShip2, true);
        int num5 = 3;
        string registration3 = "";
        long shipID3 = -1;
        List<long> nearArtificialBodyGUIDs3 = new List<long>();
        nearArtificialBodyGUIDs3.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local5 = null;
        Vector3D? positionOffset3 = new Vector3D?();
        Vector3D? velocityAtPosition3 = new Vector3D?();
        QuaternionD? localRotation3 = new QuaternionD?();
        string locationTag3 = this.LocationTag;
        int num6 = 1;
        double distanceFromSurfacePercMin3 = 0.03;
        double distanceFromSurfacePercMax3 = 0.3;
        // ISSUE: variable of the null type
        __Null local6 = null;
        double celestialBodyDeathDistanceMultiplier3 = 1.5;
        double artificialBodyDistanceCheck3 = 100.0;
        Ship newShip4 = Ship.CreateNewShip((GameScenes.SceneID) num5, registration3, shipID3, nearArtificialBodyGUIDs3, (List<long>) local5, positionOffset3, velocityAtPosition3, localRotation3, locationTag3, num6 != 0, distanceFromSurfacePercMin3, distanceFromSurfacePercMax3, (SpawnRuleOrbit) local6, celestialBodyDeathDistanceMultiplier3, artificialBodyDistanceCheck3);
        newShip4.DockToShip(newShip4.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip2, true);
        int num7 = 4;
        string registration4 = "";
        long shipID4 = -1;
        List<long> nearArtificialBodyGUIDs4 = new List<long>();
        nearArtificialBodyGUIDs4.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local7 = null;
        Vector3D? positionOffset4 = new Vector3D?();
        Vector3D? velocityAtPosition4 = new Vector3D?();
        QuaternionD? localRotation4 = new QuaternionD?();
        string locationTag4 = this.LocationTag;
        int num8 = 1;
        double distanceFromSurfacePercMin4 = 0.03;
        double distanceFromSurfacePercMax4 = 0.3;
        // ISSUE: variable of the null type
        __Null local8 = null;
        double celestialBodyDeathDistanceMultiplier4 = 1.5;
        double artificialBodyDistanceCheck4 = 100.0;
        Ship newShip5 = Ship.CreateNewShip((GameScenes.SceneID) num7, registration4, shipID4, nearArtificialBodyGUIDs4, (List<long>) local7, positionOffset4, velocityAtPosition4, localRotation4, locationTag4, num8 != 0, distanceFromSurfacePercMin4, distanceFromSurfacePercMax4, (SpawnRuleOrbit) local8, celestialBodyDeathDistanceMultiplier4, artificialBodyDistanceCheck4);
        newShip5.DockToShip(newShip5.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip4.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip4, true);
        int num9 = 9;
        string registration5 = "";
        long shipID5 = -1;
        List<long> nearArtificialBodyGUIDs5 = new List<long>();
        nearArtificialBodyGUIDs5.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local9 = null;
        Vector3D? positionOffset5 = new Vector3D?();
        Vector3D? velocityAtPosition5 = new Vector3D?();
        QuaternionD? localRotation5 = new QuaternionD?();
        string locationTag5 = this.LocationTag;
        int num10 = 1;
        double distanceFromSurfacePercMin5 = 0.03;
        double distanceFromSurfacePercMax5 = 0.3;
        // ISSUE: variable of the null type
        __Null local10 = null;
        double celestialBodyDeathDistanceMultiplier5 = 1.5;
        double artificialBodyDistanceCheck5 = 100.0;
        Ship newShip6 = Ship.CreateNewShip((GameScenes.SceneID) num9, registration5, shipID5, nearArtificialBodyGUIDs5, (List<long>) local9, positionOffset5, velocityAtPosition5, localRotation5, locationTag5, num10 != 0, distanceFromSurfacePercMin5, distanceFromSurfacePercMax5, (SpawnRuleOrbit) local10, celestialBodyDeathDistanceMultiplier5, artificialBodyDistanceCheck5);
        newShip6.DockToShip(newShip6.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 13)), newShip5.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 5)), newShip5, true);
        int num11 = 9;
        string registration6 = "";
        long shipID6 = -1;
        List<long> nearArtificialBodyGUIDs6 = new List<long>();
        nearArtificialBodyGUIDs6.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local11 = null;
        Vector3D? positionOffset6 = new Vector3D?();
        Vector3D? velocityAtPosition6 = new Vector3D?();
        QuaternionD? localRotation6 = new QuaternionD?();
        string locationTag6 = this.LocationTag;
        int num12 = 1;
        double distanceFromSurfacePercMin6 = 0.03;
        double distanceFromSurfacePercMax6 = 0.3;
        // ISSUE: variable of the null type
        __Null local12 = null;
        double celestialBodyDeathDistanceMultiplier6 = 1.5;
        double artificialBodyDistanceCheck6 = 100.0;
        Ship newShip7 = Ship.CreateNewShip((GameScenes.SceneID) num11, registration6, shipID6, nearArtificialBodyGUIDs6, (List<long>) local11, positionOffset6, velocityAtPosition6, localRotation6, locationTag6, num12 != 0, distanceFromSurfacePercMin6, distanceFromSurfacePercMax6, (SpawnRuleOrbit) local12, celestialBodyDeathDistanceMultiplier6, artificialBodyDistanceCheck6);
        newShip7.DockToShip(newShip7.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 13)), newShip5.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip5, true);
        newShip1.VesselData.VesselRegistration = !this.StationName.IsNullOrEmpty() ? this.StationName : Server.NameGenerator.GenerateStationRegistration();
        this.SpawnedVessels.Add((SpaceObjectVessel) newShip1);
        newShip1.IsArenaVessel = true;
        foreach (VesselDockingPort dockingPort in newShip1.DockingPorts)
          dockingPort.Locked = true;
        foreach (SpaceObjectVessel allDockedVessel in newShip1.AllDockedVessels)
        {
          this.SpawnedVessels.Add(allDockedVessel);
          allDockedVessel.IsArenaVessel = true;
          foreach (VesselDockingPort dockingPort in allDockedVessel.DockingPorts)
            dockingPort.Locked = true;
        }
        this.DistributeLoot(this.SpawnedVessels);
        return (SpaceObjectVessel) newShip1;
      }
      if (this.LocationType == SpawnRuleLocationType.Emergency_Supply_Post_A3)
      {
        Ship newShip1 = Ship.CreateNewShip(GameScenes.SceneID.AltCorp_LifeSupportModule, "", -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), this.LocationTag, true, 0.03, 0.3, this.Orbit, 1.5, SpawnManager.Settings.RandomLocationCheckDistance);
        if (this.IsVisibleOnRadar)
          newShip1.ToggleDistress(true, DistressType.AlwaysVisible);
        int num1 = 3;
        string registration1 = "";
        long shipID1 = -1;
        List<long> nearArtificialBodyGUIDs1 = new List<long>();
        nearArtificialBodyGUIDs1.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local1 = null;
        Vector3D? positionOffset1 = new Vector3D?();
        Vector3D? velocityAtPosition1 = new Vector3D?();
        QuaternionD? localRotation1 = new QuaternionD?();
        string locationTag1 = this.LocationTag;
        int num2 = 1;
        double distanceFromSurfacePercMin1 = 0.03;
        double distanceFromSurfacePercMax1 = 0.3;
        // ISSUE: variable of the null type
        __Null local2 = null;
        double celestialBodyDeathDistanceMultiplier1 = 1.5;
        double artificialBodyDistanceCheck1 = 100.0;
        Ship newShip2 = Ship.CreateNewShip((GameScenes.SceneID) num1, registration1, shipID1, nearArtificialBodyGUIDs1, (List<long>) local1, positionOffset1, velocityAtPosition1, localRotation1, locationTag1, num2 != 0, distanceFromSurfacePercMin1, distanceFromSurfacePercMax1, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier1, artificialBodyDistanceCheck1);
        newShip2.DockToShip(newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip1.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip1, true);
        int num3 = 4;
        string registration2 = "";
        long shipID2 = -1;
        List<long> nearArtificialBodyGUIDs2 = new List<long>();
        nearArtificialBodyGUIDs2.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local3 = null;
        Vector3D? positionOffset2 = new Vector3D?();
        Vector3D? velocityAtPosition2 = new Vector3D?();
        QuaternionD? localRotation2 = new QuaternionD?();
        string locationTag2 = this.LocationTag;
        int num4 = 1;
        double distanceFromSurfacePercMin2 = 0.03;
        double distanceFromSurfacePercMax2 = 0.3;
        // ISSUE: variable of the null type
        __Null local4 = null;
        double celestialBodyDeathDistanceMultiplier2 = 1.5;
        double artificialBodyDistanceCheck2 = 100.0;
        Ship newShip3 = Ship.CreateNewShip((GameScenes.SceneID) num3, registration2, shipID2, nearArtificialBodyGUIDs2, (List<long>) local3, positionOffset2, velocityAtPosition2, localRotation2, locationTag2, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local4, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
        newShip3.DockToShip(newShip3.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 5)), newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip2, true);
        int num5 = 5;
        string registration3 = "";
        long shipID3 = -1;
        List<long> nearArtificialBodyGUIDs3 = new List<long>();
        nearArtificialBodyGUIDs3.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local5 = null;
        Vector3D? positionOffset3 = new Vector3D?();
        Vector3D? velocityAtPosition3 = new Vector3D?();
        QuaternionD? localRotation3 = new QuaternionD?();
        string locationTag3 = this.LocationTag;
        int num6 = 1;
        double distanceFromSurfacePercMin3 = 0.03;
        double distanceFromSurfacePercMax3 = 0.3;
        // ISSUE: variable of the null type
        __Null local6 = null;
        double celestialBodyDeathDistanceMultiplier3 = 1.5;
        double artificialBodyDistanceCheck3 = 100.0;
        Ship newShip4 = Ship.CreateNewShip((GameScenes.SceneID) num5, registration3, shipID3, nearArtificialBodyGUIDs3, (List<long>) local5, positionOffset3, velocityAtPosition3, localRotation3, locationTag3, num6 != 0, distanceFromSurfacePercMin3, distanceFromSurfacePercMax3, (SpawnRuleOrbit) local6, celestialBodyDeathDistanceMultiplier3, artificialBodyDistanceCheck3);
        newShip4.DockToShip(newShip4.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip3.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip3, true);
        int num7 = 8;
        string registration4 = "";
        long shipID4 = -1;
        List<long> nearArtificialBodyGUIDs4 = new List<long>();
        nearArtificialBodyGUIDs4.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local7 = null;
        Vector3D? positionOffset4 = new Vector3D?();
        Vector3D? velocityAtPosition4 = new Vector3D?();
        QuaternionD? localRotation4 = new QuaternionD?();
        string locationTag4 = this.LocationTag;
        int num8 = 1;
        double distanceFromSurfacePercMin4 = 0.03;
        double distanceFromSurfacePercMax4 = 0.3;
        // ISSUE: variable of the null type
        __Null local8 = null;
        double celestialBodyDeathDistanceMultiplier4 = 1.5;
        double artificialBodyDistanceCheck4 = 100.0;
        Ship newShip5 = Ship.CreateNewShip((GameScenes.SceneID) num7, registration4, shipID4, nearArtificialBodyGUIDs4, (List<long>) local7, positionOffset4, velocityAtPosition4, localRotation4, locationTag4, num8 != 0, distanceFromSurfacePercMin4, distanceFromSurfacePercMax4, (SpawnRuleOrbit) local8, celestialBodyDeathDistanceMultiplier4, artificialBodyDistanceCheck4);
        newShip5.DockToShip(newShip5.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip4.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip4, true);
        int num9 = 9;
        string registration5 = "";
        long shipID5 = -1;
        List<long> nearArtificialBodyGUIDs5 = new List<long>();
        nearArtificialBodyGUIDs5.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local9 = null;
        Vector3D? positionOffset5 = new Vector3D?();
        Vector3D? velocityAtPosition5 = new Vector3D?();
        QuaternionD? localRotation5 = new QuaternionD?();
        string locationTag5 = this.LocationTag;
        int num10 = 1;
        double distanceFromSurfacePercMin5 = 0.03;
        double distanceFromSurfacePercMax5 = 0.3;
        // ISSUE: variable of the null type
        __Null local10 = null;
        double celestialBodyDeathDistanceMultiplier5 = 1.5;
        double artificialBodyDistanceCheck5 = 100.0;
        Ship newShip6 = Ship.CreateNewShip((GameScenes.SceneID) num9, registration5, shipID5, nearArtificialBodyGUIDs5, (List<long>) local9, positionOffset5, velocityAtPosition5, localRotation5, locationTag5, num10 != 0, distanceFromSurfacePercMin5, distanceFromSurfacePercMax5, (SpawnRuleOrbit) local10, celestialBodyDeathDistanceMultiplier5, artificialBodyDistanceCheck5);
        newShip6.DockToShip(newShip6.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 13)), newShip3.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip3, true);
        newShip1.VesselData.VesselRegistration = !this.StationName.IsNullOrEmpty() ? this.StationName : Server.NameGenerator.GenerateStationRegistration();
        this.SpawnedVessels.Add((SpaceObjectVessel) newShip1);
        newShip1.IsArenaVessel = true;
        foreach (VesselDockingPort dockingPort in newShip1.DockingPorts)
          dockingPort.Locked = true;
        foreach (SpaceObjectVessel allDockedVessel in newShip1.AllDockedVessels)
        {
          this.SpawnedVessels.Add(allDockedVessel);
          allDockedVessel.IsArenaVessel = true;
          foreach (VesselDockingPort dockingPort in allDockedVessel.DockingPorts)
            dockingPort.Locked = true;
        }
        this.DistributeLoot(this.SpawnedVessels);
        return (SpaceObjectVessel) newShip1;
      }
      if (this.LocationType == SpawnRuleLocationType.Listening_Post_09)
      {
        Ship newShip1 = Ship.CreateNewShip(GameScenes.SceneID.AltCorp_LifeSupportModule, "", -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), this.LocationTag, true, 0.03, 0.3, this.Orbit, 1.5, SpawnManager.Settings.RandomLocationCheckDistance);
        if (this.IsVisibleOnRadar)
          newShip1.ToggleDistress(true, DistressType.AlwaysVisible);
        int num1 = 5;
        string registration1 = "";
        long shipID1 = -1;
        List<long> nearArtificialBodyGUIDs1 = new List<long>();
        nearArtificialBodyGUIDs1.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local1 = null;
        Vector3D? positionOffset1 = new Vector3D?();
        Vector3D? velocityAtPosition1 = new Vector3D?();
        QuaternionD? localRotation1 = new QuaternionD?();
        string locationTag1 = this.LocationTag;
        int num2 = 1;
        double distanceFromSurfacePercMin1 = 0.03;
        double distanceFromSurfacePercMax1 = 0.3;
        // ISSUE: variable of the null type
        __Null local2 = null;
        double celestialBodyDeathDistanceMultiplier1 = 1.5;
        double artificialBodyDistanceCheck1 = 100.0;
        Ship newShip2 = Ship.CreateNewShip((GameScenes.SceneID) num1, registration1, shipID1, nearArtificialBodyGUIDs1, (List<long>) local1, positionOffset1, velocityAtPosition1, localRotation1, locationTag1, num2 != 0, distanceFromSurfacePercMin1, distanceFromSurfacePercMax1, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier1, artificialBodyDistanceCheck1);
        newShip2.DockToShip(newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip1.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip1, true);
        int num3 = 11;
        string registration2 = "";
        long shipID2 = -1;
        List<long> nearArtificialBodyGUIDs2 = new List<long>();
        nearArtificialBodyGUIDs2.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local3 = null;
        Vector3D? positionOffset2 = new Vector3D?();
        Vector3D? velocityAtPosition2 = new Vector3D?();
        QuaternionD? localRotation2 = new QuaternionD?();
        string locationTag2 = this.LocationTag;
        int num4 = 1;
        double distanceFromSurfacePercMin2 = 0.03;
        double distanceFromSurfacePercMax2 = 0.3;
        // ISSUE: variable of the null type
        __Null local4 = null;
        double celestialBodyDeathDistanceMultiplier2 = 1.5;
        double artificialBodyDistanceCheck2 = 100.0;
        Ship newShip3 = Ship.CreateNewShip((GameScenes.SceneID) num3, registration2, shipID2, nearArtificialBodyGUIDs2, (List<long>) local3, positionOffset2, velocityAtPosition2, localRotation2, locationTag2, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local4, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
        newShip3.DockToShip(newShip3.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip1.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip1, true);
        int num5 = 5;
        string registration3 = "";
        long shipID3 = -1;
        List<long> nearArtificialBodyGUIDs3 = new List<long>();
        nearArtificialBodyGUIDs3.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local5 = null;
        Vector3D? positionOffset3 = new Vector3D?();
        Vector3D? velocityAtPosition3 = new Vector3D?();
        QuaternionD? localRotation3 = new QuaternionD?();
        string locationTag3 = this.LocationTag;
        int num6 = 1;
        double distanceFromSurfacePercMin3 = 0.03;
        double distanceFromSurfacePercMax3 = 0.3;
        // ISSUE: variable of the null type
        __Null local6 = null;
        double celestialBodyDeathDistanceMultiplier3 = 1.5;
        double artificialBodyDistanceCheck3 = 100.0;
        Ship newShip4 = Ship.CreateNewShip((GameScenes.SceneID) num5, registration3, shipID3, nearArtificialBodyGUIDs3, (List<long>) local5, positionOffset3, velocityAtPosition3, localRotation3, locationTag3, num6 != 0, distanceFromSurfacePercMin3, distanceFromSurfacePercMax3, (SpawnRuleOrbit) local6, celestialBodyDeathDistanceMultiplier3, artificialBodyDistanceCheck3);
        newShip4.DockToShip(newShip4.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip3.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 5)), newShip3, true);
        int num7 = 8;
        string registration4 = "";
        long shipID4 = -1;
        List<long> nearArtificialBodyGUIDs4 = new List<long>();
        nearArtificialBodyGUIDs4.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local7 = null;
        Vector3D? positionOffset4 = new Vector3D?();
        Vector3D? velocityAtPosition4 = new Vector3D?();
        QuaternionD? localRotation4 = new QuaternionD?();
        string locationTag4 = this.LocationTag;
        int num8 = 1;
        double distanceFromSurfacePercMin4 = 0.03;
        double distanceFromSurfacePercMax4 = 0.3;
        // ISSUE: variable of the null type
        __Null local8 = null;
        double celestialBodyDeathDistanceMultiplier4 = 1.5;
        double artificialBodyDistanceCheck4 = 100.0;
        Ship newShip5 = Ship.CreateNewShip((GameScenes.SceneID) num7, registration4, shipID4, nearArtificialBodyGUIDs4, (List<long>) local7, positionOffset4, velocityAtPosition4, localRotation4, locationTag4, num8 != 0, distanceFromSurfacePercMin4, distanceFromSurfacePercMax4, (SpawnRuleOrbit) local8, celestialBodyDeathDistanceMultiplier4, artificialBodyDistanceCheck4);
        newShip5.DockToShip(newShip5.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip4.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip4, true);
        int num9 = 3;
        string registration5 = "";
        long shipID5 = -1;
        List<long> nearArtificialBodyGUIDs5 = new List<long>();
        nearArtificialBodyGUIDs5.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local9 = null;
        Vector3D? positionOffset5 = new Vector3D?();
        Vector3D? velocityAtPosition5 = new Vector3D?();
        QuaternionD? localRotation5 = new QuaternionD?();
        string locationTag5 = this.LocationTag;
        int num10 = 1;
        double distanceFromSurfacePercMin5 = 0.03;
        double distanceFromSurfacePercMax5 = 0.3;
        // ISSUE: variable of the null type
        __Null local10 = null;
        double celestialBodyDeathDistanceMultiplier5 = 1.5;
        double artificialBodyDistanceCheck5 = 100.0;
        Ship newShip6 = Ship.CreateNewShip((GameScenes.SceneID) num9, registration5, shipID5, nearArtificialBodyGUIDs5, (List<long>) local9, positionOffset5, velocityAtPosition5, localRotation5, locationTag5, num10 != 0, distanceFromSurfacePercMin5, distanceFromSurfacePercMax5, (SpawnRuleOrbit) local10, celestialBodyDeathDistanceMultiplier5, artificialBodyDistanceCheck5);
        newShip6.DockToShip(newShip6.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip3.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 6)), newShip3, true);
        int num11 = 8;
        string registration6 = "";
        long shipID6 = -1;
        List<long> nearArtificialBodyGUIDs6 = new List<long>();
        nearArtificialBodyGUIDs6.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local11 = null;
        Vector3D? positionOffset6 = new Vector3D?();
        Vector3D? velocityAtPosition6 = new Vector3D?();
        QuaternionD? localRotation6 = new QuaternionD?();
        string locationTag6 = this.LocationTag;
        int num12 = 1;
        double distanceFromSurfacePercMin6 = 0.03;
        double distanceFromSurfacePercMax6 = 0.3;
        // ISSUE: variable of the null type
        __Null local12 = null;
        double celestialBodyDeathDistanceMultiplier6 = 1.5;
        double artificialBodyDistanceCheck6 = 100.0;
        Ship newShip7 = Ship.CreateNewShip((GameScenes.SceneID) num11, registration6, shipID6, nearArtificialBodyGUIDs6, (List<long>) local11, positionOffset6, velocityAtPosition6, localRotation6, locationTag6, num12 != 0, distanceFromSurfacePercMin6, distanceFromSurfacePercMax6, (SpawnRuleOrbit) local12, celestialBodyDeathDistanceMultiplier6, artificialBodyDistanceCheck6);
        newShip7.DockToShip(newShip7.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip6.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip6, true);
        newShip1.VesselData.VesselRegistration = !this.StationName.IsNullOrEmpty() ? this.StationName : Server.NameGenerator.GenerateStationRegistration();
        this.SpawnedVessels.Add((SpaceObjectVessel) newShip1);
        newShip1.IsArenaVessel = true;
        foreach (VesselDockingPort dockingPort in newShip1.DockingPorts)
          dockingPort.Locked = true;
        foreach (SpaceObjectVessel allDockedVessel in newShip1.AllDockedVessels)
        {
          this.SpawnedVessels.Add(allDockedVessel);
          allDockedVessel.IsArenaVessel = true;
          foreach (VesselDockingPort dockingPort in allDockedVessel.DockingPorts)
            dockingPort.Locked = true;
        }
        this.DistributeLoot(this.SpawnedVessels);
        return (SpaceObjectVessel) newShip1;
      }
      if (this.LocationType == SpawnRuleLocationType.Emergency_Staging_Post_D8)
      {
        Ship newShip1 = Ship.CreateNewShip(GameScenes.SceneID.AltCorp_LifeSupportModule, "", -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), this.LocationTag, true, 0.03, 0.3, this.Orbit, 1.5, SpawnManager.Settings.RandomLocationCheckDistance);
        if (this.IsVisibleOnRadar)
          newShip1.ToggleDistress(true, DistressType.AlwaysVisible);
        int num1 = 4;
        string registration1 = "";
        long shipID1 = -1;
        List<long> nearArtificialBodyGUIDs1 = new List<long>();
        nearArtificialBodyGUIDs1.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local1 = null;
        Vector3D? positionOffset1 = new Vector3D?();
        Vector3D? velocityAtPosition1 = new Vector3D?();
        QuaternionD? localRotation1 = new QuaternionD?();
        string locationTag1 = this.LocationTag;
        int num2 = 1;
        double distanceFromSurfacePercMin1 = 0.03;
        double distanceFromSurfacePercMax1 = 0.3;
        // ISSUE: variable of the null type
        __Null local2 = null;
        double celestialBodyDeathDistanceMultiplier1 = 1.5;
        double artificialBodyDistanceCheck1 = 100.0;
        Ship newShip2 = Ship.CreateNewShip((GameScenes.SceneID) num1, registration1, shipID1, nearArtificialBodyGUIDs1, (List<long>) local1, positionOffset1, velocityAtPosition1, localRotation1, locationTag1, num2 != 0, distanceFromSurfacePercMin1, distanceFromSurfacePercMax1, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier1, artificialBodyDistanceCheck1);
        newShip2.DockToShip(newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 5)), newShip1.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip1, true);
        int num3 = 11;
        string registration2 = "";
        long shipID2 = -1;
        List<long> nearArtificialBodyGUIDs2 = new List<long>();
        nearArtificialBodyGUIDs2.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local3 = null;
        Vector3D? positionOffset2 = new Vector3D?();
        Vector3D? velocityAtPosition2 = new Vector3D?();
        QuaternionD? localRotation2 = new QuaternionD?();
        string locationTag2 = this.LocationTag;
        int num4 = 1;
        double distanceFromSurfacePercMin2 = 0.03;
        double distanceFromSurfacePercMax2 = 0.3;
        // ISSUE: variable of the null type
        __Null local4 = null;
        double celestialBodyDeathDistanceMultiplier2 = 1.5;
        double artificialBodyDistanceCheck2 = 100.0;
        Ship newShip3 = Ship.CreateNewShip((GameScenes.SceneID) num3, registration2, shipID2, nearArtificialBodyGUIDs2, (List<long>) local3, positionOffset2, velocityAtPosition2, localRotation2, locationTag2, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local4, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
        newShip3.DockToShip(newShip3.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip2, true);
        int num5 = 5;
        string registration3 = "";
        long shipID3 = -1;
        List<long> nearArtificialBodyGUIDs3 = new List<long>();
        nearArtificialBodyGUIDs3.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local5 = null;
        Vector3D? positionOffset3 = new Vector3D?();
        Vector3D? velocityAtPosition3 = new Vector3D?();
        QuaternionD? localRotation3 = new QuaternionD?();
        string locationTag3 = this.LocationTag;
        int num6 = 1;
        double distanceFromSurfacePercMin3 = 0.03;
        double distanceFromSurfacePercMax3 = 0.3;
        // ISSUE: variable of the null type
        __Null local6 = null;
        double celestialBodyDeathDistanceMultiplier3 = 1.5;
        double artificialBodyDistanceCheck3 = 100.0;
        Ship newShip4 = Ship.CreateNewShip((GameScenes.SceneID) num5, registration3, shipID3, nearArtificialBodyGUIDs3, (List<long>) local5, positionOffset3, velocityAtPosition3, localRotation3, locationTag3, num6 != 0, distanceFromSurfacePercMin3, distanceFromSurfacePercMax3, (SpawnRuleOrbit) local6, celestialBodyDeathDistanceMultiplier3, artificialBodyDistanceCheck3);
        newShip4.DockToShip(newShip4.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip3.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 6)), newShip3, true);
        int num7 = 12;
        string registration4 = "";
        long shipID4 = -1;
        List<long> nearArtificialBodyGUIDs4 = new List<long>();
        nearArtificialBodyGUIDs4.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local7 = null;
        Vector3D? positionOffset4 = new Vector3D?();
        Vector3D? velocityAtPosition4 = new Vector3D?();
        QuaternionD? localRotation4 = new QuaternionD?();
        string locationTag4 = this.LocationTag;
        int num8 = 1;
        double distanceFromSurfacePercMin4 = 0.03;
        double distanceFromSurfacePercMax4 = 0.3;
        // ISSUE: variable of the null type
        __Null local8 = null;
        double celestialBodyDeathDistanceMultiplier4 = 1.5;
        double artificialBodyDistanceCheck4 = 100.0;
        Ship newShip5 = Ship.CreateNewShip((GameScenes.SceneID) num7, registration4, shipID4, nearArtificialBodyGUIDs4, (List<long>) local7, positionOffset4, velocityAtPosition4, localRotation4, locationTag4, num8 != 0, distanceFromSurfacePercMin4, distanceFromSurfacePercMax4, (SpawnRuleOrbit) local8, celestialBodyDeathDistanceMultiplier4, artificialBodyDistanceCheck4);
        newShip5.DockToShip(newShip5.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip3.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 5)), newShip3, true);
        int num9 = 8;
        string registration5 = "";
        long shipID5 = -1;
        List<long> nearArtificialBodyGUIDs5 = new List<long>();
        nearArtificialBodyGUIDs5.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local9 = null;
        Vector3D? positionOffset5 = new Vector3D?();
        Vector3D? velocityAtPosition5 = new Vector3D?();
        QuaternionD? localRotation5 = new QuaternionD?();
        string locationTag5 = this.LocationTag;
        int num10 = 1;
        double distanceFromSurfacePercMin5 = 0.03;
        double distanceFromSurfacePercMax5 = 0.3;
        // ISSUE: variable of the null type
        __Null local10 = null;
        double celestialBodyDeathDistanceMultiplier5 = 1.5;
        double artificialBodyDistanceCheck5 = 100.0;
        Ship newShip6 = Ship.CreateNewShip((GameScenes.SceneID) num9, registration5, shipID5, nearArtificialBodyGUIDs5, (List<long>) local9, positionOffset5, velocityAtPosition5, localRotation5, locationTag5, num10 != 0, distanceFromSurfacePercMin5, distanceFromSurfacePercMax5, (SpawnRuleOrbit) local10, celestialBodyDeathDistanceMultiplier5, artificialBodyDistanceCheck5);
        newShip6.DockToShip(newShip6.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip2, true);
        newShip1.VesselData.VesselRegistration = !this.StationName.IsNullOrEmpty() ? this.StationName : Server.NameGenerator.GenerateStationRegistration();
        this.SpawnedVessels.Add((SpaceObjectVessel) newShip1);
        newShip1.IsArenaVessel = true;
        foreach (VesselDockingPort dockingPort in newShip1.DockingPorts)
          dockingPort.Locked = true;
        foreach (SpaceObjectVessel allDockedVessel in newShip1.AllDockedVessels)
        {
          this.SpawnedVessels.Add(allDockedVessel);
          allDockedVessel.IsArenaVessel = true;
          foreach (VesselDockingPort dockingPort in allDockedVessel.DockingPorts)
            dockingPort.Locked = true;
        }
        this.DistributeLoot(this.SpawnedVessels);
        return (SpaceObjectVessel) newShip1;
      }
      if (this.LocationType != SpawnRuleLocationType.Auxiliary_Plant_02)
        return (SpaceObjectVessel) null;
      Ship newShip9 = Ship.CreateNewShip(GameScenes.SceneID.AltCorp_LifeSupportModule, "", -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), this.LocationTag, true, 0.03, 0.3, this.Orbit, 1.5, SpawnManager.Settings.RandomLocationCheckDistance);
      if (this.IsVisibleOnRadar)
        newShip9.ToggleDistress(true, DistressType.AlwaysVisible);
      int num15 = 7;
      string registration8 = "";
      long shipID8 = -1;
      List<long> nearArtificialBodyGUIDs8 = new List<long>();
      nearArtificialBodyGUIDs8.Add(newShip9.GUID);
      // ISSUE: variable of the null type
      __Null local15 = null;
      Vector3D? positionOffset8 = new Vector3D?();
      Vector3D? velocityAtPosition8 = new Vector3D?();
      QuaternionD? localRotation8 = new QuaternionD?();
      string locationTag8 = this.LocationTag;
      int num16 = 1;
      double distanceFromSurfacePercMin8 = 0.03;
      double distanceFromSurfacePercMax8 = 0.3;
      // ISSUE: variable of the null type
      __Null local16 = null;
      double celestialBodyDeathDistanceMultiplier8 = 1.5;
      double artificialBodyDistanceCheck8 = 100.0;
      Ship newShip10 = Ship.CreateNewShip((GameScenes.SceneID) num15, registration8, shipID8, nearArtificialBodyGUIDs8, (List<long>) local15, positionOffset8, velocityAtPosition8, localRotation8, locationTag8, num16 != 0, distanceFromSurfacePercMin8, distanceFromSurfacePercMax8, (SpawnRuleOrbit) local16, celestialBodyDeathDistanceMultiplier8, artificialBodyDistanceCheck8);
      newShip10.DockToShip(newShip10.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip9.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip9, true);
      int num17 = 11;
      string registration9 = "";
      long shipID9 = -1;
      List<long> nearArtificialBodyGUIDs9 = new List<long>();
      nearArtificialBodyGUIDs9.Add(newShip9.GUID);
      // ISSUE: variable of the null type
      __Null local17 = null;
      Vector3D? positionOffset9 = new Vector3D?();
      Vector3D? velocityAtPosition9 = new Vector3D?();
      QuaternionD? localRotation9 = new QuaternionD?();
      string locationTag9 = this.LocationTag;
      int num18 = 1;
      double distanceFromSurfacePercMin9 = 0.03;
      double distanceFromSurfacePercMax9 = 0.3;
      // ISSUE: variable of the null type
      __Null local18 = null;
      double celestialBodyDeathDistanceMultiplier9 = 1.5;
      double artificialBodyDistanceCheck9 = 100.0;
      Ship newShip11 = Ship.CreateNewShip((GameScenes.SceneID) num17, registration9, shipID9, nearArtificialBodyGUIDs9, (List<long>) local17, positionOffset9, velocityAtPosition9, localRotation9, locationTag9, num18 != 0, distanceFromSurfacePercMin9, distanceFromSurfacePercMax9, (SpawnRuleOrbit) local18, celestialBodyDeathDistanceMultiplier9, artificialBodyDistanceCheck9);
      newShip11.DockToShip(newShip11.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip9.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip9, true);
      int num19 = 10;
      string registration10 = "";
      long shipID10 = -1;
      List<long> nearArtificialBodyGUIDs10 = new List<long>();
      nearArtificialBodyGUIDs10.Add(newShip9.GUID);
      // ISSUE: variable of the null type
      __Null local19 = null;
      Vector3D? positionOffset10 = new Vector3D?();
      Vector3D? velocityAtPosition10 = new Vector3D?();
      QuaternionD? localRotation10 = new QuaternionD?();
      string locationTag10 = this.LocationTag;
      int num20 = 1;
      double distanceFromSurfacePercMin10 = 0.03;
      double distanceFromSurfacePercMax10 = 0.3;
      // ISSUE: variable of the null type
      __Null local20 = null;
      double celestialBodyDeathDistanceMultiplier10 = 1.5;
      double artificialBodyDistanceCheck10 = 100.0;
      Ship newShip12 = Ship.CreateNewShip((GameScenes.SceneID) num19, registration10, shipID10, nearArtificialBodyGUIDs10, (List<long>) local19, positionOffset10, velocityAtPosition10, localRotation10, locationTag10, num20 != 0, distanceFromSurfacePercMin10, distanceFromSurfacePercMax10, (SpawnRuleOrbit) local20, celestialBodyDeathDistanceMultiplier10, artificialBodyDistanceCheck10);
      newShip12.DockToShip(newShip12.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip11.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 5)), newShip11, true);
      int num21 = 7;
      string registration11 = "";
      long shipID11 = -1;
      List<long> nearArtificialBodyGUIDs11 = new List<long>();
      nearArtificialBodyGUIDs11.Add(newShip9.GUID);
      // ISSUE: variable of the null type
      __Null local21 = null;
      Vector3D? positionOffset11 = new Vector3D?();
      Vector3D? velocityAtPosition11 = new Vector3D?();
      QuaternionD? localRotation11 = new QuaternionD?();
      string locationTag11 = this.LocationTag;
      int num22 = 1;
      double distanceFromSurfacePercMin11 = 0.03;
      double distanceFromSurfacePercMax11 = 0.3;
      // ISSUE: variable of the null type
      __Null local22 = null;
      double celestialBodyDeathDistanceMultiplier11 = 1.5;
      double artificialBodyDistanceCheck11 = 100.0;
      Ship newShip13 = Ship.CreateNewShip((GameScenes.SceneID) num21, registration11, shipID11, nearArtificialBodyGUIDs11, (List<long>) local21, positionOffset11, velocityAtPosition11, localRotation11, locationTag11, num22 != 0, distanceFromSurfacePercMin11, distanceFromSurfacePercMax11, (SpawnRuleOrbit) local22, celestialBodyDeathDistanceMultiplier11, artificialBodyDistanceCheck11);
      newShip13.DockToShip(newShip13.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip12.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 2)), newShip12, true);
      newShip9.VesselData.VesselRegistration = !this.StationName.IsNullOrEmpty() ? this.StationName : Server.NameGenerator.GenerateStationRegistration();
      this.SpawnedVessels.Add((SpaceObjectVessel) newShip9);
      newShip9.IsArenaVessel = true;
      foreach (VesselDockingPort dockingPort in newShip9.DockingPorts)
        dockingPort.Locked = true;
      foreach (SpaceObjectVessel allDockedVessel in newShip9.AllDockedVessels)
      {
        this.SpawnedVessels.Add(allDockedVessel);
        allDockedVessel.IsArenaVessel = true;
        foreach (VesselDockingPort dockingPort in allDockedVessel.DockingPorts)
          dockingPort.Locked = true;
      }
      this.DistributeLoot(this.SpawnedVessels);
      return (SpaceObjectVessel) newShip9;
    }

    private SpaceObjectVessel ExecuteStaringRule()
    {
      if (this.LocationType != SpawnRuleLocationType.StartingScene)
        return (SpaceObjectVessel) null;
      long num1 = GUIDFactory.NextLongRandom(1L, long.MaxValue);
      QuaternionD quaternionD = QuaternionD.Euler(0.0, MathHelper.RandomRange(0.0, 359.99), 0.0);
      List<SpaceObjectVessel> vessels = new List<SpaceObjectVessel>();
      Ship newShip1 = Ship.CreateNewShip(GameScenes.SceneID.AltCorp_StartingModule, "", -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(quaternionD), this.LocationTag, true, 0.03, 0.3, this.Orbit, 1.5, SpawnManager.Settings.StartingLocationCheckDistance);
      newShip1.StartingSetId = num1;
      int num2 = 22;
      string registration1 = "";
      long shipID1 = -1;
      List<long> nearArtificialBodyGUIDs1 = new List<long>();
      nearArtificialBodyGUIDs1.Add(newShip1.GUID);
      // ISSUE: variable of the null type
      __Null local1 = null;
      Vector3D? positionOffset1 = new Vector3D?();
      Vector3D? velocityAtPosition1 = new Vector3D?();
      QuaternionD? localRotation1 = new QuaternionD?();
      string locationTag1 = this.LocationTag;
      int num3 = 0;
      double distanceFromSurfacePercMin1 = 0.03;
      double distanceFromSurfacePercMax1 = 0.3;
      // ISSUE: variable of the null type
      __Null local2 = null;
      double celestialBodyDeathDistanceMultiplier1 = 1.5;
      double artificialBodyDistanceCheck1 = 100.0;
      Ship newShip2 = Ship.CreateNewShip((GameScenes.SceneID) num2, registration1, shipID1, nearArtificialBodyGUIDs1, (List<long>) local1, positionOffset1, velocityAtPosition1, localRotation1, locationTag1, num3 != 0, distanceFromSurfacePercMin1, distanceFromSurfacePercMax1, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier1, artificialBodyDistanceCheck1);
      newShip2.StartingSetId = num1;
      newShip2.DockToShip(newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 2)), newShip1.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip1, true);
      int num4 = 6;
      string registration2 = "";
      long shipID2 = -1;
      List<long> nearArtificialBodyGUIDs2 = new List<long>();
      nearArtificialBodyGUIDs2.Add(newShip1.GUID);
      // ISSUE: variable of the null type
      __Null local3 = null;
      Vector3D? positionOffset2 = new Vector3D?(quaternionD * new Vector3D(-73.0, 14.0, -39.0));
      Vector3D? velocityAtPosition2 = new Vector3D?();
      QuaternionD? localRotation2 = new QuaternionD?(quaternionD * QuaternionD.Euler(0.0, 63.0, 0.0));
      string locationTag2 = this.LocationTag;
      int num5 = 0;
      double distanceFromSurfacePercMin2 = 0.03;
      double distanceFromSurfacePercMax2 = 0.3;
      // ISSUE: variable of the null type
      __Null local4 = null;
      double celestialBodyDeathDistanceMultiplier2 = 1.5;
      double artificialBodyDistanceCheck2 = 100.0;
      Ship newShip3 = Ship.CreateNewShip((GameScenes.SceneID) num4, registration2, shipID2, nearArtificialBodyGUIDs2, (List<long>) local3, positionOffset2, velocityAtPosition2, localRotation2, locationTag2, num5 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local4, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
      newShip3.StartingSetId = num1;
      newShip3.StabilizeToTarget((SpaceObjectVessel) newShip1, true);
      int num6 = 14;
      string registration3 = "";
      long shipID3 = -1;
      List<long> nearArtificialBodyGUIDs3 = new List<long>();
      nearArtificialBodyGUIDs3.Add(newShip1.GUID);
      // ISSUE: variable of the null type
      __Null local5 = null;
      Vector3D? positionOffset3 = new Vector3D?(quaternionD * new Vector3D(39.0, 0.0, 5.5));
      Vector3D? velocityAtPosition3 = new Vector3D?();
      QuaternionD? localRotation3 = new QuaternionD?(quaternionD * QuaternionD.Euler(0.0, -90.0, 0.0));
      string locationTag3 = this.LocationTag;
      int num7 = 0;
      double distanceFromSurfacePercMin3 = 0.03;
      double distanceFromSurfacePercMax3 = 0.3;
      // ISSUE: variable of the null type
      __Null local6 = null;
      double celestialBodyDeathDistanceMultiplier3 = 1.5;
      double artificialBodyDistanceCheck3 = 100.0;
      Ship newShip4 = Ship.CreateNewShip((GameScenes.SceneID) num6, registration3, shipID3, nearArtificialBodyGUIDs3, (List<long>) local5, positionOffset3, velocityAtPosition3, localRotation3, locationTag3, num7 != 0, distanceFromSurfacePercMin3, distanceFromSurfacePercMax3, (SpawnRuleOrbit) local6, celestialBodyDeathDistanceMultiplier3, artificialBodyDistanceCheck3);
      newShip4.StartingSetId = num1;
      newShip4.StabilizeToTarget((SpaceObjectVessel) newShip1, true);
      int num8 = 15;
      string registration4 = "";
      long shipID4 = -1;
      List<long> nearArtificialBodyGUIDs4 = new List<long>();
      nearArtificialBodyGUIDs4.Add(newShip1.GUID);
      // ISSUE: variable of the null type
      __Null local7 = null;
      Vector3D? positionOffset4 = new Vector3D?(quaternionD * new Vector3D(57.0, -9.0, 31.0));
      Vector3D? velocityAtPosition4 = new Vector3D?();
      QuaternionD? localRotation4 = new QuaternionD?(quaternionD * QuaternionD.Euler(0.0, -32.0, 0.0));
      string locationTag4 = this.LocationTag;
      int num9 = 0;
      double distanceFromSurfacePercMin4 = 0.03;
      double distanceFromSurfacePercMax4 = 0.3;
      // ISSUE: variable of the null type
      __Null local8 = null;
      double celestialBodyDeathDistanceMultiplier4 = 1.5;
      double artificialBodyDistanceCheck4 = 100.0;
      Ship newShip5 = Ship.CreateNewShip((GameScenes.SceneID) num8, registration4, shipID4, nearArtificialBodyGUIDs4, (List<long>) local7, positionOffset4, velocityAtPosition4, localRotation4, locationTag4, num9 != 0, distanceFromSurfacePercMin4, distanceFromSurfacePercMax4, (SpawnRuleOrbit) local8, celestialBodyDeathDistanceMultiplier4, artificialBodyDistanceCheck4);
      newShip5.StartingSetId = num1;
      newShip5.StabilizeToTarget((SpaceObjectVessel) newShip1, true);
      vessels.Add((SpaceObjectVessel) newShip1);
      vessels.Add((SpaceObjectVessel) newShip2);
      vessels.Add((SpaceObjectVessel) newShip3);
      vessels.Add((SpaceObjectVessel) newShip4);
      vessels.Add((SpaceObjectVessel) newShip5);
      this.DistributeLoot(vessels);
      return (SpaceObjectVessel) newShip2;
    }

    public SpaceObjectVessel ExecuteRule(bool isInitialize)
    {
      switch (this.LocationType)
      {
        case SpawnRuleLocationType.Random:
          return this.ExecuterRandomRule();
        case SpawnRuleLocationType.StartingScene:
          return this.ExecuteStaringRule();
        case SpawnRuleLocationType.Automated_Refinery_B7:
        case SpawnRuleLocationType.Waystation_H4:
        case SpawnRuleLocationType.Emergency_Supply_Post_A3:
        case SpawnRuleLocationType.Listening_Post_09:
        case SpawnRuleLocationType.Emergency_Staging_Post_D8:
        case SpawnRuleLocationType.Auxiliary_Plant_02:
          return this.ExecuterStationRule(isInitialize);
        default:
          return (SpaceObjectVessel) null;
      }
    }

    public void DistributeLoot(List<SpaceObjectVessel> vessels)
    {
      List<SpawnRuleLoot> spawnRuleLootList = new List<SpawnRuleLoot>();
      foreach (SpawnRuleLoot loot in this.LootPool)
      {
        if (loot.Count > 0)
        {
          int num1;
          if (this.LocationType == SpawnRuleLocationType.Random)
          {
            int num2 = loot.CountMax / this.NumberOfClusters.Max;
            num1 = MathHelper.RandomRange(num2 / 2, num2 + 1);
            if (num1 > loot.Count)
              num1 = loot.Count;
          }
          else
            num1 = loot.Count;
          int num3 = 0;
          while (num3 < num1)
          {
            VesselAttachPoint lootAttachPoint = SpawnManager.GetLootAttachPoint(loot.Data, ref vessels);
            if (lootAttachPoint != null)
            {
              try
              {
                if (SpawnManager.SpawnDynamicObject(this, loot, lootAttachPoint))
                  ++num3;
              }
              catch (Exception ex)
              {
                Dbg.Warning(ex.Message);
                spawnRuleLootList.Add(loot);
                --num1;
              }
            }
            else
              break;
          }
          if (this.LocationType != SpawnRuleLocationType.StartingScene)
            loot.Count -= num3;
        }
      }
      foreach (SpawnRuleLoot spawnRuleLoot in spawnRuleLootList)
        this.LootPool.Remove(spawnRuleLoot);
    }

    public bool RemoveDynamicObject(DynamicObject dobj, SpawnRuleLoot loot)
    {
      ++loot.Count;
      return true;
    }

    public bool RemoveSpaceObjectVessel(SpaceObjectVessel ves, SpawnRuleScene srs, int count)
    {
      this.removeVesselFromCluster(count);
      ++srs.Count;
      this.SpawnedVessels.RemoveAll((Predicate<SpaceObjectVessel>) (m => m == ves));
      --this.totalScenes.Min;
      if (ves is Ship)
        (ves as Ship).IsPartOfSpawnSystem = false;
      return true;
    }
  }
}
