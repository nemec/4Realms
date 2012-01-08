using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    internal enum EnemyStatus
    {
        Wander,
        Ram,
        Charge,
        Stealth,
        Frozen,
        Attack
    }

    internal interface IPersonality
    {
        EnemyStatus Status { get; set; }
        void Update();
    }

    internal class ImmobilePersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        public void Update()
        {
            // Do Nothing
        }
    }

    internal class WanderingMinstrelPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal WanderingMinstrelPersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            float[] transitionDistances = new float[1] { 50f };
            while (true)
            {
                while (!EnemyAI.wander(master, targets, TimeSpan.MaxValue, transitionDistances[0], (float)Math.PI / 8))
                {
                    yield return null;
                }
                while (!EnemyAI.camp(master, targets, 0f, transitionDistances[0]))
                {
                    yield return null;
                }

                yield return null;
            }
        }
    }

    internal class PseudoWanderPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal PseudoWanderPersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            float[] transitionDistances = new float[1] { 0f };
            while (true)
            {
                Sprite closestTarget = master.GetClosestTarget(targets, 500.0f);
                
                if(closestTarget != null && Vector2.DistanceSquared(master.GroundPos, closestTarget.GroundPos) <= 500*500)
                {
                    EnemyAI.rangeWander(master, targets, TimeSpan.MaxValue, (float)Math.PI / 8, 400f);
                }

                yield return null;
            }
        }
    }

    internal class SwoopingPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal SwoopingPersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            float[] transitionDistances = new float[1] { 0f };
            while (true)
            {
                Sprite closestTarget = master.GetClosestTarget(targets, 500.0f);

                if (closestTarget != null && Vector2.DistanceSquared(master.GroundPos, closestTarget.GroundPos) <= 500 * 500)
                {
                    EnemyAI.wideRangeWander(master, targets, TimeSpan.MaxValue, (float)Math.PI / 4, 400f);
                }

                yield return null;
            }
        }
    }

    internal class AmbushPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal AmbushPersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            float[] distances = new float[3] { 1000f, 150f, 100f };
            while (true)
            {
                while (!master.stealthCharge(targets, TimeSpan.MaxValue, distances[1], distances[0], 1f))
                {
                    yield return null;
                }
                while (!master.stealthCamp(targets, distances[2], distances[1]))
                {
                    yield return null;
                }
                while (!master.charge(targets, distances[2], 2))
                {
                    yield return null;
                }

                yield return null;
            }
        }
    }

    internal class SentinelPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal SentinelPersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            Vector2 guardPosition = master.GroundPos;
            while (true)
            {
                Sprite closestTarget = master.GetClosestTarget(targets, 300.0f);


                if (closestTarget != null && Vector2.DistanceSquared(closestTarget.GroundPos, master.GroundPos) > 200 * 200)
                {
                    master.charge(closestTarget.GroundPos, 1.4f);
                }
                else if (closestTarget != null)
                {
                    master.charge(closestTarget.GroundPos, 3.0f);
                }
                else if (closestTarget == null && Vector2.DistanceSquared(guardPosition, master.GroundPos) > 20 * 20)
                {
                    master.charge(guardPosition, 3.0f);
                }
                else if (closestTarget == null && Vector2.DistanceSquared(guardPosition, master.GroundPos) <= 20 * 20)
                {
                    Sprite closestTarget2 = master.GetClosestTarget(targets, 800.0f);
                    master.State = SpriteState.Idle;
                    if (closestTarget2 != null)
                        master.Direction = closestTarget2.GroundPos - master.GroundPos;
                }

                yield return null;
            }
        }
    }

    internal class StrictSentinelPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal StrictSentinelPersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            Vector2 guardPosition = master.GroundPos;
            while (true)
            {
                if (Vector2.DistanceSquared(guardPosition, master.GroundPos) <= 20 * 20)
                {
                    Sprite closestTarget2 = master.GetClosestTarget(targets, 800.0f);
                    master.State = SpriteState.Idle;
                    if (closestTarget2 != null)
                        master.Direction = closestTarget2.GroundPos - master.GroundPos;
                }

                yield return null;
            }
        }
    }

    internal class WalkingSentinelPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal WalkingSentinelPersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            Vector2 guardPosition = master.GroundPos;
            while (true)
            {
                Sprite closestTarget = master.GetClosestTarget(targets, 550.0f);


                if (closestTarget != null && Vector2.DistanceSquared(closestTarget.GroundPos, master.GroundPos) > 100 * 100)
                {
                    master.charge(closestTarget.GroundPos, 1.3f);
                }
                else if (closestTarget == null && Vector2.DistanceSquared(guardPosition, master.GroundPos) > 20 * 20)
                {
                    master.charge(guardPosition, 3.0f);
                }
                else if (closestTarget == null && Vector2.DistanceSquared(guardPosition, master.GroundPos) <= 20 * 20)
                {
                    Sprite closestTarget2 = master.GetClosestTarget(targets, 800.0f);
                    master.State = SpriteState.Idle;
                    if (closestTarget2 != null)
                        master.Direction = closestTarget2.GroundPos - master.GroundPos;
                }

                yield return null;
            }
        }
    }

    internal class DartPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal DartPersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            while (true)
            {
                TimeSpan snapshot = This.gameTime.TotalGameTime;
                //master.Personality.Status = EnemyStatus.Wander;
                Sprite closestTarget = this.master.GetClosestTarget(targets);
                while (!master.dart(targets, 5.0f, 400, new TimeSpan(0, 0, 0, 0, 300)) && closestTarget != null &&
                    Vector2.Distance(this.master.GroundPos, closestTarget.GroundPos) < 500)
                {
                    yield return null;
                    closestTarget = this.master.GetClosestTarget(targets);
                }

                // Freeze for five seconds
                while (!master.freeze(new TimeSpan(0, 0, 0, 0, 300)))
                {
                    yield return null;
                }

                yield return null;
            }

            //}
        }
    }

    internal class DartWanderPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal DartWanderPersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            while (true)
            {
                TimeSpan snapshot = This.gameTime.TotalGameTime;
                //master.Personality.Status = EnemyStatus.Wander;
                Sprite closestTarget = this.master.GetClosestTarget(targets);
                while (!master.dartingBat(targets, 5.0f, 400, new TimeSpan(0, 0, 0, 0, 900)) && closestTarget != null &&
                    Vector2.Distance(this.master.GroundPos, closestTarget.GroundPos) < 500)
                {
                    yield return null;
                    closestTarget = this.master.GetClosestTarget(targets);
                }

                // Freeze for five seconds
                while (!master.wander(targets, new TimeSpan(0, 0, 0, 0, 300), float.MaxValue, (float)Math.PI/8))
                {
                    yield return null;
                }

                yield return null;
            }

            //}
        }
    }

    internal class PulseChargePersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal PulseChargePersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            while (true)
            {
                TimeSpan snapshot = This.gameTime.TotalGameTime;
                while (!master.pulseCharge(targets, 500, 3.2f))
                {
                    yield return null;
                }

                yield return null;
            }
        }
    }

    internal class ChargePersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal ChargePersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            while (true)
            {
                master.charge(targets, 500, 3.3f);
                yield return null;
            }
        }
    }

    #region Bosses
    internal class UndergroundAttackPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemies.Worm master;
        private IEnumerator mStates;

        internal UndergroundAttackPersonality(Enemies.Worm master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
            This.Game.AudioManager.AddSoundEffect("Effects/Worm_Spawn");
            This.Game.AudioManager.AddBackgroundMusic("Music/EarthBoss");
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable Surface()
        {
            This.Game.AudioManager.PlaySoundEffect("Effects/Worm_Spawn", .05f);
            master.Visible = true;
            master.SetAnimation(17);
            master.Rewind();
            while (master.Frame != master.FrameCount() - 1)
            {
                master.SetAnimation(17);
                yield return null;
            }
            master.SetAnimation(0);
            while (!master.freeze(new TimeSpan(0, 0, 1)))
            {
                yield return null;
            }
            master.HasVomited = false;
            master.IsSubmerged = false;
        }

        private IEnumerable Submerge()
        {
            master.IsSubmerged = true;
            if (master.HasVomited)
                master.StopAttacks();
            master.SetAnimation(16);
            master.Rewind();
            while (master.Frame != master.FrameCount() - 1)
            {
                master.SetAnimation(16);
                yield return null;
            }
            master.Visible = false;
            master.Pos = Vector2.Zero;
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;

            while (!master.camp(targets, 100, float.PositiveInfinity) && !master.AtArms)
            {
                yield return null;
            }
            master.setAtArms();
            This.Game.AudioManager.PlayBackgroundMusic("Music/EarthBoss");
            This.Game.AudioManager.BackgroundMusicVolume = 0.05f;

            foreach (Object o in Surface()) { yield return null; }

            while (true)
            {
                if (master.IsSubmerged)
                {
                    Camera cam = This.Game.CurrentLevel.Camera;
                    Viewport v = This.Game.GraphicsDevice.Viewport;
                    while (!master.delayedTeleport(new TimeSpan(0, 0, 2),
                        new Rectangle(
                            (int)(Math.Max(6792, cam.Pos.X * cam.Zoom)),
                            (int)(Math.Max(2076, cam.Pos.Y * cam.Zoom)),
                            (int)(v.Width * cam.Zoom),
                            (int)(v.Height * cam.Zoom))))
                    {
                        yield return null;
                    }
                    master.PreviousPos = master.Pos;
                    foreach (Object o in Surface()) { yield return null; }
                }
                else
                {
                    while (!master.freeze(new TimeSpan(0, 0, 5))) { yield return null; }

                    foreach (Object o in Submerge()) { yield return null; }
                }
                yield return null;
            }
        }
    }

    internal class ShiningPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemies.CrystalMan master;
        private IEnumerator mStates;
        private static Random rng = new Random();

        private int EMPTY_CRYSTAL { get { return 19; } }

        internal ShiningPersonality(Enemies.CrystalMan master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
            This.Game.AudioManager.AddBackgroundMusic("Music/CrystalBossBG");
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable TeleportIn()
        {
            master.currentCrystal.SetAnimation(5);
            master.currentCrystal.Rewind();
            while (master.currentCrystal.Frame != master.currentCrystal.FrameCount() - 1)
            {
                master.currentCrystal.SetAnimation(5);
                yield return null;
            }
            master.currentCrystal.Rewind();
            master.currentCrystal.SetAnimation(0);
            while (!master.freeze(new TimeSpan(0, 0, 1)))
            {
                yield return null;
            }
            master.attackWait = This.gameTime.TotalGameTime;
        }

        public IEnumerable TeleportOut()
        {
            master.attackWait = TimeSpan.MaxValue;
            master.currentCrystal.Rewind();
            master.currentCrystal.SetAnimation(6);
            while (master.currentCrystal.Frame != master.currentCrystal.FrameCount() - 1)
            {
                master.currentCrystal.SetAnimation(6);
                yield return null;
            }
            master.currentCrystal.Rewind();
            master.currentCrystal.SetAnimation(EMPTY_CRYSTAL);
            while (!master.freeze(new TimeSpan(0, 0, 1)))
            {
                yield return null;
            }
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            Random rng = new Random();

            while (master.Crystals == null || master.Crystals.Count == 0)
            {
                yield return null;
            }
            master.currentCrystal = master.Crystals[0];

            int initialWaitDistance = 300;
            while (!master.camp(targets, initialWaitDistance, float.PositiveInfinity) && !master.AtArms)
            {
                foreach (Enemies.Crystal crystal in master.Crystals)
                {
                    master.currentCrystal.Rewind();
                    crystal.SetAnimation(EMPTY_CRYSTAL);
                }
                yield return null;
            }
            master.setAtArms();
            This.Game.AudioManager.PlayBackgroundMusic("Music/CrystalBossBG");
            This.Game.AudioManager.BackgroundMusicVolume = 0.05f;

            while (true)
            {
                foreach (Object o in TeleportIn())
                {
                    if (master.currentCrystal.State == SpriteState.Dead)
                    {
                        break;
                    }
                    yield return null;
                }

                while (!master.freeze(new TimeSpan(0, 0, 5)))
                {
                    if (master.currentCrystal.State == SpriteState.Dead)
                    {
                        break;
                    }
                    yield return null;
                }

                foreach (Object o in TeleportOut())
                {
                    if (master.currentCrystal.State == SpriteState.Dead)
                    {
                        break;
                    }
                    yield return null;
                }

                if (master.currentCrystal.State == SpriteState.Dead)
                {
                    master.Crystals.Remove(master.currentCrystal);
                    master.OuterCrystals.Remove(master.currentCrystal);
                }

                if (master.Crystals.Count == 0)
                {
                    break;
                }

                master.currentCrystal = master.Crystals.GetRandomElement();

                yield return null;
            }

            master.Health = 0;
        }
    }

    internal class LiquidPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemies.WaterBlobBoss master;
        private IEnumerator mStates;
        private static Random rng = new Random();

        internal LiquidPersonality(Enemies.WaterBlobBoss master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
            This.Game.AudioManager.AddBackgroundMusic("Music/WaterBoss");
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;

            master.SetAnimation(18);

            int initialWaitDistance = 150;
            while (!master.camp(targets, initialWaitDistance, float.PositiveInfinity) && !master.AtArms)
            {
                yield return null;
            }
            //This.Game.AudioManager.AddSoundEffect("Effects/Splash");
            This.Game.AudioManager.PlaySoundEffect("Effects/Splash");
            master.setAtArms();
            master.attackWait = This.gameTime.TotalGameTime + new TimeSpan(0, 0, 3);

            This.Game.AudioManager.PlayBackgroundMusic("Music/WaterBoss");
            This.Game.AudioManager.BackgroundMusicVolume = 0.05f;

            Sprite spawn = new Sprite("spawn",
                new Actor(This.Game.CurrentLevel.GetAnimation("waterboss-surface.anim")));
            spawn.GroundPos = master.GroundPos + new Vector2(0, 100);

            while (spawn.Frame < spawn.FrameCount() - 1)
            {
                if (spawn.Frame == 16)
                {
                    master.SetAnimation(4);
                }
                yield return null;
            }
            This.Game.CurrentLevel.RemoveSprite(spawn);
            master.StartAnim();

            while (true)
            {
                while (!master.dart(targets, 15, 50, new TimeSpan(0, 0, 0, 0, 500)))
                {
                    yield return null;
                }
                while (!master.freeze(new TimeSpan(0, 0, 2)))
                {
                    yield return null;
                }
                

                yield return null;
            }
        }
    }

    internal class DarkLinkPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemies.FinalBoss master;
        private IEnumerator mStates;
        private static Random rng = new Random();

        internal DarkLinkPersonality(Enemies.FinalBoss master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;

            int initialWaitDistance = 300;
            while (!master.camp(targets, initialWaitDistance, float.PositiveInfinity) && !master.AtArms)
            {
                yield return null;
            }
            master.setAtArms();
            master.attackWait = This.gameTime.TotalGameTime + new TimeSpan(0, 0, 3);
            This.Game.AudioManager.PlayBackgroundMusic("Music/OldEarthBoss");
            This.Game.AudioManager.BackgroundMusicVolume = 0.05f;

            while (true)
            {
                while (!master.charge(targets, int.MaxValue, 1))
                {
                    int n = rng.Next(500);
                    if (n == 0)
                    {
                        while (!master.freeze(new TimeSpan(0, 0, rng.Next(1, 4))))
                        {
                            yield return null;
                        }
                    }
                    yield return null;
                }

                yield return null;
            }
        }
    }

    internal class LumberingPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemies.FireMan master;
        private IEnumerator mStates;
        private static Random rng = new Random();

        internal LumberingPersonality(Enemies.FireMan master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;

            int initialWaitDistance = 200;
            while (!master.camp(targets, initialWaitDistance, float.PositiveInfinity) && !master.AtArms)
            {
                yield return null;
            }
            master.setAtArms();
            master.attackWait = This.gameTime.TotalGameTime + new TimeSpan(0, 0, 3);

            This.Game.AudioManager.PlayBackgroundMusic("Music/FireBoss");
            This.Game.AudioManager.BackgroundMusicVolume = 0.05f;

            while (true)
            {
                while (!master.ram(targets, new TimeSpan(0, 0, 3), float.PositiveInfinity, 1))
                {
                    This.Game.AudioManager.InitializeLoopingSoundEffect("Effects/Golem_Walk");
                    yield return null;
                }
                if (rng.Next(5) == 0)
                {
                    while (!master.freeze(new TimeSpan(0, 0, rng.Next(3))))
                    {
                        yield return null;
                    }
                }

                yield return null;
            }
        }
    }

    #endregion

    internal static class EnemyAI
    {
        //These are only to update position of enemy

        private static Random RNG = new Random();
        private static Vector2 nextHoverPoint = Vector2.Zero;

        /// <summary>
        /// Update enemy position directly toward target for given duration - complete
        /// </summary>
        internal static bool charge(this Enemy ths, List<Sprite> targets, float stopDistance, float aggroDistance, float speedMultiplier)
        {
            Sprite min = ths.GetClosestTarget(targets, aggroDistance);

            if (min == null || Vector2.DistanceSquared(min.GroundPos, ths.GroundPos) < stopDistance * stopDistance)  // No targets, so just continue on
            {
                return true;
            }

            charge(ths, min.GroundPos, speedMultiplier);

            return false;
        }

        /// <summary>
        /// Update enemy position directly toward target for given duration - complete
        /// </summary>
        internal static bool charge(this Enemy ths, List<Sprite> targets, float aggroDistance, float speedMultiplier)
        {
            Sprite min = ths.GetClosestTarget(targets, aggroDistance);

            if (min == null)  // No targets, so just continue on
            {
                return true;
            }

            charge(ths, min.GroundPos, speedMultiplier);

            return false;
        }

        internal static void charge(this Enemy ths, Vector2 targetPos, float speedMultiplier = 1)
        {
            float chargeSpeed = ths.Speed * speedMultiplier;
            //if (Math.Abs((ths.Direction * chargeSpeed).X) < 1.5f || Math.Abs((ths.Direction * chargeSpeed).Y) < 1.5f)
            //{
                ths.GroundPos += ths.Direction * chargeSpeed;
                //This must be set after because determining the animation is dependent on the new position ( I know it's not optimal but I'm not sure where to put it)
                ths.Direction = targetPos - ths.GroundPos;
            //}
        }

        /// <summary>
        /// Update enemy position directly toward target with variation of speed (sinusoidal) for given duration - complete
        /// </summary>
        internal static bool pulseCharge(this Enemy ths, List<Sprite> targets, float aggroDistance, float speedMultiplier)
        {
            speedMultiplier = 2*(float)Math.Sin((2 * This.gameTime.TotalGameTime.Milliseconds / 1000.0) * (2 * Math.PI)) + 3.5f;

            return ths.charge(targets, aggroDistance, speedMultiplier);
        }

        /// <summary>
        /// Charge but do not update direction for length of charge - complete
        /// </summary>
        internal static bool ram(this Enemy ths, List<Sprite> targets, TimeSpan duration, float aggroDistance, float speedMultiplier)
        {
            if (ths.Personality.Status != EnemyStatus.Ram)
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;

                Sprite target = ths.GetClosestTarget(targets, aggroDistance);
                if (target != null)
                {
                    ths.Direction = target.GroundPos - ths.GroundPos;
                    ths.Personality.Status = EnemyStatus.Ram;
                }
            }

            float ramSpeed = ths.Speed * speedMultiplier;

            if (ths.Personality.Status == EnemyStatus.Ram)
            {
                if (duration == TimeSpan.MaxValue || This.gameTime.TotalGameTime <= ths.movementStartTime + duration)
                {
                    ths.GroundPos += ths.Direction * ramSpeed;
                }
                else
                {
                    ths.Personality.Status = EnemyStatus.Wander;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Hide and follow player until certain distance from player - complete
        /// </summary>
        internal static bool stealthCharge(this Enemy ths, List<Sprite> targets, TimeSpan duration, float visibleDistance, float aggroDistance, float speedMultiplier)
        {
            if (ths.Personality.Status != EnemyStatus.Charge)
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;
            }

            Sprite target = ths.GetClosestTarget(targets, aggroDistance);
            if (target != null)
            {
                ths.Personality.Status = EnemyStatus.Charge;
                if (Vector2.DistanceSquared(target.GroundPos, ths.GroundPos) <= visibleDistance * visibleDistance)
                {
                    ths.Personality.Status = EnemyStatus.Wander;
                    ths.Visible = true;
                    return true;
                }
                else
                {
                    ths.Visible = false;
                }
            }
            else
            {
                return false;
            }

            float chargeSpeed = ths.Speed * speedMultiplier;

            if (ths.Personality.Status == EnemyStatus.Charge)
            {
                if (duration == TimeSpan.MaxValue || This.gameTime.TotalGameTime <= ths.movementStartTime + duration)
                {
                    ths.Direction = target.GroundPos - ths.GroundPos;
                    ths.GroundPos += ths.Direction * chargeSpeed;
                }
                else
                {
                    ths.Personality.Status = EnemyStatus.Wander;
                    ths.Visible = true;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Be still until a certain distance from a player (between aggroDistance and ignoreDistance)
        /// </summary>
        internal static bool camp(this Enemy ths, List<Sprite> targets, float aggroDistance, float ignoreDistance)
        {
            Sprite target = ths.GetClosestTarget(targets, aggroDistance);

            if (target != null)
            {
                ths.Personality.Status = EnemyStatus.Wander;
                return true;
            }
            else
            {
                target = ths.GetClosestTarget(targets);
                if (target != null && (Vector2.DistanceSquared(target.GroundPos, ths.GroundPos) >
                    (ignoreDistance * ignoreDistance)))
                {
                    ths.Personality.Status = EnemyStatus.Wander;
                    return true;
                }

                ths.Personality.Status = EnemyStatus.Frozen;
            }

            return false;
        }

        internal static bool retreat(this Enemy ths, List<Sprite> targets, float safeDistance, float speedMultiplier)
        {
            return ths.retreat(targets, TimeSpan.MaxValue, safeDistance, speedMultiplier);
        }

        /// <summary>
        /// Move away until x seconds have passed or you are y distance away
        /// </summary>
        internal static bool retreat(this Enemy ths, List<Sprite> targets, TimeSpan duration, float safeDistance, float speedMultiplier)
        {
            if (ths.Personality.Status != EnemyStatus.Charge)
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;
            }

            Sprite target = ths.GetClosestTarget(targets, safeDistance);
            float fleeSpeed = ths.Speed * speedMultiplier;

            if (target != null)
            {
                ths.Personality.Status = EnemyStatus.Charge;
            }
            else
            {
                ths.Personality.Status = EnemyStatus.Wander;
                return true;
            }

            if (ths.Personality.Status == EnemyStatus.Charge)
            {
                if (duration == TimeSpan.MaxValue || This.gameTime.TotalGameTime <= ths.movementStartTime + duration)
                {
                    ths.Direction = target.GroundPos - ths.GroundPos;
                    ths.GroundPos -= ths.Direction * fleeSpeed;
                }
                else
                {
                    ths.Personality.Status = EnemyStatus.Wander;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Be Invisible and still until certain distance from player
        /// </summary>
        internal static bool stealthCamp(this Enemy ths, List<Sprite> targets, float aggroDistance, float ignoreDistance)
        {
            ths.Visible = camp(ths, targets, aggroDistance, ignoreDistance);
            return ths.Visible;
        }

        /// <summary>
        /// Be Invisible and move away until you are y distance away
        /// </summary>
        internal static bool stealthRetreat(this Enemy ths, List<Sprite> targets, float safeDistance, float speedMultiplier)
        {
            ths.Visible = ths.retreat(targets, safeDistance, speedMultiplier);
            return ths.Visible;
        }

        /// <summary>
        /// Move away when x distance from target until z distance from player
        /// </summary>
        internal static bool teaseRetreat(this Enemy ths, List<Sprite> targets, float aggroDistance, float safeDistance, float speedMultiplier)
        {
            if (ths.GetClosestTarget(targets, aggroDistance) != null)
            {
                return retreat(ths, targets, safeDistance, speedMultiplier);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Stop moving for x seconds - complete
        /// </summary>
        internal static bool freeze(this Enemy ths, TimeSpan duration)
        {
            if (ths.Personality.Status != EnemyStatus.Frozen)
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;
                ths.Personality.Status = EnemyStatus.Frozen;
            }

            else if (This.gameTime.TotalGameTime >= ths.movementStartTime + duration)
            {
                ths.Personality.Status = EnemyStatus.Wander;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Wander around for x seconds or until within a certain distance from a target
        /// </summary>
        internal static bool wander(this Enemy ths, List<Sprite> targets, TimeSpan duration, float safeDistance, float arcAngle)
        {
            Sprite min = ths.GetClosestTarget(targets, safeDistance);

            if (min != null)  // Near a target, move on to something else
            {
                return true;
            }
            if (RNG.NextDouble() < 0.9)
            {
                double angle = Math.Atan2(ths.Direction.Y, ths.Direction.X) + (2 * RNG.NextDouble() - 1) * arcAngle;
                ths.Direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                ths.GroundPos += ths.Direction * ths.Speed / 4;  // Wandering should be *slow*
            }
            return false;
        }


        /// <summary>
        /// Wander around within a certain range from closest enemy
        /// </summary>
        internal static bool rangeWander(this Enemy ths, List<Sprite> targets, TimeSpan duration, float arcAngle, float wanderRadius)
        {
            Sprite target = ths.GetClosestTarget(targets);
            if (target == null) return false;

            if (Vector2.DistanceSquared(ths.GroundPos, target.GroundPos) >= wanderRadius * wanderRadius)
            {
                ths.Direction = target.GroundPos - ths.GroundPos;
                ths.Direction.Normalize();
                ths.GroundPos += ths.Direction * ths.Speed / 4;
            }


            if (RNG.NextDouble() < 0.9)
            {
                double angle = Math.Atan2(ths.Direction.Y, ths.Direction.X) + (2 * RNG.NextDouble() - 1) * arcAngle;
                ths.Direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                ths.GroundPos += ths.Direction * ths.Speed / 4;  // Wandering should be *slow*
            }

            return false;
        }

        /// <summary>
        /// Wander around within a certain range from closest enemy ( For Owls)
        /// </summary>
        internal static bool wideRangeWander(this Enemy ths, List<Sprite> targets, TimeSpan duration, float arcAngle, float wanderRadius)
        {
            Sprite target = ths.GetClosestTarget(targets);
            if (target == null) return false;

            if (Vector2.DistanceSquared(ths.GroundPos, target.GroundPos) >= wanderRadius * wanderRadius)
            {
                ths.Direction = target.GroundPos - ths.GroundPos;
                ths.Direction.Normalize();
                ths.GroundPos += ths.Direction * ths.Speed;
            }


            if (RNG.NextDouble() < 0.2)
            {
                double angle = Math.Atan2(ths.Direction.Y, ths.Direction.X) + (2 * RNG.NextDouble() - 1) * arcAngle;
                ths.Direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                //ths.GroundPos += ths.Direction * ths.Speed / 4;  // Wandering should be *slow*
            }

            ths.GroundPos += ths.Direction * ths.Speed;  // Wandering should be *slow*

            return false;
        }

        /// <summary>
        ///  Go quickly to a new location within flyRadius pixels from the target
        /// </summary
        internal static bool dart(this Enemy ths, List<Sprite> targets, float dartSpeedMultiplier, int flyRadius, TimeSpan dartTimeout)
        {
            Sprite target = ths.GetClosestTarget(targets);

            if (target == null) return false;

            float dartSpeed = ths.Speed * dartSpeedMultiplier;


            if (ths.Personality.Status != EnemyStatus.Charge)
            {
                nextHoverPoint = new Vector2(
                        RNG.Next((int)target.GroundPos.X - flyRadius, (int)target.GroundPos.X + flyRadius),  //(int)(ths.Pos.X + Vector2.Distance(target.Pos, ths.Pos))),
                        RNG.Next((int)target.GroundPos.Y - flyRadius, (int)target.GroundPos.Y + flyRadius)  //(int)(ths.Pos.Y + Vector2.Distance(target.Pos, ths.Pos)))
                );

                ths.Direction = -(ths.GroundPos - nextHoverPoint);// -ths.GroundPos;
                ths.Direction.Normalize();
                ths.Personality.Status = EnemyStatus.Charge;
                dartTimeout = new TimeSpan(0, 0, 0, 0, 300);
                ths.movementStartTime = This.gameTime.TotalGameTime;
            }


            //if we choose a nextHoverPoint thats beyond a wall, we get stuck...
            else if (Vector2.Distance(ths.GroundPos, nextHoverPoint) > 3f && nextHoverPoint != Vector2.Zero && This.gameTime.TotalGameTime < ths.movementStartTime + dartTimeout)
            {
                ths.GroundPos += ths.Direction * dartSpeed;
            }

            else
            {
                ths.Personality.Status = EnemyStatus.Wander;
                nextHoverPoint = new Vector2(
                        RNG.Next((int)target.GroundPos.X - flyRadius, (int)target.GroundPos.X + flyRadius),  //(int)(ths.Pos.X + Vector2.Distance(target.Pos, ths.Pos))),
                        RNG.Next((int)target.GroundPos.Y - flyRadius, (int)target.GroundPos.Y + flyRadius)
                );

                ths.Direction = (ths.GroundPos - nextHoverPoint);// -ths.GroundPos;
                ths.Direction.Normalize();
                //dartTimeout = new TimeSpan(0, 0, 0, 0, 300);
                ths.movementStartTime = This.gameTime.TotalGameTime;
                return true;
            }

            return false;
        }


        internal static bool dartingBat(this Enemy ths, List<Sprite> targets, float dartSpeedMultiplier, int flyRadius, TimeSpan dartTimeout)
        {
            Sprite target = ths.GetClosestTarget(targets);

            if (target == null) return false;

            float dartSpeed = ths.Speed * dartSpeedMultiplier;


            if (ths.Personality.Status != EnemyStatus.Charge)
            {
                nextHoverPoint = new Vector2(
                        RNG.Next((int)target.GroundPos.X - flyRadius, (int)target.GroundPos.X + flyRadius),  //(int)(ths.Pos.X + Vector2.Distance(target.Pos, ths.Pos))),
                        RNG.Next((int)target.GroundPos.Y - flyRadius, (int)target.GroundPos.Y + flyRadius)  //(int)(ths.Pos.Y + Vector2.Distance(target.Pos, ths.Pos)))
                );

                ths.Direction = -(ths.GroundPos - nextHoverPoint);// -ths.GroundPos;
                ths.Direction.Normalize();
                ths.Personality.Status = EnemyStatus.Charge;
                dartTimeout = new TimeSpan(0, 0, 0, 0, 300);
                ths.movementStartTime = This.gameTime.TotalGameTime;
            }


            //if we choose a nextHoverPoint thats beyond a wall, we get stuck...
            else if (Vector2.Distance(ths.GroundPos, nextHoverPoint) > 3f && nextHoverPoint != Vector2.Zero && This.gameTime.TotalGameTime < ths.movementStartTime + dartTimeout)
            {
                ths.GroundPos += ths.Direction * dartSpeed;
            }

            else
            {
                ths.Personality.Status = EnemyStatus.Wander;
                nextHoverPoint = new Vector2(
                        RNG.Next((int)target.GroundPos.X - flyRadius, (int)target.GroundPos.X + flyRadius),  //(int)(ths.Pos.X + Vector2.Distance(target.Pos, ths.Pos))),
                        RNG.Next((int)target.GroundPos.Y - flyRadius, (int)target.GroundPos.Y + flyRadius)
                );

                ths.Direction = (ths.GroundPos - nextHoverPoint);// -ths.GroundPos;
                ths.Direction.Normalize();
                //dartTimeout = new TimeSpan(0, 0, 0, 0, 300);
                ths.movementStartTime = This.gameTime.TotalGameTime;
                return true;
            }

            if (RNG.NextDouble() < 0.9)
            {
                double angle = Math.Atan2(ths.Direction.Y, ths.Direction.X) + (2 * RNG.NextDouble() - 1) * Math.PI / 8;
                ths.Direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                ths.GroundPos += ths.Direction * ths.Speed / 4;  // Wandering should be *slow*
            }

            return false;
        }

        /// <summary>
        /// Move off-screen for `duration` seconds, then teleport to a random point within a rectangle
        /// </summary>
        internal static bool delayedTeleport(this Enemy ths, TimeSpan wait, Rectangle bounds)
        {
            if (ths.Personality.Status != EnemyStatus.Frozen)
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;
                ths.Personality.Status = EnemyStatus.Frozen;
            }

            else if (This.gameTime.TotalGameTime >= ths.movementStartTime + wait)
            {
                ths.Personality.Status = EnemyStatus.Wander;
                ths.GroundPos = new Vector2(bounds.X + RNG.Next(bounds.Width), bounds.Y + RNG.Next(bounds.Height));
                return true;
            }

            return false;
        }
    }
}
