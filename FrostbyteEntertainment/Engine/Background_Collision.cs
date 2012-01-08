using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte
{
    class Background_Collision : WorldObject
    {
        internal Background_Collision(CollisionObject col)
        {
            Col = col;
            CollisionList = 0;
        }

        internal override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        internal override List<CollisionObject> GetCollision()
        {
            return new List<CollisionObject>() { Col };
        }

        internal override List<Vector2> GetHotSpots()
        {
            return new List<Vector2>();
        }

        internal override void Update()
        {
            
        }
    }
}
