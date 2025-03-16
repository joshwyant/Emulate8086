using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                self.MovRMToFromR(ref csip);
            }
            else if ((self.insByte & 0b11111110) == 0b11000110)
            {
                // 1100011 w (C6 - C7)
                // Immediate to register/memory
                self.MovImmToRM(ref csip);
            }
            else if ((self.insByte & 0b11110000) == 0b10110000)
            {
                // 1011 w reg (B0 - BF)
                // Immediate to register
                self.MovImmToR(ref csip);
            }
            else if ((self.insByte & 0b11111110) == 0b10100000)
            {
                // 1010000 w (A0 - A1)
                // 
                self.MovMemToAccum(ref csip);
            }
            else if ((self.insByte & 0b11111110) == 0b10100010)
            {
                // 1010001 w (A2 - A3)
                self.MovAccumToMemory(ref csip);
            }
            else if (self.insByte == 0b10001110)
            {
                // 10001110 (8E)
                self.MovRMToSegr(ref csip);
            }
            else
            {
                Debug.Assert(0b == insByte);
                
                // 10001100 (8C)
                self.MovSegrToRM(ref csip);
            }

            self.csip = csip;
        }

        private void MovSegrToRM(ref int csip)
        {
            // 10001100 | mod 0 reg r/m
            // Segment register to register/memory
            DecodeInstruction(
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMSeg
            );

            // Execute
            insW = true; // So 2 bytes of data are written to register/memory
            SetModRMData(GetSeg(segreg));
        }

        private void MovRMToSegr(ref int csip)
        {
            // 10001110 | mod 0 reg r/m
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

        private void MovAccumToMemory(ref int csip)
        {
            // 1010001w | addr low | addr high
            // Accumulator to memory
            DecodeInstruction(
                InstructionDecoderFlags.W,
                InstructionDecoderFlags.Addr
            );
            memory.setDataAt(ins_addr, ax, w);
        }

        private void MovMemToAccum(ref int csip)
        {
            // 1010000w | addr low | addr high
            // Memory to accumulator
            DecodeInstruction(
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.Addr
            );
            
            // Execute
            // AX and AL
            SetReg(Register.AX, ins_addr, w);
        }

        private void MovImmToR(ref int csip)
        {
            // 1011 w reg | data | data if w=1
            // Immediate to register
            DecodeInstruction(
                InstructionDecoderFlags.Reg | 
                InstructionDecoderFlags.W | 
                InstructionDecoderFlags.Byte | 
                InstructionDecoderFlags.Word);

            // Execute
            SetReg(insReg, (ushort)ins_data, insW);
        }

        private void MovImmToRM(ref int csip)
        {
            // 1100011 w | mod 000 rm | data | data if w=1
            // Immediate to register/memory
            DecodeInstruction(
                InstructionDecoderFlags.W | 
                InstructionDecoderFlags.ModRM | 
                InstructionDecoderFlags.ModRMOpcode |
                InstructionDecoderFlags.Byte | 
                InstructionDecoderFlags.Word);

            // Execute
            if (insExtOpcode == 0b000)
            {
                SetModRMData(ins_data);
            }
        }

        private void MovRMToFromR(ref int csip)
        {
            // 100010 d w | mod reg r/m
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
