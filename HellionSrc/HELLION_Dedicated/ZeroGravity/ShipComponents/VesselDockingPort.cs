// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.VesselDockingPort
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class VesselDockingPort : IPersistantObject
  {
    public SpaceObjectVessel ParentVessel = (SpaceObjectVessel) null;
    public VesselObjectID ID;
    public VesselObjectID DockedToID;
    public SpaceObjectVessel DockedVessel;
    public bool DockingStatus;
    public Vector3D Position;
    public QuaternionD Rotation;
    public int[] DoorsIDs;
    public int OrderID;
    public float DoorPairingDistance;
    public bool Locked;
    public Dictionary<SceneTriggerExecuter, Vector3D> MergeExecuters;
    public double MergeExecutersDistance;

    public List<ExecuterMergeDetails> GetMergedExecuters(VesselDockingPort parentPort)
    {
      if (!this.DockingStatus)
        return (List<ExecuterMergeDetails>) null;
      if (parentPort == null)
        parentPort = this.DockedVessel.DockingPorts.Find((Predicate<VesselDockingPort>) (m => m.ID.InSceneID == this.DockedToID.InSceneID));
      List<ExecuterMergeDetails> executerMergeDetailsList = new List<ExecuterMergeDetails>();
      foreach (SceneTriggerExecuter key in parentPort.MergeExecuters.Keys)
      {
        SceneTriggerExecuter sceneTriggerExecuter1 = key.Child != null ? key : key.Parent;
        SceneTriggerExecuter sceneTriggerExecuter2 = key.Parent != null ? key : key.Child;
        if (sceneTriggerExecuter1 != null && sceneTriggerExecuter2 != null)
          executerMergeDetailsList.Add(new ExecuterMergeDetails()
          {
            ParentTriggerID = new VesselObjectID(sceneTriggerExecuter1.ParentShip.GUID, sceneTriggerExecuter1.InSceneID),
            ChildTriggerID = new VesselObjectID(sceneTriggerExecuter2.ParentShip.GUID, sceneTriggerExecuter2.InSceneID)
          });
      }
      return executerMergeDetailsList;
    }

    public PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataDockingPort objectDataDockingPort = new PersistenceObjectDataDockingPort();
      objectDataDockingPort.InSceneID = this.ID.InSceneID;
      int num = this.Locked ? 1 : 0;
      objectDataDockingPort.Locked = num != 0;
      return (PersistenceObjectData) objectDataDockingPort;
    }

    public void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        PersistenceObjectDataDockingPort objectDataDockingPort = persistenceData as PersistenceObjectDataDockingPort;
        if (objectDataDockingPort == null)
          Dbg.Warning("PersistenceObjectDataDoor data is null");
        else
          this.Locked = objectDataDockingPort.Locked;
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }

    public SceneDockingPortDetails GetDetails()
    {
      SceneDockingPortDetails dockingPortDetails = new SceneDockingPortDetails();
      dockingPortDetails.ID = this.ID;
      dockingPortDetails.DockedToID = this.DockedToID;
      int num1 = this.Locked ? 1 : 0;
      dockingPortDetails.Locked = num1 != 0;
      int num2 = this.DockingStatus ? 1 : 0;
      dockingPortDetails.DockingStatus = num2 != 0;
      float[] floatArray1 = this.ParentVessel.RelativePositionFromParent.ToFloatArray();
      dockingPortDetails.RelativePosition = floatArray1;
      float[] floatArray2 = this.ParentVessel.RelativeRotationFromParent.ToFloatArray();
      dockingPortDetails.RelativeRotation = floatArray2;
      float[] numArray = this.ParentVessel.IsDocked ? this.ParentVessel.DockedToMainVessel.VesselData.CollidersCenterOffset : this.ParentVessel.VesselData.CollidersCenterOffset;
      dockingPortDetails.CollidersCenterOffset = numArray;
      List<ExecuterMergeDetails> mergedExecuters = this.GetMergedExecuters((VesselDockingPort) null);
      dockingPortDetails.ExecutersMerge = mergedExecuters;
      List<PairedDoorsDetails> pairedDoors = this.ParentVessel.GetPairedDoors(this);
      dockingPortDetails.PairedDoors = pairedDoors;
      return dockingPortDetails;
    }
  }
}
