// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ClassHasher
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZeroGravity
{
  public static class ClassHasher
  {
    public static uint GetClassHashCode(Type type, string nspace = null)
    {
      if (nspace == null)
        nspace = type.Namespace;
      HashSet<Type> classes = new HashSet<Type>();
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        foreach (Type type1 in assembly.GetTypes())
        {
          if (type.IsClass && type1.IsSubclassOf(type) || type.IsInterface && ((IEnumerable<Type>) type1.GetInterfaces()).Contains<Type>(type))
            ClassHasher.addClass(type1, classes, nspace);
        }
      }
      Type[] array = new Type[classes.Count];
      classes.CopyTo(array);
      Array.Sort<Type>(array, (Comparison<Type>) ((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal)));
      string str = "";
      foreach (Type type1 in array)
      {
        str = str + type1.Name + ": ";
        ClassHasher.addHashingData(type1, ref str, nspace);
        str += "\r\n";
      }
      uint num = 744748791;
      for (int index = 0; index < str.Length; ++index)
        num = (num + (uint) str[index]) * 3045351289U;
      return num;
    }

    private static void addClass(Type type, HashSet<Type> classes, string nspace)
    {
      if (!type.IsClass && !type.IsInterface && !type.IsEnum || type.IsNested || !(type.Namespace == nspace))
        return;
      if (type.IsArray)
        type = type.GetElementType();
      if (type.IsInterface)
      {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          foreach (Type type1 in assembly.GetTypes())
          {
            if (((IEnumerable<Type>) type1.GetInterfaces()).Contains<Type>(type))
              ClassHasher.addClass(type1, classes, nspace);
          }
        }
      }
      else if (type.IsEnum)
      {
        classes.Add(type);
      }
      else
      {
        if (!classes.Add(type))
          return;
        foreach (MemberInfo member in type.GetMembers())
        {
          if (member.MemberType == MemberTypes.Field)
            ClassHasher.addClass((member as FieldInfo).FieldType, classes, nspace);
          else if (member.MemberType == MemberTypes.Property)
            ClassHasher.addClass((member as PropertyInfo).PropertyType, classes, nspace);
          else if (member.MemberType == MemberTypes.Method)
          {
            foreach (ParameterInfo parameter in (member as MethodInfo).GetParameters())
              ClassHasher.addClass(parameter.ParameterType, classes, nspace);
          }
          else if (member.MemberType == MemberTypes.Constructor)
          {
            foreach (ParameterInfo parameter in (member as ConstructorInfo).GetParameters())
              ClassHasher.addClass(parameter.ParameterType, classes, nspace);
          }
        }
      }
    }

    private static void addHashingData(Type type, ref string str, string nspace)
    {
      if (type.IsEnum)
      {
        foreach (string name in Enum.GetNames(type))
          str = str + name + "|";
      }
      else
      {
        MemberInfo[] members = type.GetMembers();
        Array.Sort<MemberInfo>(members, (Comparison<MemberInfo>) ((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal)));
        bool flag = true;
        foreach (MemberInfo memberInfo in members)
        {
          if (memberInfo.DeclaringType == type)
          {
            str = str + (!flag ? ", " : "") + memberInfo.Name;
            flag = false;
            if (memberInfo.MemberType == MemberTypes.Field)
              ClassHasher.addHashingDataMember((memberInfo as FieldInfo).FieldType, ref str, nspace);
            else if (memberInfo.MemberType == MemberTypes.Property)
              ClassHasher.addHashingDataMember((memberInfo as PropertyInfo).PropertyType, ref str, nspace);
            else if (memberInfo.MemberType == MemberTypes.Method)
            {
              str = str + " " + (memberInfo as MethodInfo).ReturnType.ToString();
              foreach (ParameterInfo parameter in (memberInfo as MethodInfo).GetParameters())
              {
                str = str + " " + parameter.Name;
                ClassHasher.addHashingDataMember(parameter.ParameterType, ref str, nspace);
              }
            }
            else if (memberInfo.MemberType == MemberTypes.Constructor)
            {
              foreach (ParameterInfo parameter in (memberInfo as ConstructorInfo).GetParameters())
              {
                str = str + " " + parameter.Name;
                ClassHasher.addHashingDataMember(parameter.ParameterType, ref str, nspace);
              }
            }
          }
        }
      }
    }

    private static void addHashingDataMember(Type t, ref string str, string nspace)
    {
      if (t.IsPrimitive)
        str = str + " " + t.Name;
      if (!t.IsClass || t.IsNested || !(t.Namespace == nspace))
        return;
      ClassHasher.addHashingData(t, ref str, nspace);
    }
  }
}
