using Maze.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maze.Repositories
{
    public interface ITVShowRepository
    {
        int GetTotalCount();

        Task<ICollection<TVShow>> GetAllAsync(int page, int pageSize);

        Task<ICollection<TVShow>> GetFilteredAsync(ICollection<int> ids);

        Task AddRangeAsync(IEnumerable<TVShow> tvShows);
    }
}
