using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Emulate8086.Processor
{
    public partial class CPU
    {
        bool is_seg_prefix = false;
        Register seg_prefix = Register.None;

        private static void HandleNone(CPU self)
        {
            throw new NotImplementedException();
        }

        private static void HandleImmediate(CPU self)
        {
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
                0b111 => HandleCMP
            };

            handler(self);
        }

        private static void HandleShift(CPU self)
        {
            var ins = (self.memory[self.csip] & 0b00111000) >> 3;
            Action<CPU>? handler = ins switch
            {
                0b000 => HandleROL,
                0b001 => HandleROR,
                0b010 => HandleRCL,
                0b011 => HandleRCR,
                0b100 => HandleSHL_SAL,
                0b101 => HandleSHR,
                0b110 => null,
                0b111 => HandleSAR
            };

            handler!(self);
        }

        private static void HandleGroup1(CPU self)
        {
            var ins = (self.memory[self.csip] & 0b00111000) >> 3;
            Action<CPU>? handler = ins switch
            {
                0b000 => HandleTEST,
                0b001 => null,
                0b010 => HandleNOT,
                0b011 => HandleNEG,
                0b100 => HandleMUL,
                0b101 => HandleIMUL,
                0b110 => HandleDIV,
                0b111 => HandleIDIV
            };

            handler!(self);
        }

        private static void HandleGroup2(CPU self)
        {
            var ins = (self.memory[self.csip] & 0b00111000) >> 3;
            Action<CPU>? handler = ins switch
            {
                0b000 => HandleINC,
                0b001 => HandleDEC,
                0b010 => HandleCALL,
                0b011 => HandleCALL,
                0b100 => HandleJMP,
                0b101 => HandleJMP,
                0b110 => HandlePUSH,
                0b111 => null
            };

            handler!(self);
        }

        private static void HandleESPrefix(CPU self)
        {
            self.is_seg_prefix = true;
            self.seg_prefix = Register.ES;
        }

        private static void HandleCSPrefix(CPU self)
        {
            self.is_seg_prefix = true;
            self.seg_prefix = Register.CS;
        }

        private static void HandleSSPrefix(CPU self)
        {
            self.is_seg_prefix = true;
            self.seg_prefix = Register.SS;
        }

        private static void HandleDSPrefix(CPU self)
        {
            self.is_seg_prefix = true;
            self.seg_prefix = Register.DS;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AAA()
        {
            // Logic: https://en.wikipedia.org/wiki/Intel_BCD_opcode
            //        https://web.archive.org/web/20190203181246/http://www.jaist.ac.jp/iscenter-new/mpc/altix/altixdata/opt/intel/vtune/doc/users_guide/mergedProjects/analyzer_ec/mergedProjects/reference_olh/mergedProjects/instructions/instruct32_hh/vc2a.htm
            //        https://web.archive.org/web/20081102170717/http://webster.cs.ucr.edu/AoA/Windows/HTML/AdvancedArithmetica6.html#1000255
            if ((al & 0x0F) > 10)
            {
                al = (byte)((al + 6) & 0xF);
                ah += 1;
                flags |= Flags.CF | Flags.AF;
            }
            else
            {
                flags &= ~(Flags.CF | Flags.AF);
            }
        }

        private static void HandleAAA(CPU self)
        {
            // ASCII adjust for add
            // 00110111
            self.AAA();
        }

        private static void HandleAAD(CPU self)
        {
            // ASCII adjust for divide
            // 11010101 00001010
            // Part of group?
            throw new NotImplementedException();
        }

        private static void HandleAAM(CPU self)
        {
            // ASCII adjust for multiply
            // 11010100 00001010
            // group??
            throw new NotImplementedException();
        }

        private static void HandleAAS(CPU self)
        {
            // 00111111 -- ASCII adjust for subtract
            throw new NotImplementedException();
        }

        private static void HandleADC(CPU self)
        {
            ADD(self, out var result);
            self.CF = (result & 0x00010000) != 0;
            {
                self.CF = true;
            }
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private void modrm(out Register mod, out int reg, out Register rm)
        {
            var modrm = memory[csip++];
            mod = (Register)(modrm >> 6);
            reg = (modrm & 0b00111000) >> 3;
            rm = (Register)(modrm & 0x0b111);
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private bool ins_flag(int pos)
        {
            var mask = 1 << pos;
            return (insByte & mask) != 0;
        }

        private ushort data(ref int csip, bool w)
        {
            int result = memory[csip++];
            if (w)
            {
                result |= memory[csip++] << 8;
            }
            return (ushort)result;
        }

        private static void HandleADD(CPU self)
        {
            ADD(self, out var result);
        }

        private static void ADD(CPU self, out int result)
        { 
            var csip = self.csip;
            var w = self.ins_flag(0);

            // Immediate to accumulator
            // 0000010w data, data if w=1
            if ((self.insByte & 0x1111111) == 0b00000100)
            {
                result = self.ax + self.data(ref csip, w);
                self.CF = result >= 0x00010000;
                self.ax = (ushort)result;
            }
            else
            {
                self.modrm(out var mod, out var reg, out var rm);
                self.CalcModRMAddress(ref csip, out var addr, out bool is_reg, out var reg_modrm);
                var ds = self.ins_flag(1);

                // Register/memory with register to either
                // 000000dw mod reg r/m
                if ((self.insByte & 0b11111100) == 0b00000000)
                {
                    result = self.GetModRMData(w, addr, is_reg, reg_modrm) + self.GetReg((Register)reg, w);
                    if (ds)  // reverse?
                    {
                        self.SetModRMData((ushort)result, w, addr, is_reg, reg_modrm);
                    }
                    else
                    {
                        self.SetReg((Register)reg, (ushort)result, w);
                    }
                }

                // Immediate to register/memory
                // 100000sw mod 000 r/m data, data if s:w=01
                // HandleImmediate, shares first byte w/ADC
                else
                {
                    var immediate = self.data(ref csip, w);
                    if (ds) // short form, sign extend
                    {
                        immediate = (ushort)(short)(sbyte)immediate;
                    }
                    result = immediate + self.GetModRMData(w, addr, is_reg, reg_modrm);
                    self.SetModRMData((ushort)result, w, addr, is_reg, reg_modrm);
                }
            }
            self.csip = csip;
        }

        private static void HandleAND(CPU self)
        {
            // Register/memory and register to either
            // 001000dw mod reg r/m

            // Immediate to register/memory
            // 1000000w mod 100 r/m data, data if w=1

            // Immediate to accumulator
            // 0010010w data, data if w=1
            throw new NotImplementedException();
        }

        private static void HandleCALL(CPU self)
        {
            // Direct within segment
            // 11101000 disp-low disp-high
            
            // Inderect within segment
            // 11111111 mod 010 r/m

            // Direct intersegment
            // 10011010 offs-low offs-hi seg-lo seg-hi

            // Inderect intersegment
            // 11111111 mod 011 r/m

            throw new NotImplementedException();
        }

        private static void HandleCBW(CPU self)
        {
            // Convert byte to word
            // 10011000
            throw new NotImplementedException();
        }

        private static void HandleCLC(CPU self)
        {
            // Clear carry
            // 11111000
            throw new NotImplementedException();
        }

        private static void HandleCLD(CPU self)
        {
            // Clear direction
            // 11111100
            throw new NotImplementedException();
        }

        private static void HandleCLI(CPU self)
        {
            // Clear interrupt
            // 11111010
            throw new NotImplementedException();
        }

        private static void HandleCMC(CPU self)
        {
            // Complement carry
            // 11110101
            throw new NotImplementedException();
        }

        private static void HandleCMP(CPU self)
        {
            // Register/memory and register
            // 001110dw mod reg r/m

            // Immediate with register/memory
            // 100000sw mod 111 r/m data, data if s:w =01

            // Immediate with accumulator
            // 0011110w data, data if w=1

            throw new NotImplementedException();
        }

        private static void HandleCMPS(CPU self)
        {
            // Compare string
            // 1010011w
            throw new NotImplementedException();
        }

        private static void HandleCWD(CPU self)
        {
            // Convert word to double word
            // 10011001
            throw new NotImplementedException();
        }

        private static void HandleDAA(CPU self)
        {
            // Decimal adjust for add
            // 00100111
            throw new NotImplementedException();
        }

        private static void HandleDAS(CPU self)
        {
            // Decimal adjust for subtract
            // 00101111
            throw new NotImplementedException();
        }

        private static void HandleDEC(CPU self)
        {
            // 1111111w mod 001 r/m
            // Part of group ?

            // 01001 reg

            throw new NotImplementedException();
        }

        private static void HandleDIV(CPU self)
        {
            // Divide (unsigned)
            // 1111011w mod 110 r/m
            // Part of group?
            throw new NotImplementedException();
        }

        private static void HandleESC(CPU self)
        {
            // Escape (to external device)
            // 11011xxx mod xxx r/m
            throw new NotImplementedException();
        }

        private static void HandleHLT(CPU self)
        {
            // halt
            // 11110100
            throw new NotImplementedException();
        }

        private static void HandleIDIV(CPU self)
        {
            // Integer divide (signed)
            // 1111011w mod 111 r/m
            // part of group?
            throw new NotImplementedException();
        }

        private static void HandleIMUL(CPU self)
        {
            // Integer multiply (signed)
            // 1111011w mod 101 r/m
            // Part of group?
            throw new NotImplementedException();
        }

        private static void HandleIN(CPU self)
        {
            // 1110010w port -- to al/ax 
            // 1110110w -- to al/ax from var port dx
            throw new NotImplementedException();
        }

        private static void HandleINC(CPU self)
        {
            // 1111111w mod 000 rm
            // 01000 reg
            throw new NotImplementedException();
        }

        private static void HandleINT(CPU self)
        {
            // interrupt
            // type specified
            // 11001101 type

            // type 3
            // 11001100
            throw new NotImplementedException();
        }

        private static void HandleINTO(CPU self)
        {
            // interrupt on overflow
            // 11001110
            throw new NotImplementedException();
        }

        private static void HandleIRET(CPU self)
        {
            // interrupt return
            // 11001111
            throw new NotImplementedException();
        }

        private void jmp(sbyte disp) => csip = csip_start + disp;

        private sbyte get_disp() => (sbyte)memory[csip++];

        private void jmp_disp() => jmp(get_disp());

        private void jmp_disp_on(bool cond)
        {
            if (cond) jmp_disp();
        }

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
            // logic: https://wikidev.in/wiki/assembly/8086/JNAE
            // jb/jnae = Jump on below/not above or equal
            // 01110010 disp
            self.jmp_disp_on(self.CF);
        }

        private static void HandleJBE(CPU self)
        {
            // jbe/jna jump on below or equal/not above
            // 01110110 disp
            self.jmp_disp_on(self.CF || self.ZF);
        }

        private static void HandleJCXZ(CPU self)
        {
            // jump on cx zero
            // 11100011 disp
            self.jmp_disp_on(self.cx == 0);
        }

        private static void HandleJE(CPU self)
        {
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
            // Logic: https://wikidev.in/wiki/assembly/8086/JL
            // JL/JNGE jump on less/not greater or equal
            // 01111100 disp
            self.jmp_disp_on(self.SF != self.OF);
        }

        private static void HandleJLE(CPU self)
        {
            // JLE/JNG jump on less or equal/not greater
            // 01111110 disp
            self.jmp_disp_on(self.SF != self.OF || self.ZF);
        }

        private static void HandleJMP(CPU self)
        {
            // Direct within segment
            // 11101001 disp-low disp-high

            // Direct within segment short
            // 11101011 disp

            // Indirect within segment
            // 11111111 mod 100 r/m

            // Direct intersegment
            // 11101010 off-lo off-hi seg-lo seg-hi

            // Inderect intersegment
            // 11111111 mod 101 r/m
            throw new NotImplementedException();
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
            // Logic: https://wikidev.in/wiki/assembly/8086/jnb
            // jnb/jae Jump on not below/above or equal
            // 01110011 disp
            self.jmp_disp_on(!self.CF);
        }

        private static void HandleJNBE(CPU self)
        {
            // jnbe/ja jump on not below or equal/above
            // 01110111
            self.jmp_disp_on(!(self.CF || self.ZF));
        }

        private static void HandleJNE(CPU self)
        {
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
            // Logic: https://wikidev.in/wiki/assembly/8086/jnl
            // jnl/jnge jump on not less/greater or equal
            // 01111101 disp
            self.jmp_disp_on(self.SF == self.OF);
        }

        private static void HandleJNLE(CPU self)
        {
            // JNLE/JG jump on not less or equal/greater
            // 01111111 disp
            self.jmp_disp_on((self.SF == self.OF) || self.ZF);
        }

        private static void HandleJNO(CPU self)
        {
            // jump on not overflow
            // 01110001 disp
            self.jmp_disp_on(!self.OF);
        }

        private static void HandleJNP(CPU self)
        {
            // jnp/jpo jump on not parity/parity odd
            // 01111011 disp
            self.jmp_disp_on(!self.PF);
        }

        private static void HandleJNS(CPU self)
        {
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
            // jump on overflow
            // 01110000 disp
            self.jmp_disp_on(self.OF);
        }

        private static void HandleJP(CPU self)
        {
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
            // Jump on sign
            // 01111000 disp
            self.jmp_disp_on(self.SF);
        }

        private static void HandleJZ(CPU self)
        {
            HandleJE(self);
        }

        private static void HandleLAHF(CPU self)
        {
            // Load AH with flags
            // 10011111
            throw new NotImplementedException();
        }

        private static void HandleLDS(CPU self)
        {
            // Load pointer to DS
            // 11000101 mod reg r/m
            throw new NotImplementedException();
        }

        private static void HandleLEA(CPU self)
        {
            // Load effective address to register
            // 10001101 mod reg r/m
            throw new NotImplementedException();
        }

        private static void HandleLES(CPU self)
        {
            // Load pointer to ES
            // 11000100 mod reg r/m
            throw new NotImplementedException();
        }

        private static void HandleLOCK(CPU self)
        {
            // Bus lock prefix
            // 11110000
            throw new NotImplementedException();
        }

        private static void HandleLODS(CPU self)
        {
            // Load string
            // 1010110w
            throw new NotImplementedException();
        }

        private static void HandleLOOP(CPU self)
        {
            // loop cx times
            // 11100010 disp
            throw new NotImplementedException();
        }

        private static void HandleLOOPE(CPU self)
        {
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
            // loopnz/loopne loop while not zero/not equal
            // 11100000 disp
            throw new NotImplementedException();
        }

        private static void HandleLOOPZ(CPU self)
        {
            HandleLOOPE(self);
        }

        private static void HandleMOVS(CPU self)
        {
            // 1010010w
            throw new NotImplementedException();
        }

        private static void HandleMUL(CPU self)
        {
            // Multiply unsigned
            // 1111011w mod 100 r/m
            // part of group?
            throw new NotImplementedException();
        }

        private static void HandleNEG(CPU self)
        {
            // 1111011w mod 011 r/m
            // Part of group ?

            throw new NotImplementedException();
        }

        private static void HandleNOP(CPU self)
        {
            // No operation
            // 10010000
            throw new NotImplementedException();
        }

        private static void HandleNOT(CPU self)
        {
            // 1111011w mod 010 r/m
            throw new NotImplementedException();
        }

        private static void HandleOR(CPU self)
        {
            // Register/memory and register to either
            // 000010dw mod reg r/m

            // Immedate to register/memory
            // 1000000w mod 001 r/m data, data if w=1

            // Immediate to accumulator
            // 0000110w data data if w = 1
            throw new NotImplementedException();
        }

        private static void HandleOUT(CPU self)
        {
            // 1110011w port from al/ax
            // 1110110w (dx=port) from al/ax
            throw new NotImplementedException();
        }

        private static void HandlePOP(CPU self)
        {
            // 10001111 mod 000 rm
            // 01011 reg
            // 000 seg 111
            throw new NotImplementedException();
        }

        private static void HandlePOPF(CPU self)
        {
            // Pop flags
            throw new NotImplementedException();
        }

        private static void HandlePUSH(CPU self)
        {

            // 11111111 mod 110 rm
            // 01010 reg
            // 000 seg 110
            throw new NotImplementedException();
        }

        private static void HandlePUSHF(CPU self)
        {
            // Push flags
            throw new NotImplementedException();
        }

        private static void HandleRCL(CPU self)
        {
            // Rotate through carry left
            // 110100vw mod 010 r/m
            throw new NotImplementedException();
        }

        private static void HandleRCR(CPU self)
        {
            // Rotate through carry right
            // 110100vw mod 011 r/m
            throw new NotImplementedException();
        }

        private static void HandleREP(CPU self)
        {
            // Repeat
            // 1111001z
            throw new NotImplementedException();
        }

        private static void HandleRET(CPU self)
        {
            // Return from CALL
            // Within segment
            // 11000011

            // Within segment adding immediate to SP
            // 11000010 data-lo data-hi

            // Intersegment
            // 11001011

            // Intersegment, adding immediate to SP
            // 11000010 data-lo data-hi

            throw new NotImplementedException();
        }

        private static void HandleROL(CPU self)
        {
            // 110100vw mod 000 r/m
            throw new NotImplementedException();
        }

        private static void HandleROR(CPU self)
        {
            // 110100vw mod 001 r/m
            throw new NotImplementedException();
        }

        private static void HandleSAHF(CPU self)
        {
            // Store AH into flags
            // 10011110
            throw new NotImplementedException();
        }

        private static void HandleSAR(CPU self)
        {
            // 110100vw mod 111 r/m
            throw new NotImplementedException();
        }

        private static void HandleSBB(CPU self)
        {
            // Subtract with borrow

            // r/m and r to either
            // 000110dw mod reg r/m

            // imm to reg/mem
            // 100000sw mod 011 r/m data, data if s:w=01
            // Part of Immediate group

            // imm from accum
            // 0001110w data, data if w=1
            throw new NotImplementedException();
        }

        private static void HandleSCAS(CPU self)
        {
            // Scan string
            // 1010111w
            throw new NotImplementedException();
        }

        private static void HandleSHL_SAL(CPU self)
        {
            // 110100vw mod 100 r/m
            throw new NotImplementedException();
        }

        private static void HandleSHR(CPU self)
        {
            // Shift logical right
            // 110100vw mod 101 r/m
            throw new NotImplementedException();
        }

        private static void HandleSTC(CPU self)
        {
            // Set carry
            // 11111001
            throw new NotImplementedException();
        }

        private static void HandleSTD(CPU self)
        {
            // Set direction
            // 11111101
            throw new NotImplementedException();
        }

        private static void HandleSTI(CPU self)
        {
            // Set interrupt
            // 11111011
            throw new NotImplementedException();
        }

        private static void HandleSTOS(CPU self)
        {
            // Store string
            // 1010101w
            throw new NotImplementedException();
        }

        private static void HandleSUB(CPU self)
        {
            // r/m and r to either
            // 001010dw mod reg r/m

            // imm to reg/mem
            // 100000sw mod 101 r/m data, data if s:w=01
            // Part of Immediate group
            
            // imm from accum
            // 0010110w data, data if w=1
            throw new NotImplementedException();
        }

        private static void HandleTEST(CPU self)
        {
            // And function to flags, no result

            // R/m and register
            // 1000010w mod reg r/m

            // Immediate data and register/memory
            // 1111011w mod 000 r/m data, data if w=1

            // Immediate data and accumulator
            // 1010100w data, data if w=1
            throw new NotImplementedException();
        }

        private static void HandleWAIT(CPU self)
        {
            // 10011011
            throw new NotImplementedException();
        }

        private static void HandleXCHG(CPU self)
        {
            // 1000011w mod reg rm
            // 10010 reg -- with accum
            throw new NotImplementedException();
        }

        private static void HandleXLAT(CPU self)
        {
            // translate byte to al
            // 11010111
            throw new NotImplementedException();
        }

        private static void HandleXOR(CPU self)
        {
            // Register/memory and register to either
            // 001100dw mod reg r/m

            // Immediate to register/memory
            // 1000000w mod 110 r/m data, data if w=1

            // Immediate to accumulator
            // 0011010w data, data if w=1
            throw new NotImplementedException();
        }
    }
}
