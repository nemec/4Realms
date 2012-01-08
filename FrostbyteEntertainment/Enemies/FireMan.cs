using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte.Enemies
{
    internal partial class FireMan : Frostbyte.Boss
    {
        #region Variables
        static List<String> Animations = new List<String>(){
           "fireman-idle-down.anim",
           "fireman-idle-diagdown.anim",
           "fireman-idle-right.anim",
           "fireman-idle-diagup.anim",
           "fireman-idle-up.anim",  // 4
           "fireman-walk-down.anim",
           "fireman-walk-diagdown.anim",
           "fireman-walk-right.anim",
           "fireman-walk-diagup.anim",
           "fireman-walk-up.anim",  // 9
           "fireman-attack-down.anim",
           "fireman-attack-diagdown.anim",
           "fireman-attack-right.anim",
           "fireman-attack-diagup.anim",
           "fireman-attack-up.anim",  // 14
           "fireman-die.anim",
        };

        internal TimeSpan attackWait = TimeSpan.MaxValue;
        #endregion Variables

        public FireMan(string name, Vector2 initialPosition)
            : base(name, new Actor(Animations), 20, 1500)
        {
            Speed = 1f;
            SpawnPoint = initialPosition;
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Fire;
            Personality = new LumberingPersonality(this);
            This.Game.AudioManager.AddBackgroundMusic("Music/FireBoss");
            This.Game.AudioManager.AddSoundEffect("Effects/Golem_Attack");
            This.Game.AudioManager.AddSoundEffect("Effects/Golem_Move");
        }

        protected override void Die()
        {
            FrostbyteLevel l = (This.Game.CurrentLevel as FrostbyteLevel);
            BossDeath b = new BossDeath("die", new Actor(mActor.Animations[15]));
            b.GroundPos = GroundPos;
            l.HUD.ScrollText("You feel a strong wave of heat flow throughout your body.\n\nHold down the Left trigger and press B, then release the trigger to cast a Fire spell! \n\n Try casting Tier 3 spells!");
            base.Die();
        }

        protected override void updateMovement()
        {
            Personality.Update();
        }

        protected override void updateAttack()
        {
            FrostbyteLevel l = (This.Game.CurrentLevel as FrostbyteLevel);
            Sprite currentTarget = GetClosestTarget(l.allies);
            int attackFrame = 0;

            if (attackWait < This.gameTime.TotalGameTime)
            {
                int randAttack = This.Game.rand.Next(14);
                if (randAttack < 10)
                {
                    #region Fire Tier 1
                    int attackRange = 11;

                    //Create Earth Tier 1 Particle Emmiter
                    Effect particleEffect = l.GetEffect("ParticleSystem");
                    Texture2D snowflake = l.GetTexture("fireParticle");

                    foreach (TimeSpan delay in new TimeSpan[]{
                        TimeSpan.Zero, new TimeSpan(0, 0, 0, 0, 500), new TimeSpan(0, 0, 1),
                    })
                    {
                        ParticleEmitter particleFireTier1 = new ParticleEmitter(500, particleEffect, snowflake);
                        particleFireTier1.effectTechnique = "FadeAtXPercent";
                        particleFireTier1.fadeStartPercent = .98f;
                        particleFireTier1.blendState = BlendState.Additive;
                        (particleFireTier1.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                        (particleFireTier1.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                        particleEmitters.Add(particleFireTier1);
                        mAttacks.Add(LevelFunctions.DelayEnumerable<bool>(Attacks.T1Projectile(currentTarget,
                            this,
                            20,
                            attackFrame,
                            new TimeSpan(0, 0, 0, 1, 150),
                            attackRange,
                            9f,
                            false,
                            delegate(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter)
                            {
                                Random randPosition = new Random();
                                particleEmitter.createParticles(direction * projectileSpeed, Vector2.Zero, particleEmitter.GroundPos, 10, 10);
                                Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                for (int i = -5; i < 6; i++)
                                {
                                    particleEmitter.createParticles(-direction * projectileSpeed * .75f,
                                                                            tangent * -i * 40,
                                                                            particleEmitter.GroundPos + tangent * i * ParticleEmitter.EllipsePerspectiveModifier + (float)randPosition.NextDouble() * direction * 8f,
                                                                            10.0f,
                                                                            This.Game.rand.Next(10, 300));
                                }
                            },
                            particleFireTier1,
                            new Vector2(0, -38),
                            Element.Fire
                            ), delay).GetEnumerator());
                    }

                    This.Game.AudioManager.PlaySoundEffect("Effects/Fire_T1");
                    #endregion Fire Tier 1
                }
                else if (randAttack < 11)
                {
                    #region Fire Tier 3
                    mAttacks.Add(Attacks.FirePillar(currentTarget, this, 30, attackFrame, Element.Fire).GetEnumerator());
                    This.Game.AudioManager.PlaySoundEffect("Effects/Fire_T3");
                    #endregion Fire Tier 3
                }
                else if (randAttack < 14)
                {
                    mAttacks.Add(Attacks.RammingAttack(currentTarget, this, 5, new TimeSpan(0, 0, 0, 0, 500)).GetEnumerator());
                }
                attackWait = This.gameTime.TotalGameTime + new TimeSpan(0, 0, This.Game.rand.Next(5, 8));
            }
        }
    }
}
