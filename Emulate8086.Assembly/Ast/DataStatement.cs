namespace Emulate8086.Assembly.Ast;

public class DataStatement : Statement
{
    public DataStatement(byte[] data)
    {
        Data = data;
    }

    public byte[] Data { get; }
}
