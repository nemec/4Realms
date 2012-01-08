using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte
{
    internal delegate TriggerEventArgs TriggerCondition();
    internal delegate void TriggerHandler(object sender, TriggerEventArgs args);

    internal class TriggerEventArgs : EventArgs
    {
    }

    internal class TriggerSingleTargetEventArgs : TriggerEventArgs
    {
        internal TriggerSingleTargetEventArgs(Sprite target){
            Target = target;
        }
        internal Sprite Target;
    }

    internal class TriggerMultipleEventArgs : TriggerEventArgs
    {
        internal TriggerMultipleEventArgs(List<Sprite> targets)
        {
            Targets = targets;
        }
        internal List<Sprite> Targets;
    }

    internal class Trigger : OurSprite
    {
        internal Trigger(string name, int width, int height)
            : base(name, new Actor(new DummyAnimation(name, width, height)))
        {
            GroundPos = CenterPos;
            UpdateBehavior += Update;
        }

        internal Behavior TriggerUpdate = () => { };
        internal TriggerCondition TriggerCondition = () => { return null; };
        internal event TriggerHandler TriggerEffect = (Object, TriggerSingleTargetEventArgs) => { };
        internal bool Enabled = true;
        

        private new void Update(){
            if (Enabled)
            {
                TriggerUpdate();
                TriggerEventArgs args = TriggerCondition();
                if (args != null)
                {
                    TriggerEffect(this, args);
                }
            }
        }
    }

    internal class PartyCrossTrigger : Trigger
    {
        internal PartyCrossTrigger(string name, int width, int height, List<Sprite> party)
            : base(name, width, height)
        {
            this.party = party;
            base.TriggerUpdate += TriggerUpdate;
            base.TriggerCondition += TriggerCondition;
            base.TriggerEffect += TriggerEffect;

            triggered = new Dictionary<Sprite, bool>();
            foreach (Sprite s in party)
            {
                triggered.Add(s, false);
            }

            triggerRect = new Rectangle(
                0, 
                0,
                (int)(GetAnimation().Width * triggerRectScale),
                (int)(GetAnimation().Height * triggerRectScale));
        }

        internal PartyCrossTrigger(string name, Vector2 initialPosition)
            : this(name, Tile.TileSize * 3, Tile.TileSize, (This.Game.CurrentLevel as FrostbyteLevel).allies)
        {
            Orientation = Orientations.Up;
            Pos = initialPosition;
        }

        private List<Sprite> party;
        private Dictionary<Sprite, bool> triggered;
        private float triggerRectScale = 0.6f;
        private Rectangle triggerRect;

        private float cross(Vector3 v, Vector3 w)
        {
            return v.X * w.Y - v.Y * w.X;
        }

        private new void TriggerUpdate()
        {
            triggerRect.X = (int)(Pos.X + GetAnimation().Width * (1 - triggerRectScale));
            triggerRect.Y = (int)(Pos.Y + GetAnimation().Height * (1 - triggerRectScale));
            foreach (Sprite target in this.GetTargetsInRectangle(party, triggerRect))
            {
                // http://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect/565282#565282
                Vector3 normalDir = Vector3.Cross(new Vector3(this.Direction, 0), new Vector3(0, 0, -1));
                Vector3 targetDir = new Vector3(target.Pos - target.PreviousPos, 0);
                targetDir.Normalize();
                Vector3 pmq = new Vector3(target.PreviousPos + target.Center, 0) - new Vector3(this.Pos + this.Center, 0);

                float ta = cross(pmq, targetDir);
                float tb = cross(normalDir, targetDir);
                float t = ta / tb;
                float ua = cross(pmq, normalDir);
                float ub = cross(normalDir, targetDir);
                float u = (ua / ub);

                
                if (u > 0)
                {
                    float cos = this.Direction.X * targetDir.X + this.Direction.Y * targetDir.Y;

                    // Has crossed
                    if (cos > 0)
                    {
                        if (triggered.ContainsKey(target))
                        {
                            triggered[target] = true;
                        }
                    }
                    // Has uncrossed
                    else
                    {
                        if (triggered.ContainsKey(target))
                        {
                            triggered[target] = false;
                        }
                    }
                }
            }
        }

        private new TriggerMultipleEventArgs TriggerCondition()
        {
            if (triggered.Values.All(on => on))
                {
                    return new TriggerMultipleEventArgs(triggered.Keys.ToList());
                }
                return null;
        }

        private new void TriggerEffect(object ths, TriggerEventArgs args)
        {
            Obstacle rockM = new Obstacles.Rock("rockM");
            Obstacle rockL = new Obstacles.PartialRock("rockL");
            Obstacle rockR = new Obstacles.PartialRock("rockR");
            rockL.Orientation = Orientations.Left;
            rockL.Orientation = Orientations.Right;

            rockM.CenterOn(this);
            rockL.CenterOn(this);
            rockR.CenterOn(this);

            rockM.Pos.Y += Tile.TileSize * Math.Sign(-Direction.Y);
            rockL.Pos.Y += Tile.TileSize * Math.Sign(-Direction.Y);
            rockR.Pos.Y += Tile.TileSize * Math.Sign(-Direction.Y);

            Vector3 normalDir = Vector3.Cross(new Vector3(this.Direction, 0), new Vector3(0, 0, -1));

            rockL.Pos.X -= Tile.TileSize * Math.Sign(-normalDir.X);
            rockR.Pos.X += Tile.TileSize * Math.Sign(-normalDir.X);

            this.Enabled = false;
        }
    }
}
