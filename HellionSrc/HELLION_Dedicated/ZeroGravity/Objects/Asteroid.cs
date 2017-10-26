// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Asteroid
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using ZeroGravity.BulletPhysics;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.ShipComponents;
using ZeroGravity.Spawn;

namespace ZeroGravity.Objects
{
  public class Asteroid : SpaceObjectVessel, IPersistantObject
  {
    public static List<ResourceType> MiningResourceTypes = new List<ResourceType>()
    {
      ResourceType.Ice,
      ResourceType.HeavyIce,
      ResourceType.DryIce,
      ResourceType.NitrateMinerals
    };
    public int ColliderIndex = 1;
    private Dictionary<short[], Asteroid.ResourceSection> resources = new Dictionary<short[], Asteroid.ResourceSection>((IEqualityComparer<short[]>) new DistributionManager.ShortArrayComparer());
    public int BoundingRes;
    private float[] BoundingBoxSize;

    public override SpaceObjectType ObjectType
    {
      get
      {
        return SpaceObjectType.Asteroid;
      }
    }

    public Asteroid(long guid, bool initializeOrbit, Vector3D position, Vector3D velocity, Vector3D forward, Vector3D up)
      : base(guid, initializeOrbit, position, velocity, forward, up)
    {
      this.Mass = 100000000000.0;
    }

    public static Asteroid CreateNewAsteroid(GameScenes.SceneID sceneID, string registration = "", long asteroidGUID = -1, List<long> nearArtificialBodyGUIDs = null, List<long> celestialBodyGUIDs = null, Vector3D? positionOffset = null, Vector3D? velocityAtPosition = null, QuaternionD? localRotation = null, bool checkPosition = true, double distanceFromSurfacePercMin = 0.03, double distanceFromSurfacePercMax = 0.3, SpawnRuleOrbit spawnRuleOrbit = null, double celestialBodyDeathDistanceMultiplier = 1.5, double artificialBodyDistanceCheck = 100.0)
    {
      Vector3D position = Vector3D.Zero;
      Vector3D velocity = Vector3D.Zero;
      Vector3D forward = Vector3D.Forward;
      Vector3D up = Vector3D.Up;
      Asteroid asteroid = new Asteroid(asteroidGUID < 0L ? GUIDFactory.NextVesselGUID() : asteroidGUID, false, position, velocity, forward, up);
      asteroid.VesselData = new VesselData();
      asteroid.VesselData.SceneID = sceneID;
      asteroid.VesselData.VesselRegistration = registration;
      asteroid.VesselData.VesselName = "";
      asteroid.VesselData.CollidersCenterOffset = Vector3D.Zero.ToFloatArray();
      asteroid.ReadInfoFromJson();
      asteroid.SetResourcesInSections(new float[3]
      {
        100f,
        100f,
        100f
      });
      Server.Instance.PhysicsController.CreateAndAddRigidBody((SpaceObjectVessel) asteroid);
      Server.Instance.SolarSystem.GetSpawnPosition(SpaceObjectType.Asteroid, asteroid.Radius, checkPosition, out position, out velocity, out forward, out up, nearArtificialBodyGUIDs, celestialBodyGUIDs, positionOffset, velocityAtPosition, localRotation, distanceFromSurfacePercMin, distanceFromSurfacePercMax, spawnRuleOrbit, celestialBodyDeathDistanceMultiplier, artificialBodyDistanceCheck);
      asteroid.InitializeOrbit(position, velocity, forward, up);
      if (registration.IsNullOrEmpty())
        asteroid.VesselData.VesselRegistration = Server.NameGenerator.GenerateObjectRegistration(SpaceObjectType.Asteroid, asteroid.Orbit.Parent.CelestialBody, sceneID);
      Server.Instance.Add((SpaceObjectVessel) asteroid);
      asteroid.SetPhysicsParameters();
      return asteroid;
    }

