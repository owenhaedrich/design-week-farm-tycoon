using System;

namespace MohawkTerminalGame;

public enum GameState { Story, Field, Shop, Paused }

public class TerminalGame
{
    GameState gameState = GameState.Field;
    GameState gameStateBeforePause = GameState.Field;
    bool justPaused = false;

    Shop shop = new Shop();

    /// Run once before Execute begins
    public void Setup()
    {
        Program.TerminalExecuteMode = TerminalExecuteMode.ExecuteTime;
        Program.TerminalInputMode = TerminalInputMode.EnableInputDisableReadLine;
        Program.TargetFPS = 20;
        Terminal.CursorVisible = false;
        Terminal.SetTitle("Title");

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

        // Playing states
        switch (gameState)
        {
            case GameState.Field:
                FieldView.Execute();

                // Checks if timer in FieldInfo reaches 0, then goes to Shop
                if (FieldInfo.TimerExpired)
                {
                    FieldInfo.TimerExpired = false;
                    gameState = GameState.Shop;
                    Console.Clear();
                }

                // Manually open the shop using s (Uncomment code, useful for debugging)
             //   if (Input.IsKeyPressed(ConsoleKey.S))
             //   {
             //       gameState = GameState.Shop;
             //       Console.Clear();
             //   }

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
        else if (Input.IsKeyPressed(ConsoleKey.E)) input = "exit";

        if (input == null) return;

        if (input == "exit")
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
