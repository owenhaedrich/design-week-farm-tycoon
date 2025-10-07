using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MohawkTerminalGame
{
    internal class Viewport
    {
        public static int windowWidth = 48;
        public static int windowHeight = 16;

        public static void HideCursor()
        {
            // Hide cursor and move it out of the way
            Terminal.BackgroundColor = ConsoleColor.Black;
            Terminal.ForegroundColor = ConsoleColor.Black;
            Terminal.SetCursorPosition(windowWidth, windowHeight);
        }
    }
}
