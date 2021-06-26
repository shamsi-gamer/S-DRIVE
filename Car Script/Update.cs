using Sandbox.ModAPI.Ingame;
using VRageMath;


namespace IngameScript
{
    partial class Program
    {
        void UpdateLocation()
        {
            //if (   m_centBlock != null
            //    && m_fwdBlock  != null
            //    && m_upBlock   != null)
            //{ 
	           // Vector3 fwd =
		          //  m_forwardBlockIsBehind
		          //  ? unit(m_centBlock.GetPosition() - m_fwdBlock.GetPosition())
		          //  : unit(m_fwdBlock.GetPosition() - m_centBlock.GetPosition());

	           // Vector3 up =
		          //  m_upBlockIsBelow
		          //  ? unit(m_centBlock.GetPosition() - m_upBlock.GetPosition())
		          //  : unit(m_upBlock.GetPosition() - m_centBlock.GetPosition());

	           // m_location = new Location(m_cockpit.CenterOfMass, fwd, up);
	
            //    // m_lastLocation is assigned at the end of UpdateVelocity()
            //}


	        double alt = 0;
            m_cockpit.TryGetPlanetElevation(MyPlanetElevation.Sealevel, out alt);
            m_altitude = (float)alt + m_groundLevel;
        }


        void UpdateVelocity()
        {
            // linear velocity
            if (Location.IsNaN(m_lastLocation))
                m_lastLocation = m_location;

            m_linVelocity = new Vector3(
                distanceToPlane(m_lastLocation.Position, m_location.Position, m_location.Right  ) / dt1,
                distanceToPlane(m_lastLocation.Position, m_location.Position, m_location.Up     ) / dt1,
                distanceToPlane(m_lastLocation.Position, m_location.Position, m_location.Forward) / dt1);


            // acceleration
            m_linAcceleration = (m_linVelocity - m_lastLinVelocity) / dt1;
            m_lastLinVelocity = m_linVelocity;


            // angular velocity
            float angleX = angleToPlane(m_location.Forward, m_lastLocation.Right, -m_lastLocation.Up);
            float angleY = angleToPlane(m_location.Forward, m_lastLocation.Up,     m_lastLocation.Right);
            float angleZ = angleToPlane(m_location.Up,      m_lastLocation.Right,  m_lastLocation.Forward);

            m_angVelocity = new Vector3(
                angleX / Tau / dt1,
                angleY / Tau / dt1,
                angleZ / Tau / dt1);


            // angular acceleration
            m_angAcceleration = (m_angVelocity - m_lastAngVelocity) / dt1;
            m_lastAngVelocity = m_angVelocity;


            // finishing up for UpdateLocation()
            m_lastLocation = m_location;
        }


        void UpdateOrientation()
        {
            m_orientation.X = angleToPlane(
                -m_location.Up,
                -Vector3.Cross(m_cockpit.GetNaturalGravity(), Vector3.Cross(m_cockpit.GetNaturalGravity(), m_location.Forward)),
                 Vector3.Cross(m_cockpit.GetNaturalGravity(), m_location.Forward)) / (Tau/2);

            m_orientation.Z = -angleToPlane(
                 m_location.Up,
                -Vector3.Cross(m_cockpit.GetNaturalGravity(), m_location.Forward),
                 Vector3.Cross(m_cockpit.GetNaturalGravity(), Vector3.Cross(m_cockpit.GetNaturalGravity(), m_location.Forward))) / (Tau/2);


            Vector3D planetCenter;
            m_cockpit.TryGetPlanetPosition(out planetCenter);

            var perp = Vector3.Cross(
                planetCenter - m_location.Position,
                planetCenter + unit(m_north) * m_seaLevel - m_location.Position);

            m_orientation.Y = -angleToPlane(
                Vector3.Cross(m_location.Position - planetCenter, Vector3.Cross(m_location.Forward, m_location.Position - planetCenter)),
               -perp,
                m_location.Up) / (Tau/2);


            m_heading = m_orientation.Y * 180;

            if (m_heading < 0)
                m_heading += 360;
        }


        void UpdateLatitudeAndLongitude()
        {
            var planetCenter = default(Vector3D);
            m_cockpit.TryGetPlanetPosition(out planetCenter);

            m_latitude = angleToPlane(
                m_location.Position - planetCenter,
                m_north,
                -Vector3.Cross(m_cockpit.GetNaturalGravity(), m_north)) / (Tau/2) * 90;

            m_longitude = angleToPlane(
                Vector3.Cross(m_north, Vector3.Cross(m_location.Position - planetCenter, m_north)),
                m_prime,
                m_north) / (Tau/2) * 180;
        }
    }
}
