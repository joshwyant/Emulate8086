namespace Emulate8086.Meta.Amd64;

using Tags = Amd64InstructionTags;
using Set686 = Intel686.Intel686InstructionSet;
using Pattern = InstructionOpcodePattern;
using Flags = InstructionPatternFlags;
using Def = InstructionDefinition;

public class Amd64InstructionSet : Set686
{
    public static new Amd64InstructionSet Create()
    {
        return new Amd64InstructionSet(
            Def.GetAll(typeof(Def)));
    }
    protected Amd64InstructionSet(IEnumerable<Def> instructions)
        : base(instructions) { }

    #region Patterns

    #endregion

    #region Definitions
    #endregion
    public override InstructionTag[,] InstructionMatrix { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
}