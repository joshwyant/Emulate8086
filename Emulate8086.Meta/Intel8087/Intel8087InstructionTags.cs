namespace Emulate8086.Meta.Intel8087;

using System.Collections;
using static Intel8087InstructionTagValue;
using Tag = InstructionTag<Intel8087InstructionTagValue>;

public class Intel8087InstructionTags : IEnumerable<InstructionTag>
{
    public static readonly Tag None = new(Intel8087InstructionTagValue.None);


    public Intel8087InstructionTags()
    {
        _tags = InstructionTag.GetAll(typeof(Intel8087InstructionTagValue));
    }

    private readonly InstructionTag[] _tags;

    public IEnumerator<InstructionTag> GetEnumerator()
    {
        return _tags.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
