// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.RoomAirTank
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;

namespace ZeroGravity.ShipComponents
{
  public class RoomAirTank : IResourceProvider
  {
    private HashSet<IResourceConsumer> _ConnectedConsumers = new HashSet<IResourceConsumer>();
    public float Volume = 0.0f;
    public float Quantity;
    public float AirQuality;

    public DistributionSystemType OutputType
    {
      get
      {
        return DistributionSystemType.Air;
      }
    }

    public HashSet<IResourceConsumer> ConnectedConsumers
    {
      get
      {
        return this._ConnectedConsumers;
      }
    }

    public float MaxOutput
    {
      get
      {
        return 100f;
      }
    }

    public float NominalOutput
    {
      get
      {
        return 100f;
      }
      set
      {
      }
    }

    public float OperationRate
    {
      get
      {
        return 1f;
      }
      set
      {
      }
    }

    public float Output
    {
      get
      {
        return 100f;
      }
      set
      {
      }
    }

    public RoomAirTank(Room room)
    {
      this.Volume = (float) Math.Round((double) room.Volume * 4.0 / 10.0) * 10f;
    }
  }
}
