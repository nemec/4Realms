using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Frostbyte.Enemies
{
    internal partial class FrostOwl : Frostbyte.Enemy
    {
        #region Variables
        static List<String> Animations = new List<String>(){
           "owl-idle-down.anim",
           "owl-idle-diagdown.anim",
           "owl-idle-right.anim",
           "owl-idle-diagup.anim",
           "owl-idle-up.anim",
           "owl-idle-down.anim",
           "owl-idle-diagdown.anim",
           "owl-idle-right.anim",
           "owl-idle-diagup.anim",
           "owl-idle-up.anim",
           "owl-idle-down.anim",
           "owl-idle-diagdown.anim",
           "owl-idle-right.anim",
           "owl-idle-diagup.anim",
           "owl-idle-up.anim",
        };

        #endregion Variables

        public FrostOwl(string name, Vector2 initialPosition)
            : base(name, new Actor(Animations), 5, 40)
        {
            SpawnPoint = initialPosition;
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new SwoopingPersonality(this);
            //Personality = new ChargePersonality(this);
            ElementType = Element.Water;

            This.Game.AudioManager.AddSoundEffect("Effects/Owl_Move");
            if (MovementAudioName == null)
            {
                MovementAudioName = "Effects/Owl_Move";
                This.Game.AudioManager.InitializeLoopingSoundEffect(MovementAudioName, .03f);
            }

            isDieEffectEnabled = true;
        }

        protected override void updateMovement()
        {
            Personality.Update();
        }

        protected override void updateAttack()
        {
            if (This.gameTime.TotalGameTime >= attackStartTime + new TimeSpan(0, 0, 1) && isAttackAnimDone)
            {
                float range = 450.0f;
                List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
                Sprite target = GetClosestTarget(targets, range);
                if (target != null)
                {
                    attackStartTime = This.gameTime.TotalGameTime;

                    int attackRange = 12;

                    #region Water Tier 1

                    //Create Earth Tier 1 Particle Emmiter
                    Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
                    Texture2D snowflake = This.Game.CurrentLevel.GetTexture("waterParticle");
                    ParticleEmitter particleWaterTier1 = new ParticleEmitter(500, particleEffect, snowflake);
                    particleWaterTier1.effectTechnique = "FadeAtXPercent";
                    particleWaterTier1.fadeStartPercent = .98f;
                    particleWaterTier1.blendState = BlendState.Additive;
                    (particleWaterTier1.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                    (particleWaterTier1.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                    particleEmitters.Add(particleWaterTier1);

                    mAttacks.Add(Attacks.T1Projectile(target,
                                              this,
                                              5,
                                              0,
                                              new TimeSpan(0, 0, 0, 1, 150),
                                              attackRange,
                                              6f,
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
                                              particleWaterTier1,
                                              new Vector2(0, -38),
                                              Element.Water
                                              ).GetEnumerator());
                    #endregion Water Tier 1
                }
            }
        }
    }
}
