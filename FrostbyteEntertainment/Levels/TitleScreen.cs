using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Frostbyte;
using Frostbyte.Obstacles;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte.Levels
{
    internal static class TitleScreen
    {
        static readonly TimeSpan RequiredWaitTime = new TimeSpan(0, 0, 0, 0, 0);
        static TimeSpan LevelInitTime = TimeSpan.MinValue;
        private static List<IController> gamePads = new List<IController>();

        internal static void Load(Level context)
        {
            FrostbyteLevel l = context as FrostbyteLevel;
            l.Theme = Element.None;
            LevelInitTime = TimeSpan.MinValue;
            
            Viewport v = This.Game.GraphicsDevice.Viewport;

            /** load music */
            This.Game.AudioManager.AddBackgroundMusic("Music/TitleScreenBG");
            This.Game.AudioManager.PlayBackgroundMusic("Music/TitleScreenBG", 0.1f);

            Text title = new Text("titletext", "Fonts/Title", "4Realms");
            title.CenterOn(new Vector2(v.Width / 2, v.Height / 2));
            title.Static = true;
            title.DisplayColor = Color.DodgerBlue;

            context.GetTexture("regen");
            RestorePlayerHealthTrigger t = new RestorePlayerHealthTrigger("trigger", v.Width);
            t.SpawnPoint = new Vector2(v.Width / 2, v.Height / 1.2f);

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

            if ((This.Game as FrostbyteGame).GlobalController.Start == ReleasableButtonState.Clicked
                || PlayerPressedStart)
            {
                (This.Game.CurrentLevel as FrostbyteLevel).LevelCompleted = true;
            }
            else if ((This.Game as FrostbyteGame).GlobalController.GetKeypress(Keys.B) == ReleasableButtonState.Clicked)
            {
                This.Game.AudioManager.Pause();
            }
        }
    }
}
