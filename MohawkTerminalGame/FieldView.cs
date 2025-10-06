using System;

namespace MohawkTerminalGame
{   
    public class FieldView
    {
        static ColoredText dirt = new("░", ConsoleColor.DarkYellow, ConsoleColor.DarkYellow);
        internal static TerminalGridWithColor field = new(30, 10, dirt);

        public static void ViewField()
        {
            field.ClearWrite();
        }

        public static void UpdateField()
        {
            if (Input.IsKeyPressed(ConsoleKey.RightArrow))
            {
                field.Poke(5, 5, new ColoredText("X", ConsoleColor.Red, ConsoleColor.Black));
            }
        }
    }
}
