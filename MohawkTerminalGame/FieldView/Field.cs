using System;
using System.Collections.Generic;

namespace MohawkTerminalGame
{
    // Enum for different tile types
    public enum TileType
    {
        Dirt,
        WheatSeed,
        CarrotSeed,
        Wheat,
        Carrot,
        Calf,
        Cow,
        Chicken
    }

    // Interaction Types
    public enum InteractionType
    {
        Feed, // For animals
        Harvest, // For both animals and crops
    }

    public class Field
    {
        // Initialization
        public static void Start()
        {
            field.ClearWrite();
            lastFlashTime = Time.ElapsedMilliseconds;
            isHighlightVisible = true;
            DrawField();
            ApplySelectionVisual();
            FieldInfo.Draw();
        }

        public static void Unpause()
        {
            DrawField();
            ApplySelectionVisual();
            FieldInfo.Draw();
        }

        // Main execution loop
        public static void Execute()
        {
            int moveX = 0;
            int moveY = 0;

            if (Input.IsKeyDown(ConsoleKey.RightArrow))
                moveX++;
            if (Input.IsKeyDown(ConsoleKey.LeftArrow))
                moveX--;
            if (Input.IsKeyDown(ConsoleKey.DownArrow))
                moveY++;
            if (Input.IsKeyDown(ConsoleKey.UpArrow))
                moveY--;


            if (moveX != 0 || moveY != 0)
            {
                MoveSelection(moveX, moveY);
                ApplySelectionVisual();
            }

            if (Input.IsKeyPressed(ConsoleKey.Spacebar) || Input.IsKeyPressed(ConsoleKey.Enter))
            {
            }
            else if (Input.IsKeyPressed(ConsoleKey.D1))
            {
                PlaceTile(TileType.CarrotSeed); // Placeable: Carrot Seed
            }
            else if (Input.IsKeyPressed(ConsoleKey.D2))
            {
                PlaceTile(TileType.WheatSeed); // Placeable: Wheat Seed
            }
            else if (Input.IsKeyPressed(ConsoleKey.D3))
            {
                PlaceTile(TileType.Calf); // Placeable: Calf
            }
            else if (Input.IsKeyPressed(ConsoleKey.D4))
            {
                PlaceTile(TileType.Chicken); // Placeable: Chicken
            }
            else if (Input.IsKeyPressed(ConsoleKey.H))
            {
                HarvestTile();
            }
            else if (Input.IsKeyPressed(ConsoleKey.F))
            {
                FeedTile();
            }

            FieldInfo.Update();
            
            // Keep cursor hidden consistently
            Viewport.HideCursor();
        }

        // Visual elements
        static ColoredText dirt = new("░", ConsoleColor.DarkYellow, ConsoleColor.DarkYellow);
        static int visualScaleX = 8;
        static int visualScaleY = 4;

        // Flashing highlight
        static bool isHighlightVisible = true;
        static long lastFlashTime;

        // Game elements
        public static int selectionX = 0;
        public static int selectionY = 0;
        static int previousSelectionX = 0;
        static int previousSelectionY = 0;
        internal static TerminalGridWithColor field = new(Viewport.windowWidth, Viewport.windowHeight, dirt);
        internal static LogicalGrid logicalGrid = new(Viewport.windowWidth / visualScaleX, Viewport.windowHeight / visualScaleY, visualScaleX, visualScaleY);

        // Map tile types to visual representations
        static ColoredText GetVisualForTileType(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Dirt:
                    return dirt;
                case TileType.WheatSeed:
                    return new ColoredText("s", ConsoleColor.Green, ConsoleColor.Black);
                case TileType.CarrotSeed:
                    return new ColoredText("c", ConsoleColor.Green, ConsoleColor.Black);
                case TileType.Wheat:
                    return new ColoredText("w", ConsoleColor.Yellow, ConsoleColor.Green);
                case TileType.Carrot:
                    return new ColoredText("r", ConsoleColor.DarkYellow, ConsoleColor.Green);
                case TileType.Calf:
                    return new ColoredText("a", ConsoleColor.DarkYellow, ConsoleColor.Black);
                case TileType.Cow:
                    return new ColoredText("A", ConsoleColor.White, ConsoleColor.Black);
                case TileType.Chicken:
                    return new ColoredText("b", ConsoleColor.Yellow, ConsoleColor.DarkYellow);
                default:
                    return dirt;
            }
        }

