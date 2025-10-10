using System;
using System.Collections.Generic;

namespace MohawkTerminalGame
{
    public enum ItemCategory
    {
        Crop,
        Animal
    }

    public abstract class Item
    {
        public string Name { get; }
        public string Icon { get; }
        public float BaseBuyPrice { get; }
        public int SellPrice { get; }
        public float PriceMultiplier { get; }
        public int GrowthTime { get; }
        public ItemCategory Category { get; }
        public float ConsumeBuff { get; } // For crops
        public string HarvestItem { get; } // For animals, name of meat they drop; for crops, their own name

        // Temp variables common to all items
        public int amountOwned;
        public int purchaseTurn;
        public int boughtToday = 0;

        protected Item(string name, string icon, float baseBuyPrice, int sellPrice, float priceMultiplier,
                       int growthTime, ItemCategory category, float consumeBuff = 0f, string harvestItem = "")
        {
            Name = name;
            Icon = icon;
            BaseBuyPrice = baseBuyPrice;
            SellPrice = sellPrice;
            PriceMultiplier = priceMultiplier;
            GrowthTime = growthTime;
            Category = category;
            ConsumeBuff = category == ItemCategory.Crop ? consumeBuff : 0f;
            HarvestItem = harvestItem != "" ? harvestItem : (category == ItemCategory.Animal ? "" : icon);
        }

        public virtual void AdvanceTurn()
        {
            // Field item effects only - called once per placed item
        }

        public float BuyPrice => BaseBuyPrice * (float)Math.Pow(PriceMultiplier, boughtToday);
    }

    public class Animal : Item
    {
        public int startPrice;
        public int realPrice;
        public int sellValue;
        public float dupeTax = 1.1f;
        public int growTime;
        public bool abilityActive;
        public bool abilityPassive;
        public string[,] gridBonus = new string[,]
        {
            { "", "", "", "", "", "" },
            { "", "", "", "", "", "" },
            { "", "", "", "", "", "" },
            { "", "", "", "", "", "" },
        };

        public Animal(string name, string icon, float baseBuyPrice, int sellPrice, float priceMultiplier,
                      int growthTime, ItemCategory category, float consumeBuff = 0f, string harvestItem = "")
            : base(name, icon, baseBuyPrice, sellPrice, priceMultiplier, growthTime, category, consumeBuff, harvestItem)
        {
        }

        public override void AdvanceTurn()
        {
            base.AdvanceTurn();
            realPrice = (int)Math.Round(startPrice * Math.Pow(dupeTax, amountOwned));
        }
    }

    public class Chicken : Animal
    {
        public Chicken()
            : base("Chicken", "🐔", 20f, 0, 1.1f, 0, ItemCategory.Animal, harvestItem: "Poultry")
        {
            startPrice = 20;
            sellValue = 15;
        }

        public override void AdvanceTurn()
        {
            base.AdvanceTurn();
            int eggValue = 10;
            Inventory.AddMoney(eggValue);
        }
    }

    public class Piglet : Animal
    {
        public Piglet()
            : base("Piglet", "🐷", 35f, 0, 1.1f, 0, ItemCategory.Animal, harvestItem: "")
        {
            startPrice = 35;
        }

        public override void AdvanceTurn()
        {
            base.AdvanceTurn();
        }
    }

    public class Pig : Animal
    {
        public Pig()
            : base("Pig", "🐷", 40f, 0, 1.1f, 1, ItemCategory.Animal, harvestItem: "Pork")
        {
        }

        public override void AdvanceTurn()
        {
            base.AdvanceTurn();
        }
    }

    public class Calf : Animal
    {
        public Calf()
            : base("Calf", "🐂", 25f, 0, 1.1f, 0, ItemCategory.Animal, harvestItem: "Veal")
        {
            startPrice = 25;
        }

        public override void AdvanceTurn()
        {
            base.AdvanceTurn();
        }
    }

    public class Cow : Animal
    {
        public Cow()
            : base("Cow", "🐄", 30f, 0, 1.1f, 1, ItemCategory.Animal, harvestItem: "Beef")
        {
            growTime = 1;
        }

        public override void AdvanceTurn()
        {
            base.AdvanceTurn();
        }
    }

    public class Crop : Item
    {
        public int startPrice;
        public int realPrice;
        public int sellValue;
        public float dupeTax = 1.08f;
        public int growTime;

        public Crop(string name, string icon, float baseBuyPrice, int sellPrice, float priceMultiplier,
                    int growthTime, ItemCategory category, float consumeBuff = 0f, string harvestItem = "")
            : base(name, icon, baseBuyPrice, sellPrice, priceMultiplier, growthTime, category, consumeBuff, harvestItem)
        {
        }

        public override void AdvanceTurn()
        {
            base.AdvanceTurn();
        }
    }

