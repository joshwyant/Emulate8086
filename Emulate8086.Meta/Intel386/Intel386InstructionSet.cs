namespace Emulate8086.Meta.Intel386;

using Tags = Intel386InstructionTags;
using Tags186 = Intel80186.Intel80186InstructionTags;
using Tags86 = Intel8086.Intel8086InstructionTags;
using Set86 = Intel8086.Intel8086InstructionSet;
using Set186 = Intel80186.Intel80186InstructionSet;
using Pattern = InstructionOpcodePattern;
using Flags = InstructionPatternFlags;
using Def = InstructionDefinition;

public class Intel386InstructionSet : Intel286.Intel286InstructionSet
{
    public static new Intel386InstructionSet Create()
    {
        return new Intel386InstructionSet(
            Def.GetAll(typeof(Def)));
    }
    protected Intel386InstructionSet(IEnumerable<Def> instructions)
        : base(instructions) { }


    #region 186 merge
    // 386 has some 186 instructions that
    // the 286 doesn't have to inherit from:
    //  Patterns:
    public static Pattern BOUND { get; } = Set186.BOUND;
    public static Pattern ENTER { get; } = Set186.ENTER;
    public static Pattern INS { get; } = Set186.INS;
    public static Pattern LEAVE { get; } = Set186.LEAVE;
    public static Pattern OUTS { get; } = Set186.OUTS;
    public static Pattern POPA { get; } = Set186.POPA;
    public static Pattern PUSHA { get; } = Set186.PUSHA;
    public static Pattern PUSH_ImmB { get; } = Set186.PUSH_ImmB;
    public static Pattern PUSH_ImmW { get; } = Set186.PUSH_ImmW;
    public static Pattern IMUL_ImmB { get; } = Set186.IMUL_ImmB;
    public static Pattern IMUL_ImmW { get; } = Set186.IMUL_ImmW;
    public static Pattern SHL_ImmB { get; } = Set186.SHL_ImmB;
    public static Pattern SHL_ImmW { get; } = Set186.SHL_ImmW;
    public static Pattern SHR_ImmB { get; } = Set186.SHR_ImmB;
    public static Pattern SHR_ImmW { get; } = Set186.SHR_ImmW;
    public static Pattern SAR_ImmB { get; } = Set186.SAR_ImmB;
    public static Pattern SAR_ImmW { get; } = Set186.SAR_ImmW;
    public static Pattern ROL_ImmB { get; } = Set186.ROL_ImmB;
    public static Pattern ROL_ImmW { get; } = Set186.ROL_ImmW;
    public static Pattern ROR_ImmB { get; } = Set186.ROR_ImmB;
    public static Pattern ROR_ImmW { get; } = Set186.ROR_ImmW;
    public static Pattern RCL_ImmB { get; } = Set186.RCL_ImmB;
    public static Pattern RCL_ImmW { get; } = Set186.RCL_ImmW;
    public static Pattern RCR_ImmB { get; } = Set186.RCR_ImmB;
    public static Pattern RCR_ImmW { get; } = Set186.RCR_ImmW;
    // Definitions:
    public static Def Bound { get; } = Set186.Bound;
    public static Def Enter { get; } = Set186.Enter;
    public static Def Ins { get; } = Set186.Ins;
    public static Def Leave { get; } = Set186.Leave;
    public static Def Outs { get; } = Set186.Outs;
    public static Def Popa { get; } = Set186.Popa;
    public static Def Pusha { get; } = Set186.Pusha;
    #endregion

    #region Patterns
    public static Pattern OperandSizeOverridePrefixPattern { get; }
        = Pattern.NewBuilder(0x66)
            .WithFlags(Flags.Pfix).Build();
    public static Pattern AddressSizeOverridePrefixPattern { get; }
        = Pattern.NewBuilder(0x66)
            .WithFlags(Flags.Pfix).Build();

