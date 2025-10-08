using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace MohawkTerminalGame
{
    public class Animal
    {
        public int baseBuyPrice;
        public int trueBuyPrice;
        public int sellPrice;
        public float dupeTax = 1.1f;
        public int growthTime;
        public bool abilityActive;
        public bool passiveItem;

        // temp variables
        public int amountOwned;
        public int currentTurn;
        public int purchaseTurn;
        public int money;
    }

    public class Animals
    {
        public Animal Chicken() // lvl 1
        {
            Animal chicken = new Animal();

            chicken.baseBuyPrice = 25;
            chicken.trueBuyPrice = (int)Math.Round(chicken.baseBuyPrice * Math.Pow(chicken.dupeTax, chicken.amountOwned));

            chicken.sellPrice = 20;

            // each time the turn increases after buying, give the player an egg
            if (chicken.passiveItem == true)
            {
                // either get money each turn or make egg item (TBD)
                // egg.amountOwned++;
                // money += 20
                chicken.passiveItem = false;
            }
            else
            {

            }

            return chicken;
        }

        public Animal Cow() // lvl 2
        {
            Animal cow = new Animal();

            cow.baseBuyPrice = 50;
            cow.trueBuyPrice = (int)Math.Round(cow.baseBuyPrice * Math.Pow(cow.dupeTax, cow.amountOwned));

            // add 1 to growthTime each turn until current is 2 more than purchase
            cow.purchaseTurn = cow.currentTurn;
            if (cow.currentTurn < cow.purchaseTurn + 2)
            {
                cow.growthTime++;
            }


            if (cow.growthTime >= 2)
            {
                cow.sellPrice = 100;
            }
            else
            {
                cow.sellPrice = 25;
            }

            // check adjacent plots for cows, if 3 or more are next to eachother, all gain ability. last until 1 turn after there is less than 3 together

            if (cow.abilityActive == true)
            {
                cow.sellPrice = cow.sellPrice * 2;
            }
            else
            {

            }

            return cow;
        }
    }

    public class Crop
    {
        public int baseBuyPrice;
        public int trueBuyPrice;
        public int sellPrice;
        public float dupeTax = 1.08f;
        public int growthTime;

        // temp variables
        public int amountOwned;
        public int currentTurn;
        public int purchaseTurn;
    }

    public class Crops
    {
        public Crop Wheat() // lvl 1
        {
            Crop wheat = new Crop();

            wheat.baseBuyPrice = 20;
            wheat.trueBuyPrice = (int)Math.Round(wheat.baseBuyPrice * Math.Pow(wheat.dupeTax, wheat.amountOwned));

            // add 1 to growthTime each turn until current is 1 more than purchase
            wheat.purchaseTurn = wheat.currentTurn;
            if (wheat.currentTurn < wheat.purchaseTurn + 1)
            {
                wheat.growthTime++;
            }

            wheat.growthTime = 1;
            if (wheat.growthTime >= 1)
            {
                wheat.sellPrice = 40;
            }
            else
            {
                wheat.sellPrice = 5;
            }

            return wheat;
        }

        public Crop Carrot() // lvl 2
        {
            Crop carrot = new Crop();

            carrot.baseBuyPrice = 55;
            carrot.trueBuyPrice = (int)Math.Round(carrot.baseBuyPrice * Math.Pow(carrot.dupeTax, carrot.amountOwned));

            // add 1 to growthTime each turn until current is 2 more than purchase
            carrot.purchaseTurn = carrot.currentTurn;
            if (carrot.currentTurn < carrot.purchaseTurn + 2)
            {
                carrot.growthTime++;
            }

            if (carrot.growthTime >= 2)
            {
                carrot.sellPrice = 80;
            }
            else
            {
                carrot.sellPrice = 30;
            }

            return carrot;
        }

        public Crop Corn() // lvl 3
        {
            Crop corn = new Crop();

            corn.baseBuyPrice = 110;
            corn.trueBuyPrice = (int)Math.Round(corn.baseBuyPrice * Math.Pow(corn.dupeTax, corn.amountOwned));

            // add 1 to growthTime each turn until current is 4 more than purchase
            corn.purchaseTurn = corn.currentTurn;
            if (corn.currentTurn < corn.purchaseTurn + 4)
            {
                corn.growthTime++;
            }
            corn.growthTime = 4;

            if (corn.growthTime >= 4)
            {
                corn.sellPrice = 140;
            }
            else
            {
                corn.sellPrice = 70;
            }

            return corn;
        }
    }

    public class Extra
    {
        public int baseBuyPrice;
        public int trueBuyPrice;
        public float dupeTax = 1.11f;
        public int sellPrice;
        public int usedSellPrice;
        public int maxDurability;
        public int currentDurability;
        public float reduceDurability;
        public double ability;

        // Temp
        public int currentTurn;
        public int purchaseTurn;
        public int amountOwned;
        public int amountPlaced;
    }

    public class Extras
    {
        public Extra Egg() // misc
        {
            Extra egg = new Extra();

            egg.sellPrice = 20;
            
            // random (small) chance when used, to give player a new chicken at the cost of the egg

            return egg;
        }

        public Extra Scarecrow() // event defense
        {
            Extra scarecrow = new Extra();

            scarecrow.baseBuyPrice = 100;
            scarecrow.trueBuyPrice = (int)Math.Round(scarecrow.baseBuyPrice * Math.Pow(scarecrow.dupeTax, scarecrow.amountOwned));
            scarecrow.sellPrice = 20;
            scarecrow.maxDurability = 5;
            scarecrow.currentDurability = 5;

            if (scarecrow.currentDurability > 0)
            {
                scarecrow.currentDurability--;
            }
            else
            {
                scarecrow.amountPlaced--;
            }
            return scarecrow;
        }
    }

    /// To-Do
    /// 
    /// public void newTurn (or whatever) actions that happen between / after each turn
    /// 
    /// rename variables for easier use
    /// buff animals that are given crops
    /// each crop gives different buff for different time
    /// add more items
    /// add more animals
    /// add more crops
    /// ?? add higher tax but decreases each turn you dont buy the item ??
    /// ?? item durability reduces sell value at 1:1 (%) ??
}