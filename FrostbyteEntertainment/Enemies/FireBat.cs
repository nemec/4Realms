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
    internal partial class FireBat : Frostbyte.Enemy
    {
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        #region Variables
        static List<String> Animations = new List<String>(){
           "firebat-down.anim",
           "firebat-diagdown.anim",
           "firebat-right.anim",
           "firebat-diagup.anim",
           "firebat-up.anim",
           "firebat-down.anim",
           "firebat-diagdown.anim",
           "firebat-right.anim",
           "firebat-diagup.anim",
           "firebat-up.anim",
           "firebat-down.anim",
           "firebat-diagdown.anim",
           "firebat-right.anim",
           "firebat-diagup.anim",
           "firebat-up.anim",
        };

        #endregion Variables

        public FireBat(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 2f, 40)
        {
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Fire;
            SpawnPoint = initialPos;
            Personality = new DartWanderPersonality(this);
            Scale = .5f;

            This.Game.AudioManager.AddSoundEffect("Effects/Bat_Move");
            if (MovementAudioName == null)
            {
                MovementAudioName = "Effects/Bat_Move";
                This.Game.AudioManager.InitializeLoopingSoundEffect(MovementAudioName, .06f);
            }

            isDieEffectEnabled = true;
        }

        protected override void updateMovement()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            Sprite target = GetClosestTarget(targets, 600f);
            if (target != null)
            {
                    Personality.Update();
            }
        }

        protected override void updateAttack()
        {
            if (This.gameTime.TotalGameTime >= attackStartTime + new TimeSpan(0, 0, 0, 1, 750) && isAttackAnimDone)
            {
                float range = 450.0f;
                List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
                Sprite target = GetClosestTarget(targets, range);
                if (target != null)
                {
                    attackStartTime = This.gameTime.TotalGameTime;

                    int attackRange = 3;

                    //Create Fire Tier 1 Particle Emmiter
                    Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
                    Texture2D fire = This.Game.CurrentLevel.GetTexture("fireParticle");
                    ParticleEmitter particleEmitterFire = new ParticleEmitter(3000, particleEffect, fire);
                    particleEmitterFire.blendState = BlendState.AlphaBlend;
                    (particleEmitterFire.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                    (particleEmitterFire.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                    particleEmitters.Add(particleEmitterFire);


                    mAttacks.Add(Attacks.T1Projectile(target,
                                                this,
                                                8,
                                                3,
                                                new TimeSpan(0, 0, 0, 0, 750),
                                                attackRange,
                                                6f,
                                                false,
                                                delegate(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter)
                                                {
                                                    Random rand = new Random();
                                                    Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                                    for (int i = -5; i < 6; i++)
                                                    {
                                                        float velocitySpeed = rand.Next(30, 55);
                                                        float accelSpeed = rand.Next(-30, -10);
                                                        particleEmitter.createParticles(direction * velocitySpeed,
                                                                        direction * accelSpeed,
                                                                        particleEmitter.GroundPos,
                                                                        rand.Next(5, 20),
                                                                        rand.Next(50, 300));
                                                    }
                                                },
                                                particleEmitterFire,
                                                new Vector2(0, -38),
                                                Element.Fire
                                                ).GetEnumerator());
                }
            }
        }
    }
}
