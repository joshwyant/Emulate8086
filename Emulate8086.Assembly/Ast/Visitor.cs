namespace Emulate8086.Assembly.Ast;

public abstract class Visitor
{
    public void Visit(ProgramNode program)
    {
        foreach (var statement in program.Statements)
        {
            Visit(statement);
        }
    }


    public void VisitStatement(Statement statement)
    {
        switch (statement)
        {
            case LabelStatement labelStatement:
                VisitLabelStatement(labelStatement);
                break;
            case DirectiveStatement directiveStatement:
                VisitDirectiveStatement(directiveStatement);
                break;
            case DataStatement dataStatement:
                VisitDataStatement(dataStatement);
                break;
        }
    }

    public abstract void VisitLabelStatement(LabelStatement labelStatement);
    public abstract void VisitDirectiveStatement(DirectiveStatement directiveStatement);
    public abstract void VisitDataStatement(DataStatement dataStatement);

    public void VisitExpression(Expression expression)
    {
        switch (expression)
        {
            case RegisterExpression registerExpression:
                VisitRegisterExpression(registerExpression);
                break;
            case ImmediateExpression immediateExpression:
                VisitImmediateExpression(immediateExpression);
                break;
            case BinaryExpression binaryExpression:
                VisitBinaryExpression(binaryExpression);
                break;
        }
    }

    public abstract void VisitRegisterExpression(RegisterExpression registerExpression);
    public abstract void VisitImmediateExpression(ImmediateExpression immediateExpression);
    public abstract void VisitBinaryExpression(BinaryExpression binaryExpression);
}
