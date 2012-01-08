using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    internal class Collision_AABB : CollisionObject
    {
        /// <summary>
        /// Initializes a Bounding Circle.
        /// </summary>
        internal Collision_AABB(int _id, Vector2 _topLeftPointOffset, Vector2 _bottomRightPointOffset)
        {
            TL = _topLeftPointOffset;
            BR = _bottomRightPointOffset;
            ID = _id;
            Type= CollisionType.AABB;


            //create collision object's points for drawing
            drawPoints = new VertexPositionColor[5];
            drawPoints[0].Position = new Vector3(TL.X, TL.Y, 0f);
            drawPoints[0].Color = Color.Red;
            drawPoints[1].Position = new Vector3(TL.X, BR.Y, 0f);
            drawPoints[1].Color = Color.Red;
            drawPoints[2].Position = new Vector3(BR.X, BR.Y, 0f);
            drawPoints[2].Color = Color.Red;
            drawPoints[3].Position = new Vector3(BR.X, TL.Y, 0f);
            drawPoints[3].Color = Color.Red;
            drawPoints[4].Position = new Vector3(TL.X, TL.Y, 0f);
            drawPoints[4].Color = Color.Red;
        }

        /// <summary>
        /// Offset of topLeftPoint from sprite anchor.
        /// </summary>
        internal Vector2 TL { get; set; }

        /// <summary>
        /// Offset of bottomLeftPoint from sprite anchor.
        /// </summary>
        internal Vector2 BR { get; set; }

        /// <summary>
        /// Determines which grid cells the object is in
        /// </summary>
        internal override List<Vector2> GridLocations(WorldObject worldObject)
        {
            int bottomLeftX = (int)(worldObject.Pos.X + TL.X) / (int)Collision.CellWidth;
            int bottomLeftY = (int)(worldObject.Pos.Y + BR.Y) / (int)Collision.CellHeight;
            int topRightX = (int)(worldObject.Pos.X + BR.X) / (int)Collision.CellWidth;
            int topRightY = (int)(worldObject.Pos.Y + TL.Y) / (int)Collision.CellHeight;

            List<Vector2> gridLocations = new List<Vector2>();
            for (int i = bottomLeftX; i <= topRightX; i++) //cols
            {
                for (int j = bottomLeftY; j <= topRightY; j++) //rows
                {
                    Vector2 location = new Vector2(i, j);
                    gridLocations.Add(location);
                }
            }

            return gridLocations;
        }

        /// <summary>
        /// Add this CollisionObject to bucket.
        /// </summary>
        internal override void AddToBucket(WorldObject worldObject)
        {
            int bottomLeftX = (int)(worldObject.Pos.X + TL.X) / (int)Collision.CellWidth;
            int bottomLeftY = (int)(worldObject.Pos.Y + BR.Y) / (int)Collision.CellHeight;
            int topRightX = (int)(worldObject.Pos.X + BR.X) / (int)Collision.CellWidth;
            int topRightY = (int)(worldObject.Pos.Y + TL.Y) / (int)Collision.CellHeight;

            AddToBucket(worldObject, bottomLeftX, bottomLeftY, topRightX, topRightY);
        }

        internal override void Draw(WorldObject world, Matrix transformation)
        {
            Collision.basicEffect.World = Matrix.CreateTranslation(new Vector3(world.Pos, 0)) * transformation;

            foreach (EffectPass pass in Collision.basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                This.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, drawPoints, 0, 4);
            }
        }
    }
}