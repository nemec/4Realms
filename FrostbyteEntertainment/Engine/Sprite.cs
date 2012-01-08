using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    class Sprite : WorldObject
    {
        internal Sprite(string name, Actor actor)
        {
            mName = name;
            mActor = actor;
            mLastUpdate = new GameTime();

            LoadBehavior = () => { };
            UpdateBehavior = () => { };
            EndBehavior = () => { };

            //adds the sprite to the level
            This.Game.LoadingLevel.AddSprite(this);
            Speed = 1;

            if (mActor != null)
            {
                if (mActor.Animations[mActor.CurrentAnimation].Built)
                {
                    if (mActor.Animations[mActor.CurrentAnimation].NumFrames > 1) mAnimating = true;
                }
            }

            Center = new Vector2(GetAnimation().Width / 2, GetAnimation().Height / 2);
        }

        internal Sprite(string name, Actor actor, int collisionlist)
            : this(name, actor)
        {
            CollisionList = collisionlist;
        }

        #region Properties
        /// <summary>
        ///     changes to the specified frame of the animation beginning at 0
        /// </summary>
        internal int Frame
        {
            get
            {
                return mActor.Frame;
            }
        }

        /// <summary>
        /// Returns the total number of frames in a particular animation
        /// </summary>
        internal int FrameCount()
        {
            return mActor.Animations[mActor.CurrentAnimation].NumFrames;
        }

        /// <summary>
        ///     the sprite's speed
        /// </summary>
        internal float Speed { get; set; }

        /// <summary>
        /// State for moving, idling, or attacking.
        /// </summary>
        internal SpriteState State = SpriteState.Idle;

        internal List<ParticleEmitter> particleEmitters = new List<ParticleEmitter>();

        #endregion Properties

        #region Variables

        /// <summary>
        /// The color value to multiply the image's color data with.
        /// </summary>
        public Color mColor = Color.White;

        internal Vector2 PreviousPos = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

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
        /// This Sprite's Actor.
        /// </summary>
        protected Actor mActor;
        #endregion Variables

        #region Behaviors
        /// <summary>
        /// Sprite's Load Behavior
        /// </summary>
        internal Behavior LoadBehavior;

        /// <summary>
        /// Sprite's Update Behavior
        /// </summary>
        internal Behavior UpdateBehavior;

        /// <summary>
        /// Sprite's End Behavior
        /// </summary>
        internal Behavior EndBehavior;
        #endregion Behaviors

        #region Methods

        internal override List<CollisionObject> GetCollision()
        {
            return GetAnimation().CollisionData;
        }

        internal override List<Vector2> GetHotSpots()
        {
            return GetAnimation().HotSpots;
        }

        /// <summary>
        ///     changes to the specified animation beginning at 0
        /// </summary>
        internal SpriteFrame GetAnimation()
        {
            //continue on same frame uncomment to start anim from beginning
            mActor.Frame = mActor.Frame % mActor.Animations[mActor.CurrentAnimation].Frames.Count;
            return mActor.Animations[mActor.CurrentAnimation].Frames[mActor.Frame];
        }

        /// <summary>
        /// changes to the specified animation beginning at 0.
        /// </summary>
        /// <param name="animation">The animation to select (begins at 0)</param>
        internal void SetAnimation(int animation)
        {
            //give us the best one we can
            mActor.CurrentAnimation = mActor.Animations.Count > animation ? animation : mActor.Animations.Count - 1;
            //continue on same frame uncomment to start anim from beginning
            mActor.Frame = mActor.Frame % mActor.Animations[mActor.CurrentAnimation].Frames.Count;
        }

        /// <summary>
        /// Pauses or resumes an animation.
        /// </summary>
        internal void ToggleAnim() { mAnimating = !mAnimating; }

        /// <summary>
        /// Causes the animation to play.
        /// </summary>
        internal void StartAnim() { mAnimating = true; }

        /// <summary>
        /// Causes the animation to stop.
        /// </summary>
        internal void StopAnim() { mAnimating = false; }

        /// <summary>
        ///Resets the Sprite's animation to the first frame.
        /// </summary>
        internal void Rewind() { mActor.Frame = 0; }

        public override string ToString()
        {
            return Name;
        }

        #region Collision


        /// <summary>
        /// checks for collision with sprite of a given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal Sprite CollisionWithSprite(string name)
        {
            //        //get the frame for readability
            //SpriteFrame frame = GetAnimation();
            ////get the first sprite with this name
            //Sprite s= This.Game.getCurrentLevel().findSpriteByName(name);
            //if(s==null)
            //    return null;
            //for(int i=0; i <  frame.CollisionData.Count; i++)
            //    if(frame.CollisionData[i].checkCollisions(s.getCollisionData(), s.Pos + s.GetAnimation().AnimationPeg, Pos + frame.AnimationPeg))//if there is a collision
            //        return s;
            return null;//if there aren't collisions
        }
        /// <summary>
        /// Returns collision data
        /// </summary>
        /// <returns></returns>
        //internal vector<Collision> getCollisionData();
        #endregion Collision

        internal override void Update()
        {
            UpdateBehavior();
        }

        #endregion Methods

        #region Draw
        /// <summary>
        /// Draw the Scene
        /// </summary>
        /// <param name="gameTime">Game time as given by the game class</param>
        internal override void Draw(GameTime gameTime)
        {
            //Frame so we don't have to find it so often
            SpriteFrame frame = GetAnimation();
            if (mAnimating == true && !This.Game.CurrentLevel.Paused)
            {
                //used to update the animation. Occurs once the frame's pause * sprite's speed occurs.
                if (Speed>0 && mLastUpdate.TotalGameTime.TotalMilliseconds + frame.Pause / Speed < gameTime.TotalGameTime.TotalMilliseconds)
                {
                    mActor.Frame = (mActor.Frame + 1) % mActor.Animations[mActor.CurrentAnimation].NumFrames;
                    //update frame so we don't need to worry
                    frame = mActor.Animations[mActor.CurrentAnimation].Frames[mActor.Frame];
                    mLastUpdate = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);
                }
            }
            if (Visible == true && State != SpriteState.Dead && frame.Image != null && !frame.Image.IsDisposed)
            {
                Vector2 peg = GetAnimation().AnimationPeg;
                This.Game.spriteBatch.Draw(
                        frame.Image,
                        Pos -
                            peg + /// \todo need some sort of notion of offset form centerpoint for the item that is not the animation peg so that we know the origin of the image.
                            Center-Center*Scale + //this places scaling in the correct spot (i think)
                            (Hflip ? frame.MirrorOffset * 2 : -frame.MirrorOffset)
                        ,
                        new Rectangle((int)frame.StartPos.X, (int)frame.StartPos.Y, frame.Width, frame.Height),
                        mColor,
                        Angle,
                        Vector2.Zero,
                        Scale,
                        Hflip ?
                            Vflip ?
                                SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically
                                : SpriteEffects.FlipHorizontally
                            :
                            Vflip ?
                                 SpriteEffects.FlipVertically
                                : SpriteEffects.None
                        ,
                        0
                    );
            }
        }
        #endregion Draw
    }
}
