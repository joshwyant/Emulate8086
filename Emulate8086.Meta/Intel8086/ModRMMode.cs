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
    public enum ModRMMode : byte
    {
        NoDisplacement = 0,
        SignExtendedByteDisplacement,
        WordDisplacement,
        Register,
        // Out of range; following is for further logic after decoding.
        FixedDisplacement,
    }
}
