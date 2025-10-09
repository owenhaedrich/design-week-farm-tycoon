using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MohawkTerminalGame;

namespace DefaultNamespace
{
    internal class IntroText
    {
        public static bool introPlayed = false;
        private static string continueGame = "Non-Null";

        public static void displayText()
        {
            if (introPlayed) { return; };
            Terminal.UseRoboType = true;
            Terminal.RoboTypeIntervalMilliseconds = 1;
            Terminal.WriteLine("Arise Tlzchangor. Arise and Toil!" +
                "\n\nYou are Tlzchangor the -up until recently- great Necromancer of the Shade Vale. You grew an undead army that would make the region tremble with fright. With your zombies, skeleton warriors, skeleton horses, skeleton housecats and skeleton warrior housecats the world would be yours for the taking! Your plans would not be realized however. A heroic group of heroes who did not take kindly to undead armies that would make their region tremble with fright had caught wind of your machinations! Timely arriving at the sinister doorstep to your evil abode in order to smite you. Soon a great battle ensued, ending with glorious victory (depending on how you feel about zombie hordes) for the adventurers!" +
                "\n\nIt is several days after barely surviving that great battle and you are beginning to realize that raising and managing an undead army takes a lot of money… Money which you don’t currently have. (perhaps the local bank would have been part of your planned conquest?)" +
                "\n\nNow you have demonic debt collectors* hot on your tail doing what they do best**. You have 5 days to pay up for your undead army, or else it seems like they’re keen to let the general public know where to find you and how exactly to repay you." +
                "\n\nGetting a job would be harder than it looks, considering most people don’t like it when you resurrect their family to serve as zombified flesh puppets. So you’ve gone back to doing what YOU do best. Raising things. However instead of cultivating corpses you can foster a farm! This kind of environment will do wonders for you - IF you can pay up the money to the debt collectors!" +
                "\n\nIt would maybe be in your best interest to start sooner rather than later." +
                "\n\n* These aren’t actually demon debt collectors, they’re just debt collectors who are very “demonic” in their methods, which I suppose is redundant because which debt collectors aren’t?" +
                "\n\n** Collecting debts… If you needed that clarified."); 

            while (introPlayed == false)
            {
                Terminal.WriteLine("\n\nType Start to Start Game.");
                continueGame = Terminal.ReadLine().Trim();
                if (continueGame == "Start")
                {
                    introPlayed = true;
                    Terminal.UseRoboType = false;
                }
                else if (continueGame != "Start")
                {
                    Terminal.WriteLine("Ummm...What?");
                }
                
            }
            
                
        }
    }
}
