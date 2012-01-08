using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;

namespace Frostbyte
{
#if LEVELEDITOR
    //seen by editor
    internal partial class Enemy : OurSprite
    {
        public string Name { get; set; }

        /// <summary>
        /// the sprite's speed
        /// </summary>
        internal float Speed { get; set; }

        /// <summary>
        /// make sure we take into account the centerpoint when we use this
        /// </summary>
        public Index2D Pos = new Index2D();

#else
    //seen by other things
    internal abstract partial class Enemy : OurSprite
    {
        /// <summary>
        /// Turns the object into a line of xml
        /// </summary>
        /// <returns>XML representing the object</returns>
        //internal virtual XElement ToXML()
        //{
        //    return  new XElement("Enemy");
        //}
#endif
        public Type EnemyType { get; set; }

        //seen by both
        /// <summary>
        /// Turns the object into a line of xml
        /// </summary>
        /// <returns>XML representing the object</returns>
        internal virtual XElement ToXML()
        {
            XElement e = new XElement("Enemy");
            e.SetAttributeValue("Type", EnemyType);
            e.SetAttributeValue("Name", Name);
            e.SetAttributeValue("Speed", Speed);
            e.SetAttributeValue("Health", Health);
            e.SetAttributeValue("Pos", Pos);
            //add other data about this type of enemy here
            return e;
        }
        
    }

    internal abstract partial class Boss : Enemy
    {

    }
}
