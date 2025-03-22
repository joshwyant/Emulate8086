namespace Emulate8086.Assembly.Ast;

public class BinaryExpression : Expression
{
    public BinaryExpression(Expression destination, Expression source)
    {
        Destination = destination;
        Source = source;
    }

    public Expression Destination { get; }
    public Expression Source { get; }
}
