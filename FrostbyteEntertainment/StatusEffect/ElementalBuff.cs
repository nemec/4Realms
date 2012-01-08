using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frostbyte
{
    internal class ElementalBuff : StatusEffect
    {
        internal ElementalBuff(Element e)
            : base(new TimeSpan(0, 0, 42), LevelFunctions.DoNothing)
        {
            Element = e;
            #region Particles
            Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
            Texture2D element = This.Game.CurrentLevel.GetTexture("regen");
            switch (e)
            {
                case Frostbyte.Element.Earth:
                    element = This.Game.CurrentLevel.GetTexture("dirtParticle");
                    break;
                case Frostbyte.Element.Lightning:
                    element = This.Game.CurrentLevel.GetTexture("sparkball");
                    break;
                case Frostbyte.Element.Water:
                    element = This.Game.CurrentLevel.GetTexture("waterParticle");
                    break;
                case Frostbyte.Element.Fire:
                    element = This.Game.CurrentLevel.GetTexture("fireParticle");
                    break;
            }
            particleEmitter = new ParticleEmitter(800, particleEffect, element);
            particleEmitter.effectTechnique = "NoSpecialEffect";
            switch (Element)
            {
                case Frostbyte.Element.Earth:
                    particleEmitter.blendState = BlendState.AlphaBlend;

                    break;
                case Frostbyte.Element.Lightning:
                    particleEmitter.blendState = BlendState.Additive;

                    break;
                case Frostbyte.Element.Water:
                    particleEmitter.blendState = BlendState.Additive;

                    break;
                case Frostbyte.Element.Fire:
                    particleEmitter.blendState = BlendState.AlphaBlend;

                    break;
            }
            #endregion
        }

        internal Element Element = Element.Normal;

        internal override void Draw(Sprite target)
        {
            base.Draw(target);
            if (count % 5 == 0)
            {
                Random rand = new Random();
                int numLayers = 1;
                int size = 36;
                int scale = size;
                switch (Element)
                {
                    case Frostbyte.Element.Earth:
                        scale /= 5;
                        break;
                    case Frostbyte.Element.Lightning:
                        scale /= 2;
                        break;
                    case Frostbyte.Element.Water:
                        scale /= 2;
                        break;
                    case Frostbyte.Element.Fire:
                        scale /= 2;
                        break;
                }
                for (int i = 0; i < numLayers; i++)
                {
                    double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                    Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                    Vector2 velocity = new Vector2(This.Game.rand.Next(-10, 10), -10);
                    Vector2 acceleration = new Vector2(This.Game.rand.Next(-10, 10), -10);
                    int radius = size;

                    velocity.Normalize();
                    particleEmitter.createParticles(velocity,
                                                    acceleration,
                                                    target.GroundPos + randDirection * This.Game.rand.Next(0, radius),
                                                    scale,
                                                    This.Game.rand.Next(100, 500));
                }
            }
            count++;
        }
    }
}
