using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Devices.Sensors;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using System.Diagnostics;

namespace _7Circles
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Motion _motion;
        World world;
        int xa = 0, ya = 1, za = 1;
        Body rectangle;
        ParticleEngine particleEngine;

        Texture2D rectangleSprite, background;

        public Game1()
        {
            if (Motion.IsSupported)
            {
                _motion = new Motion();
            }
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            graphics.IsFullScreen = true;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeRight;
            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
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
            if (_motion != null)
            {
                _motion.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<MotionReading>>(_motion_CurrentValueChanged);
                _motion.Start();
            }
            base.Initialize();
        }

        void _motion_CurrentValueChanged(object sender, SensorReadingEventArgs<MotionReading> e)
        {
            rectangle.Rotation = -e.SensorReading.Attitude.Pitch;
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(Content.Load<Texture2D>("circle3"));

            particleEngine = new ParticleEngine(textures, new Vector2(200, 140));

            if (world == null)
            {
                world = new World(new Vector2(xa, ya) * za);

            }
            else
            {
                world.Clear();
            }



            // Change Color.Red to the colour you want
            rectangle = BodyFactory.CreateCircle(world, 1f, 1f, 1.0f);
            rectangle.BodyType = BodyType.Dynamic;
            rectangle.Position = ConvertUnits.ToSimUnits(44, 33);  // Allows the game to exit

            rectangleSprite = Content.Load<Texture2D>("circle4");

            background = Content.Load<Texture2D>("firmamento3");




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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            foreach (TouchLocation location in TouchPanel.GetState())
            {
                rectangle.Position = ConvertUnits.ToSimUnits(location.Position.X, location.Position.Y);
                world.ClearForces();
                za = 0;
                xa = 0;
                ya = 0;
                world = new World(new Vector2(xa, ya) * za);
                Debug.WriteLine(world.ToString());
            }

            if (rectangle.Rotation >= 1)
            {

                za = -1;
                xa = 0;
                ya = 1;

            }
            else if (rectangle.Rotation > 0 && rectangle.Rotation < 1)
            {
                za = 1;
                xa = 1;
                ya = 0;
            }
            else if (rectangle.Rotation <= 0)
            {
                za = 1;
                xa = 0;
                ya = 1;
            }
            world.Gravity = new Vector2(xa, ya) * za;
            particleEngine.EmitterLocation = ConvertUnits.ToDisplayUnits(rectangle.Position.X, rectangle.Position.Y);
            particleEngine.Update();

            /* if (rectangle.Position.Y > ConvertUnits.ToSimUnits(405))
             {
                 rectangle.ApplyForce(new Vector2(0, -20));
             }
             */
            world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin(0, BlendState.Opaque);
            spriteBatch.Draw(background, GraphicsDevice.Viewport.Bounds, Color.White);
            spriteBatch.End();
            particleEngine.Draw(spriteBatch);
            spriteBatch.Begin();
            spriteBatch.Draw(rectangleSprite, ConvertUnits.ToDisplayUnits(rectangle.Position),
                                            null,
                                            Color.White, rectangle.Rotation, new Vector2(rectangleSprite.Width / 2.0f, rectangleSprite.Height / 2.0f), 0.5f,
                                            SpriteEffects.None, 0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}