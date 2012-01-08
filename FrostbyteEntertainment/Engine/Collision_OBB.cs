using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    internal class Collision_OBB : CollisionObject
    {

        /// <summary>
        /// Initializes a Bounding Circle.
        /// </summary>
        internal Collision_OBB(int _id, Vector2 _cornerOffset1, Vector2 _cornerOffset2, float _thickness)
        {
            Corner1 = _cornerOffset1;
            Corner2 = _cornerOffset2;
            Thickness = _thickness;
            ID = _id;
            Type = CollisionType.OBB;
            PreviousPos = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

            //create collision object's points for drawing
            Vector3 normal = Vector3.Normalize(new Vector3(Corner2.Y - Corner1.Y, Corner1.X - Corner2.X, 0));
            drawPoints = new VertexPositionColor[5];
            drawPoints[0].Position = new Vector3(Corner1.X, Corner1.Y, 0f);
            drawPoints[0].Color = Color.Yellow;
            drawPoints[1].Position = new Vector3(Corner2.X, Corner2.Y, 0f);
            drawPoints[1].Color = Color.Green;
            drawPoints[2].Position = new Vector3(Corner2.X + normal.X * Thickness, Corner2.Y + normal.Y * Thickness, 0f);
            drawPoints[2].Color = Color.Red;
            drawPoints[3].Position = new Vector3(Corner1.X + normal.X * Thickness, Corner1.Y + normal.Y * Thickness, 0f);
            drawPoints[3].Color = Color.Red;
            drawPoints[4].Position = new Vector3(Corner1.X, Corner1.Y, 0f);
            drawPoints[4].Color = Color.Red;

            Vector2 p1 = new Vector2(drawPoints[0].Position.X, drawPoints[0].Position.Y);
            Vector2 p2 = new Vector2(drawPoints[1].Position.X, drawPoints[1].Position.Y);
            Vector2 p3 = new Vector2(drawPoints[3].Position.X, drawPoints[3].Position.Y);
            Corner3 = p3;

            xAxis = p2 - p1;
            xAxis.Normalize();

            yAxis = p3 - p1;
            yAxis.Normalize();
        }


        /// <summary>
        /// OBB's "x axis"
        /// </summary>
        internal Vector2 xAxis;

        /// <summary>
        /// OBB's "y axis"
        /// </summary>
        internal Vector2 yAxis;


        /// <summary>
        /// Offset of Corner1 from sprite anchor.
        /// </summary>
        internal Vector2 Corner1 { get; set; }

        /// <summary>
        /// Offset of Corner2 from sprite anchor.
        /// </summary>
        internal Vector2 Corner2 { get; set; }

        /// <summary>
        /// Offset of Corner2 from sprite anchor.
        /// </summary>
        internal Vector2 Corner3 { get; set; }

        /// <summary>
        /// Thickness of box
        /// </summary>
        internal float Thickness { get; set; }

        /// <summary>
        /// Determines which grid cells the object is in
        /// </summary>
        internal override List<Vector2> GridLocations(WorldObject worldObject)
        {
            int bottomLeftX = (int)(worldObject.Pos.X + Corner1.X) / (int)Collision.CellWidth;
            int bottomLeftY = (int)(worldObject.Pos.Y + Corner2.Y) / (int)Collision.CellHeight;
            int topRightX = (int)(worldObject.Pos.X + Corner2.X) / (int)Collision.CellWidth;
            int topRightY = (int)(worldObject.Pos.Y + Corner1.Y) / (int)Collision.CellHeight;

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
            if (PreviousPos == worldObject.Pos)
            {
                foreach (Vector2 location in PreviousBuckets)
                {
                    try
                    {
                        Collision.Buckets[worldObject.CollisionList][location].Add(worldObject);
                    }
                    catch
                    {
                        List<WorldObject> possibleCollisions = new List<WorldObject>();
                        possibleCollisions.Add(worldObject);
                        Collision.Buckets[worldObject.CollisionList].Add(location, possibleCollisions);
                    }
                }
            }
            else
            {
                int highestY = (int)(worldObject.Pos.Y + drawPoints[0].Position.Y) / (int)Collision.CellHeight;
                int lowestY = highestY;
                int highestX = (int)(worldObject.Pos.X + drawPoints[0].Position.X) / (int)Collision.CellHeight;
                int lowestX = highestX;
                //find the smallest
                for (int i = 1; i < drawPoints.Length - 1; i++)
                {
                    int y = (int)(worldObject.Pos.Y + drawPoints[i].Position.Y) / (int)Collision.CellHeight;
                    //get the smallest y
                    if (y < lowestY)
                        lowestY = y;
                    //get the largest y
                    else if (y > highestY)
                        highestY = y;

                    int x = (int)(worldObject.Pos.X + drawPoints[i].Position.X) / (int)Collision.CellWidth;
                    //get the smallest x
                    if (x < lowestX)
                        lowestX = x;
                    //get the largest x
                    else if (x > highestX)
                        highestX = x;
                }


                Vector2 o1Anchor = new Vector2(worldObject.Pos.X, worldObject.Pos.Y);
                Vector2 drawPoint0 = new Vector2(drawPoints[0].Position.X + o1Anchor.X, drawPoints[0].Position.Y + o1Anchor.Y);
                Vector2 drawPoint1 = new Vector2(drawPoints[1].Position.X + o1Anchor.X, drawPoints[1].Position.Y + o1Anchor.Y);
                Vector2 drawPoint2 = new Vector2(drawPoints[2].Position.X + o1Anchor.X, drawPoints[2].Position.Y + o1Anchor.Y);
                Vector2 drawPoint3 = new Vector2(drawPoints[3].Position.X + o1Anchor.X, drawPoints[3].Position.Y + o1Anchor.Y);
                PreviousBuckets.Clear();
                for (int i = lowestX; i <= highestX; i++) //cols
                {
                    for (int j = lowestY; j <= highestY; j++) //rows
                    {
                        float slope0to1;
                        bool isInGrid = false;
                        try
                        {
                            slope0to1 = (drawPoint0.Y - drawPoint1.Y) / (drawPoint0.X - drawPoint1.X);
                            //slope1to2 = (drawPoint1.Y - drawPoint2.Y) / (drawPoint1.X - drawPoint2.X);

                            //left
                            float p1 = slope0to1 * (i * Collision.CellWidth) + (drawPoint0.Y - (slope0to1 * (drawPoint0.X)));
                            //right
                            float p2 = slope0to1 * ((i + 1) * Collision.CellWidth) + (drawPoint0.Y - (slope0to1 * (drawPoint0.X)));
                            //top
                            float p3 = (j * Collision.CellHeight - drawPoint0.Y) / slope0to1 + drawPoint0.X;
                            //bottom
                            float p4 = ((j + 1) * Collision.CellHeight - drawPoint0.Y) / slope0to1 + drawPoint0.X;
                            if (
                                    (p1 <= ((j + 1) * Collision.CellHeight) && p1 >= (j * Collision.CellHeight)) ||
                                    (p2 <= ((j + 1) * Collision.CellHeight) && p2 >= (j * Collision.CellHeight)) ||
                                    (
                                        p3 <= ((i + 1) * Collision.CellWidth) && p3 >= (i * Collision.CellWidth)
                                    )
                                    ||
                                    (
                                        p4 <= ((i + 1) * Collision.CellWidth) && p4 >= (i * Collision.CellWidth)
                                    )
                                )
                            {
                                isInGrid = true;
                            }
                        }
                        catch
                        {
                            isInGrid = true;
                        }
                        if (isInGrid) //check if grid cell is colliding with box
                        {
                            Vector2 location = new Vector2(i, j);
                            PreviousBuckets.Add(location);
                            var bucket = Collision.Buckets[worldObject.CollisionList];
                            List<WorldObject> value;
                            if (bucket.TryGetValue(location, out value))
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
                PreviousPos = worldObject.Pos;
            }
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