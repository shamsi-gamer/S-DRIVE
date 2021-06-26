using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.GUI.TextPanel;
using VRageMath;


namespace IngameScript
{
    partial class Program
    {
        public void Main(string arg, UpdateType update)
        {
            if (ProcessArg(arg))
                return;


            var baseMass = 
                m_cockpit != null 
                ? m_cockpit.CalculateShipMass().BaseMass 
                : 0;


            //if ((update & UpdateType.Update1) != 0)
            //    Update1(baseMass);


            if ((update & UpdateType.Update10) != 0)
                Update10(baseMass);


            if ((update & UpdateType.Update100) != 0)
                Update100();



            //Echo("S-DRIVE");
    
            //m_instCount += (this.Runtime.CurrentInstructionCount - m_instCount) * 0.01f;
            //Echo("Complexity: " + ((int)m_instCount).ToString() + "/" + this.Runtime.MaxInstructionCount.ToString());
        }



        void Update1(float baseMass)
        {
		    if (m_cockpit != null)
		    {
                m_move = m_cockpit.MoveIndicator;

                m_rotate = new Vector3(
                    m_cockpit.RotationIndicator.X,
                    m_cockpit.RotationIndicator.Y,
                    m_cockpit.RollIndicator);


                UpdateLocation();
                UpdateVelocity();


                var brake = m_move.Y > 0.1;

                m_braking = 
                        brake
                    || m_brakeStarted
                    ||    m_move.Z > 0 && -m_linVelocity.Z < 0 
                        && !(   Math.Abs(m_move.Z) < 0.1f
                            || Math.Abs(m_linVelocity.Z) < 0.1f) 
                        && Math.Abs(m_move.Z + m_linVelocity.Z) > 0.1f;


                if (!m_brakeStarted && m_braking)
                    m_brakeStarted = true;
                else if (Math.Abs(m_move.Z) < 0.1f)
                    m_brakeStarted = false;


                if (brake)
                    m_cruiseControl = false;


                if (m_cruiseControl)
                {
                    m_ccSpeed += -m_move.Z * 0.04f;
                    m_ccSpeed = Math.Max(-m_reverseSpeedLimit/3.6f, m_ccSpeed);
                    m_ccSpeed = Math.Min(m_ccSpeed, m_speedLimit/3.6f);

                    var moveZ = -(m_ccSpeed - m_linVelocity.Z) * 0.5f;

                    if (m_reversing)
                        moveZ *= 1 - (float)Math.Pow(Math.Min(Math.Max(0, m_linVelocity.Z / (-m_reverseSpeedLimit/3.6f)), 1), 2);

                    m_propulsion += (moveZ - m_propulsion) * 0.05f;

                    m_brakeSpeed = 0;
                }
                else if (m_braking)
                {   
                    if (m_brakeSpeed == 0)
                        m_brakeSpeed = -m_linVelocity.Z;

                    if (m_brakeSpeed != 0)
                    {
                        m_propulsion = 
                            brake
                            ? m_linVelocity.Z / ( 4 - Math.Min((m_mass-baseMass) / 120000 * 3, 3))
                            : m_linVelocity.Z / (12 - Math.Min((m_mass-baseMass) / 120000 * 11, 11));
                    }
                }
                else
                {
                    var moveZ = m_move.Z;

                    if (m_reversing)
                        moveZ *= 1 - (float)Math.Pow(Math.Min(Math.Max(0, m_linVelocity.Z / (-m_reverseSpeedLimit/3.6f)), 1), 2);

                    m_propulsion += (moveZ - m_propulsion) * 0.05f;

                    m_brakeSpeed = 0;
                }


                if (m_cruiseControl)
                {
                    m_ccHeading += m_move.X * 0.17f;

                    while (m_ccHeading >= 360) m_ccHeading -= 360;
                    while (m_ccHeading <    0) m_ccHeading += 360;

                    var turn = (m_ccHeading - m_heading) * 5;
                    while (turn < -180) turn += 360;
                    while (turn >  180) turn -= 360;
                    turn /= 180;

                    m_steering += (turn - m_steering) * 0.5f;
                    m_steer = Math.Min(Math.Max(-1, m_steer + (m_steering - m_steer) * 0.5f), 1);
			    }
                else
                {
                    m_steering += (m_move.X - m_steering) * 0.04f;
                    m_steer = Math.Min(Math.Max(-1, m_steer + (m_steering - m_steer) * 0.04f), 1);
			    }



                if (Math.Abs(m_steer) > 0.3f)
                    m_turning = true;
                else if (m_turning)
                {
                    m_turningLeft  = false;
                    m_turningRight = false;
                    m_turning      = false;
			    }


                if (m_rotate.Z < -0.9f)
                {
                    if (!m_pressedLeft)
                    {
                        m_pressedLeft = true;
                        m_counterLeft = m_pressCount;
                    }
                    else if (m_counterLeft > 0)
                        m_counterLeft--;
			    }
                else
                { 
                    if (m_counterLeft > 0)
                    {
                        if (m_turningRight) m_turningRight = false;
                        else                m_turningLeft  = true;
                    }

                    m_pressedLeft = false;
                    m_counterLeft = 0;
                }


                if (m_rotate.Z > 0.9f)
                {
                    if (!m_pressedRight)
                    {
                        m_pressedRight = true;
                        m_counterRight = m_pressCount;
                    }
                    else if (m_counterRight > 0)
                        m_counterRight--;
			    }
                else
                { 
                    if (m_counterRight > 0)
                    {
                        if (m_turningLeft) m_turningLeft  = false;
                        else               m_turningRight = true;
                    }

                    m_pressedRight = false;
                    m_counterRight = 0;
                }
            
            
                if (m_linVelocity.Z < -0.1f)
                { 
                    if (m_move.Z > 0.1f && !m_braking)
                        m_reversing = true;
                    else if (m_move.Z < -0.1f)
                        m_reversing = false;
                }


                if (   m_reversing 
                    && !m_prevReversing 
                    && m_reverseBeep)
                {
                    foreach (var s in m_beepBlocks)
                    {
                        s.LoopPeriod = 1800;
                        s.Play();
			        }
			    }
                else if (!m_reversing 
                        && m_prevReversing)
                {
                    foreach (var s in m_beepBlocks)
                    {
                        s.LoopPeriod = 0;
                        s.Stop();
                    }
                }


			    m_prevReversing = m_reversing;


                if (m_move.Y == -1)
                {
                    if (!m_ccPressed)
                    {
                        m_ccPressed = true;
                        m_ccCounter = m_pressCount;
                    }
                    else if (m_counterLeft > 0)
                        m_ccCounter--;
                }
                else
                {
                    if (m_ccCounter > 0)
                    {
                        m_cruiseControl = !m_cruiseControl;

                        if (m_cruiseControl)
                        {
                            m_ccSpeed = m_linVelocity.Z;
                            m_ccHeading = m_heading;
                        }
                    }

                    m_ccPressed = false;
                    m_ccCounter = 0;
                }
            }        
        }



