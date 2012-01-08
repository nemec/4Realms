using System;

namespace Frostbyte
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            This.Game = new FrostbyteGame();
            This.Game.Run();
        }
    }
#endif
}

