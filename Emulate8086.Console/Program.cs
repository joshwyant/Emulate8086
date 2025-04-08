// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Text;
using Emulate8086;
using Emulate8086.Meta.Intel8086;
using Emulate8086.Processor;

bool prompting = false;
var loglevel = 4;
bool break_with_debugger = false;
bool breakpoints_enabled = true;
HashSet<int> break_addrs = [
    //0x7C00,
    //0x0500,
    //0x0700,
    //0x70 * 16 + 0x232,
    //0x9F84 * 16 + 0x34B //0x442,//0x420 //34B // 38F // 442
    //0x9f44 * 16 + 0x25e //0x238 //0x559 //0x333 // 0x0238
    //0x9F82A // 9f44:03ea
    //0x7d20,
    //0x7d26,
    //0x7d2d
    //0x8bb
    //0x9f6a1
    //0x70 * 16 + 0x1803,
    //0x700,
    //0x810 * 16 + 0x128,
    //0x1fe0 * 16 +
    //0x1fe0 * 16 + 0x7c5e, // FreeDOS boot sector after relocation
    //0x1fe0 * 16 + 0x7D6C, // FreeDOS sector right before call to "show"
    //0x1fe0 * 16 + 0x7dd2, // FreeDOS after rep movsb in do_int13_read
    //0x1fe0 * 16 + 0x7de6, // FreeDOS after jnz read_next
    //0x1fe0 * 16 + 0x7cc8, // FreeDOS next_entry
    //0x7cf3, // FreeDOS after call 2nd readDisk 
    //0x7cfc, // FreeDOS next_clust
    //0x7d20, // FreeDOS or ax, ax
//0x0060 * 16 +
    //0x0000, // FreeDOS second-stage bootloader
    //0xB587, // FreeDOS exeflat.c after rep movsb
    //0x001B, // Another movsw
    //0x00aa, // Before int 10h call
//0x12ce * 16 + 0x10,
    //0x16, // kernel loaded
    //0x12ce * 16 + 0x0092, // After jcxz
    //0x8f59 * 16 + 0x11d7,
    //0x0810 * 16 + 0x1093, // MS-DOS after printing 80 spaces
    //0x0070 * 16 + 0x079d,
    //0x0070 * 16 + 0x1a7e,
    //0x0060 * 16 + 0x0090,
    //0x70 * 16 + 0x1276,
    //0x027c * 16 + 0x184c,
    //0x286 * 16 + 0x187a,
    //0x9F51 * 16 + 0x0196,
    //0x70 * 16 + 0x321A, // MS-DOS/v4.0/src/BIOS/MSINIT.ASM PUBLIC  INIT
    // 0x286 * 16 + 0x187a,
    //0x286 * 16 + 0x1828,
    0, // Execution wrapped around
];

var errorLog = (Func<string> expression) => Console.Error.WriteLine(expression());
var consoleLog = (Func<string> expression) => Console.WriteLine(expression());
var noLog = (Func<string> expression) => { };
Action<Func<string>> loggerWithColor(int minLevel, string name, ConsoleColor color, Action<Func<string>> logger)
{
    return expression =>
    {
        if (loglevel < minLevel) return;
        var prev = Console.ForegroundColor;
        var prevbg = Console.BackgroundColor;
        Console.ResetColor();
        Console.ForegroundColor = color;
        logger(() => $"[{name}] " + expression());
        Console.ForegroundColor = prev;
        Console.BackgroundColor = prevbg;
    };
}

var Error = loggerWithColor(1, "ERROR", ConsoleColor.Red, errorLog);
var Warn = loggerWithColor(2, "WARN", ConsoleColor.Yellow, consoleLog);
var Info = loggerWithColor(3, "INFO", ConsoleColor.DarkGray, consoleLog);
var Debug = loggerWithColor(4, "DEBUG", ConsoleColor.Blue, consoleLog);
var Trace = loggerWithColor(5, "TRACE", ConsoleColor.Cyan, consoleLog);

var disks = new string[] {
    "/Users/josh/Downloads/002962_ms_dos_622/disk1.img",
    "/Users/josh/Downloads/Install.img",
    "/Users/josh/Downloads/FD13-FloppyEdition/144m/x86BOOT.img",
    "/Users/josh/Downloads/PCDOS100.img",
    "/Users/josh/Downloads/5150-DIAG-100.img",
    "/Users/josh/Downloads/PCDOS200-C400.img",
    "/Users/josh/Downloads/COMPAQ-DOS211.img",
    "/Users/josh/Downloads/EMPTY-1440K.img",
};
var selectedDisk = 7;

var disk = disks[selectedDisk];
var file = File.OpenRead(disk);
int drive = 0x00,
    sectorsPerTrack = file.Length == 1474560 ? 18 : 9,
    heads = 2,
    tracks = (int)file.Length / sectorsPerTrack / heads / 512;
// int drive = 0x80, sectorsPerTrack = 17, heads = 4, tracks = (int)file.Length / 17 / 4 / 512;

var memSize = 1024 * 1024;
var mem = new Memory(memSize); // 1mb // 640KB
const int vga_cols = 80;
const int vga_rows = 25;
const int vram_size = vga_cols + vga_rows * 2;
var vram = new byte[vram_size];


mem.ClearMaps();
const int vram_addr = 0xB8000;
const int vram_end = vram_size;
// 0000:0000 - B800:0000
mem.NewWindow(
    mem.RawBytes,
    address: 0,
    size: vram_addr);
// B800:0000 - B800:xxxx
mem.NewWindow(
    vram_addr, vram_end, read_addr =>
    {
        return vram[read_addr];
    },
    (write_addr, value) =>
    {
        if ((write_addr & 1) == 0)
        {
            Console.SetCursorPosition(write_addr % (vga_cols * 2), write_addr / (vga_cols * 2));
        }
        else
        {
            Console.ForegroundColor = (ConsoleColor)(value & 0xF);
            Console.BackgroundColor = (ConsoleColor)(value >> 4);
            Console.Write((char)vram[write_addr]);
        }
        vram[write_addr] = value;
    });
