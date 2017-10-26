// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.AirConsumerBreach
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

namespace ZeroGravity.ShipComponents
{
  public class AirConsumerBreach : IAirConsumer
  {
    private BreachType _Type;
    private float _VolumeDecreaseRate;

    public float AirQualityDegradationRate
    {
      get
      {
        return 0.0f;
      }
    }

    public BreachType Type
    {
      get
      {
        return this._Type;
      }
      set
      {
        this._Type = value;
        if (value == BreachType.Micro)
          this._VolumeDecreaseRate = 0.15f;
        else if (value == BreachType.Small)
        {
          this._VolumeDecreaseRate = 1.5f;
        }
        else
        {
          if (value != BreachType.Large)
            return;
          this._VolumeDecreaseRate = 150f;
        }
      }
    }

    public float AirQuantityDecreaseRate
    {
      get
      {
        return this._VolumeDecreaseRate;
      }
    }

    public bool AffectsQuality
    {
      get
      {
        return false;
      }
    }

    public bool AffectsQuantity
    {
      get
      {
        return true;
      }
    }

    public AirConsumerBreach(BreachType type)
    {
      this.Type = type;
    }
  }
}
