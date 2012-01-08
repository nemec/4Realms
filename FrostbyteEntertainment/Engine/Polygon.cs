using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    class Polygon : Sprite
    {
        /// <summary>
        /// Creates a polygon with an arbitrary number of points.
        /// </summary>
        /// <param name="name">Name of the sprite</param>
        /// <param name="color">Sprite's display color</param>
        /// <param name="points">Vector of points</param>
        internal Polygon(string name, Color color, params Vector3[] points)
            : this(name, new Actor(new DummyAnimation(name)), color, points)
        {
            
        }

        /// <summary>
        /// Creates a polygon with an arbitrary number of points
        /// </summary>
        /// <param name="name">Name of the sprite</param>
        /// <param name="actor">The actor that the sprite will use</param>
        /// <param name="color">Sprite's display color</param>
        /// <param name="points">Vector of points</param>
        internal Polygon(string name, Actor actor, Color color, params Vector3[] points)
            : this(name, actor, points.Select(p => new VertexPositionColor(p, color)).ToArray())
        {
        }

        /// <summary>
        /// Creates a polygon with an arbitrary number of points
        /// </summary>
        /// <param name="name">Name of the sprite</param>
        /// <param name="points">Variable number of VertexPositionColor points</param>
        internal Polygon(string name, params VertexPositionColor[] points)
            : this(name, new Actor(new DummyAnimation(name)), points)
        {
        }

        /// <summary>
        /// Creates a polygon with an arbitrary number of points
        /// </summary>
        /// <param name="name">Name of the sprite</param>
        /// <param name="actor">The actor that the sprite will use</param>
        /// <param name="points">Variable number of VertexPositionColor points</param>
        internal Polygon(string name, Actor actor, params VertexPositionColor[] points)
            : base(name, actor)
        {
            Points = points;
            basicEffect.VertexColorEnabled = true;
        }

        private BasicEffect basicEffect = new BasicEffect(This.Game.GraphicsDevice);
        internal VertexPositionColor[] Points;

        internal override void Draw(GameTime gameTime)
        {
            if (Visible)
            {
                float height = This.Game.GraphicsDevice.Viewport.Height;
                float width = This.Game.GraphicsDevice.Viewport.Width;
                basicEffect.View = Matrix.CreateLookAt(
                    new Vector3(
                        This.Game.GraphicsDevice.Viewport.X + width / 2,
                        This.Game.GraphicsDevice.Viewport.Y + height / 2,
                        -10),
                    new Vector3(
                        This.Game.GraphicsDevice.Viewport.X + width / 2,
                        This.Game.GraphicsDevice.Viewport.Y + height / 2, 0),
                        new Vector3(0, -1, 0));

                basicEffect.Projection = Matrix.CreateOrthographic(This.Game.GraphicsDevice.Viewport.Width,
                    This.Game.GraphicsDevice.Viewport.Height, 1, 20);

                basicEffect.World = Matrix.CreateTranslation(new Vector3(Pos, 0));
                if (!Static)
                {
                    basicEffect.World *= This.Game.CurrentLevel.Camera.GetTransformation(This.Game.GraphicsDevice);
                }

                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Vector3 Pos3d = new Vector3(Pos, 0);
                    List<VertexPositionColor> shiftedPoints = new List<VertexPositionColor>();
                    foreach (VertexPositionColor point in Points)
                    {
                        shiftedPoints.Add(new VertexPositionColor(point.Position + Pos3d, point.Color));
                    }
                    This.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip,
                        shiftedPoints.ToArray(),
                        0, Points.Length - 1);
                }
            }
        }
    }
}
