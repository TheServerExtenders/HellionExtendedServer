// Decompiled with JetBrains decompiler
// Type: ZeroGravity.DoomedShipController
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;
using ZeroGravity.ShipComponents;
using ZeroGravity.Spawn;

namespace ZeroGravity
{
  public class DoomedShipController : IPersistantObject
  {
    public static double DestroyTimerMinSec = 1800.0;
    public static double DestroyTimerMaxSec = 7200.0;
    public static double SpawnFrequencySec = 10800.0;
    public static double SpawnChance = 0.5;
    public static double AdditionalSpawnChance1 = 0.75;
    public static double AdditionalSpawnChance2 = 0.75;
    private double spawnTimer = 0.0;

    public void SubscribeToTimer()
    {
      if (DoomedShipController.SpawnFrequencySec > 60.0)
        Server.Instance.SubscribeToTimer(UpdateTimer.TimerStep.Step_1_0_min, new UpdateTimer.TimeStepDelegate(this.UpdateTimerCallback));
      else
        Server.Instance.SubscribeToTimer(UpdateTimer.TimerStep.Step_0_5_sec, new UpdateTimer.TimeStepDelegate(this.UpdateTimerCallback));
    }

    private void UpdateTimerCallback(double deltaTime)
    {
      this.spawnTimer = this.spawnTimer + deltaTime;
      if (this.spawnTimer <= DoomedShipController.SpawnFrequencySec)
        return;
      if (MathHelper.RandomNextDouble() <= DoomedShipController.SpawnChance)
        this.SpawnDoomedShip();
      this.spawnTimer = 0.0;
    }

    public void SpawnDoomedShip()
    {
      Ship newShip1 = Ship.CreateNewShip(GameScenes.SceneID.Generic_Debris_Outpost001, "", -1L, (List<long>) null, (List<long>) null, new Vector3D?(), new Vector3D?(), new QuaternionD?(), "", true, 0.02, 0.7, (SpawnRuleOrbit) null, 1.5, 100.0);
      newShip1.Health = (float) (MathHelper.RandomRange(DoomedShipController.DestroyTimerMinSec, DoomedShipController.DestroyTimerMaxSec) * (double) newShip1.DecayRate * Server.VesselDecayRateMultiplier);
      if (MathHelper.RandomNextDouble() < DoomedShipController.AdditionalSpawnChance1)
      {
        int index1 = MathHelper.RandomRange(0, CustomServerInitializers.shipsToSpawnForDoomed.Count);
        int num1 = (int) CustomServerInitializers.shipsToSpawnForDoomed[index1];
        string registration1 = "Module";
        long shipID1 = -1;
        List<long> nearArtificialBodyGUIDs1 = new List<long>();
        nearArtificialBodyGUIDs1.Add(newShip1.GUID);
        // ISSUE: variable of the null type
        __Null local1 = null;
        Vector3D? positionOffset1 = new Vector3D?();
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
        Ship newShip2 = Ship.CreateNewShip((GameScenes.SceneID) num1, registration1, shipID1, nearArtificialBodyGUIDs1, (List<long>) local1, positionOffset1, velocityAtPosition1, localRotation1, vesselTag1, num2 != 0, distanceFromSurfacePercMin1, distanceFromSurfacePercMax1, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier1, artificialBodyDistanceCheck1);
        newShip2.Health = (float) (MathHelper.RandomRange(DoomedShipController.DestroyTimerMinSec, DoomedShipController.DestroyTimerMaxSec) * (double) newShip2.DecayRate * Server.VesselDecayRateMultiplier);
        newShip2.DockToShip(newShip2.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.OrderID == 1)), newShip1.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.OrderID == 1)), newShip1, true);
        if (MathHelper.RandomNextDouble() < DoomedShipController.AdditionalSpawnChance2)
        {
          int index2 = MathHelper.RandomRange(0, CustomServerInitializers.shipsToSpawnForDoomed.Count);
          while (index2 == index1)
            index2 = MathHelper.RandomRange(0, CustomServerInitializers.shipsToSpawnForDoomed.Count);
          int num3 = (int) CustomServerInitializers.shipsToSpawnForDoomed[index2];
          string registration2 = "Module";
          long shipID2 = -1;
          List<long> nearArtificialBodyGUIDs2 = new List<long>();
          nearArtificialBodyGUIDs2.Add(newShip1.GUID);
          // ISSUE: variable of the null type
          __Null local3 = null;
          Vector3D? positionOffset2 = new Vector3D?();
          Vector3D? velocityAtPosition2 = new Vector3D?();
          QuaternionD? localRotation2 = new QuaternionD?();
          string vesselTag2 = "";
          int num4 = 0;
          double distanceFromSurfacePercMin2 = 0.03;
          double distanceFromSurfacePercMax2 = 0.3;
          // ISSUE: variable of the null type
          __Null local4 = null;
          double celestialBodyDeathDistanceMultiplier2 = 1.5;
          double artificialBodyDistanceCheck2 = 100.0;
          Ship newShip3 = Ship.CreateNewShip((GameScenes.SceneID) num3, registration2, shipID2, nearArtificialBodyGUIDs2, (List<long>) local3, positionOffset2, velocityAtPosition2, localRotation2, vesselTag2, num4 != 0, distanceFromSurfacePercMin2, distanceFromSurfacePercMax2, (SpawnRuleOrbit) local4, celestialBodyDeathDistanceMultiplier2, artificialBodyDistanceCheck2);
          newShip3.Health = (float) (MathHelper.RandomRange(DoomedShipController.DestroyTimerMinSec, DoomedShipController.DestroyTimerMaxSec) * (double) newShip3.DecayRate * Server.VesselDecayRateMultiplier);
          newShip3.DockToShip(newShip3.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.OrderID == 1)), newShip1.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.OrderID == 2)), newShip1, true);
        }
      }
      newShip1.ToggleDoomed(true);
      Server.Instance.NetworkController.SendToAllClients((NetworkData) Server.Instance.SendSystemMessage(SystemMessagesTypes.DoomedOutpostSpawned, newShip1), -1L);
    }

    public PersistenceObjectData GetPersistenceData()
    {
      return (PersistenceObjectData) new PersistenceDataDoomController()
      {
        SpawnTimer = this.spawnTimer
      };
    }

    public void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      PersistenceDataDoomController dataDoomController = persistenceData as PersistenceDataDoomController;
      if (dataDoomController == null)
        Dbg.Warning("PersistenceDataDoomController wrong type");
      else
        this.spawnTimer = dataDoomController.SpawnTimer;
    }
  }
}
