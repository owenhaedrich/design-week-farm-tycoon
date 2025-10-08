using System;

namespace MohawkTerminalGame;

public enum GameState { Story, Field, Shop, Paused }

public class TerminalGame
{
    GameState gameState = GameState.Field;
    GameState gameStateBeforePause = GameState.Field;
    bool justPaused = false;

    Shop shop = new Shop();
    Story story = new Story();

    /// Run once before Execute begins
    public void Setup()
    {
        Program.TerminalExecuteMode = TerminalExecuteMode.ExecuteTime;
        Program.TerminalInputMode = TerminalInputMode.EnableInputDisableReadLine;
        Program.TargetFPS = 20;
        Terminal.CursorVisible = false;
        Terminal.SetTitle("Title");

        DayTimer.ResetDay();
        FieldView.Start();
    }

    // Execute() runs based on Program.TerminalExecuteMode (assign to it in Setup).
    //  ExecuteOnce: runs only once. Once Execute() is done, program closes.
    //  ExecuteLoop: runs in infinite loop. Next iteration starts at the top of Execute().
    //  ExecuteTime: runs at timed intervals (eg. "FPS"). Code tries to run at Program.TargetFPS.
    //               Code must finish within the alloted time frame for this to work well.
    public void Execute()
    {
        // Pausing and Unpausing
        if (gameState == GameState.Paused)
        {
            if (justPaused)
            {
                Terminal.SetCursorPosition((Viewport.windowWidth / 2) - 3, Viewport.windowHeight / 2);
                Terminal.BackgroundColor = ConsoleColor.Black;
                Terminal.ForegroundColor = ConsoleColor.Red;
                Terminal.WriteLine("PAUSE");
                Viewport.HideCursor();
                justPaused = false;
            }
            else if (Input.IsKeyPressed(ConsoleKey.P))
            {
                gameState = gameStateBeforePause;
                if (gameState == GameState.Field)
                {
                    FieldView.Unpause();
                }
                return;
            }
        }
        else if (Input.IsKeyPressed(ConsoleKey.P))
        {
            gameStateBeforePause = gameState;
            gameState = GameState.Paused;
            justPaused = true;
            return;
        }

        // Update day timer
        DayTimer.Update();

        // Check for day expiry
        if (DayTimer.DayExpired)
        {
            gameState = GameState.Story;
        }

        // Playing states
        switch (gameState)
        {
            case GameState.Story:
                if (story.Execute())
                {
                    gameState = GameState.Field;
                    Console.Clear();
                    FieldView.Start();
                }
                break;

            case GameState.Field:
                FieldView.Execute();

                if (Input.IsKeyPressed(ConsoleKey.E))
                {
                    gameState = GameState.Shop;
                    Console.Clear();
                    shopNeedsRedraw = true;
                }
                break;

            case GameState.Shop:
                RunShop();
                break;
        }
    }

    private bool shopNeedsRedraw = true;

    private void RunShop()
    {
        if (shopNeedsRedraw)
        {
            shop.Show();
            shopNeedsRedraw = false;
        }

        string input = null;

        if (Input.IsKeyPressed(ConsoleKey.D1)) input = "1";
        else if (Input.IsKeyPressed(ConsoleKey.D2)) input = "2";
        else if (Input.IsKeyPressed(ConsoleKey.D3)) input = "3";
        else if (Input.IsKeyPressed(ConsoleKey.D4)) input = "4";
        else if (Input.IsKeyPressed(ConsoleKey.S)) input = "s";
        else if (Input.IsKeyPressed(ConsoleKey.B)) input = "b";
        else if (Input.IsKeyPressed(ConsoleKey.E)) input = "e";

        if (input == null) return;

        if (input == "e")
        {
            gameState = GameState.Field;
            Console.Clear();
            FieldView.Start(); // resume field mode
            return;
        }

        shop.HandleInput(input);
        shopNeedsRedraw = true;
    }
}
