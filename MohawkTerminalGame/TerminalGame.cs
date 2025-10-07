using System;

namespace MohawkTerminalGame;

public enum GameState { Story, Field, Shop, Paused }

public class TerminalGame
{
    GameState gameState = GameState.Field;
    GameState gameStateBeforePause = GameState.Field;
    bool justPaused = false;

    /// Run once before Execute begins
    public void Setup()
    {
        Program.TerminalExecuteMode = TerminalExecuteMode.ExecuteTime;
        Program.TerminalInputMode = TerminalInputMode.EnableInputDisableReadLine;
        Program.TargetFPS = 20;
        Terminal.CursorVisible = false;
        Terminal.SetTitle("Title");

        FieldView.ViewField();
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
                    FieldView.ViewField();
                }
            }
        }
        else if (Input.IsKeyPressed(ConsoleKey.P))
        {
            gameStateBeforePause = gameState;
            gameState = GameState.Paused;
            justPaused = true;
        }

        // Playing states
        if (gameState == GameState.Field)
        {
            FieldView.Execute();
        }
    }
}
