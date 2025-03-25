namespace Emulate8086.Meta.IntelMMX;

using System.Collections;
using static IntelMMXInstructionTagValue;
using Tag = InstructionTag<IntelMMXInstructionTagValue>;

public class IntelMMXInstructionTags : IEnumerable<InstructionTag>
{
    public static readonly Tag None = new(IntelMMXInstructionTagValue.None);


    public IntelMMXInstructionTags()
    {
        _tags = InstructionTag.GetAll(typeof(IntelMMXInstructionTags));
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
