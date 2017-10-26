// Decompiled with JetBrains decompiler
// Type: ZeroGravity.BulletPhysics.BulletPhysicsController
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using BulletSharp;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Objects;

namespace ZeroGravity.BulletPhysics
{
  public class BulletPhysicsController
  {
    public double Restitution = 0.85;
    public double Friction = 0.7;
    private BroadphaseInterface broadphase;
    private DefaultCollisionConfiguration collisionConfiguration;
    private CollisionDispatcher dispatcher;
    public DiscreteDynamicsWorld dynamicsWorld;

    public BulletPhysicsController()
    {
      this.collisionConfiguration = new DefaultCollisionConfiguration();
      this.dispatcher = new CollisionDispatcher((CollisionConfiguration) this.collisionConfiguration);
      this.broadphase = (BroadphaseInterface) new DbvtBroadphase();
      this.dynamicsWorld = new DiscreteDynamicsWorld((Dispatcher) this.dispatcher, this.broadphase, (ConstraintSolver) null, (CollisionConfiguration) this.collisionConfiguration);
      this.dynamicsWorld.Gravity = Vector3.Zero;
      this.dynamicsWorld.SetInternalTickCallback(new DynamicsWorld.InternalTickCallback(this.InternalTickCallback));
      GImpactCollisionAlgorithm.RegisterAlgorithm(this.dispatcher);
    }

    public void CreateAndAddRigidBody(SpaceObjectVessel vessel)
    {
      CompoundShape compoundShape1 = new CompoundShape();
      CompoundShape compoundShape2 = new CompoundShape();
      foreach (VesselPrimitiveColliderData primitiveColliderData in vessel.PrimitiveCollidersData)
      {
        CollisionShape shape = (CollisionShape) null;
        Matrix localTransform = BulletHelper.AffineTransformation(1f, Quaternion.Identity, primitiveColliderData.CenterPosition.ToVector3());
        if (primitiveColliderData.Type == ColliderDataType.Box)
          shape = (CollisionShape) new BoxShape(primitiveColliderData.Bounds.X / 2.0, primitiveColliderData.Bounds.Y / 2.0, primitiveColliderData.Bounds.Z / 2.0);
        else if (primitiveColliderData.Type == ColliderDataType.Sphere)
          shape = (CollisionShape) new SphereShape(primitiveColliderData.Bounds.X);
        if (shape != null)
        {
          shape.UserObject = (object) primitiveColliderData;
          compoundShape2.AddChildShape(localTransform, shape);
        }
      }
      foreach (VesselMeshColliderData meshColliderData in vessel.MeshCollidersData)
      {
        Matrix localTransform = BulletHelper.AffineTransformation(1f, meshColliderData.Rotation.ToQuaternion(), meshColliderData.CenterPosition.ToVector3());
        GImpactMeshShape gimpactMeshShape = new GImpactMeshShape((StridingMeshInterface) new TriangleIndexVertexArray((ICollection<int>) meshColliderData.Indices, (ICollection<Vector3>) meshColliderData.Vertices));
        gimpactMeshShape.LocalScaling = meshColliderData.Scale.ToVector3();
        gimpactMeshShape.UpdateBound();
        if (gimpactMeshShape != null)
        {
          gimpactMeshShape.UserObject = (object) meshColliderData;
          compoundShape2.AddChildShape(localTransform, (CollisionShape) gimpactMeshShape);
        }
      }
      compoundShape2.UserObject = (object) vessel;
      Matrix localTransform1 = BulletHelper.AffineTransformation(1f, Quaternion.Identity, -vessel.VesselData.CollidersCenterOffset.ToVector3D().ToVector3());
      compoundShape1.AddChildShape(localTransform1, (CollisionShape) compoundShape2);
      Matrix startTrans = BulletHelper.AffineTransformation(1f, BulletHelper.LookRotation(vessel.Forward.ToVector3(), vessel.Up.ToVector3()), vessel.Position.ToVector3());
      double num = 0.0;
      if (vessel.AllDockedVessels.Count > 0)
      {
        foreach (Ship allDockedVessel in vessel.AllDockedVessels)
        {
          num += allDockedVessel.Mass;
          CompoundShape compoundShape3 = new CompoundShape();
          Matrix localTransform2 = BulletHelper.AffineTransformation(1f, allDockedVessel.RelativeRotationFromMainParent.ToQuaternion(), allDockedVessel.RelativePositionFromMainParent.ToVector3() - vessel.VesselData.CollidersCenterOffset.ToVector3D().ToVector3());
          foreach (VesselPrimitiveColliderData primitiveColliderData in allDockedVessel.PrimitiveCollidersData)
          {
            CollisionShape shape = (CollisionShape) null;
            Matrix localTransform3 = BulletHelper.AffineTransformation(1f, Quaternion.Identity, primitiveColliderData.CenterPosition.ToVector3());
            if (primitiveColliderData.Type == ColliderDataType.Box)
              shape = (CollisionShape) new BoxShape(primitiveColliderData.Bounds.X / 2.0, primitiveColliderData.Bounds.Y / 2.0, primitiveColliderData.Bounds.Z / 2.0);
            else if (primitiveColliderData.Type == ColliderDataType.Sphere)
              shape = (CollisionShape) new SphereShape(primitiveColliderData.Bounds.X);
            if (shape != null)
            {
              shape.UserObject = (object) primitiveColliderData;
              compoundShape3.AddChildShape(localTransform3, shape);
            }
          }
          foreach (VesselMeshColliderData meshColliderData in allDockedVessel.MeshCollidersData)
          {
            Matrix localTransform3 = BulletHelper.AffineTransformation(1f, meshColliderData.Rotation.ToQuaternion(), meshColliderData.CenterPosition.ToVector3());
            GImpactMeshShape gimpactMeshShape = new GImpactMeshShape((StridingMeshInterface) new TriangleIndexVertexArray((ICollection<int>) meshColliderData.Indices, (ICollection<Vector3>) meshColliderData.Vertices));
            gimpactMeshShape.LocalScaling = meshColliderData.Scale.ToVector3();
            gimpactMeshShape.UpdateBound();
            if (gimpactMeshShape != null)
            {
              gimpactMeshShape.UserObject = (object) meshColliderData;
              compoundShape3.AddChildShape(localTransform3, (CollisionShape) gimpactMeshShape);
            }
          }
          compoundShape3.UserObject = (object) allDockedVessel;
          compoundShape1.AddChildShape(localTransform2, (CollisionShape) compoundShape3);
        }
      }
      DefaultMotionState defaultMotionState = new DefaultMotionState(startTrans);
      Vector3 localInertia = compoundShape1.CalculateLocalInertia(vessel.Mass + num);
      RigidBody body = new RigidBody(new RigidBodyConstructionInfo(vessel.Mass + num, (MotionState) defaultMotionState, (CollisionShape) compoundShape1)
      {
        LocalInertia = localInertia
      });
      body.SetDamping(0.0, 0.0);
      body.SetSleepingThresholds(0.1, 0.1);
      body.Restitution = this.Restitution;
      body.Friction = this.Friction;
      body.ForceActivationState(ActivationState.DisableDeactivation);
      body.UserObject = (object) vessel;
      this.dynamicsWorld.AddRigidBody(body);
      Vector3 center;
      double radius;
      compoundShape1.GetBoundingSphere(out center, out radius);
      vessel.SetRadius(radius);
      vessel.RigidBody = body;
    }

