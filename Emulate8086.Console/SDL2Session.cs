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

    public bool PollEvents()
    {
        SDL_Event e;
        while (SDL_PollEvent(out e) != 0)
        {
            if (e.type == SDL_EventType.SDL_QUIT)
            {
                return false;
            }
        }

        if (Renderer == null) return true;

        Redraw();

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
                SDL_WindowFlags.SDL_WINDOW_SHOWN
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