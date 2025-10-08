using System;
using System.Collections.Generic;
using System.Linq;

namespace MohawkTerminalGame
{

    public class FieldInfo
    {
        // Game variables
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

        // Tile costs in inventory items
        static Dictionary<TileType, string> tileCosts = new Dictionary<TileType, string>
        {
            { TileType.Wheat, "🌾" },
            { TileType.Cow, "🐄" },
            { TileType.Chicken, "🐔" }
        };

        public static void Draw()
        {
            Inventory.MoneyChanged = true;
            Inventory.ItemsChanged = true;
            Update();
        }

        public static void Update()
        {
            // Money and inventory
            if (Inventory.MoneyChanged)
            {
                DrawMoney();
                Inventory.MoneyChanged = false;
            }

            if (InventoryNeedsRedraw())
            {
                DrawInventory();
                Inventory.ItemsChanged = false;
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
                    TimerExpired = true;
                }
                timerChange = true;
            }
            // Press S to speed up timer
            if (Input.IsKeyPressed(ConsoleKey.S))
            {
                timer -= 10;
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
            string moneyString = $"Money: ${Inventory.Money}";

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

        static void DrawInventory()
        {
            int inventoryY = 4; // Below money display
            int inventoryX = Viewport.windowWidth;
            Terminal.SetCursorPosition(inventoryX, inventoryY);
            Terminal.BackgroundColor = ConsoleColor.DarkGray;
            Terminal.ForegroundColor = ConsoleColor.White;
            // Inventory Title Line
            Terminal.Write("Inventory: ");
            int itemIndex = 0;
            foreach (var kvp in Inventory.Items.OrderBy(k => k.Key))
            {
                Terminal.SetCursorPosition(inventoryX, inventoryY + itemIndex + 1);
                Terminal.Write($"{kvp.Key} x{kvp.Value} ");
                itemIndex++;
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

                // Border with black
                Terminal.BackgroundColor = ConsoleColor.Black;
                Terminal.ForegroundColor = ConsoleColor.Black;
                Terminal.SetCursorPosition(Viewport.windowWidth + 1, timerY + row);
                Terminal.Write(' ');
            }
        }

        // Inventory management methods
        public static bool CanPlaceTile(TileType tileType)
        {
            if (tileCosts.ContainsKey(tileType))
            {
                string requiredItem = tileCosts[tileType];
                return Inventory.GetItemCount(requiredItem) > 0;
            }
            return true; // If tile doesn't have a cost, allow placement
        }

        public static bool TryPlaceTile(TileType tileType)
        {
            if (!CanPlaceTile(tileType))
                return false;

            if (tileCosts.ContainsKey(tileType))
            {
                string requiredItem = tileCosts[tileType];
                if (!Inventory.RemoveItem(requiredItem, 1))
                    return false;
            }

            return true;
        }

        public static int GetItemCount(string itemName) => Inventory.GetItemCount(itemName);

        public static bool RemoveItem(string itemName, int amount) => Inventory.RemoveItem(itemName, amount);

        public static void AddItem(string itemName, int amount) => Inventory.AddItem(itemName, amount);

        // Call this method when selection changes to update interactions
        public static void OnSelectionChanged()
        {
            interactionChange = true;
        }

        public static bool InventoryNeedsRedraw() => Inventory.ItemsChanged;
        public static bool TimerExpired = false;
    }
}
