using System;
using System.Diagnostics.Contracts;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace MohawkTerminalGame
{
    // lvl - turns untill grown
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
        public string[,] bonus = new string[,]
        {
            { "", "", "", "", "" },
            { "", "", "", "", "" },
            { "", "", "", "", "" },
            { "", "", "", "", "" },
            { "", "", "", "", "" },
        };


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

            startPrice = 30;
            realPrice = (int)Math.Round(startPrice * Math.Pow(dupeTax, amountOwned));

            sellValue = 20;
        }
        public override void AdvanceTurn()
        {
            base.AdvanceTurn();

            money += 15;
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


            int rows = bonus.GetLength(0);
            int cols = bonus.GetLength(1);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols - 2; col++)
                {
                    if (bonus[row, col] == "Cow" && bonus[row, col + 1] == "Cow" && bonus[row, col + 2] == "Cow")
                    {
                        money += 20;
                    }
                }
            }

            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows - 2; row++)
                {
                    if (bonus[row, col] == "Cow" && bonus[row + 1, col] == "Cow" && bonus[row + 2, col] == "Cow")
                    {
                        money += 20;
                    }
                }
            }

        }
    }
    #endregion

    // lvl - turns untill grown
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

    // lvl - turns untill broken
    #region Extras
    public class Extra
    {
        public int startPrice;
        public int realPrice;
        public float dupeTax = 1.11f;
        public int sellPrice;
        public int usedSellPrice;
        public int maxDurability;
        public int currentDurability;
        public float reduceDurability;
        public double ability;
        public string name = "";

        // Temp
        public int currentTurn;
        public int purchaseTurn;
        public int amountOwned;
        public int amountPlaced;

        public virtual void AdvanceTurn()
        {
            currentTurn++;
        }
    }

    // lvl 5
    public class Scarecrow : Extra
    {
        public Scarecrow()
        {
            name = "Scarecrow";

            startPrice = 100;
            realPrice = (int)Math.Round(startPrice * Math.Pow(dupeTax, amountOwned));

            maxDurability = 5;
            currentDurability = maxDurability;

            sellPrice = 20;
        }
        public override void AdvanceTurn()
        {
            base.AdvanceTurn();

            if (currentDurability > 0)
            {
                currentDurability--;
            }
            else
            {
                amountPlaced--;
            }

            if (currentDurability < maxDurability)
            {
                sellPrice = sellPrice - ((sellPrice/5) * (maxDurability - currentDurability));
            }
            else
            {

            }
        }
    }
    #endregion
}