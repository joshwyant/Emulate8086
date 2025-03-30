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
    public enum Register16 : byte
    {
        AX,
        CX,
        DX,
        BX,
        SP,
        BP,
        SI,
        DI,
    }
}
