using System.Runtime.CompilerServices;

namespace Emulate8086.Processor
{
    public class Memory
    {
        public Memory(int size)
        {
            contents = new byte[size];
        }

        byte[] contents;

        public byte this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => contents[index];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => contents[index] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort wordAt(int address)
        {
            return (ushort)(contents[address] | contents[address + 1] << 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setWordAt(int address, ushort value)
        {
            contents[address] = (byte)value;
            contents[address + 1] = (byte)(value >> 8);
        }
    }
}