using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

/// \file TileHelper.cs This is Shared with the Level Editor

namespace Frostbyte
{
    ///// <summary>
    ///// Base Class for Tileable objects
    ///// </summary>
    ///// <typeparam name="T">Will be The corresponding tile for Game/Editor</typeparam>
    //public interface TileHelper<T>
    //{
    //    XElement ToXML();
    //    //void SetParse(XElement elem);/// \todo make sure all things implementing the interface have one of these but static
    //}

    public partial class Index2D
    {
        public Index2D()
        {
            X = 0;
            Y = 0;
        }

        /// <summary>
        /// Creates an index into a 2D array
        /// </summary>
        /// <param name="x">Xcoord == Column</param>
        /// <param name="y">Ycoord == Row</param>
        public Index2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Creates an index into a 2D array (hacks off the double to an int)
        /// </summary>
        /// <param name="x">Xcoord == Column WARNING: this is floored</param>
        /// <param name="y">Ycoord == Row WARNING: this is floored</param>
        public Index2D(double x, double y) : this((int)x, (int)y) { }

#if LEVELEDITOR
        //Stuff for Level editor goes here
#else
        //stuff for the game goes here
#endif

        public int X { get; set; }
        public int Y { get; set; }

        public int MagX
        {
            get
            {
                return Math.Abs(X);
            }
        }
        public int MagY
        {
            get
            {
                return Math.Abs(Y);
            }
        }

        public static Index2D Parse(string s)
        {
            string[] ss = s.Split(new char[] { ',' });
            int x = int.Parse(ss[0]);
            int y = int.Parse(ss[1]);
            return new Index2D(x, y);
        }

        public override string ToString()
        {
            return String.Format("{0},{1}", X, Y);
        }

        public static Index2D operator +(Index2D a, Index2D i)
        {
            return new Index2D(a.X + i.X, a.Y + i.Y);
        }
        public static Index2D operator -(Index2D a, Index2D i)
        {
            return new Index2D(a.X - i.X, a.Y - i.Y);
        }

