using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Frostbyte.Enemies
{
    internal partial class Beetle : Frostbyte.Enemy
    {
        //internal override XElement ToXML()
        //{
        //    XElement e = new XElement("Enemy");
        //    e.SetAttributeValue("Type", this.GetType().ToString());
        //    e.SetAttributeValue("Name", Name);
        //    e.SetAttributeValue("Speed", Speed);
        //    e.SetAttributeValue("Health", Health);
        //    e.SetAttributeValue("Pos", Pos);
        //    //add other data about this type of enemy here
        //    return e;
        //}

        internal static Beetle Parse(XElement e)
        {
#if LEVELEDITOR
            Beetle b = new Beetle();
            foreach (XAttribute attr in e.Attributes())
            {
                if (attr.Name == "Name")
                {
                    b.Name = attr.Value;
                }
                else if (attr.Name == "Health")
                {
                    b.Health = int.Parse(attr.Value);
                }
                else if (attr.Name == "Speed")
                {
                    b.Speed = float.Parse(attr.Value);
                }
                else if (attr.Name == "Pos")
                {
                    b.Pos = Index2D.Parse(attr.Value);

                }
            }
#else
            string name = e.Attribute("Name").Value;
            int health = int.Parse(e.Attribute("Health").Value);
            Index2D pos = Index2D.Parse(e.Attribute("Pos").Value);
            Microsoft.Xna.Framework.Vector2 initpos = new Microsoft.Xna.Framework.Vector2(pos.X, pos.Y);
            Beetle b = new Beetle(name, initpos);
#endif
            return b;
        }
    }
}
