namespace Emulate8086.Meta.Intel8086;
using static Intel8086Tables;
using static Intel8086Tables.TableFlags;
using static Intel8086InstructionTagValue;
public class DecodedInstruction
{
    byte opcode;
    TableFlags flags;
    ModRMInfo? modrm;
    Intel8086InstructionTagValue ins;
    short disp;
    ushort? long_segment;
    ushort? immediate;
    int pc_orig;
    int bytes;
    byte[] memory;
    public string InstructionName { get; }
    public DecodedInstruction(byte[] memory, ref int pc, Segment prefix = Segment.None)
    {
        // Get from memory
        this.memory = memory;
        pc_orig = pc;
        opcode = memory[pc++];

        // Get the instruction
        var hi = (byte)(opcode >> 4);
        var lo = (byte)(opcode & 0xF);
        flags = FlagsMatrix[hi, lo];

        // Mod R/M Byte
        if (flags.HasFlag(rm))
        {
            modrm = new ModRMInfo(
                opcode,
                memory[pc++],
                flags,
                prefix);

            if (modrm.Value.DisplacementBytes == 1)
            {
                disp = (sbyte)memory[pc++];
            }
            else if (modrm.Value.DisplacementBytes == 2)
            {
                disp = (short)(memory[pc++] | (memory[pc++] << 8));
            }
        }

        ins = InstructionMatrix[hi, lo];
        var idx = modrm?.OpCodeVariable ?? 0;
        switch (ins)
        {
            case Immediate:
                ins = ImmediateGroupInstructions[idx];
                flags = ImmediateGroupFlags[idx];
                break;
            case Shift:
                ins = ShiftGroupInstructions[idx];
                flags = ShiftGroupFlags[idx];
                break;
            case Group1:
                ins = Group1Instructions[idx];
                flags = Group1Flags[idx];
                break;
            case Group2:
                ins = Group2Instructions[idx];
                flags = Group2Flags[idx];
                break;
        }

        if (flags.HasFlag(si))
        {
            immediate = (ushort)(short)(sbyte)memory[pc++];
        }
        else if (flags.HasFlag(i))
        {
            immediate = memory[pc++];
            if (flags.HasFlag(w))
            {
                immediate = (ushort)(immediate.Value | (memory[pc++] << 8));
            }
        }

        if (flags.HasFlag(d) || flags.HasFlag(l) || flags.HasFlag(m))
        {
            disp = (short)(ushort)(memory[pc++] | (memory[pc++] << 8));
        }
        if (flags.HasFlag(l))
        {
            long_segment = (ushort)(memory[pc++] | (memory[pc++] << 8));
        }

        InstructionName = ins switch
        {
            ES => "ES:",
            CS => "CS:",
            SS => "SS:",
            DS => "DS:",
            None => $"db {opcode:X2}",
            _ => ins.ToString().ToLower()// + (flags.HasFlag(b) ? "b" : flags.HasFlag(w) ? "w" : "")
        };

        if (flags.HasFlag(z))
        {
            InstructionName += "z";
        }

        bytes = pc - pc_orig;
    }

    public override string ToString()
    {
        return uncommented() + "  ; " + string.Join("", Enumerable.Range(pc_orig, bytes).Select(i => $"{memory[i]:x2}"));
    }

    string uncommented()
    {
        var memory = "";
        var arg = "";
        var name = InstructionName.ToString();
        if (flags.HasFlag(rm))
        {
            memory = modrm!.Value.ToString(disp);
            arg = modrm!.Value.VariableString();
            return flags.HasFlag(t) ? $"{name} {arg}, {memory}" : $"{name} {memory}, {arg}";
        }
        switch (opcode & 0b11111000)
        {
            case 0b10110000:
            case 0b10111000:
            case 0b01010000:
            case 0b01011000:
            case 0b10010000:
            case 0b01000000:
            case 0b01001000:
                var reg = opcode & 0b111;
                var regname = (flags.HasFlag(b)
                    ? ((Register8)reg).ToString()
                    : ((Register16)reg).ToString()).ToLower();
                return $"{InstructionName} {regname}" + (immediate is not null ? $", 0x{immediate:X}" : "");
        }
        switch (opcode & 0b11100111)
        {
            case 0b00000110:
            case 0b00000111:
                var seg = ((Segment)((opcode & 0b00011000) >> 3)).ToString().ToLower();
                return $"{InstructionName} {seg}";
        }
        if (flags.HasFlag(ia))
        {
            return $"{InstructionName} {(flags.HasFlag(b) ? "al" : "ax")}, 0x{immediate:X}";
        }
        if (flags.HasFlag(m) | flags.HasFlag(d))
        {
            return $"{InstructionName} [0x{disp:X4}]";
        }
        if (flags.HasFlag(l))
        {
            return $"{InstructionName} far [{long_segment:X4}:{(ushort)disp:X4}]";
        }
        if (flags.HasFlag(v))
        {
            return $"{InstructionName} {(flags.HasFlag(b) ? "dl" : "dx")}";
        }

        return InstructionName;
    }
}