namespace Emulate8086.Meta.Intel486;

using System.Collections;
using static Intel486InstructionTagValue;
using Tag = InstructionTag<Intel486InstructionTagValue>;

public class Intel486InstructionTags : IEnumerable<InstructionTag>
{
    public static readonly Tag None = new(Intel486InstructionTagValue.None);


    public Intel486InstructionTags()
    {
        _tags = InstructionTag.GetAll(typeof(Intel486InstructionTagValue));
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

    public static readonly Tag Bswap = new(BSWAP);
    public static readonly Tag CmpXchg = new(CMPXCHG);
    public static readonly Tag Xadd = new(XADD);
    public static readonly Tag Invlpg = new(INVLPG);
    public static readonly Tag Invd = new(INVD);
    public static readonly Tag Wbinvd = new(WBINVD);

}
