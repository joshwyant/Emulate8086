using Emulate8086.Console;
using static SDL2.SDL;

class ControlRenderer
{
    protected SDL2Session session;
    protected ControlTheme theme;
    public ControlRenderer(SDL2Session session, ControlTheme theme)
    {
        this.session = session;
        this.theme = theme;
    }
    public void Draw3DBorder(float btnx, float btny, float btnw, float btnh, bool pressed)
    {
        float alpha = 255;

        var color = pressed ? theme.ButtonShadow : theme.ButtonLight;
        SDL_SetRenderDrawColor(session.Renderer.Handle, color.r, color.g, color.b, (byte)alpha);
        SDL_RenderDrawLineF(session.Renderer.Handle, btnx, btny, btnx + btnw - 2, btny);
        SDL_RenderDrawLineF(session.Renderer.Handle, btnx, btny + 1, btnx, btny + btnh - 2);

        color = pressed ? theme.ButtonDkShadow : theme.ButtonHilight;
        SDL_SetRenderDrawColor(session.Renderer.Handle, color.r, color.g, color.b, (byte)alpha);
        SDL_RenderDrawLineF(session.Renderer.Handle, btnx + 1, btny + 1, btnx + btnw - 3, btny + 1);
        SDL_RenderDrawLineF(session.Renderer.Handle, btnx + 1, btny + 2, btnx + 1, btny + btnh - 2);

        color = pressed ? theme.ButtonHilight : theme.ButtonDkShadow;
        SDL_SetRenderDrawColor(session.Renderer.Handle, color.r, color.g, color.b, (byte)alpha);
        SDL_RenderDrawLineF(session.Renderer.Handle, btnx, btny + btnh - 1, btnx + btnw - 1, btny + btnh - 1);
        SDL_RenderDrawLineF(session.Renderer.Handle, btnx + btnw - 1, btny, btnx + btnw - 1, btny + btnh - 2);

        color = pressed ? theme.ButtonLight : theme.ButtonShadow;
        SDL_SetRenderDrawColor(session.Renderer.Handle, color.r, color.g, color.b, (byte)alpha);
        SDL_RenderDrawLineF(session.Renderer.Handle, btnx + 1, btny + btnh - 2, btnx + btnw - 2, btny + btnh - 2);
        SDL_RenderDrawLineF(session.Renderer.Handle, btnx + btnw - 2, btny + 1, btnx + btnw - 2, btny + btnh - 3);
    }

    public void DrawButton(float btnx, float btny, float btnw, float btnh, bool pressed)
    {
        Draw3DBorder(btnx, btny, btnw, btnh, pressed);

        var color = pressed ? theme.ButtonAlternateFace : theme.ButtonFace;
        SDL_SetRenderDrawColor(session.Renderer.Handle, color.r, color.g, color.b, 255);
        var buttonFaceRect = new SDL_FRect()
        {
            x = btnx + 2,
            y = btny + 2,
            w = btnw - 4,
            h = btnh - 4
        };
        SDL_RenderFillRectF(session.Renderer.Handle, ref buttonFaceRect);
    }

    public void DrawWindow(float wleft, float wtop, float wwidth, float wheight, bool inset, SDL_Color windowColor)
    {
        Draw3DBorder(wleft, wtop, wwidth, wheight, false);

        var color = theme.ActiveTitle;
        SDL_SetRenderDrawColor(session.Renderer.Handle, color.r, color.g, color.b, 255);
        var titleRect = new SDL_FRect()
        {
            x = wleft + 2,
            y = wtop + 2,
            w = wwidth - 4,
            h = 18
        };
        SDL_RenderFillRectF(session.Renderer.Handle, ref titleRect);

        var closeButtonRect = new SDL_FRect()
        {
            x = wleft + wwidth - 16 - 4,
            y = wtop + 4,
            w = 16,
            h = 14
        };
        DrawButton(closeButtonRect.x, closeButtonRect.y, closeButtonRect.w, closeButtonRect.h, false);

        color = theme.ButtonText;
        SDL_SetRenderDrawColor(session.Renderer.Handle, color.r, color.g, color.b, 255);
        SDL_RenderDrawLineF(session.Renderer.Handle,
            closeButtonRect.x + 4,
            closeButtonRect.y + 4,
            closeButtonRect.x + closeButtonRect.w - 6,
            closeButtonRect.y + closeButtonRect.h - 4);
        SDL_RenderDrawLineF(session.Renderer.Handle,
            closeButtonRect.x + 5,
            closeButtonRect.y + 4,
            closeButtonRect.x + closeButtonRect.w - 5,
            closeButtonRect.y + closeButtonRect.h - 4);
        SDL_RenderDrawLineF(session.Renderer.Handle,
            closeButtonRect.x + closeButtonRect.w - 6,
            closeButtonRect.y + 4,
            closeButtonRect.x + 4,
            closeButtonRect.y + closeButtonRect.h - 4);
        SDL_RenderDrawLineF(session.Renderer.Handle,
            closeButtonRect.x + closeButtonRect.w - 5,
            closeButtonRect.y + 4,
            closeButtonRect.x + 5,
            closeButtonRect.y + closeButtonRect.h - 4);

        color = windowColor;
        SDL_SetRenderDrawColor(session.Renderer.Handle, color.r, color.g, color.b, 255);

        var clientRect = new SDL_FRect()
        {
            x = wleft + 2,
            y = wtop + 2 + 18,
            w = wwidth - 4,
            h = wheight - 4 - 18,
        };
        if (inset)
        {
            Draw3DBorder(clientRect.x, clientRect.y, clientRect.w, clientRect.h, true);
            clientRect.x += 2;
            clientRect.y += 2;
            clientRect.w -= 4;
            clientRect.h -= 4;
        }
        SDL_RenderFillRectF(session.Renderer.Handle, ref clientRect);

    }
}