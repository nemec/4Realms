using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections;

namespace Frostbyte
{
    public static class LevelFunctions
    {
        internal delegate Sprite EnemyFactory();

        internal static readonly Random rand = new Random();

        public static void DoNothing() { }

        /// <summary>
        /// A behavior that sends the player to the Stage Clear screen once the level is over.
        /// </summary>
        internal static void ToStageClear()
        {
            This.Game.SetCurrentLevel("StageClear");
        }

        internal static void UnloadLevel()
        {
            string nextlevel = LoadNextLevel();
            This.Game.SetCurrentLevel(nextlevel);
        }

        internal static string LoadNextLevel()
        {
            FrostbyteLevel l = This.Game.CurrentLevel as FrostbyteLevel;
            int ix = This.Game.LevelIndex;//FrostbyteLevel.LevelProgression.IndexOf(l.Name);
            string nextlevel = FrostbyteLevel.LevelProgression[(FrostbyteLevel.CurrentStage + 1) % FrostbyteLevel.LevelProgression.Count];
            FrostbyteLevel.CurrentStage++;
            This.Game.LoadLevel(nextlevel);
            return nextlevel;
        }

        internal static void GoToGameOver()
        {
            Condition oldWin = This.Game.CurrentLevel.WinCondition;
            Behavior oldEnd = This.Game.CurrentLevel.EndBehavior;
            This.Game.CurrentLevel.WinCondition = delegate { return true; };
            This.Game.CurrentLevel.EndBehavior = delegate
            {
                // Replace the old win condition
                This.Game.CurrentLevel.WinCondition = oldWin;
                This.Game.CurrentLevel.EndBehavior = oldEnd;
                This.Game.SetCurrentLevel("GameOver");
            };
            This.Game.AudioManager.Stop();
        }

        internal static IEnumerable<string> LoadLevelNotes(string levelName, bool terminateOnEOF=false)
        {
            string entrySeparator = @"(?:\r?\n){3,}";  // At least three newlines separate each entry.

            Random rand = new Random();
            string[] errorNotes = new string[]{
                "* The diary page is charred beyond recognition.",
                "* You pick up pieces of what was once a diary. It seems monsters have since trampled and torn the pages.",
                "* As you reach down for the pages, the wind whisks them away and they disappear from your sight."
            };

            string[] notes;
            try
            {
                string text = System.IO.File.ReadAllText(String.Format("Content/Story/{0}.txt", levelName));
                notes = System.Text.RegularExpressions.Regex.Split(text, entrySeparator);
            }
            catch (Exception){
                notes = new string[] { };
            }  // Just display one of the error messages each time instead.

            foreach (string note in notes)
            {
                yield return note;
            }

            if (!terminateOnEOF)
            {
                while (true)
                {
                    yield return errorNotes[rand.Next(errorNotes.Length)];
                }
            }
        }

        internal static IEnumerable<T> DelayEnumerable<T>(IEnumerable<T> toRun, TimeSpan delay)
        {
            TimeSpan endTime = This.gameTime.TotalGameTime + delay;
            while (This.gameTime.TotalGameTime < endTime)
            {
                yield return default(T);
            }
            foreach (T obj in toRun)
            {
                yield return obj;
            }
        }

        /// <summary>
        /// Spawns Enemies created by the EnemyFactory at random locations on the screen
        /// </summary>
        /// <param name="constructEnemy">Function to define an enemy</param>
        /// <param name="numEnemies">Number of enemies</param>
        internal static void Spawn(EnemyFactory constructEnemy, int numEnemies)
        {
            if (!This.Cheats.GetCheat("SpawnEnemies").Enabled)
            {
                Level l = This.Game.CurrentLevel;
                for (int i = 0; i < numEnemies; i++)
                {
                    Sprite virus = constructEnemy();
                    virus.Pos = l.Camera.Pos +
                        new Vector2(rand.Next(-500, This.Game.GraphicsDevice.Viewport.Width+500),
                            rand.Next(-500, This.Game.GraphicsDevice.Viewport.Height+500));
                }
            }
        }

        /// <summary>
        /// Spawns Enemies created by the EnemyFactory centered within a radius 
        /// proportional to the number of enemies around a specified position
        /// </summary>
        /// <param name="constructEnemy"></param>
        /// <param name="numEnemies"></param>
        /// <param name="position"></param>
        internal static void Spawn(EnemyFactory constructEnemy, int numEnemies, Vector2 position)
        {
            if (!This.Cheats.GetCheat("SpawnEnemies").Enabled)
            {
                double radius = 160f;
                double angleInc = (1.5 * Math.PI) / numEnemies;
                double startAngle = Math.PI * 2 * rand.NextDouble();
                for (int i = 0; i < numEnemies; i++)
                {
                    Sprite virus = constructEnemy();
                    virus.Pos = position + new Vector2((float)(Math.Cos(angleInc * i + startAngle) * radius * rand.NextDouble()),
                        (float)(Math.Sin(angleInc * i + startAngle) * radius * rand.NextDouble()));
                }
            }
        }
    }
}
