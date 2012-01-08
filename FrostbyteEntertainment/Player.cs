using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    internal abstract class Player : OurSprite
    {
        public event ManaChangedHandler ManaChanged = delegate { };
        internal enum TargetAlignment
        {
            Ally,
            Enemy,
            None
        }

        internal Player(string name, Actor actor)
            : base(name, actor)
        {
            (This.Game.LoadingLevel as FrostbyteLevel).allies.Add(this);
            Mana = MaxMana;

            MaxHealth = 100;
            Health = MaxHealth;
        }

        #region Targeting
        public Sprite currentTarget { get; protected set; }
        public TargetAlignment currentTargetAlignment { get; protected set; }
        #endregion

        #region Mana
        internal int MaxMana { get { return 100; } }
        internal TimeSpan ManaRegenRate = new TimeSpan(0, 0, 2);
        internal float ManaRegenScale = 0.08f;
        private TimeSpan ElapsedManaRegenTime;

        internal TimeSpan HealthRegenRate = new TimeSpan(0, 0, 5);
        private TimeSpan ElapsedHealthRegenTime;

        /// <summary>
        /// Player's Mana value
        /// </summary>
        private int mMana;
        internal int Mana
        {
            get
            {
                return mMana;
            }
            set
            {
                mMana = value < 0 ? 0 :
                    (value > MaxMana ? MaxMana :
                        value);
                ManaChanged(this, mMana);
            }
        }
        #endregion

        #region Items
        internal int ItemBagCapacity { get { return 10; } }
        internal static List<Item> ItemBag = new List<Item>();

        /// <summary>
        /// Pick up an item from the ground. Remove the item itself from the screen and
        /// place it in the bag, if it has an icon.
        /// </summary>
        /// <param name="i"></param>
        /// <returns>Return true if the item was picked up, false if not.</returns>
        protected bool PickUpItem(Item i)
        {
            if (ItemBag.Count < ItemBagCapacity)
            {
                This.Game.CurrentLevel.RemoveSprite(i);
                if (i.Icon != null)
                {
                    ItemBag.Add(i);
                }
                return true;
            }
            return false;
        }
        #endregion

        protected void ReadDiaryEntry()
        {
            FrostbyteLevel l = This.Game.CurrentLevel as FrostbyteLevel;
            l.DiaryEntries.MoveNext();
            l.HUD.ScrollText(l.DiaryEntries.Current);
        }

        internal override void Regen()
        {
            base.Regen();
            Mana = MaxMana;
        }

        internal override void Update()
        {
            base.Update();
            ElapsedManaRegenTime += This.gameTime.ElapsedGameTime;
            if (ElapsedManaRegenTime > ManaRegenRate && State != SpriteState.Dead)
            {
                Mana += (int)(ManaRegenScale * MaxMana);
                ElapsedManaRegenTime = new TimeSpan();
            }

            ElapsedHealthRegenTime += This.gameTime.ElapsedGameTime;
            if (ElapsedHealthRegenTime > HealthRegenRate && State != SpriteState.Dead && Health > 0)
            {
                Health += 1;
                ElapsedHealthRegenTime = new TimeSpan();
            }
        }

        internal void AddStatusEffect(ElementalBuff elementalBuff)
        {
            if (StatusEffects.FindAll(delegate(StatusEffect se) { return se is ElementalBuff; }).Count == 0)
                StatusEffects.Add(elementalBuff);
            //elementalBuff.Draw(this);
        }
    }
}
