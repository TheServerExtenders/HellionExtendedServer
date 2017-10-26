// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.VesselRepairPointData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;

namespace ZeroGravity.Data
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class VesselRepairPointData : ISceneData
  {
    public int InSceneID;
    public int RoomID;
    public RepairPointDamageType DamageType;
    public int AffectedSystemID;
    public float MalfunctionThreshold;
    public float RepairThreshold;
  }
}
