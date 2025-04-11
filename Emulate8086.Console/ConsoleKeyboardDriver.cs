namespace Emulate8086.Console;
using System;
using System.Collections.Concurrent;

class ConsoleKeyboardDriver : KeyboardDriver
{
    ConcurrentQueue<(char, byte)> concurrentKeyboardBuffer = new();
    ManualResetEvent keyboardWaiting = new(false);
    public Task BackgroundTask { get; }
    Queue<(char, byte)> keyboardBuffer = new();

    public ConsoleKeyboardDriver()
    {
        BackgroundTask = Task.Run(async () =>
        {
            // For now let's only try it for redirected
            if (!Console.IsInputRedirected) return;

            while (true)
            {
                if (Console.IsInputRedirected)
                {
                    var chars = Console.ReadLine();
                    if (chars == null) break;

                    foreach (var c in chars)
                    {
                        await Task.Delay(50);
                        var scancode = MapASCIIToScanCode((char)c);
                        concurrentKeyboardBuffer.Enqueue(((char)c, scancode));
                    }
                    await Task.Delay(50);
                    concurrentKeyboardBuffer.Enqueue(('\r', 0x1C));
                    keyboardWaiting.Set();
                }
                else
                {
                    // var keyInfo = Console.ReadKey(true);
                    // MapConsoleKeyInfoToScanCode(keyInfo, out byte scancode, out byte ascii);
                    // concurrentKeyboardBuffer.Enqueue(((char)ascii, scancode));
                }
            }
        });

        var _ = Task.Run(() =>
        {
            if (!Console.IsInputRedirected) return; // Only for redirected input for now

            Thread.Sleep(2500);
            var t = DateTime.Now;
            var str = $"{t.Month}-{t.Day}-{t.Year}\r{t.Hour}:{t.Minute:D2}\r";
            foreach (var c in str)
            {
                Thread.Sleep(100);
                concurrentKeyboardBuffer.Enqueue((c, MapASCIIToScanCode(c)));
            }
        });
    }
    public override (char ascii, byte scancode) WaitForKey()
    {
        if (keyboardBuffer.Count > 0)
        {
            keyboardBuffer.TryDequeue(out (char, byte) result);
            keyboardWaiting.Reset();
            return result;
        }
        else if (Console.IsInputRedirected)
        {
            keyboardWaiting.WaitOne();
            concurrentKeyboardBuffer.TryDequeue(out (char, byte) result);
            keyboardWaiting.Reset();
            return result;
        }
        else
        {
            var keyInfo = Console.ReadKey(true);
            MapConsoleKeyInfoToScanCode(keyInfo, out byte scancode, out byte ascii);
            return ((char)ascii, scancode);
        }
    }
    public override bool CheckForKey(out char ascii, out byte scancode)
    {
        // https://stanislavs.org/helppc/int_16-1.html
        ascii = '\0';
        scancode = 0;
        if (Console.IsInputRedirected)
        {
            if (concurrentKeyboardBuffer.TryDequeue(out (char, byte) result))
            {
                (ascii, scancode) = result;
                keyboardBuffer.Enqueue(result);
                return true;
            }
        }
        else if (Console.KeyAvailable)
        {
            var keyInfo = Console.ReadKey(true);
            MapConsoleKeyInfoToScanCode(keyInfo, out scancode, out byte asciibyte);
            ascii = (char)asciibyte;
            keyboardBuffer.Enqueue((ascii, scancode));
            return true;
        }
        return false;
    }

