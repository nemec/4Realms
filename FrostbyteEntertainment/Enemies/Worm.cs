using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte.Enemies
{

    internal partial class Worm : Frostbyte.Boss
    {
        #region Variables
        static List<String> Animations = new List<String>(){
           "worm-idle-down.anim",
           "worm-idle-diagdown.anim",
           "worm-idle-right.anim",
           "worm-idle-diagup.anim",
           "worm-idle-up.anim",  // 4
           "worm-underground.anim",
           "worm-underground.anim",
           "worm-underground.anim",
           "worm-underground.anim",
           "worm-underground.anim",  // 9
           "worm-spew-down.anim",
           "worm-spew-diagdown.anim",
           "worm-spew-right.anim",
           "worm-spew-diagup.anim",
           "worm-spew-up.anim",  // 14
           "worm-underground.anim",
           "worm-submerge.anim",  // Go-underground
           "worm-surface-down.anim",  // Go-above-ground
           "worm-vomit.anim",
           "worm-die.anim",
        };
        internal bool HasVomited = false;
        internal bool IsSubmerged;
        internal TimeSpan attackWait = TimeSpan.Zero;
        internal static Random rng = new Random();
        #endregion Variables

        public Worm(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 1, 2000)
        {
            SpawnPoint = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Earth;
            Personality = new UndergroundAttackPersonality(this);
            SetAnimation(15);
            IsSubmerged = true;
            Speed = .65f;
        }
        
        protected override void Die()
        {
            BossDeath b = new BossDeath("die", new Actor(mActor.Animations[19]));
            b.GroundPos = GroundPos;
            Frostbyte.Characters.Mage.UnlockedSpells = Spells.EarthOne;
            (This.Game.CurrentLevel as FrostbyteLevel).HUD.ScrollText("You feel the strength of the earth rush in through your feet.\n\nHold down the Left trigger and press A (gamepad) S (keyboard) and then release the trigger to cast an earth spell!\n\nUse RB (gamepad) or E (keyboard) or to target an enemy and LB or Q to target an ally.");
            base.Die();
        }

        protected override void updateMovement()
        {
            Personality.Update();
        }

        protected override void updateAttack()
        {
            if (attackWait < This.gameTime.TotalGameTime)
            {
                if (isAttackAnimDone && !IsSubmerged)
                {
                    if (HasVomited || This.Game.rand.Next(4) != 0)
                    {
                        #region Spew
                        List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
                        Sprite target = GetClosestTarget(targets);
                        if (target != null)
                        {
                            int attackRange = 11;

                            //Create Particle Emitter
                            Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
                            Texture2D boulder = This.Game.CurrentLevel.GetTexture("boulder");

                            foreach (TimeSpan delay in new TimeSpan[]{
                                TimeSpan.Zero, new TimeSpan(0, 0, 0, 1, 500), new TimeSpan(0, 0, 3),
                            })
                            {
                                ParticleEmitter particleEmitterEarth = new ParticleEmitter(1000, particleEffect, boulder);
                                particleEmitterEarth.effectTechnique = "NoSpecialEffect";
                                particleEmitterEarth.blendState = BlendState.AlphaBlend;
                                (particleEmitterEarth.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                                (particleEmitterEarth.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                                particleEmitters.Add(particleEmitterEarth);

                                Vector2 oldDirection = Direction;
                                double angle = Math.Atan2(Direction.Y, Direction.X);
                                SpriteFrame frame = GetAnimation();
                                for (int x = 0; x < 3; x++)
                                {
                                    mAttacks.Add(LevelFunctions.DelayEnumerable<bool>(Attacks.T1Projectile(target,
                                        this,
                                        5,
                                        20,
                                        new TimeSpan(0, 0, 0, 1, 750),
                                        attackRange,
                                        6f,
                                        false,
                                        delegate(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter)
                                        {
                                            Random randPosition = new Random();
                                            particleEmitter.createParticles(direction * projectileSpeed, Vector2.Zero, particleEmitterEarth.GroundPos, 10, 10);
                                            Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                            for (int i = -5; i < 6; i++)
                                            {
                                                particleEmitter.createParticles(-direction * projectileSpeed * 5,
                                                    tangent * -i * 40,
                                                    particleEmitter.GroundPos + tangent * i * ParticleEmitter.EllipsePerspectiveModifier + (float)randPosition.NextDouble() * direction * 8f,
                                                    1.5f,
                                                    300);
                                            }
                                        },
                                        particleEmitterEarth,
                                        new Vector2(0, -20)), delay).GetEnumerator());

                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        SetAnimation(18);
                        mAttacks.Add(Attacks.WormVomit(this, 18, 1, 24).GetEnumerator());
                        HasVomited = true;
                    }
                }
                attackWait = This.gameTime.TotalGameTime + new TimeSpan(0, 0, rng.Next(1, 3));
            }
        }
    }
}
