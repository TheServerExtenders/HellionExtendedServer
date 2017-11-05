using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZeroGravity.Objects;
using ZeroGravity.ShipComponents;
using ZeroGravity.Network;
using ZeroGravity.Math;

namespace HellionExtendedServer.GUI.Objects
{
    /// <summary>
    /// This class contains all the Properties that are added to each object property panel
    /// </summary>
    public abstract class BaseObject
    {
       
        public BaseObject(SpaceObject parent)
        {
            SpaceObject = parent;
            GUID = parent.GUID;
        }

        [Browsable(false)]
        public SpaceObject SpaceObject { get; private set; }

        public long GUID { get; private set; }

        public virtual string Name { get; private set; }

        [Category("Space Object")]
        public virtual string Position
        {
            get => SpaceObject.Position.ToString();
            set
            {
                var vector = StringToVector(value);

                SpaceObject.Position.Set(vector.X, vector.Y, vector.Z);
            }
            
        }

        [ReadOnly(true)]
        [Category("Space Object")]
        public virtual Vector3D Velocity => SpaceObject.Velocity;


        public Vector3D StringToVector(string _value)
        {
            var args = _value.Split(',');

            if (args.Length < 0)
                return SpaceObject.Position;

            if (!double.TryParse(args[0], out double x))
                return SpaceObject.Position;

            if (!double.TryParse(args[1], out double y))
                return SpaceObject.Position;

            if (!double.TryParse(args[2], out double z))
                return SpaceObject.Position;

            return new Vector3D(x, y, z);
        }

    }
}
