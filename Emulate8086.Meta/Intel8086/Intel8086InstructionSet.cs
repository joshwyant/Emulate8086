namespace Emulate8086.Meta.Intel8086;

using Tags = Intel8086InstructionTags;
using Pattern = InstructionOpcodePattern;
using Flags = InstructionPatternFlags;
using Def = InstructionDefinition;

public class Intel8086InstructionSet : InstructionSet
{
    public static Intel8086InstructionSet Create()
    {
        return new Intel8086InstructionSet(
            Def.GetAll(typeof(Def)));
    }
    protected Intel8086InstructionSet(IEnumerable<Def> instructions)
        : base(instructions) { }

    public static Pattern Mov_RMToFromReg { get; }
        = Pattern.NewBuilder(0b111111_00, 0b100010_00)
            .WithFlags(Flags.D |
                       Flags.W |
                       Flags.ModRM |
                       Flags.ModRMReg).Build();

    public static Pattern Mov_ImmToRM { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1100011_0)
            .WithFlags(Flags.W |
                       Flags.ModRM |
                       Flags.ModRMOpcode |
                       Flags.Byte |
                       Flags.Word).Build();
    public static Pattern Mov_ImmToReg { get; }
        = Pattern.NewBuilder(0b1111_0000, 0b1011_0000)
            .WithFlags(Flags.W |
                    Flags.Reg).Build();
    public static Pattern Mov_MemToAccum { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1010000_0)
            .WithFlags(
                Flags.W | Flags.Addr
            ).Build();
    public static Pattern Mov_AccumToMem { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1010001_0)
            .WithFlags(
                Flags.W | Flags.Addr
            ).Build();
    public static Pattern Mov_RMToSeg { get; }
        = Pattern.NewBuilder(0b1000_1110)
            .WithFlags(Flags.ModRM | Flags.ModRMSeg).Build();
    public static Pattern Mov_SegToRM { get; }
        = Pattern.NewBuilder(0b1000_1100)
            .WithFlags(Flags.ModRM | Flags.ModRMSeg).Build();
    public static Def Mov { get; } = new(
        Tags.Mov,
        Mov_RMToFromReg,
        Mov_ImmToRM,
        Mov_ImmToReg,
        Mov_MemToAccum,
        Mov_AccumToMem,
        Mov_RMToSeg,
        Mov_SegToRM);

    public static Pattern Push_RM { get; }
        = Pattern.NewBuilder(0b1111_1111)
            .WithModRmOpcode(0b110).Build();

    public static Pattern Push_Reg { get; }
        = Pattern.NewBuilder(0b11111_000, 0b01010_000).Build();

    public static Pattern Push_Seg { get; }
        = Pattern.NewBuilder(0b111_00_111, 0b111_00_110).Build();

    public static Def Push { get; } = new(
        Tags.Push,
        Push_RM,
        Push_Reg,
        Push_Seg);

    public static Pattern Pop_RM { get; }
        = Pattern.NewBuilder(0b1000_1111)
            .WithModRmOpcode(0b000).Build();
    public static Pattern Pop_Reg { get; }
        = Pattern.NewBuilder(0b11111_000, 0b01011_000).Build();
    public static Pattern Pop_Seg { get; }
        = Pattern.NewBuilder(0b111_00_111, 0b000_00_111).Build();

    public static Def Pop { get; } = new(
        Tags.Pop,
        Pop_RM,
        Pop_Reg,
        Pop_Seg);

    public static Pattern Xchg_RMReg { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1000011_0)
            .WithFlags(Flags.W | Flags.ModRM | Flags.ModRMReg).Build();

    public static Pattern Xchg_RegAccum { get; }
        = Pattern.NewBuilder(0b11111_000, 0b10010_000)
            .WithFlags(Flags.Reg).Build();

    public static Def Xchg { get; } = new(
        Tags.Xchg,
        Xchg_RMReg,
        Xchg_RegAccum
    );

    public static Pattern In_ALAXFromFixedPort { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1110010_0)
            .WithFlags(Flags.W | Flags.Word).Build();

    public static Pattern In_VariablePortDx { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1110110_0)
            .WithFlags(Flags.W).Build();

    public static Def In { get; } = new(
        Tags.In,
        In_ALAXFromFixedPort,
        In_VariablePortDx
    );

    public static Pattern Out_ALAXToFixedPort { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1110011_0)
            .WithFlags(Flags.W | Flags.Word).Build();

