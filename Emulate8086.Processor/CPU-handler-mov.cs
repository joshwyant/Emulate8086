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
        // From Intel 8086 Family User's Manual October 1979:
        //   ...the D field, generally specifies the "direction" of the
        //   operation: 1 = the REG field in second byte identifies the
        //   destination operand, 0 = the REG field identifies the source
        //   operand.
        //   - Pg. 4-18, Hardware Reference Information - Machine Instruction
        //     Encoding and Decoding
        // From IBM Personal Computer Hardware Reference Library - Technical
        // Reference:
        //   if d = 1, then "to"; if d = 0 then "from"
        //   - Pg. B-15, 8088 Instruction Set Reference

        private static void HandleMOV(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, Data Transfer, p. B-5

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-31
            // - Table 2-21. Instruction Set Reference Data, p. 2-61
            // - Table 4-12. 8086 Instruction Encoding, p. 4-22

            var csip = self.csip;

            if ((self.insByte & 0b11111100) == 0b10001000)
            {
                // 100010 d w (88 - 8B)
                // Register/memory to/from register
                self.MovRMToFromR();
            }
            else if ((self.insByte & 0b11111110) == 0b11000110)
            {
                // 1100011 w (C6 - C7)
                // Immediate to register/memory
                self.MovImmToRM();
            }
            else if ((self.insByte & 0b11110000) == 0b10110000)
            {
                // 1011 w reg (B0 - BF)
                // Immediate to register
                self.MovImmToR();
            }
            else if ((self.insByte & 0b11111110) == 0b10100000)
            {
                // 1010000 w (A0 - A1)
                // 
                self.MovMemToAccum();
            }
            else if ((self.insByte & 0b11111110) == 0b10100010)
            {
                // 1010001 w (A2 - A3)
                self.MovAccumToMemory();
            }
            else if (self.insByte == 0b10001110)
            {
                // 10001110 (8E)
                self.MovRMToSegr();
            }
            else
            {
                Debug.Assert(0b10001100 == self.insByte);

                // 10001100 (8C)
                self.MovSegrToRM();
            }
        }

        private void MovSegrToRM()
        {
            // 10001100 (8C) | mod 0 reg r/m
            // Segment register to register/memory
            DecodeInstruction(
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMSeg
            );

            // Execute
            insW = true; // So 2 bytes of data are written to register/memory
            SetModRMData(GetSeg(insReg));
        }

        private void MovRMToSegr()
        {
            // 10001110 (8E) | mod 0 reg r/m
            // Register/memory to segment register
            DecodeInstruction(
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMSeg
            );

            // Execute
            insW = true; // So 2 bytes of data are read from register/memory
            var data = GetModRMData();
            SetSeg(insReg, data);
        }

        private void MovAccumToMemory()
        {
            // 1010001w (A2 - A3) | addr low | addr high
            // Accumulator to memory
            DecodeInstruction(
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.Addr
            );
            memory.setDataAt(ins_addr, ax, insW);
        }

        private void MovMemToAccum()
        {
            // 1010000w (A0 - A1) | addr low | addr high
            // Memory to accumulator
            DecodeInstruction(
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.Addr
            );

            // Execute
            // AX and AL
            var val = memory.dataAt(ins_addr, insW); // TODO: Segment prefix?
            SetReg(Register.AX, val, insW);
        }

        private void MovImmToR()
        {
            // 1011 w reg (B0 - BF) | data | data if w=1
            // Immediate to register
            DecodeInstruction(
                InstructionDecoderFlags.Reg |
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.Byte |
                InstructionDecoderFlags.Word);

            // Execute
            SetReg(insReg, (ushort)ins_data, insW);
        }

        private void MovImmToRM()
        {
            // 1100011 w (C3) | mod 000 rm | data | data if w=1
            // Immediate to register/memory
            DecodeInstruction(
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode |
                InstructionDecoderFlags.Byte |
                InstructionDecoderFlags.Word);

            // Execute
            Debug.Assert(insExtOpcode == 0b000);
            SetModRMData(ins_data);
        }

        private void MovRMToFromR()
        {
            // 100010 d w (88 - 8B) | mod reg r/m
            // Register/memory to/from register
            DecodeInstruction(
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.D |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMReg
            );

            // Execute
            if (insD)
            {
                // Register/memory to register
                var from = GetModRMData();
                SetReg(insReg, from, insW);
            }
            else
            {
                // Register/memory from register
                var data = GetReg(insReg, insW);
                SetModRMData(data);
            }
        }
    }
}
