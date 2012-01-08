using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Frostbyte.Enemies
{
    internal partial class FrostWolf : Frostbyte.Enemy
    {

        #region Variables
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<String> Animations = new List<String>(){
           "wolf-idle-down.anim",
           "wolf-idle-diagdown.anim",
           "wolf-idle-right.anim",
           "wolf-idle-diagup.anim",
           "wolf-idle-up.anim",
           "wolf-walk-down.anim",
           "wolf-walk-diagdown.anim",
           "wolf-walk-right.anim",
           "wolf-walk-diagup.anim",
           "wolf-walk-up.anim",
           "wolf-attack-down.anim",
           "wolf-attack-diagdown.anim",
           "wolf-attack-right.anim",
           "wolf-attack-diagup.anim",
           "wolf-attack-up.anim",
        };

        #endregion Variables

        public FrostWolf(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 1f, 75)
        {
            SpawnPoint = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new ChargePersonality(this);
            ElementType = Element.Water;

            This.Game.AudioManager.AddSoundEffect("Effects/Wolf_Move");
            if (MovementAudioName == null)
            {
                MovementAudioName = "Effects/Wolf_Move";
                This.Game.AudioManager.InitializeLoopingSoundEffect(MovementAudioName);
            }

            isDieEffectEnabled = true;
        }

        protected override void updateMovement()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            Sprite target = GetClosestTarget(targets, float.MaxValue);
            if (target != null)
            {
                float theirRad = (target.GetCollision().FirstOrDefault() as Collision_BoundingCircle).Radius;
                float rad = (target.GetCollision().FirstOrDefault() as Collision_BoundingCircle).Radius;
                float attackRadius = (rad + theirRad) * .92f;
                if (Vector2.DistanceSquared(target.GroundPos, this.GroundPos) > attackRadius * attackRadius)
                {
                    Personality.Update();
                }
            }
        }

        protected override void updateAttack()
        {
            if (This.gameTime.TotalGameTime >= attackStartTime + new TimeSpan(0, 0, 2) && isAttackAnimDone)
            {
                List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                Collision.CollisionData.TryGetValue(this, out collidedWith);
                if (collidedWith != null)
                {
                    foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                    {
                        if (detectedCollision.Item2 is Player)
                        {
                            mAttacks.Add(Attacks.Melee(this, 5, 18).GetEnumerator());
                            attackStartTime = This.gameTime.TotalGameTime;
                            break;
                        }
                    }
                }
            }
        }

    }
}

