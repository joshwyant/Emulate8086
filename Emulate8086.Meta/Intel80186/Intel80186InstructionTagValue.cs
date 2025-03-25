namespace Emulate8086.Meta.Intel80186;
// https://en.wikipedia.org/wiki/X86_instruction_listings

public enum Intel80186InstructionTagValue
{
    None,
    BOUND,
    ENTER,
    INS,
    LEAVE,
    OUTS,
    POPA,
    PUSHA,
}