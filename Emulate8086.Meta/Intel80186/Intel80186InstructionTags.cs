namespace Emulate8086.Meta.Intel80186;

using System.Collections;
using static Intel80186InstructionTagValue;
using Tag = InstructionTag<Intel80186InstructionTagValue>;

public class Intel80186InstructionTags : IEnumerable<InstructionTag>
{
    public static readonly Tag None = new(Intel80186InstructionTagValue.None);


    public Intel80186InstructionTags()
    {
        _tags = InstructionTag.GetAll(typeof(Intel80186InstructionTagValue));
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

    public static readonly Tag Bound = new(BOUND);
    public static readonly Tag Enter = new(ENTER);
    public static readonly Tag Ins = new(INS);
    public static readonly Tag Leave = new(LEAVE);
    public static readonly Tag Outs = new(OUTS);
    public static readonly Tag Popa = new(POPA);
    public static readonly Tag Pusha = new(PUSHA);
    // public static readonly Tag Push = new(XXX);
    // public static readonly Tag Imul = new(XXX);
    // public static readonly Tag Shl = new(XXX);
    // public static readonly Tag Shr = new(XXX);
    // public static readonly Tag Sal = new(XXX);
    // public static readonly Tag Sar = new(XXX);
    // public static readonly Tag Rol = new(XXX);
    // public static readonly Tag Ror = new(XXX);
    // public static readonly Tag Rcl = new(XXX);
    // public static readonly Tag Rcr = new(XXX);



}
