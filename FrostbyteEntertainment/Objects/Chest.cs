using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte.Obstacles
{
    internal class Chest : Obstacle
    {
        internal Chest(string name, Item HeldItem)
            : base(name, new Actor(new Animation("chest.anim")))
        {
            ZOrder = int.MinValue + 1;
            Held = HeldItem;
            HeldItem.CenterOn(this);
            HeldItem.ZOrder = int.MinValue;
            This.Game.AudioManager.AddSoundEffect("Effects/Chest_Open");
        }

        internal Chest(string name)
            : this(name, new Key(name + "_key"))
        {
        }

        public Chest(string name, Vector2 initialPos)
            : this(name)
        {
            SpawnPoint = initialPos;
        }

        private Item Held;

        internal Item Open()
        {
            This.Game.AudioManager.PlaySoundEffect("Effects/Chest_Open");
            This.Game.CurrentLevel.RemoveSprite(this);
            FrostbyteLevel l = (This.Game.CurrentLevel as FrostbyteLevel);
            if (l != null)
            {
                l.obstacles.Remove(this);
            }
            return Held;
        }
    }
}
