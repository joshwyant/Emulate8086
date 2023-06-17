using System.Runtime.CompilerServices;

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

        Memory memory;
        byte insByte;
        int csip_start;
        Instruction ins;

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

        public void SetModRMData(ushort data, bool? w, int addr, bool is_reg, Register reg)
        {
            if (is_reg)
            {
                SetReg(reg, data, w);
            }
            else
            {
                memory.setDataAt(addr, data, w);
            }
        }

        public ushort GetModRMData(bool w, int addr, bool is_reg, Register reg)
        {
            if (is_reg)
            {
                return GetReg(reg, w);
            }
            else
            {
                return memory.dataAt(addr, w);
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
                insReg = (Register)middle;
            }
            else if ((instructionFlags & InstructionDecoderFlags.ModRMReg) != 0)
            {
                insExtOpcode = middle;
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
                        // Return DISP directly as mem. addr.
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

        int modrm_addr => modrm_seg_addr * 16 + modrm_addr;

        InstructionDecoderFlags instructionFlags;
        PrefixFlags prefix;
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

            prefix = PrefixFlags.None;
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


            // Check for instruction prefixes.
            if ((flags & InstructionDecoderFlags.Pfix) != 0)
            {
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
                insReg = (Register)(insByte & 0b00000111);
                // W flag could be positioned after embedded register.
                flag_mask = 0b00001000;
            }
            // Decode embedded segment register.
            else if ((flags & InstructionDecoderFlags.Seg) != 0)
            {
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
                // Sometimes, the W flag refers to a different operation, not the data byte
                if (insW && (instructionFlags & InstructionDecoderFlags.Word) != 0)
                {
                    ins_data |= (ushort)(memory[csip++] << 8);
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
                disp |= (short)(ushort)(memory[csip++] << 8);
            }
        }

        public void Clock()
        {
            csip_start = csip;

            // Read the instruction byte
            insByte = memory[csip++];

            // Get the instruction type from the matrix
            ins = instructionMatrix[insByte >> 4, insByte & 0xF];

            // Look up the implementation for the instruction type
            var impl = instructionImpls[ins];

            // Call the instruction
            prefix = PrefixFlags.None;
            impl(this);
            if (prefix == PrefixFlags.None)
            {
                prefix = PrefixFlags.None;
            }
        }
    }
}