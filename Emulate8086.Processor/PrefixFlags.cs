using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulate8086.Processor
{
    [Flags]
    internal enum PrefixFlags
    {
        None = 0,
        CS = 1,
        DS = 1 << 1,
        ES = 1 << 2,
        SS = 1 << 3,
        REP = 1 << 4,
        LOCK = 1 << 5,
    }
}
