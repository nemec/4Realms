using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;

namespace Frostbyte
{
    [Serializable]
    class AnimationDoesNotExistException : Exception
    {
        private string name;

        public AnimationDoesNotExistException(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return String.Format("Animation {0} does not exist has not been loaded.", name);
        }
    }

    /// <summary>
    /// This class loads sets of images into
    /// an animation set.
    /// </summary>
    class Animation
    {
        #region Properties
        internal bool Built { get; set; }
        internal int NumFrames { get { return Frames.Count; } }
        internal string Name { get; set; }
        #endregion

        #region Variables
        /// <summary>
        /// Animation Frames
        /// </summary>
        internal List<SpriteFrame> Frames = new List<SpriteFrame>();

        #endregion

        #region Constructors
        internal Animation()
        {
            Built = false;
        }

        internal Animation(string filename) : this(filename, filename) { }

        internal Animation(string filename, string name)
            : this(filename, name, "Sprites")
        {
        }

        internal Animation(string filename, string name, string contentSubfolder)
        {
            Name = name;
            LoadAnimation(filename, contentSubfolder);
            Built = true;
        }
        #endregion

        #region Methods
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

                /** Set the animation Peg*/
                sf.AnimationPeg = Index2D.Parse(frame.Attribute("AnimationPeg").Value).Vector;

                //add the mirror offset if there is one
                if (frame.Attribute("MirrorOffset") != null)
                {
                    sf.MirrorOffset = Index2D.Parse(frame.Attribute("MirrorOffset").Value).Vector;
                }

                foreach (var hotspot in frame.Descendants("HotSpot"))
                {
                    sf.HotSpots.Add(Index2D.Parse(hotspot.Value).Vector);
                }

                int idCount = 0;
                foreach (var collision in frame.Descendants("Collision"))
                {
                    if (collision.Attribute("Type").Value == "Circle")
                    {
                        string[] pt = collision.Attribute("Pos").Value.Split(',');
                        sf.CollisionData.Add(new Collision_BoundingCircle(
                            idCount++,
                            new Vector2(float.Parse(pt[0]), float.Parse(pt[1])),
                            float.Parse(collision.Attribute("Radius").Value)));
                    }
                    else if (collision.Attribute("Type").Value == "Rectangle")
                    {
                        string[] tl = collision.Attribute("TLPos").Value.Split(',');
                        float tlx = float.Parse(tl[0]);
                        float tly = float.Parse(tl[1]);
                        string[] br = collision.Attribute("BRPos").Value.Split(',');
                        float brx = float.Parse(br[0]);
                        float bry = float.Parse(br[1]);
                        sf.CollisionData.Add(new Collision_AABB(
                            idCount++,
                            new Vector2(tlx, tly),
                            new Vector2(brx, bry)
                            ));
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
                        sf.CollisionData.Add(new Collision_OBB(
                            idCount++,
                            new Vector2(c1x, c1y),
                            new Vector2(c2x, c2y),
                            thickness
                            ));
                    }
                }

                Frames.Add(sf);
            }
        }

        #endregion

        #region Compare
        public static bool operator ==(Animation x, Animation y)
        {
            return x.Name == y.Name;
        }

        public static bool operator !=(Animation x, Animation y)
        {
            return x.Name != y.Name;
        }

        /// <summary>
        /// Check equality by object
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            //has to be an animation
            if (obj as Animation == null)
                return false;
            return this == obj as Animation;
        }

        public override int GetHashCode()
        {
            //\todo Implement
            return base.GetHashCode();
        }

        /// <summary>
        /// Has a good distribution.
        /// </summary>
        const int _multiplier = 89;

        #endregion Compare
    }

    /// <summary>
    /// To render Text
    /// </summary>
    class DummyAnimation : Animation
    {
        /// <summary>
        /// Creates an "animation" with no image.
        /// Needs to load no content.
        /// </summary>
        internal DummyAnimation()
            : this("__anim__")
        {
        }

        /// <summary>
        /// Creates an "animation" with no image.
        /// Needs to load no content.
        /// </summary>
        /// <param name="name">Name of the animation</param>
        internal DummyAnimation(string name)
            : this(name, 0, 0)
        {
        }

        /// <summary>
        /// Creates an "animation" with no image.
        /// Needs to load no content.
        /// </summary>
        /// <param name="name">Name of the animation</param>
        /// <param name="size">A Vector2 containing the width and height
        /// of the resulting animation</param>
        internal DummyAnimation(string name, Vector2 size)
            : this(name, (int)size.X, (int)size.Y)
        {
        }

        /// <summary>
        /// Creates an "animation" with no image.
        /// Needs to load no content.
        /// </summary>
        /// <param name="name">Name of the animation</param>
        /// <param name="width">Width of the "animation"</param>
        /// <param name="height">Height of the "animation"</param>
        internal DummyAnimation(string name, int width, int height)
        {
            Name = name;
            Built = true;
            SpriteFrame frame = new SpriteFrame();
            frame.Width = width;
            frame.Height = height;

            Frames.Add(frame);
        }
    }
}
