namespace Emulate8086.Console;

abstract class DisplayDriver
{
    public abstract int BackgroundColor { get; set; }
    public abstract int ForegroundColor { get; set; }
    public abstract void SetCursorPosition(int left, int top);
    public abstract (int left, int top) GetCursorPosition();
    public abstract bool CursorVisible { set; }
    public abstract void Write(char c);
    public abstract void Clear();
}