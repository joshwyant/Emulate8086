namespace Emulate8086.Meta.Intel80186;

using Tags = Intel80186InstructionTags;
using Set86 = Intel8086.Intel8086InstructionSet;
using Pattern = InstructionOpcodePattern;
using Flags = InstructionPatternFlags;
using Def = InstructionDefinition;

public class Intel80186InstructionSet : Intel8086.Intel8086InstructionSet
{
    public static new Intel80186InstructionSet Create()
    {
        return new Intel80186InstructionSet(
            Def.GetAll(typeof(Def)));
    }
    protected Intel80186InstructionSet(IEnumerable<Def> instructions)
        : base(instructions) { }

    public static Pattern BOUND { get; }
        = Pattern.NewBuilder(0x62)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();
    public static Pattern ENTER { get; }
        = Pattern.NewBuilder(0xC8)
            .WithFlags(Flags.W | Flags.SecondByte)
            .Build();
    public static Pattern INS { get; }
        = Pattern.NewBuilder(0b111111_00, 0x6C)
            .WithFlags(Flags.W | Flags.D).Build();
    public static Pattern LEAVE { get; }
        = Pattern.NewBuilder(0xC9).Build();
    public static Pattern OUTS { get; }
        = Pattern.NewBuilder(0b111111_00, 0x6E)
            .WithFlags(Flags.W | Flags.D).Build();
    public static Pattern POPA { get; }
        = Pattern.NewBuilder(0x61).Build();
    public static Pattern PUSHA { get; }
        = Pattern.NewBuilder(0x60).Build();
    public static Pattern PUSH_ImmB { get; }
        = Pattern.NewBuilder(0x6A)
            .WithFlags(Flags.Byte).Build();
    public static Pattern PUSH_ImmW { get; }
        = Pattern.NewBuilder(0x68)
            .WithFlags(Flags.Word).Build();
    public static Pattern IMUL_ImmB { get; }
        = Pattern.NewBuilder(0x6B)
            .WithFlags(
                Flags.Byte |
                Flags.ModRM |
                Flags.ModRMReg)
            .Build();
    public static Pattern IMUL_ImmW { get; }
        = Pattern.NewBuilder(0x69)
            .WithFlags(Flags.Word |
                Flags.ModRM |
                Flags.ModRMReg
            ).Build();
    public static Pattern SHL_ImmB { get; }
        = Pattern.NewBuilder(SHL)
            .WithOpcode(0xC0)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte)
            //.WithModRmOpcode(/*inherited*/)
            .Build();
    public static Pattern SHL_ImmW { get; }
        = Pattern.NewBuilder(SHL)
            .WithOpcode(0xC1)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte)
            //.WithModRmOpcode(/*inherited*/)
            .Build();
    public static Pattern SHR_ImmB { get; }
        = Pattern.NewBuilder(SHR)
            .WithOpcode(0xC0)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte)
            //.WithModRmOpcode(/*inherited*/)
            .Build();
    public static Pattern SHR_ImmW { get; }
        = Pattern.NewBuilder(SHR)
            .WithOpcode(0xC1)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte)
            //.WithModRmOpcode(/*inherited*/)
            .Build();
    public static Pattern SAR_ImmB { get; }
        = Pattern.NewBuilder(SAR)
            .WithOpcode(0xC0)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte)
            //.WithModRmOpcode(/*inherited*/)
            .Build();
    public static Pattern SAR_ImmW { get; }
        = Pattern.NewBuilder(SAR)
            .WithOpcode(0xC1)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte)
            //.WithModRmOpcode(/*inherited*/)
            .Build();
    public static Pattern ROL_ImmB { get; }
        = Pattern.NewBuilder(ROL)
            .WithOpcode(0xC0)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte)
            //.WithModRmOpcode(/*inherited*/)
            .Build();
    public static Pattern ROL_ImmW { get; }
        = Pattern.NewBuilder(ROL)
            .WithOpcode(0xC1)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte)
            //.WithModRmOpcode(/*inherited*/)
            .Build();
    public static Pattern ROR_ImmB { get; }
        = Pattern.NewBuilder(ROR)
            .WithOpcode(0xC0)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte)
            //.WithModRmOpcode(/*inherited*/)
            .Build();
    public static Pattern ROR_ImmW { get; }
        = Pattern.NewBuilder(ROR)
            .WithOpcode(0xC1)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte)
            //.WithModRmOpcode(/*inherited*/)
            .Build();
    public static Pattern RCL_ImmB { get; }
        = Pattern.NewBuilder(RCL)
            .WithOpcode(0xC0)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte)
            //.WithModRmOpcode(/*inherited*/)
            .Build();
    public static Pattern RCL_ImmW { get; }
        = Pattern.NewBuilder(RCL)
            .WithOpcode(0xC1)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte)
            //.WithModRmOpcode(/*inherited*/)
            .Build();
    public static Pattern RCR_ImmB { get; }
        = Pattern.NewBuilder(RCR)
            .WithOpcode(0xC0)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte)
            //.WithModRmOpcode(/*inherited*/)
            .Build();
    public static Pattern RCR_ImmW { get; }
        = Pattern.NewBuilder(RCR)
            .WithOpcode(0xC1)
            .WithFlags(
                Flags.ModRM |
                Flags.ModRMOpcode |
                Flags.Byte)
            //.WithModRmOpcode(/*inherited*/)
            .Build();

    public static Def Bound { get; } = new(Tags.Bound, BOUND);
    public static Def Enter { get; } = new(Tags.Enter, ENTER);
    public static Def Ins { get; } = new(Tags.Ins, INS);
    public static Def Leave { get; } = new(Tags.Leave, LEAVE);
    public static Def Outs { get; } = new(Tags.Outs, OUTS);
    public static Def Popa { get; } = new(Tags.Popa, POPA);
    public static Def Pusha { get; } = new(Tags.Pusha, PUSHA);

    public static new Def Push { get; } = Def.NewBuilder(Set86.Push)
        .WithPatterns(
            PUSH_ImmB,
            PUSH_ImmW)
        .Build();
    public static new Def Imul { get; } = Def.NewBuilder(Set86.Imul)
        .WithPatterns(
            IMUL_ImmB,
            IMUL_ImmW)
        .Build();
    public static new Def Shl { get; } = Def.NewBuilder(Set86.Shl)
        .WithPatterns(
            SHL_ImmB,
            SHL_ImmW)
        .Build();
    public static new Def Shr { get; } = Def.NewBuilder(Set86.Shr)
        .WithPatterns(
            SHR_ImmB,
            SHR_ImmW)
        .Build();
    public static new Def Sar { get; } = Def.NewBuilder(Set86.Sar)
        .WithPatterns(
            SAR_ImmB,
            SAR_ImmW)
        .Build();
    public static new Def Rol { get; } = Def.NewBuilder(Set86.Rol)
        .WithPatterns(
            ROL_ImmB,
            ROL_ImmW)
        .Build();
    public static new Def Ror { get; } = Def.NewBuilder(Set86.Ror)
        .WithPatterns(
            ROR_ImmB,
            ROR_ImmW)
        .Build();
    public static new Def Rcl { get; } = Def.NewBuilder(Set86.Rcl)
        .WithPatterns(
            RCL_ImmB,
            RCL_ImmW)
        .Build();
    public static new Def Rcr { get; } = Def.NewBuilder(Set86.Rcr)
        .WithPatterns(
            RCR_ImmB,
            RCR_ImmW)
        .Build();

    public override InstructionTag[,] InstructionMatrix { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
}