using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualKinect
{
    class MidiButton
    {
        Texture2D graphic;
        Rectangle dimension;
        bool pushed = false;
        Color color;
        Color oldColor;
        int pitch;
        int octave;
        int channel;
        //for matrix mode
        bool togglemode = false;
        bool trigger = true;
        public override string ToString()
        {
            return "[" + graphic + "; " + color + "; " + dimension + "]";
        }


        public MidiButton(int pitch, int octave, Rectangle rect, Color color, Texture2D texture,int channel)
        {
            this.dimension = rect;
            this.color = color;
            this.oldColor = color;
            //this.color.A=(byte)(0);
            this.pitch = pitch;
            this.octave = octave;
            this.channel = channel;
            //Draw the Button in a solid color
            graphic = texture;
        }

        public MidiButton(int pitch, int octave, Rectangle rect, Color color, GraphicsDevice graphicsDevice,int channel)
        {
            this.dimension = rect;
            this.color = color;
            this.oldColor = color;
            //this.color.A=(byte)(0);
            this.pitch = pitch;
            this.octave = octave;
            this.channel = channel;
            //Draw the Button in a solid color
            graphic = CreateCircle(graphicsDevice, color);
        }

        public Texture2D CreateCircle(GraphicsDevice gd, Color c)
        {
            int radius = dimension.Width / 2;
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(gd, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = c;
            }

            bool finished = false;
            int firstSkip = 0;
            int lastSkip = 0;
            for (int i = 0; i <= data.Length - 1; i++)
            {
                if (finished == false)
                {
                    //T = transparent W = White;
                    //Find the First Batch of Colors TTTTWWWTTTT The top of the circle
                    if ((data[i] == c) && (firstSkip == 0))
                    {
                        while (data[i + 1] == c)
                        {
                            i++;
                        }
                        firstSkip = 1;
                        i++;
                    }
                    //Now Start Filling                       TTTTTTTTWWTTTTTTTT
                    //circle in Between                       TTTTTTW--->WTTTTTT
                    //transaparent blancks                    TTTTTWW--->WWTTTTT
                    //                                        TTTTTTW--->WTTTTTT
                    //                                        TTTTTTTTWWTTTTTTTT
                    if (firstSkip == 1)
                    {
                        if (data[i] == c && data[i + 1] != c)
                        {
                            i++;
                            while (data[i] != c)
                            {
                                //Loop to check if its the last row of pixels
                                //We need to check this because of the 
                                //int outerRadius = radius * 2 + -->'2'<--;
                                for (int j = 1; j <= outerRadius; j++)
                                {
                                    if (data[i + j] != c)
                                    {
                                        lastSkip++;
                                    }
                                }
                                //If its the last line of pixels, end drawing
                                if (lastSkip == outerRadius)
                                {
                                    break;
                                }
                                else
                                {
                                    data[i] = c;
                                    i++;
                                    lastSkip = 0;
                                }
                            }
                            while (data[i] == c)
                            {
                                i++;
                            }
                            i--;
                        }


                    }
                }
            }

            texture.SetData(data);
            return texture;
        }

        public void CenterButton()
        {
            Rectangle dim = this.dimension;
            dimension = new Rectangle(dim.X - (dim.Center.X - dim.X), dim.Y - (dim.Center.Y - dim.Y), dim.Width, dim.Height);
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(graphic, dimension, color);
            batch.End();
        }

        public void Draw(SpriteBatch batch, GraphicsDevice gd)
        {
            batch.Begin();
            batch.Draw(graphic, dimension, color);
            Rectangle rect = dimension;
            rect.Inflate(-20, -20);
            Texture2D texture = new Texture2D(gd, rect.Width, rect.Height);

            Color[] data = new Color[rect.Width * rect.Height];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Chocolate;
            texture.SetData(data);
            batch.Draw(texture, rect, Color.Red);
            batch.End();
        }

        public void setMatrixSynthMode(bool toggle)
        {
            togglemode = toggle;
        }

        public int Collision(List<Vector2> positions)
        {
            Rectangle rect = dimension;
            rect.Inflate(-10, -10);

            int noIntersect = 0;
            bool intersect = false;
            foreach (Vector2 p in positions)
            {
                intersect = rect.Intersects(new Rectangle((int)p.X, (int)p.Y, 1, 1));
                if (intersect)
                    noIntersect++;
            }
            if (noIntersect > 0 && !pushed)
            {
                oldColor = color;
                color = color * 1.5f;
                PlayPitch();
                pushed = true;
            }
            if (noIntersect == 0)
            {
                StopPitch();
                if (pushed)
                {
                    pushed = false;
                }
                color = oldColor;
            }
            return noIntersect;
        }

        public int MatrixCollision(List<Vector2> positions)
        {
            Rectangle rect = dimension;
            rect.Inflate(-10, -10);

            int noIntersect = 0;
            bool intersect = false;
            foreach (Vector2 p in positions)
            {

                intersect = rect.Intersects(new Rectangle((int)p.X, (int)p.Y, 1, 1));
                if (intersect)
                    noIntersect++;
            }
            if (noIntersect > 0)
            {
                if (!pushed && trigger)
                {
                    Console.WriteLine("pushed");
                    oldColor = color;
                    color = Color.IndianRed;
                    pushed = true;
                }
                else if (pushed && trigger)
                {
                    Console.WriteLine("!pushed");
                    pushed = false;
                    color = oldColor;
                }
                trigger = false;
            }
            else
                trigger = true;

            return noIntersect;
        }

        public void PlayPitch()
        {
            MIDIChannel.playPitch((Midi.Channel)channel, pitch, octave, 127);
        }
        public void StopPitch()
        {
            MIDIChannel.stopPitch((Midi.Channel)channel, pitch, octave, 127);
        }

        public bool GetPushState()
        {
            return pushed;
        }

        public String getPitch()
        {
            int[] pitches = MIDIChannel.GetPitches();
            return "" + (Midi.Pitch)(pitches[pitch] + 12 * octave);
        }
    }
}

