// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Extensions
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Threading.Tasks;
using ZeroGravity.Math;

namespace ZeroGravity
{
  public static class Extensions
  {
    public static double[] ToArray(this Vector3D v)
    {
      return new double[3]{ v.X, v.Y, v.Z };
    }

    public static double[] ToArray(this QuaternionD q)
    {
      return new double[4]{ q.X, q.Y, q.Z, q.W };
    }

    public static float[] ToFloatArray(this Vector3D v)
    {
      return new float[3]
      {
        (float) v.X,
        (float) v.Y,
        (float) v.Z
      };
    }

    public static float[] ToFloatArray(this QuaternionD q)
    {
      return new float[4]
      {
        (float) q.X,
        (float) q.Y,
        (float) q.Z,
        (float) q.W
      };
    }

    public static Vector3D ToVector3D(this float[] arr)
    {
      if (arr.Length == 3)
        return new Vector3D((double) arr[0], (double) arr[1], (double) arr[2]);
      return Vector3D.Zero;
    }

    public static Vector3D ToVector3D(this double[] arr)
    {
      if (arr.Length == 3)
        return new Vector3D(arr[0], arr[1], arr[2]);
      return Vector3D.Zero;
    }

    public static QuaternionD ToQuaternionD(this float[] arr)
    {
      if (arr.Length == 4)
        return new QuaternionD((double) arr[0], (double) arr[1], (double) arr[2], (double) arr[3]);
      return QuaternionD.Identity;
    }

    public static QuaternionD ToQuaternionD(this double[] arr)
    {
      if (arr.Length == 4)
        return new QuaternionD(arr[0], arr[1], arr[2], arr[3]);
      return QuaternionD.Identity;
    }

    public static bool IsNotEpsilonZero(this float val, float epsilon = 1.401298E-45f)
    {
      return (double) val > (double) epsilon || (double) val < -(double) epsilon;
    }

    public static bool IsNotEpsilonZeroD(this double val, double epsilon = 4.94065645841247E-324)
    {
      return val > epsilon || val < -epsilon;
    }

    public static bool IsNotEpsilonZero(this Vector3D value, double epsilon = 4.94065645841247E-324)
    {
      return System.Math.Abs(value.X) > epsilon || System.Math.Abs(value.Y) > epsilon || System.Math.Abs(value.Z) > epsilon;
    }

    public static bool IsEpsilonZero(this Vector3D value, double epsilon = 4.94065645841247E-324)
    {
      return System.Math.Abs(value.X) <= epsilon && System.Math.Abs(value.Y) <= epsilon && System.Math.Abs(value.Z) <= epsilon;
    }

    public static bool IsInfinity(this Vector3D value)
    {
      return double.IsInfinity(value.X) || double.IsInfinity(value.Y) || double.IsInfinity(value.Z);
    }

    public static bool IsNaN(this Vector3D value)
    {
      return double.IsNaN(value.X) || double.IsNaN(value.Y) || double.IsNaN(value.Z);
    }

    public static bool IsEpsilonEqual(this float val, float other, float epsilon = 1.401298E-45f)
    {
      return (double) System.Math.Abs(val - other) <= (double) epsilon;
    }

    public static bool IsEpsilonEqualD(this double val, double other, double epsilon = 4.94065645841247E-324)
    {
      return System.Math.Abs(val - other) <= epsilon;
    }

    public static bool IsEpsilonEqual(this Vector3D val, Vector3D other, double epsilon = 4.94065645841247E-324)
    {
      return System.Math.Abs(val.X - other.X) <= epsilon && System.Math.Abs(val.Y - other.Y) <= epsilon && System.Math.Abs(val.Z - other.Z) <= epsilon;
    }

    public static bool IsEpsilonEqual(this QuaternionD val, QuaternionD other, double epsilon = 4.94065645841247E-324)
    {
      return System.Math.Abs(val.X - other.X) <= epsilon && System.Math.Abs(val.Y - other.Y) <= epsilon && System.Math.Abs(val.Z - other.Z) <= epsilon && System.Math.Abs(val.W - other.W) <= epsilon;
    }

    public static Vector3D FromOther(this Vector3D vec, Vector3D other)
    {
      vec.X = other.X;
      vec.Y = other.Y;
      vec.Z = other.Z;
      return vec;
    }

    public static bool IsValid(this Vector3D v)
    {
      return !double.IsNaN(v.X) && !double.IsInfinity(v.X) && (!double.IsNaN(v.Y) && !double.IsInfinity(v.Y)) && !double.IsNaN(v.Z) && !double.IsInfinity(v.Z);
    }

    public static Vector3D RotateAroundPivot(this Vector3D vector, Vector3D pivot, Vector3D angles)
    {
      return QuaternionD.Euler(angles) * (vector - pivot) + pivot;
    }

    public static double DistanceSquared(this Vector3D a, Vector3D b)
    {
      Vector3D vector3D = new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
      return vector3D.X * vector3D.X + vector3D.Y * vector3D.Y + vector3D.Z * vector3D.Z;
    }

    public static QuaternionD Inverse(this QuaternionD value)
    {
      return QuaternionD.Inverse(value);
    }

    public static void Invoke(Action continuationFunction, double time)
    {
      Task.Delay(TimeSpan.FromSeconds(time)).ContinueWith((Action<Task>) (_ => Task.Run(continuationFunction)));
    }

    public static bool IsNullOrEmpty(this string val)
    {
      return string.IsNullOrEmpty(val);
    }
  }
}
