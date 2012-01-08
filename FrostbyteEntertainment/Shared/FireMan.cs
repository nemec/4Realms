using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Frostbyte.Enemies
{
    internal partial class FireMan : Frostbyte.Boss
    {
        internal override XElement ToXML()
        {
            XElement e = new XElement("Enemy");
            e.SetAttributeValue("Type", this.GetType().ToString());
            e.SetAttributeValue("Name", Name);
            e.SetAttributeValue("Speed", Speed);
            e.SetAttributeValue("Health", Health);
            //add other data about this type of enemy here
            return e;
        }
    }
}
