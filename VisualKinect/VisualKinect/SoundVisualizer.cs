using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using NAudio.Dsp;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace VisualKinect
{
    class SoundVisualizer
    {
        WasapiLoopbackCaptureMod capture;
        float[] samples;
        int numberOfValues;
        float[] average;
        float sampleOffset = 6;
        int oldDynamic = 0;

        bool cap;
        private static int fftLength = 8192; // NAudio fft wants powers of two!
        // There might be a sample aggregator in NAudio somewhere but I made a variation for my needs
        private SampleAggregator sampleAggregator = new SampleAggregator(fftLength);
        float[] frequency = new float[fftLength];

        ParticleEngine[] soundParticleEngines = new ParticleEngine[fftLength / 4];

        public SoundVisualizer(int numberOfValues, List<Texture2D> textures)
        {
            sampleAggregator.FftCalculated += new EventHandler<FftEventArgs>(FftCalculated);
            sampleAggregator.PerformFFT = true;
            capture = new WasapiLoopbackCaptureMod();
            capture.DataAvailable += new EventHandler<WaveInEventArgs>(capture_DataAvailable);
            samples = new float[5000];
            this.numberOfValues = numberOfValues;
            average = new float[numberOfValues];
            cap = false;

            Color color = Color.Purple * 0.02f;
            for (int i = 0; i < fftLength / 128; i++)
            {
                soundParticleEngines[i] = new ParticleEngine(textures, new Vector2(0, 0), color);
                soundParticleEngines[i].explosionParticle = true;
            }


            color = (new Color(50, 100, 140)) * 0.03f;
            for (int i = fftLength / 128; i < fftLength / 32; i++)
            {
                soundParticleEngines[i] = new ParticleEngine(textures, new Vector2(0, 0), color);
                soundParticleEngines[i].explosionParticle = true;
            }

            color = Color.Teal * 0.02f;
            for (int i = fftLength / 32; i < fftLength / 4; i++)
            {
                soundParticleEngines[i] = new ParticleEngine(textures, new Vector2(0, 0), color);
                soundParticleEngines[i].explosionParticle = true;
            }
        }
        public void Update()
        {
            if (!cap)
            {
                capture.StartRecording();
                cap = true;
            }
            int add = 1;
            int offset = 750;
            float border;
            int dynamic = 0;

            int width = Game1.graphics.PreferredBackBufferWidth - 150;
            for (int i = 0; i < frequency.Length / 4; i = i + add)
            {
                if (i < frequency.Length / 128)
                {
                    add = 10;
                    int steps = width / (frequency.Length / 128);
                    float tmp = 0;
                    border = 0.02f;
                    for (int j = 0; j < add; j++)
                    {
                        if (tmp < frequency[i + j])
                        {
                            tmp = frequency[i + j];
                        }
                        frequency[i + j] = 0;
                    }
                    if (tmp > border)
                    {
                        dynamic++;
                        if (!(soundParticleEngines[i].durationExplosion > 0))
                        {
                            frequency[i] = tmp;

                            float y = offset - frequency[i] * 10000;
                            if (y < 0)
                            {
                                y = 50;
                            }
                            soundParticleEngines[i].EmitterLocation = new Vector2(i * steps + 100, y);
                        }
                        soundParticleEngines[i].durationExplosion = 10;

                    }
                }
                else if (i < frequency.Length / 32)
                {
                    int steps = width / (frequency.Length / 32);
                    add = 20;
                    float tmp = 0;
                    border = 0.017f;

                    for (int j = 0; j < add; j++)
                    {
                        if (tmp < frequency[i + j])
                        {
                            tmp = frequency[i + j];
                        }
                        frequency[i + j] = 0;
                    }
                    if (tmp > border)
                    {
                        dynamic++;
                        if (!(soundParticleEngines[i].durationExplosion > 0))
                        {
                            frequency[i] = tmp;

                            float y = offset - frequency[i] * 10000;
                            if (y < 0)
                            {
                                y = 50;
                            }
                            soundParticleEngines[i].EmitterLocation = new Vector2(width - (i - frequency.Length / 128) * steps + 100, y);
                        }
                        soundParticleEngines[i].durationExplosion = 16;

                    }
                }
                else if (i < frequency.Length / 4)
                {
                    int steps = width / (frequency.Length / 4);
                    add = 30;
                    float tmp = 0;
                    border = 0.007f;

                    for (int j = 0; j < add; j++)
                    {
                        if (tmp < frequency[i + j])
                        {
                            tmp = frequency[i + j];
                        }
                        frequency[i + j] = 0;
                    }
                    if (tmp > border)
                    {
                        dynamic++;
                        if (!(soundParticleEngines[i].durationExplosion > 0))
                        {
                            Random random = new Random();
                            int height = Game1.graphics.PreferredBackBufferHeight - 200;
                            int x = random.Next(width - 50);
                            int y = random.Next(height);
                            soundParticleEngines[i].EmitterLocation = new Vector2(x + 100, y + 50);
                        }
                        soundParticleEngines[i].durationExplosion = 30;
                    }
                }
            }
            for (int i = 0; i < frequency.Length / 4; i++)
            {
                soundParticleEngines[i].Update();
            }

            if (dynamic > 0 && dynamic < 4 && oldDynamic < 4 && sampleOffset < 20)
            {
                sampleOffset = sampleOffset + 0.01f;
            }
            else if (dynamic > 9)
            {
                sampleOffset = sampleOffset - 0.05f;
            }
            oldDynamic = dynamic;
            //Console.WriteLine("offset: " + sampleOffset + " " + dynamic);
        }

        public void stopVisualizer()
        {
            capture.StopRecording();
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            for (int i = 0; i < numberOfValues; i++)
            {
                //particleMusic[i].Draw(spriteBatch);
                spriteBatch.Begin();
                Color c = Color.Purple;
                c = c * 0.5f;
                String draw = ":";
                spriteBatch.DrawString(font, draw, new Vector2(i, 400 + average[i] * sampleOffset * 8), c);
                spriteBatch.End();
            }
            for (int i = 0; i < frequency.Length / 4; i++)
            {
                //spriteBatch.GraphicsDevice.Clear(Color.White);
                soundParticleEngines[i].Draw(spriteBatch);
            }

        }
        private void capture_DataAvailable(object sender, WaveInEventArgs e)
        {

            //Console.WriteLine(e.BytesRecorded+" "+e.Buffer[0]);
            int z = 0;

            byte[] buffer = e.Buffer;

            int bytesRecorded = e.BytesRecorded;

            int bufferIncrement = (int)(this.capture.WaveFormat.BlockAlign / this.capture.WaveFormat.Channels);
            int bitsPerSample = this.capture.WaveFormat.BitsPerSample;

            samples = new float[e.BytesRecorded / bufferIncrement];

            for (int index = 0; index < e.BytesRecorded; index += bufferIncrement)
            {
                float sample32 = 0;

                if (bitsPerSample <= 16) // Presume 16-bit PCM WAV
                {
                    short sample16 = (short)((buffer[index + 1] << 8) | buffer[index + 0]);
                    sample32 = sample16 / 32768f;
                }
                else if (bitsPerSample <= 32) // Presume 32-bit IEEE Float WAV
                {
                    sample32 = BitConverter.ToSingle(buffer, index);
                }
                else
                {
                    throw new Exception(bitsPerSample + " Bits Per Sample Is Not Supported!");
                }

                sample32 = sample32 * sampleOffset;
                // Clip Sample - Prevents Issues Elsewhere
                if (sample32 > 1.0f)
                    sample32 = 1.0f;
                if (sample32 < -1.0f)
                    sample32 = -1.0f;

                samples[z] = sample32;
                z++;
                //FFT
                sampleAggregator.Add(sample32);
            }
            int numberPerValue = samples.Length / numberOfValues;

            z = 0;
            for (int i = 0; i < samples.Length; i = i + numberPerValue)
            {
                if (z < numberOfValues)
                {
                    for (int j = 0; j < numberPerValue; j++)
                    {
                        average[z] = average[z] + samples[i + j];
                    }

                    average[z] = average[z] / numberPerValue;
                    z++;
                }
            }
        }
        void FftCalculated(object sender, FftEventArgs e)
        {
            // Do something with e.result!
            for (int i = 0; i < e.Result.Length; i++)
            {
                frequency[i] = e.Result[i].X;
            }
        }
    }
}
