using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Xml.Linq;
using Frostbyte.Enemies;

namespace Frostbyte
{
    /// <summary>
    /// Do anything required for Game-specific code here
    /// to avoid cluttering up the Engine
    /// </summary>
    internal class FrostbyteGame : Game
    {
        internal IController GlobalController;

        public FrostbyteGame()
            : base()
        {
            GlobalController = new KeyboardController();
            This.Cheats.AddCheat("DisableBackgroundCollision");
            This.Cheats.AddCheat("FinalBossFun");
            This.Cheats.AddCheat("ElementalBuffs");
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            SetCurrentLevel(FrostbyteLevel.LevelProgression[0]);
        }

        protected override void Update(GameTime gameTime)
        {
            GlobalController.Update();
            base.Update(gameTime);

            if (GlobalController.GetKeypress(Keys.Home) == ReleasableButtonState.Clicked)
            {
                This.Cheats.Enabled = !This.Cheats.Enabled;
            }
            if (GlobalController.GetKeypress(Keys.PageUp) == ReleasableButtonState.Clicked)
            {
                This.Cheats.GetCheat("DisableBackgroundCollision").Toggle();
            }
            if (GlobalController.GetKeypress(Keys.K) == ReleasableButtonState.Clicked && This.Cheats.Enabled)
            {
                Player.ItemBag.Add(new Key("cheat"));
            }
            if (GlobalController.GetKeypress(Keys.End) == ReleasableButtonState.Clicked && This.Cheats.Enabled)
            {
                Characters.Mage.UnlockedSpells = Spells.All;
            }
            if (GlobalController.GetKeypress(Keys.B) == ReleasableButtonState.Clicked && This.Cheats.Enabled)
            {
                This.Cheats.GetCheat("ElementalBuffs").Toggle();
            }
        }
    }

    /// <summary>
    /// Enables sorting Sprite lists by distance from an origin Sprite
    /// </summary>
    internal class DistanceSort : IComparer<Sprite>
    {
        Sprite origin;

        internal DistanceSort(Sprite origin)
        {
            this.origin = origin;
        }

