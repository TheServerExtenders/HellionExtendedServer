// Decompiled with JetBrains decompiler
// Type: ZeroGravity.BulletPhysics.BulletHelper
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using BulletSharp;
using ZeroGravity.Math;

namespace ZeroGravity.BulletPhysics
{
  public static class BulletHelper
  {
    public static Vector3[] GetVertices(this float[] v)
    {
      Vector3[] vector3Array1 = new Vector3[v.Length / 3];
      int num1 = 0;
      int num2 = 0;
      for (; num1 < v.Length / 3; ++num1)
      {
        Vector3[] vector3Array2 = vector3Array1;
        int index1 = num1;
        float[] numArray1 = v;
        int index2 = num2;
        int num3 = 1;
        int num4 = index2 + num3;
        double x = (double) numArray1[index2];
        float[] numArray2 = v;
        int index3 = num4;
        int num5 = 1;
        int num6 = index3 + num5;
        double y = (double) numArray2[index3];
        float[] numArray3 = v;
        int index4 = num6;
        int num7 = 1;
        num2 = index4 + num7;
        double z = (double) numArray3[index4];
        Vector3 vector3 = new Vector3(x, y, z);
        vector3Array2[index1] = vector3;
      }
      return vector3Array1;
    }

    public static void AffineTransformation(float scaling, ref Quaternion rotation, ref Vector3 translation, out BulletSharp.Matrix result)
    {
      result = BulletHelper.Scaling(scaling) * BulletSharp.Matrix.RotationQuaternion(rotation) * BulletSharp.Matrix.Translation(translation);
    }

    public static BulletSharp.Matrix AffineTransformation(float scaling, Quaternion rotation, Vector3 translation)
    {
      BulletSharp.Matrix result;
      BulletHelper.AffineTransformation(scaling, ref rotation, ref translation, out result);
      return result;
    }

    public static BulletSharp.Matrix Scaling(float scale)
    {
      BulletSharp.Matrix result;
      BulletHelper.Scaling(scale, out result);
      return result;
    }

    public static void Scaling(float scale, out BulletSharp.Matrix result)
    {
      result = BulletSharp.Matrix.Identity;
      result.M11 = result.M22 = result.M33 = (double) scale;
    }

    public static Quaternion LookRotation(Vector3 forward, Vector3 up)
    {
      forward.Normalize();
      Vector3 vector3_1 = Vector3.Normalize(forward);
      Vector3 right = Vector3.Normalize(Vector3.Cross(up, vector3_1));
      Vector3 vector3_2 = Vector3.Cross(vector3_1, right);
      double x1 = right.X;
      double y1 = right.Y;
      double z1 = right.Z;
      double x2 = vector3_2.X;
      double y2 = vector3_2.Y;
      double z2 = vector3_2.Z;
      double x3 = vector3_1.X;
      double y3 = vector3_1.Y;
      double z3 = vector3_1.Z;
      double num1 = x1 + y2 + z3;
      Quaternion quaternion = new Quaternion();
      if (num1 > 0.0)
      {
        double num2 = System.Math.Sqrt(num1 + 1.0);
        quaternion.W = num2 * 0.5;
        double num3 = 0.5 / num2;
        quaternion.X = (z2 - y3) * num3;
        quaternion.Y = (x3 - z1) * num3;
        quaternion.Z = (y1 - x2) * num3;
        return quaternion;
      }
      if (x1 >= y2 && x1 >= z3)
      {
        double num2 = System.Math.Sqrt(1.0 + x1 - y2 - z3);
        double num3 = 0.5 / num2;
        quaternion.X = 0.5 * num2;
        quaternion.Y = (y1 + x2) * num3;
        quaternion.Z = (z1 + x3) * num3;
        quaternion.W = (z2 - y3) * num3;
        return quaternion;
      }
      if (y2 > z3)
      {
        double num2 = System.Math.Sqrt(1.0 + y2 - x1 - z3);
        double num3 = 0.5 / num2;
        quaternion.X = (x2 + y1) * num3;
        quaternion.Y = 0.5 * num2;
        quaternion.Z = (y3 + z2) * num3;
        quaternion.W = (x3 - z1) * num3;
        return quaternion;
      }
      double num4 = System.Math.Sqrt(1.0 + z3 - x1 - y2);
      double num5 = 0.5 / num4;
      quaternion.X = (x3 + z1) * num5;
      quaternion.Y = (y3 + z2) * num5;
      quaternion.Z = 0.5 * num4;
      quaternion.W = (y1 - x2) * num5;
      return quaternion;
    }

    public static Vector3 Up(this BulletSharp.Matrix m)
    {
      return new Vector3(m.M21, m.M22, m.M23);
    }

    public static Vector3 Forward(this BulletSharp.Matrix m)
    {
      return new Vector3(m.M31, m.M32, m.M33);
    }

    public static Vector3D ToVector3D(this Vector3 v)
    {
      return new Vector3D(v.X, v.Y, v.Z);
    }

    public static Vector3 ToVector3(this Vector3D v)
    {
      return new Vector3(v.X, v.Y, v.Z);
    }

    public static QuaternionD ToQuaternionD(this Quaternion q)
    {
      return new QuaternionD(q.X, q.Y, q.Z, q.W);
    }

    public static Quaternion ToQuaternion(this QuaternionD q)
    {
      return new Quaternion(q.X, q.Y, q.Z, q.W);
    }

    public static Vector3 GetLocalVector(this BulletSharp.Matrix m, Vector3 globalVector)
    {
      Vector3 vector3 = new Vector3();
      vector3.X = m.M11 * vector3.X + m.M12 * vector3.Y + m.M13 + vector3.Z;
      vector3.Y = m.M21 * vector3.X + m.M22 * vector3.Y + m.M23 + vector3.Z;
      vector3.X = m.M31 * vector3.X + m.M32 * vector3.Y + m.M33 + vector3.Z;
      return vector3;
    }
  }
}
