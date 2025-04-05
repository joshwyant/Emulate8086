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
        #region Unconditional Transfers
        private static void HandleCALL(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-11

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-43
            // - Table 2-21. Instruction Set Reference Data, p. 2-52
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            // https://faydoc.tripod.com/cpu/call.htm

            ushort newOffset = 0;
            ushort newSegment = 0;
            bool intersegment = false;

            switch (self.insByte)
            {
                case 0b1110_1000:

                    self.DecodeInstruction(
                        InstructionDecoderFlags.DispW
                    );
                    // Direct within segment
                    // 11101000 (E8) disp-low disp-high
                    newOffset = (ushort)(self.ip + self.disp);  // TODO: use start IP?
                    break;
                case 0b1111_1111:
                    self.DecodeInstruction(
                        InstructionDecoderFlags.ModRM |
                        InstructionDecoderFlags.ModRMOpcode
                    );
                    if (self.insExtOpcode == 0b010)
                    {
                        // Indirect within segment
                        // 11111111 (FF) mod 010 r/m
                        // Part of Group 2 instructions

                        // Get address from r/m
                        newOffset = self.GetModRMData();
                    }
                    else
                    {
                        // Indirect intersegment
                        // 11111111 (FF) mod 011 r/m
                        // Part of Group 2 instructions
                        Debug.Assert(self.insExtOpcode == 0b011);

                        // Get far pointer from memory
                        // https://www.os2museum.com/wp/undocumented-8086-opcodes/comment-page-1/#comment-87135
                        // For LEA (and CALL FAR AX), ea is still the last computed address. AX in CALL FAR AX is not read.
                        ushort ea = self.modrm_eff_addr;
                        newOffset = self.memory.wordAt(self.modrm_seg_addr, ea);
                        newSegment = self.memory.wordAt(self.modrm_seg_addr, (ushort)(ea + 2));
                        intersegment = true;
                    }
                    break;
                case 0b1001_1010:
                    self.DecodeInstruction(
                        InstructionDecoderFlags.AddL
                    );
                    // Direct intersegment
                    // 10011010 (9A) offs-low offs-hi seg-lo seg-hi
                    newOffset = self.ins_eff_addr;
                    newSegment = self.ins_seg;
                    intersegment = true;
                    break;
                default:
                    // Maybe a CALL FAR AL or something?
                    // Like https://github.com/dbalsom/martypc/issues/128
                    Debug.Assert(false);
                    break;
            }

            // Push return address onto stack
            self.sp -= 2;
            self.memory.setWordAt(self.ss, self.sp, self.ip);

            if (intersegment)
            {
                // For intersegment, also push CS
                self.sp -= 2;
                self.memory.setWordAt(self.ss, self.sp, self.cs);

                // Update CS:IP
                self.cs = newSegment;
                self.ip = newOffset;
            }
            else
            {
                // Update IP only
                self.ip = newOffset;
            }
        }

        private static void HandleRET(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-12

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-45
            // - Table 2-21. Instruction Set Reference Data, p. 2-64
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            bool intersegment = false;
            ushort imm = 0;

            switch (self.insByte)
            {
                case 0b1100_0011:
                    // Return from CALL
                    // Within segment
                    // 11000011 (C3)
                    break;
                case 0b1100_0010:
                    self.DecodeInstruction(
                        InstructionDecoderFlags.Word
                    );
                    // Within segment adding immediate to SP
                    // 11000010 (C2) data-lo data-hi
                    imm = (ushort)self.ins_data;
                    break;
                case 0b1100_1011:
                    // Intersegment
                    // 11001011 (CB)
                    intersegment = true;
                    break;
                case 0b1100_1010:
                    // Intersegment, adding immediate to SP
                    // 11001010 (CA) data-lo data-hi
                    self.DecodeInstruction(
                        InstructionDecoderFlags.Word
                    );
                    imm = (ushort)self.ins_data;
                    intersegment = true;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            // Pop return address from stack
            ushort returnIP = self.memory.wordAt(self.ss, self.sp);
            self.sp += 2;

            if (intersegment)
            {
                // For intersegment, also pop CS
                ushort returnCS = self.memory.wordAt(self.ss, self.sp);
                self.sp += 2;

                // Update CS:IP
                self.cs = returnCS;
                self.ip = returnIP;
            }
            else
            {
                // Update IP only
                self.ip = returnIP;
            }

            // Add immediate to SP if specified
            if (imm > 0)
            {
                self.sp += imm;
            }
        }

        private static void HandleJMP(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-11

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-45
            // - Table 2-21. Instruction Set Reference Data, p. 2-58
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            ushort newOffset = 0;
            ushort newSegment = 0;
            bool intersegment = false;

            switch (self.insByte)
            {
                case 0b1110_1001:
                    // Direct within segment
                    // 11101001 (E9) disp-low disp-high
                    self.DecodeInstruction(
                        InstructionDecoderFlags.DispW
                    );
                    newOffset = (ushort)(self.ip + self.disp);
                    break;
                case 0b1110_1011:
                    // Direct within segment short
                    // 11101011 (EB) disp
                    self.DecodeInstruction(
                        InstructionDecoderFlags.Byte
                    );
                    // Need to sign-extend the byte displacement to a short
                    newOffset = (ushort)(self.ip + (sbyte)self.ins_data);
                    break;
                case 0b1111_1111:
                    self.DecodeInstruction(
                        InstructionDecoderFlags.ModRM |
                        InstructionDecoderFlags.ModRMOpcode
                    );

                    if (self.insExtOpcode == 0b100)
                    {
                        // Indirect within segment
                        // 11111111 (FF) mod 100 r/m
                        // Part of Group 2 instructions

                        // Get address from r/m
                        newOffset = self.memory.wordAt(self.modrm_addr);
                    }
                    else
                    {
                        // Indirect intersegment
                        // 11111111 (FF) mod 101 r/m
                        // Part of Group 2 instructions
                        Debug.Assert(self.insExtOpcode == 0b101);

                        // Get far pointer from memory
                        newOffset = self.memory.wordAt(self.modrm_addr);
                        newSegment = self.memory.wordAt(self.modrm_addr + 2);
                        intersegment = true;
                    }
                    break;
                case 0b1110_1010:
                    // Direct intersegment
                    // 11101010 (EA) off-lo off-hi seg-lo seg-hi
                    self.DecodeInstruction(
                        InstructionDecoderFlags.AddL
                    );
                    newOffset = self.ins_eff_addr;
                    newSegment = self.ins_seg;
                    intersegment = true;
                    break;
                default:
                    // Maybe a JMP FAR AL or something?
                    Debug.Assert(false);
                    break;
            }

            // Update registers
            if (intersegment)
            {
                self.cs = newSegment;
                self.ip = newOffset;
            }
            else
            {
                self.ip = newOffset;
            }
        }
        #endregion

        #region Conditional Transfers
        private static void HandleJA(CPU self)
        {
            HandleJNBE(self);
        }

        private static void HandleJAE(CPU self)
        {
            HandleJNB(self);
        }

        private static void HandleJB(CPU self)
        {
            // JB could also be JNAE or JC

            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-12
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-56
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            // logic: https://wikidev.in/wiki/assembly/8086/JNAE
            // jb/jnae = Jump on below/not above or equal

            Debug.Assert(0b0111_0010 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // 01110010 (72) disp
            self.jmp_disp_on(self.CF);
        }

        private static void HandleJBE(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-12
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-57
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            Debug.Assert(0b0111_0110 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // jbe/jna jump on below or equal/not above
            // 01110110 (76) disp
            self.jmp_disp_on(self.CF || self.ZF);
        }

        private static void HandleJC(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-57

            HandleJB(self);
        }

        private static void HandleJE(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-12
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-57
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            Debug.Assert(0b0111_0100 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // JE/JZ jump on equal/zero
            // 01110100 (74) disp
            self.jmp_disp_on(self.ZF);
        }

        private static void HandleJG(CPU self)
        {
            HandleJNLE(self);
        }

        private static void HandleJGE(CPU self)
        {
            HandleJNL(self);
        }

        private static void HandleJL(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-12
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-57
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            Debug.Assert(0b0111_1100 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // Logic: https://wikidev.in/wiki/assembly/8086/JL
            // JL/JNGE jump on less/not greater or equal
            // 01111100 (7C) disp
            self.jmp_disp_on(self.SF != self.OF);
        }

        private static void HandleJLE(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-12
            // - 8088 Conditional Transfer Operations, B-1

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-58
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            Debug.Assert(0b01111110 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // JLE/JNG jump on less or equal/not greater
            // 01111110 (7E) disp
            self.jmp_disp_on(self.SF != self.OF || self.ZF);
        }

        private static void HandleJNA(CPU self)
        {
            HandleJBE(self);
        }

        private static void HandleJNAE(CPU self)
        {
            HandleJB(self);
        }

        private static void HandleJNB(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-13
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-56
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            Debug.Assert(0b0111_0011 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // Logic: https://wikidev.in/wiki/assembly/8086/jnb
            // jnb/jae Jump on not below/above or equal
            // 01110011 (73) disp
            self.jmp_disp_on(!self.CF);
        }

        private static void HandleJNBE(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-13
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-56
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            Debug.Assert(0b0111_0111 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // jnbe/ja jump on not below or equal/above
            // 01110111 (77) disp
            self.jmp_disp_on(!(self.CF || self.ZF));
        }

        private static void HandleJNC(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-58

            HandleJNB(self);
        }

        private static void HandleJNE(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-12
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-58
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            Debug.Assert(0b0111_0101 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // jne/jnz jump on not equal/not zero
            // 01110101 (75) disp
            self.jmp_disp_on(!self.ZF);
        }

        private static void HandleJNG(CPU self)
        {
            HandleJLE(self);
        }

        private static void HandleJNGE(CPU self)
        {
            HandleJL(self);
        }

        private static void HandleJNL(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-12
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-57
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            Debug.Assert(0b0111_1101 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // Logic: https://wikidev.in/wiki/assembly/8086/jnl
            // jnl/jnge jump on not less/greater or equal
            // 01111101 (7D) disp
            self.jmp_disp_on(self.SF == self.OF);
        }

        private static void HandleJNLE(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-13
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-57
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            Debug.Assert(0b0111_1111 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // JNLE/JG jump on not less or equal/greater
            // 01111111 (7F) disp
            self.jmp_disp_on((self.SF == self.OF) || self.ZF);
        }

        private static void HandleJNO(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-13
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-58
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            Debug.Assert(0b0111_0001 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // jump on not overflow
            // 01110001 (71) disp
            self.jmp_disp_on(!self.OF);
        }

        private static void HandleJNP(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-13
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-58
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            Debug.Assert(0b0111_1011 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // jnp/jpo jump on not parity/parity odd
            // 01111011 (7B) disp
            self.jmp_disp_on(!self.PF);
        }

        private static void HandleJNS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-13
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-58
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            Debug.Assert(0b0111_1001 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // jump on not sign
            // 01111001 (79) disp
            self.jmp_disp_on(!self.SF);
        }

        private static void HandleJNZ(CPU self)
        {
            HandleJNE(self);
        }

        private static void HandleJO(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-12
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-59
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            Debug.Assert(0b0111_0000 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // jump on overflow
            // 01110000 (70) disp
            self.jmp_disp_on(self.OF);
        }

        private static void HandleJP(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-12
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-59
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            Debug.Assert(0b0111_1010 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // jp/jpe jump on parity/parity even
            // 01111010 (7A) disp
            self.jmp_disp_on(self.PF);
        }

        private static void HandleJPE(CPU self)
        {
            HandleJP(self);
        }

        private static void HandleJPO(CPU self)
        {
            HandleJNP(self);
        }

        private static void HandleJS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-12
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-59
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            Debug.Assert(0b0111_1000 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // Jump on sign
            // 01111000 (78) disp
            self.jmp_disp_on(self.SF);
        }

        private static void HandleJZ(CPU self)
        {
            HandleJE(self);
        }
        #endregion

        #region Iteration Control
        private static void HandleLOOP(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-13

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-45
            // - Table 2-21. Instruction Set Reference Data, p. 2-60
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            Debug.Assert(0b1110_0010 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // loop cx times
            // 11100010 (E2) disp

            // Decrement CX
            self.cx--;

            // Jump if CX != 0
            if (self.cx != 0)
            {
                // Sign-extend byte displacement to a short
                short disp = (sbyte)self.disp;
                self.jmp(disp);
            }
        }

        private static void HandleLOOPE(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-13

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-45
            // - Table 2-21. Instruction Set Reference Data, p. 2-60
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            Debug.Assert(0b1110_0001 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // loopz/loope loop while zero/equal
            // 11100001 (E1) disp

            // Decrement CX
            self.cx--;

            // Jump if CX != 0 and ZF = 1
            if (self.cx != 0 && self.ZF)
            {
                // Sign-extend byte displacement to a short
                short disp = (sbyte)self.disp;
                self.jmp(disp);
            }
        }

        private static void HandleLOOPNE(CPU self)
        {
            HandleLOOPNZ(self);
        }

        private static void HandleLOOPNZ(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-13

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-46
            // - Table 2-21. Instruction Set Reference Data, p. 2-60
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            Debug.Assert(0b1110_0000 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // loopnz/loopne loop while not zero/not equal
            // 11100000 (E0) disp

            // Decrement CX
            self.cx--;

            // Jump if CX != 0 and ZF = 0
            if (self.cx != 0 && !self.ZF)
            {
                // Sign-extend byte displacement to a short
                short disp = (sbyte)self.disp;
                self.jmp(disp);
            }
        }

        private static void HandleLOOPZ(CPU self)
        {
            HandleLOOPE(self);
        }

        private static void HandleJCXZ(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            // - 8088 Instruction Reference, p. B-13
            // - 8088 Conditional Transfer Operations, B-14

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-46
            // - Table 2-21. Instruction Set Reference Data, p. 2-57
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            Debug.Assert(0b1110_0011 == self.insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.DispB
            );

            // jump on cx zero
            // 11100011 (E3) disp
            self.jmp_disp_on(self.cx == 0);
        }
        #endregion

        #region Interrupt Instructions
        private static void HandleINT(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-14

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-46
            // - Table 2-21. Instruction Set Reference Data, p. 2-56
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            // ODITSZAPC
            //   00     

            byte intType = 0;

            switch (self.insByte)
            {
                case 0b1100_1101:
                    self.DecodeInstruction(
                        InstructionDecoderFlags.DispB
                    );
                    // interrupt
                    // type specified
                    // 11001101 (CD) type
                    intType = (byte)self.disp;
                    break;
                case 0b1100_1100:
                    // type 3
                    // 11001100 (CC)
                    intType = 3;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            // Save flags on stack
            self.Push((short)self.flags);

            // Save CS:IP on stack
            self.Push((short)self.cs);
            self.Push((short)self.ip);

            // Clear IF and TF flags
            self.IF = false;
            self.TF = false;

            if (self.interrupt_table[intType] != null)
            {
                self.interrupt_table[intType]!(self);

                // Pop IP, CS, and FLAGS from stack
                // (simulate IRET)
                self.ip = (ushort)self.Pop();
                self.cs = (ushort)self.Pop();
                self.flags = (Flags)self.Pop();
            }
            else
            {
                // Load new CS:IP from interrupt vector table
                uint intVectorAddr = (uint)intType * 4;
                var ip = self.memory.wordAt((ushort)intVectorAddr);
                var cs = self.memory.wordAt((ushort)(intVectorAddr + 2));

                if (cs == 0 && ip == 0)
                {
                    if (Debugger.IsAttached)
                    {
                        // Calling non-existent interrupt
                        Debugger.Break();
                    }
                    else
                    {
                        Console.WriteLine($"Calling non-existent interrupt {intType}!");

                    }
                }
                else
                {
                    self.cs = cs;
                    self.ip = ip;
                }
            }
        }

        private static void HandleINTO(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-14

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-47
            // - Table 2-21. Instruction Set Reference Data, p. 2-56

            // ODITSZAPC
            //   00     

            Debug.Assert(0b1100_1110 == self.insByte);

            // interrupt on overflow
            // 11001110 (CE)

            // If overflow flag is set, execute INT 4
            if (self.OF)
            {
                // Save flags on stack
                self.Push((short)self.flags);

                // Clear IF and TF flags
                self.IF = false;
                self.TF = false;

                // Save CS:IP on stack
                self.Push((short)self.cs);
                self.Push((short)self.ip);

                // Load new CS:IP from interrupt vector 4
                uint intVectorAddr = 4 * 4;
                self.ip = self.memory.wordAt((ushort)intVectorAddr);
                self.cs = self.memory.wordAt((ushort)(intVectorAddr + 2));
            }
        }

        private static void HandleIRET(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-14

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-47
            // - Table 2-21. Instruction Set Reference Data, p. 2-56

            // ODITSZAPC
            // RRRRRRRRR

            Debug.Assert(0b1100_1111 == self.insByte);

            // interrupt return
            // 11001111 (CF)

            // Pop IP, CS, and FLAGS from stack
            self.ip = (ushort)self.Pop();
            self.cs = (ushort)self.Pop();
            self.flags = (Flags)self.Pop();
        }
        #endregion

        #region Helpers
        private void jmp(short disp) => csip += disp;

        private void jmp_disp() => jmp(disp);

        private void jmp_disp_on(bool cond)
        {
            if (cond) jmp_disp();
        }
        #endregion
    }
}
