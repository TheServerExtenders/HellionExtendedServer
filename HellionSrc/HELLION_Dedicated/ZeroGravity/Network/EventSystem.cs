// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.EventSystem
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.Threading;
using ZeroGravity.Helpers;

namespace ZeroGravity.Network
{
  public class EventSystem
  {
    private List<Type> invokeImmediatelyDataTypes = new List<Type>();
    private ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate> networkDataGroups = new ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate>();
    private ThreadSafeQueue<NetworkData> networkBuffer = new ThreadSafeQueue<NetworkData>();
    private ThreadSafeDictionary<EventSystem.InternalEventType, EventSystem.InternalEventsDelegate> internalDataGroups = new ThreadSafeDictionary<EventSystem.InternalEventType, EventSystem.InternalEventsDelegate>();
    private ThreadSafeQueue<EventSystem.InternalEventData> internalBuffer = new ThreadSafeQueue<EventSystem.InternalEventData>();

    public EventSystem()
    {
      this.invokeImmediatelyDataTypes.Add(typeof (PlayerHitMessage));
    }

    public void AddListener(Type group, EventSystem.NetworkDataDelegate function)
    {
      if (this.networkDataGroups.ContainsKey(group))
      {
        ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate> networkDataGroups = this.networkDataGroups;
        Type index = group;
        networkDataGroups[index] = networkDataGroups[index] + function;
      }
      else
        this.networkDataGroups[group] = function;
    }

    public void RemoveListener(Type group, EventSystem.NetworkDataDelegate function)
    {
      if (!this.networkDataGroups.ContainsKey(group))
        return;
      ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate> networkDataGroups = this.networkDataGroups;
      Type index = group;
      networkDataGroups[index] = networkDataGroups[index] - function;
    }

    public void Invoke(NetworkData data)
    {
      if (this.networkDataGroups.ContainsKey(data.GetType()) && this.networkDataGroups[data.GetType()] != null)
      {
        if (this.invokeImmediatelyDataTypes.Contains(data.GetType()) || Thread.CurrentThread.ManagedThreadId == Server.MainThreadID)
          this.networkDataGroups[data.GetType()](data);
        else
          this.networkBuffer.EnqueueSafe(data);
      }
      else if (!(data is MainServerGenericResponse) || (data as MainServerGenericResponse).Response != ResponseResult.Success)
      {
        if (data is MainServerGenericResponse)
          Dbg.Info((object) "Unhandled network package", (object) data.Sender, (object) data.GetType(), (object) (data as MainServerGenericResponse).Message);
        else
          Dbg.Info((object) "Unhandled network package", (object) data.Sender, (object) data.GetType());
      }
    }

    public void InvokeQueuedData()
    {
      while (this.networkBuffer.Count > 0)
      {
        NetworkData data = this.networkBuffer.DequeueSafe();
        if (this.networkDataGroups.ContainsKey(data.GetType()) && this.networkDataGroups[data.GetType()] != null)
          this.networkDataGroups[data.GetType()](data);
      }
      while (this.internalBuffer.Count > 0)
      {
        EventSystem.InternalEventData data = this.internalBuffer.DequeueSafe();
        if (this.internalDataGroups.ContainsKey(data.Type) && this.internalDataGroups[data.Type] != null)
          this.internalDataGroups[data.Type](data);
      }
    }

    public void AddListener(EventSystem.InternalEventType group, EventSystem.InternalEventsDelegate function)
    {
      if (this.internalDataGroups.ContainsKey(group))
      {
        ThreadSafeDictionary<EventSystem.InternalEventType, EventSystem.InternalEventsDelegate> internalDataGroups = this.internalDataGroups;
        EventSystem.InternalEventType index = group;
        internalDataGroups[index] = internalDataGroups[index] + function;
      }
      else
        this.internalDataGroups[group] = function;
    }

    public void RemoveListener(EventSystem.InternalEventType group, EventSystem.InternalEventsDelegate function)
    {
      if (!this.internalDataGroups.ContainsKey(group))
        return;
      ThreadSafeDictionary<EventSystem.InternalEventType, EventSystem.InternalEventsDelegate> internalDataGroups = this.internalDataGroups;
      EventSystem.InternalEventType index = group;
      internalDataGroups[index] = internalDataGroups[index] - function;
    }

    public void Invoke(EventSystem.InternalEventData data)
    {
      if (this.internalDataGroups.ContainsKey(data.Type) && this.internalDataGroups[data.Type] != null)
      {
        if (Thread.CurrentThread.ManagedThreadId == Server.MainThreadID)
          this.internalDataGroups[data.Type](data);
        else
          this.internalBuffer.EnqueueSafe(data);
      }
      else
        Dbg.Error((object) "Cannot invoke ", (object) data.Type, (object) data);
    }

    public class InternalEventData
    {
      public EventSystem.InternalEventType Type;
      public object[] Objects;

      public InternalEventData(EventSystem.InternalEventType type, params object[] objects)
      {
        this.Type = type;
        this.Objects = objects;
      }
    }

    public delegate void NetworkDataDelegate(NetworkData data);

    public delegate void InternalEventsDelegate(EventSystem.InternalEventData data);

    public enum InternalEventType
    {
      GetPlayer,
    }
  }
}
