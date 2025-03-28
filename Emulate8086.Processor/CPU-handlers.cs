using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Emulate8086.Processor
{
    public partial class CPU
    {
        // Reference:
        // Intel 8086 Family User's Manual October 1979:
        // https://edge.edx.org/c4x/BITSPilani/EEE231/asset/8086_family_Users_Manual_1_.pdf
        // 2.7 Instruction Set, p. 2-30
        // - Table 2-8. Data Transfer Instructions, p. 2-31
        // - Figure 2-32. Flag Storage Formats, p. 2-33
        // - Table 2-9. Arithmetic Instructions, p. 2-33
        // - Arithmetic Data Formats, p. 2-33
        // - Table 2-10. Arithmetic Interpretation of 8-Bit Numbers, p. 2-34
        // - Arithmetic Instructions and Flags, p. 2-34
        // - Table 2-11. Bit Manipulation Instructions, p. 2-38
        // - Table 2-12. String Instructions, p. 2-40
        // - Table 2-13. String Instruction Register and Flag Use, p. 2-40
        // - Figure 2-33. String Operating Flow, p. 2-41
        // - Table 2-14. Program Transfer Instructions, p. 2-44
        // - Table 2-15. Interpretation of Conditional Transfers, p. 2-46
        // - Table 2-16. Processor Control Instructions, p. 2-47
        // - Instruction Set Reference Information, p. 2-48
        // - Table 2-17. Key to Instruction Coding Formats, p. 2-49
        // - Table 2-18. Key to Flag Effects, p. 2-50
        // - Table 2-19. Key to Operand Types, p. 2-50
        // - Table 2-20. Effective Address Calculation Time, p. 2-51
        // - Table 2-21. Instruction Set Reference Data, p. 2-51
        // 4.2 8086 and 8088 CPUs, p. 4-1
        // - Machine Instruction Encoding and Decoding, p. 4-18
        // - Fig. 4-20, Typical 8086/8088 Machine Instruction Format, p. 4-19
        // - Table 4-7, Single Bit Field Encoding, p. 4-19
        // - Table 4-8, MOD (Mode) Field Encoding, p. 4-20
        // - Table 4-9, REG (Register) Field Encoding, p. 4-20
        // - Table 4-10, R/M (Register/Memory) Field Encoding, p. 4-20
        // - Table 4-11, Key to Machine Instruction Encoding and Decoding, p. 4-21
        // - Table 4-12, 8086 Instruction Encoding, p. 4-22
        // - Table 4-13, Machine Instruction Decoding Guide, p. 4-27
        // - Table 4-14, Machine Instruction Encoding Matrix, p. 4-36
        // 
        // IBM Personal Computer Hardware Reference Library - Technical
        // Reference
        // https://bitsavers.org/pdf/ibm/pc/pc/1502234_PC_Technical_Reference_Apr83.pdf
        // Appendix B: 8088 Assembly Instruction Set Reference
        // - 8088 Register Model, p. B-2
        // - Operand Summary, p. B-3
        // - Second Instruction Byte Summary, p. B-3
        // - Memory Segmentation Model, p. B-4
        // - Segment Override Prefix, p. B-4
        // - Use of Segment Override, p. B-4
        // - Instructions and encoding, P. B-5
        // - 8088 Conditional Transfer Operations, p. B-14
        // - 8088 Instruction Set Matrix, p. B-16

        // No instruction for given code
        private static void HandleNone(CPU self)
        {
            // throw new NotImplementedException();
        }

        #region Groups
        private static void HandleImmediateGroup(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979, p. 4-36,
            //   Machine Instruction Encoding Matrix
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            //   Appendix B: 8088 Instruction Set Matrix, p. B-17 
            var ins = (self.memory[self.csip] & 0b00111000) >> 3;
            Action<CPU> handler = ins switch
            {
                0b000 => HandleADD,
                0b001 => HandleOR,
                0b010 => HandleADC,
                0b011 => HandleSBB,
                0b100 => HandleAND,
                0b101 => HandleSUB,
                0b110 => HandleXOR,
                0b111 => HandleCMP,
                _ => HandleNone  // dummy; outside possible range
            };

            handler(self);
        }

        private static void HandleShiftGroup(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979, p. 4-36,
            //   Machine Instruction Encoding Matrix
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            //   Appendix B: 8088 Instruction Set Matrix, p. B-17 
            var ins = (self.memory[self.csip] & 0b00111000) >> 3;
            Action<CPU> handler = ins switch
            {
                0b000 => HandleROL,
                0b001 => HandleROR,
                0b010 => HandleRCL,
                0b011 => HandleRCR,
                0b100 => HandleSHL_SAL,
                0b101 => HandleSHR,
                0b110 => HandleInvalid,  // Invalid
                0b111 => HandleSAR,
                _ => HandleNone  // dummy; outside possible range
            };

            handler(self);
        }

        private static void HandleInvalid(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleGroup1(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979, p. 4-36,
            //   Machine Instruction Encoding Matrix
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            //   Appendix B: 8088 Instruction Set Matrix, p. B-17 
            var ins = (self.memory[self.csip] & 0b00111000) >> 3;
            Action<CPU> handler = ins switch
            {
                0b000 => HandleTEST,
                0b001 => HandleInvalid,  // Invalid
                0b010 => HandleNOT,
                0b011 => HandleNEG,
                0b100 => HandleMUL,
                0b101 => HandleIMUL,
                0b110 => HandleDIV,
                0b111 => HandleIDIV,
                _ => HandleNone  // dummy; outside possible range
            };

            handler(self);
        }

        private static void HandleGroup2(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979, p. 4-36,
            //   Machine Instruction Encoding Matrix
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference
            //   Appendix B: 8088 Instruction Set Matrix, p. B-17 
            var ins = (self.memory[self.csip] & 0b00111000) >> 3;
            Action<CPU> handler = ins switch
            {
                0b000 => HandleINC,
                0b001 => HandleDEC,
                0b010 => HandleCALL,
                0b011 => HandleCALL,
                0b100 => HandleJMP,
                0b101 => HandleJMP,
                0b110 => HandlePUSH,
                0b111 => HandleInvalid,  // Invalid
                _ => HandleNone  // dummy; outside possible range
            };

            handler(self);
        }
        #endregion

        #region Segment Prefixes
        private static void HandleESPrefix(CPU self)
        {
            self.DecodeInstruction(InstructionDecoderFlags.Pfix);
        }

        private static void HandleCSPrefix(CPU self)
        {
            self.DecodeInstruction(InstructionDecoderFlags.Pfix);
        }

        private static void HandleSSPrefix(CPU self)
        {
            self.DecodeInstruction(InstructionDecoderFlags.Pfix);
        }

        private static void HandleDSPrefix(CPU self)
        {
            self.DecodeInstruction(InstructionDecoderFlags.Pfix);
        }
        #endregion

        #region Flag Operations
        private static void HandleCLC(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-15

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-47
            // - Table 2-21. Instruction Set Reference Data, p. 2-53
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            // ODITSZAPC
            //         0

            Debug.Assert(0b1111_1000 == self.insByte);

            // Clear carry
            // 11111000 (F8)
            self.CF = false;
        }

        private static void HandleCMC(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-15

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-47
            // - Table 2-21. Instruction Set Reference Data, p. 2-53
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            // ODITSZAPC
            //         X

            Debug.Assert(0b1111_0101 == self.insByte);

            // Complement carry
            // 11110101 (F5)
            self.CF = !self.CF;
        }

        private static void HandleSTC(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-15

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-47
            // - Table 2-21. Instruction Set Reference Data, p. 2-66
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            // ODITSZAPC
            //         1

            Debug.Assert(0b1111_1001 == self.insByte);

            // Set carry
            // 11111001 (F9)
            self.CF = true;
        }

        private static void HandleCLD(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-15

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-47
            // - Table 2-21. Instruction Set Reference Data, p. 2-53
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            // ODITSZAPC
            //  0       

            Debug.Assert(0b1111_1100 == self.insByte);

            // Clear direction
            // 11111100 (FC)
            self.DF = false;
        }

        private static void HandleSTD(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-15

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-47
            // - Table 2-21. Instruction Set Reference Data, p. 2-66
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            // ODITSZAPC
            //  1       

            Debug.Assert(0b1111_1101 == self.insByte);

            // Set direction
            // 11111101 (FD)
            self.DF = true;
        }

        private static void HandleCLI(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-15

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-48
            // - Table 2-21. Instruction Set Reference Data, p. 2-53
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            // ODITSZAPC
            //  0      

            Debug.Assert(0b1111_1010 == self.insByte);

            // Clear interrupt
            // 11111010 (FA)
            self.IF = false;
        }

        private static void HandleSTI(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-15

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-48
            // - Table 2-21. Instruction Set Reference Data, p. 2-66
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            // ODITSZAPC
            //   1      

            Debug.Assert(0b1111_1011 == self.insByte);

            // Set interrupt
            // 11111011 (FB)
            self.IF = true;
        }
        #endregion

        #region External Synchronization
        private static void HandleHLT(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-15

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-48
            // - Table 2-21. Instruction Set Reference Data, p. 2-55
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            Debug.Assert(0b1111_0100 == self.insByte);

            // halt
            // 11110100 (F4)
            // TODO
            //self.halted = true;
            throw new NotImplementedException();
        }

        private static void HandleWAIT(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-15

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-48
            // - Table 2-21. Instruction Set Reference Data, p. 2-67
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            Debug.Assert(0b10011011 == self.insByte);

            // 10011011 (FB)
            //throw new NotImplementedException();
        }

        private static void HandleESC(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-15

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-48
            // - Table 2-21. Instruction Set Reference Data, p. 2-54
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            Debug.Assert(0b1101_1_000 == (self.insByte & 0b11111_000));

            self.DecodeInstruction(
                InstructionDecoderFlags.ModRM
            );

            // Escape (to external device)
            // 11011xxx (D8 - DF) mod xxx r/m
            //throw new NotImplementedException();
        }

        private static void HandleLOCK(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-15

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-48
            // - Table 2-21. Instruction Set Reference Data, p. 2-60
            // - Table 4-12. 8086 Instruction Encoding, p. 4-27

            Debug.Assert(0b1111_0000 == self.insByte);

            // Bus lock prefix
            // 11110000 (F0)
            //throw new NotImplementedException();
        }
        #endregion

        #region No Operation
        private static void HandleNOP(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-15

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-48
            // - Table 2-21. Instruction Set Reference Data, p. 2-62

            // No operation (xchg eax,eax)
            // 10010000 (90)
            //throw new NotImplementedException();
        }
        #endregion
    }
}
