using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Frostbyte.Enemies
{
    internal partial class Spider : Frostbyte.Enemy
    {

        #region Variables
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<String> Animations = new List<String>(){
           "spider-idle-down.anim",
           "spider-idle-diagdown.anim",
           "spider-idle-right.anim",
           "spider-idle-diagup.anim",
           "spider-idle-up.anim",
           "spider-walk-down.anim",
           "spider-walk-diagdown.anim",
           "spider-walk-right.anim",
           "spider-walk-diagup.anim",
           "spider-walk-up.anim",
           "spider-attack-down.anim",
           "spider-attack-diagdown.anim",
           "spider-attack-right.anim",
           "spider-attack-diagup.anim",
           "spider-attack-up.anim",
        };

        #endregion Variables

        public Spider(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 1, 50)
        {
            SpawnPoint = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new PulseChargePersonality(this);
            ElementType = Element.Normal;
            Scale = 1.0f;

            This.Game.AudioManager.AddSoundEffect("Effects/Spider_Move");
            if (MovementAudioName == null)
            {
                MovementAudioName = "Effects/Spider_Move";
                This.Game.AudioManager.InitializeLoopingSoundEffect(MovementAudioName, .04f);
            }

            isDieEffectEnabled = true;
        }

        protected override void updateMovement()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            Sprite target = GetClosestTarget(targets, float.MaxValue);
            if (target != null)
            {
                float attackRadius = ((target.GetCollision()[0] as Collision_BoundingCircle).Radius + (this.GetCollision()[0] as Collision_BoundingCircle).Radius) * .92f;
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
                            mAttacks.Add(Attacks.Melee(this, 5, 9).GetEnumerator());
                            attackStartTime = This.gameTime.TotalGameTime;
                            break;
                        }
                    }
                }
            }
        }

    }
}