    public static Pattern Out_VariablePortDx { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1110110_0)
            .WithFlags(Flags.W).Build();

    public static Def Out { get; } = new(
        Tags.Out,
        Out_ALAXToFixedPort,
        Out_VariablePortDx
    );

    public static Pattern Xlat_ToAL { get; }
        = Pattern.NewBuilder(0b11010111).Build();

    public static Def Xlat { get; } = new(
        Tags.Xlat,
        Xlat_ToAL
    );
    public static Pattern LEA { get; }
        = Pattern.NewBuilder(0b1000_1101).Build();
    public static Def Lea { get; } = new(Tags.Lea, LEA);
    public static Pattern LDS { get; }
        = Pattern.NewBuilder(0b1100_0101).Build();
    public static Def Lds { get; } = new(Tags.Lds, LDS);
    public static Pattern LES { get; }
        = Pattern.NewBuilder(0b1100_0100).Build();
    public static Def Les { get; } = new(Tags.Les, LES);
    public static Pattern LAHF { get; }
        = Pattern.NewBuilder(0b1001_1111).Build();
    public static Def Lahf { get; } = new(Tags.Lahf, LAHF);
    public static Pattern SAHF { get; }
        = Pattern.NewBuilder(0b1001_1110).Build();
    public static Def Sahf { get; } = new(Tags.Sahf, SAHF);
    public static Pattern PUSHF { get; }
        = Pattern.NewBuilder(0b1001_1100).Build();
    public static Def Pushf { get; } = new(Tags.Pushf, PUSHF);
    public static Pattern POPF { get; }
        = Pattern.NewBuilder(0b1001_1101).Build();
    public static Def Popf { get; } = new(Tags.Popf, POPF);
    public static Pattern ADD_RMRegToEither { get; }
        = Pattern.NewBuilder(0b111111_00, 0b000000_00).Build();
    public static Pattern ADD_ImmToRM { get; }
        = Pattern.NewBuilder(0b111111_00, 0b100000_00)
            .WithModRmOpcode(0b000).Build();
    public static Pattern ADD_ImmToAccum { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b0000010_0).Build();
    public static Def Add { get; } = new(
        Tags.Add,
        ADD_RMRegToEither,
        ADD_ImmToRM,
        ADD_ImmToAccum);
    public static Pattern ADC_RMRegToEither { get; }
        = Pattern.NewBuilder(0b111111_00, 0b000100_00).Build();
    public static Pattern ADC_ImmToRM { get; }
        = Pattern.NewBuilder(0b111111_00, 0b100000_00)
            .WithModRmOpcode(0b010).Build();
    public static Pattern ADC_ImmToAccum { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b0001010_0).Build();
    public static Def Adc { get; } = new(
        Tags.Adc,
        ADC_RMRegToEither,
        ADC_ImmToRM,
        ADC_ImmToAccum);
    public static Pattern INC_RM { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1111111_0)
            .WithModRmOpcode(0b000).Build();
    public static Pattern INC_Reg { get; }
        = Pattern.NewBuilder(0b11111_000, 0b01000_000)
            .WithFlags(Flags.Reg).Build();
    public static Def Inc { get; } = new(
        Tags.Inc,
        INC_RM,
        INC_Reg);
    public static Pattern AAA { get; }
        = Pattern.NewBuilder(0b00110111).Build();
    public static Def Aaa { get; } = new(Tags.Aaa, AAA);
    public static Pattern DAA { get; }
        = Pattern.NewBuilder(0b00100111).Build();
    public static Def Daa { get; } = new(Tags.Daa, DAA);
    public static Pattern SUB_RMRegToEither { get; }
        = Pattern.NewBuilder(0b111111_00, 0b001010_00).Build();
    public static Pattern SUB_ImmFromRM { get; }
        = Pattern.NewBuilder(0b111111_00, 0b100000_00)
            .WithModRmOpcode(0b101).Build();
    public static Pattern SUB_ImmFromAccum { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b0010110_0).Build();
    public static Def Sub { get; } = new(
        Tags.Sub,
        SUB_RMRegToEither,
        SUB_ImmFromRM,
        SUB_ImmFromAccum);
    public static Pattern SBB_RMRegToEither { get; }
        = Pattern.NewBuilder(0b111111_00, 0b000110_00).Build();
    public static Pattern SBB_ImmFromRM { get; }
        = Pattern.NewBuilder(0b111111_00, 0b100000_00)
            .WithModRmOpcode(0b011).Build();
    public static Pattern SBB_ImmFromAccum { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b0001110_0).Build();
    public static Def Sbb { get; } = new(
        Tags.Sbb,
        SBB_RMRegToEither,
        SBB_ImmFromRM,
        SBB_ImmFromAccum);
    public static Pattern DEC_RM { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1111111_0)
            .WithModRmOpcode(0b001).Build();
    public static Pattern DEC_Reg { get; }
        = Pattern.NewBuilder(0b11111_000, 0b01001_000).Build();
    public static Def Dec { get; } = new(
        Tags.Dec,
        DEC_RM,
        DEC_Reg);
    public static Pattern NEG { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1111011_0).Build();
    public static Def Neg { get; } = new(Tags.Neg, NEG);
    public static Pattern CMP_RMReg { get; }
        = Pattern.NewBuilder(0b111111_00, 0b001110_00).Build();
    public static Pattern CMP_ImmFromRM { get; }
        = Pattern.NewBuilder(0b111111_00, 0b100000_00)
            .WithModRmOpcode(0b111).Build();
    public static Pattern CMP_ImmFromAccum { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b0011110_0).Build();
    public static Def Cmp { get; } = new(
        Tags.Cmp,
        CMP_RMReg,
        CMP_ImmFromRM,
        CMP_ImmFromAccum);
    public static Pattern AAS { get; }
        = Pattern.NewBuilder(0b0011_1111).Build();
    public static Def Aas { get; } = new(Tags.Aas, AAS);
    public static Pattern DAS { get; }
        = Pattern.NewBuilder(0b0010_1111).Build();
    public static Def Das { get; } = new(Tags.Das, DAS);
    public static Pattern MUL { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1111011_0)
            .WithModRmOpcode(0b100).Build();
    public static Def Mul { get; } = new(Tags.Mul, MUL);
    public static Pattern IMUL { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1111011_0)
            .WithModRmOpcode(0b101).Build();
    public static Def Imul { get; } = new(Tags.Imul, IMUL);
    public static Pattern AAM { get; }
        = Pattern.NewBuilder(0b11010100).Build(); // TODO: Byte 0x0A follows this
    public static Def Aam { get; } = new(Tags.Aam, AAM);
    public static Pattern DIV { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1111011_0)
            .WithModRmOpcode(0b110).Build();
    public static Def Div { get; } = new(Tags.Div, DIV);
    public static Pattern IDIV { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1111011_0)
            .WithModRmOpcode(0b111).Build();
    public static Def Idiv { get; } = new(Tags.Idiv, IDIV);
    public static Pattern AAD { get; }
        = Pattern.NewBuilder(0b1101_0101).Build(); // TODO: Byte 0x0A follows this
    public static Def Aad { get; } = new(Tags.Aad, AAD);
    public static Pattern CBW { get; }
        = Pattern.NewBuilder(0b1001_1000).Build();
    public static Def Cbw { get; } = new(Tags.Cbw, CBW);
    public static Pattern CWD { get; }
        = Pattern.NewBuilder(0b1001_1001).Build();
    public static Def Cwd { get; } = new(Tags.Cwd, CWD);
    public static Pattern NOT { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1111011_0)
            .WithModRmOpcode(0b010).Build();
    public static Def Not { get; } = new(Tags.Not, NOT);
    public static Pattern SHL { get; }
        = Pattern.NewBuilder(0b111111_00, 0b110100_00)
            .WithModRmOpcode(0b100).Build();
    public static Def Shl { get; } = new(Tags.Shl, SHL);
    public static Pattern SHR { get; }
        = Pattern.NewBuilder(0b111111_00, 0b110100_00)
            .WithModRmOpcode(0b101).Build();
    public static Def Shr { get; } = new(Tags.Shr, SHR);
    public static Pattern SAR { get; }
        = Pattern.NewBuilder(0b111111_00, 0b110100_00)
            .WithModRmOpcode(0b111).Build();
    public static Def Sar { get; } = new(Tags.Sar, SAR);
    public static Pattern ROL { get; }
        = Pattern.NewBuilder(0b111111_00, 0b110100_00)
            .WithModRmOpcode(0b000).Build();
    public static Def Rol { get; } = new(Tags.Rol, ROL);
    public static Pattern ROR { get; }
        = Pattern.NewBuilder(0b111111_00, 0b110100_00)
            .WithModRmOpcode(0b001).Build();
    public static Def Ror { get; } = new(Tags.Ror, ROR);
    public static Pattern RCL { get; }
        = Pattern.NewBuilder(0b111111_00, 0b110100_00)
            .WithModRmOpcode(0b010).Build();
    public static Def Rcl { get; } = new(Tags.Rcl, RCL);
    public static Pattern RCR { get; }
        = Pattern.NewBuilder(0b111111_00, 0b110100_00)
            .WithModRmOpcode(0b011).Build();
    public static Def Rcr { get; } = new(Tags.Rcr, RCR);

