using System;
using VRageMath;
using Sandbox.ModAPI.Ingame;


namespace IngameScript
{
    partial class Program
    {
        const float Tau = (float)Math.PI * 2;
        Vector2 vector2(float angle, float dist)
        {
            return new Vector2(
                dist * (float)Math.Cos(angle),
                dist * (float)Math.Sin(angle));
        }
        float nozero(float f) { return f == 0 ? float.Epsilon : f; }
        Vector3 unit(Vector3 v) { return v / nozero(v.Length()); }
        float distanceToPlane(Vector3 p, Vector3 pp, Vector3 pn) // pp = point on plane, pn = plane normal
        {
            Vector3 v = pp - p;
            return Vector3.Dot(v, pn) / pn.Length();
        }
        float angleToPlane(Vector3 v, Vector3 pn, Vector3 rn) // pn = plane normal, rn = rotation plane normal 
        {
            var angle = (float)Math.Asin(Vector3.Dot(v, pn) / (v.Length() * pn.Length()));

            if (Vector3.Dot(Vector3.Cross(v, pn), rn) < 0)
                angle = (float)Math.Sign(angle) * Tau/2 - angle;

            return angle;
        }
        static Vector2 Vector2_NaN = new Vector2(float.NaN, float.NaN);
        static bool Vector2_IsNaN(Vector2 v)
        {
            return 
                   float.IsNaN(v.X)
                || float.IsNaN(v.Y);
        }
        static Vector3 Vector3_NaN = new Vector3(float.NaN, float.NaN, float.NaN);
        static bool Vector3_IsNaN(Vector3 v)
        {
            return 
                   float.IsNaN(v.X)
                || float.IsNaN(v.Y)
                || float.IsNaN(v.Z);
        }


        static string printNoZero(double d, int dec)
        {
            return d.ToString((Math.Abs(d) < 1 ? "" : "0") + "." + new string('0', dec));
        }
        static string printValue(double val, int dec, bool showZero, int pad)
        {
            if (showZero)
            {
                string format =
                      "0"
                    + (dec > 0 ? "." : "")
                    + new string('0', dec);

                return
                    val
                    .ToString(format)
                    .PadLeft(pad + dec + (dec > 0 ? 1 : 0));
            }
            else
            {
                return
                    printNoZero(val, dec)
                    .PadLeft(pad + dec + (dec > 0 ? 1 : 0));
            }
        }


        static string printCoord(float val, string neg, string pos)
        {
            var aval = (float)Math.Abs(val);

            float deg = (int)aval;
            float min = (int)((aval - deg) * 60);
            float sec = (int)(((aval - deg) * 60 - min) * 60);

            return
                  deg.ToString("0").PadLeft(3) + "°"
                + Math.Abs(min).ToString("00") + "\'"
                + Math.Abs(sec).ToString("00") + "\""
                + " " + (val < 0 ? neg : (val > 0 ? pos : " "));
        }
        static string printMass(float amount, bool units = true)
        {
            if (amount >= 10000) return printValue(amount / 1000, 1, true, 4) + (units ? " tn" : "");
            else                 return printValue(amount,        0, true, 4) + (units ? " kg" : "");
        }



        Vector2 intersect(Vector2 p1, Vector2 v1, Vector2 p2, Vector2 v2)
        {
	        if (cross(v1, v2) == 0) 
		        return Vector2_NaN; // parallel lines

	        return p1 + v1 * cross(p2 - p1, v2) / cross(v1, v2);
        }

        float angle(Vector2 v)
        {
	        return (float)Math.Atan2(v.Y, v.X);
        }

        float cross(Vector2 v1, Vector2 v2)
        {
	        return
		          v1.X * v2.Y 
		        - v1.Y * v2.X;
        }	



        IMyTerminalBlock Get(string s) { return GridTerminalSystem.GetBlockWithName(s); }



        bool OK(Display dsp)
        {
            return 
                   dsp         != null
                && dsp.Surface != null;
        }



        bool OK(IMyFunctionalBlock block)
        {
            return
                   block != null
                && block.IsWorking;
        }


        static bool OK(float f) => !float.IsNaN(f);
    }
}
