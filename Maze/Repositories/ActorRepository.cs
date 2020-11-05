using Maze.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maze.Repositories
{
    internal class ActorRepository : IActorRepository
    {
        private readonly TVShowDbContext _db;

        public ActorRepository(TVShowDbContext db)
        {
            _db = db;
        }

        public async Task AddRangeAsync(IEnumerable<Actor> actors)
        {
            await _db.AddRangeAsync(actors);
        }

        public async Task<ICollection<Actor>> GetFilteredAsync(ICollection<int> excludeIDs)
        {
            return await _db.Actors
                .Where(x => excludeIDs.Contains(x.Id))
                .ToListAsync();
        }
    }
}
