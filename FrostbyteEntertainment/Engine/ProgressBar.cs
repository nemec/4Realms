using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    internal class ProgressBar : Sprite
    {
        internal ProgressBar(string name, int maxValue, Color borderColor, Color fillColor, Color backgroundColor)
            : this(name, maxValue, borderColor, fillColor, backgroundColor, new Vector2(100, 20))
        {
        }

        internal ProgressBar(string name, int maxValue, Color borderColor, Color fillColor, Color backgroundColor, Vector2 size)
            : base(name, new Actor(new DummyAnimation(name, size)))
        {
            this.MaxValue = maxValue;
            this.size = size;
            this.innerSize = size - Vector2.One * 2 * borderSize;

            border = new Texture2D(This.Game.GraphicsDevice, 1, 1);
            border.SetData<Color>(new Color[] { borderColor });

            background = new Texture2D(This.Game.GraphicsDevice, 1, 1);
            background.SetData<Color>(new Color[] { backgroundColor });

            fill = new Texture2D(This.Game.GraphicsDevice, 1, 1);
            fill.SetData<Color>(new Color[] { fillColor });
        }

        private int borderSize = 2;
        private Vector2 size;
        private Vector2 innerSize;

        internal int MaxValue { get; set; }
        private int mValue;
        internal int Value {
            get { return mValue; }
            set {
                mValue = value < 0 ? 0 :
                    (value > MaxValue ? MaxValue :
                        value);
            } 
        }
        /// <summary>
        /// The width of the overall bar
        /// </summary>
        internal float Width { get { return size.X; } set { size.X = value; } }
        /// <summary>
        /// The height of the overall bar
        /// </summary>
        internal float Height { get { return size.Y; } set { size.Y = value; } }

        Texture2D border;
        Texture2D fill;
        Texture2D background;

        internal override void Draw(GameTime gameTime)
        {
            if (Visible)
            {
                // Needs to be updated proportional to the current Value
                Vector2 fillProportion = new Vector2(innerSize.X * mValue / MaxValue, innerSize.Y);
                
                This.Game.spriteBatch.Draw(border, new Rectangle(
                    (int)Pos.X, 
                    (int)Pos.Y, 
                    (int)size.X, 
                    (int)size.Y), Color.White);

                This.Game.spriteBatch.Draw(background, new Rectangle(
                    (int)Pos.X + borderSize, 
                    (int)Pos.Y + borderSize, 
                    (int)innerSize.X,
                    (int)innerSize.Y), Color.White);

                This.Game.spriteBatch.Draw(fill, new Rectangle(
                    (int)Pos.X + borderSize, 
                    (int)Pos.Y + borderSize, 
                    (int)fillProportion.X,
                    (int)fillProportion.Y), Color.White);
            }
        }
    }
}
