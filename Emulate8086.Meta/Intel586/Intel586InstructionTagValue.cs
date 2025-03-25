namespace Emulate8086.Meta.Intel586;
// https://en.wikipedia.org/wiki/X86_instruction_listings

public enum Intel586InstructionTagValue
{
    None,
    RDMSR,
    WRMSR,
    RSM,
    CPUID,
    CMPXCHG8B,
    RDTSC,
}