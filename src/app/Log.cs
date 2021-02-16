using System;
using Spectre.Console;

namespace App
{
    public static class Log
    {
        public static void Info(string message)
        {
            Console.WriteLine(message);
        }

        public static void Error(string message)
        {
            AnsiConsole.MarkupLine("[bold red]{0}[/]", message.EscapeMarkup());
        }
    }
}
