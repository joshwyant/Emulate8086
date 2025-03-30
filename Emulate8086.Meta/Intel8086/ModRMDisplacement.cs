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
    public enum ModRMDisplacement : byte
    {
        MemBXSI = 0,
        MemBXDI,
        MemBPSI,
        MemBPDI,
        MemSI,
        MemDI,
        MemBP, // except if mod = 00 and r/m = 110 then EA = disp-high: disp-low
        MemBX,
        // Outside of range, after decoding and for actual logic
        FixedDisplacement,
        NoDisplacement,
    }
}
