using SDL2;
using System;

namespace Emulate8086.Console;

class SDL2Session
{
    Logger log;
    public SDL2Session(Logger log)
    {
        this.log = log;
    }
    public void Main()
    {
        // Initialize SDL
        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
        {
            log.Error($"SDL could not initialize! SDL_Error: {SDL.SDL_GetError()}");
            return;
        }

        // Create window
        IntPtr window = SDL.SDL_CreateWindow(
            "SDL2 Window",
            SDL.SDL_WINDOWPOS_UNDEFINED,
            SDL.SDL_WINDOWPOS_UNDEFINED,
            640,
            400,
            SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN
        );

        if (window == IntPtr.Zero)
        {
            log.Error($"Window could not be created! SDL_Error: {SDL.SDL_GetError()}");
            return;
        }

        // Create renderer
        IntPtr renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
        if (renderer == IntPtr.Zero)
        {
            log.Error($"Renderer could not be created! SDL_Error: {SDL.SDL_GetError()}");
            return;
        }

        // Main loop
        bool quit = false;
        while (!quit)
        {
            SDL.SDL_Event e;
            while (SDL.SDL_PollEvent(out e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    quit = true;
                }
            }

            // Clear screen
            SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
            SDL.SDL_RenderClear(renderer);

            // Update screen
            SDL.SDL_RenderPresent(renderer);
        }

        // Clean up
        SDL.SDL_DestroyRenderer(renderer);
        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
    }
}