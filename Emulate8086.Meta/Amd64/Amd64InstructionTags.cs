namespace Emulate8086.Meta.Amd64;

using System.Collections;
using static Amd64InstructionTagValue;
using Tag = InstructionTag<Amd64InstructionTagValue>;

public class Amd64InstructionTags : IEnumerable<InstructionTag>
{
    public static readonly Tag None = new(Amd64InstructionTagValue.None);


    public Amd64InstructionTags()
    {
        _tags = InstructionTag.GetAll(typeof(Amd64InstructionTagValue));
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

    public static Tag RexPrefix { get; }
        = new(Amd64InstructionTagValue.RexPrefix);
    public static Tag Syscall { get; } = new(SYSCALL);
    public static Tag Sysret { get; } = new(SYSRET);
    public static Tag Sysenter { get; } = new(SYSENTER);
    public static Tag Sysexit { get; } = new(SYSEXIT);
    public static Tag Cdqe { get; } = new(CDQE);
    public static Tag Cqo { get; } = new(CQO);
    public static Tag Cmpsq { get; } = new(CMPSQ);
    public static Tag Cmpxchg16b { get; } = new(CMPXCHG16B);
    public static Tag Iretq { get; } = new(IRETQ);
    public static Tag Jrcxz { get; } = new(JRCXZ);
    public static Tag Lodsq { get; } = new(LODSQ);
    public static Tag Movsxd { get; } = new(MOVSXD);
    public static Tag Movsq { get; } = new(MOVSQ);
    public static Tag Popfq { get; } = new(POPFQ);
    public static Tag Pushfq { get; } = new(PUSHFQ);
    public static Tag Scasq { get; } = new(SCASQ);
    public static Tag Stosq { get; } = new(STOSQ);
    public static Tag Swapgs { get; } = new(SWAPGS);


}
