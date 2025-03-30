using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// IBM PC Technical Reference
// Appendix B: 8088 Assembly Instruction Reference
// p. B-3 
namespace Emulate8086.Meta.Intel8086
{
    public enum Register8 : byte
    {
        AL = 0,
        CL,
        DL,
        BL,
        AH,
        CH,
        DH,
        BH,
    }
}
