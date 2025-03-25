// See https://aka.ms/new-console-template for more information
using Emulate8086.Processor;

bool stop = false;
Console.CancelKeyPress += (o, args) =>
{
    if ((args.SpecialKey & ConsoleSpecialKey.ControlC) != 0)
    {
        args.Cancel = true;
        stop = true;
    }
};

var mem = new Memory(655360); // 640KB
var cpu = new CPU(mem);

// Interrupts
// https://en.wikipedia.org/wiki/INT_13H
cpu.HookInterrupt(0x13, cpu =>
{
    switch (cpu.AH)
    {
        case 0x00: // Reset Disk System
            cpu.SetReg8(Register.AH, 1);
            break;
    }
});

var disk = "/Users/josh/Downloads/002962_ms_dos_622/disk1.img";
var bootsect = new byte[512];
using (var file = File.OpenRead(disk))
{
    file.Read(bootsect, 0, 512);
}
// Load the boot sector into memory.
for (var i = 0; i < 512; i++)
{
    mem[0x7C00 + i] = bootsect[i];
}

cpu.Jump(0x0000, 0x7C00);

while (!stop)
{
    void MemoryWindow(ushort seg, ushort addr)
    {
        for (var i = 0; i < 16; i++)
        {
            Console.Write($"{mem[seg * 16 + addr + i]:X2} ");
        }
        for (var i = 0; i < 16; i++)
        {
            var c = (char)mem[seg * 16 + addr + i];
            var ch = c >= 32 && c < 127 ? c.ToString() : ".";
            Console.Write($" {ch}");
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
    Console.Write("> ");
    switch (Console.ReadLine())
    {
        case "exit":
            stop = true;
            break;
        case "":
            cpu.Clock();
            // TODO: Disassembly
            Console.WriteLine($"Executed {cpu.PreviousInstruction}");
            break;
    }
}