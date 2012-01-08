using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

/// \file Tile.cs This is Shared with the Level Editor
namespace Frostbyte
{
    // REST OF CLASS LOCATED IN Shared/Tile.cs

    public partial class Tile : LevelObject
    {
        Texture2D Image { get; set; }

        /// <summary>
        /// The tiles's width
        /// </summary>
        internal int Width { get; set; }

        /// <summary>
        ///  The tiles's height
        /// </summary>
        internal int Height { get; set; }

        /// <summary>
        /// Scale for drawing
        /// </summary>
        internal float Scale = 1;

        /// <summary>
        /// Whether or not we should do horizontal flip
        /// </summary>
        internal bool Hflip { get; set; }

        /// <summary>
        /// Whether or not we should do a vertical flip
        /// </summary>
        internal bool Vflip { get; set; }

        /// <summary>
        /// Position of the top left corner in the image
        /// </summary>
        internal Vector2 ImageStartPos { get; set; }

        internal void Draw()
        {
            #region setup
            if (Image == null)
            {
                FrostbyteLevel l = This.Game.CurrentLevel as FrostbyteLevel;
                if (Type != TileTypes.DEFAULT)
                {
                    //Set up the way it should face
                    if (Orientation == Orientations.Up_Left)
                    {
                        Hflip = true;
                        Vflip = true;
                    }
                    else if (Orientation == Orientations.Up)
                    {
                        Vflip = true;
                    }
                    else if (Orientation == Orientations.Right)
                    {
                        Hflip = true;
                    }

                    //The file that has the spritesheet for the image
                    string file = "";

                    //determine what the image should be
                    switch (Theme != Element.DEFAULT ? Theme : l.Theme)
                    {
                        case Element.Earth:
                            file = "Earth";
                            break;
                        case Element.Lightning:
                            file = "Lightning";
                            break;
                        case Element.Water:
                            file = "Water";
                            break;
                        case Element.Fire:
                            file = "Fire";
                            break;
                        case Element.Normal:
                            file = "Normal";
                            break;
                        case Element.DEFAULT:
                            file = "Normal";
                            break;
                        case Element.None:
                            return;
                        default:
                            return;
                    }

                    Image = l.GetTexture(file);

                    List<Tile> frames = new List<Tile>();

                    XDocument doc = l.GetTextureDoc(file + ".spsh");

                    foreach (var frame in doc.Descendants("Frame"))
                    {
                        int h = int.Parse(frame.Attribute("Height").Value);
                        int w = int.Parse(frame.Attribute("Width").Value);
                        System.Windows.Point TL = System.Windows.Point.Parse(frame.Attribute("TLPos").Value);
                        frames.Add(
                            new Tile()
                            {
                                ImageStartPos = new Vector2((float)TL.X, (float)TL.Y),
                                Height = h,
                                Width = w
                            }
                        );
                    }

                    //determine the pos in the image
                    Tile f;
                    switch (Type)
                    {
                        //The items in the spritesheet will be ordered as follows:
                        //Wall, Bottom, Corner, ConvexCorner,
                        //Sidewall, TopArea, BottomCorner, BottomConvexCorner,
                        //Floor
                        case TileTypes.Wall:
                            f = frames[0];
                            ImageStartPos = f.ImageStartPos;
                            Width = f.Width;
                            Height = f.Height;
                            break;
                        case TileTypes.Bottom:
                            f = frames[1];
                            ImageStartPos = f.ImageStartPos;
                            Width = f.Width;
                            Height = f.Height;
                            break;
                        case TileTypes.Corner:
                            f = frames[2];
                            ImageStartPos = f.ImageStartPos;
                            Width = f.Width;
                            Height = f.Height;
                            break;
                        case TileTypes.BottomCorner:
                            f = frames[6];
                            ImageStartPos = f.ImageStartPos;
                            Width = f.Width;
                            Height = f.Height;
                            break;
                        case TileTypes.ConvexCorner:
                            f = frames[3];
                            ImageStartPos = f.ImageStartPos;
                            Width = f.Width;
                            Height = f.Height;
                            break;
                        case TileTypes.BottomConvexCorner:
                            f = frames[7];
                            ImageStartPos = f.ImageStartPos;
                            Width = f.Width;
                            Height = f.Height;
                            break;
                        case TileTypes.Floor:
                            f = frames[8];
                            ImageStartPos = f.ImageStartPos;
                            Width = f.Width;
                            Height = f.Height;
                            break;
                        //case TileTypes.Lava:
                        //    file = "lava";
                        //    break;
                        //case TileTypes.Water:
                        //    file = "water";
                        //    break;
                        case TileTypes.SideWall:
                            f = frames[4];
                            ImageStartPos = f.ImageStartPos;
                            Width = f.Width;
                            Height = f.Height;
                            break;
                        //case TileTypes.Room:
                        //    //do some magic to show pic for the walls etc
                        //    file = "room";
                        //    break;
                        //case TileTypes.Stone:
                        //    file = "rock";
                        //    break;
                        //case TileTypes.Empty:
                        //    file = "";
                        //    break;
                        case TileTypes.TopArea:
                            f = frames[5];
                            ImageStartPos = f.ImageStartPos;
                            Width = f.Width;
                            Height = f.Height;
                            break;
                        case TileTypes.DEFAULT:
                            f = frames[5];
                            ImageStartPos = f.ImageStartPos;
                            Width = f.Width;
                            Height = f.Height;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (l.TopAreaTile != null)
                    {
                        Image = l.TopAreaTile.Image;
                        ImageStartPos = l.TopAreaTile.ImageStartPos;
                        Width = l.TopAreaTile.Width;
                        Height = l.TopAreaTile.Height;
                    }
                    else
                    {
                        //The file that has the spritesheet for the image
                        string file = "";

                        //determine what the image should be
                        switch (l.Theme)
                        {
                            case Element.Earth:
                                file = "Earth";
                                break;
                            case Element.Lightning:
                                file = "Lightning";
                                break;
                            case Element.Water:
                                file = "Water";
                                break;
                            case Element.Fire:
                                file = "Fire";
                                break;
                            case Element.Normal:
                                file = "Normal";
                                break;
                            case Element.DEFAULT:
                                file = "Blank";
                                break;
                            case Element.None:
                                return;
                            default:
                                return;
                        }

                        l.TopAreaTile = new Tile() { Image = l.GetTexture(file) };

                        List<Tile> frames = new List<Tile>();

                        XDocument doc = l.GetTextureDoc(file + ".spsh");

                        foreach (var frame in doc.Descendants("Frame"))
                        {
                            int h = int.Parse(frame.Attribute("Height").Value);
                            int w = int.Parse(frame.Attribute("Width").Value);
                            System.Windows.Point TL = System.Windows.Point.Parse(frame.Attribute("TLPos").Value);


                            frames.Add(
                                new Tile()
                                {
                                    ImageStartPos = new Vector2((float)TL.X, (float)TL.Y),
                                    Height = h,
                                    Width = w
                                }
                            );
                        }

                        //determine the pos in the image
                        Tile f = frames[5];
                        l.TopAreaTile.ImageStartPos = f.ImageStartPos;
                        l.TopAreaTile.Width = f.Width;
                        l.TopAreaTile.Height = f.Height;

                        Image = l.TopAreaTile.Image;
                        ImageStartPos = l.TopAreaTile.ImageStartPos;
                        Width = l.TopAreaTile.Width;
                        Height = l.TopAreaTile.Height;
                    }
                }
            }

            #endregion setup

            if (GridCell != null && Image != null && !Image.IsDisposed)
            {
                This.Game.spriteBatch.Draw(
                        Image,
                        GridCell.Pos,
                        new Rectangle((int)ImageStartPos.X, (int)ImageStartPos.Y, Width, Height),
                        Microsoft.Xna.Framework.Color.White,
                        0,
                        new Vector2(0, 0),
                        Scale * (Width + 1) / (float)Width,
                        Hflip ?
                            Vflip ?
                                SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically
                                : SpriteEffects.FlipHorizontally
                            :
                            Vflip ?
                                 SpriteEffects.FlipVertically
                                : SpriteEffects.None
                        ,
                        0
                    );
            }
        }
    }
}


