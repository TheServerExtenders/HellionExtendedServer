// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.SceneTriggerProximity
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;

namespace ZeroGravity.Objects
{
  public class SceneTriggerProximity
  {
    public List<long> ObjectsInTrigger = new List<long>();
    public int TriggerID;
    public int ActiveStateID;
    public int InactiveStateID;
  }
}
