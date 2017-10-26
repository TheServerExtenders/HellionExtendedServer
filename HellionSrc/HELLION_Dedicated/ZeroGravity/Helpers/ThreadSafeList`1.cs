// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Helpers.ThreadSafeList`1
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ZeroGravity.Helpers
{
  public class ThreadSafeList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
  {
    public List<T> InnerList = new List<T>();
    private object lockList = new object();

    public void Lock()
    {
      Monitor.Enter(this.lockList);
    }

    public void Unlock()
    {
      Monitor.Exit(this.lockList);
    }

    public int IndexOf(T item)
    {
      lock (this.lockList)
        return this.InnerList.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
      lock (this.lockList)
        this.InnerList.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
      lock (this.lockList)
        this.InnerList.RemoveAt(index);
    }

    public T this[int index]
    {
      get
      {
        lock (this.lockList)
          return this.InnerList[index];
      }
      set
      {
        lock (this.lockList)
          this.InnerList[index] = value;
      }
    }

    public void Add(T item)
    {
      lock (this.lockList)
        this.InnerList.Add(item);
    }

    public void Clear()
    {
      lock (this.lockList)
        this.InnerList.Clear();
    }

    public bool Contains(T item)
    {
      lock (this.lockList)
        return this.InnerList.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      lock (this.lockList)
        this.InnerList.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get
      {
        lock (this.lockList)
          return this.InnerList.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public bool Remove(T item)
    {
      lock (this.lockList)
        return this.InnerList.Remove(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
      lock (this.lockList)
        return (IEnumerator<T>) this.InnerList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
