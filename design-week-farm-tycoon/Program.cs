// See https://aka.ms/new-console-template for more information

public class Animals
{
    int sellPrice;
    float buyPrice;
    float priceMulti = 0.02f;
    
    int growthTime;
    bool passive;
    int amountPlaced = 0;
    bool isGrown;

    public void Cow()
    {
        sellPrice = 30;
        buyPrice = 20 * ((priceMulti * amountPlaced) + 1);
        
        growthTime = 1;
        passive = false;
    }

    public void Chicken()
    {
        sellPrice = 5;
        buyPrice = 10 * ((priceMulti * amountPlaced) + 1);
        
        growthTime = 0;
        passive = true;
    }
}

public class Crops
{
    int sellPrice;
    float buyPrice;
    float priceMulti = 0.01f;
    
    int growthTime;
    int amountPlaced = 0;
    bool isGrown;
    float consumeBuff;

    public void Wheat()
    {
        sellPrice = 75;
        buyPrice = 30 * ((priceMulti * amountPlaced) + 1);
      
        growthTime = 1;
        consumeBuff = 1.15f;
    }

    public void Carrot()
    {
        sellPrice = 16;
        buyPrice = 12 + ((priceMulti * amountPlaced) + 1);
      
        growthTime = 2;
        consumeBuff = 1.2f;
    }
}
