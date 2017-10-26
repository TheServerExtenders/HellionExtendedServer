// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Ship
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using ZeroGravity.BulletPhysics;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.ShipComponents;
using ZeroGravity.Spawn;

namespace ZeroGravity.Objects
{
  public class Ship : SpaceObjectVessel, IPersistantObject
  {
    public static bool DoSpecialPrint = false;
    public List<SceneTriggerExecuter> SceneTriggerExecuters = new List<SceneTriggerExecuter>();
    private bool isRcsOnline = false;
    public Vector3D RcsThrustVelocityDifference = Vector3D.Zero;
    public Vector3D RcsThrustDirection = Vector3D.Zero;
    private double rcsThrustResetTimer = 0.0;
    private double rcsThrustResetTreshold = 0.2;
    private bool isEngineOnline = false;
    public Vector3D EngineThrustVelocityDifference = Vector3D.Zero;
    private double engineThrustPercentage = 0.0;
    private double currentEngineThrustPerc = 0.0;
    private bool isRotationOnline = false;
    public Vector3D RotationThrustVelocityDifference = Vector3D.Zero;
    public Vector3D RotationThrustDirection = Vector3D.Zero;
    private double rotationThrustResetTimer = 0.0;
    private double rotationThrustResetTreshold = 0.5;
    private double autoStabilizeTimer = 0.0;
    private double autoStabilizeTreshold = 60.0;
    private double stabilizeResetTimer = 0.0;
    private double stabilizeResetTreshold = 1.0;
    private double systemsUpdateTimer = 0.0;
    public int ColliderIndex = 1;
    private bool rcsThrustChanged = false;
    private Vector3D? _currRcsMoveThrust = new Vector3D?();
    private Vector3D? _currRcsRotationThrust = new Vector3D?();
    public double RespawnTimeForShip = 600.0;
    public double timePassedSinceRequest = 0.0;
    private bool isDoomed = false;
    private TimeSpan lastShipCollisionMessageTime = Server.Instance.RunTime;
    private bool stabilizeX;
    private bool stabilizeY;
    private bool stabilizeZ;
    public bool CollectAtmosphere;
    public bool sendResourceUpdate;
    public float Resource1;
    public float Resource2;
    public float Resource3;
    public SpaceObjectVessel CurrentSpawnedShip;
    public const double vesselRequestDistanceThreshold = 5000.0;

    public override SpaceObjectType ObjectType
    {
      get
      {
        return SpaceObjectType.Ship;
      }
    }

    private Vector3D? CurrRcsMoveThrust
    {
      get
      {
        return this._currRcsMoveThrust;
      }
      set
      {
        this._currRcsMoveThrust = value;
        this.rcsThrustChanged = true;
      }
    }

    private Vector3D? CurrRcsRotationThrust
    {
      get
      {
        return this._currRcsRotationThrust;
      }
      set
      {
        this._currRcsRotationThrust = value;
        this.rcsThrustChanged = true;
      }
    }