// B800:xxxx - 1_0000:0000
mem.NewWindow(
    mem.RawBytes,
    address: vram_end,
    offset: vram_end);

var cpu = new CPU(mem);
cpu.InfoLogger = Info;
cpu.WarnLogger = Warn;
cpu.ErrorLogger = Error;
cpu.TraceLogger = Trace;

void MemoryWindow(ushort seg, ushort addr)
{
    var sb = new StringBuilder();
    for (var i = 0; i < 16; i++)
    {
        var current = seg * 16 + addr + i;
        if (current < mem.Size)
        {
            sb.AppendFormat("{0:X2} ", mem[current]);
        }
        else
        {
            sb.Append("   ");
        }
    }
    for (var i = 0; i < 16; i++)
    {
        var current = seg * 16 + addr + i;
        if (current < mem.Size)
        {
            var c = (char)mem[current];
            var ch = c >= 32 && c < 127 ? c.ToString() : ".";
            sb.AppendFormat(" {0}", ch);
        }
        else
        {
            sb.Append("  ");
        }
    }
    Console.WriteLine(sb.ToString());
}

bool stop = false;
Console.CancelKeyPress += (o, args) =>
{
    if ((args.SpecialKey & ConsoleSpecialKey.ControlC) != 0)
    {
        args.Cancel = true;
        stop = true;
    }
};

void ReturnFlag(Flags flag, bool set, CPU cpu)
{
    var flags_addr_on_stack = cpu.SS * 16 + cpu.SP + 4;
    var original_flags = cpu.Memory.wordAt(flags_addr_on_stack);
    var updated_flags = original_flags;
    if (set)
    {
        updated_flags |= (ushort)flag;
    }
    else
    {
        updated_flags &= (ushort)~flag;
    }
    cpu.Memory.setWordAt(flags_addr_on_stack, updated_flags);
}

