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
    internal partial class FireAnt : Frostbyte.Enemy
    {
        #region Variables
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<String> Animations = new List<String>(){
           "ant-idle-down.anim",
           "ant-idle-diagdown.anim",
           "ant-idle-right.anim",
           "ant-idle-diagup.anim",
           "ant-idle-up.anim",
           "ant-walk-down.anim",
           "ant-walk-diagdown.anim",
           "ant-walk-right.anim",
           "ant-walk-diagup.anim",
           "ant-walk-up.anim",
           "ant-attack-down.anim",
           "ant-attack-diagdown.anim",
           "ant-attack-right.anim",
           "ant-attack-diagup.anim",
           "ant-attack-up.anim",
        };
        #endregion Variables

        public FireAnt(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 0.8f, 60)
        {
            SpawnPoint = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new ChargePersonality(this);
            This.Game.AudioManager.AddSoundEffect("Effects/Spider_Move");
            if (MovementAudioName == null)
            {
                MovementAudioName = "Effects/Spider_Move";
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
                            mAttacks.Add(Attacks.Melee(this, 5, 10).GetEnumerator());
                            attackStartTime = This.gameTime.TotalGameTime;
                            break;
                        }
                    }
                }
            }
        }
    }
}
