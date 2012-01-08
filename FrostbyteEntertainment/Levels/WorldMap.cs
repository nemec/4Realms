using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Frostbyte;
using Frostbyte.Obstacles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Frostbyte.Levels
{
    internal class WorldMap
    {
        static readonly TimeSpan RequiredWaitTime = new TimeSpan(0, 0, 0, 0, 0);
        static TimeSpan LevelInitTime = TimeSpan.MinValue;
        private static int visited = 0;
        private static List<IController> gamePads = new List<IController>();

        private static List<Vector2> LevelPositions = null;

        internal static void Load(Level context)
        {
            
            This.Game.AudioManager.AddBackgroundMusic("Music/WorldMapBG");
            This.Game.AudioManager.PlayBackgroundMusic("Music/WorldMapBG", 0.1f);

            FrostbyteLevel l = context as FrostbyteLevel;
            l.Theme = Element.None;
            LevelInitTime = TimeSpan.MinValue;

            

            if (visited == 0)
            {
                LevelPositions = new List<Vector2>(new Vector2[]{
                    new Vector2(265, 350),
                    new Vector2(695, 335),
                    new Vector2(540, 190),
                    new Vector2(530, 650),
                    new Vector2(500, 375)
                });
            }

            Sprite s = new Sprite("map", new Actor(l.GetAnimation("WorldMap.anim")));
            s.ZOrder = int.MinValue;

            /*Text title = new Text("titletext", "text", visited.ToString());
            title.CenterOn(new Vector2(v.Width / 2, 100));
            title.DisplayColor = Color.Chartreuse;*/

            context.GetTexture("regen");
            ConcentricCircles c = new ConcentricCircles("cc", 75 / 2);
            c.ZOrder = 100;

            if (visited < LevelPositions.Count)
            {
                c.SpawnPoint = LevelPositions[visited];
            }
            visited++;

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                gamePads.Add(new GamePadController(PlayerIndex.One));
            }
            if (GamePad.GetState(PlayerIndex.Two).IsConnected)
            {
                gamePads.Add(new GamePadController(PlayerIndex.Two));
            }
        }

        internal static void Update()
        {
            GameTime gameTime = This.gameTime;
            if (LevelInitTime == TimeSpan.MinValue)
            {
                LevelInitTime = gameTime.TotalGameTime;
            }

            bool PlayerPressedStart = false;
            foreach (IController controller in gamePads)
            {
                controller.Update();
                if (controller.Start == ReleasableButtonState.Clicked)
                    PlayerPressedStart = true;
            }

            if ((This.Game as FrostbyteGame).GlobalController.Start == ReleasableButtonState.Clicked ||
                PlayerPressedStart)
            {
                // Go to next
                (This.Game.CurrentLevel as FrostbyteLevel).LevelCompleted = true;
            }
        }
    }
}
