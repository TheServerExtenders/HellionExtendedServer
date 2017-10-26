// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.AirConsumerFire
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

namespace ZeroGravity.ShipComponents
{
  public class AirConsumerFire : IAirConsumer
  {
    public bool Persistent = false;
    private FireType _Type;
    private float _AirQualityDegradationRate;

    public float AirQualityDegradationRate
    {
      get
      {
        return this._AirQualityDegradationRate;
      }
    }

    public FireType Type
    {
      get
      {
        return this._Type;
      }
      set
      {
        this._Type = value;
        if (value == FireType.Small)
          this._AirQualityDegradationRate = 0.5f;
        else if (value == FireType.Medium)
        {
          this._AirQualityDegradationRate = 2.5f;
        }
        else
        {
          if (value != FireType.Large)
            return;
          this._AirQualityDegradationRate = 15f;
        }
      }
    }

    public float AirQuantityDecreaseRate
    {
      get
      {
        return 0.0f;
      }
    }

    public bool AffectsQuality
    {
      get
      {
        return true;
      }
    }

    public bool AffectsQuantity
    {
      get
      {
        return false;
      }
    }

    public AirConsumerFire(FireType type)
    {
      this.Type = type;
    }
  }
}
