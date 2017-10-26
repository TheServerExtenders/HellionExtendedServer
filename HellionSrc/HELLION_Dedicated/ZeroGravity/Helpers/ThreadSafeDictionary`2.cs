// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Helpers.ThreadSafeDictionary`2
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ZeroGravity.Helpers
{
  public class ThreadSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
  {
    public Dictionary<TKey, TValue> InnerDictionary = new Dictionary<TKey, TValue>();
    private object lockDict = new object();

    public void Lock()
    {
      Monitor.Enter(this.lockDict);
    }

    public void Unlock()
    {
      Monitor.Exit(this.lockDict);
    }

    public void Add(TKey key, TValue value)
    {
      lock (this.lockDict)
        this.InnerDictionary.Add(key, value);
    }

    public bool ContainsKey(TKey key)
    {
      lock (this.lockDict)
        return this.InnerDictionary.ContainsKey(key);
    }

    public ICollection<TKey> Keys
    {
      get
      {
        lock (this.lockDict)
          return (ICollection<TKey>) this.InnerDictionary.Keys;
      }
    }

    public bool Remove(TKey key)
    {
      lock (this.lockDict)
        return this.InnerDictionary.Remove(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      lock (this.lockDict)
        return this.InnerDictionary.TryGetValue(key, out value);
    }

    public ICollection<TValue> Values
    {
      get
      {
        lock (this.lockDict)
          return (ICollection<TValue>) this.InnerDictionary.Values;
      }
    }

    public TValue this[TKey key]
    {
      get
      {
        lock (this.lockDict)
          return this.InnerDictionary[key];
      }
      set
      {
        lock (this.lockDict)
          this.InnerDictionary[key] = value;
      }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
      this.Add(item.Key, item.Value);
    }

    public void Clear()
    {
      lock (this.lockDict)
        this.InnerDictionary.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      TValue obj;
      return this.TryGetValue(item.Key, out obj) && (object) obj == (object) item.Value;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      lock (this.lockDict)
        ((ICollection<KeyValuePair<TKey, TValue>>) this.InnerDictionary).CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get
      {
        lock (this.lockDict)
          return this.InnerDictionary.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      return this.Remove(item.Key);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      lock (this.lockDict)
        return (IEnumerator<KeyValuePair<TKey, TValue>>) this.InnerDictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    public TValue First()
    {
      lock (this.lockDict)
      {
        if (this.InnerDictionary.Count > 0)
        {
          using (Dictionary<TKey, TValue>.Enumerator enumerator = this.InnerDictionary.GetEnumerator())
          {
            if (enumerator.MoveNext())
              return enumerator.Current.Value;
          }
        }
        return default (TValue);
      }
    }
  }
}
