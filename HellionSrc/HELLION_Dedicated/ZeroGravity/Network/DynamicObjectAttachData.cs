// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.DynamicObjectAttachData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using ZeroGravity.Objects;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class DynamicObjectAttachData
  {
    public long ParentGUID = -1;
    public SpaceObjectType ParentType = SpaceObjectType.None;
    public bool IsAttached;
    public short InventorySlotID;
    public AttachPointDetails APDetails;
    public float[] LocalPosition;
    public float[] LocalRotation;
    public float[] Velocity;
    public float[] Torque;
    public float[] ThrowForce;
  }
}