    // https://stanislavs.org/helppc/scan_codes.html
    static void MapConsoleKeyInfoToScanCode(ConsoleKeyInfo info, out byte scancode, out byte ascii)
    {
        scancode = 0;
        ascii = 0;
        if (info.Key >= ConsoleKey.A && info.Key <= ConsoleKey.Z)
        {
            var i = info.Key - ConsoleKey.A;
            scancode = LetterScanCodes[i];
            if (info.Modifiers.HasFlag(ConsoleModifiers.Control))
            {
                ascii = (byte)(1 + i);
            }
            else if (info.Modifiers.HasFlag(ConsoleModifiers.Alt))
            {
                ascii = 0;
            }
            else if (info.Modifiers.HasFlag(ConsoleModifiers.Shift))
            {
                ascii = (byte)('A' + i);
            }
            else
            {
                ascii = (byte)('a' + i);
            }
        }
        else if (info.Key >= ConsoleKey.D0 && info.Key <= ConsoleKey.D9)
        {
            var index = info.Key == ConsoleKey.D0 ? 0 : 1 + (info.Key - ConsoleKey.D1);
            if (info.Modifiers.HasFlag(ConsoleModifiers.Alt))
            {
                scancode = (byte)(info.Key == ConsoleKey.D0
                    ? 0x81 : 0x78 + (info.Key - ConsoleKey.D1));
            }
            else if (info.Modifiers == ConsoleModifiers.Control)
            {
                scancode = info.Key switch
                {
                    ConsoleKey.D2 => 0x03,
                    ConsoleKey.D6 => 0x07,
                    _ => 0
                };
                ascii = info.Key switch
                {
                    ConsoleKey.D2 => 0x00,
                    ConsoleKey.D6 => 0x1E,
                    _ => 0
                };
            }
            else
            {
                scancode = (byte)(info.Key == ConsoleKey.D0
                    ? 0x0B : 0x02 + (info.Key - ConsoleKey.D1));

                if (info.Modifiers.HasFlag(ConsoleModifiers.Shift))
                {
                    ascii = (byte)"!@#$%^&*()"[index];
                }
                else
                {
                    ascii = (byte)('0' + index);
                }
            }
        }
        else if (info.Key >= ConsoleKey.F1 && info.Key <= ConsoleKey.F10)
        {
            var index = (int)(info.Key - ConsoleKey.F1);
            var start = 0x3B;
            if (info.Modifiers.HasFlag(ConsoleModifiers.Alt))
            {
                start = 0x68;
            }
            else if (info.Modifiers.HasFlag(ConsoleModifiers.Control))
            {
                start = 0x5E;
            }
            else if (info.Modifiers.HasFlag(ConsoleModifiers.Shift))
            {
                start = 0x54;
            }
            scancode = (byte)(start + index);
        }
        else
        {
            if (info.Modifiers.HasFlag(ConsoleModifiers.Alt))
            {
                scancode = info.Key switch
                {
                    ConsoleKey.F11 => 0x8B,
                    ConsoleKey.F12 => 0x8C,
                    ConsoleKey.Backspace => 0x0E,
                    ConsoleKey.Delete => 0xA3,
                    ConsoleKey.DownArrow => 0xA0,
                    ConsoleKey.End => 0x9F,
                    ConsoleKey.Enter => 0xA6,
                    ConsoleKey.Escape => 0x01,
                    ConsoleKey.Home => 0x97,
                    ConsoleKey.Insert => 0xA2,
                    ConsoleKey.NumPad5 => 0,
                    ConsoleKey.Multiply => 0x37,
                    ConsoleKey.Subtract => 0x4A,
                    ConsoleKey.Add => 0x4E,
                    ConsoleKey.Divide => 0xA4,
                    ConsoleKey.LeftArrow => 0x9B,
                    ConsoleKey.PageDown => 0xA1,
                    ConsoleKey.PageUp => 0x99,
                    ConsoleKey.PrintScreen => 0,
                    ConsoleKey.RightArrow => 0x9D,
                    ConsoleKey.Spacebar => 0x39,
                    ConsoleKey.Tab => 0xA5,
                    ConsoleKey.UpArrow => 0x98,
                    _ => (byte)info.Key
                };
                ascii = info.Key switch
                {
                    ConsoleKey.Spacebar => 0x20,
                    _ => 0
                };
            }
            else if (info.Modifiers.HasFlag(ConsoleModifiers.Control))
            {
                scancode = info.Key switch
                {
                    ConsoleKey.F11 => 0x89,
                    ConsoleKey.F12 => 0x8A,
                    ConsoleKey.Backspace => 0x0E,
                    ConsoleKey.Delete => 0x93,
                    ConsoleKey.DownArrow => 0x91,
                    ConsoleKey.End => 0x75,
                    ConsoleKey.Enter => 0x1C,
                    ConsoleKey.Escape => 0x01,
                    ConsoleKey.Home => 0x77,
                    ConsoleKey.Insert => 0x92,
                    ConsoleKey.NumPad5 => 0x8F,
                    ConsoleKey.Multiply => 0x96,
                    ConsoleKey.Subtract => 0x8E,
                    ConsoleKey.Add => 0,
                    ConsoleKey.Divide => 0x95,
                    ConsoleKey.LeftArrow => 0x73,
                    ConsoleKey.PageDown => 0x76,
                    ConsoleKey.PageUp => 0x84,
                    ConsoleKey.PrintScreen => 0x72,
                    ConsoleKey.RightArrow => 0x74,
                    ConsoleKey.Spacebar => 0x39,
                    ConsoleKey.Tab => 0x94,
                    ConsoleKey.UpArrow => 0x8D,
                    _ => (byte)info.Key
                };
                ascii = info.Key switch
                {
                    ConsoleKey.Backspace => 0x7F,
                    ConsoleKey.Enter => 0x0A,
                    ConsoleKey.Escape => 0x1B,
                    ConsoleKey.Spacebar => 0x20,
                    _ => 0
                };
            }
            else
            {
                switch (info.Key)
                {
                    case ConsoleKey.F11:
                        scancode = (byte)(info.Modifiers.HasFlag(ConsoleModifiers.Shift)
                            ? 0x87 : 0x85);
                        break;
                    case ConsoleKey.F12:
                        scancode = (byte)(info.Modifiers.HasFlag(ConsoleModifiers.Shift)
                            ? 0x88 : 0x86);
                        break;
                    case ConsoleKey.NumPad5:
                        scancode = (byte)(info.Modifiers.HasFlag(ConsoleModifiers.Shift)
                            ? 0x4C : 0);
                        break;
                    case ConsoleKey.Multiply:
                        scancode = (byte)(info.Modifiers.HasFlag(ConsoleModifiers.Shift)
                            ? 0 : 0x37);
                        ascii = 0x2A;
                        break;
                    default:
                        scancode = info.Key switch
                        {
                            ConsoleKey.Backspace => 0x0E,
                            ConsoleKey.Delete => 0x53,
                            ConsoleKey.DownArrow => 0x50,
                            ConsoleKey.End => 0x4F,
                            ConsoleKey.Enter => 0x1C,
                            ConsoleKey.Escape => 0x01,
                            ConsoleKey.Home => 0x47,
                            ConsoleKey.Insert => 0x52,
                            ConsoleKey.Subtract => 0x4A,
                            ConsoleKey.Add => 0x4E,
                            ConsoleKey.Divide => 0x35,
                            ConsoleKey.LeftArrow => 0x4B,
                            ConsoleKey.PageDown => 0x51,
                            ConsoleKey.PageUp => 0x49,
                            ConsoleKey.RightArrow => 0x4D,
                            ConsoleKey.Spacebar => 0x39,
                            ConsoleKey.Tab => 0x0F,
                            ConsoleKey.UpArrow => 0x48,
                            _ => (byte)info.Key
                        };
                        if (info.Modifiers.HasFlag(ConsoleModifiers.Shift))
                        {
                            ascii = info.Key switch
                            {
                                ConsoleKey.Backspace => 0x08,
                                ConsoleKey.Delete => 0x2E,
                                ConsoleKey.DownArrow => 0x32,
                                ConsoleKey.End => 0x31,
                                ConsoleKey.Enter => 0x0D,
                                ConsoleKey.Escape => 0x1B,
                                ConsoleKey.Home => 0x37,
                                ConsoleKey.Insert => 0x30,
                                ConsoleKey.NumPad5 => 0x35,
                                ConsoleKey.Multiply => 0,
                                ConsoleKey.Subtract => 0x2D,
                                ConsoleKey.Add => 0x2B,
                                ConsoleKey.Divide => 0x2F,
                                ConsoleKey.LeftArrow => 0x34,
                                ConsoleKey.PageDown => 0x33,
                                ConsoleKey.PageUp => 0x39,
                                ConsoleKey.PrintScreen => 0,
                                ConsoleKey.RightArrow => 0x36,
                                ConsoleKey.Spacebar => 0x20,
                                ConsoleKey.Tab => 0x00,
                                ConsoleKey.UpArrow => 0x38,
                                _ => (byte)(info.KeyChar > 255 ? 0 : info.KeyChar)
                            };
                        }
                        else
                        {
                            ascii = info.Key switch
                            {
                                ConsoleKey.Backspace => 0x08,
                                ConsoleKey.Delete => 0,
                                ConsoleKey.DownArrow => 0,
                                ConsoleKey.End => 0,
                                ConsoleKey.Enter => 0x0D,
                                ConsoleKey.Escape => 0x1B,
                                ConsoleKey.Home => 0,
                                ConsoleKey.Insert => 0,
                                ConsoleKey.NumPad5 => 0,
                                ConsoleKey.Multiply => 0x2A,
                                ConsoleKey.Subtract => 0x2D,
                                ConsoleKey.Add => 0x2B,
                                ConsoleKey.Divide => 0x2F,
                                ConsoleKey.LeftArrow => 0,
                                ConsoleKey.PageDown => 0,
                                ConsoleKey.PageUp => 0,
                                ConsoleKey.PrintScreen => 0,
                                ConsoleKey.RightArrow => 0,
                                ConsoleKey.Spacebar => 0x20,
                                ConsoleKey.Tab => 0x09,
                                ConsoleKey.UpArrow => 0,
                                _ => (byte)(info.KeyChar > 255 ? 0 : info.KeyChar)
                            };
                        }
                        break;
                }
            }
        }
        if (ascii == 0 && info.KeyChar != '\0' && !char.IsControl(info.KeyChar) && info.KeyChar <= 127)
        {
            ascii = (byte)info.KeyChar;
        }
    }
}