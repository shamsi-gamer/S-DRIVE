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
        public class Control
        {
            public float X, Y, W, H;
            public float fX, fY, fW, fH; // f = final

            public float Width  => W;
            public float Height => H;


            protected Control(float x, float y, float w, float h)
            {
                X = x;
                Y = y;
                W = w;
                H = h;
            }


            public virtual void Update() 
            {
                fX = X;
                fY = Y;
                fW = W;
                fH = H;
            }


            public virtual void Draw(ref MySpriteDrawFrame frame, Program prog) {}
        }
    }
}
