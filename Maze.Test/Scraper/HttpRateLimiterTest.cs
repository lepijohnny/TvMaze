using Maze.Scaraper;
using System.Threading.Tasks;
using Xunit;

namespace Maze.Test.Scraper
{
    public class HttpRateLimiterTest
    {
        [Fact]
        public async Task Should_rate_limit_when_brake()
        {
            // Arrange
            var sut = new HttpRateLimiter();

            await sut.BrakeAsync(); //add 1 second
            await sut.BrakeAsync(); //add 1 second
            await sut.BrakeAsync(); //add 1 second

            // Act
            var timeout = sut.TimeoutMillis;

            // Assert
            Assert.Equal(3000, timeout);
        }

        [Fact]
        public async Task Should_not_rate_limit_when_unbrake()
        {
            // Arrange
            var sut = new HttpRateLimiter();

            await sut.BrakeAsync(); //add 1 second
            await sut.BrakeAsync(); //add 1 second
            await sut.BrakeAsync(); //add 1 second
            await sut.UnbrakeAsync(); //add 1 second

            // Act
            var timeout = sut.TimeoutMillis;

            // Assert
            Assert.Equal(0, timeout);
        }

        [Fact]
        public async Task Should_not_exceed_max_timeout_10_seconds()
        {
            // Arrange
            var sut = new HttpRateLimiter();

            for (int i = 0; i < 100; i++)
            {
                await sut.BrakeAsync(); //add 1 second
            }

            // Act
            var timeout = sut.TimeoutMillis;

            // Assert
            Assert.Equal(10000, timeout);
        }
    }
}
