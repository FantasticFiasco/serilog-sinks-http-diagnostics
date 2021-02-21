using System.Text;

namespace LogServer.Report
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
        public const int KB = 1024 * B;

        /// <summary>
        /// Value representing 1 megabyte (MB), or mebibyte (MiB) as the unit sometimes is called.
        /// </summary>
        public const int MB = 1024 * KB;

        /// <summary>
        /// Value representing 1 gigabyte (GB), or gibibyte (GiB) as the unit sometimes is called.
        /// </summary>
        public const int GB = 1024 * MB;

        /// <summary>
        /// Returns the number of bytes produced by UTF8 encoding the characters in the specified
        /// string.
        /// </summary>
        /// <param name="text">The string containing the set of characters to encode.</param>
        /// <returns>The number of bytes produced by encoding the specified characters.</returns>
        public static int From(string text) => Encoding.UTF8.GetByteCount(text);
    }
}
