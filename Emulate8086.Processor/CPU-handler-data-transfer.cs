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
        private void push(short data)
        {
            memory[ss, --sp] = (byte)((data >> 8) & 0xFF);
            memory[ss, --sp] = (byte)(data & 0xFF);
        }

        private short pop()
        {
            var lo = memory[ss, sp++];
            var hi = memory[ss, sp++];
            return (short)((hi << 8) | lo);
        }

        #region General Purpose Data Transfers
        private static void HandlePUSH(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-5

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-31
            // - Table 2-21. Instruction Set Reference Data, p. 2-63
            // - Table 4-12. 8086 Instruction Encoding, p. 4-22
            // - Table 4-13. Machine Instruction Decoding Guide, p. 4-27

            if (self.insByte == 0xFF)
            {
                // Decode
                self.DecodeInstruction(
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode
                );

                // 11111111 mod 110 rm
                // Register/memory
                // Part of Group 2 instructions

                Debug.Assert(self.insExtOpcode == 0b110);

                // Execute
                self.push((short)self.GetModRMData());
            }
            else if (self.insByte >> 3 == 0b01010)
            {
                // Decode
                self.DecodeInstruction(
                    InstructionDecoderFlags.Reg
                );

                // 01010 reg
                // Register

                // Execute
                self.push((short)self.GetReg16(self.insReg));
            }
            else
            {
                Debug.Assert(self.insByte >> 5 == 0b000);
                self.DecodeInstruction(
                    InstructionDecoderFlags.Seg
                );

                // 000 seg 110
                // Segment register
                self.push((short)self.GetSeg(self.insReg));
            }
        }

        private static void HandlePOP(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-5

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-31
            // - Table 2-21. Instruction Set Reference Data, p. 2-62            
            // - Table 4-12. 8086 Instruction Encoding, p. 4-22

            if (self.insByte == 0b10001111)
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode
                );

                // 10001111 mod 000 rm
                // Register/memory

                Debug.Assert(self.insExtOpcode == 0b000);

                self.SetModRMData((ushort)self.pop());
            }
            else if (self.insByte >> 3 == 0b01011)
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.Reg
                );

                // 01011 reg
                // Register

                self.SetReg(self.insReg, (ushort)self.pop());
            }
            else
            {
                Debug.Assert((self.insByte & 0b111_00_111) == 0b000_00_111);
                self.DecodeInstruction(
                    InstructionDecoderFlags.Seg
                );
                // 000 seg 111
                // Segment register
                self.SetSeg(self.insReg, (ushort)self.pop());
            }
        }

        private static void HandleXCHG(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-32
            // - Table 2-21. Instruction Set Reference Data, p. 2-67
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            if ((self.insByte & 0b1111111_0) == 0b1000011_0)
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMReg
                );

                // 1000011w mod reg rm
                // Register/memory with register
                var reg = self.GetReg(self.insReg, self.insW);
                var src = self.GetModRMData();
                self.SetReg(self.insReg, src, self.insW);
                self.SetModRMData(reg);
            }
            else
            {
                Debug.Assert((self.insByte >> 3) == 0b10010);
                self.DecodeInstruction(
                    InstructionDecoderFlags.Reg
                );
                // 10010 reg 
                // Register with accumulator
                var accum = self.ax;
                var reg = self.GetReg16(self.insReg);
                self.ax = reg;
                self.SetReg16(self.insReg, accum);
            }
        }

        private static void HandleXLAT(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-32
            // - Table 2-21. Instruction Set Reference Data, p. 2-67
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            Debug.Assert(0b1101_0111 == self.insByte);

            // translate byte to al
            // 11010111
            // Replace AL with byte from 256-byte table pointed to by bx.
            // TODO: Take segment prefix into account?
            self.al = self.memory[self.bx + self.al];
        }

        private static void HandleIN(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-32
            // - Table 2-21. Instruction Set Reference Data, p. 2-55
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            // Input to AL/AX from...

            var port = 0;
            if ((self.insByte & 0b1111_111_0) == 0b1110_010_0)
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte
                );

                // 1110 010w | port
                // Fixed port (0-255)
                port = self.ins_data;
            }
            else
            {
                Debug.Assert((self.insByte & 0b1111_111_0) == 0b1110_110_0);
                self.DecodeInstruction(
                    InstructionDecoderFlags.W
                );
                // 1110 110w
                // Variable port (DX)
                port = self.dx;
            }

            if (!self.devices.ContainsKey(port))
            {
                self.SetReg(Register.AX, 0, self.insW);
            }
            else
            {
                var device = self.devices[port];
                device.In(port, self.GetReg(Register.AX, self.insW));
            }
        }

        private static void HandleOUT(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-32
            // - Table 2-21. Instruction Set Reference Data, p. 2-62
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            // Output from AL/AX to...

            var port = 0;
            if ((self.insByte & 0b1111_111_0) == 0b1110_011_0)
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte // TODO: Make sure this works
                );
                // 1110011w port
                // Fixed port (0-255)
                port = self.ins_data;
            }
            else
            {
                Debug.Assert((self.insByte & 0b1111_111_0) == 0b1110_110_0);
                self.DecodeInstruction(
                    InstructionDecoderFlags.W
                );
                // 1110110w
                // Variable port (DX)
                port = self.dx;
            }

            int output = self.GetReg(Register.AX, self.insW);
            if (self.devices.ContainsKey(port))
            {
                var device = self.devices[port];
                device.Out(port, ref output);
            }
        }
        #endregion

        #region Address Object Transfers
        private static void HandleLEA(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-32
            // - Table 2-21. Instruction Set Reference Data, p. 2-59
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            Debug.Assert(0b1000_1101 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMReg
            );

            // Load effective address to register
            // 10001101 mod reg r/m
            self.SetReg(self.insReg, (ushort)self.modrm_addr); // or eff_addr?
        }

        private static void HandleLDS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-32
            // - Table 2-21. Instruction Set Reference Data, p. 2-59

            Debug.Assert(0b1100_0101 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMReg
            );

            // Load pointer to DS
            // 11000101 mod reg r/m

            // Read offset (first word) and segment (second word)
            var offset = self.memory.wordAt(self.modrm_addr);
            var segment = self.memory.wordAt(self.modrm_addr + 2);

            // Set register to offset
            self.SetReg16(self.insReg, offset);

            // Set DS segment register to segment value
            self.SetSeg(Register.DS, segment);
        }

        private static void HandleLES(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-32
            // - Table 2-21. Instruction Set Reference Data, p. 2-59
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            Debug.Assert(0b1100_0100 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMReg
            );

            // Load pointer to ES
            // 11000100 mod reg r/m

            // Read offset (first word) and segment (second word)
            var offset = self.memory.wordAt(self.modrm_addr);
            var segment = self.memory.wordAt(self.modrm_addr + 2);

            // Set register to offset
            self.SetReg16(self.insReg, offset);

            // Set ES segment register to segment value
            self.SetSeg(Register.ES, segment);
        }
        #endregion

        #region Flag Transfers
        private static void HandleLAHF(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-32
            // - Table 2-21. Instruction Set Reference Data, p. 2-59
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            Debug.Assert(0b1001_1111 == self.insByte);

            // Load AH with flags
            // 10011111

            // Get lower byte of flags register and store in AH
            self.SetReg8(Register.AH, (byte)self.flags);
        }

        private static void HandleSAHF(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-33
            // - Table 2-21. Instruction Set Reference Data, p. 2-64
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            // ODITSZAPC
            //     RRRRR

            Debug.Assert(0b1001_1110 == self.insByte);

            // Store AH into flags
            // 10011110

            // Get value from AH and place in lower byte of flags
            byte ah = self.GetReg8(Register.AH);
            self.flags = (Flags)((int)self.flags & 0xFF00) | (Flags)ah;
        }

        private static void HandlePUSHF(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-33
            // - Table 2-21. Instruction Set Reference Data, p. 2-63
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            Debug.Assert(0b1001_1100 == self.insByte);

            // 1001 1100
            // Push flags

            // Push flags register onto stack
            self.push((short)self.flags);
        }

        private static void HandlePOPF(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-33
            // - Table 2-21. Instruction Set Reference Data, p. 2-63
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            // ODITSZAPC
            // RRRRRRRRR

            Debug.Assert(0b1001_1101 == self.insByte);

            // 1001 1101
            // Pop flags

            // Pop value from stack and set as flags register
            self.flags = (Flags)self.pop();
        }
        #endregion
    }
}
