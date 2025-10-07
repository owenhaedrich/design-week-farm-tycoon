using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Timers;

namespace MohawkTerminalGame
{
    public class FieldInfoBar
    {
        // Game variables
        static int money = 10000;
        static bool moneyChange = true;
        static bool inventoryChange = true;
        static bool timerChange = true;

        // Visual elements
        static int interactionBarHeight = 1;
        static int timerHeight = 2;

        // Timer
        static float maxTimer = 30; //seconds
        static float timer = maxTimer;
        static int maxTimerTick = Program.TargetFPS; //frames per tick
        static int timerTick = maxTimerTick;

        public static void Draw()
        {
            // Money and inventory
            if (moneyChange)
            {
                int moneyTopPadding = 2;
                int moneyBottomPadding = 2;
                int moneyRightPadding = 1;
                int moneyLeftPadding = 1;

                Terminal.SetCursorPosition(Viewport.windowWidth, moneyTopPadding);
                Terminal.BackgroundColor = ConsoleColor.DarkGray;
                Terminal.ForegroundColor = ConsoleColor.White;

                string moneyLeftBorder = new string(' ', moneyLeftPadding);
                string moneyRightBorder = new string(' ', moneyRightPadding);
                string moneyString = $"Money: ${money}";

                Terminal.Write(moneyLeftBorder + moneyString + moneyRightBorder);

                for (int row = 0; row < moneyTopPadding; row++)
                {
                    Terminal.SetCursorPosition(Viewport.windowWidth, row);
                    string moneyBackground = new string(' ', moneyLeftPadding + moneyString.Length + moneyRightPadding);
                    Terminal.Write(moneyBackground);
                }
                for (int row = moneyTopPadding + 1; row < Viewport.windowHeight; row++)
                {
                    Terminal.SetCursorPosition(Viewport.windowWidth, row);
                    string moneyBackground = new string(' ', moneyLeftPadding + moneyString.Length + moneyRightPadding);
                    Terminal.Write(moneyBackground);
                }


                moneyChange = false;
            }

            if (InventoryNeedsRedraw())
            {

            }

            // Interaction Bar

            // Timer
            if (timerChange)
            {
                int timerHeight = 2;

                for (int i = 0; i < timerHeight; i++)
                {
                    Terminal.SetCursorPosition(0, Viewport.windowHeight + i);
                    Terminal.BackgroundColor = ConsoleColor.Red;
                    Terminal.ForegroundColor = ConsoleColor.DarkRed;
                    float progress = 1 - timer / maxTimer;
                    string timerProgress = new string('-', (int)(Viewport.windowWidth * progress));
                    Terminal.Write(timerProgress);

                    Terminal.BackgroundColor = ConsoleColor.DarkGray;
                    Terminal.ForegroundColor = ConsoleColor.White;
                    string timerBackground = new string('|', Viewport.windowWidth - timerProgress.Length);
                    Terminal.Write(timerBackground);
                    timerChange = false;
                }
            }

            timerTick--;
            if (timerTick <= 0)
            {
                timer--;
                timerTick = maxTimerTick;
                if (timer < 0)
                {
                    timer = maxTimer;
                }
                timerChange = true;
            }
        }

        public static bool InventoryNeedsRedraw()
        {
            if (inventoryChange)
            {
                inventoryChange = false;
                return true;
            }

            return false;
        }
    }
}