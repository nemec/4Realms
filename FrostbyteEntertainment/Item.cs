using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    internal class Item : Sprite
    {
        internal Item(string name, Actor actor, Actor iconActor = null)
            : base(name, actor)
        {
            if (iconActor != null)
            {
                Icon = new ItemIcon(name + "_Icon", iconActor);
            }
        }

        internal Sprite Icon = null;


    }

    internal class ItemIcon : Sprite
    {
        internal ItemIcon(string name, Actor actor)
            : base(name, actor)
        {
            Visible = false;
            ZOrder = int.MaxValue;
            This.Game.LoadingLevel.RemoveSprite(this);
        }
    }

    internal class Key : Item
    {
        internal Key(string name)
            : base(name, new Actor(new DummyAnimation()),
                new Actor(This.Game.LoadingLevel.GetAnimation("key.anim")))
        {
        }
    }
}
