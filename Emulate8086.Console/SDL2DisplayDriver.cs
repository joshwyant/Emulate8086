namespace Emulate8086.Console;

class SDL2DisplayDriver : DisplayDriver
{
    SDL2Session session;
    public SDL2DisplayDriver(SDL2Session session)
    {
        this.session = session;
    }
    public override int BackgroundColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override int ForegroundColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override bool CursorVisible { set => throw new NotImplementedException(); }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override (int left, int top) GetCursorPosition()
    {
        throw new NotImplementedException();
    }

    public override void SetCursorPosition(int left, int top)
    {
        throw new NotImplementedException();
    }

    public override void Write(char c)
    {
        throw new NotImplementedException();
    }
}