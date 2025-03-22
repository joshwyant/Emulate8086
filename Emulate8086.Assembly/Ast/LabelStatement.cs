namespace Emulate8086.Assembly.Ast;

public class LabelStatement : Statement
{
    public LabelStatement(string label)
    {
        Label = label;
    }

    public string Label { get; }
}
