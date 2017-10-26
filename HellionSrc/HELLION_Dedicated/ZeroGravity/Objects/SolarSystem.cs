// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.SolarSystem
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Helpers;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Spawn;

namespace ZeroGravity.Objects
{
  public class SolarSystem
  {
    private double currentTime = 0.0;
    private List<CelestialBody> celesitalBodies = new List<CelestialBody>();
    private ThreadSafeList<ArtificialBody> artificialBodies = new ThreadSafeList<ArtificialBody>();
    private List<Station> stations = new List<Station>();
    public bool CheckDestroyMarkedBodies = false;
    public const double VisibilityLimitDestroySqr = 225000000.0;
    public const double VisibilityLimitLoadSqr = 100000000.0;
    public const double DetailsLimitUnsubscribe = 6250000.0;
    public const double DetailsLimitSubscribe = 2250000.0;

    public double CurrentTime
    {
      get
      {
        return this.currentTime;
      }
    }

    public int ArtificialBodiesCount
    {
      get
      {
        return this.artificialBodies.Count;
      }
    }

    public void AddCelestialBody(CelestialBody body)
    {
      this.celesitalBodies.Add(body);
    }

    public CelestialBody GetCelestialBody(long guid)
    {
      return this.celesitalBodies.Find((Predicate<CelestialBody>) (m => m.GUID == guid));
    }

    public CelestialBody FindCelestialBodyParent(Vector3D position)
    {
      CelestialBody celesitalBody = this.celesitalBodies[0];
      double num = (this.celesitalBodies[0].Position - position).SqrMagnitude;
      for (int index = 1; index < this.celesitalBodies.Count; ++index)
      {
        double sqrMagnitude = (this.celesitalBodies[index].Position - position).SqrMagnitude;
        if (sqrMagnitude < this.celesitalBodies[index].Orbit.GravityInfluenceRadiusSquared && sqrMagnitude < num)
        {
          celesitalBody = this.celesitalBodies[index];
          num = sqrMagnitude;
        }
      }
      return celesitalBody;
    }

    public void AddArtificialBody(ArtificialBody body)
    {
      this.artificialBodies.Add(body);
    }

    public void RemoveArtificialBody(ArtificialBody body)
    {
      this.artificialBodies.Remove(body);
    }

    public void CalculatePositionsAfterTime(double time)
    {
      this.currentTime = time;
      foreach (CelestialBody celesitalBody in this.celesitalBodies)
        celesitalBody.Update(time);
    }

    public void UpdatePositions(double timeDelta)
    {
      this.currentTime = this.currentTime + timeDelta;
      foreach (CelestialBody celesitalBody in this.celesitalBodies)
        celesitalBody.Update(timeDelta);
      try
      {
        this.artificialBodies.Lock();
        foreach (ArtificialBody inner in this.artificialBodies.InnerList)
          inner.Update(timeDelta);
        foreach (ArtificialBody inner in this.artificialBodies.InnerList)
          inner.AfterUpdate(timeDelta);
        if (!this.CheckDestroyMarkedBodies)
          return;
        for (int index = this.artificialBodies.InnerList.Count - 1; index >= 0; --index)
        {
          if (this.artificialBodies.InnerList[index].MarkForDestruction)
            Server.Instance.DestroyArtificialBody(this.artificialBodies.InnerList[index], true, false);
        }
        this.CheckDestroyMarkedBodies = false;
      }
      finally
      {
        this.artificialBodies.Unlock();
      }
    }

