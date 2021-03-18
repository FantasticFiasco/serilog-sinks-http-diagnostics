using System.IO;
using System.Threading;

namespace App
{
    public class SerilogErrors
    {
        private const string FileName = "app.error.txt";

        private long _count;

        public long Count => Interlocked.Read(ref _count);

        public void Clear()
        {
            Interlocked.Exchange(ref _count, 0);

            File.Delete(FileName);
        }

        public void Add(string message)
        {
            Interlocked.Increment(ref _count);

            Print(message);
            AppendToFile(message);
        }

        private static void Print(string message)
        {
            const int maxLength = 200;

            if (message.Length > maxLength)
            {
                message = $"{message.Substring(0, maxLength)}...";
            }

            Log.Error($"[DIAGNOSTICS] {message}");
        }

        private static void AppendToFile(string message)
        {
            var lines = new[]
            {
                message,
                ""
            };

            File.AppendAllLines(FileName, lines);
        }
    }
}
