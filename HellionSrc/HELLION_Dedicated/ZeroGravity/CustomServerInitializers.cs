// Decompiled with JetBrains decompiler
// Type: ZeroGravity.CustomServerInitializers
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Objects;
using ZeroGravity.ShipComponents;
using ZeroGravity.Spawn;

namespace ZeroGravity
{
  internal static class CustomServerInitializers
  {
    public static List<GameScenes.SceneID> shipsToSpawnForTestStarting = new List<GameScenes.SceneID>()
    {
      GameScenes.SceneID.AltCorp_AirLock,
      GameScenes.SceneID.AltCorp_Cargo_Module,
      GameScenes.SceneID.AltCorp_Command_Module,
      GameScenes.SceneID.AltCorp_Corridor45TurnModule,
      GameScenes.SceneID.AltCorp_Corridor45TurnRightModule,
      GameScenes.SceneID.AltCorp_CorridorIntersectionModule,
      GameScenes.SceneID.AltCorp_CorridorModule,
      GameScenes.SceneID.AltCorp_CorridorVertical,
      GameScenes.SceneID.AltCorp_DockableContainer,
      GameScenes.SceneID.AltCorp_LifeSupportModule,
      GameScenes.SceneID.ALtCorp_PowerSupply_Module,
      GameScenes.SceneID.AltCorp_Shuttle_SARA,
      GameScenes.SceneID.Generic_Debris_Corridor001,
      GameScenes.SceneID.Generic_Debris_Corridor002,
      GameScenes.SceneID.Generic_Debris_JuncRoom001,
      GameScenes.SceneID.Generic_Debris_JuncRoom002,
      GameScenes.SceneID.AltCorp_CrewQuarters_Module,
      GameScenes.SceneID.Generic_Debris_Outpost001
    };
    public static List<GameScenes.SceneID> shipsToSpawnForDoomed = new List<GameScenes.SceneID>()
    {
      GameScenes.SceneID.AltCorp_AirLock,
      GameScenes.SceneID.AltCorp_Cargo_Module,
      GameScenes.SceneID.AltCorp_Command_Module,
      GameScenes.SceneID.AltCorp_Corridor45TurnModule,
      GameScenes.SceneID.AltCorp_Corridor45TurnRightModule,
      GameScenes.SceneID.AltCorp_CorridorIntersectionModule,
      GameScenes.SceneID.AltCorp_CorridorModule,
      GameScenes.SceneID.AltCorp_CorridorVertical,
      GameScenes.SceneID.AltCorp_DockableContainer,
      GameScenes.SceneID.AltCorp_LifeSupportModule,
      GameScenes.SceneID.ALtCorp_PowerSupply_Module,
      GameScenes.SceneID.AltCorp_Shuttle_SARA,
      GameScenes.SceneID.Generic_Debris_Corridor001,
      GameScenes.SceneID.Generic_Debris_JuncRoom001,
      GameScenes.SceneID.Generic_Debris_JuncRoom002,
      GameScenes.SceneID.AltCorp_CrewQuarters_Module,
      GameScenes.SceneID.Generic_Debris_Outpost001
    };

    public static bool AutoInitializeWorld(Server.ServerSetupType setupType)
    {
      return setupType == Server.ServerSetupType.RadeTesting || setupType == Server.ServerSetupType.xXx_DimasGreatServerSetupGG_xXx || setupType == Server.ServerSetupType.VujaTesting || setupType == Server.ServerSetupType.Dimbe;
    }

    public static void WorldInitialize(Server server, Server.ServerSetupType setupType)
    {
      if (setupType == Server.ServerSetupType.RadeTesting)
        CustomServerInitializers.Rade_WorldInitialize(server);
      else if (setupType == Server.ServerSetupType.SpawnForShomy)
        CustomServerInitializers.Shomy_WorldInitialize(server);
      else if (setupType == Server.ServerSetupType.ZareTesting)
        CustomServerInitializers.Zare_WorldInitialize(server);
      else if (setupType == Server.ServerSetupType.xXx_DimasGreatServerSetupGG_xXx)
        CustomServerInitializers.Dima_WorldInitialize(server);
      else if (setupType == Server.ServerSetupType.VujaTesting)
        CustomServerInitializers.Vuja_WorldInitialize(server);
      else if (setupType == Server.ServerSetupType.Test)
        CustomServerInitializers.Test_WorldInitialize(server);
      else if (setupType == Server.ServerSetupType.MarioTesting)
      {
        CustomServerInitializers.Mario_WorldInitialize(server);
      }
      else
      {
        if (setupType != Server.ServerSetupType.Dimbe)
          return;
        CustomServerInitializers.Dimbe_WorldInitialize(server);
      }
    }

    public static Ship SpawnStartingModule(Server server, Server.ServerSetupType setupType)
    {
      if (setupType == Server.ServerSetupType.RadeTesting)
        return CustomServerInitializers.Rade_SpawnStartingModule(server);
      if (setupType == Server.ServerSetupType.SpawnForShomy)
        return CustomServerInitializers.Shomy_SpawnStaringModule(server);
      if (setupType == Server.ServerSetupType.ZareTesting)
        return CustomServerInitializers.Zare_SpawnStaringModule(server);
      if (setupType == Server.ServerSetupType.xXx_DimasGreatServerSetupGG_xXx)
        return CustomServerInitializers.Dima_SpawnStaringModule(server);
      if (setupType == Server.ServerSetupType.VujaTesting)
        return CustomServerInitializers.Vuja_SpawnStaringModule(server);
      if (setupType == Server.ServerSetupType.Test)
        return CustomServerInitializers.Test_SpawnStaringModule(server);
      if (setupType == Server.ServerSetupType.MarioTesting)
        return CustomServerInitializers.Mario_SpawnStaringModule(server);
      if (setupType == Server.ServerSetupType.Dimbe)
        return CustomServerInitializers.Dimbe_SpawnStaringModule(server);
      return (Ship) null;
    }