    public class WheatSeed : Crop
    {
        public WheatSeed()
            : base("Wheat Seed", "🌾🌱", 10f, 0, 1.08f, 1, ItemCategory.Crop, consumeBuff: 1.15f, harvestItem: "Wheat")
        {
            startPrice = 10;
            growTime = 1;
        }

        public override void AdvanceTurn()
        {
            base.AdvanceTurn();
            realPrice = (int)Math.Round(startPrice * Math.Pow(dupeTax, amountOwned));
        }
    }

    public class Wheat : Crop
    {
        public Wheat()
            : base("Wheat", "🌾", 30f, 20, 1.08f, 1, ItemCategory.Crop, consumeBuff: 1.15f, harvestItem: "Wheat")
        {
            sellValue = 20;
            growTime = 1;
        }

        public override void AdvanceTurn()
        {
            base.AdvanceTurn();
        }
    }

    public class CarrotSeed : Crop
    {
        public CarrotSeed()
            : base("Carrot Seed", "🥕🌱", 20f, 0, 1.08f, 2, ItemCategory.Crop, consumeBuff: 1.2f, harvestItem: "Carrot")
        {
            startPrice = 20;
            growTime = 2;
        }

        public override void AdvanceTurn()
        {
            base.AdvanceTurn();
            realPrice = (int)Math.Round(startPrice * Math.Pow(dupeTax, amountOwned));
        }
    }

    public class Carrot : Crop
    {
        public Carrot()
            : base("Carrot", "🥕", 12f, 45, 1.08f, 2, ItemCategory.Crop, consumeBuff: 1.2f, harvestItem: "Carrot")
        {
            sellValue = 45;
            growTime = 2;
        }

        public override void AdvanceTurn()
        {
            base.AdvanceTurn();
        }
    }

    public class Meat : Item
    {
        public int sellValue;

        public Meat(string name, string icon, float baseBuyPrice, int sellPrice, float priceMultiplier,
                    int growthTime, ItemCategory category, float consumeBuff = 0f, string harvestItem = "")
            : base(name, icon, baseBuyPrice, sellPrice, priceMultiplier, growthTime, category, consumeBuff, harvestItem)
        {
        }

        public override void AdvanceTurn()
        {
            base.AdvanceTurn();
        }
    }

    public class Beef : Meat
    {
        public Beef()
            : base("Beef", "🥩", 0f, 60, 1.0f, 0, ItemCategory.Animal)
        {
            sellValue = 60;
        }
    }

    public class Veal : Meat
    {
        public Veal()
            : base("Veal", "🍖", 0f, 20, 1.0f, 0, ItemCategory.Animal)
        {
            sellValue = 20;
        }
    }

    public class Poultry : Meat
    {
        public Poultry()
            : base("Poultry", "🦆", 0f, 15, 1.0f, 0, ItemCategory.Animal)
        {
            sellValue = 15;
        }
    }

    public class Pork : Meat
    {
        public Pork()
            : base("Pork", "🥓", 0f, 85, 1.0f, 0, ItemCategory.Animal)
        {
            sellValue = 85;
        }
    }

    public static class GameItems
    {
        public static readonly WheatSeed wheatSeed = new WheatSeed();
        public static readonly CarrotSeed carrotSeed = new CarrotSeed();
        public static readonly Calf calf = new Calf();
        public static readonly Chicken chicken = new Chicken();
        public static readonly Cow cow = new Cow();
        public static readonly Piglet piglet = new Piglet();
        public static readonly Pig pig = new Pig();
        public static readonly Wheat wheat = new Wheat();
        public static readonly Carrot carrot = new Carrot();
        public static readonly Beef beef = new Beef();
        public static readonly Veal veal = new Veal();
        public static readonly Poultry poultry = new Poultry();
        public static readonly Pork pork = new Pork();

        // Keep the original static items for backward compatibility during transition
        public static readonly Item WheatSeed = wheatSeed;
        public static readonly Item CarrotSeed = carrotSeed;
        public static readonly Item Calf = calf;
public static readonly Item Chicken = chicken;
        public static readonly Item Cow = cow;
        public static readonly Item Piglet = piglet;
        public static readonly Item Pig = pig;
        public static readonly Item Wheat = wheat;
        public static readonly Item Carrot = carrot;
        public static readonly Item Beef = beef;
        public static readonly Item Veal = veal;
        public static readonly Item Poultry = poultry;
        public static readonly Item Pork = pork;

        public static Item GetByName(string name) => ItemsByName.GetValueOrDefault(name);

        // Lookup by name
        public static readonly Dictionary<string, Item> ItemsByName = new()
        {
            { wheatSeed.Name, wheatSeed },
            { carrotSeed.Name, carrotSeed },
            { calf.Name, calf },
            { chicken.Name, chicken },
            { cow.Name, cow },
            { piglet.Name, piglet },
            { pig.Name, pig },
            { wheat.Name, wheat },
            { carrot.Name, carrot },
            { beef.Name, beef },
            { veal.Name, veal },
            { poultry.Name, poultry },
            { pork.Name, pork }
        };
    }
}
