using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MohawkTerminalGame
{
    internal static class SoundEffects
    {
        static Sound placeWheatSeed;
        static Sound placeCarrotSeed;
        static Sound placeCalf;
        static Sound placeChicken;
        static Sound placePiglet;
        static Sound growCow;
        static Sound growPig;
        static Sound buy;
        static Sound getMoney;
        static Sound startDay;
        static Sound harvest;

        public static void LoadAll()
        {
            Audio.Initialize();
            placeWheatSeed = Audio.LoadSound("./placeWheat.mp3");
            placeCarrotSeed = Audio.LoadSound("./placeCarrot.mp3");
            placeCalf = Audio.LoadSound("./placeCalf.mp3");
            placeChicken = Audio.LoadSound("./placeChicken.mp3");
            placePiglet = Audio.LoadSound("./placePiglet.mp3");
            growCow = Audio.LoadSound("./growCow.mp3");
            growPig = Audio.LoadSound("./growPig.mp3");
            buy = Audio.LoadSound("./buy.mp3");
            getMoney = Audio.LoadSound("./getMoney.mp3");
            startDay = Audio.LoadSound("./startDay.mp3");
            harvest = Audio.LoadSound("./harvest.mp3");
        }

        public static void PlaceWheat()
        {
            Audio.Play(placeWheatSeed);
        }

        public static void PlaceCarrot()
        {
            Audio.Play(placeCarrotSeed);
        }

        public static void PlaceCalf()
        {
            Audio.Play(placeCalf);
        }

        public static void PlaceChicken()
        {
            Audio.Play(placeChicken);
        }

        public static void PlacePiglet()
        {
            Audio.Play(placePiglet);
        }

        public static void GrowCow()
        {
            Audio.Play(growCow);
        }

        public static void GrowPig()
        {
            Audio.Play(growPig);
        }

        public static void Buy()
        {
            Audio.Play(buy);
        }

        public static void GetMoney()
        {
            Audio.Play(getMoney);
        }

        public static void StartDay()
        {
            Audio.Play(startDay);
        }
        public static void Harvest()
        {
            Audio.Play(harvest);
        }
    }
}
