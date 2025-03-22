namespace Emulate8086.Assembly;

public abstractclass PrettyPrinter : Ast.Visitor
{
    public void Print(Ast.ProgramNode program)
    {
        Visit(program);
    }

    protected StringBuilder sb = new();
    protected int indent = 0;
    protected int indentSize = 4;

    protected void PrintIndent()
    {
        sb.Append(' ', indentSize * indent);
    }    
}
