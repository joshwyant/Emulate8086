using SDL2;

namespace Emulate8086.Console;

class SDL2KeyboardDriver : KeyboardDriver
{
    SDL2Session session;
    public SDL2KeyboardDriver(SDL2Session session)
    {
        this.session = session;
    }
    public override bool CheckForKey(out char ascii, out byte scancode)
    {
        ascii = '\0';
        scancode = 0;
        if (session.KeyDownEvents.TryPeek(out SDL.SDL_KeyboardEvent e))
        {
            ascii = MapKeyToAscii(e);
            scancode = CorrectedScanCode(e);
            return true;
        }
        return false;
    }

    static byte CorrectedScanCode(SDL.SDL_KeyboardEvent e)
    {
        return e.keysym.sym switch
        {
            SDL.SDL_Keycode.SDLK_DOWN => 0x50,
            SDL.SDL_Keycode.SDLK_UP => 0x48,
            SDL.SDL_Keycode.SDLK_LEFT => 0x4B,
            SDL.SDL_Keycode.SDLK_RIGHT => 0x4D,
            _ => (byte)e.keysym.scancode
        };
    }

    public override (char ascii, byte scancode) WaitForKey()
    {
        SDL.SDL_KeyboardEvent e;
        while (!session.KeyDownEvents.TryDequeue(out e) && session.PollEvents()) ;
        return (MapKeyToAscii(e), CorrectedScanCode(e));
    }

