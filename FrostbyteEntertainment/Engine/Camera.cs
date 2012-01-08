using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    internal class Camera
    {
        protected float _zoom;
        protected float _rotation;

        internal Camera()
        {
            _zoom = 1.0f;
        }

        /// <summary>
        /// To use rotation, make the Property internal and devise a new way
        /// for determining which grid cells to draw on the screen.
        /// </summary>
        private float Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                while (true)
                {
                    if (value < 0)
                    {
                        value = 2 * (float)Math.PI + value;
                    }
                    else if (value > 2 * Math.PI)
                    {
                        value = value - 2 * (float)Math.PI;
                    }
                    else
                    {
                        _rotation = value;
                        break;
                    }
                }

            }
        }
        private Vector2 RotationPoint { get; set; }

        internal Vector2 Pos { get; set; }

        internal float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                if (_zoom < 0.1f)
                {
                    _zoom = 0.1f;
                }
            }
        }

        internal Matrix GetTransformation(GraphicsDevice device)
        {
            // To rotate around a fixed point, must translate the everything to (0,0),
            // rotate, then translate back
            Vector3 rotationMod = new Vector3(RotationPoint, 0);

            return Matrix.CreateTranslation(new Vector3(-Pos.X, -Pos.Y, 0)) *
                /*Matrix.CreateTranslation(-rotationMod) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(rotationMod) *
                */
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 1));
                
        }
    }
}