    public static Pattern AND_RMRegToEither { get; }
        = Pattern.NewBuilder(0b111111_00, 0b001000_00)
            .WithFlags(Flags.ModRM | Flags.ModRMReg).Build();

    public static Pattern AND_ImmToRM { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1000000_0)
            .WithModRmOpcode(0b100).Build();

    public static Pattern AND_ImmToAccum { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b0010010_0).Build();

    public static Def And { get; } = new(
        Tags.And,
        AND_RMRegToEither,
        AND_ImmToRM,
        AND_ImmToAccum);
    public static Pattern TEST_RMReg { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b_1000010_0)
            .WithFlags(Flags.ModRM | Flags.ModRMReg).Build();

    public static Pattern TEST_ImmAndRM { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1111011_0)
            .WithModRmOpcode(0b000).Build();

    public static Pattern TEST_ImmAndAccum { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1010100_0).Build();
    public static Def Test { get; } = new(
        Tags.Test,
        TEST_RMReg,
        TEST_ImmAndRM,
        TEST_ImmAndAccum);
    public static Pattern OR_RMRegToEither { get; }
        = Pattern.NewBuilder(0b111111_00, 0b000010_00)
            .WithFlags(Flags.ModRM | Flags.ModRMReg).Build();

    public static Pattern OR_ImmToRM { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1000000_0)
            .WithModRmOpcode(0b001).Build();

