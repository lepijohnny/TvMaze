using Maze.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maze.Repositories
{
    public interface IActorRepository
    {
        Task AddRangeAsync(IEnumerable<Actor> actors);

        Task<ICollection<Actor>> GetFilteredAsync(ICollection<int> ids);
    }
}
