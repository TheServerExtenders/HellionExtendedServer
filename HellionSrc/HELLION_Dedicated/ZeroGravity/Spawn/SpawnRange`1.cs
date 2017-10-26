// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Spawn.SpawnRange`1
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

namespace ZeroGravity.Spawn
{
  public struct SpawnRange<T>
  {
    public T Min;
    public T Max;

    public SpawnRange(T min, T max)
    {
      this.Min = min;
      this.Max = max;
    }
  }
}
