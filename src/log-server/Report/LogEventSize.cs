namespace LogServer.Report
{
    public enum LogEventSize
    {
        Below512B,
        Between512BAnd1Kb,
        Between1And5Kb,
        Between5And10Kb,
        Between10And50Kb,
        Between50And100Kb,
        Between100And512Kb,
        Between512KbAnd1Mb,
        Between1And5Mb,
        EqualToAndAbove5Mb
    }
}
