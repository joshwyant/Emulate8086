using Emulate8086.Assembly.Ast;

namespace Emulate8086.Assembly.IntelSyntax;

public class IntelParser : Parser
{
    public IntelParser(Lexer lexer)
    {
        Lexer = lexer;
    }

    public Lexer Lexer { get; }

    public override async Task<ProgramNode> Parse(string assembly)
    {
        var tokenStream = Lexer.Lex(assembly);
        var statements = new List<StatementNode>();

        while (await tokenStream.MoveNextAsync())
        {
            var token = tokenStream.Current;
            var statement = await ParseStatement(tokenStream);
            statements.Add(statement);
        }

        return new ProgramNode(statements);
    }

    private async Task<StatementNode> ParseStatement(IAsyncEnumerable<Token> tokenStream)
    {
        var token = await tokenStream.MoveNextAsync();
        if (token is null)
        {
            throw new InvalidOperationException("Unexpected end of input");
        }

        if (token.Type == TokenType.Label)
        {
            return new LabelNode(token.Value);
        }

        if (token.Type == TokenType.Instruction)

        throw new NotImplementedException();
    }
    
}
