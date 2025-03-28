using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Diagnostics;

namespace Emulate8086.Processor
{
    public partial class CPU
    {
        public CPU(Memory m)
        {
            memory = m;
            cs = 0;
            ip = 0;
        }
        public ushort CS => cs;
        public ushort IP => ip;
        public ushort SS => ss;
        public ushort SP => sp;
        public ushort DS => ds;
        public ushort SI => si;
        public ushort ES => es;
        public ushort DI => di;
        public ushort AX => ax;
        public ushort BX => bx;
        public ushort CX => cx;
        public ushort DX => dx;
        public ushort BP => bp;
        public byte AH => ah;
        public byte AL => al;
        public byte BH => bh;
        public byte BL => bl;
        public byte CH => ch;
        public byte CL => cl;
        public byte DH => dh;
        public byte DL => dl;
        private Action<CPU>?[] interrupt_table = new Action<CPU>?[256];
        public void HookInterrupt(byte index, Action<CPU> action)
        {
            interrupt_table[index] = action;
        }

        public Instruction NextInstruction => instructionMatrix[memory[csip] >> 4, memory[csip] & 0xF];
        public Instruction PreviousInstruction => ins;
        Memory memory;
        byte insByte;
        int csip_start;
        Instruction ins;
        Dictionary<int, IDevice> devices = new();
        public Memory Memory => memory;

        public void Jump(ushort cs, ushort ip)
        {
            this.cs = cs;
            this.ip = ip;
        }

