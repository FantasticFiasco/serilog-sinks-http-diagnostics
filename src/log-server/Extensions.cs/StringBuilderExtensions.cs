// ReSharper disable once CheckNamespace
namespace System.Text
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendNewLine(this StringBuilder self)
        {
            self.AppendLine("");

            return self;
        }

        public static StringBuilder AppendTabbedFormatted(this StringBuilder self, string format, params object?[] args)
        {
            return self.AppendFormatted("    " + format, args);
        }

        public static StringBuilder AppendFormatted(this StringBuilder self, string format, params object?[] args)
        {
            var line = format.Format(args);
            self.AppendLine(line);

            return self;
        }
    }
}
