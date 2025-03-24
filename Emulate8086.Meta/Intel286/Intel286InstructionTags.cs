namespace Emulate8086.Meta.Intel286;

using System.Collections;
using static Intel286InstructionTagValue;
using Tag = InstructionTag<Intel286InstructionTagValue>;

public class Intel286InstructionTags : IEnumerable<InstructionTag>
{
    public static readonly Tag None = new(Intel286InstructionTagValue.None);


    public Intel286InstructionTags()
    {
        _tags = InstructionTag.GetAll(typeof(Intel286InstructionTagValue));
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

    public static readonly Tag Lgdt = new(LGDT);
    public static readonly Tag Lidt = new(LIDT);
    public static readonly Tag Lmsw = new(LMSW);
    public static readonly Tag Clts = new(CLTS);
    public static readonly Tag Lldt = new(LLDT);
    public static readonly Tag Ltr = new(LTR);
    public static readonly Tag Sgdt = new(SGDT);
    public static readonly Tag Sidt = new(SIDT);
    public static readonly Tag Smsw = new(SMSW);
    public static readonly Tag Sldt = new(SLDT);
    public static readonly Tag Str = new(STR);
    public static readonly Tag Arpl = new(ARPL);
    public static readonly Tag Lar = new(LAR);
    public static readonly Tag Lsl = new(LSL);
    public static readonly Tag Verr = new(VERR);
    public static readonly Tag Verw = new(VERW);
    public static readonly Tag Loadall = new(LOADALL);
    public static readonly Tag Storeall = new(STOREALL);
}
