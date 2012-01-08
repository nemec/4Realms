using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    internal class DeathEffect : Sprite
    {
        #region Variables
        private Sprite deadSprite;
        private ParticleEmitter particleEmitter;
        private Color[] image;
        private Rectangle imageRegion;
        SpriteFrame frame;
        private float scale;
        private Vector2 topLeftPosition;
        int frameWidth;
        int frameHeight;
        #endregion Variables

        internal DeathEffect(Sprite _deadSprite, ParticleEmitter _particleEmitter, float sampleWidthPercent, float sampleHeightPercent)
            : base("DeathEffect", new Actor(new DummyAnimation()))
        {
            deadSprite = _deadSprite;
            particleEmitter = _particleEmitter;
            
            particleEmitter.ZOrder = deadSprite.ZOrder;

            //Scale of Sprite
            scale = deadSprite.Scale;

            //Sprite Frame
            frame = deadSprite.GetAnimation();

            //Store frame width and height
            frameWidth = (int)((float)frame.Width);
            frameHeight = (int)((float)frame.Height);

            //Region of Texture that the current frame is in
            imageRegion = new Rectangle((int)frame.StartPos.X, (int)frame.StartPos.Y, frameWidth, frameHeight);

            //Sprite Color Data as Array from Texture2D
            image = new Color[frameWidth * frameHeight];
            frame.Image.GetData<Color>(0, imageRegion, image, 0, frameHeight * frameWidth);

            //Top left position of image (copied from Sprite Draw Function)
            topLeftPosition = deadSprite.Pos - deadSprite.GetAnimation().AnimationPeg +
                            deadSprite.Center - deadSprite.Center * scale + //this places scaling in the correct spot (i think)
                            (deadSprite.Hflip ? frame.MirrorOffset * 2 : -frame.MirrorOffset);

            createParticles(sampleWidthPercent, sampleHeightPercent);
        }

        private void createParticles(float sampleWidthPercent, float sampleHeightPercent)
        {
            for (float y = 0; y < frameHeight; y += sampleHeightPercent * (float)(frameHeight))
            {
                for (float x = 0; x < frameWidth; x += sampleWidthPercent * (float)(frameWidth))
                {
                    int floorX = (int)Math.Floor(x);
                    int floorY = (int)Math.Floor(y);

                    bool isGray = (Math.Abs((float)(image[floorX + floorY * frameWidth].B - image[floorX + floorY * frameWidth].R)) < 5 &&
                                   Math.Abs((float)(image[floorX + floorY * frameWidth].B - image[floorX + floorY * frameWidth].G)) < 5f);

                    if (image[floorX + floorY * frameWidth].A > 245 || (image[floorX + floorY * frameWidth].A <= 245f && !isGray))
                    {
                        double directionAngle = This.Game.rand.NextDouble() * 2 * Math.PI;
                        Vector2 randDirection = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle) / ParticleEmitter.EllipsePerspectiveModifier);
                        particleEmitter.createParticles(new Vector2(randDirection.X*25,105),
                                                        new Vector2(-randDirection.X*15,-155),
                                                        topLeftPosition + new Vector2((deadSprite.Hflip ? frameWidth - floorX : floorX) * scale, floorY * scale),
                                                        5,
                                                        This.Game.rand.Next(600,800));
                    }
                }
            }
        }

        internal override void Update()
        {
            if (particleEmitter.ActiveParticleCount <= 0)
            {
                particleEmitter.Remove();
                This.Game.CurrentLevel.RemoveSprite(this);
            }
        }
    }
}