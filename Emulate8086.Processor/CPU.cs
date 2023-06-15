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
            return GetSeg(
                seg_prefix != Register.None ?
                    seg_prefix :
                    def
            );
        }

        public void SetModRMData(ushort data, bool w, int addr, bool is_reg, Register reg)
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

        public void CalcModRMAddress(ref int modrm_addr, out int addr, out bool is_reg, out Register reg)
        {
            CalcModRMEffectiveAddress(ref modrm_addr, out var eff_addr, out var seg_addr, out var is_reg1, out var reg1);
            addr = (seg_addr << 4) + eff_addr;
            reg = reg1;
            is_reg = is_reg1;
        }

        public void CalcModRMEffectiveAddress(ref int modrm_addr, out ushort eff_addr, out ushort seg_addr, out bool is_reg, out Register reg)
        {
            is_reg = false;
            eff_addr = 0;
            seg_addr = 0;
            reg = Register.None;
            var modrm = memory[modrm_addr++];
            var mod = (Register)(modrm >> 6);
            var rm = (Register)(modrm & 0b111);
            if (mod == Register.DispReg)
            {
                is_reg = true;
                reg = rm;
                seg_addr = rm switch
                {
                    Register.SP => seg_prefix_or_default(Register.SS),
                    Register.BP => seg_prefix_or_default(Register.SS),
                    _ => seg_prefix_or_default()
                };
                return;
            }
            int disp;
            if (mod == Register.Disp8)
            {
                disp = (sbyte)(memory[modrm_addr++]);
            }
            else // mod = Disp16 or Disp0
            {
                // Disp16, or mod/rm = 00/110
                if (mod == Register.Disp16 || (int)rm == 0b110)
                {
                    var lo = memory[modrm_addr++];
                    var hi = memory[modrm_addr++];
                    var val = (ushort)((hi << 8) | lo);
                    if (mod == Register.Disp16)
                    {
                        disp = val;
                    }
                    else // mod/rm = 00/110 (Disp0/BP)
                    {
                        // This is a special case:
                        // Return DISP directly as mem. addr.
                        eff_addr = (ushort)((hi << 8) | lo);
                        seg_addr = seg_prefix_or_default();
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
            seg_addr = seg_prefix_or_default(def_seg);

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
            eff_addr = (ushort)(addr + disp);
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
            is_seg_prefix = false;
            impl(this);
            if (!is_seg_prefix)
            {
                seg_prefix = Register.None;
            }
        }
    }
}