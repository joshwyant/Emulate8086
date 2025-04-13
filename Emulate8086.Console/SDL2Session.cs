using System.Runtime.InteropServices;
using static SDL2.SDL;
using SDL2;

namespace Emulate8086.Console;

class SDL2Session : IDisposable
{
    public event Action? Draw;
    Logger log;
    public SDL2Window? Window { get; }
    public SDL2Renderer? Renderer { get; }
    public SDL2Session(Logger log)
    {
        this.log = log;

        NativeLibrary.SetDllImportResolver(typeof(SDL_image).Assembly, (name, assembly, path) =>
        {
            if (name == "SDL2_image")
            {
                var fullPath = "/opt/homebrew/lib/libSDL2_image.dylib";
                IntPtr handle;
                if (NativeLibrary.TryLoad(fullPath, out handle))
                    return handle;
            }
            return IntPtr.Zero;
        });

        // Initialize SDL
        if (SDL_Init(SDL_INIT_VIDEO) < 0)
        {
            log.Error($"SDL could not initialize! SDL_Error: {SDL_GetError()}");
            return;
        }

        // Create window
        Window = SDL2Window.Create("Emulate8086", 640, 400);

        if (Window == null)
        {
            log.Error($"Window could not be created! SDL_Error: {SDL_GetError()}");
            return;
        }

        // Create renderer
        Renderer = SDL2Renderer.Create(Window);
        if (Renderer == null)
        {
            log.Error($"Renderer could not be created! SDL_Error: {SDL_GetError()}");
            return;
        }
    }

    public Queue<SDL_KeyboardEvent> KeyDownEvents { get; } = new Queue<SDL_KeyboardEvent>();
    // bool captured = false;
    // int captured_x = 0;
    // int captured_y = 0;
    public bool PollEvents()
    {
        SDL_Event e;

        // // Very first thing for best performance
        // if (captured)
        // {
        //     var buttons = SDL_GetGlobalMouseState(out int globalx, out int globaly);
        //     if ((buttons & SDL_BUTTON_LMASK) != 0)
        //     {
        //         SDL_SetWindowPosition(Window.Handle, globalx - captured_x, globaly - captured_y);
        //     }
        //     else
        //     {
        //         captured = false;
        //     }
        // }

        // Draw before window is shown
        if (Renderer != null)
        {
            Redraw();
        }

        while (SDL_PollEvent(out e) != 0)
        {
            switch (e.type)
            {
                case SDL_EventType.SDL_QUIT:
                    return false;
                // case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                //     if (e.motion.x > 2 && e.motion.x < 638 && e.motion.y > 2 && e.motion.y < 20)
                //     {
                //         SDL_CaptureMouse(SDL_bool.SDL_TRUE);
                //         captured = true;
                //         captured_x = e.motion.x;
                //         captured_y = e.motion.y;
                //     }
                //     break;
                // case SDL_EventType.SDL_WINDOWEVENT:
                //     if (e.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED)
                //     {
                //         var buttons = SDL_GetGlobalMouseState(out int globalx, out int globaly);
                //         SDL_GetWindowPosition(Window.Handle, out int winx, out int winy);
                //         var mousex = globalx - winx;
                //         var mousey = globaly - winy;
                //         if (mousex > 2 && mousex < 638 && mousey > 2 && mousey < 20 && (buttons & SDL_BUTTON_LMASK) != 0)
                //         {
                //             SDL_CaptureMouse(SDL_bool.SDL_TRUE);
                //             captured = true;
                //             captured_x = mousex;
                //             captured_y = mousey;
                //         }
                //     }
                //     break;
                // case SDL_EventType.SDL_MOUSEBUTTONUP:
                //     SDL_CaptureMouse(SDL_bool.SDL_FALSE);
                //     captured = false;
                //     break;
                case SDL_EventType.SDL_KEYDOWN:
                    switch (e.key.keysym.sym)
                    {
                        case SDL_Keycode.SDLK_LSHIFT:
                        case SDL_Keycode.SDLK_RSHIFT:
                        case SDL_Keycode.SDLK_LCTRL:
                        case SDL_Keycode.SDLK_RCTRL:
                        case SDL_Keycode.SDLK_LALT:
                        case SDL_Keycode.SDLK_RALT:
                            break;
                        default:
                            KeyDownEvents.Enqueue(e.key);
                            break;
                    }
                    // // If using Windows-style accelerators:
                    // if (e.key.keysym.sym == SDL_Keycode.SDLK_F4 && (e.key.keysym.mod & SDL_Keymod.KMOD_ALT) != 0)
                    // {
                    //     return false;
                    // }
                    break;
            }
        }

        return true;
    }

    public void Redraw()
    {
        // Clear screen
        SDL_SetRenderDrawColor(Renderer.Handle, 0, 0, 0, 255);
        SDL_RenderClear(Renderer.Handle);

        Draw?.Invoke();

        // Update screen
        SDL_RenderPresent(Renderer.Handle);
    }

    public class SDL2Renderer : IDisposable
    {
        public static SDL2Renderer? Create(SDL2Window window)
        {
            var handle = SDL_CreateRenderer(window.Handle, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            return handle == IntPtr.Zero ? null : new(handle);
        }

        public IntPtr Handle { get; protected set; }
        protected SDL2Renderer(IntPtr handle)
        {
            Handle = handle;
        }

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                SDL_DestroyRenderer(Handle);
            }
            Handle = IntPtr.Zero;
        }
    }

    public class SDL2Window : IDisposable
    {
        public static SDL2Window? Create(string title, int width, int height)
        {
            var handle = SDL_CreateWindow(
                title,
                SDL_WINDOWPOS_UNDEFINED,
                SDL_WINDOWPOS_UNDEFINED,
                width,
                height,
                SDL_WindowFlags.SDL_WINDOW_SHOWN // | SDL_WindowFlags.SDL_WINDOW_BORDERLESS
            );

            return handle == IntPtr.Zero ? null : new(handle);
        }
        public IntPtr Handle { get; protected set; }
        protected SDL2Window(IntPtr handle)
        {
            Handle = handle;
        }
        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                SDL_DestroyWindow(Handle);
            }
            Handle = IntPtr.Zero;
        }
    }

    public void Dispose()
    {
        Window?.Dispose();
        Renderer?.Dispose();
        SDL_Quit();
    }
}