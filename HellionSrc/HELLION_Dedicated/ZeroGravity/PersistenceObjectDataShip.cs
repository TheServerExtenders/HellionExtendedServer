// Decompiled with JetBrains decompiler
// Type: ZeroGravity.PersistenceObjectDataShip
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity
{
  public class PersistenceObjectDataShip : PersistenceObjectData
  {
    public OrbitData OrbitData;
    public double[] Forward;
    public double[] Up;
    public string Registration;
    public string Name;
    public string Tag;
    public GameScenes.SceneID SceneID;
    public float Health;
    public long? DockedToShipGUID;
    public long? DockedPortID;
    public long? DockedToPortID;
    public double timePassedSinceShipCall;
    public long? StabilizeToTargetGUID;
    public double[] StabilizeToTargetPosition;
    public bool IsDoomed;
    public bool IsDistresActive;
    public bool IsArenaVessel;
    public DistressType DistressType;
    public double DistressActivatedTime;
    public List<AuthorizedPerson> AuthorizedPersonel;
    public long StartingSetId;
    public List<PersistenceObjectData> DynamicObjects;
    public List<PersistenceObjectDataCargo> CargoBays;
    public List<PersistenceObjectDataCargo> ResourceTanks;
    public List<PersistenceObjectDataVesselComponent> Generators;
    public List<PersistenceObjectDataVesselComponent> SubSystems;
    public List<PersistenceObjectDataRoom> Rooms;
    public List<PersistenceObjectDataDoor> Doors;
    public List<PersistenceObjectDataDockingPort> DockingPorts;
    public List<PersistenceObjectDataExecuter> Executers;
    public List<PersistenceObjectDataNameTag> NameTags;
    public List<PersistenceObjectDataRepairPoint> RepairPoints;

    public override Persistence.ObjectType Type
    {
      get
      {
        return Persistence.ObjectType.Ship;
      }
    }
  }
}
