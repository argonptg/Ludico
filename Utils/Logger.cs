using System;

namespace LudicoGTK.Utils;

public abstract class Log // i dunno what abstract means i just know squiggly line gone
{
    public static void Write(string level, string message, ConsoleColor color)
    {
        var currentTime = DateTime.Now.ToString("HH:mm:ss");
        Console.ForegroundColor = color;
        Console.Write($"[{level} | {currentTime}] ");
        Console.ResetColor();
        Console.WriteLine(message);
    }

    public static void Info(string msg) => Write("Info", msg, ConsoleColor.Blue);
    public static void Warn(string msg) => Write("Warning", msg, ConsoleColor.DarkYellow);
    public static void Error(string msg) => Write("Error", msg, ConsoleColor.Red);

}