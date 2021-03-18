using System;
using System.Text;

namespace App
{
    public static class RandomMessage
    {
        private const string Characters =
            " " +
            "0123456789" +
            "abcdefghijklmnopqrstuvwxyz" +
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string Generate(Random random, int length)
        {
            var builder = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                var index = random.Next(0, Characters.Length);
                builder.Append(Characters[index]);
            }

            return builder.ToString();
        }
    }
}
