using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frostbyte
{
    internal partial class Game : Microsoft.Xna.Framework.Game
    {


        void LoadResources(){
            mLevels.Add(new FrostbyteLevel("TitleScreen", 
                Levels.TitleScreen.Load, 
                Levels.TitleScreen.Update, 
                LevelFunctions.UnloadLevel));
            mLevels.Add(new FrostbyteLevel("Intro", 
                Levels.Intro.Load, 
                LevelFunctions.DoNothing, 
                Levels.Intro.Unload,
                Levels.Intro.CompletionCondition));
            mLevels.Add(new FrostbyteLevel("WorldMap", 
                Levels.WorldMap.Load, 
                Levels.WorldMap.Update, 
                LevelFunctions.UnloadLevel));

            mLevels.Add(new FrostbyteLevel("Earth", 
                Levels.Earth.Load, 
                LevelFunctions.DoNothing, 
                LevelFunctions.UnloadLevel));
            mLevels.Add(new FrostbyteLevel("Lightning",
                Levels.Lightning.Load,
                LevelFunctions.DoNothing,
                LevelFunctions.UnloadLevel));
            mLevels.Add(new FrostbyteLevel("Water",
                Levels.Water.Load,
                LevelFunctions.DoNothing,
                LevelFunctions.UnloadLevel));
            mLevels.Add(new FrostbyteLevel("Fire",
                Levels.Fire.Load,
                LevelFunctions.DoNothing,
                LevelFunctions.UnloadLevel));
            mLevels.Add(new FrostbyteLevel("FireBoss",
               Levels.FireBoss.Load,
               LevelFunctions.DoNothing,
               LevelFunctions.UnloadLevel));
            mLevels.Add(new FrostbyteLevel("Final",
                Levels.Final.Load,
                LevelFunctions.DoNothing,
                LevelFunctions.UnloadLevel));
            mLevels.Add(new FrostbyteLevel("Credits",
                Levels.Credits.Load,
                LevelFunctions.DoNothing,
                LevelFunctions.UnloadLevel,
                Levels.Credits.CompletionCondition));
                
        }
    }
}
