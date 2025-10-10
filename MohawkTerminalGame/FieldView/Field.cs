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
        Chicken,
        Piglet,
        Pig
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
                PlaceTile(TileType.WheatSeed); // Placeable: Wheat Seed
            }
            else if (Input.IsKeyPressed(ConsoleKey.D2))
            {
                PlaceTile(TileType.CarrotSeed); // Placeable: Carrot Seed
            }
            else if (Input.IsKeyPressed(ConsoleKey.D3))
            {
                PlaceTile(TileType.Calf); // Placeable: Calf
            }
            else if (Input.IsKeyPressed(ConsoleKey.D4))
            {
                PlaceTile(TileType.Chicken); // Placeable: Chicken
            }
            else if (Input.IsKeyPressed(ConsoleKey.D5))
            {
                PlaceTile(TileType.Piglet); // Placeable: Piglet
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
            GetFgColorAndBgForTileType(tileType, out ConsoleColor fg, out ConsoleColor bg);
            return new ColoredText(FieldIcons.Icons[TileType.Dirt], fg, bg);
        }

        static ConsoleColor GetFgColorForTileType(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Dirt: return ConsoleColor.DarkYellow;
                case TileType.WheatSeed: case TileType.Wheat: return ConsoleColor.White;
                case TileType.CarrotSeed: case TileType.Carrot: return ConsoleColor.DarkYellow;
                case TileType.Calf: return ConsoleColor.DarkYellow;
                case TileType.Cow: return ConsoleColor.White;
                case TileType.Chicken: return ConsoleColor.Yellow;
                case TileType.Piglet: return ConsoleColor.DarkYellow;
                case TileType.Pig: return ConsoleColor.Red;
                default: return ConsoleColor.White;
            }
        }

        static void GetFgColorAndBgForTileType(TileType tileType, out ConsoleColor fg, out ConsoleColor bg)
        {
            fg = GetFgColorForTileType(tileType);
            bg = ConsoleColor.DarkGreen;
            if (tileType == TileType.Dirt)
            {
                bg = ConsoleColor.DarkYellow;
            }
        }

        static void DrawTile(int logicalX, int logicalY, TileType tileType, ConsoleColor bg = ConsoleColor.Black)
        {
            var (visualX, visualY) = logicalGrid.LogicalToVisual(logicalX, logicalY);
            GetFgColorAndBgForTileType(tileType, out ConsoleColor defaultFg, out ConsoleColor defaultBg);
            if (bg == ConsoleColor.Black) bg = defaultBg; // Use default bg if not overridden (e.g., for non-highlighted)
            var fg = defaultFg;

            var icon = FieldIcons.Icons[tileType];
            var lines = icon.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // First, fill the entire tile with the background
            for (int yOffset = 0; yOffset < logicalGrid.VisualTileHeight; yOffset++)
            {
                for (int xOffset = 0; xOffset < logicalGrid.VisualTileWidth; xOffset++)
                {
                    field.Poke(visualX + xOffset, visualY + yOffset, new ColoredText(" ", fg, bg));
                }
            }

            // Then, draw the icon on top
            int lineY = 0;
            foreach (var line in lines)
            {
                for (int lineX = 0; lineX < line.Length; lineX++)
                {
                    var ct = new ColoredText(line[lineX].ToString(), fg, bg);
                    field.Poke(visualX + lineX, visualY + lineY, ct);
                }
                lineY++;
            }
        }

        static void DrawField()
        {
            for (int logicalY = 0; logicalY < logicalGrid.Height; logicalY++)
            {
                for (int logicalX = 0; logicalX < logicalGrid.Width; logicalX++)
                {
                    var space = logicalGrid.GetSpace(logicalX, logicalY);

                    // Skip the currently highlighted tile
                    bool isHighlighted = (logicalX == selectionX && logicalY == selectionY);
                    if (isHighlighted)
                        continue;

                    DrawTile(logicalX, logicalY, space.TileType);
                }
            }
            ApplySelectionVisual();
        }

        static void ApplySelectionVisual()
        {
            var selectedSpace = logicalGrid.GetSpace(selectionX, selectionY);
            ConsoleColor bg = isHighlightVisible ? ConsoleColor.Red : ConsoleColor.Black;
            DrawTile(selectionX, selectionY, selectedSpace.TileType, bg);
        }

        // Remove highlight from the previous selection by restoring the underlying tile
        public static void RemoveHighlight()
        {
            var previousSpace = logicalGrid.GetSpace(previousSelectionX, previousSelectionY);
            DrawTile(previousSelectionX, previousSelectionY, previousSpace.TileType);
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

            // Placed count no longer needed - pricing uses Inventory.GetItemCount

            // Update logical grid at current selection
            logicalGrid.SetTileType(selectionX, selectionY, tileType);

            DrawTile(selectionX, selectionY, tileType);

            // Reapply highlight (the tile placement overwrote it)
            ApplySelectionVisual();

            // Play placement sound
            switch (tileType)
            {
                case TileType.WheatSeed:
                    SoundEffects.PlaceWheat();
                    break;
                case TileType.CarrotSeed:
                    SoundEffects.PlaceCarrot();
                    break;
                case TileType.Calf:
                    SoundEffects.PlaceCalf();
                    break;
                case TileType.Chicken:
                    SoundEffects.PlaceChicken();
                    break;
                case TileType.Piglet:
                    SoundEffects.PlacePiglet();
                    break;
            }

            // Update interactions
            FieldInfo.OnSelectionChanged();
        }

        public static void HarvestTile()
        {
            var space = GetCurrentSelectedSpace();
            string harvestName = null;
            switch (space.TileType)
            {
                case TileType.Wheat:
                    harvestName = GameItems.Wheat.HarvestItem; // Wheat
                    break;
                case TileType.Carrot:
                    harvestName = GameItems.Carrot.HarvestItem; // Carrot
                    break;
                case TileType.Calf:
                    harvestName = GameItems.Calf.HarvestItem; // Veal
                    break;
                case TileType.Cow:
                    harvestName = GameItems.Cow.HarvestItem; // Beef
                    break;
                case TileType.Chicken:
                    harvestName = GameItems.Chicken.HarvestItem; // Poultry
                    break;
                case TileType.Pig:
                    harvestName = GameItems.Pig.HarvestItem; // Pork
                    break;
                default:
                    return; // No harvest
            }
            if (harvestName != null)
            {
                Inventory.AddItem(harvestName, 1);
                SoundEffects.Harvest();
                logicalGrid.SetTileType(selectionX, selectionY, TileType.Dirt);
                // Redraw the tile to dirt
                DrawTile(selectionX, selectionY, TileType.Dirt);
                ApplySelectionVisual();
                FieldInfo.OnSelectionChanged();
            }
        }

        public static void FeedTile()
        {
            var space = GetCurrentSelectedSpace();
            if (space.TileType == TileType.Calf)
            {
                // Calf only eats wheat
                int wheatCount = Inventory.GetItemCount(GameItems.Wheat.Name);
                if (wheatCount > 0)
                {
                // Consume wheat to grow calf into cow
                    Inventory.RemoveItem(GameItems.Wheat.Name, 1);
                    // Turn calf to cow
                    logicalGrid.SetTileType(selectionX, selectionY, TileType.Cow);
                    // Redraw
                    DrawTile(selectionX, selectionY, TileType.Cow);
                    // Reapply highlight
                    ApplySelectionVisual();
                    SoundEffects.GrowCow();
                    FieldInfo.OnSelectionChanged();
                }
            }
            else if (space.TileType == TileType.Piglet)
            {
                // Piglet only eats carrots
                int carrotCount = Inventory.GetItemCount(GameItems.Carrot.Name);
                if (carrotCount > 0)
                {
                    // Consume carrot to grow piglet into pig
                    Inventory.RemoveItem(GameItems.Carrot.Name, 1);
                    // Turn piglet to pig
                    logicalGrid.SetTileType(selectionX, selectionY, TileType.Pig);
                    // Redraw
                    DrawTile(selectionX, selectionY, TileType.Pig);
                    // Reapply highlight
                    ApplySelectionVisual();
                    SoundEffects.GrowPig();
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
                        Item item = GameItems.ItemsByName[FieldInfo.GetInventoryKeyForTileType(space.TileType)];
                        if (item != null && space.GrowthProgress >= item.GrowthTime)
                        {
                            // Change to grown version
                            if (space.TileType == TileType.WheatSeed)
                            {
                                logicalGrid.SetTileType(x, y, TileType.Wheat);
                                // Redraw the tile
                                DrawTile(x, y, TileType.Wheat);
                            }
                            else if (space.TileType == TileType.CarrotSeed)
                            {
                                logicalGrid.SetTileType(x, y, TileType.Carrot);
                                // Redraw the tile
                                DrawTile(x, y, TileType.Carrot);
                            }
                            // Animals are already grown
                        }

                        // Advance turn for field item effects
                        item?.AdvanceTurn();
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
                case TileType.Piglet:
                    interactions.Add(InteractionType.Feed);
                    break;
                case TileType.Pig:
                    interactions.Add(InteractionType.Harvest);
                    break;
                case TileType.Dirt:
                    // No interactions for dirt
                    break;
            }

            return interactions;
        }

        // Build string grid for bonus detection
        static string[,] BuildGridNames()
        {
            string[,] grid = new string[logicalGrid.Height, logicalGrid.Width];
            for (int y = 0; y < logicalGrid.Height; y++)
            {
                for (int x = 0; x < logicalGrid.Width; x++)
                {
                    var space = logicalGrid.GetSpace(x, y);
                    if (space.TileType == TileType.Pig)
                    {
                        grid[y, x] = "Pig";
                    }
                    else if (space.TileType == TileType.Carrot)
                    {
                        grid[y, x] = "Carrot";
                    }
                    else if (space.TileType == TileType.Cow)
                    {
                        grid[y, x] = "Cow";
                    }
                    else
                    {
                        grid[y, x] = "";
                    }
                }
            }
            return grid;
        }

        // Detect Pig-Carrot adjacency bonuses
        static int DetectPigBonuses(string[,] grid)
        {
            int bonusCount = 0;
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            // Check horizontal adjacencies (Pig-Carrot next to each other in row)
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols - 1; col++)
                {
                    if ((grid[row, col] == "Pig" && grid[row, col + 1] == "Carrot") ||
                        (grid[row, col] == "Carrot" && grid[row, col + 1] == "Pig"))
                    {
                        bonusCount++;
                    }
                }
            }

            // Check vertical adjacencies (Pig-Carrot above/below each other in column)
            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows - 1; row++)
                {
                    if ((grid[row, col] == "Pig" && grid[row + 1, col] == "Carrot") ||
                        (grid[row, col] == "Carrot" && grid[row + 1, col] == "Pig"))
                    {
                        bonusCount++;
                    }
                }
            }

            return bonusCount;
        }

        // Detect three consecutive Cow bonuses
        static int DetectCowBonuses(string[,] grid)
        {
            int bonusCount = 0;
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            // Check horizontal three-in-a-row
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols - 2; col++)
                {
                    if (grid[row, col] == "Cow" && grid[row, col + 1] == "Cow" && grid[row, col + 2] == "Cow")
                    {
                        bonusCount++;
                    }
                }
            }

            // Check vertical three-in-a-column
            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows - 2; row++)
                {
                    if (grid[row, col] == "Cow" && grid[row + 1, col] == "Cow" && grid[row + 2, col] == "Cow")
                    {
                        bonusCount++;
                    }
                }
            }

            return bonusCount;
        }

        // Perform grid bonus checks and apply money rewards
        public static void PerformGridBonusChecks()
        {
            string[,] grid = BuildGridNames();

            // Detect Pig bonuses
            int pigBonuses = DetectPigBonuses(grid);
            if (pigBonuses > 0)
            {
                Inventory.AddMoney(pigBonuses * 20);
            }

            // Detect Cow bonuses
            int cowBonuses = DetectCowBonuses(grid);
            if (cowBonuses > 0)
            {
                Inventory.AddMoney(cowBonuses * 20);
            }
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
