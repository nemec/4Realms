using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.IO;
using Frostbyte;

namespace Frostbyte.Levels
{
    internal static class Credits
    {
        private static TextScroller scroller;
        private static List<IController> gamePads = new List<IController>();

        internal static void Load(Level context)
        {
            FrostbyteLevel l = context as FrostbyteLevel;
            l.Theme = Element.None;

            This.Game.AudioManager.AddBackgroundMusic("Music/CreditsBG");
            This.Game.AudioManager.PlayBackgroundMusic("Music/CreditsBG", 0.1f);

            Viewport v = This.Game.GraphicsDevice.Viewport;
            scroller = new TextScroller("intro_text", v.Width * 3 / 4, v.Height * 3 / 4);
            scroller.Pos.X = v.Width / 2.5f;
            scroller.Pos.Y = v.Height / 8;
            scroller.Static = true;


            StreamReader streamReader = new StreamReader("Content/Story/Credits.txt");
            string credits = streamReader.ReadToEnd();
            streamReader.Close();

            scroller.ScrollText(credits);



            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                gamePads.Add(new GamePadController(PlayerIndex.One));
            }
            if (GamePad.GetState(PlayerIndex.Two).IsConnected)
            {
                gamePads.Add(new GamePadController(PlayerIndex.Two));
            }
        }

        internal static bool CompletionCondition()
        {
            bool PlayerPressedStart = false;
            foreach (IController controller in gamePads)
            {
                controller.Update();
                if (controller.Start == ReleasableButtonState.Clicked)
                    PlayerPressedStart = true;
            }

            return !scroller.Scrolling ||
                (This.Game as FrostbyteGame).GlobalController.Start == ReleasableButtonState.Clicked ||
                PlayerPressedStart;
        }

        internal static void Unload()
        {
            string nextlevel = LevelFunctions.LoadNextLevel();
            This.Game.SetCurrentLevel(nextlevel);
        }
    }
}
