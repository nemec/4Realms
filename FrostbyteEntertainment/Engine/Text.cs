using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    class Text : Sprite
    {
        internal SpriteFont Font;
        private string mContent;
        internal Color DisplayColor { get; set; }

        internal string Content
        {
            get
            {
                return mContent;
            }

            set
            {
                mContent = value;
                setSize();
            }
        }

        internal Text(string name, string fontname, string content) :
            this(name, This.Game.Content.Load<SpriteFont>(fontname), content)
        {
        }

        internal Text(string name, SpriteFont Font, string content) :
            base(name, new Actor(new DummyAnimation(name)))
        {
            this.Font = Font;
            Content = content;
        }

        private void setSize()
        {
            Vector2 size = Font.MeasureString(mContent);
            mActor.Animations[mActor.CurrentAnimation].Frames[mActor.Frame].Width = (int)size.X;
            mActor.Animations[mActor.CurrentAnimation].Frames[mActor.Frame].Height = (int)size.Y;
            Center = size / new Vector2(2, 2);
        }

        internal override void Draw(GameTime gameTime)
        {
            if (Visible)
            {
                This.Game.spriteBatch.DrawString(Font, mContent, Pos, DisplayColor);
            }
        }
    }
}
