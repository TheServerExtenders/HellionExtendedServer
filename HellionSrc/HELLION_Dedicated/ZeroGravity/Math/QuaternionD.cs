// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Math.QuaternionD
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;

namespace ZeroGravity.Math
{
  public struct QuaternionD
  {
    private const double epsilon = 1E-06;
    public double X;
    public double Y;
    public double Z;
    public double W;

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
          case 3:
            return this.W;
          default:
            throw new IndexOutOfRangeException("Invalid Quaternion index!");
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
          case 3:
            this.W = value;
            break;
          default:
            throw new IndexOutOfRangeException("Invalid Quaternion index!");
        }
      }
    }

    public static QuaternionD Identity
    {
      get
      {
        return new QuaternionD(0.0, 0.0, 0.0, 1.0);
      }
    }

    public Vector3D EulerAngles
    {
      get
      {
        return QuaternionD.Internal_ToEulerRad(this) * (180.0 / System.Math.PI);
      }
      set
      {
        this = QuaternionD.Internal_FromEulerRad(value * (System.Math.PI / 180.0));
      }
    }

    public QuaternionD(double x, double y, double z, double w)
    {
      this.X = x;
      this.Y = y;
      this.Z = z;
      this.W = w;
    }

    public static QuaternionD operator *(QuaternionD lhs, QuaternionD rhs)
    {
      return new QuaternionD(lhs.W * rhs.X + lhs.X * rhs.W + lhs.Y * rhs.Z - lhs.Z * rhs.Y, lhs.W * rhs.Y + lhs.Y * rhs.W + lhs.Z * rhs.X - lhs.X * rhs.Z, lhs.W * rhs.Z + lhs.Z * rhs.W + lhs.X * rhs.Y - lhs.Y * rhs.X, lhs.W * rhs.W - lhs.X * rhs.X - lhs.Y * rhs.Y - lhs.Z * rhs.Z);
    }

    public static Vector3D operator *(QuaternionD rotation, Vector3D point)
    {
      double num1 = rotation.X * 2.0;
      double num2 = rotation.Y * 2.0;
      double num3 = rotation.Z * 2.0;
      double num4 = rotation.X * num1;
      double num5 = rotation.Y * num2;
      double num6 = rotation.Z * num3;
      double num7 = rotation.X * num2;
      double num8 = rotation.X * num3;
      double num9 = rotation.Y * num3;
      double num10 = rotation.W * num1;
      double num11 = rotation.W * num2;
      double num12 = rotation.W * num3;
      Vector3D vector3D;
      vector3D.X = (1.0 - (num5 + num6)) * point.X + (num7 - num12) * point.Y + (num8 + num11) * point.Z;
      vector3D.Y = (num7 + num12) * point.X + (1.0 - (num4 + num6)) * point.Y + (num9 - num10) * point.Z;
      vector3D.Z = (num8 - num11) * point.X + (num9 + num10) * point.Y + (1.0 - (num4 + num5)) * point.Z;
      return vector3D;
    }

    public static bool operator ==(QuaternionD lhs, QuaternionD rhs)
    {
      return QuaternionD.Dot(lhs, rhs) > 0.999998986721039;
    }

    public static bool operator !=(QuaternionD lhs, QuaternionD rhs)
    {
      return QuaternionD.Dot(lhs, rhs) <= 0.999998986721039;
    }

    public void Set(double new_x, double new_y, double new_z, double new_w)
    {
      this.X = new_x;
      this.Y = new_y;
      this.Z = new_z;
      this.W = new_w;
    }

    public static double Dot(QuaternionD a, QuaternionD b)
    {
      return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
    }

    public static QuaternionD AngleAxis(double angle, Vector3D axis)
    {
      axis.Normalize();
      QuaternionD quaternionD;
      QuaternionD.INTERNAL_CALL_AngleAxis(angle, ref axis, out quaternionD);
      return quaternionD;
    }

    public void ToAngleAxis(out double angle, out Vector3D axis)
    {
      QuaternionD.Internal_ToAxisAngleRad(this, out axis, out angle);
      angle = angle * (180.0 / System.Math.PI);
    }

    public static QuaternionD FromToRotation(Vector3D fromDirection, Vector3D toDirection)
    {
      QuaternionD quaternionD;
      QuaternionD.INTERNAL_CALL_FromToRotation(ref fromDirection, ref toDirection, out quaternionD);
      return quaternionD;
    }

    public void SetFromToRotation(Vector3D fromDirection, Vector3D toDirection)
    {
      this = QuaternionD.FromToRotation(fromDirection, toDirection);
    }

    public static QuaternionD LookRotation(Vector3D forward, Vector3D upwards)
    {
      QuaternionD quaternionD;
      QuaternionD.INTERNAL_CALL_LookRotation(ref forward, ref upwards, out quaternionD);
      return quaternionD;
    }

    public static QuaternionD LookRotation(Vector3D forward)
    {
      Vector3D up = Vector3D.Up;
      QuaternionD quaternionD;
      QuaternionD.INTERNAL_CALL_LookRotation(ref forward, ref up, out quaternionD);
      return quaternionD;
    }

    public void SetLookRotation(Vector3D view)
    {
      this.SetLookRotation(view, Vector3D.Up);
    }

    public void SetLookRotation(Vector3D view, Vector3D up)
    {
      this = QuaternionD.LookRotation(view, up);
    }

    public static QuaternionD Slerp(QuaternionD a, QuaternionD b, double t)
    {
      QuaternionD quaternionD;
      QuaternionD.INTERNAL_CALL_Slerp(ref a, ref b, t, out quaternionD);
      return quaternionD;
    }

    public static QuaternionD SlerpUnclamped(QuaternionD a, QuaternionD b, double t)
    {
      QuaternionD quaternionD;
      QuaternionD.INTERNAL_CALL_SlerpUnclamped(ref a, ref b, t, out quaternionD);
      return quaternionD;
    }

    public static QuaternionD Lerp(QuaternionD a, QuaternionD b, double t)
    {
      QuaternionD quaternionD;
      QuaternionD.INTERNAL_CALL_Lerp(ref a, ref b, t, out quaternionD);
      return quaternionD;
    }

    public static QuaternionD LerpUnclamped(QuaternionD a, QuaternionD b, double t)
    {
      QuaternionD quaternionD;
      QuaternionD.INTERNAL_CALL_LerpUnclamped(ref a, ref b, t, out quaternionD);
      return quaternionD;
    }

    public static QuaternionD RotateTowards(QuaternionD from, QuaternionD to, double maxDegreesDelta)
    {
      double num = QuaternionD.Angle(from, to);
      if (num == 0.0)
        return to;
      double t = System.Math.Min(1.0, maxDegreesDelta / num);
      return QuaternionD.SlerpUnclamped(from, to, t);
    }

    public static QuaternionD Inverse(QuaternionD rotation)
    {
      QuaternionD quaternionD;
      QuaternionD.INTERNAL_CALL_Inverse(ref rotation, out quaternionD);
      return quaternionD;
    }

    public override string ToString()
    {
      return string.Format("({0:0.###}, {1:0.###}, {2:0.###}, {3:0.###})", (object) this.X, (object) this.Y, (object) this.Z, (object) this.W);
    }

    public string ToString(string format)
    {
      return string.Format("({0}, {1}, {2}, {3})", (object) this.X.ToString(format), (object) this.Y.ToString(format), (object) this.Z.ToString(format), (object) this.W.ToString(format));
    }

    public static double Angle(QuaternionD a, QuaternionD b)
    {
      return System.Math.Acos(System.Math.Min(System.Math.Abs(QuaternionD.Dot(a, b)), 1.0)) * 2.0 * (180.0 / System.Math.PI);
    }

    public static QuaternionD Euler(double x, double y, double z)
    {
      return QuaternionD.Internal_FromEulerRad(new Vector3D(x, y, z) * (System.Math.PI / 180.0));
    }

    public static QuaternionD Euler(Vector3D euler)
    {
      return QuaternionD.Internal_FromEulerRad(euler * (System.Math.PI / 180.0));
    }

    private static Vector3D Internal_ToEulerRad(QuaternionD rotation)
    {
      Vector3D vector3D;
      QuaternionD.INTERNAL_CALL_ToEulerRad(ref rotation, out vector3D);
      return vector3D;
    }

    private static QuaternionD Internal_FromEulerRad(Vector3D euler)
    {
      QuaternionD quaternionD;
      QuaternionD.INTERNAL_CALL_FromEulerRad(ref euler, out quaternionD);
      return quaternionD;
    }

    private static void Internal_ToAxisAngleRad(QuaternionD q, out Vector3D axis, out double angle)
    {
      QuaternionD.INTERNAL_CALL_ToAxisAngleRad(ref q, out axis, out angle);
    }

    public override int GetHashCode()
    {
      return this.X.GetHashCode() ^ this.Y.GetHashCode() << 2 ^ this.Z.GetHashCode() >> 2 ^ this.W.GetHashCode() >> 1;
    }

    public override bool Equals(object other)
    {
      if (!(other is QuaternionD))
        return false;
      QuaternionD quaternionD = (QuaternionD) other;
      if (this.X.Equals(quaternionD.X) && this.Y.Equals(quaternionD.Y) && this.Z.Equals(quaternionD.Z))
        return this.W.Equals(quaternionD.W);
      return false;
    }

    private static void INTERNAL_CALL_AngleAxis(double angle, ref Vector3D axis, out QuaternionD value)
    {
      value = QuaternionD.Identity;
      if (axis.SqrMagnitude <= 1E-06)
        return;
      double num = System.Math.Sin(angle * (System.Math.PI / 180.0) * 0.5);
      double new_w = System.Math.Cos(angle * (System.Math.PI / 180.0) * 0.5);
      value.Set(axis.X * num, axis.Y * num, axis.Z * num, new_w);
    }

    private static void INTERNAL_CALL_ToAxisAngleRad(ref QuaternionD q, out Vector3D axis, out double angle)
    {
      axis = Vector3D.Zero;
      angle = 0.0;
      double num = q.X * q.X + q.Y * q.Y + q.Z * q.Z;
      if (num > 1E-06)
      {
        angle = 2.0 * System.Math.Acos(q.W);
        axis = new Vector3D(q.X, q.Y, q.Z) / num;
      }
      else
      {
        angle = 0.0;
        axis = new Vector3D(1.0, 0.0, 0.0);
      }
    }

    private static void INTERNAL_CALL_FromEulerRad(ref Vector3D euler, out QuaternionD value)
    {
      double num1 = euler.X * 0.5;
      double num2 = euler.Y * 0.5;
      double num3 = euler.Z * 0.5;
      double w1 = System.Math.Cos(num1);
      double x = System.Math.Sin(num1);
      double w2 = System.Math.Cos(num2);
      double y = System.Math.Sin(num2);
      double w3 = System.Math.Cos(num3);
      double z = System.Math.Sin(num3);
      QuaternionD[] quaternionDArray = new QuaternionD[3]
      {
        new QuaternionD(x, 0.0, 0.0, w1),
        new QuaternionD(0.0, y, 0.0, w2),
        new QuaternionD(0.0, 0.0, z, w3)
      };
      value = quaternionDArray[2] * quaternionDArray[0] * quaternionDArray[1];
    }

    private static void INTERNAL_CALL_ToEulerRad(ref QuaternionD rotation, out Vector3D value)
    {
      double[,] numArray = new double[3, 3]
      {
        {
          1.0 - (2.0 * rotation.Y * rotation.Y + 2.0 * rotation.Z * rotation.Z),
          2.0 * rotation.Y * rotation.X - 2.0 * rotation.Z * rotation.W,
          2.0 * rotation.Z * rotation.X + 2.0 * rotation.Y * rotation.W
        },
        {
          2.0 * rotation.Y * rotation.X + 2.0 * rotation.Z * rotation.W,
          1.0 - (2.0 * rotation.X * rotation.X + 2.0 * rotation.Z * rotation.Z),
          2.0 * rotation.Z * rotation.Y - 2.0 * rotation.X * rotation.W
        },
        {
          2.0 * rotation.Z * rotation.X - 2.0 * rotation.Y * rotation.W,
          2.0 * rotation.Z * rotation.Y + 2.0 * rotation.X * rotation.W,
          1.0 - (2.0 * rotation.X * rotation.X + 2.0 * rotation.Y * rotation.Y)
        }
      };
      value = Vector3D.Zero;
      double new_x = -System.Math.Asin(numArray[1, 2]);
      if (new_x >= System.Math.PI / 2.0)
        value.Set(System.Math.PI / 2.0, System.Math.Atan2(numArray[0, 1], numArray[0, 0]), 0.0);
      else if (new_x <= -1.0 * System.Math.PI / 2.0)
        value.Set(-1.0 * System.Math.PI / 2.0, System.Math.Atan2(-numArray[0, 1], numArray[0, 0]), 0.0);
      else
        value.Set(new_x, System.Math.Atan2(numArray[0, 2], numArray[2, 2]), System.Math.Atan2(numArray[1, 0], numArray[1, 1]));
    }

    private static void INTERNAL_CALL_Inverse(ref QuaternionD rotation, out QuaternionD value)
    {
      value = rotation;
      double num1 = rotation.X * rotation.X + rotation.Y * rotation.Y + rotation.Z * rotation.Z + rotation.W * rotation.W;
      if (num1 <= 1E-06)
        return;
      double num2 = 1.0 / num1;
      value.X = -rotation.X * num2;
      value.Y = -rotation.Y * num2;
      value.Z = -rotation.Z * num2;
      value.W = rotation.W * num2;
    }

    private static void INTERNAL_CALL_LookRotation(ref Vector3D forward, ref Vector3D up, out QuaternionD value)
    {
      forward = Vector3D.Normalize(forward);
      Vector3D rhs = Vector3D.Normalize(Vector3D.Cross(up, forward));
      up = Vector3D.Cross(forward, rhs);
      double x1 = rhs.X;
      double y1 = rhs.Y;
      double z1 = rhs.Z;
      double x2 = up.X;
      double y2 = up.Y;
      double z2 = up.Z;
      double x3 = forward.X;
      double y3 = forward.Y;
      double z3 = forward.Z;
      double num1 = x1 + y2 + z3;
      if (num1 > 0.0)
      {
        double num2 = System.Math.Sqrt(num1 + 1.0);
        value.W = num2 * 0.5;
        double num3 = 0.5 / num2;
        value.X = (z2 - y3) * num3;
        value.Y = (x3 - z1) * num3;
        value.Z = (y1 - x2) * num3;
      }
      else if (x1 >= y2 && x1 >= z3)
      {
        double num2 = System.Math.Sqrt(1.0 + x1 - y2 - z3);
        double num3 = 0.5 / num2;
        value.X = 0.5 * num2;
        value.Y = (y1 + x2) * num3;
        value.Z = (z1 + x3) * num3;
        value.W = (z2 - y3) * num3;
      }
      else if (y2 > z3)
      {
        double num2 = System.Math.Sqrt(1.0 + y2 - x1 - z3);
        double num3 = 0.5 / num2;
        value.X = (x2 + y1) * num3;
        value.Y = 0.5 * num2;
        value.Z = (y3 + z2) * num3;
        value.W = (x3 - z1) * num3;
      }
      else
      {
        double num2 = System.Math.Sqrt(1.0 + z3 - x1 - y2);
        double num3 = 0.5 / num2;
        value.X = (x3 + z1) * num3;
        value.Y = (y3 + z2) * num3;
        value.Z = 0.5 * num2;
        value.W = (y1 - x2) * num3;
      }
    }

    private static void INTERNAL_CALL_FromToRotation(ref Vector3D fromDirection, ref Vector3D toDirection, out QuaternionD value)
    {
      value = QuaternionD.RotateTowards(QuaternionD.LookRotation(fromDirection), QuaternionD.LookRotation(toDirection), double.MaxValue);
    }

    private static void INTERNAL_CALL_Slerp(ref QuaternionD from, ref QuaternionD to, double t, out QuaternionD value)
    {
      QuaternionD.INTERNAL_CALL_SlerpUnclamped(ref from, ref to, MathHelper.Clamp(t, 0.0, 1.0), out value);
    }

    private static void INTERNAL_CALL_SlerpUnclamped(ref QuaternionD from, ref QuaternionD to, double t, out QuaternionD value)
    {
      double d = from.X * to.X + from.Y * to.Y + from.Z * to.Z + from.W * to.W;
      bool flag = false;
      if (d < 0.0)
      {
        flag = true;
        d = -d;
      }
      double num1;
      double num2;
      if (d > 0.999999)
      {
        num1 = 1.0 - t;
        num2 = flag ? -t : t;
      }
      else
      {
        double a = System.Math.Acos(d);
        double num3 = 1.0 / System.Math.Sin(a);
        num1 = System.Math.Sin((1.0 - t) * a) * num3;
        num2 = flag ? -System.Math.Sin(t * a) * num3 : System.Math.Sin(t * a) * num3;
      }
      value.X = num1 * from.X + num2 * to.X;
      value.Y = num1 * from.Y + num2 * to.Y;
      value.Z = num1 * from.Z + num2 * to.Z;
      value.W = num1 * from.W + num2 * to.W;
    }

    private static void INTERNAL_CALL_Lerp(ref QuaternionD from, ref QuaternionD to, double t, out QuaternionD value)
    {
      QuaternionD.INTERNAL_CALL_LerpUnclamped(ref from, ref to, MathHelper.Clamp(t, 0.0, 1.0), out value);
    }

    private static void INTERNAL_CALL_LerpUnclamped(ref QuaternionD from, ref QuaternionD to, double t, out QuaternionD value)
    {
      double num1 = 1.0 - t;
      if (from.X * to.X + from.Y * to.Y + from.Z * to.Z + from.W * to.W >= 0.0)
      {
        value.X = num1 * from.X + t * to.X;
        value.Y = num1 * from.Y + t * to.Y;
        value.Z = num1 * from.Z + t * to.Z;
        value.W = num1 * from.W + t * to.W;
      }
      else
      {
        value.X = num1 * from.X - t * to.X;
        value.Y = num1 * from.Y - t * to.Y;
        value.Z = num1 * from.Z - t * to.Z;
        value.W = num1 * from.W - t * to.W;
      }
      double num2 = 1.0 / System.Math.Sqrt(value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W);
      value.X *= num2;
      value.Y *= num2;
      value.Z *= num2;
      value.W *= num2;
    }
  }
}