    static char MapKeyToAscii(SDL.SDL_KeyboardEvent info)
    {
        if (info.keysym.sym >= SDL.SDL_Keycode.SDLK_a && info.keysym.sym <= SDL.SDL_Keycode.SDLK_z)
        {
            var i = info.keysym.sym - SDL.SDL_Keycode.SDLK_a;
            if ((info.keysym.mod & SDL.SDL_Keymod.KMOD_CTRL) != 0)
            {
                return (char)(1 + i);
            }
            else if ((info.keysym.mod & SDL.SDL_Keymod.KMOD_ALT) != 0)
            {
                return '\0';
            }
            else if ((info.keysym.mod & SDL.SDL_Keymod.KMOD_SHIFT) != 0 ^ (info.keysym.mod & SDL.SDL_Keymod.KMOD_CAPS) != 0)
            {
                return (char)('A' + i);
            }
            else
            {
                return (char)('a' + i);
            }
        }
        else if (info.keysym.sym >= SDL.SDL_Keycode.SDLK_0 && info.keysym.sym <= SDL.SDL_Keycode.SDLK_9)
        {
            var index = info.keysym.sym - SDL.SDL_Keycode.SDLK_0;
            if ((info.keysym.mod & SDL.SDL_Keymod.KMOD_CTRL) != 0)
            {
                return info.keysym.sym switch
                {
                    SDL.SDL_Keycode.SDLK_2 => '\x00',
                    SDL.SDL_Keycode.SDLK_6 => '\x1E',
                    _ => '\0'
                };
            }
            else
            {
                if ((info.keysym.mod & SDL.SDL_Keymod.KMOD_SHIFT) != 0)
                {
                    return index != 0 ? "!@#$%^&*("[index - 1] : ')';
                }
                else
                {
                    return (char)('0' + index);
                }
            }
        }
        else
        {
            if ((info.keysym.mod & SDL.SDL_Keymod.KMOD_ALT) != 0)
            {

                return info.keysym.sym switch
                {
                    SDL.SDL_Keycode.SDLK_SPACE => ' ',
                    _ => '\0'
                };
            }
            else if ((info.keysym.mod & SDL.SDL_Keymod.KMOD_CTRL) != 0)
            {
                return info.keysym.sym switch
                {
                    SDL.SDL_Keycode.SDLK_BACKSPACE => '\x7F',
                    SDL.SDL_Keycode.SDLK_RETURN => '\n',
                    SDL.SDL_Keycode.SDLK_RETURN2 => '\n',
                    SDL.SDL_Keycode.SDLK_KP_ENTER => '\n',
                    SDL.SDL_Keycode.SDLK_ESCAPE => '\x1B',
                    SDL.SDL_Keycode.SDLK_SPACE => ' ',
                    _ => '\0'
                };
            }
            else
            {
                switch (info.keysym.sym)
                {
                    default:
                        if ((info.keysym.mod & SDL.SDL_Keymod.KMOD_SHIFT) != 0)
                        {
                            return info.keysym.sym switch
                            {
                                SDL.SDL_Keycode.SDLK_BACKSPACE => '\x08',
                                SDL.SDL_Keycode.SDLK_DELETE => '\x2E',
                                SDL.SDL_Keycode.SDLK_DOWN => '\x32',
                                SDL.SDL_Keycode.SDLK_END => '\x31',
                                SDL.SDL_Keycode.SDLK_RETURN => '\r',
                                SDL.SDL_Keycode.SDLK_RETURN2 => '\r',
                                SDL.SDL_Keycode.SDLK_KP_ENTER => '\r',
                                SDL.SDL_Keycode.SDLK_ESCAPE => '\x1B',
                                SDL.SDL_Keycode.SDLK_HOME => '\x37',
                                SDL.SDL_Keycode.SDLK_INSERT => '\x30',
                                SDL.SDL_Keycode.SDLK_KP_5 => '\x35',
                                SDL.SDL_Keycode.SDLK_KP_MULTIPLY => '\0',
                                SDL.SDL_Keycode.SDLK_KP_MINUS => '\x2D',
                                SDL.SDL_Keycode.SDLK_KP_PLUS => '\x2B',
                                SDL.SDL_Keycode.SDLK_KP_DIVIDE => '\x2F',
                                SDL.SDL_Keycode.SDLK_LEFT => '\x34',
                                SDL.SDL_Keycode.SDLK_PAGEDOWN => '\x33',
                                SDL.SDL_Keycode.SDLK_PAGEUP => '\x39',
                                SDL.SDL_Keycode.SDLK_PRINTSCREEN => '\0',
                                SDL.SDL_Keycode.SDLK_RIGHT => '\x36',
                                SDL.SDL_Keycode.SDLK_SPACE => '\x20',
                                SDL.SDL_Keycode.SDLK_TAB => '\x00',
                                SDL.SDL_Keycode.SDLK_UP => '\x38',
                                SDL.SDL_Keycode.SDLK_BACKQUOTE => '~',
                                SDL.SDL_Keycode.SDLK_MINUS => '_',
                                SDL.SDL_Keycode.SDLK_EQUALS => '+',
                                SDL.SDL_Keycode.SDLK_LEFTBRACKET => '{',
                                SDL.SDL_Keycode.SDLK_RIGHTBRACKET => '}',
                                SDL.SDL_Keycode.SDLK_BACKSLASH => '|',
                                SDL.SDL_Keycode.SDLK_SEMICOLON => ':',
                                SDL.SDL_Keycode.SDLK_QUOTE => '"',
                                SDL.SDL_Keycode.SDLK_COMMA => '<',
                                SDL.SDL_Keycode.SDLK_PERIOD => '>',
                                SDL.SDL_Keycode.SDLK_SLASH => '?',
                                _ => (char)(info.keysym.unicode > 255 ? 0 : info.keysym.unicode)
                            };
                        }
                        else
                        {
                            return info.keysym.sym switch
                            {
                                SDL.SDL_Keycode.SDLK_BACKSPACE => '\x08',
                                SDL.SDL_Keycode.SDLK_DELETE => '\0',
                                SDL.SDL_Keycode.SDLK_DOWN => '\0',
                                SDL.SDL_Keycode.SDLK_END => '\0',
                                SDL.SDL_Keycode.SDLK_RETURN => '\x0D',
                                SDL.SDL_Keycode.SDLK_RETURN2 => '\x0D',
                                SDL.SDL_Keycode.SDLK_KP_ENTER => '\x0D',
                                SDL.SDL_Keycode.SDLK_ESCAPE => '\x1B',
                                SDL.SDL_Keycode.SDLK_HOME => '\0',
                                SDL.SDL_Keycode.SDLK_INSERT => '\0',
                                SDL.SDL_Keycode.SDLK_KP_5 => '\0',
                                SDL.SDL_Keycode.SDLK_KP_MULTIPLY => '\x2A',
                                SDL.SDL_Keycode.SDLK_KP_MINUS => '\x2D',
                                SDL.SDL_Keycode.SDLK_KP_PLUS => '\x2B',
                                SDL.SDL_Keycode.SDLK_KP_DIVIDE => '\x2F',
                                SDL.SDL_Keycode.SDLK_LEFT => '\0',
                                SDL.SDL_Keycode.SDLK_PAGEDOWN => '\0',
                                SDL.SDL_Keycode.SDLK_PAGEUP => '\0',
                                SDL.SDL_Keycode.SDLK_PRINTSCREEN => '\0',
                                SDL.SDL_Keycode.SDLK_RIGHT => '\0',
                                SDL.SDL_Keycode.SDLK_SPACE => '\x20',
                                SDL.SDL_Keycode.SDLK_TAB => '\x09',
                                SDL.SDL_Keycode.SDLK_UP => '\0',
                                SDL.SDL_Keycode.SDLK_BACKQUOTE => '`',
                                SDL.SDL_Keycode.SDLK_MINUS => '-',
                                SDL.SDL_Keycode.SDLK_EQUALS => '=',
                                SDL.SDL_Keycode.SDLK_LEFTBRACKET => '[',
                                SDL.SDL_Keycode.SDLK_RIGHTBRACKET => ']',
                                SDL.SDL_Keycode.SDLK_BACKSLASH => '\\',
                                SDL.SDL_Keycode.SDLK_SEMICOLON => ';',
                                SDL.SDL_Keycode.SDLK_QUOTE => '\'',
                                SDL.SDL_Keycode.SDLK_COMMA => ',',
                                SDL.SDL_Keycode.SDLK_PERIOD => '.',
                                SDL.SDL_Keycode.SDLK_SLASH => '/',
                                _ => info.keysym.unicode >= 32 && info.keysym.unicode <= 127 ? (char)info.keysym.unicode : '\0'
                            };
                        }
                }
            }
        }
    }
}