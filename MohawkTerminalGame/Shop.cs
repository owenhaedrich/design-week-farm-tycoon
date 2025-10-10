using System;
using System.Collections.Generic;
using System.Linq;

namespace MohawkTerminalGame
{
    public enum ShopInputResult { None, Handled, Exit }

    public class Shop
    {
        // Shop items with price, icon, and stock
        private List<ShopItem> items = new List<ShopItem>()
        {
            new ShopItem(GameItems.WheatSeed, 20),
            new ShopItem(GameItems.CarrotSeed, 20),
            new ShopItem(GameItems.Calf, 5),
            new ShopItem(GameItems.Chicken, 10),
            new ShopItem(GameItems.Piglet, 5),
        };

        private bool isSellMode = false;

        public Shop() { }

        // Shows the shop interface
        public void Show()
        {
            Console.Clear();

            const int leftWidth = 38;
            const int rightWidth = 39;

            if (!isSellMode)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("╔══════════════════════════════════════╦═══════════════════════════════════════╗");
                Console.ForegroundColor = ConsoleColor.Yellow;
                string rightHeader = "           Inventory            ";
                Console.WriteLine($"║{"         🛒 Welcome to the Shop!      "}║{rightHeader.PadRight(39)}║");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("╠══════════════════════════════════════╬═══════════════════════════════════════╣");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"║                          🛒 Welcome to Sell Items!                           ║");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("╠══════════════════════════════════════════════════════════════════════════════╣");
            }
            if (!isSellMode)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                string rightHeader2 = " Items Owned ";
                Console.WriteLine($"║{" Item       Price  Stock              ".PadRight(38)}║{rightHeader2.PadRight(39)}║");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("╠══════════════════════════════════════╬═══════════════════════════════════════╣");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"║ Item       Sell Price  Qty                                                   ║");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("╠══════════════════════════════════════════════════════════════════════════════╣");
            }

            // Builds from Inventory.Items
            Dictionary<string, string> nameToIcon = items.ToDictionary(i => i.Name, i => i.Icon);
            Dictionary<string, int> inventoryCounts = nameToIcon.ToDictionary(kvp => kvp.Key, kvp => Inventory.GetItemCount(kvp.Key));

            // Order sellable items to match unified inputs: Wheat, Carrot, Beef, Veal, Poultry, Pork
            var sellOrder = new string[] { "Wheat", "Carrot", "Beef", "Veal", "Poultry", "Pork" };
            var sellableItems = sellOrder.Select(name => GameItems.GetByName(name)).ToList();

            int maxRows;

            if (isSellMode)

            {

                maxRows = sellableItems.Count;

            }

            else

            {

                maxRows = Math.Max(items.Count, inventoryCounts.Count);

            }

            for (int i = 0; i < maxRows; i++)
            {
                if (isSellMode)
                {
                    string sellText = "";
                    if (i < sellableItems.Count)
                    {
                        var item = sellableItems[i];
                        Console.ForegroundColor = ConsoleColor.White;
                        sellText += $"{i+1}. ";
                        sellText += $"{item.Name,-10} ";
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        sellText += $"${item.SellPrice,5} ";
                        Console.ForegroundColor = ConsoleColor.Green;
                        sellText += $"{Inventory.GetItemCount(item.Name),3} ";
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        sellText += $" {item.Icon}";
                    }
                    sellText = sellText.PadRight(78);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"║{sellText}║");
                }
                else
                {
                    string leftText = "";
                    string rightText = "";

                    // Shop items
                    if (i < items.Count)
                    {
                        var item = items[i];
                        Console.ForegroundColor = item.Stock > 0 ? ConsoleColor.White : ConsoleColor.DarkGray;
                        leftText += $"{i+1}. ";
                        leftText += $"{item.Name,-11} ";
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
                        rightText += $"{key} ";
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        rightText += icon + " ";
                        Console.ForegroundColor = ConsoleColor.Green;
                        rightText += $": {count}";
                    }
                    rightText = rightText.PadRight(rightWidth);

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"║{leftText}║{rightText}║");
                }
            }

            if (!isSellMode)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("╚══════════════════════════════════════╩═══════════════════════════════════════╝");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            string moneyText = $"Your Money: ${Inventory.Money}";
            Console.WriteLine(CenterText(moneyText, leftWidth + rightWidth + 3));

            Console.ForegroundColor = ConsoleColor.Yellow;
            string instruction = isSellMode ? "Press number to sell item, [S]witch to buy mode, or [E]xit" : "Press number to buy item, [S]witch to sell mode, or [E]xit";
            Console.WriteLine(CenterText(instruction, leftWidth + rightWidth + 3));

            Console.ForegroundColor = ConsoleColor.White;
        }

        public ShopInputResult HandleInput()
        {
            int num = 0;
            bool hasInput = false;

            if (Input.IsKeyPressed(ConsoleKey.D1))
            {
                num = 1;
                hasInput = true;
            }
            else if (Input.IsKeyPressed(ConsoleKey.D2))
            {
                num = 2;
                hasInput = true;
            }
            else if (Input.IsKeyPressed(ConsoleKey.D3))
            {
                num = 3;
                hasInput = true;
            }
            else if (Input.IsKeyPressed(ConsoleKey.D4))
            {
                num = 4;
                hasInput = true;
            }
            else if (Input.IsKeyPressed(ConsoleKey.D5))
            {
                num = 5;
                hasInput = true;
            }
            else if (Input.IsKeyPressed(ConsoleKey.D6))
            {
                num = 6;
                hasInput = true;
            }
            else if (Input.IsKeyPressed(ConsoleKey.S) && !isSellMode)
            {
                isSellMode = true;
                return ShopInputResult.Handled;
            }
            else if ((Input.IsKeyPressed(ConsoleKey.B) || Input.IsKeyPressed(ConsoleKey.S)) && isSellMode)
            {
                isSellMode = false;
                return ShopInputResult.Handled;
            }
            else if (Input.IsKeyPressed(ConsoleKey.E))
            {
                return ShopInputResult.Exit;
            }

            if (!hasInput)
            {
                return ShopInputResult.None;
            }

            if (num > 0)
            {
                if (isSellMode)
                {
                    // Order to match unified inputs: Wheat, Carrot, Beef, Veal, Poultry, Pork
                    var sellOrder = new string[] { "Wheat", "Carrot", "Beef", "Veal", "Poultry", "Pork" };
                    var sellableItems = sellOrder.Select(name => GameItems.GetByName(name)).ToList();
                    if (num >= 1 && num <= sellableItems.Count)
                    {
                        var item = sellableItems[num - 1];
                        SellItem(item.Name, 1);
                        return ShopInputResult.Handled;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No item at that number.");
                        Console.ResetColor();
                        return ShopInputResult.Handled;
                    }
                }
                else if (num >= 1 && num <= items.Count)
                {
                    var item = items[num - 1];
                    if (item.Stock <= 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Sorry, sold out.");
                        Console.ResetColor();
                        return ShopInputResult.Handled;
                    }
                    if (Inventory.SpendMoney(item.Price))
                    {
                        Inventory.AddItem(item.Name, 1);
                        item.Stock--;
                        item.Stats.boughtToday++;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Bought a {item.Name}!");
                        Console.ResetColor();
                        SoundEffects.Buy();
                        return ShopInputResult.Handled;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Not enough money.");
                        Console.ResetColor();
                        return ShopInputResult.Handled;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input.");
                    Console.ResetColor();
                    return ShopInputResult.Handled;
                }
            }

            return ShopInputResult.Handled;
        }

        private void SellItem(string itemName, int amount)
        {
            var item = GameItems.GetByName(itemName);
            if (item == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Item not found.");
                Console.ResetColor();
                return;
            }

            if (Inventory.RemoveItem(item.Name, amount))
            {
                int earn = item.SellPrice * amount;
                Inventory.AddMoney(earn);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Sold {amount} {item.Name} for ${earn}!");
                Console.ResetColor();
                SoundEffects.GetMoney();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not enough items to sell.");
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
    public class ShopItem
    {
        public string Name { get; }
        public int Price => (int)Math.Ceiling(Stats.BuyPrice); // Dynamic price from base Item, recalculated each time
        public string Icon { get; }
        public int Stock { get; set; }
        public Item Stats { get; } // Reference to the base Item stats

        public ShopItem(Item stats, int stock)
        {
            Name = stats.Name;
            Icon = stats.Icon;
            Stock = stock;
            this.Stats = stats;
        }
    }
}
