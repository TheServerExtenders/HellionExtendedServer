// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.VesselObjectID
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class VesselObjectID
  {
    public long VesselGUID;
    public int InSceneID;

    public VesselObjectID()
    {
    }

    public VesselObjectID(long vesselGUID, int inSceneID)
    {
      this.VesselGUID = vesselGUID;
      this.InSceneID = inSceneID;
    }

    public override string ToString()
    {
      return "[" + (object) this.VesselGUID + ", " + (object) this.InSceneID + "]";
    }

    public override bool Equals(object obj)
    {
      if (obj == null || !(obj is VesselObjectID))
        return false;
      VesselObjectID vesselObjectId = obj as VesselObjectID;
      return this.VesselGUID == vesselObjectId.VesselGUID && this.InSceneID == vesselObjectId.InSceneID;
    }

    public override int GetHashCode()
    {
      return (int) ((17L * 23L + this.VesselGUID) * 23L + (long) this.InSceneID & 268435455L);
    }
  }
}