    public Dictionary<ResourceType, float> GetResourcesOnPosition(Vector3D position, int lvl)
    {
      return this.resources[this.GetSectionFromPosition(position)].lvls[lvl];
    }

    public Dictionary<ResourceType, float> GetAllResources()
    {
      Dictionary<ResourceType, float> dictionary1 = new Dictionary<ResourceType, float>();
      foreach (KeyValuePair<short[], Asteroid.ResourceSection> resource in this.resources)
      {
        foreach (Dictionary<ResourceType, float> lvl in resource.Value.lvls)
        {
          foreach (KeyValuePair<ResourceType, float> keyValuePair in lvl)
          {
            if (dictionary1.ContainsKey(keyValuePair.Key))
            {
              Dictionary<ResourceType, float> dictionary2 = dictionary1;
              ResourceType key = keyValuePair.Key;
              dictionary2[key] = dictionary2[key] + keyValuePair.Value;
            }
            else
              dictionary1.Add(keyValuePair.Key, keyValuePair.Value);
          }
        }
      }
      return dictionary1;
    }

    public Dictionary<ResourceType, float> GetDrillingResources(Vector3D position, int lvl, float drillingStrenght, out short[] index, out Dictionary<ResourceType, float> dbgres)
    {
      short[] sectionFromPosition = this.GetSectionFromPosition(position);
      index = sectionFromPosition;
      dbgres = new Dictionary<ResourceType, float>();
      if (!this.resources.ContainsKey(sectionFromPosition))
        return (Dictionary<ResourceType, float>) null;
      Dictionary<ResourceType, float> dictionary = new Dictionary<ResourceType, float>();
      foreach (KeyValuePair<ResourceType, float> keyValuePair in this.resources[sectionFromPosition].lvls[lvl])
      {
        if ((double) this.resources[sectionFromPosition].lvls[lvl][keyValuePair.Key] - (double) drillingStrenght < 0.0)
          drillingStrenght = this.resources[sectionFromPosition].lvls[lvl][keyValuePair.Key];
        dictionary.Add(keyValuePair.Key, drillingStrenght);
      }
      foreach (KeyValuePair<ResourceType, float> keyValuePair in dictionary)
      {
        Dictionary<ResourceType, float> lvl1 = this.resources[sectionFromPosition].lvls[lvl];
        ResourceType key1 = keyValuePair.Key;
        lvl1[key1] = lvl1[key1] - keyValuePair.Value;
        if ((double) this.resources[sectionFromPosition].lvls[lvl][keyValuePair.Key] <= 0.0)
        {
          this.resources[sectionFromPosition].lvls[lvl].Remove(keyValuePair.Key);
        }
        else
        {
          Dictionary<ResourceType, float> lvl2 = this.resources[sectionFromPosition].lvls[lvl];
          ResourceType key2 = keyValuePair.Key;
          lvl2[key2] = lvl2[key2] - keyValuePair.Value;
        }
      }
      dbgres = this.resources[sectionFromPosition].lvls[lvl];
      return dictionary;
    }

    public short[] GetSectionFromPosition(Vector3D position)
    {
      short[] numArray = new short[3];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = (short) ((position[index] + (double) this.BoundingBoxSize[index] / 2.0) / ((double) this.BoundingBoxSize[index] / (double) this.BoundingRes));
      return numArray;
    }

    private void FillLevels(short[] index, ref float[] zbirRes)
    {
      Random random = new Random();
      if (!this.resources.ContainsKey(index))
        this.resources.Add(index, new Asteroid.ResourceSection());
      Asteroid.ResourceSection resource = this.resources[index];
      for (int index1 = 0; index1 < 3; ++index1)
      {
        int index2 = random.Next(0, Asteroid.MiningResourceTypes.Count);
        float num = (float) random.NextDouble();
        if (resource.lvls[index1].ContainsKey(Asteroid.MiningResourceTypes[index2]))
        {
          Dictionary<ResourceType, float> lvl = resource.lvls[index1];
          ResourceType miningResourceType = Asteroid.MiningResourceTypes[index2];
          lvl[miningResourceType] = lvl[miningResourceType] + num;
        }
        else
          resource.lvls[index1].Add(Asteroid.MiningResourceTypes[index2], num);
        zbirRes[index2] += resource.lvls[index1][Asteroid.MiningResourceTypes[index2]];
      }
    }

