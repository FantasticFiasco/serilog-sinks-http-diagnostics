using Shouldly;
using Xunit;

namespace LogServer.Report
{
    public class StatisticsShould
    {
        private readonly Statistics statistics;

        public StatisticsShould()
        {
            statistics = new Statistics();
        }

        [Fact]
        public void HaveNoStartGivenNoReportedBatch()
        {
            // Assert
            statistics.Start.ShouldBeNull();
        }

        [Fact]
        public void HaveStartGivenReportedBatch()
        {
            // Arrange
            statistics.ReportReceivedBatch(ByteSize.KB, new[] { ByteSize.KB });

            // Assert
            statistics.Start.ShouldNotBeNull();
        }
    }
}
