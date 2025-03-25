namespace Emulate8086.Meta.Intel8087;

using Tags = Intel8087InstructionTags;
using Pattern = InstructionOpcodePattern;
using Flags = InstructionPatternFlags;
using Def = InstructionDefinition;
// Encoding:
// https://datasheets.chipdb.org/Intel/x86/808x/datashts/8087/205835-007.pdf
public class Intel8087InstructionSet : InstructionSet
{
    public static Intel8087InstructionSet Create()
    {
        return new Intel8087InstructionSet(
            Def.GetAll(typeof(Def)));
    }
    protected Intel8087InstructionSet(IEnumerable<Def> instructions)
        : base(instructions) { }

    public override InstructionTag[,] InstructionMatrix { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
}