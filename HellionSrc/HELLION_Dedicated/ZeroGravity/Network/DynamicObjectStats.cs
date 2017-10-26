// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.DynamicObjectStats
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using System.Collections.Generic;
using ZeroGravity.Data;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  [ProtoInclude(10000, typeof (HelmetStats))]
  [ProtoInclude(10002, typeof (JetpackStats))]
  [ProtoInclude(10003, typeof (MagazineStats))]
  [ProtoInclude(10004, typeof (WeaponStats))]
  [ProtoInclude(10005, typeof (BatteryStats))]
  [ProtoInclude(10006, typeof (CanisterStats))]
  [ProtoInclude(10007, typeof (HandDrillStats))]
  [ProtoInclude(10008, typeof (GlowStickStats))]
  [ProtoInclude(10009, typeof (MedpackStats))]
  [ProtoInclude(10010, typeof (DisposableHackingToolStats))]
  [ProtoInclude(10011, typeof (MachineryPartStats))]
  [ProtoInclude(10012, typeof (HandheldAsteroidScannerStats))]
  [ProtoInclude(10013, typeof (LogItemStats))]
  [ProtoInclude(10014, typeof (GenericItemStats))]
  [ProtoInclude(10015, typeof (GrenadeStats))]
  [ProtoInclude(10016, typeof (PortableTurretStats))]
  [ProtoInclude(10017, typeof (RepairToolStats))]
  public abstract class DynamicObjectStats
  {
    public float? Health;
    public bool? IsStunned;
    public Dictionary<TypeOfDamage, float> Damages;
  }
}
