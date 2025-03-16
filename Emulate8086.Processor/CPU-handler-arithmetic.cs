using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if (self.insD)
                {
                    self.SetModRMData((ushort)result);
                }
                else
                {
                    self.SetReg(self.insReg, (ushort)result, self.insW);
                }
            }
            else if ((self.insByte & 0x11111100) == 0b10000000)
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
            else if ((self.insByte & 0b11111110) == 0b00000100)
            {
                // 0000 010w (04 - 05) | data | data if w=1
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word);

                // Add immediate to accumulator
                a = self.ax;
                b = self.data(ref csip, w);
                result = a + b;
                self.ax = (ushort)result;
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

            int result = self.CF ? 1 : 0;
            if ((self.insByte & 0b1111_1100) == 0b0001_0000)
            {
                // 0001 00dw | mod reg r/m
                //   Register/memory with register to either
                self.DecodeInstruction(
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMReg |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.D);

                // Add modrm/reg depending on direction
                result += self.GetModRMData() + self.GetReg(self.insReg, self.insW);
                if (self.insD)
                {
                    self.SetModRMData((ushort)result);
                }
                else
                {
                    self.SetReg(self.insReg, (ushort)result, self.insW);
                }
            }
            else if ((self.insByte & 0b1111_1100) == 0b1000_0011)
            {
                // 1000 00sw | mod 010 r/m | data | data if s:w=01
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
                result += self.GetModRMData() + self.ins_data;
                self.SetModRMData((ushort)result);
            }
            else if ((self.insByte & 0b1111_1110) == 0b0001_0100)
            {
                // 0001 010w | data | data if w=1
                //   Immediate to accumulator
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word);

                // Add immediate to accumulator
                result += self.ax + self.data(ref csip, w);
                self.ax = (ushort)result;
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

            // 1111111w mod 000 rm
            // Register/memory
            // Part of Group 2 instructions
            if ((insByte & 0b1111_1110) == 0x1111_1110)
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );
                Debug.Assert(insExtOpcode == 0b000);
                result = self.GetModRMData() + 1;
                self.SetModRMData((ushort)result);
            }
            // 01000 reg
            // Reg
            else if ((insByte & 0b1111_1000) == 0b0100_0000)
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.Reg
                );
                self.insW = true; // Yes or no?? Needed for addition flags
                result = self.GetReg(insReg, self.insW) + 1;
                self.SetReg(insReg, (ushort)result, self.insW)
            }

            var prevCarry = self.CF;
            self.SetAdditionFlags(result);
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

            // ASCII adjust for add
            // 00110111
            // 4 clocks

            // Logic: https://en.wikipedia.org/wiki/Intel_BCD_opcode
            //        https://web.archive.org/web/20190203181246/http://www.jaist.ac.jp/iscenter-new/mpc/altix/altixdata/opt/intel/vtune/doc/users_guide/mergedProjects/analyzer_ec/mergedProjects/reference_olh/mergedProjects/instructions/instruct32_hh/vc2a.htm
            //        https://web.archive.org/web/20081102170717/http://webster.cs.ucr.edu/AoA/Windows/HTML/AdvancedArithmetica6.html#1000255
            if ((al & 0x0F) >= 10)
            {
                al = (byte)((al + 6) & 0xF);
                ah += 1;
                flags |= Flags.CF | Flags.AF;
            }
            else
            {
                flags &= ~(Flags.CF | Flags.AF);
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

            // Decimal adjust for add
            // 00100111

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
            
            // r/m and r to either
            // 001010dw mod reg r/m

            // imm to reg/mem
            // 100000sw mod 101 r/m data, data if s:w=01
            // Part of Immediate group

            // imm from accum
            // 0010110w data, data if w=1
            throw new NotImplementedException();
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
            
            // Subtract with borrow

            // r/m and r to either
            // 000110dw mod reg r/m

            // imm to reg/mem
            // 100000sw mod 011 r/m data, data if s:w=01
            // Part of Immediate group

            // imm from accum
            // 0001110w data, data if w=1
            throw new NotImplementedException();
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

            // 1111111w mod 001 r/m
            // Part of Group 2 instructions

            // 01001 reg

            throw new NotImplementedException();
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
            
            // 1111011w mod 011 r/m
            // Part of Group 1 instructions

            throw new NotImplementedException();
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

            // Register/memory and register
            // 001110dw mod reg r/m

            // Immediate with register/memory
            // 100000sw mod 111 r/m data, data if s:w =01
            // Part of Immediate group

            // Immediate with accumulator
            // 0011110w data, data if w=1

            throw new NotImplementedException();
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
            // 00111111 -- ASCII adjust for subtract
            // 4 clocks
            throw new NotImplementedException();
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
            // U   XXXXX

            // Decimal adjust for subtract
            // 00101111
            throw new NotImplementedException();
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
            
            // Multiply unsigned
            // 1111011w mod 100 r/m
            // Part of Group 1 instructions
            throw new NotImplementedException();
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
            
            // Integer multiply (signed)
            // 1111011w mod 101 r/m
            // Part of Group 1 instructions
            throw new NotImplementedException();
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
            // ASCII adjust for multiply
            // 11010100 00001010
            // Second byte is always the same and is just "there"
            // 83 clocks
            // Description: Intel 8086 Family User's Manual October 1979, p. 2-36
            throw new NotImplementedException();
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

            // Divide (unsigned)
            // 1111011w mod 110 r/m
            // Part of Group 1 instructions
            throw new NotImplementedException();
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
            
            // Integer divide (signed)
            // 1111011w mod 111 r/m
            // Part of Group 1 instructions
            throw new NotImplementedException();
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
            // ASCII adjust for divide
            // 11010101 00001010
            // Second byte is always the same and is just "there"
            // 60 clocks
            // Description: Intel 8086 Family User's Manual October 1979, p. 2-36
            throw new NotImplementedException();
        }

        private static void HandleCBW(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-9

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-38
            // - Table 2-21. Instruction Set Reference Data, p. 2-52
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // Convert byte to word
            // 10011000
            throw new NotImplementedException();
        }

        private static void HandleCWD(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-9

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-38
            // - Table 2-21. Instruction Set Reference Data, p. 2-54
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // Convert word to double word
            // 10011001
            throw new NotImplementedException();
        }
        #endregion

        #region Helpers
        static bool parity_byte(byte data)
        {
            // Count bits set to 1 and return true if even, false if odd.
            // (according to chatgpt)
            data ^= data >> 4;
            data ^= data >> 2;
            data ^= data >> 1;
            return (data & 1) == 0;
        }

        private void SetAdditionFlags(int a, int b, int c, int result)
        {
            // Consider getting these lazily after caching the last result
            ZF = result == 0;
            CF = (result & (insW ? 0x10000 : 0x100)) != 0;
            AF = ((a & 0xF) + (b & 0xF) + c) >= 0x10; // Addition resulting in a carry outside the lower nibble
            SF = (result & (insW ? 0x8000 : 0x80)) != 0;
            PF = parity_byte(result & 0xFF);
            OF = ((a ^ result) & (b ^ result) & (insW ? 0x8000 : 0x80)) != 0; // signed overflow
        }
        #endregion
    }
}
