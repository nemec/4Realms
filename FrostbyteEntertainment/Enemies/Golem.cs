using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Frostbyte.Enemies
{
    internal partial class Golem : Frostbyte.Enemy
    {
        #region Variables
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<String> Animations = new List<String>(){
               "golem-idle-down.anim",
           "golem-idle-diagdown.anim",
           "golem-idle-right.anim",
           "golem-idle-diagup.anim",
           "golem-idle-up.anim",
           "golem-walk-down.anim",
           "golem-walk-diagdown.anim",
           "golem-walk-right.anim",
           "golem-walk-diagup.anim",
           "golem-walk-up.anim",
           "golem-attack-down.anim",
           "golem-attack-diagdown.anim",
           "golem-attack-right.anim",
           "golem-attack-diagup.anim",
           "golem-attack-up.anim",
        };

        #endregion Variables

        public Golem(string name, Vector2 initialPos, int health, List<String> anims = null)
            : base(name, new Actor(anims == null ? Animations : anims), 1, health)
        {
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new SentinelPersonality(this);
            ElementType = Element.Normal;
            SpawnPoint = initialPos;
            This.Game.AudioManager.AddSoundEffect("Effects/Golem_Attack");
            This.Game.AudioManager.AddSoundEffect("Effects/Golem_Move");
            MovementAudioName = "Effects/Golem_Move";
            This.Game.AudioManager.InitializeLoopingSoundEffect(MovementAudioName);

            isDieEffectEnabled = true;
        }


        protected override void updateMovement()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            Sprite target = GetClosestTarget(targets, float.MaxValue);
            if (target != null && target.GetCollision().Count > 0 && GetCollision().Count > 0)
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
                            mAttacks.Add(Attacks.Melee(this, 15, 18).GetEnumerator());
                            attackStartTime = This.gameTime.TotalGameTime;
                            This.Game.AudioManager.PlaySoundEffect("Effects/Golem_Attack");
                            break;
                        }
                    }
                }
            }
        }
    }
}

