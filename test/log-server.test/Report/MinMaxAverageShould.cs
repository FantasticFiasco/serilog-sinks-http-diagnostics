using Shouldly;
using Xunit;

namespace LogServer.Report
{
    public class MinMaxAverageShould
    {
        private readonly MinMaxAverage _minMaxAverage;

        public MinMaxAverageShould()
        {
            _minMaxAverage = new MinMaxAverage();
        }

        [Theory]
        [InlineData(1, 2, 3, 1)]
        [InlineData(1, 3, 2, 1)]
        [InlineData(2, 1, 3, 1)]
        [InlineData(2, 3, 1, 1)]
        [InlineData(3, 1, 2, 1)]
        [InlineData(3, 2, 1, 1)]
        [InlineData(1, 1, 1, 1)]
        [InlineData(2, 2, 2, 2)]
        [InlineData(3, 3, 3, 3)]
        public void ReturnMin(int newValue1, int newValue2, int newValue3, int want)
        {
            // Arrange
            _minMaxAverage.Update(newValue1);
            _minMaxAverage.Update(newValue2);
            _minMaxAverage.Update(newValue3);

            // Act
            var got = _minMaxAverage.Min;

            // Assert
            got.ShouldBe(want);
        }

        [Theory]
        [InlineData(1, 2, 3, 3)]
        [InlineData(1, 3, 2, 3)]
        [InlineData(2, 1, 3, 3)]
        [InlineData(2, 3, 1, 3)]
        [InlineData(3, 1, 2, 3)]
        [InlineData(3, 2, 1, 3)]
        [InlineData(1, 1, 1, 1)]
        [InlineData(2, 2, 2, 2)]
        [InlineData(3, 3, 3, 3)]
        public void ReturnMax(int newValue1, int newValue2, int newValue3, int want)
        {
            // Arrange
            _minMaxAverage.Update(newValue1);
            _minMaxAverage.Update(newValue2);
            _minMaxAverage.Update(newValue3);

            // Act
            var got = _minMaxAverage.Max;

            // Assert
            got.ShouldBe(want);
        }

        [Theory]
        [InlineData(1, 2, 3, 2)]
        [InlineData(1, 3, 2, 2)]
        [InlineData(2, 1, 3, 2)]
        [InlineData(2, 3, 1, 2)]
        [InlineData(3, 1, 2, 2)]
        [InlineData(3, 2, 1, 2)]
        [InlineData(1, 1, 1, 1)]
        [InlineData(2, 2, 2, 2)]
        [InlineData(3, 3, 3, 3)]
        public void ReturnAverage(int newValue1, int newValue2, int newValue3, int want)
        {
            // Arrange
            _minMaxAverage.Update(newValue1);
            _minMaxAverage.Update(newValue2);
            _minMaxAverage.Update(newValue3);

            // Act
            var got = _minMaxAverage.Average;

            // Assert
            got.ShouldBe(want);
        }
    }
}
