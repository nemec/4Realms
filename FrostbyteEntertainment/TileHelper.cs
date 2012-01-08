using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

/// \file TileHelper.cs This is Shared with the Level Editor

namespace Frostbyte
{
    // REST OF CLASS LOCATED IN Shared/TileHelper.cs
    public partial class Index2D
    {
        /// <summary>
        /// The position of the Index2D object in the game world
        /// </summary>
        internal Vector2 Pos
        {
            get
            {
                return new Vector2(X * Tile.TileSize, Y * Tile.TileSize);
            }
        }

        /// <summary>
        /// A simple way to create a Vector2D object from the Index2D grid position
        /// </summary>
        internal Vector2 Vector
        {
            get
            {
                return new Vector2(X, Y);
            }
        }
    }
}