        static void DrawField()
        {
            for (int logicalY = 0; logicalY < logicalGrid.Height; logicalY++)
            {
                for (int logicalX = 0; logicalX < logicalGrid.Width; logicalX++)
                {
                    var space = logicalGrid.GetSpace(logicalX, logicalY);
                    var expectedVisual = GetVisualForTileType(space.TileType);
                    var (visualX, visualY) = logicalGrid.LogicalToVisual(logicalX, logicalY);

                    // Skip the currently highlighted tile
                    bool isHighlighted = (logicalX == selectionX && logicalY == selectionY);
                    if (isHighlighted)
                        continue;

                    for (int yOffset = 0; yOffset < logicalGrid.VisualTileHeight; yOffset++)
                    {
                        for (int xOffset = 0; xOffset < logicalGrid.VisualTileWidth; xOffset++)
                        {
                            int currentX = visualX + xOffset;
                            int currentY = visualY + yOffset;

                            field.Poke(currentX, currentY, expectedVisual);

                    }
                }
            }
            ApplySelectionVisual();
        }
        }

        static ColoredText CreateHighlightedVisual()
        {
            var selectedSpace = logicalGrid.GetSpace(selectionX, selectionY);
            var normalVisual = GetVisualForTileType(selectedSpace.TileType);
            return new ColoredText(normalVisual.text, normalVisual.fgColor, ConsoleColor.Red);
        }

        static void ApplySelectionVisual()
        {
            var selectedSpace = logicalGrid.GetSpace(selectionX, selectionY);
            var visual = isHighlightVisible ? CreateHighlightedVisual() : GetVisualForTileType(selectedSpace.TileType);
            var (visualX, visualY) = logicalGrid.LogicalToVisual(selectionX, selectionY);

            for (int yOffset = 0; yOffset < logicalGrid.VisualTileHeight; yOffset++)
            {
                for (int xOffset = 0; xOffset < logicalGrid.VisualTileWidth; xOffset++)
                {
                    field.Poke(visualX + xOffset, visualY + yOffset, visual);
                }
            }
        }

        // Remove highlight from the previous selection by restoring the underlying tile
        public static void RemoveHighlight()
        {
            var (visualX, visualY) = logicalGrid.LogicalToVisual(previousSelectionX, previousSelectionY);
            var previousSpace = logicalGrid.GetSpace(previousSelectionX, previousSelectionY);
            var visual = GetVisualForTileType(previousSpace.TileType);

            for (int yOffset = 0; yOffset < logicalGrid.VisualTileHeight; yOffset++)
            {
                for (int xOffset = 0; xOffset < logicalGrid.VisualTileWidth; xOffset++)
                {
                    field.Poke(visualX + xOffset, visualY + yOffset, visual);
                }
            }
        }

        public static void MoveSelection(int x, int y)
        {
            // Store previous position before updating
            previousSelectionX = selectionX;
            previousSelectionY = selectionY;

            // Update selection position
            selectionX += x;
            selectionY += y;

            // Clamp selection
            selectionX = Math.Clamp(selectionX, 0, logicalGrid.Width - 1);
            selectionY = Math.Clamp(selectionY, 0, logicalGrid.Height - 1);

            // Only update if position actually changed
            if (selectionX != previousSelectionX || selectionY != previousSelectionY)
            {
                RemoveHighlight();
                ApplySelectionVisual();
                FieldInfo.OnSelectionChanged();
            }
        }

        public static void PlaceTile(TileType tileType)
        {
            // Check if the current space is dirt (only allow placement on dirt)
            if (logicalGrid.GetSpace(selectionX, selectionY).TileType != TileType.Dirt)
            {
                return;
            }

            // Check if we have the required inventory item
            if (!FieldInfo.CanPlaceTile(tileType))
            {
                return;
            }

            // Deduct the inventory item
            if (!FieldInfo.TryPlaceTile(tileType))
            {
                return;
            }

            // Increment placed count for dynamic pricing
            string itemName = FieldInfo.GetInventoryKeyForTileType(tileType);
            Item item = Item.ItemsByName[itemName];
            if (item != null) item.AmountPlaced++;

            // Update logical grid at current selection
            logicalGrid.SetTileType(selectionX, selectionY, tileType);

            var visual = GetVisualForTileType(tileType);
            var (visualX, visualY) = logicalGrid.LogicalToVisual(selectionX, selectionY);

            for (int yOffset = 0; yOffset < logicalGrid.VisualTileHeight; yOffset++)
            {
                for (int xOffset = 0; xOffset < logicalGrid.VisualTileWidth; xOffset++)
                {
                    field.Poke(visualX + xOffset, visualY + yOffset, visual);
                }
            }

            // Reapply highlight (the tile placement overwrote it)
            ApplySelectionVisual();

            // Update interactions
            FieldInfo.OnSelectionChanged();
        }

