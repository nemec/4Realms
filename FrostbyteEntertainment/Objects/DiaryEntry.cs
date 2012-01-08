using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte.Obstacles
{
    class DiaryEntry : OnScreenObject
    {
        public DiaryEntry(string name)
            : base(name, new Actor(This.Game.LoadingLevel.GetAnimation("diary-entry.anim")))
        {
        }

        public DiaryEntry(string name, Vector2 initialPosition)
            : this(name)
        {
            SpawnPoint = initialPosition;
            this.ZOrder = -1;
        }
    }
}
