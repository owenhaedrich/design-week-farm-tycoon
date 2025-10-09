using System;
using System.Collections.Generic;
using System.Linq;

namespace MohawkTerminalGame
{

    public class FieldInfo
    {
        // Game variables
        static bool interactionChange = true;

        private static readonly List<string> placeableItemNames = new()
        {
            GameItems.WheatSeed.Name, // [1]
            GameItems.CarrotSeed.Name,  // [2]
            GameItems.Calf.Name,       // [3]
            GameItems.Chicken.Name,    // [4]
            GameItems.Piglet.Name      // [5]
        };

        // Get inventory key (item name) for tile type
        public static string GetInventoryKeyForTileType(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.WheatSeed: return GameItems.WheatSeed.Name;
                case TileType.CarrotSeed: return GameItems.CarrotSeed.Name;
                case TileType.Wheat: return GameItems.Wheat.Name;
                case TileType.Carrot: return GameItems.Carrot.Name;
                case TileType.Calf: return GameItems.Calf.Name;
                case TileType.Cow: return GameItems.Cow.Name;
                case TileType.Chicken: return GameItems.Chicken.Name;
                case TileType.Piglet: return GameItems.Piglet.Name;
                case TileType.Pig: return GameItems.Pig.Name;
                default: return null;
            }
        }

        public static void Draw()
        {
            Inventory.MoneyChanged = true;
            Inventory.ItemsChanged = true;
            interactionChange = true;
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
            DrawInteractionBar();
        }

        static void DrawMoney()
        {
            int moneyTopPadding = 1;
            int moneyRightPadding = 9;
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
            for (int row = moneyTopPadding + 1; row < Viewport.windowHeight + 3; row++)
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
            Terminal.BackgroundColor = ConsoleColor.DarkGray;
            Terminal.ForegroundColor = ConsoleColor.White;

            int GetItemType(string name)
            {
                // Seeds first
                if (name == GameItems.WheatSeed.Name || name == GameItems.CarrotSeed.Name) return 0; // Seed
                // Then animals
                if (name == GameItems.Calf.Name || name == GameItems.Cow.Name || name == GameItems.Chicken.Name || name == GameItems.Piglet.Name || name == GameItems.Pig.Name) return 1; // Animal
                // Then plants
                if (name == GameItems.Wheat.Name || name == GameItems.Carrot.Name) return 2; // Plant
                return 3; // Other
            }

            var placeableItems = Inventory.Items.Where(kvp => placeableItemNames.Contains(kvp.Key)).OrderBy(kvp => placeableItemNames.IndexOf(kvp.Key));
            var nonPlaceableItems = Inventory.Items.Where(kvp => !placeableItemNames.Contains(kvp.Key)).OrderBy(kvp => kvp.Key);

            // Placeable section
            Terminal.SetCursorPosition(inventoryX, inventoryY);
            Terminal.Write(" Place an Item: ");
            int itemIndex = 0;
            int currentY = inventoryY + 1;
            foreach (var kvp in placeableItems)
            {
                Terminal.SetCursorPosition(inventoryX, currentY);
                Terminal.Write($" [{itemIndex + 1}] {kvp.Key} x{kvp.Value} ");
                itemIndex++;
                currentY++;
            }

            // Non-placeable section
            if (nonPlaceableItems.Any())
            {
                Terminal.SetCursorPosition(inventoryX, currentY);
                Terminal.Write(" Inventory: ");
                currentY++;
                foreach (var kvp in nonPlaceableItems)
                {
                    Terminal.SetCursorPosition(inventoryX, currentY);
                    Terminal.Write($" {kvp.Key} x{kvp.Value} ");
                    currentY++;
                }
            }

            // Draw shop indicator
            Terminal.SetCursorPosition(Viewport.windowWidth, Viewport.windowHeight);
            Terminal.Write(" Go to [S]hop ");
        }

        static void DrawInteractionBar()
        {
            int interactionBarY = Viewport.windowHeight;

            Terminal.SetCursorPosition(0, interactionBarY);
            Terminal.BackgroundColor = ConsoleColor.DarkBlue;
            Terminal.ForegroundColor = ConsoleColor.White;

            var interactions = Field.GetAvailableInteractions();
            var currentSpace = Field.GetCurrentSelectedSpace();

            string interactionText;

            if (interactions.Count > 0)
            {
                // Make interaction string from list of interactions
                var interactionStrings = new List<string>();
                foreach (var interaction in interactions)
                {
                    interactionStrings.Add(GetInteractionDisplayText(interaction));
                }

                string tileInfo = $"{currentSpace.TileType} at ({Field.selectionX}, {Field.selectionY})";
                interactionText = tileInfo;
                foreach (var interactionString in interactionStrings)
                {
                    interactionText += " | " + interactionString;
                }
            }
            else
            {
                interactionText = $"{currentSpace.TileType} at ({Field.selectionX}, {Field.selectionY}) - No interactions available";
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

        // Inventory management methods
        public static bool CanPlaceTile(TileType tileType)
        {
            string itemName = GetInventoryKeyForTileType(tileType);
            return itemName != null && Inventory.GetItemCount(itemName) > 0;
        }

        public static bool TryPlaceTile(TileType tileType)
        {
            string itemName = GetInventoryKeyForTileType(tileType);
            if (itemName == null) return true; // Dirt
            if (!CanPlaceTile(tileType)) return false;
            return Inventory.RemoveItem(itemName, 1);
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
    }
}
