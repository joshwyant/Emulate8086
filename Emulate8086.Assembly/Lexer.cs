namespace Emulate8086.Assembly;

public abstract class Lexer
{
    public abstract async IAsyncEnumerable<Token> Lex(string assembly);
}