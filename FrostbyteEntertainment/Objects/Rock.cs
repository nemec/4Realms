using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte.Obstacles
{
    internal class Rock : Obstacle
    {
        internal Rock(string name)
            : base(name, new Actor(This.Game.LoadingLevel.GetAnimation("rock.anim")))
        {
            ZOrder = int.MaxValue;
        }

        public Rock(string name, Vector2 initialPosition)
            : this(name)
        {
            SpawnPoint = initialPosition;
        }
    }
}
