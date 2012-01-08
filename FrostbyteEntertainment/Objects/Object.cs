using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frostbyte.Obstacles
{
    internal abstract class OnScreenObject : OurSprite
    {
        internal OnScreenObject(string name, Actor actor)
            : base(name, actor)
        {
            (This.Game.LoadingLevel as FrostbyteLevel).obstacles.Add(this);
        }

    }
}
