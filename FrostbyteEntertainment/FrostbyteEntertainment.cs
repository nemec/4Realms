using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    /// <summary>
    /// Do anything required for Game-specific code here
    /// to avoid cluttering up the Engine
    /// </summary>
    static class GameData
    {
        public static int Score { get; set; }
        public static int NumberOfLives { get; set; }
        public static readonly int DefaultNumberOfLives = 4;
        public static int livesAwarded = 0;
    }

    /// <summary>
    /// Add Game-specific level code here to avoid cluttering up the Engine
    /// </summary>
    class FrostbyteLevel : Level
    {
        public Vector2 PlayerSpawnPoint = new Vector2(50, 50);
        internal int waveNumber;

        /// <summary>
        /// A count of the enemies defeated in this level
        /// </summary>
        public int EnemiesDefeated { get; set; }

        public FrostbyteLevel(string n, Behavior loadBehavior, Behavior updateBehavior, Behavior endBehavior, Condition winCondition)
            : base(n, loadBehavior, updateBehavior, endBehavior, winCondition)
        {
        }

        /// <summary>
        /// A list of levels in the order they should be played through
        /// </summary>
        internal static List<string> LevelProgression = new List<string>()
        {
            "Stomach",
            "Lungs",
            "Credits"
        };

        /// <summary>
        /// Retains progress through our levels
        /// </summary>
        internal static int CurrentStage = 0;

        internal override void Update()
        {
            base.Update();
            //move the viewport if we need to
            Sprite player = GetSprite("ship");
            if (player != null)
            {
                Viewport viewport = This.Game.GraphicsDevice.Viewport;
                Vector2 cameraPos = This.Game.CurrentLevel.Camera.Pos;
                int borderWidth = 200;//viewport.Width/2;
                int borderHeight = 200;//viewport.Height/2;

                Vector2 difference = player.Pos - cameraPos;
                if (difference.X < viewport.X + borderWidth)
                {
                    cameraPos.X -= borderWidth - (difference.X);
                }
                else if (difference.X > viewport.X + viewport.Width - borderWidth)
                {
                    cameraPos.X += borderWidth - (viewport.Width - (difference.X));
                }
                if (difference.Y < viewport.Y + borderWidth)
                {
                    cameraPos.Y -= borderHeight - (difference.Y);
                }
                else if (difference.Y > viewport.Y + viewport.Height - borderWidth)
                {
                    cameraPos.Y += borderHeight - (viewport.Height - (difference.Y));
                }
                This.Game.CurrentLevel.Camera.Pos = cameraPos;
            }

            if ((int)(GameData.Score / 10000) > GameData.livesAwarded)
            {
                GameData.NumberOfLives++;
                GameData.livesAwarded++;
            }
            
        }

        internal static int NextLevel()
        {
            CurrentStage = (CurrentStage + 1) % LevelProgression.Count;
            return CurrentStage;
        }
    }
}
