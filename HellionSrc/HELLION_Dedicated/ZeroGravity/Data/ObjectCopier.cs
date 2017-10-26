// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.ObjectCopier
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZeroGravity.Data
{
  public static class ObjectCopier
  {
    public static T Copy<T>(T source)
    {
      return ObjectCopier.DeepCopy<T>(source, 0);
    }

    public static T DeepCopy<T>(T source, int depth = 10)
    {
      if ((object) source == null || depth < 0)
        return source;
      Type type = source.GetType();
      if (type.IsPrimitive || type.IsEnum || type == typeof (string) || type == typeof (Decimal))
        return source;
      T instance;
      if ((object) source is Array)
      {
        object[] objArray = new object[type.GetArrayRank()];
        for (int dimension = 0; dimension < objArray.Length; ++dimension)
          objArray[dimension] = (object) ((object) source as Array).GetLength(dimension);
        instance = (T) Activator.CreateInstance(type, objArray);
        foreach (IEnumerable<int> source1 in Enumerable.Range(0, ((object) source as Array).Rank).Select<int, IEnumerable<int>>((Func<int, IEnumerable<int>>) (x => Enumerable.Range(((object) (T) source as Array).GetLowerBound(x), ((object) (T) source as Array).GetUpperBound(x) - ((object) (T) source as Array).GetLowerBound(x) + 1))).CartesianProduct<int>())
        {
          int[] array = source1.ToArray<int>();
          ((object) instance as Array).SetValue(ObjectCopier.DeepCopy<object>(((object) source as Array).GetValue(array), depth - 1), array);
        }
      }
      else
      {
        instance = (T) Activator.CreateInstance(type);
        if ((object) source is IDictionary)
        {
          foreach (object key in (IEnumerable) ((object) source as IDictionary).Keys)
            ((object) instance as IDictionary)[key] = ObjectCopier.DeepCopy<object>(((object) source as IDictionary)[key], depth - 1);
        }
        else if (type.IsGenericType && (object) source is IEnumerable && type.GetGenericArguments().Length == 1)
        {
          MethodInfo method = type.GetMethod("Add");
          foreach (object source1 in (object) source as IEnumerable)
            method.Invoke((object) instance, new object[1]
            {
              ObjectCopier.DeepCopy<object>(source1, depth - 1)
            });
        }
        else
        {
          foreach (FieldInfo field in type.GetFields())
          {
            object source1 = field.GetValue((object) source);
            field.SetValue((object) instance, ObjectCopier.DeepCopy<object>(source1, depth - 1));
          }
          foreach (PropertyInfo property in type.GetProperties())
          {
            if (property.CanRead && property.CanWrite && property.GetIndexParameters().Length == 0)
            {
              object source1 = property.GetValue((object) source, (object[]) null);
              property.SetValue((object) instance, ObjectCopier.DeepCopy<object>(source1, depth - 1), (object[]) null);
            }
          }
        }
      }
      return instance;
    }

    public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
    {
      IEnumerable<IEnumerable<T>> seed = (IEnumerable<IEnumerable<T>>) new IEnumerable<T>[1]
      {
        Enumerable.Empty<T>()
      };
      return sequences.Aggregate<IEnumerable<T>, IEnumerable<IEnumerable<T>>>(seed, (Func<IEnumerable<IEnumerable<T>>, IEnumerable<T>, IEnumerable<IEnumerable<T>>>) ((accumulator, sequence) => accumulator.SelectMany<IEnumerable<T>, T, IEnumerable<T>>((Func<IEnumerable<T>, IEnumerable<T>>) (accseq => sequence), (Func<IEnumerable<T>, T, IEnumerable<T>>) ((accseq, item) => accseq.Concat<T>((IEnumerable<T>) new T[1]
      {
        item
      })))));
    }
  }
}