    public void SendMovementMessage()
    {
      try
      {
        this.artificialBodies.Lock();
        foreach (Player allPlayer in Server.Instance.AllPlayers)
        {
          if (allPlayer.EnviromentReady && allPlayer.IsAlive)
          {
            MovementMessage movementMessage1 = new MovementMessage();
            movementMessage1.SolarSystemTime = this.currentTime;
            movementMessage1.Timestamp = (float) Server.Instance.RunTime.TotalSeconds;
            movementMessage1.Transforms = new List<ObjectTransform>();
            if (allPlayer.Parent is SpaceObjectVessel && (allPlayer.Parent as SpaceObjectVessel).IsDocked)
            {
              SpaceObjectVessel parent = allPlayer.Parent as SpaceObjectVessel;
              parent.Orbit.CopyDataFrom(parent.DockedToMainVessel.Orbit, 0.0, true);
            }
            foreach (ArtificialBody inner in this.artificialBodies.InnerList)
            {
              if (allPlayer.Parent == null || inner.IsDistresActive || inner.ObjectType != SpaceObjectType.Ship && inner.ObjectType != SpaceObjectType.PlayerPivot && (inner.ObjectType != SpaceObjectType.DynamicObjectPivot && inner.ObjectType != SpaceObjectType.CorpsePivot) || allPlayer.AuthorizedSpawnPoint != null && allPlayer.AuthorizedSpawnPoint.Ship == inner || allPlayer.Parent.Position.DistanceSquared(inner.Position) <= 1.25E+18)
              {
                ObjectTransform objectTransform = new ObjectTransform()
                {
                  GUID = inner.GUID,
                  Type = inner.ObjectType
                };
                if (inner.CurrentCourse != null && inner.CurrentCourse.IsInProgress)
                  objectTransform.Maneuver = inner.CurrentCourse.CurrentData();
                if (inner.StabilizeToTargetObj != null)
                {
                  if (inner.StabilizeToTargetTime >= allPlayer.LastMovementMessageSolarSystemTime || allPlayer.UpdateArtificialBodyMovement.Count > 0 && allPlayer.UpdateArtificialBodyMovement.Contains(inner.GUID))
                  {
                    objectTransform.StabilizeToTargetGUID = new long?(inner.StabilizeToTargetObj.GUID);
                    objectTransform.StabilizeToTargetRelPosition = inner.StabilizeToTargetRelPosition.ToArray();
                  }
                }
                else if (inner.Orbit.IsOrbitValid)
                {
                  if (inner.Orbit.LastChangeTime >= allPlayer.LastMovementMessageSolarSystemTime || allPlayer.UpdateArtificialBodyMovement.Count > 0 && allPlayer.UpdateArtificialBodyMovement.Contains(inner.GUID))
                  {
                    objectTransform.Orbit = new OrbitData()
                    {
                      ParentGUID = inner.Orbit.Parent.CelestialBody.GUID
                    };
                    inner.Orbit.FillOrbitData(ref objectTransform.Orbit, (SpaceObjectVessel) null);
                  }
                }
                else
                  objectTransform.Realtime = new RealtimeData()
                  {
                    ParentGUID = inner.Orbit.Parent.CelestialBody.GUID,
                    Position = inner.Orbit.RelativePosition.ToArray(),
                    Velocity = inner.Orbit.RelativeVelocity.ToArray()
                  };
                if (objectTransform.Orbit != null || objectTransform.Realtime != null || inner.LastOrientationChangeTime >= allPlayer.LastMovementMessageSolarSystemTime && (allPlayer.Parent == inner || (allPlayer.Parent as ArtificialBody).Position.DistanceSquared(inner.Position) < 225000000.0))
                {
                  objectTransform.Forward = inner.Forward.ToFloatArray();
                  objectTransform.Up = inner.Up.ToFloatArray();
                  objectTransform.AngularVelocity = (inner.AngularVelocity * (180.0 / System.Math.PI)).ToFloatArray();
                  objectTransform.RotationVec = inner.Rotation.ToFloatArray();
                }
                objectTransform.CharactersMovement = new List<CharacterMovementMessage>();
                objectTransform.DynamicObjectsMovement = new List<MoveDynamicObectMessage>();
                objectTransform.CorpsesMovement = new List<MoveCorpseObectMessage>();
                if (inner is SpaceObjectVessel && (allPlayer.Parent.GUID == inner.GUID || allPlayer.IsSubscribedTo(inner.GUID)))
                {
                  SpaceObjectVessel spaceObjectVessel = inner as SpaceObjectVessel;
                  foreach (Player player in spaceObjectVessel.VesselCrew)
                  {
                    if (player.PlayerReady)
                    {
                      CharacterMovementMessage characterMovementMessage = player.GetCharacterMovementMessage();
                      if (characterMovementMessage != null)
                        objectTransform.CharactersMovement.Add(characterMovementMessage);
                    }
                  }
                  foreach (DynamicObject dynamicObject in spaceObjectVessel.DynamicObjects)
                  {
                    if (dynamicObject.PlayerReceivesMovementMessage(allPlayer.GUID) && dynamicObject.LastChangeTime >= allPlayer.LastMovementMessageSolarSystemTime)
                    {
                      MoveDynamicObectMessage dynamicObectMessage = dynamicObject.GetMoveDynamicObectMessage();
                      if (dynamicObectMessage != null)
                        objectTransform.DynamicObjectsMovement.Add(dynamicObectMessage);
                    }
                  }
                  foreach (Corpse corpse in spaceObjectVessel.Corpses)
                  {
                    if (corpse.PlayerReceivesMovementMessage(allPlayer.GUID))
                    {
                      MoveCorpseObectMessage movementMessage2 = corpse.GetMovementMessage();
                      if (movementMessage2 != null)
                        objectTransform.CorpsesMovement.Add(movementMessage2);
                    }
                  }
                }
                else if (inner is Pivot)
                {
                  Pivot pivot = inner as Pivot;
                  if (pivot.ObjectType == SpaceObjectType.PlayerPivot)
                  {
                    CharacterMovementMessage characterMovementMessage = ((Player) pivot.Child).GetCharacterMovementMessage();
                    if (characterMovementMessage != null)
                      objectTransform.CharactersMovement.Add(characterMovementMessage);
                  }
                  else if (pivot.ObjectType == SpaceObjectType.CorpsePivot)
                  {
                    Corpse child = pivot.Child as Corpse;
                    if (child.PlayerReceivesMovementMessage(allPlayer.GUID))
                    {
                      MoveCorpseObectMessage movementMessage2 = child.GetMovementMessage();
                      if (movementMessage2 != null)
                        objectTransform.CorpsesMovement.Add(movementMessage2);
                    }
                  }
                  else if (pivot.ObjectType == SpaceObjectType.DynamicObjectPivot)
                  {
                    DynamicObject child = pivot.Child as DynamicObject;
                    if (child.PlayerReceivesMovementMessage(allPlayer.GUID))
                    {
                      MoveDynamicObectMessage dynamicObectMessage = child.GetMoveDynamicObectMessage();
                      if (dynamicObectMessage != null)
                        objectTransform.DynamicObjectsMovement.Add(dynamicObectMessage);
                    }
                  }
                }
                if (objectTransform.Orbit != null || objectTransform.Realtime != null || (objectTransform.Maneuver != null || objectTransform.Forward != null) || (objectTransform.CharactersMovement != null && objectTransform.CharactersMovement.Count > 0 || objectTransform.CorpsesMovement != null && objectTransform.CorpsesMovement.Count > 0) || objectTransform.DynamicObjectsMovement != null && objectTransform.DynamicObjectsMovement.Count > 0 || objectTransform.StabilizeToTargetGUID.HasValue)
                {
                  if (inner.StabilizeToTargetObj != null)
                    movementMessage1.Transforms.Add(objectTransform);
                  else
                    movementMessage1.Transforms.Insert(0, objectTransform);
                }
              }
            }
            allPlayer.LastMovementMessageSolarSystemTime = this.currentTime;
            allPlayer.UpdateArtificialBodyMovement.Clear();
            Server.Instance.NetworkController.SendToGameClient(allPlayer.GUID, (NetworkData) movementMessage1);
          }
        }
        foreach (Player allPlayer in Server.Instance.AllPlayers)
          allPlayer.TransformDataList.Clear();
      }
      finally
      {
        this.artificialBodies.Unlock();
      }
    }

