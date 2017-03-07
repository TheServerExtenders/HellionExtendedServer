using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZeroGravity.Objects;
using ZeroGravity.ShipComponents;

namespace HellionExtendedServer.GUI.Objects
{
    class Vessel
    {
        private SpaceObjectVessel m_vessel;

        public Vessel(SpaceObjectVessel vessel)
        {
            m_vessel = vessel;
        }



        public SpaceObjectVessel ThisVessel
        {
            get {return m_vessel; }
            set {m_vessel = value; }
        }

        public SpaceObjectType Type
        {
            get {return m_vessel.ObjectType; }
        }

        public List<Door> Doors 
        {
            get { return m_vessel.Doors; }
        }

        public List<Player> Crew
        {
            get { return m_vessel.VesselCrew; }
        }



    }
}
