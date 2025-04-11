using SDL2;
using static SDL2.SDL_image;
using static SDL2.SDL;
using static SDL2.SDL_gfx;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Emulate8086.Console;

partial class SDL2DisplayDriver : DisplayDriver
{
    [LibraryImport("libSDL2.dylib")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial int SDL_SetTextureColorMod(IntPtr texture, byte r, byte g, byte b);

    SDL2Session session;
    Logger log;
    int cols = 80, rows = 25;
    nint fontTex;
    public SDL2DisplayDriver(SDL2Session session, Logger log)
    {
        this.session = session;
        this.log = log;
        fontTex = ProcessFont();
        session.Draw += SDL2Session_Draw;
    }

    private void SDL2Session_Draw()
    {
        Random r = new();
        for (var i = 0; i < 80; i++)
            for (var j = 0; j < 25; j++)
            {
                draw_char(session.Renderer.Handle, (char)(r.Next() % 256), i * 8, j * 16, MatchColor(i % 16), MatchColor((i + j + 8) % 16));
            }
    }

    unsafe void draw_char(nint r, char ch, int x, int y, SDL_Color color, SDL_Color bgcol)
    {
        SDL_SetTextureColorMod(fontTex, color.r, color.g, color.b);

        var src = new SDL_Rect
        {
            x = (ch % 32) * 8,
            y = (ch / 32) * 16,
            w = 8,
            h = 16
        };
        var dst = new SDL_Rect { x = x, y = y, w = 8, h = 16 };

        SDL_SetRenderDrawColor(r, bgcol.r, bgcol.g, bgcol.b, bgcol.a);
        SDL_RenderFillRect(r, ref dst);

        SDL_RenderCopy(r, fontTex, ref src, ref dst);
    }

    void draw_string(int x, int y, string text)
    {
        for (var i = 0; i < text.Length; i++)
        {
            draw_char(session.Renderer.Handle, text[i], x + 8 * i, y, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 }, new SDL_Color() { r = 0, g = 0, b = 0, a = 255 });
        }
    }

    SDL_PixelFormat format;
    unsafe nint ProcessFont()
    {
        SDL_Surface* old = (SDL_Surface*)IMG_Load("cp437_8x16.png");
        if (old == null)
        {
            log.Error($"Font processing failed: {IMG_GetError()}");
        }
        SDL_Surface* surface = (SDL_Surface*)SDL_ConvertSurfaceFormat((nint)old, SDL_PIXELFORMAT_RGBA8888, 0);
        SDL_FreeSurface((nint)old);

        if (SDL_MUSTLOCK((nint)surface)) SDL_LockSurface((nint)surface);

        var pixels = (uint*)surface->pixels;
        var fmt = (SDL_PixelFormat*)surface->format;
        format = *fmt;
        var keyColor = pixels[0];

        for (int i = 0; i < surface->w * surface->h; ++i)
        {
            uint px = pixels[i];
            if (px == keyColor)
            {
                // transparent
                pixels[i] = SDL_MapRGBA((nint)fmt, 255, 255, 255, 0);
            }
            else
            {
                // white foreground
                pixels[i] = SDL_MapRGBA((nint)fmt, 255, 255, 255, 255);
            }
        }

        // if (SDL_MUSTLOCK((nint)surface)) SDL_UnlockSurface((nint)surface);

        nint fontTex = SDL_CreateTextureFromSurface(session.Renderer.Handle, (nint)surface);

        SDL_SetTextureBlendMode(fontTex, SDL_BlendMode.SDL_BLENDMODE_BLEND);

        SDL_FreeSurface((nint)surface);

        return fontTex;
    }
    public override int BackgroundColor { get; set; } = 0;
    public override int ForegroundColor { get; set; } = 7;
    bool cursorVisible;
    public override bool CursorVisible { set => cursorVisible = value; }
    int cursorLeft = 0, cursorTop = 0;
    VideoMode mode = VideoMode.Text80x25_16Color_B8000;
    int pageno = 0;

    public override void Clear()
    {
        cursorLeft = 0;
        cursorTop = 0;
    }

    public override (int left, int top) GetCursorPosition()
    {
        return (cursorLeft, cursorTop);
    }

    public override (VideoMode mode, int cols, int pageno) GetVideoMode()
    {
        return (mode, cols, pageno);
    }

    public override void SetCursorPosition(int left, int top)
    {
        cursorLeft = left;
        cursorTop = top;
    }

    public override void SetVideoMode(VideoMode mode, bool clearScreen)
    {
        this.mode = mode;
        if (clearScreen) Clear();
    }

    int tabstop = 8;
    public override void Write(char c)
    {
        switch (c)
        {
            case '\a':
                System.Console.Beep();
                break;
            case '\r':
                cursorLeft = 0;
                break;
            case '\n':
                cursorTop += 1;
                break;
            case '\t':
                cursorLeft = (cursorLeft + tabstop - 1) / tabstop;
                break;
            default:
                cursorLeft++;
                if (cursorLeft == cols)
                {
                    cursorLeft = 0;
                    cursorTop++;
                }
                break;
        }
        if (cursorTop >= rows) cursorTop = rows - 1;
    }

    SDL_Color MatchColor(int color)
    {
        return color switch
        {
            1 => new SDL_Color() { r = 0, g = 0, b = 255, a = 255 },
            2 => new SDL_Color() { r = 0, g = 255, b = 0, a = 255 },
            3 => new SDL_Color() { r = 0, g = 255, b = 255, a = 255 },
            4 => new SDL_Color() { r = 255, g = 0, b = 0, a = 255 },
            5 => new SDL_Color() { r = 255, g = 0, b = 255, a = 255 },
            6 => new SDL_Color() { r = 192, g = 64, b = 0, a = 255 },
            7 => new SDL_Color() { r = 192, g = 192, b = 192, a = 255 },
            8 => new SDL_Color() { r = 64, g = 64, b = 64, a = 255 },
            9 => new SDL_Color() { r = 128, g = 128, b = 255, a = 255 },
            10 => new SDL_Color() { r = 128, g = 255, b = 128, a = 255 },
            11 => new SDL_Color() { r = 128, g = 255, b = 255, a = 255 },
            12 => new SDL_Color() { r = 255, g = 128, b = 128, a = 255 },
            13 => new SDL_Color() { r = 255, g = 128, b = 255, a = 255 },
            14 => new SDL_Color() { r = 255, g = 255, b = 0, a = 255 },
            15 => new SDL_Color() { r = 255, g = 255, b = 255, a = 255 },
            _ => new SDL_Color() { r = 0, g = 0, b = 0, a = 255 },
        };
    }
}