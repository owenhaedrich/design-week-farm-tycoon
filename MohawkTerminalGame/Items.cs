using System;
using System.Diagnostics.Contracts;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace MohawkTerminalGame
{
    #region Animals
    public class Animal
    {
        public int startPrice;
        public int realPrice;
        public int sellValue;
        public float dupeTax = 1.1f; 
        public int growTime; 
        public bool abilityActive;
        public bool abilityPassive;
        public string name = "";

        // temp variables
        public int amountOwned;
        public int currentTurn;
        public int purchaseTurn;
        public int money;
        public int currentPlot;
        public int allPlot;

        public virtual void AdvanceTurn()
        {
            currentTurn++;
        }
    }

    // lvl 1
    public class Chicken : Animal
    {
        public Chicken()
        {
            name = "Chicken";

            startPrice = 25;
            realPrice = (int)Math.Round(startPrice * Math.Pow(dupeTax, amountOwned));

            sellValue = 20;
        }
        public override void AdvanceTurn()
        {
            base.AdvanceTurn();

            money += 20;
        }
    }

    // lvl 2
    public class Pig : Animal
    {
        public Pig()
        {
            name = "Pig";

            startPrice = 35;
            realPrice = (int)Math.Round(startPrice * Math.Pow(dupeTax, amountOwned));

            purchaseTurn = currentTurn;
        }
        public override void AdvanceTurn()
        {
            base.AdvanceTurn();
            if (currentTurn < purchaseTurn + 2)
            {
                growTime++;
            }
            else
            {

            }

            if (growTime >= 2)
            {
                sellValue = 75;
            }
            else
            {
                sellValue = 25;
            }
        }
    }

    // lvl 3
    public class Cow : Animal
    {
        public Cow()
        {
            name = "Cow";

            startPrice = 50;
            realPrice = (int)Math.Round(startPrice * Math.Pow(dupeTax, amountOwned));

            purchaseTurn = currentTurn;

            // check adjacent plots for cows, if 3 or more are next to eachother, all gain ability. lose ability if less than 3 (only if grow 3

            if (abilityActive == true)
            {
                // each turn all cows with ability will give milk 
            }
            else
            {

            }

        }
        public override void AdvanceTurn()
        {
            base.AdvanceTurn();

            if (currentTurn < purchaseTurn + 3)
            {
                growTime++;
            }
            else
            {

            }

            if (growTime >= 3)
            {
                sellValue = 100;
            }
            else
            {
                sellValue = 25;
            }
        }
    }
    #endregion

    #region Crops
    public class Crop
    {
        public int startPrice;
        public int realPrice;
        public int sellValue;
        public float dupeTax = 1.08f;
        public int growTime;
        public string name = "";

        // temp variables
        public int amountOwned;
        public int currentTurn;
        public int purchaseTurn;
        public int money;

        public virtual void AdvanceTurn()
        {
            currentTurn++;
        }
    }

    // lvl 1
    public class Wheat : Crop
    {
        public Wheat()
        {
            name = "Wheat";

            startPrice = 25;
            realPrice = (int)Math.Round(startPrice * Math.Pow(dupeTax, amountOwned));

            purchaseTurn = currentTurn;
        }
        public override void AdvanceTurn()
        {
            base.AdvanceTurn();

            if (currentTurn < purchaseTurn + 1)
            {
                growTime++;
            }
            else
            {

            }

            if (growTime >= 1)
            {
                sellValue = 45;
            }
            else
            {
                sellValue = 5;
            }
        }
    }

    // lvl 2
    public class Carrot : Crop
    {
        public Carrot()
        {
            name = "Carrot";

            startPrice = 55;
            realPrice = (int)Math.Round(startPrice * Math.Pow(dupeTax, amountOwned));

            purchaseTurn = currentTurn;
        }
        public override void AdvanceTurn()
        {
            base.AdvanceTurn();

            if (currentTurn < purchaseTurn + 2)
            {
                growTime++;
            }
            else
            {

            }

            if (growTime >= 2)
            {
                sellValue = 80;
            }
            else
            {
                sellValue = 30;
            }
        }
    }

    // lvl 4
    public class Corn : Crop
    {
        public Corn()
        {
            name = "Corn";

            startPrice = 110;
            realPrice = (int)Math.Round(startPrice * Math.Pow(dupeTax, amountOwned));

            purchaseTurn = currentTurn;
        }
        public override void AdvanceTurn()
        {
            base.AdvanceTurn();

            if (currentTurn < purchaseTurn + 4)
            {
                growTime++;
            }
            else
            {

            }

            if (growTime >= 4)
            {
                sellValue = 140;
            }
            else
            {
                sellValue = 70;
            }
        }

    }
    #endregion




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

        public Extra Scarecrow() // defense lvl 5
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
    /// chicken ability - lets you use egg instead of selling to have chance for free chicken
    /// pig ability - check for carrot, if nearby AND fully grown, make sell 1.5x
    /// cow ability - check for other cows, if 2 or more nearby AND 2+ fully grown, give 1 milk per grown cow
    /// cow - give plot number then check the 8 squares around it with + and - math, if any of 2 are cows then activate buff
    /// 
    /// buff animals that are given crops
    /// each crop gives different buff for different time
    /// add more items
    /// 
    /// ?? add higher tax but decreases each turn you dont buy the item ??
    /// ?? item durability reduces sell value at 1:1 (%) ??
}