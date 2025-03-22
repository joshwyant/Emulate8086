namespace Emulate8086.Assembly.IntelSyntax;

public class IntelLexer : Lexer
{
    public override async IAsyncEnumerable<Token> Lex(IAsyncEnumerable<char> assembly)
    {
        while (assembly.MoveNext())
        {   
            var c = assembly.Current;
            if (char.IsWhiteSpace(c))
            {
                continue;
            }
            if (c == ';')
            {
                // Skip the rest of the line
                while (assembly.MoveNext() && assembly.Current != '\n')
                {
                    continue;
                }
                continue;
            }
            if (c == '(')
            {
                // Skip the rest of the line
                while (assembly.MoveNext() && assembly.Current != ')')
                {
                    continue;
                }
                continue;
            }
            if (c == '[')
            {
                yield return new Token(TokenType.OpenBracket);
                continue;
            }
            if (c == ']')
            {
                yield return new Token(TokenType.CloseBracket);
                continue;
            }
            if (c == ',')
            {
                yield return new Token(TokenType.Comma);
                continue;
            }
            if (c == ':')
            {
                yield return new Token(TokenType.Colon);
                continue;
            }
            if (c == '.')
            
            
    }
}