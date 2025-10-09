using System;

namespace MohawkTerminalGame
{
    public enum StoryMode { Intro, Progress, Ending }

    public class Story
    {
        public StoryMode Mode;
        private bool needsRedraw = true;
        public static int DailyPassiveIncome { get; set; } // Set externally by game loop

        public bool PlayAndWait()
        {
            if (needsRedraw)
            {
                Show();
                needsRedraw = false;
            }

            if (Console.KeyAvailable || Input.IsKeyPressed(ConsoleKey.Spacebar) || Input.IsKeyPressed(ConsoleKey.Enter))
            {
                // Any key to proceed
                needsRedraw = true;
                return true; // Transition to next day
            }
            return false;
        }

        private void Show()
        {
            Terminal.Clear();
            Terminal.ForegroundColor = ConsoleColor.White;
            Terminal.WriteLine("╔══════════════════════════════════════════════════════════════════════════════════════════════════════════╗");
            Terminal.WriteLine("║                                                                                                          ║");
            Terminal.WriteLine("║                          ! The Farm !                                                                    ║");
            Terminal.WriteLine("║                                                                                                          ║");

            switch (Mode)
            {
                case StoryMode.Intro:
                    Terminal.WriteLine("║  Welcome to the Farm!                                                                               ║");
                    Terminal.WriteLine("║  -----                                                                                              ║");
                    Terminal.WriteLine("║ -------                                                                                             ║");
                    break;
                case StoryMode.Progress:
                    Terminal.WriteLine("║  The day has ended on your farm!                                                                    ║");
                   Terminal.WriteLine($"║  You have ${Inventory.Money} and ${Story.DailyPassiveIncome} from eggs.                                                         ║");
                    Terminal.WriteLine("║  As night falls, you reflect on the hard work and look forward to tomorrow.                         ║");
                    break;
                case StoryMode.Ending:
                    Terminal.WriteLine("║  Congratulations!                                                                                    ║");
                    Terminal.WriteLine("║  You have successfully completed 10 days of farming.                                                 ║");
                    Terminal.WriteLine("║  ---------                                                                                           ║");
                    break;
            }

            Terminal.WriteLine("║                                                                                                          ║");
            Terminal.WriteLine("║  Press any key to start a new day...                                                                     ║");
            Terminal.WriteLine("║                                                                                                          ║");
            Terminal.WriteLine("╚══════════════════════════════════════════════════════════════════════════════════════════════════════════╝");
            Terminal.ResetColor();
        }
    }
}
