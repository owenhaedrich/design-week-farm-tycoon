using System;

namespace MohawkTerminalGame
{
    public static class DayTimer
    {
        public static int DayNumber = 0;
        public static float maxTimer = 30;
        public static float currentTimer = maxTimer;
        public static int maxTick = Program.TargetFPS;
        public static int tick = maxTick;
        private static bool dayExpiredFlag = false;
        public static bool TimerHasChanged = false;

        public static bool DayExpired
        {
            get
            {
                bool result = dayExpiredFlag;
                if (result) dayExpiredFlag = false;
                return result;
            }
        }

        public static void ResetDay()
        {
            currentTimer = maxTimer;
            tick = maxTick;
            dayExpiredFlag = false;
            TimerHasChanged = true;
            Field.UpdateGrowth();
            int passiveIncome = Field.CalculatePassiveIncome();
            if (passiveIncome > 0)
            {
                Inventory.AddMoney(passiveIncome);
            }
            DayNumber++;
            Viewport.HideCursor();
        }

        public static void Update()
        {
            tick--;
            if (tick <= 0)
            {
                tick = maxTick;
                float oldTimer = currentTimer;
                currentTimer--;
                if (oldTimer != currentTimer)
                {
                    TimerHasChanged = true;
                }
                if (currentTimer <= 0)
                {
                    currentTimer = maxTimer;
                    dayExpiredFlag = true;
                }
            }
            // Speed up for testing
            if (Input.IsKeyPressed(ConsoleKey.M))
                currentTimer = -1;
        }

        public static void CheckAndDraw()
        {
            if (TimerHasChanged)
            {
                Draw();
                TimerHasChanged = false;
            }
        }

        public static void Draw()
        {
            int timerY = Viewport.windowHeight + 1; // Below interaction bar

            for (int row = 0; row < 2; row++)
            {
                Terminal.SetCursorPosition(0, timerY + row);

                float progress = 1 - currentTimer / maxTimer;
                int progressWidth = (int)(Viewport.windowWidth * progress);
                Terminal.BackgroundColor = ConsoleColor.Red;
                Terminal.ForegroundColor = ConsoleColor.DarkRed;
                string timerProgress = new string('-', progressWidth);
                Terminal.Write(timerProgress);

                Terminal.BackgroundColor = ConsoleColor.DarkGray;
                Terminal.ForegroundColor = ConsoleColor.White;
                string timerBackground = new string('|', Viewport.windowWidth - progressWidth);
                Terminal.Write(timerBackground);
            }

            // Draw day number
            Terminal.BackgroundColor = ConsoleColor.DarkGray;
            Terminal.ForegroundColor = ConsoleColor.White;
            Terminal.SetCursorPosition(Viewport.windowWidth + 1, timerY);
            Terminal.Write($" Day {DayNumber} / 10 ");

            // Reset Cursor
            Viewport.HideCursor();
        }
    }
}
