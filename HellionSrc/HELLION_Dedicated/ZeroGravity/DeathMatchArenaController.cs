// Decompiled with JetBrains decompiler
// Type: ZeroGravity.DeathMatchArenaController
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;
using ZeroGravity.Spawn;

namespace ZeroGravity
{
  public class DeathMatchArenaController : IPersistantObject
  {
    public double SquaredDistanceThreshold = 100000000.0;
    public double timePassedSince = 0.0;
    public SpaceObjectVessel MainVessel;
    public SpaceObjectVessel CurrentSpawnedShip;
    public long arenaAsteroidID;
    public double RespawnTimeForShip;

    public DeathMatchArenaController(SpaceObjectVessel main, Ship ship, long arenaAsteroidID, double squaredDistanceThreshold = 100000000.0, float respawnTimeForShip = 100f)
    {
      this.SquaredDistanceThreshold = squaredDistanceThreshold;
      this.RespawnTimeForShip = (double) respawnTimeForShip;
      this.arenaAsteroidID = arenaAsteroidID;
      this.MainVessel = main;
      this.CurrentSpawnedShip = (SpaceObjectVessel) ship;
    }

    public DeathMatchArenaController()
    {
    }

    public void StartTimerForNewShip()
    {
      this.timePassedSince = 0.0;
      Server.Instance.SubscribeToTimer(UpdateTimer.TimerStep.Step_1_0_min, new UpdateTimer.TimeStepDelegate(this.SpawnShipCallback));
    }

    public static Ship SpawnSara(SpaceObjectVessel mainShip, Vector3D pos, long arenaAsteroidID)
    {
      int num1 = 6;
      string registration = "";
      long shipID = -1;
      List<long> nearArtificialBodyGUIDs = new List<long>();
      nearArtificialBodyGUIDs.Add(mainShip.GUID);
      // ISSUE: variable of the null type
      __Null local1 = null;
      Vector3D? positionOffset = new Vector3D?(pos);
      Vector3D? velocityAtPosition = new Vector3D?();
      QuaternionD? localRotation = new QuaternionD?(MathHelper.RandomRotation());
      string vesselTag = "";
      int num2 = 1;
      double distanceFromSurfacePercMin = 0.03;
      double distanceFromSurfacePercMax = 0.3;
      // ISSUE: variable of the null type
      __Null local2 = null;
      double celestialBodyDeathDistanceMultiplier = 1.5;
      double artificialBodyDistanceCheck = 100.0;
      Ship newShip = Ship.CreateNewShip((GameScenes.SceneID) num1, registration, shipID, nearArtificialBodyGUIDs, (List<long>) local1, positionOffset, velocityAtPosition, localRotation, vesselTag, num2 != 0, distanceFromSurfacePercMin, distanceFromSurfacePercMax, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier, artificialBodyDistanceCheck);
      newShip.StabilizeToTarget(Server.Instance.GetVessel(arenaAsteroidID), true);
      return newShip;
    }

    public static Vector3D NewSaraPos(SpaceObjectVessel v)
    {
      double radius = v.Radius;
      double num1 = MathHelper.RandomRange(0.0, 2.0 * System.Math.PI);
      double num2 = MathHelper.RandomRange(0.0, System.Math.PI);
      return MathHelper.RandomRange(v.Radius + 50.0, v.Radius + 150.0) * new Vector3D(System.Math.Cos(num1) * System.Math.Sin(num2), System.Math.Sin(num1) * System.Math.Sin(num2), System.Math.Cos(num2));
    }

    public void SpawnShipCallback(double dbl)
    {
      this.timePassedSince = this.timePassedSince + dbl;
      if (this.timePassedSince <= this.RespawnTimeForShip)
        return;
      this.timePassedSince = 0.0;
      this.CurrentSpawnedShip = (SpaceObjectVessel) DeathMatchArenaController.SpawnSara(this.MainVessel, DeathMatchArenaController.NewSaraPos(this.MainVessel), this.arenaAsteroidID);
      Server.Instance.UnsubscribeFromTimer(UpdateTimer.TimerStep.Step_1_0_min, new UpdateTimer.TimeStepDelegate(this.SpawnShipCallback));
      Server.Instance.SubscribeToTimer(UpdateTimer.TimerStep.Step_1_0_min, new UpdateTimer.TimeStepDelegate(this.DistanceCallback));
      foreach (Ship ship in new List<SpaceObjectVessel>((IEnumerable<SpaceObjectVessel>) this.MainVessel.AllDockedVessels)
      {
        this.MainVessel
      })
      {
        ship.ToggleDistress(false, DistressType.Distress);
        NetworkController networkController = Server.Instance.NetworkController;
        DistressCallResponse distressCallResponse = new DistressCallResponse();
        distressCallResponse.GUID = ship.GUID;
        distressCallResponse.Trans = ship.GetObjectTransform();
        long skipPlayerGUID = -1;
        networkController.SendToAllClients((NetworkData) distressCallResponse, skipPlayerGUID);
      }
    }

    public void DistanceCallback(double dbl)
    {
      if ((this.CurrentSpawnedShip == null ? double.MaxValue : this.CurrentSpawnedShip.Position.DistanceSquared(this.MainVessel.Position)) <= this.SquaredDistanceThreshold)
        return;
      this.CurrentSpawnedShip = (SpaceObjectVessel) null;
      Server.Instance.UnsubscribeFromTimer(UpdateTimer.TimerStep.Step_1_0_min, new UpdateTimer.TimeStepDelegate(this.DistanceCallback));
      this.StartTimerForNewShip();
    }

    public PersistenceObjectData GetPersistenceData()
    {
      return (PersistenceObjectData) new PersistenceArenaControllerData()
      {
        MainShipGUID = this.MainVessel.GUID,
        CurrentSpawnedShipGUID = (this.CurrentSpawnedShip == null ? -1L : this.CurrentSpawnedShip.GUID),
        RespawnTimeForShip = this.RespawnTimeForShip,
        SquaredDistanceThreshold = this.SquaredDistanceThreshold,
        timePassedSince = this.timePassedSince
      };
    }

    public void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      if (persistenceData is PersistenceArenaControllerData)
      {
        PersistenceArenaControllerData arenaControllerData = persistenceData as PersistenceArenaControllerData;
        this.SquaredDistanceThreshold = arenaControllerData.SquaredDistanceThreshold;
        this.RespawnTimeForShip = arenaControllerData.RespawnTimeForShip;
        this.MainVessel = Server.Instance.GetVessel(arenaControllerData.MainShipGUID);
        if (arenaControllerData.CurrentSpawnedShipGUID < 0L && this.MainVessel.IsDistresActive)
        {
          this.CurrentSpawnedShip = (SpaceObjectVessel) null;
          this.timePassedSince = arenaControllerData.timePassedSince;
          Server.Instance.SubscribeToTimer(UpdateTimer.TimerStep.Step_1_0_min, new UpdateTimer.TimeStepDelegate(this.SpawnShipCallback));
        }
        else
        {
          this.CurrentSpawnedShip = Server.Instance.GetVessel(arenaControllerData.CurrentSpawnedShipGUID);
          Server.Instance.SubscribeToTimer(UpdateTimer.TimerStep.Step_1_0_min, new UpdateTimer.TimeStepDelegate(this.DistanceCallback));
        }
        Server.Instance.DeathMatchArenaControllers.Add(this);
      }
      else
        Dbg.Warning("PersistenceArenaControllerData wrong type");
    }
  }
}
