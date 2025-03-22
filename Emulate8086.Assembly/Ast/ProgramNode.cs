namespace Emulate8086.Assembly.Ast;

public class ProgramNode : AstNode
{
    public ProgramNode(IEnumerable<StatementNode> statements)
    {
        Statements = statements.ToArray();
    }

    public IEnumerable<StatementNode> Statements { get; }
}
