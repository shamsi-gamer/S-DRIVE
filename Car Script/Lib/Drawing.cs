using System;
using System.Collections.Generic;
using VRageMath;
using VRage.Game.GUI.TextPanel;


namespace IngameScript
{
    partial class Program
    {
        static void ClipDraw(ref MySpriteDrawFrame frame, float x, float y, float w, float h)
        {
            frame.Add(MySprite.CreateClipRect(new Rectangle((int)x, (int)y, (int)w, (int)h)));
        }


        static void ClearClip(ref MySpriteDrawFrame frame)
        {
            frame.Add(MySprite.CreateClearClipRect());
        }


        static void DrawString(ref MySpriteDrawFrame frame, string str, float x, float y, float scale, Color c, TextAlignment align = TextAlignment.LEFT)
        {
            frame.Add(new MySprite()
            {
                Type            = SpriteType.TEXT,
                Data            = str,
                Position        = new Vector2(x, y),
                RotationOrScale = scale,
                Color           = c,
                Alignment       = align,
                FontId          = "Monospace"
            });
        }
        
        
        static void DrawTexture(ref MySpriteDrawFrame frame, string texture, Vector2 pos, Vector2 size, Color c, float rotation = 0)
        {
            frame.Add(new MySprite()
            {
                Type            = SpriteType.TEXTURE,
                Data            = texture,
                Position        = pos + size/2,
                Size            = size,
                Color           = c,
                Alignment       = TextAlignment.CENTER,
                RotationOrScale = rotation
            });
        }


        static void DrawTexture(ref MySpriteDrawFrame frame, string texture, float x, float y, float w, float h, Color c, float rotation = 0)
        {
            DrawTexture(ref frame, texture, new Vector2(x, y), new Vector2(w, h), c, rotation);
        }
        
        
        static void FillRect(ref MySpriteDrawFrame frame, float x, float y, float w, float h, Color c)
        {
            DrawTexture(ref frame, "SquareSimple", x, y, w, h, c);
        }


        static void DrawRect(ref MySpriteDrawFrame frame, float x, float y, float w, float h, Color c, float wd = 1)
        {
            var wd2 = wd/2;

            DrawLine(ref frame, x-wd2, y,     x+w+wd2, y,   c, wd);
            DrawLine(ref frame, x-wd2, y+h,   x+w+wd2, y+h, c, wd);
            DrawLine(ref frame, x,     y-wd2, x,       y+h, c, wd);
            DrawLine(ref frame, x+w,   y-wd2, x+w,     y+h, c, wd);
        }


        static void FillCircle(ref MySpriteDrawFrame frame, Vector2 p, float r, Color color)
        {
            DrawTexture(ref frame, "Circle", p.X - r, p.Y - r, r*2, r*2, color);
        }


        static void FillCircle(ref MySpriteDrawFrame frame, float x, float y, float r, Color color)
        {
            DrawTexture(ref frame, "Circle", x-r, y-r, r*2, r*2, color);
        }


        static void FillSemiCircle(ref MySpriteDrawFrame frame, float x, float y, float r, Color color, float rotation = 0)
        {
            FillSemiCircle(ref frame, x, y, r, r, color, rotation);
        }


        static void FillSemiCircle(ref MySpriteDrawFrame frame, float x, float y, float r1, float r2, Color color, float rotation = 0)
        {
            DrawTexture(ref frame, "SemiCircle", x-r1, y-r2, r1*2, r2*2, color, rotation);
        }


        void DrawCircle(ref MySpriteDrawFrame frame, Vector2 p, float r, Color color)
        {
            DrawTexture(ref frame, "CircleHollow", p.X - r, p.Y - r, r * 2, r * 2, color);
        }


        void DrawCircle(ref MySpriteDrawFrame frame, float x, float y, float r, Color color)
        {
            DrawTexture(ref frame, "CircleHollow", x - r, y - r, r * 2, r * 2, color);
        }


        static void DrawLine(ref MySpriteDrawFrame frame, Vector2 p1, Vector2 p2, Color col, float width = 1)
        {
            var dp    = p2 - p1;
            var len   = dp.Length();
            var angle = (float)Math.Atan2(p1.Y - p2.Y, p2.X - p1.X);

            DrawTexture(
                ref frame,
                "SquareSimple",
                p1.X + dp.X/2 - len/2,
                p1.Y + dp.Y/2 - width/2,
                len,
                width,
                col,
                -angle);
        }
        
        
        static void DrawLine(ref MySpriteDrawFrame frame, float x1, float y1, float x2, float y2, Color col, float width = 1)
        {
            DrawLine(
                ref frame, 
                new Vector2(x1, y1), 
                new Vector2(x2, y2), 
                col, 
                width);
        }
    }
}
