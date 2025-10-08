using System.Collections.Generic;

namespace MohawkTerminalGame
{
    public static class Inventory
    {
        // Item counts keyed by icon emojis
        public static readonly Dictionary<string, int> Items = new()
        {
            ["🐔"] = 0, // Chicken
            ["🐄"] = 1, // Cow
            ["🌾"] = 3  // Wheat
        };

        // Player's money
        public static int Money { get; set; } = 100;

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
        public static int GetItemCount(string icon)
        {
            return Items.ContainsKey(icon) ? Items[icon] : 0;
        }

        // Add items
        public static void AddItem(string icon, int amount)
        {
            if (Items.ContainsKey(icon))
            {
                Items[icon] += amount;
            }
            else
            {
                Items[icon] = amount;
            }
            ItemsChanged = true;
        }

        // Remove items (returns false if not enough)
        public static bool RemoveItem(string icon, int amount)
        {
            if (Items.ContainsKey(icon) && Items[icon] >= amount)
            {
                Items[icon] -= amount;
                ItemsChanged = true;
                return true;
            }
            return false;
        }
    }
}
