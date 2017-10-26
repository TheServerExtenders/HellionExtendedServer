// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Properties
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace ZeroGravity
{
  public class Properties
  {
    private Dictionary<string, string> properties = new Dictionary<string, string>();
    private string fileName = "properties.ini";
    private DateTime propertiesChangedTime;

    public Properties(string fileName)
    {
      this.fileName = fileName;
      this.loadProperties();
    }

    private void loadProperties()
    {
      try
      {
        this.propertiesChangedTime = File.GetLastWriteTime(this.fileName);
        this.properties.Clear();
        foreach (string readAllLine in File.ReadAllLines(this.fileName))
        {
          try
          {
            string[] strArray = readAllLine.Split("=".ToCharArray(), 2);
            this.properties.Add(strArray[0].ToLower(), strArray[1]);
          }
          catch
          {
          }
        }
      }
      catch
      {
      }
    }

    public T GetProperty<T>(string propertyName)
    {
      if (File.GetLastWriteTime(this.fileName) != this.propertiesChangedTime)
        this.loadProperties();
      return (T) TypeDescriptor.GetConverter(typeof (T)).ConvertFrom((object) this.properties[propertyName.ToLower()]);
    }

    public void GetProperty<T>(string propertyName, ref T value)
    {
      try
      {
        value = this.GetProperty<T>(propertyName);
      }
      catch
      {
      }
    }

    public void SetProperty(string name, string value)
    {
      this.properties[name.Trim()] = value;
    }
  }
}
