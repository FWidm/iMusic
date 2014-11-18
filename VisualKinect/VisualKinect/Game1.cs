using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;
using System.Diagnostics;
using System.Timers;
using System.Threading;

namespace VisualKinect
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphics { get; set; }
        SpriteBatch spriteBatch;
        ParticleEngine particleEngineLeftHand;
        ParticleEngine particleEngineRightHand;
        KinectSensor kinect;

        Texture2D jointTexture;
        Texture2D rectangle;

        SoundEffect sample1;
        SoundEffect sample2;
        SoundEffect sample3;
        SoundEffect sample4;

        Stopwatch watch;
        SpriteFont font;

        Boolean debugging = true;

        List<Button> toggleButtonList;

        //SoundVisualizer
        SoundVisualizer soundVisualizer;
        List<Texture2D> soundVisualizerTexture;
        List<Texture2D> textures;

        StarBackground bg;
        Texture2D fluff;
        Vector2 screen;

        List<MidiButton> midiButtonList;
        List<MidiButton> midiButtonMatrix;
        const int OCTAVES = 4;

        Texture2D buttonTexture;
        Vector2 posHandRight;
        Vector2 posHandLeft;

        Texture2D intro;
        Boolean displayIntro = true;

        Texture2D roundButtonTexture;

        String debug = "Hit Escape to close the game!";

        Boolean switchGenre = false;
        Boolean matrixSynth = false;
        float matrixTimer = 1 / 4f;         //Initialize a 10 second timer
        float resetMatrixTimer = 1 / 4f;
        int matrixIterator = 0;
        Texture2D indicatorTexture;
        const int INDICATOR_HEIGHT = 24;
        const int INDICATOR_WIDTH = 24;
        Rectangle indicatorRect = new Rectangle(0, 0, INDICATOR_HEIGHT, INDICATOR_WIDTH);
        Color backgroundColor = Color.Black;
        Color randomColor = Color.Black;
        float backgroundTimer = 3/ 3f;
        float resetBackgroundTimer = 3 / 3f;

        Boolean hideButtons = false;
        Boolean pauseGame = false;
        Boolean switchModi = false;


        //Keyboardbooleans
        private bool upPressed = false;
        private bool downPressed = false;
        private bool cPressed = false;
        private bool sPressed = false;
        private bool rPressed = false;
        public Game1()
        {
            Window.Title = "iMusic";
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            MIDIChannel.init();
            this.IsMouseVisible = true;
            SkeletonData.ScreenScale(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            //Initialise Kinect
            try
            {
                kinect = KinectSensor.KinectSensors[0];
                //kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                //kinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                kinect.SkeletonStream.Enable();
                kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);

                kinect.Start();
                //colorVideo = new Texture2D(graphics.GraphicsDevice, kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight);
                //depthVideo = new Texture2D(graphics.GraphicsDevice, kinect.DepthStream.FrameWidth, kinect.DepthStream.FrameHeight);
                Debug.WriteLineIf(debugging, kinect.Status);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            watch = new Stopwatch();

            //ButtonListe anlegen und mit Buttons befüllen
            Texture2D toggleButtonTexture = Content.Load<Texture2D>("newTextureButton");
            toggleButtonList = new List<Button>();
            toggleButtonList.Add(new Button((int)screen.X / 170, (int)screen.Y / 100, (int)screen.X / 12, (int)screen.X / 12, new Color(150, 245, 0, 255), sample1, toggleButtonTexture));
            toggleButtonList.Add(new Button((int)screen.X / 170, (int)screen.Y / 6, (int)screen.X / 12, (int)screen.X / 12, new Color(150, 245, 0, 255), sample2, toggleButtonTexture));
            toggleButtonList.Add(new Button((int)(screen.X / 1.095f), (int)screen.Y / 100, (int)screen.X / 12, (int)screen.X / 12, new Color(150, 245, 0, 255), sample3, toggleButtonTexture));
            toggleButtonList.Add(new Button((int)(screen.X / 1.095f), (int)screen.Y / 6, (int)screen.X / 12, (int)screen.X / 12, new Color(150, 245, 0, 255), sample4, toggleButtonTexture));

            //MIDIButton
            midiButtonList = new List<MidiButton>();

            for (int i = 0; i < OCTAVES; i++)
            {
                placeMidiButtons(i + 1, 640, 645, (int)(180 + Game1.graphics.PreferredBackBufferWidth / 12 * i), midiButtonList, 0, roundButtonTexture);
            }

            //MIDIMatrix
            midiButtonMatrix = new List<MidiButton>();

            for (int i = 0; i < OCTAVES; i++)
            {
                placeMidiButtons(i + 1, 640, 645, (int)(180 + Game1.graphics.PreferredBackBufferWidth / 12 * i), midiButtonMatrix, 1, roundButtonTexture);
            }

            //SoundVisualizer
            soundVisualizer = new SoundVisualizer(2000, soundVisualizerTexture);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            jointTexture = Content.Load<Texture2D>("joint");

            rectangle = Content.Load<Texture2D>("rechteck");

            roundButtonTexture = Content.Load<Texture2D>("NewRoundTextureButtons");

            //song = Content.Load<Song>("silent");

            sample1 = Content.Load<SoundEffect>("beat  Tribal Type 153");
            sample2 = Content.Load<SoundEffect>("beat drums 90");
            sample3 = Content.Load<SoundEffect>("beat Filtered dub 140");
            sample4 = Content.Load<SoundEffect>("beat Ghetto Taunt 140");

            font = Content.Load<SpriteFont>("Arial");
            // particle System
            textures = new List<Texture2D>();
            textures.Add(Content.Load<Texture2D>("circle"));
            textures.Add(Content.Load<Texture2D>("fluffyball"));
            particleEngineLeftHand = new ParticleEngine(textures, new Vector2(400, 240));
            particleEngineRightHand = new ParticleEngine(textures, new Vector2(400, 240));
            //soundParticleEngine = new ParticleEngine(textures, new Vector2(400, 240));

            // Starbackground
            fluff = Content.Load<Texture2D>("fluffyball");
            buttonTexture = Content.Load<Texture2D>("orb"); // orb oder raute
            screen = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
            bg = new StarBackground(screen, GraphicsDevice, fluff, 50);

            soundVisualizerTexture = new List<Texture2D>();
            soundVisualizerTexture.Add(Content.Load<Texture2D>("orb2"));

            indicatorTexture = Content.Load<Texture2D>("circle");

            intro = Content.Load<Texture2D>("wallpaper");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape) == true)
            {
                Console.Write("exit!");
                this.UnloadContent();
                this.Exit();
            }


            if (displayIntro)
            {
                if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Enter) == true
                    || SkeletonData.skeleton != null)
                {
                    displayIntro = false;
                }
            }
            else
            {
                if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.S))
                {
                    sPressed = true;
                }
                else if (sPressed)
                {
                    switchGenre = true;
                    sPressed = false;
                }

                if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.R))
                {
                    rPressed = true;
                }
                else if (rPressed)
                {
                    Random random = new Random();
                    randomColor = new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                    rPressed = false;
                }
              
                if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.C) == true)
                {
                    cPressed = true;
                }
                if (cPressed && Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.C) == true)
                {
                    cPressed = false;
                    switchModi = true;
                }
                //---------------------------------------------------------------
                if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Up) == true)
                {
                    upPressed = true;
                }
                if (upPressed && Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.Up) == true)
                {
                    upPressed = false;
                    if (resetMatrixTimer > 1 / 8)
                    {
                        Console.WriteLine(resetMatrixTimer);
                        resetMatrixTimer -= 1 / 16f;
                        matrixTimer = resetMatrixTimer;
                    }

                }
                if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Down) == true)
                {
                    downPressed = true;
                }
                if (downPressed && Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.Down) == true)
                {
                    downPressed = false;

                    resetMatrixTimer += 1 / 16f;
                    matrixTimer = resetMatrixTimer;
                }

                // ---------------------------------------------------

                // Update Buttons
                //--- get Handpostions
                posHandLeft = SkeletonData.GetVector2Absolute(JointType.HandLeft);
                posHandRight = SkeletonData.GetVector2Absolute(JointType.HandRight);
                List<Vector2> handPos = new List<Vector2>();
                handPos.Add(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                handPos.Add(posHandRight);
                handPos.Add(posHandLeft);
                //---

                Vector3 leftHand = new Vector3(posHandLeft, SkeletonData.GetZ(JointType.HandLeft));
                Vector3 rightHand = new Vector3(posHandRight, SkeletonData.GetZ(JointType.HandRight));
                // pause Game
                //Gestures

                if(HandGesture.HideButtons()){
                    hideButtons = !hideButtons;
                }
                
                switchGenre = switchGenre||HandGesture.SwitchGenre();
                pauseGame = HandGesture.PauseGame();
                
                switchModi = switchModi||HandGesture.switchModi();

                if (switchGenre || switchModi)
                {
                    backgroundTimer = resetBackgroundTimer;
                }

                if (!pauseGame)
                {
                    foreach (Button tmp in toggleButtonList)
                    {
                        Color c = tmp.baseColor;
                        if (hideButtons)
                        {
                            tmp.hidden = true;
                        }
                        else if (!hideButtons)
                        {
                            tmp.hidden = false;
                        }

                        tmp.collision(rightHand.X,rightHand.Y,rightHand.Z,leftHand.X,leftHand.Y,leftHand.Z, SkeletonData.GetZ(JointType.HipCenter));
                    }
                    UpdateMidiButtons(handPos, gameTime);
                }

                if (switchModi)
                {
                    matrixSynth = !matrixSynth;
                    switchModi = false;
                }

                if (switchGenre)
                {
                    // change Sound Genre
                    Console.WriteLine("switch genre");
                    if (matrixSynth)
                    {
                        MIDIChannel.changeInstrument(1);
                    }
                    else
                    {
                        MIDIChannel.changeInstrument(0);
                    }
                }
                switchGenre = false;


                //MIDI Buttons & Star Background 
                // Midi hover buttons



                //calc scrollspeed for background
                posHandLeft -= (screen / 2);
                posHandRight -= (screen / 2);
                Vector2 scrollspeed = posHandLeft / 3;
                if (posHandRight.Length() > posHandLeft.Length())
                    scrollspeed = posHandRight / 3;

                particleEngineLeftHand.EmitterLocation = posHandLeft + screen / 2;
                particleEngineRightHand.EmitterLocation = posHandRight + screen / 2;


                particleEngineLeftHand.Update();
                particleEngineRightHand.Update();

                //SoundVisualizer
                soundVisualizer.Update();

                bg.Update(gameTime, scrollspeed);
                base.Update(gameTime);
            }
        }

        private void UpdateMidiButtons(List<Vector2> handPos, GameTime gameTime)
        {
            if (!matrixSynth)
            {
                foreach (MidiButton t in midiButtonList)
                {
                    int intersections = t.Collision(handPos);
                }
            }
            else
            {
                foreach (MidiButton t in midiButtonMatrix)
                {
                    int intersections = t.MatrixCollision(handPos);
                }
            }

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            matrixTimer -= elapsed;
            if (matrixTimer < 0)
            {
                //Timer expired, execute action
                for (int i = matrixIterator; i < midiButtonMatrix.Count(); i = i + MIDIChannel.GetPitchesLength())
                {
                    if (midiButtonMatrix[i].GetPushState())
                    {
                        midiButtonMatrix[i].PlayPitch();
                        debug = midiButtonMatrix[i].getPitch();
                    }
                    else
                    {
                        midiButtonMatrix[i].StopPitch();
                    }
                }
                //Update Iterator
                Vector2 position = placeIndicator(matrixIterator, 650, 650, 8);
                indicatorRect.X = (int)position.X - (INDICATOR_WIDTH / 2);
                indicatorRect.Y = (int)position.Y - (INDICATOR_HEIGHT / 2);

                matrixIterator = (matrixIterator + 1) % MIDIChannel.GetPitchesLength();
                matrixTimer = resetMatrixTimer;   //Reset Timer
            }

        }

        private void updateBackground(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            backgroundTimer -= elapsed;
            if (backgroundTimer < 0 && pauseGame)
            {
                backgroundColor = Color.White*0.2f;
            }
            else if (backgroundTimer < 0)
            {
                backgroundColor = randomColor;
            }
            else
            {
                backgroundColor = Color.Purple * (backgroundTimer);
            }
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            updateBackground(gameTime);
            // TODO: Add your drawing code here
            GraphicsDevice.Clear(backgroundColor);

            if (displayIntro)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(intro, new Rectangle(0, 0, (int)screen.X, (int)screen.Y), Color.White);
                spriteBatch.End();
            }
            else
            {
                /*
                spriteBatch.Begin();
                //Debug-String
                spriteBatch.DrawString(font, "" + debug, new Vector2(30, 40), Color.White);

                spriteBatch.End();
                */
                base.Draw(gameTime);
                // Draws the Starbackground
                bg.Draw(spriteBatch);

                //SoundVisualizer
                soundVisualizer.Draw(spriteBatch, font);


                foreach (Button tmp in toggleButtonList)
                {
                    tmp.draw(spriteBatch);
                }


                //Draw indicator
                if (matrixSynth)
                {
                    // midi matrix Buttons
                    foreach (MidiButton t in midiButtonMatrix)
                    {
                        //to show collision rects: add graphicsdevice as 2nd param.
                        t.Draw(spriteBatch);
                    }
                    spriteBatch.Begin();
                    spriteBatch.Draw(indicatorTexture, indicatorRect, Color.White);
                    spriteBatch.End();
                }
                else
                {
                    // midi hover Buttons
                    foreach (MidiButton t in midiButtonList)
                    {
                        //to show collision rects: add graphicsdevice as 2nd param.
                        t.Draw(spriteBatch);
                    }
                }

                particleEngineLeftHand.Draw(spriteBatch);
                particleEngineRightHand.Draw(spriteBatch);
            }
        }

        //init skeleton
        void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs skeletonFrames)
        {
            SkeletonData.Update(skeletonFrames);
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            soundVisualizer.stopVisualizer();
            Console.WriteLine("Exiting from: " + sender.ToString() + " " + args.ToString());
            // Stop the threads
            if (kinect != null)
            {
                kinect.Stop();
                kinect = null;
            }

        }

        private void placeMidiButtons(int octave, int startX, int startY, int radiusFactor, List<MidiButton> list, int channel, Texture2D texture)
        {
            int pitches = MIDIChannel.GetPitchesLength() - 1;
            int width = 80;
            int height = 80;
            int numberOfButtons = MIDIChannel.GetPitchesLength() - 1;
            double angle = -(Math.PI) / numberOfButtons;
            double radiusX = radiusFactor + width;
            double radiusY = radiusFactor + height;
            int angleMultiplier = 0;
            float alphaValue = 0.8f;

            for (int p = MIDIChannel.GetPitchesLength() - 1; p >= 0; p--)
            {
                angleMultiplier = p;
                double midPosX = (Math.Cos(angle * angleMultiplier) * radiusX) + startX;
                double midPosY = (Math.Sin(angle * angleMultiplier) * radiusY) + startY;

                Color buttonColor = new Color(200, (int)(255 / ((octave + 1) * 0.3f)), (int)(255 / ((p + 1) * 0.3f)), 255);
                MidiButton midiButton = new MidiButton(pitches - p, octave, new Rectangle((int)midPosX, (int)midPosY, width, height), buttonColor * alphaValue, texture, channel);
                midiButton.CenterButton();
                list.Add(midiButton);

            }
        }

        private Vector2 placeIndicator(int p, int startX, int startY, int radiusFactor)
        {
            int width = 80;
            int height = 80;
            int numberOfButtons = MIDIChannel.GetPitchesLength() - 1;
            double angle = -(Math.PI) / numberOfButtons;
            double radiusX = radiusFactor * numberOfButtons + width;
            double radiusY = radiusFactor * numberOfButtons + height;
            int angleMultiplier = 0;

            angleMultiplier = (MIDIChannel.GetPitchesLength() - 1) - p;
            double midPosX = (Math.Cos(angle * angleMultiplier) * radiusX) + startX;
            double midPosY = (Math.Sin(angle * angleMultiplier) * radiusY) + startY;
            return new Vector2((float)midPosX, (float)midPosY);
        }
    }
}
