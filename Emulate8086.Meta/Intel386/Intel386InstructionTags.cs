namespace Emulate8086.Meta.Intel386;

using System.Collections;
using static Intel386InstructionTagValue;
using Tag = InstructionTag<Intel386InstructionTagValue>;
using Tags186 = Intel80186.Intel80186InstructionTags;
using Tag186 = InstructionTag<Intel80186.Intel80186InstructionTagValue>;

public class Intel386InstructionTags : IEnumerable<InstructionTag>
{
    public static readonly Tag None = new(Intel386InstructionTagValue.None);


    public Intel386InstructionTags()
    {
        _tags = InstructionTag.GetAll(typeof(Intel386InstructionTagValue));
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

    // Inherited from 186 but not 286
    public static readonly Tag186 Bound = Tags186.Bound;
    public static readonly Tag186 Enter = Tags186.Enter;
    public static readonly Tag186 Ins = Tags186.Ins;
    public static readonly Tag186 Leave = Tags186.Leave;
    public static readonly Tag186 Outs = Tags186.Outs;
    public static readonly Tag186 Popa = Tags186.Popa;
    public static readonly Tag186 Pusha = Tags186.Pusha;

    public static readonly Tag Bt = new(BT);
    public static readonly Tag Bts = new(BTS);
    public static readonly Tag Btr = new(BTR);
    public static readonly Tag Btc = new(BTC);
    public static readonly Tag Bsf = new(BSF);
    public static readonly Tag Bsr = new(BSR);
    public static readonly Tag Shld = new(SHLD);
    public static readonly Tag Shrd = new(SHRD);
    public static readonly Tag Movzx = new(MOVZX);
    public static readonly Tag Movsx = new(MOVSX);
    public static readonly Tag Seto = new(SETO);
    public static readonly Tag Setno = new(SETNO);
    public static readonly Tag Setb = new(SETB);
    public static readonly Tag Setnb = new(SETNB);
    public static readonly Tag Sete = new(SETE);
    public static readonly Tag Setne = new(SETNE);
    public static readonly Tag Setbe = new(SETBE);
    public static readonly Tag Setnbe = new(SETNBE);
    public static readonly Tag Sets = new(SETS);
    public static readonly Tag Setns = new(SETNS);
    public static readonly Tag Setp = new(SETP);
    public static readonly Tag Setnp = new(SETNP);
    public static readonly Tag Setl = new(SETL);
    public static readonly Tag Setnl = new(SETNL);
    public static readonly Tag Setle = new(SETLE);
    public static readonly Tag Setnle = new(SETNLE);
    public static readonly Tag FsPrefix = new(FSPrefix);
    public static readonly Tag GsPrefix = new(GSPrefix);
    public static readonly Tag Lfs = new(LFS);
    public static readonly Tag Lgs = new(LGS);
    public static readonly Tag Lss = new(LSS);
    public static readonly Tag Int1 = new(INT1);
    public static readonly Tag Umov = new(UMOV);
    public static readonly Tag Xbts = new(XBTS);
    public static readonly Tag Ibts = new(IBTS);
    public static readonly Tag Loadall386 = new(LOADALL386);

}
