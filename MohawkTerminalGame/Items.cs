using System;
using System.Diagnostics.Contracts;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace MohawkTerminalGame
{
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
        public string[,] gridBonus = new string[,]
        {
            { "", "", "", "", "", "" },
            { "", "", "", "", "", "" },
            { "", "", "", "", "", "" },
            { "", "", "", "", "", "" },
        };


        public virtual void AdvanceTurn()
        {
            currentTurn++;
        }
    }

    public class Chicken : Animal
    {
        public Chicken()
        {
            name = "Chicken";

            startPrice = 15;

            sellValue = 15;
        }
        public override void AdvanceTurn()
        {
            base.AdvanceTurn();

            realPrice = (int)Math.Round(startPrice * Math.Pow(dupeTax, amountOwned));

            money += 10;
        }
    }

    public class Piglet : Animal
    {
        public Piglet()
        {
            name = "Piglet";

            startPrice = 20;
        }
        public override void AdvanceTurn()
        {
            base.AdvanceTurn();

            realPrice = (int)Math.Round(startPrice * Math.Pow(dupeTax, amountOwned));
        }
    }
    public class Pig : Animal
    {
        public Pig()
        {
            name = "Pig";
        }
        public override void AdvanceTurn()
        {
            base.AdvanceTurn(); 

            int rows = gridBonus.GetLength(0);
            int cols = gridBonus.GetLength(1);

            
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols - 2; col++)
                {
                    if (gridBonus[row, col] == "Pig" && gridBonus[row, col + 1] == "Carrot" || gridBonus[row, col] == "Carrot" && gridBonus[row, col + 1] == "Pig")
                    {
                        money += 20;
                    }
                }
            }

            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows - 2; row++)
                {
                    if (gridBonus[row, col] == "Pig" && gridBonus[row + 1, col] == "Carrot" || gridBonus[row, col] == "Pig" && gridBonus[row + 1, col] == "Carrot")
                    {
                        money += 20;
                    }
                }
            }
        }
    }

    public class Calf : Animal
    {
        public Calf()
        {
            name = "Calf";

            startPrice = 25;

        }
        public override void AdvanceTurn()
        {
            base.AdvanceTurn();

            realPrice = (int)Math.Round(startPrice * Math.Pow(dupeTax, amountOwned));
        }
    }
    public class Cow : Animal
    {
        public Cow()
        {
            name = "Cow";

            growTime = 1;
        }
        public override void AdvanceTurn()
        {
            base.AdvanceTurn();

            int rows = gridBonus.GetLength(0);
            int cols = gridBonus.GetLength(1);

            // 3 cow bonus (milk)
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols - 2; col++)
                {
                    if (gridBonus[row, col] == "Cow" && gridBonus[row, col + 1] == "Cow" && gridBonus[row, col + 2] == "Cow")
                    {
                        money += 20;
                    }
                }
            }

            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows - 2; row++)
                {
                    if (gridBonus[row, col] == "Cow" && gridBonus[row + 1, col] == "Cow" && gridBonus[row + 2, col] == "Cow")
                    {
                        money += 20;
                    }
                }
            }

        }
    }

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

    public class WheatSeed : Crop
    {
        public WheatSeed()
        {
            name = "WheatSeed";

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
        {
            name = "Wheat";

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
        {
            name = "CarrotSeed";

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
        {
            name = "Carrot";

            sellValue = 45;

            growTime = 2;
        }
        public override void AdvanceTurn()
        {
            base.AdvanceTurn();
        }
    }
    
    public class Meat
    {
        public int sellValue;
        public int amountOwned;
        public int money;
    }

    public class Beef : Meat
    {
        public Beef()
        {
            sellValue = 60;
        }
    }

    public class Veal : Meat
    {
        public Veal()
        {
            sellValue = 20;
        }
    }

    public class Poultry : Meat
    {
        public Poultry()
        {
            sellValue = 15;
        }
    }
}