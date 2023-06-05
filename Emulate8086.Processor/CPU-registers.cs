using System.Runtime.CompilerServices;

namespace Emulate8086.Processor
{
    public partial class CPU
    {
        byte al, ah;
        byte bl, bh;
        byte cl, ch;
        byte dl, dh;
        ushort cs, ds, es, ss;
        ushort si, di;
        ushort ip;
        ushort sp, bp;
        byte flagsl, flagsh;
        ushort ax
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (ushort)(ah << 8 | al);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                ah = (byte)(value >> 8);
                al = (byte)value;
            }
        }
        ushort bx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (ushort)(bh << 8 | bl);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                bh = (byte)(value >> 8);
                bl = (byte)value;
            }
        }
        ushort cx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (ushort)(ch << 8 | cl);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                ch = (byte)(value >> 8);
                cl = (byte)value;
            }
        }
        ushort dx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (ushort)(dh << 8 | dl);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                dh = (byte)(value >> 8);
                dl = (byte)value;
            }
        }
        ushort csip
        {
            get => (ushort)((cs << 4) + ip);
            set => ip = (ushort)(value - (cs << 4));  // to support "csip++"
        }
    }
}