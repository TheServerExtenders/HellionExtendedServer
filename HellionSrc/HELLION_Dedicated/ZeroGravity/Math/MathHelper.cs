// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Math.MathHelper
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;

namespace ZeroGravity.Math
{
  public static class MathHelper
  {
    private static Random _randGenerator = new Random();
    public const double RadToDeg = 57.2957795130823;
    public const double DegToRad = 0.0174532925199433;

    public static int Clamp(int value, int min, int max)
    {
      return value < min ? min : (value > max ? max : value);
    }

    public static float Clamp(float value, float min, float max)
    {
      return (double) value < (double) min ? min : ((double) value > (double) max ? max : value);
    }

    public static double Clamp(double value, double min, double max)
    {
      return value < min ? min : (value > max ? max : value);
    }

    public static float Lerp(float value1, float value2, float amount)
    {
      return value1 + (value2 - value1) * MathHelper.Clamp(amount, 0.0f, 1f);
    }

    public static double Lerp(double value1, double value2, double amount)
    {
      return value1 + (value2 - value1) * MathHelper.Clamp(amount, 0.0, 1.0);
    }

    public static float LerpValue(float fromVelocity, float toVelocity, float lerpAmount, float epsilon = 0.01f)
    {
      if ((double) fromVelocity != (double) toVelocity)
      {
        fromVelocity = (double) fromVelocity >= (double) toVelocity ? System.Math.Max(fromVelocity + (toVelocity - fromVelocity) * MathHelper.Clamp(lerpAmount, 0.0f, 1f), toVelocity) : System.Math.Min(fromVelocity + (toVelocity - fromVelocity) * MathHelper.Clamp(lerpAmount, 0.0f, 1f), toVelocity);
        if ((double) System.Math.Abs(toVelocity - fromVelocity) < (double) epsilon)
          fromVelocity = toVelocity;
      }
      return fromVelocity;
    }

    public static double SmoothStep(double value1, double value2, double amount)
    {
      return MathHelper.Hermite(value1, 0.0, value2, 0.0, MathHelper.Clamp(amount, 0.0, 1.0));
    }

    public static int Sign(double value)
    {
      return value < 0.0 ? -1 : 1;
    }

    public static double Hermite(double value1, double tangent1, double value2, double tangent2, double amount)
    {
      double num1 = amount * amount * amount;
      double num2 = amount * amount;
      if (amount == 0.0)
        return value1;
      if (amount == 1.0)
        return value2;
      return (2.0 * value1 - 2.0 * value2 + tangent2 + tangent1) * num1 + (3.0 * value2 - 3.0 * value1 - 2.0 * tangent1 - tangent2) * num2 + tangent1 * amount + value1;
    }

    public static float ProportionalValue(float basedOnCurrent, float basedOnMin, float basedOnMax, float resultMin, float resoultMax)
    {
      return resultMin + (float) (((double) resoultMax - (double) resultMin) * (((double) basedOnCurrent - (double) basedOnMin) / ((double) basedOnMax - (double) basedOnMin)));
    }

    public static double ProportionalValueDouble(double basedOnCurrent, double basedOnMin, double basedOnMax, double resultMin, double resoultMax)
    {
      return resultMin + (resoultMax - resultMin) * ((basedOnCurrent - basedOnMin) / (basedOnMax - basedOnMin));
    }

    public static Vector3D ProportionalValue(Vector3D basedOnCurrent, Vector3D basedOnMin, Vector3D basedOnMax, Vector3D resultMin, Vector3D resoultMax)
    {
      Vector3D vector3D1 = resultMin;
      Vector3D vector3D2 = resoultMax - resultMin;
      Vector3D vector3D3 = basedOnCurrent - basedOnMin;
      double magnitude1 = vector3D3.Magnitude;
      vector3D3 = basedOnMax - basedOnMin;
      double magnitude2 = vector3D3.Magnitude;
      double num = magnitude1 / magnitude2;
      Vector3D vector3D4 = vector3D2 * num;
      return vector3D1 + vector3D4;
    }

    public static float SetEpsilonZero(float value, float epsilon = 1.401298E-45f)
    {
      return (double) System.Math.Abs(value) > (double) epsilon ? value : 0.0f;
    }

    public static long LongRandom(long min, long max, Random rand)
    {
      byte[] buffer = new byte[8];
      rand.NextBytes(buffer);
      return System.Math.Abs(BitConverter.ToInt64(buffer, 0) % (max - min)) + min;
    }

    public static double AngleSigned(Vector3D vec1, Vector3D vec2, Vector3D planeNormal)
    {
      return Vector3D.Angle(vec1, vec2) * (double) MathHelper.Sign(Vector3D.Dot(planeNormal, Vector3D.Cross(vec1, vec2)));
    }

    public static Vector3D RotateAroundPivot(Vector3D vector, Vector3D pivot, Vector3D angles)
    {
      return QuaternionD.Euler(angles) * (vector - pivot) + pivot;
    }

    public static float AverageMaxValue(float a, float b, float c, float maxA, float maxB, float maxC)
    {
      return (float) (((double) a + (double) b + (double) c) / ((double) a / (double) maxA + (double) b / (double) maxB + (double) c / (double) maxC));
    }

    public static double Acosh(double x)
    {
      return System.Math.Log(x + System.Math.Sqrt(x * x - 1.0));
    }

    public static float RandomRange(float min, float max)
    {
      return (float) (MathHelper._randGenerator.NextDouble() * ((double) max - (double) min)) + min;
    }

    public static double RandomRange(double min, double max)
    {
      return MathHelper._randGenerator.NextDouble() * (max - min) + min;
    }

    public static int RandomRange(int min, int max)
    {
      return MathHelper._randGenerator.Next(min, max);
    }

    public static double RandomNextDouble()
    {
      return MathHelper._randGenerator.NextDouble();
    }

    public static int RandomNextInt()
    {
      return MathHelper._randGenerator.Next();
    }

    public static QuaternionD RandomRotation()
    {
      return QuaternionD.Euler(MathHelper.RandomRange(0.0, 359.99), MathHelper.RandomRange(0.0, 359.99), MathHelper.RandomRange(0.0, 359.99));
    }
  }
}
