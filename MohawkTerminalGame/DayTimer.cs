using System;

namespace MohawkTerminalGame
{
    public static class DayTimer
    {
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
            if (Input.IsKeyPressed(ConsoleKey.S))
                currentTimer -= 10;
        }
    }
}