        void Update10(float baseMass)
        {
      //      if (    m_autoHandbrake
      //          &&  m_cockpit != null
      //          && !m_cockpit.IsUnderControl)
      //          m_cockpit.HandBrake = true;


      //      UpdateWheels(baseMass);



      //      if (   m_cockpit != null
      //          && m_cockpit.IsWorking)
		    //{
      //          UpdateOrientation();
      //          UpdateLatitudeAndLongitude();
		    //}


      //      UpdateLights();

            DrawDisplays();


            m_count10++;

            m_lastMove10   = m_move;
            m_lastRotate10 = m_rotate;
        }



        void Update100()
        {
            var cockpits = new List<IMyCockpit>();
            GridTerminalSystem.GetBlocksOfType(cockpits);

            if (cockpits.Count > 0)
                m_cockpit = cockpits[0];

            if (m_cockpit != null)
            { 
                m_cockpit.ControlWheels    = false;
                m_cockpit.ControlThrusters = false;


                m_prevMass = m_mass;

                m_mass = 1000;
                    //  m_cargo1mass 
                    //+ m_cargo2mass 
                    //+ m_cargo3mass 
                    //+ m_rearMass;


                if (m_display1 == null || m_display1.Provider != m_cockpit) m_display1 = new Display(m_cockpit, 0); 
                if (m_display2 == null || m_display2.Provider != m_cockpit) m_display2 = new Display(m_cockpit, 2); 


                InitWheels();
		    }

            //m_centBlock = GridTerminalSystem.GetBlockWithName("Gyroscope") as IMyGyro;
            //m_fwdBlock  = GridTerminalSystem.GetBlockWithName("Antenna") as IMyRadioAntenna;
            //m_upBlock   = GridTerminalSystem.GetBlockWithName("Collector") as IMyCollector;

            //m_headlights.Clear();
            //GridTerminalSystem.GetBlocksOfType(m_headlights);

            //if (m_autoLights)
            //{
            //    foreach (var h in m_headlights)
            //        h.Enabled = m_curSolarPower < 0.002;
            //}


            //m_lightFL = GridTerminalSystem.GetBlockWithName("Turn Light L") as IMyInteriorLight;
            //m_lightFR = GridTerminalSystem.GetBlockWithName("Turn Light R") as IMyInteriorLight;
            //m_lightBL = GridTerminalSystem.GetBlockWithName("Rear Light L") as IMyInteriorLight;
            //m_lightBR = GridTerminalSystem.GetBlockWithName("Rear Light R") as IMyInteriorLight;

            //m_batteries.Clear();
            //GridTerminalSystem.GetBlocksOfType(m_batteries);

            //m_curBatteryPower = GetTotal<IMyBatteryBlock>(b => b.CustomName != "Starter Battery" ? b.CurrentStoredPower : 0);
            //m_maxBatteryPower = GetTotal<IMyBatteryBlock>(b => b.CustomName != "Starter Battery" ? b.MaxStoredPower     : 0);

            //m_curSolarPower += (GetTotal<IMySolarPanel>(p => p.CurrentOutput) - m_curSolarPower) * 0.45f;
            //m_maxSolarPower  = GetTotal<IMySolarPanel>(p => 0.13f);


            GridTerminalSystem.GetBlocksOfType(m_beepBlocks, b => b.CustomName.ToLower().Contains("reverse"));

            //GridTerminalSystem.GetBlocksOfType(m_reactors);
        }
    }
}
