using System;
using System.Collections.Generic;
using System.Linq;

namespace MohawkTerminalGame
{

    public class FieldInfo
    {
        // Game variables
        static bool interactionChange = true;

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
                // Seeds first
                if (icon == Item.WheatSeed.Icon || icon == Item.CarrotSeed.Icon) return 0; // Seed
                // Then animals
                if (icon == Item.Calf.Icon || icon == Item.Cow.Icon || icon == Item.Chicken.Icon) return 1; // Animal                                                                                                            // Then plants
                if (icon == Item.Wheat.Icon || icon == Item.Carrot.Icon) return 2; // Plant
                return 3; // Other
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
    }
}
