using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class Cw
{
    public const ConsoleColor DEFAULT_COLOR = ConsoleColor.White;
    public const ConsoleColor ERROR_COLOR = ConsoleColor.Red;
    public const ConsoleColor WARNING_COLOR = ConsoleColor.Yellow;

    public static void Log(string text)
    {
        Log(text, DEFAULT_COLOR);
    }
    public static void Log(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = DEFAULT_COLOR;
    }
}
