using System;
using System.Numerics;

namespace MohawkTerminalGame
{   
    public class FieldView
    {
        // Visual elements
        static ColoredText dirt = new("░", ConsoleColor.DarkYellow, ConsoleColor.DarkYellow);
        static ColoredText highlight = new("X", ConsoleColor.Red, ConsoleColor.Black);

        // Game elements
        static int selectionX = 0;
        static int selectionY = 0;
        static ColoredText selectionOriginal = dirt;
        internal static TerminalGridWithColor field = new(30, 10, dirt);

        public static void ViewField()
        {
            field.ClearWrite();
        }

        public static void MoveSelection(int x, int y)
        {
            // Restore current position
            field.Poke(selectionX, selectionY, selectionOriginal);

            // Update selection position
            selectionX += x;
            selectionY += y;

            // Clamp selection within field bounds
            selectionX = Math.Clamp(selectionX, 0, field.Width - 1);
            selectionY = Math.Clamp(selectionY, 0, field.Height - 1);

            // Add highlight to new position
            selectionOriginal = field.Get(selectionX, selectionY);
            field.Poke(selectionX, selectionY, highlight);
        }
    }
}
