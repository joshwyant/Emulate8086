namespace Emulate8086.Meta.Intel686;

using Tags = Intel686InstructionTags;
using Set586 = Intel586.Intel586InstructionSet;
using Pattern = InstructionOpcodePattern;
using Flags = InstructionPatternFlags;
using Def = InstructionDefinition;

public class Intel686InstructionSet : Set586
{
    public static new Intel686InstructionSet Create()
    {
        return new Intel686InstructionSet(
            Def.GetAll(typeof(Def)));
    }
    protected Intel686InstructionSet(IEnumerable<Def> instructions)
        : base(instructions) { }

    #region Patterns

    #endregion

    #region Definitions
    #endregion
    public override InstructionTag[,] InstructionMatrix { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
}