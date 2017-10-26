// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.IAirConsumer
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

namespace ZeroGravity.ShipComponents
{
  public interface IAirConsumer
  {
    float AirQualityDegradationRate { get; }

    float AirQuantityDecreaseRate { get; }

    bool AffectsQuality { get; }

    bool AffectsQuantity { get; }
  }
}
