using System;
using System.IO;
using System.Threading;

namespace App
{
    public static class Errors
    {
        private const string FileName = "errors.txt";

        private static long count;

        public static long Count
        {
            get { return Interlocked.Read(ref count); }
        }

        public static void Clear()
        {
            Interlocked.Exchange(ref count, 0);

            File.Delete(FileName);
        }

        public static void Add(string message)
        {
            Interlocked.Increment(ref count);

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
            };

            File.AppendAllLines(FileName, lines);
        }
    }
}
