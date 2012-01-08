using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte.Enemies
{
    internal partial class CrystalMan : Frostbyte.Boss
    {
        internal List<Crystal> Crystals;
        internal List<Crystal> OuterCrystals;
        internal Enemies.Crystal currentCrystal;

        float radius = 64 * 7;
        static int numOuterCrystals { get { return 5; } }
        static int crystalHealth { get { return 200; } }
        internal TimeSpan attackWait = TimeSpan.MaxValue;

        public CrystalMan(string name, Vector2 initialPosition)
            : base(name, new Actor(new DummyAnimation()), 20, crystalHealth * (numOuterCrystals + 1))
        {
            (This.Game.LoadingLevel as FrostbyteLevel).enemies.Remove(this);
            SpawnPoint = initialPosition;
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Lightning;

            Personality = new ShiningPersonality(this);
            CollidesWithBackground = false;
        }

        protected override void Die()
        {
            FrostbyteLevel l = (This.Game.CurrentLevel as FrostbyteLevel);
            Frostbyte.Characters.Mage.UnlockedSpells = Spells.EarthOne | Spells.EarthTwo | Spells.LightningOne | Spells.LightningTwo;
            l.HUD.ScrollText("You feel all the hairs on your body stand on end.\n\nHold down the Left trigger and press Y and then release the trigger to cast a Lightning spell!");
            base.Die();
        }

        protected void updateHealth(object cryst, int health)
        {
            // Update health to sum of each of its crystals' health
            Health = Crystals.Sum(x => x.Health);
        }

        private void init()
        {
            Crystals = new List<Crystal>();
            OuterCrystals = new List<Crystal>();

            Crystal inner = new Crystal("crystal_center", GroundPos, crystalHealth, this);
            SpriteFrame animation = GetAnimation();
            animation.Height = inner.GetAnimation().Height;
            animation.Width = inner.GetAnimation().Width;
            Crystals.Add(inner);
            inner.HealthChanged += new HealthChangedHandler(updateHealth);

            for (int x = 0; x < numOuterCrystals; x++)
            {
                double angle = 2 * Math.PI * x / numOuterCrystals - Math.PI / 2;
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                Crystal outer = new Crystal("crystal_" + x, GroundPos + offset, crystalHealth, this);
                Crystals.Add(outer);
                OuterCrystals.Add(outer);
                outer.HealthChanged += new HealthChangedHandler(updateHealth);
                outer.Direction = -offset;
            }
        }

        protected override void updateMovement()
        {
            if (Crystals == null)
            {
                init();
            }
            Personality.Update();
        }

        protected override void updateAttack()
        {
            if (isAttackAnimDone && attackWait < This.gameTime.TotalGameTime)
            {
                attackWait = This.gameTime.TotalGameTime + new TimeSpan(0, 0, This.Game.rand.Next(2, 5));
                int AttackChoice = This.Game.rand.Next(7);
                if (AttackChoice <= 0 && OuterCrystals.Count > 0)
                {
                    foreach (Crystal c in OuterCrystals)
                    {
                        mAttacks.Add(Attacks.LightningSpan(c, this, 5, 0).GetEnumerator());
                    }
                }
                else if (AttackChoice <= 1 && OuterCrystals.Count > 0)
                {
                    Crystal target = OuterCrystals.GetRandomElement();
                    if (target != null)
                    {
                        mAttacks.Add(Attacks.LightningSpan(target, this, 7, 0).GetEnumerator());
                    }
                }
                else if (AttackChoice <= 2)
                {
                    AttackRotation rot;
                    if (This.Game.rand.Next(2) == 0)
                    {
                        rot = AttackRotation.Clockwise;
                    }
                    else
                    {
                        rot = AttackRotation.CounterClockwise;
                    }
                    foreach (Crystal c in OuterCrystals)
                    {
                        mAttacks.Add(Attacks.LightningSpan(c, this, 7, 0, rotation: rot).GetEnumerator());
                    }
                }
                else
                {
                    
                    attackStartTime = This.gameTime.TotalGameTime;

                    Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
                    Texture2D lightning = This.Game.CurrentLevel.GetTexture("sparkball");
                    
                    foreach (TimeSpan delay in new TimeSpan[]{
                        TimeSpan.Zero, new TimeSpan(0, 0, 0, 0, 500), new TimeSpan(0, 0, 1),
                    })
                    {
                        Sprite target = (This.Game.CurrentLevel as FrostbyteLevel).allies.Where(x => x.State != SpriteState.Dead).GetRandomElement();
                        ParticleEmitter particleEmitterLightning = new ParticleEmitter(1000, particleEffect, lightning);
                        particleEmitterLightning.effectTechnique = "FadeAtXPercent";
                        particleEmitterLightning.fadeStartPercent = .98f;
                        particleEmitterLightning.blendState = BlendState.Additive;
                        (particleEmitterLightning.collisionObjects.First() as Collision_BoundingCircle).Radius = 10;
                        (particleEmitterLightning.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                        particleEmitters.Add(particleEmitterLightning);

                        mAttacks.Add(LevelFunctions.DelayEnumerable<bool>(Attacks.LightningProjectile(target, currentCrystal, 5, 0, new TimeSpan(0, 0, 0, 5),
                            int.MaxValue, 2.5f, true,
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
                            Vector2.Zero), delay).GetEnumerator());
                    }
                }
            }
        }
    }

    internal class Crystal : Enemy
    {
        #region Variables
        static List<String> Animations = new List<String>(){
           "crystalman-idle-down.anim",
           "crystalman-idle-diagdown.anim",
           "crystalman-idle-right.anim",
           "crystalman-idle-diagup.anim",
           "crystalman-idle-up.anim",  // 4
           "crystalman-teleport-in.anim",
           "crystalman-teleport-out.anim",
           "crystalman-idle-broken-down.anim",
           "crystalman-idle-broken-diagdown.anim",
           "crystalman-idle-broken-right.anim",
           "crystalman-idle-broken-diagup.anim",
           "crystalman-idle-broken-up.anim",  // 11
           "crystalman-teleport-in-broken.anim",
           "crystalman-teleport-out-broken.anim",
           "crystalman-shatter-down.anim",
           "crystalman-shatter-diagdown.anim",
           "crystalman-shatter-right.anim",
           "crystalman-shatter-diagup.anim",
           "crystalman-shatter-up.anim",  // 18
           "crystalman-empty.anim",
           "crystalman-empty-broken.anim",
        };
        #endregion Variables

        CrystalMan master;

        public Crystal(string name, Vector2 initialPosition, int health, CrystalMan master)
            : base(name, new Actor(Animations), 1, health)
        {
            this.master = master;
            SpawnPoint = initialPosition;
            GroundPos = SpawnPoint;
            Scale = 1.25f;

            HealthChanged += new HealthChangedHandler(delegate(object o, int value)
            {
                if (Health == 0)
                {
                    State = SpriteState.Dead;
                }
                else if (Health <= MaxHealth * 0.5)
                {
                    for (int x = 0; x < 7; x++)
                    {
                        mActor.Animations[x] = mActor.Animations[x + 7];
                    }
                    // REPLACE TO BROKEN ANIMATIONS HERE
                    mActor.Animations[19] = mActor.Animations[20];
                }
            });
        }

        protected override void Die()
        {
            int ix = 14;
            switch (Orientation)
            {
                case Orientations.Down:
                    ix = 14;
                    break;
                case Orientations.Down_Left:
                    ix = 15;
                    break;
                case Orientations.Down_Right:
                    ix = 15;
                    break;
                case Orientations.Left:
                    ix = 16;
                    break;
                case Orientations.Right:
                    ix = 16;
                    break;
                case Orientations.Up_Left:
                    ix = 17;
                    break;
                case Orientations.Up_Right:
                    ix = 17;
                    break;
                case Orientations.Up:
                    ix = 18;
                    break;
            }

            BossDeath b = new BossDeath("die", new Actor(mActor.Animations[ix]));
            b.GroundPos = GroundPos;
            b.Scale = 1.25f;
            base.Die();
        }

        protected override void updateAttack()
        {
        }

        protected override void updateMovement()
        {
        }
    }
}
