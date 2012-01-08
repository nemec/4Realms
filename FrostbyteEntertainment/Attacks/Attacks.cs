using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace Frostbyte
{
    internal enum AttackRotation
    {
        None,
        Clockwise,
        CounterClockwise,
    }

    internal static class Attacks
    {
        internal delegate void CreateParticles(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter);

        /// <summary>
        /// Sets correctly oriented animation and returns number of frames in animation
        /// </summary>
        /// <returns>returns number of frames in animation</returns>
        private static void setAnimation(OurSprite attacker)
        {
            switch (attacker.Orientation)
            {
                case Orientations.Down:
                    if (attacker is Frostbyte.Characters.Mage && (attacker as Frostbyte.Characters.Mage).attackTier == 1)
                    {
                        attacker.SetAnimation(15);
                    }

                    else if (attacker is Frostbyte.Characters.Mage && ((attacker as Frostbyte.Characters.Mage).attackTier == 2 ||
                       (attacker as Frostbyte.Characters.Mage).attackTier == 3))
                    {
                        attacker.SetAnimation(20);
                    }

                    else
                    {
                        attacker.SetAnimation(0 + 5 * attacker.State.GetHashCode());
                    }

                    break;
                case Orientations.Down_Right:
                    attacker.Hflip = false;
                    if (attacker is Frostbyte.Characters.Mage && (attacker as Frostbyte.Characters.Mage).attackTier == 1)
                    {
                        attacker.SetAnimation(16);
                    }

                    else if (attacker is Frostbyte.Characters.Mage && ((attacker as Frostbyte.Characters.Mage).attackTier == 2 ||
                       (attacker as Frostbyte.Characters.Mage).attackTier == 3))
                    {
                        attacker.SetAnimation(21);
                    }

                    else
                    {
                        attacker.SetAnimation(1 + 5 * attacker.State.GetHashCode());
                    }

                    break;
                case Orientations.Down_Left:
                    attacker.Hflip = true;
                    if (attacker is Frostbyte.Characters.Mage && (attacker as Frostbyte.Characters.Mage).attackTier == 1)
                    {
                        attacker.SetAnimation(16);
                    }

                    else if (attacker is Frostbyte.Characters.Mage && ((attacker as Frostbyte.Characters.Mage).attackTier == 2 ||
                       (attacker as Frostbyte.Characters.Mage).attackTier == 3))
                    {
                        attacker.SetAnimation(21);
                    }

                    else
                    {
                        attacker.SetAnimation(1 + 5 * attacker.State.GetHashCode());
                    }

                    break;
                case Orientations.Right:
                    attacker.Hflip = false;
                    if (attacker is Frostbyte.Characters.Mage && (attacker as Frostbyte.Characters.Mage).attackTier == 1)
                    {
                        attacker.SetAnimation(17);
                    }

                    else if (attacker is Frostbyte.Characters.Mage && ((attacker as Frostbyte.Characters.Mage).attackTier == 2 ||
                       (attacker as Frostbyte.Characters.Mage).attackTier == 3))
                    {
                        attacker.SetAnimation(22);
                    }

                    else
                    {
                        attacker.SetAnimation(2 + 5 * attacker.State.GetHashCode());
                    }

                    break;
                case Orientations.Left:
                    attacker.Hflip = true;
                    if (attacker is Frostbyte.Characters.Mage && (attacker as Frostbyte.Characters.Mage).attackTier == 1)
                    {
                        attacker.SetAnimation(17);
                    }

                    else if (attacker is Frostbyte.Characters.Mage && ((attacker as Frostbyte.Characters.Mage).attackTier == 2 ||
                       (attacker as Frostbyte.Characters.Mage).attackTier == 3))
                    {
                        attacker.SetAnimation(22);
                    }

                    else
                    {
                        attacker.SetAnimation(2 + 5 * attacker.State.GetHashCode());
                    }

                    break;
                case Orientations.Up_Right:
                    attacker.Hflip = false;
                    if (attacker is Frostbyte.Characters.Mage && (attacker as Frostbyte.Characters.Mage).attackTier == 1)
                    {
                        attacker.SetAnimation(18);
                    }

                    else if (attacker is Frostbyte.Characters.Mage && ((attacker as Frostbyte.Characters.Mage).attackTier == 2 ||
                       (attacker as Frostbyte.Characters.Mage).attackTier == 3))
                    {
                        attacker.SetAnimation(23);
                    }

                    else
                    {
                        attacker.SetAnimation(3 + 5 * attacker.State.GetHashCode());
                    }

                    break;
                case Orientations.Up_Left:
                    attacker.Hflip = true;
                    if (attacker is Frostbyte.Characters.Mage && (attacker as Frostbyte.Characters.Mage).attackTier == 1)
                    {
                        attacker.SetAnimation(18);
                    }

                    else if (attacker is Frostbyte.Characters.Mage && ((attacker as Frostbyte.Characters.Mage).attackTier == 2 ||
                       (attacker as Frostbyte.Characters.Mage).attackTier == 3))
                    {
                        attacker.SetAnimation(23);
                    }

                    else
                    {
                        attacker.SetAnimation(3 + 5 * attacker.State.GetHashCode());
                    }

                    break;
                case Orientations.Up:
                    if (attacker is Frostbyte.Characters.Mage && (attacker as Frostbyte.Characters.Mage).attackTier == 1)
                    {
                        attacker.SetAnimation(19);
                    }

                    else if (attacker is Frostbyte.Characters.Mage && ((attacker as Frostbyte.Characters.Mage).attackTier == 2 ||
                       (attacker as Frostbyte.Characters.Mage).attackTier == 3))
                    {
                        attacker.SetAnimation(24);
                    }

                    else
                    {
                        attacker.SetAnimation(4 + 5 * attacker.State.GetHashCode());
                    }

                    break;
            }
        }

        /// <summary>
        /// Applies damage to target based on attacker
        /// </summary>
        /// <param name="attacker">The attacking object</param>
        /// <param name="target">The target of the attack</param>
        /// <param name="baseDamage">The attack's base damage</param>
        private static void Damage(OurSprite attacker, OurSprite target, int baseDamage = 0, Element elem = Element.Normal)
        {
            int damage = baseDamage;
            //apply status effects for normal type attacs
            if (elem == Element.Normal)
            {
                foreach (StatusEffect e in attacker.StatusEffects)
                {
                    //add effect of elemental buffs
                    if (e is ElementalBuff)
                    {
                        ElementalBuff eb = e as ElementalBuff;
                        ///same type no damage
                        if (target.ElementType == eb.Element)
                            continue;
                        switch (target.ElementType)
                        {
                            case Element.Earth:
                                switch (eb.Element)
                                {
                                    case Element.Fire:
                                        damage += (int)(baseDamage * 1.5);
                                        break;
                                    case Element.Lightning:
                                        damage += (int)(baseDamage * 0.5);
                                        break;
                                    case Element.Water:
                                        damage += (int)(baseDamage);
                                        break;
                                }
                                break;
                            case Element.Fire:
                                switch (eb.Element)
                                {
                                    case Element.Earth:
                                        damage += (int)(baseDamage * 0.5);
                                        break;
                                    case Element.Lightning:
                                        damage += (int)(baseDamage);
                                        break;
                                    case Element.Water:
                                        damage += (int)(baseDamage * 1.5);
                                        break;
                                }
                                break;
                            case Element.Lightning:
                                switch (eb.Element)
                                {
                                    case Element.Fire:
                                        damage += (int)(baseDamage);
                                        break;
                                    case Element.Earth:
                                        damage += (int)(baseDamage * 1.5);
                                        break;
                                    case Element.Water:
                                        damage += (int)(baseDamage * 0.5);
                                        break;
                                }
                                break;
                            case Element.Water:
                                switch (eb.Element)
                                {
                                    case Element.Fire:
                                        damage += (int)(baseDamage * 0.5);
                                        break;
                                    case Element.Lightning:
                                        damage += (int)(baseDamage);
                                        break;
                                    case Element.Earth:
                                        damage += (int)(baseDamage * 1.5);
                                        break;
                                }
                                break;
                            case Element.Normal:
                                damage += baseDamage;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            else // this handles elemental attacks (same as above but with element instead of buff
            {
                switch (target.ElementType)
                {
                    case Element.Earth:
                        switch (elem)
                        {
                            case Element.Fire:
                                damage += (int)(baseDamage * 1.5);
                                break;
                            case Element.Lightning:
                                damage += (int)(baseDamage * 0.5);
                                break;
                            case Element.Water:
                                damage += (int)(baseDamage);
                                break;
                        }
                        break;
                    case Element.Fire:
                        switch (elem)
                        {
                            case Element.Earth:
                                damage += (int)(baseDamage * 0.5);
                                break;
                            case Element.Lightning:
                                damage += (int)(baseDamage);
                                break;
                            case Element.Water:
                                damage += (int)(baseDamage * 1.5);
                                break;
                        }
                        break;
                    case Element.Lightning:
                        switch (elem)
                        {
                            case Element.Fire:
                                damage += (int)(baseDamage);
                                break;
                            case Element.Earth:
                                damage += (int)(baseDamage * 1.5);
                                break;
                            case Element.Water:
                                damage += (int)(baseDamage * 0.5);
                                break;
                        }
                        break;
                    case Element.Water:
                        switch (elem)
                        {
                            case Element.Fire:
                                damage += (int)(baseDamage * 0.5);
                                break;
                            case Element.Lightning:
                                damage += (int)(baseDamage);
                                break;
                            case Element.Earth:
                                damage += (int)(baseDamage * 1.5);
                                break;
                        }
                        break;
                    case Element.Normal:
                        damage += baseDamage;
                        break;
                    default:
                        break;
                }
            }
            target.Health -= damage;
        }

        /// <summary>
        /// Slows target by specified amount for specified amount of time.
        /// </summary>
        /// <param name="target">The enemy to be slowed.</param>
        /// <param name="slowMultiplier"> The amount the enemy should be slowed.</param>
        /// <param name="slowDuration"> The length of time the enemy should be slowed.</param>
        private static void Slow(OurSprite target, float slowMultiplier, TimeSpan slowDuration)
        {
            if (!target.isSlowed)
            {
                target.slowDuration = slowDuration;
                target.isSlowed = true;
                target.originalSpeed = target.Speed;
                target.slowStart = This.gameTime.TotalGameTime;
                target.Speed *= slowMultiplier;
            }

        }

        private static bool tileCircleCollision(Vector2 tileTopLeftPos, Vector2 tileBottomRightPos, Vector2 circlePos, float circleRadius)
        {
            Vector2 centerPoint = circlePos;
            Vector2 topLeftPoint = tileTopLeftPos;
            Vector2 bottomRightPoint = tileBottomRightPos;

            int regionCode = 0;

            if (centerPoint.X < topLeftPoint.X)
                regionCode += 1; // 0001
            if (centerPoint.X > bottomRightPoint.X)
                regionCode += 2; // 0010
            if (centerPoint.Y < topLeftPoint.Y)
                regionCode += 4; // 0100
            if (centerPoint.Y > bottomRightPoint.Y)
                regionCode += 8;

            float radius = circleRadius;
            switch (regionCode)
            {
                case 0: //0000
                    return true;
                case 1: //0001
                    if (Math.Abs(topLeftPoint.X - centerPoint.X) <= radius)
                        return true;
                    break;
                case 2: //0010
                    if (Math.Abs(centerPoint.X - bottomRightPoint.X) <= radius)
                        return true;
                    break;
                case 4: //0100
                    if (Math.Abs(centerPoint.Y - topLeftPoint.Y) <= radius)
                        return true;
                    break;
                case 8: //1000
                    if (Math.Abs(bottomRightPoint.Y - centerPoint.Y) <= radius)
                        return true;
                    break;
                case 5: //0101
                    if (Collision.DistanceSquared(centerPoint, topLeftPoint) <= radius * radius)
                        return true;
                    break;
                case 9: //1001
                    if (Collision.DistanceSquared(centerPoint, new Vector2(topLeftPoint.X, bottomRightPoint.Y)) <= radius * radius)
                        return true;
                    break;
                case 6: //0110
                    if (Collision.DistanceSquared(centerPoint, new Vector2(bottomRightPoint.X, topLeftPoint.Y)) <= radius * radius)
                        return true;
                    break;
                case 10: //1010
                    if (Collision.DistanceSquared(centerPoint, bottomRightPoint) <= radius * radius)
                        return true;
                    break;
            }


            return false;
        }

        /// <summary>
        /// Performs Melee Attack
        /// </summary>
        /// <returns>returns true when finished</returns>
        public static IEnumerable<bool> Melee(OurSprite _attacker, int baseDamage, int attackFrame)
        {
            OurSprite attacker = _attacker;
            bool hasAttacked = false;

            attacker.State = SpriteState.Attacking;
            setAnimation(attacker);
            int FrameCount = attacker.FrameCount();

            attacker.Rewind();

            attacker.isAttackAnimDone = false;

            if (attacker is Frostbyte.Characters.Mage)
            {
                (attacker as Frostbyte.Characters.Mage).attackTier = 0;
            }


            for (int i = 0; i < 150; i++)
            {
                attacker.State = SpriteState.Attacking;
                setAnimation(attacker);
                FrameCount = attacker.FrameCount();

                if (attacker.Frame == FrameCount - 1)
                    break;

                attacker.isAttackAnimDone = false;

                List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                Collision.CollisionData.TryGetValue(attacker, out collidedWith);
                if (collidedWith != null)
                {
                    foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                    {
                        if (((detectedCollision.Item2 is Enemy) && (attacker is Player)) || ((detectedCollision.Item2 is Player) && (attacker is Enemy)))
                        {
                            attacker.Direction = detectedCollision.Item2.GroundPos - attacker.GroundPos;
                            break;
                        }
                    }
                }

                if (!hasAttacked && attacker.Frame == attackFrame && Collision.CollisionData.Count > 0)
                {
                    List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith2;
                    Collision.CollisionData.TryGetValue(attacker, out collidedWith2);
                    if (collidedWith2 != null)
                    {
                        foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith2)
                        {
                            if (!hasAttacked && (((detectedCollision.Item2 is Enemy) && (attacker is Player)) || ((detectedCollision.Item2 is Player) && (attacker is Enemy))))
                            {
                                Vector2 dirToEnemy = detectedCollision.Item2.GroundPos - attacker.GroundPos;
                                dirToEnemy.Normalize();
                                if (attacker.Direction == dirToEnemy || Math.Abs(Math.Acos(Vector2.Dot(attacker.Direction, dirToEnemy))) <= Math.PI / 3)
                                {
                                    Damage(attacker, (detectedCollision.Item2 as OurSprite), baseDamage);
                                    hasAttacked = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                yield return false;
            }

            attacker.isAttackAnimDone = true;

            yield return true;
        }

        /// <summary>
        /// Performs Magic Tier 1 Attack
        /// </summary>
        /// <param name="_target">The target for the projectile to attack</param>
        /// <param name="_attacker">The sprite initiating the attack</param>
        /// <param name="_baseDamage">The amount of damage to inflict before constant multiplier for weakness</param>
        /// <param name="_attackFrame">The frame that the attack begins on</param>
        /// <param name="_attackEndTime">The time at which the magic attack should timeout</param>
        /// <param name="_attackRange">The distance from the target that the projectile must come within to be considered a hit</param>
        /// <param name="_projectileSpeed">The speed of the projectile</param>
        /// <returns>Returns true when finished</returns>
        public static IEnumerable<bool> T1Projectile(Sprite _target, OurSprite attacker, int baseDamage, int attackFrame, TimeSpan attackEndTime, int attackRange, float projectileSpeed, bool isHoming, CreateParticles createParticles, ParticleEmitter _particleEmitter, Vector2 spawnOffset, Element elem = Element.Normal)
        {
            #region Variables
            Level l = This.Game.CurrentLevel;
            OurSprite target = (OurSprite)_target;
            Vector2 initialDirection = attacker.Direction;
            attacker.State = SpriteState.Attacking;
            setAnimation(attacker);
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;
            Vector2 direction = new Vector2();
            Tuple<Vector2, Vector2> closestObject = new Tuple<Vector2, Vector2>(new Vector2(), new Vector2());
            ParticleEmitter particleEmitter = _particleEmitter;

            bool damageDealt = false;
            bool isLoopOne = true;
            #endregion Variables

            particleEmitter.GroundPos = attacker.GroundPos + spawnOffset;

            attacker.Rewind();

            attacker.isAttackAnimDone = false;

            if (attacker is Frostbyte.Characters.Mage)
            {
                (attacker as Frostbyte.Characters.Mage).attackTier = 1;
            }

            #region Shoot Tier 1 at attackFrame
            while (attacker.Frame < attacker.FrameCount())
            {
                attacker.isAttackAnimDone = false;

                if (target != null && target.GroundPos != attacker.GroundPos)
                    attacker.Direction = target.GroundPos - particleEmitter.GroundPos;
                attacker.State = SpriteState.Attacking;
                setAnimation(attacker);

                if (attacker.Frame == attackFrame)
                {
                    if (target == null)
                    {
                        direction = initialDirection;
                    }
                    else
                    {
                        direction = attacker.Direction;
                    }
                    direction.Normalize();
                    attackStartTime = This.gameTime.TotalGameTime;
                    break;
                }

                yield return false;
            }
            #endregion Shoot Tier 1 at attackFrame

            #region Emit Particles until particle hits target or wall or time to live runs out

            bool isAttackAnimDone = false;

            while (attackEndTime > TimeSpan.Zero)
            {
                attackEndTime -= This.gameTime.ElapsedGameTime;

                if (target != null && !((This.Game.CurrentLevel as FrostbyteLevel).enemies.Contains(target) || (This.Game.CurrentLevel as FrostbyteLevel).allies.Contains(target)))
                    target = null;

                if (isHoming && target != null)
                {
                    direction = target.GroundPos - particleEmitter.GroundPos;
                    direction.Normalize();
                }

                if (Collision.CollisionData.Count > 0)
                {
                    List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                    Collision.CollisionData.TryGetValue(particleEmitter, out collidedWith);
                    if (collidedWith != null)
                    {
                        foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                        {
                            if (((detectedCollision.Item2 is Enemy) && (attacker is Player)) || ((detectedCollision.Item2 is Player) && (attacker is Enemy)))
                            {
                                Damage(attacker, (detectedCollision.Item2 as OurSprite), baseDamage);
                                damageDealt = true;
                                break;
                            }
                            else if (This.Cheats.GetCheat("ElementalBuffs").Enabled && (detectedCollision.Item2 is Player) && (attacker is Player) && (attacker as Player).currentTarget == detectedCollision.Item2)
                            {
                                Player p = (detectedCollision.Item2 as Player);
                                p.AddStatusEffect(new ElementalBuff(elem));
                                damageDealt = true;
                                break;
                            }
                        }
                    }
                }

                if (damageDealt)
                {
                    break;
                }

                //if the attack frame has passed then allow the attacker to move
                if (attacker.Frame >= attacker.FrameCount() - 1)
                {
                    attacker.isAttackAnimDone = true;
                    isLoopOne = false;
                    isAttackAnimDone = true;
                }

                if (!isAttackAnimDone)
                    attacker.isAttackAnimDone = false;

                //move particle
                Vector2 previousPosition = particleEmitter.GroundPos;
                particleEmitter.GroundPos += direction * projectileSpeed;

                //make sure magic cannot go through walls
                if (previousPosition != particleEmitter.GroundPos)
                {
                    bool hasCollided = false;
                    float collisionRadius = particleEmitter.GroundPosRadius;
                    Tuple<int, int> topLeftMostTile = new Tuple<int, int>((int)Math.Floor(((particleEmitter.GroundPos.X - collisionRadius) / This.CellSize)),   //top left most tile that could possible hit sprite
                                                                    (int)Math.Floor(((particleEmitter.GroundPos.Y - collisionRadius)) / This.CellSize));
                    Tuple<int, int> bottomRightMostTile = new Tuple<int, int>((int)Math.Floor((particleEmitter.GroundPos.X + collisionRadius) / This.CellSize), //bottom right most tile that could possible hit sprite
                                                                            (int)Math.Floor((particleEmitter.GroundPos.Y + collisionRadius) / This.CellSize));
                    TileList tileMap = (This.Game.CurrentLevel as FrostbyteLevel).TileMap;
                    for (int x = topLeftMostTile.Item1; x <= bottomRightMostTile.Item1; x++)
                        for (int y = topLeftMostTile.Item2; y <= bottomRightMostTile.Item2; y++)
                        {
                            Tile tile;
                            tileMap.TryGetValue(x, y, out tile);

                            if (tile.Type == TileTypes.Floor)
                                continue;

                            if ((tile.Type == TileTypes.Bottom || tile.Type == TileTypes.BottomConvexCorner) && !tileCircleCollision(new Vector2(x * 64, y * 64 + 32), new Vector2(x * 64 + 64, y * 64 + 64), particleEmitter.GroundPos, collisionRadius))
                            {
                                continue;
                            }
                            else if ((tile.Type == TileTypes.Wall || tile.Type == TileTypes.ConvexCorner) && !tileCircleCollision(new Vector2(x * 64, y * 64), new Vector2(x * 64 + 64, y * 64 + 32), particleEmitter.GroundPos, collisionRadius))
                            {
                                continue;
                            }
                            else if (!tileCircleCollision(new Vector2(x * 64, y * 64), new Vector2(x * 64 + 64, y * 64 + 64), particleEmitter.GroundPos, collisionRadius))
                            {

                                continue;
                            }

                            //bool isBelowHalfWay = (particleEmitter.GroundPos.Y - collisionRadius - y * 64) > 32;
                            //if ((tile.Type == TileTypes.Wall && direction.Y < 0f && isBelowHalfWay) || (tile.Type != TileTypes.Wall && tile.Type != TileTypes.ConvexCorner)
                            //|| (tile.Type == TileTypes.ConvexCorner && direction.Y < 0f && isBelowHalfWay && closestObject.Item1.Y == closestObject.Item2.Y))
                            //{
                                hasCollided = true;
                            //}
                        }
                    if (hasCollided)
                        break;
                }

                createParticles(attacker, direction, projectileSpeed, particleEmitter);

                yield return false;
            }
            #endregion Emit Particles until particle hits target or wall or time to live runs out

            //if the attack frame has passed then allow the attacker to move
            while (attacker.Frame < attacker.FrameCount() - 1 && isLoopOne)
            {
                attacker.isAttackAnimDone = false;
                yield return false;
            }

            attacker.isAttackAnimDone = true;

            #region Finish attacking after all particles are dead
            while (particleEmitter.ActiveParticleCount > 0)
            {
                yield return false;
            }
            #endregion Finish attacking after all particles are dead

            particleEmitter.Remove();
            l.RemoveSprite(particleEmitter);
            attacker.particleEmitters.Remove(particleEmitter);

            yield return true;
        }

        /// <summary>
        /// Performs Lightning Tiers 2 & 3 Attack
        /// </summary>
        /// <param name="_target">The target for the projectile to attack</param>
        /// <param name="_attacker">The sprite initiating the attack</param>
        /// <param name="_baseDamage">The amount of damage to inflict before constant multiplier for weakness</param>
        /// <param name="_attackFrame">The frame that the attack begins on</param>
        /// <returns>Returns true when finished</returns>
        public static IEnumerable<bool> LightningStrike(Sprite _target, OurSprite attacker, int baseDamage, int attackFrame, Element elem = Element.Lightning)
        {
            #region Variables
            Level l = This.Game.CurrentLevel;
            OurSprite target = (OurSprite)_target;
            Vector2 initialDirection = attacker.Direction;
            attacker.State = SpriteState.Attacking;
            setAnimation(attacker);
            int FrameCount = attacker.FrameCount();
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;

            Effect particleEffect = l.GetEffect("ParticleSystem");
            Texture2D lightning = l.GetTexture("sparkball");
            ParticleEmitter particleEmitter = new ParticleEmitter(10000, particleEffect, lightning);
            particleEmitter.ZOrder = int.MaxValue;
            particleEmitter.effectTechnique = "FadeAtXPercent";
            particleEmitter.fadeStartPercent = .98f;
            particleEmitter.blendState = BlendState.Additive;
            (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).Radius = 125;
            (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();

            Vector2 particleTopPosition;
            #endregion Variables

            attacker.isAttackAnimDone = false;
            attacker.Rewind();

            if (attacker is Frostbyte.Characters.Mage)
            {
                (attacker as Frostbyte.Characters.Mage).attackTier = 2;
            }

            #region Shoot Attack
            while (attacker.Frame < FrameCount)
            {
                attacker.isAttackAnimDone = false;

                if (target != null && target.GroundPos != attacker.GroundPos)
                    attacker.Direction = target.GroundPos - particleEmitter.GroundPos;
                attacker.State = SpriteState.Attacking;
                setAnimation(attacker);
                FrameCount = attacker.FrameCount();

                if (attacker.Frame == attackFrame)
                {
                    break;
                }

                yield return false;
            }
            #endregion Shoot Attack

            if (target != null)
            {
                particleEmitter.GroundPos = target.GroundPos;
                particleTopPosition = new Vector2(target.GroundPos.X, target.GroundPos.Y - 400);
            }
            else
            {
                particleEmitter.GroundPos = attacker.GroundPos + 300 * initialDirection;
                particleTopPosition = new Vector2(particleEmitter.GroundPos.X, particleEmitter.GroundPos.Y - 400);
            }


            #region Generate Lightning Strike and Ground Spread and Deal Damage

            bool isAttackAnimDone = false;

            if (This.Game.AudioManager.PlaySoundEffect("Effects/Lightning_Strike", .8f))
            {
                yield return false;
            }

            for (int i = 0; i < 165; i++)
            {
                particleTopPosition = new Vector2(particleEmitter.GroundPos.X, particleEmitter.GroundPos.Y - 400);
                //Generate Start Position Ball
                for (int j = 0; j < 2; j++)
                {
                    double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                    Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                    particleEmitter.createParticles(randDirection * 30, -randDirection * 3, particleTopPosition, 25f, This.Game.rand.Next(100, 1200));
                }


                // Lightning Strike
                if (i % 2 == 0)
                {
                    for (int j = 0; j < 200; j++)
                    {
                        Vector2 directionToTarget = particleEmitter.GroundPos - particleTopPosition;
                        directionToTarget.Normalize();
                        double directionAngle2 = This.Game.rand.NextDouble() * 2 * Math.PI;
                        Vector2 randDirection2 = new Vector2((float)Math.Cos(directionAngle2), (float)Math.Sin(directionAngle2));

                        particleTopPosition += directionToTarget * 2 + randDirection2 * 3;

                        particleEmitter.createParticles(Vector2.Zero, Vector2.Zero, particleTopPosition, 8f, 85);
                    }
                }

                // Ground Spread
                for (int j = 0; j < 30; j++)
                {
                    double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                    Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                    particleEmitter.createParticles(randDirection * 170, -randDirection * 90, particleEmitter.GroundPos, 2f, This.Game.rand.Next(400, 1500));
                }

                //Deal Damage
                int count = 0;
                if (5 - i % 15 == 0 && Collision.CollisionData.Count > 0)
                {
                    List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                    Collision.CollisionData.TryGetValue(particleEmitter, out collidedWith);
                    if (collidedWith != null)
                    {
                        foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                        {
                            if (((detectedCollision.Item2 is Enemy) && (attacker is Player)) || ((detectedCollision.Item2 is Player) && (attacker is Enemy)))
                            {
                                count++;
                                Damage(attacker, (detectedCollision.Item2 as OurSprite), baseDamage);
                            }
                            if (count >= 4)
                                break;
                        }
                    }
                }

                //if the attack frame has passed then allow the attacker to move
                if (attacker.Frame >= FrameCount - 1)
                {
                    attacker.isAttackAnimDone = true;
                    isAttackAnimDone = true;
                }

                if (!isAttackAnimDone)
                    attacker.isAttackAnimDone = false;


                yield return false;
            }

            #endregion Generate Lightning Strike and Ground Spread and Deal Damage
            while (particleEmitter.ActiveParticleCount > 0)
            {
                yield return false;
            }

            particleEmitter.Remove();
            l.RemoveSprite(particleEmitter);
            attacker.particleEmitters.Remove(particleEmitter);

            attacker.isAttackAnimDone = true;

            yield return true;
        }

        /// <summary>
        /// Performs Earthquake Attack
        /// </summary>
        /// <param name="_target">The target for the projectile to attack</param>
        /// <param name="_attacker">The sprite initiating the attack</param>
        /// <param name="_baseDamage">The amount of damage to inflict before constant multiplier for weakness</param>
        /// <param name="_attackFrame">The frame that the attack begins on</param>
        /// <returns>Returns true when finished</returns>
        public static IEnumerable<bool> Earthquake(Sprite _target, OurSprite attacker, int baseDamage, int attackFrame, Element elem = Element.Earth)
        {
            #region Variables
            Level l = This.Game.CurrentLevel;
            OurSprite target = (OurSprite)_target;
            Vector2 initialDirection = attacker.Direction;
            attacker.State = SpriteState.Attacking;
            setAnimation(attacker);
            int FrameCount = attacker.FrameCount();
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;

            Effect particleEffect = l.GetEffect("ParticleSystem");
            Texture2D earthquake = l.GetTexture("earthquake");
            ParticleEmitter particleEmitterDust = new ParticleEmitter(500, particleEffect, earthquake);
            particleEmitterDust.effectTechnique = "NoSpecialEffect";
            particleEmitterDust.blendState = BlendState.AlphaBlend;
            Collision_BoundingCircle c = (particleEmitterDust.collisionObjects.First() as Collision_BoundingCircle);
            c.Radius = 125;
            c.createDrawPoints();

            Texture2D earthquakeRock = l.GetTexture("Earthquake Rock");
            ParticleEmitter particleEmitterRocks = new ParticleEmitter(200, particleEffect, earthquakeRock);
            particleEmitterRocks.effectTechnique = "NoSpecialEffect";
            particleEmitterRocks.blendState = BlendState.AlphaBlend;
            c = (particleEmitterRocks.collisionObjects.First() as Collision_BoundingCircle);
            c.Radius = 125;
            c.createDrawPoints();

            //Texture2D earthquakeRock = l.GetTexture("Earthquake Rock");
            ParticleEmitter particleEmitterRing = new ParticleEmitter(2000, particleEffect, earthquakeRock);
            particleEmitterRing.effectTechnique = "FadeAtXPercent";
            particleEmitterRing.fadeStartPercent = 0f;
            particleEmitterRing.blendState = BlendState.Additive;
            c = (particleEmitterRing.collisionObjects.First() as Collision_BoundingCircle);
            c.Radius = 125;
            c.createDrawPoints();
            #endregion Variables

            attacker.isAttackAnimDone = false;
            attacker.Rewind();

            if (attacker is Frostbyte.Characters.Mage)
            {
                (attacker as Frostbyte.Characters.Mage).attackTier = 2;
            }

            #region Shoot Attack
            while (attacker.Frame < FrameCount)
            {
                attacker.isAttackAnimDone = false;

                if (target != null && target.GroundPos != attacker.GroundPos)
                    attacker.Direction = target.GroundPos - particleEmitterDust.GroundPos;
                attacker.State = SpriteState.Attacking;
                setAnimation(attacker);
                FrameCount = attacker.FrameCount();

                if (attacker.Frame == attackFrame)
                {
                    break;
                }

                yield return false;
            }
            #endregion Shoot Attack

            if (target != null)
            {
                particleEmitterDust.GroundPos = target.GroundPos;
                particleEmitterRocks.GroundPos = target.GroundPos;
                particleEmitterRing.GroundPos = target.GroundPos;
            }
            else
            {
                particleEmitterDust.GroundPos = attacker.GroundPos + 300 * initialDirection;
                particleEmitterRocks.GroundPos = attacker.GroundPos + 300 * initialDirection;
                particleEmitterRing.GroundPos = attacker.GroundPos + 300 * initialDirection;
            }


            #region Generate Earthquake

            bool isAttackAnimDone = false;

            if (This.Game.AudioManager.PlaySoundEffect("Effects/Earthquake"))
            {
                yield return false;
            }

            for (int i = 0; i < 165; i++)
            {
                // Shaking Rocks
                for (int j = 0; j < 1; j++)
                {
                    double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                    Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                    particleEmitterRocks.createParticles(new Vector2(0, -200), new Vector2(0, 800), particleEmitterRocks.GroundPos + randDirection * This.Game.rand.Next(0, 150), 4f, This.Game.rand.Next(100, 400));
                }

                // Dust
                for (int j = 0; j < 5; j++)
                {
                    double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                    Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                    particleEmitterDust.createParticles(new Vector2(0, -10), new Vector2(0, -5), particleEmitterDust.GroundPos + randDirection * This.Game.rand.Next(0, 150), 35f, This.Game.rand.Next(300, 1200));
                }

                // Ring
                if ((double)i % 14 == 0)
                {
                    double startPos = This.Game.rand.Next(0, 50);
                    double maxLength = This.Game.rand.Next(15, 20);
                    for (double j = 0; j < maxLength; j += .15f)
                    {
                        double directionAngle2 = (((j + startPos) % 50) / 50) * 2 * Math.PI;
                        Vector2 circlePoint = new Vector2((float)Math.Cos(directionAngle2), (float)Math.Sin(directionAngle2) / ParticleEmitter.EllipsePerspectiveModifier);
                        particleEmitterRing.createParticles(new Vector2(0, -30), new Vector2(0, -35), particleEmitterRing.GroundPos + circlePoint * 150, 4f, This.Game.rand.Next(1100, 1200));
                    }
                }

                //Deal Damage
                if (5 - i % 15 == 0 && Collision.CollisionData.Count > 0)
                {
                    List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                    Collision.CollisionData.TryGetValue(particleEmitterDust, out collidedWith);
                    if (collidedWith != null)
                    {
                        foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                        {
                            if (((detectedCollision.Item2 is Enemy) && (attacker is Player)) || ((detectedCollision.Item2 is Player) && (attacker is Enemy)))
                            {
                                //Damage(attacker, (detectedCollision.Item2 as OurSprite), baseDamage);
                                Slow((detectedCollision.Item2 as OurSprite), 0.5f, new TimeSpan(0, 0, 0, 1, 500));
                            }
                        }
                    }
                }
                //if the attack frame has passed then allow the attacker to move
                if (attacker.Frame >= FrameCount - 1)
                {
                    attacker.isAttackAnimDone = true;
                    isAttackAnimDone = true;
                }

                if (!isAttackAnimDone)
                    attacker.isAttackAnimDone = false;

                yield return false;
            }

            while (particleEmitterDust.ActiveParticleCount > 0 || particleEmitterRocks.ActiveParticleCount > 0 || particleEmitterRing.ActiveParticleCount > 0)
                yield return false;

            #endregion Generate Earthquake

            particleEmitterDust.Remove();
            l.RemoveSprite(particleEmitterDust);
            attacker.particleEmitters.Remove(particleEmitterDust);

            particleEmitterRocks.Remove();
            l.RemoveSprite(particleEmitterRocks);
            attacker.particleEmitters.Remove(particleEmitterRocks);

            particleEmitterRing.Remove();
            l.RemoveSprite(particleEmitterRing);
            attacker.particleEmitters.Remove(particleEmitterRing);

            attacker.isAttackAnimDone = true;

            yield return true;
        }

        /// <summary>
        /// Performs Ground Clap Attack
        /// </summary>
        /// <param name="_target"></param>
        /// <param name="attacker"></param>
        /// <param name="baseDamage"></param>
        /// <param name="attackFrame"></param>
        /// <param name="elem"></param>
        /// <returns></returns>
        public static IEnumerable<bool> RockShower(Sprite _target, OurSprite attacker, int baseDamage, int attackFrame, Element elem = Element.Earth)
        {

            if (_target == null)
            {
                yield return true;
            }

            #region Variables
            Level l = This.Game.CurrentLevel;
            OurSprite target = (OurSprite)_target;
            Vector2 initialDirection = attacker.Direction;
            attacker.State = SpriteState.Attacking;
            setAnimation(attacker);
            int FrameCount = attacker.FrameCount();
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;
            List<CollisionObject> collisions = target.GetCollision();

            Effect particleEffectDust = l.GetEffect("ParticleSystem");
            Texture2D earthquake = l.GetTexture("earthquake");
            ParticleEmitter particleEmitterDust = new ParticleEmitter(500, particleEffectDust, earthquake);
            particleEmitterDust.effectTechnique = "NoSpecialEffect";
            particleEmitterDust.blendState = BlendState.AlphaBlend;
            Collision_BoundingCircle c = (particleEmitterDust.collisionObjects.First() as Collision_BoundingCircle);
            c.Radius = collisions.Count > 0 ? (collisions[0] as Collision_BoundingCircle).Radius + 20 : 100;
            c.createDrawPoints();
            particleEmitterDust.ZOrder = 10;

            Effect particleEffectRocks = l.GetEffect("ParticleSystem");
            Texture2D boulder = l.GetTexture("boulder");
            ParticleEmitter particleEmitterRocks = new ParticleEmitter(4000, particleEffectRocks, boulder);
            particleEmitterRocks.effectTechnique = "NoSpecialEffect";
            particleEmitterRocks.blendState = BlendState.AlphaBlend;
            c = (particleEmitterRocks.collisionObjects.First() as Collision_BoundingCircle);
            c.Radius = collisions.Count > 0 ? (collisions[0] as Collision_BoundingCircle).Radius + 20 : 100;
            c.createDrawPoints();
            float rockParticleEmitterRadius = c.Radius;
            particleEmitterRocks.ZOrder = 1;

            #endregion Variables

            attacker.isAttackAnimDone = false;
            attacker.Rewind();

            if (attacker is Frostbyte.Characters.Mage)
            {
                (attacker as Frostbyte.Characters.Mage).attackTier = 3;
            }

            #region Shoot Attack
            while (attacker.Frame < FrameCount)
            {
                attacker.isAttackAnimDone = false;

                if (target.GroundPos != attacker.GroundPos)
                    attacker.Direction = target.GroundPos - particleEmitterDust.GroundPos;
                attacker.State = SpriteState.Attacking;
                setAnimation(attacker);
                FrameCount = attacker.FrameCount();

                if (attacker.Frame == attackFrame)
                {
                    break;
                }

                yield return false;
            }
            #endregion Shoot Attack

            #region Generate Rock Shower

            bool isAttackAnimDone = false;

            for (int i = 0; i < 165; i++)
            {
                particleEmitterDust.GroundPos = target.GroundPos;
                particleEmitterRocks.GroundPos = target.GroundPos;

                // Dust Cloud
                for (int j = 0; j < rockParticleEmitterRadius / 8; j++)
                {
                    double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                    Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                    particleEmitterDust.createParticles(new Vector2(0, -6), new Vector2(0, -5), particleEmitterDust.GroundPos + new Vector2(0, -175) + randDirection * This.Game.rand.Next(0, (int)(rockParticleEmitterRadius * 1.4f)), 15f, This.Game.rand.Next(300, 1200));
                }

                // Rock Shower
                for (int j = 0; j < rockParticleEmitterRadius / 8; j++)
                {
                    double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                    Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                    particleEmitterRocks.createParticles(new Vector2(0, 275), new Vector2(10, 50), particleEmitterRocks.GroundPos + new Vector2(0, -175) + randDirection * This.Game.rand.Next(0, (int)rockParticleEmitterRadius), 5f, This.Game.rand.Next(300, 600));
                }

                //Deal Damage
                if (5 - i % 15 == 0 && Collision.CollisionData.Count > 0)
                {
                    Damage(attacker, target, baseDamage);
                    Slow(target, 0.5f, new TimeSpan(0, 0, 1));
                }

                //if the attack frame has passed then allow the attacker to move
                if (attacker.Frame >= FrameCount - 1)
                {
                    attacker.isAttackAnimDone = true;
                    isAttackAnimDone = true;
                }

                if (!isAttackAnimDone)
                    attacker.isAttackAnimDone = false;

                yield return false;
            }

            while (particleEmitterDust.ActiveParticleCount > 0 || particleEmitterRocks.ActiveParticleCount > 0)
                yield return false;

            #endregion Generate Rock Shower

            particleEmitterDust.Remove();
            l.RemoveSprite(particleEmitterDust);
            attacker.particleEmitters.Remove(particleEmitterDust);

            particleEmitterRocks.Remove();
            l.RemoveSprite(particleEmitterRocks);
            attacker.particleEmitters.Remove(particleEmitterRocks);

            attacker.isAttackAnimDone = true;

            yield return true;
        }

        /// <summary>
        /// Performs Fire Ring Attack
        /// </summary>
        /// <param name="_target">The target for the projectile to attack</param>
        /// <param name="_attacker">The sprite initiating the attack</param>
        /// <param name="_baseDamage">The amount of damage to inflict before constant multiplier for weakness</param>
        /// <param name="_attackFrame">The frame that the attack begins on</param>
        /// <returns>Returns true when finished</returns>
        public static IEnumerable<bool> FireRing(Sprite _target, OurSprite attacker, int baseDamage, int attackFrame, Element elem = Element.Fire)
        {
            #region Variables
            Level l = This.Game.CurrentLevel;
            OurSprite target = (OurSprite)_target;
            Vector2 initialDirection = attacker.Direction;
            attacker.State = SpriteState.Attacking;
            setAnimation(attacker);
            int FrameCount = attacker.FrameCount();
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;

            Effect particleEffect = l.GetEffect("ParticleSystem");
            Texture2D fire = l.GetTexture("fireParticle");
            ParticleEmitter particleEmitterFire = new ParticleEmitter(4000, particleEffect, l.GetTexture("fire darker"));
            particleEmitterFire.effectTechnique = "FadeAtXPercent";
            particleEmitterFire.fadeStartPercent = .75f;
            particleEmitterFire.blendState = BlendState.AlphaBlend;
            particleEmitterFire.ZOrder = 1;
            Collision_BoundingCircle c = (particleEmitterFire.collisionObjects.First() as Collision_BoundingCircle);
            c.Radius = 115;
            c.createDrawPoints();

            ParticleEmitter particleEmitterSkywardRing = new ParticleEmitter(4000, particleEffect, fire);
            particleEmitterSkywardRing.effectTechnique = "FadeAtXPercent";
            particleEmitterSkywardRing.blendState = BlendState.Additive;
            particleEmitterSkywardRing.fadeStartPercent = .8f;
            particleEmitterSkywardRing.ZOrder = 2;
            c.Radius = 115;
            c.createDrawPoints();

            Texture2D redfire = l.GetTexture("red fire");
            Texture2D smoke = l.GetTexture("smoke");
            ParticleEmitter particleEmitterRedFire = new ParticleEmitter(1500, particleEffect, redfire, smoke);
            particleEmitterRedFire.effectTechnique = "ChangePicAndFadeAtXPercent";
            particleEmitterRedFire.blendState = BlendState.AlphaBlend;
            particleEmitterRedFire.changePicPercent = .2f;
            particleEmitterRedFire.fadeStartPercent = .9f;
            particleEmitterRedFire.ZOrder = 3;
            c = (particleEmitterRedFire.collisionObjects.First() as Collision_BoundingCircle);
            c.Radius = 115;
            c.createDrawPoints();

            ParticleEmitter particleEmitterDOT = new ParticleEmitter(1500, particleEffect, fire);
            particleEmitterDOT.effectTechnique = "FadeAtXPercent";
            particleEmitterDOT.fadeStartPercent = .98f;
            particleEmitterDOT.blendState = BlendState.Additive;
            particleEmitterDOT.ZOrder = 4;
            c = (particleEmitterDOT.collisionObjects.First() as Collision_BoundingCircle);
            c.Radius = 115;
            c.createDrawPoints();

            Vector2 collisionOffset = new Vector2(0, 15);

            Dictionary<OurSprite, TimeSpan> DOT = new Dictionary<OurSprite, TimeSpan>();
            #endregion Variables

            attacker.isAttackAnimDone = false;
            attacker.Rewind();

            if (attacker is Frostbyte.Characters.Mage)
            {
                (attacker as Frostbyte.Characters.Mage).attackTier = 2;
            }

            #region Shoot Attack
            while (attacker.Frame < FrameCount)
            {
                attacker.isAttackAnimDone = false;

                if (target != null && target.GroundPos != attacker.GroundPos)
                    attacker.Direction = target.GroundPos - particleEmitterFire.GroundPos;
                attacker.State = SpriteState.Attacking;
                setAnimation(attacker);
                FrameCount = attacker.FrameCount();

                if (attacker.Frame == attackFrame)
                {
                    break;
                }

                yield return false;
            }
            #endregion Shoot Attack

            if (target != null)
            {
                particleEmitterFire.GroundPos = target.GroundPos - collisionOffset;
                particleEmitterRedFire.GroundPos = target.GroundPos - collisionOffset;
                particleEmitterSkywardRing.GroundPos = target.GroundPos - collisionOffset;
            }
            else
            {
                particleEmitterFire.GroundPos = attacker.GroundPos + 300 * initialDirection - collisionOffset;
                particleEmitterRedFire.GroundPos = attacker.GroundPos + 300 * initialDirection - collisionOffset;
                particleEmitterSkywardRing.GroundPos = attacker.GroundPos + 300 * initialDirection - collisionOffset;
            }


            #region Generate Fire Ring and Attack

            bool isAttackAnimDone = false;

            for (int i = 0; i < 165; i++)
            {
                //Skyward Flames
                if ((double)i % 5 == 0)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        double directionAngle = This.Game.rand.NextDouble() * 1 * Math.PI + Math.PI * 2;
                        Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                        Vector2 velocity = new Vector2(This.Game.rand.Next(-10, 10), -100);
                        Vector2 acceleration = new Vector2(This.Game.rand.Next(-10, 10), -200);
                        particleEmitterRedFire.createParticles(velocity,
                                                               acceleration,
                                                               particleEmitterRedFire.GroundPos + randDirection * This.Game.rand.Next(0, 140) + collisionOffset,
                                                               10f,
                                                               This.Game.rand.Next(100, 1200));
                    }
                }

                //Circle Firing
                if ((double)i % 3 == 0)
                {
                    for (double j = 0; j < 30; j += .3f)
                    {
                        double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                        Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                        Vector2 velocity = -randDirection * 100;
                        Vector2 acceleration = -randDirection * 70;
                        particleEmitterFire.createParticles(velocity,
                                                            acceleration,
                                                            particleEmitterFire.GroundPos + randDirection * 140 + collisionOffset,
                                                            17f,
                                                            This.Game.rand.Next(100, 1200));
                    }
                }

                //Skyward Ring
                if ((double)i % 2 == 0)
                {
                    for (double j = 0; j < 35; j += .3f)
                    {
                        double positionAngle = (((double)i + j % 35.0) / 35.0) * Math.PI * 2;
                        Vector2 position = new Vector2((float)Math.Cos(positionAngle) * 140, (float)Math.Sin(positionAngle) * This.Game.rand.Next(120, 140) / ParticleEmitter.EllipsePerspectiveModifier) + particleEmitterSkywardRing.GroundPos;
                        Vector2 direction = particleEmitterSkywardRing.GroundPos - position;
                        direction.Normalize();
                        Vector2 velocity = new Vector2(This.Game.rand.Next(-10, 10), -75);
                        Vector2 acceleration = new Vector2(This.Game.rand.Next(-10, 10), -200);
                        particleEmitterSkywardRing.createParticles(velocity,
                                                                   acceleration,
                                                                   position + collisionOffset,
                                                                   This.Game.rand.Next(5, 20),
                                                                   This.Game.rand.Next(400, 600));
                    }
                }

                //Add Enemies to DOT Dictionary
                if (i % 10 == 0 && Collision.CollisionData.Count > 0)
                {
                    List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                    Collision.CollisionData.TryGetValue(particleEmitterFire, out collidedWith);
                    if (collidedWith != null)
                    {
                        foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                        {
                            if (((detectedCollision.Item2 is Enemy) && (attacker is Player)) || ((detectedCollision.Item2 is Player) && (attacker is Enemy)))
                            {
                                if (!DOT.ContainsKey((OurSprite)detectedCollision.Item2))
                                    DOT.Add((OurSprite)detectedCollision.Item2, new TimeSpan(0, 0, 2) + This.gameTime.TotalGameTime);
                                else
                                    DOT[(OurSprite)detectedCollision.Item2] = new TimeSpan(0, 0, 2) + This.gameTime.TotalGameTime;
                            }
                        }
                    }
                }

                //Deal Damage, Remove DOT's That Have Timed Out, and Create Particles on Enemies
                if ((double)i % 18 == 0)
                {
                    List<OurSprite> removeDOT = new List<OurSprite>();
                    foreach (KeyValuePair<OurSprite, TimeSpan> dottedTarget in DOT)
                    {
                        Damage(attacker, dottedTarget.Key, baseDamage);
                        if (dottedTarget.Value < This.gameTime.TotalGameTime || !((l as FrostbyteLevel).enemies.Contains(dottedTarget.Key) || (l as FrostbyteLevel).allies.Contains(dottedTarget.Key)))
                            removeDOT.Add(dottedTarget.Key);

                        //Create Particles
                        for (int j = 0; j < 5; j++)
                        {
                            double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                            Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                            Vector2 velocity = new Vector2(This.Game.rand.Next(-10, 10), -10);
                            Vector2 acceleration = new Vector2(This.Game.rand.Next(-10, 10), -10);
                            List<CollisionObject> collisionList = dottedTarget.Key.GetCollision();
                            int radius = (int)(collisionList.Count > 0 ? (collisionList[0] as Collision_BoundingCircle).Radius + 20 : 40);
                            particleEmitterDOT.createParticles(velocity,
                                                               acceleration,
                                                               dottedTarget.Key.GroundPos + randDirection * This.Game.rand.Next(0, radius),
                                                               10f,
                                                               This.Game.rand.Next(100, 500));
                        }
                    }
                    foreach (OurSprite dottedTarget in removeDOT)
                        DOT.Remove(dottedTarget);
                }

                //if the attack frame has passed then allow the attacker to move
                if (attacker.Frame >= FrameCount - 1)
                {
                    attacker.isAttackAnimDone = true;
                    isAttackAnimDone = true;
                }

                if (!isAttackAnimDone)
                    attacker.isAttackAnimDone = false;

                yield return false;
            }

            int count = 0;
            while (particleEmitterFire.ActiveParticleCount > 0 || particleEmitterRedFire.ActiveParticleCount > 0 || particleEmitterSkywardRing.ActiveParticleCount > 0 || DOT.Count > 0)
            {
                if (count % 18 == 0)
                {
                    //Deal Damage, Remove DOT's That Have Timed Out, and Create Particles on Enemies
                    List<OurSprite> removeDOT = new List<OurSprite>();
                    foreach (KeyValuePair<OurSprite, TimeSpan> dottedTarget in DOT)
                    {
                        Damage(attacker, dottedTarget.Key, baseDamage);
                        if (dottedTarget.Value < This.gameTime.TotalGameTime || !(l as FrostbyteLevel).enemies.Contains(dottedTarget.Key))
                            removeDOT.Add(dottedTarget.Key);

                        //Create Particles
                        for (int j = 0; j < 5; j++)
                        {
                            double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                            Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                            Vector2 velocity = new Vector2(This.Game.rand.Next(-10, 10), -10);
                            Vector2 acceleration = new Vector2(This.Game.rand.Next(-10, 10), -10);
                            List<CollisionObject> collisionList = dottedTarget.Key.GetCollision();
                            int radius = (int)(collisionList.Count > 0 ? (collisionList[0] as Collision_BoundingCircle).Radius + 20 : 40);
                            particleEmitterDOT.createParticles(velocity,
                                                               acceleration,
                                                               dottedTarget.Key.GroundPos + randDirection * This.Game.rand.Next(0, radius),
                                                               10f,
                                                               This.Game.rand.Next(100, 500));
                        }
                    }
                    foreach (OurSprite dottedTarget in removeDOT)
                        DOT.Remove(dottedTarget);
                }

                count++;

                yield return false;
            }

            #endregion Generate Fire Ring and Attack

            particleEmitterFire.Remove();
            l.RemoveSprite(particleEmitterFire);
            attacker.particleEmitters.Remove(particleEmitterFire);

            particleEmitterRedFire.Remove();
            l.RemoveSprite(particleEmitterRedFire);
            attacker.particleEmitters.Remove(particleEmitterRedFire);

            particleEmitterSkywardRing.Remove();
            l.RemoveSprite(particleEmitterSkywardRing);
            attacker.particleEmitters.Remove(particleEmitterSkywardRing);

            attacker.isAttackAnimDone = true;

            yield return true;
        }

        /// <summary>
        /// Performs Fire Pillar Attack
        /// </summary>
        public static IEnumerable<bool> FirePillar(Sprite _target, OurSprite attacker, int baseDamage, int attackFrame, Element elem = Element.Fire)
        {
            if (_target == null)
            {
                yield return true;
            }

            #region Variables
            Level l = This.Game.CurrentLevel;
            OurSprite target = (OurSprite)_target;
            Vector2 initialDirection = attacker.Direction;
            attacker.State = SpriteState.Attacking;
            setAnimation(attacker);
            int FrameCount = attacker.FrameCount();
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;
            List<CollisionObject> collisions = target.GetCollision();

            Effect particleEffect = l.GetEffect("ParticleSystem");
            Texture2D fire = l.GetTexture("fireParticle");
            ParticleEmitter particleEmitterFire = new ParticleEmitter(3000, particleEffect, fire);
            particleEmitterFire.effectTechnique = "NoSpecialEffect";
            particleEmitterFire.blendState = BlendState.Additive;
            Collision_BoundingCircle c = (particleEmitterFire.collisionObjects.First() as Collision_BoundingCircle);
            c.Radius = collisions.Count > 0 ? (collisions[0] as Collision_BoundingCircle).Radius + 20 : 100;
            c.createDrawPoints();
            particleEmitterFire.ZOrder = 1;

            ParticleEmitter particleEmitterGroundFire = new ParticleEmitter(2000, particleEffect, fire);
            particleEmitterGroundFire.effectTechnique = "FadeAtXPercent";
            particleEmitterGroundFire.fadeStartPercent = .6f;
            particleEmitterGroundFire.blendState = BlendState.Additive;
            particleEmitterGroundFire.ZOrder = 2;

            Texture2D maroonfire = l.GetTexture("maroon fire");
            ParticleEmitter particleEmitterGroundRedFire = new ParticleEmitter(2000, particleEffect, maroonfire);
            particleEmitterGroundRedFire.effectTechnique = "NoSpecialEffect";
            particleEmitterGroundRedFire.fadeStartPercent = .6f;
            particleEmitterGroundRedFire.blendState = BlendState.AlphaBlend;
            particleEmitterGroundRedFire.ZOrder = 3;

            TimeSpan DOTStart = new TimeSpan();
            #endregion Variables

            attacker.isAttackAnimDone = false;
            attacker.Rewind();

            if (attacker is Frostbyte.Characters.Mage)
            {
                (attacker as Frostbyte.Characters.Mage).attackTier = 3;
            }

            #region Shoot Attack
            while (attacker.Frame < FrameCount)
            {
                attacker.isAttackAnimDone = false;

                if (target.GroundPos != attacker.GroundPos)
                    attacker.Direction = target.GroundPos - particleEmitterFire.GroundPos;
                attacker.State = SpriteState.Attacking;
                setAnimation(attacker);
                FrameCount = attacker.FrameCount();

                if (attacker.Frame == attackFrame)
                {
                    break;
                }

                yield return false;
            }
            #endregion Shoot Attack

            #region Generate Fire Pillar

            bool isAttackAnimDone = false;

            for (int i = 0; i < 165; i++)
            {
                particleEmitterFire.GroundPos = target.GroundPos;

                //Swirl
                if (i < 100)
                {
                    for (float j = 0; j < 60.0f; j += 60.0f / 6.0f)
                    {
                        for (float k = i; k < i + 1; k += .1f)
                        {
                            double positionAngle = (((k + j) % 60.0) / 60.0) * Math.PI * 2.0;
                            Vector2 position = new Vector2((float)Math.Cos(positionAngle) * 150.0f * ((float)(100.0f - k) / 100.0f),
                                                           (float)Math.Sin(positionAngle) * 150.0f * ((float)(100.0f - k) / 100.0f) / ParticleEmitter.EllipsePerspectiveModifier - k * 2.5f) + particleEmitterFire.GroundPos;
                            Vector2 direction = particleEmitterFire.GroundPos - position;
                            Vector2 velocity = direction / .200f;
                            particleEmitterFire.createParticles(velocity,
                                                                Vector2.Zero,
                                                                position + direction * (float)This.Game.rand.Next(0, (int)velocity.Length() / 16) / direction.Length(),
                                                                12,
                                                                150);
                        }
                        j += 60.0f / 6.0f;
                        for (float k = i; k < i + 1; k += .1f)
                        {
                            double positionAngle = (((k + j) % 60.0) / 60.0) * Math.PI * 2.0;
                            Vector2 position = new Vector2((float)Math.Cos(positionAngle) * 150.0f * ((float)(100.0f - k) / 100.0f),
                                                           (float)Math.Sin(positionAngle) * 150.0f * ((float)(100.0f - k) / 100.0f) / ParticleEmitter.EllipsePerspectiveModifier - k * 2.5f) + particleEmitterFire.GroundPos;
                            Vector2 direction = particleEmitterFire.GroundPos - position;
                            Vector2 velocity = direction / .200f;
                            particleEmitterGroundRedFire.createParticles(velocity,
                                                                Vector2.Zero,
                                                                position + direction * (float)This.Game.rand.Next(0, (int)velocity.Length() / 16) / direction.Length(),
                                                                12,
                                                                150);
                        }
                    }
                }


                if (i == 100)
                {
                    particleEmitterGroundFire.GroundPos = target.GroundPos - new Vector2(0, 250);
                    particleEmitterGroundRedFire.GroundPos = target.GroundPos - new Vector2(0, 250);
                }

                //Strike
                if (i >= 100 && i < 107)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                        Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle));
                        if (target.GroundPos != particleEmitterGroundFire.GroundPos)
                        {
                            Vector2 direction = target.GroundPos - particleEmitterGroundFire.GroundPos;
                            direction.Normalize();
                            Vector2 tangent = new Vector2(-direction.Y, direction.X);
                            particleEmitterGroundFire.createParticles(Vector2.Zero,
                                                                   direction * 500 + randDirection * 500,
                                                                   particleEmitterGroundFire.GroundPos + tangent * This.Game.rand.Next(-15, 15),
                                                                   20,
                                                                   500);
                            particleEmitterGroundFire.GroundPos += direction;
                        }
                    }
                    for (int j = 0; j < 20; j++)
                    {
                        double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                        Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle));
                        if (target.GroundPos != particleEmitterGroundFire.GroundPos)
                        {
                            Vector2 direction = target.GroundPos - particleEmitterGroundFire.GroundPos;
                            direction.Normalize();
                            Vector2 tangent = new Vector2(-direction.Y, direction.X);
                            particleEmitterGroundRedFire.createParticles(Vector2.Zero,
                                                                   direction * 500 + randDirection * 500,
                                                                   particleEmitterGroundFire.GroundPos + tangent * This.Game.rand.Next(-15, 15),
                                                                   20,
                                                                   300);
                            particleEmitterGroundFire.GroundPos += direction;
                        }
                    }
                }

                //Explode
                if (i >= 108)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        double directionAngle = This.Game.rand.NextDouble() * Math.PI;
                        Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                        particleEmitterGroundFire.createParticles(-randDirection * 200,
                                                               -randDirection * 300 - new Vector2(0, 800),
                                                               particleEmitterGroundFire.GroundPos + new Vector2(This.Game.rand.Next(-60, 60), 0),
                                                               20,
                                                               400);
                    }
                    for (int j = 0; j < 10; j++)
                    {
                        double directionAngle = This.Game.rand.NextDouble() * Math.PI;
                        Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                        particleEmitterGroundRedFire.createParticles(-randDirection * 200,
                                                               -randDirection * 300 - new Vector2(0, 800),
                                                               particleEmitterGroundFire.GroundPos + new Vector2(This.Game.rand.Next(-60, 60), 0),
                                                               20,
                                                               200);
                    }
                }

                //Deal Damage
                if (i == 107)
                {
                    Damage(attacker, target, baseDamage / 2);
                    DOTStart = This.gameTime.TotalGameTime;
                }


                //Draw DOT and deal damage
                if (i > 107 && i % 18 == 0)
                {
                    Damage(attacker, target, baseDamage / 35);

                    //Create Particles
                    for (int j = 0; j < 3; j++)
                    {
                        double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                        Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                        Vector2 velocity = new Vector2(This.Game.rand.Next(-10, 10), -10);
                        Vector2 acceleration = new Vector2(This.Game.rand.Next(-10, 10), -10);
                        List<CollisionObject> collisionList = target.GetCollision();
                        int radius = (int)(collisionList.Count > 0 ? (collisionList[0] as Collision_BoundingCircle).Radius + 20 : 40);
                        particleEmitterGroundFire.createParticles(velocity,
                                                           acceleration,
                                                           target.GroundPos + randDirection * This.Game.rand.Next(0, radius),
                                                           10f,
                                                           This.Game.rand.Next(400, 800));
                    }
                    //Create Particles
                    for (int j = 0; j < 3; j++)
                    {
                        double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                        Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                        Vector2 velocity = new Vector2(This.Game.rand.Next(-10, 10), -10);
                        Vector2 acceleration = new Vector2(This.Game.rand.Next(-10, 10), -10);
                        List<CollisionObject> collisionList = target.GetCollision();
                        int radius = (int)(collisionList.Count > 0 ? (collisionList[0] as Collision_BoundingCircle).Radius + 20 : 40);
                        particleEmitterGroundRedFire.createParticles(velocity,
                                                           acceleration,
                                                           target.GroundPos + randDirection * This.Game.rand.Next(0, radius),
                                                           10f,
                                                           This.Game.rand.Next(400, 800));
                    }
                }

                //if the attack frame has passed then allow the attacker to move
                if (attacker.Frame >= FrameCount - 1)
                {
                    attacker.isAttackAnimDone = true;
                    isAttackAnimDone = true;
                }

                if (!isAttackAnimDone)
                    attacker.isAttackAnimDone = false;

                yield return false;
            }

            //continue drawing dot until it is expired
            for (int i = 0; DOTStart + new TimeSpan(0, 0, 5) > This.gameTime.TotalGameTime && ((l as FrostbyteLevel).enemies.Contains(target) || (l as FrostbyteLevel).allies.Contains(target)); i++)
            {
                if (i % 18 == 0)
                {
                    Damage(attacker, target, baseDamage / 35);

                    //Create Particles
                    for (int j = 0; j < 3; j++)
                    {
                        double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                        Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                        Vector2 velocity = new Vector2(This.Game.rand.Next(-10, 10), -10);
                        Vector2 acceleration = new Vector2(This.Game.rand.Next(-10, 10), -10);
                        List<CollisionObject> collisionList = target.GetCollision();
                        int radius = (int)(collisionList.Count > 0 ? (collisionList[0] as Collision_BoundingCircle).Radius + 20 : 40);
                        particleEmitterGroundFire.createParticles(velocity,
                                                           acceleration,
                                                           target.GroundPos + randDirection * This.Game.rand.Next(0, radius),
                                                           10f,
                                                           This.Game.rand.Next(400, 800));
                    }
                    //Create Particles
                    for (int j = 0; j < 3; j++)
                    {
                        double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                        Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                        Vector2 velocity = new Vector2(This.Game.rand.Next(-10, 10), -10);
                        Vector2 acceleration = new Vector2(This.Game.rand.Next(-10, 10), -10);
                        List<CollisionObject> collisionList = target.GetCollision();
                        int radius = (int)(collisionList.Count > 0 ? (collisionList[0] as Collision_BoundingCircle).Radius + 20 : 40);
                        particleEmitterGroundRedFire.createParticles(velocity,
                                                           acceleration,
                                                           target.GroundPos + randDirection * This.Game.rand.Next(0, radius),
                                                           10f,
                                                           This.Game.rand.Next(400, 800));
                    }
                }
                yield return false;
            }

            while (particleEmitterFire.ActiveParticleCount > 0 || particleEmitterGroundFire.ActiveParticleCount > 0 || particleEmitterGroundRedFire.ActiveParticleCount > 0)
                yield return false;

            #endregion Generate Fire Pillar

            particleEmitterFire.Remove();
            l.RemoveSprite(particleEmitterFire);
            attacker.particleEmitters.Remove(particleEmitterFire);

            particleEmitterGroundFire.Remove();
            l.RemoveSprite(particleEmitterGroundFire);
            attacker.particleEmitters.Remove(particleEmitterGroundFire);

            particleEmitterGroundRedFire.Remove();
            l.RemoveSprite(particleEmitterGroundRedFire);
            attacker.particleEmitters.Remove(particleEmitterGroundRedFire);

            attacker.isAttackAnimDone = true;

            yield return true;
        }

        /// <summary>
        /// Performs Water Push Attack
        /// </summary>
        public static IEnumerable<bool> WaterPush(OurSprite attacker, int attackFrame, Element elem = Element.Water)
        {
            #region Variables
            Level l = This.Game.CurrentLevel;
            attacker.State = SpriteState.Attacking;
            setAnimation(attacker);
            int FrameCount = attacker.FrameCount();
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;

            Effect particleEffect = l.GetEffect("ParticleSystem");
            Texture2D lightning = l.GetTexture("sparkball");
            Texture2D water = l.GetTexture("ice");
            ParticleEmitter particleEmitter = new ParticleEmitter(5000, particleEffect, water);
            particleEmitter.ZOrder = 2;
            particleEmitter.effectTechnique = "FadeAtXPercent";
            particleEmitter.effectTechnique = "FadeAtXPercent";
            particleEmitter.fadeStartPercent = .98f;
            particleEmitter.blendState = BlendState.Additive;
            (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).Radius = 1;
            (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();

            ParticleEmitter particleEmitterDarkBlue = new ParticleEmitter(7000, particleEffect, water);
            particleEmitterDarkBlue.ZOrder = 1;
            particleEmitterDarkBlue.effectTechnique = "FadeAtXPercent";
            particleEmitterDarkBlue.fadeStartPercent = .9f;
            particleEmitterDarkBlue.blendState = BlendState.Additive;
            #endregion Variables

            attacker.isAttackAnimDone = false;
            attacker.Rewind();

            if (attacker is Frostbyte.Characters.Mage)
            {
                (attacker as Frostbyte.Characters.Mage).attackTier = 2;
            }

            #region Shoot Attack
            while (attacker.Frame < FrameCount)
            {
                attacker.isAttackAnimDone = false;

                attacker.State = SpriteState.Attacking;
                setAnimation(attacker);
                FrameCount = attacker.FrameCount();

                if (attacker.Frame == attackFrame)
                {
                    break;
                }

                yield return false;
            }
            #endregion Shoot Attack

            particleEmitter.GroundPos = attacker.GroundPos;

            #region Generate Wave

            bool isAttackAnimDone = false;

            if (This.Game.AudioManager.PlaySoundEffect("Effects/Lightning_Strike"))
            {
                yield return false;
            }

            for (int i = 0; i < 275; i++)
            {
                (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).Radius = 1 + ((float)i / 185f) * 82f;
                (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();

                // Ground Spread
                if (i < 150)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                        Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                        particleEmitterDarkBlue.createParticles(randDirection * 10, randDirection * 10, particleEmitter.GroundPos + randDirection * This.Game.rand.Next(0, 20), 15, This.Game.rand.Next(3500, 3800));
                    }
                }

                // Ground Spread
                for (int j = 0; j < 5; j++)
                {
                    double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                    Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                    particleEmitterDarkBlue.createParticles(new Vector2(0, -100), new Vector2(This.Game.rand.Next(-50, 50), -100), particleEmitter.GroundPos + randDirection * (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).Radius, 10, This.Game.rand.Next(800, 1200));
                }

                //Deal Damage
                int count = 0;
                if (Collision.CollisionData.Count > 0)
                {
                    List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                    Collision.CollisionData.TryGetValue(particleEmitter, out collidedWith);
                    if (collidedWith != null)
                    {
                        foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                        {
                            if (((detectedCollision.Item2 is Enemy) && (attacker is Player)) || ((detectedCollision.Item2 is Player) && (attacker is Enemy)))
                            {
                                count++;
                                Slow((detectedCollision.Item2 as OurSprite), 0.0f, new TimeSpan(0, 0, 1));
                                if (count >= 4)
                                    break;
                            }
                        }
                    }
                }

                //if the attack frame has passed then allow the attacker to move
                if (attacker.Frame >= FrameCount - 1)
                {
                    attacker.isAttackAnimDone = true;
                    isAttackAnimDone = true;
                }

                if (!isAttackAnimDone)
                    attacker.isAttackAnimDone = false;


                yield return false;
            }

            #endregion Generate Lightning Strike and Ground Spread and Deal Damage

            while (particleEmitter.ActiveParticleCount > 0 || particleEmitterDarkBlue.ActiveParticleCount > 0)
            {
                yield return false;
            }

            particleEmitter.Remove();
            l.RemoveSprite(particleEmitter);
            attacker.particleEmitters.Remove(particleEmitter);

            particleEmitterDarkBlue.Remove();
            l.RemoveSprite(particleEmitterDarkBlue);
            attacker.particleEmitters.Remove(particleEmitterDarkBlue);

            attacker.isAttackAnimDone = true;

            yield return true;
        }

        /// <summary>
        /// Performs Freeze Attack
        /// </summary>
        public static IEnumerable<bool> Freeze(Sprite _target, OurSprite attacker, int attackFrame, Element elem = Element.Water)
        {

            if (_target == null)
            {
                yield return true;
            }

            #region Variables
            Level l = This.Game.CurrentLevel;
            OurSprite target = (OurSprite)_target;
            Vector2 initialDirection = attacker.Direction;
            attacker.State = SpriteState.Attacking;
            setAnimation(attacker);
            int FrameCount = attacker.FrameCount();
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;
            List<CollisionObject> collisions = target.GetCollision();

            Effect particleEffect = l.GetEffect("ParticleSystem");
            Texture2D ice = l.GetTexture("ice");
            ParticleEmitter particleEmitterIce = new ParticleEmitter(500, particleEffect, ice);
            particleEmitterIce.effectTechnique = "FadeAtXPercent";
            particleEmitterIce.fadeStartPercent = .98f;
            particleEmitterIce.blendState = BlendState.Additive;
            Collision_BoundingCircle c = (particleEmitterIce.collisionObjects.First() as Collision_BoundingCircle);
            c.Radius = collisions.Count > 0 ? (collisions[0] as Collision_BoundingCircle).Radius + 20 : 100;
            c.createDrawPoints();
            float particleEmitterRadius = c.Radius;

            #endregion Variables

            attacker.isAttackAnimDone = false;
            attacker.Rewind();

            if (attacker is Frostbyte.Characters.Mage)
            {
                (attacker as Frostbyte.Characters.Mage).attackTier = 3;
            }

            #region Shoot Attack
            while (attacker.Frame < FrameCount)
            {
                attacker.isAttackAnimDone = false;

                if (target.GroundPos != attacker.GroundPos)
                    attacker.Direction = target.GroundPos - particleEmitterIce.GroundPos;
                attacker.State = SpriteState.Attacking;
                setAnimation(attacker);
                FrameCount = attacker.FrameCount();

                if (attacker.Frame == attackFrame)
                {
                    break;
                }

                yield return false;
            }
            #endregion Shoot Attack

            #region Generate Freeze

            bool isAttackAnimDone = false;

            for (int i = 0; i < 225; i++)
            {
                particleEmitterIce.GroundPos = target.GroundPos;

                // Ice
                for (int j = 0; j < 3; j++)
                {
                    double directionAngle = This.Game.rand.NextDouble() * Math.PI;
                    Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                    particleEmitterIce.createParticles(randDirection * 20,
                                                           randDirection * 10 - new Vector2(0, 50),
                                                           particleEmitterIce.GroundPos + randDirection * This.Game.rand.Next(5, (int)particleEmitterRadius),
                                                           This.Game.rand.Next(5, 20),
                                                           600);
                }

                // Snow
                for (int j = 0; j < particleEmitterRadius / 7; j++)
                {
                    double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                    Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                    particleEmitterIce.createParticles(new Vector2(0, 50), new Vector2(10, 40), particleEmitterIce.GroundPos + new Vector2(0, This.Game.rand.Next(-175, -50)) + randDirection * This.Game.rand.Next(0, (int)particleEmitterRadius), 5f, This.Game.rand.Next(500, 800));
                }

                //Freeze Target
                Slow(target, 0.0f, new TimeSpan(0, 0, 0, 0, 500));

                //if the attack frame has passed then allow the attacker to move
                if (attacker.Frame >= FrameCount - 1)
                {
                    attacker.isAttackAnimDone = true;
                    isAttackAnimDone = true;
                }

                if (!isAttackAnimDone)
                    attacker.isAttackAnimDone = false;

                yield return false;
            }

            while (particleEmitterIce.ActiveParticleCount > 0)
                yield return false;

            #endregion Generate Freeze

            particleEmitterIce.Remove();
            l.RemoveSprite(particleEmitterIce);
            attacker.particleEmitters.Remove(particleEmitterIce);

            attacker.isAttackAnimDone = true;

            yield return true;
        }

        /// <summary>
        /// Performs Poison Vomit attack for Worm
        /// </summary>
        /// <param name="_target">The target for the projectile to attack</param>
        /// <param name="_attacker">The sprite initiating the attack</param>
        /// <param name="_baseDamage">The amount of damage to inflict before constant multiplier for weakness</param>
        /// <param name="_attackFrame">The frame that the attack begins on</param>
        /// <returns>Returns true when finished</returns>
        public static IEnumerable<bool> WormVomit(OurSprite attacker, int animation, int baseDamage, int attackFrame, Element elem = Element.Earth)
        {
            #region Variables
            Level l = This.Game.CurrentLevel;
            Vector2 initialDirection = attacker.Direction;
            attacker.State = SpriteState.Attacking;
            attacker.SetAnimation(animation);
            int FrameCount = attacker.FrameCount();
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;

            Effect particleEffect = l.GetEffect("ParticleSystem");
            Texture2D poison = l.GetTexture("poison");
            ParticleEmitter particleEmitter = new ParticleEmitter(10000, particleEffect, poison);
            particleEmitter.ZOrder = attacker.ZOrder - 1;
            particleEmitter.effectTechnique = "FadeAtXPercent";
            particleEmitter.fadeStartPercent = .98f;
            particleEmitter.blendState = BlendState.Additive;
            (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).Radius = 105;
            (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
            #endregion Variables

            attacker.isAttackAnimDone = false;
            attacker.Rewind();

            while (attacker.Frame < FrameCount && attacker.Frame != attackFrame)
            {
                attacker.State = SpriteState.Attacking;
                attacker.SetAnimation(animation);
                FrameCount = attacker.FrameCount();
                yield return false;
            }

            particleEmitter.GroundPos = attacker.GroundPos + new Vector2(7, -30);

            #region Generate Lightning Strike and Ground Spread and Deal Damage

            if (This.Game.AudioManager.PlaySoundEffect("Effects/Lightning_Strike"))
            {
                yield return false;
            }

            for (int i = 0; i < 100; i++)
            {
                //Deal Damage
                if (5 - i % 15 == 0 && Collision.CollisionData.Count > 0)
                {
                    List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                    Collision.CollisionData.TryGetValue(particleEmitter, out collidedWith);
                    if (collidedWith != null)
                    {
                        foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                        {
                            if (((detectedCollision.Item2 is Enemy) && (attacker is Player)) || ((detectedCollision.Item2 is Player) && (attacker is Enemy)))
                            {
                                Damage(attacker, (detectedCollision.Item2 as OurSprite), baseDamage);
                            }
                        }
                    }
                }

                yield return false;
            }

            #endregion Generate Lightning Strike and Ground Spread and Deal Damage
            while (particleEmitter.ActiveParticleCount > 0)
            {
                yield return false;
            }

            particleEmitter.Remove();
            l.RemoveSprite(particleEmitter);
            attacker.particleEmitters.Remove(particleEmitter);

            attacker.isAttackAnimDone = true;

            yield return true;
        }

        public static IEnumerable<bool> RetreatingAttack(Sprite target, Enemy attacker, float distance, TimeSpan duration, IEnumerable<bool> attack)
        {
            FrostbyteLevel l = This.Game.CurrentLevel as FrostbyteLevel;
            attacker.State = SpriteState.Attacking;

            attacker.isAttackAnimDone = false;
            List<Sprite> targets = new List<Sprite>();
            targets.Add(target);

            TimeSpan endRetreat = This.gameTime.TotalGameTime + duration;

            attacker.Personality.Status = EnemyStatus.Wander;

            while (true)
            {

                attacker.previousFootPos = attacker.GroundPos;
                if (attacker.retreat(targets, distance, 5) || endRetreat < This.gameTime.TotalGameTime)
                {
                    break;
                }

                attacker.checkBackgroundCollisions();
                yield return false;
            }

            foreach(bool o in attack)
            {
                yield return false;
            }

            attacker.isAttackAnimDone = true;
            yield return true;
        }

        public static IEnumerable<bool> RammingAttack(Sprite target, Enemy attacker, int baseDamage, TimeSpan duration, Element elem = Element.None)
        {
            FrostbyteLevel l = This.Game.CurrentLevel as FrostbyteLevel;
            attacker.State = SpriteState.Attacking;

            attacker.isAttackAnimDone = false;
            List<Sprite> targets = new List<Sprite>();
            targets.Add(target);


            int dmgcount = 0;
            attacker.Personality.Status = EnemyStatus.Wander;

            while (true)
            {

                attacker.previousFootPos = attacker.GroundPos;
                if (attacker.ram(targets, duration, float.PositiveInfinity, 20))
                {
                    break;
                }

                attacker.checkBackgroundCollisions();

                if (dmgcount++ % 10 == 0 && Collision.CollisionData.Count > 0)
                {
                    List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                    Collision.CollisionData.TryGetValue(attacker, out collidedWith);
                    if (collidedWith != null)
                    {
                        foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                        {
                            if (detectedCollision.Item2 is Player && attacker is Enemy)
                            {
                                Damage(attacker, (detectedCollision.Item2 as OurSprite), baseDamage);
                            }
                        }
                    }
                }
                yield return false;
            }

            attacker.isAttackAnimDone = true;
            yield return true;
        }

        /// <summary>
        /// Performs a Lightning strike between two sprites. Collides with the base of the target.
        /// </summary>
        /// <param name="_target">The target for the projectile to attack</param>
        /// <param name="_attacker">The sprite initiating the attack</param>
        /// <param name="_baseDamage">The amount of damage to inflict before constant multiplier for weakness</param>
        /// <param name="_attackFrame">The frame that the attack begins on</param>
        /// <returns>Returns true when finished</returns>
        public static IEnumerable<bool> LightningRod(Sprite target, OurSprite attacker, int baseDamage, int attackFrame, Element elem = Element.Lightning)
        {
            #region Variables
            Level l = This.Game.CurrentLevel;
            Vector2 initialDirection = attacker.Direction;
            setAnimation(attacker);
            int FrameCount = attacker.FrameCount();
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;

            Effect particleEffect = l.GetEffect("ParticleSystem");
            Texture2D lightning = l.GetTexture("sparkball");
            ParticleEmitter particleEmitter = new ParticleEmitter(10000, particleEffect, lightning);
            particleEmitter.ZOrder = int.MaxValue;
            particleEmitter.effectTechnique = "FadeAtXPercent";
            particleEmitter.fadeStartPercent = .5f;
            particleEmitter.blendState = BlendState.Additive;
            (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).Radius = 125;
            (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();

            Vector2 particleTopPosition;
            #endregion Variables

            attacker.isAttackAnimDone = false;
            attacker.Rewind();

            if (attacker is Frostbyte.Characters.Mage)
            {
                (attacker as Frostbyte.Characters.Mage).attackTier = 2;
            }

            #region Shoot Attack
            while (attacker.Frame < FrameCount)
            {
                attacker.isAttackAnimDone = false;

                if (target != null && target.GroundPos != attacker.GroundPos)
                    attacker.Direction = target.GroundPos - particleEmitter.GroundPos;
                attacker.State = SpriteState.Attacking;
                setAnimation(attacker);
                FrameCount = attacker.FrameCount();

                if (attacker.Frame == attackFrame)
                {
                    break;
                }

                yield return false;
            }
            #endregion Shoot Attack

            if (target != null)
            {
                particleEmitter.GroundPos = target.GroundPos;
                particleTopPosition = new Vector2(target.GroundPos.X, target.GroundPos.Y - 400);
            }
            else
            {
                particleEmitter.GroundPos = attacker.GroundPos + 300 * initialDirection;
                particleTopPosition = new Vector2(particleEmitter.GroundPos.X, particleEmitter.GroundPos.Y - 400);
            }


            #region Generate Lightning Strike and Ground Spread and Deal Damage

            bool isAttackAnimDone = false;

            if (This.Game.AudioManager.PlaySoundEffect("Effects/Lightning_Strike", .8f))
            {
                yield return false;
            }

            for (int i = 0; i < 165; i++)
            {
                particleTopPosition = attacker.GroundPos;

                // Lightning Strike
                if (i % 5 == 0)
                {
                    for (int j = 0; j < 200; j++)
                    {
                        Vector2 directionToTarget = particleEmitter.GroundPos - particleTopPosition;
                        directionToTarget.Normalize();
                        double directionAngle2 = This.Game.rand.NextDouble() * 2 * Math.PI;
                        Vector2 randDirection2 = new Vector2((float)Math.Cos(directionAngle2), (float)Math.Sin(directionAngle2));

                        particleTopPosition += directionToTarget * 2 + randDirection2 * 3;

                        particleEmitter.createParticles(randDirection2 * 10, -randDirection2 * 20, particleTopPosition, 8f, 600);
                    }
                }

                //Deal Damage
                if (i % 30 == 0 && Collision.CollisionData.Count > 0)
                {
                    List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                    Collision.CollisionData.TryGetValue(particleEmitter, out collidedWith);
                    if (collidedWith != null)
                    {
                        foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                        {
                            if (((detectedCollision.Item2 is Enemy) && (attacker is Player)) || ((detectedCollision.Item2 is Player) && (attacker is Enemy)))
                            {
                                Damage(attacker, (detectedCollision.Item2 as OurSprite), baseDamage);
                            }
                        }
                    }
                }

                yield return false;
            }

            #endregion Generate Lightning Strike and Ground Spread and Deal Damage
            while (particleEmitter.ActiveParticleCount > 0)
            {
                yield return false;
            }

            //if the attack frame has passed then allow the attacker to move
            if (attacker.Frame >= FrameCount - 1)
            {
                attacker.isAttackAnimDone = true;
                isAttackAnimDone = true;
            }

            if (!isAttackAnimDone)
                attacker.isAttackAnimDone = false;

            particleEmitter.Remove();
            l.RemoveSprite(particleEmitter);
            attacker.particleEmitters.Remove(particleEmitter);

            attacker.isAttackAnimDone = true;

            yield return true;
        }

        /// <summary>
        /// Performs a Lightning strike between two sprites. Collides with anything in the path of the bolt.
        /// </summary>
        /// <param name="_target">The target for the projectile to attack</param>
        /// <param name="_attacker">The sprite initiating the attack</param>
        /// <param name="_baseDamage">The amount of damage to inflict before constant multiplier for weakness</param>
        /// <param name="_attackFrame">The frame that the attack begins on</param>
        /// <returns>Returns true when finished</returns>
        public static IEnumerable<bool> LightningSpan(Sprite target, OurSprite attacker,
            int baseDamage, int attackFrame, Element elem=Element.Lightning,
            AttackRotation rotation=AttackRotation.None)
        {
            #region Variables
            Level l = This.Game.CurrentLevel;
            Vector2 initialDirection = attacker.Direction;
            setAnimation(attacker);
            int FrameCount = attacker.FrameCount();
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;

            Effect particleEffect = l.GetEffect("ParticleSystem");
            Texture2D lightning = l.GetTexture("sparkball");
            ParticleEmitter particleEmitter = new ParticleEmitter(10000, particleEffect, lightning);
            particleEmitter.ZOrder = int.MaxValue;
            particleEmitter.effectTechnique = "FadeAtXPercent";
            particleEmitter.fadeStartPercent = .5f;
            particleEmitter.blendState = BlendState.Additive;
            particleEmitter.collisionObjects.Clear();
            particleEmitter.GroundPos = target.CenterPos;

            #region Create Lightning Collision Objects
            int collisionRadius = 20;
            Vector2 spanDirection = attacker.CenterPos - particleEmitter.GroundPos;
            spanDirection.Normalize();
            for (int x = 0; x < Vector2.Distance(particleEmitter.GroundPos, attacker.CenterPos); x += collisionRadius * 2)
            {
                particleEmitter.collisionObjects.Add(new Collision_BoundingCircle(x, x * spanDirection, 1.5f * collisionRadius));
            }
            #endregion

            Vector2 particleTopPosition;
            #endregion Variables

            attacker.isAttackAnimDone = false;
            attacker.Rewind();

            if (attacker is Frostbyte.Characters.Mage)
            {
                (attacker as Frostbyte.Characters.Mage).attackTier = 2;
            }

            #region Shoot Attack
            while (attacker.Frame < FrameCount)
            {
                attacker.isAttackAnimDone = false;

                if (target != null && target.CenterPos != attacker.CenterPos)
                    attacker.Direction = target.CenterPos - particleEmitter.CenterPos;
                attacker.State = SpriteState.Attacking;
                setAnimation(attacker);
                FrameCount = attacker.FrameCount();

                if (attacker.Frame == attackFrame)
                {
                    break;
                }

                yield return false;
            }
            #endregion Shoot Attack

            if (target != null)
            {
                particleEmitter.GroundPos = target.CenterPos;
                particleTopPosition = new Vector2(target.CenterPos.X, target.CenterPos.Y - 400);
            }
            else
            {
                particleEmitter.GroundPos = attacker.CenterPos + 300 * initialDirection;
                particleTopPosition = new Vector2(particleEmitter.CenterPos.X, particleEmitter.CenterPos.Y - 400);
            }


            #region Generate Lightning Strike and Deal Damage

            bool isAttackAnimDone = false;
            if (This.Game.AudioManager.PlaySoundEffect("Effects/Lightning_Strike", .8f))
            {
                yield return false;
            }

            for (int i = 0; i < 165; i++)
            {
                particleTopPosition = attacker.CenterPos;

                if (rotation != AttackRotation.None)
                {
                    #region Rotate Span
                    float rotateRadius = Vector2.Distance(particleEmitter.GroundPos, particleTopPosition);

                    double angle = Math.Atan2(spanDirection.Y, spanDirection.X);
                    double angleOffset = Math.PI / 2 / 165;
                    if (rotation == AttackRotation.Clockwise)
                    {
                        angle += angleOffset;
                    }
                    else if (rotation == AttackRotation.CounterClockwise)
                    {
                        angle -= angleOffset;
                    }

                    angle = ((angle + 2 * Math.PI) % (2 * Math.PI)) - Math.PI;  // Ensure it's between 0 and 2PI

                    particleEmitter.collisionObjects.Clear();
                    particleEmitter.GroundPos = attacker.CenterPos + 
                        rotateRadius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                    spanDirection = attacker.CenterPos - particleEmitter.GroundPos;
                    spanDirection.Normalize();
                    for (int x = 0; x < rotateRadius; x += collisionRadius * 2)
                    {
                        particleEmitter.collisionObjects.Add(new Collision_BoundingCircle(x, x * spanDirection, 1.5f * collisionRadius));
                    }
                    #endregion Rotate Span
                }

                // Lightning Strike
                if (i % 13 == 0)
                {
                    for (int j = 0; j < 200; j++)
                    {
                        Vector2 directionToTarget = particleEmitter.GroundPos - particleTopPosition;
                        directionToTarget.Normalize();
                        double directionAngle2 = This.Game.rand.NextDouble() * 2 * Math.PI;
                        Vector2 randDirection2 = new Vector2((float)Math.Cos(directionAngle2), (float)Math.Sin(directionAngle2));

                        particleTopPosition += directionToTarget * 2 + randDirection2 * 3;

                        particleEmitter.createParticles(randDirection2 * 10, -randDirection2 * 20, particleTopPosition, 8f, 600);
                    }
                }

                //Deal Damage
                if (i % 20 == 0 && Collision.CollisionData.Count > 0)
                {
                    List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                    Collision.CollisionData.TryGetValue(particleEmitter, out collidedWith);
                    if (collidedWith != null)
                    {
                        HashSet<WorldObject> hitObjects = new HashSet<WorldObject>();
                        foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                        {
                            if (((detectedCollision.Item2 is Enemy) && (attacker is Player)) || ((detectedCollision.Item2 is Player) && (attacker is Enemy)))
                            {
                                hitObjects.Add(detectedCollision.Item2);
                            }
                        }
                        foreach (WorldObject obj in hitObjects)
                        {
                            Damage(attacker, (obj as OurSprite), baseDamage);
                        }
                    }
                }

                yield return false;
            }

            #endregion Generate Lightning Strike and Deal Damage
            while (particleEmitter.ActiveParticleCount > 0)
            {
                #region Rotate Span
                float rotateRadius = Vector2.Distance(particleEmitter.GroundPos, particleTopPosition);

                double angle = Math.Atan2(spanDirection.Y, spanDirection.X);
                double angleOffset = Math.PI / 2 / 165;
                if (rotation == AttackRotation.Clockwise)
                {
                    angle += angleOffset;
                }
                else if (rotation == AttackRotation.CounterClockwise)
                {
                    angle -= angleOffset;
                }

                angle = ((angle + 2 * Math.PI) % (2 * Math.PI)) - Math.PI;  // Ensure it's between 0 and 2PI

                particleEmitter.collisionObjects.Clear();
                particleEmitter.GroundPos = attacker.CenterPos +
                    rotateRadius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                spanDirection = attacker.CenterPos - particleEmitter.GroundPos;
                spanDirection.Normalize();
                for (int x = 0; x < rotateRadius; x += collisionRadius * 2)
                {
                    particleEmitter.collisionObjects.Add(new Collision_BoundingCircle(x, x * spanDirection, 1.5f * collisionRadius));
                }
                #endregion Rotate Span
                yield return false;
            }

            //if the attack frame has passed then allow the attacker to move
            if (attacker.Frame >= FrameCount - 1)
            {
                attacker.isAttackAnimDone = true;
                isAttackAnimDone = true;
            }

            if (!isAttackAnimDone)
                attacker.isAttackAnimDone = false;

            particleEmitter.Remove();
            l.RemoveSprite(particleEmitter);
            attacker.particleEmitters.Remove(particleEmitter);

            attacker.isAttackAnimDone = true;

            yield return true;
        }

        /// <summary>
        /// Performs Magic Tier 1 Attack
        /// </summary>
        /// <param name="_target">The target for the projectile to attack</param>
        /// <param name="_attacker">The sprite initiating the attack</param>
        /// <param name="_baseDamage">The amount of damage to inflict before constant multiplier for weakness</param>
        /// <param name="_attackFrame">The frame that the attack begins on</param>
        /// <param name="_attackEndTime">The time at which the magic attack should timeout</param>
        /// <param name="_attackRange">The distance from the target that the projectile must come within to be considered a hit</param>
        /// <param name="_projectileSpeed">The speed of the projectile</param>
        /// <returns>Returns true when finished</returns>
        public static IEnumerable<bool> LightningProjectile(Sprite _target, OurSprite attacker,
            int baseDamage, int attackFrame, TimeSpan attackEndTime, int attackRange,
            float projectileSpeed, bool isHoming, CreateParticles createParticles,
            ParticleEmitter _particleEmitter, Vector2 spawnOffset, Element elem = Element.Lightning)
        {
            #region Variables
            Level l = This.Game.CurrentLevel;
            OurSprite target = (OurSprite)_target;
            Vector2 initialDirection = attacker.Direction;
            int FrameCount = attacker.FrameCount();
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;
            Vector2 direction = new Vector2();
            Tuple<Vector2, Vector2> closestObject = new Tuple<Vector2, Vector2>(new Vector2(), new Vector2());
            Vector2 closestIntersection = new Vector2();
            ParticleEmitter particleEmitter = _particleEmitter;

            bool damageDealt = false;
            bool isLoopOne = true;
            #endregion Variables

            particleEmitter.GroundPos = attacker.GroundPos + spawnOffset;

            attacker.Rewind();

            attacker.isAttackAnimDone = false;

            if (attacker is Frostbyte.Characters.Mage)
            {
                (attacker as Frostbyte.Characters.Mage).attackTier = 1;
            }

            #region Shoot Tier 1 at attackFrame
            while (attacker.Frame < FrameCount)
            {
                attacker.isAttackAnimDone = false;

                FrameCount = attacker.FrameCount();

                if (attacker.Frame == attackFrame)
                {
                    if (target == null)
                    {
                        direction = initialDirection;
                    }
                    else
                    {
                        direction = attacker.Direction;
                    }
                    direction.Normalize();
                    attackStartTime = This.gameTime.TotalGameTime;
                    break;
                }

                yield return false;
            }
            #endregion Shoot Tier 1 at attackFrame

            #region Emit Particles until particle hits target or wall or time to live runs out

            bool isAttackAnimDone = false;

            while ((This.gameTime.TotalGameTime - attackStartTime) < attackEndTime)
            {

                if (isHoming && target != null)
                {
                    direction = target.GroundPos - particleEmitter.GroundPos;
                    direction.Normalize();
                }

                if (Collision.CollisionData.Count > 0)
                {
                    List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                    Collision.CollisionData.TryGetValue(particleEmitter, out collidedWith);
                    if (collidedWith != null)
                    {
                        foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                        {
                            if (((detectedCollision.Item2 is Enemy) && (attacker is Player)) || ((detectedCollision.Item2 is Player) && (attacker is Enemy)))
                            {
                                Damage(attacker, (detectedCollision.Item2 as OurSprite), baseDamage);
                                damageDealt = true;
                                break;
                            }
                            else if ((detectedCollision.Item2 is Player) && (attacker is Player) && (attacker as Player).currentTarget == detectedCollision.Item2)
                            {
                                Player p = (detectedCollision.Item2 as Player);
                                p.AddStatusEffect(new ElementalBuff(elem));
                                damageDealt = true;
                                break;
                            }
                        }
                    }
                }

                if (damageDealt)
                {
                    break;
                }

                //if the attack frame has passed then allow the attacker to move
                if (attacker.Frame >= FrameCount - 1)
                {
                    attacker.isAttackAnimDone = true;
                    isLoopOne = false;
                    isAttackAnimDone = true;
                }

                if (!isAttackAnimDone)
                    attacker.isAttackAnimDone = false;

                //make sure magic cannot go through walls
                Vector2 previousPosition = particleEmitter.GroundPos;
                particleEmitter.GroundPos += direction * projectileSpeed;
                attacker.detectBackgroundCollisions(particleEmitter.GroundPos, previousPosition, out closestObject, out closestIntersection);
                if (Vector2.DistanceSquared(previousPosition, closestIntersection) <= Vector2.DistanceSquared(previousPosition, particleEmitter.GroundPos))
                {
                    break;
                }

                createParticles(attacker, direction, projectileSpeed, particleEmitter);

                yield return false;
            }
            #endregion Emit Particles until particle hits target or wall or time to live runs out

            //if the attack frame has passed then allow the attacker to move
            while (attacker.Frame < FrameCount - 1 && isLoopOne)
            {
                attacker.isAttackAnimDone = false;
                yield return false;
            }

            attacker.isAttackAnimDone = true;

            #region Finish attacking after all particles are dead
            while (particleEmitter.ActiveParticleCount > 0)
            {
                yield return false;
            }
            #endregion Finish attacking after all particles are dead

            particleEmitter.Remove();
            l.RemoveSprite(particleEmitter);
            attacker.particleEmitters.Remove(particleEmitter);

            yield return true;
        }

        public static IEnumerable<bool> WorthlessCodeDuplication(Vector2 direction, OurSprite attacker, int baseDamage, int attackFrame, TimeSpan attackEndTime, int attackRange, float projectileSpeed, bool isHoming, CreateParticles createParticles, ParticleEmitter _particleEmitter, Vector2 spawnOffset, Element elem = Element.Normal)
        {
            #region Variables
            Level l = This.Game.CurrentLevel;
            Vector2 initialDirection = direction;
            attacker.State = SpriteState.Attacking;
            setAnimation(attacker);
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;
            Tuple<Vector2, Vector2> closestObject = new Tuple<Vector2, Vector2>(new Vector2(), new Vector2());
            ParticleEmitter particleEmitter = _particleEmitter;

            bool damageDealt = false;
            bool isLoopOne = true;
            #endregion Variables

            particleEmitter.GroundPos = attacker.GroundPos + spawnOffset;

            attacker.Rewind();

            attacker.isAttackAnimDone = false;

            if (attacker is Frostbyte.Characters.Mage)
            {
                (attacker as Frostbyte.Characters.Mage).attackTier = 1;
            }

            #region Shoot Tier 1 at attackFrame
            while (attacker.Frame < attacker.FrameCount())
            {
                attacker.isAttackAnimDone = false;
                attacker.Direction = direction;
                attacker.State = SpriteState.Attacking;
                setAnimation(attacker);

                if (attacker.Frame == attackFrame)
                {
                    attackStartTime = This.gameTime.TotalGameTime;
                    break;
                }

                yield return false;
            }
            #endregion Shoot Tier 1 at attackFrame

            #region Emit Particles until particle hits target or wall or time to live runs out

            bool isAttackAnimDone = false;

            while (attackEndTime > TimeSpan.Zero)
            {
                attackEndTime -= This.gameTime.ElapsedGameTime;

                if (Collision.CollisionData.Count > 0)
                {
                    List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                    Collision.CollisionData.TryGetValue(particleEmitter, out collidedWith);
                    if (collidedWith != null)
                    {
                        foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                        {
                            if (((detectedCollision.Item2 is Enemy) && (attacker is Player)) || ((detectedCollision.Item2 is Player) && (attacker is Enemy)))
                            {
                                Damage(attacker, (detectedCollision.Item2 as OurSprite), baseDamage);
                                damageDealt = true;
                                break;
                            }
                            else if (This.Cheats.GetCheat("ElementalBuffs").Enabled && (detectedCollision.Item2 is Player) && (attacker is Player) && (attacker as Player).currentTarget == detectedCollision.Item2)
                            {
                                Player p = (detectedCollision.Item2 as Player);
                                p.AddStatusEffect(new ElementalBuff(elem));
                                damageDealt = true;
                                break;
                            }
                        }
                    }
                }

                if (damageDealt)
                {
                    break;
                }

                //if the attack frame has passed then allow the attacker to move
                if (attacker.Frame >= attacker.FrameCount() - 1)
                {
                    attacker.isAttackAnimDone = true;
                    isLoopOne = false;
                    isAttackAnimDone = true;
                }

                if (!isAttackAnimDone)
                    attacker.isAttackAnimDone = false;

                //move particle
                Vector2 previousPosition = particleEmitter.GroundPos;
                particleEmitter.GroundPos += direction * projectileSpeed;

                //make sure magic cannot go through walls
                if (previousPosition != particleEmitter.GroundPos)
                {
                    bool hasCollided = false;
                    float collisionRadius = particleEmitter.GroundPosRadius;
                    Tuple<int, int> topLeftMostTile = new Tuple<int, int>((int)Math.Floor(((particleEmitter.GroundPos.X - collisionRadius) / This.CellSize)),   //top left most tile that could possible hit sprite
                                                                    (int)Math.Floor(((particleEmitter.GroundPos.Y - collisionRadius)) / This.CellSize));
                    Tuple<int, int> bottomRightMostTile = new Tuple<int, int>((int)Math.Floor((particleEmitter.GroundPos.X + collisionRadius) / This.CellSize), //bottom right most tile that could possible hit sprite
                                                                            (int)Math.Floor((particleEmitter.GroundPos.Y + collisionRadius) / This.CellSize));
                    TileList tileMap = (This.Game.CurrentLevel as FrostbyteLevel).TileMap;
                    for (int x = topLeftMostTile.Item1; x <= bottomRightMostTile.Item1; x++)
                        for (int y = topLeftMostTile.Item2; y <= bottomRightMostTile.Item2; y++)
                        {
                            Tile tile;
                            tileMap.TryGetValue(x, y, out tile);

                            if (tile.Type == TileTypes.Floor)
                                continue;

                            if ((tile.Type == TileTypes.Bottom || tile.Type == TileTypes.BottomConvexCorner) && !tileCircleCollision(new Vector2(x * 64, y * 64 + 32), new Vector2(x * 64 + 64, y * 64 + 64), particleEmitter.GroundPos, collisionRadius))
                            {
                                continue;
                            }
                            else if ((tile.Type == TileTypes.Wall || tile.Type == TileTypes.ConvexCorner) && !tileCircleCollision(new Vector2(x * 64, y * 64), new Vector2(x * 64 + 64, y * 64 + 32), particleEmitter.GroundPos, collisionRadius))
                            {
                                continue;
                            }
                            else if (!tileCircleCollision(new Vector2(x * 64, y * 64), new Vector2(x * 64 + 64, y * 64 + 64), particleEmitter.GroundPos, collisionRadius))
                            {

                                continue;
                            }

                            //bool isBelowHalfWay = (particleEmitter.GroundPos.Y - collisionRadius - y * 64) > 32;
                            //if ((tile.Type == TileTypes.Wall && direction.Y < 0f && isBelowHalfWay) || (tile.Type != TileTypes.Wall && tile.Type != TileTypes.ConvexCorner)
                            //|| (tile.Type == TileTypes.ConvexCorner && direction.Y < 0f && isBelowHalfWay && closestObject.Item1.Y == closestObject.Item2.Y))
                            //{
                            hasCollided = true;
                            //}
                        }
                    if (hasCollided)
                        break;
                }

                createParticles(attacker, direction, projectileSpeed, particleEmitter);

                yield return false;
            }
            #endregion Emit Particles until particle hits target or wall or time to live runs out

            //if the attack frame has passed then allow the attacker to move
            while (attacker.Frame < attacker.FrameCount() - 1 && isLoopOne)
            {
                attacker.isAttackAnimDone = false;
                yield return false;
            }

            attacker.isAttackAnimDone = true;

            #region Finish attacking after all particles are dead
            while (particleEmitter.ActiveParticleCount > 0)
            {
                yield return false;
            }
            #endregion Finish attacking after all particles are dead

            particleEmitter.Remove();
            l.RemoveSprite(particleEmitter);
            attacker.particleEmitters.Remove(particleEmitter);

            yield return true;
        }

    }
}
