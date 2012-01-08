using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Frostbyte
{
    internal delegate void LoadBehavior(Level context);
    internal delegate void Behavior();
    internal delegate bool Condition();

    /// <summary>
    /// High-level controller for the game
    /// This class is tasked with the following:
    /// - creating the SDL screen
    /// - loading levels from appropriate locations
    /// - switching levels as appropriate
    /// </summary>
    internal partial class Game : Microsoft.Xna.Framework.Game
    {
        #region Properties
        /// <summary>
        /// Decides whether or not to draw bounding boxes
        /// </summary>
        internal static bool ShowCollisionData { get { return Collision.ShowCollisionData; } set { Collision.ShowCollisionData = value; } }
        /// <summary>
        /// Decides whether or not to draw FPS
        /// </summary>
        internal bool ShowFPS { get; set; }

        /// <summary>
        /// Gets the current level being played.
        /// </summary>
        internal Level CurrentLevel
        {
            get
            {
                if (mCurrentLevel < mLevels.Count)
                {
                    return mLevels[mCurrentLevel];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the level that we should be loading to.
        /// Horrible hack because Sprites "automagically" add themselves to the CurrentLevel.
        /// </summary>
        internal Level LoadingLevel
        {
            get
            {
                return CurrentLevel != NextLevel && NextLevel != null ? NextLevel : CurrentLevel;
            }
        }

        /// <summary>
        /// Gets the next level to be played (the one loading).
        /// </summary>
        internal Level NextLevel
        {
            get
            {
                if (mNextLevel >= 0 && mNextLevel < mLevels.Count)
                {
                    return mLevels[mNextLevel];
                }
                else
                {
                    return null;
                }
            }
        }

        internal AudioManager AudioManager
        {
            get
            {
                return mAudioManager;
            }
        }

        /// <summary>
        /// Gets the index of the current level being played.
        /// </summary>
        internal int LevelIndex
        {
            get
            {
                return mCurrentLevel;
            }

        }

        /// <summary>
        /// Gets the total count of levels.
        /// </summary>
        int LevelCount { get { return mLevels.Count; } }

        /// <summary>
        /// Returns the current FPS of the game
        /// </summary>
        int FPS { get { return currentFPS; } }
        #endregion Properties

        #region Variables

        #region FPS calc
        /// <summary>
        /// \todo implement or get rid of these
        /// </summary>
        int currentFPS;
        #endregion FPS calc
        /// <summary>
        /// The next level (for loading)
        /// </summary>
        int mNextLevel = -1;
        int mCurrentLevel = -1;/**< Current Level index. */
        AudioManager mAudioManager = new AudioManager();
        List<Level> mLevels = new List<Level>();
        internal Random rand = new Random();

        /// <summary>
        /// Used to detect keypresses (down-up)
        /// </summary>
        internal KeyboardState mLastKeyState;

        /// <summary>
        /// Used to detect gamepad State (down-up)
        /// </summary>
        internal GamePadState mLastPadStateP1;
        internal GamePadState mLastPadStateP2;
        #endregion Variables

        #region premade things
        internal GraphicsDeviceManager graphics;
        internal SpriteBatch spriteBatch;
        #endregion

        #region Constructor
        internal Game()
        {
            graphics = new GraphicsDeviceManager(this);
            //remove this to make it strech
            //graphics.PreparingDeviceSettings+=new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 750;
        }
        #endregion Constructor

        #region Initialization
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // todo: Add your initialization logic here

            base.Initialize();

            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 750;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            
            //Initialize Collision Cell Size
            Collision.CellHeight = This.CellSize;
            Collision.CellWidth = This.CellSize;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // todo: use this.Content to load your game content here
            LoadResources();

            SetCurrentLevel(null);
        }
        #endregion Initialization

        #region Destruction
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // todo: Unload any non ContentManager content here
        }
        #endregion Destruction

        #region Updating
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            This.gameTime = gameTime;

            #region Handle input
            // Allows the game to exit
            KeyboardState keyState = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            //close game
            if (padState.Buttons.Back == ButtonState.Pressed || keyState.IsKeyDown(Keys.Escape))
                this.Exit();
            //show collisions
            if (mLastKeyState.IsKeyDown(Keys.F11) && keyState.IsKeyUp(Keys.F11))
                ShowCollisionData = !ShowCollisionData;
            //enable cheats
            if (mLastKeyState.IsKeyDown(Keys.F9) && keyState.IsKeyUp(Keys.F9))
            {
                This.Cheats.GetCheat("SpawnEnemies").Toggle();
            }
            if (mLastKeyState.IsKeyDown(Keys.F1) && keyState.IsKeyUp(Keys.F1))
            {
                graphics.ToggleFullScreen();
            }
            #endregion Handle input


            if (CurrentLevel != null)
                CurrentLevel.Update();

            #region FPS
            if (gameTime.ElapsedGameTime.Milliseconds > 0)
                currentFPS = 1000 / gameTime.ElapsedGameTime.Milliseconds;
            #endregion FPS

            //store key and pad state
            mLastPadStateP1 = padState;
            mLastPadStateP2 = GamePad.GetState(PlayerIndex.Two);
            mLastKeyState = keyState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (CurrentLevel != null)
                CurrentLevel.Draw(gameTime);

            #region DrawFPS
            if (ShowFPS)
            {

            }
            #endregion DrawFPS

            base.Draw(gameTime);
        }
        #endregion Updating

        #region Methods
        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = displayMode.Format;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = displayMode.Width;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = displayMode.Height;

        }

        /// <summary>
        /// Sets the current level.
        /// </summary>
        /// <param name="name"></param>
        internal void SetCurrentLevel(string name)
        {
            if (name == null && mCurrentLevel < 0)
            {
                name = mLevels[0].Name;
            }

            //if this is the expected level to load load it.
            if (mNextLevel >= 0 && mLevels[mNextLevel].Name == name)
            {
                mCurrentLevel = mNextLevel;
            }
            //otherwise load the level with the given name
            else
            {
                for (int i = 0, count = mLevels.Count; i < count; i++)
                {
                    if (mLevels[i].Name == name)
                    {
                        //reset next level so we don't do something stupid
                        mNextLevel = -1;
                        mCurrentLevel = i;
                        mLevels[mCurrentLevel].Load(mLevels[mCurrentLevel]);
                        return;
                    }
                }
                throw new Exception(string.Format("Level {0} does not exist", name));
            }
        }

        /// <summary>
        /// Loads the specified level
        /// </summary>
        /// <param name="name">name of the level to load</param>
        internal void LoadLevel(string name)
        {
            for (int i = 0, count = mLevels.Count; i < count; i++)
            {
                if (mLevels[i].Name == name)
                {
                    mNextLevel = i;
                    mLevels[mNextLevel].Load(mLevels[mNextLevel]);
                    return;
                }
            }
            Console.WriteLine(string.Format("Level {0} does not exist", name));
        }
        #endregion Methods
    }
}
