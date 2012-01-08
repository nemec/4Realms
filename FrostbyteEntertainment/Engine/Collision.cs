using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace Frostbyte
{
    internal static class Collision
    {
        /// <summary>
        /// The Buckets of dictionaries for collision detection
        /// </summary>
        internal static List<Dictionary<Vector2, List<WorldObject>>> Buckets = new List<Dictionary<Vector2, List<WorldObject>>>();

        /// <summary>
        /// List of the Collision data to check
        /// </summary>
        internal static List<KeyValuePair<int, int>> Lists = new List<KeyValuePair<int, int>>();

        internal static int CellHeight { get; set; }
        internal static int CellWidth { get; set; }

        //Tuple value defs: 1=Key's Collision Object;  2=WorldObject that collided with Key;  3=Value 2's Key that Collided
        internal static Dictionary<WorldObject, List<Tuple<CollisionObject, WorldObject, CollisionObject>>> CollisionData = new Dictionary<WorldObject,List<Tuple<CollisionObject,WorldObject,CollisionObject>>>();
        internal static bool ShowCollisionData { get; set; }
        internal static BasicEffect basicEffect = new BasicEffect(This.Game.GraphicsDevice);

        internal static void FillBuckets()
        {
            Collision.Buckets = new List<Dictionary<Vector2, List<WorldObject>>>();

            /// \todo This needs to dissapear. Should only ever happen once
            var BG = This.Game.CurrentLevel.Background;
            if (BG != null)
            {
                foreach (var obj in BG.GetObjects())
                {
                    //make sure we've got a bucket list
                    while (Collision.Buckets.Count - 1 < obj.CollisionList)
                    {
                        Collision.Buckets.Add(new Dictionary<Vector2, List<WorldObject>>());
                    }
                    obj.Col.AddToBucket(obj);
                }
            }


            foreach (WorldObject worldObject in This.Game.CurrentLevel.mWorldObjects)
            {
                //make sure we've got a bucket list
                while (Collision.Buckets.Count - 1 < worldObject.CollisionList)
                {
                    Collision.Buckets.Add(new Dictionary<Vector2, List<WorldObject>>());
                }
                foreach (CollisionObject collisionObject in worldObject.GetCollision())
                {
                    collisionObject.AddToBucket(worldObject);
                }
            }
        }

        internal static void DetectCollisions()
        {
            if (This.Game.CurrentLevel.mWorldObjects.Count > 0)
            {
                CollisionData = new Dictionary<WorldObject, List<Tuple<CollisionObject, WorldObject, CollisionObject>>>();

                //for each list we care about checking we add them
                foreach (var pair in Lists)
                {
                    if (Buckets.Count > pair.Key && Buckets.Count > pair.Value)
                    {
                        var bucket1 = Buckets[pair.Key];
                        var bucket2 = Buckets[pair.Value];
                        foreach (var dict1 in bucket1)
                        {
                            var key = dict1.Key;
                            List<WorldObject> list1 = dict1.Value;
                            List<WorldObject> list2;
                            if (bucket2.TryGetValue(key, out list2))
                            {
                                foreach (var item in list1)
                                {
                                    foreach (var item2 in list2)
                                    {
                                        List<Tuple<CollisionObject, CollisionObject>> detectedCollisions = DetectCollision(item, item2);
                                        if (detectedCollisions.Count != 0)
                                        {
                                            List<Tuple<CollisionObject, WorldObject, CollisionObject>> collisionFront;
                                            List<Tuple<CollisionObject, WorldObject, CollisionObject>> collisionK;
                                            if (!CollisionData.TryGetValue(item, out collisionFront))
                                            {
                                                CollisionData[item] = collisionFront = new List<Tuple<CollisionObject, WorldObject, CollisionObject>>();
                                            }
                                            if (!CollisionData.TryGetValue(item2, out collisionK))
                                            {
                                                CollisionData[item2] = collisionK = new List<Tuple<CollisionObject, WorldObject, CollisionObject>>();
                                            }

                                            foreach (Tuple<CollisionObject, CollisionObject> tuple in detectedCollisions)
                                            {
                                                CollisionData[item].Add(new Tuple<CollisionObject, WorldObject, CollisionObject>(tuple.Item1, item2, tuple.Item2));
                                                CollisionData[item2].Add(new Tuple<CollisionObject, WorldObject, CollisionObject>(tuple.Item2, item, tuple.Item1));
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //KeyValuePair<Vector2, List<WorldObject>> bucketElem;
                        //var bucket = Buckets[pair.Key];
                        //while (bucket.Count > 0)
                        //{
                        //    bucketElem = bucket.First();
                        //    bucket.Remove(bucketElem.Key);
                        //    List<WorldObject> list = bucketElem.Value;

                        //    while (list.Count > 1)
                        //    {
                        //        WorldObject front = list.First();
                        //        list.RemoveAt(0);
                        //        for (int k = 0; k < list.Count; k++)
                        //        {
                        //            List<Tuple<CollisionObject, CollisionObject>> detectedCollisions = DetectCollision(front, list[k]);
                        //            if (detectedCollisions.Count != 0)
                        //            {
                        //                List<Tuple<CollisionObject, WorldObject, CollisionObject>> collisionFront;
                        //                List<Tuple<CollisionObject, WorldObject, CollisionObject>> collisionK;
                        //                if (!CollisionData.TryGetValue(front, out collisionFront))
                        //                {
                        //                    CollisionData[front] = collisionFront = new List<Tuple<CollisionObject, WorldObject, CollisionObject>>();
                        //                }
                        //                if (!CollisionData.TryGetValue(list[k], out collisionK))
                        //                {
                        //                    CollisionData[list[k]] = collisionK = new List<Tuple<CollisionObject, WorldObject, CollisionObject>>();
                        //                }

                        //                foreach (Tuple<CollisionObject, CollisionObject> tuple in detectedCollisions)
                        //                {
                        //                    CollisionData[front].Add(new Tuple<CollisionObject, WorldObject, CollisionObject>(tuple.Item1, list[k], tuple.Item2));
                        //                    CollisionData[list[k]].Add(new Tuple<CollisionObject, WorldObject, CollisionObject>(tuple.Item2, front, tuple.Item1));
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    }
                }
            }
        }

        internal static void Draw(Matrix transformation)
        {
            Level l = This.Game.CurrentLevel;
            if (ShowCollisionData)
            {
                float height = This.Game.GraphicsDevice.Viewport.Height;
                float width = This.Game.GraphicsDevice.Viewport.Width;
                basicEffect.View = Matrix.CreateLookAt(new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, -10),
                                                       new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, 0), new Vector3(0, -1, 0));
                basicEffect.Projection = Matrix.CreateOrthographic(This.Game.GraphicsDevice.Viewport.Width, This.Game.GraphicsDevice.Viewport.Height, 1, 20);
                basicEffect.VertexColorEnabled = true;


                foreach (WorldObject world in l.mWorldObjects)
                {
                    foreach (CollisionObject collisionObject in world.GetCollision())
                    {
                        collisionObject.Draw(world, transformation);
                    }
                }

                var BG = l.Background;
                if (BG != null)
                    foreach (var col in BG.GetCollision())
                    {
                        col.Draw(BG, transformation);
                    }
            }
        }

        internal static void Update()
        {
            FillBuckets();
            DetectCollisions();
        }

        internal static float DistanceSquared(Vector2 p1, Vector2 p2)
        {
            Vector2 d = p1 - p2;
            return d.X * d.X + d.Y * d.Y;
        }

        internal static List<Tuple<CollisionObject, CollisionObject>> DetectCollision(WorldObject w1, WorldObject w2)
        {
            List<Tuple<CollisionObject, CollisionObject>> output = new List<Tuple<CollisionObject, CollisionObject>>();

            List<CollisionObject> w1CollisionObj = w1.GetCollision();
            List<CollisionObject> w2CollisionObj = w2.GetCollision();

            foreach (CollisionObject cw1 in w1CollisionObj)
                foreach (CollisionObject cw2 in w2CollisionObj)
                {

                    if (DetectCollision(w1, (dynamic)cw1, w2, (dynamic)cw2))
                    {
                        output.Add(new Tuple<CollisionObject, CollisionObject>(cw1, cw2));
                    }
                }

            return output;
        }

        /// <summary>
        /// Determine if BoundingCircle and BoundingCircle collide
        /// </summary>
        /// /// <param name="w1">This Collision data's world object</param>
        /// /// <param name="c1">Circle to check</param>
        /// <param name="w2">Other collision data's world object</param>
        /// <param name="o">Collision circle which to check</param>
        /// <returns>Whether a collision occurred</returns>
        internal static bool DetectCollision(WorldObject w1, Collision_BoundingCircle c1, WorldObject w2, Collision_BoundingCircle o)
        {
            float ds = Collision.DistanceSquared(w1.Pos + c1.Center, w2.Pos + o.Center);
            float r = c1.Radius + o.Radius;
            return (ds <= r * r);
        }

        /// <summary>
        /// Detects OBB on OBB collision
        /// </summary>
        /// <param name="w1">This Collision data's world object</param>
        /// <param name="c1">OBB to check</param>
        /// <param name="w2">Other collision data's world object</param>
        /// <param name="o">Collision circle which to check</param>
        /// <returns>Whether a collision occurred</returns>
        internal static bool DetectCollision(WorldObject w1, Collision_OBB c1, WorldObject w2, Collision_OBB o)
        {
            //so, we've got to project the faces onto eachother and look for intersections
            //these values are values along the projected axis.
            var temp = c1.xAxis;
            var dot = 1;//Vector2.Dot(temp, temp);
            var a = (Vector2.Dot((c1.Corner1 + w1.Pos), temp) / dot);
            var b = (Vector2.Dot((c1.Corner2 + w1.Pos), temp) / dot);
            var c = (Vector2.Dot((o.Corner1 + w2.Pos), temp) / dot);
            var d = (Vector2.Dot((o.Corner2 + w2.Pos), temp) / dot);
            if (
                !(//check if they intersect, if they don't we're not colliding.
                    a <= b ?
                //is c or d between a b?
                    (a <= c && c <= b) || (a <= d && d <= b) :
                    (b <= c && c <= a) || (b <= d && d <= a)
                    )
                )
                return false;
            temp = c1.yAxis;
            dot = 1;//Vector2.Dot(temp, temp);
            a = (Vector2.Dot((c1.Corner1 + w1.Pos), temp) / dot);
            b = (Vector2.Dot((c1.Corner3 + w1.Pos), temp) / dot);
            c = (Vector2.Dot((o.Corner1 + w2.Pos), temp) / dot);
            d = (Vector2.Dot((o.Corner3 + w2.Pos), temp) / dot);
            if (
                !(//check if they intersect, if they don't we're not colliding.
                   a <= b ?
                //is c or d between a b?
                   (a <= c && c <= b) || (a <= d && d <= b) :
                   (b <= c && c <= a) || (b <= d && d <= a)
                   )
                )
                return false;
            temp = o.xAxis;
            dot = 1;//Vector2.Dot(temp, temp);
            a = (Vector2.Dot((c1.Corner1 + w1.Pos), temp) / dot);
            b = (Vector2.Dot((c1.Corner2 + w1.Pos), temp) / dot);
            c = (Vector2.Dot((o.Corner1 + w2.Pos), temp) / dot);
            d = (Vector2.Dot((o.Corner2 + w2.Pos), temp) / dot);
            if (
                !(//check if they intersect, if they don't we're not colliding.
                   a <= b ?
                //is c or d between a b?
                   (a <= c && c <= b) || (a <= d && d <= b) :
                   (b <= c && c <= a) || (b <= d && d <= a)
                   )
                )
                return false;
            temp = o.yAxis;
            dot = 1;//Vector2.Dot(temp, temp);
            a = (Vector2.Dot((c1.Corner1 + w1.Pos), temp) / dot);
            b = (Vector2.Dot((c1.Corner3 + w1.Pos), temp) / dot);
            c = (Vector2.Dot((o.Corner1 + w2.Pos), temp) / dot);
            d = (Vector2.Dot((o.Corner3 + w2.Pos), temp) / dot);
            if (
                !(//check if they intersect, if they don't we're not colliding.
                   a <= b ?
                //is c or d between a b?
                   (a <= c && c <= b) || (a <= d && d <= b) :
                   (b <= c && c <= a) || (b <= d && d <= a)
                   )
                )
                return false;
            return true;
        }



        /// <summary>
        /// Determine whether an AABB and AABB collide
        /// </summary>
        /// <param name="w1">This Collision data's world object</param>
        /// <param name="a1">AABB which to check</param>
        /// <param name="w2">Other collision data's world object</param>
        /// <param name="o">AABB which to check</param>
        /// <returns>Whether a collision occurred</returns>
        internal static bool DetectCollision(WorldObject w1, Collision_AABB a1, WorldObject w2, Collision_AABB o)
        {
            Vector2 TL1 = a1.TL + w1.Pos;
            Vector2 BR1 = a1.BR + w1.Pos;
            Vector2 TL2 = o.TL + w2.Pos;
            Vector2 BR2 = o.BR + w2.Pos;

            ///check rect vs rect really just axis aligned box check (simpler)
            bool outsideX = BR1.X < TL2.X || TL1.X > BR2.X;
            bool outsideY = BR1.Y < TL2.Y || TL1.Y > BR2.Y;
            return !(outsideY || outsideX);
        }


        /// <summary>
        /// Determine if AABB and BoundingCircle collide
        /// </summary>
        /// /// <param name="w1">This Collision data's world object</param>
        /// <param name="c1">Cricle to check</param>
        /// <param name="w2">Other collision data's world object</param>
        /// <param name="o">AABB which to check</param>
        /// <returns>Whether a collision occurred</returns>
        internal static bool DetectCollision(WorldObject w1, Collision_BoundingCircle c1, WorldObject w2, Collision_AABB o)
        {
            return DetectCollision(w2, o, w1, c1);
        }
        /// <summary>
        /// Determine whether an AABB and Collision circle collide
        /// </summary>
        /// <param name="w1">This Collision data's world object</param>
        /// <param name="a1">Collision AABB</param>
        /// <param name="w2">Other collision data's world object</param>
        /// <param name="o">Collision circle which to check</param>
        /// <returns>Whether a collision occurred</returns>
        internal static bool DetectCollision(WorldObject w1, Collision_AABB a1, WorldObject w2, Collision_BoundingCircle o)
        {
            Vector2 centerPoint = o.Center + w2.Pos;
            Vector2 topLeftPoint = a1.TL + w1.Pos;
            Vector2 bottomRightPoint = a1.BR + w1.Pos;

            int regionCode = 0;

            if (centerPoint.X < topLeftPoint.X)
                regionCode += 1; // 0001
            if (centerPoint.X > bottomRightPoint.X)
                regionCode += 2; // 0010
            if (centerPoint.Y < topLeftPoint.Y)
                regionCode += 4; // 0100
            if (centerPoint.Y > bottomRightPoint.Y)
                regionCode += 8;

            float radius = o.Radius;
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
                    if (Collision.DistanceSquared(centerPoint, new Vector2(topLeftPoint.X,bottomRightPoint.Y)) <= radius * radius)
                        return true;
                    break;
                case 6: //0110
                    if (Collision.DistanceSquared(centerPoint, new Vector2(bottomRightPoint.X,topLeftPoint.Y)) <= radius * radius)
                        return true;
                    break;
                case 10: //1010
                    if (Collision.DistanceSquared(centerPoint, bottomRightPoint) <= radius * radius)
                        return true;
                    break;
            }


            return false;
        }

        /// <summary>
        /// Determine if OBB and BoundingCircle collide
        /// </summary>
        /// /// <param name="w1">This Collision data's world object</param>
        /// <param name="c1">Cricle to check</param>
        /// <param name="w2">Other collision data's world object</param>
        /// <param name="o">OBB which to check</param>
        /// <returns>Whether a collision occurred</returns>
        internal static bool DetectCollision(WorldObject w1, Collision_BoundingCircle c1, WorldObject w2, Collision_OBB o)
        {
            return DetectCollision(w2, o, w1, c1);
        }
        /// <summary>
        /// Detect of a Collision circle and OBB collide
        /// </summary>
        /// <param name="w1">This Collision data's world object</param>
        /// <param name="o1">OBB to check</param>
        /// <param name="w2">Other collision data's world object</param>
        /// <param name="o">Collision circle which to check</param>
        /// <returns>Whether a collision occurred</returns>
        internal static bool DetectCollision(WorldObject w1, Collision_OBB o1, WorldObject w2, Collision_BoundingCircle o)
        {
            Vector2 c1Center = o.Center + w2.Pos;
            Vector2 o1Anchor = new Vector2(w1.Pos.X, w1.Pos.Y);

            Vector2 drawPoint0 = new Vector2(o1.drawPoints[0].Position.X + o1Anchor.X, o1.drawPoints[0].Position.Y + o1Anchor.Y);
            Vector2 drawPoint1 = new Vector2(o1.drawPoints[1].Position.X + o1Anchor.X, o1.drawPoints[1].Position.Y + o1Anchor.Y);
            Vector2 drawPoint2 = new Vector2(o1.drawPoints[2].Position.X + o1Anchor.X, o1.drawPoints[2].Position.Y + o1Anchor.Y);
            Vector2 drawPoint3 = new Vector2(o1.drawPoints[3].Position.X + o1Anchor.X, o1.drawPoints[3].Position.Y + o1Anchor.Y);

            Vector2 C = drawPoint1 + Vector2.Dot(c1Center - drawPoint1, Vector2.Normalize(drawPoint1 - drawPoint0)) * Vector2.Normalize(drawPoint1 - drawPoint0);
            Vector2 D = drawPoint1 + Vector2.Dot(c1Center - drawPoint1, Vector2.Normalize(drawPoint1 - drawPoint2)) * Vector2.Normalize(drawPoint1 - drawPoint2);

            float CtoDP1 = Vector2.DistanceSquared(C, drawPoint1);
            float CtoDP0 = Vector2.DistanceSquared(C, drawPoint0);
            float DP0toDP1 = Vector2.DistanceSquared(drawPoint1, drawPoint0);

            float DtoDP1 = Vector2.DistanceSquared(D, drawPoint1);
            float DtoDP2 = Vector2.DistanceSquared(D, drawPoint2);
            float DP2toDP1 = Vector2.DistanceSquared(drawPoint1, drawPoint2);

            float CentertoDP0 = Vector2.DistanceSquared(c1Center, drawPoint0);
            float CentertoDP1 = Vector2.DistanceSquared(c1Center, drawPoint1);
            float CentertoDP2 = Vector2.DistanceSquared(c1Center, drawPoint2);
            float CentertoDP3 = Vector2.DistanceSquared(c1Center, drawPoint3);

            if (((DP0toDP1 + (o.Radius * 2) * (o.Radius * 2) + 100 >= CtoDP0 + CtoDP1) && (DP2toDP1 + (o.Radius * 2) * (o.Radius * 2) + 100 >= DtoDP2 + DtoDP1)) ||
                  (CentertoDP0 <= o.Radius * o.Radius) || (CentertoDP1 <= o.Radius * o.Radius) || (CentertoDP2 <= o.Radius * o.Radius) || (CentertoDP3 <= o.Radius * o.Radius))
                return true;


            return false;
        }

        /// <summary>
        /// Detects OBB on OBB collision
        /// </summary>
        /// <param name="w1">This Collision data's world object</param>
        /// <param name="c1">OBB to check</param>
        /// <param name="w2">Other collision data's world object</param>
        /// <param name="o">AABB circle which to check</param>
        /// <returns>Whether a collision occurred</returns>
        internal static bool DetectCollision(WorldObject w1, Collision_OBB c1, WorldObject w2, Collision_AABB o)
        {
            //so, we've got to project the faces onto eachother and look for intersections
            //these values are values along the projected axis.
            Vector2 Corner1 = new Vector2(o.TL.X,o.BR.Y);
            var temp = c1.xAxis;
            var dot = 1;//Vector2.Dot(temp, temp);
            var a = (Vector2.Dot((c1.Corner1 + w1.Pos), temp) / dot);
            var b = (Vector2.Dot((c1.Corner2 + w1.Pos), temp) / dot);
            var c = (Vector2.Dot((Corner1 + w2.Pos), temp) / dot);
            var d = (Vector2.Dot((o.TL + w2.Pos), temp) / dot);
            if (
                !(//check if they intersect, if they don't we're not colliding.
                    a <= b ?
                //is c or d between a b?
                    (a <= c && c <= b) || (a <= d && d <= b) :
                    (b <= c && c <= a) || (b <= d && d <= a)
                    )
                )
                return false;
            temp = c1.yAxis;
            dot = 1;//Vector2.Dot(temp, temp);
            a = (Vector2.Dot((c1.Corner1 + w1.Pos), temp) / dot);
            b = (Vector2.Dot((c1.Corner3 + w1.Pos), temp) / dot);
            c = (Vector2.Dot((Corner1 + w2.Pos), temp) / dot);
            d = (Vector2.Dot((o.BR + w2.Pos), temp) / dot);
            if (
                !(//check if they intersect, if they don't we're not colliding.
                   a <= b ?
                //is c or d between a b?
                   (a <= c && c <= b) || (a <= d && d <= b) :
                   (b <= c && c <= a) || (b <= d && d <= a)
                   )
                )
                return false;
            return true;
        }
        /// <summary>
        /// Detects OBB on OBB collision
        /// </summary>
        /// <param name="w1">This Collision data's world object</param>
        /// <param name="c1">AABB to check</param>
        /// <param name="w2">Other collision data's world object</param>
        /// <param name="o">OBB circle which to check</param>
        /// <returns>Whether a collision occurred</returns>
        internal static bool DetectCollision(WorldObject w1, Collision_AABB c1, WorldObject w2, Collision_OBB o)
        {
            return DetectCollision(w2, o, w1, c1);
        }


    }
}
