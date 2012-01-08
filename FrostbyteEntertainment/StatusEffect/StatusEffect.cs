using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frostbyte
{
    /// <summary>
    /// Represents a status effect with a given action.
    /// </summary>
    internal class StatusEffect
    {
        #region Variables
        /// <summary>
        /// If true, the Effect is done.
        /// </summary>
        public bool Expired { get; set; }

        /// <summary>
        /// The time the effect should last
        /// </summary>
        public TimeSpan Time = new TimeSpan();

        /// <summary>
        /// Effect the Status effect has
        /// </summary>
        Behavior Effect;

        /// <summary>
        /// Particle emmitter if we want the effect to sparkle
        /// </summary>
        internal ParticleEmitter particleEmitter;

        /// <summary>
        /// The number of times this has been drawn.
        /// </summary>
        protected int count=0;
        #endregion Variables

        internal StatusEffect(TimeSpan time, Behavior effect)
        {
            Expired = false;
            Time = This.gameTime.TotalGameTime + time;
            Effect = effect;
        }

        internal void Update()
        {
            if (!Expired)
            {
                if (Time > This.gameTime.TotalGameTime)
                {
                    Effect();
                }
                else
                {
                    Expired = true;
                }
            }
        }

        internal virtual void Draw(Sprite target)
        {
        }
    }
}
