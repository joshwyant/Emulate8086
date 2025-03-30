namespace Emulate8086.Meta.Intel8086;
using static Intel8086Tables.TableFlags;

public struct ModRMInfo
{
    public string ToString(short displacement = 0)
    {
        switch (Mode)
        {
            case ModRMMode.Register:
                return (Register16?.ToString() ?? Register8?.ToString())!.ToLower();
            case ModRMMode.FixedDisplacement:
                return $"[{displacement}:X]";
            default:
                var dispText = Mode == ModRMMode.NoDisplacement ? "" : $" + 0x{displacement:X}";
                return $"[{RM switch
                {
                    ModRMDisplacement.MemBXSI => "bx + si",
                    ModRMDisplacement.MemBXDI => "bx + di",
                    ModRMDisplacement.MemBPSI => "bp + si",
                    ModRMDisplacement.MemBPDI => "bp + di",
                    ModRMDisplacement.MemSI => "si",
                    ModRMDisplacement.MemDI => "di",
                    ModRMDisplacement.MemBP => "bp",
                    ModRMDisplacement.MemBX => "bx",
                    _ => ""
                }}{dispText}]";
        }
    }

    public string VariableString(ushort immediate = 0)
    {
        return (RegisterVariable16?.ToString() ?? RegisterVariable8?.ToString() ?? SegmentVariable?.ToString() ?? $"0x{immediate:X}").ToLower();
    }

    public ModRMInfo(byte instructionByte, byte modrmbyte, Intel8086Tables.TableFlags flags = _, Segment? prefix = default)
    {
        Mode = (ModRMMode)(modrmbyte >> 6);
        RM = (ModRMDisplacement)(modrmbyte & 0b111);
        var middle = (byte)((modrmbyte & 0b00111000) >> 3);
        EffectiveSegment = prefix ?? Segment.None;

        // Variable (middle part)
        switch (instructionByte)
        {
            case 0x80: // Immediate Group
            case 0x81:
            case 0x82:
            case 0x83:
            case 0xD0: // Shift Group
            case 0xD1:
            case 0xD2:
            case 0xD3:
            case 0xF6: // Group 1
            case 0xF7:
            case 0xFE: // Group 2
            case 0xFF:
                OpCodeVariable = middle;
                break;
            default:
                break;
        }
        if (flags.HasFlag(sr))
        {
            SegmentVariable = (Segment)middle;
        }
        else if (OpCodeVariable is null)
        {
            if (flags.HasFlag(b))
            {
                RegisterVariable8 = (Register8)middle;
            }
            else
            {
                RegisterVariable16 = (Register16)middle;
            }
        }

        // Non-displacement
        if ((byte)Mode == 0b00 && (byte)RM == 0b110)
        {
            // Special case
            Mode = ModRMMode.FixedDisplacement;
            RM = ModRMDisplacement.FixedDisplacement;
        }
        else if (Mode == ModRMMode.Register)
        {
            RM = ModRMDisplacement.NoDisplacement;
            if (flags.HasFlag(b))
            {
                Register8 = (Register8)RM;
            }
            else
            {
                Register16 = (Register16)RM;
            }
        }

        DisplacementBytes = Mode switch
        {
            ModRMMode.SignExtendedByteDisplacement => 1,
            ModRMMode.WordDisplacement => 2,
            ModRMMode.FixedDisplacement => 2,
            _ => 0
        };
    }
    public readonly Segment EffectiveSegment { get; }
    public readonly Register16? RegisterVariable16 { get; }
    public readonly Register8? RegisterVariable8 { get; }
    public readonly Segment? SegmentVariable { get; }
    public readonly byte? OpCodeVariable { get; }
    public readonly int DisplacementBytes { get; }
    public Register8? Register8 { get; }
    public Register16? Register16 { get; }
    public ModRMMode Mode { get; }
    public ModRMDisplacement RM { get; }
}