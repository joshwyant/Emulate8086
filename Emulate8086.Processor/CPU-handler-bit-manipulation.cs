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
        #region Logical
        private static void HandleNOT(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-9

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-38
            // - Table 2-21. Instruction Set Reference Data, p. 2-62
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            Debug.Assert(0b1111_011_0 == (self.insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );

            // 1111011w (F6 - F7) mod 010 r/m
            // Part of Group 1 instructions

            Debug.Assert(self.insExtOpcode == 0b010);

            // Get the value, perform bitwise NOT, and store it back
            ushort value = self.GetModRMData();
            value = (ushort)(~value);
            self.SetModRMData(value);

            // NOT doesn't affect any flags
        }

        private static void HandleAND(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-9

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-38
            // - Table 2-21. Instruction Set Reference Data, p. 2-52
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            // ODITSZAPC
            // 0   XXUX0

            ushort dest = 0, src = 0, result = 0;

            if (0b0010_00_00 == (self.insByte & 0b11111100))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.D |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMReg
                );

                // Register/memory and register to either
                // 001000dw (20 - 23) mod reg r/m

                if (self.insD)
                {
                    // reg is destination
                    dest = self.GetReg(self.insReg, self.insW);
                    src = self.GetModRMData();
                    result = (ushort)(dest & src);
                    self.SetReg(self.insReg, result, self.insW);
                }
                else
                {
                    // r/m is destination
                    dest = self.GetModRMData();
                    src = self.GetReg(self.insReg, self.insW);
                    result = (ushort)(dest & src);
                    self.SetModRMData(result);
                }
            }
            else if (0b1000_00_00 == (self.insByte & 0b11111100))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.S |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immediate to register/memory
                // 100000sw (80 - 81) mod 100 r/m data, data if w=1 and s=0

                Debug.Assert(self.insExtOpcode == 0b100);

                dest = self.GetModRMData();
                src = (ushort)self.ins_data;
                result = (ushort)(dest & src);
                self.SetModRMData(result);
            }
            else
            {
                Debug.Assert(0b0010_010_0 == (self.insByte & 0b11111110));

                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immediate to accumulator
                // 0010010w (24 - 25) data, data if w=1

                dest = self.insW ? self.ax : self.al;
                src = (ushort)self.ins_data;
                result = (ushort)(dest & src);
                self.SetReg(Register.AX, (ushort)result, self.insW);
            }

            // Set flags for logical operations
            self.SetLogicalFlags(result);
        }

        private static void HandleOR(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-10

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-38
            // - Table 2-21. Instruction Set Reference Data, p. 2-62
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            // ODITSZAPC
            // 0   XXUX0

            ushort dest = 0, src = 0, result = 0;

            if (0b0000_10_00 == (self.insByte & 0b11111100))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.D |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMReg
                );

                // Register/memory and register to either
                // 000010dw (08 - 0B) mod reg r/m

                if (self.insD)
                {
                    // reg is destination
                    dest = self.GetReg(self.insReg, self.insW);
                    src = self.GetModRMData();
                    result = (ushort)(dest | src);
                    self.SetReg(self.insReg, result, self.insW);
                }
                else
                {
                    // r/m is destination
                    dest = self.GetModRMData();
                    src = self.GetReg(self.insReg, self.insW);
                    result = (ushort)(dest | src);
                    self.SetModRMData(result);
                }
            }
            else if (0b1000_000_0 == (self.insByte & 0b11111100))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.S |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immedate to register/memory
                // 100000sw (80 - 83) mod 001 r/m data, data if w=1 and s=0
                // Part of Immediate group

                Debug.Assert(self.insExtOpcode == 0b001);

                dest = self.GetModRMData();
                src = (ushort)self.ins_data;
                result = (ushort)(dest | src);
                self.SetModRMData(result);
            }
            else
            {
                Debug.Assert(0b0000_110_0 == (self.insByte & 0b11111110));

                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immediate to accumulator
                // 0000110w (0C - 0D) data data if w = 1

                dest = self.insW ? self.ax : self.al;
                src = (ushort)self.ins_data;
                result = (ushort)(dest | src);
                self.SetReg(Register.AX, (ushort)result, self.insW);
            }

            // Set flags for logical operations
            self.SetLogicalFlags(result);
        }

        private static void HandleXOR(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-10

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-38
            // - Table 2-21. Instruction Set Reference Data, p. 2-68
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            // ODITSZAPC
            // 0   XXUX0

            ushort dest = 0, src = 0, result = 0;

            if (0b0011_00_00 == (self.insByte & 0b11111100))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.D |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMReg
                );

                // Register/memory and register to either
                // 001100dw (30 - 33) mod reg r/m

                if (self.insD)
                {
                    // reg is destination
                    dest = self.GetReg(self.insReg, self.insW);
                    src = self.GetModRMData();
                    result = (ushort)(dest ^ src);
                    self.SetReg(self.insReg, result, self.insW);
                }
                else
                {
                    // r/m is destination
                    dest = self.GetModRMData();
                    src = self.GetReg(self.insReg, self.insW);
                    result = (ushort)(dest ^ src);
                    self.SetModRMData(result);
                }
            }
            else if (0b1000_000_0 == (self.insByte & 0b11111110))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immediate to register/memory
                // 1000000w (80 - 81) mod 110 r/m data, data if w=1
                // Part of Immediate group

                Debug.Assert(self.insExtOpcode == 0b110);

                dest = self.GetModRMData();
                src = (ushort)self.ins_data;
                result = (ushort)(dest ^ src);
                self.SetModRMData(result);
            }
            else
            {
                Debug.Assert(0b0011_010_0 == (self.insByte & 0b1111_1110));

                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immediate to accumulator
                // 0011010w (34 - 35) data, data if w=1

                dest = self.insW ? self.ax : self.al;
                src = (ushort)self.ins_data;
                result = (ushort)(dest ^ src);
                self.SetReg(Register.AX, (ushort)result, self.insW);
            }

            // Set flags for logical operations
            self.SetLogicalFlags(result);
        }

        private static void HandleTEST(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-10

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-39
            // - Table 2-21. Instruction Set Reference Data, p. 2-67
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            // ODITSZAPC
            // 0   XXUX0

            // And function to flags, no result
            ushort value1 = 0, value2 = 0, result = 0;

            if (0b1000_010_0 == (self.insByte & 0b1111_1110))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMReg
                );

                // R/m and register
                // 1000010w (84 - 85) mod reg r/m

                value1 = self.GetModRMData();
                value2 = self.GetReg(self.insReg, self.insW);
                result = (ushort)(value1 & value2);
            }
            else if (0b1111_011_0 == (self.insByte & 0b1111_1110))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immediate data and register/memory
                // 1111011w (F6 - F7) mod 000 r/m data, data if w=1
                // Part of Group 1 instructions

                Debug.Assert(self.insExtOpcode == 0b000);

                value1 = self.GetModRMData();
                value2 = (ushort)self.ins_data;
                result = (ushort)(value1 & value2);
            }
            else
            {
                Debug.Assert(0b1010_100_0 == (self.insByte & 0b1111_1110));

                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immediate data and accumulator
                // 1010100w (A8 - A9) data, data if w=1

                value1 = self.insW ? self.ax : self.al;
                value2 = (ushort)self.ins_data;
                result = (ushort)(value1 & value2);
            }

            // TEST only sets flags, doesn't store result
            self.SetLogicalFlags(result);
        }
        #endregion

        #region Shifts
        private static void HandleSHL_SAL(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-9

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-39
            // - Table 2-21. Instruction Set Reference Data, p. 2-65
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // X       X

            Debug.Assert(0b1101_00_00 == (self.insByte & 0b1111_1100));

            self.DecodeInstruction(
                InstructionDecoderFlags.V |
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );

            // 110100vw (D0 - D3) mod 100 r/m
            // Part of Shift group

            Debug.Assert(self.insExtOpcode == 0b100);

            // Get the value to shift and the count
            ushort value = self.GetModRMData();
            int count = self.insV ? self.cl : 1;

            // Mask count to 5 bits (0-31) as per 8086 behavior
            count &= 0x1F;

            if (count > 0)
            {
                ushort result = 0;
                bool lastBit = false; // Last bit shifted out

                // Perform the shift
                if (self.insW)
                {
                    // Word operation
                    lastBit = ((value << (count - 1)) & 0x8000) != 0;
                    result = (ushort)(value << count);
                }
                else
                {
                    // Byte operation
                    lastBit = ((byte)(value << (count - 1)) & 0x80) != 0;
                    result = (ushort)((byte)value << count);
                }

                // Update value
                self.SetModRMData(result);

                // Set flags
                self.SetLogicalFlags(result);
                self.CF = lastBit;
                self.OF = (count == 1) ? ((result & self.GetSignMask()) != 0) != ((value & self.GetSignMask()) != 0) : false;
            }
        }

        private static void HandleSHR(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-9

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-39
            // - Table 2-21. Instruction Set Reference Data, p. 2-66

            // ODITSZAPC
            // X       X

            Debug.Assert(0b1101_00_00 == (self.insByte & 0b1111_1100));

            self.DecodeInstruction(
                InstructionDecoderFlags.V |
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );

            // Shift logical right
            // 110100vw (D0 - D3) mod 101 r/m
            // Part of Shift group

            Debug.Assert(self.insExtOpcode == 0b101);

            // Get the value to shift and the count
            ushort value = self.GetModRMData();
            int count = self.insV ? self.cl : 1;

            // Mask count to 5 bits (0-31) as per 8086 behavior
            count &= 0x1F;

            if (count > 0)
            {
                ushort result = 0;
                bool lastBit = false; // Last bit shifted out

                // Perform the shift
                if (self.insW)
                {
                    // Word operation
                    lastBit = ((value >> (count - 1)) & 0x0001) != 0;
                    result = (ushort)(value >> count);
                }
                else
                {
                    // Byte operation
                    lastBit = ((byte)(value >> (count - 1)) & 0x01) != 0;
                    result = (ushort)((byte)value >> count);
                }

                // Update value
                self.SetModRMData(result);

                // Set flags
                self.SetLogicalFlags(result);
                self.CF = lastBit;
                self.OF = (count == 1) ? ((value & self.GetSignMask()) != 0) : false;
            }
        }

        private static void HandleSAR(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-9

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-39
            // - Table 2-21. Instruction Set Reference Data, p. 2-65
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // X   XXUXX

            Debug.Assert(0b1101_00_00 == (self.insByte & 0b1111_1100));

            self.DecodeInstruction(
                InstructionDecoderFlags.V |
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );

            // 110100vw (D0 - D3) mod 111 r/m
            // Part of Shift group

            Debug.Assert(self.insExtOpcode == 0b111);

            // Get the value to shift and the count
            ushort value = self.GetModRMData();
            int count = self.insV ? self.cl : 1;

            // Mask count to 5 bits (0-31) as per 8086 behavior
            count &= 0x1F;

            if (count > 0)
            {
                ushort result = 0;
                bool lastBit = false; // Last bit shifted out
                uint signMask = (uint)(self.insW ? 0x8000 : 0x80);
                bool signBit = (value & signMask) != 0;

                // Perform the arithmetic shift (preserving sign bit)
                if (self.insW)
                {
                    // Word operation
                    lastBit = ((value >> (count - 1)) & 0x0001) != 0;

                    // Perform arithmetic right shift preserving sign bit
                    short signedVal = (short)value;
                    signedVal >>= count;
                    result = (ushort)signedVal;
                }
                else
                {
                    // Byte operation
                    lastBit = ((byte)(value >> (count - 1)) & 0x01) != 0;

                    // Perform arithmetic right shift preserving sign bit
                    sbyte signedVal = (sbyte)(byte)value;
                    signedVal >>= count;
                    result = (ushort)(byte)signedVal;
                }

                // Update value
                self.SetModRMData(result);

                // Set flags
                self.SetLogicalFlags(result);
                self.CF = lastBit;
                self.OF = (count == 1) ? false : false; // SAR never sets OF with count=1
            }
        }
        #endregion

        #region Rotates
        private static void HandleROL(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-9

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-39
            // - Table 2-21. Instruction Set Reference Data, p. 2-64
            // - Table 4-12. 8086 Instruction Encoding, p. 4-24

            // ODITSZAPC
            // X       X

            Debug.Assert(0b1101_00_00 == (self.insByte & 0b1111_1100));

            self.DecodeInstruction(
                InstructionDecoderFlags.V |
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );

            // 110100vw (D0 - D3) mod 000 r/m
            // Part of Shift group

            Debug.Assert(self.insExtOpcode == 0b000);

            // Get the value to rotate and the count
            ushort value = self.GetModRMData();
            int count = self.insV ? self.cl : 1;

            // Mask count appropriately
            if (self.insW)
                count %= 16; // Word
            else
                count %= 8;  // Byte

            if (count > 0)
            {
                ushort result = 0;

                // Perform the rotation
                if (self.insW)
                {
                    // Word operation
                    result = (ushort)((value << count) | (value >> (16 - count)));
                }
                else
                {
                    // Byte operation
                    byte byteVal = (byte)value;
                    byte rotated = (byte)((byteVal << count) | (byteVal >> (8 - count)));
                    result = rotated;
                }

                // Update value
                self.SetModRMData(result);

                // Set flags
                uint signMask = (uint)(self.insW ? 0x8000 : 0x80);
                self.CF = (result & 0x01) != 0; // Lowest bit
                if (count == 1)
                    self.OF = (result & signMask) != 0 != self.CF; // OF = MSB XOR CF
            }
        }

        private static void HandleROR(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-9

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-40
            // - Table 2-21. Instruction Set Reference Data, p. 2-64
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            // ODITSZAPC
            // X       X

            Debug.Assert(0b1101_00_00 == (self.insByte & 0b1111_1100));

            self.DecodeInstruction(
                InstructionDecoderFlags.V |
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );

            // 110100vw (D0 - D3) mod 001 r/m
            // Part of Shift group

            Debug.Assert(self.insExtOpcode == 0b001);

            // Get the value to rotate and the count
            ushort value = self.GetModRMData();
            int count = self.insV ? self.cl : 1;

            // Mask count appropriately
            if (self.insW)
                count %= 16; // Word
            else
                count %= 8;  // Byte

            if (count > 0)
            {
                ushort result = 0;

                // Perform the rotation
                if (self.insW)
                {
                    // Word operation
                    result = (ushort)((value >> count) | (value << (16 - count)));
                }
                else
                {
                    // Byte operation
                    byte byteVal = (byte)value;
                    byte rotated = (byte)((byteVal >> count) | (byteVal << (8 - count)));
                    result = rotated;
                }

                // Update value
                self.SetModRMData(result);

                // Set flags
                uint signMask = (uint)(self.insW ? 0x8000 : 0x80);
                uint msbMinus1 = (uint)(self.insW ? 0x4000 : 0x40);
                self.CF = (result & signMask) != 0; // Highest bit
                if (count == 1)
                    self.OF = ((result & signMask) != 0) != ((result & msbMinus1) != 0); // OF = MSB XOR MSB-1
            }
        }

        private static void HandleRCL(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-9

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-40
            // - Table 2-21. Instruction Set Reference Data, p. 2-63
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            // ODITSZAPC
            // X       X

            Debug.Assert(0b1101_00_00 == (self.insByte & 0b1111_1100));

            self.DecodeInstruction(
                InstructionDecoderFlags.V |
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );

            // Rotate through carry left
            // 110100vw (D0 - D3) mod 010 r/m
            // Part of Shift group

            Debug.Assert(self.insExtOpcode == 0b010);

            // Get the value to rotate and the count
            ushort value = self.GetModRMData();
            int count = self.insV ? self.cl : 1;

            // Mask count appropriately
            if (self.insW)
                count %= 17; // Word + carry bit
            else
                count %= 9;  // Byte + carry bit

            if (count > 0)
            {
                ushort result = value;
                bool oldCF = self.CF;

                // Perform the rotation through carry bit
                for (int i = 0; i < count; i++)
                {
                    bool highBit = false;

                    if (self.insW)
                        highBit = (result & 0x8000) != 0;
                    else
                        highBit = (result & 0x80) != 0;

                    // Shift left
                    result <<= 1;

                    // Put old CF into bit 0
                    if (oldCF)
                        result |= 0x01;

                    // Update CF
                    oldCF = highBit;
                }

                // Mask result for byte operations
                if (!self.insW)
                    result &= 0xFF;

                // Update value
                self.SetModRMData(result);

                // Set flags
                uint signMask = (uint)(self.insW ? 0x8000 : 0x80);
                self.CF = oldCF;
                if (count == 1)
                    self.OF = ((result & signMask) != 0) != oldCF; // OF = MSB XOR CF after operation
            }
        }

        private static void HandleRCR(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-9

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-40
            // - Table 2-21. Instruction Set Reference Data, p. 2-63
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            // ODITSZAPC
            // X       X

            Debug.Assert(0b1101_00_00 == (self.insByte & 0b1111_1100));

            self.DecodeInstruction(
                InstructionDecoderFlags.V |
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );

            // Rotate through carry right
            // 110100vw (D0 - D3) mod 011 r/m
            // Part of Shift group

            Debug.Assert(self.insExtOpcode == 0b011);

            // Get the value to rotate and the count
            ushort value = self.GetModRMData();
            int count = self.insV ? self.cl : 1;

            // Mask count appropriately
            if (self.insW)
                count %= 17; // Word + carry bit
            else
                count %= 9;  // Byte + carry bit

            if (count > 0)
            {
                ushort result = value;
                bool oldCF = self.CF;

                // Save MSB for OF calculation when count=1
                uint signMask = (uint)(self.insW ? 0x8000 : 0x80);
                bool oldMSB = (result & signMask) != 0;

                // Perform the rotation through carry bit
                for (int i = 0; i < count; i++)
                {
                    bool lowBit = (result & 0x01) != 0;

                    // Shift right
                    if (self.insW)
                        result >>= 1;
                    else
                        result = (ushort)((byte)result >> 1);

                    // Put old CF into high bit
                    if (oldCF)
                    {
                        if (self.insW)
                            result |= 0x8000;
                        else
                            result |= 0x80;
                    }

                    // Update CF
                    oldCF = lowBit;
                }

                // Update value
                self.SetModRMData(result);

                // Set flags
                self.CF = oldCF;

                // For RCR, OF is set if the MSB changes in the first rotation
                if (count == 1)
                    self.OF = oldMSB != ((result & signMask) != 0);
            }
        }
        #endregion

        #region Helpers
        private void SetLogicalFlags(ushort result)
        {
            // Set flags for logical operations
            CF = false;  // Cleared
            OF = false;  // Cleared
            ZF = result == 0;
            SF = ((insW ? result & 0x8000 : result & 0x80) != 0);
            PF = parity_byte((byte)result);
        }

        private uint GetSignMask()
        {
            // Return the mask for the sign bit based on operand size
            return insW ? 0x8000u : 0x80u;
        }
        #endregion
    }
}
