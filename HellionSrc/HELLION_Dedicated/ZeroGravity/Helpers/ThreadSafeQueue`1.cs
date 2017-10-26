// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Helpers.ThreadSafeQueue`1
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;

namespace ZeroGravity.Helpers
{
  public class ThreadSafeQueue<T> : Queue<T>
  {
    private object lockObject = new object();

    public T DequeueSafe()
    {
      lock (this.lockObject)
        return this.Dequeue();
    }

    public void EnqueueSafe(T item)
    {
      lock (this.lockObject)
        this.Enqueue(item);
    }
  }
}
