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

            switch (insByte)
            {
                case 0b1110_1000:
                    // Direct within segment
                    // 11101000 disp-low disp-high
                    break;
                case 0b1111_1111:
                    if (self.insExtOpcode == 0b010)
                    {
                        // Inderect within segment
                        // 11111111 mod 010 r/m
                        // Part of Group 2 instructions
                    }
                    else
                    {
                        // Inderect intersegment
                        // 11111111 mod 011 r/m
                        // Part of Group 2 instructions
                        Debug.Assert(self.insExtOpcode == 0b011);
                    }
                    break;
                case 0b1001_1010:
                    // Direct intersegment
                    // 10011010 offs-low offs-hi seg-lo seg-hi
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            throw new NotImplementedException();
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

            switch (insByte)
            {
                case 0b1100_0011:
                    // Return from CALL
                    // Within segment
                    // 11000011
                    break;
                case 0b1100_0010:
                    // Within segment adding immediate to SP
                    // 11000010 data-lo data-hi
                    break;
                case 0b1100_1001:
                    // Intersegment
                    // 11001011
                    break;
                case 0b1100_0010:
                    // Intersegment, adding immediate to SP
                    // 11000010 data-lo data-hi
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            throw new NotImplementedException();
        }

        private static void HandleJMP(CPU self)
        {
            // IBM Personal Computer Hardware Reference Library - Technical
            // Reference, 8088 Instruction Reference, p. B-11

            // Intel 8086 Family User's Manual October 1979
            // - 2.7 Instruction Set, p. 2-45
            // - Table 2-21. Instruction Set Reference Data, p. 2-58
            // - Table 4-12. 8086 Instruction Encoding, p. 4-26

            switch (insByte)
            {
                case 0b1110_1001:
                    // Direct within segment
                    // 11101001 disp-low disp-high
                    break;
                case 0b1110_1011:
                    // Direct within segment short
                    // 11101011 disp
                    break;
                case 0b1111_1111:
                    if (self.insExtOpcode == 0b100)
                    {
                        // Indirect within segment
                        // 11111111 mod 100 r/m
                        // Part of Group 2 instructions
                    }
                    else
                    {
                        // Inderect intersegment
                        // 11111111 mod 101 r/m
                        // Part of Group 2 instructions
                    }
                    break;
                case 0b1110_1010:
                    // Direct intersegment
                    // 11101010 off-lo off-hi seg-lo seg-hi
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
            throw new NotImplementedException();
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

            // loop cx times
            // 11100010 disp
            throw new NotImplementedException();
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

            // loopz/loope loop while zero/equal
            // 11100001 disp
            throw new NotImplementedException();
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

            // loopnz/loopne loop while not zero/not equal
            // 11100000 disp
            throw new NotImplementedException();
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

            switch (insByte)
            {
                case 0b1100_1101:
                    // interrupt
                    // type specified
                    // 11001101 type
                    break;
                case 0b1100_1100:
                    // type 3
                    // 11001100
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
