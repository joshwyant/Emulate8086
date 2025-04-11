namespace Emulate8086.Console;

abstract class DisplayDriver
{
    public abstract int BackgroundColor { get; set; }
    public abstract int ForegroundColor { get; set; }
    public abstract void SetCursorPosition(int left, int top);
    public abstract (int left, int top) GetCursorPosition();
    public abstract bool CursorVisible { set; }
    public abstract void Write(char c);
    public abstract void Clear();
    public abstract void SetVideoMode(VideoMode mode, bool clearScreen);
    public abstract (VideoMode mode, int cols, int pageno) GetVideoMode();
}
// http://vitaly_filatov.tripod.com/ng/asm/asm_023.1.html
enum VideoMode : byte
{
    Text40x25_B8000 = 0x00,
    Text40x25_16Color_B8000 = 0x01,
    Text80x25_B8000 = 0x02,
    Text80x25_16Color_B8000 = 0x03,
    Graphics320x200_4Color_B8000 = 0x04,
    Graphics320x200_B8000 = 0x05,
    Graphics640x200_2Color_B8000 = 0x06,
    Text80x25BW_B0000 = 0x07,
    Graphics160x200_16ColorPCjr_B0000 = 0x08,
    Graphics320x200_16ColorPCjr_B0000 = 0x09,
    Graphics640x200_4ColorPCjr_B0000 = 0x0A,
    Reserved1EGA = 0x0B,
    Reserved2EGA = 0x0C,
    Graphics320x200_16ColorEGA_A0000 = 0x0D,
    Graphics640x200_16ColorEGA_A0000 = 0x0E,
    Graphics640x350_BW_EGA_A0000 = 0x0F,
    Graphics640x350_16ColorEGA_A0000 = 0x10,
}