// Interrupts
cpu.HookInterrupt(0x00, cpu =>
{
    // Divide by 0
    Trace(() => "Divide by zero called");

    // Does DOS multiplex this function?
    // Just do what BOCHS does
    if (cpu.AH == 0xC0)
    {
        cpu.SetReg8(Register.AH, 0);
        cpu.SetSeg(Register.ES, 0x0040);
        cpu.SetReg16(Register.BX, 0x000e);
        ReturnFlag(Flags.Carry, false, cpu);
    }
});
cpu.HookInterrupt(0x01, cpu =>
{
    // Debug
    Debug(() => "Debug trap");
    Error(() => "Int 01h, likely panic");
    Environment.Exit(1);
    if (Debugger.IsAttached)
    {
        Debugger.Break();
    }
});
// Timer
var time_on = DateTime.Now;
cpu.HookInterrupt(0x08, cpu =>
{
    var prevTicks = (uint)(cpu.Memory.wordAt(0x046C) | (cpu.Memory.wordAt(0x046E) << 16));
    var elapsed = DateTime.Now.Subtract(time_on);
    var ticks = (uint)(elapsed.TotalMilliseconds / 18.21);
    var count = ticks % 1573040;
    Trace(() => $"Timer tick {count}");
    cpu.Memory.setWordAt(0x046C, (ushort)count);
    cpu.Memory.setWordAt(0x046E, (ushort)(count >> 16));
    cpu.Memory[0x0470] = (byte)(elapsed.TotalDays > 1.0 ? 1 : 0);
    // chain to 0x1c
    if (ticks > prevTicks)
    {
        var seg = cpu.Memory.wordAt(0, 0x1C * 4 + 2);
        var off = cpu.Memory.wordAt(0, 0x1C * 4);
        Trace(() => $"INT 1Ch vector: {seg:X4}:{off:X4}");
        cpu.Jump(seg, off);
    }
});
cpu.HookInterrupt(0x10, cpu =>
{
    switch (cpu.AH)
    {
        case 0x0E:
            {
                // Teletype output
                var character = cpu.AL;
                var page = cpu.BH;
                var color = cpu.BL;

                Console.ForegroundColor = (ConsoleColor)(color & 0xF);
                Console.BackgroundColor = (ConsoleColor)(color >> 4);

                Console.Write((char)character);
                break;
            }
        case 0x0A:
            {
                var character = cpu.AL;
                var page = cpu.BH;
                var count = cpu.CX;

                for (var i = 0; i < count; i++)
                {
                    Console.Write((char)character);
                }

                break;
            }
        default:
            Warn(() => $"int 10h, ah={cpu.AH:X2}h video services called, not implemented.");
            if (Debugger.IsAttached)
            {
                // Int 10h video services AH=??
                Debugger.Break();
            }
            ReturnFlag(Flags.Carry, false, cpu);
            break;
    }
});
var equipmentList = new EquipmentList();
cpu.Memory.setWordAt(0x410, equipmentList.ToBitField());
cpu.HookInterrupt(0x11, cpu =>
{
    cpu.SetReg16(Register.AX, equipmentList.ToBitField());
});
// http://vitaly_filatov.tripod.com/ng/asm/asm_001.10.html
cpu.HookInterrupt(0x12, cpu =>
{
    // Get usable [conventional] memory size
    var size = Math.Min(640, cpu.Memory.Size - 1);
    cpu.SetReg16(Register.AX, (ushort)(size - 1)); // Bochs says 639
});
int CalculateLBA(ushort cylinder, byte head, int sector)
{
    return (cylinder * heads + head) * sectorsPerTrack + (sector - 1);
}
bool ReadSectors(byte drive, ushort cylinder, byte head, byte sector, byte count, ushort segment, ushort offset, out byte status)
{
    var baseLBA = CalculateLBA(cylinder, head, sector);
    // Console.WriteLine($"LBA: 0x{baseLBA}");
    status = 0x00; // success

    if (count < 1)
    {
        status = 1;
        return false;
    }

    for (int i = 0; i < count; i++)
    {
        int lba = baseLBA + i;
        long byteOffset = lba * 512L;

        if (byteOffset + 512 > file.Length)
        {
            status = 0x01; // etc.
            return false;
        }

        file.Seek(byteOffset, SeekOrigin.Begin);
        byte[] buffer = new byte[512];
        file.Read(buffer, 0, 512);

        uint linearAddress = (uint)((segment << 4) + offset + i * 512);
        for (var j = 0; j < 512; j++)
        {
            cpu.Memory[(int)(linearAddress + j)] = buffer[j];
        }
    }

    // for (var i = offset; i < offset + 512 * count; i += 16)
    // {
    //     Console.Write($"{segment:X4}:{i:X4}       ");
    //     MemoryWindow(segment, i);
    // }
    // Console.WriteLine();

    return true;
}
// https://en.wikipedia.org/wiki/INT_13H
cpu.HookInterrupt(0x13, cpu =>
{
    switch (cpu.AH)
    {
        case 0x00: // Reset Disk System
            cpu.CF = false;
            break;
        case 0x02: // Read
            var success = false;
            if (drive == cpu.DL)
            {
                success = ReadSectors(
                    drive: cpu.DL,
                    cylinder: (ushort)(cpu.CX >> 8),
                    head: (byte)(cpu.DX >> 8),
                    sector: (byte)(cpu.CX & 0x3F),
                    count: cpu.AL,
                    segment: cpu.ES,
                    offset: cpu.BX,
                    out byte status
                );

                string ascii(byte b)
                {
                    return b >= 0x20 && b < 128 ? ((char)b).ToString() : $"\\x{b:x2}";
                }
                string asciis(int c)
                {
                    return string.Join("", Enumerable.Range(0, c).Select(i => ascii(cpu.Memory[cpu.ES * 16 + cpu.BX + i]).ToString()));
                }
                string bytes(int c)
                {
                    return string.Join(' ', Enumerable.Range(0, c).Select(i => $"{cpu.Memory[cpu.ES * 16 + cpu.BX + i]:X2}"));
                }
                int num = 16;
                Info(() => $"Loading {cpu.AL} sectors to {cpu.ES:X4}:{cpu.BX:X4} (cx={cpu.CX:X2} dx={cpu.DX:X2} \"{asciis(num)}\"... [{bytes(num)}])");
                cpu.SetReg16(Register.AX, (ushort)((status << 8) | cpu.AL));

                ReturnFlag(Flags.Carry, !success, cpu);
            }
            break;
        case 0x08:
            cpu.SetReg8(Register.AH, 0x00); // No error
            cpu.SetReg8(Register.AL, 1); // Drives present
            cpu.SetReg8(Register.CH, (byte)tracks);   // Cylinders
            cpu.SetReg8(Register.CL, (byte)sectorsPerTrack);   // Sectors per track
            cpu.SetReg8(Register.DH, (byte)heads);    // Heads
            cpu.SetReg8(Register.DL, (byte)drive); // Hard drive
            ReturnFlag(Flags.Carry, false, cpu);
            break;
        default:
            Warn(() => $"int 13h, ah={cpu.AH:X2}h disk services called, not implemented.");
            // throw new NotImplementedException();
            if (Debugger.IsAttached)
            {
                // Int 13h disk services AH=??
                Debugger.Break();
            }
            ReturnFlag(Flags.Carry, false, cpu);
            break;
    }
});
cpu.HookInterrupt(0x14, cpu =>
{
    switch (cpu.AH)
    {
        case 0x00: // Initialize serial port
            Trace(() => $"[INT 14h] Init COM{cpu.DX + 1}: config=0x{cpu.AL:X2}");

            cpu.SetReg8(Register.AH, 0x00); // No error
            cpu.SetReg8(Register.AL, 0x03); // Line status: Transmitter ready, empty
            ReturnFlag(Flags.Carry, false, cpu);
            break;

        default:
            Warn(() => $"[INT 14h] Unsupported function AH={cpu.AH:X2}");
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            ReturnFlag(Flags.Carry, false, cpu);
            break;
    }
});
cpu.HookInterrupt(0x15, cpu =>
{
    // Advanced BIOS services?
    Trace(() => $"Int 0x15 AH={cpu.AH} called");
    ReturnFlag(Flags.Carry, true, cpu);
});
// https://stanislavs.org/helppc/scan_codes.html
byte[] LetterScanCodes = [0x1e, 0x30, 0x2e, 0x20, 0x12, 0x21, 0x22, 0x23, 0x17, 0x24, 0x25, 0x26, 0x32, 0x31, 0x18, 0x19, 0x10, 0x13, 0x1f, 0x14, 0x16, 0x2f, 0x11, 0x2D, 0x15, 0x2c];
byte[] FunctionScanCodes = [0x3b, 0x3c, 0x3d, 0x3e, 0x3f, 0x40, 0x41, 0x42, 0x43, 0x44, 0x85, 0x86];
void MapConsoleKeyInfoToScanCode(ConsoleKeyInfo info, out byte scancode, out byte ascii)
{
    scancode = 0;
    ascii = 0;
    if (info.Key >= ConsoleKey.A && info.Key <= ConsoleKey.Z)
    {
        var i = info.Key - ConsoleKey.A;
        scancode = LetterScanCodes[i];
        if (info.Modifiers.HasFlag(ConsoleModifiers.Control))
        {
            ascii = (byte)(1 + i);
        }
        else if (info.Modifiers.HasFlag(ConsoleModifiers.Alt))
        {
            ascii = 0;
        }
        else if (info.Modifiers.HasFlag(ConsoleModifiers.Shift))
        {
            ascii = (byte)('A' + i);
        }
        else
        {
            ascii = (byte)('a' + i);
        }
    }
    else if (info.Key >= ConsoleKey.D0 && info.Key <= ConsoleKey.D9)
    {
        var index = info.Key == ConsoleKey.D0 ? 0 : 1 + (info.Key - ConsoleKey.D1);
        if (info.Modifiers.HasFlag(ConsoleModifiers.Alt))
        {
            scancode = (byte)(info.Key == ConsoleKey.D0
                ? 0x81 : 0x78 + (info.Key - ConsoleKey.D1));
        }
        else if (info.Modifiers == ConsoleModifiers.Control)
        {
            scancode = info.Key switch
            {
                ConsoleKey.D2 => 0x03,
                ConsoleKey.D6 => 0x07,
                _ => 0
            };
            ascii = info.Key switch
            {
                ConsoleKey.D2 => 0x00,
                ConsoleKey.D6 => 0x1E,
                _ => 0
            };
        }
        else
        {
            scancode = (byte)(info.Key == ConsoleKey.D0
                ? 0x0B : 0x02 + (info.Key - ConsoleKey.D1));

            if (info.Modifiers.HasFlag(ConsoleModifiers.Shift))
            {
                ascii = (byte)"!@#$%^&*()"[index];
            }
            else
            {
                ascii = (byte)('0' + index);
            }
        }
    }
    else if (info.Key >= ConsoleKey.F1 && info.Key <= ConsoleKey.F10)
    {
        var index = (int)(info.Key - ConsoleKey.F1);
        var start = 0x3B;
        if (info.Modifiers.HasFlag(ConsoleModifiers.Alt))
        {
            start = 0x68;
        }
        else if (info.Modifiers.HasFlag(ConsoleModifiers.Control))
        {
            start = 0x5E;
        }
        else if (info.Modifiers.HasFlag(ConsoleModifiers.Shift))
        {
            start = 0x54;
        }
        scancode = (byte)(start + index);
    }
    else
    {
        if (info.Modifiers.HasFlag(ConsoleModifiers.Alt))
        {
            scancode = info.Key switch
            {
                ConsoleKey.F11 => 0x8B,
                ConsoleKey.F12 => 0x8C,
                ConsoleKey.Backspace => 0x0E,
                ConsoleKey.Delete => 0xA3,
                ConsoleKey.DownArrow => 0xA0,
                ConsoleKey.End => 0x9F,
                ConsoleKey.Enter => 0xA6,
                ConsoleKey.Escape => 0x01,
                ConsoleKey.Home => 0x97,
                ConsoleKey.Insert => 0xA2,
                ConsoleKey.NumPad5 => 0,
                ConsoleKey.Multiply => 0x37,
                ConsoleKey.Subtract => 0x4A,
                ConsoleKey.Add => 0x4E,
                ConsoleKey.Divide => 0xA4,
                ConsoleKey.LeftArrow => 0x9B,
                ConsoleKey.PageDown => 0xA1,
                ConsoleKey.PageUp => 0x99,
                ConsoleKey.PrintScreen => 0,
                ConsoleKey.RightArrow => 0x9D,
                ConsoleKey.Spacebar => 0x39,
                ConsoleKey.Tab => 0xA5,
                ConsoleKey.UpArrow => 0x98,
                _ => (byte)info.Key
            };
            ascii = info.Key switch
            {
                ConsoleKey.Spacebar => 0x20,
                _ => 0
            };
        }
        else if (info.Modifiers.HasFlag(ConsoleModifiers.Control))
        {
            scancode = info.Key switch
            {
                ConsoleKey.F11 => 0x89,
                ConsoleKey.F12 => 0x8A,
                ConsoleKey.Backspace => 0x0E,
                ConsoleKey.Delete => 0x93,
                ConsoleKey.DownArrow => 0x91,
                ConsoleKey.End => 0x75,
                ConsoleKey.Enter => 0x1C,
                ConsoleKey.Escape => 0x01,
                ConsoleKey.Home => 0x77,
                ConsoleKey.Insert => 0x92,
                ConsoleKey.NumPad5 => 0x8F,
                ConsoleKey.Multiply => 0x96,
                ConsoleKey.Subtract => 0x8E,
                ConsoleKey.Add => 0,
                ConsoleKey.Divide => 0x95,
                ConsoleKey.LeftArrow => 0x73,
                ConsoleKey.PageDown => 0x76,
                ConsoleKey.PageUp => 0x84,
                ConsoleKey.PrintScreen => 0x72,
                ConsoleKey.RightArrow => 0x74,
                ConsoleKey.Spacebar => 0x39,
                ConsoleKey.Tab => 0x94,
                ConsoleKey.UpArrow => 0x8D,
                _ => (byte)info.Key
            };
            ascii = info.Key switch
            {
                ConsoleKey.Backspace => 0x7F,
                ConsoleKey.Enter => 0x0A,
                ConsoleKey.Escape => 0x1B,
                ConsoleKey.Spacebar => 0x20,
                _ => 0
            };
        }
        else
        {
            switch (info.Key)
            {
                case ConsoleKey.F11:
                    scancode = (byte)(info.Modifiers.HasFlag(ConsoleModifiers.Shift)
                        ? 0x87 : 0x85);
                    break;
                case ConsoleKey.F12:
                    scancode = (byte)(info.Modifiers.HasFlag(ConsoleModifiers.Shift)
                        ? 0x88 : 0x86);
                    break;
                case ConsoleKey.NumPad5:
                    scancode = (byte)(info.Modifiers.HasFlag(ConsoleModifiers.Shift)
                        ? 0x4C : 0);
                    break;
                case ConsoleKey.Multiply:
                    scancode = (byte)(info.Modifiers.HasFlag(ConsoleModifiers.Shift)
                        ? 0 : 0x37);
                    break;
                default:
                    scancode = info.Key switch
                    {
                        ConsoleKey.Backspace => 0x0E,
                        ConsoleKey.Delete => 0x53,
                        ConsoleKey.DownArrow => 0x50,
                        ConsoleKey.End => 0x4F,
                        ConsoleKey.Enter => 0x1C,
                        ConsoleKey.Escape => 0x01,
                        ConsoleKey.Home => 0x47,
                        ConsoleKey.Insert => 0x52,
                        ConsoleKey.Subtract => 0x4A,
                        ConsoleKey.Add => 0x4E,
                        ConsoleKey.Divide => 0x35,
                        ConsoleKey.LeftArrow => 0x4B,
                        ConsoleKey.PageDown => 0x51,
                        ConsoleKey.PageUp => 0x49,
                        ConsoleKey.RightArrow => 0x4D,
                        ConsoleKey.Spacebar => 0x39,
                        ConsoleKey.Tab => 0x0F,
                        ConsoleKey.UpArrow => 0x48,
                        _ => (byte)info.Key
                    };
                    if (info.Modifiers.HasFlag(ConsoleModifiers.Shift))
                    {
                        ascii = info.Key switch
                        {
                            ConsoleKey.Backspace => 0x08,
                            ConsoleKey.Delete => 0x2E,
                            ConsoleKey.DownArrow => 0x32,
                            ConsoleKey.End => 0x31,
                            ConsoleKey.Enter => 0x0D,
                            ConsoleKey.Escape => 0x1B,
                            ConsoleKey.Home => 0x37,
                            ConsoleKey.Insert => 0x30,
                            ConsoleKey.NumPad5 => 0x35,
                            ConsoleKey.Multiply => 0,
                            ConsoleKey.Subtract => 0x2D,
                            ConsoleKey.Add => 0x2B,
                            ConsoleKey.Divide => 0x2F,
                            ConsoleKey.LeftArrow => 0x34,
                            ConsoleKey.PageDown => 0x33,
                            ConsoleKey.PageUp => 0x39,
                            ConsoleKey.PrintScreen => 0,
                            ConsoleKey.RightArrow => 0x36,
                            ConsoleKey.Spacebar => 0x20,
                            ConsoleKey.Tab => 0x00,
                            ConsoleKey.UpArrow => 0x38,
                            _ => (byte)(info.KeyChar > 255 ? 0 : info.KeyChar)
                        };
                    }
                    else
                    {
                        ascii = info.Key switch
                        {
                            ConsoleKey.Backspace => 0x08,
                            ConsoleKey.Delete => 0,
                            ConsoleKey.DownArrow => 0,
                            ConsoleKey.End => 0,
                            ConsoleKey.Enter => 0x0D,
                            ConsoleKey.Escape => 0x1B,
                            ConsoleKey.Home => 0,
                            ConsoleKey.Insert => 0,
                            ConsoleKey.NumPad5 => 0,
                            ConsoleKey.Multiply => 0x2A,
                            ConsoleKey.Subtract => 0x2D,
                            ConsoleKey.Add => 0x2B,
                            ConsoleKey.Divide => 0x2F,
                            ConsoleKey.LeftArrow => 0,
                            ConsoleKey.PageDown => 0,
                            ConsoleKey.PageUp => 0,
                            ConsoleKey.PrintScreen => 0,
                            ConsoleKey.RightArrow => 0,
                            ConsoleKey.Spacebar => 0x20,
                            ConsoleKey.Tab => 0x09,
                            ConsoleKey.UpArrow => 0,
                            _ => (byte)(info.KeyChar > 255 ? 0 : info.KeyChar)
                        };
                    }
                    break;
            }
        }
    }
}
byte MapASCIIToScanCode(char c)
{
    c = char.ToUpperInvariant(c);
    if (c >= 'A' && c <= 'Z')
    {
        return LetterScanCodes[c - 'A'];
    }
    if (c >= '0' && c <= '9')
    {
        return (byte)(c == '0' ? 0x30 : 0x31 + (c - '1'));
    }
    switch (c)
    {
        case '-':
        case '_':
            return 0x0C;
        case '=':
        case '+':
            return 0x0D;
        case '[':
        case '{':
            return 0x1A;
        case ']':
        case '}':
            return 0x1B;
        case ';':
        case ':':
            return 0x27;
        case '\'':
        case '"':
            return 0x28;
        case '`':
        case '~':
            return 0x29;
        case '\\':
        case '|':
            return 0x2B;
        case ',':
        case '<':
            return 0x33;
        case '.':
        case '>':
            return 0x34;
        case '/':
        case '?':
            return 0x35;
        case '!':
        case '@':
        case '#':
        case '$':
        case '%':
        case '&':
        case '*':
        case '(':
        case ')':
            return (byte)(0x02 + "!@#$%^&*()".IndexOf(c));
        default:
            return 0;
    }
}
cpu.HookInterrupt(0x16, cpu =>
{
    switch (cpu.AH)
    {
        case 0x00:
            if (Console.IsInputRedirected)
            {
                var c = Console.Read();
                if (c == -1)
                {
                    c = 0;
                }
                var scancode = MapASCIIToScanCode((char)c);
                cpu.SetReg8(Register.AL, (byte)c);
                cpu.SetReg8(Register.AH, scancode);
            }
            else
            {
                var keyInfo = Console.ReadKey(true);
                MapConsoleKeyInfoToScanCode(keyInfo, out byte scancode, out byte ascii);
                cpu.SetReg8(Register.AL, ascii);
                cpu.SetReg8(Register.AH, scancode);
            }
            ReturnFlag(Flags.Carry, true, cpu);
            break;
        case 0x01:
            // https://stanislavs.org/helppc/int_16-1.html
            cpu.SetReg16(Register.AX, 0); // No scan code available
            ReturnFlag(Flags.Zero, true, cpu); // No key is pressed
            break;
        default:
            Warn(() => $"Int 16h unsupported function {cpu.AH:X2}h");
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            ReturnFlag(Flags.Carry, true, cpu);
            break;
    }
});
// LPT
cpu.HookInterrupt(0x17, cpu =>
{
    var ah = cpu.GetReg8(Register.AH);
    var al = cpu.GetReg8(Register.AL);
    var dx = cpu.GetReg(Register.DX, true); // full 16-bit DX = printer port number (0 = LPT1)

    void SetStatus(byte status, bool error = false)
    {
        cpu.SetReg8(Register.AH, status); // status in AH
        ReturnFlag(Flags.Carry, error, cpu); // CF = error?
    }

    switch (ah)
    {
        case 0x00: // Initialize printer
            Trace(() => $"[INT 17h] Init LPT{dx + 1}");
            SetStatus(0x18); // ready, selected, ack
            break;

        case 0x01: // Send character
            Trace(() => $"[INT 17h] LPT{dx + 1}: PRINT '{(char)al}' (0x{al:X2})");
            SetStatus(0x18);
            break;

        case 0x02: // Get printer status
            Trace(() => $"[INT 17h] Status for LPT{dx + 1}");
            SetStatus(0x18);
            break;

        default:
            Warn(() => $"[INT 17h] Unsupported AH={ah:X2}");
            SetStatus(0x80, error: true); // error bit set
            break;
    }
});
void boot()
{
    // Console.SetBufferSize(80, 25);
    Console.BackgroundColor = ConsoleColor.Black;
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.Clear();
    // Console.WriteLine("No boot device");
    var bootsect = new byte[512];
    file.Seek(0, SeekOrigin.Begin);
    file.Read(bootsect, 0, 512);
    // Load the boot sector into memory.
    for (var i = 0; i < 512; i++)
    {
        mem[0x7C00 + i] = bootsect[i];
    }
    // Set the drive number
    cpu.SetReg8(Register.DL, (byte)drive);
}
cpu.HookInterrupt(0x19, cpu =>
{
    // Reboot
    boot();
});
boot();
cpu.HookInterrupt(0x1A, (cpu) =>
{
    var t = DateTime.Now;
    byte bcd(int i)
    {
        var tens = i / 10;
        var ones = i % 10;
        return (byte)(ones | (tens << 4));
    }
    switch (cpu.AH)
    {
        case 0x00:
            // http://vitaly_filatov.tripod.com/ng/asm/asm_029.1.html
            var elapsed = DateTime.Now.Subtract(time_on);
            var count = (int)(elapsed.TotalSeconds * 1000 / 18.21) % 1573040;
            cpu.SetReg16(Register.CX, (ushort)(count >> 16));
            cpu.SetReg16(Register.DX, (ushort)count);
            cpu.SetReg8(Register.AL, (byte)(elapsed.TotalHours >= 24 ? 0 : 1));
            ReturnFlag(Flags.Carry, false, cpu);
            break;
        case 0x02:
            // https://forum.osdev.org/viewtopic.php?t=29849
            // http://vitaly_filatov.tripod.com/ng/asm/asm_029.3.html
            // CH = hour (BCD)
            // CL = minutes (BCD)
            // DH = seconds (BCD)
            cpu.SetReg8(Register.CH, bcd(t.Hour));
            cpu.SetReg8(Register.CL, bcd(t.Minute));
            cpu.SetReg8(Register.DH, bcd(t.Second));
            cpu.SetReg8(Register.DL, (byte)(t.IsDaylightSavingTime() ? 1 : 0));
            ReturnFlag(Flags.Carry, false, cpu);
            break;
        case 0x04:
            // http://vitaly_filatov.tripod.com/ng/asm/asm_029.5.html
            cpu.SetReg8(Register.CH, (byte)(t.Year < 2000 ? 0x19 : 0x20));
            cpu.SetReg8(Register.CL, bcd(t.Year % 100));
            cpu.SetReg8(Register.DH, bcd(t.Month));
            cpu.SetReg8(Register.DL, bcd(t.Day));
            ReturnFlag(Flags.Carry, false, cpu);
            break;
        default:
            Warn(() => $"Unimplemented int 1Ah ah={cpu.AH:X2}h");
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            ReturnFlag(Flags.Carry, true, cpu);
            break;
    }
});

