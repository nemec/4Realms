using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    class Background : WorldObject
    {
        #region Variables

        private List<CollisionObject> CollisionData = new List<CollisionObject>();

        /// <summary>
        /// Tells whether to animate or not
        /// </summary>
        private bool mAnimating;

        /// <summary>
        /// Tells if the object has been drawn the first time
        /// </summary>
        //private bool mDrawn;

        /// <summary>
        /// Number that indicates when the Sprite'a animation was last updated.
        /// </summary>
        private GameTime mLastUpdate;

        /// <summary>
        /// The animation frames
        /// </summary>
        internal List<SpriteFrame> Frames = new List<SpriteFrame>();

        /// <summary>
        /// current frame of anim
        /// </summary>
        protected int Frame = 0;
        #endregion Variables

        internal Background(string name, string animfile)
            : this(name, animfile, 0)
        {
        }

        internal Background(string name, string animfile, int layer)
        {
            CollisionList = 0;

            mName = name;

            This.Game.LoadingLevel.Background = this;
            LoadAnimation(animfile, "Backgrounds");

            mLastUpdate = new GameTime();
            mAnimating = true;

            // Allows layering of background objects
            ZOrder = int.MinValue + layer;
        }

        /// <summary>
        /// changes to the specified animation beginning at 0
        /// </summary>
        internal SpriteFrame GetAnimation()
        {
            return Frames[Frame];
        }

        internal override void Draw(GameTime gameTime)
        {
            //Frame so we don't have to find it so often
            SpriteFrame frame = GetAnimation();
            if (mAnimating == true)
            {
                //used to update the animation. Occurs once the frame's pause * sprite's speed occurs.
                if (mLastUpdate.TotalGameTime.TotalMilliseconds + frame.Pause < gameTime.TotalGameTime.TotalMilliseconds)
                {
                    //obtain current peg 
                    Vector2 ppos = frame.AnimationPeg;
                    Frame = (Frame + 1) % Frames.Count;
                    //update frame so we don't need to worry
                    frame = Frames[Frame];
                    //obtain next peg
                    Vector2 npos = frame.AnimationPeg;
                    //move current position to difference of two
                    Pos += (ppos - npos);
                    mLastUpdate = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);
                }
            }
            if (Visible == true)
            {
                SpriteFrame f = Frames[Frame];
                This.Game.spriteBatch.Draw(f.Image, Pos + f.AnimationPeg, new Rectangle((int)f.StartPos.X, (int)f.StartPos.Y, f.Width, f.Height), Color.White, Angle, GetAnimation().AnimationPeg, Scale, SpriteEffects.None, 0);
            }
        }

        internal override List<CollisionObject> GetCollision()
        {
            return CollisionData;
        }

        internal override List<Vector2> GetHotSpots()
        {
            return new List<Vector2>();
        }

        internal List<Background_Collision> objects = new List<Background_Collision>();
        internal List<Background_Collision> GetObjects()
        {
            return objects;
        }

        /// <summary>
        /// Loads the animations from a file.
        /// </summary>
        /// <param name="filename">Name of animfile</param>
        /// <param name="contentSubfolder">Folder where content is stored</param>
        private void LoadAnimation(string filename, string contentSubfolder)
        {
            filename = String.Format("Content/{0}/{1}", contentSubfolder, filename);

            if (!File.Exists(filename))
            {
                throw new Exception(String.Format("Animation file {0} does not exist.", filename));
            }
            XDocument doc = XDocument.Load(filename);
            foreach (var frame in doc.Descendants("Frame"))
            {
                SpriteFrame sf = new SpriteFrame();

                string[] sp = frame.Attribute("TLPos").Value.Split(',');
                sf.StartPos = new Vector2(float.Parse(sp[0]), float.Parse(sp[1]));

                ///image
                string file = frame.Attribute("SpriteSheet").Value;
                sf.Image = This.Game.Content.Load<Texture2D>(String.Format("{0}/{1}", contentSubfolder, file));

                /** sets frame delay */
                sf.Pause = int.Parse(frame.Attribute("FrameDelay").Value);

                //Image's width and height
                sf.Width = int.Parse(frame.Attribute("Width").Value);
                sf.Height = int.Parse(frame.Attribute("Height").Value);


                var point = frame.Attribute("AnimationPeg").Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                float pegX = float.Parse(point.First());
                float pegY = float.Parse(point.Last());

                /** Set the animation Peg*/
                sf.AnimationPeg = new Vector2(pegX + (float)sf.Width / 2, pegY + (float)sf.Height / 2);

                Frames.Add(sf);
            }
            //add collision data
            int idCount = 0;
            foreach (var collision in doc.Descendants("Collision"))
            {
                if (collision.Attribute("Type").Value == "Circle")
                {
                    string[] pt = collision.Attribute("Pos").Value.Split(',');

                    var col = new Collision_BoundingCircle(
                        idCount++,
                        new Vector2(float.Parse(pt[0]), float.Parse(pt[1])),
                        float.Parse(collision.Attribute("Radius").Value));

                    CollisionData.Add(col);
                    objects.Add(new Background_Collision(col));
                }
                else if (collision.Attribute("Type").Value == "Rectangle")
                {
                    string[] tl = collision.Attribute("TLPos").Value.Split(',');
                    float tlx = float.Parse(tl[0]);
                    float tly = float.Parse(tl[1]);
                    string[] br = collision.Attribute("BRPos").Value.Split(',');
                    float brx = float.Parse(br[0]);
                    float bry = float.Parse(br[1]);

                    var col = new Collision_AABB(
                        idCount++,
                        new Vector2(tlx, tly),
                        new Vector2(brx, bry)
                        );
                    CollisionData.Add(col);
                    objects.Add(new Background_Collision(col));
                }
                else if (collision.Attribute("Type").Value == "OBB")
                {
                    string[] c1 = collision.Attribute("Corner1").Value.Split(',');
                    float c1x = float.Parse(c1[0]);
                    float c1y = float.Parse(c1[1]);
                    string[] c2 = collision.Attribute("Corner2").Value.Split(',');
                    float c2x = float.Parse(c2[0]);
                    float c2y = float.Parse(c2[1]);
                    float thickness = float.Parse(collision.Attribute("Thickness").Value.ToString());
                    var col = new Collision_OBB(
                        idCount++,
                        new Vector2(c1x, c1y),
                        new Vector2(c2x, c2y),
                        thickness
                        );
                    CollisionData.Add(col);
                    objects.Add(new Background_Collision(col));
                }
            }
        }


        internal override void Update()
        {
        }
    }
}
