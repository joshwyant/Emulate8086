namespace Emulate8086.Meta.IntelMMX;

using Tags = IntelMMXInstructionTags;
using Set586 = Intel586.Intel586InstructionSet;
using Pattern = InstructionOpcodePattern;
using Flags = InstructionPatternFlags;
using Def = InstructionDefinition;

public class IntelMMXInstructionSet : InstructionSet
{
    public static IntelMMXInstructionSet Create()
    {
        return new IntelMMXInstructionSet(
            Def.GetAll(typeof(Def)));
    }
    protected IntelMMXInstructionSet(IEnumerable<Def> instructions)
        : base(instructions) { }

    #region Patterns

    #endregion

    #region Definitions
    #endregion
    public override InstructionTag[,] InstructionMatrix { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
}