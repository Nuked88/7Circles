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
       
        Body rectangle;
        ParticleEngine particleEngine;
            //  static background &stuff
         Texture2D rectangleSprite, background;

            // bg Layers
            bgparallax bgl1;
            bgparallax bgl2;
      

        public Game1()
        {
            if (Motion.IsSupported)
            {
                _motion = new Motion();
            }

           


            //other
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.IsFullScreen = true;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeRight;
            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

     

        protected override void Initialize()
        {allvar.posizione = new Vector2(allvar.bgy, 240f);
            // TODO: Add your initialization logic here
            if (_motion != null)
            {
                _motion.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<MotionReading>>(_motion_CurrentValueChanged);
                _motion.Start();
            }
            bgl1 = new bgparallax();
            bgl2 = new bgparallax();

            base.Initialize();
        }

        void _motion_CurrentValueChanged(object sender, SensorReadingEventArgs<MotionReading> e)
        {
            rectangle.Rotation = -e.SensorReading.Attitude.Pitch;
        }
        



        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            List<Texture2D> textures = new List<Texture2D>();
            //current bg
           background = Content.Load<Texture2D>("bgbase");

            //future bg          
            bgl1.Initialize(Content, "bglayer1", GraphicsDevice.Viewport.Width, -1);
            bgl2.Initialize(Content, "bglayer2", GraphicsDevice.Viewport.Width, -2);

          



            particleEngine = new ParticleEngine(textures, new Vector2(200, 140));

            if (world == null)
            {
             //   world = new World(-Vector2.UnitY* allvar.za);
                world = new World(new Vector2(allvar.xa, allvar.ya) * allvar.za);
            }
            else
            {
                world.Clear();
            }


 textures.Add(Content.Load<Texture2D>("circle3"));
            // Change Color.Red to the colour you want
            rectangle = BodyFactory.CreateCircle(world, 1f, 1f, 1.0f);
            rectangle.BodyType = BodyType.Dynamic;
            rectangle.Position = ConvertUnits.ToSimUnits(44, 30);  // Allows the game to exit

            rectangleSprite = Content.Load<Texture2D>("circle4");

            




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
            //scrolling bg
            bgl1.Update();
            bgl2.Update();

            allvar.bgy = allvar.bgy - 0.3f;
            allvar.posizione = new Vector2(allvar.bgy, 240f);
       
            foreach (TouchLocation location in TouchPanel.GetState())
            {
                rectangle.Position = ConvertUnits.ToSimUnits(location.Position.X, location.Position.Y);
                rectangle.LinearVelocity=new Vector2(0,0);
          
                
                
              //  Debug.WriteLine(rectangle.Position.ToString());

              }

          if (rectangle.Rotation >= 1)
            {

                allvar.za = -1;
                allvar.xa = 0;
                allvar.ya = 1;

            }
            else if (rectangle.Rotation > 0 && rectangle.Rotation < 1)
            {
                allvar.za = 1;
                allvar.xa = 1;
                allvar.ya = 0;
            }
            else if (rectangle.Rotation <= 0)
            {
                allvar.za = 1;
                allvar.xa = 0;
                allvar.ya = 1;
            }
          world.Gravity = new Vector2(allvar.xa, allvar.ya) * allvar.za;
            particleEngine.EmitterLocation = ConvertUnits.ToDisplayUnits(rectangle.Position.X, rectangle.Position.Y);
            particleEngine.Update();

          if (rectangle.Position.Y > ConvertUnits.ToSimUnits(405))
             {
                 world.ClearForces();
            
                rectangle.ApplyForce(new Vector2(0, -20));
             }
           else if (rectangle.Position.Y <= ConvertUnits.ToSimUnits(150))
           {
               world.ClearForces();
               rectangle.ApplyForce(new Vector2(0, 20));
           }
           else if (rectangle.Position.X > ConvertUnits.ToSimUnits(725))
           {
               world.ClearForces();
               rectangle.ApplyForce(new Vector2(-20,0 ));
           }
           else if (rectangle.Position.X <= ConvertUnits.ToSimUnits(150))
           {
               world.ClearForces();
               rectangle.ApplyForce(new Vector2(20,0 ));
           }
             
          
       
            world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f, (1f / 30f)));

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // GraphicsDevice.Clear(Color.White);
         /*   spriteBatch.Begin(0, BlendState.Opaque);
            spriteBatch.Draw(background, GraphicsDevice.Viewport.Bounds, Color.White);
            spriteBatch.Draw(background, allvar.posizione, null, Color.White, 0,new Vector2(background.Width / 2.0f, background.Height / 2.0f), 1, SpriteEffects.None, 0);
            spriteBatch.End();*/
           spriteBatch.Begin(); 
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            
            // Draw the moving background
            bgl1.Draw(spriteBatch);
            bgl2.Draw(spriteBatch);


            spriteBatch.Draw(rectangleSprite, ConvertUnits.ToDisplayUnits(rectangle.Position),
                                            null,
                                            Color.White, 0, new Vector2(rectangleSprite.Width / 2.0f, rectangleSprite.Height / 2.0f), 0.5f,
                                            SpriteEffects.None, 0f);
            spriteBatch.End();
particleEngine.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}