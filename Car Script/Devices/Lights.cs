using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        void UpdateLights()
        {
            UpdateFrontLights();
            UpdateRearLights();
        }



        void UpdateFrontLights()
        {
            UpdateFrontLeftLights();
            UpdateFrontRightLights();


            //m_lightsOn = true;
            //foreach (var h in m_headlights) 
            //    if (!h.Enabled) { m_lightsOn = false; break; }


            //foreach (var h in m_headlights)
            //    h.SetValueFloat("Intensity", m_farLights ? 5f : 1.5f);
        }



        void UpdateFrontLeftLights()
        { 
            var blink10 = 1/dt10;

            if (m_lightFL != null) 
            {
                m_lightFL.BlinkIntervalSeconds = 10;
                m_lightFL.Enabled = m_turningLeft || m_emergency;
                m_lightFL.BlinkLength = (m_count10 - 1) % blink10 < blink10/2 ? 100 : 0; 
            }
        }



        void UpdateFrontRightLights()
        { 
            var blink10 = 1/dt10;

            if (m_lightFR != null) 
            {
                m_lightFR.BlinkIntervalSeconds = 10;
                m_lightFR.Enabled = m_turningRight || m_emergency;
                m_lightFR.BlinkLength = (m_count10 - 1) % blink10 < blink10/2 ? 100 : 0; 
            }
        }



        void UpdateRearLights()
        {
            UpdateRearLeftLights();
            UpdateRearRightLights();
        }



        void UpdateRearLeftLights()
        { 
            var blink10 = 1 / dt10;

            var colBrake = new Color(0xff, 0, 0);
            var colTurn  = new Color(0xff, 0x88, 0);
            var colRev   = Color.White;


            if (m_lightBL != null) 
            {
                if (m_braking
                    ||    (m_cockpit == null || m_cockpit.HandBrake)
                        && !m_emergency)
                {
                    m_lightBL.Enabled              = true;
                    m_lightBL.Color                = colBrake;
                    m_lightBL.BlinkIntervalSeconds = 0;
                    m_lightBL.BlinkLength          = 0;
                    m_lightBL.SetValueFloat("Intensity", 7);
                    m_lightBL.SetValueFloat("Radius", 5);
			    }
                else if (m_emergency)
                {
                    m_lightBL.Enabled              = true;
                    m_lightBL.Color                = colTurn;
                    m_lightBL.BlinkIntervalSeconds = 10;
                    m_lightBL.BlinkLength          = (m_count10 - 1) % blink10 < blink10/2 ? 100 : 0;
                    m_lightBL.SetValueFloat("Intensity", 5);
                    m_lightBL.SetValueFloat("Radius", 4);
			    }
                else if (m_reversing)
                {
                    m_lightBL.Enabled              = true;
                    m_lightBL.Color                = colRev;
                    m_lightBL.BlinkIntervalSeconds = 0;
                    m_lightBL.BlinkLength          = 0;
                    m_lightBL.SetValueFloat("Intensity", 5);
                    m_lightBL.SetValueFloat("Radius", 4);
			    }
                else if (m_turningLeft)
                {
                    m_lightBL.Enabled              = true;
                    m_lightBL.Color                = colTurn;
                    m_lightBL.BlinkIntervalSeconds = 10;
                    m_lightBL.BlinkLength          = (m_count10 - 1) % blink10 < blink10/2 ? 100 : 0;
                    m_lightBL.SetValueFloat("Intensity", 5);
                    m_lightBL.SetValueFloat("Radius", 2);
			    }
                else
                {
                    m_lightBL.Enabled = m_lightsOn;

                    if (m_lightsOn)
                        m_lightBL.Color = colBrake;

                    m_lightBL.BlinkIntervalSeconds = 0;
                    m_lightBL.BlinkLength          = 0;
                    m_lightBL.SetValueFloat("Intensity", 2);
                    m_lightBL.SetValueFloat("Radius", 1.5f);
			    }
            }
        }



        void UpdateRearRightLights()
        { 
            var blink10 = 1 / dt10;

            var colBrake = new Color(0xff, 0, 0);
            var colTurn  = new Color(0xff, 0x88, 0);
            var colRev   = Color.White;


            if (m_lightBR != null) 
            {
                if (m_braking
                    ||    (m_cockpit == null || m_cockpit.HandBrake)
                        && !m_emergency)
                {
                    m_lightBR.Enabled              = true;
                    m_lightBR.Color                = colBrake;
                    m_lightBR.BlinkIntervalSeconds = 0;
                    m_lightBR.BlinkLength          = 0;
                    m_lightBR.SetValueFloat("Intensity", 7);
                    m_lightBR.SetValueFloat("Radius", 5);
			    }
                else if (m_emergency)
                {
                    m_lightBR.Enabled              = true;
                    m_lightBR.Color                = colTurn;
                    m_lightBR.BlinkIntervalSeconds = 10;
                    m_lightBR.BlinkLength          = (m_count10 - 1) % blink10 < blink10/2 ? 100 : 0;
                    m_lightBR.SetValueFloat("Intensity", 5);
                    m_lightBR.SetValueFloat("Radius", 4);
			    }
                else if (m_reversing)
                {
                    m_lightBR.Enabled              = true;
                    m_lightBR.Color                = colRev;
                    m_lightBR.BlinkIntervalSeconds = 0;
                    m_lightBR.BlinkLength          = 0;
                    m_lightBR.SetValueFloat("Intensity", 5);
                    m_lightBR.SetValueFloat("Radius", 4);
			    }
                else if (m_turningRight)
                {
                    m_lightBR.Enabled              = true;
                    m_lightBR.Color                = colTurn;
                    m_lightBR.BlinkIntervalSeconds = 10;
                    m_lightBR.BlinkLength          = (m_count10 - 1) % blink10 < blink10/2 ? 100 : 0;
                    m_lightBR.SetValueFloat("Intensity", 5);
                    m_lightBR.SetValueFloat("Radius", 2);
			    }
                else
                {
                    m_lightBR.Enabled = m_lightsOn;

                    if (m_lightsOn)
                        m_lightBR.Color = colBrake;

                    m_lightBR.BlinkIntervalSeconds = 0;
                    m_lightBR.BlinkLength          = 0;
                    m_lightBR.SetValueFloat("Intensity", 2);
                    m_lightBR.SetValueFloat("Radius", 1.5f);
			    }
            }
        }
    }
}
