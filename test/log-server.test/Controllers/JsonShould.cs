using Shouldly;
using Xunit;

namespace LogServer.Controllers
{
    public class JsonShould
    {
        [Fact]
        public void HandleEmptyArrays()
        {
            // Arrange
            var json = "[]";

            // Act
            var got = Json.ParseArray(json);

            // Assert
            got.ShouldBeEmpty();
        }

        [Fact]
        public void HandleComplexValues()
        {
            // Arrange
            var json = "[{\"a\":1},{\"b\":2},{\"c\":3}]";

            // Act
            var got = Json.ParseArray(json);

            // Assert
            got.ShouldBe(new[] { "{\"a\":1}", "{\"b\":2}", "{\"c\":3}" });
        }

        [Fact]
        public void HandleCarriageReturn()
        {
            // Arrange
            var json = "[{\"a\":1},\r{\"b\":2},\r{\"c\":3}]";

            // Act
            var got = Json.ParseArray(json);

            // Assert
            got.ShouldBe(new[] { "{\"a\":1}", "{\"b\":2}", "{\"c\":3}" });
        }

        [Fact]
        public void HandleLinFeed()
        {
            // Arrange
            var json = "[{\"a\":1},\n{\"b\":2},\n{\"c\":3}]";

            // Act
            var got = Json.ParseArray(json);

            // Assert
            got.ShouldBe(new[] { "{\"a\":1}", "{\"b\":2}", "{\"c\":3}" });
        }

        [Fact]
        public void HandleCarriageReturnLinFeed()
        {
            // Arrange
            var json = "[{\"a\":1},\r\n{\"b\":2},\r\n{\"c\":3}]";

            // Act
            var got = Json.ParseArray(json);

            // Assert
            got.ShouldBe(new[] { "{\"a\":1}", "{\"b\":2}", "{\"c\":3}" });
        }
    }
}
