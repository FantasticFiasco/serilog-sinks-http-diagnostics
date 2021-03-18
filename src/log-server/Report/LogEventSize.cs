namespace LogServer.Report
{
    public enum LogEventSize
    {
        Below512B,
        Between512BAnd1KB,
        Between1And5KB,
        Between5And10KB,
        Between10And50KB,
        Between50And100KB,
        Between100And512KB,
        Between512KBAnd1MB,
        Between1And5MB,
        EqualToAndAbove5MB
    }
}
