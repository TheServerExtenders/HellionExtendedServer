// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Math.Vector3D
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;

namespace ZeroGravity.Math
{
  public struct Vector3D
  {
    private const double epsilon = 1E-06;
    public double X;
    public double Y;
    public double Z;

    public static Vector3D Back
    {
      get
      {
        return new Vector3D(0.0, 0.0, -1.0);
      }
    }

    public static Vector3D Down
    {
      get
      {
        return new Vector3D(0.0, -1.0, 0.0);
      }
    }

    public static Vector3D Forward
    {
      get
      {
        return new Vector3D(0.0, 0.0, 1.0);
      }
    }

    public static Vector3D Left
    {
      get
      {
        return new Vector3D(-1.0, 0.0, 0.0);
      }
    }

    public static Vector3D One
    {
      get
      {
        return new Vector3D(1.0, 1.0, 1.0);
      }
    }

    public static Vector3D Right
    {
      get
      {
        return new Vector3D(1.0, 0.0, 0.0);
      }
    }

    public static Vector3D Up
    {
      get
      {
        return new Vector3D(0.0, 1.0, 0.0);
      }
    }

    public static Vector3D Zero
    {
      get
      {
        return new Vector3D(0.0, 0.0, 0.0);
      }
    }

    public double Magnitude
    {
      get
      {
        return System.Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
      }
    }

    public double SqrMagnitude
    {
      get
      {
        return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
      }
    }

    public Vector3D Normalized
    {
      get
      {
        return Vector3D.Normalize(this);
      }
    }

    public double this[int index]
    {
      get
      {
        switch (index)
        {
          case 0:
            return this.X;
          case 1:
            return this.Y;
          case 2:
            return this.Z;
          default:
            throw new IndexOutOfRangeException("Invalid Vector3 index!");
        }
      }
      set
      {
        switch (index)
        {
          case 0:
            this.X = value;
            break;
          case 1:
            this.Y = value;
            break;
          case 2:
            this.Z = value;
            break;
          default:
            throw new IndexOutOfRangeException("Invalid Vector3 index!");
        }
      }
    }

    public Vector3D(double x, double y, double z)
    {
      this.X = x;
      this.Y = y;
      this.Z = z;
    }

    public Vector3D(double x, double y)
    {
      this.X = x;
      this.Y = y;
      this.Z = 0.0;
    }

    public static double Angle(Vector3D from, Vector3D to)
    {
      return System.Math.Acos(MathHelper.Clamp(Vector3D.Dot(from.Normalized, to.Normalized), -1.0, 1.0)) * (180.0 / System.Math.PI);
    }

    public static Vector3D ClampMagnitude(Vector3D vector, double maxLength)
    {
      if (vector.SqrMagnitude > maxLength * maxLength)
        return vector.Normalized * maxLength;
      return vector;
    }

    public static Vector3D Cross(Vector3D lhs, Vector3D rhs)
    {
      return new Vector3D(lhs.Y * rhs.Z - lhs.Z * rhs.Y, lhs.Z * rhs.X - lhs.X * rhs.Z, lhs.X * rhs.Y - lhs.Y * rhs.X);
    }

    public static double Distance(Vector3D a, Vector3D b)
    {
      Vector3D vector3D = new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
      return System.Math.Sqrt(vector3D.X * vector3D.X + vector3D.Y * vector3D.Y + vector3D.Z * vector3D.Z);
    }

    public static double Dot(Vector3D lhs, Vector3D rhs)
    {
      return lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;
    }

    private static void Internal_OrthoNormalize2(ref Vector3D a, ref Vector3D b)
    {
      Vector3D.INTERNAL_CALL_Internal_OrthoNormalize2(ref a, ref b);
    }

    private static void Internal_OrthoNormalize3(ref Vector3D a, ref Vector3D b, ref Vector3D c)
    {
      Vector3D.INTERNAL_CALL_Internal_OrthoNormalize3(ref a, ref b, ref c);
    }

    public static Vector3D Lerp(Vector3D a, Vector3D b, double t)
    {
      t = MathHelper.Clamp(t, 0.0, 1.0);
      return new Vector3D(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t, a.Z + (b.Z - a.Z) * t);
    }

    public static Vector3D LerpUnclamped(Vector3D a, Vector3D b, double t)
    {
      return new Vector3D(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t, a.Z + (b.Z - a.Z) * t);
    }

