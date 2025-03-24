using System.Collections;
using System.Diagnostics.Contracts;

namespace Emulate8086.Meta;

public abstract class InstructionSet : IEnumerable<InstructionDefinition>
{
    protected InstructionSet(IEnumerable<InstructionDefinition> instructions)
    {
        if (instructions is null)
        {
            throw new ArgumentNullException(nameof(instructions));
        }

        Instructions = [.. instructions];
    }

    protected InstructionSet(IEnumerable<InstructionDefinition> instructions, params InstructionSet[] baseInstructions)
    {
        if (instructions is null)
        {
            throw new ArgumentNullException(nameof(instructions));
        }

        Instructions = [.. baseInstructions.SelectMany(b => b.Instructions), .. instructions];
    }

    public abstract InstructionTag[,] InstructionMatrix { get; protected set; }

    public IEnumerable<InstructionDefinition> Instructions { get; }

    public IEnumerator<InstructionDefinition> GetEnumerator()
    {
        return Instructions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
