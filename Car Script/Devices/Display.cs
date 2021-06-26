using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRageMath;


namespace IngameScript
{
    partial class Program
    {
        public class Display
        {
            public IMyTextSurface         Surface;

            public IMyTextSurfaceProvider Provider;
            public int                    Index;

            public RectangleF             Viewport;

            public float                  Scale,
                                          
                                          ContentWidth,
                                          ContentHeight;

            //bool                        m_flag;

            public List<Control>          Controls;



            public Display(IMyTextSurfaceProvider provider, int index)
            {
                Surface  = provider.GetSurface(Index);
                Provider = provider;
                Index    = index;
        
                Init();
            }



            public Display(IMyTextSurface surface)
            {
                Surface  = surface;
                Provider = null;
                Index    = -1;

                Init();
            }



            void Init()
            {
                Surface.ContentType = ContentType.SCRIPT;
                Surface.Script      = "";

                Scale               = 1;
                                   
                Viewport = new RectangleF(
                    (Surface.TextureSize - Surface.SurfaceSize) / 2, 
                     Surface.SurfaceSize);
                                  
                ContentWidth  = Viewport.Width;
                ContentHeight = Viewport.Height;
                
                //m_flag      = false;

                Controls      = new List<Control>();
            }


            public float ContentScale =>
                  Math.Min(Surface.TextureSize.X, Surface.TextureSize.Y) / 512
                * Math.Min(Surface.SurfaceSize.X, Surface.SurfaceSize.Y)
                / Math.Min(Surface.TextureSize.Y, Surface.TextureSize.Y);



            public void Update()
            {
                foreach (var c in Controls)
                    c.Update();
            }



            public void Draw(Program prog)
            {
                var x = Viewport.X;
                var y = Viewport.Y;
                var w = Viewport.Width;
                var h = Viewport.Height;


                prog.Echo("x = " + x);
                prog.Echo("y = " + y);
                prog.Echo("w = " + w);
                prog.Echo("h = " + h);


                var frame = Surface.DrawFrame();
 
                //if (m_flag = !m_flag)
                //frame.Add(new MySprite());

                ClearClip(ref frame);                

                FillRect(ref frame, x, y, w/2, h, Color.Green);

                foreach (var c in Controls)
                    c.Draw(ref frame, prog);


                frame.Dispose();
            }
        }
    }
}