    public void InitializeData()
    {
      foreach (CelestialBodyData celestialBodyData in StaticData.CelestialBodyDataList)
      {
        CelestialBody celestialBody = new CelestialBody((long) celestialBodyData.GUID);
        celestialBody.Set(celestialBodyData.ParentGUID == -1L ? (CelestialBody) null : this.GetCelestialBody(celestialBodyData.ParentGUID), celestialBodyData.Mass, celestialBodyData.Radius * Server.CELESTIAL_BODY_RADIUS_MULTIPLIER, celestialBodyData.RotationPeriod, celestialBodyData.Eccentricity, celestialBodyData.SemiMajorAxis, celestialBodyData.Inclination, celestialBodyData.ArgumentOfPeriapsis, celestialBodyData.LongitudeOfAscendingNode, this.currentTime, celestialBodyData.AtmosphereLevel1, celestialBodyData.AtmosphereLevel2, celestialBodyData.AtmosphereLevel3, celestialBodyData.ResourceResolution);
        this.celesitalBodies.Add(celestialBody);
      }
    }

    public ArtificialBody[] GetArtificialBodies()
    {
      return this.artificialBodies.InnerList.ToArray();
    }

    public List<CelestialBody> GetCelestialBodies()
    {
      return this.celesitalBodies;
    }

