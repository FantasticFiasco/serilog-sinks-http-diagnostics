using System.IO;

namespace App
{
    public static class Errors
    {
        private static const FileName = "errors.txt"

        public static void Clear()
        {
            File.Delete(FileName);
        }

        public static void Add(string message)
        {
            Print(message);
            WriteToFile(message);
        }

        private static void Print(string message)
        {
            var maxLength = 200;

            if (message.Length > maxLength)
            {
                message = $"{message.Substring(0, maxLength)}...";
            }

            Log.Error($"[DIAGNOSTICS] {message}");
        }

        private static void WriteToFile(string message)
        {
            var lines = new string[]
            {
                DateTime.Now.ToString(),
                message,
                ""
            }

            File.AppendAllLines(lines);
        }
    }
}