    public static Pattern BT_RMR { get; }
        = Pattern.NewBuilder(0xA3)
            .WithPrefixGroup(0x0F)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMReg
            ).Build();
    public static Pattern BT_RMImm8 { get; }
        = Pattern.NewBuilder(0xBA)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(4)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte
            ).Build();

    public static Pattern BTS_RMR { get; }
        = Pattern.NewBuilder(0xAB)
            .WithPrefixGroup(0x0F)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMReg
            ).Build();
    public static Pattern BTS_RMImm8 { get; }
        = Pattern.NewBuilder(0xBA)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(5)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte
            ).Build();

    public static Pattern BTR_RMR { get; }
        = Pattern.NewBuilder(0xB3)
            .WithPrefixGroup(0x0F)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMReg
            ).Build();
    public static Pattern BTR_RMImm8 { get; }
        = Pattern.NewBuilder(0xBA)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(6)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte
            ).Build();

    public static Pattern BTC_RMR { get; }
        = Pattern.NewBuilder(0xBB)
            .WithPrefixGroup(0x0F)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMReg
            ).Build();
    public static Pattern BTC_RMImm8 { get; }
        = Pattern.NewBuilder(0xBA)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(7)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte
            ).Build();

    public static Pattern BSF { get; }
        = Pattern.NewBuilder(0xBC)
            // With optional first prefix NFx
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();

    public static Pattern BSR { get; }
        = Pattern.NewBuilder(0xBD)
            // With optional first prefix NFx
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();

    public static Pattern SHLD_Imm { get; }
        = Pattern.NewBuilder(0xA4)
            .WithPrefixGroup(0x0F)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMReg |
                Flags.Byte)
            .Build();
    public static Pattern SHLD_Cl { get; }
        = Pattern.NewBuilder(0xA5)
            .WithPrefixGroup(0x0F)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMReg)
            .Build();

    public static Pattern SHRD_Imm { get; }
        = Pattern.NewBuilder(0xAC)
            .WithPrefixGroup(0x0F)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMReg |
                Flags.Byte)
            .Build();
    public static Pattern SHRD_Cl { get; }
        = Pattern.NewBuilder(0xAD)
            .WithPrefixGroup(0x0F)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMReg)
            .Build();

    public static Pattern MOVZX_RegRM8 { get; }
        = Pattern.NewBuilder(0xB6)
            .WithPrefixGroup(0x0F)
            .WithFlags(
                Flags.ModRM | Flags.ModRMReg
            ).Build();
    public static Pattern MOVZX_RegRM16 { get; }
        = Pattern.NewBuilder(0xB7)
            .WithPrefixGroup(0x0F)
            .WithFlags(
                Flags.ModRM | Flags.ModRMReg
            ).Build();
    public static Pattern MOVSX_RegRM8 { get; }
        = Pattern.NewBuilder(0xBE)
            .WithPrefixGroup(0x0F)
            .WithFlags(
                Flags.ModRM | Flags.ModRMReg
            ).Build();
    public static Pattern MOVSX_RegRM16 { get; }
        = Pattern.NewBuilder(0xBF)
            .WithPrefixGroup(0x0F)
            .WithFlags(
                Flags.ModRM | Flags.ModRMReg
            ).Build();

    public static Pattern SETO { get; }
        = Pattern.NewBuilder(0x90)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();
    public static Pattern SETNO { get; }
        = Pattern.NewBuilder(0x91)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();
    public static Pattern SETB { get; }
        = Pattern.NewBuilder(0x92)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();
    public static Pattern SETNB { get; }
        = Pattern.NewBuilder(0x93)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();
    public static Pattern SETE { get; }
        = Pattern.NewBuilder(0x94)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();
    public static Pattern SETNE { get; }
        = Pattern.NewBuilder(0x95)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();
    public static Pattern SETBE { get; }
        = Pattern.NewBuilder(0x96)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();
    public static Pattern SETNBE { get; }
        = Pattern.NewBuilder(0x97)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();
    public static Pattern SETS { get; }
        = Pattern.NewBuilder(0x98)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();
    public static Pattern SETNS { get; }
        = Pattern.NewBuilder(0x99)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();
    public static Pattern SETP { get; }
        = Pattern.NewBuilder(0x9A)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();
    public static Pattern SETNP { get; }
        = Pattern.NewBuilder(0x9B)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();
    public static Pattern SETL { get; }
        = Pattern.NewBuilder(0x9C)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();
    public static Pattern SETNL { get; }
        = Pattern.NewBuilder(0x9D)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();
    public static Pattern SETLE { get; }
        = Pattern.NewBuilder(0x9E)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();
    public static Pattern SETNLE { get; }
        = Pattern.NewBuilder(0x9F)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();

    public static Pattern JO_rel1632 { get; }
        = Pattern.NewBuilder(0x80)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();
    public static Pattern JNO_rel1632 { get; }
        = Pattern.NewBuilder(0x81)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();
    public static Pattern JB_rel1632 { get; }
        = Pattern.NewBuilder(0x82)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();
    public static Pattern JNB_rel1632 { get; }
        = Pattern.NewBuilder(0x83)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();
    public static Pattern JE_rel1632 { get; }
        = Pattern.NewBuilder(0x84)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();
    public static Pattern JNE_rel1632 { get; }
        = Pattern.NewBuilder(0x85)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();
    public static Pattern JBE_rel1632 { get; }
        = Pattern.NewBuilder(0x86)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();
    public static Pattern JNBE_rel1632 { get; }
        = Pattern.NewBuilder(0x87)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();
    public static Pattern JS_rel1632 { get; }
        = Pattern.NewBuilder(0x88)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();
    public static Pattern JNS_rel1632 { get; }
        = Pattern.NewBuilder(0x89)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();
    public static Pattern JP_rel1632 { get; }
        = Pattern.NewBuilder(0x8A)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();
    public static Pattern JNP_rel1632 { get; }
        = Pattern.NewBuilder(0x8B)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();
    public static Pattern JL_rel1632 { get; }
        = Pattern.NewBuilder(0x8C)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();
    public static Pattern JNL_rel1632 { get; }
        = Pattern.NewBuilder(0x8D)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();
    public static Pattern JLE_rel1632 { get; }
        = Pattern.NewBuilder(0x8E)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();
    public static Pattern JNLE_rel1632 { get; }
        = Pattern.NewBuilder(0x8F)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Rel1632)
            .Build();

    public static Pattern IMUL_RegRM { get; }
        = Pattern.NewBuilder(0xAF)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMOpcode)
            .Build();

    public static Pattern FSPrefix { get; }
        = Pattern.NewBuilder(0x64)
            .WithFlags(Flags.Pfix)
            .Build();
    public static Pattern GSPrefix { get; }
        = Pattern.NewBuilder(0x65)
            .WithFlags(Flags.Pfix)
            .Build();

    public static Pattern PUSH_FS { get; }
        = Pattern.NewBuilder(0xA0)
            .WithPrefixGroup(0x0F)
            .Build();
    public static Pattern POP_FS { get; }
        = Pattern.NewBuilder(0xA1)
            .WithPrefixGroup(0x0F)
            .Build();
    public static Pattern PUSH_GS { get; }
        = Pattern.NewBuilder(0xA8)
            .WithPrefixGroup(0x0F)
            .Build();
    public static Pattern POP_GS { get; }
        = Pattern.NewBuilder(0xA9)
            .WithPrefixGroup(0x0F)
            .Build();

    public static Pattern LFS { get; }
        = Pattern.NewBuilder(0xB4)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();
    public static Pattern LGS { get; }
        = Pattern.NewBuilder(0xB5)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();
    public static Pattern LSS { get; }
        = Pattern.NewBuilder(0xB2)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();

    public static Pattern MOV_RegCr { get; }
        = Pattern.NewBuilder(0x20)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();
    public static Pattern MOV_CrReg { get; }
        = Pattern.NewBuilder(0x22)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();
    public static Pattern MOV_RegDr { get; }
        = Pattern.NewBuilder(0x21)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();
    public static Pattern MOV_DrReg { get; }
        = Pattern.NewBuilder(0x23)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();
    public static Pattern MOV_RegTr { get; }
        = Pattern.NewBuilder(0x24)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();
    public static Pattern MOV_TrReg { get; }
        = Pattern.NewBuilder(0x26)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();

    public static Pattern INT1 { get; }
        = Pattern.NewBuilder(0xF1).Build();

    public static Pattern UMOV_RmR8 { get; }
        = Pattern.NewBuilder(0x10)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();
    public static Pattern UMOV_RmR1632 { get; }
        = Pattern.NewBuilder(0x11)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();
    public static Pattern UMOV_R8Rm { get; }
        = Pattern.NewBuilder(0x12)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();
    public static Pattern UMOV_R1632Rm { get; }
        = Pattern.NewBuilder(0x13)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();

    public static Pattern XBTS { get; }
        = Pattern.NewBuilder(0xA6)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();

    public static Pattern IBTS { get; }
        = Pattern.NewBuilder(0xA7)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();
    public static Pattern LOADALL386 { get; }
        = Pattern.NewBuilder(0x07)
            .WithPrefixGroup(0x0F)
            .Build();
    #endregion

    #region New Definitions
    public static Def Bt { get; }
        = Def.NewBuilder(Tags.Bt)
            .WithPatterns(
                BT_RMImm8,
                BT_RMR
            ).Build();
    public static Def Bts { get; }
        = Def.NewBuilder(Tags.Bts)
            .WithPatterns(
                BTS_RMImm8,
                BTS_RMR
            ).Build();
    public static Def Btr { get; }
        = Def.NewBuilder(Tags.Btr)
            .WithPatterns(
                BTR_RMImm8,
                BTR_RMR
            ).Build();
    public static Def Btc { get; }
        = Def.NewBuilder(Tags.Btc)
            .WithPatterns(
                BTC_RMImm8,
                BTC_RMR
            ).Build();
    public static Def Bsf { get; }
        = Def.NewBuilder(Tags.Bsf).WithPattern(BSF).Build();
    public static Def Bsr { get; }
        = Def.NewBuilder(Tags.Bsr).WithPattern(BSR).Build();

    public static Def Shld { get; }
        = Def.NewBuilder(Tags.Shld)
            .WithPatterns(
                SHLD_Imm,
                SHLD_Cl
            ).Build();

    public static Def Shrd { get; }
        = Def.NewBuilder(Tags.Shrd)
            .WithPatterns(
                SHRD_Imm,
                SHRD_Cl
            ).Build();

    public static Def Movzx { get; }
        = Def.NewBuilder(Tags.Movzx)
            .WithPatterns(
                MOVZX_RegRM8,
                MOVZX_RegRM16
            ).Build();

    public static Def Movsx { get; }
        = Def.NewBuilder(Tags.Movsx)
            .WithPatterns(
                MOVSX_RegRM8,
                MOVSX_RegRM16
            ).Build();

    public static Def Seto { get; }
        = Def.NewBuilder(Tags.Seto).WithPattern(SETO).Build();
    public static Def Setno { get; }
        = Def.NewBuilder(Tags.Setno).WithPattern(SETNO).Build();
    public static Def Setb { get; }
        = Def.NewBuilder(Tags.Setb).WithPattern(SETB).Build();
    public static Def Setnb { get; }
        = Def.NewBuilder(Tags.Setnb).WithPattern(SETNB).Build();
    public static Def Sete { get; }
        = Def.NewBuilder(Tags.Sete).WithPattern(SETE).Build();
    public static Def Setne { get; }
        = Def.NewBuilder(Tags.Setne).WithPattern(SETNE).Build();
    public static Def Setbe { get; }
        = Def.NewBuilder(Tags.Setbe).WithPattern(SETBE).Build();
    public static Def Setnbe { get; }
        = Def.NewBuilder(Tags.Setnbe).WithPattern(SETNBE).Build();
    public static Def Sets { get; }
        = Def.NewBuilder(Tags.Sets).WithPattern(SETS).Build();
    public static Def Setns { get; }
        = Def.NewBuilder(Tags.Setns).WithPattern(SETNS).Build();
    public static Def Setp { get; }
        = Def.NewBuilder(Tags.Setp).WithPattern(SETB).Build();
    public static Def Setnp { get; }
        = Def.NewBuilder(Tags.Setnp).WithPattern(SETNB).Build();
    public static Def Setl { get; }
        = Def.NewBuilder(Tags.Setl).WithPattern(SETL).Build();
    public static Def Setnl { get; }
        = Def.NewBuilder(Tags.Setnl).WithPattern(SETNL).Build();
    public static Def Setle { get; }
        = Def.NewBuilder(Tags.Setle).WithPattern(SETLE).Build();
    public static Def Setnle { get; }
        = Def.NewBuilder(Tags.Setnle).WithPattern(SETNLE).Build();

    #endregion

    #region Extended Definitions
    public static new Def Jo { get; }
        = Def.NewBuilder(Set86.Jo).WithPattern(JO_rel1632).Build();
    public static new Def Jno { get; }
        = Def.NewBuilder(Set86.Jno).WithPattern(JNO_rel1632).Build();
    public static new Def Jb { get; }
        = Def.NewBuilder(Set86.Jb).WithPattern(JB_rel1632).Build();
    public static new Def Jnb { get; }
        = Def.NewBuilder(Set86.Jnb).WithPattern(JNB_rel1632).Build();
    public static new Def Je { get; }
        = Def.NewBuilder(Set86.Je).WithPattern(JE_rel1632).Build();
    public static new Def Jne { get; }
        = Def.NewBuilder(Set86.Jne).WithPattern(JNE_rel1632).Build();
    public static new Def Jbe { get; }
        = Def.NewBuilder(Set86.Jbe).WithPattern(JBE_rel1632).Build();
    public static new Def Jnbe { get; }
        = Def.NewBuilder(Set86.Jnbe).WithPattern(JNBE_rel1632).Build();
    public static new Def Js { get; }
        = Def.NewBuilder(Set86.Js).WithPattern(JS_rel1632).Build();
    public static new Def Jns { get; }
        = Def.NewBuilder(Set86.Jns).WithPattern(JNS_rel1632).Build();
    public static new Def Jp { get; }
        = Def.NewBuilder(Set86.Jp).WithPattern(JP_rel1632).Build();
    public static new Def Jnp { get; }
        = Def.NewBuilder(Set86.Jnp).WithPattern(JNP_rel1632).Build();
    public static new Def Jl { get; }
        = Def.NewBuilder(Set86.Jl).WithPattern(JL_rel1632).Build();
    public static new Def Jnl { get; }
        = Def.NewBuilder(Set86.Jnl).WithPattern(JNL_rel1632).Build();
    public static new Def Jle { get; }
        = Def.NewBuilder(Set86.Jle).WithPattern(JLE_rel1632).Build();
    public static new Def Jnle { get; }
        = Def.NewBuilder(Set86.Jnle).WithPattern(JNLE_rel1632).Build();

    public static new Def Imul { get; }
        = Def.NewBuilder(Set86.Imul).WithPattern(
            IMUL_RegRM
        ).Build();

    // FS:
    public static Def FsPrefix { get; }
        = Def.NewBuilder(Tags.FsPrefix).WithPattern(FSPrefix).Build();

    // GS:
    public static Def GsPrefix { get; }
        = Def.NewBuilder(Tags.GsPrefix).WithPattern(GSPrefix).Build();

    // Push
    public static new Def Push { get; }
        = Def.NewBuilder(Set86.Push).WithPattern(PUSH_FS).Build();
    // Pop
    public static new Def Pop { get; }
        = Def.NewBuilder(Set86.Pop).WithPattern(POP_FS).Build();

    // LFS
    public static Def Lfs { get; }
        = Def.NewBuilder(Tags.Lfs).WithPattern(LFS).Build();

    // LGS
    public static Def Lgs { get; }
        = Def.NewBuilder(Tags.Lgs).WithPattern(LGS).Build();

    // LSS
    public static Def Lss { get; }
        = Def.NewBuilder(Tags.Lss).WithPattern(LSS).Build();

    // MOV
    public static new Def Mov { get; }
        = Def.NewBuilder(Set86.Mov)
            .WithPatterns(
                MOV_RegCr,
                MOV_CrReg,
                MOV_RegDr,
                MOV_DrReg,
                MOV_RegTr,
                MOV_TrReg
            ).Build();

    // INT1
    public static Def Int1 { get; }
        = Def.NewBuilder(Tags.Int1).WithPattern(INT1).Build();

    // UMOV
    public static Def Umov { get; }
        = Def.NewBuilder(Tags.Umov)
            .WithPatterns(
                UMOV_RmR8,
                UMOV_RmR1632,
                UMOV_R8Rm,
                UMOV_R1632Rm
            ).Build();

    // XBTS
    public static Def Xbts { get; }
        = Def.NewBuilder(Tags.Xbts).WithPattern(XBTS).Build();

    // IBTS
    public static Def Ibts { get; }
        = Def.NewBuilder(Tags.Ibts).WithPattern(IBTS).Build();

    // LOADALL386
    public static Def Loadall386 { get; }
        = Def.NewBuilder(Tags.Loadall386).WithPattern(LOADALL386).Build();


    #endregion
    public override InstructionTag[,] InstructionMatrix { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
}