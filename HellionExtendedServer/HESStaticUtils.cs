using BulletSharp;
using ZeroGravity.Math;
using ZeroGravity.Objects;

namespace HellionExtendedServer
{
    public static class HESStaticUtils
    {
        public static Vector3D clone(this Vector3D v)
        {
            return new Vector3D(v.X,v.Y,v.Z);
        }

    }
}