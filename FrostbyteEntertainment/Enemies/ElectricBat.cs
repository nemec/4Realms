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
    internal partial class ElectricBat : Frostbyte.Enemy
    {
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        #region Variables
        static List<String> Animations = new List<String>(){
           "bat-down.anim",
           "bat-diagdown.anim",
           "bat-right.anim",
           "bat-diagup.anim",
           "bat-up.anim",
           "bat-down.anim",
           "bat-diagdown.anim",
           "bat-right.anim",
           "bat-diagup.anim",
           "bat-up.anim",
           "bat-down.anim",
           "bat-diagdown.anim",
           "bat-right.anim",
           "bat-diagup.anim",
           "bat-up.anim",
        };

        #endregion Variables

        public ElectricBat(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 20, 40)
        {
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Lightning;
            SpawnPoint = initialPos;
            Personality = new PseudoWanderPersonality(this);
            Scale = .5f;

            This.Game.AudioManager.AddSoundEffect("Effects/Bat_Move");
            if (MovementAudioName == null)
            {
                MovementAudioName = "Effects/Bat_Move";
                This.Game.AudioManager.InitializeLoopingSoundEffect(MovementAudioName, .05f);
            }

            isDieEffectEnabled = true;
        }

        protected override void updateMovement()
        {
            Personality.Update();
        }

        protected override void updateAttack()
        {
            if (This.gameTime.TotalGameTime >= attackStartTime + new TimeSpan(0, 0, 2) && isAttackAnimDone)
            {
                float range = 450.0f;
                List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
                Sprite target = GetClosestTarget(targets, range);
                if (target != null)
                {
                    attackStartTime = This.gameTime.TotalGameTime;

                    int attackRange = 12;

                    //Create Particle Emmiter
                    Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
                    Texture2D lightning = This.Game.CurrentLevel.GetTexture("sparkball");
                    ParticleEmitter particleEmitterLightning = new ParticleEmitter(1000, particleEffect, lightning);
                    particleEmitterLightning.effectTechnique = "FadeAtXPercent";
                    particleEmitterLightning.fadeStartPercent = .98f;
                    particleEmitterLightning.blendState = BlendState.Additive;
                    (particleEmitterLightning.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                    (particleEmitterLightning.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                    particleEmitters.Add(particleEmitterLightning);

                    mAttacks.Add(Attacks.T1Projectile(target,
                                              this,
                                              5,
                                              10,
                                              new TimeSpan(0, 0, 0, 1, 750),
                                              attackRange,
                                              6f,
                                              false,
                                              delegate(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter)
                                              {
                                                  Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                                  for (int i = -5; i < 6; i++)
                                                  {
                                                      particleEmitter.createParticles(-direction * projectileSpeed * 5,
                                                                                                  tangent * -i * 40,
                                                                                                  particleEmitter.GroundPos + tangent * i * ParticleEmitter.EllipsePerspectiveModifier - direction * (Math.Abs(i) * 7),
                                                                                                  4,
                                                                                                  300);
                                                  }
                                              },
                                              particleEmitterLightning,
                                              Vector2.Zero).GetEnumerator());
                }
            }
        }
    }
}