    public void SetResourcesInSections(float[] resourceAmountToSet)
    {
      Random random = new Random();
      for (short index1 = 0; (int) index1 < this.BoundingRes; ++index1)
      {
        for (short index2 = 0; (int) index2 < this.BoundingRes; ++index2)
        {
          for (short index3 = 0; (int) index3 < this.BoundingRes; ++index3)
          {
            short[] key = new short[3]
            {
              index1,
              index2,
              index3
            };
            Asteroid.ResourceSection resourceSection = new Asteroid.ResourceSection();
            switch (random.Next(0, 5))
            {
              case 0:
                resourceSection.lvls[0].Add(ResourceType.Ice, (float) random.NextDouble() * 1000000f);
                break;
              case 1:
                resourceSection.lvls[0].Add(ResourceType.HeavyIce, (float) random.NextDouble() * 1000000f);
                break;
              case 2:
                resourceSection.lvls[0].Add(ResourceType.DryIce, (float) random.NextDouble() * 1000000f);
                break;
              default:
                resourceSection.lvls[0].Add(ResourceType.NitrateMinerals, (float) random.NextDouble() * 1000000f);
                break;
            }
            this.resources.Add(key, resourceSection);
          }
        }
      }
    }

    public override void AddPlayerToCrew(Player pl)
    {
      if (this.VesselCrew.Contains(pl))
        return;
      this.VesselCrew.Add(pl);
    }

    public override void RemovePlayerFromCrew(Player pl, bool checkDetails = false)
    {
      this.VesselCrew.Remove(pl);
    }

    public override bool HasPlayerInCrew(Player pl)
    {
      return this.VesselCrew.Contains(pl);
    }

