using System;
using DefaultNamespace;

namespace MohawkTerminalGame;

public enum GameState { Story, Field, Shop, Paused, TitleScreen }

public class TerminalGame
{
    GameState gameState = GameState.TitleScreen;
    GameState gameStateBeforePause = GameState.Story;
    bool justPaused = false;

    Shop shop = new Shop();
    Story story = new Story();
    TitleScreen titleScreen = new TitleScreen();

    bool isFirstStory = true;
    bool hasDisplayedTitleScreen = false;

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
                    if (story.Mode == StoryMode.Ending)
                    {
                        gameState = GameState.TitleScreen;
                        isFirstStory = true;
                        story.Mode = StoryMode.Intro;
                        hasDisplayedTitleScreen = false;
                    }
                    else
                    {
                        gameState = GameState.Field;
                        if (isFirstStory)
                        {
                            DayTimer.ResetDay();
                            isFirstStory = false;
                        }
                        Terminal.Clear();
                        Field.Start();
                        DayTimer.CheckAndDraw();
                    }
                }
                break;

            case GameState.Field:
                // Update day timer
                DayTimer.Update();

                // Check for day expiry
                if (DayTimer.DayExpired)
                {
                    DayTimer.ResetDay();
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
                    DayTimer.ResetDay();
                    gameState = GameState.Story;
                    story.Mode = DayTimer.DayNumber >= 10 ? StoryMode.Ending : StoryMode.Progress;
                    break;
                }

                RunShop();
                break;

            case GameState.TitleScreen:
                HandleTitleScreen();
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

    private void HandleTitleScreen()
    {
        if (!hasDisplayedTitleScreen)
        {
            Terminal.Clear();
            titleScreen.Display();
            hasDisplayedTitleScreen = true;
        }

        if (Input.IsKeyPressed(ConsoleKey.Spacebar) || Input.IsKeyPressed(ConsoleKey.Enter))
        {
            gameState = GameState.Story;
            hasDisplayedTitleScreen = false;
            story.Mode = StoryMode.Intro;
        }
    }
}
