using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Audio;
using System.Collections;

namespace Frostbyte
{
    internal abstract partial class Enemy : OurSprite
    {
        #region Variables
        //Movement
        //protected enum movementTypes { Charge, PulseCharge, Ram, StealthCharge, StealthCamp, StealthRetreat, Retreat, TeaseRetreat, Swap, Freeze };
        //protected movementTypes currentMovementType = 0;
        internal TimeSpan movementStartTime = TimeSpan.MaxValue;
        internal TimeSpan attackStartTime = TimeSpan.Zero;

        //protected EnemyStatus Status = EnemyStatus.Wander;
        internal IPersonality Personality;
        internal string MovementAudioName;
        internal ProgressBar healthBar;
        private static Vector2 barSize = new Vector2(50, 10);
        #endregion Variables

        public Enemy(string name, Actor actor, float speed, int _health)
            : base(name, actor)
        {
            FrostbyteLevel l = This.Game.LoadingLevel as FrostbyteLevel;
            Personality = new WanderingMinstrelPersonality(this);
            UpdateBehavior = update;
            l.enemies.Add(this);
            EndBehavior = Die;
            Speed = speed;
            Health = _health;
            CollidesWithBackground = true;

            #region HealthBar
            float alpha = 0.15f;
            healthBar = new ProgressBar("Health_" + Name, MaxHealth,
                Color.Black * 0, Color.Red * 0.5f, Color.Black * alpha, barSize);
            healthBar.Pos = Pos - l.Camera.Pos + new Vector2(0, -20);
            healthBar.Static = true;
            healthBar.Value = MaxHealth;

            HealthChanged += delegate(object obj, int value)
            {
                if (healthBar != null)
                {
                    healthBar.Value = value;
                    if (value == 0)
                    {
                        This.Game.CurrentLevel.RemoveSprite(healthBar);
                    }
                }
            };
            #endregion
        }

        protected override void Die()
        {
            if (isDieEffectEnabled)
            {
                #region Default DeathEffect Particle Emitter
                Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
                Texture2D lightning = This.Game.CurrentLevel.GetTexture("light blood");
                ParticleEmitter particleEmitterDeath = new ParticleEmitter(5000, particleEffect, lightning);
                particleEmitterDeath.effectTechnique = "NoSpecialEffect";
                particleEmitterDeath.fadeStartPercent = .8f;
                particleEmitterDeath.blendState = BlendState.AlphaBlend;
                particleEmitterDeath.Static = false;
                #endregion Default DeathEffect Particle Emitter

                new DeathEffect(this, particleEmitterDeath, sampleWidthPercent, sampleHeightPercent);
            }

            FrostbyteLevel l = This.Game.CurrentLevel as FrostbyteLevel;
            // Remove Sprite
            l.RemoveSprite(this);

            foreach (ParticleEmitter p in particleEmitters)
                p.Remove();
            this.particleEmitters.Clear();

            // Remove enemy from target list so that we don't target blank space where an enemy died
            l.enemies.Remove(this);
            base.Die();
        }

        public void update()
        {
            FrostbyteLevel l = (This.Game.CurrentLevel as FrostbyteLevel);
            //necessary for collision
            if(this.CollidesWithBackground)
                previousFootPos = this.GroundPos;

            Vector2 previousDirection = Direction;
            SpriteState previousState = State;

            if (isAttackAnimDone)
            {
                PreviousPos = Pos;
                updateMovement();

                //perform collision detection with background
                if (this.CollidesWithBackground)
                    checkBackgroundCollisions();
            }

            if (healthBar != null)
            {
                healthBar.Pos = GroundPos - l.Camera.Pos + new Vector2(-healthBar.Width / 2, -(healthBar.Height + GetAnimation().Height / 2));
            }

            updateAttack();


            if (Health <= 0)
            {
                this.EndBehavior();
                return;
            }
            List<Sprite> targets = l.allies;
            if (MovementAudioName != null)
            {
                if (GetClosestTarget(targets, This.Game.GraphicsDevice.Viewport.Width * 1.5f) != null)
                {
                    if (PreviousPos != Pos)
                    {
                        This.Game.AudioManager.PlayLoopingSoundEffect(MovementAudioName);
                    }
                }
            }

            if (isAttackAnimDone)
            {
                State = PreviousPos == Pos ? SpriteState.Idle : SpriteState.Moving;

                if (Direction != previousDirection || previousState != State)
                {
                    #region update animation facing direction
                    switch (Orientation)
                    {
                        case Orientations.Down:
                            SetAnimation(0 + 5 * State.GetHashCode());
                            break;
                        case Orientations.Down_Right:
                            Hflip = false;
                            SetAnimation(1 + 5 * State.GetHashCode());
                            break;
                        case Orientations.Down_Left:
                            Hflip = true;
                            SetAnimation(1 + 5 * State.GetHashCode());
                            break;
                        case Orientations.Right:
                            Hflip = false;
                            SetAnimation(2 + 5 * State.GetHashCode());
                            break;
                        case Orientations.Left:
                            Hflip = true;
                            SetAnimation(2 + 5 * State.GetHashCode());
                            break;
                        case Orientations.Up_Right:
                            Hflip = false;
                            SetAnimation(3 + 5 * State.GetHashCode());
                            break;
                        case Orientations.Up_Left:
                            Hflip = true;
                            SetAnimation(3 + 5 * State.GetHashCode());
                            break;
                        case Orientations.Up:
                            SetAnimation(4 + 5 * State.GetHashCode());
                            break;
                    }
                    #endregion
                }
            }
        }

        /// \todo what is this for?
        //private override void checkBackgroundCollisions()
        //{
        //    //throw new NotImplementedException();
        //}
        protected abstract void updateMovement();
        protected abstract void updateAttack();

        /// \todo create projectile class (projectiles modify health of enemies/players) 
        /// \todo complete checkBackgroundCollisions
    }

    internal abstract partial class Boss : Enemy
    {
        internal Boss(string name, Actor actor, float speed, int health)
            : base(name, actor, speed, health)
        {
            Enabled = true;

            HealthChanged += delegate(object boss, int value)
            {
                if (!AtArms && value != MaxHealth)
                {
                    setAtArms();
                }
            };

            This.Game.LoadingLevel.RemoveSprite(healthBar);
            healthBar = null;
        }

        internal bool Enabled;
        internal bool AtArms = false;

        protected override void Die()
        {
            FrostbyteLevel l = (This.Game.CurrentLevel as FrostbyteLevel);
            l.HUD.RemoveBossHealthBar(this);
            l.SpawnExitPortal(SpawnPoint);
            base.Die();
        }

        internal void setAtArms()
        {
            AtArms = true;
            (This.Game.LoadingLevel as FrostbyteLevel).HUD.AddBossHealthBar(this);
        }

        internal override void Update()
        {
            if (Enabled)
            {
                base.Update();
            }
        }
    }

    internal class BossDeath : OurSprite
    {
        internal BossDeath(string name, Actor actor)
            : base(name, actor)
        {
            UpdateBehavior = update;
            mStates = states().GetEnumerator();
        }

        private IEnumerator mStates;

        internal void update()
        {
            if(!mStates.MoveNext())
            {
                Health = 0;
            }
        }

        internal IEnumerable states()
        {
            while (Frame != FrameCount() - 1)
            {
                yield return null;
            }
            StopAnim();
        }
    }
}
