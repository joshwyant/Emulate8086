namespace Emulate8086.Meta.Intel8086;

using System.Collections;
using static Intel8086InstructionTagValue;
using Tag = InstructionTag<Intel8086InstructionTagValue>;

public class Intel8086InstructionTags : IEnumerable<InstructionTag>
{
    public static readonly Tag None = new(Intel8086InstructionTagValue.None);

    public static readonly Tag Mov = new(MOV);
    public static readonly Tag Push = new(PUSH);
    public static readonly Tag Pop = new(POP);
    public static readonly Tag Xchg = new(XCHG);
    public static readonly Tag In = new(IN);
    public static readonly Tag Out = new(OUT);
    public static readonly Tag Xlat = new(XLAT);
    public static readonly Tag Lea = new(LEA);
    public static readonly Tag Lds = new(LDS);
    public static readonly Tag Les = new(LES);
    public static readonly Tag Lahf = new(LAHF);
    public static readonly Tag Sahf = new(SAHF);
    public static readonly Tag Pushf = new(PUSHF);
    public static readonly Tag Popf = new(POPF);
    public static readonly Tag Add = new(ADD);
    public static readonly Tag Adc = new(ADC);
    public static readonly Tag Inc = new(INC);
    public static readonly Tag Aaa = new(AAA);
    public static readonly Tag Daa = new(DAA);
    public static readonly Tag Sub = new(SUB);
    public static readonly Tag Sbb = new(SBB);
    public static readonly Tag Dec = new(DEC);
    public static readonly Tag Neg = new(NEG);
    public static readonly Tag Cmp = new(CMP);
    public static readonly Tag Aas = new(AAS);
    public static readonly Tag Das = new(DAS);
    public static readonly Tag Mul = new(MUL);
    public static readonly Tag Imul = new(IMUL);
    public static readonly Tag Aam = new(AAM);
    public static readonly Tag Div = new(DIV);
    public static readonly Tag Idiv = new(IDIV);
    public static readonly Tag Aad = new(AAD);
    public static readonly Tag Cbw = new(CBW);
    public static readonly Tag Cwd = new(CWD);
    public static readonly Tag Not = new(NOT);
    public static readonly Tag Shl = new(SHL);
    public static readonly Tag Shr = new(SHR);
    public static readonly Tag Sar = new(SAR);
    public static readonly Tag Rol = new(ROL);
    public static readonly Tag Ror = new(ROR);
    public static readonly Tag Rcl = new(RCL);
    public static readonly Tag Rcr = new(RCR);
    public static readonly Tag And = new(AND);
    public static readonly Tag Test = new(TEST);
    public static readonly Tag Or = new(OR);
    public static readonly Tag Xor = new(XOR);
    public static readonly Tag Rep = new(REP);
    public static readonly Tag Movs = new(MOVS);
    public static readonly Tag Cmps = new(CMPS);
    public static readonly Tag Scas = new(SCAS);
    public static readonly Tag Lods = new(LODS);
    public static readonly Tag Stos = new(STOS);
    public static readonly Tag Call = new(CALL);
    public static readonly Tag Jmp = new(JMP);
    public static readonly Tag Ret = new(RET);
    public static readonly Tag Je = new(JE);
    public static readonly Tag Jl = new(JL);
    public static readonly Tag Jle = new(JLE);
    public static readonly Tag Jb = new(JB);
    public static readonly Tag Jbe = new(JBE);
    public static readonly Tag Jp = new(JP);
    public static readonly Tag Jo = new(JO);
    public static readonly Tag Js = new(JS);
    public static readonly Tag Jne = new(JNE);
    public static readonly Tag Jnl = new(JNL);
    public static readonly Tag Jnle = new(JNLE);
    public static readonly Tag Jnb = new(JNB);
    public static readonly Tag Jnbe = new(JNBE);
    public static readonly Tag Jnp = new(JNP);
    public static readonly Tag Jno = new(JNO);
    public static readonly Tag Jns = new(JNS);
    public static readonly Tag Loop = new(LOOP);
    public static readonly Tag Loopz = new(LOOPZ);
    public static readonly Tag Loopnz = new(LOOPNZ);
    public static readonly Tag Jcxz = new(JCXZ);
    public static readonly Tag Int = new(INT);
    public static readonly Tag Into = new(INTO);
    public static readonly Tag Iret = new(IRET);
    public static readonly Tag Clc = new(CLC);
    public static readonly Tag Cmc = new(CMC);
    public static readonly Tag Cld = new(CLD);
    public static readonly Tag Cli = new(CLI);
    public static readonly Tag Stc = new(STC);
    public static readonly Tag Sti = new(STI);
    public static readonly Tag Hlt = new(HLT);
    public static readonly Tag Lock = new(LOCK);
    public static readonly Tag Nop = new(NOP);
    public static readonly Tag Std = new(STD);
    public static readonly Tag Wait = new(WAIT);
    public static readonly Tag Esc = new(ESC);
    public Intel8086InstructionTags()
    {
        _tags = InstructionTag.GetAll(typeof(Intel8086InstructionTagValue));
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
