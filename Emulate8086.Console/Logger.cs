namespace Emulate8086.Console;
using System;
using System.Runtime.CompilerServices;

public enum LogLevel
{
    None,
    Error,
    Warn,
    Info,
    Debug,
    Trace,
}

class Logger
{
    public LogLevel LogLevel { get; set; } = LogLevel.Info;

    Action<Func<string>> loggerWithColor(LogLevel minLevel, string name, ConsoleColor color, Action<Func<string>> logger)
    {
        return expression =>
        {
            if (LogLevel < minLevel) return;
            var prev = Console.ForegroundColor;
            var prevbg = Console.BackgroundColor;

            Console.ForegroundColor = color;
            logger(() => $"[EMU] [{name}] " + expression());

            Console.ForegroundColor = prev;
            Console.BackgroundColor = prevbg;
        };
    }
    public Logger()
    {
        var errorLog = (Func<string> expression) => Console.Error.WriteLine(expression());
        var consoleLog = (Func<string> expression) => Console.WriteLine(expression());
        var noLog = (Func<string> expression) => { };

        error = loggerWithColor(LogLevel.Error, "ERROR", ConsoleColor.Red, errorLog);
        warn = loggerWithColor(LogLevel.Warn, "WARN", ConsoleColor.Yellow, consoleLog);
        info = loggerWithColor(LogLevel.Info, "INFO", ConsoleColor.DarkGray, consoleLog);
        debug = loggerWithColor(LogLevel.Debug, "DEBUG", ConsoleColor.Blue, consoleLog);
        trace = loggerWithColor(LogLevel.Trace, "TRACE", ConsoleColor.Cyan, consoleLog);
    }
    Action<Func<string>> error;
    Action<Func<string>> warn;
    Action<Func<string>> info;
    Action<Func<string>> debug;
    Action<Func<string>> trace;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(Func<string> expression) => error(expression);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(string message) => error(() => message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Warn(Func<string> expression) => warn(expression);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Warn(string message) => warn(() => message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Info(Func<string> expression) => info(expression);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Info(string message) => info(() => message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Debug(Func<string> expression) => debug(expression);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Debug(string message) => debug(() => message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Trace(Func<string> expression) => trace(expression);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Trace(string message) => trace(() => message);
}