    private static void Rade_WorldInitialize(Server server)
    {
      int num1 = MathHelper.RandomRange(1000, 1008);
      string registration1 = "MOVE ASTAL";
      long asteroidGUID = -1;
      // ISSUE: variable of the null type
      __Null local1 = null;
      List<long> celestialBodyGUIDs1 = new List<long>();
      celestialBodyGUIDs1.Add(14L);
      Vector3D? positionOffset1 = new Vector3D?();
      Vector3D? velocityAtPosition1 = new Vector3D?();
      QuaternionD? localRotation1 = new QuaternionD?(MathHelper.RandomRotation());
      int num2 = 1;
      double distanceFromSurfacePercMin1 = 0.03;
      double distanceFromSurfacePercMax1 = 0.3;
      // ISSUE: variable of the null type
      __Null local2 = null;
      double celestialBodyDeathDistanceMultiplier1 = 1.5;
      double artificialBodyDistanceCheck1 = 100.0;
      Asteroid newAsteroid = Asteroid.CreateNewAsteroid((GameScenes.SceneID) num1, registration1, asteroidGUID, (List<long>) local1, celestialBodyGUIDs1, positionOffset1, velocityAtPosition1, localRotation1, num2 != 0, distanceFromSurfacePercMin1, distanceFromSurfacePercMax1, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier1, artificialBodyDistanceCheck1);
      int num3 = 6;
      string registration2 = "SARA SINGLE";
      long shipID1 = -1;
      List<long> nearArtificialBodyGUIDs1 = new List<long>();
      nearArtificialBodyGUIDs1.Add(newAsteroid.GUID);
      List<long> celestialBodyGUIDs2 = new List<long>();
      celestialBodyGUIDs2.Add(14L);
      Vector3D? positionOffset2 = new Vector3D?();
      Vector3D? velocityAtPosition2 = new Vector3D?();
      QuaternionD? localRotation2 = new QuaternionD?(MathHelper.RandomRotation());
      string vesselTag1 = "StartingScene;";
      int num4 = 1;
      double distanceFromSurfacePercMin2 = 0.03;
      double distanceFromSurfacePercMax2 = 0.3;
      // ISSUE: variable of the null type
      __Null local3 = null;
      double celestialBodyDeathDistanceMultiplier2 = 1.5;
      double artificialBodyDistanceCheck2 = 100.0;
      Ship.CreateNewShip((GameScenes.SceneID) num3, registration2, shipID1, nearArtificialBodyGUIDs1, celestialBodyGUIDs2, positionOffset2, velocityAtPosition2, localRotation2, vesselTag1, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local3, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
      int num5 = 6;
      string registration3 = "SARA DOCKED MASTER";
      long shipID2 = -1;
      List<long> nearArtificialBodyGUIDs2 = new List<long>();
      nearArtificialBodyGUIDs2.Add(newAsteroid.GUID);
      List<long> celestialBodyGUIDs3 = new List<long>();
      celestialBodyGUIDs3.Add(14L);
      Vector3D? positionOffset3 = new Vector3D?();
      Vector3D? velocityAtPosition3 = new Vector3D?();
      QuaternionD? localRotation3 = new QuaternionD?(MathHelper.RandomRotation());
      string vesselTag2 = "StartingScene;";
      int num6 = 1;
      double distanceFromSurfacePercMin3 = 0.03;
      double distanceFromSurfacePercMax3 = 0.3;
      // ISSUE: variable of the null type
      __Null local4 = null;
      double celestialBodyDeathDistanceMultiplier3 = 1.5;
      double artificialBodyDistanceCheck3 = 100.0;
      Ship newShip1 = Ship.CreateNewShip((GameScenes.SceneID) num5, registration3, shipID2, nearArtificialBodyGUIDs2, celestialBodyGUIDs3, positionOffset3, velocityAtPosition3, localRotation3, vesselTag2, num6 != 0, distanceFromSurfacePercMin3, distanceFromSurfacePercMax3, (SpawnRuleOrbit) local4, celestialBodyDeathDistanceMultiplier3, artificialBodyDistanceCheck3);
      int num7 = 14;
      string registration4 = "SARA DOCKED SLAVE";
      long shipID3 = -1;
      List<long> nearArtificialBodyGUIDs3 = new List<long>();
      nearArtificialBodyGUIDs3.Add(newAsteroid.GUID);
      List<long> celestialBodyGUIDs4 = new List<long>();
      celestialBodyGUIDs4.Add(14L);
      Vector3D? positionOffset4 = new Vector3D?();
      Vector3D? velocityAtPosition4 = new Vector3D?();
      QuaternionD? localRotation4 = new QuaternionD?(MathHelper.RandomRotation());
      string vesselTag3 = "StartingScene;";
      int num8 = 1;
      double distanceFromSurfacePercMin4 = 0.03;
      double distanceFromSurfacePercMax4 = 0.3;
      // ISSUE: variable of the null type
      __Null local5 = null;
      double celestialBodyDeathDistanceMultiplier4 = 1.5;
      double artificialBodyDistanceCheck4 = 100.0;
      Ship newShip2 = Ship.CreateNewShip((GameScenes.SceneID) num7, registration4, shipID3, nearArtificialBodyGUIDs3, celestialBodyGUIDs4, positionOffset4, velocityAtPosition4, localRotation4, vesselTag3, num8 != 0, distanceFromSurfacePercMin4, distanceFromSurfacePercMax4, (SpawnRuleOrbit) local5, celestialBodyDeathDistanceMultiplier4, artificialBodyDistanceCheck4);
      newShip2.DockToShip(newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.OrderID == 2)), newShip1.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.OrderID == 1)), newShip1, true);
    }

    private static Ship Rade_SpawnStartingModule(Server server)
    {
      return Ship.CreateNewShip(GameScenes.SceneID.AltCorp_StartingModule, "STARTING " + MathHelper.RandomNextInt().ToString("X"), -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), "StartingScene;", true, 0.03, 0.3, (SpawnRuleOrbit) null, 1.5, 100.0);
    }

    private static void Shomy_WorldInitialize(Server server)
    {
      int num1 = 6;
      string registration1 = "SPAWN HERE";
      long shipID1 = -1;
      // ISSUE: variable of the null type
      __Null local1 = null;
      List<long> celestialBodyGUIDs1 = new List<long>();
      celestialBodyGUIDs1.Add(14L);
      Vector3D? positionOffset1 = new Vector3D?(new Vector3D(-10000200.0, 0.0, 0.0));
      Vector3D? velocityAtPosition1 = new Vector3D?();
      QuaternionD? localRotation1 = new QuaternionD?();
      string vesselTag1 = "";
      int num2 = 0;
      double distanceFromSurfacePercMin1 = 0.03;
      double distanceFromSurfacePercMax1 = 0.3;
      // ISSUE: variable of the null type
      __Null local2 = null;
      double celestialBodyDeathDistanceMultiplier1 = 1.5;
      double artificialBodyDistanceCheck1 = 100.0;
      long guid = Ship.CreateNewShip((GameScenes.SceneID) num1, registration1, shipID1, (List<long>) local1, celestialBodyGUIDs1, positionOffset1, velocityAtPosition1, localRotation1, vesselTag1, num2 != 0, distanceFromSurfacePercMin1, distanceFromSurfacePercMax1, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier1, artificialBodyDistanceCheck1).GUID;
      int num3 = 11;
      string registration2 = "T1";
      long shipID2 = 1;
      List<long> nearArtificialBodyGUIDs = new List<long>();
      nearArtificialBodyGUIDs.Add(guid);
      List<long> celestialBodyGUIDs2 = new List<long>();
      celestialBodyGUIDs2.Add(14L);
      Vector3D? positionOffset2 = new Vector3D?(new Vector3D(50.0, 60.0, 50.0));
      Vector3D? velocityAtPosition2 = new Vector3D?();
      QuaternionD? localRotation2 = new QuaternionD?();
      string vesselTag2 = "";
      int num4 = 0;
      double distanceFromSurfacePercMin2 = 0.03;
      double distanceFromSurfacePercMax2 = 0.3;
      // ISSUE: variable of the null type
      __Null local3 = null;
      double celestialBodyDeathDistanceMultiplier2 = 1.5;
      double artificialBodyDistanceCheck2 = 100.0;
      Ship.CreateNewShip((GameScenes.SceneID) num3, registration2, shipID2, nearArtificialBodyGUIDs, celestialBodyGUIDs2, positionOffset2, velocityAtPosition2, localRotation2, vesselTag2, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local3, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
    }

    private static Ship Shomy_SpawnStaringModule(Server server)
    {
      return Ship.CreateNewShip(GameScenes.SceneID.AltCorp_StartingModule, "Ship " + MathHelper.RandomNextInt().ToString("X"), -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), "StartingScene;", true, 0.03, 0.3, (SpawnRuleOrbit) null, 1.5, 100.0);
    }

    private static void Zare_WorldInitialize(Server server)
    {
    }

    private static Ship Zare_SpawnStaringModule(Server server)
    {
      return Ship.CreateNewShip(GameScenes.SceneID.AltCorp_StartingModule, "Ship " + MathHelper.RandomNextInt().ToString("X"), -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), "StartingScene;", true, 0.03, 0.3, (SpawnRuleOrbit) null, 1.5, 100.0);
    }

    private static void Dima_WorldInitialize(Server server)
    {
      Ship.CreateNewShip(GameScenes.SceneID.MataPrefabs, "Prebubfz", -1L, (List<long>) null, new List<long>(), new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), "", true, 0.03, 0.3, (SpawnRuleOrbit) null, 1.5, 100.0);
      int num1 = 13;
      string registration = "Ship " + MathHelper.RandomNextInt().ToString("X");
      long shipID = 100;
      // ISSUE: variable of the null type
      __Null local1 = null;
      List<long> celestialBodyGUIDs = new List<long>();
      celestialBodyGUIDs.Add(14L);
      Vector3D? positionOffset = new Vector3D?();
      Vector3D? velocityAtPosition = new Vector3D?();
      QuaternionD? localRotation = new QuaternionD?(MathHelper.RandomRotation());
      string vesselTag = "StartingScene;";
      int num2 = 1;
      double distanceFromSurfacePercMin = 0.03;
      double distanceFromSurfacePercMax = 0.3;
      // ISSUE: variable of the null type
      __Null local2 = null;
      double celestialBodyDeathDistanceMultiplier = 1.5;
      double artificialBodyDistanceCheck = 100.0;
      Ship.CreateNewShip((GameScenes.SceneID) num1, registration, shipID, (List<long>) local1, celestialBodyGUIDs, positionOffset, velocityAtPosition, localRotation, vesselTag, num2 != 0, distanceFromSurfacePercMin, distanceFromSurfacePercMax, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier, artificialBodyDistanceCheck);
    }

    private static Ship Dima_SpawnStaringModule(Server server)
    {
      int num1 = 13;
      string registration1 = "Ship " + MathHelper.RandomNextInt().ToString("X");
      long shipID1 = 100;
      // ISSUE: variable of the null type
      __Null local1 = null;
      List<long> celestialBodyGUIDs1 = new List<long>();
      celestialBodyGUIDs1.Add(14L);
      Vector3D? positionOffset1 = new Vector3D?();
      Vector3D? velocityAtPosition1 = new Vector3D?();
      QuaternionD? localRotation1 = new QuaternionD?(MathHelper.RandomRotation());
      string vesselTag1 = "StartingScene;";
      int num2 = 1;
      double distanceFromSurfacePercMin1 = 0.03;
      double distanceFromSurfacePercMax1 = 0.3;
      // ISSUE: variable of the null type
      __Null local2 = null;
      double celestialBodyDeathDistanceMultiplier1 = 1.5;
      double artificialBodyDistanceCheck1 = 100.0;
      Ship newShip1 = Ship.CreateNewShip((GameScenes.SceneID) num1, registration1, shipID1, (List<long>) local1, celestialBodyGUIDs1, positionOffset1, velocityAtPosition1, localRotation1, vesselTag1, num2 != 0, distanceFromSurfacePercMin1, distanceFromSurfacePercMax1, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier1, artificialBodyDistanceCheck1);
      int num3 = 14;
      string registration2 = "Module " + MathHelper.RandomNextInt().ToString("X");
      long shipID2 = 101;
      // ISSUE: variable of the null type
      __Null local3 = null;
      List<long> celestialBodyGUIDs2 = new List<long>();
      celestialBodyGUIDs2.Add(14L);
      Vector3D? positionOffset2 = new Vector3D?();
      Vector3D? velocityAtPosition2 = new Vector3D?();
      QuaternionD? localRotation2 = new QuaternionD?(MathHelper.RandomRotation());
      string vesselTag2 = "StartingScene;";
      int num4 = 1;
      double distanceFromSurfacePercMin2 = 0.03;
      double distanceFromSurfacePercMax2 = 0.3;
      // ISSUE: variable of the null type
      __Null local4 = null;
      double celestialBodyDeathDistanceMultiplier2 = 1.5;
      double artificialBodyDistanceCheck2 = 100.0;
      Ship newShip2 = Ship.CreateNewShip((GameScenes.SceneID) num3, registration2, shipID2, (List<long>) local3, celestialBodyGUIDs2, positionOffset2, velocityAtPosition2, localRotation2, vesselTag2, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local4, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
      int num5 = 6;
      string registration3 = "Ship " + MathHelper.RandomNextInt().ToString("X");
      long shipID3 = 102;
      List<long> nearArtificialBodyGUIDs = new List<long>();
      nearArtificialBodyGUIDs.Add(newShip2.GUID);
      // ISSUE: variable of the null type
      __Null local5 = null;
      Vector3D? positionOffset3 = new Vector3D?(new Vector3D(0.0, 60.0, 0.0));
      Vector3D? velocityAtPosition3 = new Vector3D?();
      QuaternionD? localRotation3 = new QuaternionD?(MathHelper.RandomRotation());
      string vesselTag3 = "StartingScene;";
      int num6 = 1;
      double distanceFromSurfacePercMin3 = 0.03;
      double distanceFromSurfacePercMax3 = 0.3;
      // ISSUE: variable of the null type
      __Null local6 = null;
      double celestialBodyDeathDistanceMultiplier3 = 1.5;
      double artificialBodyDistanceCheck3 = 100.0;
      Ship.CreateNewShip((GameScenes.SceneID) num5, registration3, shipID3, nearArtificialBodyGUIDs, (List<long>) local5, positionOffset3, velocityAtPosition3, localRotation3, vesselTag3, num6 != 0, distanceFromSurfacePercMin3, distanceFromSurfacePercMax3, (SpawnRuleOrbit) local6, celestialBodyDeathDistanceMultiplier3, artificialBodyDistanceCheck3);
      return newShip1;
    }

    private static void Test_WorldInitialize(Server server)
    {
      Asteroid asteroid = (Asteroid) null;
      foreach (CelestialBody celestialBody in server.SolarSystem.GetCelestialBodies())
      {
        if (celestialBody.GUID != 1L)
        {
          for (int index = 0; index < MathHelper.RandomRange(0, 4); ++index)
          {
            string str = "Asteroid " + MathHelper.RandomNextInt().ToString("X");
            int num1 = MathHelper.RandomRange(1000, 1008);
            string registration = str;
            long asteroidGUID = -1;
            // ISSUE: variable of the null type
            __Null local1 = null;
            List<long> celestialBodyGUIDs = new List<long>();
            celestialBodyGUIDs.Add(celestialBody.GUID);
            Vector3D? positionOffset = new Vector3D?();
            Vector3D? velocityAtPosition = new Vector3D?();
            QuaternionD? localRotation = new QuaternionD?(MathHelper.RandomRotation());
            int num2 = 1;
            double distanceFromSurfacePercMin = 0.03;
            double distanceFromSurfacePercMax = 0.3;
            // ISSUE: variable of the null type
            __Null local2 = null;
            double celestialBodyDeathDistanceMultiplier = 1.5;
            double artificialBodyDistanceCheck = 100.0;
            asteroid = Asteroid.CreateNewAsteroid((GameScenes.SceneID) num1, registration, asteroidGUID, (List<long>) local1, celestialBodyGUIDs, positionOffset, velocityAtPosition, localRotation, num2 != 0, distanceFromSurfacePercMin, distanceFromSurfacePercMax, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier, artificialBodyDistanceCheck);
          }
        }
      }
    }

    private static Ship Test_SpawnStaringModule(Server server)
    {
      int num1 = 4;
      Ship newShip = Ship.CreateNewShip(GameScenes.SceneID.AltCorp_StartingModule, "Ship startingModule" + MathHelper.RandomNextInt().ToString("X"), -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(QuaternionD.Identity), "StartingScene;", true, 0.03, 0.3, (SpawnRuleOrbit) null, 1.5, 100.0);
      for (int index = 0; index < CustomServerInitializers.shipsToSpawnForTestStarting.Count; ++index)
      {
        int num2 = (index + 1) / num1;
        int num3 = (index + 1) % num1;
        string str = "Module" + (object) CustomServerInitializers.shipsToSpawnForTestStarting[index];
        int num4 = (int) CustomServerInitializers.shipsToSpawnForTestStarting[index];
        string registration = str;
        long shipID = -1;
        List<long> nearArtificialBodyGUIDs = new List<long>();
        nearArtificialBodyGUIDs.Add(newShip.GUID);
        // ISSUE: variable of the null type
        __Null local1 = null;
        Vector3D? positionOffset = new Vector3D?(new Vector3D((double) (num2 * 50), (double) (num3 * 50), 0.0));
        Vector3D? velocityAtPosition = new Vector3D?();
        QuaternionD? localRotation = new QuaternionD?(QuaternionD.Identity);
        string vesselTag = "StartingScene;";
        int num5 = 0;
        double distanceFromSurfacePercMin = 0.03;
        double distanceFromSurfacePercMax = 0.3;
        // ISSUE: variable of the null type
        __Null local2 = null;
        double celestialBodyDeathDistanceMultiplier = 1.5;
        double artificialBodyDistanceCheck = 100.0;
        Ship.CreateNewShip((GameScenes.SceneID) num4, registration, shipID, nearArtificialBodyGUIDs, (List<long>) local1, positionOffset, velocityAtPosition, localRotation, vesselTag, num5 != 0, distanceFromSurfacePercMin, distanceFromSurfacePercMax, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier, artificialBodyDistanceCheck).StabilizeToTarget((SpaceObjectVessel) newShip, true);
      }
      return newShip;
    }

    private static void Vuja_WorldInitialize(Server server)
    {
      Asteroid asteroid = (Asteroid) null;
      foreach (CelestialBody celestialBody in server.SolarSystem.GetCelestialBodies())
      {
        if (celestialBody.GUID != 1L)
        {
          for (int index = 0; index < MathHelper.RandomRange(0, 4); ++index)
          {
            string str = "Asteroid " + MathHelper.RandomNextInt().ToString("X");
            int num1 = MathHelper.RandomRange(1000, 1008);
            string registration = str;
            long asteroidGUID = -1;
            // ISSUE: variable of the null type
            __Null local1 = null;
            List<long> celestialBodyGUIDs = new List<long>();
            celestialBodyGUIDs.Add(celestialBody.GUID);
            Vector3D? positionOffset = new Vector3D?();
            Vector3D? velocityAtPosition = new Vector3D?();
            QuaternionD? localRotation = new QuaternionD?(MathHelper.RandomRotation());
            int num2 = 1;
            double distanceFromSurfacePercMin = 0.03;
            double distanceFromSurfacePercMax = 0.3;
            // ISSUE: variable of the null type
            __Null local2 = null;
            double celestialBodyDeathDistanceMultiplier = 1.5;
            double artificialBodyDistanceCheck = 100.0;
            asteroid = Asteroid.CreateNewAsteroid((GameScenes.SceneID) num1, registration, asteroidGUID, (List<long>) local1, celestialBodyGUIDs, positionOffset, velocityAtPosition, localRotation, num2 != 0, distanceFromSurfacePercMin, distanceFromSurfacePercMax, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier, artificialBodyDistanceCheck);
          }
        }
      }
      server.DoomedShipController.SpawnDoomedShip();
    }

    private static Ship Vuja_SpawnStaringModule(Server server)
    {
      Ship newShip1 = Ship.CreateNewShip(GameScenes.SceneID.AltCorp_StartingModule, "Ship startingModule" + MathHelper.RandomNextInt().ToString("X"), -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), "StartingScene;", true, 0.03, 0.3, (SpawnRuleOrbit) null, 1.5, 100.0);
      int num1 = 14;
      string registration1 = "Module cargo" + MathHelper.RandomNextInt().ToString("X");
      long shipID1 = -1;
      List<long> nearArtificialBodyGUIDs1 = new List<long>();
      nearArtificialBodyGUIDs1.Add(newShip1.GUID);
      // ISSUE: variable of the null type
      __Null local1 = null;
      Vector3D? positionOffset1 = new Vector3D?(new Vector3D(100.0, 20.0, 0.0));
      Vector3D? velocityAtPosition1 = new Vector3D?();
      QuaternionD? localRotation1 = new QuaternionD?(MathHelper.RandomRotation());
      string vesselTag1 = "StartingScene;";
      int num2 = 0;
      double distanceFromSurfacePercMin1 = 0.03;
      double distanceFromSurfacePercMax1 = 0.3;
      // ISSUE: variable of the null type
      __Null local2 = null;
      double celestialBodyDeathDistanceMultiplier1 = 1.5;
      double artificialBodyDistanceCheck1 = 100.0;
      Ship newShip2 = Ship.CreateNewShip((GameScenes.SceneID) num1, registration1, shipID1, nearArtificialBodyGUIDs1, (List<long>) local1, positionOffset1, velocityAtPosition1, localRotation1, vesselTag1, num2 != 0, distanceFromSurfacePercMin1, distanceFromSurfacePercMax1, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier1, artificialBodyDistanceCheck1);
      newShip2.DockToShip(newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 3)), newShip1.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 4)), newShip1, true);
      int num3 = 6;
      string registration2 = "Ship sara" + MathHelper.RandomNextInt().ToString("X");
      long shipID2 = -1;
      List<long> nearArtificialBodyGUIDs2 = new List<long>();
      nearArtificialBodyGUIDs2.Add(newShip1.GUID);
      // ISSUE: variable of the null type
      __Null local3 = null;
      Vector3D? positionOffset2 = new Vector3D?(new Vector3D(100.0, 0.0, 0.0));
      Vector3D? velocityAtPosition2 = new Vector3D?();
      QuaternionD? localRotation2 = new QuaternionD?(MathHelper.RandomRotation());
      string vesselTag2 = "StartingScene;";
      int num4 = 1;
      double distanceFromSurfacePercMin2 = 0.03;
      double distanceFromSurfacePercMax2 = 0.3;
      // ISSUE: variable of the null type
      __Null local4 = null;
      double celestialBodyDeathDistanceMultiplier2 = 1.5;
      double artificialBodyDistanceCheck2 = 100.0;
      Ship newShip3 = Ship.CreateNewShip((GameScenes.SceneID) num3, registration2, shipID2, nearArtificialBodyGUIDs2, (List<long>) local3, positionOffset2, velocityAtPosition2, localRotation2, vesselTag2, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local4, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
      newShip3.DockToShip(newShip3.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 2)), newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == 1)), newShip2, true);
      return newShip1;
    }

    private static void Mario_WorldInitialize(Server server)
    {
      int num1 = 6;
      string registration1 = "Sara";
      long shipID = -1;
      // ISSUE: variable of the null type
      __Null local1 = null;
      List<long> celestialBodyGUIDs1 = new List<long>();
      celestialBodyGUIDs1.Add(14L);
      Vector3D? positionOffset1 = new Vector3D?(new Vector3D(-10000200.0, 0.0, 0.0));
      Vector3D? velocityAtPosition1 = new Vector3D?();
      QuaternionD? localRotation1 = new QuaternionD?();
      string vesselTag = "";
      int num2 = 0;
      double distanceFromSurfacePercMin1 = 0.03;
      double distanceFromSurfacePercMax1 = 0.3;
      // ISSUE: variable of the null type
      __Null local2 = null;
      double celestialBodyDeathDistanceMultiplier1 = 1.5;
      double artificialBodyDistanceCheck1 = 100.0;
      long guid = Ship.CreateNewShip((GameScenes.SceneID) num1, registration1, shipID, (List<long>) local1, celestialBodyGUIDs1, positionOffset1, velocityAtPosition1, localRotation1, vesselTag, num2 != 0, distanceFromSurfacePercMin1, distanceFromSurfacePercMax1, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier1, artificialBodyDistanceCheck1).GUID;
      for (int index = 1; index <= 1; ++index)
      {
        int num3 = 1000;
        string registration2 = "Asteroid " + (object) index;
        long asteroidGUID = -1;
        List<long> nearArtificialBodyGUIDs = new List<long>();
        nearArtificialBodyGUIDs.Add(guid);
        List<long> celestialBodyGUIDs2 = new List<long>();
        celestialBodyGUIDs2.Add(14L);
        Vector3D? positionOffset2 = new Vector3D?(new Vector3D((double) (50 * index), 800.0, 50.0));
        Vector3D? velocityAtPosition2 = new Vector3D?();
        QuaternionD? localRotation2 = new QuaternionD?(QuaternionD.Identity);
        int num4 = 1;
        double distanceFromSurfacePercMin2 = 0.03;
        double distanceFromSurfacePercMax2 = 0.3;
        // ISSUE: variable of the null type
        __Null local3 = null;
        double celestialBodyDeathDistanceMultiplier2 = 1.5;
        double artificialBodyDistanceCheck2 = 100.0;
        Asteroid.CreateNewAsteroid((GameScenes.SceneID) num3, registration2, asteroidGUID, nearArtificialBodyGUIDs, celestialBodyGUIDs2, positionOffset2, velocityAtPosition2, localRotation2, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local3, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
      }
    }

    private static Ship Mario_SpawnStaringModule(Server server)
    {
      return Ship.CreateNewShip(GameScenes.SceneID.AltCorp_StartingModule, "Ship " + MathHelper.RandomNextInt().ToString("X"), -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), "StartingScene;", true, 0.03, 0.3, (SpawnRuleOrbit) null, 1.5, 100.0);
    }

    private static void Dimbe_WorldInitialize(Server server)
    {
      Ship newShip1 = Ship.CreateNewShip(GameScenes.SceneID.AltCorp_StartingModule, "sara1", -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), "StartingScene;", true, 0.03, 0.3, (SpawnRuleOrbit) null, 1.5, 100.0);
      int num1 = 11;
      string registration1 = "CargoModule";
      long shipID1 = -1;
      List<long> nearArtificialBodyGUIDs = new List<long>();
      nearArtificialBodyGUIDs.Add(newShip1.GUID);
      // ISSUE: variable of the null type
      __Null local1 = null;
      Vector3D? positionOffset1 = new Vector3D?(new Vector3D(100.0, 0.0, 0.0));
      Vector3D? velocityAtPosition1 = new Vector3D?();
      QuaternionD? localRotation1 = new QuaternionD?(MathHelper.RandomRotation());
      string vesselTag1 = "StartingScene;";
      int num2 = 1;
      double distanceFromSurfacePercMin1 = 0.03;
      double distanceFromSurfacePercMax1 = 0.3;
      // ISSUE: variable of the null type
      __Null local2 = null;
      double celestialBodyDeathDistanceMultiplier1 = 1.5;
      double artificialBodyDistanceCheck1 = 100.0;
      Ship newShip2 = Ship.CreateNewShip((GameScenes.SceneID) num1, registration1, shipID1, nearArtificialBodyGUIDs, (List<long>) local1, positionOffset1, velocityAtPosition1, localRotation1, vesselTag1, num2 != 0, distanceFromSurfacePercMin1, distanceFromSurfacePercMax1, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier1, artificialBodyDistanceCheck1);
      for (int index = 0; index < 250; ++index)
      {
        int num3 = 11;
        string registration2 = "Command";
        long shipID2 = -1;
        // ISSUE: variable of the null type
        __Null local3 = null;
        List<long> celestialBodyGUIDs = new List<long>();
        celestialBodyGUIDs.Add(14L);
        Vector3D? positionOffset2 = new Vector3D?();
        Vector3D? velocityAtPosition2 = new Vector3D?();
        QuaternionD? localRotation2 = new QuaternionD?(MathHelper.RandomRotation());
        string vesselTag2 = "StartingScene;";
        int num4 = 1;
        double distanceFromSurfacePercMin2 = 0.03;
        double distanceFromSurfacePercMax2 = 0.3;
        // ISSUE: variable of the null type
        __Null local4 = null;
        double celestialBodyDeathDistanceMultiplier2 = 1.5;
        double artificialBodyDistanceCheck2 = 100.0;
        Ship.CreateNewShip((GameScenes.SceneID) num3, registration2, shipID2, (List<long>) local3, celestialBodyGUIDs, positionOffset2, velocityAtPosition2, localRotation2, vesselTag2, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local4, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
      }
      for (int index = 0; index < 250; ++index)
      {
        int num3 = 11;
        string registration2 = "Command";
        long shipID2 = -1;
        // ISSUE: variable of the null type
        __Null local3 = null;
        List<long> celestialBodyGUIDs = new List<long>();
        celestialBodyGUIDs.Add(18L);
        Vector3D? positionOffset2 = new Vector3D?();
        Vector3D? velocityAtPosition2 = new Vector3D?();
        QuaternionD? localRotation2 = new QuaternionD?(MathHelper.RandomRotation());
        string vesselTag2 = "StartingScene;";
        int num4 = 1;
        double distanceFromSurfacePercMin2 = 0.03;
        double distanceFromSurfacePercMax2 = 0.3;
        // ISSUE: variable of the null type
        __Null local4 = null;
        double celestialBodyDeathDistanceMultiplier2 = 1.5;
        double artificialBodyDistanceCheck2 = 100.0;
        Ship.CreateNewShip((GameScenes.SceneID) num3, registration2, shipID2, (List<long>) local3, celestialBodyGUIDs, positionOffset2, velocityAtPosition2, localRotation2, vesselTag2, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local4, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
      }
      for (int index = 0; index < 250; ++index)
      {
        int num3 = 11;
        string registration2 = "Command";
        long shipID2 = -1;
        // ISSUE: variable of the null type
        __Null local3 = null;
        List<long> celestialBodyGUIDs = new List<long>();
        celestialBodyGUIDs.Add(16L);
        Vector3D? positionOffset2 = new Vector3D?();
        Vector3D? velocityAtPosition2 = new Vector3D?();
        QuaternionD? localRotation2 = new QuaternionD?(MathHelper.RandomRotation());
        string vesselTag2 = "StartingScene;";
        int num4 = 1;
        double distanceFromSurfacePercMin2 = 0.03;
        double distanceFromSurfacePercMax2 = 0.3;
        // ISSUE: variable of the null type
        __Null local4 = null;
        double celestialBodyDeathDistanceMultiplier2 = 1.5;
        double artificialBodyDistanceCheck2 = 100.0;
        Ship.CreateNewShip((GameScenes.SceneID) num3, registration2, shipID2, (List<long>) local3, celestialBodyGUIDs, positionOffset2, velocityAtPosition2, localRotation2, vesselTag2, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local4, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
      }
      for (int index = 0; index < 250; ++index)
      {
        int num3 = 11;
        string registration2 = "Command";
        long shipID2 = -1;
        // ISSUE: variable of the null type
        __Null local3 = null;
        List<long> celestialBodyGUIDs = new List<long>();
        celestialBodyGUIDs.Add(9L);
        Vector3D? positionOffset2 = new Vector3D?();
        Vector3D? velocityAtPosition2 = new Vector3D?();
        QuaternionD? localRotation2 = new QuaternionD?(MathHelper.RandomRotation());
        string vesselTag2 = "StartingScene;";
        int num4 = 1;
        double distanceFromSurfacePercMin2 = 0.03;
        double distanceFromSurfacePercMax2 = 0.3;
        // ISSUE: variable of the null type
        __Null local4 = null;
        double celestialBodyDeathDistanceMultiplier2 = 1.5;
        double artificialBodyDistanceCheck2 = 100.0;
        Ship.CreateNewShip((GameScenes.SceneID) num3, registration2, shipID2, (List<long>) local3, celestialBodyGUIDs, positionOffset2, velocityAtPosition2, localRotation2, vesselTag2, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local4, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
      }
      newShip2.DockToShip(newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.OrderID == 2)), newShip1.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.OrderID == 2)), newShip1, true);
    }

    private static Ship Dimbe_SpawnStaringModule(Server server)
    {
      Ship newShip1 = Ship.CreateNewShip(GameScenes.SceneID.AltCorp_StartingModule, "", -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(MathHelper.RandomRotation()), "StartingScene;", true, 0.03, 0.3, (SpawnRuleOrbit) null, 1.5, 100.0);
      int num1 = 9;
      string registration1 = "SARRRA" + MathHelper.RandomNextInt().ToString("X");
      long shipID1 = -1;
      List<long> nearArtificialBodyGUIDs1 = new List<long>();
      nearArtificialBodyGUIDs1.Add(newShip1.GUID);
      // ISSUE: variable of the null type
      __Null local1 = null;
      Vector3D? positionOffset1 = new Vector3D?(new Vector3D(100.0, 0.0, 0.0));
      Vector3D? velocityAtPosition1 = new Vector3D?();
      QuaternionD? localRotation1 = new QuaternionD?(MathHelper.RandomRotation());
      string vesselTag1 = "StartingScene;";
      int num2 = 1;
      double distanceFromSurfacePercMin1 = 0.03;
      double distanceFromSurfacePercMax1 = 0.3;
      // ISSUE: variable of the null type
      __Null local2 = null;
      double celestialBodyDeathDistanceMultiplier1 = 1.5;
      double artificialBodyDistanceCheck1 = 100.0;
      Ship newShip2 = Ship.CreateNewShip((GameScenes.SceneID) num1, registration1, shipID1, nearArtificialBodyGUIDs1, (List<long>) local1, positionOffset1, velocityAtPosition1, localRotation1, vesselTag1, num2 != 0, distanceFromSurfacePercMin1, distanceFromSurfacePercMax1, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier1, artificialBodyDistanceCheck1);
      int num3 = 6;
      string registration2 = "SARRRA" + MathHelper.RandomNextInt().ToString("X");
      long shipID2 = -1;
      List<long> nearArtificialBodyGUIDs2 = new List<long>();
      nearArtificialBodyGUIDs2.Add(newShip1.GUID);
      // ISSUE: variable of the null type
      __Null local3 = null;
      Vector3D? positionOffset2 = new Vector3D?(new Vector3D(100.0, 0.0, 0.0));
      Vector3D? velocityAtPosition2 = new Vector3D?();
      QuaternionD? localRotation2 = new QuaternionD?(MathHelper.RandomRotation());
      string vesselTag2 = "StartingScene;";
      int num4 = 1;
      double distanceFromSurfacePercMin2 = 0.03;
      double distanceFromSurfacePercMax2 = 0.3;
      // ISSUE: variable of the null type
      __Null local4 = null;
      double celestialBodyDeathDistanceMultiplier2 = 1.5;
      double artificialBodyDistanceCheck2 = 100.0;
      Ship newShip3 = Ship.CreateNewShip((GameScenes.SceneID) num3, registration2, shipID2, nearArtificialBodyGUIDs2, (List<long>) local3, positionOffset2, velocityAtPosition2, localRotation2, vesselTag2, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local4, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
      int num5 = 14;
      string registration3 = "";
      long shipID3 = -1;
      List<long> nearArtificialBodyGUIDs3 = new List<long>();
      nearArtificialBodyGUIDs3.Add(newShip1.GUID);
      // ISSUE: variable of the null type
      __Null local5 = null;
      Vector3D? positionOffset3 = new Vector3D?(new Vector3D(0.0, 100.0, 0.0));
      Vector3D? velocityAtPosition3 = new Vector3D?();
      QuaternionD? localRotation3 = new QuaternionD?(MathHelper.RandomRotation());
      string vesselTag3 = "StartingScene;";
      int num6 = 1;
      double distanceFromSurfacePercMin3 = 0.03;
      double distanceFromSurfacePercMax3 = 0.3;
      // ISSUE: variable of the null type
      __Null local6 = null;
      double celestialBodyDeathDistanceMultiplier3 = 1.5;
      double artificialBodyDistanceCheck3 = 100.0;
      Ship newShip4 = Ship.CreateNewShip((GameScenes.SceneID) num5, registration3, shipID3, nearArtificialBodyGUIDs3, (List<long>) local5, positionOffset3, velocityAtPosition3, localRotation3, vesselTag3, num6 != 0, distanceFromSurfacePercMin3, distanceFromSurfacePercMax3, (SpawnRuleOrbit) local6, celestialBodyDeathDistanceMultiplier3, artificialBodyDistanceCheck3);
      newShip3.StabilizeToTarget((SpaceObjectVessel) newShip1, true);
      newShip4.StabilizeToTarget((SpaceObjectVessel) newShip1, true);
      newShip2.StabilizeToTarget((SpaceObjectVessel) newShip1, true);
      return newShip1;
    }
  }
}
