// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Runtime.InteropServices.Swift;
using Emulate8086.Processor;

bool prompting = false;
bool break_with_debugger = false;
bool breakpoints_enabled = true;
HashSet<int> break_addrs = [
    //0x7C00,
    //0x0500,
    //0x0700,
    //0x70 * 16 + 0x232,
    0x9F84 * 16 + 0x34B //0x442,//0x420 //34B // 38F // 442
];

var disk = "/Users/josh/Downloads/002962_ms_dos_622/disk1.img";
var file = File.OpenRead(disk);
int sectorsPerTrack = 18, heads = 2;

var mem = new Memory(655360); // 640KB
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

            cpu.CF = !success;
            cpu.Memory.setWordAt(cpu.SS * 16 + cpu.SP + 4, (ushort)cpu.flags);
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
    cpu.SetReg16(Register.AX, (ushort)(cpu.Memory.Size >> 10));
});
var bootsect = new byte[512];
file.Seek(0, SeekOrigin.Begin);
file.Read(bootsect, 0, 512);
// Load the boot sector into memory.
for (var i = 0; i < 512; i++)
{
    mem[0x7C00 + i] = bootsect[i];
}

cpu.Jump(0x0000, 0x7C00);

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
    Console.WriteLine($": {cpu.NextInstruction}");

    // Prompt
    var command = "";
    if (!in_breakpoint || !break_with_debugger)
    {
        Console.Write("> ");
        command = Console.ReadLine();
    }
    switch (command)
    {
        case "exit":
            stop = true;
            break;
        case "c":
            prompting = false;
            break;
        case "":
            cpu.Clock(in_breakpoint && break_with_debugger);
            in_breakpoint = false;
            // TODO: Disassembly
            Console.WriteLine($"Executed {cpu.PreviousInstruction}");
            break;
    }
}