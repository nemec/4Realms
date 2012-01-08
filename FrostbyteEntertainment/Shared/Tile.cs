using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

/// \file Tile.cs This is Shared with the Level Editor
namespace Frostbyte
{
    public partial class Tile : LevelObject
    {
        public static readonly int TileSize = 64;

        public Tile()
        {
            Traversable = true;
            FloorType = TileTypes.DEFAULT;
            Type = TileTypes.DEFAULT;
            InstanceName = null;
            Theme = Element.DEFAULT;
        }

        public override XElement ToXML()
        {
            XElement e = new XElement("Tile");
            e.SetAttributeValue("Type", Type);
            if (InstanceName != null)
                e.SetAttributeValue("InstanceName", InstanceName);
            e.SetAttributeValue("Collision", Traversable);
            e.SetAttributeValue("Theme", Theme);
            e.SetAttributeValue("Orientation", Orientation);
            e.SetAttributeValue("GridCell", GridCell);
            return e;
        }

        public static Tile Parse(XElement elem)
        {
#if DEBUG1
            try
            {
#endif
            Tile t = new Tile();
            foreach (XAttribute attr in elem.Attributes())
            {
                if (attr.Name == "Type")
                {
                    t.Type = (TileTypes)Enum.Parse(typeof(TileTypes), attr.Value);
                }
                else if (attr.Name == "InstanceName")
                {
                    t.InstanceName = attr.Value;
                }
                else if (attr.Name == "Collision")
                {
                    t.Traversable = bool.Parse(attr.Value);
                }
                else if (attr.Name == "Theme")
                {
                    t.Theme = (Element)Enum.Parse(typeof(Element), attr.Value);
                }
                else if (attr.Name == "Orientation")
                {
                    t.Orientation = (Orientations)Enum.Parse(typeof(Orientations), attr.Value);
                }
                else if (attr.Name == "GridCell")
                {
                    t.GridCell = Index2D.Parse(attr.Value);
                }
            }
            return t;
#if DEBUG1
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
                return null;
            }
#endif

        }
    }
}