        public static bool operator ==(Index2D a, Index2D b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }
            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Index2D a, Index2D i)
        {
            return !(a == i);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Index2D)
                return this == (obj as Index2D);
            else
                return false;
        }
    }

    public class Wall : LevelPart
    {
        public Wall(Index2D start, Index2D end, Orientations or, TileTypes t, Element theme = Element.DEFAULT, bool move = false)
        {
            StartCell = start;
            EndCell = end;
            Type = t;
            Theme = theme;
            Traversable = move;
            Orientation = or;
        }

        public Wall(Index2D start)
        {
            StartCell = start;
        }

        public Wall(BorderWalls b)
        {
            StartCell = b.StartCell;
            EndCell = b.EndCell;
            Type = b.Type;
            Theme = b.Theme;
            Traversable = false;
        }

        public override XElement ToXML()
        {
            XElement e = new XElement("Wall");
            e.SetAttributeValue("Type", Type);
            e.SetAttributeValue("StartCell", StartCell);
            e.SetAttributeValue("EndCell", EndCell);
            e.SetAttributeValue("Orientation", Orientation);
            e.SetAttributeValue("Collision", Traversable);
            e.SetAttributeValue("Theme", Theme);
            return e;
        }

        public static Wall Parse(XElement elem)
        {
#if DEBUG1
            try
            {
#endif
            return new Wall(
                Index2D.Parse(elem.Attribute("StartCell").Value),
                Index2D.Parse(elem.Attribute("EndCell").Value),
                (Orientations)Enum.Parse(typeof(Orientations), elem.Attribute("Orientation").Value),
                (TileTypes)Enum.Parse(typeof(TileTypes), elem.Attribute("Type").Value),
                (Element)Enum.Parse(typeof(Element), elem.Attribute("Theme").Value),
                bool.Parse(elem.Attribute("Collision").Value)
                );
#if DEBUG1
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
                return null;
            }
#endif
        }

        public override LevelPart AsLevelPart()
        {
            return this;
        }
    }

    public class Floor : LevelPart
    {
        public Floor(Index2D start, Index2D end, TileTypes t, Element theme, bool move = true, TileTypes f = TileTypes.Floor)
        {
            StartCell = start;
            EndCell = end;
            Type = t;
            Theme = theme;
            Traversable = move;
            FloorType = f;
        }

        public Floor(Index2D start)
        {
            StartCell = start;
        }

        public Floor(Room r)
        {
            Index2D start = new Index2D(Math.Min(r.StartCell.X, r.EndCell.X), Math.Min(r.StartCell.Y, r.EndCell.Y));
            Index2D end = new Index2D(Math.Max(r.StartCell.X, r.EndCell.X), Math.Max(r.StartCell.Y, r.EndCell.Y));
            StartCell = new Index2D(start.X + 1, start.Y + 1);
            EndCell = new Index2D(end.X - 1, end.Y - 1);
            Type = r.Type;
            Theme = r.Theme;
            Traversable = r.FloorType == TileTypes.Floor ? true : false;
            FloorType = r.FloorType;
            Type = r.FloorType == TileTypes.Floor ? TileTypes.Floor : r.FloorType == TileTypes.Lava ? TileTypes.Lava : TileTypes.Water;
        }

        public override XElement ToXML()
        {
            XElement e = new XElement("Floor");
            e.SetAttributeValue("Type", Type);
            e.SetAttributeValue("StartCell", StartCell);
            e.SetAttributeValue("EndCell", EndCell);
            e.SetAttributeValue("Collision", Traversable);
            e.SetAttributeValue("Theme", Theme);
            return e;
        }

        public static Floor Parse(XElement elem)
        {
#if DEBUG1
            try
            {
#endif
            return new Floor(
                Index2D.Parse(elem.Attribute("StartCell").Value),
                Index2D.Parse(elem.Attribute("EndCell").Value),
                (TileTypes)Enum.Parse(typeof(TileTypes), elem.Attribute("Type").Value),
                (Element)Enum.Parse(typeof(Element), elem.Attribute("Theme").Value),
                bool.Parse(elem.Attribute("Collision").Value)
                );
#if DEBUG1
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
                return null;
            }
#endif
        }

        public override LevelPart AsLevelPart()
        {
            return this;
        }
    }


    public class BorderWalls : LevelPart
    {
        public BorderWalls(Index2D start, Index2D end, TileTypes t, Element theme, bool move = true)
        {
            StartCell = start;
            EndCell = end;
            Type = t;
            Theme = theme;
            Traversable = move;
        }

        public BorderWalls(Index2D start)
        {
            StartCell = start;
        }

        public BorderWalls(Room r)
        {
            StartCell = r.StartCell;
            EndCell = r.EndCell;
            Type = r.Type;
            Theme = r.Theme;
            Traversable = false;
        }

        public override XElement ToXML()
        {
            XElement e = new XElement("Walls");
            e.SetAttributeValue("Type", Type);
            e.SetAttributeValue("StartCell", StartCell);
            e.SetAttributeValue("EndCell", EndCell);
            e.SetAttributeValue("Collision", Traversable);
            e.SetAttributeValue("Theme", Theme);
            return e;
        }

        public static BorderWalls Parse(XElement elem)
        {
#if DEBUG1
            try
            {
#endif
            return new BorderWalls(
                Index2D.Parse(elem.Attribute("StartCell").Value),
                Index2D.Parse(elem.Attribute("EndCell").Value),
                (TileTypes)Enum.Parse(typeof(TileTypes), elem.Attribute("Type").Value),
                (Element)Enum.Parse(typeof(Element), elem.Attribute("Theme").Value),
                bool.Parse(elem.Attribute("Collision").Value)
                );
#if DEBUG1
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
                return null;
            }
#endif
        }

        public override LevelPart AsLevelPart()
        {
            return this;
        }
    }

    public class Room : LevelPart
    {
        public Room(Index2D start, Index2D end, Element theme, TileTypes f = TileTypes.Floor, bool move = true)
        {
            StartCell = start;
            EndCell = end;
            FloorType = f;
            Theme = theme;
            Traversable = move;
        }

        public Room(Index2D start)
        {
            StartCell = start;
        }

        public override XElement ToXML()
        {
            XElement e = new XElement("Room");
            e.SetAttributeValue("Type", Type);
            e.SetAttributeValue("StartCell", StartCell);
            e.SetAttributeValue("EndCell", EndCell);
            e.SetAttributeValue("Collision", Traversable);
            e.SetAttributeValue("Theme", Theme);
            return e;
        }

        public static Room Parse(XElement elem)
        {
#if DEBUG1
            try
            {
#endif
            if (elem.Attribute("FloorType") != null && elem.Attribute("Collision") != null)
                return new Room(
                    Index2D.Parse(elem.Attribute("StartCell").Value),
                    Index2D.Parse(elem.Attribute("EndCell").Value),
                    //(TileTypes)Enum.Parse(typeof(TileTypes), elem.Attribute("Type").Value),
                    (Element)Enum.Parse(typeof(Element), elem.Attribute("Theme").Value),
                    (TileTypes)Enum.Parse(typeof(TileTypes), elem.Attribute("FloorType").Value),
                    bool.Parse(elem.Attribute("Collision").Value)
                );
            else if (elem.Attribute("FloorType") != null)
                return new Room(
                    Index2D.Parse(elem.Attribute("StartCell").Value),
                    Index2D.Parse(elem.Attribute("EndCell").Value),
                    //(TileTypes)Enum.Parse(typeof(TileTypes), elem.Attribute("Type").Value),
                    (Element)Enum.Parse(typeof(Element), elem.Attribute("Theme").Value),
                    (TileTypes)Enum.Parse(typeof(TileTypes), elem.Attribute("FloorType").Value)
                );
            else
                return new Room(
                    Index2D.Parse(elem.Attribute("StartCell").Value),
                    Index2D.Parse(elem.Attribute("EndCell").Value),
                    //(TileTypes)Enum.Parse(typeof(TileTypes), elem.Attribute("Type").Value),
                    (Element)Enum.Parse(typeof(Element), elem.Attribute("Theme").Value)
                );

#if DEBUG1
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
                return null;
            }
#endif
        }

        public override LevelPart AsLevelPart()
        {
            return this;
        }
    }

    public interface ILevelSaveable
    {
        XElement ToXML();
    }

    public class LevelObject : ILevelSaveable
    {
        public TileTypes Type { get; set; }

        public Element Theme { get; set; }

        public bool Traversable { get; set; }

        public Orientations Orientation { get; set; }

        public TileTypes FloorType { get; set; }

        public Index2D GridCell { get; set; }

        public string InstanceName { get; set; }

        public virtual XElement ToXML()
        {
            return new XElement("Level");
        }

        public static bool operator ==(LevelObject a, LevelObject b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }
            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            return a.Type == b.Type && a.Theme == b.Theme && a.Traversable == b.Traversable && a.Orientation == b.Orientation && a.FloorType == b.FloorType && a.GridCell.X == b.GridCell.X && a.GridCell.Y == b.GridCell.Y && a.InstanceName == b.InstanceName && a.GetType() == b.GetType();
        }
        public static bool operator !=(LevelObject a, LevelObject b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is LevelObject)
                return this == (obj as LevelObject);
            else
                return false;
        }
    }

    public abstract class LevelPart : LevelObject
    {
        public Index2D StartCell
        {
            get
            {
                return GridCell;
            }
            set
            {
                GridCell = value;
            }
        }

        public Index2D EndCell { get; set; }

        public abstract LevelPart AsLevelPart();
    }
}
