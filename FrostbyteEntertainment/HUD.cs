using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace Frostbyte
{
    #region HUD Themes
    interface IHUDTheme
    {
        Color TextColor { get; }
        SpriteFont TitleFont { get; }
        SpriteFont TextFont { get; }
        Color TransparentBackgroundColor { get; }
    }

    internal class GenericTheme : IHUDTheme
    {
        protected byte Alpha = 90;
        public virtual Color TextColor { get { return Color.White; } }
        public virtual SpriteFont TitleFont { get { return This.Game.Content.Load<SpriteFont>("Fonts/Title"); } }
        public virtual SpriteFont TextFont { get { return This.Game.Content.Load<SpriteFont>("Fonts/Default"); } }
        public virtual Color TransparentBackgroundColor
        {
            get
            {
                Color transp = Color.Black;
                transp.A = Alpha;
                return transp;
            }
        }
    }

    internal class EarthTheme : GenericTheme
    {
        public override Color TextColor { get { return Color.Black; } }
        public override SpriteFont TextFont { get { return This.Game.Content.Load<SpriteFont>("Fonts/Earth"); } }
        public override Color TransparentBackgroundColor
        {
            get
            {
                Color transp = Color.BurlyWood;
                transp.A = Alpha;
                return transp;
            }
        }
    }

    internal class LightningTheme : GenericTheme
    {
        public override Color TextColor { get { return Color.Lavender; } }
        public override SpriteFont TextFont { get { return This.Game.Content.Load<SpriteFont>("Fonts/Lightning"); } }
    }

    internal class WaterTheme : GenericTheme
    {
        public override Color TextColor { get { return new Color(0.45f, 0.68f, 0.45f); } }
        public override SpriteFont TextFont { get { return This.Game.Content.Load<SpriteFont>("Fonts/Default"); } }
        public override Color TransparentBackgroundColor
        {
            get
            {
                Color transp = Color.Black;
                transp.A = 10;
                return transp;
            }
        }
    }

    internal class FireTheme : GenericTheme
    {
        public override Color TextColor { get { return Color.Firebrick; } }
        public override SpriteFont TextFont { get { return This.Game.Content.Load<SpriteFont>("Fonts/Fire"); } }
    }

    internal class FinalTheme : GenericTheme
    {
        public override Color TextColor { get { return Color.Blue; } }
        public override SpriteFont TextFont { get { return This.Game.Content.Load<SpriteFont>("Fonts/Final"); } }
    }
    #endregion Themes

    internal class HUD
    {
        #region Constructors
        internal HUD()
            : this(new GenericTheme())
        {
        }

        internal HUD(IHUDTheme theme)
        {
            this.theme = theme;
        }
        #endregion

        #region Variables
        internal IHUDTheme theme;
        private TextScroller scroller;
        private TextFader fader;
        private List<ProgressBar> bossHealthBars = new List<ProgressBar>();
        private List<PlayerHUD> playerHUDS = new List<PlayerHUD>();
        private static Vector2 barSize = new Vector2(100, 20);
        private static Vector2 barSpacing = new Vector2(10, 2);
        private ItemArea items;
        #endregion

        #region Methods
        internal void LoadCommon(IHUDTheme Theme = null)
        {
            if (Theme != null)
            {
                theme = Theme;
            }

            Viewport v = This.Game.GraphicsDevice.Viewport;
            scroller = new TextScroller("scroller", theme);
            scroller.Pos = new Vector2(FrostbyteLevel.BORDER_WIDTH / 2,
                v.Height - scroller.GetAnimation().Height);
            scroller.Static = true;

            fader = new TextFader("fader", theme);
            fader.Pos = new Vector2(v.Width - 10, v.Height - 10 - 30);
            fader.Anchor = Orientations.Up_Right;
            fader.Static = true;

            #region ItemBag
            items = new ItemArea("Items", theme);
            items.Pos = new Vector2(890, 10);
            items.Static = true;
            #endregion
        }

        internal void AddPlayer(Player p)
        {
            int xoffset = 80 + playerHUDS.Count * (int)(barSize.X + barSpacing.X);
            playerHUDS.Add(new PlayerHUD(theme, p, xoffset, 10));
        }

        internal void AddBossHealthBar(Boss b)
        {
            ProgressBar lastPlayerHealth = playerHUDS.Last().healthBar;
            Vector2 size = new Vector2(This.Game.GraphicsDevice.Viewport.Width * 0.8f, barSize.Y * 1.5f);
            ProgressBar healthBar = new ProgressBar(b.Name + "_health", b.MaxHealth,
                    Color.DarkRed, Color.Firebrick, Color.Black, size);
            healthBar.Pos = new Vector2(
                (This.Game.GraphicsDevice.Viewport.Width - size.X) / 2,
                This.Game.GraphicsDevice.Viewport.Height - size.Y * 2);
            healthBar.Static = true;
            healthBar.Value = b.MaxHealth;

            b.HealthChanged += delegate(object obj, int value)
            {
                healthBar.Value = value;
            };
            bossHealthBars.Add(healthBar);
        }

        internal void RemoveBossHealthBar(Boss b)
        {
            foreach (ProgressBar p in bossHealthBars.FindAll(p => p.Name == b.Name + "_health"))
            {
                This.Game.CurrentLevel.RemoveSprite(p);
                bossHealthBars.Remove(p);
            }
        }

        internal void ScrollText(string s)
        {
            if (scroller != null && s != null)
            {
                scroller.ScrollText(s);
            }
        }

        internal void FadeText(string s)
        {
            if (fader != null && s != null)
            {
                fader.FadeText(s);
            }
        }
        #endregion

        private class PlayerHUD
        {
            internal PlayerHUD(IHUDTheme theme, Player p, int xOffset, int yOffset)
            {
                #region Name
                Text name = new Text("player_name_" + p.Name, "Text", p.Name);
                name.DisplayColor = theme.TextColor;
                name.Pos = new Vector2(xOffset, yOffset);
                name.Static = true;
                #endregion

                #region HealthBar
                healthBar = new ProgressBar("Health_" + p.Name, p.MaxHealth,
                    Color.DarkRed, Color.Firebrick, Color.Black, barSize);
                healthBar.Pos = new Vector2(xOffset, name.Pos.Y + name.GetAnimation().Height);
                healthBar.Static = true;
                healthBar.Value = p.MaxHealth;

                p.HealthChanged += delegate(object obj, int value)
                {
                    healthBar.Value = value;
                    if (value == 0)
                    {
                        name.DisplayColor = Color.Tomato;
                    }
                    else
                    {
                        name.DisplayColor = theme.TextColor;
                    }
                };
                #endregion

                #region ManaBar
                manaBar = new ProgressBar("Mana_" + p.Name, p.MaxMana,
                    Color.MidnightBlue, Color.Blue, Color.Black, barSize);
                manaBar.Pos = new Vector2(xOffset,
                    healthBar.Pos.Y + barSize.Y + barSpacing.Y);
                manaBar.Static = true;
                manaBar.Value = p.MaxMana;

                p.ManaChanged += delegate(object obj, int value)
                {
                    manaBar.Value = value;
                };
                #endregion

                #region Spell Queue
                spellQueue = new SpellQueue("Queue", theme, p);
                spellQueue.Pos = new Vector2(xOffset,
                    healthBar.Pos.Y + barSize.Y + 2 * barSpacing.Y + barSize.Y);
                spellQueue.Static = true;

                #endregion


            }

            ~PlayerHUD()
            {
                This.Game.CurrentLevel.RemoveSprite(healthBar);
                This.Game.CurrentLevel.RemoveSprite(manaBar);
            }

            #region Variables
            internal ProgressBar healthBar;
            internal ProgressBar manaBar;
            internal SpellQueue spellQueue;


            internal int Health
            {
                get
                {
                    return healthBar.Value;
                }
                set
                {
                    healthBar.Value = value;
                }
            }
            internal int Mana
            {
                get
                {
                    return manaBar.Value;
                }
                set
                {
                    manaBar.Value = value;
                }
            }
            #endregion
        }

        private class ItemArea : Sprite
        {
            internal ItemArea(string name, IHUDTheme theme)
                : base(name, new Actor(new DummyAnimation(name,
                    (int)HUD.barSize.X, (int)HUD.barSize.Y)))
            {
                ZOrder = 100;
                this.theme = theme;
                background = new Texture2D(This.Game.GraphicsDevice, 1, 1);
                background.SetData(new Color[] { theme.TransparentBackgroundColor });
            }

            private IHUDTheme theme;
            private Texture2D background;

            private static Vector2 itemSpacing = new Vector2(3, 2);
            private static int itemsPerRow = 5;

            internal override void Draw(GameTime gameTime)
            {
                base.Draw(gameTime);

                This.Game.spriteBatch.Draw(background, new Rectangle(
                        (int)Pos.X,
                        (int)Pos.Y,
                        (int)GetAnimation().Width,
                        (int)GetAnimation().Height), Color.White);

                if (Player.ItemBag.Count > 0)
                {
                    for (int x = 0; x < Player.ItemBag.Count; x++)
                    {
                        Sprite icon = Player.ItemBag[x].Icon;
                        icon.Pos.X = Pos.X + itemSpacing.X + 1 +  // Initial alignment of 1px
                            (x % itemsPerRow) * (icon.GetAnimation().Width + itemSpacing.X);
                        icon.Pos.Y = Pos.Y + itemSpacing.Y +
                            (x / itemsPerRow) * (icon.GetAnimation().Height + itemSpacing.Y);
                        icon.Visible = true;
                        icon.Draw(gameTime);
                    }
                }
            }
        }

        private class SpellQueue : Sprite
        {
            internal SpellQueue(string name, IHUDTheme theme, Player player)
                : base(name, new Actor(new DummyAnimation(name,
                    (int)HUD.barSize.X, (int)HUD.barSize.Y)))
            {
                ZOrder = 100;
                this.theme = theme;
                background = new Texture2D(This.Game.GraphicsDevice, 1, 1);
                background.SetData(new Color[] { theme.TransparentBackgroundColor });
                this.player = player;
            }

            private IHUDTheme theme;
            private Texture2D background;
            private Player player;

            private static Vector2 itemSpacing = new Vector2(12, 2);
            private static int itemsPerRow = 3;

            internal override void Draw(GameTime gameTime)
            {
                base.Draw(gameTime);

                This.Game.spriteBatch.Draw(background, new Rectangle(
                        (int)Pos.X,
                        (int)Pos.Y,
                        (int)GetAnimation().Width,
                        (int)GetAnimation().Height), Color.White);

                int limit = 0;
                if (Characters.Mage.UnlockedSpells.HasFlag(Spells.EarthThree))
                    limit = 3;
                else if (Characters.Mage.UnlockedSpells.HasFlag(Spells.EarthTwo))
                    limit = 2;
                else
                    limit = 1;

                for (int x = 0; x < (player as Frostbyte.Characters.Mage).attackCounter.Count && x < limit; x++)
                {

                    if ((player as Frostbyte.Characters.Mage).attackCounter[x] == Element.Earth && Characters.Mage.UnlockedSpells.HasFlag(Spells.EarthOne))
                    {
                        Sprite icon = new ItemIcon("earth", new Actor(This.Game.CurrentLevel.GetAnimation("earthIcon.anim")));
                        icon.Pos.X = Pos.X + itemSpacing.X + 1 +  // Initial alignment of 1px
                        (x % itemsPerRow) * (icon.GetAnimation().Width + itemSpacing.X);
                        icon.Pos.Y = Pos.Y + itemSpacing.Y +
                            (x / itemsPerRow) * (icon.GetAnimation().Height + itemSpacing.Y);
                        icon.Visible = true;
                        icon.Draw(gameTime);
                        This.Game.CurrentLevel.RemoveSprite(icon);
                    }

                    else if ((player as Frostbyte.Characters.Mage).attackCounter[x] == Element.Lightning && Characters.Mage.UnlockedSpells.HasFlag(Spells.LightningOne))
                    {
                        Sprite icon = new ItemIcon("lightning", new Actor(This.Game.CurrentLevel.GetAnimation("lightningIcon.anim")));
                        icon.Pos.X = Pos.X + itemSpacing.X + 1 +  // Initial alignment of 1px
                        (x % itemsPerRow) * (icon.GetAnimation().Width + itemSpacing.X);
                        icon.Pos.Y = Pos.Y + itemSpacing.Y +
                            (x / itemsPerRow) * (icon.GetAnimation().Height + itemSpacing.Y);
                        icon.Visible = true;
                        icon.Draw(gameTime);
                        This.Game.CurrentLevel.RemoveSprite(icon);
                    }

                    else if ((player as Frostbyte.Characters.Mage).attackCounter[x] == Element.Water && Characters.Mage.UnlockedSpells.HasFlag(Spells.WaterOne))
                    {
                        Sprite icon = new ItemIcon("water", new Actor(This.Game.CurrentLevel.GetAnimation("waterIcon.anim")));
                        icon.Pos.X = Pos.X + itemSpacing.X + 1 +  // Initial alignment of 1px
                        (x % itemsPerRow) * (icon.GetAnimation().Width + itemSpacing.X);
                        icon.Pos.Y = Pos.Y + itemSpacing.Y +
                            (x / itemsPerRow) * (icon.GetAnimation().Height + itemSpacing.Y);
                        icon.Visible = true;
                        icon.Draw(gameTime);
                        This.Game.CurrentLevel.RemoveSprite(icon);
                    }

                    else if ((player as Frostbyte.Characters.Mage).attackCounter[x] == Element.Fire && Characters.Mage.UnlockedSpells.HasFlag(Spells.FireOne))
                    {
                        Sprite icon = new ItemIcon("fire", new Actor(This.Game.CurrentLevel.GetAnimation("fireIcon.anim")));
                        icon.Pos.X = Pos.X + itemSpacing.X + 1 +  // Initial alignment of 1px
                        (x % itemsPerRow) * (icon.GetAnimation().Width + itemSpacing.X);
                        icon.Pos.Y = Pos.Y + itemSpacing.Y +
                            (x / itemsPerRow) * (icon.GetAnimation().Height + itemSpacing.Y);
                        icon.Visible = true;
                        icon.Draw(gameTime);
                        This.Game.CurrentLevel.RemoveSprite(icon);
                    }

                    //Sprite icon = Player.ItemBag[x].Icon;
                    //icon.Pos.X = Pos.X + itemSpacing.X + 1 +  // Initial alignment of 1px
                    //    (x % itemsPerRow) * (icon.GetAnimation().Width + itemSpacing.X);
                    //icon.Pos.Y = Pos.Y + itemSpacing.Y +
                    //    (x / itemsPerRow) * (icon.GetAnimation().Height + itemSpacing.Y);
                    //icon.Visible = true;
                    //icon.Draw(gameTime);

                }
            }
        }
    }

    internal class TextFader : Sprite
    {
        internal TextFader(string name, IHUDTheme theme)
            : this(name, theme, 0, 0)
        {
        }

        internal TextFader(string name, int width, int height)
            : this(name, new GenericTheme(), width, height)
        {
        }

        internal TextFader(string name, IHUDTheme theme, int width, int height)
            : base(name, new Actor(new DummyAnimation(name, width, height)))
        {
            ZOrder = 100;
            UpdateBehavior = update;
            this.theme = theme;
            Center = new Vector2(0, 0);
            Anchor = Orientations.Left;

            pendingText = new Queue<string>();
            mStates = States().GetEnumerator();

            toDisplay = new Text("fader_text", theme.TextFont, "");
            toDisplay.Visible = false;
            toDisplay.DisplayColor = theme.TextColor;
            toDisplay.Static = true;
            toDisplay.ZOrder = 101;
        }

        private int MaxCharacters = 100;
        private IHUDTheme theme;
        private IEnumerator mStates;
        private Queue<string> pendingText;
        private Text toDisplay;

        private float mAlpha;
        private float mFadeIncrement = 0.01f;
        private TimeSpan mFadeDelay = new TimeSpan(0, 0, 0, 1);

        internal Orientations Anchor { get; set; }

        internal void FadeText(string text)
        {
            text = String.Join("", text.Take(MaxCharacters)).Trim();
            if (!String.IsNullOrEmpty(text))
            {
                pendingText.Enqueue(text);
            }
        }

        internal void update()
        {
            mStates.MoveNext();
        }

        private IEnumerable States()
        {
            while (true)
            {
                if (pendingText.Count > 0)
                {
                    toDisplay.Content = pendingText.Dequeue();
                    Vector2 displayPos = new Vector2();

                    #region X values
                    switch (Anchor)
                    {
                        case Orientations.Right:
                            displayPos.X = Pos.X - toDisplay.GetAnimation().Width;
                            break;
                        case Orientations.Up_Right:
                            goto case Orientations.Right;
                        case Orientations.Down_Right:
                            goto case Orientations.Right;
                        case Orientations.Left:
                            goto default;
                        case Orientations.Up_Left:
                            goto case Orientations.Left;
                        case Orientations.Down_Left:
                            goto case Orientations.Left;
                        default:
                            displayPos.X = Pos.X;
                            break;
                    }
                    #endregion

                    #region Y values
                    switch (Anchor)
                    {
                        case Orientations.Down:
                            displayPos.Y = Pos.Y - toDisplay.GetAnimation().Height;
                            break;
                        case Orientations.Down_Left:
                            goto case Orientations.Down;
                        case Orientations.Down_Right:
                            goto case Orientations.Down;
                        case Orientations.Up:
                            goto default;
                        case Orientations.Up_Left:
                            goto case Orientations.Up;
                        case Orientations.Up_Right:
                            goto case Orientations.Up;
                        default:
                            displayPos.Y = Pos.Y;
                            break;
                    }
                    #endregion
                    toDisplay.Pos = displayPos;
                    toDisplay.Visible = true;

                    mAlpha = 0;
                    toDisplay.DisplayColor = (theme.TextColor * mAlpha);

                    foreach (string type in new string[] { "in", "out" })
                    {
                        // Leave faded in (or out) for mFadeDelay time
                        TimeSpan endTime = This.gameTime.TotalGameTime + mFadeDelay;
                        while (This.gameTime.TotalGameTime < endTime)
                        {
                            yield return null;
                        }

                        while (mAlpha >= 0 && mAlpha <= 1)
                        {
                            toDisplay.DisplayColor = (theme.TextColor * mAlpha);
                            mAlpha += mFadeIncrement;
                            yield return null;
                        }

                        mAlpha -= mFadeIncrement;  // Put it back within alpha range
                        mFadeIncrement *= -1;  // Reverse fade direction
                    }
                    toDisplay.Visible = false;
                }

                yield return null;
            }
        }
    }

    internal class TextScroller : Sprite
    {
        internal TextScroller(string name, IHUDTheme theme)
            : this(name, theme, This.Game.GraphicsDevice.Viewport.Width - FrostbyteLevel.BORDER_WIDTH,
                This.Game.GraphicsDevice.Viewport.Height - (int)(2.5f * FrostbyteLevel.BORDER_HEIGHT))
        {
        }

        internal TextScroller(string name, int width, int height)
            : this(name, new GenericTheme(), width, height)
        {
        }

        internal TextScroller(string name, IHUDTheme theme, int width, int height)
            : base(name, new Actor(new DummyAnimation(name, width, height)))
        {
            ZOrder = 100;
            UpdateBehavior = update;
            this.theme = theme;
            background = new Texture2D(This.Game.GraphicsDevice, 1, 1);
            background.SetData(new Color[] { theme.TransparentBackgroundColor });
            Center = new Vector2(0, 0);
        }

        internal int MaxCharactersPerLine = 62;
        internal int TextSpacing = 2;
        internal bool SplitOnWhitespace = true;
        private IHUDTheme theme;
        private List<char> buffer = new List<char>();
        private List<Text> onScreen = new List<Text>();
        private Texture2D background;

        private int tickCount = 0;
        internal int TicksPerScroll = 3;

        #region Methods
        internal void ScrollText(string s)
        {
            buffer.AddRange(s.Replace("\r\n", "\n"));
            buffer.AddRange("\n\n");
        }
        #endregion

        #region Properties
        internal bool Scrolling { get { return buffer.Count > 0 || onScreen.Count > 0; } }
        #endregion

        #region Update
        internal void update()
        {
            tickCount = (tickCount + 1) % TicksPerScroll;
            if (onScreen.Count > 0 && tickCount == 0)
            {
                foreach (Text t in onScreen)
                {
                    t.Pos.Y -= 1;
                }

                Sprite fst = onScreen.First();
                if (fst.Pos.Y < Pos.Y)
                {
                    This.Game.CurrentLevel.RemoveSprite(fst);
                    onScreen.RemoveAt(0);
                }
            }
            if (buffer.Count != 0)
            {
                // We have room to scroll another line of text
                if (onScreen.Count == 0 ||
                    onScreen.Last().Pos.Y + onScreen.Last().GetAnimation().Height + TextSpacing <
                        Pos.Y + GetAnimation().Height - onScreen.Last().GetAnimation().Height)
                {
                    int width = GetAnimation().Width;
                    string toDisplay;

                    if (String.Join("", buffer.Take(2)) == "\n\n")
                    {
                        toDisplay = " ";
                        buffer.RemoveRange(0, 2);
                    }
                    else
                    {
                        buffer = buffer.SkipWhile(x => char.IsWhiteSpace(x)).ToList();
                        IEnumerable<char> pendingDisplay = buffer.TakeWhile((ch, ix) =>
                            theme.TextFont.MeasureString(
                                String.Join("", buffer.Take(ix + 1)).Trim()).X < width &&
                            ch != '\n');

                        if (SplitOnWhitespace &&
                            buffer.Count > pendingDisplay.Count() &&
                            buffer[pendingDisplay.Count()] != '\n')
                        {
                            // Find first instance of whitespace at end
                            pendingDisplay = pendingDisplay.Reverse().SkipWhile(x => !char.IsWhiteSpace(x)).Reverse();
                        }

                        toDisplay = String.Join("", pendingDisplay).Trim();
                        buffer.RemoveRange(0, pendingDisplay.Count());
                    }

                    Text line = new Text("text", theme.TextFont, toDisplay.ToString());
                    line.DisplayColor = theme.TextColor;
                    line.Pos = new Vector2(Pos.X, Pos.Y + GetAnimation().Height - line.GetAnimation().Height);
                    line.Static = true;
                    line.ZOrder = 101;
                    onScreen.Add(line);
                }
            }
        }
        #endregion

        #region Draw
        internal override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (onScreen.Count > 0)
            {
                This.Game.spriteBatch.Draw(background, new Rectangle(
                        (int)Pos.X,
                        (int)Pos.Y,
                        (int)GetAnimation().Width,
                        (int)GetAnimation().Height), Color.White);
            }
        }
        #endregion
    }
}
