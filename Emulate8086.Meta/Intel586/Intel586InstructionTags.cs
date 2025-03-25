namespace Emulate8086.Meta.Intel586;

using System.Collections;
using static Intel586InstructionTagValue;
using Tag = InstructionTag<Intel586InstructionTagValue>;

public class Intel586InstructionTags : IEnumerable<InstructionTag>
{
    public static readonly Tag None = new(Intel586InstructionTagValue.None);


    public Intel586InstructionTags()
    {
        _tags = InstructionTag.GetAll(typeof(Intel586InstructionTagValue));
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

    public static Tag Rdmsr { get; } = new(RDMSR);
    public static Tag Wrmsr { get; } = new(WRMSR);
    public static Tag Rsm { get; } = new(RSM);
    public static Tag CpuId { get; } = new(CPUID);
    public static Tag CmpXchg8b { get; } = new(CMPXCHG8B);
    public static Tag RdTsc { get; } = new(RDTSC);

}
