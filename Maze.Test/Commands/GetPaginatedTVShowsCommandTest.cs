using Maze.Commands;
using Maze.Models;
using Maze.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Maze.Test.Commands
{
    public class GetPaginatedTVShowsCommandTest
    {
        [Fact]
        public async Task Should_order_actors_by_date_of_birth()
        {
            // Arrange
            var datetime = DateTime.Now;
            var expected = new List<TVShow>
            {
                new TVShow{ Id = 0, Name = "A", TVShowActors = new List<TVShowActor> 
                { 
                    new TVShowActor  
                        { Actor = new Actor { Id = 0, Name = "Actor A", DoB = datetime.AddDays(-2) } },
                    new TVShowActor 
                        { Actor = new Actor { Id = 1, Name = "Actor B", DoB = datetime.AddDays(-1) } },
                    new TVShowActor 
                        { Actor = new Actor { Id = 2, Name = "Actor C", DoB = datetime.AddDays(-2) } } }
                }
            };            

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock
                .Setup(m => m.TVShows.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult<ICollection<TVShow>>(expected));

            var sut = new GetPaginatedTVShowsCommand(unitOfWorkMock.Object, 0);

            // Act
            var (data, _, _) = (await sut.Execute()).Value;

            // Assert
            Assert.True(data.Single().ActorDTOs.SequenceEqual(data.Single().ActorDTOs.OrderBy(x => x.DoB)));            
        }

        [Theory]
        [InlineData(0, 20, 100)]
        [InlineData(2, 60, 100)]
        [InlineData(20, 420, 100)]
        public async Task Should_return_last_and_total_number(int page, int expectedLast, int expectedTotal)
        {
            // Arrange
            var datetime = DateTime.Now;
            var expected = Enumerable.Repeat(
                new TVShow{ Id = 0, Name = "A", TVShowActors = new List<TVShowActor>
                {
                    new TVShowActor
                        { Actor = new Actor { Id = 0, Name = "Actor A", DoB = datetime.AddDays(-2) } },
                    new TVShowActor
                        { Actor = new Actor { Id = 1, Name = "Actor B", DoB = datetime.AddDays(-1) } },
                    new TVShowActor
                        { Actor = new Actor { Id = 2, Name = "Actor C", DoB = datetime.AddDays(-2) } } }
                }, expectedTotal).ToList();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock
                .Setup(m => m.TVShows.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult<ICollection<TVShow>>(expected));

            unitOfWorkMock
                .Setup(m => m.TVShows.GetTotalCount())
                .Returns(expected.Count);

            var sut = new GetPaginatedTVShowsCommand(unitOfWorkMock.Object, page);

            // Act
            var (_, last, total) = (await sut.Execute()).Value;

            // Assert
            Assert.Equal(expectedLast, last);
            Assert.Equal(expectedTotal, total);
        }
    }
}
