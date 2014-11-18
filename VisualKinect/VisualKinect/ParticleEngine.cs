using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualKinect
{
    public class ParticleEngine
    {
        private Random random;
        public Vector2 EmitterLocation { get; set; }
        private List<Particle> particles;
        private List<Texture2D> textures;
        private Color color;
        private int particleType;
        private List<Color> colors;
        private Color[] colorGradient;
        private int colorGradientCounter;
        private int colorListCounter;
        public bool explosionParticle { get; set; }
        public int durationExplosion { get; set; }

        // Standart Constructor
        public ParticleEngine(List<Texture2D> textures, Vector2 location)
        {
            this.particleType = 0;
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();
            explosionParticle = false;
            durationExplosion = 0;
        }

        // Color Constructor
        public ParticleEngine(List<Texture2D> textures, Vector2 location, Color color)
        {
            this.particleType = 1;
            this.color = color;
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();
            explosionParticle = false;
            durationExplosion = 0;
        }

        // Color List Constructor
        public ParticleEngine(List<Texture2D> textures, Vector2 location, List<Color> colors)
        {
            this.particleType = 2;
            this.colors = colors;
            this.colorListCounter = 0;
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();
            explosionParticle = false;
            durationExplosion = 0;
        }

        // Color Gradient Constructor
        public ParticleEngine(List<Texture2D> textures, Vector2 location, Color[] colorGradient)
        {
            this.particleType = 3;
            this.colorGradient = colorGradient;
            this.colorGradientCounter = 0;
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();
            explosionParticle = false;
            durationExplosion = 0;
        }

        // Standard Generator
        private Particle GenerateNewParticle()
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = new Vector2(
                    1f * (float)(random.NextDouble() * 2 - 1),
                    1f * (float)(random.NextDouble() * 2 - 1));
            float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
            float c = (float)random.NextDouble();
            c *= 0.3f;
            Color color = new Color(c, c, c, c);

            float size = (float)random.NextDouble() / 2;
            int ttl = 30 + random.Next(40);

            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }

        // Color Generator
        private Particle GenerateNewColoredParticle()
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = new Vector2(
                    1f * (float)(random.NextDouble() * 2 - 1),
                    1f * (float)(random.NextDouble() * 2 - 1));
            float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);

            float size = (float)random.NextDouble() / 2;
            int ttl = 30 + random.Next(40);
            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }

        // Color List Generator
        private Particle GenerateNewColorListParticle()
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = new Vector2(
                    1f * (float)(random.NextDouble() * 2 - 1),
                    1f * (float)(random.NextDouble() * 2 - 1));
            float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);

            float size = (float)random.NextDouble() / 2;
            int ttl = 30 + random.Next(40);

            if (colors.Count > colorListCounter)
            {
                colorListCounter += 1;
                return new Particle(texture, position, velocity, angle, angularVelocity, colors[colorListCounter - 1], size, ttl);
            }
            else
            {
                this.colorListCounter = 0;
                return new Particle(texture, position, velocity, angle, angularVelocity, colors[0], size, ttl);
            }

        }

        // Color Gradient Generator
        private Particle GenerateNewColorGradientParticle()
        {
            int colorSteps = 30;
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = new Vector2(
                    1f * (float)(random.NextDouble() * 2 - 1),
                    1f * (float)(random.NextDouble() * 2 - 1));
            float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);

            Color startColor = colorGradient[0];
            Color endColor = colorGradient[1];

            int absR = (int)(startColor.R - endColor.R) / colorSteps;
            int absG = (int)(startColor.G - endColor.G) / colorSteps;
            int absB = (int)(startColor.B - endColor.B) / colorSteps;

            float size = (float)random.NextDouble() / 2;
            int ttl = 30 + random.Next(40);

            if (colorGradientCounter < colorSteps)
            {
                int r = startColor.R - colorGradientCounter * absR;
                int g = startColor.G - colorGradientCounter * absG;
                int b = startColor.B - colorGradientCounter * absB;
                Color currentColor = new Color(r, g, b, 255);
                colorGradientCounter += 1;
                //Console.WriteLine("CurrentGrad Color: " + currentColor);
                return new Particle(texture, position, velocity, angle, angularVelocity, currentColor, size, ttl);
            }
            else
            {
                this.colorGradientCounter = 0;
                return new Particle(texture, position, velocity, angle, angularVelocity, endColor, size, ttl);
            }

        }

        public void Update()
        {
            int total = 10;
            if (explosionParticle)
            {
                total = 3;
                if (durationExplosion > 0)
                {
                    durationExplosion--;

                    for (int i = 0; i < total; i++)
                    {
                        addParticles();
                    }
                }
                for (int particle = 0; particle < particles.Count; particle++)
                {
                    particles[particle].Update();

                    if (particles[particle].TTL <= 0)
                    {
                        particles.RemoveAt(particle);
                        particle--;
                    }
                }
            }
            else
            {
                for (int i = 0; i < total; i++)
                {
                    addParticles();
                }
                for (int particle = 0; particle < particles.Count; particle++)
                {
                    particles[particle].Update();

                    if (particles[particle].TTL <= 0)
                    {
                        particles.RemoveAt(particle);
                        particle--;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(spriteBatch);
            }
        }

        private void addParticles()
        {
            switch (particleType)
            {
                case 0:
                    {
                        particles.Add(GenerateNewParticle());
                        break;
                    }
                case 1:
                    {
                        particles.Add(GenerateNewColoredParticle());
                        break;
                    }
                case 2:
                    {
                        particles.Add(GenerateNewColorListParticle());
                        break;
                    }
                case 3:
                    {
                        particles.Add(GenerateNewColorGradientParticle());
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Error at AddparticleEngine");
                        break;
                    }

            }
        }
    }
}
