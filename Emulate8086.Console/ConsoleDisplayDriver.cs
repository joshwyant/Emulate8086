namespace Emulate8086.Console;
using System;

class ConsoleDisplayDriver : DisplayDriver
{
    public override int BackgroundColor
    {
        get => (int)Console.BackgroundColor;
        set => Console.BackgroundColor = (ConsoleColor)value;
    }
    public override int ForegroundColor
    {
        get => (int)Console.ForegroundColor;
        set => Console.ForegroundColor = (ConsoleColor)value;
    }
    public override bool CursorVisible
    {
        set => Console.CursorVisible = value;
    }

    public override void Clear()
        => Console.Clear();

    public override (int left, int top) GetCursorPosition()
        => Console.GetCursorPosition();

    public override void SetCursorPosition(int left, int top)
        => Console.SetCursorPosition(left, top);

    public override void Write(char c)
        => Console.Write(c);
}