using System.Runtime.CompilerServices;

namespace Emulate8086.Processor
{
    public partial class CPU
    {
        public CPU(Memory m) 
        { 
            memory = m;
            cs = 0;
            ip = 0;
        }

        Memory memory;
        byte insByte;
        Instruction ins;

        public void Clock()
        {
            // Read the instruction byte
            insByte = memory[csip++];

            // Get the instruction type from the matrix
            ins = instructionMatrix[insByte >> 4, insByte & 0xF];

            // Look up the implementation for the instruction type
            var impl = instructionImpls[ins];

            // Call the instruction
            impl();
        }
    }
}