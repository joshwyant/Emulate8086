namespace Emulate8086.Meta.Intel386;
// https://en.wikipedia.org/wiki/X86_instruction_listings

public enum Intel386InstructionTagValue
{
    None,

    // 80186-added instructions were merged first with 386,
    // but we can reuse those tags elsewhere rather than
    // redefining them.

    // 386 instructions:
    OperandSizeOverridePrefix,
    AddressSizeOverridePrefix,
    BT,
    BTS,
    BTR,
    BTC,
    BSF,
    BSR,
    SHLD,
    SHRD,
    MOVZX,
    MOVSX,
    SETO,
    SETNO,
    SETB,
    SETNB,
    SETE,
    SETNE,
    SETBE,
    SETNBE,
    SETS,
    SETNS,
    SETP,
    SETNP,
    SETL,
    SETNL,
    SETLE,
    SETNLE,
    FSPrefix,
    GSPrefix,
    LFS,
    LGS,
    LSS,
    INT1,
    UMOV,
    XBTS,  // Early 386 only
    IBTS,  // Early 386 only
    LOADALL386,
}