    public void ReadInfoFromJson()
    {
      AsteroidSceneData asteroidSceneData = StaticData.AsteroidDataList.Find((Predicate<AsteroidSceneData>) (x => (int) x.ItemID == (int) (short) this.SceneID));
      this.BoundingBoxSize = asteroidSceneData.BoundingBoxSize;
      this.BoundingRes = asteroidSceneData.BoundingRes;
      this.Radius = (double) asteroidSceneData.Radius;
      this.RadarSignature = asteroidSceneData.RadarSignature;
      if (asteroidSceneData.Colliders == null)
        return;
      if (asteroidSceneData.Colliders.PrimitiveCollidersData != null && asteroidSceneData.Colliders.PrimitiveCollidersData.Count > 0)
      {
        foreach (PrimitiveColliderData primitiveColliderData1 in asteroidSceneData.Colliders.PrimitiveCollidersData)
        {
          if (primitiveColliderData1.Type == ColliderDataType.Box)
          {
            List<VesselPrimitiveColliderData> primitiveCollidersData = this.PrimitiveCollidersData;
            VesselPrimitiveColliderData primitiveColliderData2 = new VesselPrimitiveColliderData();
            primitiveColliderData2.Type = primitiveColliderData1.Type;
            Vector3D vector3D1 = primitiveColliderData1.Center.ToVector3D();
            primitiveColliderData2.CenterPosition = vector3D1;
            Vector3D vector3D2 = primitiveColliderData1.Size.ToVector3D();
            primitiveColliderData2.Bounds = vector3D2;
            int num = primitiveColliderData1.AffectingCenterOfMass ? 1 : 0;
            primitiveColliderData2.AffectingCenterOfMass = num != 0;
            primitiveCollidersData.Add(primitiveColliderData2);
          }
          else if (primitiveColliderData1.Type == ColliderDataType.Sphere)
          {
            List<VesselPrimitiveColliderData> primitiveCollidersData = this.PrimitiveCollidersData;
            VesselPrimitiveColliderData primitiveColliderData2 = new VesselPrimitiveColliderData();
            primitiveColliderData2.Type = primitiveColliderData1.Type;
            Vector3D vector3D1 = primitiveColliderData1.Center.ToVector3D();
            primitiveColliderData2.CenterPosition = vector3D1;
            Vector3D vector3D2 = primitiveColliderData1.Size.ToVector3D();
            primitiveColliderData2.Bounds = vector3D2;
            int num = primitiveColliderData1.AffectingCenterOfMass ? 1 : 0;
            primitiveColliderData2.AffectingCenterOfMass = num != 0;
            primitiveCollidersData.Add(primitiveColliderData2);
          }
        }
      }
      if (asteroidSceneData.Colliders.MeshCollidersData != null)
      {
        foreach (MeshData meshData in asteroidSceneData.Colliders.MeshCollidersData)
        {
          List<VesselMeshColliderData> meshCollidersData = this.MeshCollidersData;
          VesselMeshColliderData meshColliderData = new VesselMeshColliderData();
          int num1 = meshData.AffectingCenterOfMass ? 1 : 0;
          meshColliderData.AffectingCenterOfMass = num1 != 0;
          int colliderIndex = this.ColliderIndex;
          this.ColliderIndex = colliderIndex + 1;
          long num2 = (long) colliderIndex;
          meshColliderData.ColliderID = num2;
          int[] indices = meshData.Indices;
          meshColliderData.Indices = indices;
          Vector3[] vertices = meshData.Vertices.GetVertices();
          meshColliderData.Vertices = vertices;
          Vector3D vector3D1 = meshData.CenterPosition.ToVector3D();
          meshColliderData.CenterPosition = vector3D1;
          Vector3D vector3D2 = meshData.Bounds.ToVector3D();
          meshColliderData.Bounds = vector3D2;
          QuaternionD quaternionD = meshData.Rotation.ToQuaternionD();
          meshColliderData.Rotation = quaternionD;
          Vector3D vector3D3 = meshData.Scale.ToVector3D();
          meshColliderData.Scale = vector3D3;
          meshCollidersData.Add(meshColliderData);
        }
      }
    }

    public override void Destroy()
    {
      Server.Instance.Remove((SpaceObjectVessel) this);
      this.DisconectListener();
      base.Destroy();
    }

    private void DisconectListener()
    {
    }

    public override SpawnObjectResponseData GetSpawnResponseData(Player pl)
    {
      bool flag = (pl.Position - this.Position).SqrMagnitude > 100000000.0;
      SpawnAsteroidResponseData asteroidResponseData = new SpawnAsteroidResponseData();
      long guid = this.GUID;
      asteroidResponseData.GUID = guid;
      int sceneId = (int) (short) this.SceneID;
      asteroidResponseData.ItemID = (short) sceneId;
      string vesselRegistration = this.VesselData.VesselRegistration;
      asteroidResponseData.Name = vesselRegistration;
      double radius = this.Radius;
      asteroidResponseData.Radius = radius;
      int num = flag ? 1 : 0;
      asteroidResponseData.IsDummy = num != 0;
      double radarSignature = this.RadarSignature;
      asteroidResponseData.RadarSignature = radarSignature;
      List<ResourceType> resourceTypeList = flag ? this.GetResourceTypes(0) : (List<ResourceType>) null;
      asteroidResponseData.Level1ResourceTypes = resourceTypeList;
      return (SpawnObjectResponseData) asteroidResponseData;
    }

    private List<ResourceType> GetResourceTypes(int levelIndex)
    {
      List<ResourceType> resourceTypeList = new List<ResourceType>();
      foreach (KeyValuePair<short[], Asteroid.ResourceSection> resource in this.resources)
        resourceTypeList.Add(resource.Value.lvls[levelIndex].Keys.First<ResourceType>());
      return resourceTypeList;
    }