    public static Vector3D Max(Vector3D lhs, Vector3D rhs)
    {
      return new Vector3D(System.Math.Max(lhs.X, rhs.X), System.Math.Max(lhs.Y, rhs.Y), System.Math.Max(lhs.Z, rhs.Z));
    }

    public static Vector3D Min(Vector3D lhs, Vector3D rhs)
    {
      return new Vector3D(System.Math.Min(lhs.X, rhs.X), System.Math.Min(lhs.Y, rhs.Y), System.Math.Min(lhs.Z, rhs.Z));
    }

    public static Vector3D MoveTowards(Vector3D current, Vector3D target, double maxDistanceDelta)
    {
      Vector3D vector3D = target - current;
      double magnitude = vector3D.Magnitude;
      if (magnitude <= maxDistanceDelta || magnitude == 0.0)
        return target;
      return current + vector3D / magnitude * maxDistanceDelta;
    }

    public static Vector3D Normalize(Vector3D value)
    {
      double magnitude = value.Magnitude;
      if (magnitude > 1E-06)
        return value / magnitude;
      return Vector3D.Zero;
    }

    public static void OrthoNormalize(ref Vector3D normal, ref Vector3D tangent)
    {
      Vector3D.Internal_OrthoNormalize2(ref normal, ref tangent);
    }

    public static void OrthoNormalize(ref Vector3D normal, ref Vector3D tangent, ref Vector3D binormal)
    {
      Vector3D.Internal_OrthoNormalize3(ref normal, ref tangent, ref binormal);
    }

    public static Vector3D Project(Vector3D vector, Vector3D onNormal)
    {
      double num = Vector3D.Dot(onNormal, onNormal);
      if (num < double.Epsilon)
        return Vector3D.Zero;
      return onNormal * Vector3D.Dot(vector, onNormal) / num;
    }

    public static Vector3D ProjectOnPlane(Vector3D vector, Vector3D planeNormal)
    {
      return vector - Vector3D.Project(vector, planeNormal);
    }

    public static Vector3D Reflect(Vector3D inDirection, Vector3D inNormal)
    {
      return -2.0 * Vector3D.Dot(inNormal, inDirection) * inNormal + inDirection;
    }

    public static Vector3D RotateTowards(Vector3D current, Vector3D target, double maxRadiansDelta, double maxMagnitudeDelta)
    {
      Vector3D vector3D;
      Vector3D.INTERNAL_CALL_RotateTowards(ref current, ref target, maxRadiansDelta, maxMagnitudeDelta, out vector3D);
      return vector3D;
    }

