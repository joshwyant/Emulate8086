using Emulate8086.Assembly.Ast;

namespace Emulate8086.Assembly.IntelSyntax;

public class IntelPrettyPrinter : Visitor
{
    public override void VisitLabelStatement(Ast.LabelStatement labelStatement)
    {
        sb.AppendLine($"{labelStatement.Label}:");
    }

    public override void VisitDirectiveStatement(Ast.DirectiveStatement directiveStatement)
    {
        PrintIndent();
        sb.AppendLine($".{directiveStatement.Directive}");
    }

    public override void VisitDataStatement(Ast.DataStatement dataStatement)
    {
        PrintIndent();
        sb.AppendLine($"db {string.Join(", ", string.Format("{0:X2}h", dataStatement.Data))}");
    }

    public override void VisitRegisterExpression(Ast.RegisterExpression registerExpression)
    {
        sb.Append(registerExpression.Register);
    }
    
    public override void VisitImmediateExpression(Ast.ImmediateExpression immediateExpression)
    {
        sb.Append($"${immediateExpression.Value:X}h");
    }

    public override void VisitBinaryExpression(Ast.BinaryExpression binaryExpression)
    {
        sb.Append($"{binaryExpression.Destination}, {binaryExpression.Source}");
    }

    public override void VisitStatement(Ast.Statement statement)
    {
        switch (statement)
        {
            case Ast.LabelStatement labelStatement:
                VisitLabelStatement(labelStatement);
                break;
            case Ast.DirectiveStatement directiveStatement:
                VisitDirectiveStatement(directiveStatement);
                break;
            case Ast.DataStatement dataStatement:
                VisitDataStatement(dataStatement);
                break;
            case Ast.InstructionStatement instructionStatement:
                VisitInstructionStatement(instructionStatement);
                break;
        }
    }

    public override void VisitInstructionStatement(Ast.InstructionStatement instruction)
    {
        PrintIndent();
        sb.Append(instruction.Name);
        if (instruction is VariableInstruction variableInstruction)
        {
            sb.Append($" ");
            Visit(variableInstruction.Expression);
        }
        sb.AppendLine();
    }
}   
