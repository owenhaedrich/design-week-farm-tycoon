using System.Collections.Generic;

namespace MohawkTerminalGame
{
    public static class Inventory
    {
        // Item counts keyed by item names
        public static readonly Dictionary<string, int> Items = new()
        {
            {GameItems.WheatSeed.Name, 2}, // Wheat Seed - placeable
            {GameItems.CarrotSeed.Name, 1}, // Carrot Seed - placeable
            {GameItems.Calf.Name, 1}, // Calf - placeable
            {GameItems.Chicken.Name, 1},  // Chicken - placeable
            {GameItems.Piglet.Name, 0}  // Piglet - placeable
        };

        // Player's money
        public static int Money = 20;

        // Flags for UI redraw (integration with FieldInfo drawing)
        public static bool MoneyChanged = true;
        public static bool ItemsChanged = true;

        // Add money to player
        public static void AddMoney(int amount)
        {
            Money += amount;
            MoneyChanged = true;
        }

        // Remove money from player (returns false if not enough)
        public static bool SpendMoney(int amount)
        {
            if (Money >= amount)
            {
                Money -= amount;
                MoneyChanged = true;
                return true;
            }
            return false;
        }

        // Get item count
        public static int GetItemCount(string itemName)
        {
            return Items.ContainsKey(itemName) ? Items[itemName] : 0;
        }

        // Add items
        public static void AddItem(string itemName, int amount)
        {
            if (Items.ContainsKey(itemName))
            {
                Items[itemName] += amount;
            }
            else
            {
                Items[itemName] = amount;
            }
            ItemsChanged = true;
        }

        // Remove items (returns false if not enough)
        public static bool RemoveItem(string itemName, int amount)
        {
            if (Items.ContainsKey(itemName) && Items[itemName] >= amount)
            {
                Items[itemName] -= amount;
                ItemsChanged = true;
                return true;
            }
            return false;
        }
    }
}