    public override InitializeSpaceObjectMessage GetInitializeMessage()
    {
      InitializeSpaceObjectMessage spaceObjectMessage = new InitializeSpaceObjectMessage();
      spaceObjectMessage.GUID = this.GUID;
      spaceObjectMessage.DynamicObjects = new List<DynamicObjectDetails>();
      foreach (DynamicObject dynamicObject in this.DynamicObjects)
        spaceObjectMessage.DynamicObjects.Add(dynamicObject.GetDetails());
      spaceObjectMessage.Corpses = new List<CorpseDetails>();
      foreach (Corpse corpse in this.Corpses)
      {
        List<CorpseDetails> corpses = spaceObjectMessage.Corpses;
        CorpseDetails corpseDetails = new CorpseDetails();
        corpseDetails.GUID = corpse.GUID;
        corpseDetails.ParentGUID = corpse.Parent == null ? -1L : corpse.Parent.GUID;
        corpseDetails.ParentType = (SpaceObjectType) (corpse.Parent == null ? 0 : (int) corpse.Parent.ObjectType);
        corpseDetails.LocalPosition = corpse.LocalPosition.ToFloatArray();
        corpseDetails.LocalRotation = corpse.LocalRotation.ToFloatArray();
        int num = corpse.IsInsideSpaceObject ? 1 : 0;
        corpseDetails.IsInsideSpaceObject = num != 0;
        Dictionary<byte, RagdollItemData> ragdollDataList = corpse.RagdollDataList;
        corpseDetails.RagdollDataList = ragdollDataList;
        corpses.Add(corpseDetails);
      }
      spaceObjectMessage.Characters = new List<CharacterDetails>();
      foreach (Player player in this.VesselCrew)
        spaceObjectMessage.Characters.Add(player.GetDetails(false));
      return spaceObjectMessage;
    }

    public PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataAsteroid objectDataAsteroid = new PersistenceObjectDataAsteroid();
      objectDataAsteroid.GUID = this.GUID;
      objectDataAsteroid.OrbitData = new OrbitData();
      this.Orbit.FillOrbitData(ref objectDataAsteroid.OrbitData, (SpaceObjectVessel) null);
      objectDataAsteroid.Name = this.VesselData.VesselRegistration;
      objectDataAsteroid.SceneID = this.SceneID;
      objectDataAsteroid.Forward = this.Forward.ToArray();
      objectDataAsteroid.Up = this.Up.ToArray();
      return (PersistenceObjectData) objectDataAsteroid;
    }

    public void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        PersistenceObjectDataAsteroid objectDataAsteroid = persistenceData as PersistenceObjectDataAsteroid;
        this.VesselData = new VesselData();
        this.VesselData.CollidersCenterOffset = Vector3D.Zero.ToFloatArray();
        this.VesselData.VesselRegistration = objectDataAsteroid.Name;
        this.VesselData.SceneID = objectDataAsteroid.SceneID;
        Server.Instance.PhysicsController.CreateAndAddRigidBody((SpaceObjectVessel) this);
        this.ReadInfoFromJson();
        this.SetResourcesInSections(new float[3]
        {
          100f,
          100f,
          100f
        });
        this.InitializeOrbit(Vector3D.Zero, Vector3D.One, objectDataAsteroid.Forward.ToVector3D(), objectDataAsteroid.Up.ToVector3D());
        if (objectDataAsteroid.OrbitData != null)
          this.Orbit.ParseNetworkData(objectDataAsteroid.OrbitData, true);
        Server.Instance.Add((SpaceObjectVessel) this);
        this.SetPhysicsParameters();
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }

    public class ResourceSection
    {
      public List<Dictionary<ResourceType, float>> lvls = new List<Dictionary<ResourceType, float>>()
      {
        new Dictionary<ResourceType, float>(),
        new Dictionary<ResourceType, float>(),
        new Dictionary<ResourceType, float>()
      };
    }
  }
}
