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
        public class Gauge : Control
        {
            public float GaugeStartAngle,
                         GaugeEndAngle;

            public float ScaleStartAngle,
                         ScaleEndAngle;


            public Color BackColor = new Color(0xa0, 0xa0, 0xa0),
                         RimColor  = new Color(0x14, 0x11, 0x0c),
                         HighColor = new Color(0x66, 0x5c, 0x55),
                         TextColor = Color.Black;


            public Gauge(float x, float y, float w, float h)
                : base(x, y, w, h)
            {
                GaugeStartAngle = 0;
                GaugeEndAngle   = 0;

                ScaleStartAngle = 0;
                ScaleEndAngle   = 0;
            }



            public override void Draw(ref MySpriteDrawFrame frame, Program prog) 
            {
                prog.Echo("fX = " + fX);
                prog.Echo("fY = " + fY);
                prog.Echo("fW = " + fW);
                prog.Echo("fH = " + fH);


                FillRect(ref frame, fX, fY, 100, 100, Color.White);
                FillSemiCircle(
                    ref frame,
                    fX + fW/2,
                    fY + fH/2,
                    Math.Min(fW, fH)/2,
                    BackColor,
                    Tau/4);


                //var angleSide = right ? -1 : 1;

                //var so = 4f;
                //var hr = r/10f;
                //var aw = r/15f;

                //var fa = (curVal - valMin) / (valMax - valMin);
                //var va = vector2(1/4f * Tau + angleSide * startAngleGap + angleSide * fa * (Tau/2 - startAngleGap - endAngleGap), r * 0.835f);

                //ClipDraw(
                //    sprites,
                //    (int)(x - (right ? hr : r)),
                //    (int)(y - r - so),
                //    (int)(hr + r),
                //    (int)(r * 2 + so * 2));

                //// back
                //FillCircle(ref frame, x, y - so/2, r, colRim);
                //FillCircle(ref frame, x, y + so/2, r, colHigh);
                //FillCircle(ref frame, x, y,        r, colGauge);

                //// danger zone
                //if (!float.IsNaN(valDanger))
                //{ 
                //    var fd1  = (valDanger - valMin) / (valMax - valMin);
                //    var vd1 = vector2(1/4f * Tau + (angleSide > 0 ? Tau/2 : 0) + angleSide * startAngleGap + angleSide * fd1 * (Tau/2 - startAngleGap - endAngleGap), r * 9/10f);

                //    var fd2 = (valMax - valMin) / (valMax - valMin);
                //    var vd2 = vector2(1/4f * Tau + (angleSide > 0 ? Tau/2 : 0) + angleSide * startAngleGap + angleSide * fd2 * (Tau/2 - startAngleGap - endAngleGap), r * 9/10f);

                //    var sf = 0.71f;
                //    var ef = 0.76f;

                //    FillSemiCircle(ref frame, x, y, r*ef, new Color(0x55, 0x08, 0x08), angle(vd1));
                //    FillSemiCircle(ref frame, x, y, r*ef, colGauge, angle(vd2));

                //    FillCircle(ref frame, x, y, r*sf, colGauge);
                //}

                //// edge cleanup
                //FillRect(ref frame, x - (right ? (hr + 1) : 0), y - r, hr + 1, r * 2, fillEdge ? colGauge : colBack);

                //// shadows
                //DrawLine(ref frame, x, y + so, x + va.X, y + va.Y + so, colShadow, aw);
                //FillCircle(ref frame, x, y + so, hr, colShadow);

                //// numbers
                //for (float val = valMin; val <= valMax; val += valStep)
                //{
                //    var f = (val - valMin) / (valMax - valMin);
                //    var v = vector2(1/4f * Tau + angleSide * startAngleGap + angleSide * f * (Tau / 2 - startAngleGap - endAngleGap), r * 9/10f);

                //    DrawString(ref frame, printValue(val, 0, true, 0), x + v.X, y + v.Y - 8*m_display1.ContentScale, r / 270, colText, TextAlignment.CENTER);
                //}

                //// ticks
                //for (float val = valMin; val <= valMax; val += tickStep)
                //{
                //    var f = (val - valMin) / (valMax - valMin);
                //    var v = vector2(1/4f * Tau + angleSide * startAngleGap + angleSide * f * (Tau/2 - startAngleGap - endAngleGap), r * 9/10f);

                //    var sf = 0.79f;
                //    var ef = 0.89f;

                //    DrawLine(ref frame, x + v.X * sf, y + v.Y * sf, x + v.X * ef, y + v.Y * ef, colText, r/50);
                //}

                //DrawString(ref frame, label, x - angleSide * r * 0.275f, y + r/4, r/200, colText, TextAlignment.CENTER);

                //// arrow
                //DrawLine(ref frame, x, y, x + va.X, y + va.Y, colArrow, aw);
                //FillCircle(ref frame, x, y, hr, colArrow);

                //ClearClip(sprites);
            }
        }
    }
}
