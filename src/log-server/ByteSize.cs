using System;
using System.Text;

namespace LogServer
{
    /// <summary>
    /// Class defining various multipliers of the unit byte.
    /// </summary>
    public static class ByteSize
    {
        /// <summary>
        /// Value representing one byte (B).
        /// </summary>
        public const int B = 1;

        /// <summary>
        /// Value representing 1 kilobyte (KB), or kibibyte (KiB) as the unit sometimes is called.
        /// </summary>
        public const int Kb = 1024 * B;

        /// <summary>
        /// Value representing 1 megabyte (MB), or mebibyte (MiB) as the unit sometimes is called.
        /// </summary>
        public const int Mb = 1024 * Kb;

        /// <summary>
        /// Value representing 1 gigabyte (GB), or gibibyte (GiB) as the unit sometimes is called.
        /// </summary>
        public const int Gb = 1024 * Mb;

        /// <summary>
        /// Returns the number of bytes produced by UTF8 encoding the characters in the specified
        /// string.
        /// </summary>
        /// <param name="text">The string containing the set of characters to encode.</param>
        /// <returns>The number of bytes produced by encoding the specified characters.</returns>
        public static int From(string text) => Encoding.UTF8.GetByteCount(text);

        public static string FriendlyValue(double byteCount)
        {
            var units = new[] { "B", "KB", "MB", "GB" };

            double value = byteCount;
            var unitIndex = 0;

            while (value > 1024)
            {
                value /= 1024;
                unitIndex++;
            }

            return $"{Math.Round(value, 2)} {units[unitIndex]}";
        }
    }
}
