using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frostbyte
{
    class SpriteFrame
    {
        #region Properties
        /// <summary>
        /// Image Texture
        /// </summary>
        internal Texture2D Image { get; set; }

        /// <summary>
        /// Amount of time to pause between
        /// this frame and next.
        /// </summary>
        internal long Pause { get; set; }

        /// <summary>
        /// The offeset from center of the sprite of the image. Defaults to (0,0)
        /// </summary>
        internal Vector2 AnimationPeg { get; set; }

        /// <summary>
        /// The frame's width
        /// </summary>
        internal int Width { get; set; }

        /// <summary>
        ///  The frame's height
        /// </summary>
        internal int Height { get; set; }

        /// <summary>
        /// Position of the top left corner
        /// </summary>
        internal Vector2 StartPos { get; set; }

        /// <summary>
        /// Offset of the image from being mirroable. This value should be the value from what you want to be the center of the flip to what is the actual center of the frame.
        /// </summary>
        public Vector2 MirrorOffset { get; set; }

        #endregion

        #region Variables
        /// <summary>
        ///  Hot spots that can be used for
        ///  locating objects on the sprite
        ///  default is tagged to center
        ///  of the sprite
        /// </summary>
        internal List<Vector2> HotSpots = new List<Vector2>();

        /// <summary>
        /// The collision data for this sprite.
        /// </summary>
        internal List<CollisionObject> CollisionData = new List<CollisionObject>();
        #endregion
    }
}
