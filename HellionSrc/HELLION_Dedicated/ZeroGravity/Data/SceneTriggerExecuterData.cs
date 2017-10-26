// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.SceneTriggerExecuterData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;

namespace ZeroGravity.Data
{
  public class SceneTriggerExecuterData : ISceneData
  {
    public int InSceneID;
    public int DefaultStateID;
    public TagAction TagAction;
    public string Tags;
    public List<SceneTriggerExecuterStateData> States;
    public List<SceneTriggerProximityData> ProximityTriggers;
  }
}
