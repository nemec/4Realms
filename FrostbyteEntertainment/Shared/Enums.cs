using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// \file Enums.cs This is Shared with the Level Editor
namespace Frostbyte
{
    /// <summary>
    /// Orientations for any object on screen
    /// </summary>
    public enum Orientations
    {
        /// <summary>
        /// V
        /// </summary>
        Down = 0,
        /// <summary>
        /// <-
        /// </summary>
        Left,
        /// <summary>
        /// ^
        /// </summary>
        Up,
        /// <summary>
        /// ->
        /// </summary>
        Right,
        /// <summary>
        /// \
        /// </summary>
        Up_Left,
        /// <summary>
        /// /
        /// </summary>
        Up_Right,
        /// <summary>
        /// _/
        /// </summary>
        Down_Left,
        /// <summary>
        /// \_
        /// </summary>
        Down_Right
    }

    /// <summary>
    /// Possible tiles for the level
    /// </summary>
    public enum TileTypes
    {
        /// <summary>
        /// This is going to be used to signify an area we don't want people walking (eg the part above walls etc)
        /// </summary>
        DEFAULT = -1,
        /// <summary>
        /// Foor Top or Side wall tiles (determined by orientation)
        /// </summary>
        Wall = 0,
        /// <summary>
        /// Bottom wall tiles 
        /// </summary>
        Bottom,
        /// <summary>
        /// A corner for top wall
        /// </summary>
        Corner,
        /// A corner for top of halls
        /// </summary>
        ConvexCorner,
        /// <summary>
        /// Sidewall (faces left so we need to set Orientation accordingly to ensure flips)
        /// </summary>
        SideWall,
        /// <summary>
        /// Area where user will never walk
        /// </summary>
        TopArea,
        /// <summary>
        /// Bottom corner for map
        /// </summary>
        BottomCorner,
        /// <summary>
        /// Bottom Convex corner for map (halls etc)
        /// </summary>
        BottomConvexCorner,
        /// <summary>
        /// This is for floor tiles
        /// </summary>
        Floor,
        /// <summary>
        /// Water tile
        /// </summary>
        Water,
        /// <summary>
        /// Lava tile
        /// </summary>
        Lava,
        /// <summary>
        /// Stone tile
        /// </summary>
        Stone,
        /// <summary>
        /// For Room class (needed for editor)
        /// </summary>
        Room,
        /// <summary>
        /// A cell we want specifically empty
        /// </summary>
        Empty,
    }

    /// <summary>
    /// Elements / Themes
    /// </summary>
    public enum Element
    {
        None = -2,
        DEFAULT = -1,
        Normal = 0,
        Earth,
        Lightning,
        Water,
        Fire,
    }

    /// <summary>
    /// Different states a sprite can be in.
    /// </summary>
    public enum SpriteState
    {
        Idle=0,
        Moving,
        Attacking,
        Dead  // Dead, but able to "come back to life".
    }
}
