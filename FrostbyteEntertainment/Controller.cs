using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Frostbyte
{
    internal enum ReleasableButtonState
    {
        /// <summary>
        /// Button is held down.
        /// </summary>
        Pressed,
        /// <summary>
        /// Button is not pressed.
        /// </summary>
        Released,
        /// <summary>
        /// Button was pressed and then released.
        /// </summary>
        Clicked
    }

    interface IController
    {
        void Update();
        bool IsConnected { get; }

        /// <summary>
        /// Determines whether or not the Earth element was selected on the controller
        /// </summary>
        ReleasableButtonState Earth { get; }

        /// <summary>
        /// Determines whether or not the Fire element was selected on the controller
        /// </summary>
        ReleasableButtonState Fire { get; }

        /// <summary>
        /// Determines whether or not the Water element was selected on the controller
        /// </summary>
        ReleasableButtonState Water { get; }

        /// <summary>
        /// Determines whether or not the Lightning element was selected by the controller.
        /// </summary>
        ReleasableButtonState Lightning { get; }


        /// <summary>
        /// Determines when the Left Trigger is releases so we know what attack tier to use.
        /// </summary> 
        ReleasableButtonState LaunchAttack { get; }

        /// <summary>
        /// Determines whether or not the button for cancelling targeting was pressed
        /// </summary>
        ReleasableButtonState CancelTargeting { get; }

        /// <summary>
        /// Determines whether or not the button for interacting with the environment was pressed.
        /// </summary>
        ReleasableButtonState Interact { get; }

        /// <summary>
        /// The state of the "start" button
        /// </summary>
        ReleasableButtonState Start { get; }

        /// <summary>
        /// Provides a float representing how far down the sword trigger is pressed.
        /// </summary>
        float Sword { get; }

        /// <summary>
        /// Determines whether or not the button for targeting allies was pressed
        /// </summary>
        bool TargetAllies { get; }

        /// <summary>
        /// Determines whether or not the button for targeting enemies was pressed
        /// </summary>
        bool TargetEnemies { get; }

        /// <summary>
        /// Returns the value of the left joystick
        /// </summary>
        Vector2 Movement { get; }

        /// <summary>
        /// Return the state of a pressed key. If there are no "keys" on the
        /// controller, return ReleasableButtonState.Released
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <returns></returns>
        ReleasableButtonState GetKeypress(Keys key);

        /// <summary>
        /// Return the raw current state of the controller.
        /// Context dependent, must be casted to base type.
        /// </summary>
        /// <returns></returns>
        Object GetRawState();

        ReleasableButtonState NextLevel { get; }
        ReleasableButtonState CancelSpell { get; }
    }

    class GamePadController : IController
    {
        #region Constructors
        internal GamePadController(PlayerIndex ix)
        {
            input = ix;
            mCurrentControllerState = GamePad.GetState(input);
            mLastControllerState = mCurrentControllerState;
        }
        #endregion

        #region Variables
        private PlayerIndex input;
        internal GamePadState mLastControllerState;
        protected GamePadState mCurrentControllerState;
        private float InteractElementThreshold = 0.1f;
        #endregion

        #region Methods
        public void Update()
        {
            mLastControllerState = mCurrentControllerState;
            mCurrentControllerState = GamePad.GetState(input);
        }

        public ReleasableButtonState GetKeypress(Keys key)
        {
            return ReleasableButtonState.Released;
        }

        public Object GetRawState()
        {
            return mCurrentControllerState;
        }
        #endregion

        #region Properties
        private GamePadButtons LastButtons { get { return mLastControllerState.Buttons; } }
        private GamePadButtons CurrentButtons { get { return mCurrentControllerState.Buttons; } }

        public bool IsConnected { get { return mCurrentControllerState.IsConnected; } }


        public ReleasableButtonState Earth
        {
            get
            {
                // Only trigger spells when Left Trigger is pressed
                if (mCurrentControllerState.Triggers.Left > InteractElementThreshold)
                {
                    if (CurrentButtons.A == ButtonState.Pressed)
                    {
                        return ReleasableButtonState.Pressed;
                    }
                    else if (LastButtons.A == ButtonState.Pressed && CurrentButtons.A == ButtonState.Released)
                    {
                        return ReleasableButtonState.Clicked;
                    }
                }

                return ReleasableButtonState.Released;
            }
        }

        public ReleasableButtonState Fire
        {
            get
            {
                // Only trigger spells when Left Trigger is pressed
                if (mCurrentControllerState.Triggers.Left > InteractElementThreshold)
                {
                    if (CurrentButtons.B == ButtonState.Pressed)
                    {
                        return ReleasableButtonState.Pressed;
                    }
                    else if (LastButtons.B == ButtonState.Pressed && CurrentButtons.B == ButtonState.Released)
                    {
                        return ReleasableButtonState.Clicked;
                    }
                }

                return ReleasableButtonState.Released;
            }
        }

        public ReleasableButtonState Water
        {
            get
            {
                // Only trigger spells when Left Trigger is pressed
                if (mCurrentControllerState.Triggers.Left > InteractElementThreshold)
                {
                    if (CurrentButtons.X == ButtonState.Pressed)
                    {
                        return ReleasableButtonState.Pressed;
                    }
                    else if (LastButtons.X == ButtonState.Pressed && CurrentButtons.X == ButtonState.Released)
                    {
                        return ReleasableButtonState.Clicked;
                    }
                }

                return ReleasableButtonState.Released;
            }
        }

        public ReleasableButtonState Lightning
        {
            get
            {
                // Only trigger spells when Left Trigger is pressed
                if (mCurrentControllerState.Triggers.Left > InteractElementThreshold)
                {
                    if (CurrentButtons.Y == ButtonState.Pressed)
                    {
                        return ReleasableButtonState.Pressed;
                    }
                    else if (LastButtons.Y == ButtonState.Pressed && CurrentButtons.Y == ButtonState.Released)
                    {
                        return ReleasableButtonState.Clicked;
                    }
                }

                return ReleasableButtonState.Released;
            }
        }

        public ReleasableButtonState LaunchAttack
        {
            get
            {
                // Only trigger spells when Left Trigger is pressed
                if (mCurrentControllerState.Triggers.Left > InteractElementThreshold)
                {
                    return ReleasableButtonState.Pressed;   
                }

                else if (mLastControllerState.Triggers.Left > InteractElementThreshold && mCurrentControllerState.Triggers.Left < InteractElementThreshold)
                {
                    return ReleasableButtonState.Clicked;
                }

                return ReleasableButtonState.Released;
            }
        }

        public ReleasableButtonState Interact
        {
            get
            {
                // Only trigger spells when Left Trigger is pressed
                if (mCurrentControllerState.Triggers.Left <= InteractElementThreshold)
                {
                    if (CurrentButtons.A == ButtonState.Pressed)
                    {
                        return ReleasableButtonState.Pressed;
                    }
                    else if (LastButtons.A == ButtonState.Pressed && CurrentButtons.A == ButtonState.Released)
                    {
                        return ReleasableButtonState.Clicked;
                    }
                }

                return ReleasableButtonState.Released;
            }
        }

        public ReleasableButtonState Start
        {
            get
            {
                if (CurrentButtons.Start == ButtonState.Pressed)
                {
                    return ReleasableButtonState.Pressed;
                }
                else if (LastButtons.Start == ButtonState.Pressed && CurrentButtons.Start == ButtonState.Released)
                {
                    return ReleasableButtonState.Clicked;
                }
                else
                {
                    return ReleasableButtonState.Released;
                }
            }
        }

        public ReleasableButtonState CancelTargeting
        {
            get
            {
                // Only trigger spells when Left Trigger is pressed
                if (mCurrentControllerState.Triggers.Left <= InteractElementThreshold)
                {
                    if (CurrentButtons.B == ButtonState.Pressed)
                    {
                        return ReleasableButtonState.Pressed;
                    }
                    else if (LastButtons.B == ButtonState.Pressed && CurrentButtons.B == ButtonState.Released)
                    {
                        return ReleasableButtonState.Clicked;
                    }
                }

                return ReleasableButtonState.Released;
            }
        }

        public bool TargetAllies
        {
            get
            {
                return LastButtons.LeftShoulder == ButtonState.Pressed &&
                    CurrentButtons.LeftShoulder == ButtonState.Released;
            }
        }

        public bool TargetEnemies
        {
            get
            {
                return LastButtons.RightShoulder == ButtonState.Pressed &&
                    CurrentButtons.RightShoulder == ButtonState.Released;
            }
        }

        public float Sword
        {
            get
            {
                return mCurrentControllerState.Triggers.Right;
            }
        }

        public Vector2 Movement
        {
            get
            {
                return mCurrentControllerState.ThumbSticks.Left;
            }
        }

        public ReleasableButtonState NextLevel
        {
            get
            {
                if (CurrentButtons.LeftStick == ButtonState.Pressed)
                {
                    return ReleasableButtonState.Pressed;
                }
                else if (LastButtons.LeftStick == ButtonState.Pressed && CurrentButtons.LeftStick == ButtonState.Released)
                {
                    return ReleasableButtonState.Clicked;
                }
                else
                {
                    return ReleasableButtonState.Released;
                }
            }
        }

        public ReleasableButtonState CancelSpell
        {
            get
            {
                if (CurrentButtons.RightStick == ButtonState.Pressed)
                {
                    return ReleasableButtonState.Pressed;
                }
                else if (LastButtons.RightStick == ButtonState.Pressed && CurrentButtons.RightStick == ButtonState.Released)
                {
                    return ReleasableButtonState.Clicked;
                }
                else
                {
                    return ReleasableButtonState.Released;
                }
            }
        }

        #endregion
    }

    class KeyboardController : IController
    {
        #region Constructors
        internal KeyboardController()
        {
            mCurrentControllerState = Keyboard.GetState();
            mLastControllerState = mCurrentControllerState;
        }
        #endregion

        #region Variables
        internal KeyboardState mLastControllerState;
        protected KeyboardState mCurrentControllerState;
        #endregion

        #region Methods
        public void Update()
        {
            mLastControllerState = mCurrentControllerState;
            mCurrentControllerState = Keyboard.GetState();
        }

        public ReleasableButtonState GetKeypress(Keys key)
        {
            if (mCurrentControllerState.IsKeyDown(key))
            {
                return ReleasableButtonState.Pressed;
            }
            else if (mLastControllerState.IsKeyDown(key) && mCurrentControllerState.IsKeyUp(key))
            {
                return ReleasableButtonState.Clicked;
            }
            return ReleasableButtonState.Released;
        }

        public Object GetRawState()
        {
            return mCurrentControllerState;
        }
        #endregion

        #region Properties
        public bool IsConnected { get { return true; } }

        public ReleasableButtonState Earth
        {
            get
            {
                return GetKeypress(Keys.S);
            }
        }

        public ReleasableButtonState Fire
        {
            get
            {
                return GetKeypress(Keys.D);
            }
        }

        public ReleasableButtonState Water
        {
            get
            {
                return GetKeypress(Keys.A);
            }
        }

        public ReleasableButtonState Lightning
        {
            get
            {
                return GetKeypress(Keys.W);
            }
        }

        public ReleasableButtonState LaunchAttack
        {
            get
            {
                return GetKeypress(Keys.Space);
            }
        }

        public ReleasableButtonState Interact
        {
            get
            {
                return GetKeypress(Keys.Z);
            }
        }

        public ReleasableButtonState Start
        {
            get
            {
                return GetKeypress(Keys.Enter);
            }
        }

        public ReleasableButtonState CancelTargeting
        {
            get
            {
                return GetKeypress(Keys.C);
            }
        }

        public bool TargetAllies
        {
            get
            {
                return mLastControllerState.IsKeyDown(Keys.Q) && mCurrentControllerState.IsKeyUp(Keys.Q);
            }
        }

        public bool TargetEnemies
        {
            get
            {
                return mLastControllerState.IsKeyDown(Keys.E) && mCurrentControllerState.IsKeyUp(Keys.E);
            }
        }

        public float Sword
        {
            get
            {
                return mLastControllerState.IsKeyDown(Keys.LeftShift) &&
                        mCurrentControllerState.IsKeyUp(Keys.LeftShift) ? 1 : 0;
            }
        }

        public Vector2 Movement
        {
            get
            {
                Vector2 move = Vector2.Zero;
                if (mLastControllerState.IsKeyDown(Keys.Right))
                {
                    move.X = 1;
                }
                else if (mLastControllerState.IsKeyDown(Keys.Left))
                {
                    move.X = -1;
                }

                if (mLastControllerState.IsKeyDown(Keys.Down))
                {
                    move.Y = -1;
                }
                else if (mLastControllerState.IsKeyDown(Keys.Up))
                {
                    move.Y = 1;
                }

                return move;
            }
        }

        public ReleasableButtonState NextLevel
        {
            get
            {
                return GetKeypress(Keys.F12);
            }
        }

        public ReleasableButtonState CancelSpell
        {
            get
            {
                return GetKeypress(Keys.X);
            }
        }

        #endregion
    }
}
