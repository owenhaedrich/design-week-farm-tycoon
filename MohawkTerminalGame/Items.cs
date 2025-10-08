using System;
using System.Collections.Generic;

namespace MohawkTerminalGame
{
    public enum ItemCategory
    {
        Crop,
        Animal
    }

    public class Item
    {
        public string Name { get; }
        public string Icon { get; }
        public float BaseBuyPrice { get; }
        public int SellPrice { get; }
        public float PriceMultiplier { get; }
        public int GrowthTime { get; }
        public ItemCategory Category { get; }
        public bool Passive { get; } // For animals
        public float ConsumeBuff { get; } // For crops
        public string HarvestItem { get; } // For animals, icon of meat they drop; for crops, their own icon

        public int AmountPlaced { get; set; } = 0;
        public bool IsGrown { get; set; } = false;

        public float BuyPrice => BaseBuyPrice * (PriceMultiplier * AmountPlaced + 1);

        private Item(string name, string icon, float baseBuyPrice, int sellPrice, float priceMultiplier, int growthTime, ItemCategory category, bool passive = false, float consumeBuff = 0f, string harvestItem = "")
        {
            Name = name;
            Icon = icon;
            BaseBuyPrice = baseBuyPrice;
            SellPrice = sellPrice;
            PriceMultiplier = priceMultiplier;
            GrowthTime = growthTime;
            Category = category;
            Passive = category == ItemCategory.Animal ? passive : false;
            ConsumeBuff = category == ItemCategory.Crop ? consumeBuff : 0f;
            HarvestItem = harvestItem != "" ? harvestItem : (category == ItemCategory.Animal ? "" : icon); // Animals need meat specified, crops harvest themselves
        }

        // Static item instances as source of truth
        public static readonly Item WheatSeed = new Item("Wheat Seed", "🌾🌱", 5f, 0, 0.01f, 1, ItemCategory.Crop, consumeBuff: 1.15f, harvestItem: "🌾"); // GrowthTime 1 to become Wheat, harvests Wheat
        public static readonly Item CarrotSeed = new Item("Carrot Seed", "🥕🌱", 7f, 0, 0.01f, 2, ItemCategory.Crop, consumeBuff: 1.2f, harvestItem: "🥕"); // GrowthTime 2 to become Carrot, harvests Carrot        
        public static readonly Item Calf = new Item("Calf", "🐂", 10f, 0, 0.02f, 0, ItemCategory.Animal, passive: false, harvestItem: "🍖"); // Grows to Cow after feeding, harvest Veal
        public static readonly Item Cow = new Item("Cow", "🐄", 20f, 0, 0.02f, 1, ItemCategory.Animal, passive: false, harvestItem: "🥩"); // HarvestItem Beef
        public static readonly Item Wheat = new Item("Wheat", "🌾", 30f, 75, 0.01f, 1, ItemCategory.Crop, consumeBuff: 1.15f); // Buy to plant or harvest from WheatSeed tile
        public static readonly Item Carrot = new Item("Carrot", "🥕", 12f, 16, 0.01f, 2, ItemCategory.Crop, consumeBuff: 1.2f);
        public static readonly Item Beef = new Item("Beef", "🥩", 0f, 40, 0f, 0, ItemCategory.Animal); // Meat from Cow, buy 0, sell 40
        public static readonly Item Veal = new Item("Veal", "🍖", 0f, 25, 0f, 0, ItemCategory.Animal); // Meat from Calf, buy 0, sell 25
        public static readonly Item Chicken = new Item("Chicken", "🐔", 10f, 0, 0.02f, 0, ItemCategory.Animal, passive: true, harvestItem: "🦆"); // HarvestItem Poultry
        public static readonly Item Poultry = new Item("Poultry", "🦆", 0f, 20, 0f, 0, ItemCategory.Animal); // Meat from Chicken

        // Lookup by icon
        public static readonly Dictionary<string, Item> ItemsByIcon = new()
        {
            { WheatSeed.Icon, WheatSeed },
            { CarrotSeed.Icon, CarrotSeed },
            { Wheat.Icon, Wheat },
            { Carrot.Icon, Carrot },
            { Calf.Icon, Calf },
            { Cow.Icon, Cow },
            { Beef.Icon, Beef },
            { Veal.Icon, Veal },
            { Chicken.Icon, Chicken },
            { Poultry.Icon, Poultry }
        };

        public static Item GetByIcon(string icon) => ItemsByIcon.GetValueOrDefault(icon);
    }
}
