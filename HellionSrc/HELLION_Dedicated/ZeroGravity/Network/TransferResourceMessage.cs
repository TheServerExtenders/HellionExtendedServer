// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.TransferResourceMessage
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using ZeroGravity.Data;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class TransferResourceMessage : NetworkData
  {
    public int FromInSceneID = -1;
    public int ToInSceneID = -1;
    public ResourceLocationType FromLocationType;
    public long FromVesselGuid;
    public short FromCompartmentID;
    public ResourceLocationType ToLocationType;
    public long ToVesselGuid;
    public short ToCompartmentID;
    public ResourceType ResourceType;
    public float Quantity;
  }
}
