namespace Emulate8086.Meta;

public class InstructionOpcodePattern
{
    public byte OpcodeMask { get; }
    public byte OpcodePattern { get; }
    public int? ModRmOpcode { get; }
    public byte? PrefixGroup { get; }
    public InstructionPatternFlags Flags { get; }
    public (byte start, byte end)[] OpcodeRanges => opcodeRanges ??= ComputeOpcodeRanges();
    private byte[]? slots;
    private (byte start, byte end)[]? opcodeRanges;
    public byte[] Slots => slots ??= ComputeSlots();

    public bool IsOpcodeMatch(byte opcode) => (opcode & OpcodeMask) == OpcodePattern;

    public bool IsMatch(byte opcode)
    {
        if (ModRmOpcode is not null)
        {
            return false;
        }

        return IsOpcodeMatch(opcode);
    }

    public bool IsMatch(byte opcode, byte modRm)
    {
        if (ModRmOpcode is null)
        {
            return false;
        }

        return (opcode & OpcodeMask) == OpcodePattern
            && ((modRm & 0b111000) >> 3) == ModRmOpcode;
    }

    private InstructionOpcodePattern(byte opcodeMask, byte opcodePattern, int? modRmOpcode, byte? prefixGroup, InstructionPatternFlags flags)
    {
        OpcodeMask = opcodeMask;
        OpcodePattern = opcodePattern;
        ModRmOpcode = modRmOpcode;
        PrefixGroup = prefixGroup;
        Flags = flags;
    }

    public InstructionOpcodePattern(byte opcode)
    {
        OpcodeMask = 0xFF;
        OpcodePattern = opcode;
    }

    private byte[] ComputeSlots()
    {
        // Bytes that can make up the pattern from the variable bits
        // e.g. if mask is 0xF0 and pattern is 0xF0, then slots are 0xF0-0xFF
        var result = new List<byte>();
        // Positions of bits that are 0 in the mask (i.e. variable)
        var unmaskedBits = Enumerable.Range(0, 8)
            .Where(i => (OpcodeMask & (1 << i)) == 0)
            .ToArray();
        // Total number of combinations is 2^variable bits
        var numSlots = (1 << unmaskedBits.Length);
        // We'd gonna spread the binary numbers 0-n across the variable bits
        for (int val = 0; val < numSlots; val++)
        {
            // Start with the pattern bits
            var slot = OpcodePattern;

            // Go through the positions of each variable bit
            for (var nbit = 0; nbit < unmaskedBits.Length; nbit++)
            {
                var pos = unmaskedBits[nbit]; // Position of the variable bit
                var bit = val & (1 << nbit); // Value for the variable bit
                if (bit != 0)
                {
                    // If the bit is set, distribute it to its pushed position
                    slot |= (byte)(1 << pos);
                }
            }
            result.Add(slot);
        }
        return result.OrderBy(s => s).ToArray();
    }

    private (byte start, byte end)[] ComputeOpcodeRanges()
    {
        var ranges = new List<(byte start, byte end)>();
        for (int i = 0; i < Slots.Length; i++)
        {
            var start = Slots[i];
            var end = Slots[i];
            while (i + 1 < Slots.Length && Slots[i + 1] == end + 1)
            {
                end = Slots[++i];
            }
            ranges.Add((start, end));
        }
        return ranges.ToArray();
    }

    public static Builder NewBuilder(byte opcode) => new Builder(opcode);
    public static Builder NewBuilder(byte opcodeMask, byte opcodePattern) => new Builder(opcodeMask, opcodePattern);
    public static Builder NewBuilder(InstructionOpcodePattern pattern) => new Builder(pattern);
    public class Builder
    {
        private byte? prefixGroup;
        private byte opcodeMask;
        private byte opcodePattern;
        private int? modRmOpcode;
        private InstructionPatternFlags flags;
        internal Builder(byte opcode)
        {
            opcodeMask = 0xFF;
            opcodePattern = opcode;
        }

        internal Builder(byte opcodeMask, byte opcodePattern)
        {
            this.opcodeMask = opcodeMask;
            this.opcodePattern = opcodePattern;
        }
        internal Builder(InstructionOpcodePattern pattern)
        {
            opcodeMask = pattern.OpcodeMask;
            opcodePattern = pattern.OpcodePattern;
            prefixGroup = pattern.PrefixGroup;
            modRmOpcode = pattern.ModRmOpcode;
            flags = pattern.Flags;
        }

        public Builder WithModRmOpcode(int modRmOpcode)
        {
            this.modRmOpcode = modRmOpcode;
            return this;
        }

        public Builder WithPrefixGroup(byte prefixGroup)
        {
            this.prefixGroup = prefixGroup;
            return this;
        }

        public Builder WithFlags(InstructionPatternFlags flags)
        {
            this.flags = flags;
            return this;
        }

        public InstructionOpcodePattern Build()
        {
            return new InstructionOpcodePattern(opcodeMask, opcodePattern, modRmOpcode, prefixGroup, flags);
        }
    }

    public override int GetHashCode()
    {
        return OpcodePattern * 37 + OpcodeMask * 29 + (ModRmOpcode ?? 0) * 11;
    }

    public bool Equals(InstructionOpcodePattern other)
    {
        return ReferenceEquals(this, other);
    }
}
