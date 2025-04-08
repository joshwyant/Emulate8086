using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulate8086.Processor
{
    [Flags]
    internal enum InstructionDecoderFlags
    {
        None = 0,
        Byte = 1, // Immediate data can be at least 1 byte
        Word = 1 << 1, // Immediate data can be 2 bytes
        W = 1 << 2, // Has word size flag
        D = 1 << 3, // Has direction flag
        Seg = 1 << 4, // Middle 2 bits segment
        Reg = 1 << 5, // First 3 bits register
        S = 1 << 6, // Has short/sign extend flag
        Z = 1 << 7, // Has zero flag
        V = 1 << 8, // Has flag for count in CL
        Pfix = 1 << 9, // Instruction is a prefix
        AddL = 1 << 10, // Long address
        Addr = 1 << 11, // Short address
        ModRM = 1 << 12, // ModRM byte
        ModRMOpcode = 1 << 13, // ModRM byte can have extended opcode
        ModRMReg = 1 << 14, // ModRM byte can have register
        ModRMSeg = 1 << 15, // ModRM byte can have segment register
        DispB = 1 << 16,
        DispW = 1 << 17,
        ModRM16 = 1 << 18, // ModRM byte points to 16-bit data
    }
}
