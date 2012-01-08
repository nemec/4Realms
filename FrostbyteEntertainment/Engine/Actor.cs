using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frostbyte
{
    /// <summary>
    /// This class is tasked with keeping track of the
    /// curent Animation's state.
    /// </summary>
    class Actor
    {
        #region Properties
        /// <summary>
        /// The frame number.
        /// </summary>
        internal int Frame { get; set; }

        /// <summary>
        /// Index of the current loaded animation.
        /// </summary>
        internal int CurrentAnimation { get; set; }
        #endregion

        #region Variables
        /// <summary>
        /// List of all the Actor's animations.
        /// </summary>
        internal List<Animation> Animations = new List<Animation>();

        #endregion

        #region Constructor
        internal Actor(Animation anim)
        {
            Animations.Add(anim);
        }

        public Actor(List<Animation> anims)
        {
            Animations = anims;
        }

        public Actor(List<string> anims)
        {
            foreach (var anim in anims)
            {
                Animations.Add(This.Game.CurrentLevel.GetAnimation(anim));
            }
        }
        #endregion
    }
}