        int IComparer<Sprite>.Compare(Sprite x, Sprite y)
        {
            double lx = (x.GroundPos - origin.GroundPos).LengthSquared();
            double ly = (y.GroundPos - origin.GroundPos).LengthSquared();
            if (lx > ly)
            {
                return 1;
            }
            else if (lx < ly)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Add Game-specific level code here to avoid cluttering up the Engine
    /// </summary>
    class FrostbyteLevel : Level
    {
        #region Variables
        /// <summary>
        /// List of Allies that may be targeted
        /// </summary>
        internal List<Sprite> allies = new List<Sprite>();
        /// <summary>
        /// List of Enemy targets
        /// </summary>
        internal List<Sprite> enemies = new List<Sprite>();
        /// <summary>
        /// List of Obstacles that may be targeted
        /// </summary>
        internal List<Sprite> obstacles = new List<Sprite>();

        /// <summary>
        /// List of Diary entry text
        /// </summary>
        internal IEnumerator<string> DiaryEntries = null;

        internal TileList TileMap = new TileList();

        /// <summary>
        /// the default type of tile for the current level
        /// </summary>
        public Tile TopAreaTile = null;

        //private Polygon viewportPolygon = null;
        internal bool AutoZoom = false;

        /// <summary>
        /// The HUD manager for our display
        /// </summary>
        internal HUD HUD = new HUD();

        /// <summary>
        /// A list of levels in the order they should be played through
        /// </summary>
        internal static List<string> LevelProgression = new List<string>()
        {
            "TitleScreen",
            "Intro",
            "WorldMap",
            "Earth",
            "WorldMap",
            "Lightning",
            "WorldMap",
            "Water",
            "WorldMap",
            "Fire",
            "FireBoss",
            "Final",
            "Credits"
        };

        /// <summary>
        /// Retains progress through our levels
        /// </summary>
        internal static int CurrentStage = 0;

        internal Vector2? ExitPortalSpawnPoint = null;
        internal bool LevelCompleted = false;
        internal bool isPauseEnabled = false;
        #endregion

        #region Properties
        /// <summary>
        /// TL of area that will be visible on screen
        /// </summary>
        private Vector3 StartDraw { get; set; }
        /// <summary>
        /// BR of the area that will be visible on screen
        /// </summary>
        private Vector3 EndDraw { get; set; }
        /// <summary>
        /// The level's theme defaults to Earth
        /// </summary>
        public Element Theme { get; set; }
        #endregion Properties

        #region Constants
        private static readonly float MAX_ZOOM = 1.0f;
        private static readonly float MIN_ZOOM = 0.8f;
        internal static readonly int BORDER_WIDTH = 200;
        internal static readonly int BORDER_HEIGHT = 200;
        #endregion

        #region Constructors
        internal Vector2[] PlayerSpawnPoint = new Vector2[2]{
            new Vector2(50, 50),
            new Vector2(60, 50)
        };

        internal FrostbyteLevel(string n, LoadBehavior loadBehavior, Behavior updateBehavior,
            Behavior endBehavior, Condition winCondition = null)
            : base(n, loadBehavior, updateBehavior, endBehavior, winCondition)
        {
            if (winCondition == null)
            {
                WinCondition = delegate() { return LevelCompleted; };
            }
        }
        #endregion

        #region Methods
        internal void SpawnExitPortal(Vector2? spawnPosition=null)
        {
            if (spawnPosition.HasValue)
            {
                ExitPortalSpawnPoint = spawnPosition.Value;
            }

            if (ExitPortalSpawnPoint.HasValue)
            {

                SimpleDistanceTrigger t = new SimpleDistanceTrigger("portal", Tile.TileSize / 2);
                t.CenterOn(ExitPortalSpawnPoint.Value);
                t.TriggerCondition = delegate()
                {
                    if (t.SpritesInRange.Count > 0)
                    {
                        return new TriggerEventArgs();
                    }
                    else
                    {
                        return null;
                    }
                };

                t.TriggerEffect += delegate(Object o, TriggerEventArgs args)
                {
                    LevelCompleted = true;
                };
                NextLevelPortal c = new NextLevelPortal("next_level", Tile.TileSize / 2);
                c.CenterOn(ExitPortalSpawnPoint.Value);
                This.Game.AudioManager.AddSoundEffect("Effects/Regen");
                This.Game.AudioManager.PlaySoundEffect("Effects/Regen");
            }
            else
            {
                LevelCompleted = true;  // No place to put the portal, just send them on.
            }
        }

        internal override void Load(Level context)
        {
            base.Load(context);
            LevelCompleted = false;
            foreach (WorldObject s in ToAdd)
            {
                OurSprite o = s as OurSprite;
                if (o != null)
                {
                    o.Respawn();
                }
            }
        }

        /// <summary>
        /// Iterates through every player onscreen to gather the minimum and maximum X and Y coordinates
        /// for any of the players. The new zoom/scale factor is calculated and then the viewport is shifted
        /// if need be.
        /// </summary>
        internal void RealignViewport()
        {

            #region Create Viewport
            /*if (viewportPolygon == null)
            {
                Viewport viewport = This.Game.GraphicsDevice.Viewport;
                viewportPolygon = new Polygon("viewport", Color.DarkRed, new Vector3[5]{
                    new Vector3(BORDER_WIDTH, BORDER_HEIGHT, 0), 
                    new Vector3(viewport.Width - BORDER_WIDTH, BORDER_HEIGHT, 0), 
                    new Vector3(viewport.Width - BORDER_WIDTH, viewport.Height - BORDER_HEIGHT, 0), 
                    new Vector3(BORDER_WIDTH, viewport.Height - BORDER_HEIGHT, 0),
                    new Vector3(BORDER_WIDTH, BORDER_HEIGHT, 0)
                });
                viewportPolygon.Static = true;
            }
            */
            #endregion

            List<Sprite> players = GetSpritesByType(typeof(Player)).Where(x => x.State != SpriteState.Dead).ToList();
            if (players != null && players.Count > 0)
            {
                Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
                Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

                #region Find Min and Max
                foreach (Sprite player in players)
                {
                    if (player.Pos.X < min.X)
                    {
                        min.X = (int)player.Pos.X;
                    }
                    if (player.Pos.Y < min.Y)
                    {
                        min.Y = (int)player.Pos.Y;
                    }
                    if (player.Pos.X + player.GetAnimation().Width > max.X)
                    {
                        max.X = (int)(player.Pos.X + player.GetAnimation().Width);
                    }
                    if (player.Pos.Y + player.GetAnimation().Height > max.Y)
                    {
                        max.Y = (int)(player.Pos.Y + player.GetAnimation().Height);
                    }
                }
                #endregion

                Viewport viewport = This.Game.GraphicsDevice.Viewport;
                float zoom = This.Game.CurrentLevel.Camera.Zoom;
                float scaleX = float.PositiveInfinity;
                float scaleY = float.PositiveInfinity;
                if (AutoZoom)
                {
                    #region Calculate Zoom Factor
                    Vector2 span = max - min;
                    scaleX = (viewport.Width - 2 * BORDER_WIDTH) / span.X;
                    scaleY = (viewport.Height - 2 * BORDER_HEIGHT) / span.Y;
                    zoom = Math.Min(scaleX, scaleY);

                    // Normalize values if necessary
                    zoom = zoom > MAX_ZOOM ? MAX_ZOOM : zoom;
                    zoom = zoom < MIN_ZOOM ? MIN_ZOOM : zoom;
                    #endregion
                }

                Vector2 cameraPos = This.Game.CurrentLevel.Camera.Pos;

                #region Shift Viewport
                Vector2 topLeftCorner = min - cameraPos;
                Vector2 bottomRightCorner = max - cameraPos;
                if (scaleX >= MIN_ZOOM)
                {
                    if (topLeftCorner.X <= viewport.X + BORDER_WIDTH / zoom)
                    {
                        cameraPos.X += (int)(topLeftCorner.X - (viewport.X + BORDER_WIDTH) / zoom);
                    }
                    else if (bottomRightCorner.X > viewport.X + (viewport.Width - BORDER_WIDTH) / zoom)
                    {
                        cameraPos.X += (int)Math.Min(
                            topLeftCorner.X - (viewport.X + BORDER_WIDTH) / zoom, 
                            bottomRightCorner.X - (viewport.X + (viewport.Width - BORDER_WIDTH) / zoom));
                    }
                }
                if (scaleY >= MIN_ZOOM)
                {
                    if (topLeftCorner.Y <= viewport.Y + BORDER_HEIGHT / zoom)
                    {
                        cameraPos.Y += (int)(topLeftCorner.Y - (viewport.Y + BORDER_HEIGHT) / zoom);
                    }
                    else if (bottomRightCorner.Y > viewport.Y + (viewport.Height - BORDER_HEIGHT) / zoom)
                    {
                        cameraPos.Y += (int)Math.Min(
                            bottomRightCorner.Y - (viewport.Y + (viewport.Height - BORDER_HEIGHT) / zoom),
                            topLeftCorner.Y - (viewport.Y + BORDER_WIDTH) / zoom);
                    }
                }
                #endregion

                This.Game.CurrentLevel.Camera.Pos = cameraPos;
                This.Game.CurrentLevel.Camera.Zoom = zoom;
            }
        }

        internal override void Update()
        {
            This.Game.AudioManager.StopAllLoopingSoundEffects();

            List<Sprite> livingPlayers = (This.Game.CurrentLevel as FrostbyteLevel).allies.
                Where(x => x.State != SpriteState.Dead).ToList();
            if (livingPlayers.Count == 0)
            {
                foreach (WorldObject sprites in mWorldObjects)
                {
                    OurSprite s = sprites as OurSprite;
                    if (s != null)
                    {
                        s.Respawn();
                    }
                }
            }

            base.Update();

            RealignViewport();

            Vector3 cameraPosition = new Vector3(Camera.Pos, 0);
            Viewport viewport = This.Game.GraphicsDevice.Viewport;
            float zoom = This.Game.CurrentLevel.Camera.Zoom;

            StartDraw = (cameraPosition + new Vector3(viewport.X, viewport.Y, 0)) / Tile.TileSize;
            EndDraw = (cameraPosition + new Vector3(viewport.X + viewport.Width / zoom,
                                                viewport.Y + viewport.Height / zoom, 0)) / Tile.TileSize;

            if ((This.Game as FrostbyteGame).GlobalController.NextLevel == ReleasableButtonState.Clicked)
            {
                Unload();
            }
        }

        internal override void Draw(GameTime gameTime, bool drawAfter = false)
        {
            List<Tile> drawLater = new List<Tile>();

            if (TileMap.HasItems)
            {
                #region Draw Base Tiles
                //draw base tiles
                This.Game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.GetTransformation(This.Game.GraphicsDevice));

                for (int y = (int)Math.Floor(StartDraw.Y); y < (int)Math.Ceiling(EndDraw.Y); y++)
                {
                    for (int x = (int)Math.Floor(StartDraw.X); x < (int)Math.Ceiling(EndDraw.X); x++)
                    {
                        Tile toDraw;
                        TileMap.TryGetValue(x, y, out toDraw);

                        //toDraw.Draw();
                        if (!(toDraw.Type == TileTypes.Bottom || toDraw.Type == TileTypes.BottomConvexCorner || toDraw.Type == TileTypes.DEFAULT || toDraw.Type == TileTypes.TopArea))
                            toDraw.Draw();
                        else
                        {
                            drawLater.Add(toDraw);
                            if (toDraw.Type == TileTypes.Bottom || toDraw.Type == TileTypes.BottomConvexCorner)
                            {
                                //if it's somethign that needs floor draw me a floor piece
                                Tile t = new Tile()
                                {
                                    Type = TileTypes.Floor,
                                    FloorType = TileTypes.Floor,
                                    Orientation = Orientations.Down,
                                    GridCell = toDraw.GridCell,
                                    Theme=toDraw.Theme,
                                };
                                t.Draw();
                            }
                        }
                    }
                }

                This.Game.spriteBatch.End();

                #endregion
            }
            base.Draw(gameTime, true);

            if (TileMap.HasItems)
            {
                #region draw covering tiles
                This.Game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.GetTransformation(This.Game.GraphicsDevice));
                foreach (Tile tile in drawLater)
                {
                    tile.Draw();
                }

                This.Game.spriteBatch.End();
                #endregion draw covering tiles

            }
            #region Draw Static Sprites
            if (staticSprites.Count > 0)
            {
                This.Game.spriteBatch.Begin();

                foreach (var sprite in staticSprites)
                {
                    sprite.Draw(gameTime);
                }

                This.Game.spriteBatch.End();
            }
            #endregion

        }

