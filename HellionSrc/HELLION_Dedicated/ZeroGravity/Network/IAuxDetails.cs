// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.IAuxDetails
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  [ProtoInclude(1000, typeof (PDUAuxDetails))]
  [ProtoInclude(1001, typeof (MachineryPartSlotAuxDetails))]
  [ProtoInclude(1002, typeof (GeneratorCapacitorAuxDetails))]
  [ProtoInclude(1003, typeof (RefineryAuxDetails))]
  [ProtoInclude(1004, typeof (FTLAuxDetails))]
  [ProtoInclude(1005, typeof (GeneratorSolarAuxDetails))]
  [ProtoInclude(1006, typeof (RCSAuxDetails))]
  public interface IAuxDetails
  {
  }
}
