using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    internal partial class RockGolem : Golem
    {
        #region Variables
        static List<String> Animations = new List<String>(){
               "golem-idle-down.anim",
           "golem-idle-diagdown.anim",
           "golem-idle-right.anim",
           "golem-idle-diagup.anim",
           "golem-idle-up.anim",
           "golem-walk-down.anim",
           "golem-walk-diagdown.anim",
           "golem-walk-right.anim",
           "golem-walk-diagup.anim",
           "golem-walk-up.anim",
           "golem-attack-down.anim",
           "golem-attack-diagdown.anim",
           "golem-attack-right.anim",
           "golem-attack-diagup.anim",
           "golem-attack-up.anim",
        };
        #endregion Variables
        public RockGolem(string name, Vector2 initialPos)
            : base(name, initialPos, 200, Animations)
        {
            ElementType = Element.Earth;
        }
    }
}