        internal override void Unload()
        {
            TileMap.Clear();
            allies.Clear();
            enemies.Clear();
            obstacles.Clear();
            if (DiaryEntries != null)
            {
                DiaryEntries.Dispose();
            }
            base.Unload();
        }

        private void createParticlesInCircle(ParticleEmitter emitter, int maxNumToCreate, float radius, Vector2 circleOrigin)
        {
            Random rand = new Random(This.gameTime.TotalGameTime.Milliseconds);
            double positionAngle = ((This.gameTime.TotalGameTime.TotalMilliseconds % 8000.0) / 8000.0) * Math.PI * 2;
            Vector2 position = new Vector2((float)Math.Cos(positionAngle) * radius, (float)Math.Sin(positionAngle) * radius) + circleOrigin;

            for (int i = 0; i < maxNumToCreate; i++)
            {
                double velocityAngle = rand.NextDouble() * Math.PI * 2;
                float velocitySpeed = rand.Next(2, 15);
                double accelAngle = rand.NextDouble() * Math.PI * 2;
                float accelSpeed = rand.Next(2, 15);
                emitter.createParticles(new Vector2((float)Math.Cos(velocityAngle) * velocitySpeed, (float)Math.Sin(velocityAngle) * velocitySpeed),
                                new Vector2((float)Math.Cos(accelAngle) * accelSpeed, (float)Math.Sin(accelAngle) * accelSpeed),
                                position,
                                rand.Next(5, 20),
                                rand.Next(1000, 4000));
            }
        }

