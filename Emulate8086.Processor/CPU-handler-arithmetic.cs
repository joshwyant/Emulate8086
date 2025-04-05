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
        #region Addition
        private static void HandleADD(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-7

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-35
            // - Table 2-21. Instruction Set Reference Data, p. 2-52
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23
            // - Table 4-13. Machine Instruction Decoding Guide, p. 4-27

            // ODITSZAPC
            // X   XXXXX

            int result = 0, a = 0, b = 0;
            if ((self.insByte & 0b11111100) == 0b00000000)
            {
                //...
                if (self.Memory.wordAt(self.csip_start) == 0 &&
                    self.Memory.wordAt(self.csip_start + 2) == 0 &&
                    self.Memory.wordAt(self.csip_start + 4) == 0 &&
                    self.Memory.wordAt(self.csip_start + 6) == 0)
                {
                    if (Debugger.IsAttached)
                    {
                        // We're executing all 0's
                        Debugger.Break();
                    }
                    else
                    {
                        self.LogError(() => "Caught executing zeros");
                        Environment.Exit(1);
                    }
                }
                // 0000 00dw (00 - 03) | mod reg r/m
                // Register/memory with register to either
                self.DecodeInstruction(
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMReg |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.D);

                // Add modrm/reg depending on direction
                a = self.GetModRMData();
                b = self.GetReg(self.insReg, self.insW);
                result = a + b;
                // From page 4-19 of Intel User's Manual:
                // D = 0: Instruction source is specified in REG field
                // D = 1: Instruction destination is specified in REG field
                if (!self.insD)
                {
                    self.SetModRMData((ushort)result);
                }
                else
                {
                    self.SetReg(self.insReg, (ushort)result, self.insW);
                }
            }
            else if ((self.insByte & 0b11111100) == 0b10000000)
            {
                // 1000 00sw (80 - 83) | mod 000 r/m | data | data if s:w=01
                // Part of Immediate group
                self.DecodeInstruction(
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.S |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word);

                // Add immediate to r/m
                // Extended opcode is 000
                Debug.Assert(self.insExtOpcode == 0b000);
                a = self.GetModRMData();
                b = self.ins_data;
                result = a + b;
                self.SetModRMData((ushort)result);
            }
            else
            {
                Debug.Assert((self.insByte & 0b11111110) == 0b00000100);

                // 0000 010w (04 - 05) | data | data if w=1
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word);

                // Add immediate to accumulator
                a = self.insW ? self.ax : self.al;
                b = self.ins_data;
                result = a + b;
                self.SetReg(Register.AX, (ushort)result, self.insW);
            }
            self.SetAdditionFlags(a, b, 0, result);
        }

        private static void HandleADC(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-7

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-35
            // - Table 2-21. Instruction Set Reference Data, p. 2-52
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            // ODITSZAPC
            // X   XXXXX

            int a, b, result = self.CF ? 1 : 0;
            if ((self.insByte & 0b1111_1100) == 0b0001_0000)
            {
                // 0001 00dw (10 - 13) | mod reg r/m
                //   Register/memory with register to either
                self.DecodeInstruction(
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMReg |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.D);

                // Add modrm/reg depending on direction
                a = self.GetModRMData();
                b = self.GetReg(self.insReg, self.insW);
                result += a + b;
                if (!self.insD)
                {
                    self.SetModRMData((ushort)result);
                }
                else
                {
                    self.SetReg(self.insReg, (ushort)result, self.insW);
                }
            }
            else if ((self.insByte & 0b1111_1100) == 0b1000_0000)
            {
                // 1000 00sw (80 - 83) | mod 010 r/m | data | data if s:w=01
                //   Immediate to register/memory
                //   Part of Immediate group
                self.DecodeInstruction(
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.S |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word);

                // Add immediate to r/m
                // Extended opcode is 010
                Debug.Assert(self.insExtOpcode == 0b010);
                a = self.GetModRMData();
                b = self.ins_data;
                result += a + b;
                self.SetModRMData((ushort)result);
            }
            else
            {
                Debug.Assert((self.insByte & 0b1111_1110) == 0b0001_0100);

                // 0001 010w (14 - 15) | data | data if w=1
                //   Immediate to accumulator
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word);

                // Add immediate to accumulator
                a = self.insW ? self.ax : self.al;
                b = self.ins_data;
                result += a + b;
                self.SetReg(Register.AX, (ushort)result, self.insW);
            }
            self.SetAdditionFlags(a, b, self.CF ? 1 : 0, result);
        }

        private static void HandleINC(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-7

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-33
            // - Table 2-21. Instruction Set Reference Data, p. 2-55
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            // ODITSZAPC
            // X   XXXX 

            int result = 0;

            // 1111111w (FE - FF) mod 000 rm
            // Register/memory
            // Part of Group 2 instructions
            if ((self.insByte & 0b1111_1110) == 0b1111_1110)
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode
                );
                Debug.Assert(self.insExtOpcode == 0b000);
                result = self.GetModRMData() + 1;
                self.SetModRMData((ushort)result);
            }
            // 01000 (40 - 43) reg
            // Reg
            else
            {
                Debug.Assert((self.insByte & 0b1111_1000) == 0b0100_0000);

                self.DecodeInstruction(
                    InstructionDecoderFlags.Reg
                );
                self.insW = true; // Yes or no?? Needed for addition flags
                result = self.GetReg(self.insReg, self.insW) + 1;
                self.SetReg(self.insReg, (ushort)result, self.insW);
            }

            var prevCarry = self.CF;
            self.SetAdditionFlags(result, 0, 0, result);
            self.CF = prevCarry;
        }

        private static void HandleAAA(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-7

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-35
            // - Table 2-21. Instruction Set Reference Data, p. 2-51
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            // ODITSZAPC
            // U   UUXUX

            Debug.Assert(0b0011_0111 == self.insByte);

            // ASCII adjust for add
            // 00110111 (37)
            // 4 clocks

            // Logic: https://en.wikipedia.org/wiki/Intel_BCD_opcode
            //        https://web.archive.org/web/20190203181246/http://www.jaist.ac.jp/iscenter-new/mpc/altix/altixdata/opt/intel/vtune/doc/users_guide/mergedProjects/analyzer_ec/mergedProjects/reference_olh/mergedProjects/instructions/instruct32_hh/vc2a.htm
            //        https://web.archive.org/web/20081102170717/http://webster.cs.ucr.edu/AoA/Windows/HTML/AdvancedArithmetica6.html#1000255
            if ((self.al & 0x0F) >= 10)
            {
                self.al = (byte)((self.al + 6) & 0xF);
                self.ah += 1;
                self.flags |= Flags.CF | Flags.AF;
            }
            else
            {
                self.flags &= ~(Flags.CF | Flags.AF);
            }
        }

        private static void HandleDAA(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-7

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-36
            // - Table 2-21. Instruction Set Reference Data, p. 2-54
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            // ODITSZAPC
            // X   XXXXX

            Debug.Assert(0b0010_0111 == self.insByte);

            // Decimal adjust for add
            // 00100111 (27)

            var lower = self.al & 0xF;
            if (lower >= 10 || self.AF)
            {
                self.al += 6;
            }
            var higher = (self.al & 0xF0) >> 4;
            if (higher >= 10 || self.CF)
            {
                self.al += 96;
                self.CF = true;
            }
        }
        #endregion

        #region Subtraction
        private static void HandleSUB(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-7

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-36
            // - Table 2-21. Instruction Set Reference Data, p. 2-67
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // X   XXXXX

            int a = 0, b = 0, result = 0;

            if ((self.insByte & 0b11111100) == 0b00101000)
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.D |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMReg
                );
                // r/m and r to either
                // 001010dw (28-2B) mod reg r/m

                if (self.insD)
                {
                    // reg = reg - r/m
                    a = self.GetReg(self.insReg, self.insW);
                    b = self.GetModRMData();
                    result = a - b;
                    self.SetReg(self.insReg, (ushort)result, self.insW);
                }
                else
                {
                    // r/m = r/m - reg
                    a = self.GetModRMData();
                    b = self.GetReg(self.insReg, self.insW);
                    result = a - b;
                    self.SetModRMData((ushort)result);
                }
            }
            else if ((self.insByte & 0b11111100) == 0b10000000)
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.S |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );
                // imm to reg/mem
                // 100000sw (80 - 83) mod 101 r/m data, data if s:w=01
                // Part of Immediate group

                Debug.Assert(self.insExtOpcode == 0b101);

                a = self.GetModRMData();
                b = self.ins_data;
                result = a - b;
                self.SetModRMData((ushort)result);
            }
            else
            {
                Debug.Assert((self.insByte & 0b11111110) == 0b00101100);

                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // imm from accum
                // 0010110w (2C - 2D) data, data if w=1

                a = self.insW ? self.ax : self.al;
                b = self.ins_data;
                result = a - b;
                self.SetReg(Register.AX, (ushort)result, self.insW);
            }

            // Set flags for subtraction
            self.SetSubtractionFlags(a, b, 0, result);
        }

        private static void HandleSBB(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-8

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-36
            // - Table 2-21. Instruction Set Reference Data, p. 2-65
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // X   XXXXX

            int a = 0, b = 0, result = 0;
            int borrow = self.CF ? 1 : 0;

            // Subtract with borrow
            if ((self.insByte & 0b111111_00) == 0b000110_00)
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.D |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMReg
                );

                // r/m and r to either
                // 000110dw (18 - 1B) mod reg r/m

                if (self.insD)
                {
                    // reg = reg - r/m - CF
                    a = self.GetReg(self.insReg, self.insW);
                    b = self.GetModRMData();
                    result = a - b - borrow;
                    self.SetReg(self.insReg, (ushort)result, self.insW);
                }
                else
                {
                    // r/m = r/m - reg - CF
                    a = self.GetModRMData();
                    b = self.GetReg(self.insReg, self.insW);
                    result = a - b - borrow;
                    self.SetModRMData((ushort)result);
                }
            }
            else if (0b100000_00 == (self.insByte & 0b111111_00))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.S |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // imm to reg/mem
                // 100000sw (80 - 83) mod 011 r/m data, data if s:w=01
                // Part of Immediate group

                Debug.Assert(self.insExtOpcode == 0b011);

                a = self.GetModRMData();
                b = self.ins_data;
                result = a - b - borrow;
                self.SetModRMData((ushort)result);
            }
            else
            {
                Debug.Assert(0b0001110_0 == (self.insByte & 0b1111111_0));

                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // imm from accum
                // 0001110w (1C - 1D) data, data if w=1

                a = self.insW ? self.ax : self.al;
                b = self.ins_data;
                result = a - b - borrow;
                self.SetReg(Register.AX, (ushort)result, self.insW);
            }

            // Set flags for subtraction
            self.SetSubtractionFlags(a, b, borrow, result);
        }

        private static void HandleDEC(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-8

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-36
            // - Table 2-21. Instruction Set Reference Data, p. 2-54
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // X   XXXX 

            int a = 0, result = 0;
            var prevCarry = self.CF;

            if (0b1111111_0 == (self.insByte & 0b1111111_0))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode
                );

                // 1111111w (FE - FF) mod 001 r/m
                // Part of Group 2 instructions

                Debug.Assert(self.insExtOpcode == 0b001);

                a = self.GetModRMData();
                result = a - 1;
                self.SetModRMData((ushort)result);
            }
            else
            {
                Debug.Assert(0b01001_000 == (self.insByte & 0b11111_000));

                self.DecodeInstruction(
                    InstructionDecoderFlags.Reg
                );
                // 01001 reg (48 - 4F)

                self.insW = true; // Set for 16-bit register
                a = self.GetReg16(self.insReg);
                result = a - 1;
                self.SetReg16(self.insReg, (ushort)result);
            }

            // Set flags but preserve the carry flag
            self.SetSubtractionFlags(a, 1, 0, result);
            self.CF = prevCarry;
        }

        private static void HandleNEG(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-8

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-36
            // - Table 2-21. Instruction Set Reference Data, p. 2-62
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // X   XXXX1* (*C is 0 if destination=0)

            Debug.Assert(0b1111_011_0 == (self.insByte & 0b1111111_0));

            self.DecodeInstruction(
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );

            // 1111011w (F6-F7) mod 011 r/m
            // Part of Group 1 instructions

            Debug.Assert(self.insExtOpcode == 0b011);

            var value = self.GetModRMData();
            var result = (ushort)(-value);
            self.SetModRMData(result);

            // Set flags - NEG is basically a SUB of the value from 0
            self.SetSubtractionFlags(0, value, 0, result);

            // CF is 0 only if source operand is 0
            self.CF = value != 0;
        }

        private static void HandleCMP(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-8

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-36
            // - Table 2-21. Instruction Set Reference Data, p. 2-53
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // X   XXXXX

            int a = 0, b = 0, result = 0;

            if (0b0011_10_00 == (self.insByte & 0b111111_00))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.D |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMReg
                );

                // Register/memory and register
                // 001110dw (38 - 3B) mod reg r/m

                if (self.insD)
                {
                    // Compare reg - r/m
                    a = self.GetReg(self.insReg, self.insW);
                    b = self.GetModRMData();
                }
                else
                {
                    // Compare r/m - reg
                    a = self.GetModRMData();
                    b = self.GetReg(self.insReg, self.insW);
                }
                result = a - b;
            }
            else if (0b1000_00_00 == (self.insByte & 0b111111_00))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.S |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immediate with register/memory
                // 100000sw (80 - 83) mod 111 r/m data, data if s:w =01
                // Part of Immediate group

                Debug.Assert(self.insExtOpcode == 0b111);

                a = self.GetModRMData();
                b = self.ins_data;
                result = a - b;
            }
            else
            {
                Debug.Assert(0b0011_110_0 == (self.insByte & 0b1111111_0));

                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immediate with accumulator
                // 0011110w (3C - 3D) data, data if w=1

                a = self.insW ? self.ax : self.al;
                b = self.ins_data;
                result = a - b;
            }

            // CMP only sets flags, doesn't store result
            self.SetSubtractionFlags(a, b, 0, result);
        }

        private static void HandleAAS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-8

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-36
            // - Table 2-21. Instruction Set Reference Data, p. 2-51
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // U   UUXUX

            Debug.Assert(self.insByte == 0b0011_1111);

            // 00111111 (3F) -- ASCII adjust for subtract
            // 4 clocks

            // If lower nibble is > 9 or AF is set
            if ((self.al & 0xF) > 9 || self.AF)
            {
                self.al = (byte)((self.al - 6) & 0x0F);
                self.ah = (byte)(self.ah - 1);
                self.flags |= Flags.AF | Flags.CF;
            }
            else
            {
                self.flags &= ~(Flags.AF | Flags.CF);
            }

            // Zero the upper nibble of AL
            self.al &= 0x0F;
        }

        private static void HandleDAS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-8

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-36
            // - Table 2-21. Instruction Set Reference Data, p. 2-54
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // U   XXXX

            Debug.Assert(self.insByte == 0b0010_1111);

            // Decimal adjust for subtract
            // 00101111 (2F)

            var oldAL = self.al;
            var oldCF = self.CF;

            // If lower nibble > 9 or AF is set
            if ((self.al & 0xF) > 9 || self.AF)
            {
                self.al -= 6;
                self.AF = true;
            }
            else
            {
                self.AF = false;
            }

            // If upper nibble > 9 or CF is set
            if ((oldAL > 0x99) || oldCF)
            {
                self.al -= 0x60;
                self.CF = true;
            }
            else
            {
                self.CF = false;
            }
        }
        #endregion

        #region Multiplication
        private static void HandleMUL(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-8

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-36
            // - Table 2-21. Instruction Set Reference Data, p. 2-61
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // X   UUUUX

            Debug.Assert(0b1111_011_0 == (self.insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );

            // Multiply unsigned
            // 1111011w (F6 - F7) mod 100 r/m
            // Part of Group 1 instructions

            Debug.Assert(self.insExtOpcode == 0b100);

            if (self.insW)
            {
                // Word multiplication (AX * r/m16 = DX:AX)
                uint result = (uint)self.ax * (uint)self.GetModRMData();
                self.ax = (ushort)(result & 0xFFFF);         // Lower word
                self.dx = (ushort)((result >> 16) & 0xFFFF); // Upper word

                // Set CF/OF if upper word (DX) is non-zero
                self.CF = self.OF = (self.dx != 0);
            }
            else
            {
                // Byte multiplication (AL * r/m8 = AX)
                ushort result = (ushort)(self.al * self.GetModRMData());
                self.ax = result;

                // Set CF/OF if upper byte (AH) is non-zero
                self.CF = self.OF = ((result & 0xFF00) != 0);
            }
        }

        private static void HandleIMUL(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-8

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-37
            // - Table 2-21. Instruction Set Reference Data, p. 2-55
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // X   UUUUX

            Debug.Assert(0b1111_011_0 == (self.insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );

            // Integer multiply (signed)
            // 1111011w (F6 - F7) mod 101 r/m
            // Part of Group 1 instructions

            Debug.Assert(self.insExtOpcode == 0b101);

            if (self.insW)
            {
                // Word multiplication (AX * r/m16 = DX:AX)
                int a = (short)self.ax;
                int b = (short)self.GetModRMData();
                int result = a * b;

                self.ax = (ushort)(result & 0xFFFF);         // Lower word
                self.dx = (ushort)((result >> 16) & 0xFFFF); // Upper word

                // Set CF/OF if sign extension of AX != DX
                short signExtendedLow = (short)self.ax;
                self.CF = self.OF = (((signExtendedLow < 0) && (self.dx != 0xFFFF)) ||
                                      ((signExtendedLow >= 0) && (self.dx != 0)));
            }
            else
            {
                // Byte multiplication (AL * r/m8 = AX)
                sbyte a = (sbyte)self.al;
                sbyte b = (sbyte)(byte)self.GetModRMData();
                short result = (short)(a * b);

                self.ax = (ushort)result;

                // Set CF/OF if sign extension of AL != AH
                byte signExtendedAL = (byte)((self.al & 0x80) != 0 ? 0xFF : 0);
                self.CF = self.OF = (self.ah != signExtendedAL);
            }
        }

        private static void HandleAAM(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-8

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-37
            // - Table 2-21. Instruction Set Reference Data, p. 2-51
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // U   XXUXU

            Debug.Assert(0b1101_0100 == self.insByte &&
                         0b0000_1010 == self.memory[self.csip++]);

            // ASCII adjust for multiply
            // 11010100 00001010 (D4 0A)
            // Second byte is always the same and is just "there"
            // (It equals 10 in decimal)
            // 83 clocks
            // Description: Intel 8086 Family User's Manual October 1979, p. 2-36

            byte base10 = 10; // This is usually the second byte of the instruction

            self.ah = (byte)(self.al / base10);
            self.al = (byte)(self.al % base10);

            // Update flags
            self.SF = ((self.ah & 0x80) != 0);
            self.ZF = (self.ax == 0);
            self.PF = parity_byte(self.al);
        }
        #endregion

        #region Division
        private static void HandleDIV(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-8

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-37
            // - Table 2-21. Instruction Set Reference Data, p. 2-54
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // U   UUUUU

            Debug.Assert(0b1111_011_0 == (self.insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );

            // Divide (unsigned)
            // 1111011w (F6 - F7) mod 110 r/m
            // Part of Group 1 instructions
            Debug.Assert(self.insExtOpcode == 0b110);

            ushort divisor = self.GetModRMData();

            // Division by zero should cause an interrupt (not handled here)
            if (divisor == 0)
            {
                // In real 8086, this would trigger a divide-by-zero exception (INT 0)
                // TODO
                throw new DivideByZeroException("DIV instruction with zero divisor");
            }

            if (self.insW)
            {
                // Word division (DX:AX / r/m16 = AX quotient, DX remainder)
                uint dividend = ((uint)self.dx << 16) | self.ax;
                uint quotient = dividend / divisor;
                uint remainder = dividend % divisor;

                // Check for division overflow (quotient > 0xFFFF)
                if (quotient > 0xFFFF)
                {
                    // In real 8086, this would trigger a divide-by-zero exception (INT 0)
                    throw new OverflowException("DIV instruction quotient overflow");
                    // TODO
                }

                self.ax = (ushort)quotient;
                self.dx = (ushort)remainder;
            }
            else
            {
                // Byte division (AX / r/m8 = AL quotient, AH remainder)
                ushort dividend = self.ax;
                byte byteDiv = (byte)divisor;

                byte quotient = (byte)(dividend / byteDiv);
                byte remainder = (byte)(dividend % byteDiv);

                // Check for division overflow (quotient > 0xFF)
                if ((dividend / byteDiv) > 0xFF)
                {
                    // In real 8086, this would trigger a divide-by-zero exception (INT 0)
                    throw new OverflowException("DIV instruction quotient overflow");
                    // TODO
                }

                self.al = quotient;
                self.ah = remainder;
            }

            // DIV doesn't affect flags
        }

        private static void HandleIDIV(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-9

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-37
            // - Table 2-21. Instruction Set Reference Data, p. 2-55
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // U   UUUUU

            Debug.Assert(0b1111_011_0 == (self.insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );

            // Integer divide (signed)
            // 1111011w (F6 - F7) mod 111 r/m
            // Part of Group 1 instructions
            Debug.Assert(self.insExtOpcode == 0b111);

            short divisor = (short)self.GetModRMData();

            // Division by zero should cause an interrupt (not handled here)
            if (divisor == 0)
            {
                // In real 8086, this would trigger a divide-by-zero exception (INT 0)
                throw new DivideByZeroException("IDIV instruction with zero divisor");
                // TODO
            }

            if (self.insW)
            {
                // Word division (DX:AX / r/m16 = AX quotient, DX remainder)
                int dividend = ((int)self.dx << 16) | self.ax;
                int quotient = dividend / divisor;
                int remainder = dividend % divisor;

                // Check for division overflow
                if (quotient < -32768 || quotient > 32767)
                {
                    // In real 8086, this would trigger a divide-by-zero exception (INT 0)
                    throw new OverflowException("IDIV instruction quotient overflow");
                    // TODO
                }

                self.ax = (ushort)quotient;
                self.dx = (ushort)remainder;
            }
            else
            {
                // Byte division (AX / r/m8 = AL quotient, AH remainder)
                short dividend = (short)self.ax;
                sbyte byteDiv = (sbyte)(byte)divisor;

                sbyte quotient = (sbyte)(dividend / byteDiv);
                sbyte remainder = (sbyte)(dividend % byteDiv);

                // Check for division overflow
                if (quotient < -128 || quotient > 127)
                {
                    // In real 8086, this would trigger a divide-by-zero exception (INT 0)
                    throw new OverflowException("IDIV instruction quotient overflow");
                    // TODO
                }

                self.al = (byte)quotient;
                self.ah = (byte)remainder;
            }

            // IDIV doesn't affect flags
        }

        private static void HandleAAD(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-9

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-37
            // - Table 2-21. Instruction Set Reference Data, p. 2-51
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // U   XXUXU
            // Second byte is always the same and is just "there"
            // (It equals 10 in decimal)
            // 60 clocks
            // Description: Intel 8086 Family User's Manual October 1979, p. 2-36

            Debug.Assert(0b1101_0101 == self.insByte &&
                         0b0000_1010 == self.memory[self.csip++]);

            // ASCII adjust for divide
            // 11010101 00001010 (D5 0A)

            byte base10 = 10; // This is usually the second byte of the instruction

            // Convert AX from BCD to binary
            self.al = (byte)((self.ah * base10) + self.al);
            self.ah = 0;

            // Update flags
            self.SF = ((self.al & 0x80) != 0);
            self.ZF = (self.ax == 0);
            self.PF = parity_byte(self.al);
        }

        private static void HandleCBW(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-9

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-38
            // - Table 2-21. Instruction Set Reference Data, p. 2-52
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            Debug.Assert(self.insByte == 0b1001_1000);

            // Convert byte to word
            // 10011000 (98)

            // Sign-extend AL into AH (AL to AX)
            self.ah = (self.al & 0x80) != 0 ? (byte)0xFF : (byte)0;
        }

        private static void HandleCWD(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-9

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-38
            // - Table 2-21. Instruction Set Reference Data, p. 2-54
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            Debug.Assert(self.insByte == 0b1001_1001);

            // Convert word to double word
            // 10011001 (99)

            // Sign-extend AX into DX (AX to DX:AX)
            self.dx = (self.ax & 0x8000) != 0 ? (ushort)0xFFFF : (ushort)0;
        }
        #endregion

        #region Helpers
        static bool parity_byte(byte data)
        {
            // Count bits set to 1 and return true if even, false if odd.
            // (according to chatgpt)
            data ^= (byte)(data >> 4);
            data ^= (byte)(data >> 2);
            data ^= (byte)(data >> 1);
            return (data & 1) == 0;
        }

        private void SetAdditionFlags(int a, int b, int c, int result)
        {
            // Consider getting these lazily after caching the last result
            var actualResult = insW ? result : result & 0xFF;
            ZF = actualResult == 0;
            CF = (result & (insW ? 0x10000 : 0x100)) != 0;
            AF = ((a & 0xF) + (b & 0xF) + c) >= 0x10; // Addition resulting in a carry outside the lower nibble
            SF = (result & (insW ? 0x8000 : 0x80)) != 0;
            PF = parity_byte((byte)(result & 0xFF));
            OF = ((a ^ result) & (b ^ result) & (insW ? 0x8000 : 0x80)) != 0; // signed overflow
        }

        private void SetSubtractionFlags(int a, int b, int borrow, int result)
        {
            // Consider getting these lazily after caching the last result
            var actualResult = insW ? result : result & 0xFF;
            ZF = actualResult == 0;
            CF = (uint)a < (uint)(b + borrow);
            AF = ((a & 0xF) - (b & 0xF) - borrow) < 0; // Borrow from the higher nibble
            SF = (result & (insW ? 0x8000 : 0x80)) != 0;
            PF = parity_byte((byte)(result & 0xFF));
            OF = ((a ^ b) & (a ^ result) & (insW ? 0x8000 : 0x80)) != 0; // signed overflow
        }
        #endregion
    }
}
