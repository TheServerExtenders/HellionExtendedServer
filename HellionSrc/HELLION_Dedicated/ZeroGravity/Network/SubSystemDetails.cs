// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.SubSystemDetails
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class SubSystemDetails
  {
    public IAuxDetails AuxDetails = (IAuxDetails) null;
    public string DebugInfo = (string) null;
    public int InSceneID;
    public SystemStatus Status;
    public SystemSecondaryStatus SecondaryStatus;
    public float OperationRate;
    public float InputFactor;
    public float InputFactorStandby;
  }
}
