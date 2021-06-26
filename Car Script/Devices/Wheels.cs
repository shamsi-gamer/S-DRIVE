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
        void InitWheels()
        {
            GridTerminalSystem.GetBlocksOfType(m_lWheels, w => w.Position.X < m_cockpit.Position.X);
            GridTerminalSystem.GetBlocksOfType(m_rWheels, w => w.Position.X > m_cockpit.Position.X);


            var nWheels = Math.Max(m_lWheels.Count, m_rWheels.Count);

            if (nWheels != m_susp.Count)
            { 
                var defSusp = 5f;

                m_susp.Clear();
                for (int i = 0; i < nWheels; i++)
                    m_susp.Add(defSusp);
            }


            m_lWheels = m_lWheels.OrderBy(w => -w.Position.Z).ToList();
            m_rWheels = m_rWheels.OrderBy(w => -w.Position.Z).ToList();

            m_midZ = 0;

            foreach (var lw in m_lWheels) m_midZ += lw.Position.Z;
            foreach (var rw in m_lWheels) m_midZ += rw.Position.Z;

            if (   m_lWheels.Count > 0 
                || m_rWheels.Count > 0)
                m_midZ /= m_lWheels.Count + m_rWheels.Count;

            m_minZ = m_midZ;
            m_maxZ = m_midZ;

            m_flw = null;
            m_frw = null;
            m_rlw = null;
            m_rrw = null;

            foreach (var lw in m_lWheels) 
            { 
                m_minZ = Math.Min(m_minZ, lw.Position.Z); 
                m_maxZ = Math.Max(m_maxZ, lw.Position.Z); 

                if (lw.Position.Z == m_maxZ) m_flw = lw; 
                if (lw.Position.Z == m_minZ) m_rlw = lw; 
            }

            foreach (var rw in m_rWheels) 
            { 
                m_minZ = Math.Min(m_minZ, rw.Position.Z); 
                m_maxZ = Math.Max(m_maxZ, rw.Position.Z); 

                if (rw.Position.Z == m_maxZ) m_frw = rw; 
                if (rw.Position.Z == m_minZ) m_rrw = rw; 
            }

			var _lw = GridTerminalSystem.GetBlockWithName("Wheel L1") as IMyMotorSuspension;
            var _rw = GridTerminalSystem.GetBlockWithName("Wheel R1") as IMyMotorSuspension;

        //      if (    m_cockpit.IsUnderControl
        //          && !m_kneelFromInside
        //          && _lw != null
        //          && _rw != null 
        //          &&  m_kneel)
        //      {
        //          m_kneel = false;
		    //}
            
        //      if (!m_cockpit.IsUnderControl)
        //          m_kneelFromInside = false;
        }



        void UpdateWheels(float baseMass)
        {
            var uphill = 6;

            var dk = 0.1f / (1 + 3 * (m_mass - baseMass) / 120000);

            //if (m_kneel) m_kneelState = Math.Max(0, m_kneelState - dk);
            //else         m_kneelState = Math.Min(m_kneelState + dk, 1);


            if (m_cockpit != null)
            {
                if (m_cockpit.HandBrake)
                    m_cruiseControl = false;


                if (   m_cockpit.HandBrake
                    && Math.Abs(m_linVelocity.Z) < 1
                    && m_crouchPark) // park
                {
                    var ck = 1;// / (1 + (m_mass - baseMass) / 120000);
                    
                    m_parked = true;

                    for (int i = 0; i < m_susp.Count; i++)
                    { 
                        m_susp[i] = Math.Max(0, m_susp[i] - ck);
                        if (m_susp[i] > 0) m_parked = false;
                    }
		        }
                else if (m_susp.Count == 4) // moving
                {
                    var defSusp = 5.5f;

                    var susp0 = defSusp;//(defSusp + 8 * m_cargo1mass / 40000) * m_kneelState;
                    var susp1 = defSusp;//(defSusp + 8 * m_cargo2mass / 40000) * (0.5f + m_kneelState/2);
                    var susp2 = defSusp;// defSusp + 8 * m_cargo3mass / 40000;
                    var susp3 = defSusp;// defSusp + 1 + 8 * m_rearMass / 40000;

                    if (m_parked)
                    {
                        var ck = 1;// / (1 + (m_mass - baseMass) / 120000);

                        m_susp[0] = Math.Min(m_susp[0] + ck, susp0);
                        m_susp[1] = Math.Min(m_susp[1] + ck, susp1);
                        m_susp[2] = Math.Min(m_susp[2] + ck, susp2);
                        m_susp[3] = Math.Min(m_susp[3] + ck, susp3);

                        if (   m_susp[0] > susp0 - 0.5f
                            && m_susp[1] > susp1 - 0.5f
                            && m_susp[2] > susp2 - 0.5f
                            && m_susp[3] > susp3 - 0.5f)
                            m_parked = false;
				    }
                    else
                    { 
                        m_susp[0] = susp0;
                        m_susp[1] = susp1;
                        m_susp[2] = susp2;
                        m_susp[3] = susp3;
                    }
                }
            }


            for (int i = 0; i < m_lWheels.Count; i++)
            {
                var lw = m_lWheels[i];

                lw.Power = Math.Min(
                        30 
                    + 70 * Math.Abs(m_linVelocity.Z)/100
                    + 40 * m_orientation.X*uphill 
                    + 40 * (m_mass-baseMass) / 120000,
                    100); 
            
                lw.SetValueFloat("Propulsion override", m_propulsion * lw.Power/200);
                if (m_susp.Count == m_lWheels.Count) lw.Strength = m_susp[i];
		    }

            for (int i = 0; i < m_rWheels.Count; i++)
            {
                var rw = m_rWheels[i];

                rw.Power = Math.Min(
                        30 
                    + 70 * Math.Abs(m_linVelocity.Z)/100
                    + 40 * m_orientation.X*uphill 
                    + 40 * (m_mass-baseMass) / 120000,
                    100); 

                rw.SetValueFloat("Propulsion override", -m_propulsion * rw.Power/200);
                if (m_susp.Count == m_rWheels.Count) rw.Strength = m_susp[i];
            }


            var diff = 1f;

            if (   m_flw != null
                && m_frw != null
                && m_rlw != null
                && m_rrw != null)
            {
                if (m_steer < 0)
                {
                    var pfl = GetWheelPosition(m_flw, -1);
                    var prl = GetWheelPosition(m_rlw, -1);

                    var fla = Math.Max(-1, m_steer) * m_flw.MaxSteerAngle - Tau/2;
                    var rla = -Tau/2;

				    var vfl = vector2(fla, 1);
                    var vrl = vector2(rla, 1);

                    var pt  = intersect(pfl, vfl, prl, vrl);

                    if (!Vector2_IsNaN(pt))
                    {
                        foreach (var lw in m_lWheels) 
                        {
                            var a = angle(GetWheelPosition(lw, -1) - pt);
                            lw.SetValueFloat("Steer override", a / m_flw.MaxSteerAngle); 
                        }
                        foreach (var rw in m_rWheels) 
                        {
                            var a = angle(GetWheelPosition(rw, 1) - pt);
                            rw.SetValueFloat("Steer override", a / m_flw.MaxSteerAngle); 
                        }


                        var pfr = GetWheelPosition(m_frw, 1);

                        diff = Vector2.Distance(pt, pfl) / Vector2.Distance(pt, pfr);

                        foreach (var lw in m_lWheels) lw.Power *= diff;
                        foreach (var rw in m_rWheels) rw.Friction = diff * 100;
				    }
		        }
                else if (m_steer > 0)
                {
                    var pfr = GetWheelPosition(m_frw, 1);
                    var prr = GetWheelPosition(m_rrw, 1);

                    var fra = m_steer * m_frw.MaxSteerAngle;
                    var rra = 0;

                    var vfr = vector2(fra, 1);
                    var vrr = vector2(rra, 1);

                    var pt  = intersect(pfr, vfr, prr, vrr);
                
                    if (!Vector2_IsNaN(pt))
                    {
					    foreach (var rw in m_rWheels)
					    {
						    var a = angle(GetWheelPosition(rw, 1) - pt) + Tau / 2;
						    if (a > Tau / 2) a -= Tau;
						    rw.SetValueFloat("Steer override", a / m_frw.MaxSteerAngle);
					    }
					    foreach (var lw in m_lWheels) 
                        {
                            var a = angle(GetWheelPosition(lw, -1) - pt) + Tau/2;
                            if (a > Tau/2) a -= Tau;
                            lw.SetValueFloat("Steer override", a / m_frw.MaxSteerAngle); 
                        }


                        var pfl = GetWheelPosition(m_flw, -1);

                        diff = Vector2.Distance(pt, pfr) / Vector2.Distance(pt, pfl);

                        foreach (var rw in m_rWheels) rw.Power *= diff;
                        foreach (var lw in m_lWheels) lw.Friction = diff * 100;
				    }
		        }
            }
            else
            {
                foreach (var lw in m_lWheels) lw.SetValueFloat("Steer override", m_steering * (lw.Position.Z < m_midZ ? -1 : 1) * (lw.Position.Z - m_maxZ) / (m_minZ - m_maxZ));
                foreach (var rw in m_rWheels) rw.SetValueFloat("Steer override", m_steering * (rw.Position.Z < m_midZ ? -1 : 1) * (rw.Position.Z - m_maxZ) / (m_minZ - m_maxZ));
		    }
        }
    }
}
