// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.StructureSceneData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;

namespace ZeroGravity.Data
{
  public class StructureSceneData : ISceneData
  {
    public short ItemID;
    public string ScenePath;
    public string SceneName;
    public string GameName;
    public float Mass;
    public double RadarSignature;
    public float HeatCollectionFactor;
    public float HeatDissipationFactor;
    public List<StructureSceneConnectionData> StructureConnections;
    public List<SpawnPointData> SpawnPoints;
    public List<DynamicObjectSceneData> DynamicObjects;
    public List<BaseAttachPointData> AttachPoints;
    public List<SubSystemData> SubSystems;
    public List<GeneratorData> Generators;
    public List<RoomData> Rooms;
    public List<ResourceContainerData> ResourceContainers;
    public string Collision;
    public ServerCollisionData Colliders;
    public List<DoorData> Doors;
    public List<SceneTriggerExecuterData> SceneTriggerExecuters;
    public List<PDUGroupData> PDUGroups;
    public List<SceneDockingPortData> DockingPorts;
    public List<SpawnObjectsWithChanceData> SpawnObjectChanceData;
    public List<CargoBayData> CargoBays;
    public List<NameTagData> NameTags;
    public List<VesselRepairPointData> RepairPoints;
    public bool HasSecuritySystem;
    public float MaxHealth;
    public float Health;
    public float DecayRate;
    public SceneSpawnSettings[] SpawnSettings;
  }
}
