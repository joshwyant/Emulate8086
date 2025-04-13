using Emulate8086.Console;
using static SDL2.SDL;

class ControlTheme
{
    public SDL_Color Scrollbar = new SDL_Color() { r = 129, g = 192, b = 192, a = 255 };
    public SDL_Color Background = new SDL_Color() { r = 0, g = 128, b = 128, a = 255 };
    public SDL_Color ActiveTitle = new SDL_Color() { r = 0, g = 0, b = 128, a = 255 };
    public SDL_Color InactiveTitle = new SDL_Color() { r = 128, g = 128, b = 128, a = 255 };
    public SDL_Color Menu = new SDL_Color() { r = 192, g = 192, b = 192, a = 255 };
    public SDL_Color Window = new SDL_Color() { r = 255, g = 255, b = 255, a = 255 };
    public SDL_Color WindowFrame = new SDL_Color() { r = 0, g = 0, b = 0, a = 255 };
    public SDL_Color MenuText = new SDL_Color() { r = 0, g = 0, b = 0, a = 255 };
    public SDL_Color WindowText = new SDL_Color() { r = 0, g = 0, b = 0, a = 255 };
    public SDL_Color TitleText = new SDL_Color() { r = 255, g = 255, b = 255, a = 255 };
    public SDL_Color ActiveBorder = new SDL_Color() { r = 192, g = 192, b = 192, a = 255 };
    public SDL_Color InactiveBorder = new SDL_Color() { r = 192, g = 192, b = 192, a = 255 };
    public SDL_Color AppWorkspace = new SDL_Color() { r = 128, g = 128, b = 128, a = 255 };
    public SDL_Color Hilight = new SDL_Color() { r = 0, g = 0, b = 128, a = 255 };
    public SDL_Color HilightText = new SDL_Color() { r = 255, g = 255, b = 255, a = 255 };
    public SDL_Color ButtonFace = new SDL_Color() { r = 192, g = 192, b = 192, a = 255 };
    public SDL_Color ButtonShadow = new SDL_Color() { r = 128, g = 128, b = 128, a = 255 };
    public SDL_Color GrayText = new SDL_Color() { r = 128, g = 128, b = 128, a = 255 };
    public SDL_Color ButtonText = new SDL_Color() { r = 0, g = 0, b = 0, a = 255 };
    public SDL_Color InactiveTitleText = new SDL_Color() { r = 192, g = 192, b = 192, a = 255 };
    public SDL_Color ButtonHilight = new SDL_Color() { r = 255, g = 255, b = 255, a = 255 };
    public SDL_Color ButtonDkShadow = new SDL_Color() { r = 0, g = 0, b = 0, a = 255 };
    public SDL_Color ButtonLight = new SDL_Color() { r = 223, g = 223, b = 223, a = 255 };
    public SDL_Color InfoText = new SDL_Color() { r = 0, g = 0, b = 0, a = 255 };
    public SDL_Color InfoWindow = new SDL_Color() { r = 255, g = 255, b = 255, a = 255 };
    public SDL_Color ButtonAlternateFace = new SDL_Color() { r = 181, g = 181, b = 181, a = 255 };
    public SDL_Color HotTrackingColor = new SDL_Color() { r = 0, g = 0, b = 255, a = 255 };
    public SDL_Color GradientActiveTitle = new SDL_Color() { r = 0, g = 0, b = 128, a = 255 };
    public SDL_Color GradientInactiveTitle = new SDL_Color() { r = 128, g = 128, b = 128, a = 255 };
}