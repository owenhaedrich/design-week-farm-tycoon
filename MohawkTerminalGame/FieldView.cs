using System;
using System.Numerics;

namespace MohawkTerminalGame
{
    // Enum for different tile types
    public enum TileType
    {
        Dirt,
        Wheat,
        Cow
    }

    // Manages the field view. There is a logical grid that determines what the visuals should be.
    public class FieldView
    {
        // Visual elements
        static ColoredText highlight = new("X", ConsoleColor.Red, ConsoleColor.Black);
        static ColoredText dirt = new("░", ConsoleColor.DarkYellow, ConsoleColor.DarkYellow);

        // Game elements
        static int selectionX = 0;
        static int selectionY = 0;
        internal static TerminalGridWithColor field = new(30, 10, dirt);
        internal static LogicalGrid logicalGrid = new(15, 5, 2, 2); // For now each logical tile is 2x2 in visual grid

        // Map tile types to visual representations
        static ColoredText GetVisualForTileType(TileType tileType)
        {
            // Use a traditional switch statement with case/break
            switch (tileType)
            {
                case TileType.Dirt:
                    return dirt;
                case TileType.Wheat:
                    return new ColoredText("w", ConsoleColor.Yellow, ConsoleColor.Green); ;
                case TileType.Cow:
                    return new ColoredText("C", ConsoleColor.White, ConsoleColor.Black);
                default:
                    return dirt;
            }
        }

        public static void ViewField()
        {
            // Sync visual grid with logical grid
            SyncVisualWithLogical();
            // Reapply highlight after sync
            ApplyHighlight();
            field.ClearWrite();
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

                    // Fill the entire visual tile
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

            // Highlight the visual tile
            for (int yOffset = 0; yOffset < logicalGrid.VisualTileHeight; yOffset++)
            {
                for (int xOffset = 0; xOffset < logicalGrid.VisualTileWidth; xOffset++)
                {
                    field.Poke(visualX + xOffset, visualY + yOffset, highlight);
                }
            }
        }

        public static void MoveSelection(int x, int y)
        {
            // Remove current highlight (redrawing the tile)
            var currentSpace = logicalGrid.GetSpace(selectionX, selectionY);
            var visual = GetVisualForTileType(currentSpace.TileType);
            var (oldVisualX, oldVisualY) = logicalGrid.LogicalToVisual(selectionX, selectionY);

            for (int yOffset = 0; yOffset < logicalGrid.VisualTileHeight; yOffset++)
            {
                for (int xOffset = 0; xOffset < logicalGrid.VisualTileWidth; xOffset++)
                {
                    field.Poke(oldVisualX + xOffset, oldVisualY + yOffset, visual);
                }
            }

            // Update selection position (logical coordinates)
            selectionX += x;
            selectionY += y;

            // Clamp selection in logical grid
            selectionX = Math.Clamp(selectionX, 0, logicalGrid.Width - 1);
            selectionY = Math.Clamp(selectionY, 0, logicalGrid.Height - 1);

            ApplyHighlight();
        }

        public static void PlaceTile(TileType tileType)
        {
            // Update logical grid at current selection
            logicalGrid.SetTileType(selectionX, selectionY, tileType);

            // Draw tile on the visual grid
            var visual = GetVisualForTileType(tileType);
            var (visualX, visualY) = logicalGrid.LogicalToVisual(selectionX, selectionY);

            for (int yOffset = 0; yOffset < logicalGrid.VisualTileHeight; yOffset++)
            {
                for (int xOffset = 0; xOffset < logicalGrid.VisualTileWidth; xOffset++)
                {
                    field.Poke(visualX + xOffset, visualY + yOffset, visual);
                }
            }

            // Reapply highlight (just overwrote it)
            ApplyHighlight();
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
