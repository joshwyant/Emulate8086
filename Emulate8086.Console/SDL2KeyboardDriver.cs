namespace Emulate8086.Console;

class SDL2KeyboardDriver : KeyboardDriver
{
    SDL2Session session;
    public SDL2KeyboardDriver(SDL2Session session)
    {
        this.session = session;
    }
    public override bool CheckForKey(out char ascii, out byte scancode)
    {
        throw new NotImplementedException();
    }

    public override (char ascii, byte scancode) WaitForKey()
    {
        throw new NotImplementedException();
    }
}