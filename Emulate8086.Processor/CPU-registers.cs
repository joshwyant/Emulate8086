using System.Runtime.CompilerServices;

namespace Emulate8086.Processor
{
    public partial class CPU
    {
        ushort[] register_file = new ushort[8];
        ushort[] segment_register_file = new ushort[4];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort GetReg(Register r, bool word)
        {
            return word ? GetReg16(r) : GetReg8(r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte GetReg8(Register r)
        {
            // Low or high byte of ax/bx/cx/dx.
            // Index into ax/bx/cx/dx.
            var i = (int)r & 0b11;
            // Get ax/bx/cx/dx
            var s = register_file[i];
            // Do we want high or low byte?
            var is_low = ((int)r & 0b100) == 0;
            return (byte)(is_low ?
                s & 0xFF : // Low byte
                s >> 8);   // push high byte down to low
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort GetReg16(Register r)
        {
            return register_file[(int)r];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort GetSeg(Register r)
        {
            return segment_register_file[(int)r];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSeg(Register r, ushort value)
        {
            segment_register_file[(int)r] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetReg16(Register r, ushort val)
        {
            register_file[(int)r] = val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetReg(Register r, ushort val)
        {
            segment_register_file[(int)r] = val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetReg8(Register r, byte val)
        {
            // Low or high byte of ax/bx/cx/dx.
            // Index into ax/bx/cx/dx.
            var i = (int)r & 0b11;
            // Get ax/bx/cx/dx
            var s = register_file[i];
            // Do we want high or low byte?
            var is_low = ((int)r & 0b100) == 0;
            if (is_low)
            {
                // Change the low byte to val
                // by masking the high byte first
                s = (ushort)((s & 0xFF00) | val);
            }
            else
            {
                // Change the high byte to val
                // by masking the low byte
                // and shifting the new value to
                // the higher byte
                s = (ushort)((s & 0x00FF) | (val << 8));
            }
            // Replace the register with the new value
            register_file[i] = s;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetReg(Register r, ushort val, bool word)
        {
            if (word)
            {
                SetReg16(r, val);
            }
            else
            {
                SetReg8(r, (byte)val);
            }
        }

        /*
         ax (0)
         */

        ushort ax
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => register_file[0];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => register_file[0] = value;
        }

        byte al
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (byte)register_file[0];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                register_file[0] &= 0xFF00;
                register_file[0] |= value;
            }
        }
        byte ah
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (byte)(register_file[0] >> 8);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                register_file[0] &= 0x00FF;
                register_file[0] |= (ushort)(value << 8);
            }
        }

        /*
         cx (1)
         */

        ushort cx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => register_file[1];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => register_file[1] = value;
        }

        byte cl
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (byte)register_file[1];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                register_file[1] &= 0xFF00;
                register_file[1] |= value;
            }
        }
        byte ch
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (byte)(register_file[1] >> 8);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                register_file[1] &= 0x00FF;
                register_file[1] |= (ushort)(value << 8);
            }
        }

        /*
         dx (2)
         */

        ushort dx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => register_file[2];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => register_file[2] = value;
        }

        byte dl
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (byte)register_file[2];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                register_file[2] &= 0xFF00;
                register_file[2] |= value;
            }
        }
        byte dh
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (byte)(register_file[2] >> 8);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                register_file[2] &= 0x00FF;
                register_file[2] |= (ushort)(value << 8);
            }
        }

        /*
         bx (3)
         */

        ushort bx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => register_file[3];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => register_file[3] = value;
        }

        byte bl
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (byte)register_file[3];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                register_file[3] &= 0xFF00;
                register_file[3] |= value;
            }
        }
        byte bh
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (byte)(register_file[3] >> 8);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                register_file[3] &= 0x00FF;
                register_file[3] |= (ushort)(value << 8);
            }
        }

        ushort sp
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => register_file[4];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => register_file[4] = value;
        }

        ushort bp
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => register_file[5];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => register_file[5] = value;
        }

        ushort si
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => register_file[6];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => register_file[6] = value;
        }

        ushort di
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => register_file[7];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => register_file[7] = value;
        }

        ushort es
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => segment_register_file[0];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => segment_register_file[0] = value;
        }

        ushort cs
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => segment_register_file[1];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => segment_register_file[1] = value;
        }

        ushort ss
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => segment_register_file[2];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => segment_register_file[2] = value;
        }

        ushort ds
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => segment_register_file[3];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => segment_register_file[3] = value;
        }

        ushort ip;
        public Flags flags;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlag(Flags flag)
        {
            flags |= flag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetFlag(Flags flag)
        {
            return (flags & flag) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearFlag(Flags flag)
        {
            flags &= ~flag;
        }

        public bool CF
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (flags & Flags.CF) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value)
                {
                    flags = value ?
                        flags | Flags.CF :
                        flags & ~Flags.CF;
                }
            }
        }

        public bool PF
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (flags & Flags.PF) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value)
                {
                    flags = value ?
                        flags | Flags.PF :
                        flags & ~Flags.PF;
                }
            }
        }

        public bool AF
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (flags & Flags.AF) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value)
                {
                    flags = value ?
                        flags | Flags.AF :
                        flags & ~Flags.AF;
                }
            }
        }

        public bool ZF
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (flags & Flags.ZF) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value)
                {
                    flags = value ?
                        flags | Flags.ZF :
                        flags & ~Flags.ZF;
                }
            }
        }

        public bool SF
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (flags & Flags.SF) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value)
                {
                    flags = value ?
                        flags | Flags.SF :
                        flags & ~Flags.SF;
                }
            }
        }

        public bool TF
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (flags & Flags.TF) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value)
                {
                    flags = value ?
                        flags | Flags.TF :
                        flags & ~Flags.TF;
                }
            }
        }

        public bool IF
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (flags & Flags.IF) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value)
                {
                    flags = value ?
                        flags | Flags.IF :
                        flags & ~Flags.IF;
                }
            }
        }

        public bool DF
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (flags & Flags.DF) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value)
                {
                    flags = value ?
                        flags | Flags.DF :
                        flags & ~Flags.DF;
                }
            }
        }

        public bool OF
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (flags & Flags.OF) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value)
                {
                    flags = value ?
                        flags | Flags.OF :
                        flags & ~Flags.OF;
                }
            }
        }

        int csip
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (cs << 4) + ip;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => ip = (ushort)(value - (cs << 4));  // to support "csip++"
        }
    }
}