        public void AddDevice(IDevice device, params int[] ports)
        {
            foreach (var port in ports)
            {
                devices[port] = device;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ushort seg_prefix_or_default(Register def = Register.DS)
        {
            var seg_prefix = prefix switch
            {
                PrefixFlags.ES => Register.ES,
                PrefixFlags.DS => Register.DS,
                PrefixFlags.SS => Register.SS,
                PrefixFlags.CS => Register.CS,
                _ => def
            };
            effective_prefix = seg_prefix;
            return GetSeg(seg_prefix);
        }

        public void SetModRMData(ushort data)
        {
            if (modrm_is_reg)
            {
                SetReg(modrm_register, data, insW);
            }
            else
            {
                memory.setDataAt(modrm_addr, data, insW);
            }
        }

        public ushort GetModRMData()
        {
            if (modrm_is_reg)
            {
                return GetReg(modrm_register, insW);
            }
            else
            {
                return memory.dataAt(modrm_addr, insW);
            }
        }

        public void DecodeModRMByte()
        {
            modrm_is_reg = false;
            modrm_eff_addr = 0;
            modrm_seg_addr = 0;
            modrm_register = Register.None;

            var mod = (Register)(modrm >> 6);
            var rm = (Register)(modrm & 0b111);
            var middle = (modrm & 0b00111000) >> 3;

            // Middle byte could be an extended opcode, or another register.
            if ((instructionFlags & InstructionDecoderFlags.ModRMOpcode) != 0)
            {
                insExtOpcode = middle;
            }
            else if ((instructionFlags & InstructionDecoderFlags.ModRMReg) != 0)
            {
                insReg = (Register)middle;
            }
            else if ((instructionFlags & InstructionDecoderFlags.ModRMSeg) != 0)
            {
                insReg = (Register)(middle & 0b011);
            }

            if (mod == Register.DispReg)
            {
                modrm_is_reg = true;
                modrm_register = rm;
                modrm_seg_addr = rm switch
                {
                    Register.SP => seg_prefix_or_default(Register.SS),
                    Register.BP => seg_prefix_or_default(Register.SS),
                    _ => seg_prefix_or_default()
                };
                return;
            }
            if (mod == Register.Disp8)
            {
                disp = (sbyte)memory[csip++];
            }
            else // mod = Disp16 or Disp0
            {
                // Disp16, or mod/rm = 00/110
                if (mod == Register.Disp16 || (int)rm == 0b110)
                {
                    var lo = memory[csip++];
                    var hi = memory[csip++];
                    var val = (short)(ushort)((hi << 8) | lo);
                    if (mod == Register.Disp16)
                    {
                        disp = val;
                    }
                    else // mod/rm = 00/110 (Disp0/BP)
                    {
                        // This is a special case:
                        // Return effective address directly as mem. addr.
                        modrm_eff_addr = (ushort)((hi << 8) | lo);
                        modrm_seg_addr = seg_prefix_or_default();
                        return;
                    }
                }
                else // mod = disp0
                {
                    disp = 0;
                }
            }
            // Now use rm and disp.
            // Get the correct segment.
            var def_seg = rm switch
            {
                Register.MemBPSI => Register.SS,
                Register.MemBPDI => Register.SS,
                Register.MemBP => Register.SS,
                _ => Register.DS
            };
            modrm_seg_addr = seg_prefix_or_default(def_seg);

            // Get the effective address.
            var addr = rm switch
            {
                Register.MemBXSI => (ushort)(bx + si),
                Register.MemBXDI => (ushort)(bx + di),
                Register.MemBPSI => (ushort)(bp + si),
                Register.MemBPDI => (ushort)(bp + di),
                Register.MemSI => si,
                Register.MemDI => di,
                Register.MemBP => bp,
                Register.MemBX => bx,
                _ => 0
            };
            modrm_eff_addr = (ushort)(addr + disp);
        }

        int modrm_addr => modrm_seg_addr * 16 + modrm_eff_addr;

        InstructionDecoderFlags instructionFlags;
        PrefixFlags prefix;
        bool is_prefix;
        private bool has_ins_reg;
        private bool has_ins_seg;
        Register insReg;
        bool insW;
        bool insD;
        bool insS;
        bool insV;
        bool insZ;
        int insExtOpcode;
        ushort ins_data;
        ushort ins_addr;
        ushort ins_seg;
        byte modrm;
        ushort modrm_eff_addr;
        ushort modrm_seg_addr;
        bool modrm_is_reg;
        Register modrm_register;
        private Register effective_prefix;
        short disp;
        private bool wflag_enabled;
        private bool b_accepted_flag;
        private bool w_accepted_flag;
        private bool is_immediate_word;

        private PrefixFlags current_segment =>
            effective_prefix switch
            {
                Register.CS => PrefixFlags.CS,
                Register.ES => PrefixFlags.ES,
                Register.SS => PrefixFlags.SS,
                _ => PrefixFlags.DS,
            };

        void DecodeInstruction(InstructionDecoderFlags flags)
        {
            instructionFlags = flags;

            insReg = Register.None;
            insW = false;
            insD = false;
            insS = false;
            insV = false;
            insZ = false;
            insExtOpcode = 0;
            ins_data = 0;
            ins_addr = 0;
            ins_seg = 0;
            modrm = 0;
            disp = 0;
            is_prefix = false;
            has_ins_reg = false;
            has_ins_seg = false;


            // Check for instruction prefixes.
            if ((flags & InstructionDecoderFlags.Pfix) != 0)
            {
                is_prefix = true;
                switch (ins)
                {
                    case Instruction.CSPrefix:
                        prefix |= PrefixFlags.CS;
                        break;
                    case Instruction.DSPrefix:
                        prefix |= PrefixFlags.DS;
                        break;
                    case Instruction.ESPrefix:
                        prefix |= PrefixFlags.ES;
                        break;
                    case Instruction.SSPrefix:
                        prefix |= PrefixFlags.SS;
                        break;
                    case Instruction.REP:
                        prefix |= PrefixFlags.REP;
                        break;
                    case Instruction.LOCK:
                        prefix |= PrefixFlags.LOCK;
                        break;
                }
            }

            // Check for Z flag
            if ((flags & InstructionDecoderFlags.Z) != 0)
            {
                insZ = (insByte & 1) != 0;
            }

            // Decode embedded register
            var flag_mask = 1;  // Position of next flag
            if ((flags & InstructionDecoderFlags.Reg) != 0)
            {
                has_ins_reg = true;
                insReg = (Register)(insByte & 0b00000111);
                // W flag could be positioned after embedded register.
                flag_mask = 0b00001000;
            }
            // Decode embedded segment register.
            else if ((flags & InstructionDecoderFlags.Seg) != 0)
            {
                has_ins_seg = true;
                insReg = (Register)((insByte & 0b00011000) >> 3);
            }

            // Check for W flag
            if ((flags & InstructionDecoderFlags.W) != 0)
            {
                insW = (insByte & flag_mask) != 0;
                flag_mask <<= 1; // Next flag is to the left of the W flag.
            }

            // There can be a D, S, or V flag to the left of the W flag.
            if ((flags & InstructionDecoderFlags.D) != 0)
            {
                insD = (insByte & flag_mask) != 0;
            }
            else if ((flags & InstructionDecoderFlags.S) != 0)
            {
                insS = (insByte & flag_mask) != 0;
            }
            else if ((flags & InstructionDecoderFlags.V) != 0)
            {
                insV = (insByte & flag_mask) != 0;
            }

            // Read ModRM byte
            if ((flags & InstructionDecoderFlags.ModRM) != 0)
            {
                modrm = memory[csip++];

                DecodeModRMByte();
            }

            if ((instructionFlags & (InstructionDecoderFlags.Byte | InstructionDecoderFlags.Word)) != 0)
            {
                var dataByte = memory[csip++];
                ins_data = insS ?
                    (ushort)(short)(sbyte)dataByte :
                    dataByte;
            }
            if ((instructionFlags & InstructionDecoderFlags.Word) != 0)
            {
                // Sometimes, the W flag refers to a different operation, not the data byte; so don't use that.
                if (insW)
                {
                    if (insS && ((ins_data & 0b10000000) != 0))
                    {
                        ins_data |= 0xFF00;  // Sign-extend
                    }
                    else if (!insS) // No sign extend, read full word
                    {
                        ins_data |= (ushort)(memory[csip++] << 8);
                    }
                }
            }

            // Get an address
            if ((instructionFlags & (InstructionDecoderFlags.Addr | InstructionDecoderFlags.AddL)) != 0)
            {
                ins_addr = (ushort)(memory[csip++] | (memory[csip++] << 8));
            }

            // Get address segment
            if ((instructionFlags & InstructionDecoderFlags.AddL) != 0)
            {
                ins_seg = (ushort)(memory[csip++] | (memory[csip++] << 8));
            }

            // Calculate displacement
            if ((instructionFlags & (InstructionDecoderFlags.DispB | InstructionDecoderFlags.DispW)) != 0)
            {
                disp = (sbyte)memory[csip++];
            }
            if ((instructionFlags & InstructionDecoderFlags.DispW) != 0)
            {
                disp &= 0xFF;
                disp |= (short)(ushort)(memory[csip++] << 8);
            }

            var wflag_enabled = (instructionFlags & InstructionDecoderFlags.W) != 0;
            var b_accepted_flag = (instructionFlags & InstructionDecoderFlags.Byte) != 0;
            var w_accepted_flag = (instructionFlags & InstructionDecoderFlags.Word) != 0;
            is_immediate_word = w_accepted_flag ? wflag_enabled ? insW : true : false;
        }

        public void Clock(bool in_breakpoint = false)
        {
            csip_start = csip;

            // Read the instruction byte
            insByte = memory[csip++];

            // Get the instruction type from the matrix
            ins = instructionMatrix[insByte >> 4, insByte & 0xF];

            // Look up the implementation for the instruction type
            var impl = instructionImpls[ins];

            // Call the instruction
            if (in_breakpoint)
            {
                Debugger.Break();
            }
            impl(this);
            if (!is_prefix)
            {
                // Done with any prefix.
                // Only clear prefixes after non-prefix
                // instruction executes.
                prefix = PrefixFlags.None;
            }
        }
    }
}