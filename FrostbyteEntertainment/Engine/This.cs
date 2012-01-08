using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte
{
    internal static class This
    {
        internal static Game Game;
        internal static GameTime gameTime;

        internal static Cheats Cheats = new Cheats();

        internal static int CellSize = 64;
    }

    class Cheats
    {
        public interface ICheat{
            bool Enabled { get; }
            void Enable();
            void Disable();
            void Toggle();
        }

        internal Cheats()
        {
            AddCheat("SpawnEnemies", new DefaultCheat());
        }

        internal bool Enabled { get; set; }

        internal class NullCheat : ICheat
        {
            public bool Enabled
            {
                get
                {
                    return false;
                }
            }

            public void Enable()
            {
            }

            public void Disable()
            {
            }

            public void Toggle()
            {
            }
        }
        private NullCheat _NullCheat = new NullCheat();

        internal class DefaultCheat : ICheat
        {
            bool _enabled;

            public bool Enabled
            {
                get
                {
                    return _enabled;
                }
            }

            public void Enable()
            {
                _enabled = true;
            }

            public void Disable()
            {
                _enabled = false;
            }

            public void Toggle()
            {
                if (Enabled)
                {
                    Disable();
                }
                else
                {
                    Enable();
                }
            }
        }

        private Dictionary<string, ICheat> cheatList = new Dictionary<string, ICheat>();

        internal void AddCheat(string name, ICheat cheat=null)
        {
            if (cheat == null)
            {
                cheat = new DefaultCheat();
            }
            if (!cheatList.ContainsKey(name))
            {
                cheatList.Add(name, cheat);
            }
        }

        internal ICheat GetCheat(string name)
        {
            ICheat cheat;
            if (Enabled && cheatList.TryGetValue(name, out cheat))
            {
                return cheat;
            }
            return _NullCheat;
        }
    }
}