    public static void ComplexBoundCalculation(SpaceObjectVessel vessel, out Vector3 minValue, out Vector3 maxValue)
    {
      CompoundShape compoundShape1 = new CompoundShape();
      foreach (VesselPrimitiveColliderData primitiveColliderData in vessel.PrimitiveCollidersData)
      {
        if (primitiveColliderData.AffectingCenterOfMass)
        {
          CollisionShape shape = (CollisionShape) null;
          Matrix localTransform = BulletHelper.AffineTransformation(1f, Quaternion.Identity, primitiveColliderData.CenterPosition.ToVector3());
          if (primitiveColliderData.Type == ColliderDataType.Box)
            shape = (CollisionShape) new BoxShape(primitiveColliderData.Bounds.X / 2.0, primitiveColliderData.Bounds.Y / 2.0, primitiveColliderData.Bounds.Z / 2.0);
          else if (primitiveColliderData.Type == ColliderDataType.Sphere)
            shape = (CollisionShape) new SphereShape(primitiveColliderData.Bounds.X);
          if (shape != null)
            compoundShape1.AddChildShape(localTransform, shape);
        }
      }
      foreach (VesselMeshColliderData meshColliderData in vessel.MeshCollidersData)
      {
        if (meshColliderData.AffectingCenterOfMass)
        {
          Matrix localTransform = BulletHelper.AffineTransformation(1f, meshColliderData.Rotation.ToQuaternion(), meshColliderData.CenterPosition.ToVector3());
          GImpactMeshShape gimpactMeshShape = new GImpactMeshShape((StridingMeshInterface) new TriangleIndexVertexArray((ICollection<int>) meshColliderData.Indices, (ICollection<Vector3>) meshColliderData.Vertices));
          gimpactMeshShape.LocalScaling = meshColliderData.Scale.ToVector3();
          gimpactMeshShape.UpdateBound();
          if (gimpactMeshShape != null)
            compoundShape1.AddChildShape(localTransform, (CollisionShape) gimpactMeshShape);
        }
      }
      if (vessel.AllDockedVessels.Count > 0)
      {
        foreach (Ship allDockedVessel in vessel.AllDockedVessels)
        {
          CompoundShape compoundShape2 = new CompoundShape();
          Matrix localTransform1 = BulletHelper.AffineTransformation(1f, allDockedVessel.RelativeRotationFromMainParent.ToQuaternion(), allDockedVessel.RelativePositionFromMainParent.ToVector3());
          foreach (VesselPrimitiveColliderData primitiveColliderData in allDockedVessel.PrimitiveCollidersData)
          {
            CollisionShape shape = (CollisionShape) null;
            Matrix localTransform2 = BulletHelper.AffineTransformation(1f, Quaternion.Identity, primitiveColliderData.CenterPosition.ToVector3());
            if (primitiveColliderData.Type == ColliderDataType.Box)
              shape = (CollisionShape) new BoxShape(primitiveColliderData.Bounds.X / 2.0, primitiveColliderData.Bounds.Y / 2.0, primitiveColliderData.Bounds.Z / 2.0);
            else if (primitiveColliderData.Type == ColliderDataType.Sphere)
              shape = (CollisionShape) new SphereShape(primitiveColliderData.Bounds.X);
            if (shape != null)
              compoundShape2.AddChildShape(localTransform2, shape);
          }
          foreach (VesselMeshColliderData meshColliderData in allDockedVessel.MeshCollidersData)
          {
            Matrix localTransform2 = BulletHelper.AffineTransformation(1f, meshColliderData.Rotation.ToQuaternion(), meshColliderData.CenterPosition.ToVector3());
            GImpactMeshShape gimpactMeshShape = new GImpactMeshShape((StridingMeshInterface) new TriangleIndexVertexArray((ICollection<int>) meshColliderData.Indices, (ICollection<Vector3>) meshColliderData.Vertices));
            gimpactMeshShape.LocalScaling = meshColliderData.Scale.ToVector3();
            gimpactMeshShape.UpdateBound();
            if (gimpactMeshShape != null)
              compoundShape2.AddChildShape(localTransform2, (CollisionShape) gimpactMeshShape);
          }
          compoundShape1.AddChildShape(localTransform1, (CollisionShape) compoundShape2);
        }
      }
      Matrix t = BulletHelper.AffineTransformation(1f, Quaternion.Identity, Vector3.Zero);
      compoundShape1.GetAabb(t, out minValue, out maxValue);
    }

