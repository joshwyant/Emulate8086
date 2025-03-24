namespace Emulate8086.Meta.Intel286;

using Tags = Intel286InstructionTags;
using Pattern = InstructionOpcodePattern;
using Flags = InstructionPatternFlags;
using Def = InstructionDefinition;

public class Intel286InstructionSet : Intel8086.Intel8086InstructionSet
{
    public static new Intel286InstructionSet Create()
    {
        return new Intel286InstructionSet(
            Def.GetAll(typeof(Def)));
    }
    protected Intel286InstructionSet(IEnumerable<Def> instructions)
        : base(instructions) { }

    public static Pattern LGDT { get; }
        = Pattern.NewBuilder(0x01)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(2).Build();

    public static Pattern LIDT { get; }
        = Pattern.NewBuilder(0x01)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(3).Build();

    public static Pattern LMSW { get; }
        = Pattern.NewBuilder(0x01)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(6).Build();

    public static Pattern CLTS { get; }
        = Pattern.NewBuilder(0x06)
            .WithPrefixGroup(0x0F).Build();

    public static Pattern LLDT { get; }
        = Pattern.NewBuilder(0x00)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(2).Build();

    public static Pattern LTR { get; }
        = Pattern.NewBuilder(0x00)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(3).Build();

    public static Pattern SGDT { get; }
        = Pattern.NewBuilder(0x01)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0).Build();

    public static Pattern SIDT { get; }
        = Pattern.NewBuilder(0x01)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(1).Build();

    public static Pattern SMSW { get; }
        = Pattern.NewBuilder(0x01)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(4).Build();

    public static Pattern SLDT { get; }
        = Pattern.NewBuilder(0x00)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(0).Build();

    public static Pattern STR { get; }
        = Pattern.NewBuilder(0x00)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(1).Build();

    public static Pattern ARPL { get; }
        = Pattern.NewBuilder(0x63)
            .WithFlags(Flags.ModRM | Flags.ModRMReg).Build();

    public static Pattern LAR { get; }
        = Pattern.NewBuilder(0x02)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg).Build();

    public static Pattern LSL { get; }
        = Pattern.NewBuilder(0x03)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg).Build();

    public static Pattern VERR { get; }
        = Pattern.NewBuilder(0x00)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(4).Build();

    public static Pattern VERW { get; }
        = Pattern.NewBuilder(0x00)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(5).Build();

    public static Pattern LOADALL { get; }
        = Pattern.NewBuilder(0x05)
            .WithPrefixGroup(0x0F).Build();

    public static Pattern STOREALL { get; }
        = Pattern.NewBuilder(0x04)
            .WithPrefixGroup(0x0F)
            .Build();  // TODO: Also prefixed with F1: F1 0F 04

    public Def Lgdt { get; } = new(Tags.Lgdt, LGDT);
    public Def Lidt { get; } = new(Tags.Lidt, LIDT);
    public Def Lmsw { get; } = new(Tags.Lmsw, LMSW);
    public Def Clts { get; } = new(Tags.Clts, CLTS);
    public Def Lldt { get; } = new(Tags.Lldt, LLDT);
    public Def Ltr { get; } = new(Tags.Ltr, LTR);
    public Def Sgdt { get; } = new(Tags.Sgdt, SGDT);
    public Def Sidt { get; } = new(Tags.Sidt, SIDT);
    public Def Smsw { get; } = new(Tags.Smsw, SMSW);
    public Def Sldt { get; } = new(Tags.Sldt, SLDT);
    public Def Str { get; } = new(Tags.Str, STR);
    public Def Arpl { get; } = new(Tags.Arpl, ARPL);
    public Def Lar { get; } = new(Tags.Lar, LAR);
    public Def Lsl { get; } = new(Tags.Lsl, LSL);
    public Def Verr { get; } = new(Tags.Verr, VERR);
    public Def Verw { get; } = new(Tags.Verw, VERW);
    public Def Loadall { get; } = new(Tags.Loadall, LOADALL);
    public Def Storeall { get; } = new(Tags.Storeall, STOREALL);

    public override InstructionTag[,] InstructionMatrix { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
}