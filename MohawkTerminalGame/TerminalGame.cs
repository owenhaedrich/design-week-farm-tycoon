using System;

namespace MohawkTerminalGame;

public enum GameState { Story, Field, Shop, Paused }

public class TerminalGame
{
    GameState gameState = GameState.Story;
    GameState gameStateBeforePause = GameState.Story;
    bool justPaused = false;

    Shop shop = new Shop();
    Story story = new Story();

    bool isFirstStory = true;

    /// Run once before Execute begins
    public void Setup()
    {
        Program.TerminalExecuteMode = TerminalExecuteMode.ExecuteTime;
        Program.TerminalInputMode = TerminalInputMode.EnableInputDisableReadLine;
        Program.TargetFPS = 20;
        Terminal.CursorVisible = false;
        Terminal.SetTitle("Title");

        story.Mode = StoryMode.Intro;
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
                    Field.Unpause();
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
            case GameState.Story:
                if (story.PlayAndWait())
                {
                    gameState = GameState.Field;
                    DayTimer.ResetDay();
                    Terminal.Clear();
                    Field.Start();
                    DayTimer.CheckAndDraw();
                    isFirstStory = false;
                }
                break;

            case GameState.Field:
                // Update day timer
                DayTimer.Update();

                // Check for day expiry
                if (DayTimer.DayExpired)
                {
                    gameState = GameState.Story;
                    story.Mode = DayTimer.DayNumber >= 10 ? StoryMode.Ending : StoryMode.Progress;
                    break;
                }

                Field.Execute();

                if (Input.IsKeyPressed(ConsoleKey.E) || Input.IsKeyPressed(ConsoleKey.S))
                {
                    gameState = GameState.Shop;
                    Terminal.Clear();
                    shopNeedsRedraw = true;
                }
                DayTimer.CheckAndDraw();
                break;

            case GameState.Shop:
                // Update day timer
                DayTimer.Update();

                // Check for day expiry
                if (DayTimer.DayExpired)
                {
                    gameState = GameState.Story;
                    story.Mode = DayTimer.DayNumber >= 10 ? StoryMode.Ending : StoryMode.Progress;
                    break;
                }

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
            DayTimer.Draw(); // Always draw timer after showing shop
            shopNeedsRedraw = false;
        }

        DayTimer.CheckAndDraw(); // Update timer UI every frame in shop

        ShopInputResult result = shop.HandleInput();
        if (result == ShopInputResult.Handled)
        {
            shopNeedsRedraw = true;
        }
        else if (result == ShopInputResult.Exit)
        {
            gameState = GameState.Field;
            Terminal.Clear();
            Field.Start(); // resume field mode
            DayTimer.CheckAndDraw();
            return;
        }
        // else None, do nothing
    }
}
