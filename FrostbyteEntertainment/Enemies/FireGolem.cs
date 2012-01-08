using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Frostbyte.Enemies
{
    internal partial class FireGolem : Golem
    {
        #region Variables
        static List<String> Animations = new List<String>(){
           "firegolem-idle-down.anim",
           "firegolem-idle-diagdown.anim",
           "firegolem-idle-right.anim",
           "firegolem-idle-diagup.anim",
           "firegolem-idle-up.anim",
           "firegolem-walk-down.anim",
           "firegolem-walk-diagdown.anim",
           "firegolem-walk-right.anim",
           "firegolem-walk-diagup.anim",
           "firegolem-walk-up.anim",
           "firegolem-attack-down.anim",
           "firegolem-attack-diagdown.anim",
           "firegolem-attack-right.anim",
           "firegolem-attack-diagup.anim",
           "firegolem-attack-up.anim",
        };
        #endregion Variables

        public FireGolem(string name, Vector2 initialPos)
            : base(name, initialPos, 200, Animations)
        {
            ElementType = Element.Fire;
            Personality = new WalkingSentinelPersonality(this);
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

                    int attackRange = 17;

                    //Create Particle Emmiter
                    Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
                    Texture2D fire = This.Game.CurrentLevel.GetTexture("fireParticle");
                    ParticleEmitter particleEmitterFire = new ParticleEmitter(1000, particleEffect, fire);
                    particleEmitterFire.effectTechnique = "FadeAtXPercent";
                    particleEmitterFire.fadeStartPercent = .98f;
                    particleEmitterFire.blendState = BlendState.AlphaBlend;
                    (particleEmitterFire.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                    (particleEmitterFire.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();

                    mAttacks.Add(Attacks.T1Projectile(target,
                                              this,
                                              20,
                                              18,
                                              new TimeSpan(0, 0, 0, 1, 750),
                                              attackRange,
                                              3f,
                                              true,
                                              delegate(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter)
                                              {
                                                  Random rand = new Random();
                                                  Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                                  for (int i = -5; i < -4; i++)
                                                  {
                                                          float velocitySpeed = rand.Next(30, 55);
                                                          float accelSpeed = rand.Next(-30, -10);
                                                          particleEmitter.createParticles(-direction * velocitySpeed * 2,
                                                                              direction * accelSpeed * 15,
                                                                              particleEmitter.GroundPos,
                                                                              rand.Next(20, 50),
                                                                              rand.Next(400, 700));
                                                  }
                                              },
                                              particleEmitterFire,
                                              Vector2.Zero).GetEnumerator());
                }
            }
        }
    }
}
