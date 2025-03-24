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
    public static Def Lea { get; } = new(Tags.Lea);
    public static Def Lds { get; } = new(Tags.Lds);
    public static Def Les { get; } = new(Tags.Les);
    public static Def Lahf { get; } = new(Tags.Lahf);
    public static Def Sahf { get; } = new(Tags.Sahf);
    public static Def Pushf { get; } = new(Tags.Pushf);
    public static Def Popf { get; } = new(Tags.Popf);
    public static Def Add { get; } = new(Tags.Add);
    public static Def Adc { get; } = new(Tags.Adc);
    public static Def Inc { get; } = new(Tags.Inc);
    public static Def Aaa { get; } = new(Tags.Aaa);
    public static Def Daa { get; } = new(Tags.Daa);
    public static Def Sub { get; } = new(Tags.Sub);
    public static Def Sbb { get; } = new(Tags.Sbb);
    public static Def Dec { get; } = new(Tags.Dec);
    public static Def Neg { get; } = new(Tags.Neg);
    public static Def Cmp { get; } = new(Tags.Cmp);
    public static Def Aas { get; } = new(Tags.Aas);
    public static Def Das { get; } = new(Tags.Das);
    public static Def Mul { get; } = new(Tags.Mul);
    public static Def Imul { get; } = new(Tags.Imul);
    public static Def Aam { get; } = new(Tags.Aam);
    public static Def Div { get; } = new(Tags.Div);
    public static Def Idiv { get; } = new(Tags.Idiv);
    public static Def Aad { get; } = new(Tags.Aad);
    public static Def Cbw { get; } = new(Tags.Cbw);
    public static Def Cwd { get; } = new(Tags.Cwd);
    public static Def Not { get; } = new(Tags.Not);
    public static Def Shl { get; } = new(Tags.Shl);
    public static Def Shr { get; } = new(Tags.Shr);
    public static Def Sar { get; } = new(Tags.Sar);
    public static Def Rol { get; } = new(Tags.Rol);
    public static Def Ror { get; } = new(Tags.Ror);
    public static Def Rcl { get; } = new(Tags.Rcl);
    public static Def Rcr { get; } = new(Tags.Rcr);
    public static Def And { get; } = new(Tags.And);
    public static Def Test { get; } = new(Tags.Test);
    public static Def Or { get; } = new(Tags.Or);
    public static Def Xor { get; } = new(Tags.Xor);
    public static Def Rep { get; } = new(Tags.Rep);
    public static Def Movs { get; } = new(Tags.Movs);
    public static Def Cmps { get; } = new(Tags.Cmps);
    public static Def Scas { get; } = new(Tags.Scas);
    public static Def Lods { get; } = new(Tags.Lods);
    public static Def Stos { get; } = new(Tags.Stos);
    public static Def Call { get; } = new(Tags.Call);
    public static Def Jmp { get; } = new(Tags.Jmp);
    public static Def Ret { get; } = new(Tags.Ret);
    public static Def Je { get; } = new(Tags.Je);
    public static Def Jl { get; } = new(Tags.Jl);
    public static Def Jle { get; } = new(Tags.Jle);
    public static Def Jb { get; } = new(Tags.Jb);
    public static Def Jbe { get; } = new(Tags.Jbe);
    public static Def Jp { get; } = new(Tags.Jp);
    public static Def Jo { get; } = new(Tags.Jo);
    public static Def Js { get; } = new(Tags.Js);
    public static Def Jne { get; } = new(Tags.Jne);
    public static Def Jnl { get; } = new(Tags.Jnl);
    public static Def Jnle { get; } = new(Tags.Jnle);
    public static Def Jnb { get; } = new(Tags.Jnb);
    public static Def Jnbe { get; } = new(Tags.Jnbe);
    public static Def Jnp { get; } = new(Tags.Jnp);
    public static Def Jno { get; } = new(Tags.Jno);
    public static Def Jns { get; } = new(Tags.Jns);
    public static Def Loop { get; } = new(Tags.Loop);
    public static Def Loopz { get; } = new(Tags.Loopz);
    public static Def Loopnz { get; } = new(Tags.Loopnz);
    public static Def Jcxz { get; } = new(Tags.Jcxz);
    public static Def Int { get; } = new(Tags.Int);
    public static Def Into { get; } = new(Tags.Into);
    public static Def Iret { get; } = new(Tags.Iret);
    public static Def Clc { get; } = new(Tags.Clc);
    public static Def Cmc { get; } = new(Tags.Cmc);
    public static Def Cld { get; } = new(Tags.Cld);
    public static Def Cli { get; } = new(Tags.Cli);
    public static Def Hlt { get; } = new(Tags.Hlt);
    public static Def Lock { get; } = new(Tags.Lock);
    public static Def Stc { get; } = new(Tags.Stc);
    public static Def Nop { get; } = new(Tags.Nop);
    public static Def Std { get; } = new(Tags.Std);
    public static Def Sti { get; } = new(Tags.Sti);
    public static Def Wait { get; } = new(Tags.Wait);
    public static Def Esc { get; } = new(Tags.Esc);
    public override InstructionTag[,] InstructionMatrix { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
}