    public static Pattern OR_ImmToAccum { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b0000110_0).Build();
    public static Def Or { get; } = new(
        Tags.Or,
        OR_RMRegToEither,
        OR_ImmToRM,
        OR_ImmToAccum);
    public static Pattern XOR_RMRegToEither { get; }
        = Pattern.NewBuilder(0b111111_00, 0b001100_00)
            .WithFlags(Flags.ModRM | Flags.ModRMReg).Build();

    public static Pattern XOR_ImmToRM { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b1000000_0)
            .WithModRmOpcode(0b110).Build();

    public static Pattern XOR_ImmToAccum { get; }
        = Pattern.NewBuilder(0b1111111_0, 0b0011010_0).Build();
    public static Def Xor { get; } = new(
        Tags.Xor,
        XOR_RMRegToEither,
        XOR_ImmToRM,
        XOR_ImmToAccum);
    public static Pattern REP { get; }
        = Pattern.NewBuilder().Build();
    public static Def Rep { get; } = new(Tags.Rep, REP);
    public static Pattern MOVS { get; }
        = Pattern.NewBuilder().Build();
    public static Def Movs { get; } = new(Tags.Movs, MOVS);
    public static Pattern CMPS { get; }
        = Pattern.NewBuilder().Build();
    public static Def Cmps { get; } = new(Tags.Cmps, CMPS);
    public static Pattern SCAS { get; }
        = Pattern.NewBuilder().Build();
    public static Def Scas { get; } = new(Tags.Scas, SCAS);
    public static Pattern LODS { get; }
        = Pattern.NewBuilder().Build();
    public static Def Lods { get; } = new(Tags.Lods, LODS);
    public static Pattern STOS { get; }
        = Pattern.NewBuilder().Build();
    public static Def Stos { get; } = new(Tags.Stos, STOS);
    public static Pattern CALL { get; }
        = Pattern.NewBuilder().Build();
    public static Def Call { get; } = new(Tags.Call, CALL);
    public static Pattern JMP { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jmp { get; } = new(Tags.Jmp, JMP);
    public static Pattern RET { get; }
        = Pattern.NewBuilder().Build();
    public static Def Ret { get; } = new(Tags.Ret, RET);
    public static Pattern JE { get; }
        = Pattern.NewBuilder().Build();
    public static Def Je { get; } = new(Tags.Je, JE);
    public static Pattern JL { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jl { get; } = new(Tags.Jl, JL);
    public static Pattern JLE { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jle { get; } = new(Tags.Jle, JLE);
    public static Pattern JB { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jb { get; } = new(Tags.Jb, JB);
    public static Pattern JBE { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jbe { get; } = new(Tags.Jbe, JBE);
    public static Pattern JP { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jp { get; } = new(Tags.Jp, JP);
    public static Pattern JO { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jo { get; } = new(Tags.Jo, JO);
    public static Pattern JS { get; }
        = Pattern.NewBuilder().Build();
    public static Def Js { get; } = new(Tags.Js, JS);
    public static Pattern JNE { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jne { get; } = new(Tags.Jne, JNE);
    public static Pattern JNL { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jnl { get; } = new(Tags.Jnl, JNL);
    public static Pattern JNLE { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jnle { get; } = new(Tags.Jnle, JNLE);
    public static Pattern JNB { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jnb { get; } = new(Tags.Jnb, JNB);
    public static Pattern JNBE { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jnbe { get; } = new(Tags.Jnbe, JNBE);
    public static Pattern JNP { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jnp { get; } = new(Tags.Jnp, JNP);
    public static Pattern JNO { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jno { get; } = new(Tags.Jno, JNO);
    public static Pattern JNS { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jns { get; } = new(Tags.Jns, JNS);
    public static Pattern LOOP { get; }
        = Pattern.NewBuilder().Build();
    public static Def Loop { get; } = new(Tags.Loop, LOOP);
    public static Pattern LOOPZ { get; }
        = Pattern.NewBuilder().Build();
    public static Def Loopz { get; } = new(Tags.Loopz, LOOPZ);
    public static Pattern LOOPNZ { get; }
        = Pattern.NewBuilder().Build();
    public static Def Loopnz { get; } = new(Tags.Loopnz, LOOPNZ);
    public static Pattern JCXZ { get; }
        = Pattern.NewBuilder().Build();
    public static Def Jcxz { get; } = new(Tags.Jcxz, JCXZ);
    public static Pattern INT { get; }
        = Pattern.NewBuilder().Build();
    public static Def Int { get; } = new(Tags.Int, INT);
    public static Pattern INTO { get; }
        = Pattern.NewBuilder().Build();
    public static Def Into { get; } = new(Tags.Into, INTO);
    public static Pattern IRET { get; }
        = Pattern.NewBuilder().Build();
    public static Def Iret { get; } = new(Tags.Iret, IRET);
    public static Pattern CLC { get; }
        = Pattern.NewBuilder().Build();
    public static Def Clc { get; } = new(Tags.Clc, CLC);
    public static Pattern CMC { get; }
        = Pattern.NewBuilder().Build();
    public static Def Cmc { get; } = new(Tags.Cmc, CMC);
    public static Pattern CLD { get; }
        = Pattern.NewBuilder().Build();
    public static Def Cld { get; } = new(Tags.Cld, CLD);
    public static Pattern CLI { get; }
        = Pattern.NewBuilder().Build();
    public static Def Cli { get; } = new(Tags.Cli, CLI);
    public static Pattern HLT { get; }
        = Pattern.NewBuilder().Build();
    public static Def Hlt { get; } = new(Tags.Hlt, HLT);
    public static Pattern LOCK { get; }
        = Pattern.NewBuilder().Build();
    public static Def Lock { get; } = new(Tags.Lock, LOCK);
    public static Pattern STC { get; }
        = Pattern.NewBuilder().Build();
    public static Def Stc { get; } = new(Tags.Stc, STC);
    public static Pattern NOP { get; }
        = Pattern.NewBuilder().Build();
    public static Def Nop { get; } = new(Tags.Nop, NOP);
    public static Pattern STD { get; }
        = Pattern.NewBuilder().Build();
    public static Def Std { get; } = new(Tags.Std, STD);
    public static Pattern STI { get; }
        = Pattern.NewBuilder().Build();
    public static Def Sti { get; } = new(Tags.Sti, STI);
    public static Pattern WAIT { get; }
        = Pattern.NewBuilder().Build();
    public static Def Wait { get; } = new(Tags.Wait, WAIT);
    public static Pattern ESC { get; }
        = Pattern.NewBuilder().Build();
    public static Def Esc { get; } = new(Tags.Esc, ESC);
    public override InstructionTag[,] InstructionMatrix { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
}