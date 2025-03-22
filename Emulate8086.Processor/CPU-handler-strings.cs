using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace Emulate8086.Processor
{
    public partial class CPU
    {
        private static void HandleREPZ(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-64 

            HandleREP(self);
        }

        private static void HandleREPE(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-64 

            HandleREP(self);
        }

        private static void HandleREPNZ(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-64 

            HandleREP(self);
        }

        private static void HandleREPNE(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-64 

            HandleREP(self);
        }

        private static void HandleREP(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-10

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-42
            // - Table 2-21. Instruction Set Reference Data, p. 2-63
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            Debug.Assert(0b1111_001_0 == (self.insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.Z
            );
            // Repeat
            // 1111001z

            // Store the repeat prefix state
            self.repActive = true;
            self.repZ = self.insZ;

            // Note: The next instruction will be executed in the context of this prefix
            // The actual repeat loop is handled by the CPU execution loop
            // This is only storing the prefix state for the next string instruction
        }

        private static void HandleMOVSW(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-61

            // 1010010w
            HandleMOVS(self);
        }

        private static void HandleMOVSB(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-61

            // 1010010w
            HandleMOVS(self);
        }

        private static void HandleMOVS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-10

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-42
            // - Table 2-21. Instruction Set Reference Data, p. 2-61
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            Debug.Assert(0b1010_010_0 == (self.insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.W
            );
            // 1010010w

            // Check if we're in a REP loop
            if (self.repActive)
            {
                // If CX is zero, exit
                if (self.cx == 0)
                {
                    self.repActive = false;
                    return;
                }
            }

            // Execute MOVS
            if (self.insW)
            {
                // Word operation
                ushort value = self.memory.wordAt(self.ds, self.si);
                self.memory.setWordAt(self.es, self.di, value);

                // Update SI and DI based on DF flag
                if (self.DF)
                {
                    self.si -= 2;
                    self.di -= 2;
                }
                else
                {
                    self.si += 2;
                    self.di += 2;
                }
            }
            else
            {
                // Byte operation
                var dssi = self.ds * 16 + self.si;
                var esdi = self.es * 16 + self.di;
                self.memory[esdi] = self.memory[dssi];

                // Update SI and DI based on DF flag
                if (self.DF)
                {
                    self.si -= 1;
                    self.di -= 1;
                }
                else
                {
                    self.si += 1;
                    self.di += 1;
                }
            }

            // If REP prefix active, decrement CX and check
            if (self.repActive)
            {
                self.cx--;
                if (self.cx == 0)
                {
                    self.repActive = false;
                }
                else
                {
                    // Repeat the instruction by adjusting IP
                    self.ip--;
                }
            }
        }

        private static void HandleCMPS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-10

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-42
            // - Table 2-21. Instruction Set Reference Data, p. 2-53
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            // ODITSZAPC
            // X   XXXXX

            Debug.Assert(0b1010_011_0 == (self.insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.W
            );
            // Compare string
            // 1010011w

            // Check if we're in a REP loop
            if (self.repActive)
            {
                // If CX is zero, exit
                if (self.cx == 0)
                {
                    self.repActive = false;
                    return;
                }
            }

            // Execute CMPS
            if (self.insW)
            {
                // Word operation
                ushort value1 = self.memory.wordAt(self.ds, self.si);
                ushort value2 = self.memory.wordAt(self.es, self.di);
                ushort result = (ushort)(value1 - value2);

                // Set flags as with CMP instruction
                self.SetSubtractionFlags(value1, value2, 0, result); // TODO: Correct borrow value?

                // Update SI and DI based on DF flag
                if (self.DF)
                {
                    self.si -= 2;
                    self.di -= 2;
                }
                else
                {
                    self.si += 2;
                    self.di += 2;
                }
            }
            else
            {
                // Byte operation
                var dssi = self.ds * 16 + self.si;
                var esdi = self.es * 16 + self.di;
                byte value1 = self.memory[dssi];
                byte value2 = self.memory[esdi];
                byte result = (byte)(value1 - value2);

                // Set flags as with CMP instruction
                self.SetSubtractionFlags(value1, value2, 0, result); // TODO: Correct borrow value?

                // Update SI and DI based on DF flag
                if (self.DF)
                {
                    self.si -= 1;
                    self.di -= 1;
                }
                else
                {
                    self.si += 1;
                    self.di += 1;
                }
            }

            // If REP prefix active, handle the repeat
            if (self.repActive)
            {
                self.cx--;

                bool shouldRepeat = false;
                if (self.repZ)
                {
                    // REPE/REPZ: Repeat while equal (ZF=1)
                    shouldRepeat = self.ZF && (self.cx != 0);
                }
                else
                {
                    // REPNE/REPNZ: Repeat while not equal (ZF=0)
                    shouldRepeat = !self.ZF && (self.cx != 0);
                }

                if (!shouldRepeat)
                {
                    self.repActive = false;
                }
                else
                {
                    // Repeat the instruction by adjusting IP
                    self.ip = (ushort)self.ipStart;
                }
            }
        }

        private static void HandleSCAS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-10

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-43
            // - Table 2-21. Instruction Set Reference Data, p. 2-65
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            // ODITSZAPC
            // X   XXXXX

            Debug.Assert(0b1010_111_0 == (self.insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.W
            );
            // Scan string
            // 1010111w

            // Check if we're in a REP loop
            if (self.repActive)
            {
                // If CX is zero, exit
                if (self.cx == 0)
                {
                    self.repActive = false;
                    return;
                }
            }

            // Execute SCAS
            if (self.insW)
            {
                // Word operation
                ushort value1 = self.ax;
                ushort value2 = self.memory.wordAt(self.es, self.di);
                ushort result = (ushort)(value1 - value2);

                // Set flags as with CMP instruction
                self.SetSubtractionFlags(value1, value2, 0, result); // TODO: Correct borrow value?

                // Update DI based on DF flag
                if (self.DF)
                {
                    self.di -= 2;
                }
                else
                {
                    self.di += 2;
                }
            }
            else
            {
                // Byte operation
                byte value1 = (byte)self.ax;
                byte value2 = self.memory[self.es * 16 + self.di];
                byte result = (byte)(value1 - value2);

                // Set flags as with CMP instruction
                self.SetSubtractionFlags(value1, value2, 0, result); // TODO: Correct borrow value?

                // Update DI based on DF flag
                if (self.DF)
                {
                    self.di -= 1;
                }
                else
                {
                    self.di += 1;
                }
            }

            // If REP prefix active, handle the repeat
            if (self.repActive)
            {
                self.cx--;

                bool shouldRepeat = false;
                if (self.repZ)
                {
                    // REPE/REPZ: Repeat while equal (ZF=1)
                    shouldRepeat = self.ZF && (self.cx != 0);
                }
                else
                {
                    // REPNE/REPNZ: Repeat while not equal (ZF=0)
                    shouldRepeat = !self.ZF && (self.cx != 0);
                }

                if (!shouldRepeat)
                {
                    self.repActive = false;
                }
                else
                {
                    // Repeat the instruction by adjusting IP
                    self.ip = (ushort)self.ipStart;
                }
            }
        }

        private static void HandleLODS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-11

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-43
            // - Table 2-21. Instruction Set Reference Data, p. 2-60
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            Debug.Assert(0b1010_110_0 == (self.insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.W
            );
            // Load string
            // 1010110w

            // Check if we're in a REP loop
            if (self.repActive)
            {
                // If CX is zero, exit
                if (self.cx == 0)
                {
                    self.repActive = false;
                    return;
                }
            }

            // Execute LODS
            if (self.insW)
            {
                // Word operation
                self.ax = self.memory.wordAt(self.ds, self.si);

                // Update SI based on DF flag
                if (self.DF)
                {
                    self.si -= 2;
                }
                else
                {
                    self.si += 2;
                }
            }
            else
            {
                // Byte operation
                self.ax = (ushort)((self.ax & 0xFF00) | self.memory[self.ds * 16 + self.si]);

                // Update SI based on DF flag
                if (self.DF)
                {
                    self.si -= 1;
                }
                else
                {
                    self.si += 1;
                }
            }

            // If REP prefix active, decrement CX and check
            if (self.repActive)
            {
                self.cx--;
                if (self.cx == 0)
                {
                    self.repActive = false;
                }
                else
                {
                    // Repeat the instruction by adjusting IP
                    self.ip = (ushort)self.ipStart;
                }
            }
        }

        private static void HandleSTOS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-11

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-43
            // - Table 2-21. Instruction Set Reference Data, p. 2-66
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            Debug.Assert(0b1010_101_0 == (self.insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.W
            );
            // Store string
            // 1010101w

            // Check if we're in a REP loop
            if (self.repActive)
            {
                // If CX is zero, exit
                if (self.cx == 0)
                {
                    self.repActive = false;
                    return;
                }
            }

            // Execute STOS
            if (self.insW)
            {
                // Word operation
                self.memory.setWordAt(self.es * 16 + self.di, self.ax);

                // Update DI based on DF flag
                if (self.DF)
                {
                    self.di -= 2;
                }
                else
                {
                    self.di += 2;
                }
            }
            else
            {
                // Byte operation
                self.memory[self.es * 16 + self.di] = (byte)self.ax;

                // Update DI based on DF flag
                if (self.DF)
                {
                    self.di -= 1;
                }
                else
                {
                    self.di += 1;
                }
            }

            // If REP prefix active, decrement CX and check
            if (self.repActive)
            {
                self.cx--;
                if (self.cx == 0)
                {
                    self.repActive = false;
                }
                else
                {
                    // Repeat the instruction by adjusting IP
                    self.ip = (ushort)self.ipStart;
                }
            }
        }
        short ipStart;
        bool repActive;
        bool repZ;
    }
}
