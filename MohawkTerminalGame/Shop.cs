using System;
using System.Collections.Generic;
using System.Linq;

namespace MohawkTerminalGame
{
    public class Shop
    {
        // Shop items with price, icon, and stock
        private List<Item> items = new List<Item>()
        {
            new Item("Chicken", 20, "🐔", 10),
            new Item("Cow", 50, "🐄", 5),
            new Item("Wheat", 5, "🌾", 20),
        };

        public Shop() { }

        // Shows the shop interface
        public void Show()
        {
            Console.Clear();

            const int leftWidth = 38;
            const int rightWidth = 39;

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╔══════════════════════════════════════╦═══════════════════════════════════════╗");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("║         🛒 Welcome to the Shop!      ║               Inventory               ║");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╠══════════════════════════════════════╬═══════════════════════════════════════╣");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("║ Item       Price  Stock              ║ Items Owned                           ║");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╠══════════════════════════════════════╬═══════════════════════════════════════╣");

            // Builds from Inventory.Items
            Dictionary<string, string> nameToIcon = items.ToDictionary(i => i.Name.ToLower(), i => i.Icon);
            Dictionary<string, int> inventoryCounts = nameToIcon.ToDictionary(kvp => kvp.Key, kvp => Inventory.GetItemCount(kvp.Value));

            int maxRows = Math.Max(items.Count, inventoryCounts.Count);

            for (int i = 0; i < maxRows; i++)
            {
                string leftText = "";
                string rightText = "";

                // Shop items
                if (i < items.Count)
                {
                    var item = items[i];
                    Console.ForegroundColor = item.Stock > 0 ? ConsoleColor.White : ConsoleColor.DarkGray;

                    leftText = $"{item.Name,-10} ";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    leftText += $"{item.Price,5:C0} ";
                    Console.ForegroundColor = item.Stock > 0 ? ConsoleColor.Green : ConsoleColor.Red;
                    leftText += $"{(item.Stock > 0 ? item.Stock.ToString() : "Sold Out"),8} ";
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    leftText += item.Icon;
                }
                leftText = leftText.PadRight(leftWidth);

                // Player’s inventory
                if (i < inventoryCounts.Count)
                {
                    var key = inventoryCounts.Keys.ElementAt(i);
                    var count = inventoryCounts[key];
                    var icon = items.FirstOrDefault(it => it.Name == key)?.Icon ?? "";

                    Console.ForegroundColor = ConsoleColor.White;
                    rightText = $"{key} ";
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    rightText += icon + " ";
                    Console.ForegroundColor = ConsoleColor.Green;
                    rightText += $": {count}";
                }
                rightText = rightText.PadRight(rightWidth);

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"║{leftText}║{rightText}║");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╚══════════════════════════════════════╩═══════════════════════════════════════╝");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            string moneyText = $"Your Money: ${Inventory.Money}";
            Console.WriteLine(CenterText(moneyText, leftWidth + rightWidth + 3));

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(CenterText("Type item name to buy or 'exit' to return to field", leftWidth + rightWidth + 3));

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("> ");
        }

        public void HandleInput(string input)
        {
            var item = items.FirstOrDefault(i => i.Name.Equals(input, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Item not found.");
                Console.ResetColor();
                return;
            }

            if (item.Stock <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Sorry, sold out.");
                Console.ResetColor();
                return;
            }

            if (Inventory.SpendMoney(item.Price))
            {
                // Updates inventory
                Inventory.AddItem(item.Icon, 1);

                // Decreases shop stock
                item.Stock--;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Bought a {item.Name}!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not enough money.");
                Console.ResetColor();
            }
        }

        private string CenterText(string text, int width)
        {
            if (text.Length >= width) return text.Substring(0, width);
            int pad = (width - text.Length) / 2;
            return new string(' ', pad) + text + new string(' ', width - text.Length - pad);
        }
    }

    // Shop items with name, price, icon, and stock
    public class Item
    {
        public string Name { get; }
        public int Price { get; }
        public string Icon { get; }
        public int Stock { get; set; }

        public Item(string name, int price, string icon, int stock)
        {
            Name = name;
            Price = price;
            Icon = icon;
            Stock = stock;
        }
    }
}