    public ArtificialBody[] GetArtificialBodieslsInRange(ArtificialBody ab, float radius)
    {
      float sqRadius = radius * radius;
      return this.artificialBodies.InnerList.FindAll((Predicate<ArtificialBody>) (m =>
      {
        if (m != ab && m.Parent == ab.Parent)
          return (m.Position - ab.Position).SqrMagnitude <= (double) sqRadius;
        return false;
      })).ToArray();
    }

    public void GetSpawnPosition(SpaceObjectType type, double objectRadius, bool checkPosition, out Vector3D position, out Vector3D velocity, out Vector3D forward, out Vector3D up, List<long> nearArtificialBodyGUIDs, List<long> celestialBodyGUIDs, Vector3D? positionOffset, Vector3D? velocityAtPosition, QuaternionD? localRotation, double distanceFromSurfacePercMin, double distanceFromSurfacePercMax, SpawnRuleOrbit spawnRuleOrbit, double celestialBodyDeathDistanceMultiplier, double artificialBodyDistanceCheck)
    {
      position = Vector3D.Zero;
      velocity = Vector3D.Zero;
      forward = Vector3D.Forward;
      up = Vector3D.Up;
      CelestialBody parentBody = (CelestialBody) null;
      ArtificialBody artificialBody = (ArtificialBody) null;
      if (nearArtificialBodyGUIDs != null && nearArtificialBodyGUIDs.Count > 0)
      {
        artificialBody = nearArtificialBodyGUIDs.Count != 1 ? Server.Instance.GetObject(nearArtificialBodyGUIDs[MathHelper.RandomRange(0, nearArtificialBodyGUIDs.Count)]) as ArtificialBody : Server.Instance.GetObject(nearArtificialBodyGUIDs[0]) as ArtificialBody;
        if (artificialBody != null)
        {
          parentBody = artificialBody.Orbit.Parent.CelestialBody;
          position = artificialBody.Orbit.RelativePosition + (positionOffset.HasValue ? positionOffset.Value : Vector3D.Zero);
          velocity = artificialBody.Orbit.RelativeVelocity;
          if (position.SqrMagnitude > parentBody.Orbit.GravityInfluenceRadiusSquared * 0.9)
            Vector3D.ClampMagnitude(position, parentBody.Orbit.GravityInfluenceRadiusSquared * 0.9);
          if (localRotation.HasValue)
          {
            forward = localRotation.Value * Vector3D.Forward;
            up = localRotation.Value * Vector3D.Up;
          }
        }
      }
      if (parentBody == null && spawnRuleOrbit != null)
      {
        parentBody = this.GetCelestialBody((long) spawnRuleOrbit.CelestialBody);
        OrbitParameters randomOrbit = spawnRuleOrbit.GenerateRandomOrbit(parentBody);
        position = randomOrbit.RelativePosition;
        velocity = randomOrbit.RelativeVelocity;
        if (localRotation.HasValue)
        {
          forward = localRotation.Value * Vector3D.Forward;
          up = localRotation.Value * Vector3D.Up;
        }
      }
      if (parentBody == null)
      {
        if (celestialBodyGUIDs != null && celestialBodyGUIDs.Count > 0)
          parentBody = celestialBodyGUIDs.Count != 1 ? Server.Instance.SolarSystem.GetCelestialBody(celestialBodyGUIDs[MathHelper.RandomRange(0, celestialBodyGUIDs.Count)]) : Server.Instance.SolarSystem.GetCelestialBody(celestialBodyGUIDs[0]);
        if (parentBody == null)
          parentBody = Server.Instance.SolarSystem.GetCelestialBody((long) MathHelper.RandomRange(1, 20));
        if (positionOffset.HasValue)
        {
          position = positionOffset.Value + positionOffset.Value.Normalized * parentBody.Orbit.Radius;
          if (parentBody.GUID == 1L && position.SqrMagnitude > 897587224200.0)
            Vector3D.ClampMagnitude(position, parentBody.Orbit.GravityInfluenceRadiusSquared * 0.9);
          else if (parentBody.GUID != 1L && position.SqrMagnitude > parentBody.Orbit.GravityInfluenceRadiusSquared * 0.9)
            Vector3D.ClampMagnitude(position, parentBody.Orbit.GravityInfluenceRadiusSquared * 0.9);
          if (!velocityAtPosition.HasValue)
          {
            Vector3D vector3D1 = Vector3D.Cross(position.Normalized, Vector3D.Forward);
            Vector3D vector3D2 = Vector3D.Cross(position.Normalized, Vector3D.Up);
            velocityAtPosition = vector3D1.SqrMagnitude <= vector3D2.SqrMagnitude ? new Vector3D?(vector3D2.Normalized * parentBody.Orbit.RandomOrbitVelocityMagnitudeAtDistance(position.Magnitude)) : new Vector3D?(vector3D1.Normalized * parentBody.Orbit.RandomOrbitVelocityMagnitudeAtDistance(position.Magnitude));
          }
          velocity = velocityAtPosition.Value;
        }
        else
        {
          double distance = parentBody.GUID != 1L ? parentBody.Orbit.Radius + (parentBody.Orbit.GravityInfluenceRadius - parentBody.Orbit.Radius) * MathHelper.RandomRange(distanceFromSurfacePercMin, distanceFromSurfacePercMax) : parentBody.Orbit.Radius + (483940704314.0 - parentBody.Orbit.Radius) * MathHelper.RandomRange(0.1, 1.0);
          position = new Vector3D(-distance, 0.0, 0.0);
          velocity = Vector3D.Back * parentBody.Orbit.RandomOrbitVelocityMagnitudeAtDistance(distance);
          QuaternionD quaternionD = MathHelper.RandomRotation();
          position = quaternionD * position;
          velocity = quaternionD * velocity;
        }
        if (localRotation.HasValue)
        {
          forward = localRotation.Value * Vector3D.Forward;
          up = localRotation.Value * Vector3D.Up;
        }
        else
        {
          QuaternionD quaternionD = MathHelper.RandomRotation();
          forward = quaternionD * Vector3D.Forward;
          up = quaternionD * Vector3D.Up;
        }
      }
      double y = -100.0 / position.Magnitude * (180.0 / System.Math.PI);
      position = position + parentBody.Position;
      int num1 = 0;
      if (checkPosition)
      {
        int num2;
        do
        {
          num2 = 0;
          foreach (CelestialBody celesitalBody in this.celesitalBodies)
          {
            if (celesitalBody.Orbit.IsOrbitValid && celesitalBody.GUID != 1L && celesitalBody.Position.DistanceSquared(position) < System.Math.Pow(celesitalBody.Orbit.Radius + Server.CelestialBodyDeathDistance * celestialBodyDeathDistanceMultiplier + objectRadius, 2.0))
            {
              num2 = 2;
              break;
            }
          }
          if (num2 == 0)
          {
            foreach (SpaceObjectVessel allVessel in Server.Instance.AllVessels)
            {
              if (!allVessel.IsDocked && allVessel.Position.DistanceSquared(position) < System.Math.Pow(allVessel.Radius + objectRadius + artificialBodyDistanceCheck, 2.0))
              {
                num2 = 1;
                break;
              }
            }
          }
          if ((uint) num2 > 0U)
          {
            if (spawnRuleOrbit != null && num1 < 20)
            {
              OrbitParameters randomOrbit = spawnRuleOrbit.GenerateRandomOrbit(parentBody);
              position = randomOrbit.Position;
              velocity = randomOrbit.RelativeVelocity;
            }
            if (artificialBody != null && num2 == 1 && num1 < 80)
            {
              position = MathHelper.RotateAroundPivot(position, parentBody.Position, new Vector3D(0.0, y, 0.0));
              velocity = MathHelper.RotateAroundPivot(velocity, Vector3D.Zero, new Vector3D(0.0, y, 0.0));
            }
            else
            {
              Vector3D angles = new Vector3D(MathHelper.RandomRange(0.0, 359.99), MathHelper.RandomRange(0.0, 359.99), MathHelper.RandomRange(0.0, 359.99));
              position = MathHelper.RotateAroundPivot(position, parentBody.Position, angles);
              velocity = MathHelper.RotateAroundPivot(velocity, Vector3D.Zero, angles);
            }
          }
          ++num1;
        }
        while (num2 != 0 && num1 < 100);
      }
      velocity = velocity + parentBody.Velocity;
    }
  }
}