void SetupSoftInterrupt(byte i)
{
    cpu.Memory.setWordAt(i * 4, (ushort)(i * 4)); // Offset
    cpu.Memory.setWordAt(i * 4 + 2, (ushort)0xF000); // Segment

    cpu.Memory[0xF0000 + i * 4] = 0xCF; // iret
    cpu.Memory[0xF0000 + i * 4 + 1] = 0x90; // nop
    cpu.Memory[0xF0000 + i * 4 + 2] = 0x90; // nop
    cpu.Memory[0xF0000 + i * 4 + 3] = 0x90; // nop

    // cpu.Memory[0xF0000 + i * 4] = 0xCD; // int
    // cpu.Memory[0xF0000 + i * 4 + 1] = i; // i
    // cpu.Memory[0xF0000 + i * 4 + 2] = 0xCF; // iret
    // cpu.Memory[0xF0000 + i * 4 + 3] = 0x90; // nop
}

for (var i = 0; i < 256; i++)
{
    SetupSoftInterrupt((byte)i);
}

// Set up the diskette parameter table
// http://www.techhelpmanual.com/256-int_1eh__diskette_parameter_pointer.html
// Do what BOCHS does
ushort dptSeg = 0xf000;
ushort dptOff = 0xefde;
ushort dptLoc = 0x0078;
int dptPtr = dptSeg * 16 + dptOff;
// First, store a pointer in the IVT (int 1Eh, the 30th entry)
cpu.Memory.setWordAt(dptLoc, dptOff);
cpu.Memory.setWordAt(dptLoc + 2, dptSeg);
// rSrtHdUnld
var SRTStepRateTime = 0xF;
var headUnloadTime = 10;
cpu.Memory[dptPtr] = (byte)(SRTStepRateTime | (headUnloadTime << 4));
// rDmaHdLd
byte useDma = 0;
byte headLoadTime = 1;
cpu.Memory[dptPtr + 1] = (byte)(useDma | (headLoadTime << 1));
// bMotorOff, 55-ms increments before turning disk motor off
cpu.Memory[dptPtr + 2] = 37;
// bSectSize (0=128, 1=256, 2=512, 3=1024)
cpu.Memory[dptPtr + 3] = 2;
// bLastTrack EOT (last sector on track)
cpu.Memory[dptPtr + 4] = 18;
// bGapLen
cpu.Memory[dptPtr + 5] = 27;
// bDTL Data Transfer Length max when length not set
cpu.Memory[dptPtr + 6] = 0xFF;
// bGapFmt Gap length for format operation
cpu.Memory[dptPtr + 7] = 108;
// bFillChar fill character for format (normally f6, '÷')
cpu.Memory[dptPtr + 8] = 0xF6;
// bHdSettle Head settle time in ms
cpu.Memory[dptPtr + 9] = 15;
// bMotorOn motor startup time in 1/8 sec. intervals
cpu.Memory[dptPtr + 10] = 8;

