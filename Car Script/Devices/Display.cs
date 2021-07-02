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
                Index    = index;
                Surface  = provider.GetSurface(Index);
                Provider = provider;
        
                Init();
            }



            public Display(IMyTextSurface surface)
            {
                Index    = -1;
                Surface  = surface;
                Provider = null;

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
                { 
                    c.Update();

                    c.fX += Viewport.X;
                    c.fY += Viewport.Y;
                }
            }



            public void Draw(Program prog)
            {
                var x = Viewport.X;
                var y = Viewport.Y;
                var w = Viewport.Width;
                var h = Viewport.Height;

                var frame = Surface.DrawFrame();
 
                //if (m_flag = !m_flag)
                //frame.Add(new MySprite());

                FillRect(ref frame, x, y, w, h, Color.Black);

                foreach (var c in Controls)
                    c.Draw(ref frame, prog);

                frame.Dispose();
            }
        }
    }
}
