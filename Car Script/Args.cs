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
        bool ProcessArg(string arg)
        {
            arg = arg.Trim().ToLower();

            if (arg == "") 
                return false;


            if (arg == "far lights") 
            { 
                m_farLights = !m_farLights; 

                if (m_farLights)
                { 
                    foreach (var h in m_headlights)
                        h.Enabled = true;
                }
            }

            else if (arg == "emergency")   m_emergency  = !m_emergency;
            else if (arg == "auto lights") m_autoLights = !m_autoLights;
    
            else if (arg == "speed limit down")
            {
                foreach (var lw in m_lWheels) lw.SetValueFloat("Speed Limit", lw.GetValueFloat("Speed Limit") - 30);
                foreach (var rw in m_rWheels) rw.SetValueFloat("Speed Limit", rw.GetValueFloat("Speed Limit") - 30);
	        }

            else if (arg == "speed limit up")
            {
                foreach (var lw in m_lWheels) lw.SetValueFloat("Speed Limit", lw.GetValueFloat("Speed Limit") + 30);
                foreach (var rw in m_rWheels) rw.SetValueFloat("Speed Limit", rw.GetValueFloat("Speed Limit") + 30);
	        }

            else if (arg == "reset ground level")
                m_groundLevel -= m_altitude;


            return true;
        }
    }
}
