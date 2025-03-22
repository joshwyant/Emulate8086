namespace Emulate8086.Assembly.Ast;

public class ImmediateExpression : Expression
{
    public ImmediateExpression(byte[] data)
    {
        Data = data;
    }

    public byte[] Data { get; }
}
