using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte
{
    internal class Target : Sprite
    {
        internal Target(string name, Actor a)
            : this(name, a, Color.White, 200)
        {
        }

        internal Target(string name, Actor a, Color c)
            : this(name, a,c, 200)
        {
        }

        internal Target(string name, Actor a, Color color, int size)
            : base(name, a)
        {
            mColor=color;
            ZOrder = int.MaxValue;
        }
    }
}