    public Ship(long guid, bool initializeOrbit, Vector3D position, Vector3D velocity, Vector3D forward, Vector3D up)
      : base(guid, initializeOrbit, position, velocity, forward, up)
    {
      this.Radius = 100.0;
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (ShipStatsMessage), new EventSystem.NetworkDataDelegate(this.ShipStatsMessageListener));
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (ManeuverCourseRequest), new EventSystem.NetworkDataDelegate(this.ManeuverCourseRequestListener));
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (DistressCallRequest), new EventSystem.NetworkDataDelegate(this.DistressCallRequestListener));
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (VesselRequest), new EventSystem.NetworkDataDelegate(this.VesselRequestListener));
      Server.Instance.NetworkController.EventSystem.AddListener(typeof (VesselSecurityRequest), new EventSystem.NetworkDataDelegate(this.VesselSecurityRequestListener));
    }

    public void ResetRotationAndThrust()
    {
      this.isRcsOnline = false;
      this.RcsThrustVelocityDifference = Vector3D.Zero;
      this.RcsThrustDirection = Vector3D.Zero;
      this.rcsThrustResetTimer = 0.0;
      this.isEngineOnline = false;
      this.EngineThrustVelocityDifference = Vector3D.Zero;
      this.engineThrustPercentage = 0.0;
      this.currentEngineThrustPerc = 0.0;
      this.isRotationOnline = false;
      this.RotationThrustVelocityDifference = Vector3D.Zero;
      this.RotationThrustDirection = Vector3D.Zero;
      this.rotationThrustResetTimer = 0.0;
      this.stabilizeX = false;
      this.stabilizeY = false;
      this.stabilizeZ = false;
      this.stabilizeResetTimer = 0.0;
      this.Rotation = Vector3D.Zero;
      this.AngularVelocity = Vector3D.Zero;
    }

    private void ManeuverCourseRequestListener(NetworkData data)
    {
      ManeuverCourseRequest req = data as ManeuverCourseRequest;
      if (req.ShipGUID != this.GUID)
        return;
      if (req.CourseItems != null && req.CourseItems.Count > 0)
      {
        this.DisableStabilization(true, true);
        this.CurrentCourse = ManeuverCourse.ParseNetworkData(req, this);
        this.CurrentCourse.ReadNextManeuverCourse();
      }
      if (req.Activate.HasValue && this.CurrentCourse != null && this.CurrentCourse.CourseGUID == req.CourseGUID)
        this.CurrentCourse.ToggleActivated(req.Activate.Value);
      this.CurrentCourse.SendCourseStartResponse();
    }

    private Vector3D getClampedVector(float[] vec)
    {
      Vector3D vector3D = vec.ToVector3D();
      if (vector3D.SqrMagnitude > 1.0)
        return vector3D.Normalized;
      return vector3D;
    }

    public bool CalculateEngineThrust(double timeDelta)
    {
      double val = this.engineThrustPercentage;
      if (!this.isEngineOnline)
        val = 0.0;
      if (!val.IsNotEpsilonZeroD(double.Epsilon) && !this.currentEngineThrustPerc.IsNotEpsilonZeroD(double.Epsilon))
        return false;
      int num = 1;
      if (this.currentEngineThrustPerc < 0.0 && val >= -4.94065645841247E-324 || this.currentEngineThrustPerc > 0.0 && val > double.Epsilon)
        num = 1;
      else if (this.currentEngineThrustPerc > 0.0 && val <= double.Epsilon || this.currentEngineThrustPerc < 0.0 && val < double.Epsilon)
        num = -1;
      this.currentEngineThrustPerc = this.currentEngineThrustPerc + timeDelta * (double) this.VesselCaps.EngineAccelerationBuildup * (double) num;
      if (num == 1 && this.currentEngineThrustPerc > val)
        this.currentEngineThrustPerc = val;
      else if (num == -1 && this.currentEngineThrustPerc < val)
        this.currentEngineThrustPerc = val;
      this.EngineThrustVelocityDifference = this.Forward * (this.currentEngineThrustPerc * (this.currentEngineThrustPerc > 0.0 ? (double) this.VesselCaps.EngineAcceleration : (double) this.VesselCaps.EngineReverseAcceleration)) * timeDelta;
      this.autoStabilizeTimer = 0.0;
      return true;
    }

    public bool CalculateRcsThrust(double timeDelta)
    {
      if (!this.isRcsOnline)
      {
        if (this.CurrRcsMoveThrust.HasValue)
          this.CurrRcsMoveThrust = new Vector3D?();
        return false;
      }
      this.RcsThrustVelocityDifference = this.RcsThrustDirection * (double) this.VesselCaps.RcsAcceleration * timeDelta * Server.RCS_THRUST_MULTIPLIER;
      this.rcsThrustResetTimer = this.rcsThrustResetTimer + timeDelta;
      this.CurrRcsMoveThrust = new Vector3D?(this.RcsThrustVelocityDifference);
      this.autoStabilizeTimer = 0.0;
      if (this.rcsThrustResetTimer >= this.rcsThrustResetTreshold)
        this.isRcsOnline = false;
      return true;
    }

    public bool CalculateRotationThrust(double timeDelta)
    {
      if (!this.isRotationOnline)
      {
        if (this.CurrRcsRotationThrust.HasValue)
          this.CurrRcsRotationThrust = new Vector3D?();
        return false;
      }
      this.RotationThrustVelocityDifference = this.RotationThrustDirection * (double) this.VesselCaps.RotationAcceleration * timeDelta * Server.RCS_ROTATION_MULTIPLIER;
      this.rotationThrustResetTimer = this.rotationThrustResetTimer + timeDelta;
      this.CurrRcsRotationThrust = new Vector3D?(this.RotationThrustVelocityDifference);
      this.autoStabilizeTimer = 0.0;
      if (this.rotationThrustResetTimer >= this.rotationThrustResetTreshold)
        this.isRotationOnline = false;
      return true;
    }

    public bool CalculateRotationDampen(double timeDelta)
    {
      if (this.DistributionManager.RCS == null || !this.stabilizeX && !this.stabilizeY && !this.stabilizeZ)
        return false;
      this.DampenRotation(timeDelta, this.stabilizeX, this.stabilizeY, this.stabilizeZ, (double) this.DistributionManager.RCS.OperationRate);
      this.stabilizeResetTimer = this.stabilizeResetTimer + timeDelta;
      if (this.stabilizeResetTimer >= this.stabilizeResetTreshold)
      {
        this.stabilizeX = false;
        this.stabilizeY = false;
        this.stabilizeZ = false;
      }
      this.autoStabilizeTimer = 0.0;
      return true;
    }

    public bool CalculateAutoStabilizeRotation(double timeDelta)
    {
      if (this.DistributionManager.RCS == null)
        return false;
      if (this.Rotation.IsNotEpsilonZero(double.Epsilon))
      {
        this.autoStabilizeTimer = this.autoStabilizeTimer + timeDelta;
        if (this.autoStabilizeTimer <= this.autoStabilizeTreshold)
          return false;
        this.DampenRotation(timeDelta, true, true, true, 0.5 * (double) this.DistributionManager.RCS.OperationRate);
        return true;
      }
      this.autoStabilizeTimer = 0.0;
      return false;
    }

    public void CheckThrustStatsMessage()
    {
      if (!this.rcsThrustChanged)
        return;
      ShipStatsMessage shipStatsMessage1 = new ShipStatsMessage();
      shipStatsMessage1.GUID = this.GUID;
      ShipStatsMessage shipStatsMessage2 = shipStatsMessage1;
      RcsThrustStats rcsThrustStats = new RcsThrustStats();
      Vector3D? nullable = this.CurrRcsMoveThrust;
      float[] numArray1;
      if (!nullable.HasValue)
      {
        numArray1 = (float[]) null;
      }
      else
      {
        nullable = this.CurrRcsMoveThrust;
        numArray1 = nullable.Value.ToFloatArray();
      }
      rcsThrustStats.MoveTrust = numArray1;
      nullable = this.CurrRcsRotationThrust;
      float[] numArray2;
      if (!nullable.HasValue)
      {
        numArray2 = (float[]) null;
      }
      else
      {
        nullable = this.CurrRcsRotationThrust;
        numArray2 = nullable.Value.ToFloatArray();
      }
      rcsThrustStats.RotationTrust = numArray2;
      shipStatsMessage2.ThrustStats = rcsThrustStats;
      this.rcsThrustChanged = false;
      Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) shipStatsMessage1, -1L, (SpaceObject) this);
    }

    public void ShipStatsMessageListener(NetworkData data)
    {
      ShipStatsMessage shipStatsMessage1 = data as ShipStatsMessage;
      if (shipStatsMessage1.GUID != this.GUID)
        return;
      bool flag1 = false;
      ShipStatsMessage shipStatsMessage2 = new ShipStatsMessage();
      shipStatsMessage2.GUID = this.GUID;
      shipStatsMessage2.Temperature = new float?(this.Temperature);
      shipStatsMessage2.Health = new float?(this.Health);
      shipStatsMessage2.VesselObjects = new VesselObjects();
      Player player = Server.Instance.GetPlayer(data.Sender);
      if (!this.IsDocked)
      {
        bool flag2 = shipStatsMessage1.EngineOnline.HasValue && shipStatsMessage1.EngineOnline.Value;
        bool flag3 = shipStatsMessage1.Thrust != null || shipStatsMessage1.Rotation != null || (shipStatsMessage1.AutoStabilizeX.HasValue || shipStatsMessage1.AutoStabilizeY.HasValue) || shipStatsMessage1.AutoStabilizeZ.HasValue || shipStatsMessage1.TargetStabilizationGUID.HasValue && shipStatsMessage1.Thrust == null;
        bool flag4 = false;
        bool flag5 = false;
        bool flag6 = false;
        if (flag2 | flag3)
        {
          if (this.DistributionManager.Engine != null & flag2)
          {
            this.DistributionManager.Engine.ThrustActive = true;
            flag6 = true;
          }
          if (this.DistributionManager.RCS != null & flag3 && this.DistributionManager.RCS.Status != SystemStatus.OnLine)
          {
            this.DistributionManager.RCS.GoOnLine();
            flag6 = true;
          }
          if (flag6)
            this.DistributionManager.UpdateSystems(false, false);
          flag4 = this.DistributionManager.Engine != null && this.DistributionManager.Engine.Status == SystemStatus.OnLine;
          flag5 = this.DistributionManager.RCS != null && this.DistributionManager.RCS.Status == SystemStatus.OnLine;
        }
        if (this.DistributionManager.Engine != null)
        {
          if (shipStatsMessage1.EngineThrustPercentage.HasValue)
          {
            this.engineThrustPercentage = (double) shipStatsMessage1.EngineThrustPercentage.Value;
            shipStatsMessage2.EngineThrustPercentage = new float?((float) this.engineThrustPercentage);
            this.DistributionManager.Engine.RequiredThrust = (float) System.Math.Abs(this.engineThrustPercentage);
            this.DistributionManager.Engine.ReverseThrust = this.engineThrustPercentage < 0.0;
            flag1 = true;
          }
          if (shipStatsMessage1.EngineOnline.HasValue)
          {
            bool isEngineOnline = this.isEngineOnline;
            if (shipStatsMessage1.EngineOnline.Value)
            {
              this.isEngineOnline = flag4;
            }
            else
            {
              this.isEngineOnline = false;
              this.DistributionManager.Engine.ThrustActive = false;
            }
            if (isEngineOnline != this.isEngineOnline)
            {
              shipStatsMessage2.EngineOnline = new bool?(this.isEngineOnline);
              flag1 = true;
            }
          }
        }
        if (this.DistributionManager.RCS != null & flag5)
        {
          float val1 = 0.0f;
          float val2 = 0.0f;
          if (shipStatsMessage1.Thrust != null)
          {
            Vector3D vector3D = shipStatsMessage1.Thrust.ToVector3D();
            if (vector3D.SqrMagnitude > 1.0)
              vector3D = vector3D.Normalized;
            this.RcsThrustDirection = vector3D * (double) this.DistributionManager.RCS.MaxOperationRate;
            val1 = (float) this.RcsThrustDirection.Magnitude / this.DistributionManager.RCS.MaxOperationRate;
            if (!this.RcsThrustDirection.IsEpsilonEqual(Vector3D.Zero, 0.0001))
            {
              this.rcsThrustResetTimer = 0.0;
              this.isRcsOnline = true;
              shipStatsMessage2.TargetStabilizationGUID = new long?(-1L);
              flag1 = true;
            }
            else
              this.isRcsOnline = false;
          }
          if (shipStatsMessage1.Rotation != null && (this.CurrentCourse == null || !this.CurrentCourse.IsInProgress))
          {
            Vector3D vector3D = shipStatsMessage1.Rotation.ToVector3D();
            if (vector3D.SqrMagnitude > 1.0)
              vector3D = vector3D.Normalized;
            this.RotationThrustDirection = vector3D * (double) this.DistributionManager.RCS.MaxOperationRate;
            val1 = (float) this.RotationThrustDirection.Magnitude / this.DistributionManager.RCS.MaxOperationRate;
            if (!this.RotationThrustDirection.IsEpsilonEqual(Vector3D.Zero, 0.0001))
            {
              this.rotationThrustResetTimer = 0.0;
              this.isRotationOnline = true;
            }
            else
              this.isRotationOnline = false;
          }
          if (shipStatsMessage1.AutoStabilizeX.HasValue)
          {
            this.stabilizeX = shipStatsMessage1.AutoStabilizeX.Value;
            this.stabilizeResetTimer = 0.0;
            this.DistributionManager.RCS.OperationRate = this.DistributionManager.RCS.MaxOperationRate;
          }
          if (shipStatsMessage1.AutoStabilizeY.HasValue)
          {
            this.stabilizeY = shipStatsMessage1.AutoStabilizeY.Value;
            this.stabilizeResetTimer = 0.0;
            this.DistributionManager.RCS.OperationRate = this.DistributionManager.RCS.MaxOperationRate;
          }
          if (shipStatsMessage1.AutoStabilizeZ.HasValue)
          {
            this.stabilizeZ = shipStatsMessage1.AutoStabilizeZ.Value;
            this.stabilizeResetTimer = 0.0;
            this.DistributionManager.RCS.OperationRate = this.DistributionManager.RCS.MaxOperationRate;
          }
          if ((double) this.DistributionManager.RCS.OperationRate == 0.0)
            this.DistributionManager.RCS.OperationRate = System.Math.Max(val1, val2);
          if (shipStatsMessage1.TargetStabilizationGUID.HasValue && shipStatsMessage1.Thrust == null)
          {
            SpaceObjectVessel target = Server.Instance.GetObject(shipStatsMessage1.TargetStabilizationGUID.Value) as SpaceObjectVessel;
            int num = target == null ? 0 : (this.StabilizeToTarget(target, false) ? 1 : 0);
            shipStatsMessage2.TargetStabilizationGUID = num == 0 ? new long?(-1L) : new long?(target.GUID);
            flag1 = true;
          }
        }
      }
      else
      {
        if (this.isEngineOnline)
        {
          this.isEngineOnline = false;
          shipStatsMessage2.EngineOnline = new bool?(false);
          flag1 = true;
        }
        if (this.isRcsOnline)
        {
          this.isRcsOnline = false;
          this.RcsThrustDirection = Vector3D.Zero;
          this.RotationThrustDirection = Vector3D.Zero;
          this.stabilizeX = false;
          this.stabilizeY = false;
          this.stabilizeZ = false;
          flag1 = true;
        }
      }
      if (shipStatsMessage1.VesselObjects == null)
        return;
      bool flag7 = false;
      bool connectionsChanged = false;
      bool compoundRoomsChanged = false;
      if (shipStatsMessage1.VesselObjects.RoomTriggers != null && shipStatsMessage1.VesselObjects.RoomTriggers.Count > 0)
      {
        flag7 = true;
        foreach (RoomDetails roomTrigger in shipStatsMessage1.VesselObjects.RoomTriggers)
        {
          if (this.MainDistributionManager.GetRoom(new VesselObjectID(this.GUID, roomTrigger.InSceneID)) != null)
            this.MainDistributionManager.GetRoom(new VesselObjectID(this.GUID, roomTrigger.InSceneID)).UseGravity = roomTrigger.UseGravity;
        }
      }
      if (shipStatsMessage1.VesselObjects.Generators != null && shipStatsMessage1.VesselObjects.Generators.Count > 0)
      {
        flag7 = true;
        foreach (GeneratorDetails generator in shipStatsMessage1.VesselObjects.Generators)
          this.MainDistributionManager.GetGenerator(new VesselObjectID(this.GUID, generator.InSceneID)).SetDetails(generator);
      }
      if (shipStatsMessage1.VesselObjects.SubSystems != null && shipStatsMessage1.VesselObjects.SubSystems.Count > 0)
      {
        flag7 = true;
        foreach (SubSystemDetails subSystem in shipStatsMessage1.VesselObjects.SubSystems)
          this.MainDistributionManager.GetSubSystem(new VesselObjectID(this.GUID, subSystem.InSceneID)).SetDetails(subSystem);
      }
      if (shipStatsMessage1.VesselObjects.Doors != null)
      {
        List<DoorDetails> doorDetailsList = new List<DoorDetails>();
        foreach (DoorDetails door1 in shipStatsMessage1.VesselObjects.Doors)
        {
          DoorDetails dd = door1;
          Door door2 = this.Doors.Find((Predicate<Door>) (m => m.ID.InSceneID == dd.InSceneID));
          if (door2 != null && (door2.HasPower != dd.HasPower || door2.IsLocked != dd.IsLocked || door2.IsOpen != dd.IsOpen))
          {
            bool isSealed = door2.IsSealed;
            door2.HasPower = dd.HasPower;
            door2.IsLocked = dd.IsLocked;
            door2.IsOpen = dd.IsOpen;
            doorDetailsList.Add(dd);
            flag7 = true;
            compoundRoomsChanged = true;
            if (((dd.EquilizePressure ? 0 : (!door2.IsSealed ? 1 : 0)) & (isSealed ? 1 : 0)) != 0)
            {
              VesselObjectID doorID = new VesselObjectID(this.GUID, dd.InSceneID);
              dd.PressureEquilizationTime = this.MainDistributionManager.PressureEquilizationTime(doorID, out dd.AirFlowDirection, out dd.AirSpeed);
            }
          }
        }
        if (doorDetailsList.Count > 0)
        {
          shipStatsMessage2.VesselObjects.Doors = doorDetailsList;
          flag1 = true;
        }
      }
      if (shipStatsMessage1.VesselObjects.SceneTriggerExecuters != null)
      {
        List<SceneTriggerExecuterDetails> triggerExecuterDetailsList = new List<SceneTriggerExecuterDetails>();
        foreach (SceneTriggerExecuterDetails sceneTriggerExecuter1 in shipStatsMessage1.VesselObjects.SceneTriggerExecuters)
        {
          SceneTriggerExecuterDetails stDetails = sceneTriggerExecuter1;
          SceneTriggerExecuter sceneTriggerExecuter2 = this.SceneTriggerExecuters.Find((Predicate<SceneTriggerExecuter>) (m => m.InSceneID == stDetails.InSceneID));
          if (sceneTriggerExecuter2 != null && stDetails != null)
            triggerExecuterDetailsList.Add(sceneTriggerExecuter2.ChangeState(shipStatsMessage1.Sender, stDetails));
        }
        if (triggerExecuterDetailsList.Count > 0)
        {
          shipStatsMessage2.VesselObjects.SceneTriggerExecuters = triggerExecuterDetailsList;
          flag1 = true;
        }
      }
      if (shipStatsMessage1.VesselObjects.AttachPoints != null)
      {
        foreach (AttachPointDetails attachPoint in shipStatsMessage1.VesselObjects.AttachPoints)
        {
          if (attachPoint.AuxDetails != null && attachPoint.AuxDetails is MachineryPartSlotAuxDetails)
          {
            VesselObjectID vesselObjectId = new VesselObjectID(this.GUID, attachPoint.InSceneID);
            VesselComponent componentByPartSlot = this.MainDistributionManager.GetVesselComponentByPartSlot(vesselObjectId);
            if (componentByPartSlot != null)
              componentByPartSlot.SetMachineryPartSlotActive(vesselObjectId, (attachPoint.AuxDetails as MachineryPartSlotAuxDetails).IsActive);
          }
        }
      }
      if (shipStatsMessage1.VesselObjects.DockingPorts != null)
      {
        List<SceneDockingPortDetails> dockingPortDetailsList = new List<SceneDockingPortDetails>();
        foreach (SceneDockingPortDetails dockingPort in shipStatsMessage1.VesselObjects.DockingPorts)
        {
          SceneDockingPortDetails stDetails = dockingPort;
          VesselDockingPort port = this.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == stDetails.ID.InSceneID));
          if (port != null && (stDetails.DockedToID != null && Server.Instance.DoesObjectExist(stDetails.DockedToID.VesselGUID)))
          {
            if (stDetails.DockingStatus)
            {
              Ship vessel = Server.Instance.GetVessel(stDetails.DockedToID.VesselGUID) as Ship;
              VesselDockingPort vesselDockingPort = vessel.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == stDetails.DockedToID.InSceneID));
              if (this.DockToShip(port, vesselDockingPort, vessel, true))
              {
                stDetails.CollidersCenterOffset = this.DockedToMainVessel.VesselData.CollidersCenterOffset;
                stDetails.RelativePositionUpdate = new Dictionary<long, float[]>();
                stDetails.RelativeRotationUpdate = new Dictionary<long, float[]>();
                foreach (Ship allDockedVessel in this.DockedToMainVessel.AllDockedVessels)
                {
                  stDetails.RelativePositionUpdate.Add(allDockedVessel.GUID, allDockedVessel.RelativePositionFromParent.ToFloatArray());
                  stDetails.RelativeRotationUpdate.Add(allDockedVessel.GUID, allDockedVessel.RelativeRotationFromParent.ToFloatArray());
                }
                stDetails.ExecutersMerge = port.GetMergedExecuters(vesselDockingPort);
                stDetails.PairedDoors = this.GetPairedDoors(port);
                stDetails.RelativePosition = this.RelativePositionFromParent.ToFloatArray();
                stDetails.RelativeRotation = this.RelativeRotationFromParent.ToFloatArray();
                dockingPortDetailsList.Add(stDetails);
              }
            }
            else if (port.DockedToID != null)
            {
              Ship dockedVessel = port.DockedVessel as Ship;
              VesselDockingPort dockedToPort = dockedVessel.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == port.DockedToID.InSceneID));
              SceneDockingPortDetails details = stDetails;
              if (this.UndockFromShip(port, dockedVessel, dockedToPort, ref details))
              {
                stDetails.ExecutersMerge = new List<ExecuterMergeDetails>();
                foreach (SceneTriggerExecuter key in port.MergeExecuters.Keys)
                  stDetails.ExecutersMerge.Add(new ExecuterMergeDetails()
                  {
                    ParentTriggerID = new VesselObjectID(this.GUID, key.InSceneID)
                  });
                stDetails.RelativePosition = this.RelativeRotationFromParent.ToFloatArray();
                stDetails.RelativeRotation = this.RelativeRotationFromParent.ToFloatArray();
                dockingPortDetailsList.Add(stDetails);
              }
            }
            else
              Dbg.Warning((object) "Tried to undock non-docked docking port", (object) port.ID);
          }
        }
        if (dockingPortDetailsList.Count > 0)
        {
          shipStatsMessage2.VesselObjects.DockingPorts = dockingPortDetailsList;
          flag1 = true;
          foreach (SceneDockingPortDetails dockingPortDetails in dockingPortDetailsList)
          {
            if (dockingPortDetails.PairedDoors != null)
            {
              foreach (PairedDoorsDetails pairedDoor in dockingPortDetails.PairedDoors)
              {
                PairedDoorsDetails pdd = pairedDoor;
                Door door = this.Doors.Find((Predicate<Door>) (m => m.ID.Equals((object) pdd.DoorID)));
                if (door.LockedAutoToggle)
                {
                  bool flag2 = pdd.PairedDoorID == null;
                  if (door.IsLocked != flag2)
                  {
                    door.IsLocked = flag2;
                    if (shipStatsMessage2.VesselObjects.Doors == null)
                      shipStatsMessage2.VesselObjects.Doors = new List<DoorDetails>();
                    DoorDetails doorDetails = door.GetDetails();
                    DoorDetails doorDetails1 = shipStatsMessage2.VesselObjects.Doors.Find((Predicate<DoorDetails>) (m => m.InSceneID == doorDetails.InSceneID));
                    if (doorDetails1 != null)
                      doorDetails1.IsLocked = door.IsLocked;
                    else
                      shipStatsMessage2.VesselObjects.Doors.Add(doorDetails);
                  }
                }
              }
            }
          }
        }
      }
      if (shipStatsMessage1.VesselObjects.SpawnPoints != null)
      {
        List<SpawnPointStats> spawnPointStatsList = new List<SpawnPointStats>();
        foreach (SpawnPointStats spawnPoint in shipStatsMessage1.VesselObjects.SpawnPoints)
        {
          SpawnPointStats stats = spawnPoint;
          ShipSpawnPoint shipSpawnPoint = this.SpawnPoints.Find((Predicate<ShipSpawnPoint>) (m => m.SpawnPointID == stats.InSceneID));
          if (shipSpawnPoint != null)
          {
            SpawnPointStats spawnPointStats = shipSpawnPoint.SetStats(stats, player);
            if (spawnPointStats != null)
              spawnPointStatsList.Add(spawnPointStats);
          }
        }
        if (spawnPointStatsList.Count > 0)
        {
          shipStatsMessage2.VesselObjects.SpawnPoints = spawnPointStatsList;
          flag1 = true;
        }
      }
      if (flag7)
      {
        this.MainDistributionManager.UpdateSystems(connectionsChanged, compoundRoomsChanged);
        shipStatsMessage2.VesselObjects.SubSystems = this.MainDistributionManager.GetSubSystemsDetails(true, this.GUID);
        bool flag2 = flag1 | shipStatsMessage2.VesselObjects.SubSystems.Count > 0;
        shipStatsMessage2.VesselObjects.Generators = this.MainDistributionManager.GetGeneratorsDetails(true, this.GUID);
        bool flag3 = flag2 | shipStatsMessage2.VesselObjects.Generators.Count > 0;
        shipStatsMessage2.VesselObjects.RoomTriggers = this.MainDistributionManager.GetRoomsDetails(true, this.GUID);
        bool flag4 = flag3 | shipStatsMessage2.VesselObjects.RoomTriggers.Count > 0;
        shipStatsMessage2.VesselObjects.ResourceContainers = this.MainDistributionManager.GetResourceContainersDetails(true, this.GUID);
        flag1 = flag4 | shipStatsMessage2.VesselObjects.ResourceContainers.Count > 0;
      }
      if (shipStatsMessage1.GatherAtmosphere.HasValue)
        this.CollectAtmosphere = shipStatsMessage1.GatherAtmosphere.Value;
      if (shipStatsMessage1.AsteroidGuid.HasValue)
      {
        Asteroid vessel = Server.Instance.GetVessel(shipStatsMessage1.AsteroidGuid.Value) as Asteroid;
        if (vessel != null)
        {
          shipStatsMessage2.resourcesOnAsteroid = vessel.GetAllResources();
          shipStatsMessage2.AsteroidGuid = new long?(shipStatsMessage1.AsteroidGuid.Value);
          if (shipStatsMessage2.resourcesOnAsteroid != null)
            flag1 = true;
        }
      }
      if (!flag1)
        return;
      Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) shipStatsMessage2, -1L, (SpaceObject) this);
    }

    public void SendCollision(double vel, double impulse, double time, long otherShipGUID)
    {
      SpaceObjectVessel vessel = Server.Instance.GetVessel(otherShipGUID);
      float num1 = (float) (-impulse / time / 10000.0 * Server.VesselCollisionDamageMultiplier);
      List<SpaceObjectVessel> source1 = new List<SpaceObjectVessel>();
      source1.Add((SpaceObjectVessel) this);
      source1.AddRange((IEnumerable<SpaceObjectVessel>) this.AllDockedVessels);
      double num2 = (double) source1.OrderBy<SpaceObjectVessel, double>((Func<SpaceObjectVessel, double>) (x => MathHelper.RandomNextDouble())).First<SpaceObjectVessel>().ChangeHealthBy(num1, (List<VesselRepairPoint>) null);
      if (vessel != null)
      {
        List<SpaceObjectVessel> source2 = new List<SpaceObjectVessel>();
        source2.Add(vessel);
        source2.AddRange((IEnumerable<SpaceObjectVessel>) vessel.AllDockedVessels);
        double num3 = (double) source2.OrderBy<SpaceObjectVessel, double>((Func<SpaceObjectVessel, double>) (x => MathHelper.RandomNextDouble())).First<SpaceObjectVessel>().ChangeHealthBy(num1, (List<VesselRepairPoint>) null);
      }
      if ((Server.Instance.RunTime - this.lastShipCollisionMessageTime).TotalSeconds <= 0.1)
        return;
      this.lastShipCollisionMessageTime = Server.Instance.RunTime;
      Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) new ShipCollisionMessage()
      {
        CollisionVelocity = (float) vel,
        ShipOne = this.GUID,
        ShipTwo = otherShipGUID
      }, -1L, (SpaceObject) this, (SpaceObject) (otherShipGUID != -1L ? Server.Instance.GetVessel(otherShipGUID) : (SpaceObjectVessel) null));
    }

    public bool DockToShip(VesselDockingPort port, VesselDockingPort dockToPort, Ship dockToShip, bool disableStabilization = true)
    {
      if (dockToPort == null || port == null || port.DockingStatus || dockToPort.DockingStatus)
      {
        Dbg.Error((object) "DockToShip returned at start, check if some port IDs changed", (object) this.DockingPorts, (object) port);
        return false;
      }
      if (disableStabilization)
      {
        if (this.DockedToMainVessel != null && (this.DockedToMainVessel.StabilizeToTargetObj != null || this.DockedToMainVessel.StabilizedToTargetChildren.Count > 0))
          this.DockedToMainVessel.DisableStabilization(true, true);
        else if (this.DockedToMainVessel == null && (this.StabilizeToTargetObj != null || this.StabilizedToTargetChildren.Count > 0))
          this.DisableStabilization(true, true);
        if (dockToShip.DockedToMainVessel != null && (dockToShip.DockedToMainVessel.StabilizeToTargetObj != null || dockToShip.DockedToMainVessel.StabilizedToTargetChildren.Count > 0))
          dockToShip.DockedToMainVessel.DisableStabilization(true, true);
        else if (dockToShip.DockedToMainVessel == null && (dockToShip.StabilizeToTargetObj != null || dockToShip.StabilizedToTargetChildren.Count > 0))
          dockToShip.DisableStabilization(true, true);
      }
      if (this.IsPartOfSpawnSystem)
        SpawnManager.RemoveSpawnSystemObject((SpaceObject) this, true);
      if (dockToShip.IsPartOfSpawnSystem)
        SpawnManager.RemoveSpawnSystemObject((SpaceObject) dockToShip, true);
      port.DockedToID = dockToPort.ID;
      port.DockedVessel = (SpaceObjectVessel) dockToShip;
      port.DockingStatus = true;
      dockToPort.DockedToID = port.ID;
      dockToPort.DockedVessel = (SpaceObjectVessel) this;
      dockToPort.DockingStatus = true;
      Ship.DockUndockPlayerData playerData = Ship.DockUndockPlayerData.GetPlayerData((SpaceObjectVessel) this, (SpaceObjectVessel) dockToShip);
      SpaceObjectVessel ship1 = dockToShip.IsDocked ? dockToShip.DockedToMainVessel : (SpaceObjectVessel) dockToShip;
      SpaceObjectVessel ship2 = this.IsDocked ? this.DockedToMainVessel : (SpaceObjectVessel) this;
      SpaceObjectVessel spaceObjectVessel1 = dockToShip.IsDocked ? dockToShip.DockedToMainVessel : (SpaceObjectVessel) dockToShip;
      SpaceObjectVessel spaceObjectVessel2 = spaceObjectVessel1.AllDockedVessels.Find((Predicate<SpaceObjectVessel>) (m => m.HasSecuritySystem));
      spaceObjectVessel1.RecreateDockedVesselsTree();
      spaceObjectVessel1.DbgLogDockedVesseslTree();
      this.Orbit.SetLastChangeTime(Server.Instance.SolarSystem.CurrentTime);
      Ship dockedToMainVessel = this.DockedToMainVessel as Ship;
      this.DockedToMainVessel.CompoundDistributionManager = new DistributionManager(this.DockedToMainVessel, true);
      this.DockedToMainVessel.CompoundDistributionManager.UpdateSystems(true, true);
      dockedToMainVessel.ResetRelativePositionAndRotations();
      dockedToMainVessel.RecalculateRelativeTransforms((SpaceObjectVessel) null);
      Server.Instance.PhysicsController.RemoveRigidBody(ship1);
      Server.Instance.PhysicsController.RemoveRigidBody(ship2);
      Vector3D vector3D = this.DockedToMainVessel.VesselData.CollidersCenterOffset.ToVector3D();
      dockedToMainVessel.RecalculateCenter();
      this.DockedToMainVessel.Orbit.RelativePosition -= QuaternionD.LookRotation(this.DockedToMainVessel.Forward, this.DockedToMainVessel.Up) * (vector3D - this.DockedToMainVessel.VesselData.CollidersCenterOffset.ToVector3D());
      this.DockedToMainVessel.Orbit.InitFromCurrentStateVectors(Server.Instance.SolarSystem.CurrentTime);
      Server.Instance.PhysicsController.CreateAndAddRigidBody(this.DockedToMainVessel);
      this.DockedToMainVessel.SetPhysicsParameters();
      if (port.MergeExecuters != null && dockToPort.MergeExecuters != null)
      {
        foreach (KeyValuePair<SceneTriggerExecuter, Vector3D> mergeExecuter1 in port.MergeExecuters)
        {
          if (!mergeExecuter1.Key.IsMerged)
          {
            SceneTriggerExecuter sceneTriggerExecuter = (SceneTriggerExecuter) null;
            double num = -1.0;
            foreach (KeyValuePair<SceneTriggerExecuter, Vector3D> mergeExecuter2 in dockToPort.MergeExecuters)
            {
              if (!mergeExecuter2.Key.IsMerged)
              {
                double magnitude = (mergeExecuter1.Value - QuaternionD.AngleAxis(180.0, Vector3D.Up) * mergeExecuter2.Value).Magnitude;
                if ((magnitude < num || num == -1.0) && mergeExecuter1.Key.AreStatesEqual(mergeExecuter2.Key))
                {
                  num = magnitude;
                  sceneTriggerExecuter = mergeExecuter2.Key;
                }
              }
            }
            if (sceneTriggerExecuter != null && num <= port.MergeExecutersDistance)
              sceneTriggerExecuter.MergeWith(mergeExecuter1.Key);
          }
        }
      }
      if (spaceObjectVessel2 != null)
        spaceObjectVessel2.CopyAuthorizedPersonelListToChildren();
      playerData.ModifyPlayersPositionAndRotation();
      return true;
    }

    public bool UndockFromShip(Ship dockedToShip, ref SceneDockingPortDetails details)
    {
      if (this.DockedToVessel == dockedToShip)
      {
        VesselDockingPort port = this.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.DockedVessel == this.DockedToVessel));
        if (port != null)
        {
          VesselDockingPort dockedToPort = this.DockedToVessel.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID == port.DockedToID));
          if (dockedToPort != null)
            return this.UndockFromShip(port, dockedToShip, dockedToPort, ref details);
        }
      }
      return false;
    }

    public bool UndockFromShip(VesselDockingPort port, Ship dockedToShip, VesselDockingPort dockedToPort, ref SceneDockingPortDetails details)
    {
      if (!port.DockingStatus || port.DockedToID == null || port.DockedVessel == null || !(port.DockedVessel is Ship))
        return false;
      if (dockedToShip == null)
        dockedToShip = port.DockedVessel as Ship;
      if (dockedToPort == null)
        dockedToPort = dockedToShip.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == port.DockedToID.InSceneID));
      if (dockedToPort == null)
        return false;
      Ship ship1 = (Ship) null;
      Ship ship2 = (Ship) null;
      details.RelativePositionUpdate = new Dictionary<long, float[]>();
      details.RelativeRotationUpdate = new Dictionary<long, float[]>();
      SpaceObjectVessel ship3 = this.DockedToMainVessel != null ? this.DockedToMainVessel : dockedToShip.DockedToMainVessel;
      QuaternionD quaternionD = QuaternionD.LookRotation(ship3.Forward, ship3.Up);
      Vector3D vector3D = ship3.VesselData.CollidersCenterOffset.ToVector3D();
      Vector3D relativePosition = ship3.Orbit.RelativePosition;
      if (ship3.StabilizeToTargetObj != null)
        ship3.DisableStabilization(true, true);
      port.DockedToID = (VesselObjectID) null;
      port.DockedVessel = (SpaceObjectVessel) null;
      port.DockingStatus = false;
      dockedToPort.DockedToID = (VesselObjectID) null;
      dockedToPort.DockedVessel = (SpaceObjectVessel) null;
      dockedToPort.DockingStatus = false;
      this.DockedVessels.Remove((SpaceObjectVessel) dockedToShip);
      dockedToShip.DockedVessels.Remove((SpaceObjectVessel) this);
      this.DockedToVessel = (SpaceObjectVessel) null;
      dockedToShip.DockedToVessel = (SpaceObjectVessel) null;
      ship3.ResetDockedToVessel();
      Ship.DockUndockPlayerData playerData = Ship.DockUndockPlayerData.GetPlayerData(ship3);
      if (this.DockedToVessel == null)
      {
        this.DockedToMainVessel = (SpaceObjectVessel) null;
        this.AllDockedVessels.Clear();
        this.SetMainVesselForChldren((SpaceObjectVessel) this);
        if (this.AllDockedVessels.Count == 1 && GameScenes.Ranges.IsShip(this.AllDockedVessels[0].SceneID) && !GameScenes.Ranges.IsShip(this.SceneID))
        {
          Ship allDockedVessel = this.AllDockedVessels[0] as Ship;
          allDockedVessel.DockedToMainVessel = (SpaceObjectVessel) null;
          allDockedVessel.AllDockedVessels.Clear();
          allDockedVessel.SetMainVesselForChldren((SpaceObjectVessel) allDockedVessel);
          ship1 = allDockedVessel;
        }
        else
          ship1 = this;
      }
      if (dockedToShip.DockedToVessel == null)
      {
        dockedToShip.DockedToMainVessel = (SpaceObjectVessel) null;
        dockedToShip.AllDockedVessels.Clear();
        dockedToShip.SetMainVesselForChldren((SpaceObjectVessel) dockedToShip);
        if (dockedToShip.AllDockedVessels.Count == 1 && GameScenes.Ranges.IsShip(dockedToShip.AllDockedVessels[0].SceneID) && !GameScenes.Ranges.IsShip(dockedToShip.SceneID))
        {
          Ship allDockedVessel = dockedToShip.AllDockedVessels[0] as Ship;
          allDockedVessel.DockedToMainVessel = (SpaceObjectVessel) null;
          allDockedVessel.AllDockedVessels.Clear();
          allDockedVessel.SetMainVesselForChldren((SpaceObjectVessel) allDockedVessel);
          ship2 = allDockedVessel;
        }
        else
          ship2 = dockedToShip;
      }
      if (ship1 == null && this.DockedToMainVessel != null)
        ship1 = this.DockedToMainVessel as Ship;
      if (ship2 == null && dockedToShip.DockedToMainVessel != null)
        ship2 = dockedToShip.DockedToMainVessel as Ship;
      Vector3D positionFromMainParent1 = ship1.RelativePositionFromMainParent;
      Vector3D positionFromMainParent2 = ship2.RelativePositionFromMainParent;
      ship1.Orbit.CopyDataFrom(ship3.Orbit, Server.Instance.SolarSystem.CurrentTime, true);
      ship2.Orbit.CopyDataFrom(ship3.Orbit, Server.Instance.SolarSystem.CurrentTime, true);
      if (ship1 != ship3)
      {
        ship1.Forward = quaternionD * ship1.RelativeRotationFromMainParent * Vector3D.Forward;
        ship1.Up = quaternionD * ship1.RelativeRotationFromMainParent * Vector3D.Up;
      }
      if (ship2 != ship3)
      {
        ship2.Forward = quaternionD * ship2.RelativeRotationFromMainParent * Vector3D.Forward;
        ship2.Up = quaternionD * ship2.RelativeRotationFromMainParent * Vector3D.Up;
      }
      ship1.RecreateDockedVesselsTree();
      ship2.RecreateDockedVesselsTree();
      Server.Instance.PhysicsController.RemoveRigidBody(ship3);
      ship1.ResetRelativePositionAndRotations();
      ship2.ResetRelativePositionAndRotations();
      ship1.RecalculateRelativeTransforms((SpaceObjectVessel) null);
      ship2.RecalculateRelativeTransforms((SpaceObjectVessel) null);
      ship1.RecalculateCenter();
      ship2.RecalculateCenter();
      details.RelativePositionUpdate.Add(ship1.GUID, positionFromMainParent1.ToFloatArray());
      details.RelativeRotationUpdate.Add(ship1.GUID, QuaternionD.LookRotation(ship1.Forward, ship1.Up).ToFloatArray());
      details.RelativePositionUpdate.Add(ship2.GUID, positionFromMainParent2.ToFloatArray());
      details.RelativeRotationUpdate.Add(ship2.GUID, QuaternionD.LookRotation(ship2.Forward, ship2.Up).ToFloatArray());
      ship1.Orbit.RelativePosition += quaternionD * (-vector3D + positionFromMainParent1 + quaternionD.Inverse() * QuaternionD.LookRotation(ship1.Forward, ship1.Up) * ship1.VesselData.CollidersCenterOffset.ToVector3D());
      ship2.Orbit.RelativePosition += quaternionD * (-vector3D + positionFromMainParent2 + quaternionD.Inverse() * QuaternionD.LookRotation(ship2.Forward, ship2.Up) * ship2.VesselData.CollidersCenterOffset.ToVector3D());
      ship1.Orbit.RelativeVelocity += (ship1.Orbit.RelativePosition - ship2.Orbit.RelativePosition).Normalized * 0.15;
      ship2.Orbit.RelativeVelocity += (ship2.Orbit.RelativePosition - ship1.Orbit.RelativePosition).Normalized * 0.15;
      ship1.Orbit.InitFromCurrentStateVectors(Server.Instance.SolarSystem.CurrentTime);
      ship2.Orbit.InitFromCurrentStateVectors(Server.Instance.SolarSystem.CurrentTime);
      Server.Instance.PhysicsController.CreateAndAddRigidBody((SpaceObjectVessel) ship1);
      ship1.SetPhysicsParameters();
      Server.Instance.PhysicsController.CreateAndAddRigidBody((SpaceObjectVessel) ship2);
      ship2.SetPhysicsParameters();
      ship3.CompoundDistributionManager.UnpairAllDoors();
      ship3.CompoundDistributionManager = (DistributionManager) null;
      if (ship1.AllDockedVessels.Count > 0)
      {
        ship1.CompoundDistributionManager = new DistributionManager((SpaceObjectVessel) ship1, true);
        ship1.CompoundDistributionManager.UpdateSystems(true, true);
      }
      else
      {
        ship1.CompoundDistributionManager = (DistributionManager) null;
        ship1.MainDistributionManager.UpdateSystems(true, true);
      }
      if (ship2.AllDockedVessels.Count > 0)
      {
        ship2.CompoundDistributionManager = new DistributionManager((SpaceObjectVessel) ship2, true);
        ship2.CompoundDistributionManager.UpdateSystems(true, true);
      }
      else
      {
        ship2.CompoundDistributionManager = (DistributionManager) null;
        ship2.MainDistributionManager.UpdateSystems(true, true);
      }
      if (port.MergeExecuters != null)
      {
        foreach (KeyValuePair<SceneTriggerExecuter, Vector3D> mergeExecuter in port.MergeExecuters)
        {
          if (mergeExecuter.Key.IsMerged)
            mergeExecuter.Key.MergeWith((SceneTriggerExecuter) null);
        }
      }
      ship1.DbgLogDockedVesseslTree();
      ship2.DbgLogDockedVesseslTree();
      details.CollidersCenterOffset = ship1.VesselData.CollidersCenterOffset;
      details.CollidersCenterOffsetOther = ship2.VesselData.CollidersCenterOffset;
      foreach (Ship allDockedVessel in ship1.AllDockedVessels)
      {
        details.RelativePositionUpdate.Add(allDockedVessel.GUID, allDockedVessel.RelativePositionFromParent.ToFloatArray());
        details.RelativeRotationUpdate.Add(allDockedVessel.GUID, allDockedVessel.RelativeRotationFromParent.ToFloatArray());
      }
      foreach (Ship allDockedVessel in ship2.AllDockedVessels)
      {
        details.RelativePositionUpdate.Add(allDockedVessel.GUID, allDockedVessel.RelativePositionFromParent.ToFloatArray());
        details.RelativeRotationUpdate.Add(allDockedVessel.GUID, allDockedVessel.RelativeRotationFromParent.ToFloatArray());
      }
      details.VesselOrbit = new OrbitData();
      ship1.Orbit.FillOrbitData(ref details.VesselOrbit, (SpaceObjectVessel) null);
      details.VesselOrbitOther = new OrbitData();
      ship2.Orbit.FillOrbitData(ref details.VesselOrbitOther, (SpaceObjectVessel) null);
      playerData.ModifyPlayersPositionAndRotation();
      return true;
    }

    public override SpawnObjectResponseData GetSpawnResponseData(Player pl)
    {
      if (this.IsDocked && this.DockedToMainVessel is Ship)
        return (this.DockedToMainVessel as Ship).GetSpawnResponseData(pl);
      bool flag = (pl.Position - this.Position).SqrMagnitude > 100000000.0;
      float? nullable = new float?();
      if (this.IsDistresActive && this.IsArenaVessel)
      {
        float num = (float) (SpaceObjectVessel.ArenaRescueTime - (Server.Instance.SolarSystem.CurrentTime - this.DistressActivatedTime));
        if ((double) num > 0.0)
          nullable = new float?(num);
      }
      SpawnShipResponseData shipResponseData = new SpawnShipResponseData();
      long guid = this.GUID;
      shipResponseData.GUID = guid;
      VesselCaps vesselCaps = this.VesselCaps;
      shipResponseData.Caps = vesselCaps;
      VesselData vesselData = this.VesselData;
      shipResponseData.Data = vesselData;
      VesselObjects shipObjects = this.GetShipObjects();
      shipResponseData.VesselObjects = shipObjects;
      int num1 = flag ? 1 : 0;
      shipResponseData.IsDummy = num1 != 0;
      List<DockedVesselsData> dockedVesselsData = this.GetDockedVesselsData();
      shipResponseData.DockedVessels = dockedVesselsData;
      DistressData distressData = this.GetDistressData(true);
      shipResponseData.Distress = distressData;
      double radarSignature = this.RadarSignature;
      shipResponseData.RadarSignature = radarSignature;
      return (SpawnObjectResponseData) shipResponseData;
    }

    public List<DockedVesselsData> GetDockedVesselsData()
    {
      if (this.AllDockedVessels == null || this.AllDockedVessels.Count == 0)
        return (List<DockedVesselsData>) null;
      List<DockedVesselsData> dockedVesselsDataList = new List<DockedVesselsData>();
      foreach (SpaceObjectVessel allDockedVessel in this.AllDockedVessels)
      {
        if (allDockedVessel.ObjectType == SpaceObjectType.Ship)
        {
          Ship ship = allDockedVessel as Ship;
          dockedVesselsDataList.Add(new DockedVesselsData()
          {
            GUID = ship.GUID,
            Caps = ship.VesselCaps,
            Data = ship.VesselData,
            VesselObjects = ship.GetShipObjects()
          });
        }
      }
      return dockedVesselsDataList;
    }

    public override InitializeSpaceObjectMessage GetInitializeMessage()
    {
      InitializeSpaceObjectMessage spaceObjectMessage = new InitializeSpaceObjectMessage();
      spaceObjectMessage.GUID = this.GUID;
      spaceObjectMessage.DynamicObjects = new List<DynamicObjectDetails>();
      foreach (DynamicObject dynamicObject in this.DynamicObjects)
        spaceObjectMessage.DynamicObjects.Add(dynamicObject.GetDetails());
      spaceObjectMessage.Corpses = new List<CorpseDetails>();
      foreach (Corpse corpse in this.Corpses)
      {
        List<CorpseDetails> corpses = spaceObjectMessage.Corpses;
        CorpseDetails corpseDetails = new CorpseDetails();
        corpseDetails.GUID = corpse.GUID;
        corpseDetails.ParentGUID = corpse.Parent == null ? -1L : corpse.Parent.GUID;
        corpseDetails.ParentType = (SpaceObjectType) (corpse.Parent == null ? 0 : (int) corpse.Parent.ObjectType);
        corpseDetails.LocalPosition = corpse.LocalPosition.ToFloatArray();
        corpseDetails.LocalRotation = corpse.LocalRotation.ToFloatArray();
        int num = corpse.IsInsideSpaceObject ? 1 : 0;
        corpseDetails.IsInsideSpaceObject = num != 0;
        Dictionary<byte, RagdollItemData> ragdollDataList = corpse.RagdollDataList;
        corpseDetails.RagdollDataList = ragdollDataList;
        corpses.Add(corpseDetails);
      }
      spaceObjectMessage.Characters = new List<CharacterDetails>();
      foreach (Player player in this.VesselCrew)
        spaceObjectMessage.Characters.Add(player.GetDetails(false));
      spaceObjectMessage.AuxData = (InitializeSpaceObjectAuxData) new InitializeShipAuxData()
      {
        VesselObjects = this.GetShipObjects()
      };
      return spaceObjectMessage;
    }

    public ShipSaveData GetSaveData()
    {
      ShipSaveData shipSaveData = new ShipSaveData();
      shipSaveData.GUID = this.GUID;
      shipSaveData.Forward = this.Forward.ToFloatArray();
      shipSaveData.Up = this.Up.ToFloatArray();
      if (this.Parent != null)
      {
        shipSaveData.ParentGUID = this.Parent.GUID;
        shipSaveData.ParentType = this.Parent.ObjectType;
      }
      return shipSaveData;
    }

    public void LoadSaveData(ShipSaveData setValue)
    {
      this.GUID = setValue.GUID;
      this.Forward = setValue.Forward.ToVector3D();
      this.Up = setValue.Up.ToVector3D();
      if (setValue.ParentGUID == -1L)
        return;
      this.Parent.GUID = setValue.ParentGUID;
    }

    private void FillShipData(GameScenes.SceneID sceneID, List<StructureSceneData> structureSceneDataList, bool loadDynamicObjects = true)
    {
      StructureSceneData structureSceneData = ObjectCopier.DeepCopy<StructureSceneData>(structureSceneDataList.Find((Predicate<StructureSceneData>) (m => (int) m.ItemID == (int) (short) sceneID)), 10);
      if (structureSceneData == null)
        return;
      this.RadarSignature = structureSceneData.RadarSignature;
      this.HasSecuritySystem = structureSceneData.HasSecuritySystem;
      this.MaxHealth = structureSceneData.MaxHealth;
      this.Health = structureSceneData.Health;
      this.DecayRate = structureSceneData.DecayRate;
      if (structureSceneData.SpawnSettings != null)
      {
        foreach (SceneSpawnSettings spawnSetting in structureSceneData.SpawnSettings)
        {
          if (this.CheckTag(spawnSetting.Tag, spawnSetting.Case))
          {
            this.Health = this.MaxHealth * MathHelper.RandomRange(spawnSetting.MinHealth, spawnSetting.MaxHealth);
            break;
          }
        }
      }
      if (structureSceneData.Doors != null && structureSceneData.Doors.Count > 0)
      {
        foreach (DoorData door1 in structureSceneData.Doors)
        {
          List<Door> doors = this.Doors;
          Door door2 = new Door();
          door2.ID = new VesselObjectID(this.GUID, door1.InSceneID);
          int num1 = door1.IsSealable ? 1 : 0;
          door2.IsSealable = num1 != 0;
          int num2 = door1.HasPower ? 1 : 0;
          door2.HasPower = num2 != 0;
          int num3 = door1.IsLocked ? 1 : 0;
          door2.IsLocked = num3 != 0;
          int num4 = door1.IsOpen ? 1 : 0;
          door2.IsOpen = num4 != 0;
          int num5 = door1.LockedAutoToggle ? 1 : 0;
          door2.LockedAutoToggle = num5 != 0;
          double passageArea = (double) door1.PassageArea;
          door2.PassageArea = (float) passageArea;
          Vector3D vector3D = door1.PositionRelativeToDockingPort.ToVector3D();
          door2.PositionRelativeToDockingPort = vector3D;
          doors.Add(door2);
        }
      }
      if (structureSceneData.CargoBays != null && structureSceneData.CargoBays.Count > 0)
      {
        foreach (CargoBayData cargoBay in structureSceneData.CargoBays)
          this.CargoBays.Add(new CargoBay((SpaceObjectVessel) this, cargoBay.CargoCompartments)
          {
            InSceneID = cargoBay.InSceneID
          });
      }
      if (structureSceneData.NameTags != null && structureSceneData.NameTags.Count > 0)
      {
        foreach (NameTagData nameTag in structureSceneData.NameTags)
          this.NameTags.Add(nameTag);
      }
      if (structureSceneData.Rooms != null && structureSceneData.Rooms.Count > 0)
      {
        foreach (RoomData room1 in structureSceneData.Rooms)
        {
          Room room2 = new Room();
          room2.ID = new VesselObjectID(this.GUID, room1.InSceneID);
          room2.ParentVessel = (SpaceObjectVessel) this;
          int num1 = room1.UseGravity ? 1 : 0;
          room2.UseGravity = num1 != 0;
          double volume = (double) room1.Volume;
          room2.Volume = (float) volume;
          int num2 = room1.GravityAutoToggle ? 1 : 0;
          room2.GravityAutoToggle = num2 != 0;
          double airPressure = (double) room1.AirPressure;
          room2.AirPressure = (float) airPressure;
          double airQuality = (double) room1.AirQuality;
          room2.AirQuality = (float) airQuality;
          Room room3 = room2;
          room3.AirTank = new RoomAirTank(room3);
          this.Rooms.Add(room3);
        }
      }
      if (structureSceneData.SubSystems != null && structureSceneData.SubSystems.Count > 0)
      {
        foreach (SubSystemData subSystem in structureSceneData.SubSystems)
          this.Systems.Add((VesselComponent) this.createSubSystem(subSystem));
      }
      if (structureSceneData.Generators != null && structureSceneData.Generators.Count > 0)
      {
        foreach (GeneratorData generator in structureSceneData.Generators)
          this.Systems.Add((VesselComponent) this.createGenerator(generator));
      }
      if (structureSceneData.RepairPoints != null && structureSceneData.RepairPoints.Count > 0)
      {
        float maxHealth = (float) System.Math.Ceiling((double) this.MaxHealth / (double) structureSceneData.RepairPoints.Count);
        foreach (VesselRepairPointData repairPoint in structureSceneData.RepairPoints)
          this.RepairPoints.Add(new VesselRepairPoint((SpaceObjectVessel) this, repairPoint, maxHealth));
        float health = this.Health;
        foreach (VesselRepairPoint vesselRepairPoint in (IEnumerable<VesselRepairPoint>) ((IEnumerable<VesselRepairPoint>) this.RepairPoints.ToArray()).OrderBy<VesselRepairPoint, int>((Func<VesselRepairPoint, int>) (m => MathHelper.RandomNextInt())))
        {
          if ((double) health > (double) vesselRepairPoint.MaxHealth)
          {
            vesselRepairPoint.Health = vesselRepairPoint.MaxHealth;
            health -= vesselRepairPoint.MaxHealth;
          }
          else
          {
            vesselRepairPoint.Health = health;
            break;
          }
        }
      }
      if (structureSceneData.SceneTriggerExecuters != null && structureSceneData.SceneTriggerExecuters.Count > 0)
      {
        foreach (SceneTriggerExecuterData sceneTriggerExecuter1 in structureSceneData.SceneTriggerExecuters)
        {
          if ((uint) sceneTriggerExecuter1.TagAction > 0U)
          {
            bool flag = this.CheckTag(sceneTriggerExecuter1.Tags, SpawnSettingsCase.EnableIf);
            if (sceneTriggerExecuter1.TagAction == TagAction.EnableIfTagIs && !flag || sceneTriggerExecuter1.TagAction == TagAction.DisableIfTagIs & flag)
              continue;
          }
          SceneTriggerExecuter sceneTriggerExecuter2 = new SceneTriggerExecuter()
          {
            InSceneID = sceneTriggerExecuter1.InSceneID,
            ParentShip = this
          };
          sceneTriggerExecuter2.SetStates(sceneTriggerExecuter1.States, sceneTriggerExecuter1.DefaultStateID);
          if (sceneTriggerExecuter1.ProximityTriggers != null && sceneTriggerExecuter1.ProximityTriggers.Count > 0)
          {
            sceneTriggerExecuter2.ProximityTriggers = new Dictionary<int, SceneTriggerProximity>();
            foreach (SceneTriggerProximityData proximityTrigger in sceneTriggerExecuter1.ProximityTriggers)
              sceneTriggerExecuter2.ProximityTriggers.Add(proximityTrigger.TriggerID, new SceneTriggerProximity()
              {
                TriggerID = proximityTrigger.TriggerID,
                ActiveStateID = proximityTrigger.ActiveStateID,
                InactiveStateID = proximityTrigger.InactiveStateID,
                ObjectsInTrigger = new List<long>()
              });
          }
          this.SceneTriggerExecuters.Add(sceneTriggerExecuter2);
        }
      }
      if (structureSceneData.DockingPorts != null && structureSceneData.DockingPorts.Count > 0)
      {
        foreach (SceneDockingPortData dockingPort in structureSceneData.DockingPorts)
        {
          VesselObjectID vesselObjectId = new VesselObjectID(this.GUID, dockingPort.InSceneID);
          VesselDockingPort vesselDockingPort1 = new VesselDockingPort();
          vesselDockingPort1.ParentVessel = (SpaceObjectVessel) this;
          vesselDockingPort1.ID = vesselObjectId;
          vesselDockingPort1.DockedVessel = (SpaceObjectVessel) null;
          vesselDockingPort1.DockingStatus = false;
          vesselDockingPort1.DockedToID = (VesselObjectID) null;
          Vector3D vector3D = dockingPort.Position.ToVector3D();
          vesselDockingPort1.Position = vector3D;
          QuaternionD quaternionD = dockingPort.Rotation.ToQuaternionD();
          vesselDockingPort1.Rotation = quaternionD;
          int[] doorsIds = dockingPort.DoorsIDs;
          vesselDockingPort1.DoorsIDs = doorsIds;
          double doorPairingDistance = (double) dockingPort.DoorPairingDistance;
          vesselDockingPort1.DoorPairingDistance = (float) doorPairingDistance;
          int orderId = dockingPort.OrderID;
          vesselDockingPort1.OrderID = orderId;
          Dictionary<SceneTriggerExecuter, Vector3D> dictionary = new Dictionary<SceneTriggerExecuter, Vector3D>();
          vesselDockingPort1.MergeExecuters = dictionary;
          double executerDistance = (double) dockingPort.MergeExecuterDistance;
          vesselDockingPort1.MergeExecutersDistance = executerDistance;
          VesselDockingPort vesselDockingPort2 = vesselDockingPort1;
          foreach (SceneDockingPortExecuterMerge mergeExecuter in dockingPort.MergeExecuters)
          {
            SceneDockingPortExecuterMerge mer = mergeExecuter;
            SceneTriggerExecuter key = this.SceneTriggerExecuters.Find((Predicate<SceneTriggerExecuter>) (m => m.InSceneID == mer.InSceneID));
            vesselDockingPort2.MergeExecuters.Add(key, mer.Position.ToVector3D());
          }
          this.DockingPorts.Add(vesselDockingPort2);
        }
      }
      if (structureSceneData.SpawnPoints != null && structureSceneData.SpawnPoints.Count > 0)
      {
        foreach (SpawnPointData spawnPoint in structureSceneData.SpawnPoints)
        {
          SpawnPointData spd = spawnPoint;
          if ((uint) spd.TagAction > 0U)
          {
            bool flag = this.CheckTag(spd.Tags, SpawnSettingsCase.EnableIf);
            if (spd.TagAction == TagAction.EnableIfTagIs && !flag || spd.TagAction == TagAction.DisableIfTagIs & flag)
              continue;
          }
          SceneTriggerExecuter sceneTriggerExecuter = spd.ExecuterID > 0 ? this.SceneTriggerExecuters.Find((Predicate<SceneTriggerExecuter>) (m => m.InSceneID == spd.ExecuterID)) : (SceneTriggerExecuter) null;
          ShipSpawnPoint shipSpawnPoint1 = new ShipSpawnPoint();
          shipSpawnPoint1.SpawnPointID = spd.InSceneID;
          shipSpawnPoint1.Type = spd.Type;
          shipSpawnPoint1.Executer = sceneTriggerExecuter;
          shipSpawnPoint1.ExecuterStateID = spd.ExecuterStateID;
          shipSpawnPoint1.Ship = this;
          int num = 0;
          shipSpawnPoint1.State = (SpawnPointState) num;
          // ISSUE: variable of the null type
          __Null local = null;
          shipSpawnPoint1.Player = (Player) local;
          ShipSpawnPoint shipSpawnPoint2 = shipSpawnPoint1;
          if (shipSpawnPoint2.Executer != null)
          {
            sceneTriggerExecuter.SpawnPoint = shipSpawnPoint2;
            shipSpawnPoint2.ExecuterOccupiedStateIDs = new List<int>((IEnumerable<int>) spd.ExecuterOccupiedStateIDs);
          }
          this.SpawnPoints.Add(shipSpawnPoint2);
        }
      }
      if (structureSceneData.AttachPoints != null && structureSceneData.AttachPoints.Count > 0)
      {
        foreach (BaseAttachPointData attachPoint in structureSceneData.AttachPoints)
        {
          this.AttachPointsTypes[new VesselObjectID(this.GUID, attachPoint.InSceneID)] = attachPoint.AttachPointType;
          if (attachPoint.AttachPointType == AttachPointType.Simple || attachPoint.AttachPointType == AttachPointType.MachineryPartSlot)
          {
            Dictionary<int, VesselAttachPoint> attachPoints = this.AttachPoints;
            int inSceneId1 = attachPoint.InSceneID;
            VesselAttachPoint vesselAttachPoint = new VesselAttachPoint();
            vesselAttachPoint.Vessel = (SpaceObjectVessel) this;
            int inSceneId2 = attachPoint.InSceneID;
            vesselAttachPoint.InSceneID = inSceneId2;
            int attachPointType = (int) attachPoint.AttachPointType;
            vesselAttachPoint.Type = (AttachPointType) attachPointType;
            // ISSUE: variable of the null type
            __Null local = null;
            vesselAttachPoint.Item = (Item) local;
            attachPoints.Add(inSceneId1, vesselAttachPoint);
          }
        }
      }
      if (structureSceneData.SpawnObjectChanceData != null && structureSceneData.SpawnObjectChanceData.Count > 0)
      {
        foreach (SpawnObjectsWithChanceData objectsWithChanceData in structureSceneData.SpawnObjectChanceData)
        {
          float num = (float) MathHelper.RandomNextDouble();
          this.SpawnChance.Add(new SpaceObjectVessel.SpawnObjectsWithChance()
          {
            InSceneID = objectsWithChanceData.InSceneID,
            Chance = num
          });
        }
      }
      if (!loadDynamicObjects || structureSceneData.DynamicObjects == null || structureSceneData.DynamicObjects.Count <= 0)
        return;
      foreach (DynamicObjectSceneData dynamicObject1 in structureSceneData.DynamicObjects)
      {
        double num1 = MathHelper.RandomNextDouble();
        bool flag = false;
        float num2 = -1f;
        float max = -1f;
        float min = -1f;
        float num3 = 1f;
        if (Server.SetupType != Server.ServerSetupType.Default && (dynamicObject1.SpawnSettings == null || dynamicObject1.SpawnSettings.Length == 0))
          flag = true;
        if (dynamicObject1.SpawnSettings != null && (uint) dynamicObject1.SpawnSettings.Length > 0U)
        {
          foreach (DynaminObjectSpawnSettings spawnSetting in dynamicObject1.SpawnSettings)
          {
            if ((Server.SetupType != Server.ServerSetupType.Default || !spawnSetting.Tag.IsNullOrEmpty()) && this.CheckTag(spawnSetting.Tag, spawnSetting.Case) && ((double) spawnSetting.SpawnChance < 0.0 || (double) spawnSetting.SpawnChance > num1))
            {
              flag = true;
              num2 = spawnSetting.RespawnTime;
              max = spawnSetting.MaxHealth;
              min = spawnSetting.MinHealth;
              num3 = spawnSetting.WearMultiplier;
              break;
            }
          }
        }
        if (flag)
        {
          DynamicObject dynamicObject2 = new DynamicObject(dynamicObject1, (SpaceObject) this, -1L);
          dynamicObject2.RespawnTime = num2;
          dynamicObject2.SpawnMaxHealth = max;
          dynamicObject2.SpawnMinHealth = min;
          dynamicObject2.SpawnWearMultiplier = num3;
          if (dynamicObject2.Item != null && dynamicObject2.Item != null && (double) max >= 0.0 && (double) min >= 0.0)
          {
            IDamageable damageable = (IDamageable) dynamicObject2.Item;
            damageable.Health = (float) (int) ((double) damageable.MaxHealth * (double) MathHelper.Clamp(MathHelper.RandomRange(min, max), 0.0f, 1f));
          }
          AttachPointDetails data = (AttachPointDetails) null;
          if (dynamicObject1.AttachPointInSceneId > 0 && dynamicObject2.Item != null)
          {
            data = new AttachPointDetails()
            {
              InSceneID = dynamicObject1.AttachPointInSceneId
            };
            dynamicObject2.Item.SetAttachPoint(data);
          }
          if (dynamicObject2.Item is MachineryPart)
            (dynamicObject2.Item as MachineryPart).WearMultiplier = num3;
          dynamicObject2.APDetails = data;
        }
      }
    }

    private void CreateShipData(string shipRegistration, string shipTag, GameScenes.SceneID shipItemID, bool loadDynamicObjects = true)
    {
      this.VesselData = new VesselData()
      {
        Id = this.GUID,
        VesselRegistration = shipRegistration,
        VesselName = "",
        Tag = shipTag,
        ServerId = Server.Instance.NetworkController.ServerID,
        SceneID = shipItemID
      };
      this.FillShipData(this.VesselData.SceneID, StaticData.StructuresDataList, loadDynamicObjects);
      this.ReadBoundsAndMass(this.VesselData.SceneID, Vector3D.Zero);
      this.RecalculateCenter();
    }

    private void ReadBoundsAndMass(GameScenes.SceneID sceneID, Vector3D connectionOffset)
    {
      StructureSceneData structureSceneData = StaticData.StructuresDataList.Find((Predicate<StructureSceneData>) (m => (int) m.ItemID == (int) (short) sceneID));
      if (structureSceneData == null)
        return;
      Vector3D vector3D1 = connectionOffset;
      this.Mass = this.Mass + ((double) structureSceneData.Mass > 0.0 ? (double) structureSceneData.Mass : 1.0);
      this.HeatCollectionFactor = structureSceneData.HeatCollectionFactor;
      this.HeatDissipationFactor = structureSceneData.HeatDissipationFactor;
      if (structureSceneData.Colliders == null)
        return;
      if (structureSceneData.Colliders.PrimitiveCollidersData != null && structureSceneData.Colliders.PrimitiveCollidersData.Count > 0)
      {
        foreach (PrimitiveColliderData primitiveColliderData1 in structureSceneData.Colliders.PrimitiveCollidersData)
        {
          List<VesselPrimitiveColliderData> primitiveCollidersData = this.PrimitiveCollidersData;
          VesselPrimitiveColliderData primitiveColliderData2 = new VesselPrimitiveColliderData();
          primitiveColliderData2.Type = primitiveColliderData1.Type;
          Vector3D vector3D2 = primitiveColliderData1.Center.ToVector3D() + vector3D1;
          primitiveColliderData2.CenterPosition = vector3D2;
          Vector3D vector3D3 = primitiveColliderData1.Size.ToVector3D();
          primitiveColliderData2.Bounds = vector3D3;
          int num1 = primitiveColliderData1.AffectingCenterOfMass ? 1 : 0;
          primitiveColliderData2.AffectingCenterOfMass = num1 != 0;
          int colliderIndex = this.ColliderIndex;
          this.ColliderIndex = colliderIndex + 1;
          long num2 = (long) colliderIndex;
          primitiveColliderData2.ColliderID = num2;
          primitiveCollidersData.Add(primitiveColliderData2);
        }
      }
      if (structureSceneData.Colliders.MeshCollidersData != null)
      {
        foreach (MeshData meshData in structureSceneData.Colliders.MeshCollidersData)
        {
          List<VesselMeshColliderData> meshCollidersData = this.MeshCollidersData;
          VesselMeshColliderData meshColliderData = new VesselMeshColliderData();
          int num1 = meshData.AffectingCenterOfMass ? 1 : 0;
          meshColliderData.AffectingCenterOfMass = num1 != 0;
          int colliderIndex = this.ColliderIndex;
          this.ColliderIndex = colliderIndex + 1;
          long num2 = (long) colliderIndex;
          meshColliderData.ColliderID = num2;
          int[] indices = meshData.Indices;
          meshColliderData.Indices = indices;
          Vector3[] vertices = meshData.Vertices.GetVertices();
          meshColliderData.Vertices = vertices;
          Vector3D vector3D2 = meshData.CenterPosition.ToVector3D() + vector3D1;
          meshColliderData.CenterPosition = vector3D2;
          Vector3D vector3D3 = meshData.Bounds.ToVector3D();
          meshColliderData.Bounds = vector3D3;
          QuaternionD quaternionD = meshData.Rotation.ToQuaternionD();
          meshColliderData.Rotation = quaternionD;
          Vector3D vector3D4 = meshData.Scale.ToVector3D();
          meshColliderData.Scale = vector3D4;
          meshCollidersData.Add(meshColliderData);
        }
      }
    }

    public void ResetRelativePositionAndRotations()
    {
      this.RelativePositionFromParent = Vector3D.Zero;
      this.RelativeRotationFromParent = QuaternionD.Identity;
      this.RelativePositionFromMainParent = Vector3D.Zero;
      this.RelativeRotationFromMainParent = QuaternionD.Identity;
    }

    public void RecalculateRelativeTransforms(SpaceObjectVessel parentVessel)
    {
      foreach (VesselDockingPort dockingPort in this.DockingPorts)
      {
        VesselDockingPort port = dockingPort;
        if (port.DockingStatus && port.DockedVessel != parentVessel)
        {
          VesselDockingPort childPort = port.DockedVessel.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == port.DockedToID.InSceneID));
          Vector3D relativePosition;
          QuaternionD relativeRotation;
          Ship.CalculateRelativeTransform(port, childPort, out relativePosition, out relativeRotation);
          port.DockedVessel.RelativePositionFromParent = relativePosition;
          port.DockedVessel.RelativeRotationFromParent = relativeRotation;
          if (this.IsDocked)
          {
            port.DockedVessel.RelativePositionFromMainParent = this.RelativePositionFromMainParent + this.RelativeRotationFromMainParent * relativePosition;
            port.DockedVessel.RelativeRotationFromMainParent = this.RelativeRotationFromMainParent * relativeRotation;
          }
          else
          {
            port.DockedVessel.RelativePositionFromMainParent = relativePosition;
            port.DockedVessel.RelativeRotationFromMainParent = relativeRotation;
          }
          (port.DockedVessel as Ship).RecalculateRelativeTransforms((SpaceObjectVessel) this);
        }
      }
    }

    public static void CalculateRelativeTransform(VesselDockingPort parentPort, VesselDockingPort childPort, out Vector3D relativePosition, out QuaternionD relativeRotation)
    {
      relativeRotation = parentPort.Rotation * QuaternionD.AngleAxis(180.0, Vector3D.Up) * QuaternionD.Inverse(childPort.Rotation);
      relativePosition = parentPort.Position - relativeRotation * childPort.Position;
    }

    public void RecalculateCenter()
    {
      if (this.IsDocked)
        return;
      Vector3 maxValue = Vector3.Zero;
      Vector3 minValue = Vector3.Zero;
      BulletPhysicsController.ComplexBoundCalculation((SpaceObjectVessel) this, out minValue, out maxValue);
      Vector3D v = (minValue + maxValue).ToVector3D() / 2.0;
      Vector3D vector3D = v - (this.VesselData.CollidersCenterOffset != null ? this.VesselData.CollidersCenterOffset.ToVector3D() : Vector3D.Zero);
      this.VesselData.CollidersCenterOffset = v.ToFloatArray();
      if (this.AllDockedVessels.Count <= 0)
        return;
      foreach (SpaceObjectVessel allDockedVessel in this.AllDockedVessels)
        allDockedVessel.VesselData.CollidersCenterOffset = Vector3D.Zero.ToFloatArray();
    }

    public static Ship CreateNewShip(GameScenes.SceneID sceneID, string registration = "", long shipID = -1, List<long> nearArtificialBodyGUIDs = null, List<long> celestialBodyGUIDs = null, Vector3D? positionOffset = null, Vector3D? velocityAtPosition = null, QuaternionD? localRotation = null, string vesselTag = "", bool checkPosition = true, double distanceFromSurfacePercMin = 0.03, double distanceFromSurfacePercMax = 0.3, SpawnRuleOrbit spawnRuleOrbit = null, double celestialBodyDeathDistanceMultiplier = 1.5, double artificialBodyDistanceCheck = 100.0)
    {
      Vector3D position = Vector3D.Zero;
      Vector3D velocity = Vector3D.Zero;
      Vector3D forward = Vector3D.Forward;
      Vector3D up = Vector3D.Up;
      Ship ship = new Ship(shipID < 0L ? GUIDFactory.NextVesselGUID() : shipID, false, position, velocity, forward, up);
      ship.CreateShipData(registration, vesselTag, sceneID, true);
      ship.DistributionManager = new DistributionManager((SpaceObjectVessel) ship, false);
      ship.fillVesselCaps();
      Server.Instance.PhysicsController.CreateAndAddRigidBody((SpaceObjectVessel) ship);
      Server.Instance.SolarSystem.GetSpawnPosition(SpaceObjectType.Ship, ship.Radius, checkPosition, out position, out velocity, out forward, out up, nearArtificialBodyGUIDs, celestialBodyGUIDs, positionOffset, velocityAtPosition, localRotation, distanceFromSurfacePercMin, distanceFromSurfacePercMax, spawnRuleOrbit, celestialBodyDeathDistanceMultiplier, artificialBodyDistanceCheck);
      ship.InitializeOrbit(position, velocity, forward, up);
      if (registration.IsNullOrEmpty())
        ship.VesselData.VesselRegistration = Server.NameGenerator.GenerateObjectRegistration(SpaceObjectType.Ship, ship.Orbit.Parent.CelestialBody, sceneID);
      foreach (DynamicObject dynamicObject in ship.DynamicObjects)
      {
        if (dynamicObject.Item != null && dynamicObject.Item.AttachPointKey != null)
        {
          VesselComponent componentByPartSlot = ship.MainDistributionManager.GetVesselComponentByPartSlot(dynamicObject.Item.AttachPointKey);
          if (componentByPartSlot != null && dynamicObject.Item is MachineryPart)
            componentByPartSlot.FitPartToSlot(dynamicObject.Item.AttachPointKey, (MachineryPart) dynamicObject.Item);
        }
      }
      ship.MainDistributionManager.UpdateSystems(true, true);
      Server.Instance.Add((SpaceObjectVessel) ship);
      ship.SetPhysicsParameters();
      return ship;
    }

    public VesselObjects GetShipObjects()
    {
      VesselObjects vesselObjects = new VesselObjects();
      vesselObjects.MiscStatuses = new VesselMiscStatuses();
      vesselObjects.MiscStatuses.IsEngineOnline = this.isEngineOnline;
      if (this.CurrentCourse != null)
      {
        vesselObjects.MiscStatuses.IsManeuverActive = this.CurrentCourse.IsInProgress;
        vesselObjects.MiscStatuses.EndTimeOfCourse = new float?((float) this.CurrentCourse.EndSolarSystemTime);
      }
      else
        vesselObjects.MiscStatuses.IsManeuverActive = false;
      vesselObjects.MiscStatuses.IsMatchedToTarget = this.StabilizeToTargetObj != null;
      vesselObjects.MiscStatuses.Distress = this.GetDistressData(true);
      vesselObjects.SecurityData = this.GetVesselSecurityData(false);
      vesselObjects.SubSystems = this.DistributionManager.GetSubSystemsDetails(false, -1L);
      vesselObjects.Generators = this.DistributionManager.GetGeneratorsDetails(false, -1L);
      vesselObjects.RoomTriggers = this.DistributionManager.GetRoomsDetails(false, -1L);
      vesselObjects.ResourceContainers = this.DistributionManager.GetResourceContainersDetails(false, -1L);
      vesselObjects.RepairPoints = this.GetVesselRepairPointsDetails(false);
      vesselObjects.NameTags = this.NameTags;
      vesselObjects.Doors = this.DistributionManager.GetDoorsDetails(false, -1L);
      vesselObjects.SceneTriggerExecuters = new List<SceneTriggerExecuterDetails>();
      if (this.SceneTriggerExecuters != null && this.SceneTriggerExecuters.Count > 0)
      {
        foreach (SceneTriggerExecuter sceneTriggerExecuter in this.SceneTriggerExecuters)
          vesselObjects.SceneTriggerExecuters.Add(new SceneTriggerExecuterDetails()
          {
            InSceneID = sceneTriggerExecuter.InSceneID,
            CurrentStateID = sceneTriggerExecuter.StateID,
            NewStateID = sceneTriggerExecuter.StateID,
            PlayerThatActivated = sceneTriggerExecuter.PlayerThatActivated
          });
      }
      vesselObjects.DockingPorts = new List<SceneDockingPortDetails>();
      if (this.DockingPorts != null && this.DockingPorts.Count > 0)
      {
        foreach (VesselDockingPort dockingPort in this.DockingPorts)
        {
          if (!dockingPort.DockingStatus || dockingPort.DockingStatus && dockingPort.DockedVessel == this.DockedToVessel && this.IsDocked)
          {
            List<SceneDockingPortDetails> dockingPorts = vesselObjects.DockingPorts;
            SceneDockingPortDetails dockingPortDetails = new SceneDockingPortDetails();
            dockingPortDetails.ID = dockingPort.ID;
            dockingPortDetails.DockedToID = dockingPort.DockedToID;
            int num1 = dockingPort.Locked ? 1 : 0;
            dockingPortDetails.Locked = num1 != 0;
            int num2 = dockingPort.DockingStatus ? 1 : 0;
            dockingPortDetails.DockingStatus = num2 != 0;
            float[] floatArray1 = this.RelativePositionFromParent.ToFloatArray();
            dockingPortDetails.RelativePosition = floatArray1;
            float[] floatArray2 = this.RelativeRotationFromParent.ToFloatArray();
            dockingPortDetails.RelativeRotation = floatArray2;
            float[] numArray = this.IsDocked ? this.DockedToMainVessel.VesselData.CollidersCenterOffset : this.VesselData.CollidersCenterOffset;
            dockingPortDetails.CollidersCenterOffset = numArray;
            List<ExecuterMergeDetails> mergedExecuters = dockingPort.GetMergedExecuters((VesselDockingPort) null);
            dockingPortDetails.ExecutersMerge = mergedExecuters;
            List<PairedDoorsDetails> pairedDoors = this.GetPairedDoors(dockingPort);
            dockingPortDetails.PairedDoors = pairedDoors;
            dockingPorts.Add(dockingPortDetails);
          }
        }
      }
      vesselObjects.CargoBays = new List<CargoBayDetails>();
      foreach (CargoBay cargoBay in this.CargoBays)
        vesselObjects.CargoBays.Add(cargoBay.GetDetails());
      vesselObjects.SpawnWithChance = new List<SpawnObjectsWithChanceDetails>();
      foreach (SpaceObjectVessel.SpawnObjectsWithChance objectsWithChance in this.SpawnChance)
        vesselObjects.SpawnWithChance.Add(new SpawnObjectsWithChanceDetails()
        {
          InSceneID = objectsWithChance.InSceneID,
          Chance = objectsWithChance.Chance
        });
      vesselObjects.SpawnPoints = new List<SpawnPointStats>();
      foreach (ShipSpawnPoint spawnPoint in this.SpawnPoints)
        vesselObjects.SpawnPoints.Add(new SpawnPointStats()
        {
          InSceneID = spawnPoint.SpawnPointID,
          NewState = new SpawnPointState?(spawnPoint.State),
          NewType = new SpawnPointType?(spawnPoint.Type),
          PlayerGUID = new long?(spawnPoint.Player != null ? spawnPoint.Player.FakeGuid : -1L),
          PlayerName = spawnPoint.Player != null ? spawnPoint.Player.Name : (string) null,
          PlayerSteamID = spawnPoint.Player != null ? spawnPoint.Player.SteamId : (string) null
        });
      return vesselObjects;
    }

    public override void AddPlayerToCrew(Player pl)
    {
      if (this.VesselCrew.Contains(pl))
        return;
      this.VesselCrew.Add(pl);
      this.RemovePlayerFromExecuters(pl);
    }

    public override void RemovePlayerFromCrew(Player pl, bool checkDetails = false)
    {
      this.VesselCrew.Remove(pl);
      if (!checkDetails)
        return;
      this.RemovePlayerFromExecuters(pl);
    }

    public void RemovePlayerFromExecuters(Player pl)
    {
      List<SceneTriggerExecuterDetails> triggerExecuterDetailsList = new List<SceneTriggerExecuterDetails>();
      foreach (SceneTriggerExecuter sceneTriggerExecuter in this.SceneTriggerExecuters)
      {
        SceneTriggerExecuterDetails triggerExecuterDetails = sceneTriggerExecuter.RemovePlayerFromExecuter(pl) ?? sceneTriggerExecuter.RemovePlayerFromProximity(pl);
        if (triggerExecuterDetails != null)
          triggerExecuterDetailsList.Add(triggerExecuterDetails);
      }
      if (triggerExecuterDetailsList.Count <= 0)
        return;
      ShipStatsMessage shipStatsMessage = new ShipStatsMessage();
      shipStatsMessage.VesselObjects = new VesselObjects();
      shipStatsMessage.GUID = this.GUID;
      shipStatsMessage.VesselObjects.SceneTriggerExecuters = triggerExecuterDetailsList;
      this.ShipStatsMessageListener((NetworkData) shipStatsMessage);
    }

    public override bool HasPlayerInCrew(Player pl)
    {
      return this.VesselCrew.Contains(pl);
    }

    public void RemovePlayerFromRoom(Player pl)
    {
      this.MainDistributionManager.RemovePlayerFromRoom(pl);
    }

    public override void UpdateTimers(double deltaTime)
    {
      base.UpdateTimers(deltaTime);
      if (this.VesselCrew.Count > 0)
        this.Temperature = this.SpaceExposureTemperature(this.Temperature, this.HeatCollectionFactor, this.HeatDissipationFactor, (float) this.Mass, deltaTime);
      this.systemsUpdateTimer = this.systemsUpdateTimer + deltaTime;
    }

    public override void UpdateVesselSystems()
    {
      if (this.DockedToMainVessel == null)
        this.MainDistributionManager.UpdateSystems(false, false);
      ShipStatsMessage shipStatsMessage = new ShipStatsMessage();
      shipStatsMessage.GUID = this.GUID;
      shipStatsMessage.Temperature = new float?(this.Temperature);
      shipStatsMessage.Health = new float?(this.Health);
      shipStatsMessage.VesselObjects = new VesselObjects();
      shipStatsMessage.VesselObjects.SubSystems = this.DistributionManager.GetSubSystemsDetails(true, this.GUID);
      shipStatsMessage.VesselObjects.Generators = this.DistributionManager.GetGeneratorsDetails(true, this.GUID);
      shipStatsMessage.VesselObjects.RoomTriggers = this.DistributionManager.GetRoomsDetails(true, this.GUID);
      shipStatsMessage.VesselObjects.ResourceContainers = this.DistributionManager.GetResourceContainersDetails(true, this.GUID);
      shipStatsMessage.VesselObjects.RepairPoints = this.GetVesselRepairPointsDetails(true);
      shipStatsMessage.VesselObjects.Doors = this.DistributionManager.GetDoorsDetails(true, this.GUID);
      if (this.isEngineOnline && this.DistributionManager.Engine != null && this.DistributionManager.Engine.Status != SystemStatus.OnLine)
        shipStatsMessage.EngineOnline = new bool?(false);
      Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) shipStatsMessage, -1L, (SpaceObject) this);
      foreach (DynamicObject dynamicObject in this.DynamicObjects.InnerList.FindAll((Predicate<DynamicObject>) (x =>
      {
        if (x.Item != null)
          return (uint) x.Item.AttachPointType > 0U;
        return false;
      })))
      {
        if (dynamicObject.Item.AttachPointType == AttachPointType.BatteryRechargePoint)
        {
          if (dynamicObject.Item is Battery)
          {
            Battery battery = dynamicObject.Item as Battery;
            battery.ChangeQuantity(battery.ChargeAmount);
          }
          else if (dynamicObject.Item is Weapon)
          {
            Weapon weapon = dynamicObject.Item as Weapon;
            weapon.ChangeQuantity(weapon.ChargeAmount, false);
          }
          else if (dynamicObject.Item is Jetpack)
          {
            Jetpack jetpack = dynamicObject.Item as Jetpack;
            jetpack.ChangePowerQuantity(jetpack.ChargeAmount);
          }
        }
      }
    }

    public override void Destroy()
    {
      Server.Instance.PhysicsController.RemoveRigidBody((SpaceObjectVessel) this);
      this.DisconectListener();
      base.Destroy();
    }

    private void DisconectListener()
    {
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (ShipStatsMessage), new EventSystem.NetworkDataDelegate(this.ShipStatsMessageListener));
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (ManeuverCourseRequest), new EventSystem.NetworkDataDelegate(this.ManeuverCourseRequestListener));
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (DistressCallRequest), new EventSystem.NetworkDataDelegate(this.DistressCallRequestListener));
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (VesselRequest), new EventSystem.NetworkDataDelegate(this.VesselRequestListener));
      Server.Instance.NetworkController.EventSystem.RemoveListener(typeof (VesselSecurityRequest), new EventSystem.NetworkDataDelegate(this.VesselSecurityRequestListener));
    }

    ~Ship()
    {
      this.DisconectListener();
    }

    public void DampenRotation(double timeDelta, bool dampenX, bool dampenY, bool dampenZ, double stabilizationMultiplier = 1.0)
    {
      if (!dampenX && !dampenY && !dampenZ)
        return;
      double num = (double) this.VesselCaps.RotationStabilizationDeacceleration * stabilizationMultiplier * timeDelta * Server.RCS_ROTATION_MULTIPLIER;
      Vector3D rotation = this.Rotation;
      if (dampenX)
      {
        if (this.Rotation.X > 0.0)
          this.Rotation.X = MathHelper.Clamp(this.Rotation.X - num, 0.0, this.Rotation.X);
        else
          this.Rotation.X = MathHelper.Clamp(this.Rotation.X + num, this.Rotation.X, 0.0);
      }
      if (dampenY)
      {
        if (this.Rotation.Y > 0.0)
          this.Rotation.Y = MathHelper.Clamp(this.Rotation.Y - num, 0.0, this.Rotation.Y);
        else
          this.Rotation.Y = MathHelper.Clamp(this.Rotation.Y + num, this.Rotation.Y, 0.0);
      }
      if (dampenZ)
      {
        if (this.Rotation.Z > 0.0)
          this.Rotation.Z = MathHelper.Clamp(this.Rotation.Z - num, 0.0, this.Rotation.Z);
        else
          this.Rotation.Z = MathHelper.Clamp(this.Rotation.Z + num, this.Rotation.Z, 0.0);
      }
      if (this.CurrRcsRotationThrust.HasValue)
        return;
      this.CurrRcsRotationThrust = new Vector3D?(this.Rotation - rotation);
    }

    public void GatherAtmosphere()
    {
    }

    public PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataShip persistenceObjectDataShip = new PersistenceObjectDataShip();
      persistenceObjectDataShip.GUID = this.GUID;
      persistenceObjectDataShip.Health = this.Health;
      persistenceObjectDataShip.OrbitData = new OrbitData();
      this.Orbit.FillOrbitData(ref persistenceObjectDataShip.OrbitData, (SpaceObjectVessel) null);
      persistenceObjectDataShip.Forward = this.Forward.ToArray();
      persistenceObjectDataShip.Up = this.Up.ToArray();
      persistenceObjectDataShip.Registration = this.VesselData.VesselRegistration;
      persistenceObjectDataShip.Name = this.VesselData.VesselName;
      persistenceObjectDataShip.Tag = this.VesselData.Tag;
      persistenceObjectDataShip.SceneID = this.SceneID;
      persistenceObjectDataShip.timePassedSinceShipCall = this.timePassedSinceRequest;
      persistenceObjectDataShip.IsDoomed = this.isDoomed;
      persistenceObjectDataShip.IsDistresActive = this.IsDistresActive;
      persistenceObjectDataShip.IsArenaVessel = this.IsArenaVessel;
      persistenceObjectDataShip.DistressType = this.DistressType;
      persistenceObjectDataShip.DistressActivatedTime = this.DistressActivatedTime;
      persistenceObjectDataShip.AuthorizedPersonel = this.AuthorizedPersonel;
      persistenceObjectDataShip.StartingSetId = this.StartingSetId;
      if (this.DockedToVessel != null)
      {
        foreach (VesselDockingPort dockingPort in this.DockingPorts)
        {
          if (dockingPort.DockedVessel == this.DockedToVessel)
          {
            persistenceObjectDataShip.DockedToShipGUID = new long?(this.DockedToVessel.GUID);
            persistenceObjectDataShip.DockedPortID = new long?((long) dockingPort.ID.InSceneID);
            persistenceObjectDataShip.DockedToPortID = new long?((long) dockingPort.DockedToID.InSceneID);
            break;
          }
        }
      }
      if (this.StabilizeToTargetObj != null)
      {
        persistenceObjectDataShip.StabilizeToTargetGUID = new long?(this.StabilizeToTargetObj.GUID);
        persistenceObjectDataShip.StabilizeToTargetPosition = this.StabilizeToTargetRelPosition.ToArray();
      }
      persistenceObjectDataShip.DynamicObjects = new List<PersistenceObjectData>();
      foreach (DynamicObject inner in this.DynamicObjects.InnerList)
        persistenceObjectDataShip.DynamicObjects.Add(inner.Item != null ? inner.Item.GetPersistenceData() : inner.GetPersistenceData());
      persistenceObjectDataShip.CargoBays = new List<PersistenceObjectDataCargo>();
      foreach (CargoBay cargoBay in this.CargoBays)
        persistenceObjectDataShip.CargoBays.Add(cargoBay.GetPersistenceData() as PersistenceObjectDataCargo);
      persistenceObjectDataShip.ResourceTanks = new List<PersistenceObjectDataCargo>();
      foreach (ResourceContainer resourceContainer in this.DistributionManager.GetResourceContainers())
        persistenceObjectDataShip.ResourceTanks.Add(resourceContainer.GetPersistenceData() as PersistenceObjectDataCargo);
      persistenceObjectDataShip.Generators = new List<PersistenceObjectDataVesselComponent>();
      foreach (Generator generator in this.DistributionManager.GetGenerators())
        persistenceObjectDataShip.Generators.Add(generator.GetPersistenceData() as PersistenceObjectDataVesselComponent);
      persistenceObjectDataShip.SubSystems = new List<PersistenceObjectDataVesselComponent>();
      foreach (SubSystem subSystem in this.DistributionManager.GetSubSystems())
        persistenceObjectDataShip.SubSystems.Add(subSystem.GetPersistenceData() as PersistenceObjectDataVesselComponent);
      persistenceObjectDataShip.Rooms = new List<PersistenceObjectDataRoom>();
      foreach (Room room in this.DistributionManager.GetRooms())
        persistenceObjectDataShip.Rooms.Add(room.GetPersistenceData() as PersistenceObjectDataRoom);
      persistenceObjectDataShip.Doors = new List<PersistenceObjectDataDoor>();
      foreach (Door door in this.Doors)
        persistenceObjectDataShip.Doors.Add(door.GetPersistenceData() as PersistenceObjectDataDoor);
      persistenceObjectDataShip.DockingPorts = new List<PersistenceObjectDataDockingPort>();
      foreach (VesselDockingPort dockingPort in this.DockingPorts)
        persistenceObjectDataShip.DockingPorts.Add(dockingPort.GetPersistenceData() as PersistenceObjectDataDockingPort);
      persistenceObjectDataShip.Executers = new List<PersistenceObjectDataExecuter>();
      foreach (SceneTriggerExecuter sceneTriggerExecuter in this.SceneTriggerExecuters)
        persistenceObjectDataShip.Executers.Add(sceneTriggerExecuter.GetPersistenceData() as PersistenceObjectDataExecuter);
      persistenceObjectDataShip.NameTags = new List<PersistenceObjectDataNameTag>();
      foreach (NameTagData nameTag in this.NameTags)
        persistenceObjectDataShip.NameTags.Add(new PersistenceObjectDataNameTag()
        {
          InSceneID = nameTag.InSceneID,
          NameTagText = nameTag.NameTagText
        });
      persistenceObjectDataShip.RepairPoints = new List<PersistenceObjectDataRepairPoint>();
      foreach (VesselRepairPoint repairPoint in this.RepairPoints)
        persistenceObjectDataShip.RepairPoints.Add(repairPoint.GetPersistenceData() as PersistenceObjectDataRepairPoint);
      return (PersistenceObjectData) persistenceObjectDataShip;
    }

    public void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        PersistenceObjectDataShip data = persistenceData as PersistenceObjectDataShip;
        this.GUID = data.GUID;
        this.CreateShipData(data.Registration, data.Tag, data.SceneID, false);
        this.VesselData.VesselName = data.Name;
        this.LoadShipRequestPersistance(data.timePassedSinceShipCall);
        this.DistributionManager = new DistributionManager((SpaceObjectVessel) this, false);
        this.fillVesselCaps();
        this.InitializeOrbit(Vector3D.Zero, Vector3D.One, data.Forward.ToVector3D(), data.Up.ToVector3D());
        Server.Instance.PhysicsController.CreateAndAddRigidBody((SpaceObjectVessel) this);
        this.Health = data.Health;
        StructureSceneData structureSceneData = ObjectCopier.DeepCopy<StructureSceneData>(StaticData.StructuresDataList.Find((Predicate<StructureSceneData>) (m => (int) m.ItemID == (int) (short) this.SceneID)), 10);
        foreach (PersistenceObjectDataDynamicObject dynamicObject1 in data.DynamicObjects)
        {
          DynamicObject dynamicObject2 = Persistence.CreateDynamicObject(dynamicObject1, (SpaceObject) this, structureSceneData);
          if (dynamicObject2 != null && dynamicObject2.Item != null && dynamicObject2.Item.AttachPointKey != null)
          {
            VesselComponent componentByPartSlot = this.MainDistributionManager.GetVesselComponentByPartSlot(dynamicObject2.Item.AttachPointKey);
            if (componentByPartSlot != null && dynamicObject2.Item is MachineryPart)
              componentByPartSlot.FitPartToSlot(dynamicObject2.Item.AttachPointKey, (MachineryPart) dynamicObject2.Item);
          }
        }
        if (data.CargoBays != null)
        {
          foreach (PersistenceObjectDataCargo cargoBay1 in data.CargoBays)
          {
            PersistenceObjectDataCargo cbpd = cargoBay1;
            CargoBay cargoBay2 = this.CargoBays.Find((Predicate<CargoBay>) (m => m.InSceneID == cbpd.InSceneID));
            if (cargoBay2 != null)
              cargoBay2.LoadPersistenceData((PersistenceObjectData) cbpd);
          }
        }
        if (data.ResourceTanks != null)
        {
          foreach (PersistenceObjectDataCargo resourceTank in data.ResourceTanks)
          {
            ResourceContainer resourceContainer = this.DistributionManager.GetResourceContainer(new VesselObjectID()
            {
              VesselGUID = this.GUID,
              InSceneID = resourceTank.InSceneID
            });
            if (resourceContainer != null)
              resourceContainer.LoadPersistenceData((PersistenceObjectData) resourceTank);
          }
        }
        if (data.Generators != null)
        {
          foreach (PersistenceObjectDataVesselComponent generator1 in data.Generators)
          {
            Generator generator2 = this.DistributionManager.GetGenerator(new VesselObjectID()
            {
              VesselGUID = this.GUID,
              InSceneID = generator1.InSceneID
            });
            if (generator2 != null)
              generator2.LoadPersistenceData((PersistenceObjectData) generator1);
          }
        }
        if (data.SubSystems != null)
        {
          foreach (PersistenceObjectDataVesselComponent subSystem1 in data.SubSystems)
          {
            SubSystem subSystem2 = this.DistributionManager.GetSubSystem(new VesselObjectID()
            {
              VesselGUID = this.GUID,
              InSceneID = subSystem1.InSceneID
            });
            if (subSystem2 != null)
              subSystem2.LoadPersistenceData((PersistenceObjectData) subSystem1);
          }
        }
        if (data.Rooms != null)
        {
          foreach (PersistenceObjectDataRoom room1 in data.Rooms)
          {
            Room room2 = this.DistributionManager.GetRoom(new VesselObjectID()
            {
              VesselGUID = this.GUID,
              InSceneID = room1.InSceneID
            });
            if (room2 != null)
              room2.LoadPersistenceData((PersistenceObjectData) room1);
          }
        }
        if (data.Doors != null)
        {
          foreach (PersistenceObjectDataDoor door1 in data.Doors)
          {
            PersistenceObjectDataDoor d = door1;
            Door door2 = this.Doors.Find((Predicate<Door>) (x => x.ID.InSceneID == d.InSceneID));
            if (door2 != null)
              door2.LoadPersistenceData((PersistenceObjectData) d);
          }
        }
        if (data.DockingPorts != null)
        {
          foreach (PersistenceObjectDataDockingPort dockingPort in data.DockingPorts)
          {
            PersistenceObjectDataDockingPort dp = dockingPort;
            VesselDockingPort vesselDockingPort = this.DockingPorts.Find((Predicate<VesselDockingPort>) (x => x.ID.InSceneID == dp.InSceneID));
            if (vesselDockingPort != null)
              vesselDockingPort.LoadPersistenceData((PersistenceObjectData) dp);
          }
        }
        if (data.Executers != null)
        {
          foreach (PersistenceObjectDataExecuter executer in data.Executers)
          {
            PersistenceObjectDataExecuter e = executer;
            SceneTriggerExecuter sceneTriggerExecuter = this.SceneTriggerExecuters.Find((Predicate<SceneTriggerExecuter>) (x => x.InSceneID == e.InSceneID));
            if (sceneTriggerExecuter != null)
              sceneTriggerExecuter.LoadPersistenceData((PersistenceObjectData) e);
          }
        }
        if (data.NameTags != null)
        {
          foreach (PersistenceObjectDataNameTag nameTag in data.NameTags)
          {
            PersistenceObjectDataNameTag n = nameTag;
            NameTagData nameTagData = this.NameTags.Find((Predicate<NameTagData>) (x => x.InSceneID == n.InSceneID));
            if (nameTagData != null)
              nameTagData.NameTagText = n.NameTagText;
          }
        }
        if (data.RepairPoints != null && data.RepairPoints.Count > 0)
        {
          foreach (PersistenceObjectDataRepairPoint repairPoint in data.RepairPoints)
          {
            PersistenceObjectDataRepairPoint rp = repairPoint;
            VesselRepairPoint vesselRepairPoint = this.RepairPoints.Find((Predicate<VesselRepairPoint>) (x => x.ID.InSceneID == rp.InSceneID));
            if (vesselRepairPoint != null)
              vesselRepairPoint.LoadPersistenceData((PersistenceObjectData) rp);
          }
        }
        this.MainDistributionManager.UpdateSystems(true, true);
        if (data.OrbitData != null)
          this.Orbit.ParseNetworkData(data.OrbitData, true);
        Server.Instance.Add((SpaceObjectVessel) this);
        this.SetPhysicsParameters();
        if (data.DockedToShipGUID.HasValue)
        {
          Ship vessel = Server.Instance.GetVessel(data.DockedToShipGUID.Value) as Ship;
          this.DockToShip(this.DockingPorts.Find((Predicate<VesselDockingPort>) (m => (long) m.ID.InSceneID == data.DockedPortID.Value)), vessel.DockingPorts.Find((Predicate<VesselDockingPort>) (m => (long) m.ID.InSceneID == data.DockedToPortID.Value)), vessel, false);
        }
        if (data.StabilizeToTargetGUID.HasValue)
        {
          this.StabilizeToTarget(Server.Instance.GetObject(data.StabilizeToTargetGUID.Value) as SpaceObjectVessel, true);
          this.StabilizeToTargetRelPosition = data.StabilizeToTargetPosition.ToVector3D();
          this.UpdateStabilization(0.0);
        }
        if (data.timePassedSinceShipCall > 0.0)
          this.LoadShipRequestPersistance(data.timePassedSinceShipCall);
        this.isDoomed = data.IsDoomed;
        this.IsDistresActive = data.IsDistresActive;
        this.IsArenaVessel = data.IsArenaVessel;
        this.DistressType = data.DistressType;
        this.DistressActivatedTime = data.DistressActivatedTime;
        this.AuthorizedPersonel = data.AuthorizedPersonel;
        this.StartingSetId = data.StartingSetId;
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }

    public void VesselRequestListener(NetworkData data)
    {
      VesselRequest vesselRequest = data as VesselRequest;
      if (vesselRequest.GUID != this.GUID || this.StartingSetId == -1L)
        return;
      if (this.timePassedSinceRequest > 0.0)
        Server.Instance.NetworkController.SendToGameClient(vesselRequest.Sender, (NetworkData) Server.Instance.SendSystemMessage(SystemMessagesTypes.ShipTimerAllreadyStarted, this));
      else if (Server.Instance.GetVesselsInRange((SpaceObjectVessel) this, 5000.0, GameScenes.SceneID.AltCorp_Shuttle_SARA).Count == 0)
      {
        this.timePassedSinceRequest = 1.0;
        Server.Instance.SubscribeToTimer(UpdateTimer.TimerStep.Step_1_0_min, new UpdateTimer.TimeStepDelegate(this.SpawnShipCallback));
        Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) Server.Instance.SendSystemMessage(SystemMessagesTypes.ShipWillArriveIn, this), -1L, (SpaceObject) this);
      }
      else
      {
        Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) Server.Instance.SendSystemMessage(SystemMessagesTypes.ShipInRange, this), -1L, (SpaceObject) this);
        Server.Instance.NetworkController.SendToAllClients((NetworkData) new VesselRequestResponse()
        {
          GUID = this.GUID,
          Active = false
        }, -1L);
      }
    }

    public void LoadShipRequestPersistance(double timeSince)
    {
      if (timeSince <= 0.001)
        return;
      this.timePassedSinceRequest = timeSince;
      Server.Instance.SubscribeToTimer(UpdateTimer.TimerStep.Step_1_0_min, new UpdateTimer.TimeStepDelegate(this.SpawnShipCallback));
    }

    public void SpawnShipCallback(double dbl)
    {
      this.timePassedSinceRequest = this.timePassedSinceRequest + dbl;
      if (this.timePassedSinceRequest <= this.RespawnTimeForShip)
        return;
      this.CurrentSpawnedShip = (SpaceObjectVessel) Ship.SpawnSara((SpaceObjectVessel) this, Ship.NewSaraPos((SpaceObjectVessel) this));
      Server.Instance.UnsubscribeFromTimer(UpdateTimer.TimerStep.Step_1_0_min, new UpdateTimer.TimeStepDelegate(this.SpawnShipCallback));
      this.timePassedSinceRequest = 0.0;
      Server.Instance.NetworkController.SendToAllClients((NetworkData) new VesselRequestResponse()
      {
        Active = false,
        GUID = this.GUID
      }, -1L);
    }

    public static Vector3D NewSaraPos(SpaceObjectVessel v)
    {
      double radius = v.Radius;
      double num1 = MathHelper.RandomRange(0.0, 2.0 * System.Math.PI);
      double num2 = MathHelper.RandomRange(0.0, System.Math.PI);
      return MathHelper.RandomRange(v.Radius + 200.0, v.Radius + 350.0) * new Vector3D(System.Math.Cos(num1) * System.Math.Sin(num2), System.Math.Sin(num1) * System.Math.Sin(num2), System.Math.Cos(num2));
    }

    public static Ship SpawnSara(SpaceObjectVessel mainShip, Vector3D pos)
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
      string startingTag = SpawnManager.Settings.StartingTag;
      int num2 = 1;
      double distanceFromSurfacePercMin = 0.03;
      double distanceFromSurfacePercMax = 0.3;
      // ISSUE: variable of the null type
      __Null local2 = null;
      double celestialBodyDeathDistanceMultiplier = 1.5;
      double artificialBodyDistanceCheck = 100.0;
      Ship newShip = Ship.CreateNewShip((GameScenes.SceneID) num1, registration, shipID, nearArtificialBodyGUIDs, (List<long>) local1, positionOffset, velocityAtPosition, localRotation, startingTag, num2 != 0, distanceFromSurfacePercMin, distanceFromSurfacePercMax, (SpawnRuleOrbit) local2, celestialBodyDeathDistanceMultiplier, artificialBodyDistanceCheck);
      newShip.StabilizeToTarget(mainShip, true);
      return newShip;
    }

    public void DistressCallRequestListener(NetworkData data)
    {
      DistressCallRequest distressCallRequest = data as DistressCallRequest;
      if (distressCallRequest.GUID != this.GUID || this.isDoomed)
        return;
      if (this.IsArenaVessel)
      {
        SpaceObjectVessel spaceObjectVessel = (SpaceObjectVessel) this;
        if (this.DockedToMainVessel != null)
          spaceObjectVessel = this.DockedToMainVessel;
        if (spaceObjectVessel.IsDistresActive == distressCallRequest.IsDistressActive || spaceObjectVessel.IsDistresActive)
          return;
        foreach (Ship ship in new List<SpaceObjectVessel>((IEnumerable<SpaceObjectVessel>) spaceObjectVessel.AllDockedVessels)
        {
          spaceObjectVessel
        })
        {
          ship.ToggleDistress(distressCallRequest.IsDistressActive, DistressType.Distress);
          NetworkController networkController = Server.Instance.NetworkController;
          DistressCallResponse distressCallResponse = new DistressCallResponse();
          distressCallResponse.GUID = ship.GUID;
          distressCallResponse.Trans = ship.GetObjectTransform();
          long skipPlayerGUID = -1;
          networkController.SendToAllClients((NetworkData) distressCallResponse, skipPlayerGUID);
        }
      }
      else
      {
        this.ToggleDistress(distressCallRequest.IsDistressActive, DistressType.Distress);
        NetworkController networkController = Server.Instance.NetworkController;
        DistressCallResponse distressCallResponse = new DistressCallResponse();
        distressCallResponse.GUID = this.GUID;
        distressCallResponse.Trans = this.GetObjectTransform();
        long skipPlayerGUID = -1;
        networkController.SendToAllClients((NetworkData) distressCallResponse, skipPlayerGUID);
      }
    }

    public void VesselSecurityRequestListener(NetworkData data)
    {
      VesselSecurityRequest vesselSecurityRequest = data as VesselSecurityRequest;
      if (vesselSecurityRequest.VesselGUID != this.GUID)
        return;
      Player player = Server.Instance.GetPlayer(data.Sender);
      if (player == null)
        return;
      bool flag = false;
      if (!vesselSecurityRequest.VesselName.IsNullOrEmpty() && this.ChangeVesselName(player, vesselSecurityRequest.VesselName))
        flag = true;
      if (!vesselSecurityRequest.AddPlayerSteamID.IsNullOrEmpty() && vesselSecurityRequest.AddPlayerRank.HasValue && this.AddAuthorizedPerson(player, vesselSecurityRequest.AddPlayerSteamID, vesselSecurityRequest.AddPlayerName, vesselSecurityRequest.AddPlayerRank.Value))
        flag = true;
      if (!vesselSecurityRequest.RemovePlayerSteamID.IsNullOrEmpty() && this.RemoveAuthorizedPerson(player, vesselSecurityRequest.RemovePlayerSteamID))
        flag = true;
      if (vesselSecurityRequest.HackPanel.HasValue && vesselSecurityRequest.HackPanel.Value && this.ClearSecuritySystem(player))
        flag = true;
      if (!flag)
        return;
      this.SendSecurityResponse(true, true);
    }

    public void ResetSpawnPointsForPlayer(Player pl, bool sendStatsMessage)
    {
      if (this.SpawnPoints == null || this.SpawnPoints.Count == 0)
        return;
      ShipStatsMessage shipStatsMessage = (ShipStatsMessage) null;
      foreach (ShipSpawnPoint spawnPoint in this.SpawnPoints)
      {
        if (spawnPoint.Player == pl)
        {
          if (shipStatsMessage == null & sendStatsMessage)
          {
            shipStatsMessage = new ShipStatsMessage();
            shipStatsMessage.GUID = this.GUID;
            shipStatsMessage.Temperature = new float?(this.Temperature);
            shipStatsMessage.Health = new float?(this.Health);
            shipStatsMessage.VesselObjects = new VesselObjects();
            shipStatsMessage.VesselObjects.SpawnPoints = new List<SpawnPointStats>();
          }
          spawnPoint.Player = (Player) null;
          spawnPoint.State = SpawnPointState.Unlocked;
          spawnPoint.IsPlayerInSpawnPoint = false;
          shipStatsMessage.VesselObjects.SpawnPoints.Add(new SpawnPointStats()
          {
            InSceneID = spawnPoint.SpawnPointID,
            NewState = new SpawnPointState?(spawnPoint.State),
            PlayerGUID = new long?(-1L),
            PlayerName = ""
          });
        }
      }
      if (shipStatsMessage == null)
        return;
      Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) shipStatsMessage, -1L, (SpaceObject) this);
    }

    public override float ChangeHealthBy(float value, List<VesselRepairPoint> repairPoints = null)
    {
      if (this.IsArenaVessel)
        return 0.0f;
      float health = this.Health;
      this.Health = MathHelper.Clamp(this.Health + value, 0.0f, this.MaxHealth);
      if ((double) value < 0.0 && this.RepairPoints.Count > 0)
      {
        float num1 = System.Math.Abs(value);
        List<VesselRepairPoint> source = new List<VesselRepairPoint>((IEnumerable<VesselRepairPoint>) ((IEnumerable<VesselRepairPoint>) this.RepairPoints.ToArray()).OrderBy<VesselRepairPoint, double>((Func<VesselRepairPoint, double>) (m => ((double) m.Health == (double) m.MaxHealth ? 1.0 : 0.0) + MathHelper.RandomNextDouble())));
        if (MathHelper.RandomNextDouble() < (double) num1 * Server.ActivateRepairPointChanceMultiplier / (double) this.MaxHealth)
        {
          VesselRepairPoint vesselRepairPoint = source.Find((Predicate<VesselRepairPoint>) (m => (double) m.Health == (double) m.MaxHealth));
          if (vesselRepairPoint != null)
          {
            source.Remove(vesselRepairPoint);
            source.Insert(0, vesselRepairPoint);
          }
        }
        if (repairPoints != null)
        {
          source.RemoveAll((Predicate<VesselRepairPoint>) (m => repairPoints.Contains(m)));
          source.InsertRange(0, (IEnumerable<VesselRepairPoint>) repairPoints);
        }
        foreach (VesselRepairPoint vesselRepairPoint in source.Where<VesselRepairPoint>((Func<VesselRepairPoint, bool>) (m => (double) m.Health > 0.0)))
        {
          VesselRepairPoint rp = vesselRepairPoint;
          if ((double) rp.Health >= (double) num1)
          {
            float num2 = ((IEnumerable<VesselRepairPoint>) this.RepairPoints.ToArray()).Where<VesselRepairPoint>((Func<VesselRepairPoint, bool>) (m => m != rp)).Sum<VesselRepairPoint>((Func<VesselRepairPoint, float>) (k => k.Health));
            rp.Health = this.Health - num2;
            break;
          }
          num1 -= rp.Health;
          rp.Health = 0.0f;
        }
      }
      return this.Health - health;
    }

    public bool IsDoomed
    {
      get
      {
        return this.isDoomed;
      }
    }

    public float TimeToLive
    {
      get
      {
        return this.Health / (this.DecayRate * (float) Server.VesselDecayRateMultiplier);
      }
    }

    public void ToggleDoomed(bool enable)
    {
      this.isDoomed = enable;
      if (this.isDoomed)
        this.ToggleDistress(true, DistressType.Doomed);
      else
        this.ToggleDistress(false, DistressType.Distress);
    }

    private class DockUndockPlayerData
    {
      private List<Ship.DockUndockPlayerData.PlayerItem> players = new List<Ship.DockUndockPlayerData.PlayerItem>();

      public static Ship.DockUndockPlayerData GetPlayerData(params SpaceObjectVessel[] vessels)
      {
        Ship.DockUndockPlayerData undockPlayerData = new Ship.DockUndockPlayerData();
        foreach (SpaceObjectVessel spaceObjectVessel in vessels)
        {
          if (spaceObjectVessel.DockedToMainVessel != null)
            spaceObjectVessel = spaceObjectVessel.DockedToMainVessel;
          foreach (Player player in spaceObjectVessel.VesselCrew)
          {
            List<Ship.DockUndockPlayerData.PlayerItem> players = undockPlayerData.players;
            Ship.DockUndockPlayerData.PlayerItem playerItem = new Ship.DockUndockPlayerData.PlayerItem();
            playerItem.Player = player;
            playerItem.Parent = spaceObjectVessel;
            Vector3D vector3D = player.LocalPosition + spaceObjectVessel.VesselData.CollidersCenterOffset.ToVector3D();
            playerItem.PosFromParent = vector3D;
            QuaternionD localRotation = player.LocalRotation;
            playerItem.RotFromParent = localRotation;
            players.Add(playerItem);
          }
          foreach (SpaceObjectVessel allDockedVessel in spaceObjectVessel.AllDockedVessels)
          {
            foreach (Player player in allDockedVessel.VesselCrew)
            {
              List<Ship.DockUndockPlayerData.PlayerItem> players = undockPlayerData.players;
              Ship.DockUndockPlayerData.PlayerItem playerItem = new Ship.DockUndockPlayerData.PlayerItem();
              playerItem.Player = player;
              playerItem.Parent = allDockedVessel;
              Vector3D vector3D = allDockedVessel.RelativeRotationFromMainParent * (player.LocalPosition + spaceObjectVessel.VesselData.CollidersCenterOffset.ToVector3D() - allDockedVessel.RelativePositionFromMainParent);
              playerItem.PosFromParent = vector3D;
              QuaternionD quaternionD = player.LocalRotation * allDockedVessel.RelativeRotationFromMainParent.Inverse();
              playerItem.RotFromParent = quaternionD;
              players.Add(playerItem);
            }
          }
        }
        return undockPlayerData;
      }

      public void ModifyPlayersPositionAndRotation()
      {
        if (this.players == null || this.players.Count == 0)
          return;
        foreach (Ship.DockUndockPlayerData.PlayerItem player in this.players)
        {
          Vector3D zero = Vector3D.Zero;
          QuaternionD identity = QuaternionD.Identity;
          Vector3D locPos;
          QuaternionD locRot;
          if (player.Parent.DockedToMainVessel != null)
          {
            locPos = -player.Parent.DockedToMainVessel.VesselData.CollidersCenterOffset.ToVector3D() + player.Parent.RelativePositionFromMainParent + player.Parent.RelativeRotationFromMainParent * player.PosFromParent - player.Player.LocalPosition;
            locRot = player.Player.LocalRotation.Inverse() * (player.Parent.RelativeRotationFromMainParent * player.RotFromParent);
          }
          else
          {
            locPos = -player.Parent.VesselData.CollidersCenterOffset.ToVector3D() + player.PosFromParent - player.Player.LocalPosition;
            locRot = player.Player.LocalRotation.Inverse() * player.RotFromParent;
          }
          player.Player.ModifyLocalPositionAndRotation(locPos, locRot);
          if (Server.Instance.NetworkController.clientList.ContainsKey(player.Player.GUID) && player.Player.IsAlive && player.Player.EnviromentReady && player.Player.PlayerReady)
            player.Player.SetDockUndockCorrection(new Vector3D?(locPos), new QuaternionD?(locRot));
        }
      }

      private class PlayerItem
      {
        public Player Player;
        public SpaceObjectVessel Parent;
        public Vector3D PosFromParent;
        public QuaternionD RotFromParent;
      }
    }
  }
}