cpu.HookInPort(0x21, (cpu, port) =>
{
    // LPT/Printer
    Trace(() => $"LPT IN 0x21");
    return 0xFF; // Everything masked (safe default)
});

cpu.HookInPort(0x50, (cpu, port) =>
{
    Trace(() => $"Port 50h IN");
    return 0xFF; // or 0x00 — whatever makes code happy
});

cpu.HookInPort(0x66, (cpu, port) =>
{
    // A20?
    Trace(() => $"Port 66h IN");
    return 0xFF;
});

cpu.HookOutPort(0x20, (cpu, port, data) =>
{
    // EOI or command to PIC
    Trace(() => $"[I/O] PIC/EOI OUT 0x20, AL={data:X2}");

});

// cpu.HookOutPort(0x21, (cpu, port, data) =>
// {
//     // Data port (IRQ mask register)
//     Console.WriteLine($"[I/O] OUT 0x21, AL={data:X2}");

// });

cpu.HookOutPort(0x50, (cpu, port, data) =>
{
    Trace(() => $"OUT 0x50, AL={data:X2}");

});

cpu.Jump(0x0000, 0x7C00);

string DecodeInstruction()
{
    var ins = cpu.NextInstruction;

    try
    {
        var ip = cpu.CS * 16 + cpu.IP;
        var decoder = new DecodedInstruction(cpu.Memory.RawBytes, ref ip);
        return decoder.ToString();
    }
    catch
    {
        return ins.ToString().ToLower();
    }
}
var k = 0;
var last = 0;
var visited = new HashSet<ulong>();
var instructions_skipped = 0;
var instruction_gap = 1;
var nextInstruction = DecodeInstruction();
var last_tick = DateTime.Now;
while (!stop)
{
    if (cpu.CS * 16 + cpu.IP == 0)
    {
        Environment.Exit(1);
    }
    var in_breakpoint = false;
    if (breakpoints_enabled && break_addrs.Contains(cpu.CS * 16 + cpu.IP))
    {
        prompting = true;
        in_breakpoint = true;
    }
    if (!prompting && !in_breakpoint)
    {
        var time = DateTime.Now;
        if ((time - last_tick).TotalMilliseconds >= 18.21 && cpu.flags.HasFlag(Flags.InterruptEnable) && !Debugger.IsAttached)
        {
            //cpu.ClearFlag(Flags.InterruptEnable);
            last_tick = time;
            cpu.Push((short)cpu.flags);
            cpu.Push((short)cpu.CS);
            cpu.Push((short)cpu.IP);
            cpu.Jump(cpu.Memory.wordAt(0, 0x08 * 4 + 2), cpu.Memory.wordAt(0, 0x08 * 4));
        }
        else
        {
            if (cpu.Halted)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(18.21).Subtract(time - last_tick));
            }
        }
        if (loglevel >= 4)// && !Debugger.IsAttached)
        {
            var ip = cpu.CS * 16 + cpu.IP;
            var key = (uint)(cpu.IP | (cpu.CS << 16)) | (ulong)cpu.Memory[ip] << 32;
            if (ip != last) // Avoid repetition
            {
                if (visited.Contains(key))
                {
                    //Console.Write("%");
                    instructions_skipped++;
                }
                else
                {
                    if (instructions_skipped != 0)
                    {
                        Trace(() => $"Repeated {instructions_skipped} instructions");
                        instructions_skipped = 0;
                        instruction_gap = 1;
                    }
                    nextInstruction = DecodeInstruction();
                    //Trace(() => $"{cpu.CS:X4}:{cpu.IP:X4} a:{cpu.AX:X4} b:{cpu.BX:X4} c:{cpu.CX:X4} d:{cpu.DX:X4} s:{cpu.SI:X4} d:{cpu.DI:X4} {cpu.CS:X4}:{cpu.IP:X4} " + nextInstruction);
                    Trace(() => $"{cpu.CS:X4}:{cpu.IP:X4} " + nextInstruction);
                    visited.Add(key);
                }
            }
            last = ip;
        }
        cpu.WakeHandle.WaitOne(); // In case of HLT
        cpu.Clock();
        continue;
    }

    // CS:IP Window
    Console.WriteLine();
    Console.Write($"CS:IP  {cpu.CS:X4}:{cpu.IP:X4}    ");
    MemoryWindow(cpu.CS, cpu.IP);

    Console.Write($"SS:SP  {cpu.SS:X4}:{cpu.SP:X4}    ");
    MemoryWindow(cpu.SS, cpu.SP);

    Console.Write($"DS:SI  {cpu.DS:X4}:{cpu.SI:X4}    ");
    MemoryWindow(cpu.DS, cpu.SI);

    Console.Write($"ES:DI  {cpu.ES:X4}:{cpu.DI:X4}    ");
    MemoryWindow(cpu.ES, cpu.DI);

    Console.WriteLine($"AX: {cpu.AX:X4}  CX: {cpu.CX:X4}  BP: {cpu.BP:X4}  ODITSZAPC");
    Console.Write($"BX: {cpu.BX:X4}  DX: {cpu.DX:X4}            ");
    Console.Write((cpu.flags & Emulate8086.Flags.OF) != 0 ? "1" : " ");
    Console.Write((cpu.flags & Emulate8086.Flags.DF) != 0 ? "1" : " ");
    Console.Write((cpu.flags & Emulate8086.Flags.IF) != 0 ? "1" : " ");
    Console.Write((cpu.flags & Emulate8086.Flags.TF) != 0 ? "1" : " ");
    Console.Write((cpu.flags & Emulate8086.Flags.SF) != 0 ? "1" : " ");
    Console.Write((cpu.flags & Emulate8086.Flags.ZF) != 0 ? "1" : " ");
    Console.Write((cpu.flags & Emulate8086.Flags.AF) != 0 ? "1" : " ");
    Console.Write((cpu.flags & Emulate8086.Flags.PF) != 0 ? "1" : " ");
    Console.Write((cpu.flags & Emulate8086.Flags.CF) != 0 ? "1" : " ");
    Console.WriteLine();
    Console.WriteLine($": {DecodeInstruction()}");

    // Prompt
    var command = "";
    if (!in_breakpoint || !break_with_debugger)
    {
        Console.Write("> ");
        command = Console.ReadLine();
    }
    switch (command.Split()[0])
    {
        case "exit":
            stop = true;
            break;
        case "c":
            prompting = false;
            cpu.Clock();
            continue;
        case "x":
            try
            {
                ushort seg = 0;
                uint offs = 0;
                var part2 = command.Split()[1];
                uint FromHex(string str)
                {
                    if (str.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                    {
                        str = str.Substring(2);
                    }
                    else if (str.EndsWith("h", StringComparison.InvariantCultureIgnoreCase))
                    {
                        str = str.Remove(str.Length - 1);
                    }
                    return uint.Parse(str, System.Globalization.NumberStyles.HexNumber);
                }
                if (part2.Contains(':'))
                {
                    var addrs = part2.Split(':');
                    seg = (ushort)FromHex(addrs[0]);
                    offs = (ushort)FromHex(addrs[1]);
                }
                else
                {
                    offs = FromHex(part2);
                    if (offs > 0xFFFF)
                    {
                        seg = (ushort)((offs << 12) & 0xF000);
                        offs &= 0xFFFF;
                    }
                }
                for (var i = 0; i < 256; i += 16, offs += 16)
                {

                    Console.Write($"       {seg:X4}:{offs:X4}    ");
                    MemoryWindow(seg, (ushort)offs);
                }
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            break;
        case "s":
        case "":
            if (command == "s") prompting = true;
            cpu.Clock(in_breakpoint && break_with_debugger);
            in_breakpoint = false;
            // TODO: Disassembly
            Console.WriteLine($"Executed {nextInstruction}");
            nextInstruction = DecodeInstruction();
            break;
    }
}
struct EquipmentList
{
    // http://vitaly_filatov.tripod.com/ng/asm/asm_001.9.html
    // F E D C B A 9 8  7 6 5 4 3 2 1 0
    // x x . . . . . .  . . . . . . . .  Number of printers installed
    // . . x . . . . .  . . . . . . . .  Internal modem installed
    // . . . x . . . .  . . . . . . . .  Game adapter installed? (always 1 on PCJr)
    // . . . . x x x .  . . . . . . . .  Number of RS-232 ports
    // . . . . . . . x  . . . . . . . .  Reserved
    // . . . . . . . .  x x . . . . . .  Number of diskettes - 1 (i.e. 0=1 disk)
    // . . . . . . . .  . . x x . . . .  Initial video mode (see below)
    // . . . . . . . .  . . . . x . . .  Reserved
    // . . . . . . . .  . . . . . x . .  Reserved
    // . . . . . . . .  . . . . . . x .  Math coprocessor installed?
    // . . . . . . . .  . . . . . . . x  1=diskettes present; 0=no disks present
    //
    //                         Initial video mode
    //
    //                 Value                 Meaning
    //                     00                   Reserved
    //                     01                   40 x 25 Color
    //                     10                   80 x 25 Color
    //                     11                   80 x 25 Monochrome
    public enum VideoMode : ushort
    {
        Reserved = 0b00,
        Color40x25 = 0b01,
        Color80x25 = 0b10,
        Monochrome = 0b11,
    }
    public EquipmentList() { }
    private int printerCount;
    public int PrinterCount
    {
        readonly get => printerCount;
        set
        {
            if (value < 0 || value > 3) throw new ArgumentOutOfRangeException(nameof(value));
            printerCount = value;
        }
    }
    public bool IsModemInstalled { readonly get; set; }
    public bool IsGameAdapterInstalled { readonly get; set; }
    private int rs232PortCount;
    public int Rs232PortCount
    {
        readonly get => rs232PortCount;
        set
        {
            if (value < 0 || value > 7) throw new ArgumentOutOfRangeException(nameof(value));
            rs232PortCount = value;
        }
    }
    private int disketteCount = 1;
    public int DisketteCount
    {
        readonly get => disketteCount;
        set
        {
            if (value < 1 || value > 4) throw new ArgumentOutOfRangeException(nameof(value));
            disketteCount = value;
        }
    }
    private VideoMode initialVideoMode = VideoMode.Color80x25;
    public VideoMode InitialVideoMode
    {
        readonly get => initialVideoMode;
        set
        {
            if (value < 0 || value > (VideoMode)3) throw new ArgumentOutOfRangeException(nameof(value));
            initialVideoMode = value;
        }
    }
    public bool IsCoprocessorInstalled { readonly get; set; }
    public bool AreDiskettesPresent { readonly get; set; } = true;
    public readonly ushort ToBitField()
    {
        ushort result = 0;
        if (AreDiskettesPresent)
        {
            result |= 0b1;
        }
        if (IsCoprocessorInstalled)
        {
            result |= 0b10;
        }
        result |= (ushort)((int)initialVideoMode << 4);
        result |= (ushort)((disketteCount - 1) << 6);
        result |= (ushort)(rs232PortCount << 9);
        if (IsGameAdapterInstalled)
        {
            result |= 1 << 0xC;
        }
        if (IsModemInstalled)
        {
            result |= 1 << 0xD;
        }
        result |= (ushort)(printerCount << 0xE);
        return result;
    }
    public static EquipmentList FromBitField(ushort val)
    {
        var areDiskettesPresent = (val & 0b1) != 0;
        var isCoprocessorInstalled = (val & 0b10) != 0;
        var initialVideoMode = (VideoMode)((val >> 4) & 0b11);
        var disketteCount = ((val >> 6) & 0b11) + 1;
        var rs232PortCount = (val >> 9) & 0b111;
        var isGameAdapterInstalled = (val & (1 << 0xC)) != 0;
        var isModemInstalled = (val & (1 << 0xD)) != 0;
        var printerCount = val >> 0xE;
        return new EquipmentList
        {
            AreDiskettesPresent = areDiskettesPresent,
            IsCoprocessorInstalled = isCoprocessorInstalled,
            InitialVideoMode = initialVideoMode,
            DisketteCount = disketteCount,
            Rs232PortCount = rs232PortCount,
            IsGameAdapterInstalled = isGameAdapterInstalled,
            IsModemInstalled = isModemInstalled,
            PrinterCount = printerCount
        };
    }
}