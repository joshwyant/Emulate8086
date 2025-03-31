// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using Emulate8086;
using Emulate8086.Meta.Intel8086;
using Emulate8086.Processor;

bool prompting = false;
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
];

var disk = "/Users/josh/Downloads/002962_ms_dos_622/disk1.img";
var file = File.OpenRead(disk);
int sectorsPerTrack = 18, heads = 2, drive = 0x00;

var mem = new Memory(1024 * 1024); // 1mb // 640KB
var cpu = new CPU(mem);

bool stop = false;
Console.CancelKeyPress += (o, args) =>
{
    if ((args.SpecialKey & ConsoleSpecialKey.ControlC) != 0)
    {
        args.Cancel = true;
        stop = true;
    }
};

int CalculateLBA(ushort cylinder, byte head, int sector)
{
    return (cylinder * heads + head) * sectorsPerTrack + (sector - 1);
}

bool ReadSectors(byte drive, ushort cylinder, byte head, byte sector, byte count, ushort segment, ushort offset, out byte status)
{
    status = 0x00; // success

    if (count < 1)
    {
        status = 1;
        return false;
    }

    for (int i = 0; i < count; i++)
    {
        int lba = CalculateLBA(cylinder, head, sector + i);
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

    return true;
}

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
// https://en.wikipedia.org/wiki/INT_13H
cpu.HookInterrupt(0x13, cpu =>
{
    switch (cpu.AH)
    {
        case 0x00: // Reset Disk System
            cpu.CF = false;
            break;
        case 0x02: // Read
            var success = ReadSectors(
                drive: cpu.DL,
                cylinder: (ushort)(cpu.CX >> 8),
                head: (byte)(cpu.DX >> 8),
                sector: (byte)(cpu.CX & 0x3F),
                count: cpu.AL,
                segment: cpu.ES,
                offset: cpu.BX,
                out byte status
            );
            cpu.SetReg16(Register.AX, (ushort)((status << 8) | cpu.AL));

            ReturnFlag(Flags.Carry, !success, cpu);
            break;
        default:
            // throw new NotImplementedException();
            Debugger.Break();
            break;
    }
});
// http://vitaly_filatov.tripod.com/ng/asm/asm_001.10.html
cpu.HookInterrupt(0x12, cpu =>
{
    // Get usable [conventional] memory size
    var size = Math.Min(640, cpu.Memory.Size - 1);
    cpu.SetReg16(Register.AX, (ushort)(size - 1)); // Bochs says 639
});
cpu.HookInterrupt(0x16, cpu =>
{
    // Read key press
    Console.Read();
});
cpu.HookInterrupt(0x10, cpu =>
{
    switch (cpu.AH)
    {
        case 0x0E:
            // Teletype output
            var character = cpu.AL;
            var page = cpu.BH;
            var color = cpu.BL;
            Console.Write((char)character);
            break;
    }
});

void boot()
{
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

while (!stop)
{
    var in_breakpoint = false;
    if (breakpoints_enabled && break_addrs.Contains(cpu.CS * 16 + cpu.IP))
    {
        prompting = true;
        in_breakpoint = true;
    }
    if (!prompting && !in_breakpoint)
    {
        cpu.Clock();
        continue;
    }

    void MemoryWindow(ushort seg, ushort addr)
    {
        for (var i = 0; i < 16; i++)
        {
            var current = seg * 16 + addr + i;
            if (current < mem.Size)
            {
                Console.Write($"{mem[current]:X2} ");
            }
            else
            {
                Console.Write("   ");
            }
        }
        for (var i = 0; i < 16; i++)
        {
            var current = seg * 16 + addr + i;
            if (current < mem.Size)
            {
                var c = (char)mem[current];
                var ch = c >= 32 && c < 127 ? c.ToString() : ".";
                Console.Write($" {ch}");
            }
            else
            {
                Console.Write("  ");
            }
        }
        Console.WriteLine();
    }

    // CS:IP Window
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
            break;
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
            Console.WriteLine($"Executed {DecodeInstruction()}");
            break;
    }
}