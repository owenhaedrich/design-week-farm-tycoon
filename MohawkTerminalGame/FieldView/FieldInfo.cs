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

        // Get icon for tile type
        public static string GetIconForTileType(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.WheatSeed: return Item.WheatSeed.Icon;
                case TileType.CarrotSeed: return Item.CarrotSeed.Icon;
                case TileType.Wheat: return Item.Wheat.Icon;
                case TileType.Carrot: return Item.Carrot.Icon;
                case TileType.Calf: return Item.Calf.Icon;
                case TileType.Cow: return Item.Cow.Icon;
                case TileType.Chicken: return Item.Chicken.Icon;
                default: return null;
            }
        }

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
                    FieldView.UpdateGrowth(); // Grow crops on day change
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
            int GetItemType(string icon)
            {
                // Plants first
                if (icon == Item.WheatSeed.Icon || icon == Item.CarrotSeed.Icon || icon == Item.Wheat.Icon || icon == Item.Carrot.Icon) return 0; // Plant
                if (icon == Item.Calf.Icon || icon == Item.Cow.Icon || icon == Item.Chicken.Icon) return 1; // Animal
                return 2; // Other
            }
            foreach (var kvp in Inventory.Items.OrderBy(kvp => GetItemType(kvp.Key)).ThenBy(kvp => kvp.Key))
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
            string icon = GetIconForTileType(tileType);
            return icon != null && Inventory.GetItemCount(icon) > 0;
        }

        public static bool TryPlaceTile(TileType tileType)
        {
            string icon = GetIconForTileType(tileType);
            if (icon == null) return true; // Dirt
            if (!CanPlaceTile(tileType)) return false;
            return Inventory.RemoveItem(icon, 1);
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
