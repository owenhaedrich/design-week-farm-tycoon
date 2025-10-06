using System;

namespace MohawkTerminalGame;

public class TerminalGame
{
    // Place your variables here


    /// Run once before Execute begins
    public void Setup()
    {
        Program.TerminalExecuteMode = TerminalExecuteMode.ExecuteTime;
        Program.TerminalInputMode = TerminalInputMode.EnableInputDisableReadLine;
        Program.TargetFPS = 60;
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
        int moveX = 0;
        int moveY = 0;

        if (Input.IsKeyPressed(ConsoleKey.RightArrow))
            moveX++;
        if (Input.IsKeyPressed(ConsoleKey.LeftArrow))
            moveX--;
        if (Input.IsKeyPressed(ConsoleKey.DownArrow))
            moveY++;
        if (Input.IsKeyPressed(ConsoleKey.UpArrow))
            moveY--;


        if (moveX != 0 || moveY != 0)
        {
            FieldView.MoveSelection(moveX, moveY);
        }
    }
}
