using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulate8086.Processor
{
    public partial class CPU
    {
        private void MovSegrToRM(ref int csip)
        {
            // 10001100 | mod 0 reg r/m
            var modrm = memory[csip];
            CalcModRMAddress(ref csip, out var addr, out var is_reg, out var reg);
            var segreg = (Register)((modrm & 0b00011000) >> 3);
            SetModRMData(GetSeg(segreg), true, addr, is_reg, reg);
        }

        private void MovRMToSegr(ref int csip)
        {
            // 10001110 | mod 0 reg r/m
            var modrm = memory[csip];
            CalcModRMAddress(ref csip, out var addr, out var is_reg, out var reg);
            var segreg = (Register)((modrm & 0b00011000) >> 3);
            var data = GetModRMData(true, addr, is_reg, reg);
            SetSeg(segreg, data);
        }

        private void MovAccumToMemory(ref int csip)
        {
            // 1010001w | addr low | addr high
            var w = (insByte & 0b00000001) != 0;
            var addr = memory.wordAt(csip);
            csip += 2;
            memory.setDataAt(addr, ax, w);
        }

        private void MovMemToAccum(ref int csip)
        {
            // 1010000w | addr low | addr high
            var w = (insByte & 0b00000001) != 0;
            var addr = memory.wordAt(csip);
            csip += 2;
            // AX and AL
            SetReg(Register.AX, memory.dataAt(addr, w), w);
        }

        private void MovImmToR(ref int csip)
        {
            // 1011 w reg | data | data if w=1
            var w = (insByte & 0b00001000) != 0;
            var reg = (Register)(insByte & 0b00000111);
            int data = memory[csip++];
            if (w)
            {
                data |= memory[csip++] << 8;
            }
            SetReg(reg, (ushort)data, w);
        }

        private void MovImmToRM(ref int csip)
        {
            ushort data;
            var w = (insByte & 0b00000001) != 0;
            CalcModRMAddress(ref csip, out var addr, out var is_reg, out var reg);
            data = memory[addr];
            csip++;
            if (w)
            {
                data |= (ushort)(memory[addr + 1] << 8);
            }
            SetModRMData(data, w, addr, is_reg, reg);
        }

        private void MovRMToFromR(ref int csip)
        {
            var d = (insByte & 0b00000010) != 0;
            var w = (insByte & 0b00000001) != 0;
            var modrm = memory[csip];
            var reg2 = (Register)((modrm & 0b00111000) >> 3);
            CalcModRMAddress(ref csip, out var addr, out var is_reg, out var reg);
            if (d)
            {
                var from = GetModRMData(w, addr, is_reg, reg);
                SetReg(reg2, from, w);
            }
            else
            {
                var data = GetReg(reg2, w);
                SetModRMData(data, w, addr, is_reg, reg);
            }
        }
    }
}
