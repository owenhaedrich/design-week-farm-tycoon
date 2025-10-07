using System;
using System.Collections.Generic;

namespace MohawkTerminalGame
{
    public class FieldInfo
    {
        // Game variables
        static int money = 10000;
        static int[] inventory = [0,1,3]; // Placeholder for inventory. Zero cows, one chicken, three wheat
        static bool moneyChange = true;
        static bool inventoryChange = true;
        static bool timerChange = true;
        static bool interactionChange = true;

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
            moneyChange = true;
            inventoryChange = true;
            timerChange = true;
            interactionChange = true;
            Update();
        }

        public static void Update()
        {
            // Money and inventory
            if (moneyChange)
            {
                DrawMoney();
                moneyChange = false;
            }

            if (InventoryNeedsRedraw())
            {

            }

            // Interaction Bar
            if (interactionChange)
            {
                DrawInteractionBar();
                interactionChange = false;
            }

            // Timer
            if (timerChange)
            {
                DrawTimer();
                timerChange = false;
            }

            // Update timer
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

        static void DrawMoney()
        {
            int moneyTopPadding = 2;
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
        }

        static void DrawInteractionBar()
        {
            int interactionBarY = Viewport.windowHeight;

            Terminal.SetCursorPosition(0, interactionBarY);
            Terminal.BackgroundColor = ConsoleColor.DarkBlue;
            Terminal.ForegroundColor = ConsoleColor.White;

            var interactions = FieldView.GetAvailableInteractions();
            var currentSpace = FieldView.GetCurrentSelectedSpace();

            string interactionText;

            if (interactions.Count > 0)
            {
                // Make interaction string from list of interactions
                var interactionStrings = new List<string>();
                foreach (var interaction in interactions)
                {
                    interactionStrings.Add(GetInteractionDisplayText(interaction));
                }

                string tileInfo = $"{currentSpace.TileType} at ({FieldView.selectionX}, {FieldView.selectionY})";
                interactionText = tileInfo;
                foreach (var interactionString in interactionStrings)
                {
                    interactionText += " | " + interactionString;
                }
            }
            else
            {
                interactionText = $"{currentSpace.TileType} at ({FieldView.selectionX}, {FieldView.selectionY}) - No interactions available";
            }

            // Pad the interaction text to fill the entire width
            if (interactionText.Length < Viewport.windowWidth)
            {
                interactionText = interactionText + new string(' ', Viewport.windowWidth - interactionText.Length);
            }

            Terminal.Write(interactionText);
        }

        static string GetInteractionDisplayText(InteractionType interaction)
        {
            switch (interaction)
            {
                case InteractionType.Feed:
                    return "[F]eed";
                case InteractionType.Harvest:
                    return "[H]arvest";
                default:
                    return interaction.ToString();
            }
        }

        static void DrawTimer()
        {
            int timerY = Viewport.windowHeight + interactionBarHeight;

            // Progress bar for timer. Progress is red
            for (int row = 0; row < timerHeight; row++)
            {
                Terminal.SetCursorPosition(0, timerY + row);

                float progress = 1 - timer / maxTimer;
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
        }

        // Call this method when selection changes to update interactions
        public static void OnSelectionChanged()
        {
            interactionChange = true;
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