using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulate8086.Processor
{
    public partial class CPU
    {
        private static void HandleREPZ(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-64 

            HandleREP(self);
        }

        private static void HandleREPE(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-64 

            HandleREP(self);
        }

        private static void HandleREPNZ(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-64 

            HandleREP(self);
        }

        private static void HandleREPNE(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-64 

            HandleREP(self);
        }

        private static void HandleREP(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-10

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-42
            // - Table 2-21. Instruction Set Reference Data, p. 2-63
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            Debug.Assert(0b1111_001_0 == (insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.Z
            );
            // Repeat
            // 1111001z
            throw new NotImplementedException();
        }

        private static void HandleMOVSW(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-61

            // 1010010w
            HandleMOVS(self);
        }

        private static void HandleMOVSB(CPU self)
        {
            // Intel 8086 Family User's Manual October 1979
            // - Table 2-21. Instruction Set Reference Data, p. 2-61

            // 1010010w
            HandleMOVS(self);
        }

        private static void HandleMOVS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-10

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-42
            // - Table 2-21. Instruction Set Reference Data, p. 2-61
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            Debug.Assert(0b1010_010_0 == (insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.W
            );
            // 1010010w
            throw new NotImplementedException();
        }

        private static void HandleCMPS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-10

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-42
            // - Table 2-21. Instruction Set Reference Data, p. 2-53
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            // ODITSZAPC
            // X   XXXXX

            Debug.Assert(0b1010_011_0 == (insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.W
            );
            // Compare string
            // 1010011w
            
            throw new NotImplementedException();
        }

        private static void HandleSCAS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-10

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-43
            // - Table 2-21. Instruction Set Reference Data, p. 2-65
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            // ODITSZAPC
            // X   XXXXX
            
            Debug.Assert(0b1010_111_0 == (insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.W
            );
            // Scan string
            // 1010111w
            throw new NotImplementedException();
        }

        private static void HandleLODS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-11

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-43
            // - Table 2-21. Instruction Set Reference Data, p. 2-60
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            Debug.Assert(0b1010_110_0 == (insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.W
            );
            // Load string
            // 1010110w
            throw new NotImplementedException();
        }

        private static void HandleSTOS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-11

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-43
            // - Table 2-21. Instruction Set Reference Data, p. 2-66
            // - Table 4-12. 8086 Instruction Encoding, p. 4-25

            Debug.Assert(0b1010_101_0 == (insByte & 0b11111110));

            self.DecodeInstruction(
                InstructionDecoderFlags.W
            );
            // Store string
            // 1010101w
            throw new NotImplementedException();
        }
    }
}
