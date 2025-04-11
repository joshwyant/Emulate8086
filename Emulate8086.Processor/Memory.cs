using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Emulate8086.Processor
{
    public class Memory
    {
        internal CPU? cpu;
        public Memory(int size)
        {
            contents = new byte[size];
            last = NewWindow(contents);
        }

        public Window NewWindow(byte[] contents, int address, int offset = 0, int? size = null)
        {
            return NewWindow(address, address + (size ?? (contents.Length - offset)),
                index => contents[index + offset],
                (index, value) => contents[index + offset] = value);
        }

        public Window NewWindow(byte[] contents, int start = 0)
        {
            return NewWindow(start, start + contents.Length, contents.ElementAt, (index, value) => contents[index] = value);
        }

        public Window NewWindow(int start, int end, Func<int, byte> read, Action<int, byte> write)
        {
            previous = last;
            return last = new Window(Mapped, start, end, read, write);
        }

        public void ClearMaps()
        {
            Mapped.Clear();
            last = null!;
            previous = null;
        }

        public int Size => contents.Length;

        byte[] contents;

        public class Window
        {
            public Window(LinkedList<Window> list, int start, int end, Func<int, byte> read, Action<int, byte> write)
            {
                Start = start;
                End = end;
                Read = read;
                Write = write;
                Node = list.AddFirst(this);
            }
            public int Start;
            public int End;
            public Func<int, byte> Read;
            public Action<int, byte> Write;
            public LinkedListNode<Window> Node;
        }

        public readonly LinkedList<Window> Mapped = new();
        Window last;
        Window? previous;

        public byte[] RawBytes => contents;

        private HashSet<int> BDAWritten = new HashSet<int>();
        private HashSet<ushort> SegmentsWritten = new HashSet<ushort>();

        public byte this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((index >= 0 && index < 0x500 || index >= 0xF0000) && !BDAWritten.Contains(index))
                {
                    Console.WriteLine($"{cpu?.CS:X4}:{cpu?.IP} R. bad BDA {index:X5}");
                    // Trap uninitialized reads from the BIOS data area
                    //Debugger.Break();
                    BDAWritten.Add(index);
                }
                if (!SegmentsWritten.Contains((ushort)(index >> 4)))
                {
                    Console.WriteLine($"{cpu?.CS:X4}:{cpu?.IP} Rd. bad sg. {index >> 4:X4}");
                    SegmentsWritten.Add((ushort)(index >> 4));
                    //Debugger.Break();
                }
                if (index >= last.Start && index < last.End || UseRange(index))
                {
                    return last.Read(index - last.Start);
                }
                return 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (index >= last.Start && index < last.End || UseRange(index))
                {
                    last.Write(index - last.Start, value);
                }
                if (index >= 0 && index < 0x500 && !BDAWritten.Contains(index))
                {
                    BDAWritten.Add(index);
                    Console.WriteLine($"{cpu?.CS:X4}:{cpu?.IP} Wrote to BDA {index:X5}h");
                }
                SegmentsWritten.Add((ushort)(index >> 4));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SwitchRange(Window r)
        {
            var prev = last;
            last = r;
            previous = prev;
            Mapped.Remove(r.Node);
            Mapped.AddFirst(r.Node);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool UseRange(int address)
        {
            if (previous is not null && address >= previous.Start && address < previous.End)
            {
                SwitchRange(previous);
                return true;
            }
            var current = previous?.Node.Next ?? last.Node.Next;

            for (; current is LinkedListNode<Window> node; current = current.Next)
            {
                var range = node.Value;
                if (address >= range.Start && address < range.End)
                {
                    SwitchRange(range);
                    return true;
                }
            }
            return false;
        }

        public byte this[int segment, int offset]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[segment * 16 + offset];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => this[segment * 16 + offset] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort wordAt(int address)
        {
            return (ushort)(this[address] | this[address + 1] << 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort wordAt(int segment, int offset)
        {
            return (ushort)(this[segment * 16 + offset] | this[segment * 16 + offset + 1] << 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setWordAt(int address, ushort value)
        {
            this[address] = (byte)value;
            this[address + 1] = (byte)(value >> 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setWordAt(int segment, int offset, ushort value)
        {
            this[segment * 16 + offset] = (byte)value;
            this[segment * 16 + offset + 1] = (byte)(value >> 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort dataAt(int addr, bool w)
        {
            return w ? wordAt(addr) : this[addr];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort dataAt(int seg, int off, bool w)
        {
            return w ? wordAt(seg, off) : this[seg * 16 + off];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setDataAt(int address, ushort value, bool w)
        {
            if (w)
            {
                setWordAt(address, value);
            }
            else
            {
                this[address] = (byte)value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void setDataAt(int seg, int off, ushort value, bool w)
        {
            if (w)
            {
                setWordAt(seg, off, value);
            }
            else
            {
                this[seg * 16 + off] = (byte)value;
            }
        }
    }
}