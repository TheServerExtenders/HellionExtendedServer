// Decompiled with JetBrains decompiler
// Type: ZeroGravity.UpdateTimer
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

namespace ZeroGravity
{
  public class UpdateTimer
  {
    private double timePassed;
    private double updateInverval;
    public UpdateTimer.TimeStepDelegate OnTick;

    public UpdateTimer.TimerStep Step { get; private set; }

    public UpdateTimer(UpdateTimer.TimerStep type)
    {
      this.Step = type;
      switch (type)
      {
        case UpdateTimer.TimerStep.Step_0_1_sec:
          this.updateInverval = 0.1;
          break;
        case UpdateTimer.TimerStep.Step_0_5_sec:
          this.updateInverval = 0.5;
          break;
        case UpdateTimer.TimerStep.Step_1_0_sec:
          this.updateInverval = 1.0;
          break;
        case UpdateTimer.TimerStep.Step_1_0_min:
          this.updateInverval = 60.0;
          break;
        case UpdateTimer.TimerStep.Step_1_0_hr:
          this.updateInverval = 3600.0;
          break;
      }
    }

    public void AddTime(double deltaTime)
    {
      this.timePassed = this.timePassed + deltaTime;
      if (this.timePassed <= this.updateInverval)
        return;
      if (this.OnTick != null)
        this.OnTick(this.timePassed);
      this.timePassed = 0.0;
    }

    public enum TimerStep
    {
      Step_0_1_sec = 1,
      Step_0_5_sec = 2,
      Step_1_0_sec = 3,
      Step_1_0_min = 4,
      Step_1_0_hr = 5,
    }

    public delegate void TimeStepDelegate(double dbl);
  }
}
