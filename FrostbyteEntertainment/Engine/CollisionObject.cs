using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
        /// <summary>
        /// Possible Collision types
        /// </summary>
        internal enum CollisionType { 
            Circle=0,
            AABB,
            OBB,
        }
    internal abstract class CollisionObject
    {
        /// <summary>
        /// Id that will be used to determine which CollisionObject was touched.
        /// </summary>
        internal int ID { get; set; }

        internal CollisionType Type { get; set; }

        /// <summary>
        /// Previous bucket locations
        /// </summary>
        internal List<Vector2> PreviousBuckets = new List<Vector2>();

        internal Vector2 PreviousPos = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

        /// <summary>
        /// Determines which grid cells the object is in
        /// </summary>
        internal abstract List<Vector2> GridLocations(WorldObject worldObject);

        /// <summary>
        /// Add this CollisionObject to bucket.
        /// </summary>
        internal abstract void AddToBucket(WorldObject worldObject);

        /// <summary>
        /// Adds The world object to all cell contained within the four points
        /// </summary>
        /// <param name="worldObject">Object to add</param>
        /// <param name="bottomLeftX">Bottom left xcoord point of the rect</param>
        /// <param name="bottomLeftY">Bottom left ycoord point of the rect</param>
        /// <param name="topRightX">Top Right xcoord point of the rect</param>
        /// <param name="topRightY">Top Right ycoord point of the rect</param>
        internal void AddToBucket(WorldObject worldObject, int bottomLeftX, int bottomLeftY, int topRightX, int topRightY)
        {
            for (int i = bottomLeftX; i <= topRightX; i++) //cols
            {
                for (int j = bottomLeftY; j <= topRightY; j++) //rows
                {
                    Dictionary<Vector2,List<WorldObject>> bucket = Collision.Buckets[worldObject.CollisionList];
                    Vector2 location = new Vector2(i, j);
                    List<WorldObject> value;
                    if(bucket.TryGetValue(location, out value))
                    {
                        value.Add(worldObject);
                    }
                    else
                    {
                        value = new List<WorldObject>();
                        value.Add(worldObject);
                        bucket[location] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Holds the points that make up the linestrip for drawing
        /// </summary>
        internal VertexPositionColor[] drawPoints;

        internal abstract void Draw(WorldObject world, Matrix transformation);

        public override int GetHashCode()
        {
            return ID;
        }
    }
}
