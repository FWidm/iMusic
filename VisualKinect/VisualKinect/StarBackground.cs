using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VisualKinect
{
    public class StarBackground
    {
        private Texture2D _StarTexture;
        private Texture2D _CloudTexture;

        private List<Star> stars;
        private int intensity;
        private Random _Random;

        private int MaxX;
        private int MaxY;

        Vector2 scrollspeed = new Vector2(20,0);

        public StarBackground(Vector2 size,GraphicsDevice gD, Texture2D cloud,int intensity)
        {
            this.intensity = intensity;
            stars = new List<Star>();
            _Random = new Random(DateTime.Now.Millisecond);

            MaxX = (int)size.X;
            MaxY = (int)size.Y;

            _StarTexture = GetSinglePixleWhiteTexture(gD);
            _CloudTexture = cloud;

            for (int i = 0; i <= intensity; i++)
            {
                int StarColorR = _Random.Next(25, 100);
                int StarColorG = _Random.Next(10, 100);
                int StarColorB = _Random.Next(90, 150);
                int StarColorA = _Random.Next(10, 50);

                float Scale = _Random.Next(100, 900) / 100f;
                int Depth = _Random.Next(4, 7);

                stars.Add(new Star(new Vector2(_Random.Next(MaxX / -2 - 500, MaxX / 2 + 500), _Random.Next(MaxY / -2 - 500, MaxY / 2 + 500)), new Color(StarColorR / 3, StarColorG / 3, StarColorB / 3, StarColorA / 3), Scale, Depth, true));
            }

            for (int i = 0; i <= intensity / 2; i++)
            {
                int StarColor = _Random.Next(100, 200);
                int Depth = _Random.Next(2, 6);
                float Scale = _Random.Next(2, 9) / 100f;

                stars.Add(new Star(new Vector2(_Random.Next(MaxX / -2 - 200, MaxX / 2 + 200), _Random.Next(MaxY / -2 - 200, MaxY / 2 + 200)), new Color(StarColor, StarColor, StarColor, StarColor), Scale, Depth, false));
            }

        }

        private Texture2D GetSinglePixleWhiteTexture(GraphicsDevice gD)
        {
            Texture2D singlePixleWhiteTexture = new Texture2D(gD, 1, 1);
            singlePixleWhiteTexture.SetData(new Color[] { Color.White });
            return singlePixleWhiteTexture;
        }

        public void Update(GameTime time)
        {
            foreach (Star s in stars)
            {
                s.Position += scrollspeed* -1f / s.Depth;

                if (s.Position.X > MaxX + 501)
                    s.Position.X -= s.Position.X + 500;

                if (s.Position.Y > MaxY + 500)
                    s.Position.Y -= s.Position.Y + 500;

                if (s.Position.X < -560)
                    s.Position.X += MaxX + 550;

                if (s.Position.Y < -570)
                    s.Position.Y += MaxY + 510;
            }
        }

        public void Update(GameTime time, Vector2 scrollspeed)
        {
            this.scrollspeed = scrollspeed;
            foreach (Star s in stars)
            {
                s.Position += scrollspeed * -1f / s.Depth;

                if (s.Position.X > MaxX + 501)
                    s.Position.X -= s.Position.X + 500;

                if (s.Position.Y > MaxY + 500)
                    s.Position.Y -= s.Position.Y + 500;

                if (s.Position.X < -560)
                    s.Position.X += MaxX + 550;

                if (s.Position.Y < -570)
                    s.Position.Y += MaxY + 510;
            }
        }


        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            foreach (Star s in stars)
            {
                spriteBatch.Draw(_CloudTexture, s.Position, null, s.Color, 0, Vector2.Zero, s.Scale, SpriteEffects.None, 0);
            }
            spriteBatch.End();
        }

    }
    class Star
    {
        public Vector2 Position;
        public Color Color;
        public float Scale;
        public float Depth;
        public bool isCloud;
        public Star(Vector2 Position, Color Color, float Scale, int Depth, bool isCloud)
        {
            this.Position = Position;
            this.Color = Color;
            this.Scale = Scale;
            this.Depth = Depth;
            this.isCloud = isCloud;
        }
    }
}