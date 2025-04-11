namespace Emulate8086.Console;

abstract class KeyboardDriver
{
    public abstract (char ascii, byte scancode) WaitForKey();
    public abstract bool CheckForKey(out char ascii, out byte scancode);
    // https://stanislavs.org/helppc/scan_codes.html
    protected static byte[] LetterScanCodes = [0x1e, 0x30, 0x2e, 0x20, 0x12, 0x21, 0x22, 0x23, 0x17, 0x24, 0x25, 0x26, 0x32, 0x31, 0x18, 0x19, 0x10, 0x13, 0x1f, 0x14, 0x16, 0x2f, 0x11, 0x2D, 0x15, 0x2c];
    protected static byte[] FunctionScanCodes = [0x3b, 0x3c, 0x3d, 0x3e, 0x3f, 0x40, 0x41, 0x42, 0x43, 0x44, 0x85, 0x86];
    static protected byte MapASCIIToScanCode(char c)
    {
        c = char.ToUpperInvariant(c);
        if (c >= 'A' && c <= 'Z')
        {
            return LetterScanCodes[c - 'A'];
        }
        if (c >= '0' && c <= '9')
        {
            return (byte)(c == '0' ? 0x30 : 0x31 + (c - '1'));
        }
        switch (c)
        {
            case '-':
            case '_':
                return 0x0C;
            case '=':
            case '+':
                return 0x0D;
            case '[':
            case '{':
                return 0x1A;
            case ']':
            case '}':
                return 0x1B;
            case ';':
            case ':':
                return 0x27;
            case '\'':
            case '"':
                return 0x28;
            case '`':
            case '~':
                return 0x29;
            case '\\':
            case '|':
                return 0x2B;
            case ',':
            case '<':
                return 0x33;
            case '.':
            case '>':
                return 0x34;
            case '/':
            case '?':
                return 0x35;
            case '\n':
            case '\r':
                return 0x1C;
            case '\b':
                return 0x0E;
            case '\t':
                return 0x0F;
            case ' ':
                return 0x39;
            case '!':
            case '@':
            case '#':
            case '$':
            case '%':
            case '&':
            case '*':
            case '(':
            case ')':
                return (byte)(0x02 + "!@#$%^&*()".IndexOf(c));
            default:
                return 0;
        }
    }
}