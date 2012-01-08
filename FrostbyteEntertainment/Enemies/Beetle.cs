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
    internal partial class Beetle : Frostbyte.Enemy
    {
        #region Variables
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<String> Animations = new List<String>(){
           "beetle-idle-down.anim",
           "beetle-idle-diagdown.anim",
           "beetle-idle-right.anim",
           "beetle-idle-diagup.anim",
           "beetle-idle-up.anim",
           "beetle-walk-down.anim",
           "beetle-walk-diagdown.anim",
           "beetle-walk-right.anim",
           "beetle-walk-diagup.anim",
           "beetle-walk-up.anim",
           "beetle-attack-down.anim",
           "beetle-attack-diagdown.anim",
           "beetle-attack-right.anim",
           "beetle-attack-diagup.anim",
           "beetle-attack-up.anim",
        };
        #endregion Variables

        public Beetle(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 1, 50)
        {
            SpawnPoint = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new ChargePersonality(this);
            This.Game.AudioManager.AddSoundEffect("Effects/Beetle_Move");

            if (MovementAudioName == null)
            {
                MovementAudioName = "Effects/Beetle_Move";
                This.Game.AudioManager.InitializeLoopingSoundEffect(MovementAudioName);
            }

            isDieEffectEnabled = true;
        }

        protected override void updateMovement()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            Sprite target = GetClosestTarget(targets, float.MaxValue);
            if (target != null && target.GetCollision().Count>0 && GetCollision().Count>0)
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