    public static Vector3D Scale(Vector3D a, Vector3D b)
    {
      return new Vector3D(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    }

    public static Vector3D Slerp(Vector3D a, Vector3D b, double t)
    {
      Vector3D vector3D;
      Vector3D.INTERNAL_CALL_Slerp(ref a, ref b, t, out vector3D);
      return vector3D;
    }

    public static Vector3D SlerpUnclamped(Vector3D a, Vector3D b, double t)
    {
      Vector3D vector3D;
      Vector3D.INTERNAL_CALL_SlerpUnclamped(ref a, ref b, t, out vector3D);
      return vector3D;
    }

    public static Vector3D SmoothDamp(Vector3D current, Vector3D target, ref Vector3D currentVelocity, double smoothTime, double deltaTime)
    {
      return Vector3D.SmoothDamp(current, target, ref currentVelocity, smoothTime, double.PositiveInfinity, deltaTime);
    }

    public static Vector3D SmoothDamp(Vector3D current, Vector3D target, ref Vector3D currentVelocity, double smoothTime, double maxSpeed, double deltaTime)
    {
      smoothTime = System.Math.Max(0.0001, smoothTime);
      double num1 = 2.0 / smoothTime;
      double num2 = num1 * deltaTime;
      double num3 = 1.0 / (1.0 + num2 + 0.48 * num2 * num2 + 0.235 * num2 * num2 * num2);
      Vector3D vector = current - target;
      Vector3D vector3D1 = target;
      double maxLength = maxSpeed * smoothTime;
      Vector3D vector3D2 = Vector3D.ClampMagnitude(vector, maxLength);
      target = current - vector3D2;
      Vector3D vector3D3 = (currentVelocity + num1 * vector3D2) * deltaTime;
      currentVelocity = (currentVelocity - num1 * vector3D3) * num3;
      Vector3D vector3D4 = target + (vector3D2 + vector3D3) * num3;
      if (Vector3D.Dot(vector3D1 - current, vector3D4 - vector3D1) > 0.0)
      {
        vector3D4 = vector3D1;
        currentVelocity = (vector3D4 - vector3D1) / deltaTime;
      }
      return vector3D4;
    }

    public override bool Equals(object other)
    {
      if (!(other is Vector3D))
        return false;
      Vector3D vector3D = (Vector3D) other;
      return this.X.Equals(vector3D.X) && this.Y.Equals(vector3D.Y) && this.Z.Equals(vector3D.Z);
    }

    public override int GetHashCode()
    {
      return this.X.GetHashCode() ^ this.Y.GetHashCode() << 2 ^ this.Z.GetHashCode() >> 2;
    }

    public void Normalize()
    {
      double magnitude = this.Magnitude;
      if (magnitude > 1E-06)
        this = this / magnitude;
      else
        this = Vector3D.Zero;
    }

    public void Scale(Vector3D scale)
    {
      this.X = this.X * scale.X;
      this.Y = this.Y * scale.Y;
      this.Z = this.Z * scale.Z;
    }

    public void Set(double new_x, double new_y, double new_z)
    {
      this.X = new_x;
      this.Y = new_y;
      this.Z = new_z;
    }

    public string ToString(string format)
    {
      return string.Format("({0}, {1}, {2})", (object) this.X.ToString(format), (object) this.Y.ToString(format), (object) this.Z.ToString(format));
    }

    public override string ToString()
    {
      return string.Format("({0:0.###}, {1:0.###}, {2:0.###})", (object) this.X, (object) this.Y, (object) this.Z);
    }

    public static Vector3D operator +(Vector3D a, Vector3D b)
    {
      return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static Vector3D operator /(Vector3D a, double d)
    {
      return new Vector3D(a.X / d, a.Y / d, a.Z / d);
    }

    public static bool operator ==(Vector3D lhs, Vector3D rhs)
    {
      return (lhs - rhs).SqrMagnitude < 9.999999E-11;
    }

    public static bool operator !=(Vector3D lhs, Vector3D rhs)
    {
      return (lhs - rhs).SqrMagnitude >= 9.999999E-11;
    }

    public static Vector3D operator *(double d, Vector3D a)
    {
      return new Vector3D(a.X * d, a.Y * d, a.Z * d);
    }

    public static Vector3D operator *(Vector3D a, double d)
    {
      return new Vector3D(a.X * d, a.Y * d, a.Z * d);
    }

    public static Vector3D operator -(Vector3D a, Vector3D b)
    {
      return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public static Vector3D operator -(Vector3D a)
    {
      return new Vector3D(-a.X, -a.Y, -a.Z);
    }

    private static void INTERNAL_CALL_Internal_OrthoNormalize2(ref Vector3D a, ref Vector3D b)
    {
      throw new Exception("INTERNAL_CALL_Internal_OrthoNormalize2 IS NOT IMPLEMENTED");
    }

    private static void INTERNAL_CALL_Internal_OrthoNormalize3(ref Vector3D a, ref Vector3D b, ref Vector3D c)
    {
      a.Normalize();
      double num1 = Vector3D.Dot(a, b);
      b = b - num1 * a;
      b.Normalize();
      double num2 = Vector3D.Dot(b, c);
      double num3 = Vector3D.Dot(a, c);
      c = c - (num3 * a + num2 * b);
      c.Normalize();
    }

    private static void INTERNAL_CALL_RotateTowards(ref Vector3D current, ref Vector3D target, double maxRadiansDelta, double maxMagnitudeDelta, out Vector3D value)
    {
      value = Vector3D.Zero;
      throw new Exception("INTERNAL_CALL_RotateTowards IS NOT IMPLEMENTED");
    }

    private static void INTERNAL_CALL_Slerp(ref Vector3D a, ref Vector3D b, double t, out Vector3D value)
    {
      value = Vector3D.Zero;
      throw new Exception("INTERNAL_CALL_Slerp IS NOT IMPLEMENTED");
    }

    private static void INTERNAL_CALL_SlerpUnclamped(ref Vector3D a, ref Vector3D b, double t, out Vector3D value)
    {
      value = Vector3D.Zero;
      throw new Exception("INTERNAL_CALL_SlerpUnclamped IS NOT IMPLEMENTED");
    }
  }
}
