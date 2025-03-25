namespace Emulate8086.Meta.Amd64;
// https://en.wikipedia.org/wiki/X86_instruction_listings

public enum Amd64InstructionTagValue
{
    None,
    RexPrefix,
    SYSCALL,
    SYSRET,
    SYSENTER,
    SYSEXIT,
    CDQE,
    CQO,
    CMPSQ,
    CMPXCHG16B,
    IRETQ,
    JRCXZ,
    LODSQ,
    MOVSXD,
    MOVSQ,
    POPFQ,
    PUSHFQ,
    SCASQ,
    STOSQ,
    SWAPGS,
}