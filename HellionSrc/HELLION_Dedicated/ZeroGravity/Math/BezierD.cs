// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Math.BezierD
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

namespace ZeroGravity.Math
{
  public class BezierD
  {
    public double Length = 0.0;
    public Vector3D P0;
    public Vector3D P1;
    public Vector3D P2;
    public Vector3D P3;

    public BezierD(Vector3D p0, Vector3D p1, Vector3D p2, Vector3D p3)
    {
      this.P0 = p0;
      this.P1 = p1;
      this.P2 = p2;
      this.P3 = p3;
    }

    public void SetPoints(Vector3D p0, Vector3D p1, Vector3D p2, Vector3D p3)
    {
      this.P0 = p0;
      this.P1 = p1;
      this.P2 = p2;
      this.P3 = p3;
    }

    public void FillDataAtPart(double t, ref Vector3D point, ref Vector3D tangent)
    {
      Vector3D a1 = Vector3D.Lerp(this.P0, this.P1, t);
      Vector3D vector3D = Vector3D.Lerp(this.P1, this.P2, t);
      Vector3D b1 = Vector3D.Lerp(this.P2, this.P3, t);
      Vector3D a2 = Vector3D.Lerp(a1, vector3D, t);
      Vector3D b2 = Vector3D.Lerp(vector3D, b1, t);
      point = Vector3D.Lerp(a2, b2, t);
      tangent = b2 - a2;
    }
  }
}
