using System.Collections.Generic;

namespace LogServer.Controllers
{
    public static class Json
    {
        public static string[] ParseArray(string json)
        {
            json = json
                .TrimStart('[')
                .TrimEnd(']');

            var logEvents = new List<string>();

            var startIndex = 0;
            var squigglyBracketCounter = 0;

            for (var i = 0; i < json.Length; i++)
            {
                if (json[i] == '{')
                {
                    squigglyBracketCounter++;
                }
                else if (json[i] == '}')
                {
                    squigglyBracketCounter--;
                }

                if (squigglyBracketCounter == 0 && json[i] == '}')
                {
                    var logEventLength = i - startIndex + 1;

                    var logEvent = json
                        .Substring(startIndex, logEventLength)
                        .Trim(',', '\r', '\n');

                    logEvents.Add(logEvent);

                    startIndex += logEventLength;
                }
            }

            return logEvents.ToArray();
        }
    }
}
