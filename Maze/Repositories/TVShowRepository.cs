using Maze.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Maze.Repositories
{
    internal class TVShowRepository : ITVShowRepository
    {
        private readonly TVShowDbContext _db;

        public TVShowRepository(TVShowDbContext db)
        {
            _db = db;
        }

        public async Task AddRangeAsync(IEnumerable<TVShow> tvShows)
        {
            await _db.AddRangeAsync(tvShows);
        }

        public async Task<ICollection<TVShow>> GetAllAsync(int page, int pageSize)
        {
            return await _db.TVShows
                .Skip(page * pageSize)
                .Take(pageSize)
                .Include(x => x.TVShowActors)
                .ThenInclude(x => x.Actor)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ICollection<TVShow>> GetFilteredAsync(ICollection<int> excludeIDs)
        {
            return await _db.TVShows
                .Where(x => excludeIDs.Contains(x.Id))
                .ToListAsync();
        }

        public int GetTotalCount()
        {
            return _db.TVShows.Count();
        }
    }
}
