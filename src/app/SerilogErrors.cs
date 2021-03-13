using System.IO;
using System.Threading;

namespace App
{
    public class SerilogErrors
    {
        private const string FileName = "app.error.txt";

        private long count;

        public long Count => Interlocked.Read(ref count);

        public void Clear()
        {
            Interlocked.Exchange(ref count, 0);

            File.Delete(FileName);
        }

        public void Add(string message)
        {
            Interlocked.Increment(ref count);

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
