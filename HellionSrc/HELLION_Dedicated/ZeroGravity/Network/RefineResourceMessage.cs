// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.RefineResourceMessage
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using ZeroGravity.Data;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class RefineResourceMessage : NetworkData
  {
    public int FromInSceneID = -1;
    public int ToInSceneID = -1;
    public ResourceLocationType FromLocationType;
    public long FromVesselGuid;
    public int FromCompartmentID;
    public long ToVesselGuid;
    public ResourceType ResourceType;
    public float Quantity;
  }
}
