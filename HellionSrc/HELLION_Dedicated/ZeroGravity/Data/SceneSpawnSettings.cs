// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.SceneSpawnSettings
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;

namespace ZeroGravity.Data
{
  [Serializable]
  public class SceneSpawnSettings : ISceneData
  {
    public SpawnSettingsCase Case = SpawnSettingsCase.EnableIf;
    public float MinHealth = -1f;
    public float MaxHealth = -1f;
    public string Tag;
  }
}