    public bool RayCast(Vector3 from, Vector3 to, out ClosestRayResultCallback result)
    {
      result = new ClosestRayResultCallback(ref from, ref to);
      this.dynamicsWorld.RayTest(from, to, (RayResultCallback) result);
      return result.HasHit;
    }

    public bool RayCastAll(Vector3 from, Vector3 to, out AllHitsRayResultCallback result)
    {
      result = new AllHitsRayResultCallback(from, to);
      this.dynamicsWorld.RayTest(from, to, (RayResultCallback) result);
      return result.HasHit;
    }

    private void InternalTickCallback(DynamicsWorld world, double timeStep)
    {
      foreach (SpaceObjectVessel allVessel in Server.Instance.AllVessels)
        allVessel.ReadPhysicsParameters();
      int numManifolds = world.Dispatcher.NumManifolds;
      for (int index1 = 0; index1 < numManifolds; ++index1)
      {
        PersistentManifold manifoldByIndexInternal = world.Dispatcher.GetManifoldByIndexInternal(index1);
        RigidBody body0 = manifoldByIndexInternal.Body0 as RigidBody;
        RigidBody body1 = manifoldByIndexInternal.Body1 as RigidBody;
        Vector3 vector3 = body0.LinearVelocity - body1.LinearVelocity;
        List<long> longList = new List<long>();
        if (body0.UserObject is Ship)
          longList.Add((body0.UserObject as Ship).GUID);
        if (body1.UserObject is Ship)
          longList.Add((body1.UserObject as Ship).GUID);
        int numContacts = manifoldByIndexInternal.NumContacts;
        for (int index2 = 0; index2 < numContacts; ++index2)
        {
          ManifoldPoint contactPoint = manifoldByIndexInternal.GetContactPoint(index2);
          if (contactPoint.AppliedImpulse.IsNotEpsilonZeroD(double.Epsilon) && longList.Count > 0)
            (Server.Instance.GetVessel(longList[0]) as Ship).SendCollision(vector3.Length, contactPoint.AppliedImpulse, (double) contactPoint.LifeTime, longList.Count > 1 ? longList[1] : -1L);
        }
        manifoldByIndexInternal.ClearManifold();
      }
    }

    public void Update(double deltaTime)
    {
      this.dynamicsWorld.StepSimulation(deltaTime);
    }

    public bool RemoveRigidBody(SpaceObjectVessel ship)
    {
      if (ship.RigidBody != null && this.dynamicsWorld.CollisionObjectArray.Contains((CollisionObject) ship.RigidBody))
      {
        this.dynamicsWorld.RemoveRigidBody(ship.RigidBody);
        ship.RigidBody = (RigidBody) null;
        return true;
      }
      if (ship.IsDocked)
        return this.RemoveRigidBody((SpaceObjectVessel) (ship.DockedToMainVessel as Ship));
      return true;
    }
  }
}