        public static void HarvestTile()
        {
            var space = GetCurrentSelectedSpace();
            string harvestIcon = null;
            switch (space.TileType)
            {
                case TileType.Wheat:
                    harvestIcon = Item.Wheat.HarvestItem; // Wheat
                    break;
                case TileType.Carrot:
                    harvestIcon = Item.Carrot.HarvestItem; // Carrot
                    break;
                case TileType.Calf:
                    harvestIcon = Item.Calf.HarvestItem; // Veal
                    break;
                case TileType.Cow:
                    harvestIcon = Item.Cow.HarvestItem; // Beef
                    break;
                case TileType.Chicken:
                    harvestIcon = Item.Chicken.HarvestItem; // Poultry
                    break;
                default:
                    return; // No harvest
            }
            if (harvestIcon != null)
            {
                Inventory.AddItem(Item.GetByIcon(harvestIcon).Name, 1);
                logicalGrid.SetTileType(selectionX, selectionY, TileType.Dirt);
                // Redraw the tile to dirt
                var visual = GetVisualForTileType(TileType.Dirt);
                var (visualX, visualY) = logicalGrid.LogicalToVisual(selectionX, selectionY);
                for (int yOffset = 0; yOffset < logicalGrid.VisualTileHeight; yOffset++)
                {
                    for (int xOffset = 0; xOffset < logicalGrid.VisualTileWidth; xOffset++)
                    {
                        field.Poke(visualX + xOffset, visualY + yOffset, visual);
                    }
                }
                ApplySelectionVisual();
                FieldInfo.OnSelectionChanged();
            }
        }

        public static void FeedTile()
        {
            var space = GetCurrentSelectedSpace();
            if (space.TileType == TileType.Calf)
            {
                // Need at least one crop in inventory
                int wheatCount = Inventory.GetItemCount(Item.Wheat.Name);
                int carrotCount = Inventory.GetItemCount(Item.Carrot.Name);
                if (wheatCount > 0 || carrotCount > 0)
                {
                    // Consume one crop, prefer wheat
                    if (wheatCount > 0)
                    {
                        Inventory.RemoveItem(Item.Wheat.Name, 1);
                    }
                    else
                    {
                        Inventory.RemoveItem(Item.Carrot.Name, 1);
                    }
                    // Turn calf to cow
                    logicalGrid.SetTileType(selectionX, selectionY, TileType.Cow);
                    // Redraw
                    var visual = GetVisualForTileType(TileType.Cow);
                    var (visualX, visualY) = logicalGrid.LogicalToVisual(selectionX, selectionY);
                    for (int yOffset = 0; yOffset < logicalGrid.VisualTileHeight; yOffset++)
                    {
                        for (int xOffset = 0; xOffset < logicalGrid.VisualTileWidth; xOffset++)
                        {
                            field.Poke(visualX + xOffset, visualY + yOffset, visual);
                        }
                    }
                    // Reapply highlight
                    ApplySelectionVisual();
                    FieldInfo.OnSelectionChanged();
                }
            }
        }

