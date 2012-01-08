using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    internal delegate TriggerEventArgs TriggerCondition();
    internal delegate void TriggerHandler(object sender, TriggerEventArgs args);

    internal class TriggerEventArgs : EventArgs
    {
    }

    internal class TriggerSingleTargetEventArgs : TriggerEventArgs
    {
        internal TriggerSingleTargetEventArgs(Sprite target)
        {
            Target = target;
        }
        internal Sprite Target;
    }

    internal class TriggerMultipleTargetEventArgs : TriggerEventArgs
    {
        internal TriggerMultipleTargetEventArgs(List<Sprite> targets)
        {
            Targets = targets;
        }
        internal List<Sprite> Targets;
    }

    internal class Trigger : OurSprite
    {
        internal Trigger(string name, int width, int height)
            : base(name, new Actor(new DummyAnimation(name, width, height)))
        {
            Center = new Vector2(width / 2, height / 2);
            UpdateBehavior += Update;
        }

        internal Behavior TriggerUpdate = () => { };
        internal TriggerCondition TriggerCondition = () => { return null; };
        internal event TriggerHandler TriggerEffect = (Object, TriggerSingleTargetEventArgs) => { };
        internal bool Enabled = true;


        private new void Update()
        {
            if (Enabled)
            {
                TriggerUpdate();
                TriggerEventArgs args = TriggerCondition();
                if (args != null)
                {
                    TriggerEffect(this, args);
                }
            }
        }
    }

    internal class PartyCrossTrigger : Trigger
    {
        internal PartyCrossTrigger(string name, int width, int height, List<Sprite> party)
            : base(name, width, height)
        {
            this.party = party;
            base.TriggerUpdate += TriggerUpdate;
            base.TriggerCondition += TriggerCondition;
            base.TriggerEffect += TriggerEffect;

            triggerRect = new Rectangle(
                0,
                0,
                (int)(GetAnimation().Width / (1 - triggerRectScale)),
                (int)(GetAnimation().Height * triggerRectScale));
        }

        /// <summary>
        /// Constructor for the Level Editor
        /// </summary>
        /// <param name="name">Sprite name of Trigger</param>
        /// <param name="initialPos">Position of Trigger</param>
        /// <param name="orientation">Trigger's orientation/direction</param>
        public PartyCrossTrigger(string name, Vector2 initialPosition, Orientations orientation = Orientations.Up)
            : this(name, Tile.TileSize, Tile.TileSize, (This.Game.LoadingLevel as FrostbyteLevel).allies)
        {
            Orientation = orientation;
            SpawnPoint = initialPosition;
        }

        private List<Sprite> party;
        private Dictionary<Sprite, bool> triggered;
        private float triggerRectScale = 0.6f;
        private Rectangle triggerRect;

        #region Methods
        private float cross(Vector3 v, Vector3 w)
        {
            return v.X * w.Y - v.Y * w.X;
        }

        private new void TriggerUpdate()
        {
            if (triggered == null)
            {
                triggered = new Dictionary<Sprite, bool>();
                foreach (Sprite s in party)
                {
                    triggered.Add(s, false);
                }
            }

            triggerRect.X = (int)(Pos.X - GetAnimation().Width * (1 - triggerRectScale));
            triggerRect.Y = (int)(Pos.Y + GetAnimation().Height * (1 - triggerRectScale));
            foreach (Sprite target in this.GetTargetsInRectangle(party, triggerRect))
            {
                // http://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect/565282#565282
                Vector3 normalDir = Vector3.Cross(new Vector3(this.Direction, 0), new Vector3(0, 0, -1));
                Vector3 targetDir = new Vector3(target.Pos - target.PreviousPos, 0);
                targetDir.Normalize();
                Vector3 pmq = new Vector3(target.PreviousPos + target.Center, 0) -
                    new Vector3(this.Pos + new Vector2(this.Center.X, 0), 0);  // The top of the trigger

                float ta = cross(pmq, targetDir);
                float tb = cross(normalDir, targetDir);
                float t = ta / tb;
                float ua = cross(pmq, normalDir);
                float ub = cross(normalDir, targetDir);
                float u = (ua / ub);


                if (u > 0)
                {
                    float cos = this.Direction.X * targetDir.X + this.Direction.Y * targetDir.Y;

                    // Has crossed
                    if (cos > 0)
                    {
                        if (triggered.ContainsKey(target))
                        {
                            triggered[target] = true;
                        }
                    }
                    // Has uncrossed
                    else
                    {
                        if (triggered.ContainsKey(target))
                        {
                            triggered[target] = false;
                        }
                    }
                }
            }
        }

        private new TriggerMultipleTargetEventArgs TriggerCondition()
        {
            if (triggered.Count > 0 && triggered.Values.All(on => on))
            {
                return new TriggerMultipleTargetEventArgs(triggered.Keys.ToList());
            }
            return null;
        }

        private new void TriggerEffect(object ths, TriggerEventArgs args)
        {
            Obstacles.Obstacle rock = new Obstacles.Rock("rock");
            rock.SpawnOn(this);
            rock.SpawnPoint += new Vector2(0, Tile.TileSize * Math.Sign(-Direction.Y) + 32);
            rock.Respawn();

            this.Enabled = false;
        }
        #endregion
    }

    internal class SimpleDistanceTrigger : Trigger
    {
        internal SimpleDistanceTrigger(string name, int width, int height)
            : base(name, width, height)
        {
            base.TriggerUpdate += TriggerUpdate;
            collisionObjects.Add(new Collision_AABB(1, Vector2.Zero, new Vector2(width, height)));
        }

        internal SimpleDistanceTrigger(string name, int radius)
            : base(name, radius * 2, radius * 2)
        {
            base.TriggerUpdate += TriggerUpdate;
            collisionObjects.Add(new Collision_BoundingCircle(1, Vector2.Zero, radius));
            this.CollisionList = 2;
        }

        internal List<Sprite> SpritesInRange = new List<Sprite>();
        internal List<CollisionObject> collisionObjects = new List<CollisionObject>();

        private new void TriggerUpdate()
        {
            SpritesInRange.Clear();
            List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
            Collision.CollisionData.TryGetValue(this, out collidedWith);
            if (collidedWith != null)
            {
                foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                {
                    if (detectedCollision.Item2 is Player)
                    {
                        SpritesInRange.Add(detectedCollision.Item2 as Player);
                    }
                }
            }
        }

        internal override List<CollisionObject> GetCollision()
        {
            return collisionObjects;
        }
    }

    internal class RestorePlayerHealthTrigger : SimpleDistanceTrigger
    {
        internal RestorePlayerHealthTrigger(string name, int radius)
            : base(name, radius)
        {
            base.TriggerUpdate = TriggerUpdate;
            base.TriggerCondition = TriggerCondition;
            base.TriggerEffect += TriggerEffect;

            SpritesInRange = new List<Sprite>();

            #region Particles
            Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
            Texture2D regen = This.Game.CurrentLevel.GetTexture("regen");
            Texture2D evil = This.Game.CurrentLevel.GetTexture("evil");
            particleEmitterTrigger = new ParticleEmitter(1000, particleEffect, regen, evil);
            particleEmitterTrigger.effectTechnique = "ChangePicAndFadeAtPercent";
            particleEmitterTrigger.fadeStartPercent = 0.9f;
            (particleEmitterTrigger.GetCollision()[0] as Collision_BoundingCircle).Radius = GetAnimation().Height / 2;
            (particleEmitterTrigger.GetCollision()[0] as Collision_BoundingCircle).createDrawPoints();
            particleEmitterTrigger.blendState = BlendState.Additive;
            particleEmitters.Add(particleEmitterTrigger);
            #endregion
            Random rand = new Random();
            mAttacks.Add(Attacks.T1Projectile(null, this, 0, 0,
                TimeSpan.MaxValue,
                0, 0, false,
                delegate(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter)
                {
                    int numLayers = 5;
                    int maxRadius = attacker.GetAnimation().Height / 2;
                    for (int i = 0; i < numLayers; i++)
                    {
                        double spawnRadius = (i + 1) * maxRadius / numLayers;
                        double theta = rand.NextDouble() * 2 * Math.PI - Math.PI;
                        Vector2 origin = new Vector2((float)(spawnRadius * Math.Cos(theta)), (float)(spawnRadius * Math.Sin(theta) / ParticleEmitter.EllipsePerspectiveModifier));
                        Vector2 velocity = new Vector2(-origin.Y, origin.X);

                        velocity.Normalize();
                        particleEmitter.createParticles(new Vector2(0, -10 * (i + 1)),
                                        (velocity * velocity) / new Vector2((float)spawnRadius, (float)spawnRadius) * 100,
                                        attacker.GroundPos + origin + new Vector2(0, maxRadius / ParticleEmitter.EllipsePerspectiveModifier),
                                        maxRadius / numLayers - 1,
                                        1000);
                    }
                },
                particleEmitterTrigger,
                Vector2.Zero).GetEnumerator());
        }

        /// <summary>
        /// Constructor for the Level Editor
        /// </summary>
        /// <param name="name">Sprite name of Trigger</param>
        /// <param name="initialPos">Position of Trigger</param>
        /// <param name="orientation">Trigger's orientation/direction</param>
        public RestorePlayerHealthTrigger(string name, Vector2 initialPosition)
            : this(name, Tile.TileSize / 2)
        {
            SpawnPoint = initialPosition;
        }

        private ParticleEmitter particleEmitterTrigger;

        private new TriggerMultipleTargetEventArgs TriggerCondition()
        {
            if (SpritesInRange.Count > 0)
            {
                particleEmitterTrigger.changePicPercent = 0;
                return new TriggerMultipleTargetEventArgs(SpritesInRange);
            }
            else
            {
                particleEmitterTrigger.changePicPercent = 1;
            }
            return null;
        }

        private new void TriggerEffect(object ths, TriggerEventArgs args)
        {
            //This.Game.AudioManager.PlaySoundEffect("regen");
            foreach (Player p in SpritesInRange)
            {
                p.Regen();
            }
        }
    }

    internal class SetRespawnTrigger : SimpleDistanceTrigger
    {
        internal SetRespawnTrigger(string name, int radius, List<Sprite> party)
            : base(name, radius)
        {
            this.party = party;
            base.TriggerCondition += TriggerCondition;
            base.TriggerEffect += TriggerEffect;
        }

        /// <summary>
        /// Constructor for the Level Editor
        /// </summary>
        /// <param name="name">Sprite name of Trigger</param>
        /// <param name="initialPos">Position of Trigger</param>
        /// <param name="orientation">Trigger's orientation/direction</param>
        public SetRespawnTrigger(string name, Vector2 initialPosition)
            : this(name, Tile.TileSize / 2, (This.Game.LoadingLevel as FrostbyteLevel).allies)
        {
            SpawnPoint = initialPosition;
        }

        private List<Sprite> party;

        private new TriggerMultipleTargetEventArgs TriggerCondition()
        {
            if (SpritesInRange.Count != 0)
            {
                return new TriggerMultipleTargetEventArgs(party);
            }
            return null;
        }

        private new void TriggerEffect(object ths, TriggerEventArgs args)
        {
            foreach (Player p in party)
            {
                p.SpawnPoint = p.CenteredOn(this);
            }
            this.Enabled = false;
        }
    }

    internal class AcquireSpellTrigger : SimpleDistanceTrigger
    {
        internal AcquireSpellTrigger(string name, int radius, List<Sprite> party)
            : base(name, radius)
        {
            this.party = party;
            base.TriggerCondition += TriggerCondition;
            base.TriggerEffect += TriggerEffect;
        }

        /// <summary>
        /// Constructor for the Level Editor
        /// </summary>
        /// <param name="name">Sprite name of Trigger</param>
        /// <param name="initialPos">Position of Trigger</param>
        /// <param name="orientation">Trigger's orientation/direction</param>
        public AcquireSpellTrigger(string name, Vector2 initialPosition)
            : this(name, Tile.TileSize / 2, (This.Game.LoadingLevel as FrostbyteLevel).allies)
        {
            SpawnPoint = initialPosition;
        }

        private List<Sprite> party;

        private new TriggerMultipleTargetEventArgs TriggerCondition()
        {
            if (SpritesInRange.Count != 0)
            {
                return new TriggerMultipleTargetEventArgs(party);
            }
            return null;
        }

        private new void TriggerEffect(object ths, TriggerEventArgs args)
        {
            FrostbyteLevel l = This.Game.CurrentLevel as FrostbyteLevel;
            if (l.Name == "Lightning")
            {
                Characters.Mage.UnlockedSpells = Characters.Mage.UnlockedSpells | Spells.EarthTwo;
                l.HUD.ScrollText("You feel a rush of power enter your body.\n\n\n\n\n\nYou can now cast level two spells by pressing the corresponding element two times.\n\n\n\n\n\n Try it on those spiders up ahead!");
            }
            if (l.Name == "Fire")
            {
                Characters.Mage.UnlockedSpells = Characters.Mage.UnlockedSpells | Spells.EarthThree | Spells.LightningThree | Spells.WaterThree;
                l.HUD.ScrollText("You feel a huge rush of power enter your body.\n\n\n\n\n\nYou can now cast level two spells by pressing the corresponding element three times. Remember to target your enemy first!");
            }
            this.Enabled = false;
        }
    }

    internal class ConcentricCircles : OurSprite
    {
        internal ConcentricCircles(string name, int radius)
            : base(name, new Actor(new DummyAnimation(name, radius * 2, radius * 2)))
        {
            #region Particles
            Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
            Texture2D lightning = This.Game.CurrentLevel.GetTexture("evil");
            ParticleEmitter particleEmitterTrigger = new ParticleEmitter(1000, particleEffect, lightning);
            particleEmitterTrigger.effectTechnique = "FadeAtXPercent";
            particleEmitterTrigger.fadeStartPercent = .98f;
            particleEmitterTrigger.blendState = BlendState.Additive;
            (particleEmitterTrigger.GetCollision()[0] as Collision_BoundingCircle).Radius = GetAnimation().Height / 2;
            (particleEmitterTrigger.GetCollision()[0] as Collision_BoundingCircle).createDrawPoints();
            particleEmitters.Add(particleEmitterTrigger);
            #endregion
            Random rand = new Random();
            mAttacks.Add(Attacks.T1Projectile(null, this, 0, 0,
                TimeSpan.MaxValue,
                0, 0, false,
                delegate(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter)
                {
                    int numLayers = 5;
                    int maxRadius = attacker.GetAnimation().Height / 2;
                    for (int i = 0; i < numLayers; i++)
                    {
                        double spawnRadius = (i + 1) * maxRadius / numLayers;
                        double theta = rand.NextDouble() * 2 * Math.PI - Math.PI;
                        Vector2 origin = new Vector2((float)(spawnRadius * Math.Cos(theta)), (float)(spawnRadius * Math.Sin(theta)));
                        Vector2 velocity = new Vector2(-origin.Y, origin.X);

                        velocity.Normalize();
                        particleEmitter.createParticles(new Vector2(0, -10 * (i + 1)),
                                        (velocity * velocity) / new Vector2((float)spawnRadius, (float)spawnRadius) * 100,
                                        attacker.GroundPos + origin,
                                        maxRadius / numLayers - 1,
                                        1000);
                    }
                },
                particleEmitterTrigger,
                Vector2.Zero).GetEnumerator());

            collisionObjects.Add(new Collision_BoundingCircle(1, Vector2.Zero, radius));
            this.CollisionList = 2;
        }


        internal List<CollisionObject> collisionObjects = new List<CollisionObject>();

        internal override List<CollisionObject> GetCollision()
        {
            return collisionObjects;
        }
    }

    internal class NextLevelPortal : ConcentricCircles
    {
        internal NextLevelPortal(string name, int radius)
            : base(name, radius)
        {
            UpdateBehavior = Update;
        }

        private new void Update()
        {
        }
    }
}
