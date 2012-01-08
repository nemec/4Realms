using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Frostbyte
{
    internal struct ParticleVertexFormat
    {
        private Vector2 initialPosition;
        private Vector2 initialvelocity;
        private Vector2 acceleration;
        private HalfVector2 textureCoord;
        private Single timeCreated; //in milliseconds
        private Single timeToLive; //in milliseconds

        internal ParticleVertexFormat(Vector2 initialPosition, Vector2 initialvelocity, Vector2 acceleration, HalfVector2 textureCoord, int timeCreated, int timeToLive)
        {
            this.initialPosition = initialPosition;
            this.initialvelocity = initialvelocity;
            this.acceleration = acceleration;
            this.textureCoord = textureCoord;
            this.timeCreated = timeCreated;
            this.timeToLive = timeToLive;
        }

        internal readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
                (
                    new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),                             //initialposition
                    new VertexElement(sizeof(float) * 2, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),    //initialvelocity
                    new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),    //acceleration
                    new VertexElement(sizeof(float) * 6, VertexElementFormat.HalfVector2, VertexElementUsage.TextureCoordinate, 2),//textureCoord
                    new VertexElement(sizeof(float) * 7, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 3),     //time created
                    new VertexElement(sizeof(float) * 8, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 4)      //time to live
                );
    }

    internal class ParticleEmitter : OurSprite
    {
        //Collision data
        internal List<CollisionObject> collisionObjects = new List<CollisionObject>();

        #region Private Variables

        private int maxNumOfParticles;                      //max number of particles (set in constructor)
        private ParticleVertexFormat[] particleVertices;    //list of all particles' vertices
        private int[] particleIndices;                      //list of all particles' indices
        private DynamicVertexBuffer vertexBuffer;           //vertex buffer stored on gpu
        private IndexBuffer indexBuffer;                    //index buffer stored on gpu
        private int[] expirationTime;                       //expiration time for each particle in milliseconds
        private int numOfActiveParticles;                   //number of active particles
        private const int sizeOfParticle = 36;              //size of particle in bytes

        //Set by property
        private string m_effectTechnique = "FadeAtXPercent";
        private float m_fadeStartPercent = .80f;
        private float m_changePicPercent = .80f;

        //World/View/Projection Matrices
        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        //Effect file for shaders
        private Effect effect;

        //Texture for particle
        private Texture2D texture1;
        private Texture2D texture2;

        #endregion Private Variables

        #region Internal Properties
        /// <summary>
        /// The ammount that we will scale off the y coordinate 
        /// Useage: circle(x,y/EllipsePerspectiveModifier)
        /// </summary>
        internal static float EllipsePerspectiveModifier { get { return 1.7f; } } 

        internal BlendState blendState = BlendState.AlphaBlend;

        /// <summary>
        /// Types are "FadeAtXPercent", "ChangePicAndFadeAtPercent", "NoSpecialEffect"
        /// </summary>
        internal string effectTechnique
        {
            get
            {
                return m_effectTechnique;
            }
            set
            {
                if (value == "FadeAtXPercent")
                    m_effectTechnique = value;
                else if (value == "ChangePicAndFadeAtPercent")
                    m_effectTechnique = value;
                else if (value == "NoSpecialEffect")
                    m_effectTechnique = value;
                else
                    m_effectTechnique = "FadeAtXPercent";
            }
        }

        /// <summary>
        /// Percent between 0 and 1
        /// </summary>
        internal float fadeStartPercent
        {
            get
            {
                return m_fadeStartPercent;
            }
            set
            {
                if (value < 0)
                    m_fadeStartPercent = 0.0f;
                else if (value > 1)
                    m_fadeStartPercent = 1.0f;
                else
                    m_fadeStartPercent = value;
                effect.Parameters["xFadeStartPercent"].SetValue(m_fadeStartPercent);
            }
        }

        /// <summary>
        /// Percent between 0 and 1
        /// </summary>
        internal float changePicPercent
        {
            get
            {
                return m_changePicPercent;
            }
            set
            {
                if (value < 0.0f)
                    m_changePicPercent = 0.0f;
                else if (value > 1.0f)
                    m_changePicPercent = 1.0f;
                else
                    m_changePicPercent = value;
                effect.Parameters["xChangePicPercent"].SetValue(m_changePicPercent);
            }
        }
        internal Behavior deathEffect = () => { };
        internal int ActiveParticleCount
        {
            get
            {
                return numOfActiveParticles;
            }
        }

        #endregion internal Properties

        internal ParticleEmitter(int maxNumOfParticles, Effect effect, Texture2D texture1, Texture2D texture2 = null)
            : base("particleEmitter", new Actor(new DummyAnimation()))
        {
            //This makes particles draw on top of all objects
            Static = true;

            //sets max number of particles 
            this.maxNumOfParticles = maxNumOfParticles;

            //instantiate array of particle vertices & indicies
            particleVertices = new ParticleVertexFormat[maxNumOfParticles * 4];
            particleIndices = new int[maxNumOfParticles * 6];

            //instantiate array of time to lives to all particles
            expirationTime = new int[maxNumOfParticles];

            numOfActiveParticles = 0;

            //Allocate memory on gpu for vertices & indicies
            vertexBuffer = new DynamicVertexBuffer(This.Game.GraphicsDevice, ParticleVertexFormat.VertexDeclaration, particleVertices.Length, BufferUsage.WriteOnly);
            indexBuffer = new IndexBuffer(This.Game.GraphicsDevice, typeof(int), particleIndices.Length, BufferUsage.WriteOnly);

            //create all indices (they never change)
            for (int i = 0; i < maxNumOfParticles; i++)
                setIndices(i);

            //give buffer's their data
            vertexBuffer.SetData(particleVertices);
            indexBuffer.SetData(particleIndices);

            //load Texture
            this.texture1 = texture1;
            this.texture2 = texture2;

            //load the HLSL code
            this.effect = effect.Clone();

            sendConstantsToGPU();

            collisionObjects.Add(new Collision_BoundingCircle(1, Vector2.Zero, 10));
            this.CollisionList = 2;
        }

        internal override void Update()
        {
            removeExpiredParticles();
        }

        private void sendConstantsToGPU()
        {
            effect.Parameters["xTexture1"].SetValue(texture1);
            effect.Parameters["xTexture2"].SetValue(texture2);
        }

        private void removeExpiredParticles()
        {
            int currentTime = (int)This.gameTime.TotalGameTime.TotalMilliseconds;

            for (int i = 0; i < numOfActiveParticles; i++)
            {
                if (expirationTime[i] <= currentTime)
                {
                    deleteParticle(i);
                }
            }
        }

        /// <summary>
        /// Create Particles
        /// </summary>
        /// <param name="startPosition">Position in Relation to World Coordinates</param>
        /// <param name="size">2 = 4x4 square; 10 = 20x20 square</param>
        /// <param name="timeToLive">Time to Live in Milliseconds</param>
        internal void createParticles(Vector2 velocity, Vector2 acceleration, Vector2 startPosition, float size, int timeToLive)
        {
            if (numOfActiveParticles < maxNumOfParticles)
            {
                //create particle vertices
                particleVertices[numOfActiveParticles * 4] = new ParticleVertexFormat(new Vector2(startPosition.X - size, startPosition.Y - size), velocity, acceleration, new HalfVector2(0, 0), (int)This.gameTime.TotalGameTime.TotalMilliseconds, timeToLive);
                particleVertices[numOfActiveParticles * 4 + 1] = new ParticleVertexFormat(new Vector2(startPosition.X + size, startPosition.Y - size), velocity, acceleration, new HalfVector2(1, 0), (int)This.gameTime.TotalGameTime.TotalMilliseconds, timeToLive);
                particleVertices[numOfActiveParticles * 4 + 2] = new ParticleVertexFormat(new Vector2(startPosition.X + size, startPosition.Y + size), velocity, acceleration, new HalfVector2(1, 1), (int)This.gameTime.TotalGameTime.TotalMilliseconds, timeToLive);
                particleVertices[numOfActiveParticles * 4 + 3] = new ParticleVertexFormat(new Vector2(startPosition.X - size, startPosition.Y + size), velocity, acceleration, new HalfVector2(0, 1), (int)This.gameTime.TotalGameTime.TotalMilliseconds, timeToLive);

                //set time to live
                expirationTime[numOfActiveParticles] = timeToLive + (int)This.gameTime.TotalGameTime.TotalMilliseconds;

                moveParticleToGPU(numOfActiveParticles);

                //increment number of particles
                numOfActiveParticles++;
            }
        }

        //Move last active particle to new location
        private void deleteParticle(int particleNumber)
        {
            int lastParticle = numOfActiveParticles - 1;
            particleVertices[particleNumber * 4] = particleVertices[lastParticle * 4];
            particleVertices[particleNumber * 4 + 1] = particleVertices[lastParticle * 4 + 1];
            particleVertices[particleNumber * 4 + 2] = particleVertices[lastParticle * 4 + 2];
            particleVertices[particleNumber * 4 + 3] = particleVertices[lastParticle * 4 + 3];

            expirationTime[particleNumber] = expirationTime[lastParticle];

            moveParticleToGPU(particleNumber);

            deathEffect();

            numOfActiveParticles--;
        }

        private void setIndices(int particleNumber)
        {
            //set particle indices
            particleIndices[particleNumber * 6] = particleNumber * 4;
            particleIndices[particleNumber * 6 + 1] = particleNumber * 4 + 1;
            particleIndices[particleNumber * 6 + 2] = particleNumber * 4 + 2;
            particleIndices[particleNumber * 6 + 3] = particleNumber * 4 + 2;
            particleIndices[particleNumber * 6 + 4] = particleNumber * 4 + 3;
            particleIndices[particleNumber * 6 + 5] = particleNumber * 4;
        }

        private void moveParticleToGPU(int particleNumber)
        {
            This.Game.GraphicsDevice.SetVertexBuffer(null);

            //overwrite vertex data on gpu
            vertexBuffer.SetData(sizeOfParticle * particleNumber * 4,       //start location on gpu buffer to overwrite
                                 particleVertices,                          //array to pull data from to overwrite gpu buffer
                                 particleNumber * 4,                        //first element in array to use
                                 4,                                         //number of elements in array to use
                                 sizeOfParticle,                            //size in bytes of one element in array
                                 SetDataOptions.None);                      //tell setData() to overwrite data
        }

        internal override void Draw(GameTime gameTime)
        {
            if (numOfActiveParticles > 0)
            {
                // Restore the vertex buffer contents if the graphics device was lost.
                if (vertexBuffer.IsContentLost)
                    vertexBuffer.SetData(particleVertices);

                //Set buffers on gpu
                This.Game.GraphicsDevice.SetVertexBuffer(vertexBuffer);
                This.Game.GraphicsDevice.Indices = indexBuffer;

                //Set blendstate
                This.Game.GraphicsDevice.BlendState = blendState;

                //Set the effect technique
                effect.CurrentTechnique = effect.Techniques[m_effectTechnique];

                //Create View and Projection Matrices
                viewMatrix = Matrix.CreateLookAt(new Vector3(This.Game.GraphicsDevice.Viewport.X + This.Game.GraphicsDevice.Viewport.Width / 2,
                                                             This.Game.GraphicsDevice.Viewport.Y + This.Game.GraphicsDevice.Viewport.Height / 2,
                                                             -10),
                                                 new Vector3(This.Game.GraphicsDevice.Viewport.X + This.Game.GraphicsDevice.Viewport.Width / 2,
                                                             This.Game.GraphicsDevice.Viewport.Y + This.Game.GraphicsDevice.Viewport.Height / 2,
                                                             0),
                                                 new Vector3(0, -1, 0));
                projectionMatrix = Matrix.CreateOrthographic(This.Game.GraphicsDevice.Viewport.Width, This.Game.GraphicsDevice.Viewport.Height, -50f, 50f);

                //These lines store data in variables on the graphics card
                effect.Parameters["xCurrentTime"].SetValue((int)gameTime.TotalGameTime.TotalMilliseconds);
                effect.Parameters["xWorldViewProjection"].SetValue(Matrix.Identity * This.Game.CurrentLevel.Camera.GetTransformation(This.Game.GraphicsDevice) * viewMatrix * projectionMatrix);

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    This.Game.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                                                                   particleVertices,
                                                                   0,
                                                                   numOfActiveParticles * 4,
                                                                   particleIndices,
                                                                   0,
                                                                   numOfActiveParticles * 2,
                                                                   ParticleVertexFormat.VertexDeclaration
                                                                   );
                }
            }
        }

        internal override List<CollisionObject> GetCollision()
        {
            return collisionObjects;
        }

        internal override List<Vector2> GetHotSpots()
        {
            return new List<Vector2>();
        }

        internal void Remove()
        {
            collisionObjects.Clear();
            maxNumOfParticles = 0;            
            particleVertices = null;
            particleIndices = null;
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            expirationTime = null; 
            numOfActiveParticles = 0;
            m_effectTechnique = null;
            m_fadeStartPercent = 0;
            m_changePicPercent = 0;
            This.Game.CurrentLevel.RemoveSprite(this);
        }
    }
}
