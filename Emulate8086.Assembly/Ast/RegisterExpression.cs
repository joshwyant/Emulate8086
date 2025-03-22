namespace Emulate8086.Assembly.Ast;

public class RegisterExpression : Expression
{
    public RegisterExpression(string register)
    {
        Register = register;
    }

    public string Register { get; }
}
