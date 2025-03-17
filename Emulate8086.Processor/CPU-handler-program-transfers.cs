using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            ushort returnOffset = (ushort)(self.ip);
            ushort newOffset = 0;
            ushort newSegment = 0;
            bool intersegment = false;

            switch (insByte)
            {
                case 0b1110_1000:

                    self.DecodeInstruction(
                        InstructionDecoderFlags.DispW
                    );
                    // Direct within segment
                    // 11101000 disp-low disp-high
                    newOffset = (ushort)(self.ip + self.disp);
                    break;
                case 0b1111_1111:
                    if (self.insExtOpcode == 0b010)
                    {
                        self.DecodeInstruction(
                            InstructionDecoderFlags.ModRM |
                            InstructionDecoderFlags.ModRMOpcode
                        );
                        // Indirect within segment
                        // 11111111 mod 010 r/m
                        // Part of Group 2 instructions
                        Debug.Assert(self.insExtOpcode == 0b010)
                        
                        // Get address from r/m
                        newOffset = self.GetModRMData();
                    }
                    else
                    {
                        self.DecodeInstruction(
                            InstructionDecoderFlags.ModRM |
                            InstructionDecoderFlags.ModRMOpcode
                        );
                        // Indirect intersegment
                        // 11111111 mod 011 r/m
                        // Part of Group 2 instructions
                        Debug.Assert(self.insExtOpcode == 0b011);
                        
                        // Get far pointer from memory
                        ushort ea = self.CalcEA();
                        newOffset = self.mem.ReadWord(self.ds, ea);
                        newSegment = self.mem.ReadWord(self.ds, (ushort)(ea + 2));
                        intersegment = true;
                    }
                    break;
                case 0b1001_1010:
                    self.DecodeInstruction(
                        InstructionDecoderFlags.AddrL
                    );
                    // Direct intersegment
                    // 10011010 offs-low offs-hi seg-lo seg-hi
                    newOffset = (ushort)self.ins_offsetL;
                    newSegment = (ushort)self.ins_segL;
                    intersegment = true;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            // Push return address onto stack
            self.sp -= 2;
            self.mem.WriteWord(self.ss, self.sp, returnOffset);
            
            if (intersegment)
            {
                // For intersegment, also push CS
                self.sp -= 2;
                self.mem.WriteWord(self.ss, self.sp, self.cs);
                
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

            switch (insByte)
            {
                case 0b1100_0011:
                    // Return from CALL
                    // Within segment
                    // 11000011
                    break;
                case 0b1100_0010:
                    self.DecodeInstruction(
                        InstructionDecoderFlags.Word
                    );
                    // Within segment adding immediate to SP
                    // 11000010 data-lo data-hi
                    imm = (ushort)self.ins_data;
                    break;
                case 0b1100_1001:
                    // Intersegment
                    // 11001011
                    intersegment = true;
                    break;
                case 0b1100_1010:
                    // Intersegment, adding immediate to SP
                    // 11001010 data-lo data-hi
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
            ushort returnIP = self.mem.ReadWord(self.ss, self.sp);
            self.sp += 2;
            
            if (intersegment)
            {
                // For intersegment, also pop CS
                ushort returnCS = self.mem.ReadWord(self.ss, self.sp);
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

            switch (insByte)
            {
                case 0b1110_1001:
                    // Direct within segment
                    // 11101001 disp-low disp-high
                    self.DecodeInstruction(
                        InstructionDecoderFlags.DispW
                    );
                    newOffset = (ushort)(self.ip + self.disp);
                    break;
                case 0b1110_1011:
                    // Direct within segment short
                    // 11101011 disp
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
                        // 11111111 mod 100 r/m
                        // Part of Group 2 instructions
                        Debug.Assert(self.insExtOpcode == 0b100);
                        
                        // Get address from r/m
                        newOffset = self.GetModRMData();
                    }
                    else
                    {
                        // Indirect intersegment
                        // 11111111 mod 101 r/m
                        // Part of Group 2 instructions
                        Debug.Assert(self.insExtOpcode == 0b101);
                        
                        // Get far pointer from memory
                        ushort ea = self.CalcEA();
                        newOffset = self.mem.ReadWord(self.ds, ea);
                        newSegment = self.mem.ReadWord(self.ds, (ushort)(ea + 2));
                        intersegment = true;
                    }
                    break;
                case 0b1110_1010:
                    // Direct intersegment
                    // 11101010 off-lo off-hi seg-lo seg-hi
                    self.DecodeInstruction(
                        InstructionDecoderFlags.AddrL
                    );
                    newOffset = (ushort)self.ins_offsetL;
                    newSegment = (ushort)self.ins_segL;
                    intersegment = true;
                    break;
                default:
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

            Debug.Assert(0b0111_0010 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );
            
            // 01110010 disp
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

            Debug.Assert(0b0111_0110 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // jbe/jna jump on below or equal/not above
            // 01110110 disp
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

            Debug.Assert(0b0111_0100 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // JE/JZ jump on equal/zero
            // 01110100 disp
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

            Debug.Assert(0b0111_1100 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // Logic: https://wikidev.in/wiki/assembly/8086/JL
            // JL/JNGE jump on less/not greater or equal
            // 01111100 disp
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

            Debug.Assert(0b01111110 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // JLE/JNG jump on less or equal/not greater
            // 01111110 disp
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

            Debug.Assert(0b0111_0011 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // Logic: https://wikidev.in/wiki/assembly/8086/jnb
            // jnb/jae Jump on not below/above or equal
            // 01110011 disp
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

            Debug.Assert(0b0111_0111 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // jnbe/ja jump on not below or equal/above
            // 01110111 disp
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

            Debug.Assert(0b0111_0101 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // jne/jnz jump on not equal/not zero
            // 01110101 disp
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

            Debug.Assert(0b0111_1101 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // Logic: https://wikidev.in/wiki/assembly/8086/jnl
            // jnl/jnge jump on not less/greater or equal
            // 01111101 disp
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

            Debug.Assert(0b0111_1111 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // JNLE/JG jump on not less or equal/greater
            // 01111111 disp
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

            Debug.Assert(0b0111_0001 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // jump on not overflow
            // 01110001 disp
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

            Debug.Assert(0b0111_1011 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // jnp/jpo jump on not parity/parity odd
            // 01111011 disp
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

            Debug.Assert(0b0111_1001 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // jump on not sign
            // 01111001 disp
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

            Debug.Assert(0b0111_0000 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // jump on overflow
            // 01110000 disp
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

            Debug.Assert(0b0111_1010 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // jp/jpe jump on parity/parity even
            // 01111010 disp
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

            Debug.Assert(0b0111_1000 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // Jump on sign
            // 01111000 disp
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

            Debug.Assert(0b1110_0010 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // loop cx times
            // 11100010 disp
            
            // Decrement CX
            self.cx--;
            
            // Jump if CX != 0
            if (self.cx != 0)
            {
                // Sign-extend byte displacement to a short
                short disp = (sbyte)self.ins_data;
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

            Debug.Assert(0b1110_0001 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // loopz/loope loop while zero/equal
            // 11100001 disp
            
            // Decrement CX
            self.cx--;
            
            // Jump if CX != 0 and ZF = 1
            if (self.cx != 0 && self.ZF)
            {
                // Sign-extend byte displacement to a short
                short disp = (sbyte)self.ins_data;
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

            Debug.Assert(0b1110_0000 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // loopnz/loopne loop while not zero/not equal
            // 11100000 disp
            
            // Decrement CX
            self.cx--;
            
            // Jump if CX != 0 and ZF = 0
            if (self.cx != 0 && !self.ZF)
            {
                // Sign-extend byte displacement to a short
                short disp = (sbyte)self.ins_data;
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

            Debug.Assert(0b1110_0011 == insByte);

            self.DecodeInstruction(
                InstructionDecoderFlags.Byte
            );

            // jump on cx zero
            // 11100011 disp
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

            switch (insByte)
            {
                case 0b1100_1101:
                    self.DecodeInstruction(
                        InstructionDecoderFlags.Byte
                    );
                    // interrupt
                    // type specified
                    // 11001101 type
                    intType = (byte)self.ins_data;
                    break;
                case 0b1100_1100:
                    // type 3
                    // 11001100
                    intType = 3;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            // Save flags on stack
            self.sp -= 2;
            self.mem.WriteWord(self.ss, self.sp, self.GetFlags());
            
            // Clear IF and TF flags
            self.IF = false;
            self.TF = false;
            
            // Save CS:IP on stack
            self.sp -= 2;
            self.mem.WriteWord(self.ss, self.sp, self.cs);
            self.sp -= 2;
            self.mem.WriteWord(self.ss, self.sp, self.ip);
            
            // Load new CS:IP from interrupt vector table
            uint intVectorAddr = (uint)intType * 4;
            self.ip = self.mem.ReadWord(0, (ushort)intVectorAddr);
            self.cs = self.mem.ReadWord(0, (ushort)(intVectorAddr + 2));
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

            Debug.Assert(0b1100_1110 == insByte);
            
            // interrupt on overflow
            // 11001110
            
            // If overflow flag is set, execute INT 4
            if (self.OF)
            {
                // Save flags on stack
                self.sp -= 2;
                self.mem.WriteWord(self.ss, self.sp, self.GetFlags());
                
                // Clear IF and TF flags
                self.IF = false;
                self.TF = false;
                
                // Save CS:IP on stack
                self.sp -= 2;
                self.mem.WriteWord(self.ss, self.sp, self.cs);
                self.sp -= 2;
                self.mem.WriteWord(self.ss, self.sp, self.ip);
                
                // Load new CS:IP from interrupt vector 4
                uint intVectorAddr = 4 * 4;
                self.ip = self.mem.ReadWord(0, (ushort)intVectorAddr);
                self.cs = self.mem.ReadWord(0, (ushort)(intVectorAddr + 2));
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

            Debug.Assert(0b1100_1111 == insByte);
            
            // interrupt return
            // 11001111
            
            // Pop IP, CS, and FLAGS from stack
            self.ip = self.mem.ReadWord(self.ss, self.sp);
            self.sp += 2;
            
            self.cs = self.mem.ReadWord(self.ss, self.sp);
            self.sp += 2;
            
            ushort flags = self.mem.ReadWord(self.ss, self.sp);
            self.sp += 2;
            
            // Restore flags
            self.SetFlags(flags);
        }
        #endregion

        #region Helpers
        private void jmp(short disp) => csip = csip_start + disp;

        private void jmp_disp() => jmp(disp);

        private void jmp_disp_on(bool cond)
        {
            if (cond) jmp_disp();
        }
        #endregion
    }
}
