namespace Emulate8086.Meta.Intel386;
// https://en.wikipedia.org/wiki/X86_instruction_listings

public enum Intel386InstructionTagValue
{
    None,

    // Added with 80186:
    BOUND,
    ENTER,
    INSB,
    INSW,
    LEAVE,
    OUTSB,
    OUTSW,
    POPA,
    PUSHA,
    PUSH, // Immediate version
    IMUL, // Immediate version
    SHL, // Immediate version
    SHR, // Immediate version
    SAL, // Immediate version
    SAR, // Immediate version
    ROL, // Immediate version
    ROR, // Immediate version
    RCL, // Immediate version
    RCR, // Immediate version

    // 386 instructions:
    // LODSD, etc.
}