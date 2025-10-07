using System;
using System.Collections.Generic;

namespace MohawkTerminalGame
{
    // Enum for different tile types
    public enum TileType
    {
        Dirt,
        Wheat,
        Cow,
        Chicken
    }

    // Interaction Types
    public enum InteractionType
    {
        Feed, // For animals
        Harvest, // For both animals and crops
    }

    public class FieldView
    {
        // Visual elements
        static ColoredText highlight = new("X", ConsoleColor.Red, ConsoleColor.Black);
        static ColoredText dirt = new("░", ConsoleColor.DarkYellow, ConsoleColor.DarkYellow);
        static int visualScaleX = 8;
        static int visualScaleY = 4;

        // Game elements
        public static int selectionX = 0;
        public static int selectionY = 0;
        static int previousSelectionX = 0;
        static int previousSelectionY = 0;
        internal static TerminalGridWithColor field = new(Viewport.windowWidth, Viewport.windowHeight, dirt);
        internal static LogicalGrid logicalGrid = new(Viewport.windowWidth / visualScaleX, Viewport.windowHeight / visualScaleY, visualScaleX, visualScaleY);

        // Initialization
        public static void ViewField()
        {
            SyncVisualWithLogical();
            ApplyHighlight();
            field.ClearWrite();
        }

        // Map tile types to visual representations
        static ColoredText GetVisualForTileType(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Dirt:
                    return dirt;
                case TileType.Wheat:
                    return new ColoredText("w", ConsoleColor.Yellow, ConsoleColor.Green);
                case TileType.Cow:
                    return new ColoredText("C", ConsoleColor.White, ConsoleColor.Black);
                case TileType.Chicken:
                    return new ColoredText("c", ConsoleColor.Yellow, ConsoleColor.DarkYellow);
                default:
                    return dirt;
            }
        }

        static void SyncVisualWithLogical()
        {
            for (int logicalY = 0; logicalY < logicalGrid.Height; logicalY++)
            {
                for (int logicalX = 0; logicalX < logicalGrid.Width; logicalX++)
                {
                    var space = logicalGrid.GetSpace(logicalX, logicalY);
                    var visual = GetVisualForTileType(space.TileType);
                    var (visualX, visualY) = logicalGrid.LogicalToVisual(logicalX, logicalY);

                    for (int yOffset = 0; yOffset < logicalGrid.VisualTileHeight; yOffset++)
                    {
                        for (int xOffset = 0; xOffset < logicalGrid.VisualTileWidth; xOffset++)
                        {
                            field.Poke(visualX + xOffset, visualY + yOffset, visual);
                        }
                    }
                }
            }
        }

        // Apply highlight to the currently selected logical tile
        static void ApplyHighlight()
        {
            var (visualX, visualY) = logicalGrid.LogicalToVisual(selectionX, selectionY);

            for (int yOffset = 0; yOffset < logicalGrid.VisualTileHeight; yOffset++)
            {
                for (int xOffset = 0; xOffset < logicalGrid.VisualTileWidth; xOffset++)
                {
                    field.Poke(visualX + xOffset, visualY + yOffset, highlight);
                }
            }
        }

        // Remove highlight from the previous selection by restoring the underlying tile
        static void RemoveHighlight()
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
                ApplyHighlight();
                FieldInfoBar.OnSelectionChanged();
            }
        }

        public static void PlaceTile(TileType tileType)
        {
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
            ApplyHighlight();
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
                case TileType.Cow:
                    interactions.Add(InteractionType.Feed);
                    interactions.Add(InteractionType.Harvest);
                    break;
                case TileType.Chicken:
                    interactions.Add(InteractionType.Feed);
                    interactions.Add(InteractionType.Harvest);
                    break;
                case TileType.Wheat:
                    interactions.Add(InteractionType.Harvest);
                    break;
                case TileType.Dirt:
                    // No interactions for dirt
                    break;
            }

            return interactions;
        }
    }
    
    // Represents a single space on the logical grid
    public class GridSpace
    {
        public int X;
        public int Y;
        public TileType TileType;

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
            }
        }

        // Convert visual coordinates to logical coordinates
        public (int logicalX, int logicalY) VisualToLogical(int visualX, int visualY)
        {
            return (visualX / VisualTileWidth, visualY / VisualTileHeight);
        }

        // Convert logical coordinates to visual coordinates (top-left corner)
        public (int visualX, int visualY) LogicalToVisual(int logicalX, int logicalY)
        {
            return (logicalX * VisualTileWidth, logicalY * VisualTileHeight);
        }

        // Get the tile type at visual coordinates
        public TileType GetTileTypeAtVisual(int visualX, int visualY)
        {
            var (logicalX, logicalY) = VisualToLogical(visualX, visualY);
            var space = GetSpace(logicalX, logicalY);
            return space.TileType;
        }
    }
}
