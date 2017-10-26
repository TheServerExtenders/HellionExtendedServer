// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.GeneratorData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Network;
using ZeroGravity.ShipComponents;

namespace ZeroGravity.Data
{
  public class GeneratorData : ISceneData
  {
    public int InSceneID;
    public GeneratorType Type;
    public int RoomID;
    public float Output;
    public float PowerUpTime;
    public float CoolDownTime;
    public bool AutoReactivate;
    public float NominalOutput;
    public float RequestedOutput;
    public float OutputRate;
    public SystemStatus Status;
    public ResourceRequirement[] ResourceRequirements;
    public SystemSpawnSettings[] SpawnSettings;
    public List<int> MachineryPartSlots;
    public List<int> ResourceContainers;
    public SubSystemAuxData AuxData;
  }
}