        private void createParticlesLikeFlame(ParticleEmitter emitter, int maxNumToCreate, Vector2 flameOrigin)
        {
            Random rand = new Random(This.gameTime.TotalGameTime.Milliseconds);
            Vector2 position = new Vector2((float)rand.Next(0, 20), (float)rand.Next(0, 20)) + flameOrigin;

            for (int i = 0; i < maxNumToCreate; i++)
            {
                double velocityAngle = rand.NextDouble() * Math.PI * .5 + Math.PI / 4;
                float velocitySpeed = rand.Next(2, 15);
                double accelAngle = rand.NextDouble() * Math.PI * .25 + (Math.PI * 3) / 8;
                float accelSpeed = rand.Next(2, 15);
                emitter.createParticles(new Vector2((float)Math.Cos(velocityAngle) * velocitySpeed, (float)Math.Sin(velocityAngle) * velocitySpeed),
                                new Vector2((float)Math.Cos(accelAngle) * accelSpeed, (float)Math.Sin(accelAngle) * accelSpeed),
                                position,
                                rand.Next(5, 20),
                                rand.Next(1000, 4500));
            }
        }

        private void createParticlesLikeFlamingRing(ParticleEmitter emitter, int maxNumToCreate, float radius, Vector2 flameOrigin)
        {
            Random rand = new Random(This.gameTime.TotalGameTime.Milliseconds);

            for (int i = 0; i < maxNumToCreate; i++)
            {
                double positionAngle = rand.NextDouble() * Math.PI * 2;
                Vector2 position = new Vector2((float)Math.Cos(positionAngle) * radius, (float)Math.Sin(positionAngle) * radius) + flameOrigin;
                double velocityAngle = rand.NextDouble() * Math.PI * .25 + (Math.PI * 3) / 8;
                float velocitySpeed = rand.Next(2, 15);
                double accelAngle = rand.NextDouble() * Math.PI * .25 + (Math.PI * 3) / 8;
                float accelSpeed = rand.Next(2, 15);
                emitter.createParticles(new Vector2((float)Math.Cos(velocityAngle) * velocitySpeed, (float)Math.Sin(velocityAngle) * velocitySpeed),
                                new Vector2((float)Math.Cos(accelAngle) * accelSpeed, (float)Math.Sin(accelAngle) * accelSpeed),
                                position,
                                rand.Next(5, 20),
                                rand.Next(1000, 4000));
            }
        }


        internal void Load(XDocument doc)
        {
            TileMap = new TileList(doc);
            foreach (XElement elem in doc.Descendants("Enemy"))
            {
                string type = elem.Attribute("Type").Value;
                Type t = Type.GetType(type);
                var obj = Activator.CreateInstance(t, new object[] { elem.Attribute("Name").Value, Index2D.Parse(elem.Attribute("Pos").Value).Vector });
            }
            foreach (XElement elem in doc.Descendants("Object"))
            {
                string type = elem.Attribute("Type").Value;
                Type t = Type.GetType(type);
                if (elem.Attribute("Orientation") != null)
                {
                    var obj = Activator.CreateInstance(t, new object[] { elem.Attribute("Name").Value, Index2D.Parse(elem.Attribute("Pos").Value).Vector, (Orientations)Enum.Parse(typeof(Orientations), elem.Attribute("Orientation").Value) });
                }
                else
                {
                    var obj = Activator.CreateInstance(t, new object[] { elem.Attribute("Name").Value, Index2D.Parse(elem.Attribute("Pos").Value).Vector });
                }
            }
        }
        #endregion Methods

    }
}
