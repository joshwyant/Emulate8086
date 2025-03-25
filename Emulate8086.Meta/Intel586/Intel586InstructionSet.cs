namespace Emulate8086.Meta.Intel586;

using Tags = Intel586InstructionTags;
using Set486 = Intel486.Intel486InstructionSet;
using Pattern = InstructionOpcodePattern;
using Flags = InstructionPatternFlags;
using Def = InstructionDefinition;

public class Intel586InstructionSet : Set486
{
    public static new Intel586InstructionSet Create()
    {
        return new Intel586InstructionSet(
            Def.GetAll(typeof(Def)));
    }
    protected Intel586InstructionSet(IEnumerable<Def> instructions)
        : base(instructions) { }

    #region Patterns

    #endregion

    #region Definitions
    #endregion
    public override InstructionTag[,] InstructionMatrix { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
}