using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

using Frostbyte.Obstacles;

namespace Frostbyte.Characters
{
    class Mage : Player
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
           "player1-swordcast-up.anim"
//           "player1-death.anim",
        };
        static List<String> Animations2 = new List<String>(){
           "player2-idle-down.anim",
           "player2-idle-diagdown.anim",
           "player2-idle-right.anim",
           "player2-idle-diagup.anim",
           "player2-idle-up.anim",
           "player2-walk-down.anim",
           "player2-walk-diagdown.anim",
           "player2-walk-right.anim",
           "player2-walk-diagup.anim",
           "player2-walk-up.anim",
           "player2-attack-down.anim",
           "player2-attack-diagdown.anim",
           "player2-attack-right.anim",
           "player2-attack-diagup.anim",
           "player2-attack-up.anim",
           "player2-spellcast-down.anim",
           "player2-spellcast-diagdown.anim",
           "player2-spellcast-right.anim",
           "player2-spellcast-diagup.anim",
           "player2-spellcast-up.anim",
           "player2-swordcast-down.anim",
           "player2-swordcast-diagdown.anim",
           "player2-swordcast-right.anim",
           "player2-swordcast-diagup.anim",
           "player2-swordcast-up.anim",
           //"player2-death.anim",
        };
        #endregion

        #region Cheats
        internal class SpeedUpCheat : Cheats.ICheat
        {
            internal SpeedUpCheat(OurSprite m, float newSpeed)
            {
                master = m;
                baseSpeed = m.Speed;
                this.newSpeed = newSpeed;
            }

            private OurSprite master;
            private float baseSpeed;
            private float newSpeed;

            public bool Enabled
            {
                get
                {
                    return master.Speed == newSpeed;
                }
            }

            public void Enable()
            {
                master.Speed = newSpeed;
            }

            public void Disable()
            {
                master.Speed = baseSpeed;
            }

            public void Toggle()
            {
                if (Enabled)
                {
                    Disable();
                }
                else
                {
                    Enable();
                }
            }
        }
        #endregion

        #region Constructors
        public Mage(string name, Actor actor, Color targetColor)
            : this(name, PlayerIndex.One, targetColor, Color.White)
        {
        }

        internal Mage(string name, PlayerIndex input, Color targetColor, Color tint)
            : base(name, new Actor(name.Contains("1")?Animations:Animations2))
        {

            if (GamePad.GetState(input).IsConnected)
            {
                controller = new GamePadController(input);
            }
            else
            {
                controller = new KeyboardController();
            }
            currentTargetAlignment = TargetAlignment.None;
            This.Game.LoadingLevel.AddAnimation(new Animation("target.anim"));
            target = new Sprite("target", new Actor(new Animation("target.anim")));
            target.Visible = false;
            target.Static = true;
            target.mColor = targetColor;
            mColor = tint;
            target.Scale = 1.5f;
            sortType = new DistanceSort(this);

            UpdateBehavior = mUpdate;
            CollidesWithBackground = true;

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


            This.Cheats.AddCheat("SpeedUp_" + Name, new SpeedUpCheat(this, Speed * 2));

            CollisionList = 3;

            Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
            Texture2D bloodParticle = This.Game.CurrentLevel.GetTexture("blood");
            Texture2D sparkball = This.Game.CurrentLevel.GetTexture("sparkball");
            blood = new ParticleEmitter(200, particleEffect, bloodParticle);
            spellCast = new ParticleEmitter(1000, particleEffect, sparkball);

            isDieEffectEnabled = true;
        }
        #endregion

        #region Variables
        internal IController controller;
        internal int attackTier;
        private Sprite target;
        BasicEffect basicEffect = new BasicEffect(This.Game.GraphicsDevice);
        private IComparer<Sprite> sortType;
        internal List<Element> attackCounter = new List<Element>();
        /// <summary>
        /// Which spells the player can cast
        /// </summary>
        internal static Spells UnlockedSpells = 0;
        private int previousHealth = 100;

        private ParticleEmitter blood;
        private ParticleEmitter spellCast;

        #endregion

        #region Methods
        /// <summary>
        /// dies
        /// </summary>
        protected override void Die()
        {
            BossDeath b = new BossDeath("die", new Actor(mActor.Animations[25]));
            b.GroundPos = GroundPos;
            base.Die();
        }

        /// <summary>
        /// Finds the closest enemy sprite to the player that's further than the current target
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private Sprite findMinimum(List<Sprite> list)
        {
            if (list.Contains(this))
            {
                list.Remove(this);
            }
            list.Sort(sortType);

            int next = list.IndexOf(currentTarget);
            for (int x = 0; x < list.Count; x++)
            {
                Sprite target = list[(next + 1 + x) % list.Count];
                if (target.Visible)
                {
                    return target;
                }
            }

            cancelTarget(); // Every sprite in list is invisible, there's nothing to target
            return null;
        }

        private void cancelTarget()
        {
            target.Visible = false;
            currentTarget = null;
            currentTargetAlignment = TargetAlignment.None;
        }

        /// <summary>
        /// Chooses and executes the attack.
        /// Ensures that only one attack is performed per update (eg. no sword *and* magic)
        /// </summary>
        private void attack()
        {
            Level l = This.Game.CurrentLevel;
            if (isAttackAnimDone)
            {
                //create spell cast particles
                if (controller.LaunchAttack == ReleasableButtonState.Pressed && UnlockedSpells != Spells.None)
                {
                    double directionAngle = This.Game.rand.NextDouble() * Math.PI;
                    Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                    spellCast.createParticles(randDirection * 20,
                                                           randDirection * 10 - new Vector2(0, 50),
                                                           GroundPos + randDirection * This.Game.rand.Next(0, (int)(GroundPosRadius / 1.25f)) + new Vector2(0,8),
                                                           This.Game.rand.Next(6, 9),
                                                           300);
                }

                if (controller.LaunchAttack == ReleasableButtonState.Clicked)
                {
                    #region Release Attacks
                    if (attackCounter.Count != 0)
                    {
                        switch (attackCounter.First())
                        {

                            case Element.Earth:
                                if (attackCounter.Count == 1)
                                {
                                    if (UnlockedSpells.HasFlag(Spells.EarthOne) && Mana >= 10)
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
                                                                  true,
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
                                        #endregion Earth Tier 1
                                        This.Game.AudioManager.PlaySoundEffect("Effects/Earth_T1", .1f);
                                        Mana -= 10;
                                    }
                                }

                                else if (attackCounter.Count == 2)
                                {
                                    if (UnlockedSpells.HasFlag(Spells.EarthTwo) && Mana >= 20)
                                    {
                                        #region Earth Tier 2
                                        mAttacks.Add(Attacks.Earthquake(this, this, 10, 10).GetEnumerator());
                                        #endregion Earth Tier 2
                                        Mana -= 20;
                                    }
                                }

                                else
                                {
                                    if (UnlockedSpells.HasFlag(Spells.EarthThree) && Mana >= 25 && currentTarget != null && !(currentTarget is Player))
                                    {
                                        #region Earth Tier 3
                                        mAttacks.Add(Attacks.RockShower(currentTarget, this, 10, 10).GetEnumerator());
                                        #endregion Earth Tier 3
                                        This.Game.AudioManager.PlaySoundEffect("Effects/RockShower");
                                        Mana -= 25;
                                    }
                                }
                                break;

                            case Element.Lightning:
                                if (attackCounter.Count == 1)
                                {
                                    if (UnlockedSpells.HasFlag(Spells.LightningOne) && Mana >= 10)
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
                                                                  true,
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
                                        #endregion Lightning Tier 1

                                        This.Game.AudioManager.PlaySoundEffect("Effects/Lightning_T1", .1f);
                                        Mana -= 10;
                                    }
                                }

                                else if (UnlockedSpells.HasFlag(Spells.LightningTwo) && attackCounter.Count == 2)
                                {
                                    if (Mana >= 50)
                                    {
                                        #region Lightning Tier 2
                                        mAttacks.Add(Attacks.LightningStrike(this, this, 4, 10).GetEnumerator());
                                        #endregion Lightning Tier 2
                                        Mana -= 50;
                                    }
                                }

                                else
                                {
                                    if (UnlockedSpells.HasFlag(Spells.LightningThree) && Mana >= 50)
                                    {
                                        #region Lightning Tier 3
                                        mAttacks.Add(Attacks.LightningStrike(currentTarget, this, 4, 10).GetEnumerator());
                                        #endregion Lightning Tier 3
                                        Mana -= 50;
                                    }
                                }
                                break;

                            case Element.Water:
                                if (attackCounter.Count == 1)
                                {
                                    if (UnlockedSpells.HasFlag(Spells.WaterOne) && Mana >= 10)
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
                                                                  true,
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
                                        This.Game.AudioManager.PlaySoundEffect("Effects/Water_T1");
                                        Mana -= 10;
                                    }
                                }

                                else if (UnlockedSpells.HasFlag(Spells.WaterTwo) && attackCounter.Count == 2)
                                {
                                    if (Mana >= 20 && currentTarget != null && !(currentTarget is Player))
                                    {
                                        #region Water Tier 3
                                        mAttacks.Add(Attacks.Freeze(currentTarget, this, 10).GetEnumerator());
                                        #endregion Water Tier 3
                                        This.Game.AudioManager.PlaySoundEffect("Effects/Water_T3");
                                        Mana -= 20;
                                    }
                                }

                                else
                                {
                                    if (UnlockedSpells.HasFlag(Spells.WaterThree) && Mana >= 80)
                                    {
                                        #region Water Tier 2
                                        mAttacks.Add(Attacks.WaterPush(this, 10).GetEnumerator());
                                        #endregion Water Tier 2
                                        This.Game.AudioManager.PlaySoundEffect("Effects/Water_T2", .1f);
                                        Mana -= 80;
                                    }
                                }
                                break;

                            case Element.Fire:
                                if (attackCounter.Count == 1)
                                {
                                    if (UnlockedSpells.HasFlag(Spells.FireOne) && Mana >= 10)
                                    {
                                        #region Fire Tier 1

                                        int attackRange = 11;

                                        //Create Fire Tier 1 Particle Emmiter
                                        Effect particleEffect = l.GetEffect("ParticleSystem");
                                        Texture2D fire = l.GetTexture("fireParticle");
                                        ParticleEmitter particleFireTier1 = new ParticleEmitter(3000, particleEffect, fire);
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
                                                                  true,
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
                                        #endregion Fire Tier 1
                                        This.Game.AudioManager.PlaySoundEffect("Effects/Fire_T1", .2f);
                                        Mana -= 10;
                                    }
                                }

                                else if (attackCounter.Count == 2)
                                {
                                    if (UnlockedSpells.HasFlag(Spells.FireTwo) && Mana >= 50)
                                    {
                                        #region Fire Tier 2
                                        mAttacks.Add(Attacks.FireRing(this, this, 2, 10).GetEnumerator());
                                        #endregion Fire Tier 2
                                        This.Game.AudioManager.PlaySoundEffect("Effects/Fire_T2", .05f);
                                        Mana -= 50;
                                    }
                                }

                                else
                                {
                                    if (UnlockedSpells.HasFlag(Spells.FireThree) && Mana >= 25 && currentTarget != null && !(currentTarget is Player))
                                    {
                                        #region Fire Tier 3
                                        mAttacks.Add(Attacks.FirePillar(currentTarget, this, 100, 10).GetEnumerator());
                                        #endregion Fire Tier 3
                                        This.Game.AudioManager.PlaySoundEffect("Effects/Fire_T3");
                                        Mana -= 25;
                                    }
                                }
                                break;
                        }

                        attackCounter.Clear();
                    }
                    #endregion
                }

                else if (controller.Earth == ReleasableButtonState.Pressed &&
                        ((controller is GamePadController) && ((controller as GamePadController).mLastControllerState.Buttons.A == ButtonState.Released) ||
                        (controller is KeyboardController) && (controller as KeyboardController).mLastControllerState.IsKeyUp(Keys.S)))
                {
                    if ((attackCounter.Count == 0 && attackCounter.Count < 3) || attackCounter.First() == Element.Earth)
                    {
                        attackCounter.Add(Element.Earth);
                    }
                    return;
                }

                else if (controller.Fire == ReleasableButtonState.Pressed &&
                        ((controller is GamePadController) && ((controller as GamePadController).mLastControllerState.Buttons.B == ButtonState.Released) ||
                        (controller is KeyboardController) && (controller as KeyboardController).mLastControllerState.IsKeyUp(Keys.D)))
                {
                    if ((attackCounter.Count == 0 && attackCounter.Count < 3) || attackCounter.First() == Element.Fire)
                    {
                        attackCounter.Add(Element.Fire);
                    }
                    return;
                }

                else if (controller.Lightning == ReleasableButtonState.Pressed &&
                        ((controller is GamePadController) && ((controller as GamePadController).mLastControllerState.Buttons.Y == ButtonState.Released) ||
                        (controller is KeyboardController) && (controller as KeyboardController).mLastControllerState.IsKeyUp(Keys.W)))
                {
                    if ((attackCounter.Count == 0 && attackCounter.Count < 3) || attackCounter.First() == Element.Lightning)
                    {
                        attackCounter.Add(Element.Lightning);
                    }
                    return;
                }

                else if (controller.Water == ReleasableButtonState.Pressed &&
                        ((controller is GamePadController) && ((controller as GamePadController).mLastControllerState.Buttons.X == ButtonState.Released) ||
                        (controller is KeyboardController) && (controller as KeyboardController).mLastControllerState.IsKeyUp(Keys.A)))
                {
                    if ((attackCounter.Count == 0 && attackCounter.Count < 3) || attackCounter.First() == Element.Water)
                    {
                        attackCounter.Add(Element.Water);
                    }
                }
                if (controller.Sword > 0)
                {
                    #region Start Melee Attack
                    mAttacks.Add(Attacks.Melee(this, 25, 11).GetEnumerator());
                    This.Game.AudioManager.PlaySoundEffect("Effects/Sword_Attack", .1f);
                    #endregion Start Melee Attack
                    return;
                }
            }
        }

        private void interact()
        {
            List<Sprite> obstacles = (This.Game.CurrentLevel as FrostbyteLevel).obstacles;
            float distance = GroundPosRadius + 50;
            if (obstacles != null)
            {
                List<Sprite> targets = GetTargetsInRange(obstacles, distance);
                foreach (Sprite target in targets)
                {
                    if (target is Obstacles.Door)
                    {
                        for (int x = 0; x < ItemBag.Count; x++)
                        {
                            if (ItemBag[x] is Key)
                            {
                                This.Game.CurrentLevel.RemoveSprite(ItemBag[x]);
                                ItemBag.RemoveAt(x);
                                (target as Obstacles.Door).Open();
                                return;
                            }
                        }
                    }
                    else if (target is Obstacles.Chest)
                    {
                        Item i = (target as Obstacles.Chest).Open();
                        PickUpItem(i);
                        return;
                    }
                    else if (target is Obstacles.DiaryEntry)
                    {
                        This.Game.CurrentLevel.RemoveSprite(target);
                        obstacles.Remove(target);
                        ReadDiaryEntry();
                        return;
                    }
                }
            }
        }

        public void mUpdate()
        {
            controller.Update();

            if (controller.GetKeypress(Keys.LeftControl) == ReleasableButtonState.Pressed)
            {
                This.Cheats.GetCheat("SpeedUp_" + Name).Enable();
            }
            else
            {
                This.Cheats.GetCheat("SpeedUp_" + Name).Disable();
            }

            if (currentTarget != null && !currentTarget.Visible)
            {
                cancelTarget();
            }

            if (State == SpriteState.Dead)
            {
                return;
            }

            if (Health == 0)
            {
                State = SpriteState.Dead;
                Pos = Vector2.Zero;
                cancelTarget();
                if (isDieEffectEnabled) //must be before pos is set to zero
                {
                    #region Default DeathEffect Particle Emitter
                    Effect particleEffectDeath = This.Game.CurrentLevel.GetEffect("ParticleSystem");
                    Texture2D lightning = This.Game.CurrentLevel.GetTexture("light blood");
                    ParticleEmitter particleEmitterDeath = new ParticleEmitter(2500, particleEffectDeath, lightning);
                    particleEmitterDeath.effectTechnique = "NoSpecialEffect";
                    particleEmitterDeath.fadeStartPercent = .1f;
                    particleEmitterDeath.blendState = BlendState.AlphaBlend;
                    #endregion Default DeathEffect Particle Emitter

                    new DeathEffect(this, particleEmitterDeath, sampleWidthPercent, sampleHeightPercent);
                }
                return;
            }
            else
            {
                State = SpriteState.Idle;
            }

            if (controller.IsConnected)
            {
                if (controller.CancelSpell == ReleasableButtonState.Clicked)
                    attackCounter.Clear();

                //necessary for collision
                if (this.CollidesWithBackground)
                    previousFootPos = this.GroundPos;

                #region Targeting
                FrostbyteLevel l = This.Game.CurrentLevel as FrostbyteLevel;
                if (controller.TargetEnemies)
                {
                    if (currentTargetAlignment == TargetAlignment.Ally)
                    {
                        cancelTarget();
                    }
                    else
                    {

                        currentTarget = findMinimum(GetTargetsInRange(
                            l.enemies,
                            This.Game.GraphicsDevice.Viewport.Width / 2));

                        if (currentTarget != null)
                        {
                            currentTargetAlignment = TargetAlignment.Enemy;
                        }
                    }
                }
                else if (controller.TargetAllies)
                {
                    if (currentTargetAlignment == TargetAlignment.Enemy)
                    {
                        cancelTarget();

                    }
                    else if (This.Cheats.GetCheat("ElementalBuffs").Enabled)
                    {
                        currentTarget = findMinimum(GetTargetsInRange(
                            l.allies.Concat(
                            l.obstacles).ToList(),
                            This.Game.GraphicsDevice.Viewport.Width));
                        if (currentTarget != null)
                        {
                            currentTargetAlignment = TargetAlignment.Ally;
                        }
                    }
                }

                if (controller.CancelTargeting == ReleasableButtonState.Clicked)
                {
                    cancelTarget();

                }

                if (currentTarget != null && currentTarget.Visible)
                {
                    target.Visible = true;
                    target.Pos = (target.CenteredOn(currentTarget) - l.Camera.Pos) * l.Camera.Zoom;
                }

                if (!(l.enemies.Contains(currentTarget) ||
                    l.allies.Contains(currentTarget) ||
                    (l.obstacles.Contains(currentTarget) &&
                        currentTarget.GetType().IsAssignableFrom(typeof(TargetableObstacle)))))
                {
                    cancelTarget();
                }
                #endregion Targeting

                #region Movement
                if (isAttackAnimDone)
                {
                    PreviousPos = Pos;

                    Pos.X += Math.Sign(controller.Movement.X) * Math.Min(Math.Abs(controller.Movement.X), .8f) * 1.25f * 3 * Speed;
                    Pos.Y -= Math.Sign(controller.Movement.Y) * Math.Min(Math.Abs(controller.Movement.Y), .8f) * 1.25f * 3 * Speed;

                    Vector2 newDirection = controller.Movement;
                    newDirection.Y *= -1;
                    Direction = newDirection;
                }
                #endregion Movement

                //perform collision detection with background
                if (this.CollidesWithBackground && !This.Cheats.GetCheat("DisableBackgroundCollision").Enabled)
                {
                    checkBackgroundCollisions();
                }

                if (isAttackAnimDone)
                {
                    if (PreviousPos == Pos)
                        State = SpriteState.Idle;
                    else
                        State = SpriteState.Moving;
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

                attack();

                if (controller.Interact == ReleasableButtonState.Clicked)
                {
                    interact();
                }

                if (controller.Start == ReleasableButtonState.Clicked && l.isPauseEnabled)
                {
                    l.Paused = true;
                    l.PauseSprite.Visible = true;
                    This.Game.AudioManager.BackgroundMusicVolume = This.Game.AudioManager.BackgroundMusicVolume / 2;
                }
            }

            if(previousHealth > Health)
            {
                for (int i = 0; i < 40; i++)
                {
                    double directionAngle = This.Game.rand.NextDouble() * Math.PI;
                    Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle) / 2.3f, (float)Math.Sin(directionAngle));
                    blood.createParticles(randDirection * 20,
                                          randDirection * 10 - new Vector2(0, 50),
                                          GroundPos + randDirection * This.Game.rand.Next(5, 46) + new Vector2(0, -55),
                                          This.Game.rand.Next(6, 9),
                                          200);
                }
            }
            previousHealth = Health;
        }
        #endregion
    }
}
