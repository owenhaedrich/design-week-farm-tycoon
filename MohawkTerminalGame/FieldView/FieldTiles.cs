namespace MohawkTerminalGame
{
    using System.Collections.Generic;

    internal class FieldIcons
    {
        public static readonly Dictionary<TileType, string> Icons = new()
        {
            { TileType.WheatSeed, "╒══════╕\r\n│𓇢𓇢𓇢│\r\n│𓇢𓇢𓇢│\r\n╘══════╛\r\n" },
            { TileType.Wheat, "╒╂⟚⟚⟚╂╕\r\n│🌾🌾🌾│\r\n│🌾🌾🌾│\r\n╘╂⟚⟚⟚╂╛" },
            { TileType.CarrotSeed, "╒══════╕\r\n│𓇢𓇢𓇢│\r\n│𓇢𓇢𓇢│\r\n╘══════╛\r\n" },
            { TileType.Carrot, "╒╂⟚⟚⟚╂╕\r\n│🥕🥕🥕│\r\n│🥕🥕🥕│\r\n╘╂⟚⟚⟚╂╛" },
            { TileType.Chicken, "╒╈╊╊╊╊╈╕\r\n│🐓🐓  │\r\n│🐓🐓  │\r\n╘╈╊╊╊╊╈╛" },
            { TileType.Calf, "╒╈╊╊╊╊╈╕\r\n│🐮🐮  │\r\n│🐮🐮  │\r\n╘╈╊╊╊╊╈╛" },
            { TileType.Cow, "╒╈╊╊╊╊╈╕\r\n│🐄🐄  │\r\n│🐄🐄  │\r\n╘╈╊╊╊╊╈╛" },
            { TileType.Piglet, "╒╈╊╊╊╊╈╕\r\n│🐷🐷  │\r\n│🐷🐷  │\r\n╘╈╊╊╊╊╈╛" },
            { TileType.Pig, "╒╈╊╊╊╊╈╕\r\n│🐖🐖  │\r\n│🐖🐖  │\r\n╘╈╊╊╊╊╈╛" },
            { TileType.Dirt, "░░░░░░░░\r\n░░░░░░░░\r\n░░░░░░░░\r\n░░░░░░░░" }
        };
    }
}
