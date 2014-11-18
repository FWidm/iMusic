using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;

namespace VisualKinect
{
    class Button
    {
        int x, y, width, height;
        float zOld;
        Texture2D graphic;
        Rectangle dimension;
        bool triggered = false;
        bool pushed = false;
        //bool hidden = false;
        SoundEffect sample;
        SoundEffectInstance instance;
        public Color color { get; set; }
        public Color baseColor { get; set; }
        public bool hidden { get; set; }

        public Button(int x, int y, int width, int height, Color color, SoundEffect sample, Texture2D graphic)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.graphic = graphic;
            this.sample = sample;
            dimension = new Rectangle(x, y, width, height);

            zOld = 0;
            instance = sample.CreateInstance();
            instance.IsLooped = true;
            this.color = color;
            this.baseColor = color;
            this.color = new Color(60, 20, 60);
        }

        public Button(int x, int y, int width, int height, Color color, SoundEffect sample, GraphicsDevice graphicsDevice)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.sample = sample;
            dimension = new Rectangle(x, y, width, height);
            zOld = 0;
            instance = sample.CreateInstance();
            instance.IsLooped = true;
            this.color = color;
            this.baseColor = color;

            var rect = new Texture2D(graphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = color;
            }
            rect.SetData(data);
            graphic = rect;
            this.color = new Color(60, 20, 60);
        }

        public void draw(SpriteBatch batch)
        {
            if (!hidden)
            {
                batch.Begin();
                batch.Draw(graphic, dimension, color);
                batch.End();
            }
        }
        public void collision(float xLHand, float yLHand, float zLHand,float xRHand, float yRHand, float zRHand, float zBody)
        {
            if (!pushed)
            {
                bool intersectRight = dimension.Intersects(new Rectangle((int)xRHand - 5, (int)yRHand - 5, 10, 10));
                bool intersectLeft = dimension.Intersects(new Rectangle((int)xLHand - 5, (int)yLHand - 5, 10, 10));
                if (intersectRight)
                {
                    float xHand = xRHand;
                    float yHand = yRHand;
                    float zHand = zRHand;
                    if (triggered && zHand + 0.10 - zBody < 0)
                    {
                        triggered = false;
                        //Console.WriteLine("Falsch " + zOld + " " + zHand + " Body_z:" + zBody);
                        zOld = zRHand;
                        instance.Stop();
                        pushed = true;
                    }
                    else if (zHand + 0.10 - zBody < 0)
                    {
                        triggered = true;
                        //Console.WriteLine("Wahr " + zOld + " " + zHand + " Body_z:" + zBody);
                        zOld = zRHand;
                        instance.Play();
                        pushed = true;
                    }
                }
                if (intersectLeft)
                {
                    float xHand = xLHand;
                    float yHand = yLHand;
                    float zHand = zLHand;
                    if (triggered && zHand + 0.10 - zBody < 0)
                    {
                        triggered = false;
                        //Console.WriteLine("Falsch " + zOld + " " + zHand + " Body_z:" + zBody);
                        zOld = zRHand;
                        instance.Stop();
                        pushed = true;
                    }
                    else if (zHand + 0.10 - zBody < 0)
                    {
                        triggered = true;
                        //Console.WriteLine("Wahr " + zOld + " " + zHand + " Body_z:" + zBody);
                        zOld = zRHand;
                        instance.Play();
                        pushed = true;
                    }
                }
            }
            else
            {
                if (zBody - zRHand < 0.1 && zBody - zRHand < 0.1)
                {
                    pushed = false;
                }
            }
        }

    }
}
