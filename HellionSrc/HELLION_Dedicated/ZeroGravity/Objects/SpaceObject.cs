// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.SpaceObject
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Helpers;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Spawn;

namespace ZeroGravity.Objects
{
  public abstract class SpaceObject
  {
    private double _ScanningRange = -1.0;
    public ThreadSafeList<DynamicObject> DynamicObjects = new ThreadSafeList<DynamicObject>();
    public ThreadSafeList<Corpse> Corpses = new ThreadSafeList<Corpse>();
    protected double baseSunHeatTransferPerSec = 9.4E+21;
    public long GUID;
    public bool IsExposedToSunlight;
    public double SqrDistanceFromSun;
    public bool IsPartOfSpawnSystem;

    public double ScanningRange
    {
      get
      {
        return this._ScanningRange;
      }
      set
      {
        this._ScanningRange = value < 10000.0 ? 10000.0 : value;
      }
    }

    public virtual SpaceObjectType ObjectType
    {
      get
      {
        return SpaceObjectType.None;
      }
    }

    public virtual SpaceObject Parent { get; set; }

    public SpaceObject(long guid)
    {
      this.GUID = guid;
    }

    public virtual Vector3D Position
    {
      get
      {
        return Vector3D.Zero;
      }
    }

    public virtual Vector3D Velocity
    {
      get
      {
        return Vector3D.Zero;
      }
    }

    public virtual InitializeSpaceObjectMessage GetInitializeMessage()
    {
      return (InitializeSpaceObjectMessage) null;
    }

    public virtual SpawnObjectResponseData GetSpawnResponseData(Player pl)
    {
      return (SpawnObjectResponseData) null;
    }

    public virtual void UpdateTimers(double deltaTime)
    {
      this.IsExposedToSunlight = this.isExposedToSunlight(out this.SqrDistanceFromSun);
    }

    public virtual void Destroy()
    {
      foreach (SpaceObject spaceObject in new List<DynamicObject>((IEnumerable<DynamicObject>) this.DynamicObjects.InnerList))
        spaceObject.Destroy();
      NetworkController networkController = Server.Instance.NetworkController;
      DestroyObjectMessage destroyObjectMessage = new DestroyObjectMessage();
      destroyObjectMessage.ID = this.GUID;
      destroyObjectMessage.ObjectType = this.ObjectType;
      long skipPlalerGUID = -1;
      int depth = 4;
      networkController.SendToClientsSubscribedToParents((NetworkData) destroyObjectMessage, this, skipPlalerGUID, depth);
      if (this is Player)
        Server.Instance.Remove(this as Player);
      else if (this is SpaceObjectVessel)
        Server.Instance.Remove(this as SpaceObjectVessel);
      else if (this is DynamicObject)
        Server.Instance.Remove(this as DynamicObject);
      else if (this is Corpse)
        Server.Instance.Remove(this as Corpse);
      if (this.Parent is Pivot)
        Server.Instance.SolarSystem.RemoveArtificialBody((ArtificialBody) (this.Parent as Pivot));
      this.Parent = (SpaceObject) null;
      if (!this.IsPartOfSpawnSystem)
        return;
      SpawnManager.RemoveSpawnSystemObject(this, false);
    }

    private bool isExposedToSunlight(out double sqrDistFromSun)
    {
      sqrDistFromSun = 0.0;
      CelestialBody celestialBody1 = Server.Instance.SolarSystem.GetCelestialBodies()[0];
      while (celestialBody1.Parent != null)
        celestialBody1 = celestialBody1.Parent;
      Vector3D vector3D1;
      if (this is SpaceObjectVessel)
      {
        vector3D1 = this.Position;
      }
      else
      {
        if (!(this is SpaceObjectTransferable) || !(this.Parent is Pivot))
          return false;
        vector3D1 = this.Parent.Position + (this as SpaceObjectTransferable).LocalPosition;
      }
      Vector3D vector3D2 = celestialBody1.Position - vector3D1;
      sqrDistFromSun = vector3D2.SqrMagnitude;
      foreach (CelestialBody celestialBody2 in Server.Instance.SolarSystem.GetCelestialBodies())
      {
        if (celestialBody2.Parent == celestialBody1)
        {
          Vector3D vector = celestialBody2.Position - vector3D1;
          if (vector.SqrMagnitude < vector3D2.SqrMagnitude && Vector3D.Project(vector, vector3D2).Normalized == vector3D2.Normalized)
          {
            double magnitude = Vector3D.ProjectOnPlane(vector, vector3D2).Magnitude;
            if (celestialBody2.Radius > magnitude)
              return false;
          }
        }
      }
      return true;
    }

    public float SpaceExposureTemperature(float currentTemperature, float heatCollectionFactor, float heatDissipationFactor, float mass, double deltaTime)
    {
      double num1 = 0.0;
      if (this.IsExposedToSunlight)
        num1 = this.baseSunHeatTransferPerSec * (double) heatCollectionFactor / (double) mass / this.SqrDistanceFromSun;
      double num2 = (double) heatDissipationFactor / (double) mass * ((double) currentTemperature + 273.15);
      return currentTemperature + (float) ((num1 - num2) * deltaTime);
    }

    public List<SpaceObject> GetParents(bool includeMe, int depth = 10)
    {
      List<SpaceObject> spaceObjectList = new List<SpaceObject>();
      for (SpaceObject spaceObject = includeMe ? this : this.Parent; spaceObject != null && depth > 0; --depth)
      {
        spaceObjectList.Add(spaceObject);
        spaceObject = spaceObject.Parent;
      }
      return spaceObjectList;
    }
  }
}
