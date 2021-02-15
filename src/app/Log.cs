using Spectre.Console;

namespace App
{
    public static class Log
    {
        public static void Info(string message)
        {
            AnsiConsole.MarkupLine($"[blue]{Sanitize(message)}[/]");
        }

        public static void Error(string message)
        {
            AnsiConsole.MarkupLine($"[red bold]{Sanitize(message)}[/]");
        }

        private static string Sanitize(string message)
        {
            return message
                .Replace("[", "[[")
                .Replace("]", "]]");
        }
    }
}
