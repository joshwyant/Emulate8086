namespace Emulate8086.Meta.Intel286;
// https://en.wikipedia.org/wiki/X86_instruction_listings

public enum Intel286InstructionTagValue
{
    None,
    LGDT,
    LIDT,
    LMSW,
    CLTS,
    LLDT,
    LTR,
    SGDT,
    SIDT,
    SMSW,
    SLDT,
    STR,
    ARPL,
    LAR,
    LSL,
    VERR,
    VERW,
    LOADALL,
    STOREALL
}