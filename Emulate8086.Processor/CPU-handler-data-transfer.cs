using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulate8086.Processor
{
    public partial class CPU
    {
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

            if (insByte == 0xFF)
            {
                // 11111111 mod 110 rm
                // Register/memory
                // Part of Group 2 instructions
            }
            else if (insByte >> 3 == 0b01010)
            {
                // 01010 reg
                // Register
            }
            else if (insByte >> 5 == 0b000)
            {
                // 000 seg 110
                // Segment register
            }

            throw new NotImplementedException();
        }

        private static void HandlePOP(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-5

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-31
            // - Table 2-21. Instruction Set Reference Data, p. 2-62            
            // - Table 4-12. 8086 Instruction Encoding, p. 4-22

            if (insByte == 0b10001111)
            {
                // 10001111 mod 000 rm
                // Register/memory
            }
            else if (insByte >> 3 == 0b01011)
            {
                // 01011 reg
                // Register
            }
            else if ((insByte & 0b111_00_111) == 0b000_00_111)
            {
                // 000 seg 111
                // Segment register
            }

            throw new NotImplementedException();
        }

        private static void HandleXCHG(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-32
            // - Table 2-21. Instruction Set Reference Data, p. 2-67
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            if ((insByte & 0b1111111_0) == 0b1000011_0)
            {
                // 1000011w mod reg rm
                // Register/memory with register
            }
            else if ((insByte >> 3) == 0b10010)
            {
                // 10010 reg 
                // Register with accumulator
            }

            throw new NotImplementedException();
        }

        private static void HandleXLAT(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-32
            // - Table 2-21. Instruction Set Reference Data, p. 2-67
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            // translate byte to al
            // 11010111
            throw new NotImplementedException();
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

            if ((insByte & 0b1111_111_0) == 0b1110_010_0)
            {
                // 1110 010w | port
                // Fixed port
            }
            else if ((insByte & 0b1111_111_0) == 0b1110_110_0)
            {
                // 1110 110w
                // Variable port (DX)
            }

            throw new NotImplementedException();
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

            if ((insByte & 0b1111_111_0) == 0b1110_011_0)
            {
                // 1110011w port
                // Fixed port
            }
            else if ((insByte & 0b1111_111_0) == 0b1110_110_0)
            {
                // 1110110w
                // Variable port (DX)
            }
            throw new NotImplementedException();
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

            
            // Load effective address to register
            // 10001101 mod reg r/m
            throw new NotImplementedException();
        }

        private static void HandleLDS(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-32
            // - Table 2-21. Instruction Set Reference Data, p. 2-59

            // Load pointer to DS
            // 11000101 mod reg r/m
            throw new NotImplementedException();
        }

        private static void HandleLES(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-32
            // - Table 2-21. Instruction Set Reference Data, p. 2-59
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            // Load pointer to ES
            // 11000100 mod reg r/m
            throw new NotImplementedException();
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

            // Load AH with flags
            // 10011111
            throw new NotImplementedException();
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
            
            // Store AH into flags
            // 10011110
            throw new NotImplementedException();
        }

        private static void HandlePUSHF(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-6

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-33
            // - Table 2-21. Instruction Set Reference Data, p. 2-63
            // - Table 4-12. 8086 Instruction Encoding, p. 4-23

            // 1001 1100
            // Push flags
            throw new NotImplementedException();
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
            
            // 1001 1101
            // Pop flags
            throw new NotImplementedException();
        }
        #endregion
    }
}
