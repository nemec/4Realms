using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    public delegate void ManaChangedHandler(object obj, int value);

    internal abstract partial class OurSprite : Sprite
    {
        #region Attacking Variables
        public bool isAttackAnimDone = true;
        protected List<IEnumerator<bool>> mAttacks = new List<IEnumerator<bool>>();
        internal float originalSpeed;
        internal TimeSpan slowStart = new TimeSpan(0, 0, 0);
        internal TimeSpan slowDuration = new TimeSpan(0, 0, 0);
        internal bool isSlowed = false;
        #endregion Attacking Variables

        #region Death Variables
        protected bool isDieEffectEnabled = false;
        protected float sampleWidthPercent = 0.015f;
        protected float sampleHeightPercent = 0.015f;
        #endregion Death Variables

        internal OurSprite(string name, Actor actor)
            : base(name, actor)
        {
            Health = MaxHealth;
        }

        internal OurSprite(string name, Actor actor, int collisionlist)
            : base(name, actor, collisionlist)
        {
            Health = MaxHealth;
        }

        #region Targeting
        /// <summary>
        /// Returns a sprite in targets that is closest to the sprite's current position
        /// and within aggroDistance distance from the current position.
        /// </summary>
        internal Sprite GetClosestTarget(List<Sprite> targets, float aggroDistance = float.PositiveInfinity)
        {
            Sprite min = null;
            foreach (Sprite target in targets)
            {
                if (target == this || target.State == SpriteState.Dead)
                {
                    continue;
                }
                if (min == null ||
                    Vector2.DistanceSquared(target.GroundPos, GroundPos) <
                    Vector2.DistanceSquared(min.GroundPos, GroundPos))
                {
                    if (Vector2.DistanceSquared(target.GroundPos, GroundPos) <= aggroDistance * aggroDistance)
                    {
                        min = target;
                    }
                }
            }
            return min;
        }

        /// <summary>
        /// Returns a list of sprites within aggroDistance distance from the current position.
        /// </summary>
        internal List<Sprite> GetTargetsInRange(List<Sprite> targets, float aggroDistance = float.PositiveInfinity)
        {
            List<Sprite> range = new List<Sprite>();
            foreach (Sprite target in targets)
            {
                if (target == this || target.State == SpriteState.Dead)
                {
                    continue;
                }
                Vector2 correctTargetGroundPos = target.GroundPos;
                correctTargetGroundPos.Y -= 32;
                if (Vector2.DistanceSquared(correctTargetGroundPos, GroundPos) <= aggroDistance * aggroDistance)
                {
                    range.Add(target);
                }
            }
            return range;
        }

        /// <summary>
        /// Returns a list of sprites within the specified rectangle.
        /// </summary>
        internal List<Sprite> GetTargetsInRectangle(List<Sprite> targets, Rectangle rect)
        {
            List<Sprite> range = new List<Sprite>();
            foreach (Sprite target in targets)
            {
                if (target == this || target.State == SpriteState.Dead)
                {
                    continue;
                }

                Vector2 center = target.GroundPos;
                if (rect.Left < center.X && rect.Right > center.X &&
                    rect.Top < center.Y && rect.Bottom > center.Y)
                {
                    range.Add(target);
                }
            }
            return range;
        }
        #endregion Targeting

        #region Collision
        internal Vector2 previousFootPos = Vector2.Zero;

        /// <summary>
        /// Check for collision with background and move enemy out of collision with background until no collisions exist
        /// </summary>
        internal void checkBackgroundCollisions()
        {
            if (Vector2.DistanceSquared(previousFootPos, GroundPos) <= .2f)
            {
                GroundPos = previousFootPos;
                return;
            }

            Vector2 positiveInfinity = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Tuple<Vector2, Vector2> closestObject = new Tuple<Vector2, Vector2>(positiveInfinity, positiveInfinity);
            Vector2 closestIntersection = positiveInfinity;
            Vector2 footPos = this.GroundPos;
            Vector2 originalFootPos = previousFootPos;

            while (Vector2.DistanceSquared(footPos, previousFootPos) > .2f)
            {
                detectBackgroundCollisions(footPos, previousFootPos, out closestObject, out closestIntersection);

                if (closestIntersection == positiveInfinity)
                    break;

                if (closestObject.Item2 == positiveInfinity && closestObject.Item1 != positiveInfinity) //this is for a circle
                {
                    Vector2 A1 = closestObject.Item1 - closestIntersection;
                    Vector2 B1 = footPos - closestIntersection;
                    A1.Normalize();
                    Vector2 tangentToA1 = new Vector2(-A1.Y, A1.X);
                    float magnitudeOfTangentToA1 = A1.Length();
                    float magnitudeOfb = B1.Length();
                    float distFromFootToProjIntersection = Vector2.Dot(tangentToA1, B1) / magnitudeOfTangentToA1;
                    Vector2 newFootPos = closestIntersection + distFromFootToProjIntersection * tangentToA1;
                    if (Vector2.DistanceSquared(previousFootPos, closestIntersection) <= Vector2.DistanceSquared(previousFootPos, footPos) && closestIntersection != positiveInfinity)
                    {
                        Vector2 normal = new Vector2(-A1.X, -A1.Y);
                        normal.Normalize();
                        previousFootPos = closestIntersection + 0.2f * normal;
                        footPos = newFootPos + 0.2f * normal;
                    }
                    else
                    {
                        previousFootPos = footPos;
                    }
                }
                else //this is for a line segment
                {
                    Vector2 A1 = new Vector2();
                    if (closestObject.Item2 == closestIntersection && closestIntersection != positiveInfinity)
                        A1 = closestObject.Item1 - closestIntersection;
                    else
                        A1 = closestObject.Item2 - closestIntersection;
                    Vector2 B1 = footPos - closestIntersection;
                    A1.Normalize();
                    float magnitudeOfa = A1.Length();
                    float magnitudeOfb = B1.Length();
                    float distFromFootToProjIntersection = Vector2.Dot(A1, B1) / magnitudeOfa;
                    Vector2 newFootPos = closestIntersection + distFromFootToProjIntersection * A1;
                    if (Vector2.DistanceSquared(previousFootPos, closestIntersection) <= Vector2.DistanceSquared(previousFootPos, footPos) && closestIntersection != positiveInfinity)
                    {
                        Vector2 tangent = closestObject.Item1 - closestObject.Item2;
                        Vector2 normal = new Vector2(-tangent.Y, tangent.X);
                        normal.Normalize();
                        previousFootPos = closestIntersection + .2f * normal;
                        footPos = newFootPos + .2f * normal;
                    }
                    else
                    {
                        previousFootPos = footPos;
                    }
                }
            }


            //This takes care of the sprite moving too slow and updates position
            if (Vector2.DistanceSquared(footPos, originalFootPos) >= .2f)
            {
                bool doNotMove = false;

                float collisionRadius = this.GroundPosRadius;
                Tuple<int, int> topLeftMostTile = new Tuple<int, int>((int)Math.Floor(((footPos.X - collisionRadius) / This.CellSize)),   //top left most tile that could possible hit sprite
                                                                (int)Math.Floor(((footPos.Y - collisionRadius)) / This.CellSize));
                Tuple<int, int> bottomRightMostTile = new Tuple<int, int>((int)Math.Floor((footPos.X + collisionRadius) / This.CellSize), //bottom right most tile that could possible hit sprite
                                                                        (int)Math.Floor((footPos.Y + collisionRadius) / This.CellSize));
                TileList tileMap = (This.Game.CurrentLevel as FrostbyteLevel).TileMap;
                for (int x = topLeftMostTile.Item1; x <= bottomRightMostTile.Item1; x++)
                    for (int y = topLeftMostTile.Item2; y <= bottomRightMostTile.Item2; y++)
                    {
                        Tile tile;
                        tileMap.TryGetValue(x, y, out tile);

                        if (tile.Type == TileTypes.Floor)
                            continue;

                        if ((tile.Type == TileTypes.Bottom || tile.Type == TileTypes.BottomConvexCorner) && !tileCircleCollision(new Vector2(x * 64, y * 64 + 32), new Vector2(x * 64 + 64, y * 64 + 64), footPos, collisionRadius))
                        {
                            continue;
                        }
                        else if (!tileCircleCollision(new Vector2(x * 64, y * 64), new Vector2(x * 64 + 64, y * 64 + 64), footPos, collisionRadius))
                        {
                            continue;
                        }

                        doNotMove = true;
                    }


                if (doNotMove)
                    GroundPos = originalFootPos;
                else
                    GroundPos = footPos;
            }
            else
            {
                this.GroundPos = originalFootPos;
            }
        }

        bool tileCircleCollision(Vector2 tileTopLeftPos, Vector2 tileBottomRightPos, Vector2 circlePos, float circleRadius)
        {
            Vector2 centerPoint = circlePos;
            Vector2 topLeftPoint = tileTopLeftPos;
            Vector2 bottomRightPoint = tileBottomRightPos;

            int regionCode = 0;

            if (centerPoint.X < topLeftPoint.X)
                regionCode += 1; // 0001
            if (centerPoint.X > bottomRightPoint.X)
                regionCode += 2; // 0010
            if (centerPoint.Y < topLeftPoint.Y)
                regionCode += 4; // 0100
            if (centerPoint.Y > bottomRightPoint.Y)
                regionCode += 8;

            float radius = circleRadius;
            switch (regionCode)
            {
                case 0: //0000
                    return true;
                case 1: //0001
                    if (Math.Abs(topLeftPoint.X - centerPoint.X) <= radius)
                        return true;
                    break;
                case 2: //0010
                    if (Math.Abs(centerPoint.X - bottomRightPoint.X) <= radius)
                        return true;
                    break;
                case 4: //0100
                    if (Math.Abs(centerPoint.Y - topLeftPoint.Y) <= radius)
                        return true;
                    break;
                case 8: //1000
                    if (Math.Abs(bottomRightPoint.Y - centerPoint.Y) <= radius)
                        return true;
                    break;
                case 5: //0101
                    if (Collision.DistanceSquared(centerPoint, topLeftPoint) <= radius * radius)
                        return true;
                    break;
                case 9: //1001
                    if (Collision.DistanceSquared(centerPoint, new Vector2(topLeftPoint.X, bottomRightPoint.Y)) <= radius * radius)
                        return true;
                    break;
                case 6: //0110
                    if (Collision.DistanceSquared(centerPoint, new Vector2(bottomRightPoint.X, topLeftPoint.Y)) <= radius * radius)
                        return true;
                    break;
                case 10: //1010
                    if (Collision.DistanceSquared(centerPoint, bottomRightPoint) <= radius * radius)
                        return true;
                    break;
            }


            return false;
        }

        internal void detectBackgroundCollisions(Vector2 currentPosition, Vector2 previousPosition, out Tuple<Vector2, Vector2> closestObjectOut, out Vector2 closestIntersectionOut)
        {
            float collisionRadius = this.GroundPosRadius;
            List<Tuple<Vector2, Vector2>> boundaryLineSegments = new List<Tuple<Vector2, Vector2>>();   //line segments to check collision with sprite
            List<Tuple<Vector2, Vector2>> boundaryCircles = new List<Tuple<Vector2, Vector2>>();        //circles to check collision with sprite

            //If previous position is same as current position then no collision is possible
            if (previousPosition == currentPosition)
            {
                closestIntersectionOut = new Vector2(float.PositiveInfinity);
                closestObjectOut = new Tuple<Vector2, Vector2>(new Vector2(float.PositiveInfinity), new Vector2(float.PositiveInfinity));
                return;
            }

            //Add line segments and circles from each tile inside bounding box formed by topLeftMostTile and bottomRightMostTile
            TileList tileMap = (This.Game.CurrentLevel as FrostbyteLevel).TileMap;
            Tuple<int, int> topLeftMostTile = new Tuple<int, int>((int)Math.Floor(((Math.Min(previousPosition.X, currentPosition.X) - collisionRadius) / This.CellSize)),     //top left most tile that could possible hit sprite
                                                                (int)Math.Floor(((Math.Min(previousPosition.Y, currentPosition.Y) - collisionRadius)) / This.CellSize));
            Tuple<int, int> bottomRightMostTile = new Tuple<int, int>((int)Math.Floor((Math.Max(previousPosition.X, currentPosition.X) + collisionRadius) / This.CellSize), //bottom right most tile that could possible hit sprite
                                                                    (int)Math.Floor((Math.Max(previousPosition.Y, currentPosition.Y) + collisionRadius) / This.CellSize));

            for (int x = topLeftMostTile.Item1; x <= bottomRightMostTile.Item1; x++)
                for (int y = topLeftMostTile.Item2; y <= bottomRightMostTile.Item2; y++)
                {
                    Tile tile;
                    tileMap.TryGetValue(x, y, out tile);

                    float tileStartPosX = 0;
                    float tileStartPosY = 0;
                    if (tile.GridCell != null)  //protect collision from tileMap code
                    {
                        tileStartPosX = tile.GridCell.Pos.X;
                        tileStartPosY = tile.GridCell.Pos.Y;
                    }

                    #region Add Tile Boundary Line Segments and Circles to Appropriate Lists
                    switch (tile.Type)
                    {
                        case TileTypes.Wall: //top wall
                            boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize, tileStartPosY + This.CellSize + collisionRadius), //add bottom side of tile
                                                                                 new Vector2(tileStartPosX, tileStartPosY + This.CellSize + collisionRadius)));
                            break;
                        case TileTypes.Bottom: //bottom wall
                            boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX, tileStartPosY + This.CellSize / 2 - collisionRadius), //add top side of tile
                                                                                 new Vector2(tileStartPosX + This.CellSize, tileStartPosY + This.CellSize / 2 - collisionRadius)));
                            break;
                        case TileTypes.SideWall: //side wall
                            if (tile.Orientation == Orientations.Up_Left) //right side wall
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX - collisionRadius, tileStartPosY + This.CellSize), //add left side of tile
                                                                                     new Vector2(tileStartPosX - collisionRadius, tileStartPosY)));
                            else //left side wall
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY), //add left side of tile
                                                                                     new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY + This.CellSize)));
                            break;
                        case TileTypes.BottomConvexCorner: //bottom convex corner wall
                            boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX, tileStartPosY + This.CellSize / 2 - collisionRadius), //add top side of tile
                                                                                 new Vector2(tileStartPosX + This.CellSize, tileStartPosY + This.CellSize / 2 - collisionRadius)));
                            if (tile.Orientation == Orientations.Right) //right bottom convex corner
                            {
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX - collisionRadius, tileStartPosY + This.CellSize), //add left side of tile
                                                                                     new Vector2(tileStartPosX - collisionRadius, tileStartPosY + This.CellSize / 2)));
                                boundaryCircles.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX, tileStartPosY + This.CellSize / 2), new Vector2(float.PositiveInfinity, float.PositiveInfinity))); //add top left point of tile
                            }
                            else //left bottom convex corner
                            {
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY + This.CellSize / 2), //add right side of tile
                                                                                     new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY + This.CellSize)));
                                boundaryCircles.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize, tileStartPosY + This.CellSize / 2), new Vector2(float.PositiveInfinity, float.PositiveInfinity))); //add top right point of tile
                            }
                            break;
                        case TileTypes.ConvexCorner: //top convex corner wall
                            boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize, tileStartPosY + This.CellSize + collisionRadius), //add bottom side of tile
                                                                                 new Vector2(tileStartPosX, tileStartPosY + This.CellSize + collisionRadius)));
                            if (tile.Orientation == Orientations.Right) //right top convex corner |_
                            {
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX - collisionRadius, tileStartPosY + This.CellSize), //add left side of tile
                                                                                     new Vector2(tileStartPosX - collisionRadius, tileStartPosY)));
                                boundaryCircles.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX, tileStartPosY + This.CellSize), new Vector2(float.PositiveInfinity, float.PositiveInfinity))); //add bottom left point of tile
                            }
                            else //left top convex corner _|
                            {
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY), //add right side of tile
                                                                                     new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY + This.CellSize)));
                                boundaryCircles.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize, tileStartPosY + This.CellSize), new Vector2(float.PositiveInfinity, float.PositiveInfinity))); //add bottom right point of tile
                            }
                            break;
                        case TileTypes.BottomCorner: //bottom concave corner wall
                            if (tile.Orientation == Orientations.Right) //right bottom concave corner _|
                            {
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX - collisionRadius, tileStartPosY + This.CellSize / 2), //add left side of tile
                                                                                     new Vector2(tileStartPosX - collisionRadius, tileStartPosY)));

                                //boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2((tileStartPosX - collisionRadius) + 1, (tileStartPosY + This.CellSize / 2)), //add left side of tile
                                //                                                     new Vector2((tileStartPosX - collisionRadius) + 1, tileStartPosY + 2)));

                                //boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2((tileStartPosX - collisionRadius) + 2, tileStartPosY + This.CellSize / 2), //add left side of tile
                                //                                                     new Vector2((tileStartPosX - collisionRadius) + 2, tileStartPosY + 4)));

                                //boundaryCircles.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX - 15, tileStartPosY + This.CellSize / 2 - 15 ), new Vector2(float.PositiveInfinity, float.PositiveInfinity))); //add bottom left point of tile

                            }
                            else //left bottom concave corner |_
                            {
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY), //add right side of tile
                                                                                     new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY + This.CellSize / 2)));
                            }
                            break;
                        case TileTypes.Corner: //top concave corner wall
                            //add nothing because it is not possible to hit
                            break;
                        default:
                            break;
                    }
                    #endregion Add Tile Boundary Line Segments and Circles to Appropriate Lists
                }

            foreach (Sprite obstacle in (This.Game.CurrentLevel as FrostbyteLevel).obstacles)
            {
                if (obstacle is Frostbyte.Obstacles.Obstacle)
                {
                    float obstacleStartX = obstacle.Pos.X;
                    float obstacleStartY = obstacle.Pos.Y;
                    boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(obstacleStartX + This.CellSize, obstacleStartY + This.CellSize + collisionRadius), //add bottom side of tile
                                                                                        new Vector2(obstacleStartX, obstacleStartY + This.CellSize + collisionRadius)));
                    boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(obstacleStartX, obstacleStartY - collisionRadius), //add top side of tile
                                                                                        new Vector2(obstacleStartX + This.CellSize, obstacleStartY - collisionRadius)));
                    boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(obstacleStartX - collisionRadius, obstacleStartY + This.CellSize), //add left side of tile
                                                            new Vector2(obstacleStartX - collisionRadius, obstacleStartY)));
                    boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(obstacleStartX + This.CellSize + collisionRadius, obstacleStartY), //add right side of tile
                                                                                            new Vector2(obstacleStartX + This.CellSize + collisionRadius, obstacleStartY + This.CellSize)));
                    boundaryCircles.Add(new Tuple<Vector2, Vector2>(new Vector2(obstacleStartX + This.CellSize, obstacleStartY + This.CellSize), new Vector2(float.PositiveInfinity, float.PositiveInfinity))); //add bottom right point of tile
                    boundaryCircles.Add(new Tuple<Vector2, Vector2>(new Vector2(obstacleStartX, obstacleStartY + This.CellSize), new Vector2(float.PositiveInfinity, float.PositiveInfinity))); //add bottom left point of tile
                    boundaryCircles.Add(new Tuple<Vector2, Vector2>(new Vector2(obstacleStartX + This.CellSize, obstacleStartY), new Vector2(float.PositiveInfinity, float.PositiveInfinity))); //add top right point of tile
                    boundaryCircles.Add(new Tuple<Vector2, Vector2>(new Vector2(obstacleStartX, obstacleStartY), new Vector2(float.PositiveInfinity, float.PositiveInfinity))); //add top left point of tile
                }
            }

            //If there are no line segments or circle then there are no possible collisions
            if (boundaryLineSegments.Count == 0 && boundaryCircles.Count == 0)
            {
                closestIntersectionOut = new Vector2(float.PositiveInfinity);
                closestObjectOut = new Tuple<Vector2, Vector2>(new Vector2(float.PositiveInfinity), new Vector2(float.PositiveInfinity));
                return;
            }

            #region Calculate closest intersection
            Vector2 positiveInfinity = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Tuple<Vector2, Vector2> closestObject = new Tuple<Vector2, Vector2>(positiveInfinity, positiveInfinity);
            float closestDistanceSquared = float.PositiveInfinity;
            Vector2 closestIntersection = positiveInfinity;

            foreach (Tuple<Vector2, Vector2> lineSegment in boundaryLineSegments)   //calculate closest line segment
            {
                float tValue = ((lineSegment.Item2.X - lineSegment.Item1.X) * (previousPosition.Y - lineSegment.Item1.Y) - (lineSegment.Item2.Y - lineSegment.Item1.Y) * (previousPosition.X - lineSegment.Item1.X)) /
                               ((lineSegment.Item2.Y - lineSegment.Item1.Y) * (currentPosition.X - previousPosition.X) - (lineSegment.Item2.X - lineSegment.Item1.X) * (currentPosition.Y - previousPosition.Y));

                Vector2 intersection = new Vector2(previousPosition.X + tValue * (currentPosition.X - previousPosition.X), previousPosition.Y + tValue * (currentPosition.Y - previousPosition.Y));

                float distanceSquared = Vector2.DistanceSquared(previousPosition, intersection);

                intersection.X = (float)Math.Round(intersection.X, 4);  //protect against floating point errors
                intersection.Y = (float)Math.Round(intersection.Y, 4);  //protect against floating point errors

                if (distanceSquared >= 0 && distanceSquared <= closestDistanceSquared && (tValue >= 0)
                    && intersection.X <= Math.Max(lineSegment.Item1.X, lineSegment.Item2.X) && intersection.Y <= Math.Max(lineSegment.Item1.Y, lineSegment.Item2.Y)
                    && intersection.X >= Math.Min(lineSegment.Item1.X, lineSegment.Item2.X) && intersection.Y >= Math.Min(lineSegment.Item1.Y, lineSegment.Item2.Y))
                {
                    closestDistanceSquared = distanceSquared;
                    closestObject = lineSegment;
                    closestIntersection = intersection;
                }
            }

            foreach (Tuple<Vector2, Vector2> circle in boundaryCircles) //calculate closest circle
            {
                Vector2 D = currentPosition - previousFootPos;
                //D.Normalize();
                float A = Vector2.Dot(D, D);
                float B = 2f * Vector2.Dot((previousFootPos - circle.Item1), D);
                float C = Vector2.Dot((previousFootPos - circle.Item1), (previousFootPos - circle.Item1)) - collisionRadius * collisionRadius;

                float discriminant = B * B - 4 * A * C;

                if (discriminant < 0)
                {
                    //there are no intersections
                }
                else if (discriminant == 0)
                {
                    float tValue = (-B) / (2 * A);

                    Vector2 intersection = new Vector2(previousPosition.X + tValue * (currentPosition.X - previousPosition.X), previousPosition.Y + tValue * (currentPosition.Y - previousPosition.Y));

                    float distanceSquared = Vector2.DistanceSquared(previousPosition, intersection);

                    if (distanceSquared >= 0 && distanceSquared <= closestDistanceSquared && tValue >= 0)
                    {
                        closestDistanceSquared = distanceSquared;
                        closestObject = circle;
                        closestIntersection = intersection;
                    }
                }
                else //discriminant > 0
                {
                    float tValue1 = (-B + (float)Math.Sqrt(discriminant)) / (2 * A);
                    float tValue2 = (-B - (float)Math.Sqrt(discriminant)) / (2 * A);

                    float closesttValue = 0;
                    if (tValue1 < tValue2 && tValue1 >= 0)
                    {
                        closesttValue = tValue1;
                    }
                    else if (tValue2 >= 0)
                    {
                        closesttValue = tValue2;
                    }
                    else
                        closesttValue = -1f;


                    Vector2 intersection = new Vector2(previousPosition.X + closesttValue * (currentPosition.X - previousPosition.X), previousPosition.Y + closesttValue * (currentPosition.Y - previousPosition.Y));

                    float distanceSquared = Vector2.DistanceSquared(previousPosition, intersection);

                    if (distanceSquared >= 0 && distanceSquared <= closestDistanceSquared && (closesttValue >= 0))
                    {
                        closestDistanceSquared = distanceSquared;
                        closestObject = circle;
                        closestIntersection = intersection;
                    }
                }
            }
            #endregion Calculate closest intersection


            closestObjectOut = closestObject;
            closestIntersectionOut = closestIntersection;
            boundaryCircles.Clear();
            boundaryLineSegments.Clear();
        }
        #endregion Collision

        #region Direction
        protected Vector2 mDirection = new Vector2(0, 1);

        internal Vector2 Direction
        {
            get
            {
                return mDirection;
            }
            set
            {
                //State = PreviousPos == Pos ? SpriteState.Idle : SpriteState.Moving;

                if (value != Vector2.Zero)
                    mDirection = value;

                if (mDirection != Vector2.Zero)
                    mDirection.Normalize();
                double angle = Math.Atan2(mDirection.Y, mDirection.X);
                if (-Math.PI / 8 <= angle && angle < Math.PI / 8)
                {
                    base.Orientation = Orientations.Right;
                }
                else if (Math.PI / 8 <= angle && angle < 3 * Math.PI / 8)
                {
                    base.Orientation = Orientations.Down_Right;
                }
                else if (3 * Math.PI / 8 <= angle && angle < 5 * Math.PI / 8)
                {
                    base.Orientation = Orientations.Down;
                }
                else if (5 * Math.PI / 8 <= angle && angle < 7 * Math.PI / 8)
                {
                    base.Orientation = Orientations.Down_Left;
                }
                else if (-3 * Math.PI / 8 <= angle && angle < -Math.PI / 8)
                {
                    base.Orientation = Orientations.Up_Right;
                }
                else if (-5 * Math.PI / 8 <= angle && angle < -3 * Math.PI / 8)
                {
                    base.Orientation = Orientations.Up;
                }
                else if (-7 * Math.PI / 8 <= angle && angle < -5 * Math.PI / 8)
                {
                    base.Orientation = Orientations.Up_Left;
                }
                else
                {
                    base.Orientation = Orientations.Left;
                }
            }
        }

        internal new Orientations Orientation
        {
            get { return base.Orientation; }
            set
            {
                switch (value)
                {
                    case Orientations.Down:
                        mDirection = new Vector2(0, 1); break;
                    case Orientations.Down_Left:
                        mDirection = new Vector2(-1, 1); break;
                    case Orientations.Down_Right:
                        mDirection = new Vector2(1, 1); break;
                    case Orientations.Left:
                        mDirection = new Vector2(-1, 0); break;
                    case Orientations.Right:
                        mDirection = new Vector2(1, 0); break;
                    case Orientations.Up:
                        mDirection = new Vector2(0, -1); break;
                    case Orientations.Up_Left:
                        mDirection = new Vector2(-1, -1); break;
                    case Orientations.Up_Right:
                        mDirection = new Vector2(1, -1); break;
                    default:
                        throw new Exception("Need to add Orientation to switch statement, " +
                            "otherwise Direction will not be set.");

                }
                if (mDirection != Vector2.Zero)
                    mDirection.Normalize();
                base.Orientation = value;
            }
        }
        #endregion Direction

        #region StatusEffects/Agumentation
        internal List<StatusEffect> StatusEffects = new List<StatusEffect>();
        #endregion StatusEffects/Agumentation

        #region Spawnpoint
        internal Vector2 SpawnPoint = Vector2.Zero;

        internal void SpawnOn(OurSprite target)
        {
            if (target.Pos == Vector2.Zero)
            {
                SpawnPoint = target.SpawnPoint;
            }
            else
            {
                SpawnPoint = target.CenterPos;
            }
        }

        /// <summary>
        /// Regenerates status (eg. Health, Mana)
        /// Possibly removing status effects as well.
        /// </summary>
        internal virtual void Regen()
        {
            Health = MaxHealth;

            foreach (Frostbyte.Characters.Mage mage in (This.Game.CurrentLevel as FrostbyteLevel).allies)
            {
                if (mage.State == SpriteState.Dead)
                {
                    mage.State = SpriteState.Idle;
                    mage.GroundPos = this.GroundPos;
                    mage.Health = 1;
                }
            }
        }

        /// <summary>
        /// Respawns the player at their spawn point with their default attributes
        /// </summary>
        internal virtual void Respawn()
        {
            GroundPos = SpawnPoint;
            Regen();
            Rewind();
            StartAnim();
        }
        #endregion Spawnpoint

        #region Properties
        //Elemental Properties
        internal Element ElementType { get; set; }
        #endregion

        internal void StopAttacks()
        {
            mAttacks.Clear();
        }

        #region Death Effect
        protected virtual void Die()
        {
        }
        #endregion Death Effect

        #region Update
        internal override void Update()
        {
            base.Update();
            List<StatusEffect> toDelete = new List<StatusEffect>();
            foreach (StatusEffect e in StatusEffects)
            {
                e.Update();
                if (e.Expired)
                {
                    This.Game.CurrentLevel.RemoveSprite(e.particleEmitter);
                    toDelete.Add(e);
                }
            }
            foreach (StatusEffect e in toDelete)
            {
                StatusEffects.Remove(e);
            }
            foreach (StatusEffect e in StatusEffects)
            {
                e.Draw(this);
            }

            List<IEnumerator<bool>> removeTheseAttacks = new List<IEnumerator<bool>>();
            foreach (IEnumerator<bool> attack in mAttacks)
            {
                attack.MoveNext();
                if (attack.Current)
                    removeTheseAttacks.Add(attack);
            }
            foreach (IEnumerator<bool> attack in removeTheseAttacks)
            {
                attack.Dispose();
                mAttacks.Remove(attack);
            }

            if (isSlowed && This.gameTime.TotalGameTime >= slowStart + slowDuration)
            {
                isSlowed = false;
                Speed = originalSpeed;
            }


        }
        #endregion Update

        public Element mStatusEffect { get; set; }
    }
}
