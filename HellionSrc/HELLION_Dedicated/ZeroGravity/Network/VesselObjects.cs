// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.VesselObjects
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using System.Collections.Generic;
using ZeroGravity.Data;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class VesselObjects
  {
    public VesselMiscStatuses MiscStatuses;
    public List<SubSystemDetails> SubSystems;
    public List<GeneratorDetails> Generators;
    public List<ResourceContainerDetails> ResourceContainers;
    public List<RoomDetails> RoomTriggers;
    public List<DoorDetails> Doors;
    public List<SceneTriggerExecuterDetails> SceneTriggerExecuters;
    public List<AttachPointDetails> AttachPoints;
    public List<SceneDockingPortDetails> DockingPorts;
    public List<SpawnObjectsWithChanceDetails> SpawnWithChance;
    public List<CargoBayDetails> CargoBays;
    public List<SpawnPointStats> SpawnPoints;
    public List<NameTagData> NameTags;
    public List<VesselRepairPointDetails> RepairPoints;
    public VesselSecurityData SecurityData;
  }
}
