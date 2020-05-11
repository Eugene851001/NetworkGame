using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ClientGame
{
    class KeyboardState
    {
        private const int KeyPressed = 0x8000;

        public static bool IsKeyDown(int key)
        {
            return ((GetKeyState(key) & KeyPressed) != 0);
        }

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int key);
    }
}
