using System;

namespace MohawkTerminalGame
{
    public class Story
    {
        private bool needsRedraw = true;

        public bool Execute()
        {
            if (needsRedraw)
            {
                Show();
                needsRedraw = false;
            }

            if (Console.KeyAvailable || Input.IsKeyPressed(ConsoleKey.Spacebar) || Input.IsKeyPressed(ConsoleKey.Enter))
            {
                // Any key to proceed
                DayTimer.ResetDay();
                return true; // Transition to next day
            }
            return false;
        }

        private void Show()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                                                                                                      ║");
            Console.WriteLine("║                          🌟 Story Mode 🌟                                                             ║");
            Console.WriteLine("║                                                                                                      ║");
            Console.WriteLine("║  The day has ended on your farm!                                                                    ║");
            Console.WriteLine("║  Your crops have grown and your animals have thrived.                                               ║");
            Console.WriteLine("║  As night falls, you reflect on the hard work and look forward to tomorrow.                         ║");
            Console.WriteLine("║                                                                                                      ║");
            Console.WriteLine("║  Press any key to start a new day...                                                                ║");
            Console.WriteLine("║                                                                                                      ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }
    }
}
