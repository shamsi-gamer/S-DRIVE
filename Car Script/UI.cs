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
        static Color colGauge  = new Color(0xa0, 0xa0, 0xa0);
        static Color colRim    = new Color(0x14, 0x11, 0x0c);
        static Color colHigh   = new Color(0x66, 0x5c, 0x55);
        static Color colBack   = new Color(0x22, 0x22, 0x22);
        static Color colText   = Color.Black;
        //static Color colShadow = new Color(0x22, 0x22, 0x22);
        static Color colLabel   = new Color(0x55, 0x44, 0x33);
        static Color colWarning = new Color(0x44, 0, 0);
        static Color colShadow  = new Color(0x00, 0x00, 0x00, 0x40);
        static Color colArrow   = new Color(0x88, 0x33, 0x00);



        void InitDisplays()
        {
            m_displays = new List<Display>();

            m_displays.Add(m_display1 = new Display(m_cockpit, 0));
            m_displays.Add(m_display2 = new Display(m_cockpit, 2));

            InitDisplay1();
            InitDisplay2();
        }



        void InitDisplay1()
        {   
            var x = m_display1.Viewport.X;
            var y = m_display1.Viewport.Y;
            var w = m_display1.Viewport.Width;
            var h = m_display1.Viewport.Height;

            var szSpeed = Math.Min(w, h)*1/3;

            m_display1.Controls.Add(new Gauge(
                w/4-szSpeed/2, 
                h/4-szSpeed/2, 
                szSpeed, 
                szSpeed,
                0, 
                0));

            m_display1.Controls.Add(new Gauge(
                (w-szSpeed)/2, 
                (h-szSpeed)/2, 
                szSpeed, 
                szSpeed,
                0,
                Tau/2));

            m_display1.Controls.Add(new Gauge(
                w*3/4f-szSpeed/2, 
                h*3/4f-szSpeed/2, 
                szSpeed, 
                szSpeed,
                Tau/2,
                Tau));
        }



        void InitDisplay2()
        {   
            var x = m_display1.Viewport.X;
            var y = m_display1.Viewport.Y;
            var w = m_display1.Viewport.Width;
            var h = m_display1.Viewport.Height;
        }



        void DrawDisplays()
        {
            foreach (var d in m_displays)
            { 
                d.Update();
                d.Draw(this);
            }
        }



        //void DrawDisplay1()
        //{
        //    var sprites = new List<MySprite>();

        //    var v = m_display1.Viewport;

        //    var x = v.X;
        //    var y = v.Y;
        //    var w = v.Width;
        //    var h = v.Height;

        //    FillRect(ref frame, x, y, w, h, colBack);


        //    //DrawHalfGauge(ref frame, false, "Speed", w*2/3f, h*0.7f, h/3, 0, 40, 30, 10, 5, 0, new Color(0x66, 0x11, 0x00));


        //    //DrawTachometer(sprites);
        //    //DrawSpeedometer(sprites);

        //    //DrawFuelGauge(sprites);

        //    //DrawDriveModeLight(sprites); // P/R/N/D

        //    //DrawCruiseControl(sprites);

        //    //DrawParkingBrakeLight(sprites);
        //    //DrawEmergencyLight(sprites);

        //    //DrawLeftTurnLight(sprites);
        //    //DrawRightTurnLight(sprites);

        //    m_display1.Draw(sprites);
        //}



   //     void DrawDisplay2()
   //     {
   //         var x2 = m_display2.Viewport.Center.X - 80;
			//var y2 = 0;

			//var line2  = 0;
			//var step2  = 16;

   //         var scale2 = 0.5f;


   //         var sprites = new List<MySprite>();

   //         //DrawRollGauge(sprites);
   //         //DrawPitchGauge(sprites);

   //         //DrawCompass(sprites);

   //         //DrawVehicleCoordinates(sprites);
   //         //DrawVehicleAltitude(sprites);

   //         m_display2.Draw(sprites);
   //     }



        void DrawHalfGauge(ref MySpriteDrawFrame frame, bool right, string label, float x, float y, float r, float valMin, float valMax, float valDanger, float valStep, float tickStep, float curVal, Color colArrow, float startAngleGap = Tau / 72, float endAngleGap = Tau / 72, bool fillEdge = true)
        {
            var angleSide = right ? -1 : 1;

            var so = 4f;
            var hr = r/10f;
            var aw = r/15f;

            var fa = (curVal - valMin) / (valMax - valMin);
            var va = vector2(1/4f * Tau + angleSide * startAngleGap + angleSide * fa * (Tau/2 - startAngleGap - endAngleGap), r * 0.835f);

            ClipDraw(
                ref frame,
                (int)(x - (right ? hr : r)),
                (int)(y - r - so),
                (int)(hr + r),
                (int)(r * 2 + so * 2));

            // back
            FillCircle(ref frame, x, y - so/2, r, colRim);
            FillCircle(ref frame, x, y + so/2, r, colHigh);
            FillCircle(ref frame, x, y,        r, colGauge);

            // danger zone
            if (!float.IsNaN(valDanger))
            { 
                var fd1  = (valDanger - valMin) / (valMax - valMin);
                var vd1 = vector2(1/4f * Tau + (angleSide > 0 ? Tau/2 : 0) + angleSide * startAngleGap + angleSide * fd1 * (Tau/2 - startAngleGap - endAngleGap), r * 9/10f);

                var fd2 = (valMax - valMin) / (valMax - valMin);
                var vd2 = vector2(1/4f * Tau + (angleSide > 0 ? Tau/2 : 0) + angleSide * startAngleGap + angleSide * fd2 * (Tau/2 - startAngleGap - endAngleGap), r * 9/10f);

                var sf = 0.71f;
                var ef = 0.76f;

                FillSemiCircle(ref frame, x, y, r*ef, new Color(0x55, 0x08, 0x08), angle(vd1));
                FillSemiCircle(ref frame, x, y, r*ef, colGauge, angle(vd2));

                FillCircle(ref frame, x, y, r*sf, colGauge);
            }

            // edge cleanup
            FillRect(ref frame, x - (right ? (hr + 1) : 0), y - r, hr + 1, r * 2, fillEdge ? colGauge : colBack);

            // shadows
            DrawLine(ref frame, x, y + so, x + va.X, y + va.Y + so, colShadow, aw);
            FillCircle(ref frame, x, y + so, hr, colShadow);

            // numbers
            for (float val = valMin; val <= valMax; val += valStep)
            {
                var f = (val - valMin) / (valMax - valMin);
                var v = vector2(1/4f * Tau + angleSide * startAngleGap + angleSide * f * (Tau / 2 - startAngleGap - endAngleGap), r * 9/10f);

                DrawString(ref frame, printValue(val, 0, true, 0), x + v.X, y + v.Y - 8*m_display1.ContentScale, r / 270, colText, TextAlignment.CENTER);
            }

            // ticks
            for (float val = valMin; val <= valMax; val += tickStep)
            {
                var f = (val - valMin) / (valMax - valMin);
                var v = vector2(1/4f * Tau + angleSide * startAngleGap + angleSide * f * (Tau/2 - startAngleGap - endAngleGap), r * 9/10f);

                var sf = 0.79f;
                var ef = 0.89f;

                DrawLine(ref frame, x + v.X * sf, y + v.Y * sf, x + v.X * ef, y + v.Y * ef, colText, r/50);
            }

            DrawString(ref frame, label, x - angleSide * r * 0.275f, y + r/4, r/200, colText, TextAlignment.CENTER);

            // arrow
            DrawLine(ref frame, x, y, x + va.X, y + va.Y, colArrow, aw);
            FillCircle(ref frame, x, y, hr, colArrow);

            ClearClip(ref frame);
        }



        void DrawRoundLight(ref MySpriteDrawFrame frame, float x, float y, float r)
        {
            var so = 2f;

            FillCircle(ref frame, x, y - so, r, colRim);
            FillCircle(ref frame, x, y + so, r, colHigh);
            FillCircle(ref frame, x, y, r, Color.Black);
        }
        
        
        
        void DrawRoundLight2(ref MySpriteDrawFrame frame, float x, float y, float r)
        {
            var so = 1f;

            FillCircle(ref frame, x, y - so, r, colRim);
            FillCircle(ref frame, x, y + so, r, new Color(0x88, 0x88, 0x88));
            FillCircle(ref frame, x, y, r, Color.Black);
        }



        void DrawExtLight(ref MySpriteDrawFrame frame, float x, float y, float r)
        {
            DrawRoundLight(ref frame, x, y, r);

            var col =
                true //extLightsA[0].Enabled
                ? new Color(0x00, 0x22, 0xFF)
                : new Color(0x00, 0x01, 0x08);

            FillSemiCircle(ref frame, x - r * 0.7f, y - r * 0.6f - 1, r, r * 1.1f, col, -Tau/4);
            FillRect(ref frame, x, y - r * 0.6f - 1, r * 0.6f, r * 0.2f, col);
            FillRect(ref frame, x, y - r * 0.2f - 1, r * 0.6f, r * 0.2f, col);
            FillRect(ref frame, x, y + r * 0.2f - 1, r * 0.6f, r * 0.2f, col);
        }



        void DrawIntLight(ref MySpriteDrawFrame frame, float x, float y, float r)
        {
            DrawRoundLight2(ref frame, x, y, r);

            var col =
                true //intLightsA[0].Enabled
                ? new Color(0xFF, 0x44, 0x00)
                : new Color(0x0c, 0x03, 0x00);

            FillRect(ref frame, x - r * 0.5f, y - 1 - r*0.4f, r * 1f, r * 0.2f, col);

            FillRect(ref frame, x - r * 0.1f, y - 1, r * 0.2f, r * 0.6f, col);
            FillRect(ref frame, x - r * 0.5f, y - 1, r * 0.2f, r * 0.6f, col);
            FillRect(ref frame, x + r * 0.3f, y - 1, r * 0.2f, r * 0.6f, col);
        }



        void DrawParkingLight(ref MySpriteDrawFrame frame, float x, float y, float r)
        {
            DrawRoundLight(ref frame, x, y, r);

            var col =
                true //seat.HandBrake
                ? new Color(0x00, 0xFF, 0x00)
                : new Color(0x00, 0x08, 0x00);

            ClipDraw(
                ref frame,
                (int)(x - r),
                (int)(y - r * 0.6f),
                (int)(r * 2),
                (int)(r * 1.2f));
            
            DrawCircle(ref frame, x, y, r * 0.75f, col);
            ClearClip(ref frame);

            DrawCircle(ref frame, x, y, r * 0.55f, col);

            DrawString(ref frame, "P", x + r * 0.05f, y - r * 0.4f, r/40f, col, TextAlignment.CENTER);
        }



        //void DrawBatteryLight(ref MySpriteDrawFrame frame, float x, float y, float w, float h)
        //{
        //    var so = 2f;

        //    FillRect(ref frame, x, y - so, w, h, colRim);
        //    FillRect(ref frame, x, y + so, w, h, colRim);
        //    FillRect(ref frame, x, y, w, h, Color.Black);

        //    //var nCharging = 0;
        //    var batCur = 0f;
        //    var batMax = 0f;

        //    var batCurOut = 0f;
        //    var batMaxOut = 0f;

        //    //foreach (var b in batteries)
        //    //{
        //    //    batCur += b.CurrentStoredPower;
        //    //    batMax += b.MaxStoredPower;

        //    //    batCurOut += b.CurrentOutput;
        //    //    batMaxOut += b.MaxOutput;
        //    //}

        //    var using_  = batCurOut / batMaxOut;
        //    var charged = batCur / batMax;

        //    var clbar = 
        //        charged > 0.2f
        //        ? new Color(0x55, 0x4c, 0)
        //        : new Color(0x55, 0,    0);

        //    var cloff = 
        //        charged > 0.2f
        //        ? new Color(0x0c, 0x08, 0)
        //        : new Color(0x0c, 0,    0);

        //    FillRect(ref frame, x, y, w, h, cloff));
        //    FillRect(ref frame, x, y + h, w/2, -h * (float)Math.Pow(using_, 0.2f), clbar));
        //    FillRect(ref frame, x + w/2 + 2, y + h, w/2 - 2, -h * charged, clbar));
        //}



   //     void DrawTachometer(ref MySpriteDrawFrame frame)
   //     {
   //         var tSize = 80;

			//var xp = m_display1.Viewport.Center.X + 42 - 95;
			//var yp = m_display1.Viewport.Center.Y - 50;

   //         FillCircle    (ref frame, xp - tSize/2,       yp - tSize/2,       tSize,       colGauge);
   //         FillSemiCircle(ref frame, xp - tSize/2*0.98f, yp - tSize/2*0.98f, tSize*0.98f, colWarning, -Tau/4);
   //         FillSemiCircle(ref frame, xp - tSize/2,       yp - tSize/2,       tSize,       colGauge,   -Tau/4 - Tau/10);
   //         FillCircle    (ref frame, xp - tSize/2*0.74f, yp - tSize/2*0.74f, tSize*0.74f, colGauge);

   //         FillCircle(ref frame, xp - 1.8f*tSize/2 + 88, yp - 1.8f*tSize/2, 1.8f*tSize, Color.Black);

			//DrawString(ref frame, "%", xp - 18, yp - 9, 0.5f, colLabel, TextAlignment.CENTER);

   //         for (float p = 0; p <= 100; p += 20)
   //         {
   //             var v = vector2(Tau*1/4f + p/100 * Tau/2, tSize/2 + 5);
   // 			DrawString(ref frame, printValue(p, 0, true, 0), xp + v.X*1.02f, yp + v.Y*1.02f * 1.1f - 5, 0.35f, Color.Gray, p < 20 || p > 80 ? TextAlignment.CENTER : TextAlignment.RIGHT);
   // 			DrawLine(ref frame, xp + v.X*0.7f, yp + v.Y*0.7f, xp + v.X*0.89f, yp + v.Y*0.89f, Color.Gray, 3);
			//}


   //         var avgPower = 0f;

   //         foreach (var lw in m_lWheels) avgPower += lw.Power;
   //         foreach (var rw in m_rWheels) avgPower += rw.Power;

   //         if (   m_lWheels.Count > 0
   //             || m_rWheels.Count > 0)
   //             avgPower /= m_lWheels.Count + m_rWheels.Count;

   //         var propulsion = m_propulsion * avgPower/100;


   //         m_dspPower += 
   //             Math.Sign(propulsion) == Math.Sign(-m_linVelocity.Z) 
   //             ? (propulsion - m_dspPower) * 0.4f
   //             : -m_dspPower * 0.4f;


   //         var vp = vector2(Tau*1/4f + (float)Math.Min(Math.Abs(m_dspPower), 1) * Tau/2, tSize/2);

   // 		DrawLine  (ref frame, xp, yp + 4, xp + vp.X, yp + vp.Y + 4, colShadow, 9);
   //         FillCircle(ref frame, xp - 7, yp - 7 + 4, 14, colShadow);
   // 		DrawLine  (ref frame, xp, yp, xp + vp.X, yp + vp.Y, colArrow, 5);
   //         FillCircle(ref frame, xp - 5, yp - 5, 10, colArrow);
   //     }


        
   //     void DrawSpeedometer(ref MySpriteDrawFrame frame)
   //     {
   //         var sSize = 100;

			//var xs = m_display1.Viewport.Center.X + 39;
			//var ys = m_display1.Viewport.Center.Y - 50;

   //         FillCircle(ref frame, xs - sSize/2, ys - sSize/2, sSize, colGauge);

			//DrawString(ref frame, "km/h", xs, ys + 25, 0.45f, colLabel, TextAlignment.CENTER);


   //         m_speedLimit = 0f;

   //         foreach (var lw in m_lWheels) m_speedLimit += lw.GetValueFloat("Speed Limit");
   //         foreach (var rw in m_rWheels) m_speedLimit += rw.GetValueFloat("Speed Limit");

   //         if (   m_lWheels.Count > 0
   //             || m_rWheels.Count > 0)
   //             m_speedLimit /= m_lWheels.Count + m_rWheels.Count;
            

   //         for (float f = 0; f <= 360; f += 30)
   //         {
   //             var v = vector2(Tau*3/8f + f/360 * Tau*3/4f, sSize/2 + 5);
   //             var c = f <= m_speedLimit ? Color.Gray : new Color(0x08, 0x08, 0x08);
   // 			DrawString(ref frame, printValue(f, 0, true, 0), xs + v.X, ys + v.Y * 1.1f - 5, 0.35f, c, f < 180 ? TextAlignment.RIGHT : (f > 180 ? TextAlignment.LEFT : TextAlignment.CENTER));
   // 			DrawLine(ref frame, xs + v.X*0.73f, ys + v.Y*0.73f, xs + v.X*0.895f, ys + v.Y*0.895f, c, 2.5f);
			//}

   //         m_dspSpeed += (m_linVelocity.Z - m_dspSpeed) * 0.5f;

   //         var va = vector2(Tau*3/8f + (float)Math.Abs(m_dspSpeed/100) * Tau*3/4f, sSize/2);
   //         DrawLine(ref frame, xs, ys + 4, xs + va.X, ys + va.Y + 4, colShadow, 9);
   //         FillCircle(ref frame, xs - 7, ys - 7 + 4, 14, colShadow);
   //         DrawLine(ref frame, xs, ys, xs + va.X, ys + va.Y, colArrow, 5);
   //         FillCircle(ref frame, xs - 5, ys - 5, 10, colArrow);
   //     }



   //     void DrawFuelGauge()
   //     {
   //         var fSize = 50;

			//var xf = 38;
			//var yf = m_display0.Viewport.Center.Y + 84;

   //         m_display0.Add(ref frame0, DrawTexture("Circle", xf - fSize/2, yf - fSize/2, fSize, fSize, colGauge));
   //         m_display0.Add(ref frame0, DrawTexture("SemiCircle", xf - fSize/2*0.95f, yf - fSize/2*0.95f, fSize*0.95f, fSize*0.95f, colWarning, Tau/2 + Tau*3/8f));
   //         m_display0.Add(ref frame0, DrawTexture("SemiCircle", xf - fSize/2, yf - fSize/2, fSize, fSize, colGauge, Tau/2 + Tau*3/8f + Tau*3/16));
   //         m_display0.Add(ref frame0, DrawTexture("Circle", xf - fSize/2*0.7f, yf - fSize/2*0.7f, fSize*0.7f, fSize*0.7f, colGauge));
   //         Echo("2.25");

   //         m_display0.Add(ref frame0, DrawTexture("IconEnergy", xf-8, yf-8 + 16, 16, 16, colLabel));

   //         for (float f = 0; f <= 1.000001; f += 0.25f)
   //         {
   //             var v = vector2(Tau*3/8f + f * Tau*3/4f, fSize/2 + 5);
   // 			m_display0.Add(ref frame0, DrawLine(xf + v.X*0.6, yf + v.Y*0.6f, xf + v.X*0.8f, yf + v.Y*0.8f, Color.Gray, 2));
			//}
   //         Echo("2.26");

   //         var ve = vector2(Tau*3/8f, fSize/2 + 5);
   //			m_display0.Add(ref frame0, DrawString("E", xf + ve.X*1.1f, yf + ve.Y*1.1f, 0.35f, Color.Gray, TextAlignment.CENTER));

   //         var vf = vector2(Tau*3/8f + Tau*3/4f, fSize/2 + 5);
   //			m_display0.Add(ref frame0, DrawString("F", xf + vf.X*1.1f, yf + vf.Y*1.1f, 0.35f, Color.Gray, TextAlignment.CENTER));
   //     }



   //     //void DrawBatteryLight()
   //     //{
   //         //      var bp = m_curBatteryPower / m_maxBatteryPower;
   //         //      var vpf = vector2(Tau*3/8f + bp * Tau*3/4f, fSize/2);
   //         //m_display0.Add(ref frame0, DrawLine(xf, yf + 4, xf + vpf.X, yf + vpf.Y + 4, colShadow, 9));
   //         //      m_display0.Add(ref frame0, DrawTexture("Circle", xf - 5, yf - 5 + 4, 14, 14, colShadow));
   //         //m_display0.Add(ref frame0, DrawLine(xf, yf, xf + vpf.X, yf + vpf.Y, colArrow, 3));
   //         //      m_display0.Add(ref frame0, DrawTexture("Circle", xf - 4, yf - 4, 10, 10, colArrow));


   //         //         // battery light
   //         //         var pcl = new Vector2(90, m_display0.Viewport.Bottom - 55);

   //         //         var nCharging = 0;
   //         //         var batCur = 0f;
   //         //         var batMax = 0f;

   //         //         foreach (var b in m_batteries)
   //         //         {
   //         //             if (b.ChargeMode != ChargeMode.Recharge) nCharging++;
   //         //             batCur += b.CurrentOutput;
   //         //             batMax += b.MaxOutput;
   //         //         }

   //         //         var clon  = new Color(0xaa, 0xaa, 0);
   //         //         var clbar = new Color(0x55, 0x4c, 0);
   //         //         var cloff = new Color(0x0c, 0x08, 0);
   //         //         var clc   = nCharging >= m_batteries.Count/2 ? clon : cloff;

   //         //         if (m_help)
   //         //         {
   //         //             m_display0.Add(ref frame0, DrawString("1", pcl.X, pcl.Y + 28, 0.5f, clon, TextAlignment.CENTER));
   //         //}
   //         //         else
   //         //         { 
   //         //             m_display0.Add(ref frame0, DrawTexture("CircleHollow", pcl.X-9, pcl.Y-9 + 36, 18, 18, clc));
   //         //             m_display0.Add(ref frame0, DrawTexture("IconEnergy", pcl.X-8, pcl.Y-8 + 36, 16, 16, clc));
   //         //         }

   //         //         m_display0.Add(ref frame0, FillRect(pcl.X-7, pcl.Y+20, 14, -30, cloff));
   //         //         m_display0.Add(ref frame0, FillRect(pcl.X-7, pcl.Y+20, 14, -30 * batCur/batMax, clbar));


   //         //         // nuclear reactor light
   //         //         if (m_reactors.Count > 0)
   //         //         { 
   //         //             var reacOut = 0f;
   //         //             var reacU = 0f;
   //         //             var enabled = false;

   //         //             foreach (var r in m_reactors)
   //         //             { 
   //         //                 reacOut += r.CurrentOutput;
   //         //                 reacU += (float)r.GetInventory(0).CurrentMass;
   //         //                 enabled |= r.Enabled;
   //         //             }


   //         //             if (m_help)
   //         //                 m_display0.Add(ref frame0, DrawString("2", 114, m_display0.Viewport.Bottom - 27, 0.5f, clon, TextAlignment.CENTER));
   //         //             else
   //         //                 m_display0.Add(ref frame0, DrawTexture("Textures\\FactionLogo\\Others\\OtherIcon_19.dds", 105, m_display0.Viewport.Bottom - 28, 18, 18, enabled ? new Color(0xaa, 0xaa, 0) : new Color(0x0c, 0x08, 0)));

   //         //             m_display0.Add(ref frame0, FillRect(114-7, m_display0.Viewport.Bottom-35, 14, -30, cloff));
   //         //             m_display0.Add(ref frame0, FillRect(114-7, m_display0.Viewport.Bottom-35, 14, -30 * Math.Min(Math.Max(0, reacOut/batMax), 1), clbar));

   //         //             m_display0.Add(ref frame0, DrawString(printMass(reacU), 127, m_display0.Viewport.Bottom-20, 0.3f, new Color(0x88, 0x82, 0)));
   //         //         }


   //         //         // far lights
   //         //         var pfl = m_display0.Viewport.Center + new Vector2(-110, -9);
   //         //         var flc = m_farLights ? new Color(0, 0x44, 0xff) : new Color(0, 0x03, 0x18);
   //         //         if (m_help)
   //         //         {
   //         //             m_display0.Add(ref frame0, DrawString("3", pfl.X + 2, pfl.Y + 29, 0.5f, new Color(0, 0x44, 0xff), TextAlignment.CENTER));
   //         //}
   //         //         else
   //         //         { 
   //         //             m_display0.Add(ref frame0, DrawTexture("SemiCircle", pfl.X-5, pfl.Y-5 + 37, 11, 11, flc, -Tau/4));
   //         //             m_display0.Add(ref frame0, FillRect(pfl.X+9 - 7, pfl.Y-9 + 41, 8, 2, flc));
   //         //             m_display0.Add(ref frame0, FillRect(pfl.X+9 - 7, pfl.Y-9 + 44, 8, 2, flc));
   //         //             m_display0.Add(ref frame0, FillRect(pfl.X+9 - 7, pfl.Y-9 + 47, 8, 2, flc));
   //         //             m_display0.Add(ref frame0, FillRect(pfl.X+9 - 7, pfl.Y-9 + 50, 8, 2, flc));
   //         //         }


   //         //         // near lights
   //         //         var pnl = m_display0.Viewport.Center + new Vector2(-75, -9);
   //         //         var nlc = m_lightsOn ? new Color(0, 0xcc, 0) : new Color(0, 0x08, 0);
   //         //         if (m_help)
   //         //         {
   //         //             m_display0.Add(ref frame0, DrawString("L", pnl.X + 4, pnl.Y + 29, 0.5f, new Color(0, 0xcc, 0), TextAlignment.CENTER));
   //         //             m_display0.Add(ref frame0, DrawString("4", pnl.X + 4, pnl.Y + 45, 0.3f, new Color(0, 0xcc, 0), TextAlignment.CENTER));
   //         //}
   //         //         else
   //         //         { 
   //         //             m_display0.Add(ref frame0, DrawTexture("SemiCircle", pnl.X-5, pnl.Y-5 + 37, 10.5f, 10.5f, nlc, -Tau/4));
   //         //             m_display0.Add(ref frame0, FillRect(pnl.X+9 - 7, pnl.Y-9 + 41, 8, 1, nlc));
   //         //             m_display0.Add(ref frame0, FillRect(pnl.X+9 - 7, pnl.Y-9 + 44, 8, 1, nlc));
   //         //             m_display0.Add(ref frame0, FillRect(pnl.X+9 - 7, pnl.Y-9 + 47, 8, 1, nlc));
   //         //             m_display0.Add(ref frame0, FillRect(pnl.X+9 - 7, pnl.Y-9 + 50, 8, 1, nlc));

   //         //             var alc = m_autoLights ? new Color(0, 0xcc, 0) : new Color(0, 0x08, 0);
   //         //             m_display0.Add(ref frame0, DrawString("AUTO", pnl.X + 3, pnl.Y + 45, 0.2f, alc, TextAlignment.CENTER));
   //         //         }
   //     //}



   //     void DrawDriveModeLight()
   //     {
   //         var pdm = m_display0.Viewport.Center + new Vector2(0, 17);
   //         var mode = m_cockpit.HandBrake ? "P" : (m_reversing ? "R" : "D");
   //         m_display0.Add(ref frame0, FillRect(pdm.X - 12, pdm.Y - 2, 22, 26, new Color(0x04, 0x04, 0x06)));
   //         m_display0.Add(ref frame0, DrawString(mode, pdm.X, pdm.Y, 0.7f, Color.Gray, TextAlignment.CENTER));
   //     }



   //     void DrawCruiseControl()
   //     {
   //         var pcc = m_display0.Viewport.Center + new Vector2(50, 85);
   //         var ccc = m_cruiseControl ? new Color(0, 0xcc, 0) : new Color(0, 0x13, 0);
   //         var ccv = new Color(0x66, 0x66, 0x66);
   //         var ccl = new Color(0x33, 0x33, 0x33);

   //         m_display0.Add(ref frame0, FillRect(pcc.X - 3, pcc.Y - 31, 26, 14, new Color(0x04, 0x04, 0x06)));

   //         if (m_cruiseControl)
   //             m_display0.Add(ref frame0, DrawString(printValue(m_ccHeading, 0, true, 3), pcc.X+1, pcc.Y - 29, 0.33f, ccv));

   //         m_display0.Add(ref frame0, DrawString("°", pcc.X + 25, pcc.Y - 29, 0.2f, ccv));

   //         m_display0.Add(ref frame0, FillRect(pcc.X - 3, pcc.Y - 16, 26, 14, new Color(0x04, 0x04, 0x06)));

			//if (m_cruiseControl)
   //             m_display0.Add(ref frame0, DrawString(printValue(m_ccSpeed * 3.6f, 0, true, 3), pcc.X+1, pcc.Y - 14, 0.33f, ccv));

   //         m_display0.Add(ref frame0, DrawString("km/h", pcc.X + 25, pcc.Y - 11, 0.2f, ccv));

   //         m_display0.Add(ref frame0, DrawString("CRUISE", pcc.X-1, pcc.Y, 0.2f, ccc));
   //     }



   //     void DrawParkingBrakeLight()
   //     {
   //         var ppl = m_display0.Viewport.Center + new Vector2(60, -8);
   //         var plc = m_cockpit.HandBrake ? new Color(0, 0xcc, 0) : new Color(0, 0x13, 0);
   //         if (m_help)
   //         {
   //             m_display0.Add(ref frame0, DrawString("P", ppl.X, ppl.Y + 29, 0.5f, new Color(0, 0xcc, 0), TextAlignment.CENTER));
			//}
   //         else
   //         { 
   //             m_display0.Add(ref frame0, DrawTexture("CircleHollow", ppl.X-14, ppl.Y-14 + 36, 28, 28, plc));
   //             m_display0.Add(ref frame0, FillRect(ppl.X-20, ppl.Y-13 + 35, 40, 6, Color.Black));
   //             m_display0.Add(ref frame0, FillRect(ppl.X-20, ppl.Y-13 + 57, 40, 6, Color.Black));
   //             m_display0.Add(ref frame0, DrawTexture("CircleHollow", ppl.X-10, ppl.Y-10 + 36, 20, 20, plc));
   //             m_display0.Add(ref frame0, DrawString("!", ppl.X, ppl.Y + 30f, 0.4f, plc, TextAlignment.CENTER));
   //         }
   //     }



   //     void DrawEmergencyLight()
   //     {
   //         var pel = m_display0.Viewport.Center + new Vector2(100, -10);
   //         var elc = m_emergency && m_count10 % blink10 < blink10/2 ? new Color(0xaa, 0, 0) : new Color(0x08, 0, 0);
   //         if (m_help)
   //         {
   //             m_display0.Add(ref frame0, DrawString("8", pel.X, pel.Y + 32, 0.5f, new Color(0xaa, 0, 0), TextAlignment.CENTER));
			//}
   //         else
   //         { 
   //             m_display0.Add(ref frame0, DrawTexture("Triangle", pel.X-11, pel.Y-11 + 36, 22, 22, elc));
   //             m_display0.Add(ref frame0, DrawTexture("Triangle", pel.X-9, pel.Y-9 + 36.7f, 18, 18, Color.Black));
   //             m_display0.Add(ref frame0, DrawTexture("Triangle", pel.X-6, pel.Y-6 + 38, 12, 12, elc));
   //             m_display0.Add(ref frame0, DrawTexture("Triangle", pel.X-4, pel.Y-4 + 38.7f, 8, 8, Color.Black));
   //         }
   //     }



   //     void DrawLeftTurnLight()
   //     {
   //         var plt = m_display0.Viewport.Position + new Vector2(4, 17);
   //         var ltc = (m_turningLeft || m_emergency) && m_count10 % blink10 < blink10/2 ? new Color(0, 0xcc, 0) : new Color(0, 0x08, 0);
   //         m_display0.Add(ref frame0, DrawTexture("Triangle", plt.X-8 + 10, plt.Y-8, 16, 16, ltc, -Tau/4));
   //         m_display0.Add(ref frame0, FillRect(plt.X-4 + 20, plt.Y-4, 8, 8, ltc));
   //     }



   //     void DrawRightTurnLight()
   //     {
   //         var prt = m_display0.Viewport.Position + new Vector2(m_display0.Viewport.Width - 4, 17);
   //         var rtc = (m_turningRight || m_emergency) && m_count10 % blink10 < blink10/2  ? new Color(0, 0xcc, 0) : new Color(0, 0x08, 0);
   //         m_display0.Add(ref frame0, DrawTexture("Triangle", prt.X-8 - 10, prt.Y-8, 16, 16, rtc, Tau/4));
   //         m_display0.Add(ref frame0, FillRect(prt.X-4 - 20, prt.Y-4, 8, 8, rtc));
   //     }



   //     void DrawRollGauge()
   //     {
   //         var rSize = 70;

			//var xr = m_display2.Viewport.Width - 50;
			//var yr = 170 - rSize/2;

   //         m_display2.Add(ref frame2, DrawTexture("Circle", xr - rSize/2, yr - rSize/2, rSize, rSize, colGauge));

			//m_display2.Add(ref frame2, DrawString("R", xr, yr + 8, 0.5f, colLabel, TextAlignment.CENTER));

   //         for (float a = 0; a < Tau; a += Tau/8)
   //         {
   //             var v = vector2(a, rSize/2 + 5);
   // 			//m_display2.Add(ref frame2, DrawString(printValue(p, 0, true, 0), xr + v.X*1.02f, yr + v.Y*1.02f * 1.1f - 5, 0.35f, Color.Gray, p < 20 || p > 80 ? TextAlignment.CENTER : TextAlignment.RIGHT));
   // 			m_display2.Add(ref frame2, DrawLine(xr + v.X*0.7, yr + v.Y*0.7f, xr + v.X*0.89f, yr + v.Y*0.89f, Color.Gray, 3));
			//}

   // 		m_display2.Add(ref frame2, DrawLine(xr - rSize/2 - 7, yr, xr + rSize/2 + 7, yr, Color.Gray, 1));

   //         var vr = vector2(m_orientation.Z * Tau/2, rSize/2);
   // 		m_display2.Add(ref frame2, DrawLine(xr - vr.X, yr - vr.Y + 4, xr + vr.X, yr + vr.Y + 4, colShadow, 9));
   //         m_display2.Add(ref frame2, DrawTexture("Circle", xr - 5, yr - 5 + 4, 10, 10, colShadow));
   // 		m_display2.Add(ref frame2, DrawLine(xr - vr.X, yr - vr.Y, xr + vr.X, yr + vr.Y, colArrow, 4));
   //         m_display2.Add(ref frame2, DrawTexture("Circle", xr - 4, yr - 4, 8, 8, colArrow));
   //     }



   //     void DrawPitchGauge()
   //     {
   //         var ptSize = 70;

			//var xpt = m_display2.Viewport.X + 232;
			//var ypt = 85 - ptSize/2;

   //         m_display2.Add(ref frame2, DrawTexture("Circle", xpt - ptSize/2, ypt - ptSize/2, ptSize, ptSize, colGauge));
   //         m_display2.Add(ref frame2, FillRect(xpt + 13, ypt - 40, xpt + 50, ypt + 40, Color.Black));

			//m_display2.Add(ref frame2, DrawString("P", xpt - 6, ypt + 8, 0.5f, colLabel, TextAlignment.CENTER));

   //         for (float a = Tau/4; a <= Tau*3/4; a += Tau/8)
   //         {
   //             var v = vector2(a, ptSize/2 + 5);
   // 			//m_display2.Add(ref frame2, DrawString(printValue(p, 0, true, 0), xpt + v.X*1.02f, ypt + v.Y*1.02f * 1.1f - 5, 0.35f, Color.Gray, p < 20 || p > 80 ? TextAlignment.CENTER : TextAlignment.RIGHT));
   // 			m_display2.Add(ref frame2, DrawLine(xpt + v.X*0.7, ypt + v.Y*0.7f, xpt + v.X*0.89f, ypt + v.Y*0.89f, Color.Gray, 3));
			//}

   // 		m_display2.Add(ref frame2, DrawLine(xpt - ptSize/2 - 7, ypt, xpt + 10, ypt, Color.Gray, 1));

			//var vpt = vector2(Tau/2 + Math.Min(m_orientation.X, 1) * Tau/2, ptSize/2);
   // 		m_display2.Add(ref frame2, DrawLine(xpt, ypt + 4, xpt + vpt.X, ypt + vpt.Y + 4, colShadow, 9));
   //         m_display2.Add(ref frame2, DrawTexture("Circle", xpt - 5, ypt - 5 + 4, 10, 10, colShadow));
   // 		m_display2.Add(ref frame2, DrawLine(xpt, ypt, xpt + vpt.X, ypt + vpt.Y, colArrow, 4));
   //         m_display2.Add(ref frame2, DrawTexture("Circle", xpt - 4, ypt - 4, 8, 8, colArrow));        
   //     }



   //     void DrawVehicleCoordinates()
   //     {
   //         line2 = 0;
   //         m_display2.Add(ref frame2, DrawString(/*"LON " + */printCoord(m_longitude, "W", "E"), 10, y2 + 150 + line2++*step2, 0.36f, Color.Gray));
   //         m_display2.Add(ref frame2, DrawString(/*"LAT " + */printCoord(m_latitude, "S", "N"),  10, y2 + 150 + line2++*step2, 0.36f, Color.Gray));
   //     }



   //     void DrawVehicleAltitude()
   //     {
   //         m_display2.Add(ref frame2, DrawString(printValue(m_altitude, 0, true, 5) + " m", 115, m_display2.Viewport.Height - 26, 0.4f, Color.Gray));
   //     }



   //     void DrawCompass()
   //     {
   //         var cSize = 80;

			//var xc = m_display2.Viewport.X + 97;
			//var yc = m_display2.Viewport.Y + 80 - cSize/2;

   //         m_display2.Add(ref frame2, DrawTexture("Circle", xc - cSize/2, yc - cSize/2, cSize, cSize, colGauge));

   //         for (float a = 0; a < Tau; a += Tau/12)
   //         {
   //             var v  = vector2(-m_heading/360*Tau + a, cSize/2 + 5);
   //             var v1 = vector2(-m_heading/360*Tau + a + Tau/36, cSize/2 + 5);
   //             var v2 = vector2(-m_heading/360*Tau + a + Tau/18, cSize/2 + 5);

   //             if (   Math.Abs(a          ) > 0.001
   //                 && Math.Abs(a - Tau/4  ) > 0.001
   //                 && Math.Abs(a - Tau/2  ) > 0.001
   //                 && Math.Abs(a - Tau*3/4) > 0.001)
   //     			m_display2.Add(ref frame2, DrawLine(xc + v.X*0.78, yc + v.Y*0.78f, xc + v.X*0.89f, yc + v.Y*0.89f, Color.Gray, 3));

   // 			m_display2.Add(ref frame2, DrawLine(xc + v1.X*0.85, yc + v1.Y*0.85f, xc + v1.X*0.89f, yc + v1.Y*0.89f, Color.Gray, 1));
   // 			m_display2.Add(ref frame2, DrawLine(xc + v2.X*0.85, yc + v2.Y*0.85f, xc + v2.X*0.89f, yc + v2.Y*0.89f, Color.Gray, 1));
			//}


   //         m_display2.Add(ref frame2, DrawTexture("Triangle", xc-5, yc-6 - cSize/2+10, 10, 12, Color.Gray));


   //         var heading = -m_heading/360*Tau;

   //         var vn = vector2(heading - Tau/4, cSize/2);
   //         m_display2.Add(ref frame2, DrawTexture("Triangle", xc+vn.X - 5, yc+vn.Y - 5, 10, 10, new Color(0x88, 0, 0), heading));


   //         for (float a = 0; a <= Tau/2; a += Tau/4)
   //         {
   //             var vs = vector2(heading + a, cSize/2);
   //             m_display2.Add(ref frame2, DrawTexture("Triangle", xc+vs.X - 4, yc+vs.Y - 4, 8, 8, Color.Gray, heading + Tau/4 + a));
			//}

   //         m_display2.Add(ref frame2, DrawString(printValue(m_heading, 0, true, 3) + "°", xc + 2, yc - 8, scale2, Color.Gray, TextAlignment.CENTER));
   //     }
    }
}
