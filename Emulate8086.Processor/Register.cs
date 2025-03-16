using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// IBM PC Technical Reference
// Appendix B: 8088 Assembly Instruction Reference
// p. B-3 
namespace Emulate8086.Processor
{
    public enum Register : int
    {
        None = -1,
        AX,
        CX,
        DX,
        BX,
        SP,
        BP,
        SI,
        DI,
        AL = 0,
        CL,
        DL,
        BL,
        AH,
        CH,
        DH,
        BH,
        ES = 0,
        CS,
        SS,
        DS,
        Disp0 = 0,
        DispDirectMem = 0,
        Disp8,
        Disp16,
        DispReg,
        MemBXSI = 0,
        MemBXDI,
        MemBPSI,
        MemBPDI,
        MemSI,
        MemDI,
        MemBP, // except if mod = 00 and r/m = 110 then EA = disp-high: disp-low
        MemBX,
        MemDirect = 0b110
    }
}
