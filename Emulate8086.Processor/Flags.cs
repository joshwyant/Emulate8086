// IBM PC Technical Reference
// Appendix B: 8088 Assembly Instruction Reference
// p. B-3 
namespace Emulate8086
{
    [Flags]
    public enum Flags : ushort
    {
        None,
        Carry = 0x00001,            
        Parity = 0x00004,
        AuxiliaryCarry = 0x0010,
        Zero = 0x0040,
        Sign = 0x0080,
        Trap = 0x0100,
        InterruptEnable = 0x0200,
        Direction = 0x0400,
        Overflow = 0x0800,
        CF = 0x0001,
        PF = 0x0004,
        AF = 0x0010,
        ZF = 0x0040,
        SF = 0x0080,
        TF = 0x0100,
        IF = 0x0200,
        DF = 0x0400,
        OF = 0x0800
    }
}