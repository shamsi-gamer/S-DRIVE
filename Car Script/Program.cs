using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRageMath;


namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // use sound block for turn signal clicks
        // figure out how to use sound block with that sound to make springs sound rusty in a more predictable way
        // see if same trick can be used to make an engine sound

        // cruise control should actively brake if exceeding set speed (ex. downhill)
        // brake on sudden obstacle (when on hills there could be a sudden climb etc)
        // deploy parachutes automatically if vertical speed exceeds falling threshold
        // add reactor auto on/off (when fully discharded/recharghed)
        // add ON/OFF function

        // get all uranium in network connected to reactors
        // compare command hashes not commands
        // skid steering
        // norton commander for the car
        // use solar panel light info to adjust display color brightness
        // don't spin wheels not in contact with ground (except maybe to match landing speed), but when the truck is on its side, don't spin the wheels in the air if autopilot is on

        // autopilot
        //   where to draw the next destination w/o map? distance/eta?
        //      around compass, but instead of dots draw distances
        //   define roads with series of points, add intersections
        //
        //   shoot vertical rays forward
        //   if farthest rays hit further than a parallel ground (with some curvature), assume a descent and slow down
        //   if they hit a steep angle, slow down before hill (will have to be tuned)
        //   if they hit something steeper than an even steeper angle, assume an obstacle and go around
        //   use slope of terrain to reset "level" ground and re-aim the rays

        Vector3      m_move;
        Vector3      m_rotate;

        Vector3      m_lastMove10;
        Vector3      m_lastRotate10;

        float        m_instCount;
        IMyCockpit   m_cockpit;

        Display      m_display1;
        Display      m_display2;

        List<Display> m_displays;


        List<IMyMotorSuspension> m_lWheels;
        List<IMyMotorSuspension> m_rWheels;

        List<float> m_susp;

        //bool  m_kneel;
        //float m_kneelState;


        IMyMotorSuspension m_flw;
        IMyMotorSuspension m_frw;
        IMyMotorSuspension m_rlw;
        IMyMotorSuspension m_rrw;

        List<IMyReflectorLight> m_headlights;

        IMyInteriorLight m_lightFL;
        IMyInteriorLight m_lightFR;
        IMyInteriorLight m_lightBL;
        IMyInteriorLight m_lightBR;

        //List<IMyBatteryBlock> m_batteries;

        int          m_midZ;
        int          m_minZ;
        int          m_maxZ;

        float        m_steering;
        float        m_steer;
        float        m_propulsion;

        long         m_count10;

        float        m_speedLimit; // in m/s
        float        m_reverseSpeedLimit; // in km/h
        bool         m_reverseBeep;
        List<IMySoundBlock> m_beepBlocks;

        bool         m_lightsOn;
        bool         m_farLights;
        bool         m_reversing;
        bool         m_braking;
        bool         m_emergency;

        bool         m_brakeStarted;
        bool         m_prevReversing;

        bool         m_cruiseControl;
        float        m_ccSpeed;
        float        m_ccHeading;
        bool         m_ccPressed;
        int          m_ccCounter;

        bool         m_turningLeft;
        bool         m_pressedLeft;
        int          m_counterLeft;

        bool         m_turningRight;
        bool         m_pressedRight;
        int          m_counterRight;

        bool         m_parked;

        bool         m_autoHandbrake;
        bool         m_crouchPark;

        bool         m_turning;
        int          m_pressCount;

        Location     m_location;
        Location     m_lastLocation;

        float        m_dspSpeed;
        float        m_dspPower;

        Vector3      m_north;
        Vector3      m_prime;
        Vector3      m_orientation;

        float        m_latitude;
        float        m_longitude;

        float        m_seaLevel;
        float        m_groundLevel;

        float        m_altitude;

        float        m_heading;

        Vector3      m_linVelocity;
        Vector3      m_lastLinVelocity;
        Vector3      m_linAcceleration;

        Vector3      m_angVelocity;
        Vector3      m_lastAngVelocity;
        Vector3      m_angAcceleration;

        //IMyCubeBlock m_centBlock;
        //IMyCubeBlock m_fwdBlock;
        //bool         m_forwardBlockIsBehind;
        //IMyCubeBlock m_upBlock;
        //bool         m_upBlockIsBelow;

        //float        m_curBatteryPower;
        //float        m_maxBatteryPower;
        //float        m_curSolarPower;
        //float        m_maxSolarPower;

        //List<IMyReactor> m_reactors;

        float        m_mass;
        float        m_prevMass;

        //bool         m_kneelFromInside;
        bool         m_autoLights;
        float        m_brakeSpeed;
        //bool         m_help;


        public struct Location
        {
            public Vector3 Position;
            public Vector3 Forward;
            public Vector3 Up;

            public Vector3 Right { get { return Vector3.Cross(Forward, Up); } }

            public Location(Vector3 pos, Vector3 fwd, Vector3 up)
            {
                Position = pos;
                Forward  = fwd;
                Up       = up;
            }

            public static Location operator +(Location l1, Location l2)
            {
                return new Location(
                    l1.Position + l2.Position,
                    l1.Forward  + l2.Forward,
                    l1.Up       + l2.Up);
            }

            public static Location operator -(Location l1, Location l2)
            {
                return new Location(
                    l1.Position - l2.Position,
                    l1.Forward  - l2.Forward,
                    l1.Up       - l2.Up);
            }

            public static Location operator *(Location l1, float f)
            {
                return new Location(
                    l1.Position * f,
                    l1.Forward  * f,
                    l1.Up       * f);
            }

            public static bool IsNaN(Location l)
            {
                return
                       Vector3_IsNaN(l.Position)
                    || Vector3_IsNaN(l.Forward)
                    || Vector3_IsNaN(l.Up);
            }

            public static Location Zero = new Location(Vector3.Zero, Vector3.Zero, Vector3.Zero);
            public static Location NaN  = new Location(Vector3_NaN,  Vector3_NaN,  Vector3_NaN);
        }



        public Program()
        {
            this.Runtime.UpdateFrequency = 
                  UpdateFrequency.Update100
                | UpdateFrequency.Update10;
                //| UpdateFrequency.Update1;

            m_move   = default(Vector3);
            m_rotate = default(Vector3);

            m_lastMove10   = default(Vector3);
            m_lastRotate10 = default(Vector3);


            var cockpits = new List<IMyCockpit>();
            GridTerminalSystem.GetBlocksOfType(cockpits);

            m_cockpit = cockpits[0];


            m_instCount = 0;
                
            m_lWheels = new List<IMyMotorSuspension>();
            m_rWheels = new List<IMyMotorSuspension>();
                 
            m_susp = new List<float>();

            m_flw = null;
            m_frw = null;
            m_rlw = null;
            m_rrw = null;

            m_headlights = new List<IMyReflectorLight>();

            m_lightFL = null;
            m_lightFR = null;
            m_lightBL = null;
            m_lightBR = null;

            m_midZ = 0;
            m_minZ = 0;
            m_maxZ = 0;
                   
            //m_batteries = new List<IMyBatteryBlock>();

            m_steering   = 0;
            m_steer      = 0;
            m_propulsion = 0;

            m_count10 = 0;

            m_speedLimit = 0; // m/s
            m_reverseSpeedLimit = 30; // km/h
            m_reverseBeep = true;
            m_beepBlocks = new List<IMySoundBlock>();

            m_lightsOn     = false;
            m_farLights    = false;
            m_reversing    = false;
            m_braking      = false;
            m_emergency    = false;

            m_brakeStarted = false;
            m_prevReversing = false;

            m_cruiseControl = false;
            m_ccSpeed       = 0;
            m_ccHeading     = 0;
            m_ccPressed     = false;
            m_ccCounter     = 0;

            m_parked       = false;

            m_autoHandbrake = false;
            m_crouchPark    = true;

            m_turningLeft  = false;
            m_pressedLeft  = false;
            m_counterLeft  = 0;

            m_turningRight = false;
            m_pressedRight = false;
            m_counterRight = 0;

            m_turning   = false;
            m_pressCount = 20;

            m_dspSpeed = 0;
            m_dspPower = 0;

            m_location     = Location.NaN;
            m_lastLocation = Location.NaN;

            m_north       = new Vector3( 0, 1, 0);
            m_prime       = new Vector3(-1, 0, 0);
            m_orientation = default(Vector3);

            m_latitude  = 0;
            m_longitude = 0;
                  
            m_seaLevel    = 60000;
            m_groundLevel = 0;

            m_altitude = 0;

            m_heading = 0;

            m_linVelocity     = default(Vector3);
            m_linAcceleration = default(Vector3);
            m_lastLinVelocity = default(Vector3);

            m_angVelocity     = default(Vector3);
            m_lastAngVelocity = default(Vector3);
            m_angAcceleration = default(Vector3);
    
            //m_centBlock       = null;
            //m_fwdBlock        = null;
            //m_forwardBlockIsBehind = false;
            //m_upBlock         = null;
            //m_upBlockIsBelow  = false;

            //m_curBatteryPower = 0;
            //m_maxBatteryPower = 0;
            //m_curSolarPower   = 0;
            //m_maxSolarPower   = 0;

            //m_reactors    = new List<IMyReactor>();

            m_mass       = 0;
            m_prevMass   = 0;

            m_brakeSpeed = 0;
            m_autoLights = true;
            //m_help       = false;


            InitDisplays();
        }


        float GetTotal<T>(Func<T, float> getQuantity) where T : class
        {
            var blocks = new List<T>();
            GridTerminalSystem.GetBlocksOfType<T>(blocks);
            if (blocks.Count == 0) return 0;

            float total = 0;
            blocks.ForEach(block => total += getQuantity(block));
            return total;
        }





        Vector2 GetWheelPosition(IMyMotorSuspension w, float mult)
        {
            var pw = w.GetPosition();
            var pc = m_cockpit.GetPosition();

            return new Vector2(
                distanceToPlane(pw, pc, -m_location.Right) - mult,
                distanceToPlane(pw, pc, m_location.Forward));
        }

    }
}