        public static void UpdateGrowth()
        {
            for (int y = 0; y < logicalGrid.Height; y++)
            {
                for (int x = 0; x < logicalGrid.Width; x++)
                {
                    var space = logicalGrid.GetSpace(x, y);
                    if (space.TileType != TileType.Dirt)
                    {
                    space.GrowthProgress++;
                        Item item = Item.ItemsByName[FieldInfo.GetInventoryKeyForTileType(space.TileType)];
                        if (item != null && space.GrowthProgress >= item.GrowthTime)
                        {
                            // Change to grown version
                            if (space.TileType == TileType.WheatSeed)
                            {
                                logicalGrid.SetTileType(x, y, TileType.Wheat);
                                // Redraw the tile
                                var visual = GetVisualForTileType(TileType.Wheat);
                                var (visualX, visualY) = logicalGrid.LogicalToVisual(x, y);
                                for (int yOffset = 0; yOffset < logicalGrid.VisualTileHeight; yOffset++)
                                {
                                    for (int xOffset = 0; xOffset < logicalGrid.VisualTileWidth; xOffset++)
                                    {
                                        field.Poke(visualX + xOffset, visualY + yOffset, visual);
                                    }
                                }
                            }
                            else if (space.TileType == TileType.CarrotSeed)
                            {
                                logicalGrid.SetTileType(x, y, TileType.Carrot);
                                // Redraw the tile
                                var visual = GetVisualForTileType(TileType.Carrot);
                                var (visualX, visualY) = logicalGrid.LogicalToVisual(x, y);
                                for (int yOffset = 0; yOffset < logicalGrid.VisualTileHeight; yOffset++)
                                {
                                    for (int xOffset = 0; xOffset < logicalGrid.VisualTileWidth; xOffset++)
                                    {
                                        field.Poke(visualX + xOffset, visualY + yOffset, visual);
                                    }
                                }
                            }
                            // Animals are already grown
                        }
                    }
                }
            }
        }

        public static GridSpace GetCurrentSelectedSpace()
        {
            return logicalGrid.GetSpace(selectionX, selectionY);
        }

        public static List<InteractionType> GetAvailableInteractions()
        {
            var space = GetCurrentSelectedSpace();
            var interactions = new List<InteractionType>();

            switch (space.TileType)
            {
                case TileType.Wheat:
                case TileType.Carrot:
                    interactions.Add(InteractionType.Harvest);
                    break;
                case TileType.Calf:
                case TileType.Cow:
                    interactions.Add(InteractionType.Feed);
                    interactions.Add(InteractionType.Harvest);
                    break;
                case TileType.Chicken:
                    interactions.Add(InteractionType.Harvest);
                    break;
                case TileType.Dirt:
                    // No interactions for dirt
                    break;
            }

            return interactions;
        }

        public static int CalculatePassiveIncome()
        {
            int total = 0;
            for (int y = 0; y < logicalGrid.Height; y++)
            {
                for (int x = 0; x < logicalGrid.Width; x++)
                {
                    var space = logicalGrid.GetSpace(x, y);
                    string itemName = FieldInfo.GetInventoryKeyForTileType(space.TileType);
                    if (itemName != null)
                    {
                        Item item = Item.ItemsByName[itemName];
                        if (item != null && item.Passive)
                        {
                            total += item.PassiveIncome;
                        }
                    }
                }
            }
            return total;
        }
    }

    // Represents a single space on the logical grid
    public class GridSpace
    {
        public int X;
        public int Y;
        public TileType TileType;
        public int GrowthProgress = 0; // For crops, increments to GrowthTime

        public GridSpace(int x, int y, TileType tileType = TileType.Dirt)
        {
            X = x;
            Y = y;
            TileType = tileType;
        }
    }

    // Manages the logical grid and its mapping to the visual grid
    public class LogicalGrid
    {
        GridSpace[,] grid; // The comma makes it a 2D array
        public int Width;
        public int Height;
        public int VisualTileWidth;
        public int VisualTileHeight;

        public LogicalGrid(int width, int height, int visualTileWidth = 2, int visualTileHeight = 2)
        {
            Width = width;
            Height = height;
            VisualTileWidth = visualTileWidth;
            VisualTileHeight = visualTileHeight;

            grid = new GridSpace[width, height];

            // Initialize all grid spaces
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    grid[x, y] = new GridSpace(x, y, TileType.Dirt);
                }
            }
        }

        // Get a grid space at logical coordinates
        public GridSpace GetSpace(int x, int y)
        {
            return grid[x, y];
        }

        // Set tile type at logical coordinates
        public void SetTileType(int x, int y, TileType tileType)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                grid[x, y].TileType = tileType;
                grid[x, y].GrowthProgress = 0; // Reset progress when tile type changes
            }
        }

        // Convert logical coordinates to visual coordinates (top-left corner)
        public (int visualX, int visualY) LogicalToVisual(int logicalX, int logicalY)
        {
            return (logicalX * VisualTileWidth, logicalY * VisualTileHeight);
        }
    }
}
