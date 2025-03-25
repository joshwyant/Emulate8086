namespace Emulate8086.Meta.Intel686;

using System.Collections;
using static Intel686InstructionTagValue;
using Tag = InstructionTag<Intel686InstructionTagValue>;

public class Intel686InstructionTags : IEnumerable<InstructionTag>
{
    public static readonly Tag None = new(Intel686InstructionTagValue.None);


    public Intel686InstructionTags()
    {
        _tags = InstructionTag.GetAll(typeof(Intel686InstructionTagValue));
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

    public static Tag Cmovo { get; } = new(CMOVO);
    public static Tag Cmovno { get; } = new(CMOVNO);
    public static Tag Cmovb { get; } = new(CMOVB);
    public static Tag Cmovnb { get; } = new(CMOVNB);
    public static Tag Cmove { get; } = new(CMOVE);
    public static Tag Cmovne { get; } = new(CMOVNE);
    public static Tag Cmovbe { get; } = new(CMOVBE);
    public static Tag Cmovnbe { get; } = new(CMOVNBE);
    public static Tag Cmovs { get; } = new(CMOVS);
    public static Tag Cmovns { get; } = new(CMOVNS);
    public static Tag Cmovp { get; } = new(CMOVP);
    public static Tag Cmovnp { get; } = new(CMOVNP);
    public static Tag Cmovl { get; } = new(CMOVL);
    public static Tag Cmovnl { get; } = new(CMOVNL);
    public static Tag Cmovle { get; } = new(CMOVLE);
    public static Tag Cmovnle { get; } = new(CMOVNLE);

}
