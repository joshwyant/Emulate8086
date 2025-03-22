namespace Emulate8086.Assembly.Tokens;

public abstractclass Token
{
    public Token(TokenType type,int line, int column)
    {
        Type = type;
        Line = line;
        Column = column;
    }

    public TokenType Type { get; }
    public int Line { get; }
    public int Column { get; }
}