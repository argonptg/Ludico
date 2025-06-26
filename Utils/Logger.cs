using System;

namespace LudicoGTK.Utils;

public abstract class Log // i dunno what abstract means i just know squiggly line gone
{
    public static void Error(string text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[Error] ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(text);
    }

    public static void Info(string text)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("[Info] ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(text);
    }
    
    public static void Warn(string text)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write("[Warning] ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(text);
    }
}