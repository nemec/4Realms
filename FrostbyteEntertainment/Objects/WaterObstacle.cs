using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte.Obstacles
{
    class WaterObstacle : Obstacle
    {
        internal WaterObstacle(string name)
            : base(name, new Actor(This.Game.LoadingLevel.GetAnimation("WaterObstacle.anim")))
        {
            ZOrder = int.MaxValue;
            CollisionList = -1;
        }

        public WaterObstacle(string name, Vector2 initialPosition)
            : this(name)
        {
            SpawnPoint = initialPosition;
        }
    }
}
