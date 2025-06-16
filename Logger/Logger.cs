using System;

namespace LudicoGTK.Logger;

public abstract class Log // i dunno what abstract means i just know squiggly line gone
{
    public static void Error(string text)
    {
        Console.WriteLine($"[Error] {text}");
    }
    
    public static void Info(string text)
    {
        Console.WriteLine($"[Info] {text}");
    }
    
    public static void Warn(string text)
    {
        Console.WriteLine($"[Warning] {text}");
    }
}