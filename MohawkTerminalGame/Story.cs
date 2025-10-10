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
            Terminal.WriteLine("╔══════════════════════════════════════════════════════════════════════════════════════════════════════╗");
            Terminal.WriteLine("║                                                                                                      ║");
            Terminal.WriteLine("║                                     ! Farm Up !                                                      ║");
            Terminal.WriteLine("║                                                                                                      ║");

            switch (Mode)
            {
                case StoryMode.Intro:
                    Terminal.UseRoboType = true;
                    Terminal.RoboTypeIntervalMilliseconds = 1;
                    string[] storyLines = new string[]
                    {
                        "Arise Tlzchangor. Arise and Toil!",
                        "",
                        "You are Tlzchangor the -up until recently- great Necromancer of the Shade Vale. You grew",
                        "an undead army that would make the region tremble with fright. With your zombies, skeleton",
                        "warriors, skeleton horses, skeleton housecats and skeleton warrior housecats the world",
                        "would be yours for the taking! Your plans would not be realized however. A heroic group",
                        "of heroes who did not take kindly to undead armies that would make their region tremble",
                        "with fright had caught wind of your machinations! Timely arriving at the sinister doorstep",
                        "to your evil abode in order to smite you. Soon a great battle ensued, ending with glorious",
                        "victory (depending on how you feel about zombie hordes) for the adventurers!",
                        "",
                        "It is several days after barely surviving that great battle and you are beginning to",
                        "realize that raising and managing an undead army takes a lot of money… Money which you",
                        "don’t currently have. (perhaps the local bank would have been part of your planned",
                        "conquest?)",
                        "",
                        "Now you have demonic debt collectors* hot on your tail doing what they do best**. You",
                        "have 5 days to pay up for your undead army, or else it seems like they’re keen to let the",
                        "general public know where to find you and how exactly to repay you.",
                        "",
                        "Getting a job would be harder than it looks, considering most people don’t like it when",
                        "you resurrect their family to serve as zombified flesh puppets. So you’ve gone back to",
                        "doing what YOU do best. Raising things. However instead of cultivating corpses you can",
                        "foster a farm! This kind of environment will do wonders for you - IF you can pay up the",
                        "money to the debt collectors!",
                        "",
                        "It would maybe be in your best interest to start sooner rather than later.",
                        "",
                        "* These aren’t actually demon debt collectors, they’re just debt collectors who are very",
                        "“demonic” in their methods, which I suppose is redundant because which debt collectors",
                        "aren’t?",
                        "",
                        "** Collecting debts… If you needed that clarified."
                    };
                    foreach (string line in storyLines)
                    {
                        string formattedLine = "║  " + line.PadRight(99) + " ║";
                        Terminal.WriteLine(formattedLine);
                    }
                    Terminal.UseRoboType = false;
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
