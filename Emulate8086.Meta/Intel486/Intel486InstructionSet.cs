namespace Emulate8086.Meta.Intel486;

using Tags = Intel486InstructionTags;
using Set386 = Intel386.Intel386InstructionSet;
using Pattern = InstructionOpcodePattern;
using Flags = InstructionPatternFlags;
using Def = InstructionDefinition;

public class Intel486InstructionSet : Set386
{
    public static new Intel486InstructionSet Create()
    {
        return new Intel486InstructionSet(
            Def.GetAll(typeof(Def)));
    }
    protected Intel486InstructionSet(IEnumerable<Def> instructions)
        : base(instructions) { }

    #region Patterns
    public static Pattern BSWAP { get; }
        = Pattern.NewBuilder(0b11111_000, 0xC8)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.Reg)
            .Build();
    public static Pattern CMPXCHG_RMReg { get; }
        = Pattern.NewBuilder(0xB0)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();

    public static Pattern CMPXCHG_RMReg8 { get; }
        = Pattern.NewBuilder(0xB1)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();
    public static Pattern XADD_RMReg8 { get; }
        = Pattern.NewBuilder(0xC0)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();
    public static Pattern XADD_RMReg { get; }
        = Pattern.NewBuilder(0xC1)
            .WithPrefixGroup(0x0F)
            .WithFlags(Flags.ModRM | Flags.ModRMReg)
            .Build();
    public static Pattern INVLPG { get; }
        = Pattern.NewBuilder(0x01)
            .WithPrefixGroup(0x0F)
            .WithModRmOpcode(7) // TODO: Is this correct? 0F 01 /7 but no r/m operands
            .WithFlags(Flags.Byte)
            .Build();
    public static Pattern INVD { get; }
        = Pattern.NewBuilder(0x08)
            .WithPrefixGroup(0x0F)
            .Build();
    public static Pattern WBINVD { get; }
        = Pattern.NewBuilder(0x09)
            .WithPrefixGroup(0x0F)
            .Build(); // TODO: NFx prefix?

    #endregion

    #region Definitions
    public static Def Bswap { get; } = new(Tags.Bswap, BSWAP);
    public static Def CmpXchg { get; } = new(
        Tags.CmpXchg,
        CMPXCHG_RMReg,
        CMPXCHG_RMReg8);
    public static Def Xadd { get; } = new(
        Tags.Xadd,
        XADD_RMReg8,
        XADD_RMReg);
    public static Def Invlpg { get; } = new(Tags.Invlpg, INVLPG);
    public static Def Invd { get; } = new(Tags.Invd, INVD);
    public static Def Wbinvd { get; } = new(Tags.Wbinvd, WBINVD);
    #endregion
    public override InstructionTag[,] InstructionMatrix { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
}