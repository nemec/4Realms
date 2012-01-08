using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte.Enemies
{
    internal partial class FinalBoss : Frostbyte.Boss
    {
        #region Animations
        static List<String> Animations = new List<String>(){
           "player1-idle-down.anim",
           "player1-idle-diagdown.anim",
           "player1-idle-right.anim",
           "player1-idle-diagup.anim",
           "player1-idle-up.anim",
           "player1-walk-down.anim",
           "player1-walk-diagdown.anim",
           "player1-walk-right.anim",
           "player1-walk-diagup.anim",
           "player1-walk-up.anim",
           "player1-attack-down.anim",
           "player1-attack-diagdown.anim",
           "player1-attack-right.anim",
           "player1-attack-diagup.anim",
           "player1-attack-up.anim",
           "player1-spellcast-down.anim",
           "player1-spellcast-diagdown.anim",
           "player1-spellcast-right.anim",
           "player1-spellcast-diagup.anim",
           "player1-spellcast-up.anim",
           "player1-swordcast-down.anim",
           "player1-swordcast-diagdown.anim",
           "player1-swordcast-right.anim",
           "player1-swordcast-diagup.anim",
           "player1-swordcast-up.anim",
        };
        #endregion

        internal TimeSpan attackWait = TimeSpan.MaxValue;
        internal static Random rng = new Random();

        public FinalBoss(string name, Vector2 initialPosition)
            : base(name, new Actor(Animations), 20, 2000)
        {
            SpawnPoint = initialPosition;
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.None;
            Speed = 2.5f;

            Personality = new DarkLinkPersonality(this);

            #region Audio
            This.Game.AudioManager.AddSoundEffect("Effects/Sword_Attack");
            This.Game.AudioManager.AddSoundEffect("Effects/Lightning_Strike");
            This.Game.AudioManager.AddSoundEffect("Effects/Earthquake");
            This.Game.AudioManager.AddSoundEffect("Effects/RockShower");
            This.Game.AudioManager.AddSoundEffect("Effects/Lightning_T1");
            This.Game.AudioManager.AddSoundEffect("Effects/Water_T1");
            This.Game.AudioManager.AddSoundEffect("Effects/Water_T2");
            This.Game.AudioManager.AddSoundEffect("Effects/Water_T3");
            This.Game.AudioManager.AddSoundEffect("Effects/Fire_T1");
            This.Game.AudioManager.AddSoundEffect("Effects/Fire_T2");
            This.Game.AudioManager.AddSoundEffect("Effects/Fire_T3");
            This.Game.AudioManager.AddSoundEffect("Effects/Earth_T1");
            #endregion Audio
        }

        protected override void Die()
        {
            (This.Game.CurrentLevel as FrostbyteLevel).HUD.ScrollText(
                "Congratulations, you've stopped Caelestis! You are awesome! Also...\n\nYou've made the gods proud.");
            base.Die();
        }

        protected override void updateMovement()
        {
            Personality.Update();
        }

        protected override void updateAttack()
        {
            FrostbyteLevel l = (This.Game.CurrentLevel as FrostbyteLevel);
            if (isAttackAnimDone)
            {
                int attackTier = 0;
                Sprite currentTarget = GetClosestTarget(l.allies);
                if (attackWait < This.gameTime.TotalGameTime){
                    int randTier = rng.Next(20);
                    if (randTier < 2)
                    {
                        attackTier = 3;
                    }
                    else if (randTier < 7 && GetTargetsInRange(l.allies, 125).Count != 0)
                    {
                        attackTier = 2;
                    }
                    else
                    {
                        attackTier = 1;
                    }
                    
                    attackWait = This.gameTime.TotalGameTime + new TimeSpan(0, 0, rng.Next(3, 5));
                }

                Element type = new Element[]{ 
                        Element.Earth, 
                        Element.Fire, 
                        Element.Water, 
                        Element.Lightning 
                    }.GetRandomElement();

                #region Do Attacks
                switch (type)
                {
                    case Element.Earth:
                        if (attackTier == 1)
                        {
                            #region Earth Tier 1
                            int attackRange = 11;

                            //Create Earth Tier 1 Particle Emmiter
                            Effect particleEffect = l.GetEffect("ParticleSystem");
                            Texture2D boulder = l.GetTexture("boulder");
                            ParticleEmitter particleEarthTier1 = new ParticleEmitter(1000, particleEffect, boulder);
                            particleEarthTier1.effectTechnique = "NoSpecialEffect";
                            particleEarthTier1.blendState = BlendState.AlphaBlend;
                            (particleEarthTier1.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                            (particleEarthTier1.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                            particleEmitters.Add(particleEarthTier1);

                            mAttacks.Add(Attacks.T1Projectile(currentTarget,
                                this,
                                20,
                                10,
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
                                            1.5f,
                                            300);
                                    }
                                },
                                particleEarthTier1,
                                new Vector2(0, -38),
                                Element.Earth
                                ).GetEnumerator());
                            This.Game.AudioManager.PlaySoundEffect("Effects/Earth_T1", .1f);
                            #endregion Earth Tier 1
                        }
                        else if (attackTier == 2)
                        {
                            #region Earth Tier 2
                            mAttacks.Add(Attacks.Earthquake(this, this, 10, 10).GetEnumerator());
                            #endregion Earth Tier 2
                        }
                        else if (attackTier == 3)
                        {
                            #region Earth Tier 3
                            mAttacks.Add(Attacks.RetreatingAttack(currentTarget, this, 400, new TimeSpan(0, 0, 2),
                                Attacks.RockShower(currentTarget, this, 1, 10)).GetEnumerator());
                            This.Game.AudioManager.PlaySoundEffect("Effects/RockShower");
                            #endregion Earth Tier 3
                        }
                        break;
                    case Element.Lightning:
                        if (attackTier == 1)
                        {
                            #region Lightning Tier 1

                            int attackRange = 3;

                            //Create Lightning Tier 1 Particle Emmiter
                            Effect particleEffect = l.GetEffect("ParticleSystem");
                            Texture2D lightning = l.GetTexture("sparkball");
                            ParticleEmitter particleLightningTier1 = new ParticleEmitter(1000, particleEffect, lightning);
                            particleLightningTier1.effectTechnique = "FadeAtXPercent";
                            particleLightningTier1.fadeStartPercent = .98f;
                            particleLightningTier1.blendState = BlendState.Additive;
                            (particleLightningTier1.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                            (particleLightningTier1.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                            particleEmitters.Add(particleLightningTier1);

                            mAttacks.Add(Attacks.T1Projectile(currentTarget,
                                                        this,
                                                        20,
                                                        10,
                                                        new TimeSpan(0, 0, 0, 1, 250),
                                                        attackRange,
                                                        8f,
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
                                                        particleLightningTier1,
                                                        new Vector2(0, -38),
                                                        Element.Lightning
                                                        ).GetEnumerator());
                            This.Game.AudioManager.PlaySoundEffect("Effects/Lightning_T1", .1f);
                            #endregion Lightning Tier 1
                        }
                        else if (attackTier == 2)
                        {
                            #region Lightning Tier 2
                            mAttacks.Add(Attacks.LightningStrike(this, this, 1, 10).GetEnumerator());
                            #endregion Lightning Tier 2
                        }
                        else if (attackTier == 3)
                        {
                            #region Lightning Tier 3
                            mAttacks.Add(Attacks.RetreatingAttack(currentTarget, this, 400, new TimeSpan(0, 0, 2),
                                Attacks.LightningStrike(currentTarget, this, 1, 10)).GetEnumerator());
                            #endregion Lightning Tier 3
                        }
                        break;
                    case Element.Water:
                        if (attackTier == 1)
                        {
                            #region Water Tier 1

                            int attackRange = 11;

                            //Create Earth Tier 1 Particle Emmiter
                            Effect particleEffect = l.GetEffect("ParticleSystem");
                            Texture2D snowflake = l.GetTexture("waterParticle");
                            ParticleEmitter particleWaterTier1 = new ParticleEmitter(500, particleEffect, snowflake);
                            particleWaterTier1.effectTechnique = "FadeAtXPercent";
                            particleWaterTier1.fadeStartPercent = .98f;
                            particleWaterTier1.blendState = BlendState.Additive;
                            (particleWaterTier1.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                            (particleWaterTier1.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                            particleEmitters.Add(particleWaterTier1);

                            mAttacks.Add(Attacks.T1Projectile(currentTarget,
                                                        this,
                                                        20,
                                                        10,
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
                                                        particleWaterTier1,
                                                        new Vector2(0, -38),
                                                        Element.Water
                                                        ).GetEnumerator());

                            This.Game.AudioManager.PlaySoundEffect("Effects/Water_T1");
                            #endregion Water Tier 1
                        }
                        else if (attackTier == 2)
                        {
                            #region Water Tier 2
                            mAttacks.Add(Attacks.WaterPush(this, 10).GetEnumerator());
                            This.Game.AudioManager.PlaySoundEffect("Effects/Water_T2", .1f);
                            #endregion Water Tier 2
                        }
                        else if (attackTier == 3)
                        {
                            #region Water Tier 3
                            mAttacks.Add(Attacks.RetreatingAttack(currentTarget, this, 400, new TimeSpan(0, 0, 2),
                                Attacks.Freeze(currentTarget, this, 10)).GetEnumerator());
                            This.Game.AudioManager.PlaySoundEffect("Effects/Water_T3");
                            #endregion Water Tier 3
                        }
                        break;
                    case Element.Fire:
                        if (attackTier == 1)
                        {
                            #region Fire Tier 1

                            int attackRange = 11;

                            //Create Fire Tier 1 Particle Emmiter
                            Effect particleEffect = l.GetEffect("ParticleSystem");
                            Texture2D fire = l.GetTexture("fireParticle");
                            ParticleEmitter particleFireTier1 = new ParticleEmitter(3000, particleEffect, fire);
                            particleFireTier1.effectTechnique = "NoSpecialEffect";
                            particleFireTier1.blendState = BlendState.AlphaBlend;
                            (particleFireTier1.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                            (particleFireTier1.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                            particleEmitters.Add(particleFireTier1);


                            mAttacks.Add(Attacks.T1Projectile(currentTarget,
                                                        this,
                                                        30,
                                                        10,
                                                        new TimeSpan(0, 0, 0, 0, 750),
                                                        attackRange,
                                                        9f,
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
                                                        particleFireTier1,
                                                        new Vector2(0, -38),
                                                        Element.Fire
                                                        ).GetEnumerator());
                            This.Game.AudioManager.PlaySoundEffect("Effects/Fire_T1", .2f);
                            #endregion Fire Tier 1
                        }
                        else if (attackTier == 2)
                        {
                            #region Fire Tier 2
                            mAttacks.Add(Attacks.FireRing(this, this, 1, 10).GetEnumerator());
                            This.Game.AudioManager.PlaySoundEffect("Effects/Fire_T2", .05f);
                            #endregion Fire Tier 2
                        }
                        else if (attackTier == 3)
                        {
                            #region Fire Tier 3
                            mAttacks.Add(Attacks.RetreatingAttack(currentTarget, this, 400, new TimeSpan(0, 0, 2),
                                Attacks.FirePillar(currentTarget, this, 10, 10)).GetEnumerator());
                            This.Game.AudioManager.PlaySoundEffect("Effects/Fire_T3");
                            #endregion Fire Tier 3
                        }
                        break;
                    default: 
                        List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                        Collision.CollisionData.TryGetValue(this, out collidedWith);
                        if (collidedWith != null)
                        {
                            foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                            {
                                if (detectedCollision.Item2 is Player)
                                {
                                    mAttacks.Add(Attacks.Melee(this, 5, 11).GetEnumerator());
                                    This.Game.AudioManager.PlaySoundEffect("Effects/Sword_Attack");
                                    break;
                                }
                            }
                        }
                        break;
                }
                #endregion
            }
        }
    }
}
