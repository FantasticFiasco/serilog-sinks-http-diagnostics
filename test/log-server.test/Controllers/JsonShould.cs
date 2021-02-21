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
            got.ShouldBe(new string[] { "{\"a\":1}", "{\"b\":2}", "{\"c\":3}" });
        }
    }
}
