using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Debug.Assert(0b1111_011_0 == (insByte & 0b11111110))

            self.DecodeInstruction(
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );

            // 1111011w mod 010 r/m
            // Part of Group 1 instructions

            Debug.Assert(self.insExtOpcode == 0b010);

            throw new NotImplementedException();
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

            if (0b0010_00_00 == (insByte & 0b11111100))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.D |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMReg
                );

                // Register/memory and register to either
                // 001000dw mod reg r/m
            }
            else if (0b1000_00_00 == (insByte & 0b11111110))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immediate to register/memory
                // 1000000w mod 100 r/m data, data if w=1

                Debug.Assert(self.insExtOpcode == 0b100);
            }
            else
            {
                Debug.Assert(0b0010_010_0 == (insByte & 0b11111110));

                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );
                
                // Immediate to accumulator
                // 0010010w data, data if w=1
            }
            throw new NotImplementedException();
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
            
            if (0b0000_10_00 == (insByte & 0b11111100))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.D |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMReg
                );

                // Register/memory and register to either
                // 000010dw mod reg r/m
            }
            else if (0b1000_000_0 == (insByte & 0b11111110))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immedate to register/memory
                // 1000000w mod 001 r/m data, data if w=1
                // Part of Immediate group

                Debug.Assert(self.insExtOpcode == 0b001);
            }
            else
            {
                Debug.Assert(0b0000_110_0 == (insByte & 0b11111110));

                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immediate to accumulator
                // 0000110w data data if w = 1
            }
            throw new NotImplementedException();
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
            
            if (0b0011_00_00 == (insByte & 0b11111100))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.D |
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMReg
                );

                // Register/memory and register to either
                // 001100dw mod reg r/m
            }
            else if (0b1000_000_0 == (insByte & 0b11111110))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immediate to register/memory
                // 1000000w mod 110 r/m data, data if w=1
                // Part of Immediate group

                Debug.Assert(self.insExtOpcode == 0b110);
            }
            else
            {
                Debug.Assert(0b0011_010_0 == (insByte & 0b1111_1110));

                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immediate to accumulator
                // 0011010w data, data if w=1
            }
            throw new NotImplementedException();
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

            if (0b1000_010_0 == (insByte & 0b1111_1110))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMReg
                );

                // R/m and register
                // 1000010w mod reg r/m
            }
            else if (0b1111_011_0 == (insByte & 0b1111_1110))
            {
                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.ModRM |
                    InstructionDecoderFlags.ModRMOpcode |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immediate data and register/memory
                // 1111011w mod 000 r/m data, data if w=1
                // Part of Group 1 instructions

                Debug.Assert(self.insExtOpcode == 0b000);
            }
            else
            {
                Debug.Assert(0b1010_100_0 == (insByte & 0b1111_1110));

                self.DecodeInstruction(
                    InstructionDecoderFlags.W |
                    InstructionDecoderFlags.Byte |
                    InstructionDecoderFlags.Word
                );

                // Immediate data and accumulator
                // 1010100w data, data if w=1
            }
            throw new NotImplementedException();
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
            
            Debug.Assert(0b1101_00_00 == (insByte & 0b1111_1100));
        
            self.DecodeInstruction(
                InstructionDecoderFlags.V |
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );

            // 110100vw mod 100 r/m
            // Part of Shift group

            Debug.Assert(self.insExtOpcode == 0b100);

            throw new NotImplementedException();
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
            
            Debug.Assert(0b1101_00_00 == (insByte & 0b1111_1100));

            self.DecodeInstruction(
                InstructionDecoderFlags.V |
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );
            
            // Shift logical right
            // 110100vw mod 101 r/m
            // Part of Shift group

            Debug.Assert(self.insExtOpcode == 0b101);
            
            throw new NotImplementedException();
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
            
            Debug.Assert(0b1101_00_00 == (insByte & 0b1111_1100));
        
            self.DecodeInstruction(
                InstructionDecoderFlags.V |
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );
            
            // 110100vw mod 111 r/m
            // Part of Shift group

            Debug.Assert(self.insExtOpcode == 0b111);
                
            throw new NotImplementedException();
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

            Debug.Assert(0b1101_00_00 == (insByte & 0b1111_1100));

            self.DecodeInstruction(
                InstructionDecoderFlags.V |
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );
           
            // 110100vw mod 000 r/m
            // Part of Shift group

            Debug.Assert(self.insExtOpcode == 0b000);

            throw new NotImplementedException();
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
            
            Debug.Assert(0b1101_00_00 == (insByte & 0b1111_1100));
        
            self.DecodeInstruction(
                InstructionDecoderFlags.V |
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );
            
            // 110100vw mod 001 r/m
            // Part of Shift group

            Debug.Assert(self.insExtOpcode == 0b001);

            throw new NotImplementedException();
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
            
            Debug.Assert(0b1101_00_00 == (insByte & 0b1111_1100));

            self.DecodeInstruction(
                InstructionDecoderFlags.V |
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );
            
            // Rotate through carry left
            // 110100vw mod 010 r/m
            // Part of Shift group

            Debug.Assert(self.insExtOpcode == 0b010);
            
            throw new NotImplementedException();
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
            
            Debug.Assert(0b1101_00_00 == (insByte & 0b1111_1100));
        
            self.DecodeInstruction(
                InstructionDecoderFlags.V |
                InstructionDecoderFlags.W |
                InstructionDecoderFlags.ModRM |
                InstructionDecoderFlags.ModRMOpcode
            );
            
            // Rotate through carry right
            // 110100vw mod 011 r/m

            Debug.Assert(self.insExtOpcode == 0b011);

            throw new NotImplementedException();
        }
        #endregion
    }
}
